using UnityEngine;
using DuckovESPv3.Core.Configuration;
using DuckovESPv3.Infrastructure.Localization;

namespace DuckovESPv3.Core.Systems.Cheat.UI
{
    /// <summary>
    /// 作弊状态叠加层 - 显示在屏幕右上角
    /// </summary>
    public class CheatStatusOverlay : MonoBehaviour
    {
        private CheatSystem? _cheatSystem;
        private CheatSystemConfig? _config;
        
        private GUIStyle? _enabledStyle;
        private GUIStyle? _disabledStyle;
        private GUIStyle? _titleStyle;
        
        private bool _stylesInitialized = false;
        
        public void Initialize(CheatSystem cheatSystem, CheatSystemConfig config)
        {
            _cheatSystem = cheatSystem;
            _config = config;
        }
        
        private void InitializeStyles()
        {
            if (_stylesInitialized || _config == null)
                return;
            
            // 启用状态样式（亮绿色）
            _enabledStyle = new GUIStyle(GUI.skin.label)
            {
                normal = { textColor = _config.EnabledColor },
                fontSize = _config.OverlayFontSize,
                alignment = TextAnchor.UpperRight,
                fontStyle = FontStyle.Bold
            };
            
            // 禁用状态样式（灰色）
            _disabledStyle = new GUIStyle(GUI.skin.label)
            {
                normal = { textColor = _config.DisabledColor },
                fontSize = _config.OverlayFontSize,
                alignment = TextAnchor.UpperRight,
                fontStyle = FontStyle.Normal
            };
            
            // 标题样式（白色）
            _titleStyle = new GUIStyle(GUI.skin.label)
            {
                normal = { textColor = Color.white },
                fontSize = _config.OverlayFontSize + 2,
                alignment = TextAnchor.UpperRight,
                fontStyle = FontStyle.Bold
            };
            
            _stylesInitialized = true;
        }
        
        private void OnGUI()
        {
            if (_cheatSystem == null || _config == null || !_config.ShowCheatStatusOverlay)
                return;
            
            if (!_stylesInitialized)
            {
                InitializeStyles();
            }
            
            if (_enabledStyle == null || _disabledStyle == null || _titleStyle == null)
                return;
            
            // 计算位置（右上角）
            float x = Screen.width - _config.OverlayPaddingRight;
            float y = _config.OverlayPaddingTop;
            float width = 250f;
            float lineHeight = _config.OverlayFontSize + 6;
            
            // 使用 GUILayout.BeginArea 创建右对齐区域
            GUILayout.BeginArea(new Rect(x - width, y, width, 300));
            
            // 标题
            GUILayout.Label(LocalizationManager.Get("UI.CheatOverlay.HeaderText"), _titleStyle);
            GUILayout.Space(5);
            
            // 功能列表
            DrawFeatureStatus(LocalizationManager.Get("UI.CheatOverlay.GodMode"), _cheatSystem.GodModeEnabled);
            DrawFeatureStatus(LocalizationManager.Get("UI.CheatOverlay.OneHitKill"), _cheatSystem.OneHitKillEnabled);
            DrawFeatureStatus(LocalizationManager.Get("UI.CheatOverlay.SpeedBoost"), _cheatSystem.SpeedBoostEnabled);
            DrawFeatureStatus(LocalizationManager.Get("UI.CheatOverlay.InfiniteWeight"), _cheatSystem.InfiniteWeightEnabled);
            DrawFeatureStatus(LocalizationManager.Get("UI.CheatOverlay.InfiniteAmmo"), _cheatSystem.InfiniteAmmoEnabled);
            DrawFeatureStatus(LocalizationManager.Get("UI.CheatOverlay.InfiniteStamina"), _cheatSystem.InfiniteStaminaEnabled);
            
            GUILayout.EndArea();
        }
        
        private void DrawFeatureStatus(string featureName, bool isEnabled)
        {
            if (_enabledStyle == null || _disabledStyle == null)
                return;
            
            GUIStyle style = isEnabled ? _enabledStyle : _disabledStyle;
            string prefix = isEnabled ? "✓" : "✗";
            GUILayout.Label($"{prefix} {featureName}", style);
        }
    }
}
