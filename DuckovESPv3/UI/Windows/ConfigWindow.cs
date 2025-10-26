using UnityEngine;
using DuckovESPv3.Core.Configuration;
using DuckovESPv3.Infrastructure.Localization;
using System;
using System.Linq;

namespace DuckovESPv3.UI.Windows
{
    /// <summary>
    /// 配置窗口接口
    /// </summary>
    public interface IUIWindow
    {
        bool IsOpen { get; }
        void Toggle();
        void Draw();
    }

    /// <summary>
    /// ESP配置菜单窗口
    /// </summary>
    /// <remarks>
    /// 架构层级：UI/Windows
    /// 职责：
    /// - 绘制配置菜单界面
    /// - 处理用户输入
    /// - 更新配置值
    /// - 实时显示当前配置状态
    /// 性能特征：仅在 OnGUI 中绘制，避免每帧计算
    /// </remarks>
    public class ConfigWindow : IUIWindow
    {
        private bool _isOpen = false;
        private Rect _windowRect = new Rect(50, 50, 500, 700);
        private Vector2 _scrollPosition = Vector2.zero;
        private readonly ESPSystemConfig _config;
        private readonly AimbotSystemConfig? _aimbotConfig;
        private readonly MeleeAutoAttackConfig? _meleeAutoAttackConfig;
        private readonly UI.Rendering.GUIStyleManager _styleManager;
        private float _uiScale = 1f;
        
        // ConfigurationManager引用（用于保存配置）
        private ConfigurationManager? _configurationManager;
        
        // ESP系统管理器引用（用于刷新标记）
        private Core.Systems.ESP.ESPSystemManager? _espSystemManager;
        
        // 小地图标记系统引用（用于配置变更刷新）
        private Core.Systems.ESP.Minimap.MinimapMarkerSystem? _minimapMarkerSystem;
        
        // 缓存过滤配置的旧值，用于检测变更
        private int _lastMinQuality;
        private bool _lastShowByValue;
        private bool _lastShowWishlisted;
        private bool _lastShowQuest;
        private bool _lastShowBuilding;
        private bool _lastShowEmptyBoxes;
        private bool _lastShowNotWishlistedKeys;
        private int _lastFontSize;
        
        // 缓存小地图标记配置，用于检测变更
        private int _lastMinQualityMapMarkers;
        
        // 缓存ESP开关状态，用于检测变更
        private bool _lastEnable3DESP;
        private bool _lastEnableLootESP;  // 新增：物资ESP开关
        private bool _lastEnableEnemyESP;
        
        // 缓存敌人ESP详细配置，用于检测变更
        private bool _lastShowEnemyHealth;
        private bool _lastShowEnemyWeapon;
        private bool _lastShowEnemyValue;
        private bool _lastEnableEnemyLines;
        
        // 缓存距离配置，用于检测变更
        private float _lastMaxEnemyESPDistance;

        public bool IsOpen => _isOpen;

        /// <summary>
        /// 获取或设置UI缩放比例 (0.5f - 2f)
        /// </summary>
        public float UIScale
        {
            get => _uiScale;
            set
            {
                float newScale = Mathf.Clamp(value, 0.5f, 2f);
                if (!Mathf.Approximately(_uiScale, newScale))
                {
                    _uiScale = newScale;
                    _styleManager.UIScale = newScale;
                    
                    // 同步 ESP 文字大小
                    if (_espSystemManager != null)
                    {
                        _espSystemManager.SetUIScale(newScale);
                    }
                }
            }
        }

        public ConfigWindow(ESPSystemConfig config, UI.Rendering.GUIStyleManager styleManager, AimbotSystemConfig? aimbotConfig = null, MeleeAutoAttackConfig? meleeAutoAttackConfig = null)
        {
            _config = config;
            _aimbotConfig = aimbotConfig;
            _meleeAutoAttackConfig = meleeAutoAttackConfig;
            _styleManager = styleManager;
            _styleManager.Initialize();
            
            // 从配置加载UIScale
            _uiScale = Mathf.Clamp(config.UIScale, 0.5f, 2f);
            _styleManager.UIScale = _uiScale;
            
            // 初始化缓存的过滤配置
            _lastMinQuality = config.MinQualityFilter3D;
            _lastShowByValue = config.ShowByValue;
            _lastShowWishlisted = config.ShowWishlistedItems;
            _lastShowQuest = config.ShowQuestItems;
            _lastShowBuilding = config.ShowBuildingMaterials;
            _lastShowEmptyBoxes = config.ShowEmptyBoxes;
            _lastShowNotWishlistedKeys = config.ShowNotWishlistedKeys;
            _lastFontSize = config.ESPFontSize;
            
            // 初始化缓存的小地图标记配置
            _lastMinQualityMapMarkers = config.MinQualityForMapMarkers;
            
            // 初始化缓存的ESP开关状态
            _lastEnable3DESP = config.Enable3DESP;
            _lastEnableLootESP = config.EnableLootESP;
            _lastEnableEnemyESP = config.EnableEnemyESP;
            
            // 初始化敌人ESP详细配置缓存
            _lastShowEnemyHealth = config.ShowEnemyHealth;
            _lastShowEnemyWeapon = config.ShowEnemyWeapon;
            _lastShowEnemyValue = config.ShowEnemyValue;
            _lastEnableEnemyLines = config.EnableEnemyLines;
            
            // 初始化距离配置缓存
            _lastMaxEnemyESPDistance = config.MaxEnemyESPDistance;
        }

