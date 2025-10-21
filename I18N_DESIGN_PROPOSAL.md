# DuckovESP i18n 实现方案设计

## 📋 方案概述

根据对游戏源码的分析和项目结构，提出一套完整的国际化(i18n)实现方案。

### 核心特性
- ✅ 支持多语言（中文、英文、日文等）
- ✅ 通过游戏内设置获取当前语言/locale
- ✅ Fallback机制：缺少的翻译自动回退到英文
- ✅ INI + JSON 双格式支持
- ✅ 模块化架构，易于维护和扩展
- ✅ 动态热重载语言配置（无需重启游戏）

---

## 🎯 方案设计细节

### 1. 获取Locale的方案

#### 选项A：游戏LocalizationManager（推荐）
```csharp
// 从游戏的LocalizationManager获取当前语言
// 游戏已有: SodaCraft.Localizations.LocalizationManager.CurrentLanguageDisplayName
// 可能的值: "简体中文", "English", "日本語" 等

// 优点：
// - 与游戏语言设置保持同步
// - 游戏支持的所有语言都支持
// - 无需额外配置

// 缺点：
// - 需要依赖游戏库（但mod已经依赖了）
```

#### 选项B：系统CultureInfo备用
```csharp
// 如果无法获取游戏设置，使用系统locale作为备用
System.Globalization.CultureInfo.CurrentCulture.Name // "zh-CN", "en-US", "ja-JP"

// 优点：
// - 完全独立，无依赖
// - 系统级别支持

// 缺点：
// - 可能与游戏设置不一致
```

### 2. 文件结构设计

```
Assets/Mods/DuckovESP/
├── config.json                 # 主配置文件（现有）
├── i18n/                       # 新增国际化目录
│   ├── Localization.cs         # i18n管理系统代码
│   ├── Translations/           # 翻译文件目录
│   │   ├── zh-CN.ini          # 中文翻译（INI格式）
│   │   ├── en-US.ini          # 英文翻译（INI格式）
│   │   ├── ja-JP.ini          # 日文翻译（INI格式）
│   │   ├── zh-CN.json         # 中文翻译（JSON格式）
│   │   ├── en-US.json         # 英文翻译（JSON格式）
│   │   └── ja-JP.json         # 日文翻译（JSON格式）
│   └── translations.json       # 翻译元数据（支持的语言列表）
```

### 3. 配置文件格式

#### INI 格式（示例：zh-CN.ini）
```ini
[UI.Menu]
Title=DuckovESP 配置菜单 [⚠️ 包含作弊功能]
ESP3DSettings=3D ESP 设置
MapMarkerSettings=小地图标记设置
FilterSettings=过滤设置
AppearanceSettings=外观设置
EnemyESPSettings=敌人ESP设置
QuestItemSettings=任务物品&建筑材料
AimbotSettings=自动瞄准设置（Aimbot）
TriggerBotSettings=自动扳机设置（Trigger Bot）
NoRecoilSettings=无后座力设置（No Recoil）
CheatFunctions=作弊功能（快捷键）

[UI.Button]
Enable3DESP=启用3D ESP
ShowDistance=显示距离
ShowConnectLine=显示连接线
EnableMapMarkers=启用小地图标记
ShowEmptyBoxes=显示空箱子

[UI.Settings]
MaxDisplayDistance=最大显示距离
FontSize=字体大小
MaxDisplayItems=最大显示物品数

[Debug.Message]
ModLoaded=DuckovESP已加载
ModEnabled=已启用
MenuOpened=配置菜单已打开
MenuClosed=配置菜单已关闭
ConfigSaved=配置已保存

[Alert]
HighValueTarget=高价值目标检测！
NoAmmo=弹药不足
```

#### JSON 格式（示例：zh-CN.json）
```json
{
  "UI": {
    "Menu": {
      "Title": "DuckovESP 配置菜单 [⚠️ 包含作弊功能]",
      "ESP3DSettings": "3D ESP 设置",
      "MapMarkerSettings": "小地图标记设置"
    },
    "Button": {
      "Enable3DESP": "启用3D ESP"
    }
  },
  "Debug": {
    "Message": {
      "ModLoaded": "DuckovESP已加载"
    }
  }
}
```

