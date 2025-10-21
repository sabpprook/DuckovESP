# TriggerBot 基地保护功能

## 🛡️ 功能说明

为了防止在基地内误伤友军或 NPC，TriggerBot 现在会自动检测玩家是否在基地内。

**在基地内时，自动扳机将被自动禁用。**

---

## 🔍 基地检测逻辑

### 方法 1：场景名称检测（主要方法）

系统会检查当前场景名称，如果包含以下关键词，则认为在基地内：

- `shelter` - 庇护所
- `base` - 基地
- `safe` - 安全区
- `home` - 家
- `menu` - 菜单
- `lobby` - 大厅

### 方法 2：物体检测（备用方案，已注释）

代码中保留了一个备用方案（当前已注释），可以检测玩家周围是否有：
- Shelter（庇护所）
- Bed（床）
- 其他基地建筑

**如果需要启用此功能**，可以取消 `IsPlayerInBase()` 方法中的注释。

---

## 🎮 使用效果

### 在基地外（战斗区域）
- ✅ TriggerBot 正常工作
- ✅ 检测到敌人自动开火
- ✅ 子弹传送功能正常

### 在基地内（安全区域）
- ⛔ TriggerBot 自动禁用
- ⛔ 不会自动开火
- ✅ 扳机自动释放
- ✅ 其他功能（Aimbot、ESP）不受影响

---

## 📋 日志输出

启动时会显示：
```
[TriggerBot] 已启动 - 仅瞄准触发: True/False, 延迟: 0.05s
[TriggerBot] 使用自动瞄准的目标检测系统
[TriggerBot] 基地保护：在基地内禁用自动扳机
```

当玩家在基地内时：
- TriggerBot 静默禁用，不会有额外日志
- 如果扳机已按下，会看到 `[TriggerBot] 释放扳机`

---

## 🔧 自定义配置

如果游戏使用不同的场景名称，可以修改 `IsPlayerInBase()` 方法：

```csharp
private bool IsPlayerInBase()
{
    string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
    
    if (sceneName != null)
    {
        string lowerSceneName = sceneName.ToLower();
        
        // 添加你的基地场景名称
        if (lowerSceneName.Contains("your_base_name") ||
            lowerSceneName.Contains("another_safe_zone"))
        {
            return true;
        }
    }
    
    return false;
}
```

---

## 🐛 故障排除

### 问题：TriggerBot 在战斗区域也被禁用

**可能原因：**
- 场景名称包含了基地关键词
- 检查控制台查看当前场景名称

**解决方案：**
1. 查看当前场景名称：
   ```csharp
   Debug.Log($"当前场景: {UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}");
   ```
2. 从 `IsPlayerInBase()` 中移除误判的关键词

### 问题：在基地内 TriggerBot 仍然工作

**可能原因：**
- 基地场景名称不在检测列表中

**解决方案：**
1. 查看基地场景名称
2. 添加到 `IsPlayerInBase()` 方法的关键词列表中

### 问题：需要更精确的基地检测

**解决方案：**
启用物体检测（取消注释）：
```csharp
// 检查附近是否有 Shelter 或 Bed
Collider[] nearbyObjects = Physics.OverlapSphere(_player.transform.position, 10f);
foreach (var obj in nearbyObjects)
{
    if (obj.name.Contains("Shelter") || obj.name.Contains("Bed"))
        return true;
}
```

---

## 💡 高级功能（可选）

### 1. 添加距离检测

检测玩家是否离出生点太近：

```csharp
private Vector3 _spawnPoint = Vector3.zero; // 在 Awake 中记录出生点

private bool IsPlayerInBase()
{
    // 如果距离出生点 < 50 米，认为在基地
    if (_player != null && Vector3.Distance(_player.transform.position, _spawnPoint) < 50f)
    {
        return true;
    }
    
    // ... 其他检测逻辑
}
```

### 2. 添加友军密度检测

如果周围友军数量 > 3，认为在基地：

```csharp
private bool IsPlayerInBase()
{
    int friendlyCount = 0;
    Collider[] nearbyCharacters = Physics.OverlapSphere(_player.transform.position, 20f);
    
    foreach (var col in nearbyCharacters)
    {
        CharacterMainControl character = col.GetComponent<CharacterMainControl>();
        if (character != null && character.Team == _player.Team)
        {
            friendlyCount++;
        }
    }
    
    // 周围有 3+ 个友军，可能在基地
    if (friendlyCount >= 3)
    {
        return true;
    }
    
    // ... 其他检测逻辑
}
```

### 3. 添加白名单/黑名单场景

```csharp
private static readonly string[] BASE_SCENES = new[]
{
    "Shelter_Main",
    "PlayerBase_01",
    "SafeZone_Alpha"
};

private static readonly string[] COMBAT_SCENES = new[]
{
    "Forest_01",
    "City_Downtown",
    "Military_Base"
};

private bool IsPlayerInBase()
{
    string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
    
    // 白名单：明确的基地场景
    if (BASE_SCENES.Contains(sceneName))
        return true;
    
    // 黑名单：明确的战斗场景
    if (COMBAT_SCENES.Contains(sceneName))
        return false;
    
    // 默认使用关键词检测
    // ...
}
```

---

## 📊 性能影响

- **场景名称检测**：几乎无性能影响（每帧一次字符串比较）
- **物体检测**：轻微影响（每帧进行 Physics.OverlapSphere）
- **建议**：优先使用场景名称检测，仅在必要时启用物体检测

---

## ✅ 总结

TriggerBot 基地保护功能确保：
- ✅ 在基地内不会误伤友军
- ✅ 在战斗区域正常工作
- ✅ 自动检测，无需手动切换
- ✅ 性能影响极小
- ✅ 易于自定义和扩展

**默认配置适用于大多数情况，如需调整请参考"自定义配置"部分。**

---

*最后更新：2025-10-20*
*版本：v2.4.0*
