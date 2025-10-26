# 敌人 ESP 系统实现计划

## 📋 计划概述

基于游戏源码分析和 v2 实现经验，设计一个高性能、事件驱动的敌人 ESP 数据采集系统。

---

## 🔍 游戏源码分析

### 1. 核心类结构

#### `CharacterMainControl` - 角色控制器
**位置**：`game_source_code/TeamSoda.Duckov.Core/CharacterMainControl.cs`

**关键属性**：
```csharp
public class CharacterMainControl : MonoBehaviour
{
    public static CharacterMainControl Main { get; }  // 主角引用
    public Teams Team { get; }                         // 队伍归属
    public Item CharacterItem { get; }                 // 角色物品（包含Inventory）
    public Health Health { get; }                      // 生命值组件
    public DuckovItemAgent CurrentHoldItemAgent { get; } // 当前手持武器
    public bool Hidden { get; }                        // 是否隐身
    public bool IsMainCharacter { get; }               // 是否主角
    public CharacterRandomPreset characterPreset { get; } // 角色预设
    
    // 事件
    public event Action<CharacterMainControl, Vector3> OnSetPositionEvent;
    public event Action<DamageInfo> BeforeCharacterSpawnLootOnDead;
}
```

**用途**：
- 判断敌我关系（`Team.IsEnemy(player.Team, enemy.Team)`）
- 获取位置、武器、装备信息
- 检测角色状态

#### `Health` - 生命值组件
**位置**：`game_source_code/TeamSoda.Duckov.Core/Health.cs`

**关键属性和事件**：
```csharp
public class Health : MonoBehaviour
{
    public float CurrentHealth { get; set; }     // 当前HP
    public float MaxHealth { get; }              // 最大HP
    public bool IsDead { get; }                  // 是否死亡
    public bool Invincible { get; }              // 无敌状态
    
    // 全局静态事件（关键）
    public static event Action<Health, DamageInfo> OnHurt;  // 受伤事件
    public static event Action<Health, DamageInfo> OnDead;  // 死亡事件
    
    // 实例事件
    public UnityEvent<DamageInfo> OnHurtEvent;
    public UnityEvent<DamageInfo> OnDeadEvent;
    public UnityEvent<Health> OnHealthChange;
    public UnityEvent<Health> OnMaxHealthChange;
    
    // 方法
    public CharacterMainControl TryGetCharacter();  // 获取关联的角色
    public bool Hurt(DamageInfo damageInfo);        // 造成伤害
}
```

**用途**：
- 实时血量监控
- 死亡事件驱动（移除已死亡敌人）
- 伤害事件驱动（检测威胁）

#### `Teams` - 队伍枚举
**位置**：`game_source_code/TeamSoda.Duckov.Core/Teams.cs`

```csharp
public enum Teams
{
    player,    // 玩家
    enemy1,    // 敌对1
    enemy2,    // 敌对2
    enemy3,    // 敌对3
    neutral    // 中立
}
```

**判断敌我**：
```csharp
// 静态方法（推测）
Team.IsEnemy(Teams team1, Teams team2);
```

### 2. 关键事件系统

#### Health.OnDead（死亡事件）
- **类型**：`Action<Health, DamageInfo>`
- **触发时机**：角色死亡时（CurrentHealth <= 0）
- **用途**：移除已死亡敌人，避免无效检测

#### Health.OnHurt（受伤事件）
- **类型**：`Action<Health, DamageInfo>`
- **触发时机**：角色受到伤害时
- **用途**：
  - 检测敌人受到玩家攻击（威胁评估）
  - 监控战斗状态

#### LevelManager.OnAfterLevelInitialized
- **触发时机**：关卡完全初始化完成
- **用途**：初始化时机，执行初始扫描

---

## 🏗️ 架构设计

### 1. 分层结构

```
Core/Systems/Enemy/
├── Detection/
│   ├── IEnemyCollectionService.cs       // 接口定义
│   ├── EnemyCollectionService.cs        // 数据采集服务
│   └── EnemyMonitor.cs                  // 事件监听器
├── Models/
│   └── EnemyData.cs                     // 敌人数据模型
├── Events/
│   ├── EnemyDiscoveredEvent.cs          // 敌人发现事件
│   ├── EnemyRemovedEvent.cs             // 敌人移除事件
│   └── EnemyHealthChangedEvent.cs       // 血量变化事件
└── Rendering/
    └── EnemyESPRenderer.cs              // 渲染器（后续实现）
```

### 2. 核心组件

