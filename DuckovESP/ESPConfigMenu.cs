using UnityEngine;

namespace DuckovESP
{
    /// <summary>
    /// ESP配置菜单UI
    /// </summary>
    public class ESPConfigMenu
    {
        private bool _showMenu = false;
        private Rect _menuRect = new Rect(50, 50, 450, 600);
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
                "DuckovESP 配置菜单", 
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
            
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, GUILayout.Height(520));
            
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
            GUILayout.Label($"按 {_config.MenuToggleKey} 打开/关闭此菜单", _labelStyle);
            
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
