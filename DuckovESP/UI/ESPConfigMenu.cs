using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DuckovESP.Utils.Localization;

namespace DuckovESP
{
    /// <summary>
    /// ESP配置菜单UI
    /// </summary>
    public class ESPConfigMenu
    {
        private bool _showMenu = false;
        private Rect _menuRect = new Rect(50, 50, 520, 750); // 增加窗口大小容纳敌人ESP
        private Vector2 _scrollPosition = Vector2.zero;
        private GUIStyle _windowStyle;
        private GUIStyle _labelStyle;
        private GUIStyle _buttonStyle;
        private GUIStyle _toggleStyle;
        private bool _stylesInitialized = false;
        
        private ESPConfig _config;
        
        // Language selector dropdown
        private string[] _languageCodes = new string[] { };
        private string[] _languageNames = new string[] { };
        private int _selectedLanguageIndex = 0;
        private bool _showLanguageDropdown = false;
        
        public ESPConfigMenu(ESPConfig config)
        {
            _config = config;
            InitializeLanguageOptions();
        }
        
        private void InitializeLanguageOptions()
        {
            var supportedLanguages = LocalizationManager.GetSupportedLanguages();
            _languageCodes = new string[supportedLanguages.Count()];
            _languageNames = new string[supportedLanguages.Count()];
            
            int index = 0;
            foreach (var langCode in supportedLanguages)
            {
                _languageCodes[index] = langCode;
                _languageNames[index] = LocalizationManager.GetLanguageDisplayNameByCode(langCode);
                
                if (langCode == LocalizationManager.GetCurrentLanguage())
                {
                    _selectedLanguageIndex = index;
                }
                
                index++;
            }
        }
        
        public bool IsMenuOpen => _showMenu;
        
        public void ToggleMenu()
        {
            _showMenu = !_showMenu;
            if (!_showMenu)
            {
                _showLanguageDropdown = false; // Close dropdown when menu is closed
            }
            Debug.Log(LocalizationManager.Get("UI.ConfigMenu.MenuToggled", ("status", _showMenu ? "打开" : "关闭")));
        }
        
        public void DrawMenu()
        {
            if (!_showMenu)
                return;
                
            InitializeStyles();
            
            _menuRect = GUILayout.Window(
                999999, 
                _menuRect, 
                DrawMenuWindow, 
                LocalizationManager.Get("UI.Menu.Title"), 
                _windowStyle
            );
        }
        
        private void InitializeStyles()
        {
            if (_stylesInitialized)
                return;
                
            _windowStyle = new GUIStyle(GUI.skin.window);
            _windowStyle.normal.background = MakeTex(2, 2, new Color(0.1f, 0.1f, 0.1f, 0.95f));
            
            _labelStyle = new GUIStyle(GUI.skin.label);
            _labelStyle.normal.textColor = Color.white;
            _labelStyle.fontSize = 12;
            
            _buttonStyle = new GUIStyle(GUI.skin.button);
            _buttonStyle.fontSize = 12;
            _buttonStyle.normal.textColor = Color.white;
            
            _toggleStyle = new GUIStyle(GUI.skin.toggle);
            _toggleStyle.normal.textColor = Color.white;
            _toggleStyle.fontSize = 12;
            
            _stylesInitialized = true;
        }
        
        private void DrawMenuWindow(int windowID)
        {
            GUILayout.BeginVertical();
            
            // 语言选择下拉框（放在最顶部）
            GUILayout.BeginHorizontal();
            GUILayout.Label(LocalizationManager.Get("Language.CurrentLanguage", ("lang", LocalizationManager.GetCurrentLanguageName())), _labelStyle, GUILayout.Width(150));
            
            // Draw dropdown button
            if (GUILayout.Button($"▼ {_languageNames[_selectedLanguageIndex]}", _buttonStyle, GUILayout.Width(150)))
            {
                _showLanguageDropdown = !_showLanguageDropdown;
            }
            
            GUILayout.EndHorizontal();
            
            // Draw dropdown options
            if (_showLanguageDropdown && _languageNames.Length > 0)
            {
                GUILayout.BeginVertical(GUI.skin.box);
                for (int i = 0; i < _languageNames.Length; i++)
                {
                    if (GUILayout.Button(_languageNames[i], _buttonStyle, GUILayout.Height(25)))
                    {
                        if (_selectedLanguageIndex != i)
                        {
                            _selectedLanguageIndex = i;
                            LocalizationManager.SetLanguage(_languageCodes[i]);
                            Debug.Log(LocalizationManager.Get("Language.SwitchedTo", ("lang", _languageNames[i])));
                            // Re-initialize language options to update display names
                            InitializeLanguageOptions();
                        }
                        _showLanguageDropdown = false;
                    }
                }
                GUILayout.EndVertical();
            }
            
            GUILayout.Space(10);
            
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, GUILayout.Height(650)); // 增加高度以容纳敌人ESP选项
            
