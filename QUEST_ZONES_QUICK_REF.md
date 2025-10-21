# 任务区域标记功能 - 快速参考

## 简要总结

### 功能目标
在游戏中标记任务目标位置（需要到达的范围），在3D ESP和小地图上显示

### 核心技术
- **扫描**: `FindObjectsOfType<CountDownArea>()` 找到所有任务区域
- **信息**: 从 `ShowLocationInMap` 获取任务名称
- **范围**: 从 `Collider` 获取到达范围
- **渲染**: 使用GL绘制圆形，OnGUI绘制文字

### 游戏逻辑流程
```
任务加载 → 区域就绪 → 玩家靠近 → 进入范围 → 计时开始 → 时间到 → 任务完成
```

---

## 核心数据结构

```csharp
public class QuestZoneMarkerData
{
    public CountDownArea countDownArea;          // 任务区域组件
    public ShowLocationInMap locationMarker;     // 显示信息
    public Collider triggerCollider;             // 触发范围
    public GameObject miniMapMarker;             // 小地图标记
    public string displayName;                   // 任务名称
    public Vector3 centerPosition;               // 位置
    public float radius;                         // 范围半径
    public float requiredTime;                   // 需要停留时间
    public bool isActive;                        // 是否激活
    public float progress;                       // 进度 (0-1)
    public float remainingTime;                  // 剩余时间
}
```

---

## 实现流程

### 1. 扫描 (每2秒一次)
```
ScanQuestZones()
├── FindObjectsOfType<CountDownArea>()
├── GetComponent<ShowLocationInMap>()  // 获取名称
├── GetComponent<Collider>()           // 获取范围
└── CreateQuestZoneMarker()            // 创建小地图标记
```

### 2. 更新 (每帧)
```
UpdateQuestZonesStatus()
├── 更新位置
├── 更新进度 (Progress)
├── 更新剩余时间 (RemainingTime)
└── 检测失效的区域
```

### 3. 渲染 (每帧)
```
OnRenderObject()
└── DrawQuestZones3DESP()
    ├── DrawCircleGL()         // GL绘制圆形
    └── 根据进度改变颜色

OnGUI()
└── DrawQuestZonesInfo()
    ├── 显示任务名称
    ├── 显示距离
    └── 显示进度
```

---

## 关键API

### 获取任务区域
```csharp
CountDownArea area = GetComponent<CountDownArea>();
```

### 获取任务信息
```csharp
ShowLocationInMap marker = area.GetComponent<ShowLocationInMap>();
string name = marker.DisplayName;
```

### 获取范围
```csharp
Collider collider = area.GetComponent<Collider>();
Vector3 center = collider.bounds.center;
float radius = collider.bounds.extents.magnitude;
```

### 获取状态
```csharp
float progress = area.Progress;           // 0-1
float remainingTime = area.RemainingTime; // 秒
float requiredTime = area.RequiredExtrationTime;
```

---

## 配置项 (新增)

```csharp
// ESPConfig.cs中添加
public bool EnableQuestZoneMarkers = true;
public Color QuestZoneColor = Color.yellow;
public Color QuestZoneInProgressColor = Color.green;
public bool ShowQuestZoneDistance = true;
public bool ShowQuestZoneProgress = true;
```

---

## 翻译 (新增)

```json
// zh-CN.json
"EnableQuestZoneMarkers": "启用任务区域标记",
"QuestZoneColor": "任务区域颜色",
"ShowQuestZoneDistance": "显示任务距离",
"ShowQuestZoneProgress": "显示任务进度"

// en-US.json
"EnableQuestZoneMarkers": "Enable Quest Zone Markers",
"QuestZoneColor": "Quest Zone Color",
"ShowQuestZoneDistance": "Show Quest Zone Distance",
"ShowQuestZoneProgress": "Show Quest Zone Progress"

// de-DE.json
"EnableQuestZoneMarkers": "Quest-Zone-Markierungen aktivieren",
"QuestZoneColor": "Quest-Zone-Farbe",
"ShowQuestZoneDistance": "Quest-Entfernung anzeigen",
"ShowQuestZoneProgress": "Quest-Fortschritt anzeigen"
```

---

## 代码位置

在 `ModBehaviour.cs` 中添加：

```csharp
// 行 ~28: 添加字段
private Dictionary<CountDownArea, QuestZoneMarkerData> _trackedQuestZones = 
    new Dictionary<CountDownArea, QuestZoneMarkerData>();

// 行 ~285: 在Update的扫描中添加
ScanQuestZones();
UpdateQuestZonesStatus();

// 行 ~650: 在OnRenderObject中添加
DrawQuestZones3DESP();

// 行 ~600: 在OnGUI中添加
DrawQuestZonesInfo();

// 行 ~1500: 添加实现方法
private void ScanQuestZones() { ... }
private void UpdateQuestZonesStatus() { ... }
private void DrawQuestZones3DESP() { ... }
private void DrawQuestZoneCircle() { ... }
private void DrawQuestZonesInfo() { ... }
private float GetColliderRadius(Collider collider) { ... }
private void CreateQuestZoneMarker(QuestZoneMarkerData markerData) { ... }
```

---

## 颜色方案

| 状态 | 颜色 | 说明 |
|------|------|------|
| 未激活 | 灰色 | 不可用的任务区域 |
| 就绪 | 黄色 | 可以进入的任务区域 |
| 进行中 | 绿色 | 正在计时的任务区域 |
| 完成 | 消失 | 任务完成后消失 |

---

## 性能影响

- **扫描**: 每2秒扫描一次 (低频)
- **更新**: 每帧更新追踪的区域 (中等)
- **渲染**: GL圆形 + GUI文字 (中等)
- **总体**: 约占0.5-1ms/帧

---

## 测试检查清单

- [ ] 小地图显示任务位置标记
- [ ] 3D ESP显示圆形范围指示
- [ ] 显示正确的任务名称
- [ ] 显示正确的距离
- [ ] 进入范围时显示进度
- [ ] 任务完成后标记消失
- [ ] 没有性能下降
- [ ] 没有控制台错误