        /// <summary>
        /// 设置ESP系统管理器引用
        /// </summary>
        public void SetESPSystemManager(Core.Systems.ESP.ESPSystemManager espSystemManager)
        {
            _espSystemManager = espSystemManager;
        }

        /// <summary>
        /// 设置小地图标记系统引用
        /// </summary>
        public void SetMinimapMarkerSystem(Core.Systems.ESP.Minimap.MinimapMarkerSystem minimapMarkerSystem)
        {
            _minimapMarkerSystem = minimapMarkerSystem;
        }

        /// <summary>
        /// 设置ConfigurationManager引用
        /// </summary>
        public void SetConfigurationManager(ConfigurationManager configurationManager)
        {
            _configurationManager = configurationManager;
        }

        /// <summary>
        /// 切换窗口显示/隐藏
        /// </summary>
        public void Toggle()
        {
            _isOpen = !_isOpen;
        }

        /// <summary>
        /// 绘制窗口
        /// </summary>
        public void Draw()
        {
            if (!_isOpen)
                return;

            // 支持 Ctrl+鼠标滚轮来调整UI缩放
            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            {
                float scrollDelta = Input.GetAxis("Mouse ScrollWheel");
                if (!Mathf.Approximately(scrollDelta, 0))
                {
                    UIScale += scrollDelta * 0.2f;
                }
            }

            // 开始 GUI 缩放
            GUI.matrix = Matrix4x4.Scale(new Vector3(_uiScale, _uiScale, 1f));

            _windowRect = GUILayout.Window(
                9999,
                _windowRect,
                DrawWindowContent,
                LocalizationManager.Get("UI.Menu.Title", ("title", "DuckovESP v3")),
                _styleManager.WindowStyle
            );

            // 拦截鼠标事件，避免穿透到游戏
            // 注意：需要在GUI.matrix恢复前检测，因为windowRect是在缩放坐标系中
            var scaledMousePos = Event.current.mousePosition / _uiScale;
            if (_windowRect.Contains(scaledMousePos))
            {
                // 消费所有鼠标事件
                if (Event.current.type == EventType.MouseDown || 
                    Event.current.type == EventType.MouseUp || 
                    Event.current.type == EventType.MouseDrag ||
                    Event.current.type == EventType.ScrollWheel)
                {
                    Event.current.Use();
                }
            }

            // 恢复 GUI 矩阵
            GUI.matrix = Matrix4x4.identity;
        }

        /// <summary>
        /// 绘制窗口内容
        /// </summary>
        private void DrawWindowContent(int windowID)
        {
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, GUILayout.Height(650));

            // 标题
            GUILayout.Label(LocalizationManager.Get("UI.Menu.Title"), _styleManager.TitleLabelStyle);
            GUILayout.Space(10);

            // ==================== 基础设置 ====================
            GUILayout.Label(LocalizationManager.Get("UI.Section.BasicSettings"), _styleManager.TitleLabelStyle);
            GUILayout.Space(5);

            // 语言选择
            GUILayout.BeginHorizontal();
            GUILayout.Label(LocalizationManager.Get("UI.Label.Language"), _styleManager.LabelStyle, GUILayout.Width(150));
            if (GUILayout.Button(_config.Language, _styleManager.ButtonStyle, GUILayout.Width(100)))
            {
                // 循环切换语言
                _config.Language = _config.Language switch
                {
                    "zh-CN" => "en-US",
                    "en-US" => "de-DE",
                    "de-DE" => "zh-CN",
                    _ => "zh-CN"
                };
                LocalizationManager.SetLanguage(_config.Language);
            }
            GUILayout.EndHorizontal();

            // 调试模式
            GUILayout.BeginHorizontal();
            _config.DebugEnabled = GUILayout.Toggle(_config.DebugEnabled, LocalizationManager.Get("UI.Toggle.DebugMode"), _styleManager.ToggleStyle);
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            // ==================== 3D ESP设置 ====================
            GUILayout.Label(LocalizationManager.Get("UI.Section.ESP3D"), _styleManager.TitleLabelStyle);
            GUILayout.Space(5);