            // 标题
            GUILayout.Label(LocalizationManager.Get("UI.Menu.ESP3DSettings"), _labelStyle);
            GUILayout.Space(5);
            
            // 3D ESP开关
            _config.Enable3DESP = GUILayout.Toggle(_config.Enable3DESP, LocalizationManager.Get("UI.Toggle.Enable3DESP"), _toggleStyle);
            
            // 最大距离
            GUILayout.BeginHorizontal();
            GUILayout.Label(LocalizationManager.Get("UI.Label.MaxDisplayDistance") + $": {_config.MaxESPDistance:F0}m", _labelStyle, GUILayout.Width(200));
            _config.MaxESPDistance = GUILayout.HorizontalSlider(_config.MaxESPDistance, 10f, 500f, GUILayout.Width(200));
            GUILayout.EndHorizontal();
            
            // 字体大小
            GUILayout.BeginHorizontal();
            GUILayout.Label(LocalizationManager.Get("UI.Label.FontSize") + $": {_config.ESPFontSize}", _labelStyle, GUILayout.Width(200));
            _config.ESPFontSize = (int)GUILayout.HorizontalSlider(_config.ESPFontSize, 8, 24, GUILayout.Width(200));
            GUILayout.EndHorizontal();
            
            // 最大显示物品数
            GUILayout.BeginHorizontal();
            GUILayout.Label(LocalizationManager.Get("UI.Label.MaxDisplayItems") + $": {_config.MaxDisplayItems}", _labelStyle, GUILayout.Width(200));
            _config.MaxDisplayItems = (int)GUILayout.HorizontalSlider(_config.MaxDisplayItems, 1, 10, GUILayout.Width(200));
            GUILayout.EndHorizontal();
            
            // 显示选项
            _config.ShowDistance = GUILayout.Toggle(_config.ShowDistance, LocalizationManager.Get("UI.Toggle.ShowDistance"), _toggleStyle);
            _config.ShowConnectLine = GUILayout.Toggle(_config.ShowConnectLine, LocalizationManager.Get("UI.Toggle.ShowConnectLine"), _toggleStyle);
            
            GUILayout.Space(10);
            GUILayout.Label(LocalizationManager.Get("UI.Menu.FilterSettings"), _labelStyle);
            GUILayout.Space(5);
            
            // 3D ESP品质过滤
            GUILayout.BeginHorizontal();
            GUILayout.Label(LocalizationManager.Get("UI.Label.Quality3DMinLabel", ("quality", GetQualityName(_config.MinQualityFilter3D))), _labelStyle, GUILayout.Width(200));
            _config.MinQualityFilter3D = (int)GUILayout.HorizontalSlider(_config.MinQualityFilter3D, 0, 6, GUILayout.Width(200));
            GUILayout.EndHorizontal();
            
            _config.ShowEmptyBoxes = GUILayout.Toggle(_config.ShowEmptyBoxes, LocalizationManager.Get("UI.Toggle.ShowEmptyBoxes"), _toggleStyle);
            
            GUILayout.Space(10);
            GUILayout.Label(LocalizationManager.Get("UI.Menu.MapMarkerSettings"), _labelStyle);
            GUILayout.Space(5);
            
            // 小地图标记开关
            _config.EnableMapMarkers = GUILayout.Toggle(_config.EnableMapMarkers, LocalizationManager.Get("UI.Toggle.EnableMapMarkers"), _toggleStyle);
            
