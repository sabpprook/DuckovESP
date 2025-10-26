using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DuckovESPv3.Core.Systems.ESP.Models;
using DuckovESPv3.Core.Systems.ESP.Formatting;
using DuckovESPv3.Core.Configuration;

namespace DuckovESPv3.Core.Systems.ESP.Rendering
{
    /// <summary>
    /// ESP渲染模式
    /// </summary>
    public enum ESPRenderMode
    {
        /// <summary>世界空间 - 高性能，可能有遮挡</summary>
        WorldSpace = 0,
        /// <summary>屏幕空间 - 无遮挡，但性能较低（-30% FPS）</summary>
        ScreenOverlay = 1
    }

    /// <summary>
    /// ESP 标记组件
    /// 负责单个物品/箱子的 3D 空间标记渲染
    /// </summary>
    public class ESPMarker : MonoBehaviour
    {
        // ===== 渲染组件 =====
        public SpriteRenderer spriteRenderer;  // 保留但不使用
        public LineRenderer lineRenderer;
        public Canvas canvas;
        public TextMeshProUGUI nameText;  // 使用 TMP 替代 UI.Text
        public UnityEngine.UI.Image backgroundPanel;  // 背景遮罩

        // ===== 渲染模式 =====
        private ESPRenderMode _renderMode = ESPRenderMode.WorldSpace;
        private RectTransform? _canvasRect;

        // ===== 追踪目标 =====
        private Transform? _target;
        private Transform? _playerTransform;
        private Camera? _mainCamera;

        // ===== 显示属性 =====
        private Color _markerColor;
        private float _maxDistance = 100f;
        private bool _showLine = true;
        private bool _showDistance = true;
        private float _uiScale = 1f;  // UI 缩放
        private int _baseFontSize = 16;  // 基础字体大小

        // ===== 数据引用 =====
        private object? _dataReference; // LootboxData 或 WorldItemData
        private ESPSystemConfig? _config; // 配置引用（用于敌人信息格式化）