            // 启用3D ESP（实时生效）
            bool lastEnable3DESP = _config.Enable3DESP;
            _config.Enable3DESP = GUILayout.Toggle(_config.Enable3DESP, LocalizationManager.Get("UI.Toggle.Enable3DESPWithNote"), _styleManager.ToggleStyle);
            
            // 检测变化并实时生效
            if (_config.Enable3DESP != lastEnable3DESP && _espSystemManager != null)
            {
                if (_config.Enable3DESP)
                {
                    _espSystemManager.RefreshAllMarkers();
                }
                else
                {
                    _espSystemManager.ClearAllMarkers();
                }
            }

            if (_config.Enable3DESP)
            {
            // 渲染模式选择（通用）
            GUILayout.BeginVertical(GUI.skin.box);
            GUILayout.Label(LocalizationManager.Get("UI.RenderMode.Title"), _styleManager.LabelStyle);
            GUILayout.Space(5);
            
            int newRenderMode = -1;
            
            // 世界空间模式
            GUILayout.BeginHorizontal();
            bool worldSpaceSelected = GUILayout.Toggle(_config.ESPRenderMode == 0, "", _styleManager.ToggleStyle, GUILayout.Width(20));
            if (worldSpaceSelected && _config.ESPRenderMode != 0) newRenderMode = 0;
            
            GUILayout.BeginVertical();
            GUILayout.Label(LocalizationManager.Get("UI.RenderMode.WorldSpace"), _styleManager.LabelStyle);
            GUILayout.Label(LocalizationManager.Get("UI.RenderMode.HighFPS"), _styleManager.LabelStyle);
            GUILayout.Label(LocalizationManager.Get("UI.Label.OccludedWarning"), _styleManager.LabelStyle);
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            
            GUILayout.Space(3);
            
            // 屏幕空间模式
            GUILayout.BeginHorizontal();
            bool screenSpaceSelected = GUILayout.Toggle(_config.ESPRenderMode == 1, "", _styleManager.ToggleStyle, GUILayout.Width(20));
            if (screenSpaceSelected && _config.ESPRenderMode != 1) newRenderMode = 1;
            
            GUILayout.BeginVertical();
            GUILayout.Label(LocalizationManager.Get("UI.RenderMode.ScreenSpace"), _styleManager.LabelStyle);
            GUILayout.Label(LocalizationManager.Get("UI.RenderMode.NeverOccluded"), _styleManager.LabelStyle);
            GUILayout.Label(LocalizationManager.Get("UI.RenderMode.PerformanceHit"), _styleManager.LabelStyle);
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            
            // 如果模式改变，立即刷新
            if (newRenderMode != -1 && newRenderMode != _config.ESPRenderMode)
            {
                _config.ESPRenderMode = newRenderMode;
                if (_espSystemManager != null)
                {
                    _espSystemManager.ClearAllMarkers();
                    _espSystemManager.RefreshAllMarkers();
                }
            }
            
            GUILayout.EndVertical();
            GUILayout.Space(10);

            // ==================== 物资ESP ====================
            GUILayout.BeginVertical(GUI.skin.box);
            _config.EnableLootESP = GUILayout.Toggle(_config.EnableLootESP, LocalizationManager.Get("UI.Label.LootESPTitle"), _styleManager.ToggleStyle);
            
            if (_config.EnableLootESP)
            {
                GUILayout.Space(5);
                
                // 最大显示距离
                GUILayout.BeginHorizontal();
                GUILayout.Label(LocalizationManager.Get("UI.Label.MaxDistance", ("value", $"{_config.MaxESPDistance:F0}")), _styleManager.LabelStyle, GUILayout.Width(200));
                float lastMaxDistance = _config.MaxESPDistance;
                _config.MaxESPDistance = GUILayout.HorizontalSlider(_config.MaxESPDistance, 10f, 500f, GUILayout.Width(250));
                
                // 检测变化并实时更新
                if (!Mathf.Approximately(lastMaxDistance, _config.MaxESPDistance) && _espSystemManager != null)
                {
                    _espSystemManager.UpdateAllMarkersMaxDistance(_config.MaxESPDistance);
                }
                GUILayout.EndHorizontal();
                
                // 显示距离和连接线（物资专用）
                bool lastShowDistance = _config.ShowDistance;
                _config.ShowDistance = GUILayout.Toggle(_config.ShowDistance, LocalizationManager.Get("UI.Toggle.ShowDistance"), _styleManager.ToggleStyle);
                
                bool lastShowConnectLine = _config.ShowConnectLine;
                _config.ShowConnectLine = GUILayout.Toggle(_config.ShowConnectLine, LocalizationManager.Get("UI.Toggle.ShowConnectLine"), _styleManager.ToggleStyle);
                
                // 检测变化并实时更新
                if ((lastShowDistance != _config.ShowDistance || lastShowConnectLine != _config.ShowConnectLine) && _espSystemManager != null)
                {
                    _espSystemManager.UpdateAllMarkersDisplayOptions(_config.ShowConnectLine, _config.ShowDistance);
                }
            }
            GUILayout.EndVertical();
            GUILayout.Space(10);

            // ==================== 敌人ESP ====================
            GUILayout.BeginVertical(GUI.skin.box);
            _config.EnableEnemyESP = GUILayout.Toggle(_config.EnableEnemyESP, LocalizationManager.Get("UI.Label.EnemyESPTitle"), _styleManager.ToggleStyle);
            
            if (_config.EnableEnemyESP)
            {
                GUILayout.Space(5);
                
                // 敌人专用距离设置
                GUILayout.BeginHorizontal();
                GUILayout.Label(LocalizationManager.Get("UI.Label.MaxEnemyDistance", ("value", $"{_config.MaxEnemyESPDistance:F0}")), _styleManager.LabelStyle, GUILayout.Width(200));
                _config.MaxEnemyESPDistance = GUILayout.HorizontalSlider(_config.MaxEnemyESPDistance, 50f, 500f, GUILayout.Width(250));
                GUILayout.EndHorizontal();
                
                // 敌人连接线（独立于物资连接线）
                _config.EnableEnemyLines = GUILayout.Toggle(_config.EnableEnemyLines, LocalizationManager.Get("UI.Toggle.ShowEnemyLines"), _styleManager.ToggleStyle);
                
                // 敌人信息显示配置
                GUILayout.Label(LocalizationManager.Get("UI.Label.DisplayInfo"), _styleManager.LabelStyle);
                _config.ShowEnemyHealth = GUILayout.Toggle(_config.ShowEnemyHealth, LocalizationManager.Get("UI.Toggle.ShowEnemyHealth"), _styleManager.ToggleStyle);
                _config.ShowEnemyWeapon = GUILayout.Toggle(_config.ShowEnemyWeapon, LocalizationManager.Get("UI.Toggle.ShowEnemyWeapon"), _styleManager.ToggleStyle);
                _config.ShowEnemyValue = GUILayout.Toggle(_config.ShowEnemyValue, LocalizationManager.Get("UI.Toggle.ShowEnemyValue"), _styleManager.ToggleStyle);
            }
            GUILayout.EndVertical();
            GUILayout.Space(10);

            // 字体大小（通用）
            GUILayout.BeginHorizontal();
            GUILayout.Label(LocalizationManager.Get("UI.Label.FontSize", ("value", _config.ESPFontSize.ToString())), _styleManager.LabelStyle, GUILayout.Width(200));
            _config.ESPFontSize = (int)GUILayout.HorizontalSlider(_config.ESPFontSize, 8, 24, GUILayout.Width(250));
            GUILayout.EndHorizontal();

            // 显示距离（通用，已移到物资ESP内）
            // 显示连接线（通用，已移到物资ESP内）
            }

