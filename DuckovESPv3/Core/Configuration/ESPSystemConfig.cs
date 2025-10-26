using UnityEngine;

namespace DuckovESPv3.Core.Configuration
{
    /// <summary>
    /// ESP系统配置
    /// </summary>
    /// <remarks>
    /// 架构层级：Core/Configuration
    /// 包含所有ESP功能的配置项
    /// 所有配置都通过 ConfigEntry 特性标注，支持自动序列化/反序列化
    /// </remarks>
    public class ESPSystemConfig
    {
        // ==================== 基础设置 ====================
        
        /// <summary>
        /// 菜单切换快捷键
        /// </summary>
        [ConfigEntry("Menu.ToggleKey", DefaultValue = KeyCode.F5)]
        public KeyCode MenuToggleKey { get; set; } = KeyCode.F5;

        /// <summary>
        /// 是否启用调试日志
        /// </summary>
        [ConfigEntry("Debug.Enabled", DefaultValue = false)]
        public bool DebugEnabled { get; set; } = false;

        /// <summary>
        /// 用户界面语言
        /// </summary>
        [ConfigEntry("UI.Language", DefaultValue = "zh-CN")]
        public string Language { get; set; } = "zh-CN";

        /// <summary>
        /// UI缩放比例（0.5 - 2.0）
        /// </summary>
        [ConfigEntry("UI.Scale", DefaultValue = 1.0f)]
        public float UIScale { get; set; } = 1.0f;

        // ==================== 3D ESP设置 ====================

        /// <summary>
        /// 启用3D ESP透视（总开关）
        /// </summary>
        [ConfigEntry("ESP.Enable3D", DefaultValue = true)]
        public bool Enable3DESP { get; set; } = true;
        
        /// <summary>
        /// 是否启用物资ESP（箱子和地面物品）
        /// </summary>
        [ConfigEntry("ESP.EnableLoot", DefaultValue = true)]
        public bool EnableLootESP { get; set; } = true;

        /// <summary>
        /// ESP渲染模式（0=世界空间World Space，1=屏幕空间Screen Overlay）
        /// 世界空间：可能有遮挡，但性能高
        /// 屏幕空间：无遮挡，但性能较低（约-30% FPS）
        /// </summary>
        [ConfigEntry("ESP.RenderMode", DefaultValue = 1)]
        public int ESPRenderMode { get; set; } = 1;

        /// <summary>
        /// 3D ESP最大显示距离（米）
        /// </summary>
        [ConfigEntry("ESP.MaxDistance", DefaultValue = 100f)]
        public float MaxESPDistance { get; set; } = 100f;

        /// <summary>
        /// ESP标签字体大小
        /// </summary>
        [ConfigEntry("ESP.FontSize", DefaultValue = 14)]
        public int ESPFontSize { get; set; } = 14;

        /// <summary>
        /// 最多显示的物品数量
        /// </summary>
        [ConfigEntry("ESP.MaxDisplayItems", DefaultValue = 5)]
        public int MaxDisplayItems { get; set; } = 5;

        /// <summary>
        /// 是否显示距离
        /// </summary>
        [ConfigEntry("ESP.ShowDistance", DefaultValue = true)]
        public bool ShowDistance { get; set; } = true;

        /// <summary>
        /// 是否显示连接线
        /// </summary>
        [ConfigEntry("ESP.ShowConnectLine", DefaultValue = true)]
        public bool ShowConnectLine { get; set; } = true;

        // ==================== 小地图标记设置 ====================

        /// <summary>
        /// 启用小地图标记
        /// </summary>
        [ConfigEntry("MapMarkers.Enabled", DefaultValue = true)]
        public bool EnableMapMarkers { get; set; } = true;

