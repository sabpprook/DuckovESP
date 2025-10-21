using UnityEngine;

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
        
        public ESPConfigMenu(ESPConfig config)
        {
            _config = config;
        }
        
        public bool IsMenuOpen => _showMenu;
        
        public void ToggleMenu()
        {
            _showMenu = !_showMenu;
            Debug.Log($"DuckovESP: 配置菜单 {(_showMenu ? "打开" : "关闭")}");
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
                "DuckovESP 配置菜单 [⚠️ 包含作弊功能]", 
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
            
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, GUILayout.Height(650)); // 增加高度以容纳敌人ESP选项
            
            // 标题
            GUILayout.Label("=== 3D ESP 设置 ===", _labelStyle);
            GUILayout.Space(5);
            
            // 3D ESP开关
            _config.Enable3DESP = GUILayout.Toggle(_config.Enable3DESP, " 启用3D ESP", _toggleStyle);
            
            // 最大距离
            GUILayout.BeginHorizontal();
            GUILayout.Label($"最大显示距离: {_config.MaxESPDistance:F0}m", _labelStyle, GUILayout.Width(200));
            _config.MaxESPDistance = GUILayout.HorizontalSlider(_config.MaxESPDistance, 10f, 500f, GUILayout.Width(200));
            GUILayout.EndHorizontal();
            
            // 字体大小
            GUILayout.BeginHorizontal();
            GUILayout.Label($"字体大小: {_config.ESPFontSize}", _labelStyle, GUILayout.Width(200));
            _config.ESPFontSize = (int)GUILayout.HorizontalSlider(_config.ESPFontSize, 8, 24, GUILayout.Width(200));
            GUILayout.EndHorizontal();
            
            // 最大显示物品数
            GUILayout.BeginHorizontal();
            GUILayout.Label($"最大显示物品数: {_config.MaxDisplayItems}", _labelStyle, GUILayout.Width(200));
            _config.MaxDisplayItems = (int)GUILayout.HorizontalSlider(_config.MaxDisplayItems, 1, 10, GUILayout.Width(200));
            GUILayout.EndHorizontal();
            
            // 显示选项
            _config.ShowDistance = GUILayout.Toggle(_config.ShowDistance, " 显示距离", _toggleStyle);
            _config.ShowConnectLine = GUILayout.Toggle(_config.ShowConnectLine, " 显示连接线", _toggleStyle);
            
            GUILayout.Space(10);
            GUILayout.Label("=== 过滤设置 ===", _labelStyle);
            GUILayout.Space(5);
            
            // 3D ESP品质过滤
            GUILayout.BeginHorizontal();
            GUILayout.Label($"3D ESP最小品质: {GetQualityName(_config.MinQualityFilter3D)}", _labelStyle, GUILayout.Width(200));
            _config.MinQualityFilter3D = (int)GUILayout.HorizontalSlider(_config.MinQualityFilter3D, 0, 6, GUILayout.Width(200));
            GUILayout.EndHorizontal();
            
            _config.ShowEmptyBoxes = GUILayout.Toggle(_config.ShowEmptyBoxes, " 显示空箱子", _toggleStyle);
            
            GUILayout.Space(10);
            GUILayout.Label("=== 小地图标记设置 ===", _labelStyle);
            GUILayout.Space(5);
            
            // 小地图标记开关
            _config.EnableMapMarkers = GUILayout.Toggle(_config.EnableMapMarkers, " 启用小地图标记", _toggleStyle);
            
            // 小地图品质过滤
            GUILayout.BeginHorizontal();
            GUILayout.Label($"地图标记最小品质: {GetQualityName(_config.MinQualityForMapMarkers)}", _labelStyle, GUILayout.Width(200));
            _config.MinQualityForMapMarkers = (int)GUILayout.HorizontalSlider(_config.MinQualityForMapMarkers, 0, 6, GUILayout.Width(200));
            GUILayout.EndHorizontal();
            
            GUILayout.Space(10);
            GUILayout.Label("=== 外观设置 ===", _labelStyle);
            GUILayout.Space(5);
            
            // 背景透明度
            GUILayout.BeginHorizontal();
            GUILayout.Label($"背景透明度: {_config.BackgroundAlpha:F2}", _labelStyle, GUILayout.Width(200));
            _config.BackgroundAlpha = GUILayout.HorizontalSlider(_config.BackgroundAlpha, 0f, 1f, GUILayout.Width(200));
            GUILayout.EndHorizontal();
            
            // 边框粗细
            GUILayout.BeginHorizontal();
            GUILayout.Label($"边框粗细: {_config.BorderThickness:F1}px", _labelStyle, GUILayout.Width(200));
            _config.BorderThickness = GUILayout.HorizontalSlider(_config.BorderThickness, 1f, 5f, GUILayout.Width(200));
            GUILayout.EndHorizontal();
            
            GUILayout.Space(10);
            GUILayout.Label("=== 🎯 敌人ESP设置 ===", _labelStyle);
            GUILayout.Space(5);
            
            // 启用敌人ESP
            _config.EnableEnemyESP = GUILayout.Toggle(_config.EnableEnemyESP, " 启用敌人ESP", _toggleStyle);
            
            // 最大距离
            GUILayout.BeginHorizontal();
            GUILayout.Label($"敌人ESP最大距离: {_config.MaxEnemyESPDistance:F0}m", _labelStyle, GUILayout.Width(200));
            _config.MaxEnemyESPDistance = GUILayout.HorizontalSlider(_config.MaxEnemyESPDistance, 50f, 500f, GUILayout.Width(200));
            GUILayout.EndHorizontal();
            
            // 显示选项
            _config.ShowEnemyHealth = GUILayout.Toggle(_config.ShowEnemyHealth, " 显示敌人血量", _toggleStyle);
            _config.ShowEnemyWeapon = GUILayout.Toggle(_config.ShowEnemyWeapon, " 显示敌人武器", _toggleStyle);
            _config.ShowEnemyValue = GUILayout.Toggle(_config.ShowEnemyValue, " 显示库存价值", _toggleStyle);
            
            // 连线设置
            _config.EnableEnemyLines = GUILayout.Toggle(_config.EnableEnemyLines, " 启用敌人连线", _toggleStyle);
            
            GUILayout.BeginHorizontal();
            GUILayout.Label($"连线最大距离: {_config.MaxEnemyLineDistance:F0}m", _labelStyle, GUILayout.Width(200));
            _config.MaxEnemyLineDistance = GUILayout.HorizontalSlider(_config.MaxEnemyLineDistance, 20f, 200f, GUILayout.Width(200));
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            GUILayout.Label($"连线粗细: {_config.EnemyLineWidth:F1}px", _labelStyle, GUILayout.Width(200));
            _config.EnemyLineWidth = GUILayout.HorizontalSlider(_config.EnemyLineWidth, 1f, 5f, GUILayout.Width(200));
            GUILayout.EndHorizontal();
            
            // 警报设置
            GUILayout.Space(5);
            _config.EnableHighValueAlert = GUILayout.Toggle(_config.EnableHighValueAlert, " 高价值目标警报", _toggleStyle);
            
            GUILayout.BeginHorizontal();
            GUILayout.Label($"高价值阈值: ¥{_config.HighValueThreshold:N0}", _labelStyle, GUILayout.Width(200));
            _config.HighValueThreshold = (long)GUILayout.HorizontalSlider(_config.HighValueThreshold, 10000f, 200000f, GUILayout.Width(200));
            GUILayout.EndHorizontal();
            
            _config.EnableTraderAlert = GUILayout.Toggle(_config.EnableTraderAlert, " 商人检测警报", _toggleStyle);
            
            GUILayout.Space(5);
            GUILayout.Label($"提示: 按 {_config.EnemyListToggleKey} 打开敌人列表窗口", _labelStyle);
            
            GUILayout.Space(10);
            GUILayout.Label("=== 📦 任务物品&建筑材料 ===", _labelStyle);
            GUILayout.Space(5);
            
            // 任务物品高亮
            _config.HighlightQuestItems = GUILayout.Toggle(_config.HighlightQuestItems, " 高亮任务物品", _toggleStyle);
            
            // 建筑材料高亮
            _config.HighlightBuildingMaterials = GUILayout.Toggle(_config.HighlightBuildingMaterials, " 高亮建筑材料", _toggleStyle);
            
            GUILayout.Space(5);
            GUILayout.Label("说明: 自动标记当前任务所需物品和未建造建筑的材料", _labelStyle);
            GUILayout.Label("使用特殊边框颜色（加粗）来突出显示", _labelStyle);
            
            GUILayout.Space(10);
            GUILayout.Label("=== ⚠️ 自动瞄准设置（Aimbot）⚠️ ===", _labelStyle);
            GUILayout.Space(5);
            
            // 启用自动瞄准
            _config.EnableAimbot = GUILayout.Toggle(_config.EnableAimbot, " 启用自动瞄准", _toggleStyle);
            
            // 瞄准FOV
            GUILayout.BeginHorizontal();
            GUILayout.Label($"瞄准视野角度: {_config.AimbotFOV:F0}°", _labelStyle, GUILayout.Width(200));
            _config.AimbotFOV = GUILayout.HorizontalSlider(_config.AimbotFOV, 5f, 90f, GUILayout.Width(200));
            GUILayout.EndHorizontal();
            
            // 平滑度
            GUILayout.BeginHorizontal();
            GUILayout.Label($"平滑度: {_config.AimbotSmoothness:F1}", _labelStyle, GUILayout.Width(200));
            _config.AimbotSmoothness = GUILayout.HorizontalSlider(_config.AimbotSmoothness, 1f, 20f, GUILayout.Width(200));
            GUILayout.EndHorizontal();
            
            // 最大距离
            GUILayout.BeginHorizontal();
            GUILayout.Label($"最大瞄准距离: {_config.AimbotMaxDistance:F0}m", _labelStyle, GUILayout.Width(200));
            _config.AimbotMaxDistance = GUILayout.HorizontalSlider(_config.AimbotMaxDistance, 50f, 500f, GUILayout.Width(200));
            GUILayout.EndHorizontal();
            
            // 瞄准选项
            _config.AimbotAimAtHead = GUILayout.Toggle(_config.AimbotAimAtHead, " 瞄准头部（否则瞄准身体）", _toggleStyle);
            _config.AimbotPredictMovement = GUILayout.Toggle(_config.AimbotPredictMovement, " 预测目标移动", _toggleStyle);
            _config.AimbotIgnoreWalls = GUILayout.Toggle(_config.AimbotIgnoreWalls, " 忽略墙壁（穿墙瞄准）", _toggleStyle);
            _config.AimbotIgnoreTeamCheck = GUILayout.Toggle(_config.AimbotIgnoreTeamCheck, " [测试] 忽略队伍检查（可攻击友军）", _toggleStyle);
            
            if (_config.AimbotIgnoreTeamCheck)
            {
                GUILayout.Label("  ⚠️ 测试模式：将攻击所有目标（包括友军）", _labelStyle);
            }
            
            GUILayout.Space(10);
            GUILayout.Label("=== ⚠️ 自动扳机设置（Trigger Bot）⚠️ ===", _labelStyle);
            GUILayout.Space(5);
            
            // 启用自动扳机（提示依赖关系）
            bool originalTriggerBot = _config.EnableTriggerBot;
            _config.EnableTriggerBot = GUILayout.Toggle(_config.EnableTriggerBot, " 启用自动扳机", _toggleStyle);
            
            // 如果启用自动扳机但自动瞄准未启用，显示警告
            if (_config.EnableTriggerBot && !_config.EnableAimbot)
            {
                GUILayout.Label("⚠️ 警告: 自动扳机需要启用自动瞄准才能工作！", _labelStyle);
                // 自动启用自动瞄准
                if (!originalTriggerBot && _config.EnableTriggerBot)
                {
                    _config.EnableAimbot = true;
                    GUILayout.Label("✓ 已自动启用自动瞄准", _labelStyle);
                }
            }
            
            GUILayout.Space(5);
            GUILayout.Label("说明: 自动扳机使用自动瞄准的目标检测", _labelStyle);
            GUILayout.Label("范围内有敌人时自动开火", _labelStyle);
            
            // 扳机延迟
            GUILayout.BeginHorizontal();
            GUILayout.Label($"触发延迟: {_config.TriggerBotDelay:F3}秒", _labelStyle, GUILayout.Width(200));
            _config.TriggerBotDelay = GUILayout.HorizontalSlider(_config.TriggerBotDelay, 0f, 0.5f, GUILayout.Width(200));
            GUILayout.EndHorizontal();
            
            // 扳机选项
            _config.TriggerBotOnlyADS = GUILayout.Toggle(_config.TriggerBotOnlyADS, " 仅在瞄准时触发", _toggleStyle);
            _config.TriggerBotTeamCheck = GUILayout.Toggle(_config.TriggerBotTeamCheck, " 检查队友（避免误伤）", _toggleStyle);
            
            GUILayout.Space(5);
            GUILayout.Label("💡 提示: TriggerBot 会自动使用子弹传送模式，无视墙体", _labelStyle);
            
            GUILayout.Space(10);
            GUILayout.Label("=== ⚠️ 无后座力设置（No Recoil）⚠️ ===", _labelStyle);
            GUILayout.Space(5);
            
            // 启用无后座力
            _config.EnableNoRecoil = GUILayout.Toggle(_config.EnableNoRecoil, " 启用无后座力", _toggleStyle);
            
            GUILayout.Space(5);
            GUILayout.Label("说明: 消除所有武器的后座力，射击时枪械不会抖动", _labelStyle);
            GUILayout.Label("工作原理: 修改角色后座力控制和武器属性", _labelStyle);
            
            GUILayout.Space(10);
            GUILayout.Label("=== 🎮 作弊功能（快捷键）===", _labelStyle);
            GUILayout.Space(5);
            
            GUILayout.Label("⚠️ 注意：所有快捷键都需要按住 Shift 键！", _labelStyle);
            GUILayout.Space(3);
            
            GUILayout.Label($"• Shift + F7 = 无敌模式（保持满血）", _labelStyle);
            GUILayout.Label($"• Shift + F8 = 一击必杀（需要自动瞄准启用）", _labelStyle);
            GUILayout.Label($"• Shift + F9 = 速度提升（{2.5f}x 跑步速度）", _labelStyle);
            GUILayout.Label($"• Shift + F10 = 无限负重（99万kg）", _labelStyle);
            GUILayout.Label($"• Shift + F11 = 无限子弹（弹匣永不减少）", _labelStyle);
            GUILayout.Label($"• Shift + F12 = 无限耐力（永不疲劳）", _labelStyle);
            
            GUILayout.Space(5);
            GUILayout.Label("⚠️ 警告: 作弊功能会影响游戏平衡性", _labelStyle);
            
            GUILayout.EndScrollView();
            
            GUILayout.Space(10);
            
            // 按钮
            GUILayout.BeginHorizontal();
            
            if (GUILayout.Button("保存配置", _buttonStyle, GUILayout.Height(30)))
            {
                _config.Save();
            }
            
            if (GUILayout.Button("重置默认", _buttonStyle, GUILayout.Height(30)))
            {
                _config.ResetToDefault();
            }
            
            if (GUILayout.Button("关闭", _buttonStyle, GUILayout.Height(30)))
            {
                _showMenu = false;
            }
            
            GUILayout.EndHorizontal();
            
            GUILayout.Space(5);
            GUILayout.Label($"按 Shift + {_config.MenuToggleKey} 打开/关闭此菜单", _labelStyle);
            
            GUILayout.EndVertical();
            
            GUI.DragWindow(new Rect(0, 0, 10000, 20));
        }
        
        private string GetQualityName(int quality)
        {
            switch (quality)
            {
                case 0: return "白色(普通)";
                case 1: return "绿色(稀有)";
                case 2: return "蓝色(精良)";
                case 3: return "紫色(史诗)";
                case 4: return "橙色(传说)";
                case 5: return "浅红(神话)";
                case 6: return "红色(至尊)";
                default: return "未知";
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
