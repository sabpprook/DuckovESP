using System;
using System.Collections.Generic;
using System.Reflection;
using DuckovESPv3.Core.Configuration;
using DuckovESPv3.Infrastructure.Logging;
using ItemStatsSystem;
using UnityEngine;

namespace DuckovESPv3.Core.Systems.Cheat
{
    /// <summary>
    /// 作弊系统 - 提供无敌、一击必杀、速度提升等功能
    /// V3 重构版本 - 模块化设计 + 性能优化
    /// </summary>
    public class CheatSystem
    {
        private readonly CheatSystemConfig _config;
        private readonly Infrastructure.Logging.ILogger _logger;
        private readonly ConfigurationManager? _configManager;
        
        // 反射访问私有字段
        private static readonly FieldInfo CurrentStaminaField = typeof(CharacterMainControl).GetField("currentStamina", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly FieldInfo StarveField = typeof(CharacterMainControl).GetField("starve", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly FieldInfo ThirstyField = typeof(CharacterMainControl).GetField("thirsty", BindingFlags.Instance | BindingFlags.NonPublic);
        
        // Stat Hash 缓存
        private static readonly int DamageStatHash = "Damage".GetHashCode();
        private static readonly int RunSpeedHash = "RunSpeed".GetHashCode();
        private static readonly int WalkSpeedHash = "WalkSpeed".GetHashCode();
        private static readonly int CarryWeightHash = "CarryWeight".GetHashCode();
        
        // 作弊状态
        public bool GodModeEnabled { get; private set; }
        public bool OneHitKillEnabled { get; private set; }
        public bool SpeedBoostEnabled { get; private set; }
        public bool InfiniteWeightEnabled { get; private set; }
        public bool InfiniteAmmoEnabled { get; private set; }
        public bool InfiniteStaminaEnabled { get; private set; }
        
        // 原始数值备份
        private float _originalSpeed = 0f;
        private Dictionary<Item, Dictionary<int, float>> _originalStatValues = new Dictionary<Item, Dictionary<int, float>>();
        
        // 脏标记优化
        private class FeatureState
        {
            public bool IsEnabled;
            public bool IsDirty;
        }
        
        private Dictionary<string, FeatureState> _featureStates = new Dictionary<string, FeatureState>
        {
            { "GodMode", new FeatureState() },
            { "OneHitKill", new FeatureState() },
            { "SpeedBoost", new FeatureState() },
            { "InfiniteWeight", new FeatureState() },
            { "InfiniteStamina", new FeatureState() }
        };
        
        public CheatSystem(CheatSystemConfig config, Infrastructure.Logging.ILogger logger, ConfigurationManager? configManager = null)
        {
            _config = config;
            _logger = logger;
            _configManager = configManager;
            
            // 从配置加载初始状态
            GodModeEnabled = config.GodModeEnabled;
            OneHitKillEnabled = config.OneHitKillEnabled;
            SpeedBoostEnabled = config.SpeedBoostEnabled;
            InfiniteWeightEnabled = config.InfiniteWeightEnabled;
            InfiniteAmmoEnabled = config.InfiniteAmmoEnabled;
            InfiniteStaminaEnabled = config.InfiniteStaminaEnabled;
            
            _logger.Info($"[CheatSystem] 已加载配置 - GodMode:{GodModeEnabled}, OneHit:{OneHitKillEnabled}, Speed:{SpeedBoostEnabled}, Weight:{InfiniteWeightEnabled}, Ammo:{InfiniteAmmoEnabled}, Stamina:{InfiniteStaminaEnabled}");
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
            
            // 应用作弊效果（使用脏标记优化）
            if (GodModeEnabled)
                ApplyGodMode(player);
            
            if (OneHitKillEnabled)
                ApplyOneHitKill(player);
            
            if (SpeedBoostEnabled)
                ApplySpeedBoost(player);
            
            if (InfiniteWeightEnabled)
                ApplyInfiniteWeight(player);
            
            // 注意：InfiniteAmmo 通过 Harmony Patch 实现，不需要每帧更新
            
            if (InfiniteStaminaEnabled)
                ApplyInfiniteStamina(player);
        }
        
        /// <summary>
        /// 检查快捷键
        /// </summary>
        private void CheckHotkeys()
        {
            // 需要按住 Shift 键才能触发所有快捷键
            if (!Input.GetKey(KeyCode.LeftShift))
                return;
            
            // F7 - 无敌模式
            if (Input.GetKeyDown(_config.GodModeKey))
            {
                ToggleGodMode();
            }
            
            // F8 - 一击必杀
            if (Input.GetKeyDown(_config.OneHitKillKey))
            {
                ToggleOneHitKill();
            }
            
            // F9 - 速度提升
            if (Input.GetKeyDown(_config.SpeedBoostKey))
            {
                ToggleSpeedBoost();
            }
            
            // F10 - 无限负重
            if (Input.GetKeyDown(_config.InfiniteWeightKey))
            {
                ToggleInfiniteWeight();
            }
            
            // F11 - 无限子弹
            if (Input.GetKeyDown(_config.InfiniteAmmoKey))
            {
                ToggleInfiniteAmmo();
            }
            
            // F12 - 无限耐力
            if (Input.GetKeyDown(_config.InfiniteStaminaKey))
            {
                ToggleInfiniteStamina();
            }
        }
        
        #region 功能切换方法
        
        /// <summary>
        /// 保存配置到文件
        /// </summary>
        private void SaveConfig()
        {
            if (_configManager != null)
            {
                _config.GodModeEnabled = GodModeEnabled;
                _config.OneHitKillEnabled = OneHitKillEnabled;
                _config.SpeedBoostEnabled = SpeedBoostEnabled;
                _config.InfiniteWeightEnabled = InfiniteWeightEnabled;
                _config.InfiniteAmmoEnabled = InfiniteAmmoEnabled;
                _config.InfiniteStaminaEnabled = InfiniteStaminaEnabled;
                
                _configManager.Save();
            }
        }
        
        public void ToggleGodMode()
        {
            GodModeEnabled = !GodModeEnabled;
            _featureStates["GodMode"].IsEnabled = GodModeEnabled;
            _featureStates["GodMode"].IsDirty = true;
            
            _logger.Info($"[CheatSystem] 无敌模式: {(GodModeEnabled ? "已启用" : "已禁用")}");
            
            if (!GodModeEnabled)
            {
                DisableGodMode();
            }
            
            SaveConfig();
        }
        
        public void ToggleOneHitKill()
        {
            OneHitKillEnabled = !OneHitKillEnabled;
            _featureStates["OneHitKill"].IsEnabled = OneHitKillEnabled;
            _featureStates["OneHitKill"].IsDirty = true;
            
            _logger.Info($"[CheatSystem] 一击必杀: {(OneHitKillEnabled ? "已启用" : "已禁用")}");
            
            if (!OneHitKillEnabled)
            {
                RestoreOriginalDamage();
            }
            
            SaveConfig();
        }
        
        public void ToggleSpeedBoost()
        {
            SpeedBoostEnabled = !SpeedBoostEnabled;
            _featureStates["SpeedBoost"].IsEnabled = SpeedBoostEnabled;
            _featureStates["SpeedBoost"].IsDirty = true;
            
            _logger.Info($"[CheatSystem] 速度提升: {(SpeedBoostEnabled ? "已启用" : "已禁用")}");
            
            CharacterMainControl player = CharacterMainControl.Main;
            if (!SpeedBoostEnabled && player != null && _originalSpeed > 0f)
            {
                RestoreOriginalSpeed(player);
            }
            
            SaveConfig();
        }
        
        public void ToggleInfiniteWeight()
        {
            InfiniteWeightEnabled = !InfiniteWeightEnabled;
            _featureStates["InfiniteWeight"].IsEnabled = InfiniteWeightEnabled;
            _featureStates["InfiniteWeight"].IsDirty = true;
            
            _logger.Info($"[CheatSystem] 无限负重: {(InfiniteWeightEnabled ? "已启用" : "已禁用")}");
            
            SaveConfig();
        }
        
        public void ToggleInfiniteAmmo()
        {
            InfiniteAmmoEnabled = !InfiniteAmmoEnabled;
            _logger.Info($"[CheatSystem] 无限子弹: {(InfiniteAmmoEnabled ? "已启用" : "已禁用")}");
            
            SaveConfig();
        }
        
        public void ToggleInfiniteStamina()
        {
            InfiniteStaminaEnabled = !InfiniteStaminaEnabled;
            _featureStates["InfiniteStamina"].IsEnabled = InfiniteStaminaEnabled;
            _featureStates["InfiniteStamina"].IsDirty = true;
            
            _logger.Info($"[CheatSystem] 无限耐力: {(InfiniteStaminaEnabled ? "已启用" : "已禁用")}");
            
            SaveConfig();
        }
        
        #endregion
        
        #region 功能实现方法
        
        /// <summary>
        /// 应用无敌模式
        /// </summary>
        private void ApplyGodMode(CharacterMainControl player)
        {
            try
            {
                var health = player.Health;
                if (health == null)
                    return;
                
                // 使用 Health.SetInvincible(true) 来防止所有伤害
                if (!health.Invincible)
                {
                    health.SetInvincible(true);
                }
                
                // 备份：定期检查并恢复血量
                if (health.CurrentHealth <= 0)
                {
                    health.SetHealth(health.MaxHealth);
                    player.AddHealth(health.MaxHealth);
                }
            }
            catch (Exception ex)
            {
                _logger.Warning($"[CheatSystem] 应用无敌模式失败: {ex.Message}");
            }
        }
        
        /// <summary>
        /// 禁用无敌模式
        /// </summary>
        private void DisableGodMode()
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
                _logger.Warning($"[CheatSystem] 禁用无敌模式失败: {ex.Message}");
            }
        }
        
