using UnityEngine;

namespace DuckovESPv3.UI.Rendering
{
    /// <summary>
    /// GUI风格管理器
    /// </summary>
    /// <remarks>
    /// 架构层级：UI/Rendering
    /// 职责：
    /// - 集中管理所有GUI元素的样式
    /// - 避免在每次绘制时重新创建GUIStyle
    /// - 提供统一的UI外观
    /// 性能特征：样式在首次使用时创建，之后缓存
    /// </remarks>
    public class GUIStyleManager
    {
        private GUIStyle? _windowStyle;
        private GUIStyle? _labelStyle;
        private GUIStyle? _buttonStyle;
        private GUIStyle? _toggleStyle;
        private GUIStyle? _horizontalSliderStyle;
        private GUIStyle? _titleLabelStyle;
        private bool _initialized = false;
        private float _uiScale = 1f;

        /// <summary>
        /// 获取或设置UI缩放比例
        /// </summary>
        public float UIScale
        {
            get => _uiScale;
            set
            {
                if (_uiScale != value)
                {
                    _uiScale = value;
                    _initialized = false; // 强制重新初始化样式
                }
            }
        }

        /// <summary>
        /// 初始化所有GUI样式
        /// </summary>
        public void Initialize()
        {
            if (_initialized)
                return;

            // 窗口样式
            _windowStyle = new GUIStyle(GUI.skin.window);
            _windowStyle.normal.background = MakeTex(2, 2, new Color(0.1f, 0.1f, 0.1f, 0.95f));
            _windowStyle.fontSize = (int)(12 * _uiScale);
            // 固定padding，不随缩放变化（避免内容区域缩小）
            _windowStyle.padding = new RectOffset(10, 10, 20, 10);

            // 标签样式
            _labelStyle = new GUIStyle(GUI.skin.label);
            _labelStyle.normal.textColor = Color.white;
            _labelStyle.fontSize = (int)(12 * _uiScale);

            // 标题标签样式
            _titleLabelStyle = new GUIStyle(GUI.skin.label);
            _titleLabelStyle.normal.textColor = new Color(1f, 0.8f, 0f); // 金色
            _titleLabelStyle.fontSize = (int)(14 * _uiScale);
            _titleLabelStyle.fontStyle = FontStyle.Bold;

            // 按钮样式
            _buttonStyle = new GUIStyle(GUI.skin.button);
            _buttonStyle.fontSize = (int)(12 * _uiScale);
            _buttonStyle.normal.textColor = Color.white;
            _buttonStyle.normal.background = MakeTex(2, 2, new Color(0.3f, 0.3f, 0.3f));
            _buttonStyle.hover.background = MakeTex(2, 2, new Color(0.4f, 0.4f, 0.4f));
            // 固定padding，避免文字裁切
            _buttonStyle.padding = new RectOffset(10, 10, 5, 5);
            // 设置最小尺寸，确保文字有足够空间
            _buttonStyle.fixedHeight = 0;
            _buttonStyle.stretchHeight = true;

            // 复选框样式
            _toggleStyle = new GUIStyle(GUI.skin.toggle);
            _toggleStyle.normal.textColor = Color.white;
            _toggleStyle.fontSize = (int)(12 * _uiScale);
            // 固定margin
            _toggleStyle.margin = new RectOffset(0, 0, 3, 3);

            // 滑块样式
            _horizontalSliderStyle = new GUIStyle(GUI.skin.horizontalSlider);

            _initialized = true;
        }

        /// <summary>
        /// 获取窗口样式
        /// </summary>
        public GUIStyle WindowStyle
        {
            get
            {
                if (!_initialized) Initialize();
                return _windowStyle!;
            }
        }

        /// <summary>
        /// 获取标签样式
        /// </summary>
        public GUIStyle LabelStyle
        {
            get
            {
                if (!_initialized) Initialize();
                return _labelStyle!;
            }
        }

        /// <summary>
        /// 获取标题标签样式
        /// </summary>
        public GUIStyle TitleLabelStyle
        {
            get
            {
                if (!_initialized) Initialize();
                return _titleLabelStyle!;
            }
        }

        /// <summary>
        /// 获取按钮样式
        /// </summary>
        public GUIStyle ButtonStyle
        {
            get
            {
                if (!_initialized) Initialize();
                return _buttonStyle!;
            }
        }

        /// <summary>
        /// 获取复选框样式
        /// </summary>
        public GUIStyle ToggleStyle
        {
            get
            {
                if (!_initialized) Initialize();
                return _toggleStyle!;
            }
        }

        /// <summary>
        /// 获取滑块样式
        /// </summary>
        public GUIStyle HorizontalSliderStyle
        {
            get
            {
                if (!_initialized) Initialize();
                return _horizontalSliderStyle!;
            }
        }

        /// <summary>
        /// 创建纯色纹理
        /// </summary>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <param name="col">颜色</param>
        /// <returns>纹理</returns>
        private static Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; i++)
            {
                pix[i] = col;
            }
            Texture2D result = new Texture2D(width, height, TextureFormat.RGBA32, false);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }
    }
}
