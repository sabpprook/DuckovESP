# 自动扳机更新 v2.4

## 概述

自动扳机（Trigger Bot）系统进行了重大改进，现在与自动瞄准系统深度集成，提供更可靠的自动射击体验。

## 主要改进

### 1. **使用自动瞄准的目标检测系统**

**之前的实现：**
- 自动扳机使用独立的射线检测（`GetTargetUnderCrosshair()`）
- 只检测准星正中心的目标
- 需要精确对准敌人才能触发

**现在的实现：**
```csharp
// 查找最佳目标（自动瞄准和自动扳机共享）
_lastBestTarget = (_config.EnableAimbot || _config.EnableTriggerBot) 
    ? FindBestTarget() 
    : default(AutoAimCandidate);

// 自动扳机使用自动瞄准找到的目标
bool currentTargetInSight = (_lastBestTarget.Receiver != null);
```

**优势：**
- ✅ 使用自动瞄准的智能目标选择
- ✅ 范围内有敌人就能触发（不需要准星对准）
- ✅ 享受自动瞄准的所有过滤逻辑（距离、视线、队友检查等）
- ✅ 性能更好（共享目标扫描，避免重复计算）

### 2. **强制依赖自动瞄准**

**原因：**
- 自动扳机使用自动瞄准的目标检测系统
- 单独启用自动扳机没有意义
- 统一配置，避免用户困惑

**实现：**
```csharp
// 自动扳机（必须在自动瞄准启用时才工作）
if (_config.EnableTriggerBot && _config.EnableAimbot)
{
    PerformTriggerBot();
}
```

**配置UI改进：**
```csharp
// 如果启用自动扳机但自动瞄准未启用，显示警告
if (_config.EnableTriggerBot && !_config.EnableAimbot)
{
    GUILayout.Label("⚠️ 警告: 自动扳机需要启用自动瞄准才能工作！");
    
    // 自动启用自动瞄准
    if (!originalTriggerBot && _config.EnableTriggerBot)
    {
        _config.EnableAimbot = true;
        GUILayout.Label("✓ 已自动启用自动瞄准");
    }
}
```

### 3. **改进的诊断日志**

添加了详细的状态日志，帮助调试：

```csharp
// 首次启动日志
Debug.Log($"[TriggerBot] 已启动 - 仅瞄准触发: {_config.TriggerBotOnlyADS}, 延迟: {_config.TriggerBotDelay}s");
Debug.Log($"[TriggerBot] 使用自动瞄准的目标检测系统");

// 检测到目标
Debug.Log($"[TriggerBot] 检测到目标: {_lastBestTarget.Receiver.name}, 距离: {_lastBestTarget.RayDistance:F1}m");

// 目标消失
Debug.Log("[TriggerBot] 目标消失");
```

## 工作流程

```
用户操作
    ↓
启用自动瞄准 (EnableAimbot = true)
    ↓
启用自动扳机 (EnableTriggerBot = true)
    ↓
每帧 Update() 调用
    ↓
FindBestTarget() ← 自动瞄准和自动扳机共享
    ↓
找到最佳目标 (_lastBestTarget)
    ↓
    ├─→ 自动瞄准: 使用目标重定向子弹
    └─→ 自动扳机: 检测目标并自动开火
            ↓
        目标在范围内?
            ├─ 是 → 延迟触发 → TryDirectShoot()
            └─ 否 → ReleaseTrigger()
```

## 配置说明

### 必需配置
```csharp
// 1. 必须先启用自动瞄准
EnableAimbot = true;

// 2. 然后启用自动扳机
EnableTriggerBot = true;
```

### 可选配置
```csharp
// 触发延迟（秒）- 检测到目标后延迟多久开火
TriggerBotDelay = 0.05f;  // 推荐 0.02-0.1秒

// 仅瞄准时触发（需要ADS）
TriggerBotOnlyADS = true;  // TODO: 需要实现ADS检测

// 队友检查
TriggerBotTeamCheck = true;  // 避免误伤队友
```

### 自动瞄准配置影响自动扳机
```csharp
// 这些设置会影响自动扳机的触发范围
AimbotMaxDistance = 200f;  // 最大瞄准距离
AimbotIgnoreWalls = false;  // 是否忽略墙壁
// ... 其他自动瞄准设置
```

## 使用方法

### 1. 基本使用
```
1. 进入游戏
2. 按 F6 打开配置菜单
3. 启用 "启用自动瞄准（Aimbot）"
4. 启用 "启用自动扳机"
5. 调整触发延迟（可选）
6. 保存配置
7. 装备武器，靠近敌人
8. 自动瞄准找到目标后，自动扳机会自动开火
```

### 2. 自动启用功能
如果你只勾选了自动扳机：
```
✅ 系统会自动启用自动瞄准
✅ 配置菜单会显示提示信息
✅ 无需手动操作
```