#### IEnemyCollectionService（接口）
```csharp
public interface IEnemyCollectionService
{
    // 获取所有存活敌人
    IReadOnlyList<EnemyData> GetAllEnemies();
    
    // 按距离过滤敌人
    IReadOnlyList<EnemyData> GetEnemiesInRange(float maxDistance);
    
    // 获取威胁等级最高的敌人
    IReadOnlyList<EnemyData> GetTopThreats(int count);
    
    // 获取高价值目标（携带贵重物品）
    IReadOnlyList<EnemyData> GetHighValueTargets();
    
    // 事件
    event Action<EnemyData> OnEnemyDiscovered;
    event Action<EnemyData> OnEnemyRemoved;
    event Action<EnemyData> OnEnemyHealthChanged;
    
    // 生命周期
    void Initialize();
    void Update(float deltaTime);
    void Cleanup();
}
```

#### EnemyData（数据模型）
```csharp
public class EnemyData
{
    // ===== 基础信息 =====
    public CharacterMainControl Character { get; set; }     // 角色引用
    public Health HealthComponent { get; set; }             // 生命组件
    public string Name { get; set; }                        // 角色名称
    public Teams Team { get; set; }                         // 队伍
    
    // ===== 位置信息 =====
    public Vector3 Position { get; set; }                   // 身体位置
    public Vector3 HeadPosition { get; set; }               // 头部位置
    public float DistanceToPlayer { get; set; }             // 距离
    
    // ===== 生命信息 =====
    public float CurrentHealth { get; set; }                // 当前HP
    public float MaxHealth { get; set; }                    // 最大HP
    public float HealthPercent { get; set; }                // 血量百分比
    public bool IsAlive { get; set; }                       // 存活状态
    
    // ===== 装备信息 =====
    public string WeaponName { get; set; }                  // 武器名称
    public Item WeaponItem { get; set; }                    // 武器物品
    public Item HelmetItem { get; set; }                    // 头盔
    public Item ArmorItem { get; set; }                     // 护甲
    
    // ===== 价值信息 =====
    public long InventoryValue { get; set; }                // 库存价值
    public bool IsHighValueTarget { get; set; }             // 高价值目标
    public bool IsTrader { get; set; }                      // 商人
    public bool IsBoss { get; set; }                        // Boss
    
    // ===== 威胁信息 =====
    public bool IsAimingAtPlayer { get; set; }              // 是否瞄准玩家
    public float ThreatLevel { get; set; }                  // 威胁等级
    public bool IsInCombat { get; set; }                    // 是否在战斗
    
    // ===== 时间戳 =====
    public DateTime DiscoveryTime { get; set; }             // 发现时间
    public DateTime LastUpdateTime { get; set; }            // 最后更新
    public DateTime LastSeenTime { get; set; }              // 最后可见时间
    
    // ===== 辅助方法 =====
    public bool IsValid() => Character != null && HealthComponent != null;
    public bool IsInRange(float maxDistance) => DistanceToPlayer <= maxDistance;
}
```

#### EnemyMonitor（事件监听器）
```csharp
public class EnemyMonitor
{
    // 事件
    public event Action<CharacterMainControl>? OnEnemySpawned;
    public event Action<CharacterMainControl>? OnEnemyDied;
    public event Action<CharacterMainControl, float>? OnEnemyHealthChanged;
    
    // 追踪数据
    private HashSet<CharacterMainControl> _trackedEnemies;
    private Dictionary<CharacterMainControl, float> _lastHealthValues;
    
    // 订阅全局事件
    public void StartMonitoring()
    {
        Health.OnDead += OnHealthDead;
        Health.OnHurt += OnHealthHurt;
        LevelManager.OnAfterLevelInitialized += OnLevelInitialized;
    }
    
    // 停止监听
    public void StopMonitoring()
    {
        Health.OnDead -= OnHealthDead;
        Health.OnHurt -= OnHealthHurt;
        LevelManager.OnAfterLevelInitialized -= OnLevelInitialized;
    }
    
    private void OnHealthDead(Health health, DamageInfo damageInfo)
    {
        var character = health.TryGetCharacter();
        if (character != null && _trackedEnemies.Contains(character))
        {
            OnEnemyDied?.Invoke(character);
        }
    }
    
    private void OnHealthHurt(Health health, DamageInfo damageInfo)
    {
        var character = health.TryGetCharacter();
        if (character != null && IsEnemy(character))
        {
            float newHealth = health.CurrentHealth;
            if (_lastHealthValues.TryGetValue(character, out float oldHealth))
            {
                if (Math.Abs(newHealth - oldHealth) > 0.1f)
                {
                    OnEnemyHealthChanged?.Invoke(character, newHealth);
                    _lastHealthValues[character] = newHealth;
                }
            }
        }
    }
}
```

