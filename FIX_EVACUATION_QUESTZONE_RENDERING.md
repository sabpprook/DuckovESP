# 撤离点与任务区域屏幕标识渲染修复方案

## 一、问题现象总结

### 1. 撤离点屏幕标识（绿色箭头）
- **v2**: 正常工作，显示绿色箭头和圆形标记，屏幕边缘箭头指示清晰
- **v3**: 不能渲染

### 2. 任务区域屏幕标识（黄色箭头）
- **v2**: 正常工作，显示黄色箭头和圆形标记，屏幕边缘箭头指示清晰，小地图标点可见
- **v3**: 
  - 不能渲染
  - 任务区域获取有问题

### 3. 小地图标点
- **v2**: 可正常显示
- **v3**: 相应功能缺失

---

## 二、v2与v3的实现方式对比

### v2架构

#### 数据采集
```
CheatSystem.cs
├─ GetEvacuationPoints()        // 获取撤离点（缓存机制）
│  └─ FindObjectsOfType<SimplePointOfInterest>()
│  └─ 根据名称判断 (MapElement/Exit)
│  └─ 返回 List<(Vector3, float)> - 位置+距离
│
└─ GetQuestZones()             // 获取任务区域
   └─ FindObjectsOfType<MonoBehaviour>() 
   └─ 根据类型名反射判断（QuestTask_ReachLocation/QuestTask_TaskEvent）
   └─ 返回 Dictionary<MonoBehaviour, QuestZoneMarkerData>
```

#### 渲染实现
```
ModBehaviour.cs

OnGUI() 中:
├─ DrawEvacuationIndicatorsText()     // 绘制撤离点文字+距离
├─ DrawQuestZoneIndicatorsText()      // 绘制任务区域文字+距离
└─ 小地图标点创建（CreateQuestZoneMiniMapMarker）

OnRenderObject() 中:
├─ DrawEvacuationIndicatorsGL()       // 使用GL绘制撤离点箭头/圆形
└─ DrawQuestZoneIndicatorsGL()        // 使用GL绘制任务区域箭头/圆形
```

#### 关键实现细节
- **屏幕坐标转换**: `screenPos.y = screenHeight - screenPos.y`（Unity Y轴翻转）
- **GL材质**: 使用 `Shader.Find("Hidden/Internal-Colored")` 和 `GL.LoadOrtho()`
- **圆形绘制**: 使用 `GL.LINE_STRIP` 和三角函数计算圆周顶点
- **箭头位置计算**: 根据屏幕外点的方向，计算与屏幕边界的交点
- **距离显示**: 在屏幕标记上显示到玩家的距离

---

### v3架构 - 存在的问题

#### 数据采集分离
```
QuestMarker/Collectors/
├─ EvacuationPointTracker.cs     // 撤离点追踪
│  └─ 发布 EvacuationPointsUpdatedEvent
│
└─ QuestZoneTracker.cs          // 任务区域追踪
   └─ 发布 QuestZonesUpdatedEvent
```

#### 渲染实现问题
```
ESPSystemManager.cs

OnPostRender() 中:
├─ DrawEdgeIndicator()           // 函数存在但实现不完整
├─ DrawArrowGL()                 // 实现质量低
└─ 问题:
   ├─ 没有屏幕坐标系转换（Y轴翻转）
   ├─ 没有GL.LoadOrtho()设置
   ├─ DrawArrowGL中的箭头方向角度计算有问题
   ├─ 没有屏幕边缘箭头指示（只有屏幕内绘制）
   ├─ 没有小地图标点功能
   └─ 没有OnGUI()文字标签绘制
```

#### 任务区域获取问题
```
QuestZoneTracker.cs
├─ ScanAllQuestZones() 实现不完整
│  └─ 缺少对不同任务类型的处理逻辑
│  └─ 没有位置提取代码
│
└─ QuestZoneData 字段不完整
   └─ 缺少关键字段（radius, centerPosition等）
```

---

## 三、问题根本原因分析

### 问题1: 撤离点/任务区域渲染不显示

**原因:**
1. `OnPostRender()` 中调用 `GL.LoadPixelMatrix()` 而不是 `GL.LoadOrtho()`
2. 没有正确的屏幕坐标系转换（Y轴翻转缺失）
3. `DrawEdgeIndicator()` 的实现不完整：
   - 只检查屏幕内的点，没有屏幕外箭头指示
   - 没有调用绘制函数（`DrawArrowGL` 从未被调用）
