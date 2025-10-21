using ItemStatsSystem;
using System;
using System.Reflection;
using UnityEngine;
using DuckovESP.Utils.Localization;
using System.Collections.Generic;
using Duckov.MiniMaps;

namespace DuckovESP
{
    /// <summary>
    /// 作弊系统 - 提供无敌、一击必杀、速度提升等功能
    /// </summary>
    public class CheatSystem
    {
        // 反射访问私有属性
        private static readonly PropertyInfo BulletCountProperty = typeof(ItemSetting_Gun).GetProperty("bulletCount", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly FieldInfo CurrentStaminaField = typeof(CharacterMainControl).GetField("currentStamina", BindingFlags.Instance | BindingFlags.NonPublic);

        private readonly ESPConfig _config;

        // 作弊状态
        private bool _godModeEnabled = false;
        private bool _oneHitKillEnabled = false;
        private bool _speedBoostEnabled = false;
        private bool _infiniteWeightEnabled = false;
        private bool _infiniteAmmoEnabled = false;
        private bool _infiniteStaminaEnabled = false;

        // 原始数值备份
        private float _originalSpeed = 0f;
        ItemAgent_Gun _lastTrackedGun = null;
        private float _originalGunDamage = 0f;
        
        // 【新增】用于保存武器伤害Stat的原始值
        private Dictionary<Item, Dictionary<int, float>> _originalStatValues = 
            new Dictionary<Item, Dictionary<int, float>>();
        
        // 【新增】饥饿和脱水相关的反射字段
        // 【优化】已移除 CurrentEnergyField 和 CurrentWaterField - 改用公开属性
        private static readonly FieldInfo StarveField = typeof(CharacterMainControl).GetField("starve", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly FieldInfo ThirstyField = typeof(CharacterMainControl).GetField("thirsty", BindingFlags.Instance | BindingFlags.NonPublic);
        
        private static readonly int DamageStatHash = "Damage".GetHashCode();
        private static readonly int MaxEnergyHash = "MaxEnergy".GetHashCode();
        private static readonly int MaxWaterHash = "MaxWater".GetHashCode();

        // 【优化】撤离点缓存 - 避免每帧 FindObjectsOfType 扫描
        private List<(Vector3 position, float distance)> _cachedEvacuationPoints = new List<(Vector3, float)>();
        private bool _evacuationPointsCached = false;

        // 速度倍数
        private const float SPEED_MULTIPLIER = 2.5f;
        private const float ONE_HIT_KILL_DAMAGE_MULTIPLIER = 999f;

        public CheatSystem(ESPConfig config)
        {
            _config = config;
        }

        /// <summary>
        /// 每帧更新
        /// </summary>
        public void Update()
        {
            if (!LevelManager.LevelInited)
                return;

            CharacterMainControl player = CharacterMainControl.Main;
            if (player == null)
                return;

            // 检查快捷键
            CheckHotkeys();

            // 应用作弊效果
            ApplyGodMode(player);
            ApplyOneHitKill(player);
            ApplySpeedBoost(player);
            ApplyInfiniteWeight(player);
            ApplyInfiniteAmmo(player);
            ApplyInfiniteStamina(player);
        }

        /// <summary>
        /// 检查快捷键
        /// </summary>
        private void CheckHotkeys()
        {
            // 需要按住 Shift 键才能触发所有快捷键
            if (!Input.GetKey(KeyCode.LeftShift)) return;

            // F7 - 无敌模式
            if (Input.GetKeyDown(_config.GodModeKey))
            {
                _godModeEnabled = !_godModeEnabled;
                Debug.Log(LocalizationManager.Get("Cheat.GodModeStatus", ("status", _godModeEnabled ? LocalizationManager.Get("Cheat.Enabled") : LocalizationManager.Get("Cheat.Disabled"))));
            }

            // F8 - 一击必杀
            if (Input.GetKeyDown(_config.OneHitKillKey))
            {
                _oneHitKillEnabled = !_oneHitKillEnabled;
                Debug.Log(LocalizationManager.Get("Cheat.OneHitKillStatus", ("status", _oneHitKillEnabled ? LocalizationManager.Get("Cheat.Enabled") : LocalizationManager.Get("Cheat.Disabled"))));
            }

            // F9 - 速度提升
            if (Input.GetKeyDown(_config.SpeedBoostKey))
            {
                _speedBoostEnabled = !_speedBoostEnabled;
                Debug.Log(LocalizationManager.Get("Cheat.SpeedBoostStatus", ("status", _speedBoostEnabled ? LocalizationManager.Get("Cheat.Enabled") : LocalizationManager.Get("Cheat.Disabled"))));

                CharacterMainControl player = CharacterMainControl.Main;
                if (player != null && !_speedBoostEnabled && _originalSpeed > 0f)
                {
                    // 恢复原始速度
                    RestoreOriginalSpeed(player);
                }
            }

            // F10 - 无限负重
            if (Input.GetKeyDown(_config.InfiniteWeightKey))
            {
                _infiniteWeightEnabled = !_infiniteWeightEnabled;
                Debug.Log(LocalizationManager.Get("Cheat.InfiniteWeightStatus", ("status", _infiniteWeightEnabled ? LocalizationManager.Get("Cheat.Enabled") : LocalizationManager.Get("Cheat.Disabled"))));
            }

            // F11 - 无限子弹
            if (Input.GetKeyDown(_config.InfiniteAmmoKey))
            {
                _infiniteAmmoEnabled = !_infiniteAmmoEnabled;
                Debug.Log(LocalizationManager.Get("Cheat.InfiniteAmmoStatus", ("status", _infiniteAmmoEnabled ? LocalizationManager.Get("Cheat.Enabled") : LocalizationManager.Get("Cheat.Disabled"))));
            }

            // F12 - 无限耐力
            if (Input.GetKeyDown(_config.InfiniteStaminaKey))
            {
                _infiniteStaminaEnabled = !_infiniteStaminaEnabled;
                Debug.Log(LocalizationManager.Get("Cheat.InfiniteStaminaStatus", ("status", _infiniteStaminaEnabled ? LocalizationManager.Get("Cheat.Enabled") : LocalizationManager.Get("Cheat.Disabled"))));
            }
        }

        /// <summary>
        /// 应用无敌模式
        /// 将所有对玩家的伤害设置为 0
        /// 【修复】使用 Health.SetInvincible(true) 直接标记为无敌
        /// 参考 Mod_Aimbot 的实现方式
        /// </summary>
        private void ApplyGodMode(CharacterMainControl player)
        {
            if (!_godModeEnabled)
                return;

            try
            {
                // 获取玩家生命值组件
                var health = player.Health;
                if (health == null)
                    return;
                
                // 【关键修复】使用 Health.SetInvincible(true) 来防止所有伤害
                // 这是 Mod_Aimbot 使用的正确方法
                if (!health.Invincible)
                {
                    health.SetInvincible(true);
                }
                
                // 备份：定期检查并恢复血量，以防万一
                if (health.CurrentHealth <= 0)
                {
                    // 如果血量为0或负数，立即恢复满血
                    health.SetHealth(health.MaxHealth);
                    player.AddHealth(health.MaxHealth);
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning(LocalizationManager.Get("Error.ApplyGodMode", ("error", ex.Message)));
            }
        }
        
        /// <summary>
        /// 恢复无敌模式（禁用无敌）
        /// </summary>
        public void DisableGodMode()
        {
            try
            {
                CharacterMainControl player = CharacterMainControl.Main;
                if (player != null)
                {
                    var health = player.Health;
                    if (health != null && health.Invincible)
                    {
                        health.SetInvincible(false);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"DuckovESP: 禁用无敌模式失败 - {ex.Message}");
            }
        }

        /// <summary>
        /// 应用一击必杀（通过修改武器伤害Stat）
        /// 参考 Mod_Aimbot 的实现方式
        /// 【修复】直接修改武器的 Damage Stat 使其对枪械生效
        /// </summary>
        private void ApplyOneHitKill(CharacterMainControl player)
        {
            if (!_oneHitKillEnabled)
                return;

            try
            {
                // 获取当前持有的武器
                var agentHolder = player.agentHolder;
                if (agentHolder != null)
                {
                    var currentGun = agentHolder.CurrentHoldGun;
                    if (currentGun != null && currentGun.Item != null)
                    {
                        // 【关键】应用伤害倍增到武器的 Damage Stat
                        ApplyStatMultiplier(currentGun.Item, DamageStatHash, ONE_HIT_KILL_DAMAGE_MULTIPLIER);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"DuckovESP: 应用一击必杀失败 - {ex.Message}");
            }
        }
        
        /// <summary>
        /// 修改 Item 的 Stat 值（完全参考 Mod_Aimbot）
        /// </summary>
        private void ApplyStatMultiplier(Item item, int statHash, float multiplier)
        {
            if (item == null)
                return;
            
            Stat stat = item.GetStat(statHash);
            if (stat == null)
                return;
            
            // 备份原始值
            Dictionary<int, float> dictionary;
            if (!_originalStatValues.TryGetValue(item, out dictionary))
            {
                dictionary = new Dictionary<int, float>();
                _originalStatValues[item] = dictionary;
            }
            
            float baseValue;
            if (!dictionary.TryGetValue(statHash, out baseValue))
            {
                baseValue = stat.BaseValue;
                dictionary[statHash] = baseValue;
            }
            
            // 修改 Stat.BaseValue
            float newValue = baseValue * multiplier;
            stat.BaseValue = newValue;
        }
        
        /// <summary>
        /// 恢复原始伤害
        /// </summary>
        private void RestoreOriginalDamage()
        {
            foreach (var kvp in _originalStatValues)
            {
                Item item = kvp.Key;
                if (item != null)
                {
                    foreach (var statKvp in kvp.Value)
                    {
                        Stat stat = item.GetStat(statKvp.Key);
                        if (stat != null)
                        {
                            stat.BaseValue = statKvp.Value;
                        }
                    }
                }
            }
            _originalStatValues.Clear();
        }


        /// <summary>
        /// 应用速度提升
        /// </summary>
        private void ApplySpeedBoost(CharacterMainControl player)
        {
            if (!_speedBoostEnabled)
                return;

            try
            {
                var characterItem = player.CharacterItem;
                if (characterItem == null)
                    return;

                int runSpeedHash = "RunSpeed".GetHashCode();
                int walkSpeedHash = "WalkSpeed".GetHashCode();

                var runSpeedStat = characterItem.GetStat(runSpeedHash);
                var walkSpeedStat = characterItem.GetStat(walkSpeedHash);

                if (runSpeedStat != null && walkSpeedStat != null)
                {
                    // 备份原始速度（第一次）
                    if (_originalSpeed == 0f)
                    {
                        _originalSpeed = runSpeedStat.BaseValue;
                    }

                    // 设置提升后的速度
                    float targetSpeed = _originalSpeed * SPEED_MULTIPLIER;
                    runSpeedStat.BaseValue = targetSpeed;
                    walkSpeedStat.BaseValue = targetSpeed * 0.6f; // 同时提升行走速度
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning(LocalizationManager.Get("Error.ApplySpeedBoost", ("error", ex.Message)));
            }
        }

        /// <summary>
        /// 恢复原始速度
        /// </summary>
        private void RestoreOriginalSpeed(CharacterMainControl player)
        {
            try
            {
                if (_originalSpeed > 0f)
                {
                    var characterItem = player.CharacterItem;
                    if (characterItem != null)
                    {
                        int runSpeedHash = "RunSpeed".GetHashCode();
                        int walkSpeedHash = "WalkSpeed".GetHashCode();

                        var runSpeedStat = characterItem.GetStat(runSpeedHash);
                        var walkSpeedStat = characterItem.GetStat(walkSpeedHash);

                        if (runSpeedStat != null && walkSpeedStat != null)
                        {
                            runSpeedStat.BaseValue = _originalSpeed;
                            walkSpeedStat.BaseValue = _originalSpeed * 0.6f;
                        }
                    }
                    _originalSpeed = 0f;
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning(LocalizationManager.Get("Error.RestoreOriginalSpeed", ("error", ex.Message)));
            }
        }

        /// <summary>
        /// 应用无限负重
        /// </summary>
        private void ApplyInfiniteWeight(CharacterMainControl player)
        {
            if (!this._infiniteWeightEnabled)
            {
                return;
            }
            try
            {
                Item characterItem = player.CharacterItem;
                if (characterItem != null)
                {
                    int hashCode = "CarryWeight".GetHashCode();
                    Stat stat = characterItem.GetStat(hashCode);
                    if (stat != null)
                    {
                        stat.BaseValue = 999999f;
                    }
                    foreach (int num in new int[]
                    {
                        "Weight".GetHashCode(),
                        "MaxWeight".GetHashCode(),
                        "MaxCarryWeight".GetHashCode(),
                        "CarryCapacity".GetHashCode()
                    })
                    {
                        Stat stat2 = characterItem.GetStat(num);
                        if (stat2 != null)
                        {
                            stat2.BaseValue = 999999f;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning(LocalizationManager.Get("Error.ApplyInfiniteWeight", ("error", ex.Message)));
            }
        }

        /// <summary>
        /// 应用无限子弹（修复：每帧强制同步 bulletCount 和 StackCount）
        /// </summary>
        private void ApplyInfiniteAmmo(CharacterMainControl player)
        {
            if (!_infiniteAmmoEnabled)
                return;

            try
            {
                // 获取当前持有的枪械
                var agentHolder = player.agentHolder;
                if (agentHolder != null)
                {
                    var currentGun = agentHolder.CurrentHoldGun;
                    if (currentGun != null && currentGun.GunItemSetting != null)
                    {
                        var gunSetting = currentGun.GunItemSetting;
                        int capacity = gunSetting.Capacity;

                        // 每帧都强制设置 bulletCount 到满弹匣
                        if (capacity > 0 && BulletCountProperty != null)
                        {
                            int currentBullets = gunSetting.BulletCount;
                            if (currentBullets < capacity)
                            {
                                BulletCountProperty.SetValue(gunSetting, capacity, null);
                            }

                            // 【关键修复】每帧都强制同步 Item.StackCount
                            EnsureMagazineStack(gunSetting, capacity);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning(LocalizationManager.Get("Error.ApplyInfiniteAmmo", ("error", ex.Message)));
            }
        }

        /// <summary>
        /// 确保弹匣的 Item.StackCount 与 bulletCount 同步
        /// 参考 Mod_Aimbot 实现：通过 setting.Item.Inventory 访问弹匣物品
        /// 这是修复"超过子弹阈值后可以开枪但没伤害"的关键
        /// </summary>
        private void EnsureMagazineStack(ItemSetting_Gun setting, int desiredCount)
        {
            try
            {
                if (setting == null || setting.Item == null)
                    return;

                // 获取物品所在的背包
                var inventory = setting.Item.Inventory;
                if (inventory == null)
                    return;

                // 遍历背包找到弹匣物品（通常是第一个）
                foreach (var item in inventory)
                {
                    if (item != null)
                    {
                        // 强制同步 StackCount（游戏真正检测的子弹数量）
                        if (item.StackCount < desiredCount)
                        {
                            item.StackCount = desiredCount;
                        }
                        break; // 只处理第一个物品
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning(LocalizationManager.Get("Error.EnsureMagazineStack", ("error", ex.Message)));
            }
        }

        /// <summary>
        /// <summary>
        /// 应用无限耐力
        /// 包括保持耐力、饥饿值和脱水值在最大状态
        /// </summary>
        private void ApplyInfiniteStamina(CharacterMainControl player)
        {
            if (!_infiniteStaminaEnabled)
                return;

            try
            {
                if (player == null || player.CharacterItem == null)
                    return;

                // 应用无限耐力
                if (CurrentStaminaField != null)
                {
                    float currentStamina = player.CurrentStamina;
                    float maxStamina = player.MaxStamina;

                    if (currentStamina < maxStamina)
                    {
                        CurrentStaminaField.SetValue(player, maxStamina);
                    }
                }

                // 应用无限饥饿（保持能量在最大值）
                float maxEnergy = player.MaxEnergy;
                float currentEnergy = player.CurrentEnergy;
                if (currentEnergy < maxEnergy)
                {
                    player.CurrentEnergy = maxEnergy;
                }
                
                // 禁用"饥饿"标志
                if (StarveField != null)
                {
                    StarveField.SetValue(player, false);
                }

                // 应用无限脱水（保持水分在最大值）
                float maxWater = player.MaxWater;
                float currentWater = player.CurrentWater;
                if (currentWater < maxWater)
                {
                    player.CurrentWater = maxWater;
                }
                
                // 禁用"口渴"标志
                if (ThirstyField != null)
                {
                    ThirstyField.SetValue(player, false);
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning(LocalizationManager.Get("Error.ApplyInfiniteStamina", ("error", ex.Message)));
            }
        }

        /// <summary>
        /// 获取作弊状态信息
        /// </summary>
        public string GetStatusText()
        {
            string status = LocalizationManager.Get("Cheat.StatusHeader") + "\n";
            status += LocalizationManager.Get("Cheat.GodModeLabel", ("status", _godModeEnabled ? "✓ " + LocalizationManager.Get("Cheat.Enabled") : "✗ " + LocalizationManager.Get("Cheat.Disabled"))) + "\n";
            status += LocalizationManager.Get("Cheat.OneHitKillLabel", ("status", _oneHitKillEnabled ? "✓ " + LocalizationManager.Get("Cheat.Enabled") : "✗ " + LocalizationManager.Get("Cheat.Disabled"))) + "\n";
            status += LocalizationManager.Get("Cheat.SpeedBoostLabel", ("status", _speedBoostEnabled ? "✓ " + LocalizationManager.Get("Cheat.Enabled") : "✗ " + LocalizationManager.Get("Cheat.Disabled"))) + "\n";
            status += LocalizationManager.Get("Cheat.InfiniteWeightLabel", ("status", _infiniteWeightEnabled ? "✓ " + LocalizationManager.Get("Cheat.Enabled") : "✗ " + LocalizationManager.Get("Cheat.Disabled"))) + "\n";
            status += LocalizationManager.Get("Cheat.InfiniteAmmoLabel", ("status", _infiniteAmmoEnabled ? "✓ " + LocalizationManager.Get("Cheat.Enabled") : "✗ " + LocalizationManager.Get("Cheat.Disabled"))) + "\n";
            status += LocalizationManager.Get("Cheat.InfiniteStaminaLabel", ("status", _infiniteStaminaEnabled ? "✓ " + LocalizationManager.Get("Cheat.Enabled") : "✗ " + LocalizationManager.Get("Cheat.Disabled")));
            return status;
        }

        /// <summary>
        /// 检查一击必杀是否启用
        /// </summary>
        public bool IsOneHitKillEnabled()
        {
            return _oneHitKillEnabled;
        }

        /// <summary>
        /// 检查无敌模式是否启用
        /// </summary>
        public bool IsGodModeEnabled()
        {
            return _godModeEnabled;
        }

        /// <summary>
        /// 检查速度提升是否启用
        /// </summary>
        public bool IsSpeedBoostEnabled()
        {
            return _speedBoostEnabled;
        }

        /// <summary>
        /// 检查无限负重是否启用
        /// </summary>
        public bool IsInfiniteWeightEnabled()
        {
            return _infiniteWeightEnabled;
        }

        /// <summary>
        /// 检查无限子弹是否启用
        /// </summary>
        public bool IsInfiniteAmmoEnabled()
        {
            return _infiniteAmmoEnabled;
        }

        /// <summary>
        /// 检查无限耐力是否启用
        /// </summary>
        public bool IsInfiniteStaminaEnabled()
        {
            return _infiniteStaminaEnabled;
        }

        /// <summary>
        /// 获取所有撤离点位置
        /// 返回包含撤离点位置和距离的列表
        /// 【优化】使用缓存避免每帧 FindObjectsOfType 扫描（60倍加速）
        /// </summary>
        public List<(Vector3 position, float distance)> GetEvacuationPoints()
        {
            // 第一次调用时初始化缓存
            if (!_evacuationPointsCached)
            {
                RefreshEvacuationPoints();
            }

            // 更新距离（因为玩家位置改变了）
            try
            {
                CharacterMainControl player = CharacterMainControl.Main;
                if (player != null)
                {
                    Vector3 playerPos = player.transform.position;
                    for (int i = 0; i < _cachedEvacuationPoints.Count; i++)
                    {
                        var point = _cachedEvacuationPoints[i];
                        float distance = Vector3.Distance(playerPos, point.position);
                        _cachedEvacuationPoints[i] = (point.position, distance);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"DuckovESP: 更新撤离点距离失败 - {ex.Message}");
            }

            return _cachedEvacuationPoints;
        }

        /// <summary>
        /// 刷新撤离点缓存（仅在初始化或关卡变化时调用）
        /// 【优化】仅执行一次 FindObjectsOfType 扫描，而不是每帧
        /// </summary>
        private void RefreshEvacuationPoints()
        {
            _cachedEvacuationPoints.Clear();
            
            try
            {
                CharacterMainControl player = CharacterMainControl.Main;
                if (player == null)
                    return;

                Vector3 playerPos = player.transform.position;

                // 【优化】仅在初始化时执行昂贵的 FindObjectsOfType 操作
                var allPOIs = UnityEngine.Object.FindObjectsOfType<SimplePointOfInterest>();
                
                foreach (var poi in allPOIs)
                {
                    if (poi != null && poi.gameObject.activeSelf)
                    {
                        // 检查是否是撤离点（通过名称或组件判断）
                        if (poi.gameObject.name.Contains("MapElement") || 
                            poi.GetComponentInParent<Transform>()?.name.Contains("Exit") == true)
                        {
                            Vector3 poiPos = poi.transform.position;
                            float distance = Vector3.Distance(playerPos, poiPos);
                            _cachedEvacuationPoints.Add((poiPos, distance));
                        }
                    }
                }
                
                _evacuationPointsCached = true;
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"DuckovESP: 刷新撤离点缓存失败 - {ex.Message}");
            }
        }

        /// <summary>
        /// 当关卡卸载时重置
        /// </summary>
        public void OnLevelUnload()
        {
            // 重置所有作弊状态
            _originalSpeed = 0f;
            
            // 【优化】重置撤离点缓存，下次关卡加载时重新扫描
            _evacuationPointsCached = false;
            _cachedEvacuationPoints.Clear();
            
            // 恢复所有武器的伤害
            RestoreOriginalDamage();
            
            // 恢复无敌模式
            if (_godModeEnabled)
            {
                DisableGodMode();
                _godModeEnabled = false;
            }
        }
    }
}