        /// <summary>
        /// 初始化标记
        /// </summary>
        public void Initialize(
            Transform target,
            Transform player,
            Color color,
            float maxDistance,
            bool showLine,
            bool showDistance,
            object dataReference,
            int fontSize = 12,
            float uiScale = 1f,
            ESPRenderMode renderMode = ESPRenderMode.WorldSpace,
            ESPSystemConfig? config = null)
        {
            try
            {
                _target = target;
                _playerTransform = player;
                _mainCamera = Camera.main;
                _markerColor = color;
                _maxDistance = maxDistance;
                _showLine = showLine;
                _showDistance = showDistance;
                _dataReference = dataReference;
                _uiScale = uiScale;
                _baseFontSize = fontSize;
                _renderMode = renderMode;
                _config = config;

                // 根据渲染模式配置Canvas
                if (canvas != null)
                {
                    _canvasRect = canvas.GetComponent<RectTransform>();
                    
                    if (renderMode == ESPRenderMode.ScreenOverlay)
                    {
                        // ScreenSpaceOverlay模式：Canvas必须是根节点，不能有父对象
                        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                        canvas.sortingOrder = 32767;
                        
                        // 关键：将Canvas移到场景根节点，脱离父对象
                        canvas.transform.SetParent(null, false);
                        
                        // 配置RectTransform - 设置为全屏，锚点在左下角
                        if (_canvasRect != null)
                        {
                            // 填满整个屏幕
                            _canvasRect.anchorMin = Vector2.zero;
                            _canvasRect.anchorMax = Vector2.one;
                            _canvasRect.offsetMin = Vector2.zero;
                            _canvasRect.offsetMax = Vector2.zero;
                            _canvasRect.localScale = Vector3.one;
                        }
                        
                        if (nameText != null && nameText.transform.parent != null)
                        {
                            // 文字的父容器需要设置锚点
                            var textRect = nameText.GetComponent<RectTransform>();
                            if (textRect != null)
                            {
                                textRect.anchorMin = Vector2.zero;
                                textRect.anchorMax = Vector2.zero;
                                textRect.pivot = new Vector2(0.5f, 0.5f);
                            }
                            nameText.transform.localScale = Vector3.one;
                        }
                        if (backgroundPanel != null && backgroundPanel.transform.parent != null)
                        {
                            var bgRect = backgroundPanel.GetComponent<RectTransform>();
                            if (bgRect != null)
                            {
                                bgRect.anchorMin = Vector2.zero;
                                bgRect.anchorMax = Vector2.zero;
                                bgRect.pivot = new Vector2(0.5f, 0.5f);
                            }
                            backgroundPanel.transform.localScale = Vector3.one;
                        }
                        
                        Debug.Log("[ESPMarker] 使用ScreenSpaceOverlay渲染模式（无遮挡），Canvas已移到根节点");
                    }
                    else
                    {
                        // 世界空间模式 - 高性能
                        canvas.renderMode = RenderMode.WorldSpace;
                        canvas.sortingOrder = 32767;
                        
                        // 如果Canvas不是子对象，重新挂回来（从ScreenOverlay模式切换过来）
                        if (canvas.transform.parent != transform)
                        {
                            canvas.transform.SetParent(transform, false);
                            canvas.transform.localPosition = Vector3.zero;
                            canvas.transform.localRotation = Quaternion.identity;
                        }
                        
                        // 重置RectTransform为WorldSpace默认设置
                        if (_canvasRect != null)
                        {
                            _canvasRect.anchorMin = new Vector2(0.5f, 0.5f);
                            _canvasRect.anchorMax = new Vector2(0.5f, 0.5f);
                            _canvasRect.pivot = new Vector2(0.5f, 0.5f);
                            _canvasRect.localScale = Vector3.one;
                        }
                        
                        // 恢复世界空间缩放
                        if (nameText != null && nameText.transform.parent != null)
                        {
                            var textRect = nameText.GetComponent<RectTransform>();
                            if (textRect != null)
                            {
                                textRect.anchorMin = new Vector2(0.5f, 0.5f);
                                textRect.anchorMax = new Vector2(0.5f, 0.5f);
                                textRect.pivot = new Vector2(0.5f, 0.5f);
                                textRect.anchoredPosition = Vector2.zero; // 重置位置
                            }
                            nameText.transform.localScale = Vector3.one * 0.008f;
                        }
                        if (backgroundPanel != null && backgroundPanel.transform.parent != null)
                        {
                            var bgRect = backgroundPanel.GetComponent<RectTransform>();
                            if (bgRect != null)
                            {
                                bgRect.anchorMin = new Vector2(0.5f, 0.5f);
                                bgRect.anchorMax = new Vector2(0.5f, 0.5f);
                                bgRect.pivot = new Vector2(0.5f, 0.5f);
                                bgRect.anchoredPosition = Vector2.zero; // 重置位置
                            }
                            backgroundPanel.transform.localScale = Vector3.one * 0.008f;
                        }
                        
                        Debug.Log("[ESPMarker] 使用世界空间渲染模式（高性能）");
                    }
                }

                // 隐藏 SpriteRenderer（不再显示圆形）
                if (spriteRenderer != null)
                {
                    spriteRenderer.enabled = false;
                }

                // 配置 LineRenderer
                if (lineRenderer != null)
                {
                    lineRenderer.enabled = showLine;
                    if (showLine)
                    {
                        // 判断是否为敌人数据，使用不同样式
                        bool isEnemy = dataReference is EnemyData;
                        
                        if (isEnemy)
                        {
                            // 敌人：红色虚线，略粗
                            var enemyData = (EnemyData)dataReference;
                            Color enemyLineColor = EnemyInfoFormatter.GetEnemyLineColor(enemyData);
                            
                            lineRenderer.startColor = enemyLineColor;
                            lineRenderer.endColor = new Color(enemyLineColor.r, enemyLineColor.g, enemyLineColor.b, 0.3f);
                            lineRenderer.startWidth = 0.08f;  // 略粗
                            lineRenderer.endWidth = 0.03f;
                            lineRenderer.positionCount = 2;
                            
                            // 虚线效果
                            lineRenderer.textureMode = LineTextureMode.Tile;
                        }
                        else
                        {
                            // 物品/箱子：原有实线样式
                            lineRenderer.startColor = color;
                            lineRenderer.endColor = new Color(color.r, color.g, color.b, 0.3f);
                            lineRenderer.startWidth = 0.05f;
                            lineRenderer.endWidth = 0.02f;
                            lineRenderer.positionCount = 2;
                        }
                        
                        // 确保线条不被遮挡 - 创建一个始终在最前面的材质
                        Shader? lineShader = null;
                        
                        // 敌人虚线需要支持贴图的shader
                        if (isEnemy)
                        {
                            lineShader = Shader.Find("Particles/Additive");
                            if (lineShader == null)
                            {
                                lineShader = Shader.Find("Mobile/Particles/Additive");
                            }
                        }
                        
                        // 物品实线使用简单shader
                        if (lineShader == null)
                        {
                            lineShader = Shader.Find("Unlit/Color");
                            if (lineShader == null)
                            {
                                lineShader = Shader.Find("Sprites/Default");
                            }
                        }
                        
                        if (lineShader != null)
                        {
                            Material lineMat = new Material(lineShader);
                            lineMat.renderQueue = 4998;  // 线条在背景之上
                            lineMat.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.Always);  // 永远通过深度测试
                            
                            // 虚线效果（敌人）
                            if (isEnemy)
                            {
                                // 创建简单的虚线纹理
                                Texture2D dashTexture = new Texture2D(32, 1);
                                for (int i = 0; i < 32; i++)
                                {
                                    // 前16像素白色，后16像素透明 = 虚线
                                    dashTexture.SetPixel(i, 0, i < 16 ? Color.white : Color.clear);
                                }
                                dashTexture.Apply();
                                dashTexture.wrapMode = TextureWrapMode.Repeat;
                                
                                lineMat.mainTexture = dashTexture;
                                lineMat.mainTextureScale = new Vector2(10, 1); // 10段虚线
                            }
                            
                            lineRenderer.material = lineMat;
                        }
                    }
                }

                // 配置名称文本 - 使用游戏标准样式 + 背景遮罩
                if (canvas != null && nameText != null)
                {
                    canvas.gameObject.SetActive(true);
                    
                    // 设置背景遮罩（半透明黑色，增强对比度）
                    if (backgroundPanel != null)
                    {
                        backgroundPanel.color = new Color(0, 0, 0, 0.7f);  // 70% 不透明度
                        backgroundPanel.raycastTarget = false;  // 不拦截鼠标事件
                    }
                    
                    // 禁用文本的鼠标事件拦截（透传给游戏）
                    nameText.raycastTarget = false;
                    
                    // 设置粗体 (TMP 使用 FontStyles)
                    nameText.fontStyle = FontStyles.Bold;
                    
                    // 设置字体大小（直接使用配置值，不再额外放大）
                    nameText.fontSize = Mathf.RoundToInt(fontSize * _uiScale);
                    
                    // 设置颜色（使用物品品质颜色，确保明亮可见）
                    nameText.color = color;
                    
                    // 配置材质实现AlwaysOnTop效果（无需自定义Shader）
                    if (nameText.fontMaterial != null)
                    {
                        // 方案1: 尝试使用自定义Shader（如果可用）
                        Shader? customShader = ShaderLoader.GetTMPShader();
                        if (customShader != null)
                        {
                            Material mat = new Material(customShader);
                            mat.SetColor("_FaceColor", color);
                            mat.renderQueue = 5000;  // 文本最高优先级
                            nameText.fontMaterial = mat;
                            Debug.Log("[ESPMarker] 使用自定义TMP Shader");
                        }
                        else
                        {
                            // 方案2: Material配置器（主要方案，无需Shader文件）
                            Material mat = new Material(nameText.fontSharedMaterial);
                            nameText.fontMaterial = mat;
                            MaterialConfigurator.ConfigureTMPMaterial(mat, color);
                            
                            // 验证配置
                            if (!MaterialConfigurator.VerifyMaterialConfiguration(mat))
                            {
                                Debug.LogWarning("[ESPMarker] TMP材质配置可能未完全生效");
                            }
                        }
                        
                        nameText.UpdateMeshPadding();
                    }
                    
                    // 背景遮罩材质
                    if (backgroundPanel != null && backgroundPanel.material != null)
                    {
                        Shader? bgShader = ShaderLoader.GetUIShader();
                        if (bgShader != null)
                        {
                            Material bgMat = new Material(bgShader);
                            bgMat.SetColor("_Color", new Color(0, 0, 0, 0.7f));
                            bgMat.renderQueue = 4999;  // 背景在文本之下
                            backgroundPanel.material = bgMat;
                            Debug.Log("[ESPMarker] 使用自定义UI Shader");
                        }
                        else
                        {
                            Material bgMat = new Material(backgroundPanel.material);
                            backgroundPanel.material = bgMat;
                            MaterialConfigurator.ConfigureUIMaterial(bgMat, new Color(0, 0, 0, 0.7f));
                        }
                    }
                    
                    // 根据数据类型显示不同内容（先设置文本）
                    if (dataReference is LootboxData lootboxData)
                    {
                        // 箱子：显示所有物品名称（换行）
                        string itemNames = GetLootboxItemNames(lootboxData);
                        nameText.text = itemNames;
                    }
                    else if (dataReference is WorldItemData itemData)
                    {
                        // 地面物品：显示物品名称
                        nameText.text = itemData.DisplayName;
                    }
                    else if (dataReference is EnemyData enemyData)
                    {
                        // 敌人：显示格式化的敌人信息
                        // 使用配置控制显示内容，如果配置为null，使用默认配置（全部显示）
                        if (config != null)
                        {
                            nameText.text = EnemyInfoFormatter.FormatEnemyInfo(enemyData, showDistance, config);
                        }
                        else
                        {
                            // 后备方案：如果没有配置，使用默认全部显示
                            var defaultConfig = new ESPSystemConfig
                            {
                                ShowEnemyHealth = true,
                                ShowEnemyWeapon = true,
                                ShowEnemyValue = true
                            };
                            nameText.text = EnemyInfoFormatter.FormatEnemyInfo(enemyData, showDistance, defaultConfig);
                        }
                        
                        // 使用敌人威胁等级颜色
                        Color enemyColor = EnemyInfoFormatter.GetEnemyColor(enemyData);
                        nameText.color = enemyColor;
                        
                        // 更新材质颜色
                        if (nameText.fontMaterial != null)
                        {
                            nameText.fontMaterial.SetColor("_FaceColor", enemyColor);
                        }
                    }
                    
                    // 强制更新文本网格
                    nameText.ForceMeshUpdate();
                }

                gameObject.SetActive(true);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[ESPMarker] 初始化失败: {ex.Message}\n{ex.StackTrace}");
                throw;
            }
        }