        /// <summary>
        /// 应用一击必杀
        /// </summary>
        private void ApplyOneHitKill(CharacterMainControl player)
        {
            try
            {
                var agentHolder = player.agentHolder;
                if (agentHolder != null)
                {
                    var currentGun = agentHolder.CurrentHoldGun;
                    if (currentGun != null && currentGun.Item != null)
                    {
                        ApplyStatMultiplier(currentGun.Item, DamageStatHash, _config.OneHitKillDamageMultiplier);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Warning($"[CheatSystem] 应用一击必杀失败: {ex.Message}");
            }
        }
        
        /// <summary>
        /// 修改 Item 的 Stat 值
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
            try
            {
                var characterItem = player.CharacterItem;
                if (characterItem == null)
                    return;
                
                var runSpeedStat = characterItem.GetStat(RunSpeedHash);
                var walkSpeedStat = characterItem.GetStat(WalkSpeedHash);
                
                if (runSpeedStat != null && walkSpeedStat != null)
                {
                    // 备份原始速度（第一次）
                    if (_originalSpeed == 0f)
                    {
                        _originalSpeed = runSpeedStat.BaseValue;
                    }
                    
                    // 设置提升后的速度
                    float targetSpeed = _originalSpeed * _config.SpeedMultiplier;
                    runSpeedStat.BaseValue = targetSpeed;
                    walkSpeedStat.BaseValue = targetSpeed * 0.6f;
                }
            }
            catch (Exception ex)
            {
                _logger.Warning($"[CheatSystem] 应用速度提升失败: {ex.Message}");
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
                        var runSpeedStat = characterItem.GetStat(RunSpeedHash);
                        var walkSpeedStat = characterItem.GetStat(WalkSpeedHash);
                        
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
                _logger.Warning($"[CheatSystem] 恢复原始速度失败: {ex.Message}");
            }
        }
        
        /// <summary>
        /// 应用无限负重
        /// </summary>
        private void ApplyInfiniteWeight(CharacterMainControl player)
        {
            try
            {
                Item characterItem = player.CharacterItem;
                if (characterItem != null)
                {
                    Stat stat = characterItem.GetStat(CarryWeightHash);
                    if (stat != null)
                    {
                        stat.BaseValue = _config.InfiniteWeightCapacity;
                    }
                    
                    // 同时设置其他负重相关的 Stat
                    foreach (int statHash in new int[]
                    {
                        "Weight".GetHashCode(),
                        "MaxWeight".GetHashCode(),
                        "MaxCarryWeight".GetHashCode(),
                        "CarryCapacity".GetHashCode()
                    })
                    {
                        Stat stat2 = characterItem.GetStat(statHash);
                        if (stat2 != null)
                        {
                            stat2.BaseValue = _config.InfiniteWeightCapacity;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Warning($"[CheatSystem] 应用无限负重失败: {ex.Message}");
            }
        }
        
        /// <summary>
        /// 应用无限耐力
        /// </summary>
        private void ApplyInfiniteStamina(CharacterMainControl player)
        {
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
                _logger.Warning($"[CheatSystem] 应用无限耐力失败: {ex.Message}");
            }
        }
        
        #endregion
        
        /// <summary>
        /// 当关卡卸载时重置
        /// </summary>
        public void OnLevelUnload()
        {
            _originalSpeed = 0f;
            RestoreOriginalDamage();
        }
    }
}
