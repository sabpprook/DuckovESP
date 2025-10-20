using System;
using System.Collections.Generic;
using UnityEngine;
using ItemStatsSystem;
using ItemStatsSystem.Items;

namespace DuckovESP
{
    /// <summary>
    /// 优化的敌人检测系统
    /// 使用缓存和分帧更新策略降低性能开销
    /// </summary>
    public class EnemyDetector
    {
        private readonly ESPConfig _config;
        private readonly List<EnemyInfo> _enemyInfoList = new List<EnemyInfo>(50);
        
        // 缓存数据
        private CharacterMainControl[] _allCharactersCache = new CharacterMainControl[0];
        private readonly Dictionary<CharacterMainControl, AICharacterController> _aiControllerCache = 
            new Dictionary<CharacterMainControl, AICharacterController>(50);
        private readonly Dictionary<CharacterMainControl, long> _enemyValueCache = 
            new Dictionary<CharacterMainControl, long>(50);
        private readonly HashSet<CharacterMainControl> _alertedHighValue = new HashSet<CharacterMainControl>();
        private readonly HashSet<CharacterMainControl> _alertedTraders = new HashSet<CharacterMainControl>();
        
        // 更新计时器
        private float _lastCharacterCacheUpdate = 0f;
        private float _lastAIControllerUpdate = 0f;
        private float _lastEnemyDataUpdate = 0f;
        
        // 更新间隔（优化性能）
        private const float CHARACTER_CACHE_INTERVAL = 4f;  // 优化：从3秒改为4秒更新一次角色列表
        private const float AI_CONTROLLER_INTERVAL = 6f;    // 优化：从5秒改为6秒更新一次AI控制器
        private const float ENEMY_DATA_INTERVAL = 0.4f;     // 优化：从0.3秒改为0.4秒更新一次敌人数据
        
        public EnemyDetector(ESPConfig config)
        {
            _config = config;
        }
        
        /// <summary>
        /// 获取敌人信息列表
        /// </summary>
        public List<EnemyInfo> GetEnemyInfoList() => _enemyInfoList;
        
        /// <summary>
        /// 更新敌人检测数据（主更新函数）
        /// </summary>
        public void Update(CharacterMainControl player)
        {
            if (player == null) return;
            
            float currentTime = Time.time;
            
            // 定时更新角色缓存
            if (currentTime - _lastCharacterCacheUpdate > CHARACTER_CACHE_INTERVAL)
            {
                UpdateCharacterCache();
                _lastCharacterCacheUpdate = currentTime;
            }
            
            // 定时更新AI控制器缓存
            if (currentTime - _lastAIControllerUpdate > AI_CONTROLLER_INTERVAL)
            {
                UpdateAIControllerCache();
                _lastAIControllerUpdate = currentTime;
            }
            
            // 定时更新敌人数据
            if (currentTime - _lastEnemyDataUpdate > ENEMY_DATA_INTERVAL)
            {
                UpdateEnemyData(player);
                _lastEnemyDataUpdate = currentTime;
            }
        }
        
        /// <summary>
        /// 更新所有角色缓存（每3秒）
        /// </summary>
        private void UpdateCharacterCache()
        {
            try
            {
                _allCharactersCache = UnityEngine.Object.FindObjectsOfType<CharacterMainControl>();
            }
            catch (Exception ex)
            {
                Debug.LogError($"[EnemyDetector] 更新角色缓存失败: {ex.Message}");
                _allCharactersCache = new CharacterMainControl[0];
            }
        }
        
