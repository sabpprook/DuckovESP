# TriggerBot 墙体检测调试指南

## 🔍 问题现象
TriggerBot 在有墙壁的情况下仍然开枪

---

## ✅ 已实施的修复

### 1. **双重检查机制**
```csharp
// 第一层：遵守 AimbotIgnoreWalls 设置
if (currentTargetInSight && _lastBestTarget.RequiresPenetration && !_config.AimbotIgnoreWalls)
{
    Debug.Log($"[TriggerBot] 目标被墙遮挡，不触发");
    currentTargetInSight = false;
}

// 第二层：TriggerBot 永远不穿墙射击（额外安全保护）
if (currentTargetInSight && _lastBestTarget.RequiresPenetration)
{
    Debug.Log($"[TriggerBot] 目标被墙遮挡，TriggerBot 不穿墙射击");
    currentTargetInSight = false;
}
```

### 2. **改进障碍物层检测**
```csharp
// 初始化时输出日志
Debug.Log($"[Aimbot] 添加 Wall 层 (Layer {wallLayer})");
Debug.Log($"[Aimbot] 添加 Ground 层 (Layer {groundLayer})");
Debug.Log($"[Aimbot] 添加 HalfObsticle 层 (Layer {halfObstacleLayer})");

// 如果没有找到层，使用默认遮罩
if (ObstacleLayerMaskValue == 0)
{
    Debug.LogWarning("[Aimbot] ⚠️ 没有找到任何障碍物层！使用默认层遮罩");
    ObstacleLayerMaskValue = ~(1 << LayerMask.NameToLayer("Ignore Raycast"));
}
```

### 3. **详细的视线检测日志**
```csharp
// 在 HasLineOfSight 中可以启用详细日志（已注释）
// Debug.Log($"[Aimbot] 目标被遮挡: {hit.collider.name} (Layer: {LayerMask.LayerToName(hit.collider.gameObject.layer)}, 距离: {hit.distance:F2}m)");
```

---

## 🎮 测试步骤

### 步骤 1：启动游戏并查看初始化日志
```
期望看到：
[Aimbot] 添加 Wall 层 (Layer X)
[Aimbot] 添加 Ground 层 (Layer Y)
[Aimbot] 障碍物层遮罩初始化完成: 0xXXXX
[TriggerBot] 已启动 - 仅瞄准触发: True/False
```

**如果看到：**
```
⚠️ [Aimbot] 没有找到任何障碍物层！使用默认层遮罩
```
说明游戏的层名称可能不同，但会使用默认遮罩（应该也能工作）

---

### 步骤 2：站在墙后测试
1. 找一堵墙，站在墙的一侧
2. 确保敌人在墙的另一侧
3. 启用 **Aimbot** 和 **TriggerBot**
4. **不要** 启用 "Aimbot 忽略墙壁"

**期望行为：**
- Aimbot 不会锁定墙后的敌人
- TriggerBot 不会自动开火
- 控制台日志：无目标检测日志

---

### 步骤 3：启用穿墙测试（验证双重保护）
1. 保持墙后有敌人的场景
2. 在配置中启用 **"Aimbot 忽略墙壁"**
3. 观察行为

**期望行为：**
- Aimbot **会** 锁定墙后的敌人（准星跟随）
- TriggerBot **不会** 自动开火（双重保护）
- 控制台日志应该显示：
  ```
  [TriggerBot] 目标被墙遮挡，不触发
  [TriggerBot] 目标被墙遮挡，TriggerBot 不穿墙射击
  ```

---

### 步骤 4：无遮挡测试
1. 站在开阔地，确保和敌人之间无障碍物
2. 启用 TriggerBot

**期望行为：**
- TriggerBot 检测到目标并自动开火
- 控制台日志：
  ```
  [TriggerBot] 检测到目标: XXX, 距离: X.Xm [无遮挡]
  ```

---

## 🐛 如果问题依然存在

### 诊断步骤 1：启用详细日志
在 `HasLineOfSight` 方法中取消注释这一行：
```csharp
// 找到这行（约 line 675）
Debug.Log($"[Aimbot] 目标被遮挡: {hit.collider.name} (Layer: {LayerMask.LayerToName(hit.collider.gameObject.layer)}, 距离: {hit.distance:F2}m)");
```

**重新编译并测试**，查看是否输出遮挡信息。

---

### 诊断步骤 2：检查层遮罩值
在游戏中按 `Shift + F5` 打开配置菜单，查看控制台日志中的：
```
[Aimbot] 障碍物层遮罩初始化完成: 0xXXXX
```

**如果值为 0x0**：说明没有找到任何障碍物层
**如果值为 0xFFFFFFFF**：说明使用了默认遮罩（所有层）

---

### 诊断步骤 3：检查 RequiresPenetration 标志
添加调试日志确认标志是否正确设置：

