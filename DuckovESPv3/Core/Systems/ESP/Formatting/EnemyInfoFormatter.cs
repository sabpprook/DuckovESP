using System.Text;
using UnityEngine;
using DuckovESPv3.Core.Systems.ESP.Models;
using DuckovESPv3.Core.Configuration;

namespace DuckovESPv3.Core.Systems.ESP.Formatting
{
    /// <summary>
    /// 敌人信息格式化工具
    /// </summary>
    public static class EnemyInfoFormatter
    {
        /// <summary>
        /// 格式化敌人信息为多行文本
        /// </summary>
        public static string FormatEnemyInfo(EnemyData enemy, bool showDistance, ESPSystemConfig config)
        {
            var sb = new StringBuilder();
            
            // 第1行：名称（加粗）
            sb.AppendLine($"<b>{enemy.Name}</b>");
            
            // 第2行：血条（彩色）- 根据配置显示
            if (config.ShowEnemyHealth)
            {
                string healthColor = GetHealthColorHex(enemy.HealthPercent);
                sb.AppendLine($"<color={healthColor}>HP {enemy.CurrentHealth:F0}/{enemy.MaxHealth:F0} ({enemy.HealthPercent:P0})</color>");
            }
            
            // 第3行：武器 - 根据配置显示
            if (config.ShowEnemyWeapon && !string.IsNullOrEmpty(enemy.WeaponName) && enemy.WeaponName != "无")
            {
                sb.AppendLine($"[武器] {enemy.WeaponName}");
            }
            
            // 第4行：价值 - 根据配置显示
            if (config.ShowEnemyValue && enemy.InventoryValue > 0)
            {
                sb.AppendLine($"[价值] ¥{enemy.InventoryValue:N0}");
            }
            
            // 第5行：距离
            if (showDistance)
            {
                sb.Append($"[距离] {enemy.DistanceToPlayer:F1}m");
            }
            
            return sb.ToString();
        }
        
        /// <summary>
        /// 根据血量百分比返回颜色十六进制
        /// </summary>
        private static string GetHealthColorHex(float healthPercent)
        {
            if (healthPercent > 0.6f) return "#00FF00"; // 绿色（健康）
            if (healthPercent > 0.3f) return "#FFFF00"; // 黄色（受伤）
            return "#FF0000"; // 红色（濒死）
        }
        
        /// <summary>
        /// 根据威胁等级和类型返回标记颜色
        /// </summary>
        public static Color GetEnemyColor(EnemyData enemy)
        {
            // Boss 优先显示紫色
            if (enemy.IsBoss)
            {
                return new Color(1f, 0f, 1f); // 紫色 - Boss
            }
            
            // 根据威胁等级
            switch (enemy.ThreatLevel)
            {
                case 5: return new Color(1f, 0f, 0f);       // 红色 - 极度危险
                case 4: return new Color(1f, 0.5f, 0f);     // 橙色 - 高危
                case 3: return new Color(1f, 1f, 0f);       // 黄色 - 中危
                case 2: return new Color(0.5f, 1f, 0.5f);   // 浅绿 - 低危
                default: return Color.white;                 // 白色 - 无威胁
            }
        }
        
        /// <summary>
        /// 获取敌人连接线颜色（红色系）
        /// </summary>
        public static Color GetEnemyLineColor(EnemyData enemy)
        {
            if (enemy.IsBoss)
            {
                return new Color(1f, 0f, 1f); // 紫色
            }
            
            // 敌人统一使用红色系
            return new Color(1f, 0.2f, 0.2f);
        }
    }
}
