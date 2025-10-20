# DuckovESP 修复实施计划

**创建日期:** 2025-10-20  
**基于调查报告:** INVESTIGATION_REPORT.md  
**参考源码:** source_code_reference/Mod_Aimbot

---

## 📋 修复优先级

### 🔴 第一阶段: 核心功能修复（预计 4-6 小时）

#### 修复 1.1: 自动瞄准 - 子弹重定向逻辑 ⚡
**文件:** `DuckovESP/Features/Aimbot/AimbotSystem.cs`  
**问题:** 使用 `FindObjectsOfType` 查找子弹不可靠，应该使用反射直接获取  
**优先级:** 🔴 最高

#### 修复 1.2: 自动扳机 - 持续触发逻辑 ⚡
**文件:** `DuckovESP/Features/Aimbot/AimbotSystem.cs`  
**问题:** 只调用一次 SetTrigger，缺少状态管理  
**优先级:** 🔴 最高

#### 修复 6: 无限子弹 - 弹匣物品同步 ⚡
**文件:** `DuckovESP/Features/Cheats/CheatSystem.cs`  
**问题:** 只修改 bulletCount，未修改 Item.StackCount  
**优先级:** 🔴 最高

---

### 🟡 第二阶段: 功能完善（预计 3-5 小时）

#### 修复 2: 一击必杀 - 改用武器Stat修改 🎯
**文件:** `DuckovESP/Features/Cheats/CheatSystem.cs`, `AimbotSystem.cs`  
**问题:** 修改子弹伤害不稳定，应该修改武器属性  
**优先级:** 🟡 中

#### 修复 5: 地面物品 - 小地图标记和连线 📍
**文件:** `DuckovESP/ModBehaviour.cs`  
**问题:** 只为箱子创建标记，地面物品缺失  
**优先级:** 🟡 中

---

### 🔵 第三阶段: 体验优化（预计 1-2 小时）

#### 优化 4: 颜色系统 - 文本标签和配置 🎨
**文件:** `DuckovESP/ModBehaviour.cs`, `ESPConfig.cs`  
**问题:** 任务物品颜色覆盖高品质  
**优先级:** 🔵 低

---

## 🔧 详细修复方案

---

### 修复 1.1: 自动瞄准 - 子弹重定向逻辑 ⚡

#### 问题根因
```csharp
// ❌ 当前错误实现
private void RetargetProjectilesToTarget(...)
{
    // 使用 FindObjectsOfType 扫描（慢且不可靠）
    Projectile[] allProjectiles = FindObjectsOfType<Projectile>();
    // 子弹可能已经飞走，或者还未初始化完成
}
```

#### 参考源码分析
```csharp
// ✅ Mod_Aimbot 的正确实现
// Line 3329: 定义反射字段
private static readonly FieldInfo GunProjectileField = 
    typeof(ItemAgent_Gun).GetField("projInst", BindingFlags.Instance | BindingFlags.NonPublic);

// Line 2588: 在射击回调中使用
private Projectile RetargetImmediateProjectile(ItemAgent_Gun gun, ...)
{
    // 通过反射直接获取刚生成的子弹
    Projectile projectile = GunProjectileField.GetValue(gun) as Projectile;
    
    // 立即重定向
    if (projectile != null && GetProjectileTraveledDistance(projectile) <= 0.06f)
    {
        TryRetargetProjectile(projectile, direction, targetPoint, true);
    }
}
```

#### 修复步骤

**步骤 1: 添加反射字段声明**
```csharp
// 在 AimbotSystem.cs 顶部添加
public class AimbotSystem
{
    // ✅ 新增：反射字段定义（参考 Mod_Aimbot Line 3329）
    private static readonly FieldInfo GunProjectileField = 
        typeof(ItemAgent_Gun).GetField("projInst", BindingFlags.Instance | BindingFlags.NonPublic);
    
    private static readonly FieldInfo ProjectileTraveledDistanceField = 
        typeof(Projectile).GetField("traveledDistance", BindingFlags.Instance | BindingFlags.NonPublic);
    
    // 其他字段...
}
```

**位置:** Line ~25（类的顶部字段区域）

---