在 `EvaluateCandidate` 方法中添加（约 line 618）：
```csharp
Debug.Log($"[Aimbot] 候选目标: {receiver.name}, hasLineOfSight={hasLineOfSight}, RequiresPenetration={!hasLineOfSight}");
```

然后在墙后测试，查看日志输出。

---

### 诊断步骤 4：验证 Raycast 是否工作
临时修改 `HasLineOfSight` 方法（约 line 650）：
```csharp
// 射线检测
RaycastHit hit;
bool hitSomething = Physics.Raycast(origin, direction, out hit, distance - 0.2f, ObstacleLayerMaskValue, QueryTriggerInteraction.Ignore);

Debug.Log($"[Aimbot] Raycast: hitSomething={hitSomething}, LayerMask=0x{ObstacleLayerMaskValue:X}, distance={distance:F2}");

if (!hitSomething)
{
    Debug.Log($"[Aimbot] 无遮挡 → 视线通畅");
    return true;
}

Debug.Log($"[Aimbot] 碰撞: {hit.collider.name}, Layer={hit.collider.gameObject.layer}, 距离={hit.distance:F2}m");
```

---

## 📋 可能的原因和解决方案

### 原因 1：障碍物层名称不匹配
**症状：** 日志显示 "没有找到任何障碍物层"

**解决方案：**
1. 检查游戏的实际层名称（可能不是 "Wall", "Ground", "HalfObsticle"）
2. 修改 `AimbotSystem` 构造函数中的层名称
3. 或者保持使用默认遮罩（应该也能工作）

---

### 原因 2：Raycast 距离不足
**症状：** 近距离可以检测墙壁，远距离不行

**解决方案：**
检查 `distance - 0.2f` 是否太短，可以改为：
```csharp
Physics.Raycast(origin, direction, out hit, distance * 1.1f, ObstacleLayerMaskValue, QueryTriggerInteraction.Ignore)
```

---

### 原因 3：QueryTriggerInteraction 设置问题
**症状：** 某些墙壁（如玻璃）无法检测

**解决方案：**
尝试改为 `QueryTriggerInteraction.Collide`：
```csharp
Physics.Raycast(origin, direction, out hit, distance - 0.2f, ObstacleLayerMaskValue, QueryTriggerInteraction.Collide)
```

---

### 原因 4：目标的 Collider 和 DamageReceiver 层级问题
**症状：** 射线直接命中了敌人的 Collider，误判为"视线通畅"

**解决方案：**
检查 `GetComponentInParent<DamageReceiver>()` 是否正确找到目标：
```csharp
DamageReceiver hitReceiver = hit.collider.GetComponentInParent<DamageReceiver>();
Debug.Log($"[Aimbot] hitReceiver={hitReceiver?.name}, target={receiver?.name}, match={hitReceiver == receiver}");
```

---

## 🔧 临时禁用 TriggerBot 穿墙（如果以上都不行）

最简单的解决方案：**让 TriggerBot 永远不穿墙**

找到 `PerformTriggerBot` 方法（约 line 870），确保这段代码存在：
```csharp
// 额外检查：即使配置允许穿墙，TriggerBot 默认也不穿墙射击
if (currentTargetInSight && _lastBestTarget.RequiresPenetration)
{
    Debug.Log($"[TriggerBot] 目标被墙遮挡，TriggerBot 不穿墙射击");
    currentTargetInSight = false;
}
```

这段代码 **已经添加**，应该能阻止所有穿墙射击。

---

## 📊 测试结果记录

请运行测试并记录以下信息：

1. **初始化日志**
   ```
   [ ] 看到 "添加 Wall 层"
   [ ] 看到 "添加 Ground 层"
   [ ] 看到 "障碍物层遮罩初始化完成"
   [ ] 遮罩值: 0x________
   ```

2. **墙后测试（不启用穿墙）**
   ```
   [ ] Aimbot 不锁定墙后敌人
   [ ] TriggerBot 不开火
   [ ] 控制台无 "[TriggerBot] 检测到目标" 日志
   ```

3. **墙后测试（启用穿墙）**
   ```
   [ ] Aimbot 锁定墙后敌人
   [ ] TriggerBot **不** 开火
   [ ] 控制台显示 "[TriggerBot] 目标被墙遮挡" 日志
   ```

4. **无遮挡测试**
   ```
   [ ] TriggerBot 正常开火
   [ ] 控制台显示 "[TriggerBot] 检测到目标: XXX [无遮挡]"
   ```

---

## 💡 建议

如果上述测试都通过，但你仍然遇到问题，可能是：
- 特定场景的墙壁使用了不同的层
- 某些障碍物是 Trigger Collider（不会被 Raycast 检测到）
- 游戏的物理系统有特殊设置

请提供：
1. 完整的控制台日志
2. 发生问题时的场景描述
3. 是否启用了 "Aimbot 忽略墙壁" 选项
