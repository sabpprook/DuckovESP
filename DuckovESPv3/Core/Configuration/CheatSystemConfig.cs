using UnityEngine;

namespace DuckovESPv3.Core.Configuration
{
    /// <summary>
    /// 作弊系统配置
    /// </summary>
    public class CheatSystemConfig
    {
        // ===== 快捷键配置 =====
        public KeyCode GodModeKey { get; set; } = KeyCode.F7;
        public KeyCode OneHitKillKey { get; set; } = KeyCode.F8;
        public KeyCode SpeedBoostKey { get; set; } = KeyCode.F9;
        public KeyCode InfiniteWeightKey { get; set; } = KeyCode.F10;
        public KeyCode InfiniteAmmoKey { get; set; } = KeyCode.F11;
        public KeyCode InfiniteStaminaKey { get; set; } = KeyCode.F12;
        
        // ===== 功能参数 =====
        public float SpeedMultiplier { get; set; } = 2.5f;
        public float OneHitKillDamageMultiplier { get; set; } = 999f;
        public float InfiniteWeightCapacity { get; set; } = 999999f;
        
        // ===== 功能状态持久化 =====
        public bool GodModeEnabled { get; set; } = false;
        public bool OneHitKillEnabled { get; set; } = false;
        public bool SpeedBoostEnabled { get; set; } = false;
        public bool InfiniteWeightEnabled { get; set; } = false;
        public bool InfiniteAmmoEnabled { get; set; } = false;
        public bool InfiniteStaminaEnabled { get; set; } = false;
        
        // ===== 状态显示配置 =====
        public bool ShowCheatStatusOverlay { get; set; } = true;
        public Color EnabledColor { get; set; } = new Color(0.2f, 1f, 0.2f, 1f); // 亮绿色
        public Color DisabledColor { get; set; } = new Color(0.5f, 0.5f, 0.5f, 0.7f); // 灰色
        public int OverlayFontSize { get; set; } = 14;
        public float OverlayPaddingRight { get; set; } = 20f;
        public float OverlayPaddingTop { get; set; } = 80f;
        
        // ===== 优化开关 =====
        public bool EnableDirtyFlagOptimization { get; set; } = true;
    }
}