            // 小地图品质过滤
            GUILayout.BeginHorizontal();
            GUILayout.Label(LocalizationManager.Get("UI.Label.MapMarkerQualityLabel", ("quality", GetQualityName(_config.MinQualityForMapMarkers))), _labelStyle, GUILayout.Width(200));
            _config.MinQualityForMapMarkers = (int)GUILayout.HorizontalSlider(_config.MinQualityForMapMarkers, 0, 6, GUILayout.Width(200));
            GUILayout.EndHorizontal();
            
            // 撤离点指示开关
            _config.EnableEvacuationIndicator = GUILayout.Toggle(_config.EnableEvacuationIndicator, LocalizationManager.Get("UI.Toggle.EnableEvacuationIndicator"), _toggleStyle);
            
            // 显示撤离点距离
            _config.ShowEvacuationDistance = GUILayout.Toggle(_config.ShowEvacuationDistance, LocalizationManager.Get("UI.Toggle.ShowEvacuationDistance"), _toggleStyle);
            
            // 撤离点指示大小
            GUILayout.BeginHorizontal();
            GUILayout.Label(LocalizationManager.Get("UI.Label.EvacuationIndicatorSizeLabel", ("size", _config.EvacuationIndicatorSize)), _labelStyle, GUILayout.Width(200));
            _config.EvacuationIndicatorSize = GUILayout.HorizontalSlider(_config.EvacuationIndicatorSize, 10f, 50f, GUILayout.Width(200));
            GUILayout.EndHorizontal();
            
            // 任务区域指示开关
            GUILayout.Space(5);
            _config.EnableQuestZoneIndicator = GUILayout.Toggle(_config.EnableQuestZoneIndicator, LocalizationManager.Get("UI.Toggle.EnableQuestZoneIndicator"), _toggleStyle);
            
            // 显示任务区域距离
            _config.ShowQuestZoneDistance = GUILayout.Toggle(_config.ShowQuestZoneDistance, LocalizationManager.Get("UI.Toggle.ShowQuestZoneDistance"), _toggleStyle);
            
            // 任务区域指示大小
            GUILayout.BeginHorizontal();
            GUILayout.Label(LocalizationManager.Get("UI.Label.QuestZoneIndicatorSizeLabel", ("size", _config.QuestZoneIndicatorSize)), _labelStyle, GUILayout.Width(200));
            _config.QuestZoneIndicatorSize = GUILayout.HorizontalSlider(_config.QuestZoneIndicatorSize, 10f, 50f, GUILayout.Width(200));
            GUILayout.EndHorizontal();
            
            GUILayout.Space(10);
            GUILayout.Label(LocalizationManager.Get("UI.Menu.AppearanceSettings"), _labelStyle);
            GUILayout.Space(5);
            
            // 背景透明度
            GUILayout.BeginHorizontal();
            GUILayout.Label(LocalizationManager.Get("UI.Label.BackgroundAlphaLabel", ("value:F2", _config.BackgroundAlpha)), _labelStyle, GUILayout.Width(200));
            _config.BackgroundAlpha = GUILayout.HorizontalSlider(_config.BackgroundAlpha, 0f, 1f, GUILayout.Width(200));
            GUILayout.EndHorizontal();
            
            // 边框粗细
            GUILayout.BeginHorizontal();
            GUILayout.Label(LocalizationManager.Get("UI.Label.BorderThicknessLabel", ("value:F1", _config.BorderThickness)), _labelStyle, GUILayout.Width(200));
            _config.BorderThickness = GUILayout.HorizontalSlider(_config.BorderThickness, 1f, 5f, GUILayout.Width(200));
            GUILayout.EndHorizontal();
            
            GUILayout.Space(10);
            GUILayout.Label(LocalizationManager.Get("UI.Menu.EnemyESPSettings"), _labelStyle);
            GUILayout.Space(5);
            
            // 启用敌人ESP
            _config.EnableEnemyESP = GUILayout.Toggle(_config.EnableEnemyESP, LocalizationManager.Get("UI.Toggle.EnableEnemyESP"), _toggleStyle);
            
            // 最大距离
            GUILayout.BeginHorizontal();
            GUILayout.Label(LocalizationManager.Get("UI.Label.MaxEnemyDistanceLabel", ("value:F0", _config.MaxEnemyESPDistance)), _labelStyle, GUILayout.Width(200));
            _config.MaxEnemyESPDistance = GUILayout.HorizontalSlider(_config.MaxEnemyESPDistance, 50f, 500f, GUILayout.Width(200));
            GUILayout.EndHorizontal();
            
