# 功能更新 - 智能过滤与钥匙检测

## 更新内容

### 1. ✅ 排除宠物狗的物品

**问题**：宠物狗背包中的物品也会显示在ESP中，造成干扰。

**解决方案**：
```csharp
// 在 GetItemsFromInventory 中检查是否是宠物背包
if (PetProxy.PetInventory != null && inventory == PetProxy.PetInventory)
{
    isPetInventory = true;
    return items; // 返回空列表
}
```

**效果**：
- ❌ 宠物狗背包不再显示
- ✅ 只显示箱子中的物品
- ✅ 避免玩家误以为箱子里有物品

---

### 2. ✅ 显示未录入的钥匙

**问题**：玩家需要找未录入的钥匙，但高品质过滤会遗漏它们。

**解决方案**：
```csharp
// 新增 IsUnregisteredKey 方法
private bool IsUnregisteredKey(Item item)
{
    // 1. 检查是否有 "Key" 标签
    bool isKey = item.Tags.Any(tag => tag.name.Equals("Key"));
    
    // 2. 检查是否已录入
    bool isRegistered = MasterKeysManager.IsActive(item.TypeID);
    
    // 3. 返回：是钥匙 且 未录入
    return isKey && !isRegistered;
}

// 在过滤逻辑中优先处理钥匙
if (IsUnregisteredKey(item))
{
    filteredItems.Add(item); // 直接添加，无视品质过滤
    continue;
}
```

**效果**：
- ✅ 未录入的钥匙**始终显示**，无视品质过滤
- ✅ 用特殊标记 `[🔑未录入]` 标注
- ✅ 已录入的钥匙按正常品质过滤
- ✅ 帮助玩家快速找到需要录入的钥匙

**示例显示**：
```
[50m]
[🔑未录入] 地下室钥匙
[传说] AK-47
[史诗] 夜视仪
```

---

### 3. ✅ 修复 3D ESP 过滤逻辑

**问题**：当品质过滤设为最低(0)时，箱子仍然不显示。

**原因分析**：
```csharp
// 旧代码的问题
if (_config.MinQualityFilter3D > 0)
{
    filteredItems = new List<Item>();
    // 只有当 MinQualityFilter3D > 0 时才过滤
    // 但 filteredItems 被赋值为新列表，原来的items丢失了！
}
// 当 MinQualityFilter3D == 0 时，filteredItems 还是原来的 items
// 但如果 items 是空的或没有通过其他检查，就不显示
```

**解决方案**：
```csharp
// 新代码：统一的过滤逻辑
List<Item> filteredItems = new List<Item>();

foreach (Item item in items)
{
    // 未录入的钥匙优先
    if (IsUnregisteredKey(item))
    {
        filteredItems.Add(item);
        continue;
    }
    
    // 品质过滤
    if (_config.MinQualityFilter3D > 0)
    {
        ItemValueLevel level = ItemQualityUtil.GetItemValueLevel(item);
        if ((int)level >= _config.MinQualityFilter3D)
        {
            filteredItems.Add(item);
        }
    }
    else
    {
        // 品质过滤为 0 时，显示所有物品
        filteredItems.Add(item);  // ✅ 修复点：明确添加所有物品
    }
}
```

**效果**：
- ✅ 品质过滤为 0（白色/普通）时，显示**所有物品**
- ✅ 品质过滤为 1-6 时，只显示符合品质的物品
- ✅ 未录入的钥匙始终显示，无视过滤等级

---

## 测试场景

### 场景 1: 宠物狗附近有箱子
**测试步骤**：
1. 宠物狗背包里放一些物品
2. 附近有一个箱子
3. 查看 ESP 显示

**预期结果**：
- ✅ 只显示箱子里的物品
- ❌ 不显示宠物狗背包的物品

---

### 场景 2: 箱子里有未录入的钥匙
**测试步骤**：
1. 箱子里有：未录入的钥匙 + 低品质物品
2. 3D ESP 品质过滤设为 4（橙色/传说）
3. 查看 ESP 显示

**预期结果**：
- ✅ 显示未录入的钥匙（标记为 `[🔑未录入]`）
- ❌ 不显示低品质物品（被过滤）
- ✅ 显示传说级及以上物品

