using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json.Linq;

namespace DuckovESPv3.Infrastructure.Localization
{
    /// <summary>
    /// 本地化管理器
    /// </summary>
    /// <remarks>
    /// 架构层级：Infrastructure/Localization
    /// 职责：
    /// - 加载和管理多语言翻译资源
    /// - 支持参数替换（如 {key}）
    /// - 支持语言切换
    /// - 自动检测游戏语言或系统语言
    /// - 支持翻译回退机制
    /// 性能特征：翻译数据缓存在内存，O(1) 查询
    /// 支持的语言：zh-CN（简体中文）、en-US（英语）、de-DE（德语）
    /// 回退顺序：用户选择 → 游戏语言 → 系统语言 → 英文（en-US）
    /// </remarks>
    public class LocalizationManager
    {
        private static LocalizationManager? _instance;
        private readonly Dictionary<string, Dictionary<string, string>> _translations = new Dictionary<string, Dictionary<string, string>>();
        private string _currentLanguage = "en-US";
        private readonly string _translationPath;
        
        // 支持的语言列表
        private static readonly string[] SupportedLanguages = new[] { "zh-CN", "en-US", "de-DE" };

        private LocalizationManager()
        {
            // 获取DLL所在目录
            string dllPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string dllDirectory = Path.GetDirectoryName(dllPath);
            _translationPath = Path.Combine(dllDirectory, "i18n", "Translations");
            
            Debug.Log($"[Localization] LocalizationManager 构造函数执行");
            Debug.Log($"[Localization] DLL路径: {dllPath}");
            Debug.Log($"[Localization] DLL目录: {dllDirectory}");
            Debug.Log($"[Localization] 翻译文件路径: {_translationPath}");
            Debug.Log($"[Localization] 翻译文件路径是否存在: {Directory.Exists(_translationPath)}");
        }

