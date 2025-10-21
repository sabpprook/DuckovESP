using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

namespace DuckovESP.Utils.Localization
{
    /// <summary>
    /// Stores and manages translation data from JSON files.
    /// Supports nested dictionary structure for organized key management.
    /// Provides parameter replacement and fallback mechanisms.
    /// </summary>
    public class LocalizationDataStore
    {
        private readonly Dictionary<string, Dictionary<string, string>> _translations = 
            new Dictionary<string, Dictionary<string, string>>();

        private readonly string _translationPath;
        private const string DefaultLanguage = "en-US";

        public LocalizationDataStore(string translationPath)
        {
            _translationPath = translationPath;
        }

        /// <summary>
        /// Load a translation file for a specific language
        /// </summary>
        public bool LoadLanguage(string languageCode)
        {
            try
            {
                string filePath = Path.Combine(_translationPath, $"{languageCode}.json");
                if (!File.Exists(filePath))
                {
                    Debug.LogWarning($"[Localization] Translation file not found: {filePath}");
                    return false;
                }

                string json = File.ReadAllText(filePath, System.Text.Encoding.UTF8);

                if (!_translations.ContainsKey(languageCode))
                {
                    _translations[languageCode] = new Dictionary<string, string>();
                }

                // Parse JSON manually
                ParseJsonIntoFlat(json, languageCode);

                Debug.Log($"[Localization] Loaded {languageCode}: {_translations[languageCode].Count} keys");
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[Localization] Error loading language {languageCode}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Parse JSON string into flat key-value dictionary
        /// Uses simple parsing suitable for translation files
        /// </summary>
        private void ParseJsonIntoFlat(string json, string languageCode)
        {
            // Remove whitespace and newlines
            json = Regex.Replace(json, @"\s+", " ");

            // Remove outer braces
            json = json.Trim();
            if (json.StartsWith("{")) json = json.Substring(1);
            if (json.EndsWith("}")) json = json.Substring(0, json.Length - 1);

            // Simple recursive parser
            ParseJsonLevel(json, "", languageCode);
        }

        private void ParseJsonLevel(string json, string prefix, string languageCode)
        {
            // Split by commas not inside braces/brackets
            var tokens = SplitJsonTokens(json);

            foreach (var token in tokens)
            {
                string trimmedToken = token.Trim();
                if (string.IsNullOrEmpty(trimmedToken)) continue;

                // Check if this is a nested object or a key-value pair
                if (trimmedToken.Contains(":"))
                {
                    int colonIndex = FindFirstUnenclosedColon(trimmedToken);
                    if (colonIndex > 0)
                    {
                        string keyPart = trimmedToken.Substring(0, colonIndex).Trim();
                        string valuePart = trimmedToken.Substring(colonIndex + 1).Trim();

                        // Clean up key (remove quotes)
                        keyPart = CleanJsonString(keyPart);

                        string fullKey = string.IsNullOrEmpty(prefix) ? keyPart : $"{prefix}.{keyPart}";

                        // Check if value is nested object
                        if (valuePart.StartsWith("{"))
                        {
                            // Nested object - recurse
                            string innerJson = valuePart.Substring(1);
                            if (innerJson.EndsWith("}")) innerJson = innerJson.Substring(0, innerJson.Length - 1);
                            ParseJsonLevel(innerJson, fullKey, languageCode);
                        }
                        else
                        {
                            // Simple value
                            string value = CleanJsonString(valuePart);
                            _translations[languageCode][fullKey] = value;
                        }
                    }
                }
            }
        }

        private List<string> SplitJsonTokens(string json)
        {
            var tokens = new List<string>();
            int braceDepth = 0;
            int bracketDepth = 0;
            int quoteDepth = 0;
            int lastIndex = 0;

            for (int i = 0; i < json.Length; i++)
            {
                char c = json[i];

                // Track quote depth
                if (c == '"' && (i == 0 || json[i - 1] != '\\'))
                {
                    quoteDepth = quoteDepth > 0 ? 0 : 1;
                }

                if (quoteDepth > 0) continue;

                // Track depth
                if (c == '{') braceDepth++;
                else if (c == '}') braceDepth--;
                else if (c == '[') bracketDepth++;
                else if (c == ']') bracketDepth--;
                else if (c == ',' && braceDepth == 0 && bracketDepth == 0)
                {
                    // Found a top-level comma
                    string token = json.Substring(lastIndex, i - lastIndex);
                    if (!string.IsNullOrWhiteSpace(token))
                    {
                        tokens.Add(token);
                    }
                    lastIndex = i + 1;
                }
            }

            // Add remaining token
            if (lastIndex < json.Length)
            {
                string token = json.Substring(lastIndex);
                if (!string.IsNullOrWhiteSpace(token))
                {
                    tokens.Add(token);
                }
            }

            return tokens;
        }

        private int FindFirstUnenclosedColon(string s)
        {
            int braceDepth = 0;
            int bracketDepth = 0;
            bool inQuote = false;

            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];

                if (c == '"' && (i == 0 || s[i - 1] != '\\'))
                {
                    inQuote = !inQuote;
                }

                if (inQuote) continue;

                if (c == '{') braceDepth++;
                else if (c == '}') braceDepth--;
                else if (c == '[') bracketDepth++;
                else if (c == ']') bracketDepth--;
                else if (c == ':' && braceDepth == 0 && bracketDepth == 0)
                {
                    return i;
                }
            }

            return -1;
        }

