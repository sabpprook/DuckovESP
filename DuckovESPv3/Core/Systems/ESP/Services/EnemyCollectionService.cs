using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DuckovESPv3.Core.Systems.ESP.Models;
using DuckovESPv3.Core.EventBus;

namespace DuckovESPv3.Core.Systems.ESP.Services
{
    /// <summary>
    /// 敌人数据采集服务（0 轮询架构）
    /// </summary>
    public class EnemyCollectionService
    {
        private readonly Infrastructure.Logging.ILogger _logger;
        private readonly IEventBus _eventBus;
        private readonly Detection.EnemyMonitor _monitor;

        // 敌人数据缓存
        private readonly Dictionary<CharacterMainControl, EnemyData> _enemies = new();

        // 对外发布的事件
        public event Action<EnemyData>? OnEnemyAdded;
        public event Action<CharacterMainControl>? OnEnemyRemoved;
        public event Action<EnemyData>? OnEnemyUpdated;

        public EnemyCollectionService(
            Infrastructure.Logging.ILogger logger,
            IEventBus eventBus)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _monitor = new Detection.EnemyMonitor(logger);
        }

        /// <summary>
        /// 初始化服务（启动 0 轮询监听）
        /// </summary>
        public void Initialize()
        {
            // 订阅监听器的事件
            _monitor.OnEnemyDiscovered += HandleEnemyDiscovered;
            _monitor.OnEnemyDied += HandleEnemyDied;
            _monitor.OnEnemyHealthChanged += HandleEnemyHealthChanged;

            // 启动监听（0 轮询模式）
            _monitor.StartMonitoring();

            _logger.Info("[EnemyCollectionService] 初始化完成（0 轮询模式）");
        }

        /// <summary>
        /// 扫描场景中已存在的敌人（用于关卡加载后）
        /// 应在关卡完全加载后调用（LevelManager.OnAfterLevelInitialized）
        /// </summary>
        public void ScanExistingEnemies()
        {
            _monitor.ScanExistingEnemies();
        }

        /// <summary>
        /// 清理资源
        /// </summary>
        public void Cleanup()
        {
            _monitor.StopMonitoring();
            _monitor.OnEnemyDiscovered -= HandleEnemyDiscovered;
            _monitor.OnEnemyDied -= HandleEnemyDied;
            _monitor.OnEnemyHealthChanged -= HandleEnemyHealthChanged;

            _enemies.Clear();
            _logger.Info("[EnemyCollectionService] 清理完成");
        }

        /// <summary>
        /// 获取所有敌人数据
        /// </summary>
        public IReadOnlyList<EnemyData> GetAllEnemies()
        {
            return _enemies.Values.ToList();
        }

        /// <summary>
        /// 获取指定敌人数据
        /// </summary>
        public EnemyData? GetEnemy(CharacterMainControl character)
        {
            return _enemies.TryGetValue(character, out var data) ? data : null;
        }

