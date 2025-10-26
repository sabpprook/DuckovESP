# Inventory 监听器实现文档

## 📋 功能概述

实现了实时监听游戏中物品拾取事件的系统，当玩家从箱子中拿走物品或捡起地面物品时，ESP 标记会自动更新或移除。

## 🎯 核心功能

### 1. 箱子物品监听
- **监听方式**：订阅 `Inventory.onContentChanged` 事件
- **触发时机**：当箱子中的物品被拿走时
- **处理逻辑**：
  - 箱子变空 → 移除整个箱子标记
  - 箱子还有物品 → 刷新标记内容（更新物品列表和品质颜色）

### 2. 地面物品监听
- **监听方式**：订阅 `Item.onParentChanged` 事件
- **触发时机**：当物品被捡起（添加到 Inventory）时
- **处理逻辑**：
  - 检测物品是否进入玩家 Inventory
  - 确认后移除该物品的 ESP 标记

## 🏗️ 架构设计

### 核心类：`InventoryMonitor`

**位置**：`Core/Systems/ESP/Detection/InventoryMonitor.cs`

**职责**：
1. 管理所有 Inventory 和 Item 的事件订阅
2. 追踪物品快照，检测变化
3. 触发自定义事件通知 ESP 系统更新

**事件定义**：
```csharp
// 箱子物品被移除
public event Action<Inventory, Item, int>? OnItemRemovedFromLootbox;

// 地面物品被捡起
public event Action<Item>? OnWorldItemPickedUp;
```

### 集成到 ESPSystemManager

**初始化**：
```csharp
// Initialize() 方法中
_inventoryMonitor = new Detection.InventoryMonitor(_logger);
_inventoryMonitor.OnItemRemovedFromLootbox += HandleItemRemovedFromLootbox;
_inventoryMonitor.OnWorldItemPickedUp += HandleWorldItemPickedUp;
```

**标记创建时启动监听**：
```csharp
// CreateLootboxMarker() 中
_inventoryToLootbox[data.Inventory] = data.Lootbox;
_inventoryMonitor.StartMonitoringLootbox(data.Lootbox, data.Inventory);

// CreateWorldItemMarker() 中
_inventoryMonitor.StartMonitoringWorldItem(data.Item);
```

## 🔍 实现细节

### 箱子物品变化检测

**工作原理**：
1. 记录初始物品快照（HashSet<Item>）
2. 当 `onContentChanged` 触发时：
   - 收集当前所有物品
   - 对比快照找出被移除的物品
   - 更新快照
   - 触发 `OnItemRemovedFromLootbox` 事件

**关键代码**：
```csharp
private void OnLootboxContentChanged(Inventory inventory, int position)
{
    var previousItems = _trackedLootboxes[inventory];
    var currentItems = new HashSet<Item>();
    
    foreach (var item in inventory.Content)
    {
        if (item != null) currentItems.Add(item);
    }
    
    // 找出被移除的物品
    var removedItems = new HashSet<Item>(previousItems);
    removedItems.ExceptWith(currentItems);
    
    // 触发事件
    foreach (var removedItem in removedItems)
    {
        OnItemRemovedFromLootbox?.Invoke(inventory, removedItem, position);
    }
    
    // 更新快照
    _trackedLootboxes[inventory] = currentItems;
}
```

### 地面物品捡起检测

**工作原理**：
1. 监听 `Item.onParentChanged` 事件
2. 检查物品是否被添加到 Inventory
3. 进一步判断是否是玩家 Inventory（排除箱子间转移）
4. 触发 `OnWorldItemPickedUp` 事件

**玩家 Inventory 判断**：
```csharp
private bool IsPlayerInventory(Inventory inventory)
{
    // 方法1：排除箱子 Inventory（在 LootBoxInventoriesParent 下）
    var lootBoxParent = LevelManager.LootBoxInventoriesParent;
    if (lootBoxParent != null && inventory.transform.IsChildOf(lootBoxParent))
    {
        return false;
    }

    // 方法2：检查是否属于主角色
    var character = inventory.GetComponentInParent<CharacterMainControl>();
    if (character != null && character == CharacterMainControl.Main)
    {
        return true;
    }

    // 方法3：检查是否是 PlayerStorage
    var playerStorage = inventory.GetComponentInParent<PlayerStorage>();
    if (playerStorage != null && playerStorage == PlayerStorage.Instance)
    {
        return true;
    }

    return false;
}
```

### ESP 标记更新处理

**箱子标记更新**（`HandleItemRemovedFromLootbox`）：
```csharp
// 1. 查找对应的箱子
if (!_inventoryToLootbox.TryGetValue(inventory, out var lootbox)) return;

// 2. 检查是否为空
bool isEmpty = true;
foreach (var item in inventory.Content)
{
    if (item != null) { isEmpty = false; break; }
}

// 3. 空箱子 → 移除标记
if (isEmpty)
{
    RemoveLootboxMarker(lootbox);
    _inventoryMonitor?.StopMonitoringLootbox(inventory);
    _inventoryToLootbox.Remove(inventory);
}
// 4. 还有物品 → 刷新标记
else
{
    RefreshLootboxMarker(lootbox, inventory);
}
```

**地面物品标记移除**（`HandleWorldItemPickedUp`）：
```csharp
if (_itemMarkers.ContainsKey(pickedItem))
{
    RemoveWorldItemMarker(pickedItem);
    _inventoryMonitor?.StopMonitoringWorldItem(pickedItem);
}
```

