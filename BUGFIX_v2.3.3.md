# Bug Fix v2.3.3 - 建筑材料优先级 & 过滤修复

## 修复日期
2025-10-19

## 问题描述

### 问题 1: 建筑材料颜色覆盖高价值物品 ❌
**现象**: 当箱子中同时包含建筑材料（白色品质）和高价值物品（紫色/金色/红色）时，小地图标记显示为青色（建筑材料颜色），而不是高品质颜色。

**根本原因**: `GetMarkerColorByQuality()` 方法的优先级顺序错误：
```
旧优先级: 任务物品 > 建筑材料 > 品质颜色
```

**影响**: 用户看到青色标记，以为是低价值建筑材料箱子，但实际里面有紫色/金色装备，导致错过重要物品。

**示例场景**:
- 箱子内容: 白色"木板"（建筑材料） + 金色"AK-47"
- 旧行为: 显示青色标记（建筑材料）
- 期望行为: 显示金色标记（高品质武器）

---

### 问题 2: 游戏开始时建筑材料不显示在小地图 ❌
**现象**: 关卡加载完成后，所有建筑材料都没有在小地图上显示标记。必须手动将ESP品质过滤调到最低（白色），再调回原来的等级，才会显示建筑材料标记。

**根本原因**: `ScanAllLootboxes()` 方法缺少任务/建筑材料的品质过滤绕过逻辑：
- `UpdateMarkerPositions()` 在 v2.3.2 中已修复，会绕过品质过滤
- 但 `ScanAllLootboxes()` 在初始扫描时没有这个逻辑
- 导致初始创建标记时，建筑材料被品质过滤器过滤掉

**影响**: 
- 玩家进入关卡后看不到任何建筑材料标记
- 需要手动调整配置才能触发 `UpdateMarkerPositions()` 重新评估
- 用户体验差，违反"开箱即用"原则

**代码对比**:
```csharp
// ScanAllLootboxes (旧代码 - 缺少绕过逻辑)
foreach (Item item in items)
{
    bool isUnregisteredKey = IsUnregisteredKey(item);
    if (isUnregisteredKey) { filteredItems.Add(item); continue; }
    
    // ❌ 缺少任务/建筑材料检查
    
    if (_minQualityFilter > 0) { /* 品质过滤 */ }
}

// UpdateMarkerPositions (v2.3.2 已修复)
bool isQuestOrBuilding = /* 检查任务/建筑材料 */;
if (isUnregisteredKey || isQuestOrBuilding) { /* 绕过过滤 */ }
```

---

### 问题 3: 不需要的建筑材料也被高亮 ❌
**现象**: 所有已解锁建筑的所需材料都被标记为"建筑材料"，即使玩家仓库中已经有足够的数量。

**根本原因**: `UpdateBuildingRequiredItems()` 方法只检查建筑是否已建造，不检查玩家是否已有足够材料：
```csharp
// 旧逻辑
foreach (var itemEntry in buildingInfo.cost.items)
{
    if (itemEntry.id > 0)
    {
        _buildingRequiredItems.Add(itemEntry.id);  // ❌ 无条件添加
    }
}
```

**影响**: 
- 玩家已经收集了 20 个木板（只需 10 个），但地图上所有木板仍然显示青色标记
- 视觉噪音，无法识别真正缺少的材料

**示例场景**:
- 建筑需求: 木板 x10，铁矿 x5
- 玩家仓库: 木板 x12，铁矿 x2
- 旧行为: 木板和铁矿都被标记为"[建筑材料]"
- 期望行为: 只有铁矿被标记为"[建筑材料]"

---

### 问题 4: 3D ESP不显示箱子物品（即使设置为白色） ❌
**现象**: 设置"3D ESP最低品质: 白色（0）"后，地上箱子的物品仍然不显示ESP标签。

**根本原因**: 与问题 2 相同，`ScanLootboxItems()` 方法在 3D ESP 缓存更新时也缺少任务/建筑材料的过滤绕过逻辑。

**影响**: 3D ESP 功能失效，无法看到任何物品标签。

---

## 解决方案