**步骤 2: 重写子弹重定向方法**
```csharp
/// <summary>
/// 重定向子弹到目标 - 优先使用反射获取，失败时回退到扫描
/// </summary>
private void RetargetProjectilesToTarget(Vector3 muzzlePosition, Vector3 targetPoint)
{
    if (_trackedGun == null)
        return;
    
    Vector3 direction = (targetPoint - muzzlePosition).normalized;
    
    // ✅ 方法1: 使用反射直接获取刚发射的子弹（优先）
    Projectile immediateProjectile = TryGetImmediateProjectile(_trackedGun);
    if (immediateProjectile != null)
    {
        int retargetedCount = 0;
        
        // 重定向主子弹
        if (TryRetargetSingleProjectile(immediateProjectile, direction, targetPoint))
        {
            _retargetedProjectiles.Add(immediateProjectile.GetInstanceID());
            retargetedCount++;
            
            Debug.Log($"DuckovESP Aimbot: 使用反射成功重定向主子弹");
        }
        
        // 散弹枪需要处理多个子弹
        if (_trackedGun.ShotCount > 1)
        {
            retargetedCount += RetargetAdditionalProjectiles(direction, targetPoint, _trackedGun.ShotCount - 1);
        }
        
        if (retargetedCount > 0)
        {
            Debug.Log($"DuckovESP Aimbot: 成功重定向 {retargetedCount} 枚子弹");
            return;
        }
    }
    
    // ❌ 方法2: 回退到扫描方式（仅在反射失败时）
    Debug.LogWarning("DuckovESP Aimbot: 反射获取子弹失败，回退到扫描方式");
    RetargetProjectilesByScanning(muzzlePosition, direction, targetPoint);
}

/// <summary>
/// 使用反射获取刚发射的子弹（参考 Mod_Aimbot Line 2588）
/// </summary>
private Projectile TryGetImmediateProjectile(ItemAgent_Gun gun)
{
    if (GunProjectileField == null)
    {
        Debug.LogWarning("DuckovESP Aimbot: GunProjectileField 反射失败");
        return null;
    }
    
    try
    {
        Projectile projectile = GunProjectileField.GetValue(gun) as Projectile;
        
        if (projectile == null)
        {
            Debug.LogWarning("DuckovESP Aimbot: 反射获取的子弹为 null");
            return null;
        }
        
        // 验证子弹所有者
        if (projectile.context.fromCharacter != gun.Holder)
        {
            Debug.LogWarning("DuckovESP Aimbot: 子弹所有者不匹配");
            return null;
        }
        
        // 验证子弹未飞行太远
        float traveledDistance = GetProjectileTraveledDistance(projectile);
        if (traveledDistance > 0.06f)
        {
            Debug.LogWarning($"DuckovESP Aimbot: 子弹已飞行 {traveledDistance}m，太远了");
            return null;
        }
        
        return projectile;
    }
    catch (Exception ex)
    {
        Debug.LogError($"DuckovESP Aimbot: 反射获取子弹异常 - {ex.Message}");
        return null;
    }
}

/// <summary>
/// 重定向单个子弹（参考 Mod_Aimbot Line 2694）
/// </summary>
private bool TryRetargetSingleProjectile(Projectile projectile, Vector3 direction, Vector3 targetPoint)
{
    if (projectile == null || direction.sqrMagnitude < 0.0001f)
        return false;
    
    try
    {
        // 设置子弹方向
        projectile.context.direction = direction;
        projectile.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
        
        // 调整检测起点
        projectile.context.firstFrameCheckStartPoint = projectile.transform.position - direction * 0.1f;
        
        // 确保射程足够
        float distanceToTarget = Vector3.Distance(projectile.transform.position, targetPoint) + 2f;
        if (projectile.context.distance < distanceToTarget)
        {
            projectile.context.distance = distanceToTarget;
        }
        
        // 强制暴击（瞄准头部）
        if (_config.AimbotAimAtHead)
        {
            projectile.context.critRate = 1f;
        }
        
        // 一击必杀
        if (_cheatSystem != null && _cheatSystem.IsOneHitKillEnabled())
        {
            // ⚠️ 注意：这里的伤害修改可能不生效，后续会改用武器Stat修改
            projectile.context.damage *= 10f; // 降低到10倍避免溢出
            projectile.context.critRate = 1f;
            projectile.context.critDamageFactor = 5f;
        }
        
        // 忽略半掩体
        projectile.context.ignoreHalfObsticle = true;
        
        return true;
    }
    catch (Exception ex)
    {
        Debug.LogError($"DuckovESP Aimbot: 重定向子弹失败 - {ex.Message}");
        return false;
    }
}

/// <summary>
/// 重定向额外的子弹（散弹枪）
/// </summary>
private int RetargetAdditionalProjectiles(Vector3 direction, Vector3 targetPoint, int count)
{
    // 扫描场景中的其他子弹
    Projectile[] allProjectiles = UnityEngine.Object.FindObjectsOfType<Projectile>();
    if (allProjectiles == null || allProjectiles.Length == 0)
        return 0;
    
    int retargetedCount = 0;
    
    foreach (Projectile projectile in allProjectiles
        .Where(p => p != null && p.context.fromCharacter == _player)
        .OrderBy(p => GetProjectileTraveledDistance(p)))
    {
        if (retargetedCount >= count)
            break;
        
        int projectileId = projectile.GetInstanceID();
        if (_retargetedProjectiles.Contains(projectileId))
            continue;
        
        float traveledDistance = GetProjectileTraveledDistance(projectile);
        if (traveledDistance > 0.06f)
            continue;
        
        if (TryRetargetSingleProjectile(projectile, direction, targetPoint))
        {
            _retargetedProjectiles.Add(projectileId);
            retargetedCount++;
        }
    }
    
    return retargetedCount;
}

/// <summary>
/// 回退方法：扫描所有子弹
/// </summary>
private void RetargetProjectilesByScanning(Vector3 muzzlePosition, Vector3 direction, Vector3 targetPoint)
{
    // 保留原有的扫描逻辑作为备用
    Projectile[] allProjectiles = UnityEngine.Object.FindObjectsOfType<Projectile>();
    // ... 原有逻辑 ...
}

/// <summary>
/// 获取子弹已飞行的距离（参考 Mod_Aimbot Line 2774）
/// </summary>
private static float GetProjectileTraveledDistance(Projectile projectile)
{
    if (ProjectileTraveledDistanceField == null)
        return 0f;
    
    try
    {
        return (float)ProjectileTraveledDistanceField.GetValue(projectile);
    }
    catch
    {
        return 0f;
    }
}
```

