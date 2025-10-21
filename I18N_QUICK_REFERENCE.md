# DuckovESP i18n 快速参考卡片

## 📌 一页纸方案总结

### 核心方案

```
获取 Locale
├─ 优先：游戏 LocalizationManager
├─ 备用：系统 CultureInfo  
└─ 默认：en-US

翻译文件
├─ 格式：INI 或 JSON (UTF-8编码)
├─ 位置：i18n/Translations/
└─ 支持：zh-CN, en-US + 其他语言

Fallback 机制
└─ 当前语言 → en-US → 键名本身

架构
├─ LocalizationManager (单例)
├─ LocalizationDataStore (数据存储)
└─ 动态文件加载
```

---

## 🎯 实现步骤（简化版）

### Step 1: 创建核心类 (2小时)
```
Utils/Localization/
  ├─ LocalizationManager.cs
  └─ LocalizationDataStore.cs
```

### Step 2: 创建翻译文件 (3小时)
```
i18n/Translations/
  ├─ zh-CN.ini
  └─ en-US.ini
```

### Step 3: 集成代码 (2小时)
```csharp
// 在 ModBehaviour.Awake() 中
LocalizationManager.Initialize();

// 在菜单中替换所有硬编码字符串
LocalizationManager.Get("UI.Menu.Title")
```

### Step 4: 测试与优化 (1小时)

**总耗时：8小时**

---

## 💻 代码模板

### 最小化使用示例

```csharp
// 初始化
LocalizationManager.Initialize();

// 获取翻译
string title = LocalizationManager.Get("UI.Menu.Title");

// 带参数
string msg = LocalizationManager.Get("Alert.Found", 
    new Dictionary<string, string> { { "count", "5" } });

// 切换语言
LocalizationManager.SetLanguage("zh-CN");

// 获取支持的语言
var langs = LocalizationManager.GetSupportedLanguages();
```

---

## 📄 翻译文件模板

### INI 格式

```ini
[Section.SubSection]
Key=翻译值
AnotherKey=Another translation

[UI.Menu]
Title=标题
Button=按钮
```

### JSON 格式

```json
{
  "UI": {
    "Menu": {
      "Title": "标题",
      "Button": "按钮"
    }
  }
}
```

---

## ✅ 验收标准

| 功能 | 验收条件 |
|------|---------|
| **Locale检测** | ✅ 自动识别游戏语言 |
| **翻译加载** | ✅ 启动时加载完整翻译 |
| **Fallback** | ✅ 缺失翻译自动回退 |
| **UI集成** | ✅ 菜单完全国际化 |
| **语言切换** | ✅ 菜单内实时切换 |
| **性能** | ✅ 启动时间 +0-100ms |

---

## 🎮 使用流程图

```
启动 → 检测系统语言 → 加载翻译 → 显示菜单
                 ↓          ↓
            (en-US)   (zh-CN.ini)
                         ↓
                   玩家点击菜单
                         ↓
                   语言切换按钮
                         ↓
                   重新加载翻译
                         ↓
                   菜单实时更新
```

---

## 🔑 关键设计决策

| 决策 | 理由 |
|------|------|
| **游戏API优先** | 与游戏语言设置同步，用户无需二次配置 |
| **INI+JSON双格式** | INI易编辑，JSON易自动化 |
| **三层Fallback** | 确保内容总是可见，即使翻译不完整 |
| **单例+缓存** | 最小化内存占用，零运行时性能影响 |
| **菜单内切换** | 提升用户体验，无需重启游戏 |

---

## 📊 预期结果

### 优势
- ✅ 支持多语言，满足国际用户需求
- ✅ Fallback保障，翻译缺失也能用
- ✅ 模块化设计，易于维护扩展
- ✅ 零性能影响，启动时间基本不变
- ✅ 用户体验好，菜单内即可切换语言

### 投入
- 开发时间：8小时左右
- 翻译工作量：每语言6小时（初期只需中英文）
- 文件体积：增加 20-50KB（可接受）

### 受益人群
- 国际玩家（使用English）
- 日本玩家（potential support）
- 韩国玩家（potential support）
- 俄罗斯玩家（potential support）
- ...以及其他所有语言用户

---

## 🚨 常见问题速查

**Q: 游戏没有LocalizationManager怎么办?**
A: 自动回退到系统CultureInfo，然后回落到en-US

**Q: 翻译太多怎么办?**
A: 分阶段翻译。先做zh-CN和en-US，其他语言后续补充

**Q: 性能会变差吗?**
A: 不会。翻译在启动时一次性加载到内存，之后Get()操作是O(1)

**Q: 支持实时语言切换吗?**
A: 是的，菜单中添加按钮即可实现

**Q: 可以参与众包翻译吗?**
A: 可以。翻译文件是INI格式，任何人都可以编辑和贡献

---

## 📋 文件清单

| 文件 | 说明 | 优先级 |
|------|------|--------|
| LocalizationManager.cs | i18n核心管理器 | 🔴 必须 |
| LocalizationDataStore.cs | 翻译数据存储 | 🔴 必须 |
| zh-CN.ini | 中文翻译 | 🔴 必须 |
| en-US.ini | 英文翻译 | 🔴 必须 |
| ja-JP.ini | 日文翻译 | 🟡 可选 |
| *.json | JSON格式翻译 | 🟡 可选 |

---

## 🔗 文档关联

- **详细设计**：参考 `I18N_DESIGN_PROPOSAL.md`
- **实现代码**：参考 `I18N_IMPLEMENTATION_GUIDE.md`
- **对比分析**：参考 `I18N_SUMMARY_AND_RECOMMENDATIONS.md`
- **本文档**：快速参考卡片

---

## 💾 快速命令参考

```csharp
// 初始化系统
LocalizationManager.Initialize();

// 获取文本
LocalizationManager.Get("UI.Menu.Title")

// 带参数获取
LocalizationManager.Get("Alert.Items", 
    new Dict { { "count", "5" } })

// 切换语言
LocalizationManager.SetLanguage("zh-CN")

// 获取当前语言
LocalizationManager.GetCurrentLanguage()

// 获取支持的语言
LocalizationManager.GetSupportedLanguages()

// 获取语言显示名称
LocalizationManager.GetLanguageDisplayName("zh-CN")

// 检查是否支持
LocalizationManager.IsSupportedLanguage("ja-JP")
```

---

## ⏱️ 时间估算

| 任务 | 工作量 |
|------|--------|
| 开发LocalizationManager | 2小时 |
| 开发LocalizationDataStore | 1.5小时 |
| 创建翻译文件框架 | 0.5小时 |
| 翻译菜单文本（中英文） | 3小时 |
| 集成到代码 | 2小时 |
| 测试和优化 | 1小时 |
| **总计** | **10小时** |

---

**方案状态**：✅ 已完成  
**实施难度**：⭐️⭐️ (中等)  
**投资回报率**：⭐️⭐️⭐️⭐️⭐️ (极高)  