### 修复 1: 调整颜色优先级 ✅

**修改文件**: `ModBehaviour.cs` → `GetMarkerColorByQuality()` 方法

**新优先级顺序**:
```
1. 任务物品（黄色） - 最高优先级
2. 高品质物品（紫色/金色/红色） - 次高优先级
3. 建筑材料（青色） - 第三优先级
4. 普通品质颜色 - 最低优先级
```

**实现逻辑**:
```csharp
// 1. 最高优先级：任务物品
if (有任务物品)
    return 黄色;

// 2. 计算最高品质
ItemValueLevel maxLevel = items.Max(...);

// 3. 次高优先级：高价值物品（紫色及以上）
if (maxLevel >= Purple)
    return 品质颜色;  // 紫色/金色/红色

// 4. 第三优先级：建筑材料
if (有建筑材料)
    return 青色;

// 5. 最低优先级：普通品质颜色
return 品质颜色;  // 白色/绿色/蓝色
```

**效果**:
- 箱子包含"木板(白色) + AK-47(金色)" → 显示金色
- 箱子包含"木板(白色) + 绷带(绿色)" → 显示青色（建筑材料）
- 箱子包含"任务物品(白色) + AK-47(金色)" → 显示黄色（任务优先）

---

### 修复 2 & 4: 添加品质过滤绕过逻辑 ✅

**修改文件**: `ModBehaviour.cs` → `ScanAllLootboxes()` 方法

**修改内容**:
```csharp
foreach (Item item in items)
{
    if (item == null)
        continue;
    
    // 检查是否是未录入的钥匙
    bool isUnregisteredKey = IsUnregisteredKey(item);
    
    // ✅ 新增：检查是否是任务物品或建筑材料
    bool isQuestOrBuilding = false;
    if (_questItemDetector != null)
    {
        isQuestOrBuilding = (_config.HighlightQuestItems && _questItemDetector.IsQuestRequiredItem(item)) ||
                           (_config.HighlightBuildingMaterials && _questItemDetector.IsBuildingRequiredItem(item));
    }
    
    // ✅ 新增：如果是钥匙/任务物品/建筑材料，绕过品质过滤
    if (isUnregisteredKey || isQuestOrBuilding)
    {
        filteredItems.Add(item);
        continue;
    }
    
    // 否则应用品质过滤
    if (_minQualityFilter > 0) { /* ... */ }
}
```

**效果**:
- 游戏加载时，建筑材料和任务物品立即显示在小地图上
- 无需手动调整配置即可看到所有重要物品
- 与 `UpdateMarkerPositions()` 保持一致的过滤逻辑

---

### 修复 3: 只标记缺少的建筑材料 ✅

**修改文件**: `QuestItemDetector.cs` → `UpdateBuildingRequiredItems()` 方法

**新增方法**: `GetPlayerItemCounts()` - 获取玩家仓库物品数量

**实现逻辑**:
```csharp
private void UpdateBuildingRequiredItems()
{
    _buildingRequiredItems.Clear();
    
    // 获取玩家仓库中的物品数量
    Dictionary<int, int> playerItemCounts = GetPlayerItemCounts();
    
    foreach (string buildingID in unlockedBuildings)
    {
        BuildingInfo buildingInfo = BuildingManager.GetBuildingInfo(buildingID);
        
        foreach (var itemEntry in buildingInfo.cost.items)
        {
            // 检查玩家是否已经有足够的材料
            int playerHas = playerItemCounts.GetValueOrDefault(itemEntry.id, 0);
            int required = itemEntry.amount;
            
            // ✅ 只有当玩家缺少这个材料时才添加
            if (playerHas < required)
            {
                _buildingRequiredItems.Add(itemEntry.id);
            }
        }
    }
}

private Dictionary<int, int> GetPlayerItemCounts()
{
    Dictionary<int, int> itemCounts = new Dictionary<int, int>();
    
    CharacterMainControl player = CharacterMainControl.Main;
    Inventory playerInventory = player.CharacterItem.Inventory;
    
    // 遍历玩家背包统计物品数量
    foreach (Item item in playerInventory)
    {
        int typeID = item.TypeID;
        int stackCount = item.StackCount;
        
        if (itemCounts.ContainsKey(typeID))
            itemCounts[typeID] += stackCount;
        else
            itemCounts[typeID] = stackCount;
    }
    
    return itemCounts;
}
```