**位置:** 替换现有的 `RetargetProjectilesToTarget` 方法（Line ~145-220）

---

**步骤 3: 更新 OnGunShoot 调用**
```csharp
/// <summary>
/// 枪械射击事件回调
/// </summary>
private void OnGunShoot()
{
    if (!_config.EnableAimbot)
        return;
    
    if (_trackedGun == null)
        return;
    
    // 查找最佳目标
    AutoAimCandidate candidate = FindBestTarget();
    if (candidate.Receiver == null)
    {
        Debug.Log("DuckovESP Aimbot: 未找到有效目标");
        return;
    }
    
    Debug.Log($"DuckovESP Aimbot: 锁定目标 {candidate.Receiver.name}, 屏幕距离 {candidate.ScreenDistance:F1}px, 实际距离 {candidate.RayDistance:F1}m");
    
    // 获取枪口位置
    Vector3 muzzlePosition = _trackedGun.muzzle != null 
        ? _trackedGun.muzzle.position 
        : _trackedGun.transform.position;
    
    // ✅ 使用新的重定向方法
    RetargetProjectilesToTarget(muzzlePosition, candidate.AimPoint);
}
```

**位置:** 更新现有的 `OnGunShoot` 方法（Line ~125-150）

---

**测试验证:**
```csharp
// 添加详细日志
Debug.Log($"反射字段状态: GunProjectileField={(GunProjectileField != null ? "✓" : "✗")}");
Debug.Log($"获取到的子弹: {(projectile != null ? projectile.name : "null")}");
Debug.Log($"子弹飞行距离: {traveledDistance}m");
Debug.Log($"重定向结果: {(success ? "成功" : "失败")}");
```

---

### 修复 1.2: 自动扳机 - 持续触发逻辑 ⚡

#### 问题根因
```csharp
// ❌ 当前错误实现
private void TryShoot()
{
    // 只调用一次，无法持续射击
    _trackedGun.SetTrigger(true, true, false);
}
```

