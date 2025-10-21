# 4个问题的修复方案 (基于源代码分析)

## 📋 总结

本文档基于 `source_code_reference/Mod_Aimbot/CheatMenu/ModBehaviour.cs` 的源代码分析，提供可靠的修复方案。

---

## 问题 #1: 玩家自身高价值装备被标记到小地图中

### 根本原因
`ModBehaviour.cs` 中的 `ScanAllLootboxes()` 不区分"独立箱子"和"人物身上的背包"。

### 当前代码问题
```csharp
// ModBehaviour.cs Line 1406
if (IsLootboxOnCharacter(box))
    continue;

// Line 1549 - 检测逻辑太简单
private bool IsLootboxOnCharacter(InteractableLootbox lootbox)
{
    CharacterMainControl character = lootbox.GetComponent<CharacterMainControl>();
    if (character == null)
        character = lootbox.GetComponentInParent<CharacterMainControl>();
    return character != null;
}
```

### 改进方案
**关键点**: 检查 `lootbox` 是否属于**玩家本身**（自己的背包/装备），而不是其他角色

```csharp
private bool IsLootboxOnCharacter(InteractableLootbox lootbox)
{
    if (lootbox == null)
        return false;

    try
    {
        // 【关键】获取当前玩家
        CharacterMainControl playerChar = CharacterMainControl.Main;
        if (playerChar == null)
            return false;

        // 检查 Lootbox 是否在玩家上
        CharacterMainControl owner = lootbox.GetComponentInParent<CharacterMainControl>();
        if (owner != null)
        {
            // 如果拥有者就是玩家自己，则过滤掉
            if (owner == playerChar)
                return true;
        }

        // 检查是否是独立的地面箱子（通常距离玩家有一定距离）
        Vector3 boxPos = lootbox.transform.position;
        Vector3 playerPos = playerChar.transform.position;
        float distance = Vector3.Distance(boxPos, playerPos);
        
        // 如果在玩家身上（距离 < 3米），可能是玩家的装备
        if (distance < 3f && owner != null && owner != playerChar)
        {
            return true; // 也过滤掉（这是其他角色的装备）
        }

        return false;
    }
    catch
    {
        return false;
    }
}
```

---

## 问题 #2: 无敌模式还会被雷炸死

### 源代码参考

**Mod_Aimbot 的实现** (ModBehaviour.cs Line 3514):
```csharp
private void ApplyInvincibility(CharacterMainControl character)
{
    if (!this._invincibilityEnabled)
    {
        this.RestoreCharacterInvincibility();
        return;
    }
    if (character == null)
        return;
    
    Health health = character.Health;
    if (health == null)
        return;
    
    // 【关键】直接调用 Health.SetInvincible(true)
    if (!health.Invincible)
    {
        health.SetInvincible(true);  // ⭐ 这是正确的做法！
    }
    
    this._invincibilityTargetHealth = health;
}
```

### 当前 DuckovESP 的问题
```csharp
// CheatSystem.cs - 当前实现只是恢复血量
private void ApplyGodMode(CharacterMainControl player)
{
    if (!_godModeEnabled)
        return;

    try
    {
        var health = player.Health;
        if (health != null)
        {
            // ❌ 问题：只是恢复血量，不能阻止即将到来的伤害
            if (health.CurrentHealth < health.MaxHealth)
            {
                player.AddHealth(health.MaxHealth - health.CurrentHealth);
            }
        }
    }
    catch (Exception ex)
    {
        Debug.LogWarning($"Error: {ex.Message}");
    }
}
```

