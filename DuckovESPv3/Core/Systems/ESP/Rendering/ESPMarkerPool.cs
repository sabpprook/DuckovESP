using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Duckov.Utilities;

namespace DuckovESPv3.Core.Systems.ESP.Rendering
{
    /// <summary>
    /// ESP 标记对象池
    /// 管理 ESPMarker 的创建、复用和回收
    /// </summary>
    public class ESPMarkerPool : MonoBehaviour
    {
        public static ESPMarkerPool? Instance { get; private set; }

        [SerializeField] private GameObject markerPrefab;
        [SerializeField] private int initialPoolSize = 5000;
        [SerializeField] private int maxPoolSize = 100000;

        private Queue<ESPMarker> _availableMarkers = new Queue<ESPMarker>();
        private HashSet<ESPMarker> _activeMarkers = new HashSet<ESPMarker>();
        private Transform _poolContainer;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            // 创建容器
            _poolContainer = new GameObject("ESP_Marker_Pool").transform;
            _poolContainer.SetParent(transform);

            // 如果没有 Prefab，自动创建
            if (markerPrefab == null)
            {
                var createdPrefab = CreateMarkerPrefab();
                if (createdPrefab != null)
                {
                    markerPrefab = createdPrefab;
                }
                else
                {
                    Debug.LogError("[ESPMarkerPool] 无法创建 Marker Prefab，对象池初始化失败");
                    return;
                }
            }

            // 预创建对象池
            if (markerPrefab != null)
            {
                for (int i = 0; i < initialPoolSize; i++)
                {
                    CreateNewMarker();
                }
            }
        }

