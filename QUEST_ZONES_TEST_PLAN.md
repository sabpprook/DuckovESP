# 任务区域标记 - 测试和调试计划

## 当前状态总结

### 已完成
- ✅ TriggerBot日志全部删除（~50行）
- ✅ 任务区域基础渲染框架实现
- ✅ GL和IMGUI渲染代码完成
- ✅ 配置和本地化完成
- ✅ 改进的二级扫描策略实现

### 当前问题
- ❌ 任务区域在游戏中不可见
- ❌ 扫描结果：找到7个CountDownArea，全是"Exit(Clone)"（撤离点）
- ❌ 真正的任务区域尚未被检测到

### 根本原因
CountDownArea用于撤离/提取，不是任务区域的主要标记组件。真正的任务区域需要通过其他方式识别。

## 编译状态
- ✅ 最新编译成功
- ⚠️ 12个警告（预期的，来自异常处理）
- ✅ 0个错误

## 下一步测试流程

### Phase 1: 数据收集 (立即执行)
**目标:** 通过新的诊断日志理解任务区域的实际结构

**步骤:**
1. 打包最新编译的DLL
2. 进入游戏，加载任何有任务的地图
3. 开启DuckovESP mod
4. 查看Unity Console输出，寻找以下日志：

```
[DuckovESP] 扫描到 X 个ShowLocationInMap组件
[DuckovESP] 发现任务区域(通过ShowLocationInMap): ...
[DuckovESP] ShowLocationInMap '...' 没有Collider组件
[DuckovESP] ShowLocationInMap '...' 没有关联的CountDownArea组件
[DuckovESP] 扫描到 X 个CountDownArea
[DuckovESP] CountDownArea '...' 没有ShowLocationInMap组件
```

**收集信息:**
- 有多少个ShowLocationInMap被找到？
- 其中有多少有Collider？
- 其中有多少有CountDownArea？
- CountDownArea还有其他属性值得注意吗？

### Phase 2: 数据分析
**根据日志输出，决定下一步方向：**

#### 情景A: 成功（显示"发现任务区域"）
→ 跳转到Phase 3: 渲染验证

#### 情景B: ShowLocationInMap缺少CountDownArea
```
原因: 任务区域使用不同的组件结构
行动:
  1. 修改扫描逻辑以跳过CountDownArea要求
  2. 直接使用ShowLocationInMap创建标记
  3. 需要找到替代方式获取任务区域大小/范围
```

#### 情景C: ShowLocationInMap缺少Collider
```
原因: Collider在子对象或不存在
行动:
  1. 已有GetComponentInChildren逻辑，应该能处理
  2. 如果仍无效，需要查询游戏对象大小的替代方法
  3. 可能需要在game code中直接计算范围
```

#### 情景D: 没找到任何ShowLocationInMap
```
原因: 
  - 任务区域使用完全不同的显示系统
  - 游戏可能动态加载/卸载这些组件
  - ShowLocationInMap只在特定游戏状态下存在
  
行动:
  1. 在Phase 1收集更多信息时记录所有GameObject名称
  2. 搜索包含"quest"、"objective"、"task"的对象
  3. 可能需要重新审视游戏源代码
  4. 考虑使用Zone组件作为替代检测方法
```

### Phase 3: 渲染验证
**如果找到任务区域，验证是否正确显示**

**验证清单:**
- [ ] 3D场景中显示圆形标记？
- [ ] 小地图中显示箭头或图标？
- [ ] 距离信息正确显示？
- [ ] 颜色和大小设置正确？
- [ ] 标记名称/标签显示？
- [ ] 标记随玩家位置更新？

**常见渲染问题:**
| 问题 | 原因 | 解决方案 |
|------|------|--------|
| 3D标记不显示 | GL渲染未正确调用 | 检查ModBehaviour.OnPostRender() |
| 小地图标记不显示 | 坐标变换错误 | 检查小地图坐标系转换 |
| 标记显示在错误位置 | 坐标系混淆 | 检查centerPosition计算 |
| 标记大小不对 | 半径计算错误 | 检查GetQuestZoneRadius() |

