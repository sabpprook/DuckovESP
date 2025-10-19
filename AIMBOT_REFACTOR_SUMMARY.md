# 自动瞄准/自动扳机重构完成 ✅

## 改动总结

### 核心变更
已根据 **Mod_Aimbot** 的优秀实现完全重构了 `AimbotSystem.cs`，主要变更：

1. **从视角控制改为子弹重定向** 🎯
   - 旧版：移动 CameraArm 的 pitch/yaw（会让视角抖动）
   - 新版：在射击事件中直接修改 Projectile 的方向
   - 优势：玩家视角不动，但子弹自动飞向敌人，体验更自然

2. **使用 DamageReceiver 检测** 🎮
   - 旧版：只检测 CharacterMainControl
   - 新版：检测所有 DamageReceiver（覆盖更全面）

3. **改进的目标选择算法** 📊
   - 综合评分：`screenDistance + angle*2.25 + rayDistance*0.1`
   - 优先选择屏幕中心附近的敌人

4. **支持障碍物检测** 🧱
   - 使用 `Physics.Raycast` 检查视线
   - 新增配置：`AimbotIgnoreWalls`（是否穿墙）
   - 被遮挡的目标会降低优先级但不会被排除

5. **精确的头部检测** 🎯
   - 通过 `HeadCollider` / `HelmatSocket` / `FaceMaskSocket` 精确定位头部
   - 添加小随机偏移（±3cm）模拟真实瞄准

### 文件变更

#### 修改的文件
- ✅ `AimbotSystem.cs` - 完全重构（从 369 行变为约 750 行）
- ✅ `ESPConfig.cs` - 添加 `AimbotIgnoreWalls` 配置项
- ✅ `ESPConfigMenu.cs` - 添加"忽略墙壁"选项

#### 新增文件
- 📄 `AIMBOT_REFACTOR_NOTES.md` - 详细的重构说明文档

## 使用方法

### 1. 自动瞄准（Auto Aimbot）
```
1. 按 F5 打开配置菜单
2. 启用"启用自动瞄准"
3. 调整 FOV（建议 100-300 像素）
4. 启用"瞄准头部"（建议开启）
5. 射击时子弹会自动飞向敌人
```

**工作原理：**
- 你正常瞄准射击
- 系统在射击事件中自动重定向子弹
- **不改变你的视角**

### 2. 自动扳机（Trigger Bot）
```
1. 按 F5 打开配置菜单
2. 启用"启用自动扳机"
3. 设置触发延迟（建议 0.05-0.1 秒）
4. 将准星对准敌人
5. 系统会自动射击
```

### 3. 穿墙瞄准（可选）
```
1. 在配置菜单中勾选"忽略墙壁"
2. 现在可以对墙后的敌人射击
3. 被遮挡的目标优先级较低但仍会被瞄准
```

## 推荐配置

### 平衡模式（公平游戏）
```json
{
  "EnableAimbot": true,
  "AimbotFOV": 200.0,         // 中等范围
  "AimbotMaxDistance": 80.0,   // 限制距离
  "AimbotAimAtHead": true,     // 瞄准头部
  "AimbotIgnoreWalls": false,  // 不穿墙
  "EnableTriggerBot": false    // 手动射击
}
```

### 辅助模式（略微提高命中率）
```json
{
  "EnableAimbot": true,
  "AimbotFOV": 100.0,          // 小范围
  "AimbotMaxDistance": 50.0,   // 近距离
  "AimbotAimAtHead": false,    // 瞄准身体
  "AimbotIgnoreWalls": false,  
  "EnableTriggerBot": false
}
```

### 测试模式（验证功能）
```json
{
  "EnableAimbot": true,
  "AimbotFOV": 300.0,          // 大范围
  "AimbotMaxDistance": 200.0,  // 远距离
  "AimbotAimAtHead": true,
  "AimbotIgnoreWalls": true,   // 穿墙
  "EnableTriggerBot": true,
  "TriggerBotDelay": 0.05
}
```

## 测试步骤

1. **基础测试**
   ```
   ✓ 对准敌人射击
   ✓ 观察子弹是否自动命中
   ✓ 检查 Console 日志："DuckovESP Aimbot: 锁定目标..."
   ```

2. **头部瞄准测试**
   ```
   ✓ 启用"瞄准头部"
   ✓ 射击敌人
   ✓ 检查是否造成暴击伤害
   ```

3. **穿墙测试**
   ```
   ✓ 启用"忽略墙壁"
   ✓ 对墙后的敌人射击
   ✓ 验证子弹是否命中
   ```

4. **自动扳机测试**
   ```
   ✓ 启用自动扳机
   ✓ 将准星对准敌人（不按射击）
   ✓ 等待延迟时间后自动射击
   ```

## 已知问题

1. **反射警告**：使用反射获取 `_traveledDistance` 字段，编译时会有 `DynamicallyAccessedMembersAttribute` 警告（可忽略）

2. **层遮罩硬编码**：障碍物层名称（"Wall", "Ground", "HalfObsticle"）是硬编码的，如果游戏更新可能需要调整

## 下一步优化建议

1. 添加平滑度控制（虽然已经很平滑了）
2. 支持散弹枪多目标锁定
3. 自适应FOV（根据武器类型调整）
4. 添加延迟模拟（模拟人类反应时间）

## 对比表格

| 特性 | 旧版本 | 新版本（重构后） |
|------|--------|------------------|
| 瞄准方式 | ❌ 移动视角 | ✅ 重定向子弹 |
| 检测范围 | ❌ 仅角色 | ✅ 所有伤害接收器 |
| 目标选择 | ❌ 简单距离 | ✅ 综合评分 |
| 穿墙支持 | ❌ 无 | ✅ 可配置 |
| 头部检测 | ❌ 估算 | ✅ 精确获取 |
| 用户体验 | ❌ 视角抖动 | ✅ 无感知 |

## 参考资料
- 详细文档：`AIMBOT_REFACTOR_NOTES.md`
- 原始代码：`source_code_reference/Mod_Aimbot/CheatMenu/ModBehaviour.cs`

---

**注意：** 本功能仅供学习和研究使用，请在单机模式下测试，切勿在多人游戏中使用以免破坏游戏平衡！