#### EnemyCollectionService（采集服务）
```csharp
public class EnemyCollectionService : IEnemyCollectionService
{
    // 依赖
    private readonly ILogger _logger;
    private readonly IEventBus _eventBus;
    private readonly EnemyMonitor _monitor;
    
    // 数据缓存
    private Dictionary<CharacterMainControl, EnemyData> _enemies;
    private List<CharacterMainControl> _allCharacters;  // 初始扫描结果
    
    // 更新计时
    private float _lastUpdateTime;
    private const float UPDATE_INTERVAL = 0.3f;  // 300ms更新一次
    
    public void Initialize()
    {
        // 1. 初始化监听器
        _monitor = new EnemyMonitor(_logger);
        _monitor.OnEnemySpawned += HandleEnemySpawned;
        _monitor.OnEnemyDied += HandleEnemyDied;
        _monitor.OnEnemyHealthChanged += HandleEnemyHealthChanged;
        _monitor.StartMonitoring();
        
        // 2. 初始扫描（仅一次FindObjectsOfType）
        DiscoverExistingEnemies();
        
        _logger.Info("[EnemyCollectionService] 初始化完成");
    }
    
    private void DiscoverExistingEnemies()
    {
        var allCharacters = Object.FindObjectsOfType<CharacterMainControl>();
        _allCharacters = new List<CharacterMainControl>(allCharacters);
        
        var player = CharacterMainControl.Main;
        if (player == null) return;
        
        foreach (var character in allCharacters)
        {
            if (IsEnemy(player, character) && !character.Health.IsDead)
            {
                var enemyData = CreateEnemyData(character, player);
                _enemies[character] = enemyData;
                
                // 触发发现事件
                OnEnemyDiscovered?.Invoke(enemyData);
                _eventBus.Publish(new EnemyDiscoveredEvent(enemyData));
            }
        }
    }
    
    public void Update(float deltaTime)
    {
        _lastUpdateTime += deltaTime;
        if (_lastUpdateTime < UPDATE_INTERVAL) return;
        _lastUpdateTime = 0f;
        
        var player = CharacterMainControl.Main;
        if (player == null) return;
        
        // 更新所有敌人数据
        foreach (var kvp in _enemies.ToList())
        {
            var character = kvp.Key;
            var enemyData = kvp.Value;
            
            if (character == null || character.Health.IsDead)
            {
                RemoveEnemy(character);
                continue;
            }
            
            // 更新数据
            UpdateEnemyData(enemyData, player);
        }
    }
    
    private EnemyData CreateEnemyData(CharacterMainControl character, CharacterMainControl player)
    {
        var data = new EnemyData
        {
            Character = character,
            HealthComponent = character.Health,
            Name = GetEnemyName(character),
            Team = character.Team,
            Position = character.transform.position,
            HeadPosition = GetHeadPosition(character),
            CurrentHealth = character.Health.CurrentHealth,
            MaxHealth = character.Health.MaxHealth,
            HealthPercent = character.Health.CurrentHealth / character.Health.MaxHealth,
            IsAlive = !character.Health.IsDead,
            WeaponName = GetWeaponName(character),
            InventoryValue = CalculateInventoryValue(character),
            DiscoveryTime = DateTime.Now,
            LastUpdateTime = DateTime.Now
        };
        
        // 计算距离
        data.DistanceToPlayer = Vector3.Distance(player.transform.position, data.Position);
        
        // 判断特殊类型
        data.IsHighValueTarget = data.InventoryValue > 50000;
        data.IsBoss = IsBoss(character);
        data.IsTrader = IsTrader(character);
        
        return data;
    }
}
```

---

## 🔄 事件驱动流程

### 1. 初始化阶段
```
1. ModBehaviour.OnLevelInitialized
   ↓
2. EnemyCollectionService.Initialize()
   ↓
3. EnemyMonitor.StartMonitoring()
   - 订阅 Health.OnDead
   - 订阅 Health.OnHurt
   ↓
4. DiscoverExistingEnemies()
   - FindObjectsOfType<CharacterMainControl>() (仅一次)
   - 过滤敌人
   - 创建 EnemyData
   - 触发 EnemyDiscoveredEvent
```

### 2. 运行时更新
```
Update Loop (300ms间隔):
   ↓
1. 遍历 _enemies 字典
   ↓
2. 检查存活状态
   - 死亡 → RemoveEnemy → EnemyRemovedEvent
   ↓
3. 更新数据
   - 位置、距离
   - 血量、状态
   - 威胁评估
```