        /// <summary>
        /// 小地图标记最小品质等级（0-6）
        /// 0=白色(普通) 1=绿色(稀有) 2=蓝色(精良) 3=紫色(史诗) 4=橙色(传说) 5=浅红(神话) 6=红色(至尊)
        /// </summary>
        [ConfigEntry("MapMarkers.MinQuality", DefaultValue = 1)]
        public int MinQualityForMapMarkers { get; set; } = 1;

        // ==================== 过滤设置 ====================

        /// <summary>
        /// 3D ESP最小品质过滤（0-6）
        /// </summary>
        [ConfigEntry("Filter.MinQuality3D", DefaultValue = 0)]
        public int MinQualityFilter3D { get; set; } = 0;

        /// <summary>
        /// 是否显示空的箱子
        /// </summary>
        [ConfigEntry("Filter.ShowEmptyBoxes", DefaultValue = false)]
        public bool ShowEmptyBoxes { get; set; } = false;
        
        /// <summary>
        /// 是否按价值显示物品
        /// </summary>
        [ConfigEntry("Filter.ShowByValue", DefaultValue = false)]
        public bool ShowByValue { get; set; } = false;
        
        /// <summary>
        /// 是否显示心愿单物品
        /// </summary>
        [ConfigEntry("Filter.ShowWishlistedItems", DefaultValue = false)]
        public bool ShowWishlistedItems { get; set; } = false;
        
        /// <summary>
        /// 是否显示任务物品
        /// </summary>
        [ConfigEntry("Filter.ShowQuestItems", DefaultValue = false)]
        public bool ShowQuestItems { get; set; } = false;
        
        /// <summary>
        /// 是否显示任务地点标记
        /// </summary>
        [ConfigEntry("QuestMarker.ShowQuestZones", DefaultValue = true)]
        public bool ShowQuestZones { get; set; } = true;
        
        /// <summary>
        /// 是否显示撤离点指示器
        /// </summary>
        [ConfigEntry("QuestMarker.ShowEvacuationPoints", DefaultValue = true)]
        public bool ShowEvacuationPoints { get; set; } = true;
        
        /// <summary>
        /// 任务地点标记最大显示距离（米）
        /// </summary>
        [ConfigEntry("QuestMarker.QuestZoneMaxDistance", DefaultValue = 500f)]
        public float QuestZoneMaxDistance { get; set; } = 500f;
        
        /// <summary>
        /// 任务物品高亮颜色（R,G,B,A格式字符串）
        /// 默认金黄色：1,0.843,0,1
        /// </summary>
        [ConfigEntry("QuestMarker.QuestItemColor", DefaultValue = "1,0.843,0,1")]
        public string QuestItemColorString { get; set; } = "1,0.843,0,1";
        
        /// <summary>
        /// 任务地点标记颜色（R,G,B,A格式字符串）
        /// 默认青色：0.2,0.8,1,1
        /// </summary>
        [ConfigEntry("QuestMarker.QuestZoneColor", DefaultValue = "0.2,0.8,1,1")]
        public string QuestZoneColorString { get; set; } = "0.2,0.8,1,1";
        
        /// <summary>
        /// 任务物品高亮颜色（运行时解析）
        /// </summary>
        public Color QuestItemColor
        {
            get => ParseColor(QuestItemColorString, new Color(1f, 0.843f, 0f, 1f));
            set => QuestItemColorString = ColorToString(value);
        }
        
        /// <summary>
        /// 任务地点标记颜色（运行时解析）
        /// </summary>
        public Color QuestZoneColor
        {
            get => ParseColor(QuestZoneColorString, new Color(0.2f, 0.8f, 1f, 1f));
            set => QuestZoneColorString = ColorToString(value);
        }
        
        /// <summary>
        /// 是否显示建筑材料
        /// </summary>
        [ConfigEntry("Filter.ShowBuildingMaterials", DefaultValue = false)]
        public bool ShowBuildingMaterials { get; set; } = false;
        
