# Bug 修复报告 - 2025-10-25

## 🐛 问题 1: 对象池无法获取标记

### 症状
```
[ESPSystemManager] 无法从对象池获取标记
[ESPSystemManager] 无法从对象池获取标记
[ESPSystemManager] 无法从对象池获取标记
```

### 根本原因
**初始化顺序错误**：
1. `ModBehaviour.Awake()` 创建数据采集器
2. `ModBehaviour.Awake()` 创建 ESP 系统管理器
3. ESP 系统管理器订阅事件
4. 数据采集器初始化时立即发布事件
5. 此时 `ESPMarkerPool` 的 Awake() 还未执行（Unity 生命周期限制）
6. `ESPMarkerPool.Instance` 为 null
7. `Get()` 返回 null

**时序图：**
```
ModBehaviour.Awake()
  ├─ 创建 ESPSystemManager GameObject         [帧 N]
  ├─ ESPSystemManager.Initialize()            [帧 N]
  │   └─ 订阅 4 个事件                        [帧 N]
  ├─ 创建数据采集器                           [帧 N]
  └─ OnLevelLoaded() 触发                     [帧 N]
      └─ LootboxCollector.Initialize()       [帧 N]
          └─ 发布 LootboxDiscoveredEvent      [帧 N]
              └─ ESPSystemManager 收到事件    [帧 N]
                  └─ ESPMarkerPool.Get()      [帧 N] ❌ Instance=null
                  
ESPMarkerPool.Awake()                          [帧 N+1] ⚠️ 太晚了！
```

### 修复方案

**方案 1: 手动创建对象池（已采用）**
```csharp
// ModBehaviour.Awake()

// 5. 初始化 ESP 对象池（必须在数据采集器之前）
var poolObject = new GameObject("ESPMarkerPool");
poolObject.transform.SetParent(transform);
var pool = poolObject.AddComponent<ESPMarkerPool>();
// Unity 会在下一帧执行 Awake()，但我们可以提前创建 GameObject

// 6. 初始化 ESP 系统管理器
var espManagerObject = new GameObject("ESPSystemManager");
espManagerObject.transform.SetParent(transform);
_espSystemManager = espManagerObject.AddComponent<ESPSystemManager>();
_espSystemManager.Initialize(_espConfig, _eventBus, _logger);

// 7. 初始化数据采集器（必须在 ESP 系统之后）
_lootboxCollector = new LootboxDataCollector(_eventBus, _logger);
_worldItemCollector = new WorldItemDataCollector(_eventBus, _logger);
```

**新的时序：**
```
ModBehaviour.Awake()
  ├─ 创建 ESPMarkerPool GameObject            [帧 N]
  ├─ 创建 ESPSystemManager GameObject         [帧 N]
  ├─ ESPSystemManager.Initialize()            [帧 N]
  └─ 创建数据采集器                           [帧 N]
  
Unity 生命周期
  ├─ ESPMarkerPool.Awake()                    [帧 N，Awake 阶段]
  │   └─ Instance = this                      [帧 N]
  │   └─ CreateMarkerPrefab()                 [帧 N]
  │   └─ 预创建 50 个标记                      [帧 N]
  └─ OnEnable()                               [帧 N，OnEnable 阶段]
      └─ 订阅关卡加载事件                     [帧 N]
      
OnLevelLoaded() 触发                          [帧 M > N]
  └─ LootboxCollector.Initialize()
      └─ 发布 LootboxDiscoveredEvent
          └─ ESPSystemManager 收到事件
              └─ ESPMarkerPool.Get()          ✅ Instance 已初始化
```

### 验证方法
```
1. 启动游戏，进入关卡
2. 查看日志：
   ✅ [ModBehaviour] ESP对象池已创建
   ✅ [ESPMarkerPool] 程序化创建 Marker Prefab 完成
   ✅ [ESPSystemManager] ESP 系统已初始化
   ✅ [ESPSystemManager] 创建箱子标记: XXX
   
3. 不应该再看到 "无法从对象池获取标记"
```