---

### 场景 3: 品质过滤设为最低
**测试步骤**：
1. 3D ESP 品质过滤设为 0（白色/普通）
2. 箱子里有各种品质的物品
3. 查看 ESP 显示

**预期结果**：
- ✅ 显示箱子里的**所有物品**
- ✅ 包括白色、绿色、蓝色等所有品质

---

### 场景 4: 已录入的钥匙
**测试步骤**：
1. 箱子里有一把已经录入过的钥匙
2. 3D ESP 品质过滤设为 4（橙色/传说）
3. 查看 ESP 显示

**预期结果**：
- ❌ 不显示已录入的钥匙（按正常品质过滤）
- ✅ 只有未录入的钥匙会特殊显示

---

## 配置建议

### 刷钥匙模式
```
3D ESP最小品质: 橙色(传说)
地图标记最小品质: 橙色(传说)
```
**效果**：
- 只显示高价值物品 + 所有未录入的钥匙
- 高效率找钥匙

### 全物品模式
```
3D ESP最小品质: 白色(普通)
地图标记最小品质: 紫色(史诗)
```
**效果**：
- 3D ESP 显示所有物品（近距离查看细节）
- 小地图只标记高价值箱子（远距离导航）

### 高价值模式
```
3D ESP最小品质: 橙色(传说)
地图标记最小品质: 红色(至尊)
```
**效果**：
- 只关注最顶级物品
- 未录入的钥匙依然显示（特殊处理）

---

## 技术细节

### 钥匙检测流程
```
1. 检查物品标签
   └─ 是否包含 "Key" 标签？
       ├─ 是 → 继续检查
       └─ 否 → 不是钥匙

2. 检查录入状态
   └─ MasterKeysManager.IsActive(item.TypeID)
       ├─ true → 已录入（按正常过滤）
       └─ false → 未录入（特殊显示）

3. 显示逻辑
   └─ 未录入的钥匙
       ├─ 无视品质过滤
       ├─ 添加到 filteredItems
       └─ 显示为 [🔑未录入]
```

### 宠物背包检测
```
PetProxy.PetInventory
├─ 存在 → 获取宠物背包引用
│   └─ inventory == PetInventory?
│       ├─ 是 → 跳过，不读取物品
│       └─ 否 → 正常读取
└─ 不存在 → 正常读取（游戏可能没有宠物系统）
```

### 品质过滤逻辑
```
foreach (Item item in items)
{
    ┌─ 是未录入的钥匙？
    │   ├─ 是 → 直接添加到 filteredItems ✅
    │   └─ 否 → 继续检查
    │
    └─ 品质过滤 > 0？
        ├─ 是 → 检查品质 >= 过滤值
        │   ├─ 符合 → 添加到 filteredItems ✅
        │   └─ 不符合 → 跳过 ❌
        └─ 否（== 0）→ 添加所有物品 ✅
}
```

---

## 代码改动摘要

### 修改的方法
1. `GetItemsFromInventory()` - 添加宠物背包检测
2. `UpdateESPCache()` - 修复过滤逻辑 + 添加钥匙检测
3. `ScanAllLootboxes()` - 修复过滤逻辑 + 添加钥匙检测
4. `DrawESPBox()` - 添加钥匙特殊标记显示

### 新增的方法
1. `IsUnregisteredKey(Item item)` - 判断是否为未录入的钥匙

### 新增依赖
- `PetProxy` 类（来自游戏核心）
- `Duckov.MasterKeys.MasterKeysManager` 类（来自游戏核心）

---

## 总结

这次更新主要解决了三个问题：

1. ✅ **宠物狗物品干扰** → 通过 `PetProxy.PetInventory` 检测并排除
2. ✅ **钥匙查找困难** → 未录入的钥匙无视品质过滤，特殊显示
3. ✅ **品质过滤失效** → 修复了当过滤值为0时的逻辑错误

现在玩家可以：
- 专注于箱子中的实际物品（不被宠物背包干扰）
- 高效率找到需要录入的钥匙（无视品质过滤）
- 正常使用品质过滤功能（包括显示所有物品）

所有功能已测试，无编译错误！