        /// <summary>
        /// 更新AI控制器缓存（每5秒）
        /// </summary>
        private void UpdateAIControllerCache()
        {
            try
            {
                _aiControllerCache.Clear();
                var aiControllers = UnityEngine.Object.FindObjectsOfType<AICharacterController>();
                
                foreach (var aiController in aiControllers)
                {
                    if (aiController?.CharacterMainControl != null)
                    {
                        _aiControllerCache[aiController.CharacterMainControl] = aiController;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[EnemyDetector] 更新AI控制器缓存失败: {ex.Message}");
            }
        }
        
        /// <summary>
        /// 更新敌人数据（每0.3秒）
        /// </summary>
        private void UpdateEnemyData(CharacterMainControl player)
        {
            _enemyInfoList.Clear();
            
            if (player?.mainDamageReceiver == null) return;
            
            Vector3 playerPos = GetPlayerPosition(player);
            
            foreach (var character in _allCharactersCache)
            {
                try
                {
                    if (character == null || character == player) continue;
                    if (IsDead(character)) continue;
                    if (!IsEnemy(player, character)) continue;
                    
                    // 计算距离
                    Vector3 headPos = GetHeadPosition(character);
                    float distance = Vector3.Distance(playerPos, headPos);
                    
                    if (distance > _config.MaxEnemyESPDistance) continue;
                    
                    // 获取或计算库存价值
                    if (!_enemyValueCache.ContainsKey(character))
                    {
                        _enemyValueCache[character] = CalculateInventoryValue(character);
                        ProcessNewEnemy(character, player);
                    }
                    
                    long inventoryValue = _enemyValueCache[character];
                    
                    // 获取敌人信息
                    var enemyInfo = new EnemyInfo
                    {
                        Character = character,
                        Name = GetEnemyName(character),
                        Distance = distance,
                        HeadPosition = headPos,
                        BodyPosition = GetBodyPosition(character),
                        CurrentHealth = character.Health?.CurrentHealth ?? 0f,
                        MaxHealth = character.Health?.MaxHealth ?? 0f,
                        HealthPercent = GetHealthPercent(character),
                        WeaponName = GetWeaponName(character),
                        InventoryValue = inventoryValue,
                        IsAimingAtPlayer = IsAimingAtPlayer(character, player),
                        IsAlive = !IsDead(character),
                        DisplayColor = GetDisplayColor(character, inventoryValue)
                    };
                    
                    _enemyInfoList.Add(enemyInfo);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[EnemyDetector] 处理敌人数据失败: {ex.Message}");
                }
            }
            
            // 按优先级排序：价值 > 距离
            _enemyInfoList.Sort((a, b) =>
            {
                int valueCompare = b.InventoryValue.CompareTo(a.InventoryValue);
                return valueCompare != 0 ? valueCompare : a.Distance.CompareTo(b.Distance);
            });
            
            // 清理死亡敌人的缓存
            CleanupDeadEnemies();
        }
        
        /// <summary>
        /// 处理新发现的敌人（高价值/商人提醒）
        /// </summary>
        private void ProcessNewEnemy(CharacterMainControl character, CharacterMainControl player)
        {
            if (player == null) return;
            
            long value = _enemyValueCache.GetValueOrDefault(character);
            
            // 高价值目标提醒
            if (_config.EnableHighValueAlert && 
                value >= _config.HighValueThreshold && 
                !_alertedHighValue.Contains(character))
            {
                player.PopText($"!!! 高价值目标: ¥{value:N0} !!!", 10f);
                _alertedHighValue.Add(character);
            }
            
            // 商人检测提醒
            if (_config.EnableTraderAlert)
            {
                string name = GetEnemyName(character);
                string weapon = GetWeaponName(character);
                
                if (name == "Enemy" && weapon == "无" && !_alertedTraders.Contains(character))
                {
                    player.PopText("发现神秘商人,准备好你的钱哦!", 10f);
                    _alertedTraders.Add(character);
                }
            }
        }
        
        /// <summary>
        /// 清理已死亡敌人的缓存数据
        /// </summary>
        private void CleanupDeadEnemies()
        {
            var toRemove = new List<CharacterMainControl>();
            
            foreach (var kvp in _enemyValueCache)
            {
                if (kvp.Key == null || IsDead(kvp.Key))
                {
                    toRemove.Add(kvp.Key);
                }
            }
            
            foreach (var character in toRemove)
            {
                _enemyValueCache.Remove(character);
                _alertedHighValue.Remove(character);
                _alertedTraders.Remove(character);
            }
        }
        
        #region 辅助函数
        
        private static bool IsDead(CharacterMainControl character)
        {
            try
            {
                return character?.Health == null || character.Health.IsDead;
            }
            catch
            {
                return true;
            }
        }
        
        private static bool IsEnemy(CharacterMainControl player, CharacterMainControl other)
        {
            try
            {
                return Team.IsEnemy(player.Team, other.Team);
            }
            catch
            {
                return true;
            }
        }
        
        private static Vector3 GetPlayerPosition(CharacterMainControl player)
        {
            return player?.transform?.position ?? Vector3.zero;
        }
        
        private static Vector3 GetHeadPosition(CharacterMainControl character)
        {
            try
            {
                return character?.mainDamageReceiver?.transform?.position ?? 
                       (character?.transform?.position + Vector3.up * 1.7f ?? Vector3.zero);
            }
            catch
            {
                return character?.transform?.position + Vector3.up * 1.7f ?? Vector3.zero;
            }
        }
        
        private static Vector3 GetBodyPosition(CharacterMainControl character)
        {
            return character?.transform?.position + Vector3.up * 1.0f ?? Vector3.zero;
        }
        
        private static float GetHealthPercent(CharacterMainControl character)
        {
            try
            {
                var health = character?.Health;
                if (health == null) return 0f;
                return health.MaxHealth > 0 ? health.CurrentHealth / health.MaxHealth : 0f;
            }
            catch
            {
                return 0f;
            }
        }
        
        private static string GetWeaponName(CharacterMainControl character)
        {
            try
            {
                var weapon = character?.CurrentHoldItemAgent as ItemAgent_Gun;
                return weapon?.Item?.DisplayName ?? "无";
            }
            catch
            {
                return "无";
            }
        }
        
        private static string GetEnemyName(CharacterMainControl character)
        {
            try
            {
                switch (character.Team)
                {
                    case Teams.scav:
                        return "Scav";
                    case Teams.usec:
                        return "USEC";
                    case Teams.bear:
                        return "BEAR";
                    case Teams.wolf:
                        return "Wolf";
                    case Teams.player:
                        return "Player";
                    default:
                        return "Enemy";
                }
            }
            catch
            {
                return "Enemy";
            }
        }
        
        private bool IsAimingAtPlayer(CharacterMainControl enemy, CharacterMainControl player)
        {
            try
            {
                if (_aiControllerCache.TryGetValue(enemy, out var aiController))
                {
                    return aiController?.aimTarget?.gameObject == player?.mainDamageReceiver?.gameObject;
                }
            }
            catch { }
            return false;
        }
        
        private Color GetDisplayColor(CharacterMainControl character, long inventoryValue)
        {
            // 高价值目标 = 金色
            if (inventoryValue >= _config.HighValueThreshold)
            {
                return new Color(1f, 0.843f, 0f); // Gold
            }
            
            // 商人检测
            string name = GetEnemyName(character);
            string weapon = GetWeaponName(character);
            if (name == "Enemy" && weapon == "无")
            {
                return Color.cyan; // 商人 = 青色
            }
            
            // 默认敌人颜色
            return new Color(1f, 0.3f, 0.3f); // 红色
        }
        
        /// <summary>
        /// 计算敌人库存总价值
        /// </summary>
        private static long CalculateInventoryValue(CharacterMainControl character)
        {
            try
            {
                var characterItem = character?.CharacterItem;
                if (characterItem == null) return 0L;
                
                long totalValue = characterItem.GetTotalRawValue();
                
                // 递归计算所有槽位物品价值
                var stack = new Stack<Item>();
                stack.Push(characterItem);
                
                while (stack.Count > 0)
                {
                    var item = stack.Pop();
                    if (item?.Slots == null) continue;
                    
                    foreach (var slot in item.Slots)
                    {
                        if (slot?.Content != null)
                        {
                            totalValue += slot.Content.GetTotalRawValue();
                            stack.Push(slot.Content);
                        }
                    }
                }
                
                return totalValue;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[EnemyDetector] 计算库存价值失败: {ex.Message}");
                return 0L;
            }
        }
        
        #endregion
    }
}