            GUILayout.Space(10);

            // ==================== 小地图标记设置 ====================
            GUILayout.Label(LocalizationManager.Get("UI.Section.MapMarkers"), _styleManager.TitleLabelStyle);
            GUILayout.Space(5);

            _config.EnableMapMarkers = GUILayout.Toggle(_config.EnableMapMarkers, LocalizationManager.Get("UI.Toggle.EnableMapMarkers"), _styleManager.ToggleStyle);

            if (_config.EnableMapMarkers)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(LocalizationManager.Get("UI.Label.MinQualityMapMarkers", ("value", _config.MinQualityForMapMarkers.ToString())), _styleManager.LabelStyle, GUILayout.Width(200));
                int newMinQualityMapMarkers = (int)GUILayout.HorizontalSlider(_config.MinQualityForMapMarkers, 0, 6, GUILayout.Width(250));
                
                // 检测小地图品质过滤变更
                if (newMinQualityMapMarkers != _lastMinQualityMapMarkers)
                {
                    _config.MinQualityForMapMarkers = newMinQualityMapMarkers;
                    _lastMinQualityMapMarkers = newMinQualityMapMarkers;
                    
                    // 触发小地图标记刷新
                    if (_minimapMarkerSystem != null && _minimapMarkerSystem.IsInitialized)
                    {
                        _minimapMarkerSystem.RefreshAllMarkers();
                        Debug.Log($"[ConfigWindow] 小地图品质过滤变更: {newMinQualityMapMarkers}，已触发标记刷新");
                    }
                }
                
                GUILayout.EndHorizontal();
                