---

## 🐛 问题 2: 翻译键不存在

### 症状
```
[Localization] 翻译键不存在: UI.Toggle.EnableMapMarkers
```

### 根本原因
**JSON 解析逻辑不完整**：

原代码使用简化的 `JsonUtility.FromJson<SimpleJsonObject>`，但该方法：
1. 只能解析平铺的 JSON（单层）
2. 无法处理嵌套对象（如 `"UI": { "Toggle": { ... } }`）
3. 只能读取预定义的字段（key/value）

实际翻译文件结构：
```json
{
  "UI": {
    "Toggle": {
      "EnableMapMarkers": "启用小地图标记"
    }
  }
}
```

期望的平铺键：`UI.Toggle.EnableMapMarkers`

### 修复方案

**重写 JSON 解析器（手动递归）**：

```csharp
/// <summary>
/// 递归解析嵌套 JSON（手动实现）
/// </summary>
private void ParseNestedJson(string json, string prefix, Dictionary<string, string> dict)
{
    // 算法：
    // 1. 逐字符扫描 JSON
    // 2. 追踪嵌套深度（{ } 计数）
    // 3. 遇到键值对时：
    //    - 如果值是字符串："key": "value" → dict["key"] = "value"
    //    - 如果值是对象："key": { ... } → 递归调用，前缀为 "key"
    // 4. 构建点分隔的完整键："UI.Toggle.EnableMapMarkers"
}
```

**示例转换：**
```
输入 JSON:
{
  "UI": {
    "Toggle": {
      "EnableMapMarkers": "启用小地图标记",
      "Enable3DESP": "启用3D ESP透视"
    },
    "Label": {
      "Language": "语言 (Language):"
    }
  }
}

输出 Dictionary:
"UI.Toggle.EnableMapMarkers" → "启用小地图标记"
"UI.Toggle.Enable3DESP" → "启用3D ESP透视"
"UI.Label.Language" → "语言 (Language):"
```

**算法复杂度：**
- 时间复杂度：O(n)，n 为 JSON 字符数
- 空间复杂度：O(k)，k 为键值对数量
- 嵌套深度：支持任意深度

### 验证方法
```
1. 启动游戏
2. 查看日志：
   ✅ [Localization] 已加载语言: zh-CN
   ✅ 不应该再看到 "翻译键不存在" 警告
   
3. 打开配置菜单（F8）
4. 所有文本应该正确显示中文翻译
```

---

## 📊 测试结果

### 编译状态
```
✅ Release 编译成功
⚠️ 22 个 null 警告（不影响运行，减少 2 个）
📦 DLL 大小: 65 KB → 67 KB (+2 KB，新增 JSON 解析逻辑)
```

### 代码变更
| 文件 | 修改类型 | 变更行数 |
|------|---------|---------|
| `ModBehaviour.cs` | 修改 | +10 行（调整初始化顺序）|
| `LocalizationManager.cs` | 重写 | +150 行（新 JSON 解析器）|

### 新增功能
- ✅ 完整的嵌套 JSON 解析器
- ✅ 支持任意深度的对象嵌套
- ✅ 字符串转义处理（\n, \t, \", \\）
- ✅ 健壮的错误处理

---

## 🚀 部署步骤

1. **编译新版本**
   ```powershell
   cd D:\VSProjects\DuckovESPv3\DuckovESPv3
   dotnet build -c Release
   ```

2. **复制 DLL**
   ```powershell
   Copy-Item "DuckovESPv3\bin\Release\netstandard2.1\DuckovESPv3.dll" `
             -Destination "[游戏目录]\Mods\DuckovESPv3.dll" `
             -Force
   ```

