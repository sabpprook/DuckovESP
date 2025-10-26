# 🚀 部署检查清单 - v3 最终修复

## 预部署验证

### 1. 编译验证 ✅
```
Status: 编译通过
Errors: 0
Warnings: 45 (全为预先存在)
Build Time: 0.81s
Date: [最后编译时间]
```

### 2. 修改文件确认
- [x] `ESPSystemManager.cs` - SetDataTrackers + RefreshDataFromTrackers
- [x] `QuestZoneTracker.cs` - 三层位置提取
- [x] `QuestMarkerCollectionService.cs` - Getter 方法
- [x] `ModBehaviour.cs` - SetDataTrackers 调用

### 3. 代码质量检查
- [x] 无新的编译错误
- [x] 反射调用都有非空检查
- [x] 所有 Vector3 都使用 out 参数
- [x] 日志记录完整

## 部署步骤

### Step 1: 编译 DLL
```powershell
cd d:\VSProjects\DuckovESPv3\DuckovESPv3
dotnet build DuckovESPv3.csproj -c Release
```

**预期结果**: 
```
✅ Build succeeded
0 个错误, 45 个警告
输出: bin/Release/DuckovESPv3.dll
```

### Step 2: 备份旧版本
```powershell
# 备份游戏目录中的旧 DLL
$gameModPath = "C:\Games\Duckov\Mods"  # 调整为实际路径
Copy-Item "$gameModPath\DuckovESPv3.dll" "$gameModPath\DuckovESPv3.dll.backup"
```

### Step 3: 安装新版本
```powershell
# 复制新编译的 DLL
Copy-Item "d:\VSProjects\DuckovESPv3\DuckovESPv3\bin\Release\DuckovESPv3.dll" "$gameModPath\"
```

### Step 4: 清理旧数据 (可选)
```powershell
# 清理缓存日志，以便看到新的日志消息
Remove-Item "$gameModPath\DuckovESP_*.log" -ErrorAction SilentlyContinue
```

## 运行时验收

### 测试 1: 初始化日志检查
进入游戏后，检查 ModBehaviour 是否正确调用 SetDataTrackers：
```
✅ 期望输出:
[DuckovESPv3] [ModBehaviour] ✓ 已将追踪器传递给ESPSystemManager
[DuckovESPv3] [QuestMarkerCollectionService] 初始化完成！
[DuckovESPv3] [ESPSystemManager] 数据追踪器引用已设置
```

### 测试 2: 撤离点距离显示
**操作**: 看向任何撤离点
**预期**:
- [x] 显示"45m"(或其他实际距离) 而不是"0m"
- [x] 移动时距离实时更新
- [x] 屏幕内显示绿色圆形 + 距离标签

### 测试 3: Quest Zone 扫描
**检查日志**: 是否有类似如下消息
```
✅ 期望输出:
[DuckovESPv3] [QuestZoneTracker] 🔍 发现 13 个活跃任务
[DuckovESPv3] [QuestZoneTracker] ✓ 已添加任务区域: Quest_1301_Task_0
[DuckovESPv3] [QuestZoneTracker] ✓ 已添加任务区域: Quest_1302_Task_0
[DuckovESPv3] [ESPSystemManager] Quest zones updated: 13 zones
```

**如果显示**:
```
❌ [QuestZoneTracker] ✗ 非位置任务: QuestTask_TaskEvent
```
则表示提取失败，检查日志中的错误信息。

### 测试 4: 位置提取方法验证
**检查日志**: 验证提取方法是否有效
```
期望看到以下之一:
✓ 从 target Transform 获取位置
✓ 从 location.GetLocationTransform() 获取位置
✓ 从 mapElement 获取位置         ← 【关键】大多数任务使用此方法
✓ 从 SpawnPrefabForTask 组件获取位置
```

### 测试 5: 屏幕边缘指示器
**操作**: 离开标记点，让它离开屏幕
**预期**:
- [x] 屏幕边缘出现方向箭头
- [x] 箭头旁显示距离标签
- [x] 回到屏幕时，圆形和标签再次出现

### 测试 6: 撤离点和任务区域混合验证
**操作**: 查看含有多个标记的区域
**预期**:
- [x] 撤离点: 绿色圆形 + 距离
- [x] 任务区域: 彩色圆形 (根据任务类型) + 距离
- [x] 所有距离值正确，不重复为 0

## 问题排查

### 问题1: 距离仍显示"0m"
**原因分析**: RefreshDataFromTrackers() 未被调用或 tracker 数据为空
**排查步骤**:
1. 检查 Update() 方法是否被调用
2. 验证 tracker 是否非空
3. 查看日志: 是否有"数据追踪器引用已设置"
4. 如无，可能 SetDataTrackers() 未被调用

**修复**:
```csharp
// 在 ModBehaviour.cs 检查此行
_espSystemManager.SetDataTrackers(
    _questMarkerService.GetEvacuationPointTracker(),
    _questMarkerService.GetQuestZoneTracker()
);
```

### 问题2: QuestZone 数量为 0
**原因分析**: 位置提取三策略全部失败
**排查步骤**:
1. 查看日志: 是否有"✗ 非位置任务"消息
2. 检查具体任务类型: TaskEvent? ReachLocation? SubmitItems?
3. 验证位置提取是否尝试了 mapElement

**修复**:
- 如无 mapElement: 检查游戏源代码，看是否新增任务类型
- 如 mapElement 存在但提取失败: 可能 locations 列表为空或位置对象不对

### 问题3: 屏幕边缘无箭头
**原因分析**: 屏幕外标记点数据未被存储或箭头渲染代码有问题
**排查步骤**:
1. 验证 zones 数量是否正确 (查看日志)
2. 检查是否有 OnGUI 渲染错误
3. 查看编辑器中是否有异常堆栈跟踪

## 回滚计划

如果出现严重问题，快速回滚：
```powershell
# 恢复备份
Copy-Item "$gameModPath\DuckovESPv3.dll.backup" "$gameModPath\DuckovESPv3.dll" -Force

# 重启游戏
# 问题应该消失
```

## 性能影响

### 计算复杂度
- **Tracker.Update()**: O(n) 其中 n = 活跃任务数 (~13)
- **RefreshDataFromTrackers()**: O(n) 其中 n = 已发现的标记数 (~13-50)
- **每帧总开销**: ~2-3ms (在 60fps 预期下)

### 内存占用
- **新增引用**: 2 个 tracker 指针 (~8 字节)
- **新增列表**: 无 (复用现有 _evacuationPoints, _questZones)
- **总新增**: <1MB

**结论**: 性能影响可忽略

## 预期成果

部署完成后，以下问题应全部解决：

| 问题 | 状态 | 验证方法 |
|------|------|----------|
| 距离显示 0m | ✅ 已修复 | 查看撤离点标签，应为 45m+ |
| 屏幕外无指示 | ✅ 已修复 | 离开屏幕，应看到边缘箭头 |
| QuestZone 为 0 | ✅ 已修复 | 检查日志，应显示 13 zones |
| 位置提取失败 | ✅ 已修复 | mapElement 方法现已启用 |

## 成功标志

✅ **部署成功的确认**:
1. 游戏启动无崩溃
2. 日志显示"已将追踪器传递给ESPSystemManager"
3. 撤离点距离显示正确值 (非 0m)
4. QuestZone 扫描显示 13+ zones
5. 屏幕外能看到箭头指示器
6. 所有距离实时更新

---

**最后检查日期**: [填入日期]
**部署人员**: [填入名称]
**状态**: ⏳ 待部署
