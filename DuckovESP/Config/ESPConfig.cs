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
        
        // 自动瞄准设置（Aimbot）
        public bool EnableAimbot = false;
        public KeyCode AimbotKey = KeyCode.Mouse1; // 右键瞄准
        public float AimbotFOV = 300f; // 瞄准半径（像素），建议值 200-500
        public float AimbotSmoothness = 5f; // 平滑度（1-20）
        public bool AimbotPredictMovement = true; // 预测目标移动
        public bool AimbotAimAtHead = true; // 瞄准头部
        public float AimbotMaxDistance = 200f; // 最大瞄准距离
        public bool AimbotIgnoreWalls = false; // 是否忽略墙壁（穿墙）
        public bool AimbotIgnoreTeamCheck = false; // 测试模式：忽略队伍检查（可攻击友军，用于测试墙体检测）
        
        // 自动扳机设置（Trigger Bot）
        public bool EnableTriggerBot = false;
        public float TriggerBotDelay = 0.05f; // 延迟（秒）避免检测
        public bool TriggerBotOnlyADS = true; // 仅在瞄准时触发
        public bool TriggerBotTeamCheck = true; // 检查队友
        
        // 敌人ESP设置
        public bool EnableEnemyESP = true;
        public float MaxEnemyESPDistance = 300f;
        public bool ShowEnemyHealth = true;
        public bool ShowEnemyWeapon = true;
        public bool ShowEnemyValue = true;
        public bool EnableEnemyLines = true;
        public float MaxEnemyLineDistance = 100f;
        public float EnemyLineWidth = 2f;
        
        // 敌人列表窗口
        public KeyCode EnemyListToggleKey = KeyCode.F6;
        
        // 作弊功能快捷键
        public KeyCode GodModeKey = KeyCode.F7; // 无敌模式
        public KeyCode OneHitKillKey = KeyCode.F8; // 一击必杀
        public KeyCode SpeedBoostKey = KeyCode.F9; // 速度提升
        public KeyCode InfiniteWeightKey = KeyCode.F10; // 无限负重
        public KeyCode InfiniteAmmoKey = KeyCode.F11; // 无限子弹
        public KeyCode InfiniteStaminaKey = KeyCode.F12; // 无限耐力
        
        // 任务物品和建筑材料标记
        public bool HighlightQuestItems = true; // 高亮任务物品
        public bool HighlightBuildingMaterials = true; // 高亮建筑材料
        public Color QuestItemColor = Color.yellow; // 任务物品颜色
        public Color BuildingMaterialColor = Color.cyan; // 建筑材料颜色
        
        // 敌人警报设置
        public bool EnableHighValueAlert = true;
        public long HighValueThreshold = 50000; // 高价值阈值（¥50000）
        public bool EnableTraderAlert = true;
        
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
            
            // 敌人ESP默认配置
            EnableEnemyESP = true;
            MaxEnemyESPDistance = 300f;
            ShowEnemyHealth = true;
            ShowEnemyWeapon = true;
            ShowEnemyValue = true;
            EnableEnemyLines = true;
            MaxEnemyLineDistance = 100f;
            EnemyLineWidth = 2f;
            EnemyListToggleKey = KeyCode.F6;
            EnableHighValueAlert = true;
            HighValueThreshold = 50000;
            EnableTraderAlert = true;
            
            // 任务物品和建筑材料
            HighlightQuestItems = true;
            HighlightBuildingMaterials = true;
            QuestItemColor = Color.yellow;
            BuildingMaterialColor = Color.cyan;
            
            // 自动瞄准默认关闭
            EnableAimbot = false;
            AimbotKey = KeyCode.Mouse1;
            AimbotFOV = 300f; // 像素半径
            AimbotSmoothness = 5f;
            AimbotPredictMovement = true;
            AimbotAimAtHead = true;
            AimbotMaxDistance = 200f;
            AimbotIgnoreWalls = false;
            
            // 自动扳机默认关闭
            EnableTriggerBot = false;
            TriggerBotDelay = 0.05f;
            TriggerBotOnlyADS = true;
            TriggerBotTeamCheck = true;
            
            // 作弊功能快捷键
            GodModeKey = KeyCode.F7;
            OneHitKillKey = KeyCode.F8;
            SpeedBoostKey = KeyCode.F9;
            InfiniteWeightKey = KeyCode.F10;
            InfiniteAmmoKey = KeyCode.F11;
            InfiniteStaminaKey = KeyCode.F12;
        }
    }
}
