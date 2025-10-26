using UnityEngine;

namespace DuckovESPv3.Core.Configuration
{
    /// <summary>
    /// 自瞄系统配置
    /// </summary>
    public class AimbotSystemConfig
    {
        // ===== 自动瞄准设置 (Aimbot) =====
        
        [ConfigEntry("Aimbot.Enable", DefaultValue = false)]
        public bool EnableAimbot { get; set; } = false;
        
        [ConfigEntry("Aimbot.Key", DefaultValue = KeyCode.Mouse1)]
        public KeyCode AimbotKey { get; set; } = KeyCode.Mouse1;
        
        [ConfigEntry("Aimbot.FOV", DefaultValue = 300f)]
        public float AimbotFOV { get; set; } = 300f;
        
        [ConfigEntry("Aimbot.Smoothness", DefaultValue = 5f)]
        public float AimbotSmoothness { get; set; } = 5f;
        
        [ConfigEntry("Aimbot.PredictMovement", DefaultValue = true)]
        public bool AimbotPredictMovement { get; set; } = true;
        
        [ConfigEntry("Aimbot.AimAtHead", DefaultValue = true)]
        public bool AimbotAimAtHead { get; set; } = true;
        
        [ConfigEntry("Aimbot.MaxDistance", DefaultValue = 200f)]
        public float AimbotMaxDistance { get; set; } = 200f;
        
        [ConfigEntry("Aimbot.IgnoreWalls", DefaultValue = false)]
        public bool AimbotIgnoreWalls { get; set; } = false;
        
        [ConfigEntry("Aimbot.IgnoreTeamCheck", DefaultValue = false)]
        public bool AimbotIgnoreTeamCheck { get; set; } = false;

        // ===== 自动扳机设置 (TriggerBot) =====
        
        [ConfigEntry("TriggerBot.Enable", DefaultValue = false)]
        public bool EnableTriggerBot { get; set; } = false;
        
        [ConfigEntry("TriggerBot.Delay", DefaultValue = 0.05f)]
        public float TriggerBotDelay { get; set; } = 0.05f;
        
        [ConfigEntry("TriggerBot.OnlyADS", DefaultValue = true)]
        public bool TriggerBotOnlyADS { get; set; } = true;
        
        [ConfigEntry("TriggerBot.TeamCheck", DefaultValue = true)]
        public bool TriggerBotTeamCheck { get; set; } = true;

        // ===== 无后座力设置 (NoRecoil) =====
        
        [ConfigEntry("NoRecoil.Enable", DefaultValue = false)]
        public bool EnableNoRecoil { get; set; } = false;

        /// <summary>
        /// 重置为默认值
        /// </summary>
        public void ResetToDefaults()
        {
            EnableAimbot = false;
            AimbotKey = KeyCode.Mouse1;
            AimbotFOV = 300f;
            AimbotSmoothness = 5f;
            AimbotPredictMovement = true;
            AimbotAimAtHead = true;
            AimbotMaxDistance = 200f;
            AimbotIgnoreWalls = false;
            AimbotIgnoreTeamCheck = false;

            EnableTriggerBot = false;
            TriggerBotDelay = 0.05f;
            TriggerBotOnlyADS = true;
            TriggerBotTeamCheck = true;

            EnableNoRecoil = false;
        }
    }
}
