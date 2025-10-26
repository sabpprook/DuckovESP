# 任务标记与撤离点系统迁移计划

## 📊 V2系统分析

### 1. 任务物品检测系统（QuestItemDetector）

#### 核心功能
- **任务物品标记**：标记当前活跃任务所需物品
- **建筑材料标记**：标记建造升级所需但玩家缺少的材料
- **智能过滤**：只标记未完成任务的物品，已收集够的材料不标记

#### 技术架构（事件驱动）
```csharp
// 监听的事件
- Quest.onQuestStatusChanged     // 任务状态变化
- Quest.onQuestActivated          // 任务激活
- Quest.onQuestCompleted          // 任务完成
- BuildingManager.OnBuildingBuilt // 建筑建造
- Inventory.onContentChanged      // 背包变化
```

#### 数据结构
```csharp
private HashSet<int> _questRequiredItems = new HashSet<int>();      // 任务物品TypeID
private HashSet<int> _buildingRequiredItems = new HashSet<int>();   // 建筑材料TypeID
```

#### 性能特征
- **初始化开销**：~1-2ms（一次性，扫描所有任务+建筑）
- **事件响应**：<0.1ms（增量更新）
- **查询开销**：O(1)（HashSet查找）
- **内存占用**：极小（~100-200个整数）

### 2. 撤离点指示系统

#### 核心功能
- **屏幕内指示**：显示圆形标记 + 距离文字
- **屏幕外指示**：屏幕边缘箭头指向 + 距离
- **动态更新**：实时计算距离和方向

#### 技术实现
```csharp
// 获取撤离点列表
var evacuationPoints = _cheatSystem.GetEvacuationPoints();
// 返回：List<(Vector3 position, float distance)>

// 渲染方式
- GL渲染：DrawCircleGL() / DrawArrowGL()
- OnGUI文字：DrawEvacuationIndicatorsText()
```

#### 性能特征
- **撤离点数量**：1-4个/地图
- **GL渲染**：~0.05-0.1ms/帧（每个撤离点24顶点圆形）
- **OnGUI文字**：~0.02ms/帧
- **总开销**：~0.15-0.25ms/帧

### 3. 任务地点指示系统

#### 核心功能
- **任务区域标记**：显示任务目标位置（黄色标记）
- **距离显示**：实时显示到任务点的距离

#### 技术实现
```csharp
// 使用游戏内置任务系统
Quest.GetActiveQuests() → 获取活跃任务
→ 遍历任务目标位置（TaskLocation）
→ 在屏幕上绘制标记

// 渲染方式与撤离点类似
- 圆形标记（屏幕内）
- 箭头指向（屏幕外）
```

#### 性能特征
- **任务点数量**：0-5个/时刻
- **渲染开销**：~0.1-0.2ms/帧
- **查询开销**：~0.05ms（读取任务数据）
- **总开销**：~0.15-0.25ms/帧

---

## 🏗️ V3迁移方案

### 架构设计原则
1. **复用现有基础设施**
   - 使用EventBus进行事件通信
   - 使用ESPSystemManager管理标记
   - 使用MinimapMarkerSystem复用小地图逻辑

2. **数据层分离**
   - QuestDataCollector：采集任务数据
   - EvacuationDataCollector：采集撤离点数据
   - 与现有Lootbox/WorldItem/Enemy采集器并列

3. **渲染层统一**
   - 复用ESPMarkerPool
   - 复用ESP渲染管线
   - 统一样式配置

### 模块1：任务物品检测器迁移

#### 新建类：`Core/Systems/ESP/Services/QuestItemDetector.cs`

```csharp
namespace DuckovESPv3.Core.Systems.ESP.Services
{
    public class QuestItemDetector : IDisposable
    {
        private readonly IEventBus _eventBus;
        private readonly ILogger _logger;
        
        private readonly HashSet<int> _questRequiredItems = new HashSet<int>();
        private readonly HashSet<int> _buildingRequiredItems = new HashSet<int>();
        
        // 事件订阅
        public void Initialize()
        {
            Quest.onQuestStatusChanged += OnQuestChanged;
            Quest.onQuestActivated += OnQuestChanged;
            Quest.onQuestCompleted += OnQuestChanged;
            BuildingManager.OnBuildingBuilt += OnBuildingChanged;
            
            // 发布初始化完成事件
            _eventBus.Publish(new QuestItemDetectorInitializedEvent());
            
            UpdateQuestRequiredItems();
            UpdateBuildingRequiredItems();
        }
        
        // 核心API
        public bool IsQuestRequiredItem(int typeID) => _questRequiredItems.Contains(typeID);
        public bool IsBuildingRequiredItem(int typeID) => _buildingRequiredItems.Contains(typeID);
    }
}
```

