using System;
using System.Collections.Generic;
using UnityEngine;
using ItemStatsSystem;
using DuckovESPv3.Core.Configuration;
using DuckovESPv3.Core.EventBus;
using DuckovESPv3.Core.Systems.ESP.Events;
using DuckovESPv3.Core.Systems.ESP.Models;
using DuckovESPv3.Core.Systems.ESP.Rendering;
using DuckovESPv3.Infrastructure.Logging;
using DuckovESPv3.Features.QuestMarker.Events;
using DuckovESPv3.Features.QuestMarker.Data;

namespace DuckovESPv3.Core.Systems.ESP
{
    /// <summary>
    /// ESP 系统管理器
    /// 负责管理所有 ESP 标记的创建、更新和销毁
    /// </summary>
    public class ESPSystemManager : MonoBehaviour
    {
        // ===== 依赖注入 =====
        private ESPSystemConfig _config;
        private IEventBus _eventBus;
        private Infrastructure.Logging.ILogger _logger;

        // ===== 数据采集器引用 =====
        private Detection.ILootboxCollectionService? _lootboxCollector;
        private Detection.IWorldItemCollectionService? _worldItemCollector;
        private Services.EnemyCollectionService? _enemyCollector;

        // ===== Inventory 监听器 =====
        private Detection.InventoryMonitor? _inventoryMonitor;

        // ===== 数据追踪器引用（用于更新距离/屏幕坐标） =====
        private Features.QuestMarker.Collectors.EvacuationPointTracker? _evacuationPointTracker;
        private Features.QuestMarker.Collectors.QuestZoneTracker? _questZoneTracker;

        // ===== 标记管理 =====
        private Dictionary<InteractableLootbox, ESPMarker> _lootboxMarkers = new Dictionary<InteractableLootbox, ESPMarker>();
        private Dictionary<ItemStatsSystem.Item, ESPMarker> _itemMarkers = new Dictionary<ItemStatsSystem.Item, ESPMarker>();
        private Dictionary<CharacterMainControl, ESPMarker> _enemyMarkers = new Dictionary<CharacterMainControl, ESPMarker>();
        private Dictionary<string, ESPMarker> _questZoneMarkers = new Dictionary<string, ESPMarker>();
        private Dictionary<string, GameObject> _questZoneGameObjects = new Dictionary<string, GameObject>();
        
        // Quest区域和撤离点（屏幕边缘指示，不使用3D标记）
        private List<EvacuationPointData> _evacuationPoints = new List<EvacuationPointData>();
        private List<QuestZoneData> _questZones = new List<QuestZoneData>();
        
        // ===== 箱子到Inventory映射（用于物品移除时快速查找） =====
        private Dictionary<Inventory, InteractableLootbox> _inventoryToLootbox = new Dictionary<Inventory, InteractableLootbox>();

        // ===== 玩家引用 =====
        private Transform _playerTransform;

        // ===== 颜色配置 =====
        private Dictionary<ItemValueLevel, Color> _qualityColors;
        
        // ===== UI 缩放 =====
        private float _uiScale = 1f;

        // ===== 屏幕标记样式 =====
        private GUIStyle? _edgeIndicatorLabelStyle;

        /// <summary>
        /// 初始化
        /// </summary>
        public void Initialize(ESPSystemConfig config, IEventBus eventBus, Infrastructure.Logging.ILogger logger)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            InitializeColorMap();
            SubscribeEvents();
            
            // 初始化 Inventory 监听器
            _inventoryMonitor = new Detection.InventoryMonitor(_logger);
            _inventoryMonitor.OnItemRemovedFromLootbox += HandleItemRemovedFromLootbox;
            _inventoryMonitor.OnWorldItemPickedUp += HandleWorldItemPickedUp;

            _logger.Info("[ESPSystemManager] ESP 系统已初始化");
        }

        /// <summary>
        /// 设置数据采集器引用（用于刷新标记）
        /// </summary>
        public void SetDataCollectors(Detection.ILootboxCollectionService lootboxCollector, Detection.IWorldItemCollectionService worldItemCollector)
        {
            _lootboxCollector = lootboxCollector;
            _worldItemCollector = worldItemCollector;
            _logger.Info("[ESPSystemManager] 数据采集器引用已设置");
        }

        /// <summary>
        /// 设置敌人采集服务引用
        /// </summary>
        public void SetEnemyCollectionService(Services.EnemyCollectionService enemyCollector)
        {
            _enemyCollector = enemyCollector;
            
            // 订阅敌人事件
            _enemyCollector.OnEnemyAdded += HandleEnemyAdded;
            _enemyCollector.OnEnemyRemoved += HandleEnemyRemoved;
            _enemyCollector.OnEnemyUpdated += HandleEnemyUpdated;
            
            _logger.Info("[ESPSystemManager] 敌人采集器引用已设置，事件已订阅");
        }

        /// <summary>
        /// 设置数据追踪器引用（用于更新距离和屏幕坐标）
        /// </summary>
        public void SetDataTrackers(
            Features.QuestMarker.Collectors.EvacuationPointTracker? evacuationTracker,
            Features.QuestMarker.Collectors.QuestZoneTracker? questTracker)
        {
            _evacuationPointTracker = evacuationTracker;
            _questZoneTracker = questTracker;
            _logger.Info("[ESPSystemManager] 数据追踪器引用已设置");
        }
        
        /// <summary>
        /// 设置 UI 缩放比例（同步到所有标记）
        /// </summary>
        public void SetUIScale(float scale)
        {
            _uiScale = scale;
            
            // 更新所有现有标记的UI缩放
            foreach (var marker in _lootboxMarkers.Values)
            {
                if (marker != null)
                {
                    marker.UpdateUIScale(scale);
                }
            }
            
            foreach (var marker in _itemMarkers.Values)
            {
                if (marker != null)
                {
                    marker.UpdateUIScale(scale);
                }
            }
            
            foreach (var marker in _enemyMarkers.Values)
            {
                if (marker != null)
                {
                    marker.UpdateUIScale(scale);
                }
            }
        }

        /// <summary>
        /// 更新所有现有标记的字体大小
        /// </summary>
        public void UpdateFontSize(int fontSize)
        {
            // 更新所有现有标记的字体大小
            foreach (var marker in _lootboxMarkers.Values)
            {
                if (marker != null)
                {
                    marker.UpdateFontSize(fontSize);
                }
            }
            
            foreach (var marker in _itemMarkers.Values)
            {
                if (marker != null)
                {
                    marker.UpdateFontSize(fontSize);
                }
            }
            
            foreach (var marker in _enemyMarkers.Values)
            {
                if (marker != null)
                {
                    marker.UpdateFontSize(fontSize);
                }
            }
        }