### 改进方案
```csharp
private void ApplyGodMode(CharacterMainControl player)
{
    if (!_godModeEnabled)
        return;

    try
    {
        var health = player.Health;
        if (health == null)
            return;
        
        // 【关键修复】使用 Health.SetInvincible(true)
        // 这个方法会直接标记角色为无敌，游戏不会对其造成任何伤害
        if (!health.Invincible)
        {
            health.SetInvincible(true);
        }
        
        // 备份恢复血量逻辑，以防 SetInvincible 失效
        if (health.CurrentHealth <= 0)
        {
            health.Recovery(health.MaxHealth);
        }
    }
    catch (Exception ex)
    {
        Debug.LogWarning(LocalizationManager.Get("Error.ApplyGodMode", ("error", ex.Message)));
    }
}

// 恢复无敌模式时调用
public void DisableGodMode()
{
    try
    {
        CharacterMainControl player = CharacterMainControl.Main;
        if (player != null)
        {
            var health = player.Health;
            if (health != null && health.Invincible)
            {
                health.SetInvincible(false);
            }
        }
    }
    catch { }
}
```

---

## 问题 #3: 一击必杀无效

### 源代码参考

**Mod_Aimbot 的实现** (ModBehaviour.cs Line 2406):
```csharp
private void ApplyStatMultiplier(Item item, int statHash, float multiplier)
{
    if (item == null)
        return;
    
    Stat stat = item.GetStat(statHash);
    if (stat == null)
        return;
    
    // 备份原始值
    Dictionary<int, float> dictionary;
    if (!this._statMultiplierOriginalValues.TryGetValue(item, out dictionary))
    {
        dictionary = new Dictionary<int, float>();
        this._statMultiplierOriginalValues[item] = dictionary;
    }
    
    float baseValue;
    if (!dictionary.TryGetValue(statHash, out baseValue))
    {
        baseValue = stat.BaseValue;
        dictionary[statHash] = baseValue;
    }
    
    // 【关键】修改 Stat.BaseValue
    float newValue = baseValue * multiplier;
    stat.BaseValue = newValue;
}

// 调用方式（Line 2402）:
private static readonly int DamageStatHash = "Damage".GetHashCode();
this.ApplyStatMultiplier(gun.Item, ModBehaviour.DamageStatHash, this._damageMultiplier);
```

### 改进方案

需要在每次射击时或武器切换时应用伤害倍增：

```csharp
private Dictionary<Item, Dictionary<int, float>> _originalStatValues = 
    new Dictionary<Item, Dictionary<int, float>>();

private static readonly int DamageStatHash = "Damage".GetHashCode();

/// <summary>
/// 应用一击必杀（通过修改武器伤害Stat）
/// 参考 Mod_Aimbot 的实现方式
/// </summary>
private void ApplyOneHitKill(CharacterMainControl player)
{
    if (!_oneHitKillEnabled)
        return;

    try
    {
        // 获取当前持有的武器
        var agentHolder = player.agentHolder;
        if (agentHolder != null)
        {
            var currentGun = agentHolder.CurrentHoldGun;
            if (currentGun != null && currentGun.Item != null)
            {
                // 【关键】应用伤害倍增到武器的 Damage Stat
                ApplyStatMultiplier(currentGun.Item, DamageStatHash, 999f);
            }
        }
    }
    catch (Exception ex)
    {
        Debug.LogWarning($"DuckovESP: 应用一击必杀失败 - {ex.Message}");
    }
}

/// <summary>
/// 修改 Item 的 Stat 值（完全参考 Mod_Aimbot）
/// </summary>
private void ApplyStatMultiplier(Item item, int statHash, float multiplier)
{
    if (item == null)
        return;
    
    Stat stat = item.GetStat(statHash);
    if (stat == null)
        return;
    
    // 备份原始值
    Dictionary<int, float> dictionary;
    if (!_originalStatValues.TryGetValue(item, out dictionary))
    {
        dictionary = new Dictionary<int, float>();
        _originalStatValues[item] = dictionary;
    }
    
    float baseValue;
    if (!dictionary.TryGetValue(statHash, out baseValue))
    {
        baseValue = stat.BaseValue;
        dictionary[statHash] = baseValue;
    }
    
    // 修改 Stat.BaseValue
    float newValue = baseValue * multiplier;
    stat.BaseValue = newValue;
}

/// <summary>
/// 恢复原始伤害
/// </summary>
private void RestoreOriginalDamage()
{
    foreach (var kvp in _originalStatValues)
    {
        Item item = kvp.Key;
        if (item != null)
        {
            foreach (var statKvp in kvp.Value)
            {
                Stat stat = item.GetStat(statKvp.Key);
                if (stat != null)
                {
                    stat.BaseValue = statKvp.Value;
                }
            }
        }
    }
    _originalStatValues.Clear();
}
```