#### 参考源码分析
```csharp
// ✅ Mod_Aimbot 的正确实现（Line 862-924）
private void MaintainTriggerAccess(CharacterMainControl character)
{
    // 每帧检查扳机输入
    InputManager inputManager = LevelManager.Instance.InputManager;
    bool triggerInput = inputManager.TriggerInput;
    
    // 计算边缘触发
    bool justPressed = triggerInput && !this._lastTriggerInput;
    bool justReleased = !triggerInput && this._lastTriggerInput;
    this._lastTriggerInput = triggerInput;
    
    // 如果不满足射击条件，强制调用 SetTrigger
    if (character.Running || disableTriggerTimer > 0 || isDashing)
    {
        agentHolder.SetTrigger(triggerInput, justPressed, justReleased);
    }
}
```

#### 修复步骤

**步骤 1: 添加状态跟踪字段**
```csharp
public class AimbotSystem
{
    // ✅ 新增：扳机状态跟踪
    private bool _lastTriggerState = false;
    private float _continuousFireTimer = 0f;
    private const float FIRE_RATE_INTERVAL = 0.1f; // 最小射击间隔（秒）
    
    // 其他字段...
}
```

**位置:** Line ~40（字段定义区域）

---

**步骤 2: 重写 PerformTriggerBot 方法**
```csharp
/// <summary>
/// 执行自动扳机（参考 Mod_Aimbot Line 862-924）
/// </summary>
private void PerformTriggerBot()
{
    // 更新连续射击计时器
    if (_continuousFireTimer > 0)
    {
        _continuousFireTimer -= Time.deltaTime;
    }
    
    // 检查是否有敌人在准星下
    DamageReceiver target = GetTargetUnderCrosshair();
    bool currentTargetInSight = (target != null);
    
    // 目标状态变化
    if (currentTargetInSight && !_targetInSight)
    {
        // 刚发现目标
        _targetInSight = true;
        _triggerDelayTimer = _config.TriggerBotDelay;
        Debug.Log($"DuckovESP TriggerBot: 发现目标 {target.name}，延迟 {_config.TriggerBotDelay}s 后射击");
    }
    else if (!currentTargetInSight && _targetInSight)
    {
        // 目标消失
        _targetInSight = false;
        _triggerDelayTimer = 0f;
        
        // ✅ 释放扳机
        ReleaseTrigger();
        Debug.Log("DuckovESP TriggerBot: 目标消失，释放扳机");
    }
    
    // 延迟结束，执行射击
    if (_targetInSight && _triggerDelayTimer <= 0 && _continuousFireTimer <= 0)
    {
        // ✅ 持续按住扳机（而不是一次性触发）
        HoldTrigger();
        
        // 设置下次射击间隔（避免射速过快）
        _continuousFireTimer = FIRE_RATE_INTERVAL;
    }
}

/// <summary>
/// 按住扳机（持续射击）
/// </summary>
private void HoldTrigger()
{
    if (_trackedGun == null)
        return;
    
    try
    {
        bool justPressed = !_lastTriggerState; // 首次按下
        
        // ✅ 持续调用 SetTrigger
        // trigger=true: 扳机按住
        // justPressed=true: 仅在首次按下时为true
        // justReleased=false: 未释放
        _trackedGun.SetTrigger(true, justPressed, false);
        
        _lastTriggerState = true;
        
        if (justPressed)
        {
            Debug.Log("DuckovESP TriggerBot: 按下扳机");
        }
    }
    catch (Exception ex)
    {
        Debug.LogError($"DuckovESP TriggerBot: 射击时出错 - {ex.Message}");
    }
}

/// <summary>
/// 释放扳机
/// </summary>
private void ReleaseTrigger()
{
    if (_trackedGun == null)
        return;
    
    try
    {
        if (_lastTriggerState)
        {
            // ✅ 释放扳机
            // trigger=false: 扳机释放
            // justPressed=false: 未按下
            // justReleased=true: 刚释放
            _trackedGun.SetTrigger(false, false, true);
            
            _lastTriggerState = false;
            Debug.Log("DuckovESP TriggerBot: 释放扳机");
        }
    }
    catch (Exception ex)
    {
        Debug.LogError($"DuckovESP TriggerBot: 释放扳机时出错 - {ex.Message}");
    }
}
```

**位置:** 替换现有的 `PerformTriggerBot` 和 `TryShoot` 方法（Line ~600-710）

---