        private void OnEnable()
        {
            // 获取玩家 Transform
            UpdatePlayerTransform();
        }

        private void Update()
        {
            // 定期更新玩家位置
            if (_playerTransform == null || !_playerTransform.gameObject.activeInHierarchy)
            {
                UpdatePlayerTransform();
            }

            // 调用追踪器的Update方法来计算距离和屏幕坐标，然后从trackers拉取最新数据
            try
            {
                _evacuationPointTracker?.Update();
                _questZoneTracker?.Update();
                
                // 【关键】调用Update后，从trackers拉取最新的数据（已计算距离）
                RefreshDataFromTrackers();
            }
            catch (Exception ex)
            {
                _logger.Error($"[ESPSystemManager] 更新追踪器时出错: {ex.Message}");
            }

            // 调试：输出撤离点数量
            if (_evacuationPoints.Count > 0)
            {
                _logger.Debug($"[ESPSystemManager] Update: {_evacuationPoints.Count} evacuation points, first distance: {_evacuationPoints[0].DistanceToPlayer:F1}m");
            }
        }
        
        /// <summary>
        /// 从trackers拉取最新数据（距离、屏幕坐标等）
        /// </summary>
        private void RefreshDataFromTrackers()
        {
            try
            {
                if (_evacuationPointTracker != null)
                {
                    var latestPoints = _evacuationPointTracker.GetEvacuationPoints();
                    if (latestPoints != null)
                    {
                        _evacuationPoints.Clear();
                        _evacuationPoints.AddRange(latestPoints);
                    }
                }
                
                if (_questZoneTracker != null)
                {
                    var latestZones = _questZoneTracker.GetQuestZones();
                    if (latestZones != null)
                    {
                        _questZones.Clear();
                        _questZones.AddRange(latestZones);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"[ESPSystemManager] 从trackers拉取数据时出错: {ex.Message}");
            }
        }

        /// <summary>
        /// 在GUI阶段绘制文字标签和距离标注
        /// </summary>
        private void OnGUI()
        {
            if (_playerTransform == null || !LevelManager.LevelInited || Camera.main == null)
                return;

            Vector2 screenSize = new Vector2(Screen.width, Screen.height);
            Camera mainCamera = Camera.main;

            // 初始化样式（如果需要）
            if (_edgeIndicatorLabelStyle == null)
            {
                _edgeIndicatorLabelStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 12,
                    alignment = TextAnchor.MiddleCenter,
                    fontStyle = FontStyle.Bold
                };
            }

            // 绘制撤离点文字标签
            if (_config.ShowEvacuationPoints)
            {
                _edgeIndicatorLabelStyle.normal.textColor = new Color(0.2f, 1f, 0.2f);
                foreach (var point in _evacuationPoints)
                {
                    DrawEdgeIndicatorLabel(mainCamera, screenSize, point.Position, point.DistanceToPlayer);
                }
            }

            // 绘制任务区域文字标签
            if (_config.ShowQuestZones)
            {
                _edgeIndicatorLabelStyle.normal.textColor = _config.QuestZoneColor;
                foreach (var zone in _questZones)
                {
                    DrawEdgeIndicatorLabel(mainCamera, screenSize, zone.Position, zone.Distance);
                }
            }
        }

