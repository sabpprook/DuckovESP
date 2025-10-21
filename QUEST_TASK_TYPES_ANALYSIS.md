# 任务类型分析 - 地点信息分布

## 📊 任务类型汇总 (10 种)

| # | 任务类型 | 类名 | 有地点信息 | 地点来源 | 备注 |
|---|---------|------|----------|---------|------|
| 1 | 到达位置 | **QuestTask_ReachLocation** | ✅ **有** | `location` (MultiSceneLocation) | 最直接的位置任务 |
| 2 | 提交物品 | **SubmitItems** | ✅ **有** | `mapElement` (MapElementForTask) | 可能在指定地点提交 |
| 3 | 使用物品 | **QuestTask_UseItem** | ❌ 无 | 无位置限制 | 全地图范围内使用 |
| 4 | 提交金钱 | **QuestTask_SubmitMoney** | ❌ 无 | 无位置限制 | 全地图范围内提交 |
| 5 | 建造建筑 | **QuestTask_ConstructBuilding** | ❌ 无 | buildingID (动态) | 建筑位置由游戏决定 |
| 6 | 击杀计数 | **QuestTask_KillCount** | ❌ 无 | 可能需要指定场景 | 场景限制，非位置 |
| 7 | 检查存档 | **QuestTask_CheckSaveData** | ❌ 无 | 无位置 | 纯数据检查 |
| 8 | 撤离逃脱 | **QuestTask_Evacuate** | ❌ 无 | 无固定位置 | 撤离点由游戏决定 |
| 9 | 解锁天赋 | **QuestTask_UnlockPerk** | ❌ 无 | 无位置 | 技能树操作 |
| 10 | 任务事件 | **QuestTask_TaskEvent** | ✅ **有** | `mapElement` (MapElementForTask) | 事件触发，但有地图标记 |

---

## 🎯 有地点信息的任务类型 (3 种)

### 1️⃣ **QuestTask_ReachLocation** ⭐ (最优先)
```csharp
[SerializeField]
private MultiSceneLocation location;      // ← 地点对象
[SerializeField]
private float radius = 1f;                // ← 到达范围
[SerializeField]
private MapElementForTask mapElement;     // ← 地图标记
```
- **检测方法**: `location.TryGetLocationPosition(out Vector3 position)`
- **状态检查**: `finished` 字段
- **已验证**: ✅ 在游戏中找到 12 个实例，6 个成功提取坐标

---

### 2️⃣ **SubmitItems** (需要验证)
```csharp
[SerializeField]
[ItemTypeID]
private int itemTypeID;                   // ← 物品类型

[SerializeField]
private MapElementForTask mapElement;     // ← 可能有地点！
```
- **特点**: 有 `mapElement` 字段，可能指定提交位置
- **检测方法**: 如果 `mapElement` 不为 null → 提取其地点
- **状态检查**: `submittedAmount >= requiredAmount`
- **需要验证**: mapElement 是否自动配置

---

### 3️⃣ **QuestTask_TaskEvent** (次优先)
```csharp
[SerializeField]
private string eventKey;                  // ← 事件标识

[SerializeField]
private MapElementForTask mapElement;     // ← 可能有事件发生地点
```
- **特点**: 虽然是事件驱动，但可能在特定位置触发
- **检测方法**: 如果 `mapElement` 不为 null → 提取其地点
- **状态检查**: `finished` 字段
- **需要验证**: mapElement 是否标记事件位置

---

## 🔍 其他任务类型的分析

### QuestTask_UseItem
- ❌ 无位置信息，全地图范围
- 不需要标记

### QuestTask_SubmitMoney
- ❌ 无位置信息，全地图范围
- 不需要标记

### QuestTask_ConstructBuilding
- ❌ 无固定位置，建筑由游戏动态生成
- 建筑位置由 BuildingManager 管理

### QuestTask_KillCount
- ❌ 虽然有 `requireSceneID`，但这是场景限制，不是位置
- 不需要标记具体地点

### QuestTask_CheckSaveData
- ❌ 完全是数据检查，无位置
- 不需要标记

### QuestTask_Evacuate
- ❌ 撤离点由游戏决定，非静态位置
- CountDownArea 已实现 (出口标记)

### QuestTask_UnlockPerk
- ❌ 技能树操作，无位置
- 不需要标记

---

## 📋 改进计划

### 优先级 1: QuestTask_ReachLocation ✅ (已完成)
- [x] 检测实例: 12 个
- [x] 提取坐标: 6/12 成功
- [x] 渲染标记: 待实现

### 优先级 2: SubmitItems (需要改进)
- [ ] 验证 mapElement 字段是否有效
- [ ] 从 mapElement 提取地点
- [ ] 渲染标记

### 优先级 3: QuestTask_TaskEvent (需要验证)
- [ ] 验证 mapElement 字段是否有效
- [ ] 从 mapElement 提取地点
- [ ] 渲染标记

---

## 🛠️ 实现策略

### 增强 ScanQuestZones() 方法

```csharp
// 方案 1: 通过 mapElement 统一提取
foreach (var task in allTasks)
{
    Type taskType = task.GetType();
    
    // 先尝试直接位置提取 (QuestTask_ReachLocation)
    if (taskType.Name == "QuestTask_ReachLocation")
    {
        // 使用 location.TryGetLocationPosition()
    }
    
    // 再尝试从 mapElement 提取 (SubmitItems, QuestTask_TaskEvent)
    var mapElementField = taskType.GetField("mapElement", BindingFlags.NonPublic | BindingFlags.Instance);
    if (mapElementField != null)
    {
        var mapElement = mapElementField.GetValue(task);
        if (mapElement != null)
        {
            // mapElement.locations[] → 多个位置
            // mapElement.range → 范围
        }
    }
}
```

### 关键 Bug 修复

当前发现的问题:
- `TryGetLocationPosition()` 对某些位置返回 false (6/12)
- 需要寻找替代位置获取方法

**备选方案**:
1. 检查 `location` 对象的其他属性 (GetDisplayName 工作，说明对象有效)
2. 搜索 location 名称在场景中的对象 (SceneLocationsProvider)
3. 添加调试: 输出所有属性名，找到包含坐标的字段

