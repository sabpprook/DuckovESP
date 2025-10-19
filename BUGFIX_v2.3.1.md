# DuckovESP v2.3.1 Bug 修复

## 🐛 修复的问题

### 1. ✅ 建筑材料和任务物品绕过品质过滤
**问题描述：**
- 设置了"3D ESP最小品质"后，低品质的任务物品和建筑材料也被过滤掉了
- 例如：白色品质的"木板"是建筑材料，但设置"只显示紫色+"后就看不到了

**修复方案：**
```csharp
// 检查是否是任务物品或建筑材料
bool isQuestOrBuilding = false;
if (_questItemDetector != null)
{
    isQuestOrBuilding = (_config.HighlightQuestItems && _questItemDetector.IsQuestRequiredItem(item)) ||
                       (_config.HighlightBuildingMaterials && _questItemDetector.IsBuildingRequiredItem(item));
}

// 如果是任务/建筑材料，直接添加（绕过品质过滤）
if (isUnregisteredKey || isQuestOrBuilding)
{
    filteredItems.Add(item);
    continue;
}

// 否则才应用品质过滤
```

**修复效果：**
- ✅ 任务物品总是显示，无论品质
- ✅ 建筑材料总是显示，无论品质
- ✅ 钥匙总是显示，无论品质
- ✅ 普通物品应用品质过滤

### 2. ✅ 3D ESP 使用正确的设置项
**问题描述：**
- 代码中使用了 `_config.MinQualityFilter3D`（正确）
- 但可能有地方误用了 `_minQualityFilter`（小地图设置）

**检查结果：**
- ✅ 箱子物品扫描使用 `_config.MinQualityFilter3D`
- ✅ 地图物品扫描使用 `_config.MinQualityFilter3D`
- ✅ 小地图标记使用 `_minQualityFilter`（等价于 `_config.MinQualityForMapMarkers`）
- ✅ 设置项分离正确

## 📊 过滤逻辑总结

### 3D ESP 过滤优先级（从高到低）
1. **钥匙**（未录入）→ 总是显示
2. **任务物品** → 总是显示（绕过品质过滤）✨ 新增
3. **建筑材料** → 总是显示（绕过品质过滤）✨ 新增
4. **普通物品** → 应用品质过滤（`MinQualityFilter3D`）

### 小地图标记过滤优先级
1. **任务物品** → 总是显示，使用黄色
2. **建筑材料** → 总是显示，使用青色
3. **普通物品** → 应用品质过滤（`MinQualityForMapMarkers`）

## 🎯 使用示例

### 场景1：设置高品质过滤
```
配置：
- 3D ESP最小品质：紫色（4）
- 高亮任务物品：✅
- 高亮建筑材料：✅

任务：收集 5 个木板（白色品质）
建筑：需要 螺丝x20（绿色品质）

进入关卡后：
✅ 木板（白色）仍然显示 - [任务物品]
✅ 螺丝（绿色）仍然显示 - [建筑材料]
❌ 其他白色/绿色/蓝色物品被过滤
✅ 紫色及以上物品正常显示
```

### 场景2：完成任务后
```
提交任务："收集木板" → 完成

2 秒后：
✅ 木板不再是任务物品
❌ 如果是白色品质，被品质过滤掉
✅ 如果还是建筑材料，继续显示
```

## 🔧 修改的代码

### ModBehaviour.cs

**位置1：ScanLootboxItems() 方法（第 318-354 行）**
```csharp
// 修改前
bool isUnregisteredKey = IsUnregisteredKey(item);
if (isUnregisteredKey) {
    filteredItems.Add(item);
    continue;
}

// 修改后
bool isUnregisteredKey = IsUnregisteredKey(item);

bool isQuestOrBuilding = false;
if (_questItemDetector != null)
{
    isQuestOrBuilding = (_config.HighlightQuestItems && _questItemDetector.IsQuestRequiredItem(item)) ||
                       (_config.HighlightBuildingMaterials && _questItemDetector.IsBuildingRequiredItem(item));
}

if (isUnregisteredKey || isQuestOrBuilding) {
    filteredItems.Add(item);
    continue;
}
```

**位置2：ScanWorldItems() 方法（第 416-430 行）**
```csharp
// 修改前
bool isUnregisteredKey = IsUnregisteredKey(item);
if (!isUnregisteredKey && (int)itemLevel < _config.MinQualityFilter3D)
    continue;

// 修改后
bool isUnregisteredKey = IsUnregisteredKey(item);

bool isQuestOrBuilding = false;
if (_questItemDetector != null)
{
    isQuestOrBuilding = (_config.HighlightQuestItems && _questItemDetector.IsQuestRequiredItem(item)) ||
                       (_config.HighlightBuildingMaterials && _questItemDetector.IsBuildingRequiredItem(item));
}

if (!isUnregisteredKey && !isQuestOrBuilding && (int)itemLevel < _config.MinQualityFilter3D)
    continue;
```

## ✅ 测试检查

### 编译检查
```
✅ No errors found.
```

### 功能测试
测试步骤：
1. 接取一个需要白色物品的任务（如木板）
2. 设置"3D ESP最小品质"为"紫色"
3. 进入关卡
4. 验证白色木板仍然显示 [任务物品] 标签
5. 验证其他白色物品被过滤

预期结果：
- [x] 任务物品显示，无论品质
- [x] 建筑材料显示，无论品质
- [x] 钥匙显示
- [x] 低品质普通物品被过滤

## 📅 更新日期
2025-10-19

## 📝 升级说明

从 v2.3 升级到 v2.3.1：
- ✅ 无需修改配置
- ✅ 自动生效
- ✅ 向后兼容

---

**修复完成！现在任务物品和建筑材料不会被品质过滤器过滤了！** 🎉
