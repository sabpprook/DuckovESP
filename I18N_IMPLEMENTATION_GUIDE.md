# DuckovESP i18n 实现代码示例

本文档提供了完整的i18n系统实现代码框架和示例。

## 1. LocalizationManager.cs

```csharp
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Globalization;

namespace DuckovESP.Utils
{
    /// <summary>
    /// 国际化管理器
    /// 单例模式，管理多语言翻译
    /// </summary>
    public static class LocalizationManager
    {
        private static LocalizationDataStore _dataStore;
        private static string _currentLanguage = "en-US";
        private static string _fallbackLanguage = "en-US";
        private static bool _initialized = false;

        // 支持的语言列表
        private static readonly Dictionary<string, string> SupportedLanguages = new Dictionary<string, string>
        {
            { "zh-CN", "简体中文" },
            { "en-US", "English" },
            { "ja-JP", "日本語" },
            { "ko-KR", "한국어" },
            { "ru-RU", "Русский" },
            { "es-ES", "Español" },
            { "fr-FR", "Français" },
            { "de-DE", "Deutsch" }
        };

        // 初始化i18n系统
        public static void Initialize()
        {
            if (_initialized)
                return;

            try
            {
                _dataStore = new LocalizationDataStore();
                
                // 1. 尝试从游戏API获取当前语言
                DetectGameLanguage();
                
                // 2. 加载翻译文件
                LoadTranslations();
                
                _initialized = true;
                Debug.Log($"[Localization] 已初始化，当前语言: {_currentLanguage}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[Localization] 初始化失败: {ex.Message}");
                _currentLanguage = "en-US";
                _initialized = false;
            }
        }

        /// <summary>
        /// 尝试从游戏LocalizationManager获取当前语言
        /// </summary>
        private static void DetectGameLanguage()
        {
            try
            {
                // 方案A：尝试获取游戏的LocalizationManager
                var gameLocalizationMgr = Type.GetType("SodaCraft.Localizations.LocalizationManager");
                if (gameLocalizationMgr != null)
                {
                    var currentLangProp = gameLocalizationMgr.GetProperty("CurrentLanguageDisplayName");
                    if (currentLangProp != null)
                    {
                        string displayName = (string)currentLangProp.GetValue(null, null);
                        _currentLanguage = MapGameLanguageToLocale(displayName);
                        Debug.Log($"[Localization] 从游戏API检测到语言: {displayName} -> {_currentLanguage}");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[Localization] 无法从游戏API获取语言: {ex.Message}");
            }

            // 方案B：使用系统CultureInfo作为备用
            try
            {
                string systemLocale = CultureInfo.CurrentCulture.Name;
                if (SupportedLanguages.ContainsKey(systemLocale))
                {
                    _currentLanguage = systemLocale;
                    Debug.Log($"[Localization] 使用系统locale: {_currentLanguage}");
                    return;
                }
                
                // 尝试按语言code匹配（例如 "zh-Hans" 匹配到 "zh-CN"）
                string langCode = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
                foreach (var lang in SupportedLanguages.Keys)
                {
                    if (lang.StartsWith(langCode))
                    {
                        _currentLanguage = lang;
                        Debug.Log($"[Localization] 通过语言码匹配系统locale: {systemLocale} -> {_currentLanguage}");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[Localization] 无法获取系统locale: {ex.Message}");
            }

            // 默认使用英文
            _currentLanguage = "en-US";
            Debug.Log("[Localization] 使用默认语言: en-US");
        }

        /// <summary>
        /// 将游戏显示的语言名称映射到标准locale代码
        /// </summary>
        private static string MapGameLanguageToLocale(string gameLanguageName)
        {
            if (string.IsNullOrEmpty(gameLanguageName))
                return "en-US";

            // 游戏可能的语言名称映射
            var languageMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "简体中文", "zh-CN" },
                { "English", "en-US" },
                { "日本語", "ja-JP" },
                { "한국어", "ko-KR" },
                { "Русский", "ru-RU" },
                { "Español", "es-ES" },
                { "Français", "fr-FR" },
                { "Deutsch", "de-DE" },
                // 备选名称
                { "Chinese", "zh-CN" },
                { "Japanese", "ja-JP" },
                { "Korean", "ko-KR" },
                { "Russian", "ru-RU" },
                { "Spanish", "es-ES" },
                { "French", "fr-FR" },
                { "German", "de-DE" }
            };

            if (languageMapping.ContainsKey(gameLanguageName))
                return languageMapping[gameLanguageName];

            return "en-US";
        }

        /// <summary>
        /// 加载翻译文件
        /// </summary>
        private static void LoadTranslations()
        {
            if (_dataStore == null)
                return;

            string translationDir = Path.Combine(Application.dataPath, "Mods", "DuckovESP", "i18n", "Translations");
            
            if (!Directory.Exists(translationDir))
            {
                Debug.LogWarning($"[Localization] 翻译文件目录不存在: {translationDir}");
                return;
            }

            // 加载当前语言的翻译
            LoadLanguageTranslations(_currentLanguage, translationDir);

            // 加载fallback语言的翻译（作为备用）
            if (_currentLanguage != _fallbackLanguage)
            {
                LoadLanguageTranslations(_fallbackLanguage, translationDir);
            }
        }

        /// <summary>
        /// 加载特定语言的翻译文件
        /// </summary>
        private static void LoadLanguageTranslations(string language, string translationDir)
        {
            // 优先加载JSON格式，其次是INI格式
            string jsonPath = Path.Combine(translationDir, $"{language}.json");
            string iniPath = Path.Combine(translationDir, $"{language}.ini");

            if (File.Exists(jsonPath))
            {
                _dataStore.LoadFromJson(language, jsonPath);
            }
            else if (File.Exists(iniPath))
            {
                _dataStore.LoadFromIni(language, iniPath);
            }
            else
            {
                Debug.LogWarning($"[Localization] 未找到语言文件: {language}");
            }
        }

        /// <summary>
        /// 获取翻译字符串
        /// </summary>
        public static string Get(string key, Dictionary<string, string> parameters = null)
        {
            if (!_initialized)
                Initialize();

            if (_dataStore == null)
                return key; // 返回键名作为默认值

            string result = _dataStore.Get(key, _currentLanguage, _fallbackLanguage);
            
            // 参数替换
            if (parameters != null && result != null)
            {
                foreach (var param in parameters)
                {
                    result = result.Replace($"{{{param.Key}}}", param.Value);
                }
            }

            return result ?? key;
        }

        /// <summary>
        /// 获取当前语言代码
        /// </summary>
        public static string GetCurrentLanguage()
        {
            return _currentLanguage;
        }

        /// <summary>
        /// 设置当前语言
        /// </summary>
        public static void SetLanguage(string languageCode)
        {
            if (!SupportedLanguages.ContainsKey(languageCode))
            {
                Debug.LogWarning($"[Localization] 不支持的语言: {languageCode}");
                return;
            }

            _currentLanguage = languageCode;
            Debug.Log($"[Localization] 已切换语言: {languageCode}");
            
            // 重新加载翻译（如果需要）
            if (_initialized)
            {
                LoadTranslations();
            }
        }

        /// <summary>
        /// 检查是否支持某语言
        /// </summary>
        public static bool IsSupportedLanguage(string languageCode)
        {
            return SupportedLanguages.ContainsKey(languageCode);
        }

        /// <summary>
        /// 获取支持的所有语言列表
        /// </summary>
        public static List<string> GetSupportedLanguages()
        {
            return new List<string>(SupportedLanguages.Keys);
        }

        /// <summary>
        /// 获取语言的显示名称
        /// </summary>
        public static string GetLanguageDisplayName(string languageCode)
        {
            if (SupportedLanguages.ContainsKey(languageCode))
                return SupportedLanguages[languageCode];

            return languageCode;
        }
    }
}
```

