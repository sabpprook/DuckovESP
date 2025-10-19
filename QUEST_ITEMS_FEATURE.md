# 任务物品与建筑材料标记功能

## 功能概述
自动检测并标记当前任务所需的物品和未建造建筑所需的材料，让玩家在ESP中一眼看到哪些物品是真正需要的。

**✨ v2.2 新增功能：**
- 支持建筑物升级材料检测（包括锻炼身体、生存技能、建造台升级、技能强化等所有建筑类型）
- 支持扫描地图上直接生成的物品（不在箱子里的ItemAgent）

## 新增文件
- **QuestItemDetector.cs** - 核心检测器类，负责扫描任务和建筑需求

## 工作原理

### 1. 任务物品检测
```csharp
// 使用游戏API获取所有活跃任务需要的物品
IEnumerable<int> requiredItems = QuestManager.GetAllRequiredItems();
```

**检测逻辑：**
- 遍历所有活跃任务（`QuestManager.Instance.ActiveQuests`）
- 查找类型为 `SubmitItems` 的任务
- 提取未完成任务的 `ItemTypeID`
- 每2秒自动更新一次列表

### 2. 建筑材料检测
```csharp
// 获取所有已解锁但未建造的建筑
var unlockedBuildings = GetUnlockedUnbuiltBuildings();

// 提取每个建筑的材料需求
BuildingInfo buildingInfo = BuildingManager.GetBuildingInfo(buildingID);
if (buildingInfo.cost.items != null)
{
    foreach (var itemEntry in buildingInfo.cost.items)
    {
        _buildingRequiredItems.Add(itemEntry.id);
    }
}
```

**检测逻辑：**
- 从 `GameplayDataSettings.BuildingDataCollection` 获取所有建筑
- 检查建筑是否已解锁（`IsBuildingUnlocked`）
- 检查建筑是否已建造（`BuildingManager.Any(id)`）
- 提取未建造建筑的所有材料（`buildingInfo.cost.items`）
- 每2秒自动更新一次列表

**✨ 支持的建筑类型：**
- 🏋️ **身体锻炼** - 提升角色体能的训练设施
- 🎯 **生存技能** - 增强生存能力的设施
- 🔨 **建造台升级** - 解锁更高级建造配方
- ⚡ **技能强化** - 提升战斗技能的设施
- 🏠 **其他所有建筑** - 所有 BuildingDataCollection 中的建筑

### 3. 地图物品扫描
```csharp
// 扫描场景中所有 ItemAgent（pickup类型）
DuckovItemAgent[] allItemAgents = FindObjectsOfType<DuckovItemAgent>();

foreach (DuckovItemAgent itemAgent in allItemAgents)
{
    // 只扫描pickup类型（地图上直接生成的物品）
    if (itemAgent.AgentType != ItemAgent.AgentTypes.pickUp)
        continue;
    
    // 获取物品并应用品质过滤
    Item item = itemAgent.Item;
    // ... 添加到ESP显示
}
```

**扫描范围：**
- 📦 **箱子中的物品** - 通过 InteractableLootbox 扫描
- 🌍 **地图上的物品** - 通过 ItemAgent.pickUp 类型扫描（不在箱子里的物品）
- 💰 **掉落物品** - 敌人死亡掉落、玩家丢弃的物品等

**扫描特点：**
- 每 0.2 秒更新一次ESP缓存
- 自动过滤超出距离的物品
- 应用品质过滤（可配置）
- 支持任务/建筑材料高亮

### 4. ESP标签增强
在物品ESP标签上添加特殊标记：
```
[白色] 镇痛药 x2
[绿色][任务物品] 抗生素 x1
[蓝色][建筑材料] 木板 x5
[紫色][任务+建筑] 钢板 x3
```

## 配置选项

### ESPConfig.cs 新增字段
```csharp
public bool HighlightQuestItems = true;           // 高亮任务物品
public bool HighlightBuildingMaterials = true;    // 高亮建筑材料
public Color QuestItemColor = Color.yellow;       // 任务物品颜色（黄色）
public Color BuildingMaterialColor = Color.cyan;  // 建筑材料颜色（青色）
```

### 配置菜单
在 F5 配置菜单中新增 "📦 任务物品&建筑材料" 部分：
- ✅ 高亮任务物品
- ✅ 高亮建筑材料
- 说明文字提示功能用途

## 使用方法

### 1. 启用功能
1. 按 `F5` 打开配置菜单
2. 找到 "📦 任务物品&建筑材料" 部分
3. 勾选 "高亮任务物品" 和/或 "高亮建筑材料"
4. 点击"保存配置"

