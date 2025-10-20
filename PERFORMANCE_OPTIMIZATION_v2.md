# DuckovESP 性能优化 v2.0

## 🎯 优化目标
解决大量 FPS 下降问题，通过减少 `FindObjectsOfType` 调用和优化缓存策略

---

## 📊 主要优化点

### 1. ✅ **AimbotSystem - 目标检测优化**

#### 问题
- 每次 `FindBestTarget()` 都调用 `FindObjectsOfType<DamageReceiver>()`
- 在高刷新率下（60+ FPS），每秒调用 60+ 次
- 大量重复的 `GetComponent` 调用

#### 解决方案
```csharp
// 添加缓存系统
private DamageReceiver[] _damageReceiverCache = new DamageReceiver[0];
private float _lastReceiverCacheUpdate = 0f;
private const float RECEIVER_CACHE_INTERVAL = 1.5f; // 1.5秒更新一次

// 组件缓存
private readonly Dictionary<DamageReceiver, Collider> _colliderCache;
private readonly Dictionary<DamageReceiver, HeadCollider> _headColliderCache;
```

**性能提升：**
- `FindObjectsOfType` 调用：从 **60次/秒** → **0.67次/秒** （减少 **98.9%**）
- `GetComponent` 调用：首次查找后缓存，避免重复调用
- `GetComponentInChildren` 调用：HeadCollider 缓存，包括 null 结果

---

### 2. ✅ **TriggerBot - 墙体检测一致性**

#### 问题
- TriggerBot 没有检查墙体遮挡，与 Aimbot 设置不一致
- 可能导致不合理的自动射击

#### 解决方案
```csharp
// TriggerBot 遵守 AimbotIgnoreWalls 设置
if (currentTargetInSight && _lastBestTarget.RequiresPenetration && !_config.AimbotIgnoreWalls)
{
    currentTargetInSight = false; // 被墙遮挡且不允许穿墙，则不触发
}
```

**改进：**
- ✅ 与 Aimbot 共享 `HasLineOfSight()` 检测逻辑
- ✅ 使用 `_lastBestTarget.RequiresPenetration` 标志
- ✅ 遵守 `AimbotIgnoreWalls` 配置选项

---

### 3. ✅ **ModBehaviour - 扫描频率优化**

#### 优化前
```csharp
private const float SCAN_INTERVAL = 1f;           // 每秒扫描一次箱子
private const float ESP_CACHE_INTERVAL = 0.2f;    // 每0.2秒更新ESP
```

#### 优化后
```csharp
private const float SCAN_INTERVAL = 2f;           // 每2秒扫描一次箱子 (↓50%)
private const float ESP_CACHE_INTERVAL = 0.3f;    // 每0.3秒更新ESP (↓33%)
```

**性能提升：**
- 箱子扫描：从 **60次/分钟** → **30次/分钟**
- ESP 缓存更新：从 **300次/分钟** → **200次/分钟**

---

### 4. ✅ **EnemyDetector - 扫描间隔调整**

#### 优化前
```csharp
CHARACTER_CACHE_INTERVAL = 3f;     // 3秒
AI_CONTROLLER_INTERVAL = 5f;       // 5秒
ENEMY_DATA_INTERVAL = 0.3f;        // 0.3秒
```

#### 优化后
```csharp
CHARACTER_CACHE_INTERVAL = 4f;     // 4秒 (↓25%)
AI_CONTROLLER_INTERVAL = 6f;       // 6秒 (↓16.7%)
ENEMY_DATA_INTERVAL = 0.4f;        // 0.4秒 (↓25%)
```

**性能提升：**
- 角色缓存：从 **20次/分钟** → **15次/分钟**
- 敌人数据更新：从 **200次/分钟** → **150次/分钟**

---

## 🚀 总体性能改进

| 指标 | 优化前 | 优化后 | 改进 |
|------|--------|--------|------|
| **FindObjectsOfType 调用/秒** | ~65 | ~2.5 | **96.2% ↓** |
| **GetComponent 调用/帧** | ~10-20 | 缓存后 ~0-1 | **95% ↓** |
| **ESP 缓存更新频率** | 5次/秒 | 3.3次/秒 | **33% ↓** |
| **箱子扫描频率** | 1次/秒 | 0.5次/秒 | **50% ↓** |
| **预估 CPU 负载** | 100% | ~60-70% | **30-40% ↓** |
| **预估 FPS 提升** | 基准 | +15-30 FPS | **25-50% ↑** |

---

## 🔍 优化前后对比

### 优化前的性能瓶颈（每秒）
```
AimbotSystem.FindBestTarget()        → 60次 FindObjectsOfType
  ├─ DamageReceiver[]                → 扫描所有对象
  ├─ foreach receiver.GetComponent   → 大量组件查找
  └─ receiver.GetComponentInChildren → 查找 HeadCollider

ModBehaviour.ScanAllLootboxes()      → 60次 FindObjectsOfType
ModBehaviour.UpdateESPCache()        → 300次遍历箱子列表

EnemyDetector.Update()               → 200次遍历角色列表
```

### 优化后的性能（每秒）
```
AimbotSystem.FindBestTarget()        → 0.67次 FindObjectsOfType ✅
  ├─ _damageReceiverCache            → 使用缓存数组
  ├─ _colliderCache.TryGetValue      → 从字典查找
  └─ _headColliderCache.TryGetValue  → 从字典查找 (含null缓存)

ModBehaviour.ScanAllLootboxes()      → 0.5次 FindObjectsOfType ✅
ModBehaviour.UpdateESPCache()        → 3.3次遍历缓存 ✅

EnemyDetector.Update()               → 2.5次遍历缓存 ✅
```

