using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace DuckovESPv3.Core.Systems.ESP.Detection
{
    /// <summary>
    /// 敌人监听器：订阅游戏事件，实现 0 轮询的实时检测
    /// </summary>
    public class EnemyMonitor
    {
        private readonly Infrastructure.Logging.ILogger _logger;
        private bool _isMonitoring;

        // 已订阅血量变化事件的敌人
        private readonly HashSet<CharacterMainControl> _subscribedEnemies = new();

        // 对外发布的事件
        public event Action<CharacterMainControl>? OnEnemyDiscovered;
        public event Action<CharacterMainControl>? OnEnemyDied;
        public event Action<CharacterMainControl, float>? OnEnemyHealthChanged;

        public EnemyMonitor(Infrastructure.Logging.ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// 开始监听（订阅所有游戏事件）
        /// </summary>
        public void StartMonitoring()
        {
            if (_isMonitoring)
            {
                _logger.Warning("[EnemyMonitor] 已经在监听中，跳过重复启动");
                return;
            }

            // 订阅 Harmony Hook 的角色生成事件
            CharacterSpawnHook.OnCharacterSpawned += OnCharacterSpawned;

            // 订阅全局死亡事件
            Health.OnDead += OnHealthDead;

            // 订阅全局受伤事件（用于更新血量）
            Health.OnHurt += OnHealthHurt;

            _isMonitoring = true;
            _logger.Info("[EnemyMonitor] 开始监听敌人事件（0 轮询模式）");
        }
        
        /// <summary>
        /// 扫描场景中已存在的敌人（用于检测预置的NPC）
        /// 应在关卡完全加载后调用（LevelManager.OnAfterLevelInitialized）
        /// </summary>
        public void ScanExistingEnemies()
        {
            try
            {
                // 通过CharacterSpawnerRoot获取所有已生成的角色
                var spawnerRoots = UnityEngine.Resources.FindObjectsOfTypeAll<CharacterSpawnerRoot>();
                int scannedCount = 0;
                int enemyCount = 0;
                
                foreach (var spawnerRoot in spawnerRoots)
                {
                    if (spawnerRoot == null) continue;
                    
                    // 使用反射访问私有字段 createdCharacters
                    var characters = spawnerRoot.GetType().GetField("createdCharacters", 
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                        ?.GetValue(spawnerRoot) as System.Collections.Generic.List<CharacterMainControl>;
                    
                    if (characters == null) continue;
                    
                    foreach (var character in characters)
                    {
                        if (character == null) continue;
                        scannedCount++;
                        
                        // 检查是否为敌人
                        if (IsEnemy(character))
                        {
                            enemyCount++;
                            _logger.Debug($"[EnemyMonitor] 扫描发现预置敌人: {character.name}, Team: {character.Team}");
                            
                            // 订阅该敌人的血量变化事件
                            SubscribeToEnemyHealth(character);
                            
                            OnEnemyDiscovered?.Invoke(character);
                        }
                    }
                }
                
                _logger.Info($"[EnemyMonitor] 场景扫描完成: 总角色={scannedCount}, 敌人={enemyCount}");
            }
            catch (Exception ex)
            {
                _logger.Error($"[EnemyMonitor] 扫描场景敌人失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 订阅敌人的血量变化事件（用于捕获加血和减血）
        /// </summary>
        private void SubscribeToEnemyHealth(CharacterMainControl enemy)
        {
            if (enemy == null || enemy.Health == null) return;
            if (_subscribedEnemies.Contains(enemy)) return; // 避免重复订阅

            enemy.Health.OnHealthChange.AddListener((Health health) => 
            {
                try
                {
                    var character = health.TryGetCharacter();
                    if (character == null || !IsEnemy(character)) return;

                    float newHealth = health.CurrentHealth;
                    _logger.Debug($"[EnemyMonitor] 敌人血量变化: {character.name}, 血量: {newHealth}/{health.MaxHealth}");
                    OnEnemyHealthChanged?.Invoke(character, newHealth);
                }
                catch (Exception ex)
                {
                    _logger.Error($"[EnemyMonitor] 处理血量变化事件失败: {ex.Message}");
                }
            });

            _subscribedEnemies.Add(enemy);
            _logger.Debug($"[EnemyMonitor] 已订阅敌人血量变化: {enemy.name}");
        }

        /// <summary>
        /// 停止监听（取消订阅所有事件）
        /// </summary>
        public void StopMonitoring()
        {
            if (!_isMonitoring) return;

            CharacterSpawnHook.OnCharacterSpawned -= OnCharacterSpawned;
            Health.OnDead -= OnHealthDead;
            Health.OnHurt -= OnHealthHurt;

            // 清空已订阅列表（UnityEvent会在对象销毁时自动清理）
            _subscribedEnemies.Clear();

            _isMonitoring = false;
            _logger.Info("[EnemyMonitor] 停止监听敌人事件");
        }

        /// <summary>
        /// 处理角色生成事件（CharacterSpawnHook 触发）
        /// </summary>
        private void OnCharacterSpawned(CharacterMainControl character)
        {
            try
            {
                if (character == null) return;
                
                var player = CharacterMainControl.Main;
                if (player == null)
                {
                    _logger.Debug($"[EnemyMonitor] 玩家未就绪，跳过角色检测: {character.name}");
                    return;
                }

                // 判断是否为敌人
                bool isEnemy = IsEnemy(character);
                _logger.Debug($"[EnemyMonitor] 角色生成检测: {character.name}, Team: {character.Team}, PlayerTeam: {player.Team}, IsEnemy: {isEnemy}");
                
                if (!isEnemy) return;

                _logger.Info($"[EnemyMonitor] 🎯 发现新敌人: {character.name}, Team: {character.Team}");
                
                // 订阅该敌人的血量变化事件（用于捕获加血）
                SubscribeToEnemyHealth(character);
                
                OnEnemyDiscovered?.Invoke(character);
            }
            catch (Exception ex)
            {
                _logger.Error($"[EnemyMonitor] 处理角色生成事件失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 处理死亡事件（Health.OnDead 触发）
        /// </summary>
        private void OnHealthDead(Health health, DamageInfo damageInfo)
        {
            try
            {
                // 尝试多种方式获取角色引用
                CharacterMainControl? character = null;
                
                // 方法1: 使用TryGetCharacter
                character = health.TryGetCharacter();
                
                // 方法2: 如果失败，尝试通过Component获取
                if (character == null && health != null)
                {
                    character = health.GetComponent<CharacterMainControl>();
                }
                
                // 方法3: 如果还是失败，尝试通过GameObject查找
                if (character == null && health != null && health.gameObject != null)
                {
                    character = health.gameObject.GetComponentInParent<CharacterMainControl>();
                }
                
                if (character == null)
                {
                    _logger.Warning($"[EnemyMonitor] ⚠️ 死亡事件：无法获取角色引用 (Health={health?.name})");
                    return;
                }
                
                // 记录所有死亡事件（包括玩家和敌人），用于调试
                var player = CharacterMainControl.Main;
                bool isEnemy = player != null && IsEnemy(character);
                _logger.Debug($"[EnemyMonitor] 死亡事件: {character.name}, Team: {character.Team}, IsEnemy: {isEnemy}");
                
                if (!isEnemy)
                {
                    return;
                }

                _logger.Info($"[EnemyMonitor] ☠️ 敌人死亡: {character.name}");
                OnEnemyDied?.Invoke(character);
                
                // 从订阅列表中移除
                _subscribedEnemies.Remove(character);
            }
            catch (Exception ex)
            {
                _logger.Error($"[EnemyMonitor] 处理死亡事件失败: {ex.Message}\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// 处理受伤事件（Health.OnHurt 触发）
        /// </summary>
        private void OnHealthHurt(Health health, DamageInfo damageInfo)
        {
            try
            {
                var character = health.TryGetCharacter();
                if (character == null || !IsEnemy(character)) return;

                float newHealth = health.CurrentHealth;
                _logger.Debug($"[EnemyMonitor] 敌人受伤: {character.name}, 血量: {newHealth}/{health.MaxHealth}");
                OnEnemyHealthChanged?.Invoke(character, newHealth);
            }
            catch (Exception ex)
            {
                _logger.Error($"[EnemyMonitor] 处理受伤事件失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 判断是否为敌人
        /// </summary>
        private bool IsEnemy(CharacterMainControl character)
        {
            if (character == null || character.Health == null) return false;

            var player = CharacterMainControl.Main;
            if (player == null) return false;

            // 使用游戏内置的队伍判断逻辑（处理所有队伍关系）
            return Team.IsEnemy(player.Team, character.Team);
        }
    }
}