### 2. 游戏中使用
1. 接取任务后，进入关卡
2. 任务需要的物品会在ESP标签上显示 `[任务物品]`
3. 建筑材料会显示 `[建筑材料]`
4. 同时需要的会显示 `[任务+建筑]`

### 3. 查看统计
调试日志会显示：
```
DuckovESP: 检测到 3 个任务物品, 5 个建筑材料
```

## 性能优化

### 1. 更新频率控制
- 每 **2秒** 更新一次任务和建筑需求列表
- 避免每帧查询游戏API
- 使用 `HashSet<int>` 快速查找

### 2. 缓存机制
```csharp
private HashSet<int> _questRequiredItems = new HashSet<int>();
private HashSet<int> _buildingRequiredItems = new HashSet<int>();
private float _lastUpdateTime = 0f;
```

### 3. 条件更新
只有在功能启用时才更新：
```csharp
if (_config.HighlightQuestItems || _config.HighlightBuildingMaterials)
{
    _questItemDetector?.Update();
}
```

## 技术细节

### 检测到的任务类型
- **SubmitItems** - 提交物品任务（最常见）
- 未来可扩展其他任务类型

### 建筑解锁判断
```csharp
private bool IsBuildingUnlocked(BuildingInfo info)
{
    // 无解锁条件 = 默认解锁
    if (info.unlockConditions == null || info.unlockConditions.Length == 0)
        return true;
    
    // 有解锁条件 = 假设已解锁（保守策略）
    // 实际游戏中可通过 condition.Check() 精确判断
    return true;
}
```

**注意：** 目前采用保守策略，可能会显示一些尚未解锁的建筑材料。未来可以通过访问解锁条件的 `Check()` 方法来精确判断。

### 依赖的游戏API
| API | 用途 | 来源 |
|-----|------|------|
| `QuestManager.GetAllRequiredItems()` | 获取任务物品 | Duckov.Quests |
| `BuildingManager.GetBuildingInfo()` | 获取建筑信息 | Duckov.Buildings |
| `BuildingManager.Any()` | 检查建筑是否存在 | Duckov.Buildings |
| `GameplayDataSettings.BuildingDataCollection` | 建筑数据集合 | Duckov.Utilities |

## 代码集成点

### ModBehaviour.cs
```csharp
// 1. 声明检测器
private QuestItemDetector _questItemDetector;

// 2. 初始化（Awake）
_questItemDetector = new QuestItemDetector();

// 3. 更新（Update）
if (_config.HighlightQuestItems || _config.HighlightBuildingMaterials)
{
    _questItemDetector?.Update();
}

// 4. 绘制标签（DrawESPBox）
string requirementTag = _questItemDetector?.GetItemRequirementType(item) ?? "";
sb.AppendLine($"{qualityTag}{requirementTag} {itemName}");
```

## 未来改进

### 1. 精确的解锁检测
```csharp
// TODO: 实现精确的建筑解锁检测
foreach (var condition in info.unlockConditions)
{
    if (!condition.Check())
        return false;  // 未解锁
}
```

### 2. 更多任务类型支持
- `QuestTask_KillCount` - 击杀任务相关物品（如特定武器）
- `QuestTask_ConstructBuilding` - 建造任务
- `QuestTask_UseItem` - 使用物品任务

### 3. 优先级排序
按需求紧急程度排序：
- 🔴 急需（数量不足）
- 🟡 需要（数量足够）
- 🟢 充足（超过需求）

### 4. 颜色自定义
在配置菜单中添加颜色选择器：
```csharp
_config.QuestItemColor = ColorField("任务物品颜色", _config.QuestItemColor);
_config.BuildingMaterialColor = ColorField("建筑材料颜色", _config.BuildingMaterialColor);
```

### 5. 数量提示
显示还需要多少个：
```
[任务物品](还需3个) 抗生素 x1
[建筑材料](还需10个) 木板 x5
```

## 测试建议

### 1. 任务物品测试
1. 接取一个需要提交物品的任务
2. 进入关卡
3. 查看任务所需物品是否被标记为 `[任务物品]`
4. 完成任务后，标记应该消失

### 2. 建筑材料测试
1. 解锁一个新建筑但不建造
2. 查看该建筑所需材料是否被标记为 `[建筑材料]`
3. 建造完成后，标记应该消失

### 3. 双重需求测试
1. 找一个既是任务物品又是建筑材料的物品（如木板）
2. 验证是否显示 `[任务+建筑]`

### 4. 性能测试
1. 观察FPS是否有明显下降
2. 检查日志中的更新频率（应该是每2秒一次）

## 常见问题

### Q1: 为什么有些建筑材料没有被标记？
**A:** 可能是该建筑尚未解锁，或者已经建造。检查游戏中的建筑菜单确认状态。