            // 显示选项
            _config.ShowEnemyHealth = GUILayout.Toggle(_config.ShowEnemyHealth, LocalizationManager.Get("UI.Toggle.ShowEnemyHealth"), _toggleStyle);
            _config.ShowEnemyWeapon = GUILayout.Toggle(_config.ShowEnemyWeapon, LocalizationManager.Get("UI.Toggle.ShowEnemyWeapon"), _toggleStyle);
            _config.ShowEnemyValue = GUILayout.Toggle(_config.ShowEnemyValue, LocalizationManager.Get("UI.Toggle.ShowEnemyValue"), _toggleStyle);
            
            // 连线设置
            _config.EnableEnemyLines = GUILayout.Toggle(_config.EnableEnemyLines, LocalizationManager.Get("UI.Toggle.EnableEnemyLines"), _toggleStyle);
            
            GUILayout.BeginHorizontal();
            GUILayout.Label(LocalizationManager.Get("UI.Label.MaxEnemyLineDistanceLabel", ("value:F0", _config.MaxEnemyLineDistance)), _labelStyle, GUILayout.Width(200));
            _config.MaxEnemyLineDistance = GUILayout.HorizontalSlider(_config.MaxEnemyLineDistance, 20f, 200f, GUILayout.Width(200));
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            GUILayout.Label(LocalizationManager.Get("UI.Label.EnemyLineWidthLabel", ("value:F1", _config.EnemyLineWidth)), _labelStyle, GUILayout.Width(200));
            _config.EnemyLineWidth = GUILayout.HorizontalSlider(_config.EnemyLineWidth, 1f, 5f, GUILayout.Width(200));
            GUILayout.EndHorizontal();
            
            // 警报设置
            GUILayout.Space(5);
            _config.EnableHighValueAlert = GUILayout.Toggle(_config.EnableHighValueAlert, LocalizationManager.Get("UI.Toggle.EnableHighValueAlert"), _toggleStyle);
            
            GUILayout.BeginHorizontal();
            GUILayout.Label(LocalizationManager.Get("UI.Label.HighValueThresholdLabel", ("value:N0", _config.HighValueThreshold)), _labelStyle, GUILayout.Width(200));
            _config.HighValueThreshold = (long)GUILayout.HorizontalSlider(_config.HighValueThreshold, 10000f, 200000f, GUILayout.Width(200));
            GUILayout.EndHorizontal();
            
            _config.EnableTraderAlert = GUILayout.Toggle(_config.EnableTraderAlert, LocalizationManager.Get("UI.Toggle.EnableTraderAlert"), _toggleStyle);
            
            GUILayout.Space(5);
            GUILayout.Label(LocalizationManager.Get("UI.Label.EnemyListHint", ("key", _config.EnemyListToggleKey)), _labelStyle);
            
            GUILayout.Space(10);
            GUILayout.Label(LocalizationManager.Get("UI.Menu.QuestItemSettings"), _labelStyle);
            GUILayout.Space(5);
            
            // 任务物品高亮
            _config.HighlightQuestItems = GUILayout.Toggle(_config.HighlightQuestItems, LocalizationManager.Get("UI.Toggle.HighlightQuestItems"), _toggleStyle);
            
            // 建筑材料高亮
            _config.HighlightBuildingMaterials = GUILayout.Toggle(_config.HighlightBuildingMaterials, LocalizationManager.Get("UI.Toggle.HighlightBuildingMaterials"), _toggleStyle);
            
            GUILayout.Space(5);
            GUILayout.Label(LocalizationManager.Get("UI.Label.QuestItemDescription"), _labelStyle);
            GUILayout.Label(LocalizationManager.Get("UI.Label.QuestItemHighlight"), _labelStyle);
            
            GUILayout.Space(10);
            GUILayout.Label(LocalizationManager.Get("UI.Menu.AimbotSettings"), _labelStyle);
            GUILayout.Space(5);
            
            // 启用自动瞄准
            _config.EnableAimbot = GUILayout.Toggle(_config.EnableAimbot, LocalizationManager.Get("UI.Toggle.EnableAimbot"), _toggleStyle);
            
