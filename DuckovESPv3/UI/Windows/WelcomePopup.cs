using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using DuckovESPv3.Infrastructure.Localization;
using DuckovESPv3.UI.Rendering;

namespace DuckovESPv3.UI.Windows
{
    /// <summary>
    /// DuckovESP v3 欢迎弹窗
    /// 
    /// 显示从 v2 到 v3 的主要升级内容，包含 MOD 预览图像
    /// 使用现代化 UI 主题，支持国际化
    /// </summary>
    public class WelcomePopup : IUIWindow
    {
        private bool _isVisible = true;
        private Rect _windowRect;
        private Vector2 _scrollPosition = Vector2.zero;
        
        // 样式缓存
        private GUIStyle? _windowStyle;
        private GUIStyle? _titleStyle;
        private GUIStyle? _sectionHeaderStyle;
        private GUIStyle? _descriptionStyle;
        private GUIStyle? _buttonStyle;
        private GUIStyle? _noticeStyle;
        
        // 预览图像
        private Texture2D? _previewImage;
        private bool _previewImageLoadAttempted = false;
        
        // 升级内容列表
        private readonly List<(string Title, string Description, Color HighlightColor)> _upgrades;

        public bool IsOpen => _isVisible;

        public WelcomePopup()
        {
            // 初始化窗口位置（屏幕中央）
            float width = 900;
            float height = 700;
            _windowRect = new Rect(
                (Screen.width - width) / 2f,
                (Screen.height - height) / 2f,
                width,
                height
            );

            // 初始化升级内容列表
            _upgrades = new List<(string, string, Color)>
            {
                (LocalizationManager.Get("Welcome.ArchitectureTitle"), 
                 LocalizationManager.Get("Welcome.ArchitectureDesc"), 
                 new Color(0.3f, 0.8f, 1f, 1f)),    // 青色
                 
                (LocalizationManager.Get("Welcome.DataCollectionTitle"), 
                 LocalizationManager.Get("Welcome.DataCollectionDesc"), 
                 new Color(0.5f, 1f, 0.5f, 1f)),    // 绿色
                 
                (LocalizationManager.Get("Welcome.RenderingTitle"), 
                 LocalizationManager.Get("Welcome.RenderingDesc"), 
                 new Color(1f, 0.8f, 0.3f, 1f)),    // 黄色
                 
                (LocalizationManager.Get("Welcome.QuestTitle"), 
                 LocalizationManager.Get("Welcome.QuestDesc"), 
                 new Color(0.9f, 0.5f, 0.3f, 1f)),  // 橙色
                 
                (LocalizationManager.Get("Welcome.EnemyTitle"), 
                 LocalizationManager.Get("Welcome.EnemyDesc"), 
                 new Color(1f, 0.4f, 0.4f, 1f)),    // 红色
                 
                (LocalizationManager.Get("Welcome.CheatsTitle"), 
                 LocalizationManager.Get("Welcome.CheatsDesc"), 
                 new Color(1f, 0.7f, 0.9f, 1f)),    // 粉色
                 
                (LocalizationManager.Get("Welcome.UITitle"), 
                 LocalizationManager.Get("Welcome.UIDesc"), 
                 new Color(0.7f, 0.7f, 1f, 1f)),    // 紫色
                 
                (LocalizationManager.Get("Welcome.OptimizationTitle"), 
                 LocalizationManager.Get("Welcome.OptimizationDesc"), 
                 new Color(0.3f, 1f, 0.7f, 1f)),    // 青绿色
                 
                (LocalizationManager.Get("Welcome.LocalizationTitle"), 
                 LocalizationManager.Get("Welcome.LocalizationDesc"), 
                 new Color(0.8f, 0.6f, 1f, 1f))     // 紫罗兰色
            };
        }

        /// <summary>
        /// 切换欢迎弹窗显示状态
        /// </summary>
        public void Toggle()
        {
            _isVisible = !_isVisible;
        }

        /// <summary>
        /// 显示欢迎弹窗
        /// </summary>
        public void Show()
        {
            _isVisible = true;
        }

