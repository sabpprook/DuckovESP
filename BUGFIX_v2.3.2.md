# Bug Fix v2.3.2 - 小地图标记过滤 & 连线优化

## 修复日期
2025-10-19

## 问题描述

### 问题 1: 小地图标记不会随品质过滤器变化而更新
**现象**: 当玩家提高小地图ESP的品质过滤等级时（例如从"显示所有"改为"仅紫色以上"），已经创建的低品质物品标记不会被移除，导致地图上显示大量不符合当前过滤条件的标记。

**根本原因**: `UpdateMarkerPositions()` 方法只检查箱子是否被掠夺或为空，但不重新评估物品是否仍然符合当前的品质过滤条件。

**影响**: 用户体验差，地图上充斥着不需要的低品质物品标记。

### 问题 2: 3D ESP连线与小地图标记不一致
**现象**: 某些物品在3D ESP中显示（因为3D ESP有独立的品质过滤器），但在小地图上没有标记（因为小地图ESP有更高的品质要求），这些物品仍然会绘制从玩家到物品的连线。

**根本原因**: `DrawItemLines()` 方法遍历所有 `_espDataCache` 中的物品（3D ESP缓存），无条件绘制连线，没有检查物品是否在小地图上有对应标记。

**影响**: 视觉混乱，连线指向的物品在地图上可能不存在标记。

## 解决方案

### 修复 1: 动态重新评估标记有效性

**修改文件**: `ModBehaviour.cs` → `UpdateMarkerPositions()` 方法

**实现逻辑**:
1. 在每次更新标记位置时，重新从箱子的 `Inventory` 读取物品列表
2. 应用当前的品质过滤条件（与 `ScanAllLootboxes()` 相同的逻辑）
3. 如果过滤后没有符合条件的物品，将标记添加到移除列表
4. 更新 `LootboxMarkerData.items` 为过滤后的物品列表

**关键代码**:
```csharp
// 重新检查物品是否仍然符合当前的品质过滤条件
List<Item> currentItems = GetItemsFromInventory(data.inventory);
List<Item> filteredItems = new List<Item>();

foreach (Item item in currentItems)
{
    // 应用与 ScanAllLootboxes 相同的过滤逻辑
    bool isUnregisteredKey = IsUnregisteredKey(item);
    bool isQuestOrBuilding = /* 检查任务/建筑材料 */;
    
    if (isUnregisteredKey || isQuestOrBuilding)
    {
        filteredItems.Add(item);  // 绕过品质过滤
        continue;
    }
    
    // 应用品质过滤
    if (_minQualityFilter > 0)
    {
        ItemValueLevel level = ItemQualityUtil.GetItemValueLevel(item);
        if ((int)level >= _minQualityFilter)
        {
            filteredItems.Add(item);
        }
    }
}

// 如果没有符合条件的物品了，移除标记
if (filteredItems.Count == 0 && !_showLowValueItems)
{
    toRemove.Add(box);
    continue;
}

// 更新缓存的物品列表
data.items = filteredItems;
```

### 修复 2: 连线只绘制有小地图标记的物品

**修改文件**: `ModBehaviour.cs` → `DrawItemLines()` 方法

**实现逻辑**:
1. 在绘制每个物品连线前，检查该物品位置是否有对应的小地图标记
2. 通过遍历 `_trackedLootboxes` 并比较位置距离来判断
3. 如果距离小于 0.1米，认为是同一个位置
4. 只有在小地图上有标记的物品才绘制连线

**关键代码**:
```csharp
foreach (var espData in _espDataCache)
{
    // 检查这个物品位置是否有对应的小地图标记
    bool hasMapMarker = false;
    foreach (var markerData in _trackedLootboxes.Values)
    {
        if (markerData.lootbox != null && 
            Vector3.Distance(markerData.lootbox.transform.position, espData.worldPosition) < 0.1f)
        {
            hasMapMarker = true;
            break;
        }
    }
    
    // 如果没有小地图标记，跳过绘制连线
    if (!hasMapMarker)
        continue;
    
    // ... 绘制连线逻辑
}
```

## 测试步骤

### 测试场景 1: 动态品质过滤
1. 进入游戏关卡，打开配置菜单
2. 设置"小地图ESP最低品质: 白色（0）"
3. 观察小地图上出现大量物品标记（包括白色、绿色等低品质物品）
4. 将"小地图ESP最低品质"提高到"紫色（4）"
5. **预期结果**: 1-2秒后（下次 `UpdateMarkerPositions()` 调用时），低品质物品标记从地图上消失
6. 只有紫色、金色、红色品质的物品标记保留