---

## 2. LocalizationDataStore.cs

```csharp
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

namespace DuckovESP.Utils
{
    /// <summary>
    /// 翻译数据存储
    /// 存储多语言的键值对，支持嵌套命名空间
    /// </summary>
    public class LocalizationDataStore
    {
        // 嵌套字典结构: language -> namespace -> value
        private Dictionary<string, Dictionary<string, string>> _translations = 
            new Dictionary<string, Dictionary<string, string>>();

        /// <summary>
        /// 从INI文件加载翻译
        /// </summary>
        public void LoadFromIni(string language, string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    Debug.LogWarning($"[Localization] INI文件不存在: {filePath}");
                    return;
                }

                Dictionary<string, string> translations = new Dictionary<string, string>();
                string currentSection = "";
                
                string[] lines = File.ReadAllLines(filePath, System.Text.Encoding.UTF8);
                
                foreach (string line in lines)
                {
                    string trimmed = line.Trim();
                    
                    // 跳过空行和注释
                    if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith(";") || trimmed.StartsWith("#"))
                        continue;

                    // 处理section [SectionName]
                    if (trimmed.StartsWith("[") && trimmed.EndsWith("]"))
                    {
                        currentSection = trimmed.Substring(1, trimmed.Length - 2);
                        continue;
                    }

                    // 处理键值对 key=value
                    int eqIndex = trimmed.IndexOf('=');
                    if (eqIndex > 0)
                    {
                        string key = trimmed.Substring(0, eqIndex).Trim();
                        string value = trimmed.Substring(eqIndex + 1).Trim();
                        
                        // 构建完整键：section.key
                        string fullKey = string.IsNullOrEmpty(currentSection) 
                            ? key 
                            : $"{currentSection}.{key}";
                        
                        translations[fullKey] = value;
                    }
                }

                _translations[language] = translations;
                Debug.Log($"[Localization] 已从INI加载 {language}: {translations.Count} 个翻译");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[Localization] 加载INI文件失败 ({language}): {ex.Message}");
            }
        }

        /// <summary>
        /// 从JSON文件加载翻译
        /// </summary>
        public void LoadFromJson(string language, string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    Debug.LogWarning($"[Localization] JSON文件不存在: {filePath}");
                    return;
                }

                string json = File.ReadAllText(filePath, System.Text.Encoding.UTF8);
                Dictionary<string, string> translations = new Dictionary<string, string>();
                
                // 简单的JSON解析，处理嵌套结构
                ParseJsonObject(json, "", translations);

                _translations[language] = translations;
                Debug.Log($"[Localization] 已从JSON加载 {language}: {translations.Count} 个翻译");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[Localization] 加载JSON文件失败 ({language}): {ex.Message}");
            }
        }

        /// <summary>
        /// 递归解析JSON对象
        /// </summary>
        private void ParseJsonObject(string json, string prefix, Dictionary<string, string> result)
        {
            // 简单的JSON对象解析（处理嵌套）
            // 注：为了简化，这里使用基于键值对的正则表达式
            // 实际项目中建议使用 JsonUtility.FromJson<Dictionary<>>
            
            // 移除花括号
            json = json.Trim().TrimStart('{').TrimEnd('}');

            // 匹配 "key": value 或 "key": { ... }
            string keyValuePattern = @"""([^""]+)"":\s*(?:""([^""]*)""|(\{[^}]+\})|(\d+)|true|false)";
            MatchCollection matches = Regex.Matches(json, keyValuePattern);

            foreach (Match match in matches)
            {
                string key = match.Groups[1].Value;
                string fullKey = string.IsNullOrEmpty(prefix) ? key : $"{prefix}.{key}";
                
                // 简单值（字符串）
                if (!string.IsNullOrEmpty(match.Groups[2].Value))
                {
                    result[fullKey] = match.Groups[2].Value;
                }
                // 数字或布尔值
                else if (!string.IsNullOrEmpty(match.Groups[4].Value))
                {
                    result[fullKey] = match.Groups[4].Value;
                }
            }

            // 处理嵌套对象
            string nestedPattern = @"""([^""]+)"":\s*(\{[^}]+\})";
            MatchCollection nestedMatches = Regex.Matches(json, nestedPattern);

            foreach (Match match in nestedMatches)
            {
                string key = match.Groups[1].Value;
                string nestedJson = match.Groups[2].Value;
                string newPrefix = string.IsNullOrEmpty(prefix) ? key : $"{prefix}.{key}";

                ParseJsonObject(nestedJson, newPrefix, result);
            }
        }

        /// <summary>
        /// 获取翻译值，支持fallback
        /// </summary>
        public string Get(string key, string language, string fallbackLanguage)
        {
            // 尝试当前语言
            if (_translations.ContainsKey(language))
            {
                var langDict = _translations[language];
                if (langDict.ContainsKey(key))
                    return langDict[key];
            }

            // 尝试fallback语言
            if (!string.IsNullOrEmpty(fallbackLanguage) && fallbackLanguage != language)
            {
                if (_translations.ContainsKey(fallbackLanguage))
                {
                    var langDict = _translations[fallbackLanguage];
                    if (langDict.ContainsKey(key))
                        return langDict[key];
                }
            }

            // 都没找到，返回键名
            return null;
        }

        /// <summary>
        /// 调试：输出所有已加载的翻译
        /// </summary>
        public void DebugPrintAll()
        {
            foreach (var lang in _translations)
            {
                Debug.Log($"=== Language: {lang.Key} ({lang.Value.Count} entries) ===");
                foreach (var entry in lang.Value)
                {
                    Debug.Log($"  {entry.Key} = {entry.Value}");
                }
            }
        }
    }
}
```

