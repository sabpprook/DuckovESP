using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace DuckovESP.Utils.Localization
{
    /// <summary>
    /// Singleton manager for internationalization (i18n) in DuckovESP.
    /// 
    /// Features:
    /// - Auto-detect game language from SodaCraft.Localizations.LocalizationManager
    /// - Fallback to system CultureInfo, then "en-US"
    /// - Runtime language switching
    /// - Parameter replacement in translation strings
    /// - Three-layer fallback mechanism: current language → en-US → key name
    /// 
    /// Usage:
    /// - Initialize: LocalizationManager.Initialize()
    /// - Get translation: LocalizationManager.Get("UI.Menu.Title")
    /// - Get with params: LocalizationManager.Get("Error.Found", ("count", 5))
    /// - Switch language: LocalizationManager.SetLanguage("zh-CN")
    /// </summary>
    public class LocalizationManager : MonoBehaviour
    {
        private static LocalizationManager _instance;
        private LocalizationDataStore _dataStore;
        private string _currentLanguage = "en-US";
        private string _fallbackLanguage = "en-US";
        private string _translationPath;

        // Language display name mapping (game language display names to locale codes)
        private static readonly Dictionary<string, string> LanguageMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "简体中文", "zh-CN" },
            { "English", "en-US" },
            { "日本語", "ja-JP" },
            { "한국어", "ko-KR" },
            { "Русский", "ru-RU" },
            { "Español", "es-ES" },
            { "Français", "fr-FR" },
            { "Deutsch", "de-DE" },
            // Add more mappings as needed
        };

        public static LocalizationManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    Debug.LogError("[Localization] LocalizationManager not initialized! Call Initialize() first.");
                }
                return _instance;
            }
        }

        /// <summary>
        /// Initialize the localization system.
        /// Should be called from ModBehaviour.Awake()
        /// </summary>
        public static void Initialize()
        {
            if (_instance != null)
            {
                return; // Already initialized
            }

            try
            {
                // Create a new GameObject with LocalizationManager component
                GameObject localizationObj = new GameObject("LocalizationManager");
                _instance = localizationObj.AddComponent<LocalizationManager>();
                DontDestroyOnLoad(localizationObj);

                _instance.InitializeLocalization();
            }
            catch (Exception ex)
            {
                Debug.LogError($"[Localization] Initialization failed: {ex.Message}\n{ex.StackTrace}");
            }
        }

        private void InitializeLocalization()
        {
            try
            {
                // Setup translation path - use the directory where DuckovESP.dll is located
                string baseDir = GetModuleDirectory();
                _translationPath = Path.Combine(baseDir, "i18n", "Translations");

                Debug.Log($"[Localization] Module directory: {baseDir}");
                Debug.Log($"[Localization] Translation path: {_translationPath}");

                _dataStore = new LocalizationDataStore(_translationPath);

                // Detect current language
                DetectCurrentLanguage();

                // Load all available languages
                LoadAllAvailableLanguages();

                Debug.Log($"[Localization] Initialized, current language: {_currentLanguage}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[Localization] InitializeLocalization failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Detect current language from game settings or system locale
        /// </summary>
        private void DetectCurrentLanguage()
        {
            try
            {
                // Try to get game's current language
                string gameLanguage = GetGameCurrentLanguage();
                if (!string.IsNullOrEmpty(gameLanguage) && LanguageMapping.ContainsKey(gameLanguage))
                {
                    _currentLanguage = LanguageMapping[gameLanguage];
                    Debug.Log($"[Localization] Detected game language: {gameLanguage} → {_currentLanguage}");
                    return;
                }

                // Fallback to system locale
                string systemLocale = GetSystemLocale();
                if (!string.IsNullOrEmpty(systemLocale))
                {
                    _currentLanguage = systemLocale;
                    Debug.Log($"[Localization] Using system locale: {_currentLanguage}");
                    return;
                }

                // Default to English
                _currentLanguage = "en-US";
                Debug.Log("[Localization] Using default language: en-US");
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[Localization] Error detecting language: {ex.Message}");
                _currentLanguage = "en-US";
            }
        }

        /// <summary>
        /// Get current language from SodaCraft.Localizations.LocalizationManager
        /// </summary>
        private string GetGameCurrentLanguage()
        {
            try
            {
                // Use reflection to access SodaCraft.Localizations.LocalizationManager
                Type localizationType = Type.GetType("SodaCraft.Localizations.LocalizationManager");
                if (localizationType == null)
                {
                    Debug.LogWarning("[Localization] SodaCraft.Localizations.LocalizationManager not found");
                    return null;
                }

                // Get CurrentLanguageDisplayName property
                PropertyInfo displayNameProp = localizationType.GetProperty("CurrentLanguageDisplayName", 
                    BindingFlags.Public | BindingFlags.Static);
                if (displayNameProp == null)
                {
                    Debug.LogWarning("[Localization] CurrentLanguageDisplayName property not found");
                    return null;
                }

                object displayName = displayNameProp.GetValue(null);
                return displayName?.ToString();
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[Localization] Failed to fetch game language: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Get system locale
        /// </summary>
        private string GetSystemLocale()
        {
            try
            {
                CultureInfo culture = CultureInfo.CurrentCulture;
                string locale = culture.Name; // e.g., "zh-CN", "en-US"
                
                // Normalize the locale code
                if (locale.Contains("-"))
                {
                    return locale;
                }

                // If only language code, add default country
                return $"{locale}-{GetCountryForLanguage(locale)}";
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[Localization] Failed to get system locale: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Get default country code for a language code
        /// </summary>
        private string GetCountryForLanguage(string languageCode)
        {
            Dictionary<string, string> countryMap = new Dictionary<string, string>
            {
                { "zh", "CN" },
                { "en", "US" },
                { "ja", "JP" },
                { "ko", "KR" },
                { "ru", "RU" },
                { "es", "ES" },
                { "fr", "FR" },
                { "de", "DE" },
            };

            return countryMap.ContainsKey(languageCode) ? countryMap[languageCode] : "US";
        }

        /// <summary>
        /// Load all available translation files
        /// </summary>
        private void LoadAllAvailableLanguages()
        {
            try
            {
                if (!Directory.Exists(_translationPath))
                {
                    Debug.LogWarning($"[Localization] Translation directory not found: {_translationPath}");
                    return;
                }

                string[] jsonFiles = Directory.GetFiles(_translationPath, "*.json");
                foreach (string file in jsonFiles)
                {
                    string languageCode = Path.GetFileNameWithoutExtension(file);
                    if (_dataStore.LoadLanguage(languageCode))
                    {
                        Debug.Log($"[Localization] Loaded translation: {languageCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[Localization] Error loading translation files: {ex.Message}");
            }
        }

        /// <summary>
        /// Get a translated string by key
        /// </summary>
        public static string Get(string key, params (string name, object value)[] parameters)
        {
            if (Instance == null)
            {
                return key; // Return key itself if not initialized
            }

            return Instance._dataStore.Get(key, Instance._currentLanguage, Instance._fallbackLanguage, parameters);
        }

        /// <summary>
        /// Get a translated string with a specific language override
        /// </summary>
        public static string Get(string key, string language, params (string name, object value)[] parameters)
        {
            if (Instance == null)
            {
                return key;
            }

            return Instance._dataStore.Get(key, language, Instance._fallbackLanguage, parameters);
        }

        /// <summary>
        /// Set the current language
        /// </summary>
        public static void SetLanguage(string languageCode)
        {
            if (Instance == null)
            {
                Debug.LogError("[Localization] Cannot set language, LocalizationManager not initialized");
                return;
            }

            if (Instance._dataStore.HasKey("Language.Supported." + languageCode) || 
                Instance._dataStore.GetSupportedLanguages().Contains(languageCode))
            {
                Instance._currentLanguage = languageCode;
                Debug.Log($"[Localization] Language switched to: {languageCode}");
            }
            else
            {
                Debug.LogWarning($"[Localization] Language not supported: {languageCode}");
            }
        }

        /// <summary>
        /// Get current language code
        /// </summary>
        public static string GetCurrentLanguage()
        {
            return Instance?._currentLanguage ?? "en-US";
        }

        /// <summary>
        /// Get the display name of the current language
        /// </summary>
        public static string GetCurrentLanguageName()
        {
            string currentLang = GetCurrentLanguage();
            return currentLang switch
            {
                "zh-CN" => "中文",
                "en-US" => "English",
                "ja-JP" => "日本語",
                "ko-KR" => "한국어",
                "ru-RU" => "Русский",
                "es-ES" => "Español",
                "fr-FR" => "Français",
                "de-DE" => "Deutsch",
                _ => currentLang
            };
        }

        /// <summary>
        /// Get the display name for a specific language code
        /// </summary>
        public static string GetLanguageDisplayNameByCode(string languageCode)
        {
            return languageCode switch
            {
                "zh-CN" => "中文",
                "en-US" => "English",
                "ja-JP" => "日本語",
                "ko-KR" => "한국어",
                "ru-RU" => "Русский",
                "es-ES" => "Español",
                "fr-FR" => "Français",
                "de-DE" => "Deutsch",
                _ => languageCode
            };
        }

        /// <summary>
        /// Get all supported languages
        /// </summary>
        public static IEnumerable<string> GetSupportedLanguages()
        {
            return Instance?._dataStore.GetSupportedLanguages() ?? new List<string>();
        }

        /// <summary>
        /// Get language display name for a locale code
        /// </summary>
        public static string GetLanguageDisplayName(string languageCode)
        {
            string key = $"Language.Supported.{languageCode}";
            return Get(key);
        }

        /// <summary>
        /// Get the directory where the DuckovESP module is located
        /// Handles both normal assemblies and single-file apps
        /// </summary>
#pragma warning disable IL3000, IL3001, IL3002
        private static string GetModuleDirectory()
        {
            try
            {
                // Strategy 1: Try to get from Assembly.Location (works for normal DLLs)
                var assembly = typeof(LocalizationManager).Assembly;
                try
                {
                    string location = assembly.Location;
                    if (!string.IsNullOrEmpty(location))
                    {
                        string directory = Path.GetDirectoryName(location);
                        if (!string.IsNullOrEmpty(directory) && Directory.Exists(directory))
                        {
                            return directory;
                        }
                    }
                }
                catch
                {
                    // Assembly.Location may not be available in single-file apps
                }

                // Strategy 2: Try CodeBase with proper URI handling (backup for normal DLLs)
                try
                {
#pragma warning disable CA1806
                    string codeBase = assembly.CodeBase;
#pragma warning restore CA1806
                    if (!string.IsNullOrEmpty(codeBase) && codeBase.StartsWith("file://"))
                    {
                        UriBuilder uri = new UriBuilder(codeBase);
                        string path = Uri.UnescapeDataString(uri.Path);
                        string directory = Path.GetDirectoryName(path);
                        if (!string.IsNullOrEmpty(directory) && Directory.Exists(directory))
                        {
                            return directory;
                        }
                    }
                }
                catch
                {
                    // CodeBase may not be available or not valid
                }

                // Strategy 3: Use AppContext.BaseDirectory (most reliable, works everywhere)
                return AppContext.BaseDirectory;
            }
            catch
            {
                // Last resort fallback
                return Environment.CurrentDirectory;
            }
        }
#pragma warning restore IL3000, IL3001, IL3002

        /// <summary>
        /// Check if a key exists
        /// </summary>
        public static bool HasKey(string key, string language = null)
        {
            if (Instance == null) return false;
            
            if (language == null)
            {
                language = Instance._currentLanguage;
            }

            return Instance._dataStore.HasKey(key, language);
        }
    }
}