        /// <summary>
        /// 获取单例实例
        /// </summary>
        public static LocalizationManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LocalizationManager();
                }
                return _instance;
            }
        }

        /// <summary>
        /// 初始化本地化系统（自动检测语言）
        /// </summary>
        public void Initialize()
        {
            Debug.Log($"[Localization] Initialize() 被调用（自动检测模式）");
            
            // 1. 尝试从游戏设置读取语言（如果存在）
            string detectedLanguage = DetectGameLanguage();
            
            // 2. 如果游戏语言检测失败，尝试系统语言
            if (string.IsNullOrEmpty(detectedLanguage))
            {
                detectedLanguage = DetectSystemLanguage();
            }
            
            // 3. 最后的fallback是英文
            if (string.IsNullOrEmpty(detectedLanguage))
            {
                detectedLanguage = "en-US";
            }
            
            Debug.Log($"[Localization] 检测到的语言: {detectedLanguage}");
            LoadLanguage(detectedLanguage);
        }

        /// <summary>
        /// 初始化本地化系统（指定默认语言）
        /// </summary>
        /// <param name="defaultLanguage">默认语言代码（如 "zh-CN"）</param>
        public void Initialize(string defaultLanguage = "en-US")
        {
            Debug.Log($"[Localization] Initialize() 被调用，语言: {defaultLanguage}");
            _currentLanguage = defaultLanguage;
            LoadLanguage(defaultLanguage);
            Debug.Log($"[Localization] Initialize() 完成");
        }

        /// <summary>
        /// 检测游戏语言设置
        /// </summary>
        /// <returns>语言代码或空字符串</returns>
        private string DetectGameLanguage()
        {
            try
            {
                // 尝试从游戏Localization系统读取
                // 这是通用的Unity游戏语言检测方式
                var localizationSettings = Resources.Load("LocalizationSettings");
                if (localizationSettings != null)
                {
                    Debug.Log($"[Localization] 找到游戏Localization设置");
                    // 如果游戏使用了Unity Localization，可以这样读取
                    // var gameLanguage = UnityEngine.Localization.Settings.LocalizationSettings.SelectedLocale.Identifier.Code;
                    // 返回映射的语言代码
                }
                
                // 尝试从特殊的游戏配置文件读取
                string gameConfigPath = Path.Combine(
                    Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location),
                    "DuckovGame.config"
                );
                
                if (File.Exists(gameConfigPath))
                {
                    string content = File.ReadAllText(gameConfigPath);
                    if (content.Contains("language"))
                    {
                        Debug.Log($"[Localization] 从游戏配置读取语言设置");
                        // 简单的配置解析
                        foreach (var line in content.Split('\n'))
                        {
                            if (line.Contains("language=zh"))
                                return "zh-CN";
                            if (line.Contains("language=en"))
                                return "en-US";
                            if (line.Contains("language=de"))
                                return "de-DE";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[Localization] 检测游戏语言时出错: {ex.Message}");
            }
            
            return "";
        }

        /// <summary>
        /// 检测系统语言
        /// </summary>
        /// <returns>支持的语言代码或空字符串</returns>
        private string DetectSystemLanguage()
        {
            try
            {
                // 获取系统语言
                string systemLang = System.Globalization.CultureInfo.CurrentCulture.Name;
                Debug.Log($"[Localization] 系统语言: {systemLang}");
                
                // 映射系统语言到支持的语言
                if (systemLang.StartsWith("zh"))
                    return "zh-CN";
                if (systemLang.StartsWith("de"))
                    return "de-DE";
                if (systemLang.StartsWith("en"))
                    return "en-US";
                
                // 返回系统语言对应的国家代码版本
                string languagePrefix = systemLang.Substring(0, 2);
                foreach (var lang in SupportedLanguages)
                {
                    if (lang.StartsWith(languagePrefix))
                        return lang;
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[Localization] 检测系统语言时出错: {ex.Message}");
            }
            
            return "";
        }

        /// <summary>
        /// 加载指定语言的翻译
        /// </summary>
        /// <param name="languageCode">语言代码（如 "zh-CN"）</param>
        public void LoadLanguage(string languageCode)
        {
            Debug.Log($"[Localization] LoadLanguage() 被调用，语言: {languageCode}");
            
            if (_translations.ContainsKey(languageCode))
            {
                Debug.Log($"[Localization] 语言 {languageCode} 已加载，直接切换");
                _currentLanguage = languageCode;
                return;
            }

            try
            {
                string filePath = Path.Combine(_translationPath, $"{languageCode}.json");
                Debug.Log($"[Localization] 尝试加载文件: {filePath}");
                Debug.Log($"[Localization] 文件是否存在: {File.Exists(filePath)}");
                
                if (!File.Exists(filePath))
                {
                    Debug.LogWarning($"[Localization] 翻译文件不存在: {filePath}，尝试fallback到en-US");
                    
                    // Fallback到英文
                    if (languageCode != "en-US")
                    {
                        LoadLanguage("en-US");
                        return;
                    }
                    else
                    {
                        Debug.LogError($"[Localization] 英文翻译文件也不存在，无法继续");
                        return;
                    }
                }

                string json = File.ReadAllText(filePath);
                Debug.Log($"[Localization] 文件读取成功，长度: {json.Length} 字符");
                
                var translationDict = new Dictionary<string, string>();
                
                // 使用 Newtonsoft.Json 解析 JSON
                var jObject = JObject.Parse(json);
                FlattenJson(jObject, "", translationDict);
                
                _translations[languageCode] = translationDict;
                _currentLanguage = languageCode;
                
                Debug.Log($"[Localization] 已加载语言: {languageCode}，共 {translationDict.Count} 个键");
                
                // 调试：打印前10个键
                int count = 0;
                foreach (var kvp in translationDict)
                {
                    Debug.Log($"[Localization] 键: {kvp.Key} = {kvp.Value}");
                    if (++count >= 10) break;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[Localization] 加载语言 {languageCode} 失败: {ex.Message}");
                Debug.LogError($"[Localization] 堆栈跟踪: {ex.StackTrace}");
                
                // 如果不是英文，尝试fallback到英文
                if (languageCode != "en-US")
                {
                    Debug.LogWarning($"[Localization] 尝试fallback到en-US");
                    LoadLanguage("en-US");
                }
            }
        }

        /// <summary>
        /// 获取翻译文本（带fallback机制）
        /// </summary>
        /// <param name="key">翻译键（支持嵌套，如 "Menu.Title"）</param>
        /// <param name="parameters">参数替换（如 ("name", "Alice")）</param>
        /// <returns>翻译后的文本或原始键</returns>
        public static string Get(string key, params (string name, string value)[] parameters)
        {
            return Instance.GetInternal(key, parameters);
        }

        private string GetInternal(string key, params (string name, string value)[] parameters)
        {
            // 1. 尝试从当前语言获取
            if (_translations.TryGetValue(_currentLanguage, out var translationDict))
            {
                if (translationDict.TryGetValue(key, out var text))
                {
                    return ApplyParameters(text, parameters);
                }
            }
            
            // 2. Fallback到英文（如果当前语言不是英文）
            if (_currentLanguage != "en-US" && _translations.TryGetValue("en-US", out var englishDict))
            {
                if (englishDict.TryGetValue(key, out var englishText))
                {
                    Debug.LogWarning($"[Localization] 键 '{key}' 在{_currentLanguage}中不存在，使用英文版本");
                    return ApplyParameters(englishText, parameters);
                }
            }
            
            // 3. 都没有找到，返回原始键
            Debug.LogWarning($"[Localization] 翻译键不存在: {key} (当前语言: {_currentLanguage})");
            return key;
        }

        /// <summary>
        /// 应用参数替换
        /// </summary>
        private string ApplyParameters(string text, (string name, string value)[] parameters)
        {
            if (parameters.Length == 0)
                return text;
            
            foreach (var (name, value) in parameters)
            {
                text = text.Replace("{" + name + "}", value);
            }
            
            return text;
        }

        /// <summary>
        /// 获取当前语言
        /// </summary>
        public static string GetCurrentLanguage()
        {
            return Instance._currentLanguage;
        }

        /// <summary>
        /// 设置当前语言
        /// </summary>
        public static void SetLanguage(string languageCode)
        {
            Instance.LoadLanguage(languageCode);
        }

        /// <summary>
        /// 获取支持的所有语言
        /// </summary>
        public static string[] GetSupportedLanguages()
        {
            return (string[])SupportedLanguages.Clone();
        }

        /// <summary>
        /// 获取语言的显示名称
        /// </summary>
        public static string GetLanguageDisplayName(string languageCode)
        {
            return languageCode switch
            {
                "zh-CN" => "简体中文",
                "en-US" => "English",
                "de-DE" => "Deutsch",
                _ => languageCode
            };
        }

        /// <summary>
        /// 递归展平 JSON 对象（使用 Newtonsoft.Json）
        /// </summary>
        /// <param name="token">JSON token</param>
        /// <param name="prefix">当前路径前缀</param>
        /// <param name="dict">目标字典</param>
        private void FlattenJson(JToken token, string prefix, Dictionary<string, string> dict)
        {
            if (token is JObject obj)
            {
                foreach (var property in obj.Properties())
                {
                    string key = string.IsNullOrEmpty(prefix) ? property.Name : $"{prefix}.{property.Name}";
                    FlattenJson(property.Value, key, dict);
                }
            }
            else if (token is JValue value)
            {
                dict[prefix] = value.ToString();
            }
        }
    }
}