        private string CleanJsonString(string s)
        {
            s = s.Trim();
            
            // Remove quotes
            if (s.StartsWith("\"") && s.EndsWith("\""))
            {
                s = s.Substring(1, s.Length - 2);
            }

            // Unescape common escape sequences
            s = s.Replace("\\\"", "\"");
            s = s.Replace("\\\\", "\\");
            s = s.Replace("\\n", "\n");
            s = s.Replace("\\r", "\r");
            s = s.Replace("\\t", "\t");

            return s;
        }

        /// <summary>
        /// Get a translation by key with optional parameter replacement
        /// </summary>
        public string Get(string key, string language = DefaultLanguage, string fallbackLanguage = DefaultLanguage, 
            params (string name, object value)[] parameters)
        {
            // Try current language first
            string result = GetFromLanguage(key, language);
            
            // Fallback to fallback language if key not found
            if (result == null && language != fallbackLanguage)
            {
                result = GetFromLanguage(key, fallbackLanguage);
            }

            // If still not found, return the key itself as last resort
            if (result == null)
            {
                result = key;
            }

            // Replace parameters in the string
            // Supports both {name} and {name:format} syntax (e.g., {value:F2}, {count:N0})
            if (parameters != null && parameters.Length > 0)
            {
                foreach (var (name, value) in parameters)
                {
                    // Match both {name} and {name:format} patterns
                    string pattern = $@"\{{{name}(?::([^}}]*))?\}}";
                    result = Regex.Replace(result, pattern, match =>
                    {
                        if (value == null) return "";
                        
                        // Check if format specifier exists
                        string format = match.Groups[1].Value;
                        
                        try
                        {
                            if (!string.IsNullOrEmpty(format))
                            {
                                // Apply format specifier if value supports IFormattable
                                if (value is IFormattable formattable)
                                {
                                    return formattable.ToString(format, CultureInfo.CurrentCulture);
                                }
                                // Otherwise just return the string representation
                                return value.ToString();
                            }
                            else
                            {
                                // No format specifier, just convert to string
                                return value.ToString();
                            }
                        }
                        catch
                        {
                            // If formatting fails, just return the string value
                            return value.ToString();
                        }
                    });
                }
            }

            return result;
        }

        /// <summary>
        /// Get a translation from a specific language
        /// </summary>
        private string GetFromLanguage(string key, string language)
        {
            if (!_translations.ContainsKey(language))
            {
                return null;
            }

            var langDict = _translations[language];
            
            if (langDict.ContainsKey(key))
            {
                return langDict[key];
            }

            return null;
        }

        /// <summary>
        /// Check if a key exists in a language
        /// </summary>
        public bool HasKey(string key, string language = DefaultLanguage)
        {
            return _translations.ContainsKey(language) && 
                   _translations[language].ContainsKey(key);
        }

        /// <summary>
        /// Get all keys for a language
        /// </summary>
        public IEnumerable<string> GetKeys(string language = DefaultLanguage)
        {
            if (_translations.ContainsKey(language))
            {
                return _translations[language].Keys;
            }
            return new List<string>();
        }

        /// <summary>
        /// Get supported languages
        /// </summary>
        public IEnumerable<string> GetSupportedLanguages()
        {
            return _translations.Keys;
        }
    }
}