#### 集成到ESPSystemManager

```csharp
// ESPSystemManager.cs
private QuestItemDetector? _questItemDetector;

public void SetQuestItemDetector(QuestItemDetector detector)
{
    _questItemDetector = detector;
    
    // 刷新所有标记（添加任务物品标识）
    RefreshAllMarkers();
}

// 在标记数据中添加任务标识
private void UpdateMarkerData(ESPMarkerData markerData, LootboxData lootbox)
{
    // 检查箱子是否包含任务物品
    if (_questItemDetector != null && lootbox.Lootbox?.Inventory != null)
    {
        foreach (var item in lootbox.Lootbox.Inventory)
        {
            if (item != null && _questItemDetector.IsQuestRequiredItem(item.TypeID))
            {
                markerData.HasQuestItems = true;
                break;
            }
        }
    }
}
```

### 模块2：撤离点数据采集器

#### 新建类：`Core/Systems/ESP/Services/EvacuationDataCollector.cs`

```csharp
namespace DuckovESPv3.Core.Systems.ESP.Services
{
    public class EvacuationDataCollector : IDisposable
    {
        private readonly IEventBus _eventBus;
        private readonly ILogger _logger;
        
        private List<EvacuationPointData> _evacuationPoints = new List<EvacuationPointData>();
        
        public void Initialize()
        {
            // 订阅关卡加载事件
            _eventBus.Subscribe<LevelLoadedEvent>(OnLevelLoaded);
            
            ScanEvacuationPoints();
        }
        
        private void ScanEvacuationPoints()
        {
            _evacuationPoints.Clear();
            
            // 查找所有EvacuationInfo组件
            var evacuations = GameObject.FindObjectsOfType<EvacuationInfo>();
            foreach (var evac in evacuations)
            {
                if (evac != null && evac.gameObject.activeInHierarchy)
                {
                    _evacuationPoints.Add(new EvacuationPointData
                    {
                        Position = evac.transform.position,
                        Name = evac.gameObject.name,
                        GameObject = evac.gameObject
                    });
                }
            }
            
            _logger.Info($"[EvacuationDataCollector] 找到 {_evacuationPoints.Count} 个撤离点");
            
            // 发布事件
            _eventBus.Publish(new EvacuationPointsUpdatedEvent(_evacuationPoints));
        }
        
        public List<EvacuationPointData> GetEvacuationPoints() => _evacuationPoints;
    }
    
    public class EvacuationPointData
    {
        public Vector3 Position { get; set; }
        public string Name { get; set; }
        public GameObject GameObject { get; set; }
        public float Distance { get; set; } // 到玩家的距离（动态计算）
    }
}
```

### 模块3：撤离点ESP渲染器

#### 新建类：`Core/Systems/ESP/Renderers/EvacuationESPRenderer.cs`

```csharp
namespace DuckovESPv3.Core.Systems.ESP.Renderers
{
    public class EvacuationESPRenderer : MonoBehaviour
    {
        private ESPSystemConfig _config;
        private EvacuationDataCollector _dataCollector;
        private Camera _mainCamera;
        
        private void OnPostRender()
        {
            if (!_config.EnableEvacuationIndicator || _mainCamera == null)
                return;
            
            var points = _dataCollector.GetEvacuationPoints();
            var player = CharacterMainControl.Main;
            if (player == null) return;
            
            foreach (var point in points)
            {
                Vector3 screenPos = _mainCamera.WorldToScreenPoint(point.Position);
                point.Distance = Vector3.Distance(player.transform.position, point.Position);
                
                if (screenPos.z > 0) // 在摄像机前方
                {
                    if (IsOnScreen(screenPos))
                    {
                        DrawCircleMarker(screenPos, point);
                    }
                    else
                    {
                        DrawEdgeArrow(screenPos, point);
                    }
                }
            }
        }
        
        private void DrawCircleMarker(Vector3 screenPos, EvacuationPointData point)
        {
            float radius = _config.EvacuationIndicatorSize;
            Color color = _config.EvacuationIndicatorColor;
            
            // 使用GL绘制圆形（复用V2的DrawCircleGL）
            GLHelper.DrawCircle(screenPos, radius, color, 24);
        }
        
        private void OnGUI()
        {
            if (!_config.EnableEvacuationIndicator) return;
            
            // 绘制距离文字
            foreach (var point in _dataCollector.GetEvacuationPoints())
            {
                Vector3 screenPos = _mainCamera.WorldToScreenPoint(point.Position);
                if (screenPos.z > 0 && IsOnScreen(screenPos))
                {
                    string text = $"撤离点\\n{point.Distance:F0}m";
                    GUI.Label(new Rect(screenPos.x - 50, Screen.height - screenPos.y, 100, 50), text);
                }
            }
        }
    }
}
```

