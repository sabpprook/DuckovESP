using System;
using UnityEngine;
using DuckovESP.Utils.Localization;

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
            GUILayout.Label(LocalizationManager.Get("Welcome.Title"), _titleStyle);
            GUILayout.Space(5);
            GUILayout.Label(LocalizationManager.Get("Welcome.Subtitle"), _headerStyle);
            GUILayout.Space(20);
            
            // 滚动区域
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, GUILayout.ExpandHeight(true));
            
            // v2.0 修复内容
            GUILayout.Label(LocalizationManager.Get("Welcome.SectionMajorFixes"), _headerStyle);
            GUILayout.Space(5);
            GUILayout.Label(
                LocalizationManager.Get("Welcome.FixesDescription"),
                _contentStyle
            );
            GUILayout.Space(5);
            DrawFixList();
            GUILayout.Space(15);
            
            // 简介
            GUILayout.Label(LocalizationManager.Get("Welcome.SectionAbout"), _headerStyle);
            GUILayout.Space(5);
            GUILayout.Label(
                LocalizationManager.Get("Welcome.AboutDescription"),
                _contentStyle
            );
            GUILayout.Space(15);
            
            // 主要功能
            GUILayout.Label(LocalizationManager.Get("Welcome.SectionFeatures"), _headerStyle);
            GUILayout.Space(5);
            DrawFeatureList();
            GUILayout.Space(15);
            
            // 快捷键说明
            GUILayout.Label(LocalizationManager.Get("Welcome.SectionHotkeys"), _headerStyle);
            GUILayout.Space(5);
            GUILayout.Label(LocalizationManager.Get("Welcome.HotkeyWarning"), _hotkeyStyle);
            GUILayout.Space(5);
            DrawHotkeyList();
            GUILayout.Space(15);
            
            // 使用提示
            GUILayout.Label(LocalizationManager.Get("Welcome.SectionTips"), _headerStyle);
            GUILayout.Space(5);
            DrawTips();
            GUILayout.Space(15);
            
            // 免责声明
            GUILayout.Label(LocalizationManager.Get("Welcome.SectionDisclaimer"), _headerStyle);
            GUILayout.Space(5);
            GUILayout.Label(
                LocalizationManager.Get("Welcome.DisclaimerText"),
                _contentStyle
            );
            
            GUILayout.EndScrollView();
            
            // 底部按钮
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(LocalizationManager.Get("Welcome.ButtonStart"), GUILayout.Width(200), GUILayout.Height(40)))
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
                LocalizationManager.Get("Welcome.Fix1"),
                LocalizationManager.Get("Welcome.Fix2"),
                LocalizationManager.Get("Welcome.Fix3"),
                LocalizationManager.Get("Welcome.Fix4"),
                LocalizationManager.Get("Welcome.Fix5"),
                LocalizationManager.Get("Welcome.Fix6"),
                LocalizationManager.Get("Welcome.Fix7"),
                LocalizationManager.Get("Welcome.Fix8"),
                LocalizationManager.Get("Welcome.Fix9"),
                LocalizationManager.Get("Welcome.Fix10")
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
                LocalizationManager.Get("Welcome.Feature1"),
                LocalizationManager.Get("Welcome.Feature2"),
                LocalizationManager.Get("Welcome.Feature3"),
                LocalizationManager.Get("Welcome.Feature4"),
                LocalizationManager.Get("Welcome.Feature5"),
                LocalizationManager.Get("Welcome.Feature6"),
                LocalizationManager.Get("Welcome.Feature7")
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
                (LocalizationManager.Get("Welcome.Hotkey1Key"), LocalizationManager.Get("Welcome.Hotkey1Desc")),
                (LocalizationManager.Get("Welcome.Hotkey2Key"), LocalizationManager.Get("Welcome.Hotkey2Desc")),
                (LocalizationManager.Get("Welcome.Hotkey3Key"), LocalizationManager.Get("Welcome.Hotkey3Desc")),
                (LocalizationManager.Get("Welcome.Hotkey4Key"), LocalizationManager.Get("Welcome.Hotkey4Desc")),
                (LocalizationManager.Get("Welcome.Hotkey5Key"), LocalizationManager.Get("Welcome.Hotkey5Desc")),
                (LocalizationManager.Get("Welcome.Hotkey6Key"), LocalizationManager.Get("Welcome.Hotkey6Desc")),
                (LocalizationManager.Get("Welcome.Hotkey7Key"), LocalizationManager.Get("Welcome.Hotkey7Desc")),
                (LocalizationManager.Get("Welcome.Hotkey8Key"), LocalizationManager.Get("Welcome.Hotkey8Desc")),
                (LocalizationManager.Get("Welcome.Hotkey9Key"), LocalizationManager.Get("Welcome.Hotkey9Desc"))
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
                LocalizationManager.Get("Welcome.Tip1"),
                LocalizationManager.Get("Welcome.Tip2"),
                LocalizationManager.Get("Welcome.Tip3"),
                LocalizationManager.Get("Welcome.Tip4"),
                LocalizationManager.Get("Welcome.Tip5"),
                LocalizationManager.Get("Welcome.Tip6"),
                LocalizationManager.Get("Welcome.Tip7")
            };
            
            foreach (var tip in tips)
            {
                GUILayout.Label(tip, _contentStyle);
            }
        }
    }
}