---

## 问题 #4: 敌人列表翻译失效

### 根本原因
EnemyListWindow.cs 调用的本地化键名前缀错误。

### 当前代码问题
```csharp
// EnemyListWindow.cs - 所有调用都用了 "UI.EnemyList.XXX" 的前缀
LocalizationManager.Get("UI.EnemyList.Title", ...)  // ❌ 错误
LocalizationManager.Get("UI.EnemyList.NoEnemiesDetected")  // ❌ 错误
```

### JSON 文件结构
```json
{
  "UI": {
    // ...其他键...
  },
  "EnemyList": {  // ⭐ 在根级别，不在 UI 下面
    "Title": "🎯 敌人雷达 - {count} 个目标 (按{key}切换)",
    "NoEnemiesDetected": "未检测到敌人",
    "EnemyCount": "敌人数量: {count}",
    // ... 等等
  }
}
```

### 修复方案
**简单方案：修改所有调用的前缀**

将所有 `"UI.EnemyList.XXX"` 改为 `"EnemyList.XXX"`：

```csharp
// EnemyListWindow.cs

// Line 51
LocalizationManager.Get("EnemyList.Title", ...)  // ✅ 正确

// Line 83
LocalizationManager.Get("EnemyList.NoEnemiesDetected")  // ✅ 正确

// Line 113
LocalizationManager.Get("EnemyList.EnemyCount", ...)  // ✅ 正确

// Line 124
LocalizationManager.Get("EnemyList.TotalValue", ...)  // ✅ 正确

// Line 139
LocalizationManager.Get("EnemyList.AimingWarning", ...)  // ✅ 正确

// Line 151-156
LocalizationManager.Get("EnemyList.ColumnName")  // ✅ 正确
LocalizationManager.Get("EnemyList.ColumnDistance")  // ✅ 正确
// 等等...

// Line 201
LocalizationManager.Get("EnemyList.Aiming")  // ✅ 正确
LocalizationManager.Get("EnemyList.Normal")  // ✅ 正确

// Line 219
LocalizationManager.Get("EnemyList.Hint", ...)  // ✅ 正确
```

---

## 验证清单

- [ ] 问题 #1：测试玩家自身装备是否不显示在小地图
- [ ] 问题 #2：启用无敌模式后被雷击测试
- [ ] 问题 #3：一击必杀对枪械是否生效
- [ ] 问题 #4：启动游戏检查敌人列表窗口是否正确显示

---

## 风险评估

| 问题 | 风险等级 | 说明 |
|------|---------|------|
| #1 | 🟢 低 | 只是添加距离检查，逻辑清晰 |
| #2 | 🟢 低 | Mod_Aimbot 已验证的方法 |
| #3 | 🟢 低 | Mod_Aimbot 已验证的方法 |
| #4 | 🟢 低 | 只是修改字符串，无逻辑风险 |

---

## 参考文献

- `source_code_reference/Mod_Aimbot/CheatMenu/ModBehaviour.cs:3514` - ApplyInvincibility
- `source_code_reference/Mod_Aimbot/CheatMenu/ModBehaviour.cs:2406` - ApplyStatMultiplier
- `source_code_reference/Mod_Aimbot/CheatMenu/ModBehaviour.cs:4966` - DamageStatHash 定义