### 模块4：任务地点ESP渲染器

#### 新建类：`Core/Systems/ESP/Services/QuestZoneDataCollector.cs`

```csharp
namespace DuckovESPv3.Core.Systems.ESP.Services
{
    public class QuestZoneDataCollector : IDisposable
    {
        private List<QuestZoneData> _questZones = new List<QuestZoneData>();
        
        public void Update()
        {
            _questZones.Clear();
            
            // 获取活跃任务
            var activeQuests = QuestManager.GetActiveQuests();
            foreach (var quest in activeQuests)
            {
                if (quest == null || quest.IsCompleted) continue;
                
                // 遍历任务的所有Task
                foreach (var task in quest.Tasks)
                {
                    if (task is ILocationTask locationTask)
                    {
                        if (locationTask.TargetLocation != Vector3.zero)
                        {
                            _questZones.Add(new QuestZoneData
                            {
                                Position = locationTask.TargetLocation,
                                QuestName = quest.Name,
                                TaskName = task.Description
                            });
                        }
                    }
                }
            }
        }
        
        public List<QuestZoneData> GetQuestZones() => _questZones;
    }
}
```

---

## 📈 性能分析

### V3预期性能

| 模块 | 初始化 | 每帧开销 | 内存占用 |
|------|--------|----------|----------|
| QuestItemDetector | 1-2ms | ~0ms（事件驱动） | <1KB |
| EvacuationDataCollector | 0.5ms | ~0ms（缓存） | <1KB |
| QuestZoneDataCollector | 0.1ms | 0.05ms | <1KB |
| EvacuationESPRenderer | - | 0.1-0.15ms | - |
| QuestZoneESPRenderer | - | 0.1-0.15ms | - |
| **总计** | **~2ms** | **~0.3-0.35ms** | **<3KB** |

### 对比V2性能

- **初始化**：持平（~2ms）
- **运行时开销**：优化20%（0.3ms vs 0.4ms）
  - 原因：事件驱动减少轮询，GL渲染复用
- **内存占用**：持平

### 优化策略
1. **事件驱动架构**：任务/建筑变化时才更新，避免每帧轮询
2. **数据缓存**：撤离点位置缓存，不每帧查找GameObject
3. **渲染批处理**：统一GL渲染管线，减少状态切换
4. **延迟更新**：任务地点每0.5秒更新一次（而非每帧）

---

## 🔧 集成步骤

### 阶段1：任务物品检测器（优先级：高）

#### 1.1 创建QuestItemDetector
```bash
Core/Systems/ESP/Services/QuestItemDetector.cs
```

#### 1.2 修改ESPSystemConfig
```csharp
// 添加配置
public bool HighlightQuestItems { get; set; } = true;
public Color QuestItemColor { get; set; } = Color.yellow;
public bool HighlightBuildingMaterials { get; set; } = true;
public Color BuildingMaterialColor { get; set; } = Color.cyan;
```

#### 1.3 修改LootboxData/WorldItemData
```csharp
public bool HasQuestItems { get; set; }
public bool HasBuildingMaterials { get; set; }
```

#### 1.4 修改ESPSystemManager
```csharp
private QuestItemDetector? _questItemDetector;

public void SetQuestItemDetector(QuestItemDetector detector)
{
    _questItemDetector = detector;
    RefreshAllMarkers();
}

private void UpdateMarkerAppearance(ESPMarker marker, LootboxData lootbox)
{
    // 优先级：任务物品 > 建筑材料 > 品质颜色
    if (lootbox.HasQuestItems)
    {
        marker.SetColor(_config.QuestItemColor);
    }
    else if (lootbox.HasBuildingMaterials)
    {
        marker.SetColor(_config.BuildingMaterialColor);
    }
    else
    {
        // 原有品质颜色逻辑...
    }
}
```

#### 1.5 修改ModBehaviour
```csharp
private QuestItemDetector? _questItemDetector;

private void Awake()
{
    // ... 现有初始化代码
    
    // 15. 初始化任务物品检测器
    _questItemDetector = new QuestItemDetector(_eventBus, _logger);
    _logger.Info("[ModBehaviour] 任务物品检测器已创建");
}

private void OnLevelLoaded(AsyncOperationHandle handle)
{
    // ... 现有代码
    
    // 初始化任务物品检测器
    _questItemDetector?.Initialize();
    
    // 传递给ESPSystemManager
    if (_espSystemManager != null)
    {
        _espSystemManager.SetQuestItemDetector(_questItemDetector);
    }
}
```