### 3. 事件响应
```
Health.OnDead 触发
   ↓
EnemyMonitor.OnHealthDead
   ↓
HandleEnemyDied
   ↓
RemoveEnemy
   ↓
EnemyRemovedEvent 发布
```

---

## 🎯 核心功能

### 1. 敌人检测
- [x] **初始扫描**：关卡加载时一次性 FindObjectsOfType
- [x] **敌我判断**：`Team.IsEnemy(player.Team, enemy.Team)`
- [x] **死亡检测**：订阅 `Health.OnDead` 事件
- [x] **动态追踪**：字典缓存，事件驱动更新

### 2. 数据采集
- [x] **基础信息**：名称、队伍、位置
- [x] **生命信息**：HP、血量百分比
- [x] **装备信息**：武器、头盔、护甲
- [x] **价值评估**：库存价值计算
- [x] **威胁评估**：瞄准检测、战斗状态

### 3. 特殊识别
- [x] **Boss 识别**：通过名称或特定标记
- [x] **商人识别**：中立队伍 + 特定标记
- [x] **高价值目标**：库存价值阈值

### 4. 性能优化
- [x] **事件驱动**：避免轮询，零开销检测
- [x] **初始扫描**：仅一次 FindObjectsOfType
- [x] **增量更新**：300ms 间隔，只更新存活敌人
- [x] **缓存管理**：字典查找 O(1)，死亡即移除

---

## 📊 数据结构设计

### 1. 主缓存
```csharp
private Dictionary<CharacterMainControl, EnemyData> _enemies;
// Key: Character 引用
// Value: 完整的敌人数据
// 优点：O(1) 查找，便于更新和移除
```

### 2. 辅助索引（可选）
```csharp
// 按距离排序（用于范围查询）
private SortedDictionary<float, List<EnemyData>> _enemiesByDistance;

// 高价值目标快速访问
private HashSet<CharacterMainControl> _highValueTargets;

// Boss 列表
private List<EnemyData> _bosses;
```

---

## 🔧 辅助方法

### 1. 敌我判断
```csharp
private bool IsEnemy(CharacterMainControl player, CharacterMainControl other)
{
    if (player == null || other == null) return false;
    if (player == other) return false;  // 不是自己
    if (other.Health.IsDead) return false;  // 已死亡
    
    return Team.IsEnemy(player.Team, other.Team);
}
```

### 2. 位置获取
```csharp
private Vector3 GetHeadPosition(CharacterMainControl character)
{
    // 方法1: mainDamageReceiver（头部受伤点）
    if (character.mainDamageReceiver != null)
    {
        return character.mainDamageReceiver.transform.position;
    }
    
    // 方法2: 估算（身体位置 + 1.7m）
    return character.transform.position + Vector3.up * 1.7f;
}
```

### 3. 武器名称
```csharp
private string GetWeaponName(CharacterMainControl character)
{
    var agent = character.CurrentHoldItemAgent;
    if (agent == null) return "徒手";
    
    var item = agent.Item;
    if (item == null) return "未知";
    
    return item.DisplayName;
}
```

### 4. 库存价值
```csharp
private long CalculateInventoryValue(CharacterMainControl character)
{
    long totalValue = 0;
    
    var characterItem = character.CharacterItem;
    if (characterItem == null || characterItem.Inventory == null)
        return 0;
    
    foreach (var item in characterItem.Inventory.Content)
    {
        if (item != null)
        {
            totalValue += item.Value * item.StackCount;
        }
    }
    
    return totalValue;
}
```

### 5. Boss 识别
```csharp
private bool IsBoss(CharacterMainControl character)
{
    // 方法1: 名称检测
    string name = character.name.ToLower();
    if (name.Contains("boss") || name.Contains("老板") || 
        name.Contains("巴布"))
    {
        return true;
    }
    
    // 方法2: 特定预设
    var preset = character.characterPreset;
    if (preset != null && preset.name.Contains("Boss"))
    {
        return true;
    }
    
    // 方法3: 血量阈值（Boss 通常血量很高）
    if (character.Health.MaxHealth > 500)
    {
        return true;
    }
    
    return false;
}
```

---

## ⚠️ 注意事项

### 1. 性能考虑
- ✅ **避免每帧 FindObjectsOfType**：仅初始化时调用一次
- ✅ **事件驱动**：使用 Health.OnDead 自动移除
- ✅ **增量更新**：300ms 间隔，只更新必要数据
- ⚠️ **库存计算**：首次计算后缓存，避免重复计算