## 关键代码检查点

### 检查1: 扫描逻辑
**文件:** `DuckovESP/Features/Cheats/CheatSystem.cs`
**方法:** `ScanQuestZones()` (行753-825)

验证项:
- [ ] ShowLocationInMap搜索在前
- [ ] CountDownArea搜索在后
- [ ] GetComponentInParent被正确调用
- [ ] GetComponentInChildren被正确调用

### 检查2: 数据结构
**文件:** `DuckovESP/Features/Cheats/QuestZoneMarkerData.cs`

验证项:
- [ ] 所有必要字段都存在
- [ ] 默认值合理

### 检查3: 渲染调用
**文件:** `DuckovESP/ModBehaviour.cs`

验证项:
- [ ] DrawQuestZoneIndicatorsGL() 在OnPostRender()中被调用
- [ ] DrawQuestZoneIndicatorsText() 在OnGUI()中被调用
- [ ] ESPConfig.EnableQuestZoneIndicator 被检查

### 检查4: 配置激活
**文件:** `DuckovESP/Config/ESPConfig.cs`

验证项:
- [ ] EnableQuestZoneIndicator 默认启用
- [ ] 其他设置有合理的默认值

## 快速故障排除

### 问题: 看到很多日志但没有"发现任务区域"
**可能原因:** 
- 任务区域不使用ShowLocationInMap
- 或没有Collider
- 或没有CountDownArea

**快速诊断:**
```csharp
// 在CheatSystem.ScanQuestZones()中添加
Debug.Log("[DuckovESP] ===== ShowLocationInMap详细信息 =====");
foreach (var marker in locationMarkers)
{
    Debug.Log($"  名称: {marker.gameObject.name}");
    Debug.Log($"    - 显示名: {marker.DisplayName}");
    Debug.Log($"    - 有Collider: {marker.GetComponent<Collider>() != null}");
    Debug.Log($"    - 有CountDownArea: {marker.GetComponent<CountDownArea>() != null}");
    Debug.Log($"    - 有子Collider: {marker.GetComponentInChildren<Collider>() != null}");
}
```

### 问题: 编译失败
**解决方案:**
1. 检查QuestZoneMarkerData.cs是否存在
2. 检查所有必要的using语句
3. 运行 `dotnet clean` 然后重新编译

### 问题: 日志输出很少或没有
**解决方案:**
1. 确保CheatSystem在运行
2. 确保_shouldScanQuestZones为true
3. 检查Unity Console是否正确配置
4. 确保DuckovESP mod已启用

## 成功指标

任务区域功能被认为成功当：
1. ✅ 日志显示找到了任务区域
2. ✅ 3D视图中显示圆形标记
3. ✅ 小地图显示标记
4. ✅ 靠近区域时标记正确移动
5. ✅ 显示任务名称和距离信息
6. ✅ 配置菜单中的设置生效（颜色、大小、可见性）

## 报告日志的格式

执行测试后，请按以下格式报告：

```
## 测试日期: 2024-XX-XX

### 扫描结果
- ShowLocationInMap数量: X
- 发现任务区域数: Y
- CountDownArea数量: Z
- 其中Exit(Clone): W

### 关键日志
[粘贴所有[DuckovESP]开头的日志]

### 渲染测试
- 3D标记显示: [是/否]
- 小地图标记显示: [是/否]
- 其他观察: [描述]

### 下一步建议
[根据结果分析]
```

## 参考资源

- QUEST_ZONES_ENHANCED_DEBUG.md - 详细的诊断指南
- CheatSystem.cs - 扫描逻辑实现
- ModBehaviour.cs - 渲染实现
- ESPConfig.cs - 配置