            // 瞄准FOV
            GUILayout.BeginHorizontal();
            GUILayout.Label(LocalizationManager.Get("UI.Label.AimbotFOVLabel", ("value:F0", _config.AimbotFOV)), _labelStyle, GUILayout.Width(200));
            _config.AimbotFOV = GUILayout.HorizontalSlider(_config.AimbotFOV, 5f, 90f, GUILayout.Width(200));
            GUILayout.EndHorizontal();
            
            // 平滑度
            GUILayout.BeginHorizontal();
            GUILayout.Label(LocalizationManager.Get("UI.Label.AimbotSmoothnessLabel", ("value:F1", _config.AimbotSmoothness)), _labelStyle, GUILayout.Width(200));
            _config.AimbotSmoothness = GUILayout.HorizontalSlider(_config.AimbotSmoothness, 1f, 20f, GUILayout.Width(200));
            GUILayout.EndHorizontal();
            
            // 最大距离
            GUILayout.BeginHorizontal();
            GUILayout.Label(LocalizationManager.Get("UI.Label.AimbotMaxDistanceLabel", ("value:F0", _config.AimbotMaxDistance)), _labelStyle, GUILayout.Width(200));
            _config.AimbotMaxDistance = GUILayout.HorizontalSlider(_config.AimbotMaxDistance, 50f, 500f, GUILayout.Width(200));
            GUILayout.EndHorizontal();
            
            // 瞄准选项
            _config.AimbotAimAtHead = GUILayout.Toggle(_config.AimbotAimAtHead, LocalizationManager.Get("UI.Toggle.AimbotAimAtHead"), _toggleStyle);
            _config.AimbotPredictMovement = GUILayout.Toggle(_config.AimbotPredictMovement, LocalizationManager.Get("UI.Toggle.AimbotPredictMovement"), _toggleStyle);
            _config.AimbotIgnoreWalls = GUILayout.Toggle(_config.AimbotIgnoreWalls, LocalizationManager.Get("UI.Toggle.AimbotIgnoreWalls"), _toggleStyle);
            _config.AimbotIgnoreTeamCheck = GUILayout.Toggle(_config.AimbotIgnoreTeamCheck, LocalizationManager.Get("UI.Toggle.AimbotIgnoreTeamCheck"), _toggleStyle);
            
            if (_config.AimbotIgnoreTeamCheck)
            {
                GUILayout.Label(LocalizationManager.Get("UI.Warning.TestMode"), _labelStyle);
            }
            
            GUILayout.Space(10);
            GUILayout.Label(LocalizationManager.Get("UI.Menu.TriggerBotSettings"), _labelStyle);
            GUILayout.Space(5);
            
            // 启用自动扳机（提示依赖关系）
            bool originalTriggerBot = _config.EnableTriggerBot;
            _config.EnableTriggerBot = GUILayout.Toggle(_config.EnableTriggerBot, LocalizationManager.Get("UI.Toggle.EnableTriggerBot"), _toggleStyle);
            
            // 如果启用自动扳机但自动瞄准未启用，显示警告
            if (_config.EnableTriggerBot && !_config.EnableAimbot)
            {
                GUILayout.Label(LocalizationManager.Get("UI.Warning.TriggerBotRequiresAimbot"), _labelStyle);
                // 自动启用自动瞄准
                if (!originalTriggerBot && _config.EnableTriggerBot)
                {
                    _config.EnableAimbot = true;
                    GUILayout.Label(LocalizationManager.Get("UI.Warning.AimbotAutoEnabled"), _labelStyle);
                }
            }
            
            GUILayout.Space(5);
            GUILayout.Label(LocalizationManager.Get("UI.Label.TriggerBotDescription"), _labelStyle);
            GUILayout.Label(LocalizationManager.Get("UI.Label.TriggerBotRangeDesc"), _labelStyle);
            
            // 扳机延迟
            GUILayout.BeginHorizontal();
            GUILayout.Label(LocalizationManager.Get("UI.Label.TriggerBotDelayLabel", ("value:F3", _config.TriggerBotDelay)), _labelStyle, GUILayout.Width(200));
            _config.TriggerBotDelay = GUILayout.HorizontalSlider(_config.TriggerBotDelay, 0f, 0.5f, GUILayout.Width(200));
            GUILayout.EndHorizontal();
            
