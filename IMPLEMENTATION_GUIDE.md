# 屏幕标识渲染修复 - 实现指南

## 修复优先级和实现顺序

### 阶段1: 基础数据模型修复 (高优先级)

#### 1.1 修复QuestZoneData数据模型
**文件**: `DuckovESPv3/Features/QuestMarker/Data/QuestZoneData.cs`

需要添加的字段:
```csharp
public Vector3 Position { get; set; }      // 新增：世界空间中心位置
public float Radius { get; set; }          // 新增：影响范围
public float Distance { get; set; }        // 新增：到玩家距离（实时）
public bool IsActive { get; set; }         // 新增：是否激活显示
public string UniqueId { get; set; }       // 新增：唯一标识
```

#### 1.2 检查EvacuationPointData
**文件**: `DuckovESPv3/Features/QuestMarker/Data/EvacuationPointData.cs`

检查已有字段是否包括:
- `Position` ✓
- `Name` ✓
- `DistanceToPlayer` ✓
- `IsVisible` ✓

---

### 阶段2: 数据采集逻辑完善 (高优先级)

#### 2.1 完善QuestZoneTracker
**文件**: `DuckovESPv3/Features/QuestMarker/Collectors/QuestZoneTracker.cs`

**问题**: `ScanAllQuestZones()` 方法不完整，缺少位置提取逻辑

**解决方案**: 从v2的CheatSystem.cs移植逻辑

参考文件中需要移植的方法:
- `TryExtractReachLocationPosition()` - 从v2 CheatSystem.cs (~800行)
- `TryExtractTaskEventPosition()` - 从v2 CheatSystem.cs (~850行)
- `TryExtractMapElementPosition()` - 从v2 CheatSystem.cs (~900行)

**实现步骤**:

1. 在v2的 `CheatSystem.cs` 中找到上述三个方法的完整实现
2. 复制到v3的 `QuestZoneTracker.cs` 中（改为私有方法）
3. 在 `ScanAllQuestZones()` 的循环中调用这些方法

**关键代码骨架**:
```csharp
private void ScanAllQuestZones()
{
    try
    {
        _questZones.Clear();
        
        var questManager = QuestManager.Instance;
        if (questManager == null) return;

        var activeQuests = questManager.ActiveQuests;
        if (activeQuests == null || activeQuests.Count == 0) return;

        foreach (var quest in activeQuests)
        {
            var tasks = quest.Tasks;
            if (tasks == null) continue;

            foreach (var task in tasks)
            {
                Vector3 position;
                float radius;
                
                // 根据任务类型提取位置
                if (task is QuestTask_ReachLocation reachLocation)
                {
                    if (TryExtractReachLocationPosition(reachLocation, out position, out radius))
                    {
                        var zoneData = new QuestZoneData
                        {
                            Position = position,
                            Radius = radius,
                            Description = task.Master?.DisplayName ?? "Unknown",
                            Distance = 0f, // 后续Update时更新
                            IsActive = true,
                            UniqueId = $"Quest_{quest.ID}_Task_{task.ID}"
                        };
                        _questZones[zoneData.UniqueId] = zoneData;
                    }
                }
                // ... 处理其他任务类型
            }
        }
        
        PublishUpdateEvent();
    }
    catch (Exception ex)
    {
        _logger.Error($"[QuestZoneTracker] Scan failed: {ex.Message}", ex);
    }
}

private bool TryExtractReachLocationPosition(QuestTask_ReachLocation task, 
    out Vector3 position, out float radius)
{
    // 从v2复制的完整逻辑
    position = Vector3.zero;
    radius = 10f;
    
    try
    {
        // 使用反射或直接属性访问获取位置
        // ...参考v2的实现
        return true;
    }
    catch
    {
        return false;
    }
}
```

#### 2.2 验证EvacuationPointTracker
**文件**: `DuckovESPv3/Features/QuestMarker/Collectors/EvacuationPointTracker.cs`