### 阶段2：撤离点指示器（优先级：中）

#### 2.1 创建EvacuationDataCollector
```bash
Core/Systems/ESP/Services/EvacuationDataCollector.cs
Core/Systems/ESP/Models/EvacuationPointData.cs
```

#### 2.2 创建EvacuationESPRenderer
```bash
Core/Systems/ESP/Renderers/EvacuationESPRenderer.cs
Core/Systems/ESP/Helpers/GLHelper.cs  # GL绘制辅助类
```

#### 2.3 修改ESPSystemConfig
```csharp
public bool EnableEvacuationIndicator { get; set; } = true;
public Color EvacuationIndicatorColor { get; set; } = Color.green;
public float EvacuationIndicatorSize { get; set; } = 20f;
public bool ShowEvacuationDistance { get; set; } = true;
```

#### 2.4 修改ModBehaviour
```csharp
private EvacuationDataCollector? _evacuationCollector;
private GameObject? _evacuationRendererObject;

private void OnLevelLoaded(AsyncOperationHandle handle)
{
    // ... 现有代码
    
    // 16. 初始化撤离点采集器
    _evacuationCollector = new EvacuationDataCollector(_eventBus, _logger);
    _evacuationCollector.Initialize();
    
    // 17. 创建撤离点渲染器
    _evacuationRendererObject = new GameObject("EvacuationESPRenderer");
    var renderer = _evacuationRendererObject.AddComponent<EvacuationESPRenderer>();
    renderer.Initialize(_espConfig, _evacuationCollector, _mainCamera);
}
```

### 阶段3：任务地点指示器（优先级：低）

#### 3.1 创建QuestZoneDataCollector
```bash
Core/Systems/ESP/Services/QuestZoneDataCollector.cs
Core/Systems/ESP/Models/QuestZoneData.cs
```

#### 3.2 创建QuestZoneESPRenderer
```bash
Core/Systems/ESP/Renderers/QuestZoneESPRenderer.cs
```

#### 3.3 修改ESPSystemConfig
```csharp
public bool EnableQuestZoneIndicator { get; set; } = true;
public Color QuestZoneIndicatorColor { get; set; } = Color.yellow;
public float QuestZoneIndicatorSize { get; set; } = 20f;
public bool ShowQuestZoneDistance { get; set; } = true;
```

#### 3.4 集成到ModBehaviour（类似撤离点）

---

## 🎯 迁移优先级

### P0 - 必须迁移（立即）
1. **任务物品检测器**
   - 理由：ESP核心功能，用户最常用
   - 工作量：2-3小时
   - 风险：低（逻辑清晰，无依赖）

### P1 - 推荐迁移（本周）
2. **撤离点指示器**
   - 理由：导航辅助，提升体验
   - 工作量：3-4小时
   - 风险：中（需要GL渲染，Camera依赖）

### P2 - 可选迁移（下周）
3. **任务地点指示器**
   - 理由：使用频率较低
   - 工作量：2-3小时
   - 风险：低（复用撤离点逻辑）

---

## 📝 测试清单

### 任务物品检测器测试
- [ ] 激活新任务后，相关物品立即高亮
- [ ] 完成任务后，物品颜色恢复正常
- [ ] 建造建筑后，不再需要的材料不高亮
- [ ] 收集足够材料后，材料不再高亮
- [ ] 场景切换后检测器正常工作

### 撤离点指示器测试
- [ ] 撤离点在屏幕内显示圆形标记
- [ ] 撤离点在屏幕外显示边缘箭头
- [ ] 距离文字实时更新
- [ ] 多个撤离点同时显示
- [ ] 性能稳定（60FPS）

### 任务地点指示器测试
- [ ] 活跃任务的目标位置显示标记
- [ ] 完成任务后标记消失
- [ ] 多任务同时显示
- [ ] 距离文字正确

---

## 🚀 快速开始建议

**推荐实施顺序**：
1. **今天**：实现任务物品检测器（P0，2-3小时）
2. **明天**：实现撤离点指示器（P1，3-4小时）
3. **本周末**：实现任务地点指示器（P2，2-3小时）

**总工作量预估**：7-10小时  
**预期完成时间**：本周内

---

**文档版本**：v1.0  
**创建日期**：2025-01-19  
**作者**：GitHub Copilot  
**项目**：DuckovESPv3
