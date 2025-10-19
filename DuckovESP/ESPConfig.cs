using System;
using System.IO;
using UnityEngine;

namespace DuckovESP
{
    /// <summary>
    /// ESP配置类 - 可序列化并持久化
    /// </summary>
    [Serializable]
    public class ESPConfig
    {
        // 3D ESP设置
        public bool Enable3DESP = true;
        public float MaxESPDistance = 100f;
        public int ESPFontSize = 14;
        public int MaxDisplayItems = 5;
        public bool ShowDistance = true;
        public bool ShowConnectLine = true;
        
        // 小地图标记设置
        public bool EnableMapMarkers = true;
        public int MinQualityForMapMarkers = 4; // 0-6: 0=白 1=绿 2=蓝 3=紫 4=橙 5=浅红 6=红
        
        // 过滤设置
        public int MinQualityFilter3D = 0; // 3D ESP的最小品质过滤 (0-6)
        public bool ShowEmptyBoxes = false;
        
        // 显示设置
        public float BackgroundAlpha = 0.7f;
        public float BorderThickness = 2f;
        
        // 菜单设置
        public KeyCode MenuToggleKey = KeyCode.F5;
        
        /// <summary>
        /// 配置文件路径
        /// </summary>
        private static string ConfigFilePath
        {
            get
            {
                string modFolder = Path.Combine(Application.dataPath, "Mods", "DuckovESP");
                if (!Directory.Exists(modFolder))
                {
                    Directory.CreateDirectory(modFolder);
                }
                return Path.Combine(modFolder, "config.json");
            }
        }
        
        /// <summary>
        /// 保存配置到文件
        /// </summary>
        public void Save()
        {
            try
            {
                string json = JsonUtility.ToJson(this, true);
                File.WriteAllText(ConfigFilePath, json);
                Debug.Log($"DuckovESP: 配置已保存到 {ConfigFilePath}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"DuckovESP: 保存配置失败 - {ex.Message}");
            }
        }
        
        /// <summary>
        /// 从文件加载配置
        /// </summary>
        public static ESPConfig Load()
        {
            try
            {
                if (File.Exists(ConfigFilePath))
                {
                    string json = File.ReadAllText(ConfigFilePath);
                    ESPConfig config = JsonUtility.FromJson<ESPConfig>(json);
                    Debug.Log($"DuckovESP: 配置已从 {ConfigFilePath} 加载");
                    return config;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"DuckovESP: 加载配置失败 - {ex.Message}");
            }
            
            // 返回默认配置
            Debug.Log("DuckovESP: 使用默认配置");
            return new ESPConfig();
        }
        
        /// <summary>
        /// 重置为默认配置
        /// </summary>
        public void ResetToDefault()
        {
            Enable3DESP = true;
            MaxESPDistance = 100f;
            ESPFontSize = 14;
            MaxDisplayItems = 5;
            ShowDistance = true;
            ShowConnectLine = true;
            EnableMapMarkers = true;
            MinQualityForMapMarkers = 4;
            MinQualityFilter3D = 0;
            ShowEmptyBoxes = false;
            BackgroundAlpha = 0.7f;
            BorderThickness = 2f;
            MenuToggleKey = KeyCode.F5;
        }
    }
}
