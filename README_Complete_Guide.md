# DuckovESPv3 完整指南

## ✅ 系统状态

**编译状态**: ✅ Release 成功编译（24 个 null 警告，不影响运行）

**已完成功能**:
- ✅ 事件驱动的数据采集系统
- ✅ ESP 渲染系统（程序化 Prefab）
- ✅ 对象池管理（50 初始，500 最大）
- ✅ 智能过滤系统（品质/心愿单/任务/建筑材料）
- ✅ 颜色分级（7 级品质）
- ✅ 距离淡出和连接线

---

## 🎮 使用方法

### 1. 安装 Mod

1. 编译项目：
   ```powershell
   cd D:\VSProjects\DuckovESPv3\DuckovESPv3
   dotnet build -c Release
   ```

2. 将编译后的 DLL 复制到游戏 Mods 目录：
   ```
   DuckovESPv3\bin\Release\netstandard2.1\DuckovESPv3.dll
   → [游戏根目录]\Mods\
   ```

3. 启动游戏

### 2. 默认配置

**快捷键**:
- `F8`: 打开/关闭配置菜单

**ESP 设置** (在 `config.json` 中):
```json
{
  "Enable3DESP": true,
  "MaxESPDistance": 100.0,
  "ShowConnectLine": true,
  "ShowDistance": true,
  "MinQualityFilter3D": 0,
  "ShowEmptyBoxes": true,
  "ShowOnlyWishlistedItems": false,
  "ShowOnlyQuestItems": false,
  "ShowOnlyBuildingMaterials": false
}
```

### 3. 配置说明

| 配置项 | 类型 | 默认值 | 说明 |
|--------|------|--------|------|
| `Enable3DESP` | bool | true | 启用/禁用 ESP |
| `MaxESPDistance` | float | 100 | 最大显示距离（米）|
| `ShowConnectLine` | bool | true | 显示连接线 |
| `ShowDistance` | bool | true | 显示距离文本 |
| `MinQualityFilter3D` | int | 0 | 最低品质过滤（0-6）|
| `ShowEmptyBoxes` | bool | true | 显示空箱子 |
| `ShowOnlyWishlistedItems` | bool | false | 仅显示心愿单物品 |
| `ShowOnlyQuestItems` | bool | false | 仅显示任务物品 |
| `ShowOnlyBuildingMaterials` | bool | false | 仅显示建筑材料 |

### 4. 品质等级和颜色

| 品质 | 颜色 | RGB |
|------|------|-----|
| White | 灰白色 | (0.8, 0.8, 0.8) |
| Green | 绿色 | (0.2, 1.0, 0.2) |
| Blue | 蓝色 | (0.3, 0.5, 1.0) |
| Purple | 紫色 | (0.8, 0.3, 1.0) |
| Orange | 橙色 | (1.0, 0.6, 0.0) |
| LightRed | 浅红色 | (1.0, 0.5, 0.5) |
| Red | 红色 | (1.0, 0.2, 0.2) |

---

## 🔧 技术架构

### 核心组件

```
ModBehaviour (入口点)
    ├─ EventBus (事件总线)
    │   └─ 4 个数据事件
    │
    ├─ LootboxDataCollector (箱子扫描)
    │   └─ 发布: LootboxDiscoveredEvent, LootboxRemovedEvent
    │
    ├─ WorldItemDataCollector (物品追踪)
    │   └─ 发布: WorldItemDiscoveredEvent, WorldItemRemovedEvent
    │
    └─ ESPSystemManager (渲染管理)
        ├─ 订阅: 4 个数据事件
        ├─ 过滤逻辑
        ├─ 颜色映射
        └─ ESPMarkerPool (对象池)
            ├─ ESPSpriteGenerator (程序化生成)
            └─ ESPMarker × 50-500 (标记实例)
                ├─ SpriteRenderer (圆形图标)
                ├─ LineRenderer (连接线)
                └─ Canvas + Text (距离文本)
```

### 性能特性

1. **零轮询设计**
   - 所有逻辑由事件驱动
   - 无 Update() 循环扫描
   - CPU 占用极低

2. **对象池管理**
   - 初始容量: 50 个标记
   - 最大容量: 500 个标记
   - 自动回收和复用

3. **GPU 加速渲染**
   - Unity SpriteRenderer (GPU)
   - Unity LineRenderer (GPU)
   - 无 CPU 绘制开销

4. **智能剔除**
   - 距离剔除: >100m 不显示
   - 透明度淡出: 远处半透明
   - 自动回收: 目标销毁时释放

---

## 🐛 故障排查

### 问题 1: 看不到 ESP 标记

**可能原因**:
- ESP 未启用: 检查 `Enable3DESP = true`
- 距离过远: 调整 `MaxESPDistance`
- 过滤器太严格: 检查品质/心愿单过滤器