### 测试场景 2: 连线与标记一致性
1. 设置"3D ESP最低品质: 白色（0）"（显示所有物品）
2. 设置"小地图ESP最低品质: 紫色（4）"（只显示高品质）
3. 启用"显示连接线"选项
4. 进入游戏并观察
5. **预期结果**: 
   - 3D ESP框显示所有品质的物品（白色、绿色、蓝色、紫色等）
   - 小地图上只有紫色及以上的标记
   - **连线只连接到有小地图标记的物品**（紫色及以上）
   - 没有小地图标记的低品质物品（白色、绿色、蓝色）不会绘制连线

### 测试场景 3: 特殊物品绕过
1. 接取需要白色品质物品的任务（例如"木板"）
2. 设置"小地图ESP最低品质: 紫色（4）"
3. 进入关卡寻找任务物品
4. **预期结果**: 即使设置了高品质过滤，白色的任务物品仍然:
   - 在小地图上有标记（带"[任务物品]"标签）
   - 在3D ESP中显示
   - **有连线指向玩家**（因为有小地图标记）

## 性能影响

### 修复 1
- **CPU开销**: 轻微增加（每秒调用1次 `UpdateMarkerPositions()`）
- **每次调用**: 需要重新遍历所有追踪的箱子并应用品质过滤
- **预估开销**: 10-50个箱子时约 0.1-0.5ms 额外开销
- **优化点**: 只在配置改变时重新评估（未实现，因为配置改变频率低）

### 修复 2
- **CPU开销**: 中等增加（每帧在 `OnRenderObject()` 中调用）
- **每次调用**: 需要对每个ESP物品进行位置匹配检查
- **预估开销**: O(N * M)，N=ESP物品数，M=小地图标记数
- **最坏情况**: 100个ESP物品 × 50个标记 = 5000次距离计算/帧
- **优化建议**: 
  - 方案A: 在 `UpdateESPCache()` 中标记哪些物品有小地图标记（推荐）
  - 方案B: 使用空间哈希加速位置匹配
  - 方案C: 只在箱子物品时匹配，世界物品直接跳过

## 潜在优化（未实现）

### 优化 1: 标记 ESP 缓存中的小地图状态
```csharp
private class ESPData
{
    public Vector3 worldPosition;
    public List<Item> items;
    public float distance;
    public ItemValueLevel maxLevel;
    public bool hasMapMarker; // 新增：是否有小地图标记
}

// 在 UpdateESPCache() → ScanLootboxItems() 中设置
espData.hasMapMarker = _trackedLootboxes.ContainsKey(box);

// 在 DrawItemLines() 中直接检查
if (!espData.hasMapMarker)
    continue;
```

**优势**: 将 O(N*M) 降低到 O(1) 检查
**实现时机**: 如果性能成为瓶颈

### 优化 2: 配置变更监听
```csharp
private int _lastMinQualityFilter = -1;

private void UpdateMarkerPositions()
{
    // 只在配置改变时重新评估所有标记
    bool configChanged = (_lastMinQualityFilter != _minQualityFilter);
    if (configChanged)
    {
        _lastMinQualityFilter = _minQualityFilter;
        // 触发全量重新评估
    }
}
```

**优势**: 避免每秒都重新过滤
**缺点**: 箱子内容变化时不会立即反映

## 版本历史

- **v2.3.0**: 初始实现小地图标记和3D ESP
- **v2.3.1**: 修复任务/建筑材料被品质过滤错误过滤的问题
- **v2.3.2**: 
  - 修复小地图标记不随品质过滤器动态更新的问题
  - 修复3D ESP连线与小地图标记不一致的问题

## 相关文件

- `ModBehaviour.cs` - 主要修改
  - `UpdateMarkerPositions()` - 添加动态过滤逻辑
  - `DrawItemLines()` - 添加小地图标记检查
- `ESPConfig.cs` - 配置系统（无修改）
- `QuestItemDetector.cs` - 任务物品检测（无修改）

## 作者备注

这两个修复解决了用户体验中的两个关键问题：
1. 用户现在可以动态调整品质过滤器，地图会立即响应
2. 连线现在与小地图标记保持一致，避免视觉混乱

修复2的性能开销需要关注，如果未来出现性能问题，优先实现"优化1"方案。