检查点:
- [ ] `IsEvacuationPoint()` 方法的名称判断是否完整
- [ ] `ScanEvacuationPoints()` 是否正确调用 `FindObjectsOfType<SimplePointOfInterest>()`
- [ ] `Update()` 方法是否被调用（需要在某处每帧调用）

**问题**: 撤离点追踪器在 `Update()` 方法但未被调用

**解决方案**: 需要在ESPSystemManager中每帧调用
```csharp
// 在 ESPSystemManager.Update() 中添加
_evacuationPointTracker?.Update();
_questZoneTracker?.Update(); // 如果有类似方法也需要调用
```

---

### 阶段3: 屏幕渲染实现 (最关键)

#### 3.1 修复OnPostRender()方法
**文件**: `DuckovESPv3/Core/Systems/ESP/ESPSystemManager.cs`

**当前问题**:
- 行数约180-230
- 使用了 `GL.LoadPixelMatrix()` 而应该使用 `GL.LoadOrtho()`
- 屏幕坐标Y轴未翻转
- 没有绘制屏幕外箭头

**修复方案**: 完全替换现有的OnPostRender()和相关方法

```csharp
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
                                   point.Name, evacuationColor);
        }
    }
    
    // 绘制任务区域指示器（黄色）
    if (_config.ShowQuestZones)
    {
        Color questColor = _config.QuestZoneColor;
        foreach (var zone in _questZones)
        {
            DrawScreenEdgeIndicator(mainCamera, screenSize, zone.Position, 
                                   zone.Description, questColor);
        }
    }
}

private void DrawScreenEdgeIndicator(Camera camera, Vector2 screenSize, 
    Vector3 worldPos, string label, Color color)
{
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
            
            GL.Vertex3(x / Screen.width, y / Screen.height, 0f);
        }

        GL.End();
    }
    
    GL.PopMatrix();
}

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

        GL.Vertex3(tip.x / Screen.width, tip.y / Screen.height, 0);
        GL.Vertex3(left.x / Screen.width, left.y / Screen.height, 0);
        GL.Vertex3(right.x / Screen.width, right.y / Screen.height, 0);

        GL.End();
    }

    GL.PopMatrix();
}
```

#### 3.2 添加OnGUI()文字标签
**文件**: `DuckovESPv3/Core/Systems/ESP/ESPSystemManager.cs`

添加字段:
```csharp
private GUIStyle _edgeIndicatorLabelStyle;
```

添加方法:
```csharp
private void OnGUI()
{
    if (_playerTransform == null || !LevelManager.LevelInited || Camera.main == null)
        return;

    Vector2 screenSize = new Vector2(Screen.width, Screen.height);
    Camera mainCamera = Camera.main;

    // 绘制撤离点标签
    if (_config.ShowEvacuationPoints)
    {
        DrawEdgeIndicatorLabels(mainCamera, screenSize, _evacuationPoints, Color.green);
    }

    // 绘制任务区域标签
    if (_config.ShowQuestZones)
    {
        DrawEdgeIndicatorLabels(mainCamera, screenSize, _questZones, _config.QuestZoneColor);
    }
}

private void DrawEdgeIndicatorLabels(Camera camera, Vector2 screenSize, 
    List<dynamic> markers, Color color)
{
    if (_edgeIndicatorLabelStyle == null)
    {
        _edgeIndicatorLabelStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 12,
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold
        };
    }

    _edgeIndicatorLabelStyle.normal.textColor = color;

    foreach (var marker in markers)
    {
        Vector3 screenPos = camera.WorldToScreenPoint(marker.Position);
        if (screenPos.z <= 0) continue;

        screenPos.y = screenSize.y - screenPos.y;

        float distance = Vector3.Distance(_playerTransform.position, marker.Position);
        
        bool isOnScreen = screenPos.x >= 0 && screenPos.x <= screenSize.x && 
                          screenPos.y >= 0 && screenPos.y <= screenSize.y;

        if (isOnScreen)
        {
            string label = $"{distance:F0}m";
            GUI.Label(new Rect(screenPos.x - 30, screenPos.y + 20, 60, 30), 
                     label, _edgeIndicatorLabelStyle);
        }
    }
}
```

