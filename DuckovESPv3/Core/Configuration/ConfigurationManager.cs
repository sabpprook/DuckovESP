using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using Newtonsoft.Json;

namespace DuckovESPv3.Core.Configuration
{
    /// <summary>
    /// 配置管理器
    /// </summary>
    /// <remarks>
    /// 架构层级：Core/Configuration
    /// 职责：
    /// - 管理配置的加载、保存、修改
    /// - 通过反射自动序列化/反序列化标注了 ConfigEntry 特性的属性
    /// - 发布配置变更事件
    /// - 性能特征：配置仅在启动时加载一次，之后缓存在内存中
    /// </remarks>
    public class ConfigurationManager : IConfiguration
    {
        private readonly string _configPath;
        private readonly Dictionary<string, object> _configValues = new Dictionary<string, object>();
        private Type? _configType;

        public event Action<string, object>? OnConfigChanged;

        public ConfigurationManager(string modName = "DuckovESPv3")
        {
            // 初始化配置文件路径
            string modFolder = Path.Combine(Application.dataPath, "Mods", modName);
            if (!Directory.Exists(modFolder))
            {
                Directory.CreateDirectory(modFolder);
            }
            _configPath = Path.Combine(modFolder, "config.json");
        }

        /// <summary>
        /// 初始化配置管理器，关联到指定的配置类
        /// </summary>
        /// <typeparam name="T">配置类型，应包含 ConfigEntry 特性标注的属性</typeparam>
        public void Initialize<T>() where T : class, new()
        {
            _configType = typeof(T);
            
            // 从文件加载或使用默认值
            if (File.Exists(_configPath))
            {
                try
                {
                    Load();
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[ConfigurationManager] 加载配置失败: {ex.Message}，使用默认值");
                    ResetToDefault();
                }
            }
            else
            {
                ResetToDefault();
            }
        }

        /// <summary>
        /// 获取配置值
        /// </summary>
        public T? Get<T>(string key, T? defaultValue = default)
        {
            if (_configValues.TryGetValue(key, out var value))
            {
                try
                {
                    return (T?)Convert.ChangeType(value, typeof(T));
                }
                catch
                {
                    return defaultValue;
                }
            }
            return defaultValue;
        }

        /// <summary>
        /// 设置配置值
        /// </summary>
        public void Set<T>(string key, T value)
        {
            object? oldValue = null;
            if (_configValues.TryGetValue(key, out oldValue) && Equals(oldValue, value))
            {
                return; // 值未改变，不发送事件
            }

            _configValues[key] = value!;
            OnConfigChanged?.Invoke(key, value!);
        }

        /// <summary>
        /// 检查配置键是否存在
        /// </summary>
        public bool HasKey(string key)
        {
            return _configValues.ContainsKey(key);
        }

        /// <summary>
        /// 保存配置到文件
        /// </summary>
        public void Save()
        {
            try
            {
                // 使用Newtonsoft.Json序列化Dictionary
                var settings = new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    TypeNameHandling = TypeNameHandling.Auto,
                    NullValueHandling = NullValueHandling.Include
                };
                
                string json = JsonConvert.SerializeObject(_configValues, settings);
                File.WriteAllText(_configPath, json);
                Debug.Log($"[ConfigurationManager] 配置已保存到 {_configPath}，共 {_configValues.Count} 项");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[ConfigurationManager] 保存配置失败: {ex.Message}\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// 从文件加载配置
        /// </summary>
        public void Load()
        {
            try
            {
                if (!File.Exists(_configPath))
                {
                    Debug.LogWarning($"[ConfigurationManager] 配置文件不存在: {_configPath}");
                    return;
                }

                string json = File.ReadAllText(_configPath);
                
                // 使用Newtonsoft.Json反序列化Dictionary
                var settings = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto
                };
                
                var loadedValues = JsonConvert.DeserializeObject<Dictionary<string, object>>(json, settings);
                
                if (loadedValues != null)
                {
                    _configValues.Clear();
                    foreach (var kvp in loadedValues)
                    {
                        _configValues[kvp.Key] = kvp.Value;
                    }
                    Debug.Log($"[ConfigurationManager] 配置已从 {_configPath} 加载，共 {_configValues.Count} 项");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[ConfigurationManager] 加载配置失败: {ex.Message}\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// 重置为默认配置
        /// </summary>
        public void ResetToDefault()
        {
            _configValues.Clear();

            if (_configType != null)
            {
                // 通过反射获取所有标注了 ConfigEntry 的属性
                PropertyInfo[] properties = _configType.GetProperties(
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

                foreach (PropertyInfo prop in properties)
                {
                    var configEntry = prop.GetCustomAttribute<ConfigEntry>();
                    if (configEntry != null && configEntry.DefaultValue != null)
                    {
                        _configValues[configEntry.Key] = configEntry.DefaultValue;
                    }
                }
            }

            Debug.Log("[ConfigurationManager] 配置已重置为默认值");
        }

        /// <summary>
        /// 获取配置类的实例（从当前配置值构建）
        /// </summary>
        /// <typeparam name="T">配置类型</typeparam>
        /// <returns>配置实例</returns>
        public T GetConfigInstance<T>() where T : class, new()
        {
            T instance = new T();
            PropertyInfo[] properties = typeof(T).GetProperties(
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

            foreach (PropertyInfo prop in properties)
            {
                var configEntry = prop.GetCustomAttribute<ConfigEntry>();
                if (configEntry != null && _configValues.TryGetValue(configEntry.Key, out var value))
                {
                    try
                    {
                        prop.SetValue(instance, Convert.ChangeType(value, prop.PropertyType));
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning($"[ConfigurationManager] 设置属性 {prop.Name} 失败: {ex.Message}");
                    }
                }
            }

            return instance;
        }

        /// <summary>
        /// 从配置类实例保存所有配置值
        /// </summary>
        /// <typeparam name="T">配置类型</typeparam>
        /// <param name="instance">配置实例</param>
        public void SetFromInstance<T>(T instance) where T : class
        {
            if (instance == null)
                return;

            PropertyInfo[] properties = typeof(T).GetProperties(
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

            foreach (PropertyInfo prop in properties)
            {
                var configEntry = prop.GetCustomAttribute<ConfigEntry>();
                if (configEntry != null)
                {
                    try
                    {
                        var value = prop.GetValue(instance);
                        _configValues[configEntry.Key] = value;
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning($"[ConfigurationManager] 读取属性 {prop.Name} 失败: {ex.Message}");
                    }
                }
            }
        }
    }
}
