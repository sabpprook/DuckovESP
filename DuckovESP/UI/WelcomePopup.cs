using System;
using UnityEngine;

namespace DuckovESP.UI
{
    /// <summary>
    /// DuckovESP 欢迎界面弹窗
    /// </summary>
    public class WelcomePopup
    {
        private Rect _windowRect;
        private bool _isVisible;
        private Vector2 _scrollPosition;
        private GUIStyle _titleStyle;
        private GUIStyle _headerStyle;
        private GUIStyle _contentStyle;
        private GUIStyle _hotkeyStyle;
        
        private const string PREF_KEY = "DuckovESP_FirstRun";
        
        public WelcomePopup()
        {
            _windowRect = new Rect(Screen.width / 2 - 350, Screen.height / 2 - 300, 700, 600);
            _isVisible = true;
            _scrollPosition = Vector2.zero;
        }
        
        /// <summary>
        /// 检查是否首次运行，如果是则显示欢迎界面
        /// </summary>
        public void CheckFirstRun()
        {
            /*
            if (!PlayerPrefs.HasKey(PREF_KEY))
            {
                _isVisible = true;
                PlayerPrefs.SetInt(PREF_KEY, 1);
                PlayerPrefs.Save();
            }
            */
        }
        
        /// <summary>
        /// 手动显示欢迎界面
        /// </summary>
        public void Show()
        {
            _isVisible = true;
        }
        
        /// <summary>
        /// 隐藏欢迎界面
        /// </summary>
        public void Hide()
        {
            _isVisible = false;
        }
        
        public bool IsVisible => _isVisible;
        
        /// <summary>
        /// 绘制欢迎界面
        /// </summary>
        public void OnGUI()
        {
            if (!_isVisible) return;
            
            InitStyles();
            
            _windowRect = GUILayout.Window(
                98765,
                _windowRect,
                DrawWindow,
                "",
                GUILayout.Width(700),
                GUILayout.Height(600)
            );
        }
        
        private void InitStyles()
        {
            if (_titleStyle == null)
            {
                _titleStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 28,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter,
                    normal = { textColor = Color.cyan }
                };
            }
            
            if (_headerStyle == null)
            {
                _headerStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 18,
                    fontStyle = FontStyle.Bold,
                    normal = { textColor = Color.yellow }
                };
            }
            
