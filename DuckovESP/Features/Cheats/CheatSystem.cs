using ItemStatsSystem;
using System;
using System.Reflection;
using UnityEngine;
using DuckovESP.Utils.Localization;

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
        private ItemAgent_Gun _lastTrackedGun = null;
        private float _originalGunDamage = 0f;

        // 速度倍数
        private const float SPEED_MULTIPLIER = 2.5f;
        private const float ONE_HIT_KILL_DAMAGE = 999999f;

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
                Debug.LogWarning(LocalizationManager.Get("Error.ApplyGodMode", ("error", ex.Message)));
            }
        }

        /// <summary>
        /// 应用一击必杀（简化版：禁用功能暂不实现对枪械的支持）
        /// 注意：这个功能对近战武器有效，对枪械目前无可靠实现方法
        /// </summary>
        private void ApplyOneHitKill(CharacterMainControl player)
        {
            // 暂时禁用一击必杀，因为无可靠的API修改枪械伤害
            // 子弹伤害修改不可靠（已在 AimbotSystem 中移除）
            // ItemSetting_Gun 没有公开的 Damage 属性
            // 需要等待更深入的游戏API研究

            if (!_oneHitKillEnabled)
                return;

            // TODO: 等待找到可靠的枪械伤害修改方法
            // 目前仅对近战武器有效
        }

        /// <summary>
        /// 恢复武器原始伤害（暂未实现）
        /// </summary>
        private void RestoreOriginalGunDamage()
        {
            // 占位方法
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
        /// 当关卡卸载时重置
        /// </summary>
        public void OnLevelUnload()
        {
            // 重置所有作弊状态
            _originalSpeed = 0f;
        }
    }
}