### 3. 调试模式
查看日志输出（`BepInEx/LogOutput.log`）：
```
[TriggerBot] 已启动 - 仅瞄准触发: False, 延迟: 0.05s
[TriggerBot] 使用自动瞄准的目标检测系统
[TriggerBot] 检测到目标: Enemy_Guard, 距离: 15.3m
[TriggerBot] 尝试直接射击
[TriggerBot] 使用 SetTrigger() 方法
[TriggerBot] 目标消失
```

## 性能优化

### 共享目标扫描
- **之前**: 自动瞄准扫描一次 + 自动扳机扫描一次 = **2次扫描**
- **现在**: 共享扫描结果 = **1次扫描**
- **性能提升**: ~50% CPU开销降低

### 条件执行
```csharp
// 只在需要时扫描目标
_lastBestTarget = (_config.EnableAimbot || _config.EnableTriggerBot) 
    ? FindBestTarget() 
    : default(AutoAimCandidate);
```

## 故障排除

### 问题1: 自动扳机没有反应
**检查清单:**
1. ✅ 是否启用了自动瞄准？
2. ✅ 是否启用了自动扳机？
3. ✅ 是否持有武器？
4. ✅ 是否在敌人附近（距离 < AimbotMaxDistance）？
5. ✅ 是否有视线（没有被墙挡住）？

**日志检查:**
```
如果看到: "[TriggerBot] 已启动"
说明: 自动扳机正在运行 ✅

如果看到: "[TriggerBot] 检测到目标"
说明: 目标检测正常 ✅

如果没有任何日志:
问题: 自动扳机未启动或自动瞄准未启用 ❌
```

### 问题2: 自动扳机触发但不射击
**可能原因:**
1. `SetTrigger()` 方法不存在或签名错误
2. 需要其他射击方法（Shoot, Fire, OnShoot）
3. 需要输入模拟而非直接调用

**解决方案:**
查看日志中的 `ListGunMethods()` 输出，确认可用方法：
```
=== ItemAgent_Gun 可用方法列表 ===
  Void Shoot()  ← 如果有这个方法，会自动尝试
  Void Fire()   ← 备选方法
  ...
================================
```

### 问题3: 误伤队友
**解决方案:**
```csharp
// 启用队友检查
TriggerBotTeamCheck = true;
```

## 未来改进计划

### 1. ADS检测实现
```csharp
// TODO: 实现瞄准状态检测
if (_config.TriggerBotOnlyADS)
{
    bool isADS = CheckADSState();  // 需要实现
    if (!isADS) return;
}
```

### 2. 更智能的射击方法
```csharp
// 根据武器类型选择最佳射击方式
- 全自动: 持续按住扳机
- 半自动: 快速点击
- 霰弹枪: 单发射击
```

### 3. 射击模式优化
```csharp
// 根据距离调整射击模式
- 近距离: 疯狂射击
- 中距离: 控制射速
- 远距离: 单发点射
```

## 技术细节

### 目标检测共享
```csharp
private AutoAimCandidate _lastBestTarget;  // 缓存最佳目标

// Update()中只扫描一次
_lastBestTarget = FindBestTarget();

// 自动瞄准使用
if (candidate.Receiver != null)
{
    RetargetProjectilesToTarget(muzzlePosition, candidate.AimPoint);
}

// 自动扳机使用
bool currentTargetInSight = (_lastBestTarget.Receiver != null);
if (currentTargetInSight && _triggerDelayTimer <= 0)
{
    TryDirectShoot();
}
```

### 状态管理
```csharp
// 禁用时清理状态
if (!_config.EnableTriggerBot || !_config.EnableAimbot)
{
    if (_lastTriggerState)
    {
        ReleaseTrigger();  // 释放扳机
    }
    
    _targetInSight = false;      // 重置目标状态
    _triggerDelayTimer = 0f;      // 重置延迟计时器
}
```

## 对比表

| 功能 | v2.3 (旧版) | v2.4 (新版) |
|------|-------------|-------------|
| **目标检测** | 独立射线检测 | 使用自动瞄准系统 |
| **触发条件** | 准星正中心 | 自动瞄准范围内 |
| **依赖关系** | 独立运行 | 必须启用自动瞄准 |
| **性能开销** | 2次目标扫描 | 1次目标扫描（共享） |
| **配置复杂度** | 独立配置 | 简化配置 |
| **可靠性** | 中等 | 高（使用成熟的瞄准系统） |
| **调试信息** | 基础日志 | 详细诊断日志 |
| **用户体验** | 需要精确瞄准 | 更宽松的触发范围 |

## 版本信息

- **版本**: v2.4
- **更新日期**: 2025-10-20
- **作者**: MadLongTom
- **状态**: ✅ 编译成功，待测试

## 总结

✅ **自动扳机现在更可靠**：使用自动瞄准的成熟目标检测系统

✅ **性能更好**：共享目标扫描，减少50%开销

✅ **配置更简单**：自动启用依赖项，避免配置错误

✅ **触发范围更大**：不需要准星精确对准，自动瞄准范围内即可触发

✅ **更好的调试支持**：详细日志帮助诊断问题

⚠️ **注意**: 自动扳机现在必须与自动瞄准一起使用！
