# v3 硬编码字符串国际化完整扫描报告

## 概述

通过全面扫描 DuckovESPv3 代码库（排除 Log 和注释），发现大量硬编码的用户界面字符串。本文档记录所有发现和修复方案。

## 一、发现的硬编码字符串分类

### 1. ConfigWindow.cs 中的硬编码字符串

| 字符串 | 出现位置 | 推荐键名 |
|--------|--------|--------|
| "  ✗ 可能被3D物体遮挡" | 300行 | `UI.Label.OccludedWarning` |
| "【屏幕空间】" | 310行 | `UI.Label.ScreenSpaceMode` |
| "  ✓ 永不遮挡（完全可见）" | 311行 | `UI.Label.NoOcclusion` |
| "  ✗ 降低约30%帧数" | 312行 | `UI.Label.PerformanceWarning` |
| "【物资ESP】 - 箱子和地面物品" | 330行 | `UI.Label.LootESPTitle` |
| "【敌人ESP】 - 玩家和NPC" | 359行 | `UI.Label.EnemyESPTitle` |
| "显示信息:" | 372行 | `UI.Label.DisplayInfo` |
| "显示任务地点标记" | 413行 | `UI.Toggle.ShowQuestZones` |
| "任务地点最大距离: {distance}m" | 416行 | `UI.Label.QuestZoneMaxDistance` |
| "任务地点颜色:" | 420行 | `UI.Label.QuestZoneColor` |
| "显示撤离点指示器（绿色箭头）" | 432行 | `UI.Toggle.ShowEvacuationPoints` |
| "基础过滤:" | 438行 | `UI.Label.BasicFilters` |
| "显示条件 (满足任意一项即显示):" | 444行 | `UI.Label.DisplayConditions` |
| "按价值显示 (>=最小品质)" | 446行 | `UI.Toggle.ShowByValue` |
| "显示心愿单物品" | 447行 | `UI.Toggle.ShowWishlistedItems` |
| "显示任务物品" | 448行 | `UI.Toggle.ShowQuestItems` |
| "显示建筑材料" | 449行 | `UI.Toggle.ShowBuildingMaterials` |
| "任务物品颜色:" | 454行 | `UI.Label.QuestItemColor` |
| `RGB({r}, {g}, {b})` | 461行 | `UI.Label.RGBFormat` |
| "钥匙过滤:" | 465行 | `UI.Label.KeyFilters` |
| "显示未录入的钥匙" | 466行 | `UI.Toggle.ShowUnregisteredKeys` |

### 2. CheatStatusOverlay.cs 中的硬编码字符串

| 字符串 | 出现位置 | 推荐键名 |
|--------|--------|--------|
| "DuckovESP 作弊模块状态" | 标题 | `UI.CheatOverlay.Title` |
| "启用" / "禁用" | 状态显示 | `UI.Status.Enabled` / `UI.Status.Disabled` |

### 3. WelcomePopup.cs 中的硬编码字符串

已经国际化，使用 `LocalizationManager.Get("Welcome.*")` 调用。

## 二、修复方案

### 步骤 1: 更新翻译文件（zh-CN.json）

添加新的翻译键值对到 `UI.Label` 和 `UI.Toggle` 部分。

### 步骤 2: 修改源代码

将硬编码字符串替换为 `LocalizationManager.Get()` 调用。

### 步骤 3: 编译和验证

确保所有引用都正确解析。

## 三、实现细节

### 添加到 zh-CN.json 的新键值

```json
{
  "UI": {
    "Label": {
      "OccludedWarning": "  ✗ 可能被3D物体遮挡",
      "ScreenSpaceMode": "【屏幕空间】",
      "NoOcclusion": "  ✓ 永不遮挡（完全可见）",
      "PerformanceWarning": "  ✗ 降低约30%帧数",
      "LootESPTitle": "【物资ESP】 - 箱子和地面物品",
      "EnemyESPTitle": "【敌人ESP】 - 玩家和NPC",
      "DisplayInfo": "显示信息:",
      "QuestZoneMaxDistance": "任务地点最大距离: {value}m",
      "QuestZoneColor": "任务地点颜色:",
      "BasicFilters": "基础过滤:",
      "DisplayConditions": "显示条件 (满足任意一项即显示):",
      "QuestItemColor": "任务物品颜色:",
      "RGBFormat": "RGB({r}, {g}, {b})",
      "KeyFilters": "钥匙过滤:"
    },
    "Toggle": {
      "ShowQuestZones": "显示任务地点标记",
      "ShowEvacuationPoints": "显示撤离点指示器（绿色箭头）",
      "ShowByValue": "按价值显示 (>=最小品质)",
      "ShowWishlistedItems": "显示心愿单物品",
      "ShowQuestItems": "显示任务物品",
      "ShowBuildingMaterials": "显示建筑材料",
      "ShowUnregisteredKeys": "显示未录入的钥匙"
    },
    "Status": {
      "Enabled": "启用",
      "Disabled": "禁用"
    },
    "CheatOverlay": {
      "Title": "DuckovESP 作弊模块状态"
    }
  }
}
```

### 代码修改示例

**之前**:
```csharp
GUILayout.Label("  ✗ 可能被3D物体遮挡", _styleManager.LabelStyle);
```

**之后**:
```csharp
GUILayout.Label(LocalizationManager.Get("UI.Label.OccludedWarning"), _styleManager.LabelStyle);
```

## 四、风险评估

- **低风险**: 大部分硬编码字符串都是 UI 标签，替换为国际化调用不会影响逻辑
- **测试需求**: 需要验证所有 `LocalizationManager.Get()` 调用都能正确解析
- **性能影响**: 字符串查询可能有微小的性能开销，但在 UI 渲染中可忽略

## 五、验收标准

- [ ] 所有硬编码 UI 字符串都被替换为 `LocalizationManager.Get()` 调用
- [ ] 编译通过，0 错误
- [ ] 所有翻译键都存在于 zh-CN.json
- [ ] 运行时游戏内 UI 显示正常
- [ ] 语言切换功能正常工作

## 六、后续维护

1. 每次添加新 UI 文本时，必须添加对应的翻译键
2. 定期检查代码中是否有新的硬编码字符串
3. 在添加新语言时，确保所有键都有翻译

---

**扫描完成日期**: 2024年10月26日  
**扫描覆盖范围**: v3 源代码 + 翻译文件  
**扫描工具**: 代码审查 + 正则表达式搜索  
**下一步**: 实施修复和编译验证