        /// <summary>
        /// 是否显示未录入心愿单的钥匙
        /// 默认false（隐藏未录入的钥匙，减少干扰）
        /// </summary>
        [ConfigEntry("Filter.ShowNotWishlistedKeys", DefaultValue = false)]
        public bool ShowNotWishlistedKeys { get; set; } = false;

        // ==================== 显示设置 ====================

        /// <summary>
        /// 背景透明度（0-1）
        /// </summary>
        [ConfigEntry("Display.BackgroundAlpha", DefaultValue = 0.7f)]
        public float BackgroundAlpha { get; set; } = 0.7f;

        /// <summary>
        /// 边框厚度（像素）
        /// </summary>
        [ConfigEntry("Display.BorderThickness", DefaultValue = 2f)]
        public float BorderThickness { get; set; } = 2f;

        // ==================== 敌人ESP设置 ====================

        /// <summary>
        /// 启用敌人ESP
        /// </summary>
        [ConfigEntry("EnemyESP.Enabled", DefaultValue = true)]
        public bool EnableEnemyESP { get; set; } = true;

        /// <summary>
        /// 敌人ESP最大显示距离（米）
        /// </summary>
        [ConfigEntry("EnemyESP.MaxDistance", DefaultValue = 300f)]
        public float MaxEnemyESPDistance { get; set; } = 300f;

        /// <summary>
        /// 是否显示敌人生命值
        /// </summary>
        [ConfigEntry("EnemyESP.ShowHealth", DefaultValue = true)]
        public bool ShowEnemyHealth { get; set; } = true;

        /// <summary>
        /// 是否显示敌人武器
        /// </summary>
        [ConfigEntry("EnemyESP.ShowWeapon", DefaultValue = true)]
        public bool ShowEnemyWeapon { get; set; } = true;

        /// <summary>
        /// 是否显示敌人价值
        /// </summary>
        [ConfigEntry("EnemyESP.ShowValue", DefaultValue = true)]
        public bool ShowEnemyValue { get; set; } = true;

        /// <summary>
        /// 是否显示敌人连接线
        /// </summary>
        [ConfigEntry("EnemyESP.ShowLines", DefaultValue = true)]
        public bool EnableEnemyLines { get; set; } = true;

        /// <summary>
        /// 敌人连接线最大距离（米）
        /// </summary>
        [ConfigEntry("EnemyESP.MaxLineDistance", DefaultValue = 100f)]
        public float MaxEnemyLineDistance { get; set; } = 100f;

        /// <summary>
        /// 敌人连接线宽度（像素）
        /// </summary>
        [ConfigEntry("EnemyESP.LineWidth", DefaultValue = 2f)]
        public float EnemyLineWidth { get; set; } = 2f;

        /// <summary>
        /// 敌人列表窗口切换快捷键
        /// </summary>
        [ConfigEntry("EnemyESP.ListToggleKey", DefaultValue = KeyCode.F6)]
        public KeyCode EnemyListToggleKey { get; set; } = KeyCode.F6;
        
        // ==================== 辅助方法 ====================
        
        /// <summary>
        /// 解析颜色字符串（R,G,B,A格式）
        /// </summary>
        private Color ParseColor(string colorString, Color defaultColor)
        {
            if (string.IsNullOrEmpty(colorString))
            {
                return defaultColor;
            }
            
            try
            {
                var parts = colorString.Split(',');
                if (parts.Length >= 3)
                {
                    float r = float.Parse(parts[0].Trim());
                    float g = float.Parse(parts[1].Trim());
                    float b = float.Parse(parts[2].Trim());
                    float a = parts.Length >= 4 ? float.Parse(parts[3].Trim()) : 1f;
                    return new Color(r, g, b, a);
                }
            }
            catch
            {
                // 解析失败，返回默认值
            }
            
            return defaultColor;
        }
        
        /// <summary>
        /// 将颜色转换为字符串（R,G,B,A格式）
        /// </summary>
        private string ColorToString(Color color)
        {
            return $"{color.r},{color.g},{color.b},{color.a}";
        }
    }
}
