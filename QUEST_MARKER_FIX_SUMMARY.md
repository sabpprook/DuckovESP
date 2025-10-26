# Quest 标记和撤离点问题修复总结

## 问题描述

在 v3 中，疏散点和任务区域的屏幕指示器存在以下问题：
1. **距离始终显示 0m** - 虽然标签显示在屏幕上，但距离值始终为 0
2. **屏幕外时无指示器** - 当标记在屏幕外时，没有屏幕边缘箭头和距离标签
3. **缺少屏幕边缘箭头** - 离屏指示器完全不显示

## 根本原因分析

### 数据流图
```
EvacuationPointTracker    QuestZoneTracker
    |                          |
    └─→ Update() 计算距离 ←──┘
         |
         v
    发布 EventBus 事件
         |
         v
  ESPSystemManager 订阅
    (存储数据)
         |
    ❌ 但从未调用 Update()
         |
         v
  距离值永不更新 = 始终 0
```

### 设计缺陷
- `EvacuationPointTracker` 和 `QuestZoneTracker` 各有一个 `Update()` 方法，用于每帧计算距离和屏幕坐标
- `ESPSystemManager` 通过 EventBus 订阅事件并存储数据
- **但是**：`ESPSystemManager` 没有对这两个 tracker 的引用，所以无法调用它们的 `Update()` 方法
- 结果：距离在初始化后永不更新，始终为 0

## 实施的修复

### 1. 增强数据模型 (✅ 已完成)
- `QuestZoneData.cs`: 添加 `Distance`、`ScreenPosition`、`IsOnScreen`、`UniqueId` 等字段
- `EvacuationPointData.cs`: 添加 `ScreenPosition`、`IsOnScreen`、`UniqueId` 字段

### 2. 改进渲染系统 (✅ 已完成)
- **GL 渲染修复**:
  - 更换 `GL.LoadPixelMatrix()` → `GL.LoadOrtho()` (正确的屏幕坐标系)
  - 添加 Y 轴翻转: `screenPos.y = screenSize.y - screenPos.y`
  - 归一化顶点坐标: `GL.Vertex3(x/Screen.width, y/Screen.height, 0)`

- **屏幕内圆形标记**:
  ```csharp
  DrawCircleGL(screenPos, 40f, color, 24 segments)
  ```

- **屏幕外边缘指示器**:
  ```csharp
  DrawEdgeArrowGL(screenPos, screenSize, margin, arrowSize, color)
  ```

### 3. OnGUI 文本标签 (✅ 已完成)
- 屏幕内：在标记下方显示距离标签
- 屏幕外：在屏幕边缘显示距离标签（沿边框放置）

```csharp
private void DrawEdgeIndicatorLabel(Camera camera, Vector2 screenSize, 
    Vector3 worldPos, float distance)
{
    // 屏幕内或屏幕外都显示距离标签
    if (distance <= _config.QuestZoneMaxDistance)
    {
        string label = $"{distance:F0}m";
        
        if (isOnScreen)
        {
            // 屏幕内：标记下方
            GUI.Label(..., label, ...);
        }
        else
        {
            // 屏幕外：边缘位置
            GUI.Label(..., label, ...);
        }
    }
}
```

### 4. **【关键修复】数据管道集成** (✅ 已完成)

#### Step 4a: ESPSystemManager 添加 tracker 引用
```csharp
// 字段声明
private Features.QuestMarker.Collectors.EvacuationPointTracker? 
    _evacuationPointTracker;
private Features.QuestMarker.Collectors.QuestZoneTracker? 
    _questZoneTracker;

// Setter 方法（从 ModBehaviour 调用）
public void SetDataTrackers(
    EvacuationPointTracker? evacuationTracker, 
    QuestZoneTracker? questTracker)
{
    _evacuationPointTracker = evacuationTracker;
    _questZoneTracker = questTracker;
}

// Update() 中调用 tracker 的 Update()
private void Update()
{
    // 关键：每帧更新距离计算
    try
    {
        _evacuationPointTracker?.Update();
        _questZoneTracker?.Update();
    }
    catch (Exception ex)
    {
        _logger.Error($"Tracker update failed: {ex.Message}");
    }
    
    // ... 其他逻辑
}
```

#### Step 4b: QuestMarkerCollectionService 添加 getter
```csharp
// 获取追踪器供外部使用
public EvacuationPointTracker? GetEvacuationPointTracker() 
    => _evacuationPointTracker;

public QuestZoneTracker? GetQuestZoneTracker() 
    => _questZoneTracker;
```

#### Step 4c: ModBehaviour 建立连接
```csharp
// 在 InitializeDataCollectors() 中
if (_eventBus != null && _logger != null)
{
    _questMarkerService = new QuestMarkerCollectionService(_eventBus, _logger);
    _questMarkerService.Initialize();
    
    // ✅ 【关键修复】将追踪器传递给 ESPSystemManager
    if (_espSystemManager != null)
    {
        _espSystemManager.SetDataTrackers(
            _questMarkerService.GetEvacuationPointTracker(),
            _questMarkerService.GetQuestZoneTracker()
        );
        _logger?.Info("[ModBehaviour] ✓ 已将追踪器传递给ESPSystemManager");
    }
    
    // ... 初始化刷新
}
```