        /// <summary>
        /// 更新UI缩放比例
        /// </summary>
        public void UpdateUIScale(float scale)
        {
            _uiScale = scale;
            if (nameText != null)
            {
                nameText.fontSize = Mathf.RoundToInt(_baseFontSize * _uiScale);
            }
        }

        /// <summary>
        /// 更新字体大小
        /// </summary>
        public void UpdateFontSize(int fontSize)
        {
            _baseFontSize = fontSize;
            if (nameText != null)
            {
                nameText.fontSize = Mathf.RoundToInt(_baseFontSize * _uiScale);
            }
        }

        /// <summary>
        /// 更新最大显示距离
        /// </summary>
        public void UpdateMaxDistance(float maxDistance)
        {
            _maxDistance = maxDistance;
        }

        /// <summary>
        /// 更新显示选项
        /// </summary>
        public void UpdateDisplayOptions(bool showLine, bool showDistance)
        {
            _showLine = showLine;
            _showDistance = showDistance;
            
            // 立即更新LineRenderer状态
            if (lineRenderer != null)
            {
                lineRenderer.enabled = _showLine;
            }
        }

        /// <summary>
        /// 更新敌人标记文本（血量变化时调用）
        /// </summary>
        public void UpdateEnemyText(EnemyData enemyData)
        {
            if (nameText != null && _dataReference is EnemyData && _config != null)
            {
                nameText.text = EnemyInfoFormatter.FormatEnemyInfo(enemyData, _showDistance, _config);
                nameText.ForceMeshUpdate();
            }
        }