        /// <summary>
        /// 隐藏欢迎弹窗
        /// </summary>
        public void Hide()
        {
            _isVisible = false;
        }

        /// <summary>
        /// 绘制弹窗
        /// </summary>
        public void Draw()
        {
            if (!_isVisible)
                return;

            InitializeStyles();

            // 【修复】保持窗口大小恒定，防止宽度递增
            _windowRect.width = 900;
            _windowRect.height = 700;

            // 在 GUI 缩放前记录鼠标位置
            var mousePos = Event.current.mousePosition;

            // 【修复】使用 GUILayout.BeginArea 来确保固定尺寸
            GUILayout.BeginArea(_windowRect, "", _windowStyle!);
            
            GUILayout.BeginVertical();

            // ===== 标题区域 =====
            GUILayout.Space(10);
            GUILayout.Label(LocalizationManager.Get("Welcome.Title"), _titleStyle!);
            GUILayout.Space(5);
            GUILayout.Label(LocalizationManager.Get("Welcome.Subtitle"), _sectionHeaderStyle!);
            GUILayout.Space(15);

            // ===== 预览图像区域 =====
            DrawPreviewImage();
            GUILayout.Space(15);

            // ===== 升级内容滚动区域 =====
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, GUILayout.Height(300));
            DrawUpgradeContent();
            GUILayout.EndScrollView();

            GUILayout.Space(15);

            // ===== 提示文本 =====
            GUILayout.Label(LocalizationManager.Get("Welcome.CloseNotice"), _noticeStyle!);
            GUILayout.Space(10);

            // ===== 按钮区域 =====
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(LocalizationManager.Get("Welcome.StartButton"), _buttonStyle!, 
                GUILayout.Width(200), GUILayout.Height(45)))
            {
                Hide();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.EndVertical();
            
            GUILayout.EndArea();

            // 消费窗口区域内的鼠标事件，防止穿透到游戏
            if (_windowRect.Contains(mousePos))
            {
                if (Event.current.type == EventType.MouseDown || 
                    Event.current.type == EventType.MouseUp || 
                    Event.current.type == EventType.MouseDrag ||
                    Event.current.type == EventType.ScrollWheel)
                {
                    Event.current.Use();
                }
            }
        }

