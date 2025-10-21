# 撤离点与任务区域标记 - 快速参考

## 功能概览

| 功能 | 状态 | 配置键 | 说明 |
|------|------|--------|------|
| 撤离点标记 | ✅ 完成 | EnableEvacuationIndicator | 绿色圆形/箭头显示撤离点 |
| 撤离点发光 | ✅ 完成 | - | 双层圆形和箭头创建霓虹效果 |
| 任务区域标记 | ✅ 完成 | EnableQuestZoneIndicator | 黄色圆形/箭头显示任务区域 |
| 距离显示 | ✅ 完成 | ShowEvacuationDistance / ShowQuestZoneDistance | 屏幕上显示距离文本 |

## 关键改进

### 撤离点标记修复

1. **Y轴修正**
   ```csharp
   // GL坐标系Y轴修正
   float y = Screen.height - center.y + radius * Mathf.Sin(angle);
   ```

2. **尺寸扩大**
   - 圆形：30-150px（之前是10px以下）
   - 箭头：40px（之前是20px）

3. **霓虹效果**
   - 外圈：完全不透明（1.0）
   - 内圈：半透明（0.5）+ 暗化

4. **无距离限制**
   - 远处仍显示方向箭头
   - 屏幕外箭头始终可见

### 任务区域标记实现

1. **自动扫描**
   - 每2秒扫描一次新的CountDownArea
   - 使用缓存避免频繁查询

2. **范围计算**
   - 支持多种碰撞体类型
   - 自动从bounds计算半径

3. **渲染两层次**
   - 屏幕内：圆形表示范围
   - 屏幕外：箭头表示方向

4. **完整集成**
   - 配置菜单控制
   - 多语言支持
   - GL渲染（性能优化）

## 文件变更清单

### 新建文件
- `Features/Cheats/QuestZoneMarkerData.cs` (55行)

### 修改文件

#### CheatSystem.cs (添加～150行)
- 添加任务区域缓存字段
- GetQuestZones() - 获取任务区域
- ScanQuestZones() - 扫描任务区域
- GetQuestZoneRadius() - 计算范围

#### ModBehaviour.cs (添加～200行)
- 修复Y轴坐标（DrawCircleGL, DrawArrowGL）
- 增大标记尺寸
- 添加发光效果
- DrawQuestZoneIndicatorsGL() - 任务区域GL渲染
- DrawQuestZoneCircleGL() - 任务圆形
- DrawQuestZoneArrowGL() - 任务箭头
- DrawQuestZoneIndicatorsText() - 任务文字

#### ESPConfig.cs (添加～8行)
```csharp
public bool EnableQuestZoneIndicator = true;
public Color QuestZoneIndicatorColor = Color.yellow;
public float QuestZoneIndicatorSize = 20f;
public bool ShowQuestZoneDistance = true;
```

#### ESPConfigMenu.cs (添加～12行)
- 任务区域启用切换
- 显示距离切换
- 标记大小滑块

#### 本地化文件 (各3行)
- zh-CN.json
- en-US.json
- de-DE.json

## 配置说明

### 撤离点设置
```csharp
EnableEvacuationIndicator = true;      // 启用/禁用
EvacuationIndicatorColor = Color.green; // 绿色
EvacuationIndicatorSize = 20f;         // 尺寸缩放
ShowEvacuationDistance = true;         // 显示距离
```

### 任务区域设置
```csharp
EnableQuestZoneIndicator = true;       // 启用/禁用
QuestZoneIndicatorColor = Color.yellow; // 黄色
QuestZoneIndicatorSize = 20f;          // 尺寸缩放
ShowQuestZoneDistance = true;          // 显示距离
```

## 性能指标

| 操作 | 频率 | 性能 | 备注 |
|------|------|------|------|
| 撤离点扫描 | 仅初始化 | ~0ms | 使用缓存 |
| 任务区域扫描 | 每2秒 | <1ms | 定期更新 |
| 撤离点GL渲染 | 每帧 | <1ms | 双层线条 |
| 任务区域GL渲染 | 每帧 | <1ms | 双层形状 |
| 文字渲染 | 每帧 | <2ms | OnGUI |

## 故障排除

### 标记显示位置不对
- ✅ 检查：Y轴已修正，所有坐标都使用`Screen.height - y`

### 标记尺寸太小
- ✅ 检查：撤离点圆形30-150px, 箭头40px
- ✅ 检查：任务区域使用相同算法

### 发光效果不明显
- ✅ 检查：外圈/内圈两层已实现
- ✅ 检查：外圈透明度1.0（完全不透明）

### 远处看不到箭头
- ✅ 检查：已移除距离限制
- ✅ 检查：屏幕外箭头始终显示

### 任务区域没有显示
- ✅ 检查：EnableQuestZoneIndicator需要启用
- ✅ 检查：关卡内才会扫描任务

## 下一步改进

1. 【可选】小地图标记支持
2. 【可选】任务进度条显示
3. 【可选】完成任务通知
4. 【可选】自定义标记颜色UI