### 2. 空引用检查
```csharp
// 所有 Unity 对象访问前都要检查
if (character != null && character.Health != null && !character.Health.IsDead)
{
    // 安全访问
}
```

### 3. 事件订阅管理
```csharp
// OnDisable 和 OnDestroy 中必须取消订阅
public void Cleanup()
{
    _monitor?.StopMonitoring();
    Health.OnDead -= OnHealthDead;
    Health.OnHurt -= OnHealthHurt;
}
```

### 4. 线程安全
- Unity 主线程：所有 Unity 对象访问必须在主线程
- 事件触发：Health.OnDead 在主线程触发，无需同步

---

## 📅 实施步骤

### Phase 1: 核心数据模型（1小时）
- [ ] 创建 `EnemyData.cs` 数据模型
- [ ] 创建 `IEnemyCollectionService.cs` 接口
- [ ] 创建事件类：
  - [ ] `EnemyDiscoveredEvent.cs`
  - [ ] `EnemyRemovedEvent.cs`
  - [ ] `EnemyHealthChangedEvent.cs`

### Phase 2: 事件监听器（1.5小时）
- [ ] 创建 `EnemyMonitor.cs`
- [ ] 订阅 `Health.OnDead` 事件
- [ ] 订阅 `Health.OnHurt` 事件
- [ ] 实现事件过滤和转发

### Phase 3: 数据采集服务（2小时）
- [ ] 创建 `EnemyCollectionService.cs`
- [ ] 实现初始化扫描
- [ ] 实现增量更新逻辑
- [ ] 实现数据创建和更新方法

### Phase 4: 辅助功能（1小时）
- [ ] 实现敌我判断
- [ ] 实现位置获取
- [ ] 实现库存价值计算
- [ ] 实现 Boss/商人识别

### Phase 5: 集成测试（1小时）
- [ ] 集成到 ModBehaviour
- [ ] 测试初始扫描
- [ ] 测试死亡事件
- [ ] 测试数据更新
- [ ] 性能测试

### Phase 6: 文档和优化（0.5小时）
- [ ] 编写实现文档
- [ ] 性能分析和优化
- [ ] 代码 Review

**总预估时间**：7 小时

---

## 🎓 学习要点

1. **Unity 事件系统**：静态事件的订阅和取消
2. **性能优化**：事件驱动 vs 轮询
3. **数据缓存**：字典查找和增量更新
4. **游戏逻辑**：队伍系统、生命系统
5. **威胁评估**：距离、血量、装备、瞄准

---

## 📚 相关游戏源码

### CharacterMainControl.cs
- `public static CharacterMainControl Main`：主角引用
- `public Teams Team`：队伍归属
- `public Health Health`：生命组件
- `public DuckovItemAgent CurrentHoldItemAgent`：当前武器

### Health.cs
- `public static event Action<Health, DamageInfo> OnDead`：死亡事件
- `public static event Action<Health, DamageInfo> OnHurt`：受伤事件
- `public float CurrentHealth / MaxHealth`：血量
- `public CharacterMainControl TryGetCharacter()`：获取角色

### Team.cs (推测)
- `public static bool IsEnemy(Teams team1, Teams team2)`：判断敌我

---

## 🔗 与物品 ESP 的对比

| 特性 | 物品 ESP | 敌人 ESP |
|------|---------|---------|
| **检测方式** | Inventory 字典 + Item.onParentChanged | Health.OnDead + 初始扫描 |
| **更新频率** | 事件驱动（实时） | 300ms 增量更新 |
| **数据复杂度** | 简单（名称、品质、位置） | 复杂（HP、装备、价值、威胁） |
| **性能开销** | 极低（纯事件） | 低（事件 + 定时更新） |
| **主要挑战** | Inventory 变化检测 | 威胁评估、瞄准检测 |

---

## ✅ Review Checklist

请 Review 以下要点：

- [ ] **架构设计**：分层是否合理？
- [ ] **数据模型**：EnemyData 字段是否完整？
- [ ] **事件系统**：Health.OnDead 是否可靠？
- [ ] **性能方案**：300ms 更新是否合适？
- [ ] **初始扫描**：FindObjectsOfType 一次是否够用？
- [ ] **辅助功能**：Boss/商人识别逻辑是否准确？
- [ ] **错误处理**：空引用检查是否完善？
- [ ] **时间预估**：7小时是否现实？

---

## 🚀 下一步

Review 通过后：
1. 开始 Phase 1 实施
2. 边开发边测试
3. 及时调整方案

期待您的反馈！ 🎯