        /// <summary>
        /// 程序化创建 Marker Prefab
        /// </summary>
        private GameObject? CreateMarkerPrefab()
        {
            try
            {
                // 创建根对象
                GameObject prefab = new GameObject("ESPMarker_Template");
                
                // 设置为模板对象，不激活
                prefab.SetActive(false);
                
                // 放入池容器下作为模板
                prefab.transform.SetParent(_poolContainer);

                // 添加 SpriteRenderer
                SpriteRenderer spriteRenderer = prefab.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = ESPSpriteGenerator.CreateCircleSprite(64, true);
                spriteRenderer.color = Color.white;
                
                // 尝试查找 Shader
                Shader shader = Shader.Find("Sprites/Default");
                if (shader == null)
                {
                    shader = Shader.Find("UI/Default");
                }
                if (shader == null)
                {
                    shader = Shader.Find("Unlit/Color");
                }

                // 创建 Canvas（World Space + 自定义Shader性能更好）
                GameObject canvasObj = new GameObject("NameCanvas");
                canvasObj.transform.SetParent(prefab.transform);
                canvasObj.transform.localPosition = new Vector3(0, 0.8f, 0);

                Canvas canvas = canvasObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.WorldSpace;
                canvas.sortingOrder = 32767;  // 最大排序顺序

                UnityEngine.UI.CanvasScaler scaler = canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
                scaler.dynamicPixelsPerUnit = 10f;
                
                // 不添加 GraphicRaycaster，避免拦截鼠标事件（让事件透传到游戏）
                // canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();

                // 创建背景面板（半透明黑色遮罩）
                GameObject bgPanelObj = new GameObject("BackgroundPanel");
                bgPanelObj.transform.SetParent(canvasObj.transform);
                bgPanelObj.transform.localPosition = Vector3.zero;
                bgPanelObj.transform.localScale = Vector3.one * 0.008f;  // World Space缩放
                
                UnityEngine.UI.Image bgImage = bgPanelObj.AddComponent<UnityEngine.UI.Image>();
                bgImage.color = new Color(0, 0, 0, 0.7f);  // 半透明黑色
                bgImage.raycastTarget = false;
                
                // 配置背景材质实现AlwaysOnTop效果
                Shader? bgShader = ShaderLoader.GetUIShader();
                if (bgShader != null)
                {
                    // 方案1: 使用自定义Shader
                    Material bgMat = new Material(bgShader);
                    bgMat.SetColor("_Color", new Color(0, 0, 0, 0.7f));
                    bgMat.renderQueue = 4999;  // 背景在文本之下
                    bgImage.material = bgMat;
                    Debug.Log("[ESPMarkerPool] 背景使用自定义UI Shader");
                }
                else
                {
                    // 方案2: Material配置器（主要方案）
                    Shader? defaultShader = Shader.Find("UI/Default");
                    if (defaultShader != null)
                    {
                        Material bgMat = new Material(defaultShader);
                        bgImage.material = bgMat;
                        MaterialConfigurator.ConfigureUIMaterial(bgMat, new Color(0, 0, 0, 0.7f));
                        Debug.Log("[ESPMarkerPool] 背景使用MaterialConfigurator配置");
                    }
                }
                
                RectTransform bgRect = bgPanelObj.GetComponent<RectTransform>();
                bgRect.sizeDelta = new Vector2(420, 220);  // World Space像素尺寸
                bgRect.anchorMin = new Vector2(0.5f, 0.5f);
                bgRect.anchorMax = new Vector2(0.5f, 0.5f);
                bgRect.pivot = new Vector2(0.5f, 0.5f);

                // 创建 TextMeshPro 文本（物品名称）
                GameObject textObj = new GameObject("NameText");
                textObj.transform.SetParent(canvasObj.transform);
                textObj.transform.localPosition = Vector3.zero;
                textObj.transform.localScale = Vector3.one * 0.008f;  // World Space缩放

                TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
                text.text = "Item Name";
                
                // 尝试使用游戏的标准模板字体（支持多语言 fallback）
                try
                {
                    // 尝试使用游戏的 UIStyle 模板
                    if (GameplayDataSettings.UIStyle?.TemplateTextUGUI != null)
                    {
                        var template = GameplayDataSettings.UIStyle.TemplateTextUGUI;
                        if (template.font != null)
                        {
                            text.font = template.font;
                            Debug.Log("[ESPMarkerPool] 使用游戏标准 TMP 字体（支持多语言）");
                        }
                    }
                    else
                    {
                        // Fallback: 加载 Resources 中的字体
                        TMP_FontAsset tmpFont = Resources.Load<TMP_FontAsset>("fonts & materials/LiberationSans SDF");
                        if (tmpFont != null)
                        {
                            text.font = tmpFont;
                        }
                        Debug.LogWarning("[ESPMarkerPool] 使用 Resources 字体");
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogWarning($"[ESPMarkerPool] 加载字体失败: {ex.Message}，使用默认字体");
                }
                
                text.fontSize = 32;  // World Space基础字体大小
                text.color = Color.white;
                text.alignment = TextAlignmentOptions.Center;
                text.enableWordWrapping = false;
                text.overflowMode = TextOverflowModes.Overflow;

                RectTransform rectTransform = textObj.GetComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(400, 200);  // World Space像素尺寸

                // 添加 ESPMarker 组件
                ESPMarker marker = prefab.AddComponent<ESPMarker>();
                marker.spriteRenderer = spriteRenderer;
                marker.canvas = canvas;
                marker.nameText = text;
                marker.backgroundPanel = bgImage;  // 设置背景遮罩

                Debug.Log("[ESPMarkerPool] 程序化创建 Marker Prefab 完成");
                return prefab;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[ESPMarkerPool] 创建 Marker Prefab 失败: {ex.Message}");
                return null;
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        /// <summary>
        /// 创建新的 Marker
        /// </summary>
        private ESPMarker? CreateNewMarker()
        {
            if (markerPrefab == null)
            {
                Debug.LogError("[ESPMarkerPool] markerPrefab 为 null，无法创建标记");
                return null;
            }

            GameObject markerObj = Instantiate(markerPrefab, _poolContainer);
            markerObj.SetActive(false);

            ESPMarker marker = markerObj.GetComponent<ESPMarker>();
            if (marker == null)
            {
                marker = markerObj.AddComponent<ESPMarker>();
            }

            _availableMarkers.Enqueue(marker);
            return marker;
        }

        /// <summary>
        /// 从池中获取 Marker
        /// </summary>
        public ESPMarker? Get()
        {
            ESPMarker? marker;

            if (_availableMarkers.Count > 0)
            {
                marker = _availableMarkers.Dequeue();
            }
            else
            {
                // 检查是否超过最大容量
                if (_activeMarkers.Count >= maxPoolSize)
                {
                    Debug.LogWarning($"[ESPMarkerPool] 已达到最大容量 {maxPoolSize}，无法创建更多标记");
                    return null;
                }

                marker = CreateNewMarker();
                if (marker == null)
                {
                    Debug.LogError("[ESPMarkerPool] 创建新标记失败");
                    return null;
                }
            }

            _activeMarkers.Add(marker);
            return marker;
        }

        /// <summary>
        /// 归还 Marker 到池中
        /// </summary>
        public void Return(ESPMarker marker)
        {
            if (marker == null)
                return;

            if (!_activeMarkers.Contains(marker))
            {
                Debug.LogWarning("[ESPMarkerPool] 尝试归还不属于此池的标记");
                return;
            }

            marker.Cleanup();
            _activeMarkers.Remove(marker);
            _availableMarkers.Enqueue(marker);
        }

        /// <summary>
        /// 归还所有活动标记
        /// </summary>
        public void ReturnAll()
        {
            var markersToReturn = new List<ESPMarker>(_activeMarkers);
            foreach (var marker in markersToReturn)
            {
                Return(marker);
            }
        }

        /// <summary>
        /// 获取统计信息
        /// </summary>
        public (int active, int available, int total) GetStats()
        {
            return (_activeMarkers.Count, _availableMarkers.Count, _activeMarkers.Count + _availableMarkers.Count);
        }

        /// <summary>
        /// 设置 Prefab（运行时）
        /// </summary>
        public void SetPrefab(GameObject prefab)
        {
            if (markerPrefab == null)
            {
                markerPrefab = prefab;
            }
        }
    }
}