---

## 3. 在ModBehaviour中的集成

```csharp
using DuckovESP.Utils;

namespace DuckovESP
{
    public class ModBehaviour : Duckov.Modding.ModBehaviour
    {
        private void Awake()
        {
            // 初始化i18n系统（第一步）
            LocalizationManager.Initialize();

            Debug.Log("=== " + LocalizationManager.Get("Debug.ModLoaded") + " ===");

            // ... 其他初始化代码
            
            Debug.Log(LocalizationManager.Get("Debug.Enable3DESP", 
                new Dictionary<string, string> { { "status", _config.Enable3DESP ? "启用" : "禁用" } }));
        }

        private void Update()
        {
            // ... 现有代码
            
            // 检查菜单切换
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(_config.MenuToggleKey))
            {
                _configMenu.ToggleMenu();
                Debug.Log(LocalizationManager.Get("Debug.MenuToggled"));
            }
        }
    }
}
```

---

## 4. 在ESPConfigMenu中的集成

```csharp
using DuckovESP.Utils;

namespace DuckovESP.UI
{
    public class ESPConfigMenu
    {
        private void DrawMenuWindow(int windowID)
        {
            GUILayout.BeginVertical();
            
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, GUILayout.Height(650));
            
            // 使用翻译的标题
            GUILayout.Label("=== " + LocalizationManager.Get("UI.Menu.ESP3DSettings") + " ===", _labelStyle);
            GUILayout.Space(5);
            
            // 使用翻译的按钮和标签
            _config.Enable3DESP = GUILayout.Toggle(_config.Enable3DESP, 
                " " + LocalizationManager.Get("UI.Button.Enable3DESP"), _toggleStyle);
            
            _config.ShowDistance = GUILayout.Toggle(_config.ShowDistance, 
                " " + LocalizationManager.Get("UI.Button.ShowDistance"), _toggleStyle);
            
            _config.ShowConnectLine = GUILayout.Toggle(_config.ShowConnectLine, 
                " " + LocalizationManager.Get("UI.Button.ShowConnectLine"), _toggleStyle);
            
            // ... 其他UI元素，全部使用LocalizationManager.Get()

            // 新增：语言切换菜单
            GUILayout.Space(10);
            GUILayout.Label("=== " + LocalizationManager.Get("UI.Menu.LanguageSettings") + " ===", _labelStyle);
            
            var languages = LocalizationManager.GetSupportedLanguages();
            GUILayout.BeginHorizontal();
            foreach (var lang in languages)
            {
                if (GUILayout.Button(LocalizationManager.GetLanguageDisplayName(lang)))
                {
                    LocalizationManager.SetLanguage(lang);
                    Debug.Log(LocalizationManager.Get("Debug.LanguageSwitched"));
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }
    }
}
```