        /// <summary>
        /// 处理敌人发现事件
        /// </summary>
        private void HandleEnemyDiscovered(CharacterMainControl character)
        {
            try
            {
                if (_enemies.ContainsKey(character))
                {
                    _logger.Warning($"[EnemyCollectionService] 敌人已存在，跳过: {character.name}");
                    return;
                }

                var player = CharacterMainControl.Main;
                if (player == null) return;

                // 创建敌人数据
                var enemyData = CreateEnemyData(character, player);
                _enemies[character] = enemyData;

                _logger.Info($"[EnemyCollectionService] 添加敌人: {enemyData.Name} ({enemyData.DistanceToPlayer:F1}m)");

                // 触发事件
                OnEnemyAdded?.Invoke(enemyData);
                _eventBus.Publish(new EnemyDiscoveredEvent(enemyData));
            }
            catch (Exception ex)
            {
                _logger.Error($"[EnemyCollectionService] 处理敌人发现事件失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 处理敌人死亡事件
        /// </summary>
        private void HandleEnemyDied(CharacterMainControl character)
        {
            try
            {
                bool wasTracked = _enemies.ContainsKey(character);
                
                if (!_enemies.Remove(character))
                {
                    _logger.Warning($"[EnemyCollectionService] ⚠️ 敌人死亡但未被追踪: {character.name} (可能在生成时未被检测为敌人)");
                    return;
                }

                _logger.Info($"[EnemyCollectionService] 移除敌人（死亡）: {character.name}");

                // 触发事件
                OnEnemyRemoved?.Invoke(character);
                _eventBus.Publish(new EnemyDiedEvent(character));
            }
            catch (Exception ex)
            {
                _logger.Error($"[EnemyCollectionService] 处理敌人死亡事件失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 处理敌人血量变化事件
        /// </summary>
        private void HandleEnemyHealthChanged(CharacterMainControl character, float newHealth)
        {
            try
            {
                if (!_enemies.TryGetValue(character, out var enemyData))
                {
                    _logger.Warning($"[EnemyCollectionService] 敌人不存在，跳过血量更新: {character.name}");
                    return;
                }

                // 更新血量
                enemyData.CurrentHealth = newHealth;
                enemyData.LastUpdateTime = DateTime.Now;

                // 触发事件
                OnEnemyUpdated?.Invoke(enemyData);
                _eventBus.Publish(new EnemyHealthChangedEvent(enemyData));
            }
            catch (Exception ex)
            {
                _logger.Error($"[EnemyCollectionService] 处理敌人血量变化事件失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 创建敌人数据
        /// </summary>
        private EnemyData CreateEnemyData(CharacterMainControl character, CharacterMainControl player)
        {
            var data = new EnemyData
            {
                Character = character,
                HealthComponent = character.Health,
                Name = GetEnemyName(character),
                Team = character.Team,
                Position = character.transform.position,
                HeadPosition = GetHeadPosition(character),
                CurrentHealth = character.Health.CurrentHealth,
                MaxHealth = character.Health.MaxHealth,
                IsAlive = !character.Health.IsDead,
                WeaponName = GetWeaponName(character),
                InventoryValue = CalculateInventoryValue(character),
                DiscoveryTime = DateTime.Now,
                LastUpdateTime = DateTime.Now,
                AIController = character.GetComponent<AICharacterController>()
            };

            // 计算距离
            data.DistanceToPlayer = Vector3.Distance(player.transform.position, data.Position);

            // 判断 Boss
            data.IsBoss = IsBoss(data);

            // 计算威胁等级
            data.ThreatLevel = CalculateThreatLevel(data);

            return data;
        }

        // ========== 辅助方法 ==========

        private string GetEnemyName(CharacterMainControl character)
        {
            try
            {
                // 尝试获取显示名称
                if (character.CharacterItem != null)
                {
                    return character.CharacterItem.DisplayName ?? character.name;
                }
            }
            catch
            {
                // 忽略异常
            }

            // 回退到 GameObject 名称
            return character.name;
        }

        private Vector3 GetHeadPosition(CharacterMainControl character)
        {
            try
            {
                // 尝试获取头部 Socket
                if (character.characterModel?.HelmatSocket != null)
                {
                    return character.characterModel.HelmatSocket.position;
                }
            }
            catch
            {
                // 忽略异常
            }

            return character.transform.position + Vector3.up * 1.8f;
        }

        private string GetWeaponName(CharacterMainControl character)
        {
            try
            {
                if (character.CurrentHoldItemAgent != null)
                {
                    // 尝试获取武器物品
                    var weaponTransform = character.CurrentHoldItemAgent.transform;
                    return weaponTransform?.name ?? "无";
                }
            }
            catch
            {
                // 忽略异常
            }

            return "无";
        }

        private long CalculateInventoryValue(CharacterMainControl character)
        {
            // TODO: 实现完整的库存价值计算
            // 暂时返回固定值
            return 1000;
        }

        private bool IsBoss(EnemyData data)
        {
            // Boss 判断逻辑：
            // 1. 血量 > 500
            // 2. 名称包含 "Boss"
            if (data.MaxHealth > 500f) return true;
            
            if (!string.IsNullOrEmpty(data.Name))
            {
                return data.Name.IndexOf("Boss", StringComparison.OrdinalIgnoreCase) >= 0;
            }
            
            return false;
        }

        private int CalculateThreatLevel(EnemyData data)
        {
            // 威胁等级：1-5 级
            int level = 1;

            if (data.IsBoss) level += 2;
            if (data.DistanceToPlayer < 20f) level += 1;
            if (data.InventoryValue > 50000) level += 1;

            return Mathf.Clamp(level, 1, 5);
        }
    }
}