        /// <summary>
        /// 获取箱子内物品名称列表
        /// </summary>
        private string GetLootboxItemNames(LootboxData lootboxData)
        {
            if (lootboxData.Items == null || lootboxData.Items.Count == 0)
            {
                return "[空箱子]";
            }

            var itemNames = new System.Collections.Generic.List<string>();
            foreach (var item in lootboxData.Items)
            {
                if (item != null && !string.IsNullOrEmpty(item.DisplayName))
                {
                    // 如果有多个相同物品，显示数量
                    int stackCount = item.StackCount > 1 ? item.StackCount : 1;
                    if (stackCount > 1)
                    {
                        itemNames.Add($"{item.DisplayName} x{stackCount}");
                    }
                    else
                    {
                        itemNames.Add(item.DisplayName);
                    }
                }
            }

            // 限制显示前 5 个物品
            if (itemNames.Count > 5)
            {
                itemNames = itemNames.GetRange(0, 5);
                itemNames.Add($"... (+{lootboxData.Items.Count - 5})");
            }

            return string.Join("\n", itemNames);
        }

        private void LateUpdate()
        {
            if (_target == null || _playerTransform == null)
            {
                ReturnToPool();
                return;
            }
            
            // 如果是敌人标记，检查敌人是否已死亡（后备清理机制）
            if (_dataReference is EnemyData enemyDataRef)
            {
                // 检查Health组件是否存在且是否死亡
                if (enemyDataRef.HealthComponent == null || enemyDataRef.HealthComponent.IsDead)
                {
                    Debug.Log($"[ESPMarker] 检测到死亡敌人标记，自动清理: {enemyDataRef.Name}");
                    ReturnToPool();
                    return;
                }
                
                // 额外安全检查：Character是否为null（可能已被销毁）
                if (enemyDataRef.Character == null)
                {
                    Debug.Log($"[ESPMarker] 检测到无效敌人引用，自动清理: {enemyDataRef.Name}");
                    ReturnToPool();
                    return;
                }
            }

            // 每帧更新摄像机引用，避免摄像机切换导致的问题
            _mainCamera = Camera.main;
            if (_mainCamera == null)
            {
                return;
            }

            // 计算玩家到目标的距离
            float distance = Vector3.Distance(_playerTransform.position, _target.position);

            // 距离裁剪
            bool isInRange = distance <= _maxDistance;
            
            // 根据渲染模式处理位置和旋转
            if (_renderMode == ESPRenderMode.ScreenOverlay)
            {
                // ScreenSpaceOverlay模式：使用RectTransform.anchoredPosition设置屏幕坐标
                Vector3 worldPos = _target.position + Vector3.up * 1.5f;
                Vector3 screenPos = _mainCamera.WorldToScreenPoint(worldPos);
                
                // 判断是否在屏幕范围内（用于UI显示）
                bool isOnScreen = screenPos.z > 0 && screenPos.x >= 0 && screenPos.x <= Screen.width && 
                                  screenPos.y >= 0 && screenPos.y <= Screen.height;
                
                // UI元素：只在屏幕内且距离内显示
                if (isOnScreen && isInRange)
                {
                    if (canvas != null)
                    {
                        canvas.enabled = true;
                        
                        // 使用nameText的RectTransform.anchoredPosition设置屏幕位置
                        if (nameText != null)
                        {
                            var textRect = nameText.GetComponent<RectTransform>();
                            if (textRect != null)
                            {
                                textRect.anchoredPosition = new Vector2(screenPos.x, screenPos.y);
                            }
                        }
                        
                        // 背景跟随文字
                        if (backgroundPanel != null)
                        {
                            var bgRect = backgroundPanel.GetComponent<RectTransform>();
                            if (bgRect != null)
                            {
                                bgRect.anchoredPosition = new Vector2(screenPos.x, screenPos.y);
                            }
                        }
                    }
                }
                else
                {
                    // UI不在屏幕内或超出距离，隐藏
                    if (canvas != null) canvas.enabled = false;
                }
                
                // 连接线：只要在距离内就显示，不受屏幕范围限制
                // 这样行为和WorldSpace模式一致
                if (lineRenderer != null)
                {
                    lineRenderer.enabled = _showLine && isInRange;
                }
                
                // 如果超出距离，跳过后续更新
                if (!isInRange)
                {
                    return;
                }
            }
            else
            {
                // 世界空间模式 - Billboard效果（高性能）
                transform.position = _target.position + Vector3.up * 1.5f;
                transform.rotation = _mainCamera.transform.rotation;
                
                // 确保Canvas始终面向摄像机
                if (canvas != null && canvas.transform != null)
                {
                    canvas.transform.rotation = _mainCamera.transform.rotation;
                }

                // 距离裁剪 - 只禁用渲染组件
                if (canvas != null && canvas.enabled != isInRange)
                {
                    canvas.enabled = isInRange;
                }
                
                if (lineRenderer != null && _showLine)
                {
                    if (lineRenderer.enabled != isInRange)
                    {
                        lineRenderer.enabled = isInRange;
                    }
                }
            }
            
            // 如果超出距离，跳过后续更新
            if (!isInRange)
            {
                return;
            }

            // 距离淡化
            float alpha = Mathf.Clamp01(1 - (distance / _maxDistance));

            // 更新连接线
            if (lineRenderer != null && lineRenderer.enabled && _showLine)
            {
                lineRenderer.SetPosition(0, _playerTransform.position + Vector3.up * 1.0f);
                lineRenderer.SetPosition(1, _target.position);

                // 判断是否为敌人，使用不同的连接线颜色
                Color lineColor;
                if (_dataReference is EnemyData enemyData)
                {
                    // 敌人：使用敌人专用连接线颜色（红色/紫色）
                    lineColor = EnemyInfoFormatter.GetEnemyLineColor(enemyData);
                }
                else
                {
                    // 物品/箱子：使用标记颜色
                    lineColor = _markerColor;
                }

                // 线条淡化
                Color startColor = new Color(lineColor.r, lineColor.g, lineColor.b, alpha * 0.6f);
                Color endColor = new Color(lineColor.r, lineColor.g, lineColor.b, alpha * 0.2f);
                lineRenderer.startColor = startColor;
                lineRenderer.endColor = endColor;
            }

            // 更新敌人距离显示（如果是敌人数据）
            if (_dataReference is EnemyData enemyDataForUpdate && _showDistance)
            {
                // 更新敌人数据中的距离
                enemyDataForUpdate.DistanceToPlayer = distance;
                
                // 更新文本显示（每帧更新）
                if (nameText != null && _config != null)
                {
                    nameText.text = EnemyInfoFormatter.FormatEnemyInfo(enemyDataForUpdate, _showDistance, _config);
                }
            }

            // 文字保持完全不透明
        }

        /// <summary>
        /// 返回对象池
        /// </summary>
        public void ReturnToPool()
        {
            if (ESPMarkerPool.Instance != null)
            {
                ESPMarkerPool.Instance.Return(this);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// 获取数据引用
        /// </summary>
        public object? GetDataReference()
        {
            return _dataReference;
        }

        /// <summary>
        /// 清理
        /// </summary>
        public void Cleanup()
        {
            // 如果Canvas是根节点（ScreenOverlay模式），重新挂回来
            if (canvas != null && canvas.transform.parent != transform)
            {
                canvas.transform.SetParent(transform, false);
                canvas.transform.localPosition = Vector3.zero;
                canvas.transform.localRotation = Quaternion.identity;
            }
            
            // 清理Canvas内容
            if (canvas != null)
            {
                canvas.enabled = false;
            }
            
            // 清理LineRenderer
            if (lineRenderer != null)
            {
                lineRenderer.enabled = false;
            }
            
            _target = null;
            _playerTransform = null;
            _dataReference = null;
            gameObject.SetActive(false);
        }
    }
}