4. GL材质设置不正确

**修复策略:**
- 补全 `OnPostRender()` 中的文字绘制逻辑（需要 `OnGUI()` 实现）
- 完整实现 `DrawEdgeIndicator()` 的屏幕边缘箭头逻辑
- 修复 `DrawArrowGL()` 的箭头角度计算
- 添加正确的GL初始化代码

### 问题2: 任务区域获取失败

**原因:**
1. `QuestZoneTracker.ScanAllQuestZones()` 的实现不完整
   - v2中有详细的反射逻辑来提取任务位置
   - v3中缺少这部分逻辑
2. `QuestZoneData` 缺少关键字段
3. 没有正确处理不同任务类型（ReachLocation vs TaskEvent）

**修复策略:**
- 从v2的CheatSystem.cs中移植任务扫描逻辑
- 完善 `QuestZoneData` 数据模型
- 添加对不同任务类型的处理

### 问题3: 小地图标点缺失

**原因:**
1. v3中没有实现小地图标点功能
2. 事件发布了但没有订阅者处理

**修复策略:**
- 创建 `Services/MinimapMarkerService.cs` 处理小地图标点
- 在ESPSystemManager中订阅相关事件

---

## 四、修复方案详细步骤

### 步骤1: 修复QuestZoneData数据模型

文件: `Features/QuestMarker/Data/QuestZoneData.cs`

**添加缺失字段:**
```csharp
public class QuestZoneData
{
    // 现有字段...
    
    // 新增字段
    public Vector3 Position { get; set; }              // 屏幕世界坐标中心点
    public float Radius { get; set; }                  // 影响范围半径（用于屏幕内绘制）
    public float Distance { get; set; }                // 到玩家的距离（实时更新）
    public Vector3 ScreenPosition { get; set; }        // 屏幕坐标（每帧更新）
    public bool IsOnScreen { get; set; }              // 是否在屏幕内
    public string UniqueId { get; set; }              // 唯一标识（Quest_TaskId）
}
```

### 步骤2: 完善QuestZoneTracker的扫描逻辑

文件: `Features/QuestMarker/Collectors/QuestZoneTracker.cs`

**在 `ScanAllQuestZones()` 中补全:**
```csharp
// 从v2的CheatSystem中移植的逻辑
private bool TryExtractReachLocationPosition(MonoBehaviour taskComponent, 
    out Vector3 position, out float radius)
{
    // 使用反射提取位置信息
    // 参考v2的 TryExtractReachLocationPosition 实现
}

private bool TryExtractTaskEventPosition(MonoBehaviour taskComponent,
    out Vector3 position, out float radius)
{
    // 使用反射提取位置信息
    // 参考v2的 TryExtractTaskEventPosition 实现
}
```

### 步骤3: 修复ESPSystemManager中的屏幕标识渲染

文件: `Core/Systems/ESP/ESPSystemManager.cs`

**问题3.1: 修复OnPostRender()及相关方法**

```csharp
// 替换现有的 OnPostRender() 和相关方法

private void OnPostRender()
{
    if (_playerTransform == null || Camera.main == null)
        return;

    Camera mainCamera = Camera.main;
    Vector2 screenSize = new Vector2(Screen.width, Screen.height);
    
    // 绘制撤离点指示器（绿色）
    if (_config.ShowEvacuationPoints)
    {
        Color evacuationColor = new Color(0.2f, 1f, 0.2f);
        foreach (var point in _evacuationPoints)
        {
            DrawScreenEdgeIndicator(mainCamera, screenSize, point.Position, 
                                   point.Name, evacuationColor, point.DistanceToPlayer);
        }
    }
    
    // 绘制任务区域指示器（黄色）
    if (_config.ShowQuestZones)
    {
        Color questColor = _config.QuestZoneColor;
        foreach (var zone in _questZones)
        {
            DrawScreenEdgeIndicator(mainCamera, screenSize, zone.Position, 
                                   zone.Description, questColor, zone.Distance);
        }
    }
}

/// <summary>
/// 绘制屏幕边缘指示器（完整实现，参考v2）
/// </summary>
private void DrawScreenEdgeIndicator(Camera camera, Vector2 screenSize, 
    Vector3 worldPos, string label, Color color, float distance)
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
        DrawCircleGL(new Vector2(screenPos.x, screenPos.y), 
                    Mathf.Clamp(40f, 30f, 100f), color, 24);
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
/// 使用GL绘制箭头指向
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

        // 计算箭头顶点（3D在屏幕空间）
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
```