**步骤 3: 在 Update 中持续调用**
```csharp
/// <summary>
/// 更新自动瞄准系统
/// </summary>
public void Update()
{
    // ... 现有逻辑 ...
    
    // ✅ 自动扳机（每帧更新）
    if (_config.EnableTriggerBot)
    {
        PerformTriggerBot();
    }
    else
    {
        // 禁用时确保释放扳机
        if (_lastTriggerState)
        {
            ReleaseTrigger();
        }
    }
    
    // ... 其他逻辑 ...
}
```

**位置:** 更新现有的 `Update` 方法（Line ~80-120）

---

**测试验证:**
```csharp
// 添加状态日志
Debug.Log($"扳机状态: {(_lastTriggerState ? "按住" : "释放")}");
Debug.Log($"目标在准星下: {_targetInSight}");
Debug.Log($"延迟计时器: {_triggerDelayTimer:F2}s");
Debug.Log($"连续射击间隔: {_continuousFireTimer:F2}s");
```

---

### 修复 6: 无限子弹 - 弹匣物品同步 ⚡

#### 问题根因
```csharp
// ❌ 当前错误实现
private void ApplyInfiniteAmmo(CharacterMainControl player)
{
    // 只修改了 bulletCount（子弹计数）
    BulletCountProperty.SetValue(gunSetting, capacity, null);
    
    // ❌ 没有修改弹匣中的实际子弹物品
    // 游戏检测到 Item.StackCount = 0，认为没有子弹
    // → 伤害 = 0
}
```

#### 参考源码分析
```csharp
// ✅ Mod_Aimbot 的正确实现（Line 2136-2177）
private void RestoreGunAmmo(ItemAgent_Gun gun)
{
    // 1. 修改 bulletCount
    ApplyBulletCount(gunItemSetting, num2);
    
    // 2. ✅ 修改弹匣中的子弹物品数量
    EnsureMagazineStack(gunItemSetting, num2);
}

private void EnsureMagazineStack(ItemSetting_Gun setting, int desiredCount)
{
    Inventory inventory = setting.Item?.Inventory;
    
    // 查找弹匣中的子弹 Item
    Item item = null;
    foreach (Item item2 in inventory)
    {
        if (item2 != null)
        {
            item = item2;
            break;
        }
    }
    
    // ✅ 修改堆叠数量
    if (item.StackCount < desiredCount)
    {
        item.StackCount = desiredCount;
    }
}
```

#### 修复步骤

**步骤 1: 添加 EnsureMagazineStack 方法**
```csharp
/// <summary>
/// 确保弹匣中有足够的子弹物品（参考 Mod_Aimbot Line 2156）
/// </summary>
private void EnsureMagazineStack(ItemSetting_Gun setting, int desiredCount)
{
    if (setting == null || setting.Item == null)
        return;
    
    try
    {
        // 获取弹匣 Inventory
        Inventory inventory = setting.Item.Inventory;
        if (inventory == null)
        {
            Debug.LogWarning("DuckovESP: 无限子弹 - 弹匣 Inventory 为空");
            return;
        }
        
        // 查找弹匣中的子弹 Item
        Item bulletItem = null;
        foreach (Item item in inventory)
        {
            if (item != null)
            {
                bulletItem = item;
                break;
            }
        }
        
        if (bulletItem == null)
        {
            Debug.LogWarning("DuckovESP: 无限子弹 - 未找到弹匣中的子弹物品");
            return;
        }
        
        // ✅ 修改堆叠数量
        if (bulletItem.StackCount < desiredCount)
        {
            int originalCount = bulletItem.StackCount;
            bulletItem.StackCount = desiredCount;
            
            Debug.Log($"DuckovESP: 无限子弹 - 子弹物品数量 {originalCount} → {desiredCount}");
        }
    }
    catch (Exception ex)
    {
        Debug.LogError($"DuckovESP: 确保弹匣子弹失败 - {ex.Message}");
    }
}
```

**位置:** 在 `CheatSystem.cs` 中添加新方法（Line ~270 之后）

---