### 4. 核心API设计

```csharp
// LocalizationManager.cs 使用示例

// 初始化（在Awake中调用一次）
LocalizationManager.Initialize();

// 获取翻译字符串
string menuTitle = LocalizationManager.Get("UI.Menu.Title");

// 带参数的翻译
string msg = LocalizationManager.Get("Alert.DistanceMessage", 
    new { distance = 50, item = "Gold" });
// 翻译文本: "发现{distance}m距离的{item}"
// 结果: "发现50m距离的Gold"

// 获取当前语言代码
string currentLang = LocalizationManager.GetCurrentLanguage();

// 手动切换语言
LocalizationManager.SetLanguage("en-US");

// 检查是否支持某语言
bool supported = LocalizationManager.IsSupportedLanguage("ja-JP");

// 获取支持的所有语言列表
List<string> languages = LocalizationManager.GetSupportedLanguages();

// 获取语言显示名称
string displayName = LocalizationManager.GetLanguageDisplayName("zh-CN");
// 结果: "简体中文"
```

### 5. 实现的关键类

#### LocalizationManager.cs
```
职责：
- 单例管理器
- 初始化翻译系统
- 处理locale检测
- 提供Get/Set方法
- Fallback机制
```

#### LocalizationDataStore.cs
```
职责：
- 存储翻译键值对
- 支持嵌套命名空间 (UI.Menu.Title)
- 内存优化
- 支持参数替换
```

#### TranslationFileLoader.cs
```
职责：
- 加载INI文件
- 加载JSON文件
- 解析配置格式
- 错误处理
```

---

## 🔄 Fallback 机制

```
用户流程：
1. 检查游戏LocalizationManager的当前语言
2. 如果为 "简体中文"，加载 zh-CN 翻译
3. 查询键值对：
   - 如果存在 → 返回翻译
   - 如果不存在 → 尝试fallback
4. Fallback链：zh-CN → en-US → 键名本身
   
示例：
- 查询 "UI.Menu.MyNewFeature"
- 在 zh-CN.ini 中不存在
- 自动查询 en-US.ini
- 在 en-US.ini 中存在 "MyNewFeature"
- 返回英文翻译
- 如果英文也不存在，返回键名 "UI.Menu.MyNewFeature"
```

---

## 📝 集成方案

### Step 1: 创建i18n系统文件
```
创建 Utils/Localization/LocalizationManager.cs
创建 Utils/Localization/LocalizationDataStore.cs
创建 Utils/Localization/TranslationFileLoader.cs
```

### Step 2: 更新ModBehaviour.cs
```csharp
private void Awake()
{
    // 在初始化之前，初始化i18n系统
    LocalizationManager.Initialize();
    
    // ... 现有代码
    Debug.Log(LocalizationManager.Get("Debug.ModLoaded"));
}
```

### Step 3: 更新ESPConfigMenu.cs
```csharp
private void DrawMenuWindow(int windowID)
{
    // 使用翻译字符串替代硬编码
    GUILayout.Label(LocalizationManager.Get("UI.Menu.ESP3DSettings"), _labelStyle);
    GUILayout.Label(LocalizationManager.Get("UI.Button.Enable3DESP"), _labelStyle);
    // ... 其他UI元素
}
```

### Step 4: 创建翻译文件
```
i18n/Translations/
├── zh-CN.ini
├── en-US.ini
└── ja-JP.ini
```

### Step 5: 在菜单中添加语言切换
```csharp
// 在ESPConfigMenu中添加语言选择下拉菜单
GUILayout.Label("语言设置:");
var languages = LocalizationManager.GetSupportedLanguages();
for (int i = 0; i < languages.Count; i++)
{
    if (GUILayout.Button(LocalizationManager.GetLanguageDisplayName(languages[i])))
    {
        LocalizationManager.SetLanguage(languages[i]);
    }
}
```