3. **复制翻译文件**（如果修改过）
   ```powershell
   Copy-Item "DuckovESPv3\i18n\Translations\*.json" `
             -Destination "[游戏目录]\Mods\DuckovESPv3\i18n\Translations\" `
             -Recurse -Force
   ```

4. **启动游戏测试**

---

## ✅ 验证清单

### 基础功能
- [ ] 游戏启动无错误
- [ ] 日志中无 "无法从对象池获取标记"
- [ ] 日志中无 "翻译键不存在"
- [ ] 进入关卡后 ESP 标记正常显示

### ESP 功能
- [ ] 箱子上方显示圆形图标
- [ ] 图标颜色根据品质变化（白/绿/蓝/紫/橙/红）
- [ ] 连接线从玩家指向目标
- [ ] 距离文本正确显示（如 "25m"）
- [ ] 远距离标记半透明

### 本地化
- [ ] 配置菜单（F8）所有文本显示中文
- [ ] 没有显示键名（如 "UI.Toggle.XXX"）
- [ ] 特殊字符正确显示（冒号、括号等）

### 性能
- [ ] FPS 无明显下降（<10%）
- [ ] 切换场景流畅，无卡顿
- [ ] 拾取物品后标记立即消失

---

## 🔧 如果仍有问题

### 问题 1: 标记仍不显示
**检查：**
```
1. 日志中是否有 "[ESPMarkerPool] 程序化创建 Marker Prefab 完成"
2. 日志中是否有 "[LootboxDataCollector] 扫描找到 X 个战利品箱"
3. 日志中是否有 "[ESPSystemManager] 创建箱子标记: XXX"
```

**如果第 1 步失败**：
- ESPMarkerPool.Awake() 未执行
- 检查 GameObject 是否正确创建

**如果第 2 步失败**：
- 关卡加载事件未触发
- 检查 LevelManager.OnAfterLevelInitialized 订阅

**如果第 3 步失败**：
- 过滤器过滤掉了所有物品
- 修改配置：`Enable3DESP = true`, `MinQualityFilter3D = 0`

### 问题 2: 翻译键仍然缺失
**检查：**
```
1. 翻译文件路径是否正确
   日志应该显示: [Localization] 翻译文件路径: D:\...\Mods\DuckovESPv3\i18n\Translations
   
2. 文件是否存在
   检查: [游戏目录]\Mods\DuckovESPv3\i18n\Translations\zh-CN.json
   
3. JSON 格式是否正确
   使用 JSON 验证器检查语法
```

**手动测试 JSON 解析：**
```csharp
// 在 LocalizationManager.LoadLanguage() 中添加调试日志
Debug.Log($"[Localization] 解析到 {translationDict.Count} 个键");
foreach (var key in translationDict.Keys.Take(5))
{
    Debug.Log($"[Localization] 键: {key} → 值: {translationDict[key]}");
}
```

---

## 📝 技术笔记

### Unity 生命周期顺序
```
1. Awake()        - 对象创建时（只执行一次）
2. OnEnable()     - 激活时
3. Start()        - 第一帧之前
4. FixedUpdate()  - 固定时间步
5. Update()       - 每帧
6. LateUpdate()   - Update 之后
7. OnDisable()    - 禁用时
8. OnDestroy()    - 销毁时
```

**重要规则：**
- **同一帧内的执行顺序**：所有 Awake() → 所有 OnEnable() → 所有 Start()
- **组件创建顺序**：父对象先于子对象
- **事件订阅时机**：必须在 OnEnable() 或更晚

### 对象池模式最佳实践
```csharp
// ❌ 错误：在构造函数中初始化单例
public class MyPool
{
    public static MyPool Instance { get; private set; }
    
    public MyPool()  // 构造函数在 AddComponent 时调用
    {
        Instance = this;  // 此时其他脚本可能已经在查找 Instance
    }
}

// ✅ 正确：在 Awake 中初始化单例
public class MyPool : MonoBehaviour
{
    public static MyPool Instance { get; private set; }
    
    private void Awake()  // Unity 保证在任何代码运行前调用
    {
        Instance = this;
    }
}
```

---

**修复日期**: 2025-10-25  
**修复版本**: v3.0.1  
**状态**: ✅ 已修复，等待测试确认
