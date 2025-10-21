using ItemStatsSystem;
using System;
using System.Reflection;
using UnityEngine;
using DuckovESP.Utils.Localization;
using System.Collections.Generic;
using Duckov.MiniMaps;
using Duckov.Quests;

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

        // 【新增】任务区域缓存
        private Dictionary<MonoBehaviour, QuestZoneMarkerData> _trackedQuestZones = 
            new Dictionary<MonoBehaviour, QuestZoneMarkerData>();
        private bool _questZonesCached = false;

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
        /// 获取所有追踪的任务区域
        /// 每2秒扫描一次以发现新的任务
        /// </summary>
        public Dictionary<MonoBehaviour, QuestZoneMarkerData> GetQuestZones()
        {
            // 仅在关卡初始化时扫描一次任务区域
            if (!_questZonesCached)
            {
                ScanQuestZones();
                _questZonesCached = true;
                Debug.Log("[DuckovESP] ✓ 任务区域初始化扫描完成");
            }

            // 更新现有任务的距离和进度
            try
            {
                CharacterMainControl player = CharacterMainControl.Main;
                if (player != null)
                {
                    Vector3 playerPos = player.transform.position;
                    foreach (var kvp in _trackedQuestZones)
                    {
                        if (kvp.Key != null && kvp.Value != null)
                        {
                            QuestZoneMarkerData data = kvp.Value;
                            data.distance = Vector3.Distance(playerPos, data.centerPosition);
                            
                            // 更新任务的活跃状态
                            if (kvp.Key.gameObject.activeInHierarchy != data.isActive)
                            {
                                data.isActive = kvp.Key.gameObject.activeInHierarchy;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"DuckovESP: 更新任务区域失败 - {ex.Message}");
            }

            return _trackedQuestZones;
        }

        /// <summary>
        /// 扫描并缓存所有任务区域
        /// 【优化】仅在关卡初始化时扫描一次，不再定期扫描
        /// 支持多种任务类型: QuestTask_ReachLocation, SubmitItems, QuestTask_TaskEvent
        /// </summary>
#pragma warning disable IL2087, IL2075
        private void ScanQuestZones()
        {
            try
            {
                CharacterMainControl player = CharacterMainControl.Main;
                if (player == null)
                    return;

                Vector3 playerPos = player.transform.position;
                var allMonoBehaviours = UnityEngine.Object.FindObjectsOfType<MonoBehaviour>();
                var questTasks = new Dictionary<MonoBehaviour, string>(); // 组件 -> 类型名
                
                // 步骤 1: 收集所有任务相关的组件
                foreach (var mono in allMonoBehaviours)
                {
                    string typeName = mono.GetType().Name;
                    // 支持 QuestTask_ReachLocation 和 QuestTask_TaskEvent
                    if (typeName == "QuestTask_ReachLocation" || typeName == "QuestTask_TaskEvent")
                    {
                        questTasks[mono] = typeName;
                    }
                }
                
                Debug.Log($"[DuckovESP] 扫描到 {questTasks.Count} 个任务组件");
                
                if (questTasks.Count == 0)
                {
                    Debug.Log("[DuckovESP] 未找到任何任务区域");
                    return;
                }
                
                int successCount = 0;
                
                foreach (var (taskComponent, taskType) in questTasks)
                {
                    if (taskComponent == null)
                        continue;
                    
                    try
                    {
                        // 步骤 2: 获取任务名称（所有任务类型通用）
                        string taskName = taskComponent.gameObject.name;
                        var masterProperty = taskComponent.GetType().BaseType.GetProperty("Master",
                            BindingFlags.Public | BindingFlags.Instance);
                        
                        if (masterProperty != null)
                        {
                            var masterObj = masterProperty.GetValue(taskComponent);
                            if (masterObj != null)
                            {
                                Type masterObjType = masterObj.GetType();
                                var displayNameProp = masterObjType.GetProperty("DisplayName");
                                if (displayNameProp != null)
                                {
                                    taskName = (string)displayNameProp.GetValue(masterObj);
                                }
                            }
                        }
                        
                        Vector3 position = Vector3.zero;
                        float radius = 5f;
                        bool positionFound = false;
                        
                        // 步骤 3: 根据任务类型提取位置信息
                        if (taskType == "QuestTask_ReachLocation")
                        {
                            positionFound = TryExtractReachLocationPosition(taskComponent, out position, out radius);
                        }
                        else if (taskType == "QuestTask_TaskEvent")
                        {
                            positionFound = TryExtractTaskEventPosition(taskComponent, out position, out radius);
                        }
                        else if (taskType == "SubmitItems")
                        {
                            positionFound = TryExtractMapElementPosition(taskComponent, out position, out radius);
                        }
                        
                        if (!positionFound)
                        {
                            // 添加详细诊断信息
                            if (taskType == "QuestTask_ReachLocation")
                            {
                                // 诊断 ReachLocation 失败原因
                                var locationField = taskComponent.GetType().GetField("location", 
                                    BindingFlags.NonPublic | BindingFlags.Instance);
                                if (locationField != null)
                                {
                                    var locationObj = locationField.GetValue(taskComponent);
                                    if (locationObj == null)
                                        Debug.Log($"[DuckovESP] 诊断: ReachLocation.location 为 null");
                                    else
                                        Debug.Log($"[DuckovESP] 诊断: ReachLocation.location 存在但无法提取坐标");
                                }
                            }
                            else if (taskType == "SubmitItems" || taskType == "QuestTask_TaskEvent")
                            {
                                // 诊断 mapElement 失败原因
                                var mapElementField = taskComponent.GetType().GetField("mapElement",
                                    BindingFlags.NonPublic | BindingFlags.Instance);
                                if (mapElementField != null)
                                {
                                    var mapElement = mapElementField.GetValue(taskComponent);
                                    if (mapElement == null)
                                        Debug.Log($"[DuckovESP] 诊断: {taskType}.mapElement 为 null");
                                    else
                                        Debug.Log($"[DuckovESP] 诊断: {taskType}.mapElement 存在但无法提取坐标");
                                }
                                else
                                    Debug.Log($"[DuckovESP] 诊断: {taskType} 无 mapElement 字段");
                            }
                            continue;
                        }
                        
                        // 步骤 4: 检查任务完成状态，只显示未完成的任务
                        bool isFinished = IsTaskFinished(taskComponent);
                        if (isFinished)
                        {
                            Debug.Log($"[DuckovESP] 任务已完成，跳过: '{taskName}'");
                            continue;
                        }
                        
                        successCount++;
                        Debug.Log($"[DuckovESP] ✓ 发现任务区域 #{successCount}: '{taskName}' ({taskType})");
                        Debug.Log($"[DuckovESP]   位置: {position}, 半径: {radius}");
                        
                        // 创建标记数据
                        QuestZoneMarkerData markerData = new QuestZoneMarkerData
                        {
                            countDownArea = null,
                            locationMarker = null,
                            triggerCollider = null,
                            displayName = taskName,
                            centerPosition = position,
                            radius = radius > 0 ? radius : 5f,
                            requiredTime = 0f,
                            isActive = taskComponent.gameObject.activeInHierarchy,
                            progress = 0f,
                            remainingTime = 0f,
                            distance = Vector3.Distance(playerPos, position)
                        };
                        
                        // 用任务组件作为字典的键
                        _trackedQuestZones[taskComponent] = markerData;
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning($"[DuckovESP] 处理任务组件失败: {ex.Message}");
                    }
                }
                
                Debug.Log($"[DuckovESP] ═══ 扫描完成 ═══ 找到 {questTasks.Count} 个任务组件，成功解析 {successCount} 个");
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"DuckovESP: 扫描任务区域失败 - {ex.Message}\n{ex.StackTrace}");
            }
#pragma warning restore IL2087, IL2075
        }
        
        /// <summary>
        /// 从 QuestTask_ReachLocation 提取位置信息
        /// </summary>
#pragma warning disable IL2087, IL2075
        private bool TryExtractReachLocationPosition(MonoBehaviour taskComponent, out Vector3 position, out float radius)
        {
            position = Vector3.zero;
            radius = 5f;
            
            // 【方案 1】直接从 target Transform 获取位置（最快）
            var targetField = taskComponent.GetType().GetField("target",
                BindingFlags.NonPublic | BindingFlags.Instance);
            if (targetField != null)
            {
                try
                {
                    var targetTransform = targetField.GetValue(taskComponent) as Transform;
                    if (targetTransform != null)
                    {
                        position = targetTransform.position;
                        
                        // 获取 radius 字段
                        var radiusField = taskComponent.GetType().GetField("radius",
                            BindingFlags.NonPublic | BindingFlags.Instance);
                        if (radiusField != null)
                        {
                            radius = (float)radiusField.GetValue(taskComponent);
                        }
                        Debug.Log($"[DuckovESP] ✓ 从 target Transform 成功提取坐标: {position}");
                        return true;
                    }
                }
                catch { }
            }
            
            // 【方案 2】通过 GetLocationTransform() 获取位置
            var locationField = taskComponent.GetType().GetField("location", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            if (locationField != null)
            {
                try
                {
                    var locationObj = locationField.GetValue(taskComponent);
                    if (locationObj != null)
                    {
                        Debug.Log($"[DuckovESP] 调试: location 对象存在，类型={locationObj.GetType().Name}");
                        
                        var getLocationTransformMethod = locationObj.GetType().GetMethod("GetLocationTransform",
                            BindingFlags.Public | BindingFlags.Instance);
                        if (getLocationTransformMethod != null)
                        {
                            var targetTransform = getLocationTransformMethod.Invoke(locationObj, null) as Transform;
                            if (targetTransform != null)
                            {
                                position = targetTransform.position;
                                
                                // 获取 radius 字段
                                var radiusField = taskComponent.GetType().GetField("radius",
                                    BindingFlags.NonPublic | BindingFlags.Instance);
                                if (radiusField != null)
                                {
                                    radius = (float)radiusField.GetValue(taskComponent);
                                }
                                Debug.Log($"[DuckovESP] ✓ 从 GetLocationTransform() 成功提取坐标: {position}");
                                return true;
                            }
                            else
                            {
                                Debug.Log($"[DuckovESP] 调试: GetLocationTransform() 返回 null");
                            }
                        }
                        else
                        {
                            Debug.Log($"[DuckovESP] 调试: location 对象没有 GetLocationTransform() 方法");
                        }
                    }
                }
                catch (Exception ex) 
                { 
                    Debug.Log($"[DuckovESP] 调试: 方案2异常 - {ex.Message}");
                }
            }
            
            // 【方案 3】通过 TryGetLocationPosition() 获取位置
            if (locationField != null)
            {
                try
                {
                    var locationObj = locationField.GetValue(taskComponent);
                    if (locationObj != null)
                    {
                        Type locationObjType = locationObj.GetType();
                        var tryGetPositionMethod = locationObjType.GetMethod("TryGetLocationPosition",
                            BindingFlags.Public | BindingFlags.Instance);
                        
                        if (tryGetPositionMethod != null)
                        {
                            Vector3 tempPos = Vector3.zero;
                            var parameters = new object[] { tempPos };
                            bool success = (bool)tryGetPositionMethod.Invoke(locationObj, parameters);
                            if (success)
                            {
                                position = (Vector3)parameters[0];
                                
                                // 获取 radius 字段
                                var radiusField = taskComponent.GetType().GetField("radius",
                                    BindingFlags.NonPublic | BindingFlags.Instance);
                                if (radiusField != null)
                                {
                                    radius = (float)radiusField.GetValue(taskComponent);
                                }
                                Debug.Log($"[DuckovESP] ✓ 从 TryGetLocationPosition() 成功提取坐标: {position}");
                                return true;
                            }
                            else
                            {
                                Debug.Log($"[DuckovESP] 调试: TryGetLocationPosition() 返回 false");
                            }
                        }
                        else
                        {
                            Debug.Log($"[DuckovESP] 调试: location 对象没有 TryGetLocationPosition() 方法");
                        }
                    }
                }
                catch (Exception ex) 
                { 
                    Debug.Log($"[DuckovESP] 调试: 方案3异常 - {ex.Message}");
                }
            }
            
            return false;
        }
#pragma warning restore IL2087, IL2075
        
        /// <summary>
        /// 从 mapElement (MapElementForTask) 提取位置信息
        /// 用于 SubmitItems 和 QuestTask_TaskEvent
        /// </summary>
#pragma warning disable IL2087, IL2075
        private bool TryExtractMapElementPosition(MonoBehaviour taskComponent, out Vector3 position, out float radius)
        {
            position = Vector3.zero;
            radius = 5f;
            
            // 获取 mapElement 字段
            var mapElementField = taskComponent.GetType().GetField("mapElement",
                BindingFlags.NonPublic | BindingFlags.Instance);
            if (mapElementField == null)
                return false;
            
            var mapElement = mapElementField.GetValue(taskComponent);
            if (mapElement == null)
                return false;
            
            // 尝试从 mapElement.locations[] 获取位置
            var locationsProperty = mapElement.GetType().GetProperty("locations",
                BindingFlags.Public | BindingFlags.Instance);
            if (locationsProperty == null)
                return false;
            
            var locationsList = locationsProperty.GetValue(mapElement);
            if (locationsList == null)
                return false;
            
            // 获取 List 的 Count
            var countProp = locationsList.GetType().GetProperty("Count");
            if (countProp == null)
                return false;
            
            int count = (int)countProp.GetValue(locationsList);
            if (count == 0)
                return false;
            
            // 获取第一个位置
            var indexer = locationsList.GetType().GetProperty("Item",
                BindingFlags.Public | BindingFlags.Instance);
            if (indexer == null)
                return false;
            
            var firstLocation = indexer.GetValue(locationsList, new object[] { 0 });
            if (firstLocation == null)
                return false;
            
            // 从第一个位置提取坐标
            Type locationObjType = firstLocation.GetType();
            var tryGetPositionMethod = locationObjType.GetMethod("TryGetLocationPosition",
                BindingFlags.Public | BindingFlags.Instance);
            
            if (tryGetPositionMethod != null)
            {
                Vector3 tempPos = Vector3.zero;
                var parameters = new object[] { tempPos };
                bool success = (bool)tryGetPositionMethod.Invoke(firstLocation, parameters);
                if (success)
                {
                    position = (Vector3)parameters[0];
                    
                    // 获取 range 字段
                    var rangeProperty = mapElement.GetType().GetProperty("range",
                        BindingFlags.Public | BindingFlags.Instance);
                    if (rangeProperty != null)
                    {
                        radius = (float)rangeProperty.GetValue(mapElement);
                    }
                    return true;
                }
            }
            
            return false;
        }
#pragma warning restore IL2087, IL2075
        
        /// <summary>
        /// 从 QuestTask_TaskEvent 的 gameObject 中获取 SpawnPrefabForTask 组件
        /// 从 SpawnPrefabForTask.locations 列表中提取实际的地址信息
        /// </summary>
#pragma warning disable IL2087, IL2075
        private bool TryExtractTaskEventPosition(MonoBehaviour taskComponent, out Vector3 position, out float radius)
        {
            position = Vector3.zero;
            radius = 5f;
            bool foundPosition = false;
            
            try
            {
                // 步骤 1: 从 TaskEvent 的 gameObject 获取 SpawnPrefabForTask 组件
                var gameObject = taskComponent.gameObject;
                if (gameObject == null)
                {
                    Debug.Log($"[DuckovESP] 调试: TaskEvent 的 gameObject 为 null");
                    return false;
                }
                
                // 获取所有组件
                var components = gameObject.GetComponents<MonoBehaviour>();
                MonoBehaviour spawnPrefabForTask = null;
                
                foreach (var component in components)
                {
                    if (component.GetType().Name == "SpawnPrefabForTask")
                    {
                        spawnPrefabForTask = component;
                        Debug.Log($"[DuckovESP] ✓ 找到 SpawnPrefabForTask 组件");
                        break;
                    }
                }
                
                if (spawnPrefabForTask == null)
                {
                    Debug.Log($"[DuckovESP] 调试: TaskEvent.gameObject 没有 SpawnPrefabForTask 组件");
                    return false;
                }
                
                // 步骤 2: 从 SpawnPrefabForTask 获取 locations 字段
                var locationsField = spawnPrefabForTask.GetType().GetField("locations",
                    BindingFlags.NonPublic | BindingFlags.Instance);
                
                if (locationsField == null)
                {
                    Debug.Log($"[DuckovESP] 调试: SpawnPrefabForTask 没有 locations 字段");
                    return false;
                }
                
                var locationsList = locationsField.GetValue(spawnPrefabForTask);
                if (locationsList == null)
                {
                    Debug.Log($"[DuckovESP] 调试: SpawnPrefabForTask.locations 为 null");
                    return false;
                }
                
                // 步骤 3: 获取 List 的 Count
                var countProp = locationsList.GetType().GetProperty("Count");
                if (countProp == null)
                {
                    Debug.Log($"[DuckovESP] 调试: locations List 没有 Count 属性");
                    return false;
                }
                
                int count = (int)countProp.GetValue(locationsList);
                if (count == 0)
                {
                    Debug.Log($"[DuckovESP] 调试: SpawnPrefabForTask.locations 为空");
                    return false;
                }
                
                Debug.Log($"[DuckovESP] SpawnPrefabForTask.locations 中找到 {count} 个位置");
                
                // 步骤 4: 获取第一个位置并提取坐标
                var indexer = locationsList.GetType().GetProperty("Item",
                    BindingFlags.Public | BindingFlags.Instance);
                
                if (indexer == null)
                {
                    Debug.Log($"[DuckovESP] 调试: locations List 没有索引器");
                    return false;
                }
                
                var firstLocation = indexer.GetValue(locationsList, new object[] { 0 });
                if (firstLocation == null)
                {
                    Debug.Log($"[DuckovESP] 调试: locations[0] 为 null");
                    return false;
                }
                
                // 步骤 5: 从第一个位置提取坐标
                Type locationObjType = firstLocation.GetType();
                var tryGetPositionMethod = locationObjType.GetMethod("TryGetLocationPosition",
                    BindingFlags.Public | BindingFlags.Instance);
                
                if (tryGetPositionMethod == null)
                {
                    Debug.Log($"[DuckovESP] 调试: MultiSceneLocation 没有 TryGetLocationPosition 方法");
                    return false;
                }
                
                Vector3 tempPos = Vector3.zero;
                var parameters = new object[] { tempPos };
                bool success = (bool)tryGetPositionMethod.Invoke(firstLocation, parameters);
                
                if (success)
                {
                    position = (Vector3)parameters[0];
                    Debug.Log($"[DuckovESP] ✓ 从 SpawnPrefabForTask.locations[0] 成功提取坐标: {position}");
                    
                    // 尝试从 SpawnPrefabForTask 获取 radius 字段
                    var radiusField = spawnPrefabForTask.GetType().GetField("radius",
                        BindingFlags.NonPublic | BindingFlags.Instance);
                    if (radiusField != null)
                    {
                        radius = (float)radiusField.GetValue(spawnPrefabForTask);
                        Debug.Log($"[DuckovESP] SpawnPrefabForTask.radius = {radius}");
                    }
                    
                    foundPosition = true;
                }
                else
                {
                    Debug.Log($"[DuckovESP] 调试: SpawnPrefabForTask.locations[0].TryGetLocationPosition() 返回 false");
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"[DuckovESP] 调试: TaskEvent SpawnPrefabForTask 提取异常 - {ex.Message}");
            }
            
            if (foundPosition)
            {
                return true;
            }
            
            // 如果 SpawnPrefabForTask 提取失败，尝试降级到 mapElement 提取
            Debug.Log($"[DuckovESP] 调试: TaskEvent 所有方法都失败，尝试降级到 mapElement 提取");
            return TryExtractMapElementPosition(taskComponent, out position, out radius);
        }
#pragma warning restore IL2087, IL2075
        
        /// <summary>
        /// 检查任务是否已完成
        /// </summary>
        private bool IsTaskFinished(MonoBehaviour taskComponent)
        {
            try
            {
                // 所有任务类型都继承自 Task 基类，检查 finished 字段或通过 IsFinished() 方法
                
                // 先尝试通过反射调用 IsFinished() 方法（如果是 public）
                var isFinishedMethod = taskComponent.GetType().GetMethod("IsFinished",
                    BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance);
                if (isFinishedMethod != null)
                {
                    return (bool)isFinishedMethod.Invoke(taskComponent, null);
                }
                
                // 如果 IsFinished() 不可用，尝试访问 finished 字段
                var finishedField = taskComponent.GetType().GetField("finished",
                    BindingFlags.NonPublic | BindingFlags.Instance);
                if (finishedField != null)
                {
                    return (bool)finishedField.GetValue(taskComponent);
                }
                
                // 默认返回 false（认为任务未完成）
                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 根据碰撞体类型计算任务区域的半径
        /// </summary>
        private float GetQuestZoneRadius(Collider collider)
        {
            if (collider == null)
                return 5f;
            
            try
            {
                // 根据碰撞体类型计算半径
                if (collider is SphereCollider sphereCollider)
                {
                    return sphereCollider.radius * collider.transform.lossyScale.x;
                }
                else if (collider is CapsuleCollider capsuleCollider)
                {
                    return capsuleCollider.radius * Mathf.Max(collider.transform.lossyScale.x, 
                                                               collider.transform.lossyScale.z);
                }
                else if (collider is BoxCollider boxCollider)
                {
                    Vector3 size = boxCollider.size;
                    float maxSize = Mathf.Max(size.x, size.z);
                    return maxSize * 0.5f * Mathf.Max(collider.transform.lossyScale.x, 
                                                      collider.transform.lossyScale.z);
                }
                else
                {
                    // 其他类型的碰撞体，使用bounds计算
                    return collider.bounds.extents.magnitude;
                }
            }
            catch
            {
                return collider.bounds.extents.magnitude;
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
            
            // 【新增】重置任务区域缓存
            _questZonesCached = false;
            _trackedQuestZones.Clear();
            
            // 恢复所有武器的伤害
            RestoreOriginalDamage();
            
            // 【修复】无敌模式不在关卡切换时重置，允许跨关卡保持
            // 只清空临时数据，保留 _godModeEnabled 状态
        }
    }
}