---

## 🎮 使用场景

### 场景1：菜单UI
```csharp
// ModBehaviour.cs
GUILayout.Label(LocalizationManager.Get("UI.Menu.Title"));
```

### 场景2：调试日志
```csharp
Debug.Log(LocalizationManager.Get("Debug.ModLoaded"));
Debug.Log(LocalizationManager.Get("Debug.ConfigSaved"));
```

### 场景3：运行时提示
```csharp
if (highlightItems.Count > 0)
{
    // 旧方式：
    // notification.Show($"发现 {count} 个目标");
    
    // 新方式：
    string msg = LocalizationManager.Get("Alert.ItemsFound", 
        new { count = highlightItems.Count });
    notification.Show(msg);
}
```

---

## 📊 支持的语言

初期支持（必须）：
- ✅ 简体中文 (zh-CN)
- ✅ 英文 (en-US)

后续支持（可选）：
- 🟡 繁体中文 (zh-TW)
- 🟡 日文 (ja-JP)
- 🟡 韩文 (ko-KR)
- 🟡 俄文 (ru-RU)
- 🟡 西班牙文 (es-ES)
- 🟡 法文 (fr-FR)
- 🟡 德文 (de-DE)

---

## ✅ 优势分析

| 方面 | 优势 |
|------|------|
| **获取Locale** | 通过游戏API获取，与游戏语言设置同步 |
| **文件格式** | 双格式支持（INI更易阅读，JSON易于解析）|
| **Fallback** | 自动降级到英文，确保内容总是显示 |
| **维护性** | 模块化设计，易于添加新语言 |
| **性能** | 内存缓存翻译，无需频繁文件IO |
| **扩展性** | 支持参数替换，灵活的键命名空间 |
| **用户体验** | 菜单内可切换语言，实时生效 |

---

## ⚠️ 注意事项

1. **编码问题**
   - INI/JSON文件必须使用 UTF-8 编码
   - 不能使用 System.Text.Encoding.Default (GBK)

2. **键命名规范**
   - 使用点号分隔的嵌套命名空间
   - 示例：`UI.Menu.Title`, `Debug.Message.ConfigSaved`

3. **翻译维护**
   - 保持所有语言的键同步
   - 定期更新翻译文件

4. **性能考虑**
   - 翻译在启动时加载并缓存
   - 运行时Get操作为O(1)复杂度

---

## 🚀 实现优先级

### Phase 1（必须）
- [ ] 创建LocalizationManager核心系统
- [ ] 支持zh-CN和en-US
- [ ] 集成到ModBehaviour和ESPConfigMenu
- [ ] 创建翻译INI文件

### Phase 2（推荐）
- [ ] 添加JSON格式支持
- [ ] 在菜单中添加语言切换UI
- [ ] 翻译Debug日志
- [ ] 参数替换功能

### Phase 3（可选）
- [ ] 支持更多语言（日文、韩文等）
- [ ] 翻译导出工具
- [ ] 社区翻译系统

---

## 📞 问题答疑

**Q: 为什么用GameSettings而不是系统locale?**
A: 游戏已有LocalizationManager，使用它可以保持一致性。系统locale作为备用方案。

**Q: Fallback只支持一层吗？**
A: 建议设计成链式：zh-CN → en-US → 键名本身，最多三层。

**Q: INI和JSON都支持会不会复杂？**
A: 统一内部表示(LocalizationDataStore)，加载层处理格式差异，使用层无感知。

**Q: 翻译文件会很大吗？**
A: 初期预期 500-1000 个键，INI格式约 20-30KB，完全可以接受。

---

## 📚 参考资源

- 游戏LocalizationManager: `SodaCraft.Localizations`
- .NET CultureInfo: `System.Globalization.CultureInfo`
- INI解析库：可使用简单的正则或自己实现
- JSON解析：Unity内置`JsonUtility`