        /// <summary>
        /// 绘制单个屏幕标记的文字标签和距离
        /// </summary>
        private void DrawEdgeIndicatorLabel(Camera camera, Vector2 screenSize, Vector3 worldPos, float distance)
        {
            try
            {
                Vector3 screenPos = camera.WorldToScreenPoint(worldPos);
                if (screenPos.z <= 0) return;

                // 【修复】先检查是否在屏幕内（使用翻转前的坐标）
                bool isOnScreen = screenPos.x >= 0 && screenPos.x <= screenSize.x && 
                                  screenPos.y >= 0 && screenPos.y <= screenSize.y;

                // 【修复】翻转 Y 坐标（GUI 坐标系）
                screenPos.y = screenSize.y - screenPos.y;

                // 屏幕内或屏幕外都显示距离标签（如果不超过最大距离）
                if (distance <= _config.QuestZoneMaxDistance)
                {
                    string label = $"{distance:F0}m";
                    
                    if (isOnScreen)
                    {
                        // 屏幕内：在标记下方显示
                        // 【修复】添加边界检查，防止标签在屏幕外显示
                        float labelX = Mathf.Clamp(screenPos.x - 30, 0, screenSize.x - 60);
                        float labelY = Mathf.Clamp(screenPos.y + 20, 0, screenSize.y - 30);
                        
                        GUI.Label(new Rect(labelX, labelY, 60, 30), 
                                 label, _edgeIndicatorLabelStyle);
                    }
                    else
                    {
                        // 屏幕外：在屏幕边缘显示
                        // 【修复】在翻转后的坐标基础上计算方向
                        Vector2 center = new Vector2(screenSize.x / 2, screenSize.y / 2);
                        Vector2 direction = (new Vector2(screenPos.x, screenPos.y) - center).normalized;
                        
                        const float edgeMargin = 80f;
                        Vector2 labelPos;
                        
                        float absX = Mathf.Abs(direction.x);
                        float absY = Mathf.Abs(direction.y);
                        
                        if (absX > absY)
                        {
                            // 左或右边缘
                            labelPos.x = direction.x > 0 ? screenSize.x - edgeMargin : edgeMargin;
                            labelPos.y = center.y + direction.y * (screenSize.x / 2 - edgeMargin) / (absX + 0.001f);
                        }
                        else
                        {
                            // 上或下边缘
                            labelPos.y = direction.y > 0 ? screenSize.y - edgeMargin : edgeMargin;
                            labelPos.x = center.x + direction.x * (screenSize.y / 2 - edgeMargin) / (absY + 0.001f);
                        }
                        
                        // 【修复】确保标签位置在屏幕内
                        labelPos.x = Mathf.Clamp(labelPos.x - 30, 0, screenSize.x - 60);
                        labelPos.y = Mathf.Clamp(labelPos.y - 15, 0, screenSize.y - 30);
                        
                        GUI.Label(new Rect(labelPos.x, labelPos.y, 60, 30), 
                                 label, _edgeIndicatorLabelStyle);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"[ESPSystemManager] 绘制标签失败: {ex.Message}");
            }
        }

        /// <summary>
        /// GL渲染：绘制撤离点和任务区域的屏幕边缘指示器
        /// </summary>
        private void OnPostRender()
        {
            if (_playerTransform == null || Camera.main == null)
                return;

            Camera mainCamera = Camera.main;
            Vector2 screenSize = new Vector2(Screen.width, Screen.height);
            
            // 调试日志
            if (_evacuationPoints.Count > 0 || _questZones.Count > 0)
            {
                _logger.Debug($"[ESPSystemManager] OnPostRender: {_evacuationPoints.Count} evac points, {_questZones.Count} quest zones, ShowEvac={_config.ShowEvacuationPoints}, ShowQuest={_config.ShowQuestZones}");
            }
            
            // 绘制撤离点指示器（绿色）
            if (_config.ShowEvacuationPoints && _evacuationPoints.Count > 0)
            {
                Color evacuationColor = new Color(0.2f, 1f, 0.2f); // 亮绿色
                foreach (var point in _evacuationPoints)
                {
                    DrawScreenEdgeIndicator(mainCamera, screenSize, point.Position, evacuationColor);
                }
            }
            
            // 绘制任务区域指示器（黄色）
            if (_config.ShowQuestZones && _questZones.Count > 0)
            {
                Color questColor = _config.QuestZoneColor;
                foreach (var zone in _questZones)
                {
                    DrawScreenEdgeIndicator(mainCamera, screenSize, zone.Position, questColor);
                }
            }
        }

        /// <summary>
        /// 绘制屏幕边缘指示器（完整实现，参考v2）
        /// </summary>
        private void DrawScreenEdgeIndicator(Camera camera, Vector2 screenSize, Vector3 worldPos, Color color)
        {
            if (_playerTransform == null || camera == null) return;

            Vector3 screenPos = camera.WorldToScreenPoint(worldPos);
            
            // 如果在摄像机后方，不绘制
            if (screenPos.z <= 0)
                return;

            // Unity屏幕坐标Y轴翻转
            screenPos.y = screenSize.y - screenPos.y;

            const float edgeMargin = 60f;
            const float arrowSize = 40f;
            
            bool isOnScreen = screenPos.x >= 0 && screenPos.x <= screenSize.x && 
                              screenPos.y >= 0 && screenPos.y <= screenSize.y;

            if (isOnScreen)
            {
                // 在屏幕内：绘制圆形标记
                DrawCircleGL(new Vector2(screenPos.x, screenPos.y), 40f, color, 24);
            }
            else
            {
                // 在屏幕外：绘制屏幕边缘箭头
                DrawEdgeArrowGL(new Vector2(screenPos.x, screenPos.y), screenSize, 
                               edgeMargin, arrowSize, color);
            }
        }

        /// <summary>
        /// 使用GL绘制圆形（屏幕坐标）
        /// </summary>
        private void DrawCircleGL(Vector2 center, float radius, Color color, int segments)
        {
            GL.PushMatrix();
            GL.LoadOrtho();
            
            Material lineMaterial = new Material(Shader.Find("Hidden/Internal-Colored"));
            if (lineMaterial != null)
            {
                lineMaterial.SetPass(0);
                GL.Begin(GL.LINE_STRIP);
                GL.Color(color);

                float angleStep = 360f / segments * Mathf.Deg2Rad;
                for (int i = 0; i <= segments; i++)
                {
                    float angle = i * angleStep;
                    float x = center.x + radius * Mathf.Cos(angle);
                    float y = center.y + radius * Mathf.Sin(angle);
                    
                    // 转换为GL归一化坐标
                    GL.Vertex3(x / Screen.width, y / Screen.height, 0f);
                }

                GL.End();
            }
            
            GL.PopMatrix();
        }

        /// <summary>
        /// 绘制屏幕边缘箭头（参考v2实现）
        /// </summary>
        private void DrawEdgeArrowGL(Vector2 worldScreenPos, Vector2 screenSize,
            float margin, float arrowSize, Color color)
        {
            Vector2 center = new Vector2(screenSize.x / 2, screenSize.y / 2);
            Vector2 direction = (worldScreenPos - center).normalized;

            Vector2 arrowPos;
            float absX = Mathf.Abs(direction.x);
            float absY = Mathf.Abs(direction.y);

            if (absX > absY)
            {
                // 左或右边缘
                arrowPos.x = direction.x > 0 ? screenSize.x - margin : margin;
                arrowPos.y = center.y + direction.y * (screenSize.x / 2 - margin) / absX;
            }
            else
            {
                // 上或下边缘
                arrowPos.y = direction.y > 0 ? screenSize.y - margin : margin;
                arrowPos.x = center.x + direction.x * (screenSize.y / 2 - margin) / absY;
            }

            // 绘制箭头
            DrawArrowGL(arrowPos, direction, arrowSize, color);
        }

        /// <summary>
        /// 使用GL绘制箭头指向（屏幕坐标）
        /// </summary>
        private void DrawArrowGL(Vector2 position, Vector2 direction, float size, Color color)
        {
            GL.PushMatrix();
            GL.LoadOrtho();

            Material lineMaterial = new Material(Shader.Find("Hidden/Internal-Colored"));
            if (lineMaterial != null)
            {
                lineMaterial.SetPass(0);
                GL.Begin(GL.TRIANGLES);
                GL.Color(color);

                // 计算箭头顶点（基于方向角度计算）
                float angle = Mathf.Atan2(direction.y, direction.x);
                Vector2 tip = position;
                Vector2 left = position + new Vector2(
                    Mathf.Cos(angle + 2.5f) * size,
                    Mathf.Sin(angle + 2.5f) * size
                );
                Vector2 right = position + new Vector2(
                    Mathf.Cos(angle - 2.5f) * size,
                    Mathf.Sin(angle - 2.5f) * size
                );

                // 转换为GL归一化坐标并绘制
                GL.Vertex3(tip.x / Screen.width, tip.y / Screen.height, 0);
                GL.Vertex3(left.x / Screen.width, left.y / Screen.height, 0);
                GL.Vertex3(right.x / Screen.width, right.y / Screen.height, 0);

                GL.End();
            }

            GL.PopMatrix();
        }

        /// <summary>
        /// 绘制屏幕边缘指示器（完整实现，参考v2）
        /// </summary>
        private void DrawEdgeIndicator(Camera camera, Vector2 screenSize, Vector3 worldPos, string label, Color color)
        {
            if (_playerTransform == null) return;

            Vector3 screenPos = camera.WorldToScreenPoint(worldPos);
            float distance = Vector3.Distance(_playerTransform.position, worldPos);

            // 如果在屏幕后方或距离太远，不绘制
            if (screenPos.z < 0 || distance > _config.QuestZoneMaxDistance)
                return;

            bool isOnScreen = screenPos.x >= 0 && screenPos.x <= screenSize.x && 
                              screenPos.y >= 0 && screenPos.y <= screenSize.y;

            if (!isOnScreen)
            {
                // 计算箭头位置（屏幕边缘）
                Vector2 direction = new Vector2(screenPos.x - screenSize.x / 2, screenPos.y - screenSize.y / 2).normalized;
                float margin = 50f;
                
                // 计算箭头在屏幕边缘的位置
                Vector2 arrowPos = new Vector2(screenSize.x / 2, screenSize.y / 2);
                
                // 计算与屏幕边界的交点
                float tX = Mathf.Abs(direction.x) > 0.001f ? ((screenSize.x / 2 - margin) / Mathf.Abs(direction.x)) : float.MaxValue;
                float tY = Mathf.Abs(direction.y) > 0.001f ? ((screenSize.y / 2 - margin) / Mathf.Abs(direction.y)) : float.MaxValue;
                float t = Mathf.Min(tX, tY);
                
                arrowPos += direction * t;

                // 绘制箭头
                DrawArrowGL(arrowPos, direction, 20f, color);

                // 绘制文字标签和距离（使用GUI，不是GL）
                // 注意：这需要在OnGUI中处理，这里只记录位置
            }
        }

        /// <summary>
        /// 更新玩家 Transform 引用
        /// </summary>
        private void UpdatePlayerTransform()
        {
            var levelManager = LevelManager.Instance;
            if (levelManager != null && levelManager.MainCharacter != null)
            {
                _playerTransform = levelManager.MainCharacter.transform;
            }
        }

        /// <summary>
        /// 初始化颜色映射
        /// </summary>
        private void InitializeColorMap()
        {
            _qualityColors = new Dictionary<ItemValueLevel, Color>
            {
                { ItemValueLevel.White, new Color(0.8f, 0.8f, 0.8f) },
                { ItemValueLevel.Green, new Color(0.2f, 1f, 0.2f) },
                { ItemValueLevel.Blue, new Color(0.3f, 0.5f, 1f) },
                { ItemValueLevel.Purple, new Color(0.8f, 0.3f, 1f) },
                { ItemValueLevel.Orange, new Color(1f, 0.6f, 0f) },
                { ItemValueLevel.LightRed, new Color(1f, 0.5f, 0.5f) },
                { ItemValueLevel.Red, new Color(1f, 0.2f, 0.2f) }
            };
        }

        /// <summary>
        /// 订阅事件
        /// </summary>
        private void SubscribeEvents()
        {
            _eventBus.Subscribe<LootboxDiscoveredEvent>(OnLootboxDiscovered);
            _eventBus.Subscribe<LootboxRemovedEvent>(OnLootboxRemoved);
            _eventBus.Subscribe<WorldItemDiscoveredEvent>(OnWorldItemDiscovered);
            _eventBus.Subscribe<WorldItemRemovedEvent>(OnWorldItemRemoved);
            _eventBus.Subscribe<QuestZonesUpdatedEvent>(OnQuestZonesUpdated);
            _eventBus.Subscribe<EvacuationPointsUpdatedEvent>(OnEvacuationPointsUpdated);
        }

        /// <summary>
        /// 取消订阅事件
        /// </summary>
        private void UnsubscribeEvents()
        {
            _eventBus.Unsubscribe<LootboxDiscoveredEvent>(OnLootboxDiscovered);
            _eventBus.Unsubscribe<LootboxRemovedEvent>(OnLootboxRemoved);
            _eventBus.Unsubscribe<WorldItemDiscoveredEvent>(OnWorldItemDiscovered);
            _eventBus.Unsubscribe<WorldItemRemovedEvent>(OnWorldItemRemoved);
            _eventBus.Unsubscribe<QuestZonesUpdatedEvent>(OnQuestZonesUpdated);
            _eventBus.Unsubscribe<EvacuationPointsUpdatedEvent>(OnEvacuationPointsUpdated);
        }

        /// <summary>
        /// 箱子发现事件处理
        /// </summary>
        private void OnLootboxDiscovered(LootboxDiscoveredEvent evt)
        {
            if (!_config.Enable3DESP)
            {
                _logger.Debug("[ESPSystemManager] 3D ESP 未启用，跳过箱子");
                return;
            }
            
            // 记录检测到箱子
            _logger.Info($"[ESPSystemManager] 收到箱子发现事件: {evt.Data.Lootbox.name}, 物品数: {evt.Data.Items.Count}");
            
            if (!ShouldShowLootbox(evt.Data))
            {
                _logger.Debug($"[ESPSystemManager] 箱子被过滤: {evt.Data.Lootbox.name}, 原因: 品质={evt.Data.MaxItemQuality}, 物品数={evt.Data.Items.Count}");
                return;
            }

            CreateLootboxMarker(evt.Data);
        }

        /// <summary>
        /// 箱子移除事件处理
        /// </summary>
        private void OnLootboxRemoved(LootboxRemovedEvent evt)
        {
            RemoveLootboxMarker(evt.Data.Lootbox);
        }

        /// <summary>
        /// 物品发现事件处理
        /// </summary>
        private void OnWorldItemDiscovered(WorldItemDiscoveredEvent evt)
        {
            if (!_config.Enable3DESP) return;
            if (!ShouldShowWorldItem(evt.Data)) return;

            CreateWorldItemMarker(evt.Data);
        }

        /// <summary>
        /// 物品移除事件处理
        /// </summary>
        private void OnWorldItemRemoved(WorldItemRemovedEvent evt)
        {
            RemoveWorldItemMarker(evt.Data.Item);
        }

        /// <summary>
        /// Quest区域更新事件处理（存储数据，用于屏幕边缘指示）
        /// </summary>
        private void OnQuestZonesUpdated(QuestZonesUpdatedEvent evt)
        {
            _questZones.Clear();
            // 注意：不在此处检查ShowQuestZones，始终存储数据
            if (evt.QuestZones != null)
            {
                // 不做距离过滤，让所有任务区域都显示
                // 用户可以通过屏幕边缘箭头看到远处的任务区域
                _questZones.AddRange(evt.QuestZones);
                _logger.Info($"[ESPSystemManager] Quest zones updated: {_questZones.Count} zones for edge indicators");
            }
            else
            {
                _logger.Debug("[ESPSystemManager] Quest zones event but data is null");
            }
        }

        /// <summary>
        /// 撤离点更新事件处理（存储数据，用于屏幕边缘指示）
        /// </summary>
        private void OnEvacuationPointsUpdated(EvacuationPointsUpdatedEvent evt)
        {
            _evacuationPoints.Clear();
            // 注意：不在此处检查ShowEvacuationPoints，始终存储数据
            // 这样即使用户后来启用，也能立即显示
            if (evt.EvacuationPoints != null)
            {
                _evacuationPoints.AddRange(evt.EvacuationPoints);
                _logger.Info($"[ESPSystemManager] Evacuation points updated: {_evacuationPoints.Count} points for edge indicators");
            }
            else
            {
                _logger.Debug("[ESPSystemManager] Evacuation points event but data is null");
            }
        }

        /// <summary>
        /// 判断是否应该显示箱子
        /// </summary>
        private bool ShouldShowLootbox(LootboxData data)
        {
            // 空箱子过滤
            if (!_config.ShowEmptyBoxes && (data.Items == null || data.Items.Count == 0))
                return false;

            // 检查箱子中是否有未录入的钥匙
            bool hasUnregisteredKey = false;
            if (data.Items != null)
            {
                foreach (var item in data.Items)
                {
                    if (item != null && Utils.KeyItemHelper.IsUnregisteredKey(item))
                    {
                        hasUnregisteredKey = true;
                        break;
                    }
                }
            }

            // 如果箱子中只有未录入的钥匙，根据配置决定是否显示
            if (hasUnregisteredKey)
            {
                // 如果不显示未录入的钥匙，检查是否还有其他物品
                if (!_config.ShowNotWishlistedKeys)
                {
                    // 检查是否有非钥匙物品
                    bool hasOtherItems = false;
                    if (data.Items != null)
                    {
                        foreach (var item in data.Items)
                        {
                            if (item != null && !Utils.KeyItemHelper.IsKey(item))
                            {
                                hasOtherItems = true;
                                break;
                            }
                        }
                    }

                    // 如果只有未录入的钥匙，则不显示
                    if (!hasOtherItems)
                        return false;
                }
            }

            // 如果没有启用任何特定过滤器，则使用品质过滤
            bool hasAnyFilter = _config.ShowByValue || _config.ShowWishlistedItems || 
                               _config.ShowQuestItems || _config.ShowBuildingMaterials;
            
            if (!hasAnyFilter)
            {
                // 默认：仅品质过滤
                return (int)data.MaxItemQuality >= _config.MinQualityFilter3D;
            }
            
            // 启用了特定过滤器：任意一个条件满足即显示（或逻辑）
            bool matchesFilter = false;
            
            if (_config.ShowByValue && (int)data.MaxItemQuality >= _config.MinQualityFilter3D)
                matchesFilter = true;
            
            if (_config.ShowWishlistedItems && data.HasWishlistedItems)
                matchesFilter = true;
            
            if (_config.ShowQuestItems && data.HasQuestItems)
                matchesFilter = true;
            
            if (_config.ShowBuildingMaterials && data.HasBuildingRequiredItems)
                matchesFilter = true;
            
            return matchesFilter;
        }

        /// <summary>
        /// 判断是否应该显示地面物品
        /// </summary>
        private bool ShouldShowWorldItem(WorldItemData data)
        {
            // 检查是否为未录入的钥匙
            if (data.Item != null && Utils.KeyItemHelper.IsUnregisteredKey(data.Item))
            {
                // 根据配置决定是否显示未录入的钥匙
                return _config.ShowNotWishlistedKeys;
            }

            // 如果没有启用任何特定过滤器，则使用品质过滤
            bool hasAnyFilter = _config.ShowByValue || _config.ShowWishlistedItems || 
                               _config.ShowQuestItems || _config.ShowBuildingMaterials;
            
            if (!hasAnyFilter)
            {
                // 默认：仅品质过滤
                return (int)data.Quality >= _config.MinQualityFilter3D;
            }
            
            // 启用了特定过滤器：任意一个条件满足即显示（或逻辑）
            bool matchesFilter = false;
            
            if (_config.ShowByValue && (int)data.Quality >= _config.MinQualityFilter3D)
                matchesFilter = true;
            
            if (_config.ShowWishlistedItems && data.IsWishlisted)
                matchesFilter = true;
            
            if (_config.ShowQuestItems && data.IsQuestRequired)
                matchesFilter = true;
            
            if (_config.ShowBuildingMaterials && data.IsBuildingRequired)
                matchesFilter = true;
            
            return matchesFilter;
        }

        /// <summary>
        /// 创建箱子标记
        /// </summary>
        private void CreateLootboxMarker(LootboxData data)
        {
            try
            {
                if (data.Lootbox == null || _lootboxMarkers.ContainsKey(data.Lootbox) || _playerTransform == null)
                    return;

                var marker = ESPMarkerPool.Instance?.Get();
                if (marker == null)
                {
                    _logger.Warning("[ESPSystemManager] 无法从对象池获取标记");
                    return;
                }

                Color color = GetColorForQuality(data.MaxItemQuality);

                // Quest物品使用金色高亮
                if (data.HasQuestItems && _config.ShowQuestItems)
                {
                    color = _config.QuestItemColor;
                }

                // 获取渲染模式
                ESPRenderMode renderMode = (ESPRenderMode)_config.ESPRenderMode;

                marker.Initialize(
                    data.Lootbox.transform,
                    _playerTransform,
                    color,
                    _config.MaxESPDistance,
                    _config.ShowConnectLine,
                    _config.ShowDistance,
                    data,
                    _config.ESPFontSize,
                    _uiScale,
                    renderMode
                );

                _lootboxMarkers[data.Lootbox] = marker;
                
                // 开始监听箱子 Inventory 变化
                if (data.Inventory != null && _inventoryMonitor != null)
                {
                    _inventoryToLootbox[data.Inventory] = data.Lootbox;
                    _inventoryMonitor.StartMonitoringLootbox(data.Lootbox, data.Inventory);
                }
                
                _logger.Debug($"[ESPSystemManager] 创建箱子标记: {data.Lootbox.name}");
            }
            catch (Exception ex)
            {
                _logger.Error($"[ESPSystemManager] 创建箱子标记失败: {ex.Message}\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// 创建地面物品标记
        /// </summary>
        private void CreateWorldItemMarker(WorldItemData data)
        {
            if (data.Item == null || data.Agent == null) return;
            if (_itemMarkers.ContainsKey(data.Item)) return;
            if (_playerTransform == null) return;

            var marker = ESPMarkerPool.Instance?.Get();
            if (marker == null)
            {
                _logger.Warning("[ESPSystemManager] 无法从对象池获取标记");
                return;
            }

            Color color = GetColorForQuality(data.Quality);
            
            // Quest物品使用金色高亮
            if (data.IsQuestRequired && _config.ShowQuestItems)
            {
                color = _config.QuestItemColor;
            }
            
            // 调试日志：检查颜色
            _logger.Info($"[ESP调试] 物品={data.DisplayName} 品质={data.Quality}({(int)data.Quality}) 颜色=RGB({color.r:F2},{color.g:F2},{color.b:F2})");

            // 获取渲染模式
            ESPRenderMode renderMode = (ESPRenderMode)_config.ESPRenderMode;

            marker.Initialize(
                data.Agent.transform,
                _playerTransform,
                color,
                _config.MaxESPDistance,
                _config.ShowConnectLine,
                _config.ShowDistance,
                data,
                _config.ESPFontSize,
                _uiScale,
                renderMode
            );

            _itemMarkers[data.Item] = marker;
            
            // 开始监听地面物品
            if (_inventoryMonitor != null)
            {
                _inventoryMonitor.StartMonitoringWorldItem(data.Item);
            }
            
            _logger.Debug($"[ESPSystemManager] 创建物品标记: {data.DisplayName}");
        }

        /// <summary>
        /// 移除箱子标记
        /// </summary>
        private void RemoveLootboxMarker(InteractableLootbox lootbox)
        {
            if (lootbox == null) return;
            if (!_lootboxMarkers.TryGetValue(lootbox, out var marker)) return;

            marker.ReturnToPool();
            _lootboxMarkers.Remove(lootbox);
            _logger.Debug($"[ESPSystemManager] 移除箱子标记: {lootbox.name}");
        }

        /// <summary>
        /// 移除地面物品标记
        /// </summary>
        private void RemoveWorldItemMarker(ItemStatsSystem.Item item)
        {
            if (item == null) return;
            if (!_itemMarkers.TryGetValue(item, out var marker)) return;

            marker.ReturnToPool();
            _itemMarkers.Remove(item);
            _logger.Debug($"[ESPSystemManager] 移除物品标记: {item.DisplayName}");
        }

        /// <summary>
        /// 根据品质获取颜色
        /// </summary>
        private Color GetColorForQuality(ItemValueLevel quality)
        {
            return _qualityColors.TryGetValue(quality, out var color) ? color : Color.white;
        }

        /// <summary>
        /// 清理所有标记
        /// </summary>
        public void ClearAllMarkers()
        {
            foreach (var marker in _lootboxMarkers.Values)
            {
                marker.ReturnToPool();
            }
            _lootboxMarkers.Clear();

            foreach (var marker in _itemMarkers.Values)
            {
                marker.ReturnToPool();
            }
            _itemMarkers.Clear();

            foreach (var marker in _enemyMarkers.Values)
            {
                marker.ReturnToPool();
            }
            _enemyMarkers.Clear();

            // Quest区域和撤离点使用GL渲染，无需清理标记
            _questZones.Clear();
            _evacuationPoints.Clear();

            _logger.Info("[ESPSystemManager] 已清理所有标记");
        }
        
        /// <summary>
        /// 更新所有敌人标记的最大距离
        /// </summary>
        public void UpdateEnemyMarkersDistance(float maxDistance)
        {
            foreach (var marker in _enemyMarkers.Values)
            {
                marker.UpdateMaxDistance(maxDistance);
            }
            _logger.Debug($"[ESPSystemManager] 已更新所有敌人标记距离: {maxDistance}m");
        }

        /// <summary>
        /// 刷新所有标记（重新应用过滤条件）
        /// 注意：敌人标记不受Enable3DESP控制，由EnableEnemyESP独立控制
        /// </summary>
        public void RefreshAllMarkers()
        {
            _logger.Info("[ESPSystemManager] 正在刷新所有标记...");
            
            int removedLootboxes = 0;
            int removedItems = 0;
            int removedEnemies = 0;
            int addedLootboxes = 0;
            int addedItems = 0;
            int addedEnemies = 0;

            // ===== 第一步：移除不再符合条件的标记 =====
            
            // === 物品/箱子标记（受Enable3DESP控制）===
            if (_config.Enable3DESP)
            {
                // 检查箱子标记
                var lootboxesToRemove = new System.Collections.Generic.List<InteractableLootbox>();
                foreach (var kvp in _lootboxMarkers)
                {
                    var lootbox = kvp.Key;
                    var marker = kvp.Value;
                    
                    if (marker.GetDataReference() is LootboxData data)
                    {
                        if (!ShouldShowLootbox(data))
                        {
                            lootboxesToRemove.Add(lootbox);
                        }
                    }
                }

                // 移除箱子标记
                foreach (var lootbox in lootboxesToRemove)
                {
                    if (_lootboxMarkers.TryGetValue(lootbox, out var marker))
                    {
                        marker.ReturnToPool();
                        _lootboxMarkers.Remove(lootbox);
                        removedLootboxes++;
                    }
                }

                // 检查物品标记
                var itemsToRemove = new System.Collections.Generic.List<Item>();
                foreach (var kvp in _itemMarkers)
                {
                    var item = kvp.Key;
                    var marker = kvp.Value;
                    
                    if (marker.GetDataReference() is WorldItemData data)
                    {
                        if (!ShouldShowWorldItem(data))
                        {
                            itemsToRemove.Add(item);
                        }
                    }
                }

                // 移除物品标记
                foreach (var item in itemsToRemove)
                {
                    if (_itemMarkers.TryGetValue(item, out var marker))
                    {
                        marker.ReturnToPool();
                        _itemMarkers.Remove(item);
                        removedItems++;
                    }
                }
            }
            else
            {
                // Enable3DESP=false：移除所有物品/箱子标记
                foreach (var marker in _lootboxMarkers.Values)
                {
                    marker.ReturnToPool();
                }
                removedLootboxes = _lootboxMarkers.Count;
                _lootboxMarkers.Clear();

                foreach (var marker in _itemMarkers.Values)
                {
                    marker.ReturnToPool();
                }
                removedItems = _itemMarkers.Count;
                _itemMarkers.Clear();
            }

            // === 敌人标记（受EnableEnemyESP独立控制）===
            if (!_config.EnableEnemyESP)
            {
                // EnableEnemyESP=false：移除所有敌人标记
                foreach (var marker in _enemyMarkers.Values)
                {
                    marker.ReturnToPool();
                }
                removedEnemies = _enemyMarkers.Count;
                _enemyMarkers.Clear();
            }

            // ===== 第二步：添加新符合条件的标记 =====
            
            // === 添加物品/箱子标记（仅当Enable3DESP=true）===
            if (_config.Enable3DESP)
            {
                // 添加箱子标记
                if (_lootboxCollector != null)
                {
                    foreach (var data in _lootboxCollector.GetAllTrackedLootboxes())
                    {
                        if (data.Lootbox == null)
                            continue;

                        // 如果标记不存在且现在符合条件，创建新标记
                        if (!_lootboxMarkers.ContainsKey(data.Lootbox) && ShouldShowLootbox(data))
                        {
                            CreateLootboxMarker(data);
                            addedLootboxes++;
                        }
                    }
                }

                // 添加物品标记
                if (_worldItemCollector != null)
                {
                    foreach (var kvp in _worldItemCollector.GetTrackedWorldItems())
                    {
                        var item = kvp.Key;
                        var data = kvp.Value;

                        if (item == null)
                            continue;

                        // 如果标记不存在且现在符合条件，创建新标记
                        if (!_itemMarkers.ContainsKey(item) && ShouldShowWorldItem(data))
                        {
                            CreateWorldItemMarker(data);
                            addedItems++;
                        }
                    }
                }
            }

            // === 添加敌人标记（仅当EnableEnemyESP=true）===
            if (_config.EnableEnemyESP && _enemyCollector != null)
            {
                foreach (var enemyData in _enemyCollector.GetAllEnemies())
                {
                    if (enemyData.Character == null)
                        continue;

                    // 如果标记不存在，创建新标记
                    if (!_enemyMarkers.ContainsKey(enemyData.Character))
                    {
                        var marker = CreateEnemyMarker(enemyData);
                        if (marker != null)
                        {
                            _enemyMarkers[enemyData.Character] = marker;
                            addedEnemies++;
                        }
                    }
                }
            }

            _logger.Info($"[ESPSystemManager] ✓ 刷新完成：移除 {removedLootboxes} 箱子/{removedItems} 物品/{removedEnemies} 敌人，添加 {addedLootboxes} 箱子/{addedItems} 物品/{addedEnemies} 敌人");
        }

        /// <summary>
        /// 更新所有标记的显示距离
        /// </summary>
        public void UpdateAllMarkersMaxDistance(float maxDistance)
        {
            foreach (var marker in _lootboxMarkers.Values)
            {
                marker.UpdateMaxDistance(maxDistance);
            }
            
            foreach (var marker in _itemMarkers.Values)
            {
                marker.UpdateMaxDistance(maxDistance);
            }
        }

        /// <summary>
        /// 更新所有标记的显示选项
        /// </summary>
        public void UpdateAllMarkersDisplayOptions(bool showLine, bool showDistance)
        {
            foreach (var marker in _lootboxMarkers.Values)
            {
                marker.UpdateDisplayOptions(showLine, showDistance);
            }
            
            foreach (var marker in _itemMarkers.Values)
            {
                marker.UpdateDisplayOptions(showLine, showDistance);
            }
        }

        /// <summary>
        /// 处理箱子中物品被移除事件
        /// </summary>
        private void HandleItemRemovedFromLootbox(Inventory inventory, ItemStatsSystem.Item removedItem, int position)
        {
            try
            {
                // 查找对应的箱子
                if (!_inventoryToLootbox.TryGetValue(inventory, out var lootbox))
                {
                    _logger.Debug($"[ESPSystemManager] 无法找到Inventory对应的箱子");
                    return;
                }

                // 检查箱子是否为空
                bool isEmpty = true;
                foreach (var item in inventory.Content)
                {
                    if (item != null)
                    {
                        isEmpty = false;
                        break;
                    }
                }

                if (isEmpty)
                {
                    // 箱子已空，移除整个箱子标记
                    _logger.Info($"[ESPSystemManager] 箱子已空，移除标记: {lootbox.name}");
                    RemoveLootboxMarker(lootbox);
                    _inventoryMonitor?.StopMonitoringLootbox(inventory);
                    
                    // 发布箱子变为空事件（用于小地图标记更新）
                    var lootboxData = _lootboxCollector?.GetLootboxData(lootbox);
                    if (lootboxData != null)
                    {
                        _eventBus.Publish(new Events.LootboxContentChangedEvent(lootboxData, Events.ChangeReason.BecameEmpty));
                        _logger.Debug($"[ESPSystemManager] 已发布箱子变空事件");
                    }
                }
                else
                {
                    // 箱子还有物品，刷新标记内容
                    _logger.Debug($"[ESPSystemManager] 箱子物品变化，刷新标记: {lootbox.name}");
                    RefreshLootboxMarker(lootbox, inventory);
                    
                    // 发布箱子内容变更事件（用于小地图标记更新）
                    var lootboxData = _lootboxCollector?.GetLootboxData(lootbox);
                    if (lootboxData != null)
                    {
                        _eventBus.Publish(new Events.LootboxContentChangedEvent(lootboxData, Events.ChangeReason.ItemRemoved));
                        _logger.Debug($"[ESPSystemManager] 已发布箱子内容变更事件（物品移除）");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"[ESPSystemManager] 处理箱子物品移除失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 处理地面物品被捡起事件
        /// </summary>
        private void HandleWorldItemPickedUp(ItemStatsSystem.Item pickedItem)
        {
            try
            {
                if (_itemMarkers.ContainsKey(pickedItem))
                {
                    _logger.Info($"[ESPSystemManager] 地面物品被捡起，移除标记: {pickedItem.DisplayName}");
                    RemoveWorldItemMarker(pickedItem);
                    _inventoryMonitor?.StopMonitoringWorldItem(pickedItem);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"[ESPSystemManager] 处理地面物品捡起失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 刷新箱子标记内容（物品列表变化时）
        /// </summary>
        private void RefreshLootboxMarker(InteractableLootbox lootbox, Inventory inventory)
        {
            try
            {
                if (!_lootboxMarkers.TryGetValue(lootbox, out var marker))
                {
                    return;
                }

                // 重新收集箱子数据
                var items = new List<ItemStatsSystem.Item>();
                ItemValueLevel maxQuality = ItemValueLevel.White;

                foreach (var item in inventory.Content)
                {
                    if (item == null) continue;
                    items.Add(item);

                    // 获取物品品质
                    var quality = DuckovESPv3.Utils.ItemQualityUtil.GetItemValueLevel(item);
                    if (quality > maxQuality)
                    {
                        maxQuality = quality;
                    }
                }

                // 创建新的 LootboxData
                var lootboxData = new LootboxData
                {
                    Lootbox = lootbox,
                    Inventory = inventory,
                    Position = lootbox.transform.position,
                    Items = items,
                    MaxItemQuality = maxQuality
                };

                // 重新初始化标记（更新显示内容）
                Color color = GetColorForQuality(maxQuality);
                ESPRenderMode renderMode = (ESPRenderMode)_config.ESPRenderMode;

                marker.Initialize(
                    lootbox.transform,
                    _playerTransform,
                    color,
                    _config.MaxESPDistance,
                    _config.ShowConnectLine,
                    _config.ShowDistance,
                    lootboxData,
                    _config.ESPFontSize,
                    _uiScale,
                    renderMode
                );

                _logger.Debug($"[ESPSystemManager] 已刷新箱子标记: {lootbox.name}, 剩余物品: {items.Count}");
            }
            catch (Exception ex)
            {
                _logger.Error($"[ESPSystemManager] 刷新箱子标记失败: {ex.Message}");
            }
        }

        // ===== 敌人事件处理 =====

        /// <summary>
        /// 处理敌人发现事件
        /// </summary>
        private void HandleEnemyAdded(EnemyData enemyData)
        {
            // 检查是否启用敌人ESP
            if (!_config.EnableEnemyESP)
                return;

            try
            {
                if (enemyData.Character == null)
                {
                    _logger.Warning("[ESPSystemManager] 敌人角色为null，跳过创建标记");
                    return;
                }

                // 检查是否已存在标记
                if (_enemyMarkers.ContainsKey(enemyData.Character))
                {
                    _logger.Warning($"[ESPSystemManager] 敌人标记已存在: {enemyData.Name}");
                    return;
                }

                // 创建敌人标记
                var marker = CreateEnemyMarker(enemyData);
                if (marker != null)
                {
                    _enemyMarkers[enemyData.Character] = marker;
                    _logger.Info($"[ESPSystemManager] 已创建敌人标记: {enemyData.Name}, 距离: {enemyData.DistanceToPlayer:F1}m");
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"[ESPSystemManager] 处理敌人发现事件失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 处理敌人死亡事件
        /// </summary>
        private void HandleEnemyRemoved(CharacterMainControl character)
        {
            try
            {
                if (_enemyMarkers.TryGetValue(character, out var marker))
                {
                    marker.ReturnToPool();
                    _enemyMarkers.Remove(character);
                    _logger.Info($"[ESPSystemManager] 已移除敌人标记: {character.name}");
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"[ESPSystemManager] 处理敌人死亡事件失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 处理敌人数据更新事件（血量变化）
        /// </summary>
        private void HandleEnemyUpdated(EnemyData enemyData)
        {
            try
            {
                if (_enemyMarkers.TryGetValue(enemyData.Character, out var marker))
                {
                    // 更新标记文本（血量、距离等）
                    marker.UpdateEnemyText(enemyData);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"[ESPSystemManager] 处理敌人更新事件失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 创建敌人标记
        /// </summary>
        private ESPMarker? CreateEnemyMarker(EnemyData enemyData)
        {
            try
            {
                var marker = ESPMarkerPool.Instance?.Get();
                if (marker == null)
                {
                    _logger.Error("[ESPSystemManager] 无法从对象池获取标记");
                    return null;
                }

                Color enemyColor = Formatting.EnemyInfoFormatter.GetEnemyColor(enemyData);
                ESPRenderMode renderMode = (ESPRenderMode)_config.ESPRenderMode;

                // 使用独立的敌人连接线配置
                marker.Initialize(
                    enemyData.Character.transform,
                    _playerTransform,
                    enemyColor,
                    _config.MaxEnemyESPDistance,  // 使用敌人专用距离
                    _config.EnableEnemyLines,      // 使用敌人专用连接线配置
                    _config.ShowDistance,
                    enemyData,
                    _config.ESPFontSize,
                    _uiScale,
                    renderMode,
                    _config  // 传入配置以控制敌人信息显示
                );

                return marker;
            }
            catch (Exception ex)
            {
                _logger.Error($"[ESPSystemManager] 创建敌人标记失败: {ex.Message}");
                return null;
            }
        }

        private void OnDisable()
        {
            UnsubscribeEvents();
            
            // 取消敌人事件订阅
            if (_enemyCollector != null)
            {
                _enemyCollector.OnEnemyAdded -= HandleEnemyAdded;
                _enemyCollector.OnEnemyRemoved -= HandleEnemyRemoved;
                _enemyCollector.OnEnemyUpdated -= HandleEnemyUpdated;
            }
            
            // 清理 Inventory 监听器
            if (_inventoryMonitor != null)
            {
                _inventoryMonitor.OnItemRemovedFromLootbox -= HandleItemRemovedFromLootbox;
                _inventoryMonitor.OnWorldItemPickedUp -= HandleWorldItemPickedUp;
                _inventoryMonitor.Dispose();
                _inventoryMonitor = null;
            }
            
            ClearAllMarkers();
        }

        private void OnDestroy()
        {
            UnsubscribeEvents();
            
            // 取消敌人事件订阅
            if (_enemyCollector != null)
            {
                _enemyCollector.OnEnemyAdded -= HandleEnemyAdded;
                _enemyCollector.OnEnemyRemoved -= HandleEnemyRemoved;
                _enemyCollector.OnEnemyUpdated -= HandleEnemyUpdated;
            }
            
            // 清理 Inventory 监听器
            if (_inventoryMonitor != null)
            {
                _inventoryMonitor.OnItemRemovedFromLootbox -= HandleItemRemovedFromLootbox;
                _inventoryMonitor.OnWorldItemPickedUp -= HandleWorldItemPickedUp;
                _inventoryMonitor.Dispose();
                _inventoryMonitor = null;
            }
            
            ClearAllMarkers();
        }
    }
}