**解决方法**:
1. 按 `F8` 打开配置菜单
2. 确认 "Enable 3D ESP" 已勾选
3. 将 "Min Quality Filter" 设置为 0（显示所有品质）
4. 将所有过滤器设置为 `false`

### 问题 2: 性能下降/卡顿

**可能原因**:
- 同时显示标记过多（>500）
- MaxESPDistance 设置过大

**解决方法**:
1. 降低 `MaxESPDistance` 到 50-75 米
2. 启用品质过滤: `MinQualityFilter3D = 2` (仅显示蓝色及以上)
3. 检查日志中是否有 "已达到最大容量" 警告

### 问题 3: 关卡切换后标记消失

**已自动处理**:
- 系统会在关卡加载时自动清理旧标记
- 新关卡加载完成后自动重新扫描
- 无需手动操作

### 问题 4: 子场景物品不显示

**已自动处理**:
- 系统监听 `MultiSceneCore.OnSubSceneLoaded` 事件
- 子场景加载后自动执行增量扫描
- 不会清除主场景的标记

---

## 📊 性能监控

### 查看对象池统计

在日志中搜索：
```
[ESPMarkerPool] 当前统计: 活动=X, 可用=Y, 总计=Z
```

### 性能目标

| 场景 | 标记数量 | 目标帧时间 | 实际表现 |
|------|----------|-----------|---------|
| 小型关卡 | <100 | <1ms | ✅ 预期达标 |
| 中型关卡 | 100-300 | <2ms | ✅ 预期达标 |
| 大型关卡 | 300-500 | <3ms | ⚠️ 需测试 |
| 超大关卡 | >500 | N/A | ❌ 达到容量上限 |

### 优化建议

如果遇到性能问题：
1. 降低 `MaxESPDistance`（影响最大）
2. 提高 `MinQualityFilter3D`（减少低品质物品）
3. 启用特定过滤器（如仅显示心愿单）
4. 禁用 `ShowConnectLine`（减少 LineRenderer 开销）

---

## 🔬 开发调试

### 启用详细日志

在 `ModBehaviour.cs` 的 Awake() 中：
```csharp
_serviceContainer.RegisterSingleton<ILogger>(
    container => new UnityLogger("DuckovESPv3", true) // 第二个参数改为 true
);
```

### 日志级别

- `[Debug]`: 详细调试信息（标记创建/销毁）
- `[Info]`: 一般信息（系统初始化）
- `[Warning]`: 警告（容量达到上限）
- `[Error]`: 错误（异常情况）

### 常见日志消息

**正常运行**:
```
[ModBehaviour] DuckovESPv3 Mod 正在初始化...
[ESPMarkerPool] 程序化创建 Marker Prefab 完成
[ESPSystemManager] ESP 系统已初始化
[LootboxDataCollector] 发现箱子: Lootbox_XXX
[ESPSystemManager] 创建箱子标记: Lootbox_XXX
```

**性能警告**:
```
[ESPMarkerPool] 已达到最大容量 500，无法创建更多标记
```

**场景切换**:
```
[ModBehaviour] 主关卡加载完成，清理旧数据并重新初始化采集器
[ESPSystemManager] 已清理所有标记
```

---

## 📝 代码统计

**总行数**: 1,034 行
- `ESPSpriteGenerator.cs`: 171 行（程序化生成）
- `ESPMarkerPool.cs`: 219 行（对象池）
- `ESPMarker.cs`: 172 行（渲染组件）
- `ESPSystemManager.cs`: 311 行（系统管理）
- `LootboxDataCollector.cs`: 347 行（数据采集）
- `WorldItemDataCollector.cs`: 307 行（数据采集）
- `ModBehaviour.cs`: 集成代码

**架构模式**:
- 事件驱动架构 (Event-Driven)
- 对象池模式 (Object Pool)
- 单例模式 (Singleton)
- 依赖注入 (Dependency Injection)

---

## 🚀 下一步开发

### Phase 3: UI 配置面板 (待实现)

**目标**: 在游戏内实时调整 ESP 设置

**功能列表**:
- [ ] ESP 启用/禁用开关
- [ ] 距离滑块 (0-200m)
- [ ] 品质过滤下拉菜单
- [ ] 过滤器复选框组
- [ ] 显示选项（连接线、距离文本）
- [ ] 预设配置（默认/高性能/高品质）

### Phase 4: 高级功能 (可选)

- [ ] 自定义颜色配置
- [ ] 多种标记形状（圆形/方形/菱形）
- [ ] 动画效果（脉冲/旋转/闪烁）
- [ ] 小地图集成
- [ ] 声音提示（发现高品质物品）
- [ ] 统计面板（发现物品数量、总价值等）

---

## 📄 许可证

本项目仅供学习和研究使用。

**更新时间**: 2025-10-25  
**版本**: v3.0 Phase 2 完成  
**状态**: ✅ 可用于游戏测试