            // 扳机选项
            _config.TriggerBotOnlyADS = GUILayout.Toggle(_config.TriggerBotOnlyADS, LocalizationManager.Get("UI.Toggle.TriggerBotOnlyADS"), _toggleStyle);
            _config.TriggerBotTeamCheck = GUILayout.Toggle(_config.TriggerBotTeamCheck, LocalizationManager.Get("UI.Toggle.TriggerBotTeamCheck"), _toggleStyle);
            
            GUILayout.Space(5);
            GUILayout.Label(LocalizationManager.Get("UI.Warning.TriggerBotTip"), _labelStyle);
            
            GUILayout.Space(10);
            GUILayout.Label(LocalizationManager.Get("UI.Menu.NoRecoilSettings"), _labelStyle);
            GUILayout.Space(5);
            
            // 启用无后座力
            _config.EnableNoRecoil = GUILayout.Toggle(_config.EnableNoRecoil, LocalizationManager.Get("UI.Toggle.EnableNoRecoil"), _toggleStyle);
            
            GUILayout.Space(5);
            GUILayout.Label(LocalizationManager.Get("UI.Label.NoRecoilDescription"), _labelStyle);
            GUILayout.Label(LocalizationManager.Get("UI.Label.NoRecoilMechanism"), _labelStyle);
            
            GUILayout.Space(10);
            GUILayout.Label(LocalizationManager.Get("UI.Menu.CheatFunctions"), _labelStyle);
            GUILayout.Space(5);
            
            GUILayout.Label(LocalizationManager.Get("UI.Label.CheatKeysWarning"), _labelStyle);
            GUILayout.Space(3);
            
            GUILayout.Label(LocalizationManager.Get("UI.CheatKeys.GodMode"), _labelStyle);
            GUILayout.Label(LocalizationManager.Get("UI.CheatKeys.OneHitKill"), _labelStyle);
            GUILayout.Label(LocalizationManager.Get("UI.CheatKeys.SpeedBoost"), _labelStyle);
            GUILayout.Label(LocalizationManager.Get("UI.CheatKeys.InfiniteWeight"), _labelStyle);
            GUILayout.Label(LocalizationManager.Get("UI.CheatKeys.InfiniteAmmo"), _labelStyle);
            GUILayout.Label(LocalizationManager.Get("UI.CheatKeys.InfiniteStamina"), _labelStyle);
            
            GUILayout.Space(5);
            GUILayout.Label(LocalizationManager.Get("UI.Label.CheatWarning"), _labelStyle);
            
            GUILayout.EndScrollView();
            
            GUILayout.Space(10);
            
            // 按钮
            GUILayout.BeginHorizontal();
            
            if (GUILayout.Button(LocalizationManager.Get("UI.Button.SaveConfig"), _buttonStyle, GUILayout.Height(30)))
            {
                _config.Save();
            }
            
            if (GUILayout.Button(LocalizationManager.Get("UI.Button.ResetDefault"), _buttonStyle, GUILayout.Height(30)))
            {
                _config.ResetToDefault();
            }
            
            if (GUILayout.Button(LocalizationManager.Get("UI.Button.Close"), _buttonStyle, GUILayout.Height(30)))
            {
                _showMenu = false;
            }
            
            GUILayout.EndHorizontal();
            
            GUILayout.Space(5);
            GUILayout.Label(LocalizationManager.Get("UI.Label.MenuToggleHint", ("key", _config.MenuToggleKey)), _labelStyle);
            
            GUILayout.EndVertical();
            
            GUI.DragWindow(new Rect(0, 0, 10000, 20));
        }
        
        private string GetQualityName(int quality)
        {
            switch (quality)
            {
                case 0: return LocalizationManager.Get("Quality.WhiteFull");
                case 1: return LocalizationManager.Get("Quality.GreenFull");
                case 2: return LocalizationManager.Get("Quality.BlueFull");
                case 3: return LocalizationManager.Get("Quality.PurpleFull");
                case 4: return LocalizationManager.Get("Quality.OrangeFull");
                case 5: return LocalizationManager.Get("Quality.LightRedFull");
                case 6: return LocalizationManager.Get("Quality.RedFull");
                default: return LocalizationManager.Get("Quality.Unknown");
            }
        }
        
        private Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; i++)
                pix[i] = col;
                
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }
    }
}