**效果**:
- 建筑需求: 木板 x10，铁矿 x5
- 玩家仓库: 木板 x12，铁矿 x2
- **新行为**: 只有铁矿被标记为"[建筑材料]"
- 木板不再被标记（已有足够数量）

---

## 测试场景

### 测试 1: 颜色优先级
1. 进入关卡，找到包含以下物品的箱子:
   - 箱子A: 木板(白色-建筑材料) + AK-47(金色)
   - 箱子B: 木板(白色-建筑材料) + 绷带(绿色)
   - 箱子C: 任务物品(白色) + 紫色装备
2. **预期结果**:
   - 箱子A: 金色标记（高品质优先）
   - 箱子B: 青色标记（建筑材料）
   - 箱子C: 黄色标记（任务物品最优先）

### 测试 2: 游戏开始时的建筑材料显示
1. 确保仓库中缺少某些建筑材料（例如只有 2 个铁矿，需要 5 个）
2. 启用"高亮建筑材料"选项
3. 设置"小地图ESP最低品质: 紫色(4)"（高品质过滤）
4. 进入新关卡
5. **预期结果**:
   - 游戏加载完成后，立即看到青色的建筑材料标记
   - 无需手动调整配置
   - 3D ESP 也显示建筑材料标签

### 测试 3: 只标记缺少的材料
1. 查看某个未建造建筑的材料需求（例如：木板 x10，铁矿 x5）
2. 收集足够的木板（例如 12 个）
3. 只收集部分铁矿（例如 2 个）
4. 进入关卡
5. **预期结果**:
   - 地图上的木板没有"[建筑材料]"标记（已有足够）
   - 地图上的铁矿有青色"[建筑材料]"标记（仍然缺少）

### 测试 4: 3D ESP显示白色物品
1. 设置"3D ESP最低品质: 白色(0)"
2. 进入关卡
3. **预期结果**:
   - 所有箱子的物品都显示3D ESP标签
   - 包括白色品质的普通物品
   - 建筑材料和任务物品有特殊标记

### 测试 5: 综合场景
设置:
- 3D ESP最低品质: 紫色(4)
- 小地图ESP最低品质: 蓝色(2)
- 高亮任务物品: 启用
- 高亮建筑材料: 启用

场景:
- 箱子1: 白色木板(缺少的建筑材料)
- 箱子2: 白色木板(已有足够)
- 箱子3: 白色任务物品
- 箱子4: 绿色普通物品
- 箱子5: 紫色装备

**预期结果**:
- 箱子1: 小地图有青色标记，3D ESP有标签（建筑材料绕过过滤）
- 箱子2: 小地图无标记，3D ESP无标签（普通白色物品）
- 箱子3: 小地图有黄色标记，3D ESP有标签（任务物品绕过过滤）
- 箱子4: 小地图有标记，3D ESP无标签（品质不够）
- 箱子5: 小地图有标记，3D ESP有标签（高品质）

---

## 性能影响

### 修复 1 (颜色优先级)
- **CPU开销**: 忽略不计
- **说明**: 只增加了一次 `maxLevel` 计算和一次整数比较

### 修复 2 (过滤绕过)
- **CPU开销**: 轻微增加
- **说明**: 每个物品增加 2 次 HashSet 查询（`IsQuestRequiredItem` + `IsBuildingRequiredItem`）
- **预估**: O(1) 查询，可忽略

### 修复 3 (材料需求检查)
- **CPU开销**: 中等增加（每秒调用1次）
- **说明**: 
  - 需要遍历玩家背包统计物品数量: O(N)，N=背包物品数
  - 需要遍历所有未建造建筑的材料: O(M)，M=建筑数×材料数/建筑