---

## 5. 翻译文件示例

### zh-CN.ini

```ini
[Debug.Message]
ModLoaded=DuckovESP已加载
Enable3DESP=3D ESP: {status}
MenuToggled=配置菜单已切换
LanguageSwitched=已切换语言
ConfigSaved=配置已保存
ConfigLoaded=配置已加载

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
LanguageSettings=语言设置

[UI.Button]
Enable3DESP=启用3D ESP
ShowDistance=显示距离
ShowConnectLine=显示连接线
EnableMapMarkers=启用小地图标记
ShowEmptyBoxes=显示空箱子
EnableEnemyESP=启用敌人ESP
HighlightQuestItems=高亮任务物品
HighlightBuildingMaterials=高亮建筑材料
EnableAimbot=启用自动瞄准
EnableTriggerBot=启用自动扳机
EnableNoRecoil=启用无后座力
EnableGodMode=启用无敌模式
EnableOneHitKill=启用一击必杀
EnableSpeedBoost=启用速度提升
EnableInfiniteWeight=启用无限负重
EnableInfiniteAmmo=启用无限子弹
EnableInfiniteStamina=启用无限耐力

[UI.Settings]
MaxDisplayDistance=最大显示距离
FontSize=字体大小
MaxDisplayItems=最大显示物品数
BackgroundAlpha=背景透明度
BorderThickness=边框粗细
MinQualityFilter=最小品质过滤
MaxEnemyDistance=敌人ESP最大距离
LineWidth=连线粗细
ValueThreshold=高价值阈值

[Alert]
HighValueTargetDetected=⚠️ 发现高价值目标！
TradersNearby=⚠️ 商人在附近！
ItemsFound=发现 {count} 个目标物品

[Tooltip]
AimbotFOV=瞄准视野范围（推荐值 200-500像素）
AimbotSmoothness=目标追踪的平滑度（1-20）
TriggerBotDelay=自动开火的延迟时间（秒）
```

