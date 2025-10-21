# 任务目标位置标记功能 - 源码研究报告

## 1. 核心组件分析

### 1.1 CountDownArea (计数区域)
**位置**: `TeamSoda.Duckov.Core/CountDownArea.cs`

**功能**:
- 定义一个触发器区域（需要到达的目标位置）
- 玩家进入区域后开始倒计时
- 倒计时完成后触发任务成功事件

**关键属性**:
```csharp
public float RequiredExtrationTime  // 需要停留的时间
public float RemainingTime          // 剩余时间
public float Progress               // 进度（0-1）
public bool enabled                 // 区域是否启用
```

**关键方法**:
- `OnTriggerEnter/Exit()` - 检测玩家进入/离开
- `BeginCountDown()` - 开始倒计时
- `AbortCountDown()` - 中止倒计时

**可访问的事件**:
- `onCountDownStarted` - 倒计时开始
- `onCountDownStopped` - 倒计时停止
- `onCountDownSucceed` - 倒计时成功

---

### 1.2 ShowLocationInMap (地图位置显示)
**位置**: `TeamSoda.Duckov.Core/ShowLocationInMap.cs`

**功能**:
- 标记一个在小地图上显示的位置
- 提供显示名称

**关键属性**:
```csharp
public string DisplayName      // 显示在小地图上的名称
public string DisplayNameRaw   // 原始名称
```

**关键特性**:
- 这个组件通常挂载在一个GameObject上，该GameObject也包含Collider（用于触发区域）
- 结合Zone/CountDownArea使用来标记任务位置

---

### 1.3 Zone (区域基类)
**位置**: `TeamSoda.Duckov.Core/Zone.cs`

**功能**:
- 基础区域管理
- 跟踪在区域内的所有角色
- 支持按距离激活（`setActiveByDistance`）

**关键属性**:
```csharp
public HashSet<Health> Healths      // 区域内的所有角色
public bool onlyPlayerTeam          // 仅限玩家队伍
```

---

### 1.4 BoundaryGenerator (边界生成器)
**位置**: `TeamSoda.Duckov.Core/BoundaryGenerator.cs`

**功能**:
- 根据多个点生成边界碰撞体
- 创建多边形或环形的触发区域

**关键属性**:
```csharp
public List<Vector3> points         // 定义区域边界的点列表
public float colliderThickness      // 碰撞体厚度
public float colliderHeight         // 碰撞体高度
```

---

## 2. 任务目标位置的实现方式

### 2.1 GameObject 结构
```
[GameObject] - Task Target Location
├── [CountDownArea] Component
├── [ShowLocationInMap] Component  
├── [Collider] Component (BoxCollider or CapsuleCollider)
│   └── isTrigger = true
├── [BoundaryGenerator] Component (可选，用于复杂边界)
└── [Zone] Component (可选，用于管理多个角色)
```

### 2.2 工作流程
```
1. 游戏加载时：ShowLocationInMap将位置标记到小地图
2. 玩家接近时：Collider触发OnTriggerEnter
3. 玩家进入区域：CountDownArea的OnTriggerEnter被调用
4. 玩家停留：CountDownArea开始计时
5. 计时完成：触发onCountDownSucceed事件，任务完成
```

---

## 3. ESP标记策略

### 3.1 3D ESP 标记
在游戏世界中绘制：
- **位置**: CountDownArea的GameObject位置
- **外观**: 
  - 圆形边框（表示到达范围）
  - 显示距离
  - 显示任务名称（从ShowLocationInMap获取）
- **颜色**: 特殊的任务目标颜色（黄色或绿色）

### 3.2 小地图标记
在小地图上显示：
- 已有ShowLocationInMap组件处理
- 我们需要检测到这些区域并标记
- 可以根据RequiredExtrationTime添加进度指示

### 3.3 可视化需求
```
3D ESP中:
- 绘制圆形范围指示（使用CountDownArea的Collider范围）
- 显示文字: "[任务名称] [剩余时间:0s]"
- 显示距离: "[100m]"
- 根据进度改变颜色

小地图中:
- 显示位置标记点
- 显示任务名称
- 可选：显示进度条
```

---

## 4. 代码实现方案

### 4.1 数据结构
```csharp
public class QuestZoneMarkerData
{
    public CountDownArea countDownArea;
    public ShowLocationInMap locationMarker;
    public Collider triggerCollider;
    public GameObject markerObject;
    public Vector3 center;
    public float radius;
    public string displayName;
    public float requiredTime;
    public bool isActive;
}
```

### 4.2 扫描流程
1. `FindObjectsOfType<CountDownArea>()` - 找到所有任务区域
2. 对每个CountDownArea，检查是否有：
   - `ShowLocationInMap` 组件 (获取显示名称)
   - `Collider` 组件 (获取范围)
3. 缓存所有找到的任务区域
4. 定时更新位置和状态

### 4.3 绘制流程
**OnRenderObject()中**:
- 使用GL绘制3D圆形范围指示

**OnGUI()中**:
- 绘制任务名称和进度信息
- 绘制距离显示

---

## 5. 优化考虑

### 5.1 性能
- 缓存`FindObjectsOfType`结果，每帧不重复调用
- 只在关卡加载时扫描一次，后续只更新状态
- 使用距离检查决定是否绘制标记

### 5.2 准确性
- 从CountDownArea.Collider获取精确的触发范围
- 处理复杂边界（使用BoundaryGenerator的points）
- 支持多个同心区域

### 5.3 视觉
- 不同状态使用不同颜色：
  - 未激活：灰色
  - 激活中：黄色/绿色
  - 进行中（计时中）：闪烁或渐变
  - 已完成：绿色或消失

---

## 6. 相关API总结

### 6.1 获取任务区域
```csharp
CountDownArea[] areas = FindObjectsOfType<CountDownArea>();
```

### 6.2 获取显示信息
```csharp
ShowLocationInMap locationMarker = countDownArea.GetComponent<ShowLocationInMap>();
if (locationMarker != null)
{
    string name = locationMarker.DisplayName;
}
```

### 6.3 获取触发范围
```csharp
Collider collider = countDownArea.GetComponent<Collider>();
float radius = collider.bounds.extents.magnitude;
Vector3 center = collider.bounds.center;
```

### 6.4 获取状态
```csharp
float remainingTime = countDownArea.RemainingTime;
float progress = countDownArea.Progress;
bool isCountingDown = countDownArea.IsCountingDown; // 需要反射获取
```

---

## 7. 实现优先级建议

### Phase 1 (基础)
- [x] 扫描所有CountDownArea
- [x] 从ShowLocationInMap获取名称
- [x] 获取Collider范围
- [x] 在小地图创建标记
- [x] 在3D ESP绘制圆形

### Phase 2 (增强)
- [ ] 显示剩余时间
- [ ] 显示进度百分比
- [ ] 根据状态改变颜色
- [ ] 支持复杂边界（BoundaryGenerator）

### Phase 3 (优化)
- [ ] 缓存优化
- [ ] 性能测试
- [ ] 距离剔除
- [ ] 动画效果