---

### 阶段4: 其他支持系统 (中优先级)

#### 4.1 添加Update()调用
**文件**: `DuckovESPv3/Core/Systems/ESP/ESPSystemManager.cs`

在 `Update()` 方法中添加:
```csharp
private void Update()
{
    // ... 现有代码 ...
    
    // 更新撤离点和任务区域
    _evacuationPointTracker?.Update();
    _questZoneTracker?.Update();
}
```

#### 4.2 小地图标点服务 (可选，后续实现)
**文件**: `DuckovESPv3/Core/Systems/ESP/Services/MinimapMarkerService.cs` (新建)

这是可选的后续改进，不影响屏幕标识显示

---

## 测试清单

完成每个阶段后的验证:

### 阶段1测试
- [ ] 编译通过，没有编译错误
- [ ] QuestZoneData包含所有必需字段

### 阶段2测试
- [ ] 撤离点能被扫描到（检查Debug.Log输出）
- [ ] 任务区域能被扫描到（检查Debug.Log输出）
- [ ] 距离实时更新（监控DistanceToPlayer字段）

### 阶段3测试 - 核心测试
- [ ] 进入关卡，看到屏幕内的绿色撤离点圆形
- [ ] 进入关卡，看到屏幕内的黄色任务区域圆形
- [ ] 移动相机，使撤离点/任务区域离开屏幕，看到屏幕边缘的箭头
- [ ] 箭头指向屏幕外的目标
- [ ] 距离标签随玩家移动实时更新
- [ ] 没有性能下降

### 阶段4测试
- [ ] 切换关卡时标记正确刷新
- [ ] 任务完成时标记消失

---

## 调试技巧

### 问题：屏幕标识不显示

**检查清单:**
1. 查看Console，是否有错误消息
2. 确认 `ShowEvacuationPoints` 和 `ShowQuestZones` 配置为true
3. 在OnPostRender中添加Debug.Log：
   ```csharp
   _logger.Info($"[ESPSystemManager] Rendering {_evacuationPoints.Count} evacuation points");
   ```

### 问题：箭头显示但方向不对

**检查:**
1. 角度计算：`Mathf.Atan2(direction.y, direction.x)` 的值是否合理
2. 使用Debug.DrawLine验证方向：
   ```csharp
   Debug.DrawLine(position, position + direction * 50, color);
   ```

### 问题：屏幕坐标混乱

**检查:**
1. 确认Y轴翻转: `screenPos.y = screenSize.y - screenPos.y;`
2. GL坐标系转换是否正确
3. 使用 `GL.LoadOrtho()` 而非 `GL.LoadPixelMatrix()`

---

## 关键参考文件位置

| 文件 | 行数 | 内容 |
|------|------|------|
| `v2/ModBehaviour.cs` | ~1120 | DrawEvacuationIndicatorsGL() 完整实现 |
| `v2/ModBehaviour.cs` | ~1200 | DrawCircleGL() 实现 |
| `v2/ModBehaviour.cs` | ~1405 | DrawQuestZoneIndicatorsGL() 实现 |
| `v2/CheatSystem.cs` | ~619 | GetEvacuationPoints() |
| `v2/CheatSystem.cs` | ~696 | GetQuestZones() |
| `v2/CheatSystem.cs` | ~750 | 任务位置提取方法 |

---

## 预期完成时间

- 阶段1 (数据模型): 15-30分钟
- 阶段2 (数据采集): 45-60分钟
- 阶段3 (屏幕渲染): 60-90分钟  ← 最关键
- 阶段4 (其他): 20-30分钟

**总计**: 2-3小时

---

## 注意事项

1. **保存原始代码**: 修改前备份现有的OnPostRender()等方法
2. **增量修改**: 不要一次性修改所有代码，分阶段进行
3. **测试频繁**: 每完成一个方法就测试一次
4. **日志记录**: 添加足够的Debug.Log以便问题诊断
5. **版本控制**: 使用git commit保存每个阶段的进度