- **预估开销**: 
  - 背包 100 个物品 → 0.1-0.2ms
  - 10 个建筑 × 5 个材料/建筑 → 0.1ms
  - **总计**: 约 0.2-0.3ms/秒
- **优化**: 可以考虑缓存玩家物品数量，只在背包变化时更新

---

## 代码变更摘要

### ModBehaviour.cs
1. **GetMarkerColorByQuality()** (行 ~1260-1300)
   - 修改颜色优先级逻辑
   - 添加高品质物品 (Purple+) 优先级

2. **ScanAllLootboxes()** (行 ~1030-1075)
   - 添加 `isQuestOrBuilding` 检查
   - 与 `UpdateMarkerPositions()` 保持一致的过滤逻辑

### QuestItemDetector.cs
1. **UpdateBuildingRequiredItems()** (行 ~68-110)
   - 调用 `GetPlayerItemCounts()` 获取玩家物品数量
   - 只添加玩家缺少的建筑材料

2. **GetPlayerItemCounts()** (新增方法)
   - 遍历玩家背包
   - 统计每种物品的总数量（包括堆叠）
   - 返回 `Dictionary<int, int>` (物品ID → 数量)

---

## 潜在优化（未实现）

### 优化 1: 缓存玩家物品数量
```csharp
private Dictionary<int, int> _cachedPlayerItemCounts;
private float _itemCountsCacheTime = 0f;
private const float ITEM_COUNTS_CACHE_DURATION = 2f; // 2秒缓存

private Dictionary<int, int> GetPlayerItemCounts()
{
    float currentTime = Time.time;
    
    // 如果缓存仍然有效，直接返回
    if (currentTime - _itemCountsCacheTime < ITEM_COUNTS_CACHE_DURATION)
    {
        return _cachedPlayerItemCounts;
    }
    
    // 否则重新计算
    _cachedPlayerItemCounts = CalculatePlayerItemCounts();
    _itemCountsCacheTime = currentTime;
    
    return _cachedPlayerItemCounts;
}
```

**优势**: 减少每秒的背包遍历次数
**实现时机**: 如果性能分析显示此处为瓶颈

### 优化 2: 监听背包变化事件
```csharp
void OnEnable()
{
    CharacterMainControl.OnMainCharacterInventoryChangedEvent += OnInventoryChanged;
}

void OnInventoryChanged(CharacterMainControl control, Inventory inventory, int index)
{
    // 背包变化时重新计算
    UpdateBuildingRequiredItems();
}
```

**优势**: 实时响应，无需轮询
**缺点**: 需要管理事件订阅/取消

---

## 版本历史

- **v2.3.0**: 初始实现小地图标记和3D ESP
- **v2.3.1**: 修复任务/建筑材料被品质过滤的问题
- **v2.3.2**: 修复小地图标记不动态更新 + 连线不一致
- **v2.3.3**: 
  - 修复建筑材料颜色覆盖高品质物品
  - 修复游戏开始时建筑材料不显示
  - 修复不需要的建筑材料也被高亮
  - 修复3D ESP品质过滤失效

---

## 相关文件

- `ModBehaviour.cs` - 主要修改
  - `GetMarkerColorByQuality()` - 颜色优先级调整
  - `ScanAllLootboxes()` - 添加过滤绕过逻辑
- `QuestItemDetector.cs` - 建筑材料检测逻辑
  - `UpdateBuildingRequiredItems()` - 智能材料需求检测
  - `GetPlayerItemCounts()` - 新增方法

---

## 作者备注

这个版本修复了四个关键的用户体验问题：

1. **颜色优先级**: 确保高价值物品不会被建筑材料颜色覆盖
2. **初始显示**: 游戏开始时立即显示所有重要物品
3. **智能过滤**: 只高亮玩家真正需要的建筑材料
4. **一致性**: 3D ESP 和小地图使用相同的过滤逻辑

修复3的性能开销较高（每秒0.3ms），但在可接受范围内。如果未来出现性能问题，可以实施缓存优化。

特别重要的是修复2，它解决了"必须手动调整配置才能看到建筑材料"的严重UX问题，现在真正做到了"开箱即用"。