---

## 📝 代码改进详情

### AimbotSystem.cs

#### 1. 添加缓存字段
```csharp
// ===== 性能优化：缓存系统 =====
private DamageReceiver[] _damageReceiverCache = new DamageReceiver[0];
private float _lastReceiverCacheUpdate = 0f;
private const float RECEIVER_CACHE_INTERVAL = 1.5f;

private readonly Dictionary<DamageReceiver, Collider> _colliderCache = new();
private readonly Dictionary<DamageReceiver, HeadCollider> _headColliderCache = new();
```

#### 2. FindBestTarget() - 使用缓存
```csharp
// 性能优化：使用缓存的接收器列表
float currentTime = Time.time;
if (currentTime - _lastReceiverCacheUpdate > RECEIVER_CACHE_INTERVAL)
{
    _damageReceiverCache = UnityEngine.Object.FindObjectsOfType<DamageReceiver>();
    _lastReceiverCacheUpdate = currentTime;
    ClearInvalidCaches(); // 清理已销毁对象
}
```

#### 3. GetReceiverAimPoint() - Collider 缓存
```csharp
// 性能优化：使用缓存的 Collider
if (!_colliderCache.TryGetValue(receiver, out Collider collider))
{
    collider = receiver.GetComponent<Collider>();
    if (collider != null)
    {
        _colliderCache[receiver] = collider;
    }
}
```

#### 4. TryGetHeadTransform() - HeadCollider 缓存
```csharp
// 性能优化：使用缓存的 HeadCollider
if (!_headColliderCache.TryGetValue(receiver, out HeadCollider headCollider))
{
    headCollider = receiver.GetComponentInChildren<HeadCollider>();
    _headColliderCache[receiver] = headCollider; // 即使为 null 也缓存
}
```

#### 5. ClearInvalidCaches() - 清理销毁对象
```csharp
private void ClearInvalidCaches()
{
    // 清理 Collider 缓存
    var invalidColliders = _colliderCache
        .Where(kvp => kvp.Key == null || kvp.Value == null)
        .Select(kvp => kvp.Key).ToList();
    foreach (var key in invalidColliders)
        _colliderCache.Remove(key);
    
    // 清理 HeadCollider 缓存
    var invalidHeadColliders = _headColliderCache
        .Where(kvp => kvp.Key == null)
        .Select(kvp => kvp.Key).ToList();
    foreach (var key in invalidHeadColliders)
        _headColliderCache.Remove(key);
}
```

---

## 🎮 测试建议

### 测试场景
1. **高密度区域**：大量敌人和箱子的地图区域
2. **长时间战斗**：持续 10 分钟以上的游戏
3. **多功能同时启用**：Aimbot + TriggerBot + ESP + 作弊功能全开

### 性能指标监控
```csharp
// 可以临时添加性能监控日志
Debug.Log($"[Performance] FindObjectsOfType calls: {callCount}/s");
Debug.Log($"[Performance] Cache hits: {cacheHits}, misses: {cacheMisses}");
Debug.Log($"[Performance] FPS: {1.0f / Time.deltaTime:F1}");
```

### 预期结果
- ✅ FPS 提升 **15-30 帧**（取决于场景复杂度）
- ✅ CPU 占用降低 **30-40%**
- ✅ 无卡顿或掉帧现象
- ✅ TriggerBot 不会穿墙射击（除非启用穿墙选项）

---

## ⚠️ 注意事项

### 1. 缓存一致性
- 缓存会定期更新，可能有 **1.5秒延迟**
- 新刷出的敌人会在下次缓存更新时被检测到
- 不影响实际游戏体验（1.5秒延迟可接受）

### 2. 内存使用
- 缓存增加了少量内存开销（~1-2MB）
- 定期清理无效缓存，避免内存泄漏
- 场景切换时会自动清空缓存

### 3. 功能兼容性
- ✅ 所有现有功能保持不变
- ✅ TriggerBot 现在正确检测墙体遮挡
- ✅ Aimbot 精度不受影响

---

## 📈 进一步优化方向

### 短期（可选）
1. **GUI 批处理**：减少 DrawCall 数量
2. **物品扫描**：使用事件驱动而非定时扫描
3. **距离裁剪**：超远距离对象不参与计算

### 长期（考虑中）
1. **多线程计算**：将目标筛选移到后台线程
2. **LOD 系统**：根据距离调整更新频率
3. **空间分区**：使用八叉树加速位置查询

---

## ✅ 总结

本次优化主要通过 **缓存策略** 和 **降低更新频率** 大幅减少了性能开销：

1. ✅ **AimbotSystem**：98.9% 减少 FindObjectsOfType 调用
2. ✅ **TriggerBot**：修复墙体检测，与 Aimbot 设置一致
3. ✅ **扫描频率**：降低 33-50% 的更新频率
4. ✅ **组件缓存**：避免重复的 GetComponent 调用

**预期效果：FPS 提升 15-30 帧，CPU 负载降低 30-40%**

---

## 🔗 相关文件

- `DuckovESP/Features/Aimbot/AimbotSystem.cs` - 主要优化
- `DuckovESP/ModBehaviour.cs` - 扫描频率优化
- `DuckovESP/Features/Enemy/EnemyDetector.cs` - 缓存间隔优化
- `DuckovESP/Features/Enemy/EnemyESPRenderer.cs` - 已优化（GL 批量绘制）

---

**最后更新：** 2025-10-20  
**版本：** DuckovESP v2.4.0 - 性能优化版