**步骤 2: 修改 ApplyInfiniteAmmo 方法**
```csharp
/// <summary>
/// 应用无限子弹（参考 Mod_Aimbot Line 2136-2177）
/// </summary>
private void ApplyInfiniteAmmo(CharacterMainControl player)
{
    if (!_infiniteAmmoEnabled)
        return;
    
    try
    {
        // 获取当前持有的枪械
        var agentHolder = player.agentHolder;
        if (agentHolder == null)
            return;
        
        var currentGun = agentHolder.CurrentHoldGun;
        if (currentGun == null || currentGun.GunItemSetting == null)
            return;
        
        var gunSetting = currentGun.GunItemSetting;
        int bulletCount = gunSetting.BulletCount;
        int capacity = Mathf.Max(0, gunSetting.Capacity);
        
        // 计算目标子弹数（+1发，避免触发换弹）
        int targetCount = bulletCount + 1;
        if (capacity > 0)
        {
            targetCount = Mathf.Clamp(targetCount, 0, capacity);
        }
        
        // ✅ 1. 修改 bulletCount
        ApplyBulletCount(gunSetting, targetCount);
        
        // ✅ 2. 修改弹匣中的子弹物品数量（关键！）
        EnsureMagazineStack(gunSetting, targetCount);
    }
    catch (Exception ex)
    {
        Debug.LogWarning($"DuckovESP: 应用无限子弹失败 - {ex.Message}");
    }
}

/// <summary>
/// 应用子弹数量
/// </summary>
private void ApplyBulletCount(ItemSetting_Gun setting, int desiredCount)
{
    if (BulletCountProperty == null)
        return;
    
    try
    {
        int originalCount = setting.BulletCount;
        BulletCountProperty.SetValue(setting, desiredCount, null);
        
        Debug.Log($"DuckovESP: 无限子弹 - bulletCount {originalCount} → {desiredCount}");
    }
    catch (Exception ex)
    {
        Debug.LogWarning($"DuckovESP: 设置子弹数量失败 - {ex.Message}");
    }
}
```

**位置:** 替换现有的 `ApplyInfiniteAmmo` 方法（Line ~239-270）

---

**步骤 3: 在射击时立即恢复子弹（可选，更可靠）**
```csharp
// 在 AimbotSystem.cs 的 OnGunShoot() 中添加
private void OnGunShoot()
{
    // ... 自瞄逻辑 ...
    
    // ✅ 射击后立即恢复子弹（避免延迟）
    if (_cheatSystem != null && _cheatSystem.IsInfiniteAmmoEnabled())
    {
        _cheatSystem.RestoreGunAmmoImmediate(_trackedGun);
    }
}

// 在 CheatSystem.cs 中添加公共方法
public void RestoreGunAmmoImmediate(ItemAgent_Gun gun)
{
    if (gun == null || gun.GunItemSetting == null)
        return;
    
    var gunSetting = gun.GunItemSetting;
    int capacity = Mathf.Max(0, gunSetting.Capacity);
    
    ApplyBulletCount(gunSetting, capacity);
    EnsureMagazineStack(gunSetting, capacity);
}
```

---

**测试验证:**
```csharp
// 添加详细日志
Debug.Log($"枪械: {currentGun.name}");
Debug.Log($"容量: {capacity}");
Debug.Log($"当前子弹计数: {bulletCount}");
Debug.Log($"弹匣物品数量: {bulletItem.StackCount}");
Debug.Log($"目标子弹数: {targetCount}");
```

---

## 📊 第一阶段修复总结

### 修改文件清单
1. ✅ `DuckovESP/Features/Aimbot/AimbotSystem.cs`
   - 添加反射字段（GunProjectileField, ProjectileTraveledDistanceField）
   - 重写 `RetargetProjectilesToTarget` 方法
   - 添加 `TryGetImmediateProjectile` 方法
   - 添加 `TryRetargetSingleProjectile` 方法
   - 重写 `PerformTriggerBot` 方法
   - 添加 `HoldTrigger` 和 `ReleaseTrigger` 方法

2. ✅ `DuckovESP/Features/Cheats/CheatSystem.cs`
   - 修改 `ApplyInfiniteAmmo` 方法
   - 添加 `ApplyBulletCount` 方法
   - 添加 `EnsureMagazineStack` 方法
   - 添加 `RestoreGunAmmoImmediate` 公共方法

### 预期效果
- ✅ 自动瞄准可靠工作（使用反射直接获取子弹）
- ✅ 自动扳机持续射击（状态管理+持续触发）
- ✅ 无限子弹有伤害（同时修改 bulletCount 和 Item.StackCount）