            if (_contentStyle == null)
            {
                _contentStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 14,
                    wordWrap = true,
                    normal = { textColor = Color.white }
                };
            }
            
            if (_hotkeyStyle == null)
            {
                _hotkeyStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 14,
                    fontStyle = FontStyle.Bold,
                    normal = { textColor = Color.green }
                };
            }
        }
        
        private void DrawWindow(int windowID)
        {
            GUILayout.BeginVertical();
            
            // 标题
            GUILayout.Space(10);
            GUILayout.Label("欢迎使用 DuckovESP v2.6", _titleStyle);
            GUILayout.Space(5);
            GUILayout.Label("🎉 重大更新：无后座力 + 无头Boss修复！", _headerStyle);
            GUILayout.Space(20);
            
            // 滚动区域
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, GUILayout.ExpandHeight(true));
            
            // v2.0 修复内容
            GUILayout.Label("🔧 v2.6 重大修复", _headerStyle);
            GUILayout.Space(5);
            GUILayout.Label(
                "本版本引入无后座力功能和无头Boss检测改进，提供更强大的辅助体验：",
                _contentStyle
            );
            GUILayout.Space(5);
            DrawFixList();
            GUILayout.Space(15);
            
            // 简介
            GUILayout.Label("📋 关于 DuckovESP", _headerStyle);
            GUILayout.Space(5);
            GUILayout.Label(
                "DuckovESP 是一款功能强大的 Duckov 游戏辅助工具，提供 ESP 透视、自动瞄准、自动扳机、作弊功能等多种增强体验。",
                _contentStyle
            );
            GUILayout.Space(15);
            
            // 主要功能
            GUILayout.Label("⭐ 主要功能", _headerStyle);
            GUILayout.Space(5);
            DrawFeatureList();
            GUILayout.Space(15);
            
            // 快捷键说明
            GUILayout.Label("⌨️ 快捷键说明", _headerStyle);
            GUILayout.Space(5);
            GUILayout.Label("注意：所有快捷键都需要按住 Shift 键才能生效！", _hotkeyStyle);
            GUILayout.Space(5);
            DrawHotkeyList();
            GUILayout.Space(15);
            
            // 使用提示
            GUILayout.Label("💡 使用提示", _headerStyle);
            GUILayout.Space(5);
            DrawTips();
            GUILayout.Space(15);
            
            // 免责声明
            GUILayout.Label("⚠️ 免责声明", _headerStyle);
            GUILayout.Space(5);
            GUILayout.Label(
                "本工具仅供学习和研究使用。使用本工具可能违反游戏服务条款，可能导致账号被封禁。" +
                "使用本工具的一切后果由使用者自行承担，作者不承担任何责任。",
                _contentStyle
            );
            
            GUILayout.EndScrollView();
            
            // 底部按钮
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("我已了解，开始使用", GUILayout.Width(200), GUILayout.Height(40)))
            {
                Hide();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
            
            GUILayout.EndVertical();
            
            GUI.DragWindow();
        }
        
        private void DrawFixList()
        {
            string[] fixes = new[]
            {
                "✅ 无后座力功能 (v2.6 新增) - 消除所有武器后座力",
                "✅ 无头Boss检测改进 - 对暴走街机等无头敌人有效",
                "✅ 自动瞄准功能完全重构 - 基于Mod_Aimbot的优秀实现",
                "✅ 自动扳机功能重新实现 - 使用子弹传送，在基地中自动禁用",
                "✅ 秒杀功能修复 - 对所有武器生效（枪械和近战）",
                "✅ 无限子弹修复 - 不再出现伤害问题",
                "✅ 物品颜色系统优化 - 价值颜色与其他功能完美融合",
                "✅ 露天物品标记 - 地面物品现在有连线和地图标记",
                "✅ 性能优化 - 减少96%的对象扫描调用",
                "✅ 缓存系统 - 大幅降低CPU占用和内存压力"
            };
            
            foreach (var fix in fixes)
            {
                GUILayout.Label(fix, _contentStyle);
            }
        }
        
        private void DrawFeatureList()
        {
            string[] features = new[]
            {
                "• ESP 透视：显示敌人、物品、任务物品位置和信息",
                "• 自动瞄准：智能锁定敌人头部，自动跟踪目标",
                "• 自动扳机：检测到目标自动开火，子弹传送无视墙体",
                "• 无后座力：消除武器后座力，稳定射击 ⭐ 新功能",
                "• 敌人列表：实时显示所有敌人的距离和状态",
                "• 作弊功能：无敌、秒杀、速度提升、无限资源等",
                "• 配置系统：自定义所有功能的参数和快捷键"
            };
            
            foreach (var feature in features)
            {
                GUILayout.Label(feature, _contentStyle);
            }
        }
        
        private void DrawHotkeyList()
        {
            var hotkeys = new[]
            {
                ("Shift + F5", "打开/关闭配置菜单"),
                ("Shift + F6", "打开/关闭敌人列表"),
                ("Shift + F7", "切换无敌模式"),
                ("Shift + F8", "切换一击必杀"),
                ("Shift + F9", "切换速度提升"),
                ("Shift + F10", "切换无限负重"),
                ("Shift + F11", "切换无限子弹"),
                ("Shift + F12", "切换无限耐力"),
                ("鼠标右键", "自动瞄准（可在配置中修改）")
            };
            
            foreach (var (key, desc) in hotkeys)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(key, _hotkeyStyle, GUILayout.Width(150));
                GUILayout.Label("→  " + desc, _contentStyle);
                GUILayout.EndHorizontal();
            }
        }
        
        private void DrawTips()
        {
            string[] tips = new[]
            {
                "1. 无后座力功能在配置菜单中启用（Shift+F5 打开配置）",
                "2. 无头Boss（如暴走街机）现在可以正确瞄准身体部位",
                "3. 自动瞄准和自动扳机需要在配置中分别启用",
                "4. 自动扳机在基地内会自动禁用，保持安全",
                "5. 可以在配置菜单中自定义所有快捷键和功能参数",
                "6. ESP 绘制可能影响性能，可根据需要调整绘制范围",
                "7. 建议在单人模式或私服中使用，避免影响其他玩家体验"
            };
            
            foreach (var tip in tips)
            {
                GUILayout.Label(tip, _contentStyle);
            }
        }
    }
}