**问题3.2: 在OnGUI()中添加文字标签**

```csharp
private void OnGUI()
{
    if (_playerTransform == null || !LevelManager.LevelInited)
        return;

    Camera mainCamera = Camera.main;
    if (mainCamera == null) return;

    Vector2 screenSize = new Vector2(Screen.width, Screen.height);

    // 绘制撤离点标签和距离
    if (_config.ShowEvacuationPoints)
    {
        DrawEdgeIndicatorLabels(mainCamera, screenSize, _evacuationPoints, 
                               "撤离点", new Color(0.2f, 1f, 0.2f));
    }

    // 绘制任务区域标签和距离
    if (_config.ShowQuestZones)
    {
        DrawEdgeIndicatorLabels(mainCamera, screenSize, _questZones,
                               "任务区域", _config.QuestZoneColor);
    }
}

private void DrawEdgeIndicatorLabels(Camera camera, Vector2 screenSize, 
    IEnumerable<dynamic> markers, string prefix, Color color)
{
    if (_indicatorLabelStyle == null)
    {
        InitializeIndicatorLabelStyle();
    }

    _indicatorLabelStyle.normal.textColor = color;

    foreach (var marker in markers)
    {
        Vector3 screenPos = camera.WorldToScreenPoint(marker.Position);
        if (screenPos.z <= 0) continue;

        screenPos.y = screenSize.y - screenPos.y;
        float distance = Vector3.Distance(_playerTransform.position, marker.Position);

        // 显示距离标签
        string label = $"{prefix}\n{distance:F0}m";
        GUI.Label(new Rect(screenPos.x - 50, screenPos.y - 30, 100, 60), 
                 label, _indicatorLabelStyle);
    }
}

private void InitializeIndicatorLabelStyle()
{
    _indicatorLabelStyle = new GUIStyle(GUI.skin.label)
    {
        fontSize = 12,
        alignment = TextAnchor.MiddleCenter,
        fontStyle = FontStyle.Bold
    };
}
```

### 步骤4: 添加小地图标点功能

文件: `Core/Systems/ESP/Services/MinimapMarkerService.cs` (新建)

```csharp
using UnityEngine;
using DuckovESPv3.Core.EventBus;
using DuckovESPv3.Features.QuestMarker.Events;
using DuckovESPv3.Features.QuestMarker.Data;
using DuckovESPv3.Infrastructure.Logging;
using System;
using System.Collections.Generic;

namespace DuckovESPv3.Core.Systems.ESP.Services
{
    /// <summary>
    /// 小地图标点服务
    /// 负责为撤离点和任务区域创建小地图标记
    /// </summary>
    public class MinimapMarkerService : IDisposable
    {
        private readonly IEventBus _eventBus;
        private readonly ILogger _logger;
        private readonly Dictionary<string, GameObject> _minimapMarkers = 
            new Dictionary<string, GameObject>();

        public MinimapMarkerService(IEventBus eventBus, ILogger logger)
        {
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// 初始化服务（订阅事件）
        /// </summary>
        public void Initialize()
        {
            _eventBus.Subscribe<QuestZonesUpdatedEvent>(OnQuestZonesUpdated);
            _eventBus.Subscribe<EvacuationPointsUpdatedEvent>(OnEvacuationPointsUpdated);
            _logger.Info("[MinimapMarkerService] Initialized");
        }

        /// <summary>
        /// 处理任务区域更新事件
        /// </summary>
        private void OnQuestZonesUpdated(QuestZonesUpdatedEvent evt)
        {
            if (evt.QuestZones == null || evt.QuestZones.Count == 0)
                return;

            try
            {
                foreach (var zone in evt.QuestZones)
                {
                    CreateOrUpdateMinimapMarker(zone, Color.yellow, "QuestZone");
                }

                _logger.Debug($"[MinimapMarkerService] Created {evt.QuestZones.Count} quest zone minimap markers");
            }
            catch (Exception ex)
            {
                _logger.Error($"[MinimapMarkerService] Failed to create quest zone markers: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 处理撤离点更新事件
        /// </summary>
        private void OnEvacuationPointsUpdated(EvacuationPointsUpdatedEvent evt)
        {
            if (evt.EvacuationPoints == null || evt.EvacuationPoints.Count == 0)
                return;

            try
            {
                foreach (var point in evt.EvacuationPoints)
                {
                    CreateOrUpdateMinimapMarker(point, Color.green, "EvacuationPoint");
                }

                _logger.Debug($"[MinimapMarkerService] Created {evt.EvacuationPoints.Count} evacuation point minimap markers");
            }
            catch (Exception ex)
            {
                _logger.Error($"[MinimapMarkerService] Failed to create evacuation point markers: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 创建或更新小地图标记
        /// </summary>
        private void CreateOrUpdateMinimapMarker(dynamic markerData, Color color, string type)
        {
            string markerId = $"{type}_{markerData.UniqueId}";

            if (!_minimapMarkers.TryGetValue(markerId, out var marker))
            {
                // 创建新标记
                var minimap = FindMinimap();
                if (minimap == null) return;

                marker = new GameObject($"Minimap_{type}_{markerData.UniqueId}");
                marker.transform.SetParent(minimap.transform);
                // 设置UI位置等...
                
                _minimapMarkers[markerId] = marker;
            }

            // 更新标记位置和颜色
            UpdateMarkerPosition(marker, markerData.Position);
        }

        private Transform FindMinimap()
        {
            // TODO: 根据游戏API找到小地图对象
            return null;
        }

        private void UpdateMarkerPosition(GameObject marker, Vector3 worldPosition)
        {
            // TODO: 将世界坐标转换为小地图坐标并更新位置
        }

        public void Dispose()
        {
            _eventBus.Unsubscribe<QuestZonesUpdatedEvent>(OnQuestZonesUpdated);
            _eventBus.Unsubscribe<EvacuationPointsUpdatedEvent>(OnEvacuationPointsUpdated);

            foreach (var marker in _minimapMarkers.Values)
            {
                if (marker != null)
                    Object.Destroy(marker);
            }

            _minimapMarkers.Clear();
            _logger.Info("[MinimapMarkerService] Disposed");
        }
    }
}
```