### 测试清单
- [ ] 自动瞄准：单发武器（手枪、步枪）
- [ ] 自动瞄准：散弹枪（多个子弹）
- [ ] 自动扳机：发现目标后延迟射击
- [ ] 自动扳机：目标消失后停止射击
- [ ] 无限子弹：射击后子弹数量恢复
- [ ] 无限子弹：射击有伤害
- [ ] 组合测试：自瞄+自动扳机+无限子弹

---

## 🎯 第二阶段修复方案（简要）

### 修复 2: 一击必杀 - 改用武器Stat修改

**原理:** 不修改子弹伤害，而是修改武器的基础伤害Stat

```csharp
// ✅ 参考 Mod_Aimbot Line 1114-1128
private void ApplyStatMultiplier(Item item, int statHash, float multiplier)
{
    Stat stat = item.GetStat(statHash);
    float originalValue = stat.BaseValue;
    stat.BaseValue = originalValue * multiplier;
}

// 应用伤害倍增
if (_oneHitKillEnabled)
{
    ApplyStatMultiplier(gun.Item, DamageStatHash, 10f); // 10倍伤害
}
```

**实施位置:** `CheatSystem.cs` 新增方法，在武器切换时应用

---

### 修复 5: 地面物品 - 小地图标记和连线

**原理:** 为 DuckovItemAgent (pickup类型) 创建小地图标记

```csharp
// 新增数据结构
private readonly Dictionary<DuckovItemAgent, WorldItemMarkerData> _trackedWorldItems = new();

// 在 ScanWorldItems 中创建标记
GameObject marker = CreateMarkerForWorldItem(itemAgent, item);
_trackedWorldItems[itemAgent] = new WorldItemMarkerData { ... };
```

**实施位置:** `ModBehaviour.cs`

---

## 📋 实施时间表

### Day 1 (4-6小时)
- ✅ 修复 1.1: 自动瞄准（2-3小时）
- ✅ 修复 1.2: 自动扳机（1-2小时）
- ✅ 修复 6: 无限子弹（1小时）
- ✅ 测试核心功能（1小时）

### Day 2 (3-5小时)
- ✅ 修复 2: 一击必杀（1-2小时）
- ✅ 修复 5: 地面物品标记（2-3小时）
- ✅ 测试和调试（1小时）

### Day 3 (可选，1-2小时)
- ✅ 优化 4: 颜色系统
- ✅ 更新文档
- ✅ 最终测试

---

## 🧪 完整测试场景

### 场景 1: 自动瞄准基础测试
1. 进入游戏，装备手枪
2. 启用自动瞄准（FOV=30°）
3. 寻找敌人，右键瞄准
4. **预期:** 摄像机自动锁定敌人，平滑跟随
5. **日志:** 查看 "使用反射成功重定向主子弹"

### 场景 2: 自动扳机测试
1. 启用自动扳机（延迟0.1秒）
2. 准星对准敌人
3. **预期:** 0.1秒后自动开火
4. 移开准星
5. **预期:** 立即停止射击

### 场景 3: 无限子弹测试
1. 启用无限子弹
2. 持续射击直到弹匣为空
3. **预期:** 子弹数量始终恢复，所有射击都有伤害
4. **日志:** 查看 "子弹物品数量 X → Y"

### 场景 4: 组合测试
1. 同时启用：自瞄+自动扳机+无限子弹+一击必杀
2. 进入战斗
3. **预期:** 自动锁定→自动射击→无限子弹→秒杀敌人

---

## 📝 备注

### 关键发现
1. **反射字段名:** `projInst` 不是 `projectile`
2. **子弹检测时机:** 必须在 OnShootEvent 内立即获取
3. **扳机状态管理:** 需要跟踪 lastTriggerState
4. **子弹伤害问题:** Item.StackCount 是关键

### 风险评估
- ✅ 低风险：反射获取子弹（Mod_Aimbot 已验证）
- ✅ 低风险：扳机状态管理（标准实现）
- ⚠️ 中风险：弹匣物品操作（可能影响存档）
- ⚠️ 中风险：伤害倍增（可能破坏游戏平衡）

### 回滚计划
- 保留 `RetargetProjectilesByScanning` 作为备用
- 添加配置选项：`UseReflectionForAimbot`
- 如果反射失败，自动回退到扫描方式

---

**准备就绪！请确认是否开始实施修复。**
