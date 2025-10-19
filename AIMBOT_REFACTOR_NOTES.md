# Aimbot 系统重构说明

## 重构日期
2025年

## 重构原因
原来的自动瞄准系统不好用，根据 `Mod_Aimbot` 中的优秀实现进行了完全重构。

## 核心设计变更

### 1. **从视角控制改为子弹重定向**
**之前的方式（不好用）：**
- 通过控制 `CameraArm.pitch` 和 `CameraArm.yaw` 来移动视角
- 玩家会看到视角自动移动到敌人身上
- 体验不自然，容易被发现

**现在的方式（更好）：**
- 订阅枪械的 `OnShootEvent` 事件
- 在射击后立即重定向子弹轨迹
- **玩家视角不变**，但子弹会自动飞向敌人
- 实现了"看哪打哪"的效果，更加自然

### 2. **使用 DamageReceiver 而非 CharacterMainControl**
**之前：** 只能检测 `CharacterMainControl`（角色控制器）
**现在：** 检测所有 `DamageReceiver`（伤害接收器），包括：
- 玩家角色
- AI敌人
- 可破坏物体（如果需要）
- 更全面的目标检测

### 3. **改进的目标选择算法**
基于 Mod_Aimbot 的评分系统：
```
score = screenDistance + (angle * 2.25) + (rayDistance * 0.1)
```
- 主要考虑：屏幕距离（与准星的距离）
- 次要考虑：角度偏差、实际距离
- 如果被遮挡还会增加惩罚

### 4. **视线检测与穿墙支持**
- 使用 `Physics.Raycast` 检测是否有障碍物
- 如果启用了 `AimbotIgnoreWalls` 配置，可以穿墙瞄准
- 被遮挡的目标会降低优先级但不会被排除

### 5. **精确的头部检测**
- 尝试获取 `HeadCollider` 组件
- 通过 `CharacterModel.HelmatSocket` 或 `FaceMaskSocket` 定位头部
- 添加小随机偏移（±3厘米）模拟真实瞄准

## 新增配置项

在 `ESPConfig` 中：
```csharp
// 建议添加这个配置：
public bool AimbotIgnoreWalls = false; // 是否忽略墙壁（穿墙）
```

## 工作流程

### 自动瞄准（Aimbot）
1. 玩家射击 → 触发 `OnGunShoot` 事件
2. 查找屏幕范围内的最佳目标
3. 重定向刚发射的子弹（飞行距离 < 0.06米）
4. 子弹轨迹被修改，自动飞向目标

### 自动扳机（TriggerBot）
1. 每帧检测准星下是否有敌人
2. 如果检测到敌人，等待延迟时间
3. 延迟结束后自动射击

## 关键代码位置

### 子弹重定向
```csharp
private void OnGunShoot()
{
    // 查找目标
    AutoAimCandidate candidate = FindBestTarget();
    
    // 重定向子弹
    RetargetProjectilesToTarget(muzzlePosition, candidate.AimPoint);
}
```

### 子弹修改
```csharp
private bool RetargetProjectile(Projectile projectile, Vector3 direction, Vector3 targetPoint)
{
    projectile.context.direction = direction;  // 修改方向
    projectile.context.distance = distanceToTarget;  // 确保射程足够
    projectile.context.critRate = 1f;  // 强制暴击（头部）
    projectile.transform.rotation = Quaternion.LookRotation(direction);
}
```

## 性能优化

1. **子弹追踪清理**：使用 `HashSet<int>` 记录已处理的子弹ID，定期清理已销毁的记录
2. **早期退出**：多层有效性检查，不合格的目标立即跳过
3. **缓存机制**：枪械引用、摄像机引用只获取一次

## 测试建议

1. **基础测试**：
   - 开启自瞄，射击屏幕中心附近的敌人
   - 验证子弹是否自动命中

2. **穿墙测试**：
   - 启用 `AimbotIgnoreWalls`
   - 对墙后的敌人射击
   - 验证是否能穿墙命中

3. **自动扳机测试**：
   - 将准星对准敌人（不射击）
   - 验证是否在延迟后自动射击

4. **头部瞄准测试**：
   - 启用 `AimbotAimAtHead`
   - 验证是否造成暴击伤害

## 已知限制

1. **反射获取子弹距离**：使用反射获取 `_traveledDistance` 私有字段，未来版本可能失效
2. **障碍物层遮罩**：硬编码了 "Wall"、"Ground"、"HalfObsticle" 层名称
3. **无预测功能**：移除了旧版本的移动预测功能（实际效果不明显）

## 配置建议

```json
{
  "EnableAimbot": true,           // 启用自动瞄准
  "EnableTriggerBot": true,       // 启用自动扳机
  "AimbotFOV": 300.0,             // 屏幕半径（像素）
  "AimbotMaxDistance": 80.0,      // 最大距离（米）
  "AimbotAimAtHead": true,        // 瞄准头部
  "AimbotIgnoreWalls": false,     // 不穿墙（公平游戏）
  "TriggerBotDelay": 0.1,         // 延迟0.1秒（更自然）
  "TriggerBotTeamCheck": true,    // 检查队伍（不打队友）
  "TriggerBotOnlyADS": false      // 任何状态都触发
}
```

## 相比原版的优势

| 特性 | 旧版本 | 新版本（重构后） |
|------|--------|------------------|
| 瞄准方式 | 移动视角 | 重定向子弹 |
| 检测范围 | 仅 CharacterMainControl | 所有 DamageReceiver |
| 目标选择 | 简单距离 | 综合评分系统 |
| 穿墙支持 | 无 | 支持（可配置） |
| 头部检测 | 估算 | 精确获取头部位置 |
| 用户体验 | 视角抖动 | 无感知，自然 |
| 代码质量 | 基础实现 | 参考优秀开源项目 |

## 未来改进方向

1. **添加平滑度控制**：虽然现在是瞬间重定向，但可以添加轻微的随机偏移模拟不完美瞄准
2. **支持多目标锁定**：如果是散弹枪，可以让每颗子弹锁定不同的目标
3. **添加延迟模拟**：在重定向前添加微小延迟，模拟人类反应时间
4. **自适应FOV**：根据武器类型自动调整检测范围（狙击枪 > 步枪 > 手枪）

## 参考资料
- 原始实现：`source_code_reference/Mod_Aimbot/CheatMenu/ModBehaviour.cs`
- 关键方法：`TryAutoAimShot()`, `RetargetProjectile()`, `FindBestTarget()`
