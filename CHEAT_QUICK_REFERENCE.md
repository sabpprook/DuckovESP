# 作弊系统快速参考 - DuckovESPv3

## 🎮 快捷键速查表

| 按键 | 功能 | 效果 |
|------|------|------|
| **Shift+F7** | 无敌模式 | 免疫所有伤害 |
| **Shift+F8** | 一击必杀 | 攻击伤害×999 |
| **Shift+F9** | 速度提升 | 移动速度×2.5 |
| **Shift+F10** | 无限负重 | 可携带999999单位重量 |
| **Shift+F11** | 无限子弹 | 子弹永不耗尽，换弹自动满 |
| **Shift+F12** | 无限耐力 | 耐力/饥饿/口渴不减少 |

## 📊 状态指示器（右上角）

```
━━━━━━━━━━━━━━━━━━
   作弊功能状态
━━━━━━━━━━━━━━━━━━
✓ 无敌模式 [Shift+F7]      ← 绿色=启用
✗ 一击必杀 [Shift+F8]      ← 灰色=禁用
✓ 速度提升 [Shift+F9]
✗ 无限负重 [Shift+F10]
✓ 无限子弹 [Shift+F11]
✗ 无限耐力 [Shift+F12]
━━━━━━━━━━━━━━━━━━
```

## 🚀 推荐组合

### 战神模式（屠杀）
- ✅ 无敌模式（Shift+F7）
- ✅ 一击必杀（Shift+F8）
- ✅ 速度提升（Shift+F9）
- ✅ 无限子弹（Shift+F11）

### 探索模式（刷物资）
- ✅ 速度提升（Shift+F9）
- ✅ 无限负重（Shift+F10）
- ✅ 无限耐力（Shift+F12）

### 枪神模式（射击流）
- ✅ 无限子弹（Shift+F11）
- ✅ 一击必杀（Shift+F8）
- ✅ 速度提升（Shift+F9）

## ⚠️ 注意事项

1. **仅战斗场景生效**：基地内功能无效
2. **状态持久化**：切换场景后功能保持
3. **无性能影响**：所有功能<0.1ms开销
4. **安全性**：Harmony Patch，不修改游戏文件

## 🐛 常见问题

**Q: 为什么快捷键没反应？**  
A: 确保在战斗场景（非基地），并正确按下Shift键

**Q: 状态叠加层不显示？**  
A: 检查配置文件中 `ShowCheatStatusOverlay` 是否为 `true`

**Q: 无限子弹不生效？**  
A: 确保装备的是枪械（不是刀具），并按Shift+F11切换

**Q: 功能启用后如何关闭？**  
A: 再按一次相同快捷键即可切换

## 📄 文件位置

- **配置**：`DuckovESPv3/appsettings.json` → `CheatSystem` 节点
- **核心系统**：`Core/Systems/Cheat/CheatSystem.cs`
- **Harmony Hooks**：`Core/Systems/Cheat/Hooks/InfiniteAmmoHook.cs`
- **UI叠加层**：`Core/Systems/Cheat/UI/CheatStatusOverlay.cs`
- **配置类**：`Core/Configuration/CheatSystemConfig.cs`

## 🔧 自定义参数（高级）

编辑 `appsettings.json`：

```json
{
  "CheatSystem": {
    "ShowCheatStatusOverlay": true,
    "SpeedMultiplier": 2.5,              // 速度倍率（默认2.5x）
    "OneHitKillDamageMultiplier": 999,   // 伤害倍率（默认999x）
    "InfiniteWeightCapacity": 999999,    // 负重上限
    "OverlayPaddingRight": 20,           // UI右边距
    "OverlayPaddingTop": 80,             // UI上边距
    "OverlayFontSize": 14                // UI字体大小
  }
}
```

## 📞 技术支持

遇到问题？查看完整文档：
- `CHEAT_SYSTEM_INTEGRATION_COMPLETE.md` - 集成完成报告
- `CHEAT_SYSTEM_ANALYSIS_AND_MIGRATION_PLAN.md` - 技术分析

---

**版本**：V3.0.0  
**更新日期**：2025-01-19  
**状态**：✅ 已集成并可用