                // Quest任务地点标记
                GUILayout.Space(5);
                _config.ShowQuestZones = GUILayout.Toggle(_config.ShowQuestZones, LocalizationManager.Get("UI.Toggle.ShowQuestZones"), _styleManager.ToggleStyle);
                
                if (_config.ShowQuestZones)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(LocalizationManager.Get("UI.Label.QuestZoneMaxDistance", ("value", $"{_config.QuestZoneMaxDistance:F0}")), _styleManager.LabelStyle, GUILayout.Width(200));
                    _config.QuestZoneMaxDistance = GUILayout.HorizontalSlider(_config.QuestZoneMaxDistance, 50f, 1000f, GUILayout.Width(250));
                    GUILayout.EndHorizontal();
                    
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(LocalizationManager.Get("UI.Label.QuestZoneColor"), _styleManager.LabelStyle, GUILayout.Width(100));
                    var questZoneColor = _config.QuestZoneColor;
                    questZoneColor.r = GUILayout.HorizontalSlider(questZoneColor.r, 0f, 1f, GUILayout.Width(50));
                    questZoneColor.g = GUILayout.HorizontalSlider(questZoneColor.g, 0f, 1f, GUILayout.Width(50));
                    questZoneColor.b = GUILayout.HorizontalSlider(questZoneColor.b, 0f, 1f, GUILayout.Width(50));
                    _config.QuestZoneColor = questZoneColor;
                    GUILayout.EndHorizontal();
                }
                