        /// <summary>
        /// 初始化所有 GUI 样式
        /// </summary>
        private void InitializeStyles()
        {
            if (_windowStyle != null)
                return;

            // ===== 窗口样式 =====
            _windowStyle = new GUIStyle(GUI.skin.window)
            {
                normal = { background = CreateSolidColorTexture(new Color(0.12f, 0.12f, 0.14f, 0.98f)) },
                padding = new RectOffset(20, 20, 25, 20),
                border = new RectOffset(1, 1, 1, 1)
            };
            _windowStyle.normal.background.filterMode = FilterMode.Point;

            // ===== 标题样式 =====
            _titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 28,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = new Color(0.2f, 1f, 0.8f, 1f) } // 青绿色
            };

            // ===== 部分标题样式 =====
            _sectionHeaderStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 14,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft,
                normal = { textColor = Color.white }
            };

            // ===== 描述文本样式 =====
            _descriptionStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 12,
                wordWrap = true,
                alignment = TextAnchor.UpperLeft,
                normal = { textColor = new Color(0.85f, 0.85f, 0.85f, 1f) }
            };

            // ===== 按钮样式 =====
            _buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 14,
                fontStyle = FontStyle.Bold,
                normal = { background = CreateSolidColorTexture(new Color(0.2f, 1f, 0.8f, 1f)), textColor = new Color(0.1f, 0.1f, 0.1f, 1f) },
                hover = { background = CreateSolidColorTexture(new Color(0.3f, 1f, 0.9f, 1f)) },
                active = { background = CreateSolidColorTexture(new Color(0.1f, 0.9f, 0.7f, 1f)) }
            };
            _buttonStyle.padding = new RectOffset(15, 15, 8, 8);

            // ===== 提示文本样式 =====
            _noticeStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 11,
                wordWrap = true,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = new Color(1f, 0.8f, 0.2f, 1f) } // 金色
            };
        }

        /// <summary>
        /// 绘制窗口内容
        /// </summary>
        private void DrawWindowContent(int windowID)
        {
            // 此方法已移除，逻辑合并到 Draw() 中以使用 GUILayout.BeginArea
        }

        /// <summary>
        /// 绘制预览图像
        /// </summary>
        private void DrawPreviewImage()
        {
            // 尝试加载预览图像（仅加载一次）
            if (!_previewImageLoadAttempted)
            {
                LoadPreviewImage();
            }

            if (_previewImage != null)
            {
                // 计算缩放尺寸（宽度最大为窗口宽度-40，高度自适应）
                float maxWidth = _windowRect.width - 40;
                float aspectRatio = (float)_previewImage.width / _previewImage.height;
                float displayHeight = maxWidth / aspectRatio;

                // 限制最大高度
                if (displayHeight > 150)
                {
                    displayHeight = 150;
                }

                // 居中显示图像
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Box(_previewImage, GUILayout.Width(maxWidth), GUILayout.Height(displayHeight));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
        }

        /// <summary>
        /// 绘制升级内容
        /// </summary>
        private void DrawUpgradeContent()
        {
            int index = 0;
            foreach (var (title, description, highlightColor) in _upgrades)
            {
                DrawUpgradeItem(title, description, highlightColor, index);
                GUILayout.Space(8);
                index++;
            }
        }

        /// <summary>
        /// 绘制单个升级项目
        /// </summary>
        private void DrawUpgradeItem(string title, string description, Color highlightColor, int index)
        {
            // 创建高亮标题样式
            var titleStyle = new GUIStyle(_sectionHeaderStyle!)
            {
                normal = { textColor = highlightColor }
            };

            GUILayout.BeginVertical(GUI.skin.box);
            
            // 编号 + 标题
            GUILayout.BeginHorizontal();
            GUILayout.Label($"✦ {title}", titleStyle, GUILayout.Width(300));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            // 描述文本
            GUILayout.Space(3);
            GUILayout.Label(description, _descriptionStyle!);

            GUILayout.EndVertical();
        }

        /// <summary>
        /// 加载预览图像
        /// </summary>
        private void LoadPreviewImage()
        {
            _previewImageLoadAttempted = true;

            try
            {
                // 获取 DLL 所在目录
                string dllPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                string dllDirectory = Path.GetDirectoryName(dllPath);
                string previewPath = Path.Combine(dllDirectory ?? "", "preview.png");

                if (File.Exists(previewPath))
                {
                    // 【修复】读取 PNG 数据并使用反射调用 Texture2D.LoadImage
                    byte[] imageData = File.ReadAllBytes(previewPath);
                    _previewImage = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                    
                    // 使用反射调用 Texture2D.LoadImage 方法
                    var loadImageMethod = typeof(Texture2D).GetMethod("LoadImage", 
                        System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                    
                    if (loadImageMethod != null)
                    {
                        bool result = (bool)loadImageMethod.Invoke(_previewImage, new object[] { imageData })!;
                        if (result)
                        {
                            _previewImage.filterMode = FilterMode.Bilinear;
                            Debug.Log($"[WelcomePopup] 成功加载预览图像: {previewPath} ({_previewImage.width}x{_previewImage.height})");
                        }
                        else
                        {
                            Debug.LogError($"[WelcomePopup] 加载 PNG 图像失败: LoadImage 返回 false");
                            _previewImage = null;
                        }
                    }
                    else
                    {
                        Debug.LogError("[WelcomePopup] 找不到 Texture2D.LoadImage 方法");
                        _previewImage = null;
                    }
                }
                else
                {
                    Debug.LogWarning($"[WelcomePopup] 预览图像不存在: {previewPath}");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[WelcomePopup] 加载预览图像异常: {ex.Message}\n{ex.StackTrace}");
                _previewImage = null;
            }
        }

        /// <summary>
        /// 创建纯色纹理
        /// </summary>
        private static Texture2D CreateSolidColorTexture(Color color)
        {
            var texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            texture.SetPixel(0, 0, color);
            texture.Apply();
            texture.filterMode = FilterMode.Point;
            return texture;
        }
    }
}
