using System;
using System.Reflection;
using UnityEngine;

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
        
        // 速度倍数
        private const float SPEED_MULTIPLIER = 2.5f;
        
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
            // F7 - 无敌模式
            if (Input.GetKeyDown(_config.GodModeKey))
            {
                _godModeEnabled = !_godModeEnabled;
                Debug.Log($"DuckovESP: 无敌模式 {(_godModeEnabled ? "启用" : "禁用")}");
            }
            
            // F8 - 一击必杀
            if (Input.GetKeyDown(_config.OneHitKillKey))
            {
                _oneHitKillEnabled = !_oneHitKillEnabled;
                Debug.Log($"DuckovESP: 一击必杀 {(_oneHitKillEnabled ? "启用" : "禁用")}");
            }
            
            // F9 - 速度提升
            if (Input.GetKeyDown(_config.SpeedBoostKey))
            {
                _speedBoostEnabled = !_speedBoostEnabled;
                Debug.Log($"DuckovESP: 速度提升 {(_speedBoostEnabled ? "启用" : "禁用")}");
                
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
                Debug.Log($"DuckovESP: 无限负重 {(_infiniteWeightEnabled ? "启用" : "禁用")}");
            }
            
            // F11 - 无限子弹
            if (Input.GetKeyDown(_config.InfiniteAmmoKey))
            {
                _infiniteAmmoEnabled = !_infiniteAmmoEnabled;
                Debug.Log($"DuckovESP: 无限子弹 {(_infiniteAmmoEnabled ? "启用" : "禁用")}");
            }
            
            // F12 - 无限耐力
            if (Input.GetKeyDown(_config.InfiniteStaminaKey))
            {
                _infiniteStaminaEnabled = !_infiniteStaminaEnabled;
                Debug.Log($"DuckovESP: 无限耐力 {(_infiniteStaminaEnabled ? "启用" : "禁用")}");
            }
        }
        
        /// <summary>
        /// 应用无敌模式
        /// </summary>
        private void ApplyGodMode(CharacterMainControl player)
        {
            if (!_godModeEnabled)
                return;
            
            try
            {
                // 获取玩家生命值组件
                var health = player.Health;
                if (health != null)
                {
                    // 保持满血
                    if (health.CurrentHealth < health.MaxHealth)
                    {
                        player.AddHealth(health.MaxHealth - health.CurrentHealth);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"DuckovESP: 应用无敌模式失败 - {ex.Message}");
            }
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
                Debug.LogWarning($"DuckovESP: 应用速度提升失败 - {ex.Message}");
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
                Debug.LogWarning($"DuckovESP: 恢复原始速度失败 - {ex.Message}");
            }
        }
        
        /// <summary>
        /// 应用无限负重（实际上负重系统比较复杂，这里简化处理）
        /// </summary>
        private void ApplyInfiniteWeight(CharacterMainControl player)
        {
            if (!_infiniteWeightEnabled)
                return;
            
            try
            {
                // 负重系统由 TotalWeight 属性管理，该属性是只读的
                // 实际负重在物品添加时计算，这里无法直接修改
                // 可以考虑修改物品的重量或其他方式，但需要更深入的API理解
                // 暂时保留此功能但实际效果有限
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"DuckovESP: 应用无限负重失败 - {ex.Message}");
            }
        }
        
        /// <summary>
        /// 应用无限子弹
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
                        
                        // 使用反射设置私有属性 bulletCount
                        if (capacity > 0 && BulletCountProperty != null)
                        {
                            int currentBullets = gunSetting.BulletCount;
                            if (currentBullets < capacity)
                            {
                                BulletCountProperty.SetValue(gunSetting, capacity, null);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"DuckovESP: 应用无限子弹失败 - {ex.Message}");
            }
        }
        
        /// <summary>
        /// 应用无限耐力
        /// </summary>
        private void ApplyInfiniteStamina(CharacterMainControl player)
        {
            if (!_infiniteStaminaEnabled)
                return;
            
            try
            {
                // 使用反射设置私有字段 currentStamina
                if (CurrentStaminaField != null)
                {
                    float currentStamina = player.CurrentStamina;
                    float maxStamina = player.MaxStamina;
                    
                    if (currentStamina < maxStamina)
                    {
                        CurrentStaminaField.SetValue(player, maxStamina);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"DuckovESP: 应用无限耐力失败 - {ex.Message}");
            }
        }
        
        /// <summary>
        /// 获取作弊状态信息
        /// </summary>
        public string GetStatusText()
        {
            string status = "=== 作弊功能状态 ===\n";
            status += $"无敌模式 (F7): {(_godModeEnabled ? "✓ 启用" : "✗ 禁用")}\n";
            status += $"一击必杀 (F8): {(_oneHitKillEnabled ? "✓ 启用" : "✗ 禁用")}\n";
            status += $"速度提升 (F9): {(_speedBoostEnabled ? "✓ 启用" : "✗ 禁用")}\n";
            status += $"无限负重 (F10): {(_infiniteWeightEnabled ? "✓ 启用" : "✗ 禁用")}\n";
            status += $"无限子弹 (F11): {(_infiniteAmmoEnabled ? "✓ 启用" : "✗ 禁用")}\n";
            status += $"无限耐力 (F12): {(_infiniteStaminaEnabled ? "✓ 启用" : "✗ 禁用")}";
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
        /// 当关卡卸载时重置
        /// </summary>
        public void OnLevelUnload()
        {
            // 重置所有作弊状态
            _originalSpeed = 0f;
        }
    }
}