### Q2: 任务完成后标记还在？
**A:** 检测器每2秒更新一次，等待下一次更新周期即可。如果持续存在，可能是任务系统状态未正确更新。

### Q3: 标记太多导致界面混乱？
**A:** 可以在配置菜单中关闭 "高亮建筑材料"，只保留 "高亮任务物品"。

### Q4: 性能影响大吗？
**A:** 非常小。使用 HashSet 查找复杂度为 O(1)，每2秒才更新一次列表，对性能几乎没有影响。

### Q5: 地图上的物品会被标记吗？
**A:** ✅ 会！v2.2 版本新增了地图物品扫描功能，所有通过 ItemAgent（pickup类型）生成的物品都会被扫描，包括：
- 地图上直接刷新的物品
- 敌人死亡掉落的物品
- 玩家丢弃的物品
- 爆炸或破坏产生的掉落物

### Q6: 所有建筑类型都支持吗？
**A:** ✅ 支持！包括但不限于：
- 身体锻炼设施（体能提升）
- 生存技能设施（技能训练）
- 建造台升级（工作台等级）
- 技能强化设施（战斗技能）
- 以及 BuildingDataCollection 中的所有其他建筑类型

## 版本历史

### v2.2.0 (2025-10-19) - 建筑升级支持 + 地图物品扫描
- ✅ 修复建筑材料检测（`cost.items` 而非 `costs` 数组）
- ✅ 支持所有建筑类型（身体锻炼、生存技能、建造台升级、技能强化等）
- ✅ 添加地图物品扫描功能（扫描 ItemAgent.pickUp 类型）
- ✅ 自动检测地图上直接生成的物品（不在箱子里）
- ✅ 统一的品质过滤应用于箱子和地图物品

### v2.1.0 (2025-10-19) - 初版实现
- ✅ 实现任务物品检测
- ✅ 实现建筑材料检测
- ✅ 添加ESP标签标记
- ✅ 添加配置选项
- ✅ 性能优化（2秒更新间隔）

## 技术细节更新

### BuildingInfo.cost 结构
```csharp
public struct BuildingInfo
{
    public Cost cost;  // 单个 Cost 对象，不是数组！
}

public struct Cost
{
    public long money;              // 金钱花费
    public Cost.ItemEntry[] items;  // 物品花费数组
}

public struct ItemEntry
{
    public int id;      // 物品TypeID
    public long amount; // 需要数量
}
```

### ItemAgent 类型判断
```csharp
// 地图上的物品
if (itemAgent.AgentType == ItemAgent.AgentTypes.pickUp)
{
    // 这是地图上可以拾取的物品
}

// 手持的物品
if (itemAgent.AgentType == ItemAgent.AgentTypes.handheld)
{
    // 这是角色手上拿着的物品
}
```

### ESP扫描流程
```
Update() [每帧]
  └─> UpdateESPCache() [每 0.2 秒]
       ├─> ScanLootboxItems(playerPos)
       │    └─> 遍历 _trackedLootboxes
       │         └─> 应用品质过滤 + 钥匙逻辑
       └─> ScanWorldItems(playerPos)
            └─> FindObjectsOfType<DuckovItemAgent>()
                 └─> 过滤 AgentType == pickUp
                      └─> 应用品质过滤 + 距离检查
```

## 性能优化

### 双层缓存机制
1. **任务/建筑列表缓存** - 每 2 秒更新（QuestItemDetector）
   - `_questRequiredItems: HashSet<int>`
   - `_buildingRequiredItems: HashSet<int>`
   - O(1) 查询复杂度

2. **ESP显示缓存** - 每 0.2 秒更新（ModBehaviour）
   - `_espDataCache: List<ESPData>`
   - 减少 OnGUI 中的计算量
   - 避免重复的世界坐标转换

### 扫描优化
- **箱子扫描**：使用已追踪列表，避免重复 FindObjectsOfType
- **地图物品扫描**：每 0.2 秒扫描一次，而非每帧
- **距离剔除**：超出 `_maxESPDistance` 的物品不处理
- **视锥剔除**：在摄像机后面的物品不显示

## 版本历史

### v2.1.0 (2025-10-19)
- ✅ 实现任务物品检测
- ✅ 实现建筑材料检测
- ✅ 添加ESP标签标记
- ✅ 添加配置选项
- ✅ 性能优化（2秒更新间隔）

## 参考资料
- 游戏源码：`source_code_reference/TeamSoda.Duckov.Core/Duckov/Quests/`
- 游戏源码：`source_code_reference/TeamSoda.Duckov.Core/Duckov/Buildings/`
- QuestManager API：`QuestManager.GetAllRequiredItems()`
- BuildingManager API：`BuildingManager.GetBuildingInfo()`