### en-US.ini

```ini
[Debug.Message]
ModLoaded=DuckovESP Loaded
Enable3DESP=3D ESP: {status}
MenuToggled=Config Menu Toggled
LanguageSwitched=Language Switched
ConfigSaved=Config Saved
ConfigLoaded=Config Loaded

[UI.Menu]
Title=DuckovESP Config Menu [⚠️ Contains Cheats]
ESP3DSettings=3D ESP Settings
MapMarkerSettings=Map Marker Settings
FilterSettings=Filter Settings
AppearanceSettings=Appearance Settings
EnemyESPSettings=Enemy ESP Settings
QuestItemSettings=Quest Items & Building Materials
AimbotSettings=Aimbot Settings ⚠️
TriggerBotSettings=Trigger Bot Settings ⚠️
NoRecoilSettings=No Recoil Settings ⚠️
CheatFunctions=Cheat Functions (Hotkeys)
LanguageSettings=Language Settings

[UI.Button]
Enable3DESP=Enable 3D ESP
ShowDistance=Show Distance
ShowConnectLine=Show Connect Lines
EnableMapMarkers=Enable Map Markers
ShowEmptyBoxes=Show Empty Boxes
EnableEnemyESP=Enable Enemy ESP
HighlightQuestItems=Highlight Quest Items
HighlightBuildingMaterials=Highlight Building Materials
EnableAimbot=Enable Aimbot
EnableTriggerBot=Enable Trigger Bot
EnableNoRecoil=Enable No Recoil
EnableGodMode=Enable God Mode
EnableOneHitKill=Enable One Hit Kill
EnableSpeedBoost=Enable Speed Boost
EnableInfiniteWeight=Enable Infinite Weight
EnableInfiniteAmmo=Enable Infinite Ammo
EnableInfiniteStamina=Enable Infinite Stamina

[UI.Settings]
MaxDisplayDistance=Max Display Distance
FontSize=Font Size
MaxDisplayItems=Max Display Items
BackgroundAlpha=Background Alpha
BorderThickness=Border Thickness
MinQualityFilter=Min Quality Filter
MaxEnemyDistance=Max Enemy ESP Distance
LineWidth=Line Width
ValueThreshold=High Value Threshold

[Alert]
HighValueTargetDetected=⚠️ High-value target detected!
TradersNearby=⚠️ Traders nearby!
ItemsFound=Found {count} target items

[Tooltip]
AimbotFOV=Aiming Field of View (Recommended 200-500 pixels)
AimbotSmoothness=Target tracking smoothness (1-20)
TriggerBotDelay=Auto-fire delay time (seconds)
```

---

## 6. 使用模式总结

### 简单使用
```csharp
string text = LocalizationManager.Get("UI.Menu.Title");
```

### 带参数的使用
```csharp
var msg = LocalizationManager.Get("Alert.ItemsFound", 
    new Dictionary<string, string> { { "count", "5" } });
```

### 语言切换
```csharp
LocalizationManager.SetLanguage("zh-CN");
```

### 支持的语言列表
```csharp
var languages = LocalizationManager.GetSupportedLanguages();
foreach (var lang in languages)
{
    string displayName = LocalizationManager.GetLanguageDisplayName(lang);
    Debug.Log($"{lang}: {displayName}");
}
```

---

## 7. 文件组织结构（最终）

```
DuckovESP/
├── ModBehaviour.cs
├── Config/
│   └── ESPConfig.cs
├── UI/
│   ├── ESPConfigMenu.cs
│   ├── EnemyListWindow.cs
│   └── WelcomePopup.cs
├── Utils/
│   └── Localization/                   # 新增
│       ├── LocalizationManager.cs      # 核心管理器
│       └── LocalizationDataStore.cs    # 数据存储
├── i18n/                               # 新增
│   └── Translations/                   # 新增
│       ├── zh-CN.ini                   # 新增
│       ├── en-US.ini                   # 新增
│       ├── ja-JP.ini                   # 新增（可选）
│       ├── zh-CN.json                  # 新增（可选）
│       ├── en-US.json                  # 新增（可选）
│       └── translations.json           # 新增（可选）
├── Features/
│   └── ...
└── Properties/
    └── ...
```