### 步骤5: 在ESPSystemManager中集成小地图服务

```csharp
// 在 ESPSystemManager 中添加

private Services.MinimapMarkerService? _minimapMarkerService;

public void Initialize(ESPSystemConfig config, IEventBus eventBus, ILogger logger)
{
    // ... 现有初始化代码 ...
    
    // 初始化小地图标点服务
    _minimapMarkerService = new Services.MinimapMarkerService(eventBus, logger);
    _minimapMarkerService.Initialize();
    
    _logger.Info("[ESPSystemManager] Minimap marker service initialized");
}

public void OnDestroy()
{
    _minimapMarkerService?.Dispose();
    // ... 其他清理代码 ...
}
```

---

## 五、验证清单

修复完成后检查项:

- [ ] 撤离点在屏幕内显示绿色圆形，屏幕外显示绿色箭头
- [ ] 任务区域在屏幕内显示黄色圆形，屏幕外显示黄色箭头
- [ ] 屏幕标记上显示到玩家的实时距离
- [ ] 小地图上能看到撤离点和任务区域的标记
- [ ] 任务完成时对应的屏幕标记自动消失
- [ ] 新关卡加载时重新扫描并显示标记
- [ ] 没有性能下降（使用事件驱动而非轮询）

---

## 六、关键代码参考

### v2的关键实现（可直接参考/移植）

1. **v2的GL圆形绘制**: `ModBehaviour.cs` 第~1200行 `DrawCircleGL()`
2. **v2的箭头绘制**: `ModBehaviour.cs` 第~1300行 `DrawEvacuationArrowGL()`
3. **v2的撤离点扫描**: `CheatSystem.cs` 第619行 `GetEvacuationPoints()`
4. **v2的任务区域扫描**: `CheatSystem.cs` 第696行 `GetQuestZones()` 和相关的反射逻辑
5. **v2的位置提取**: `CheatSystem.cs` 第800+行的 `TryExtractReachLocationPosition()` 等方法

---

## 七、预期效果

修复后v3将与v2行为完全一致:

```
┌─────────────────────────────────┐
│     游戏屏幕视图               │
│                                 │
│    ◎ 撤离点(绿色圆形)           │
│    ⊙ 任务区域(黄色圆形)         │
│                                 │
│  ↑ 屏幕外撤离点(绿色箭头)       │
│  ← 屏幕外任务区域(黄色箭头)     │
│                                 │
│  距离: 100m      距离: 150m     │
└─────────────────────────────────┘

小地图:
┌───────┐
│▲ ● ▼ │ ● = 撤离点
│ ◆   │ ◆ = 任务区域
│     P │ P = 玩家
└───────┘
```