## 🛠️ 辅助功能

### 刷新箱子标记（`RefreshLootboxMarker`）

**功能**：当箱子物品部分被拿走时，更新标记显示的内容

**实现步骤**：
1. 重新收集箱子中的所有物品
2. 计算新的最高品质
3. 创建新的 `LootboxData` 对象
4. 重新调用 `marker.Initialize()` 更新显示

**关键代码**：
```csharp
// 重新收集物品和品质
var items = new List<ItemStatsSystem.Item>();
ItemValueLevel maxQuality = ItemValueLevel.White;

foreach (var item in inventory.Content)
{
    if (item == null) continue;
    items.Add(item);
    
    var quality = Utils.ItemQualityUtil.GetItemValueLevel(item);
    if (quality > maxQuality) maxQuality = quality;
}

// 重新初始化标记
var lootboxData = new LootboxData
{
    Lootbox = lootbox,
    Inventory = inventory,
    Items = items,
    MaxItemQuality = maxQuality
};

marker.Initialize(..., lootboxData, ...);
```

## 🔄 生命周期管理

### 资源清理

**OnDisable / OnDestroy**：
```csharp
// 取消所有事件订阅
if (_inventoryMonitor != null)
{
    _inventoryMonitor.OnItemRemovedFromLootbox -= HandleItemRemovedFromLootbox;
    _inventoryMonitor.OnWorldItemPickedUp -= HandleWorldItemPickedUp;
    _inventoryMonitor.Dispose();
    _inventoryMonitor = null;
}
```

**InventoryMonitor.Dispose()**：
```csharp
// 取消所有 Inventory 监听
foreach (var kvp in _trackedLootboxes)
{
    if (kvp.Key != null)
    {
        kvp.Key.onContentChanged -= OnLootboxContentChanged;
    }
}

// 取消所有 Item 监听
foreach (var item in _trackedWorldItems)
{
    if (item != null)
    {
        item.onParentChanged -= OnWorldItemParentChanged;
    }
}
```

## 📊 数据结构

### 追踪数据
```csharp
// InventoryMonitor
private Dictionary<Inventory, HashSet<Item>> _trackedLootboxes;  // 箱子快照
private HashSet<Item> _trackedWorldItems;                         // 地面物品

// ESPSystemManager
private Dictionary<Inventory, InteractableLootbox> _inventoryToLootbox;  // Inventory→箱子映射
```

## 🚀 性能优化

### 1. 快照对比（HashSet）
- **优点**：O(n) 复杂度快速找出差异
- **场景**：箱子物品变化检测

### 2. 刷新而非重建
- **优点**：避免销毁/重建 GameObject，只更新数据
- **场景**：箱子部分物品被拿走时

### 3. 及时停止监听
- **优点**：减少事件处理开销
- **场景**：
  - 箱子变空后停止监听
  - 物品被捡起后停止监听

## 🧪 测试场景

### 测试用例

1. **箱子完全清空**
   - 操作：拿走箱子中所有物品
   - 预期：箱子 ESP 标记消失

2. **箱子部分清空**
   - 操作：拿走箱子中部分物品
   - 预期：
     - 标记仍存在
     - 显示剩余物品列表
     - 颜色更新为剩余物品的最高品质

3. **地面物品捡起**
   - 操作：按 E 捡起地面物品
   - 预期：该物品的 ESP 标记立即消失

4. **物品在箱子间转移**
   - 操作：将物品从一个箱子拖到另一个箱子
   - 预期：两个箱子的标记都正确更新

## ⚠️ 注意事项

### 1. 命名空间冲突
```csharp
// 错误：UnityEngine.ILogger 与自定义 ILogger 冲突
private readonly ILogger _logger;

// 正确：使用完整命名空间
private readonly DuckovESPv3.Infrastructure.Logging.ILogger _logger;
```

### 2. 空引用检查
```csharp
// 所有 Unity 对象访问前都要检查 null
if (inventory != null && inventory.Content != null)
{
    // 安全访问
}
```

### 3. 线程安全
- Unity 事件在主线程触发，无需额外同步
- HashSet 操作在单线程环境安全

## 📝 后续优化建议

1. **性能监控**
   - 添加事件触发频率统计
   - 监控刷新操作耗时

2. **批量更新**
   - 如果多个物品同时被拿走，考虑批量处理

3. **缓存优化**
   - 缓存品质计算结果（如果物品属性不变）

4. **日志等级**
   - 生产环境降低 Debug 日志输出

## 🎓 学习要点

1. **Unity 事件系统**：`Action<T>` 委托的使用
2. **游戏源码分析**：通过反编译找到关键事件
3. **快照模式**：通过对比快照检测变化
4. **分层架构**：InventoryMonitor 独立于 ESP 系统，可复用

## 📚 相关游戏源码

### Inventory.cs
- `event Action<Inventory, int> onContentChanged`
- 在 `AddAt()`、`RemoveAt()` 时触发

### Item.cs
- `event Action<Item> onParentChanged`
- 在 `NotifyAddedToInventory()` 时触发

### LevelManager.cs
- `LootBoxInventoriesParent`：所有箱子 Inventory 的父节点
- 用于区分箱子和玩家 Inventory