                // 撤离点指示器
                GUILayout.Space(5);
                _config.ShowEvacuationPoints = GUILayout.Toggle(_config.ShowEvacuationPoints, LocalizationManager.Get("UI.Toggle.ShowEvacuationPoints"), _styleManager.ToggleStyle);
            }

            GUILayout.Space(10);

            // ==================== 过滤设置 ====================
            GUILayout.Label(LocalizationManager.Get("UI.Section.Filters"), _styleManager.TitleLabelStyle);
            GUILayout.Space(5);

            GUILayout.Label(LocalizationManager.Get("UI.Label.BasicFilters"), _styleManager.LabelStyle);
            
            GUILayout.BeginHorizontal();
            GUILayout.Label(LocalizationManager.Get("UI.Label.MinQuality3D", ("value", _config.MinQualityFilter3D.ToString())), _styleManager.LabelStyle, GUILayout.Width(200));
            _config.MinQualityFilter3D = (int)GUILayout.HorizontalSlider(_config.MinQualityFilter3D, 0, 6, GUILayout.Width(250));
            GUILayout.EndHorizontal();

            _config.ShowEmptyBoxes = GUILayout.Toggle(_config.ShowEmptyBoxes, LocalizationManager.Get("UI.Toggle.ShowEmptyBoxes"), _styleManager.ToggleStyle);

            GUILayout.Space(5);
            GUILayout.Label(LocalizationManager.Get("UI.Label.DisplayConditions"), _styleManager.LabelStyle);
            
            _config.ShowByValue = GUILayout.Toggle(_config.ShowByValue, LocalizationManager.Get("UI.Toggle.ShowByValue"), _styleManager.ToggleStyle);
            _config.ShowWishlistedItems = GUILayout.Toggle(_config.ShowWishlistedItems, LocalizationManager.Get("UI.Toggle.ShowWishlistedItems"), _styleManager.ToggleStyle);
            _config.ShowQuestItems = GUILayout.Toggle(_config.ShowQuestItems, LocalizationManager.Get("UI.Toggle.ShowQuestItems"), _styleManager.ToggleStyle);
            _config.ShowBuildingMaterials = GUILayout.Toggle(_config.ShowBuildingMaterials, LocalizationManager.Get("UI.Toggle.ShowBuildingMaterials"), _styleManager.ToggleStyle);
            
            // Quest物品颜色配置
            if (_config.ShowQuestItems)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(LocalizationManager.Get("UI.Label.QuestItemColor"), _styleManager.LabelStyle, GUILayout.Width(100));
                var questItemColor = _config.QuestItemColor;
                questItemColor.r = GUILayout.HorizontalSlider(questItemColor.r, 0f, 1f, GUILayout.Width(50));
                questItemColor.g = GUILayout.HorizontalSlider(questItemColor.g, 0f, 1f, GUILayout.Width(50));
                questItemColor.b = GUILayout.HorizontalSlider(questItemColor.b, 0f, 1f, GUILayout.Width(50));
                _config.QuestItemColor = questItemColor;
                string rgbText = LocalizationManager.Get("UI.Label.RGBFormat", ("r", questItemColor.r.ToString("F2")), ("g", questItemColor.g.ToString("F2")), ("b", questItemColor.b.ToString("F2")));
                GUILayout.Label(rgbText, _styleManager.LabelStyle);
                GUILayout.EndHorizontal();
            }
            
            GUILayout.Space(5);
            GUILayout.Label(LocalizationManager.Get("UI.Label.KeyFilters"), _styleManager.LabelStyle);
            _config.ShowNotWishlistedKeys = GUILayout.Toggle(_config.ShowNotWishlistedKeys, LocalizationManager.Get("UI.Toggle.ShowUnregisteredKeys"), _styleManager.ToggleStyle);

            GUILayout.Space(10);

            // ==================== 自瞄系统设置 ====================
            if (_aimbotConfig != null)
            {
                GUILayout.Label(LocalizationManager.Get("UI.Aimbot.Title"), _styleManager.TitleLabelStyle);
                GUILayout.Space(5);

                // 自动瞄准
                _aimbotConfig.EnableAimbot = GUILayout.Toggle(_aimbotConfig.EnableAimbot, LocalizationManager.Get("UI.Aimbot.EnableAimbot"), _styleManager.ToggleStyle);

                if (_aimbotConfig.EnableAimbot)
                {
                    GUILayout.BeginVertical(GUI.skin.box);
                    
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(LocalizationManager.Get("UI.Aimbot.AimbotFOV", ("value", _aimbotConfig.AimbotFOV.ToString("F0"))), _styleManager.LabelStyle, GUILayout.Width(200));
                    _aimbotConfig.AimbotFOV = GUILayout.HorizontalSlider(_aimbotConfig.AimbotFOV, 100f, 1000f, GUILayout.Width(250));
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label(LocalizationManager.Get("UI.Aimbot.AimbotMaxDistance", ("value", _aimbotConfig.AimbotMaxDistance.ToString("F0"))), _styleManager.LabelStyle, GUILayout.Width(200));
                    _aimbotConfig.AimbotMaxDistance = GUILayout.HorizontalSlider(_aimbotConfig.AimbotMaxDistance, 50f, 500f, GUILayout.Width(250));
                    GUILayout.EndHorizontal();

                    _aimbotConfig.AimbotAimAtHead = GUILayout.Toggle(_aimbotConfig.AimbotAimAtHead, LocalizationManager.Get("UI.Aimbot.AimbotAimAtHead"), _styleManager.ToggleStyle);
                    _aimbotConfig.AimbotIgnoreWalls = GUILayout.Toggle(_aimbotConfig.AimbotIgnoreWalls, LocalizationManager.Get("UI.Aimbot.AimbotIgnoreWalls"), _styleManager.ToggleStyle);
                    _aimbotConfig.AimbotIgnoreTeamCheck = GUILayout.Toggle(_aimbotConfig.AimbotIgnoreTeamCheck, LocalizationManager.Get("UI.Aimbot.AimbotIgnoreTeamCheck"), _styleManager.ToggleStyle);
                    
                    GUILayout.EndVertical();
                }

                GUILayout.Space(5);

                // 自动扳机
                _aimbotConfig.EnableTriggerBot = GUILayout.Toggle(_aimbotConfig.EnableTriggerBot, LocalizationManager.Get("UI.Aimbot.EnableTriggerBot"), _styleManager.ToggleStyle);

                if (_aimbotConfig.EnableTriggerBot)
                {
                    GUILayout.BeginVertical(GUI.skin.box);
                    
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(LocalizationManager.Get("UI.Aimbot.TriggerBotDelay", ("value", _aimbotConfig.TriggerBotDelay.ToString("F2"))), _styleManager.LabelStyle, GUILayout.Width(200));
                    _aimbotConfig.TriggerBotDelay = GUILayout.HorizontalSlider(_aimbotConfig.TriggerBotDelay, 0f, 0.5f, GUILayout.Width(250));
                    GUILayout.EndHorizontal();

                    _aimbotConfig.TriggerBotOnlyADS = GUILayout.Toggle(_aimbotConfig.TriggerBotOnlyADS, LocalizationManager.Get("UI.Aimbot.TriggerBotOnlyADS"), _styleManager.ToggleStyle);
                    
                    GUILayout.EndVertical();
                }

                GUILayout.Space(5);

                // 无后座力
                _aimbotConfig.EnableNoRecoil = GUILayout.Toggle(_aimbotConfig.EnableNoRecoil, LocalizationManager.Get("UI.Aimbot.EnableNoRecoil"), _styleManager.ToggleStyle);
            }

            GUILayout.Space(10);

            // ==================== 刀自动攻击设置 ====================
            if (_meleeAutoAttackConfig != null)
            {
                GUILayout.Label(LocalizationManager.Get("UI.MeleeAutoAttack.Title"), _styleManager.TitleLabelStyle);
                GUILayout.Space(5);

                // 启用刀自动攻击
                _meleeAutoAttackConfig.Enable = GUILayout.Toggle(_meleeAutoAttackConfig.Enable, LocalizationManager.Get("UI.MeleeAutoAttack.Enable"), _styleManager.ToggleStyle);

                if (_meleeAutoAttackConfig.Enable)
                {
                    GUILayout.BeginVertical(GUI.skin.box);
                    
                    GUILayout.Label(LocalizationManager.Get("UI.MeleeAutoAttack.Description1"), _styleManager.LabelStyle);
                    GUILayout.Label(LocalizationManager.Get("UI.MeleeAutoAttack.Description2"), _styleManager.LabelStyle);
                    GUILayout.Space(5);

                    _meleeAutoAttackConfig.RequireEnemyAiming = GUILayout.Toggle(_meleeAutoAttackConfig.RequireEnemyAiming, LocalizationManager.Get("UI.MeleeAutoAttack.RequireEnemyAiming"), _styleManager.ToggleStyle);
                    
                    GUILayout.EndVertical();
                }
            }

            GUILayout.Space(10);

            // ==================== 显示设置 ====================
            GUILayout.Label(LocalizationManager.Get("UI.Section.Display"), _styleManager.TitleLabelStyle);
            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.Label(LocalizationManager.Get("UI.Label.BackgroundAlpha", ("value", $"{_config.BackgroundAlpha:F2}")), _styleManager.LabelStyle, GUILayout.Width(200));
            _config.BackgroundAlpha = GUILayout.HorizontalSlider(_config.BackgroundAlpha, 0f, 1f, GUILayout.Width(250));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(LocalizationManager.Get("UI.Label.BorderThickness", ("value", $"{_config.BorderThickness:F1}")), _styleManager.LabelStyle, GUILayout.Width(200));
            _config.BorderThickness = GUILayout.HorizontalSlider(_config.BorderThickness, 1f, 5f, GUILayout.Width(250));
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            // ==================== 按钮操作 ====================
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(LocalizationManager.Get("UI.Button.Save"), _styleManager.ButtonStyle, GUILayout.Width(100), GUILayout.Height(30)))
            {
                // 同步UIScale到配置
                _config.UIScale = _uiScale;
                
                // 保存配置到文件
                if (_configurationManager != null)
                {
                    // 保存ESP系统配置
                    _configurationManager.SetFromInstance(_config);
                    
                    // 保存自瞄系统配置
                    if (_aimbotConfig != null)
                    {
                        _configurationManager.SetFromInstance(_aimbotConfig);
                        Debug.Log("[ConfigWindow] 自瞄配置已保存");
                    }
                    
                    // 保存刀自动攻击配置
                    if (_meleeAutoAttackConfig != null)
                    {
                        _configurationManager.SetFromInstance(_meleeAutoAttackConfig);
                        Debug.Log("[ConfigWindow] 刀自瞄配置已保存");
                    }
                    
                    _configurationManager.Save();
                    Debug.Log("[ConfigWindow] 所有配置已保存到文件");
                }
                else
                {
                    Debug.LogWarning("[ConfigWindow] ConfigurationManager引用为空，无法保存配置");
                }
            }

            if (GUILayout.Button(LocalizationManager.Get("UI.Button.Reset"), _styleManager.ButtonStyle, GUILayout.Width(100), GUILayout.Height(30)))
            {
                // 重置配置为默认值
                if (_configurationManager != null)
                {
                    _configurationManager.ResetToDefault();
                    // 重新加载配置到_config实例
                    var newConfig = _configurationManager.GetConfigInstance<ESPSystemConfig>();
                    // 复制所有属性
                    typeof(ESPSystemConfig).GetProperties().ToList().ForEach(prop =>
                    {
                        if (prop.CanWrite)
                        {
                            prop.SetValue(_config, prop.GetValue(newConfig));
                        }
                    });
                    
                    // 同步UIScale
                    UIScale = _config.UIScale;
                    
                    // 刷新ESP标记
                    if (_espSystemManager != null)
                    {
                        _espSystemManager.ClearAllMarkers();
                        _espSystemManager.RefreshAllMarkers();
                    }
                    
                    Debug.Log("[ConfigWindow] 配置已重置为默认值");
                }
            }

            if (GUILayout.Button(LocalizationManager.Get("UI.Button.Close"), _styleManager.ButtonStyle, GUILayout.Width(100), GUILayout.Height(30)))
            {
                _isOpen = false;
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            
            // UI 缩放提示
            GUILayout.Label(LocalizationManager.Get("UI.Common.UIScaleTip", ("scale", _uiScale.ToString("F1"))), _styleManager.LabelStyle);

            GUILayout.EndScrollView();

            // 检测过滤配置变更并触发刷新
            CheckAndRefreshFilters();

            // 窗口拖拽
            GUI.DragWindow(new Rect(0, 0, _windowRect.width, 20));
        }

        /// <summary>
        /// 检测过滤配置变更并触发刷新
        /// </summary>
        private void CheckAndRefreshFilters()
        {
            bool filterChanged = false;
            
            if (_config.MinQualityFilter3D != _lastMinQuality)
            {
                _lastMinQuality = _config.MinQualityFilter3D;
                filterChanged = true;
            }
            
            if (_config.ShowByValue != _lastShowByValue)
            {
                _lastShowByValue = _config.ShowByValue;
                filterChanged = true;
            }
            
            if (_config.ShowWishlistedItems != _lastShowWishlisted)
            {
                _lastShowWishlisted = _config.ShowWishlistedItems;
                filterChanged = true;
            }
            
            if (_config.ShowQuestItems != _lastShowQuest)
            {
                _lastShowQuest = _config.ShowQuestItems;
                filterChanged = true;
            }
            
            if (_config.ShowBuildingMaterials != _lastShowBuilding)
            {
                _lastShowBuilding = _config.ShowBuildingMaterials;
                filterChanged = true;
            }
            
            if (_config.ShowEmptyBoxes != _lastShowEmptyBoxes)
            {
                _lastShowEmptyBoxes = _config.ShowEmptyBoxes;
                filterChanged = true;
            }
            
            if (_config.ShowNotWishlistedKeys != _lastShowNotWishlistedKeys)
            {
                _lastShowNotWishlistedKeys = _config.ShowNotWishlistedKeys;
                filterChanged = true;
            }
            
            // 检查ESP总开关变更
            if (_config.Enable3DESP != _lastEnable3DESP)
            {
                _lastEnable3DESP = _config.Enable3DESP;
                filterChanged = true;
            }
            
            // 检查物资ESP开关变更
            if (_config.EnableLootESP != _lastEnableLootESP)
            {
                _lastEnableLootESP = _config.EnableLootESP;
                filterChanged = true;
            }
            
            // 检查敌人ESP开关变更
            if (_config.EnableEnemyESP != _lastEnableEnemyESP)
            {
                _lastEnableEnemyESP = _config.EnableEnemyESP;
                filterChanged = true;
            }
            
            // 检查敌人ESP详细配置变更
            bool enemyDetailsChanged = false;
            
            if (_config.ShowEnemyHealth != _lastShowEnemyHealth)
            {
                _lastShowEnemyHealth = _config.ShowEnemyHealth;
                enemyDetailsChanged = true;
            }
            
            if (_config.ShowEnemyWeapon != _lastShowEnemyWeapon)
            {
                _lastShowEnemyWeapon = _config.ShowEnemyWeapon;
                enemyDetailsChanged = true;
            }
            
            if (_config.ShowEnemyValue != _lastShowEnemyValue)
            {
                _lastShowEnemyValue = _config.ShowEnemyValue;
                enemyDetailsChanged = true;
            }
            
            if (_config.EnableEnemyLines != _lastEnableEnemyLines)
            {
                _lastEnableEnemyLines = _config.EnableEnemyLines;
                enemyDetailsChanged = true;
            }
            
            // 检查敌人ESP距离变更
            if (Math.Abs(_config.MaxEnemyESPDistance - _lastMaxEnemyESPDistance) > 0.1f)
            {
                _lastMaxEnemyESPDistance = _config.MaxEnemyESPDistance;
                // 更新所有敌人标记的距离
                if (_espSystemManager != null)
                {
                    _espSystemManager.UpdateEnemyMarkersDistance(_config.MaxEnemyESPDistance);
                }
            }
            
            // 如果敌人ESP详细配置变更，需要更新所有敌人标记的文本
            if (enemyDetailsChanged && _espSystemManager != null)
            {
                _espSystemManager.RefreshAllMarkers();
            }
            
            // 检查字体大小变更
            if (_config.ESPFontSize != _lastFontSize)
            {
                _lastFontSize = _config.ESPFontSize;
                if (_espSystemManager != null)
                {
                    _espSystemManager.UpdateFontSize(_config.ESPFontSize);
                }
            }
            
            // 如果过滤配置变更，触发刷新
            if (filterChanged && _espSystemManager != null)
            {
                _espSystemManager.RefreshAllMarkers();
            }
        }

        /// <summary>
        /// 获取品质等级名称
        /// </summary>
        private string GetQualityName(int quality)
        {
            return quality switch
            {
                0 => "白色",
                1 => "绿色",
                2 => "蓝色",
                3 => "紫色",
                4 => "金色",
                5 => "红色",
                6 => "彩色",
                _ => "未知"
            };
        }
    }
}
