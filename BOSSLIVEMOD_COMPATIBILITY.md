# DuckovESP 与 BossLiveMapMod 兼容性说明

## 概述

DuckovESP 与 BossLiveMapMod 可以安全地同时使用。两个 mod 都使用小地图标记系统，但已实现完整的兼容性处理。

## 兼容性策略

### 功能分离
- **BossLiveMapMod**：在小地图上显示 Boss 位置（红色标记）
- **DuckovESP**：在小地图上显示箱子和物品位置（彩色标记）
- **结果**：两种标记互不干扰，可同时显示

### 技术实现

#### 1. 图标资源隔离
```csharp
// BossLiveMapMod 使用：
MapMarkerManager.Icons[2]  // Boss 专用图标

// DuckovESP 使用：
MapMarkerManager.Icons[5]  // 物品/宝箱图标（优先）
MapMarkerManager.Icons[7]  // 备用图标
MapMarkerManager.Icons[1]  // 备用图标
// 特意避开 Icons[2] 防止冲突
```

#### 2. 标记对象命名规范
```csharp
// BossLiveMapMod 创建：
"BossMarker:" + BossName

// DuckovESP 创建：
"LootboxMarker_" + instanceID
"WorldItemMarker_" + instanceID
```

#### 3. 安全的标记清理
```csharp
private void ClearAllMarkers()
{
    // 只清理我们自己创建的标记
    foreach (var marker in _trackedMarkers)
    {
        // 检查名称前缀确保是我们创建的
        if (marker.name.StartsWith("LootboxMarker_") || 
            marker.name.StartsWith("WorldItemMarker_"))
        {
            Destroy(marker);
        }
    }
}
```

#### 4. Mod 检测机制
```csharp
private void DetectBossLiveMapMod()
{
    // 在启动时检测 BossLiveMapMod 是否存在
    var assemblies = AppDomain.CurrentDomain.GetAssemblies();
    foreach (var assembly in assemblies)
    {
        if (assembly.GetName().Name.Contains("BossLiveMapMod"))
        {
            _bossLiveMapModDetected = true;
            Debug.Log("[DuckovESP] 检测到 BossLiveMapMod，启用兼容模式");
            break;
        }
    }
}
```

## 冲突点分析

### ✅ 已解决的潜在冲突

1. **图标资源冲突**
   - ❌ 问题：两个 mod 都使用 `MapMarkerManager.Icons`
   - ✅ 解决：DuckovESP 避开 BossLiveMapMod 使用的 icons[2]

2. **标记对象混淆**
   - ❌ 问题：可能错误删除对方的标记
   - ✅ 解决：使用不同的命名前缀，删除时检查名称

3. **场景切换清理**
   - ❌ 问题：关卡重新加载时可能清理对方的标记
   - ✅ 解决：只清理我们追踪的对象，通过名称验证

4. **性能影响**
   - ❌ 问题：两个 mod 都频繁扫描场景
   - ✅ 解决：
     - BossLiveMapMod：0.75秒扫描一次，只在地图打开时激活
     - DuckovESP：1秒扫描一次，始终运行
     - 总开销可接受

### ✅ 无冲突的设计

1. **数据存储**
   - BossLiveMapMod：`Dictionary<CharacterMainControl, SimplePointOfInterest>`
   - DuckovESP：`Dictionary<InteractableLootbox, LootboxMarkerData>`
   - 不共享数据结构

2. **事件监听**
   - BossLiveMapMod：监听 `Health.OnDead` 事件
   - DuckovESP：不监听该事件
   - 不干扰彼此的事件处理

3. **场景管理**
   - 两个 mod 都使用 `MultiSceneCore.MainScene`
   - 但创建的对象有不同的名称和引用
   - Unity 会正确管理所有对象

## 测试建议

### 测试场景
1. **同时启用**：安装两个 mod，进入游戏
2. **地图验证**：打开小地图，应该看到：
   - 红色 Boss 标记（来自 BossLiveMapMod）
   - 彩色物品/箱子标记（来自 DuckovESP）
3. **关卡切换**：切换关卡，验证标记正确清理和重建
4. **Boss 击杀**：击杀 Boss，验证 Boss 标记消失但物品标记保留

### 预期行为
```
[DuckovESP] 检测到 BossLiveMapMod，启用兼容模式
[BossLiveMapMod] live boss markers enabled
[DuckovESP] 已清理所有标记
[BossLiveMapMod] 已清理所有标记
[DuckovESP] 发现 15 个新的箱子标记
[BossLiveMapMod] 发现 2 个 Boss
```

## 最佳实践

### 对于用户
- ✅ 可以安全地同时安装两个 mod
- ✅ 不需要任何配置更改
- ✅ 如果遇到问题，检查日志中的兼容性消息

### 对于开发者
- ✅ 使用唯一的对象命名前缀
- ✅ 只清理自己创建的对象
- ✅ 避免使用对方已使用的图标索引
- ✅ 在日志中记录兼容性检测结果

## 性能考虑

### 扫描频率
| Mod | 扫描间隔 | 激活条件 | 性能影响 |
|-----|---------|---------|---------|
| BossLiveMapMod | 0.75秒 | 仅地图打开时 | 低 |
| DuckovESP | 1秒 | 始终运行 | 中等 |
| **总计** | - | - | **可接受** |

### 优化措施
- BossLiveMapMod 在地图关闭时完全停止扫描
- DuckovESP 使用缓存机制减少重复计算
- 两个 mod 都使用增量更新而非全量扫描

## 未来改进

### 可能的优化方向
1. **共享扫描机制**
   - 建立公共事件系统
   - 避免重复扫描同一场景

2. **统一配置界面**
   - 创建兼容性配置页面
   - 允许用户调整扫描频率

3. **API 标准化**
   - 定义小地图标记的标准接口
   - 允许更多 mod 安全地添加标记

## 总结

✅ **完全兼容**：DuckovESP 和 BossLiveMapMod 可以安全地同时使用

✅ **零配置**：用户无需任何设置即可享受两个 mod 的功能

✅ **性能良好**：两个 mod 的总体性能开销在可接受范围内

✅ **持续维护**：如果发现兼容性问题，会在后续版本中修复

## 版本信息

- **DuckovESP 版本**：v2.3.3+
- **BossLiveMapMod 版本**：所有版本
- **最后更新**：2025-10-20