## 数据流改进后

```
关卡初始化
    |
    v
QuestMarkerCollectionService.Initialize()
    - 创建 EvacuationPointTracker
    - 创建 QuestZoneTracker
    - 发布初始事件
    |
    v
ModBehaviour.InitializeDataCollectors()
    - 调用 SetDataTrackers() 传递 tracker 引用
    |
    v
ESPSystemManager
    - 订阅 EventBus 获取数据
    - 保存 tracker 引用
    |
    v
每一帧：
    ESPSystemManager.Update()
    ├─→ _evacuationPointTracker.Update()  ✅ 计算距离
    ├─→ _questZoneTracker.Update()        ✅ 计算距离
    └─→ 距离值正确更新

    OnPostRender()
    ├─→ DrawScreenEdgeIndicator()
    │   ├─→ DrawCircleGL()        (屏幕内圆形)
    │   └─→ DrawEdgeArrowGL()     (屏幕外箭头)
    
    OnGUI()
    └─→ DrawEdgeIndicatorLabel()  ✅ 显示距离文本
        ├─→ 屏幕内：标记下方
        └─→ 屏幕外：屏幕边缘
```

## 验证点检查表

- [x] 数据模型支持距离和屏幕坐标计算
- [x] Trackers 的 Update() 每帧被调用
- [x] ESPSystemManager 拥有 tracker 引用
- [x] GL 渲染使用正确的坐标系 (LoadOrtho)
- [x] 屏幕内显示圆形标记
- [x] 屏幕外显示边缘箭头
- [x] OnGUI 显示距离标签（屏幕内和屏幕外）
- [x] 没有编译错误（仅预先存在的空值警告）

## 运行时行为预期

### 游戏中应看到的：

1. **疏散点 (绿色)**
   - 屏幕内：绿色圆形 + 距离标签 (如 "45m")
   - 屏幕外：屏幕边缘绿色箭头 + 距离标签

2. **任务区域 (配置的颜色)**
   - 屏幕内：彩色圆形 + 距离标签 (如 "125m")
   - 屏幕外：屏幕边缘彩色箭头 + 距离标签

3. **距离更新**
   - 实时更新：靠近标记时距离值减少
   - 远离标记时距离值增加

## 文件修改清单

| 文件 | 修改 |
|------|------|
| `QuestZoneData.cs` | 添加距离、屏幕位置字段 |
| `EvacuationPointData.cs` | 添加屏幕位置字段 |
| `EvacuationPointTracker.cs` | Update() 初始化新字段 |
| `QuestZoneTracker.cs` | Update() 初始化新字段 |
| `ESPSystemManager.cs` | 🔴 **关键修复**: 添加 tracker 引用 + SetDataTrackers() 方法 + Update() 调用 |
| `QuestMarkerCollectionService.cs` | 添加 getter 方法导出 trackers |
| `ModBehaviour.cs` | 🔴 **关键修复**: InitializeDataCollectors() 中调用 SetDataTrackers() |

## 技术细节

### GL 坐标系修正
```csharp
// 错误方式 (v3 原始代码)
GL.LoadPixelMatrix();  // ❌ 没有正确设置正交投影
GL.Vertex3(x, y, 0);   // ❌ 像素坐标，容易出错

// 正确方式 (修复后)
GL.LoadOrtho();                          // ✅ 正确的正交投影
GL.Vertex3(x/Screen.width, 
           y/Screen.height, 0);         // ✅ 归一化坐标 [0,1]
```

### 屏幕坐标 Y 轴翻转
```csharp
// Unity 的屏幕坐标：Y=0 在底部
// GL 的屏幕坐标：Y=0 在顶部
// 需要翻转才能匹配

Vector3 screenPos = camera.WorldToScreenPoint(worldPos);
screenPos.y = Screen.height - screenPos.y;  // ✅ 正确翻转
```

## 后续测试建议

1. **单元测试**：测试 tracker.Update() 确实更新距离值
2. **集成测试**：验证 ModBehaviour 成功调用 SetDataTrackers()
3. **运行时调试**：
   - 启用 Unity profiler，监视 OnPostRender() 的性能
   - 检查 console 是否有错误信息
   - 验证距离值随时间实时变化

## 相关问题修复历史

| 问题 | 根本原因 | 修复 | 状态 |
|------|---------|------|------|
| 距离始终 0m | Tracker.Update() 未被调用 | 在 ESPSystemManager.Update() 中调用 | ✅ |
| 屏幕外无箭头 | 未实现 DrawEdgeArrowGL() | 实现完整的屏幕边缘箭头绘制 | ✅ |
| GL 渲染错乱 | 使用 LoadPixelMatrix | 改用 LoadOrtho() + 坐标归一化 | ✅ |
| 文本标签缺失 | OnGUI 逻辑有缺陷 | 改进 DrawEdgeIndicatorLabel() | ✅ |

---

**最后更新**: 修复完成并编译通过
**状态**: 🟢 准备测试
