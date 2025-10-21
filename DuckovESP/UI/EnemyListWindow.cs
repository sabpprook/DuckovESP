using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DuckovESP.Utils.Localization;

namespace DuckovESP
{
    /// <summary>
    /// 敌人列表UI窗口
    /// 显示所有检测到的敌人详细信息
    /// </summary>
    public class EnemyListWindow
    {
        private readonly ESPConfig _config;
        private bool _showWindow = false;
        private Rect _windowRect = new Rect(100, 100, 550, 400);
        private Vector2 _scrollPosition = Vector2.zero;
        
        // GUI样式缓存
        private GUIStyle _windowStyle;
        private GUIStyle _headerStyle;
        private GUIStyle _labelStyle;
        private GUIStyle _valueStyle;
        private bool _stylesInitialized = false;
        
        public EnemyListWindow(ESPConfig config)
        {
            _config = config;
        }
        
        public void ToggleWindow()
        {
            _showWindow = !_showWindow;
        }
        
        public bool IsWindowVisible => _showWindow;
        
        /// <summary>
        /// 绘制敌人列表窗口
        /// </summary>
        public void DrawWindow(List<EnemyInfo> enemies)
        {
            if (!_showWindow || enemies == null) return;
            
            InitializeStyles();
            
            Color originalColor = GUI.color;
            GUI.color = new Color(1, 1, 1, 0.95f);
            
            _windowRect = GUI.Window(54321, _windowRect, (id) => DrawWindowContent(enemies), 
                LocalizationManager.Get("UI.EnemyList.Title", ("count", enemies.Count.ToString()), ("key", _config.EnemyListToggleKey)), _windowStyle);
            
            GUI.color = originalColor;
            
            // 边界检查
            _windowRect.x = Mathf.Clamp(_windowRect.x, 0, Screen.width - _windowRect.width);
            _windowRect.y = Mathf.Clamp(_windowRect.y, 0, Screen.height - _windowRect.height);
        }
        
        /// <summary>
        /// 绘制窗口内容
        /// </summary>
        private void DrawWindowContent(List<EnemyInfo> enemies)
        {
            GUILayout.BeginVertical();
            
            // 绘制统计信息
            DrawStatistics(enemies);
            
            GUILayout.Space(5);
            
            // 绘制表头
            DrawTableHeader();
            
            // 分隔线
            DrawSeparator();
            
            // 滚动区域 - 敌人列表
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, GUILayout.Height(250));
            
            if (enemies.Count == 0)
            {
                GUILayout.Label(LocalizationManager.Get("UI.EnemyList.NoEnemiesDetected"), _labelStyle);
            }
            else
            {
                foreach (var enemy in enemies)
                {
                    DrawEnemyRow(enemy);
                }
            }
            
            GUILayout.EndScrollView();
            
            GUILayout.Space(5);
            
            // 底部控制按钮
            DrawControls();
            
            GUILayout.EndVertical();
            
            GUI.DragWindow();
        }
        
        /// <summary>
        /// 绘制统计信息
        /// </summary>
        private void DrawStatistics(List<EnemyInfo> enemies)
        {
            GUILayout.BeginHorizontal();
            
            // 总数统计
            GUILayout.Label(LocalizationManager.Get("UI.EnemyList.EnemyCount", ("count", enemies.Count.ToString())), _headerStyle);
            
            GUILayout.FlexibleSpace();
            
            // 总价值统计
            long totalValue = enemies.Sum(e => e.InventoryValue);
            Color valueColor = totalValue >= _config.HighValueThreshold * enemies.Count ? 
                new Color(1f, 0.843f, 0f) : Color.white;
            
            var originalColor = _headerStyle.normal.textColor;
            _headerStyle.normal.textColor = valueColor;
            GUILayout.Label(LocalizationManager.Get("UI.EnemyList.TotalValue", ("value", totalValue.ToString("N0"))), _headerStyle);
            _headerStyle.normal.textColor = originalColor;
            
            GUILayout.EndHorizontal();
            
            // 警告统计
            int aimingCount = enemies.Count(e => e.IsAimingAtPlayer);
            if (aimingCount > 0)
            {
                GUILayout.BeginHorizontal();
                var warningStyle = new GUIStyle(_labelStyle)
                {
                    normal = { textColor = Color.red },
                    fontStyle = FontStyle.Bold
                };
                GUILayout.Label(LocalizationManager.Get("UI.EnemyList.AimingWarning", ("count", aimingCount.ToString())), warningStyle);
                GUILayout.EndHorizontal();
            }
        }
        
        /// <summary>
        /// 绘制表头
        /// </summary>
        private void DrawTableHeader()
        {
            GUILayout.BeginHorizontal();
            
            GUILayout.Label(LocalizationManager.Get("UI.EnemyList.ColumnName"), _headerStyle, GUILayout.Width(80));
            GUILayout.Label(LocalizationManager.Get("UI.EnemyList.ColumnDistance"), _headerStyle, GUILayout.Width(60));
            GUILayout.Label(LocalizationManager.Get("UI.EnemyList.ColumnHealth"), _headerStyle, GUILayout.Width(100));
            GUILayout.Label(LocalizationManager.Get("UI.EnemyList.ColumnWeapon"), _headerStyle, GUILayout.Width(120));
            GUILayout.Label(LocalizationManager.Get("UI.EnemyList.ColumnValue"), _headerStyle, GUILayout.Width(100));
            GUILayout.Label(LocalizationManager.Get("UI.EnemyList.ColumnStatus"), _headerStyle, GUILayout.Width(60));
            
            GUILayout.EndHorizontal();
        }
        
        /// <summary>
        /// 绘制分隔线
        /// </summary>
        private void DrawSeparator()
        {
            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(2));
        }
        
        /// <summary>
        /// 绘制敌人信息行
        /// </summary>
        private void DrawEnemyRow(EnemyInfo enemy)
        {
            GUILayout.BeginHorizontal();
            
            // 名字
            var nameStyle = new GUIStyle(_labelStyle) { normal = { textColor = enemy.DisplayColor } };
            GUILayout.Label(TruncateString(enemy.Name, 10), nameStyle, GUILayout.Width(80));
            
            // 距离
            Color distanceColor = GetDistanceColor(enemy.Distance);
            var distanceStyle = new GUIStyle(_labelStyle) { normal = { textColor = distanceColor } };
            GUILayout.Label($"{enemy.Distance:F0}m", distanceStyle, GUILayout.Width(60));
            
            // 血量
            Color healthColor = GetHealthColor(enemy.HealthPercent);
            var healthStyle = new GUIStyle(_labelStyle) { normal = { textColor = healthColor } };
            string healthBar = CreateHealthBar(enemy.HealthPercent, 8);
            GUILayout.Label($"{healthBar} {enemy.CurrentHealth:F0}", healthStyle, GUILayout.Width(100));
            
            // 武器
            GUILayout.Label(TruncateString(enemy.WeaponName, 12), _labelStyle, GUILayout.Width(120));
            
            // 价值
            Color valueColor = enemy.InventoryValue >= _config.HighValueThreshold ? 
                new Color(1f, 0.843f, 0f) : Color.white;
            var valueStyleLocal = new GUIStyle(_valueStyle) { normal = { textColor = valueColor } };
            GUILayout.Label($"¥{enemy.InventoryValue:N0}", valueStyleLocal, GUILayout.Width(100));
            
            // 状态
            string status = enemy.IsAimingAtPlayer ? LocalizationManager.Get("UI.EnemyList.Aiming") : LocalizationManager.Get("UI.EnemyList.Normal");
            var statusStyle = new GUIStyle(_labelStyle)
            {
                normal = { textColor = enemy.IsAimingAtPlayer ? Color.red : Color.green },
                fontStyle = enemy.IsAimingAtPlayer ? FontStyle.Bold : FontStyle.Normal
            };
            GUILayout.Label(status, statusStyle, GUILayout.Width(60));
            
            GUILayout.EndHorizontal();
        }
        
        /// <summary>
        /// 绘制控制按钮
        /// </summary>
        private void DrawControls()
        {
            GUILayout.BeginHorizontal();
            
            GUILayout.Label(LocalizationManager.Get("UI.EnemyList.Hint", ("key", _config.EnemyListToggleKey)), _labelStyle);
            
            GUILayout.FlexibleSpace();
            
            if (GUILayout.Button(LocalizationManager.Get("UI.Button.Close"), GUILayout.Width(60)))
            {
                _showWindow = false;
            }
            
            GUILayout.EndHorizontal();
        }
        
        #region 辅助函数
        
        /// <summary>
        /// 初始化GUI样式
        /// </summary>
        private void InitializeStyles()
        {
            if (_stylesInitialized) return;
            
            _windowStyle = new GUIStyle(GUI.skin.window)
            {
                fontSize = 14,
                fontStyle = FontStyle.Bold
            };
            
            _headerStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 13,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.white },
                alignment = TextAnchor.MiddleLeft
            };
            
            _labelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 12,
                normal = { textColor = Color.white },
                alignment = TextAnchor.MiddleLeft
            };
            
            _valueStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.white },
                alignment = TextAnchor.MiddleRight
            };
            
            _stylesInitialized = true;
        }
        
        /// <summary>
        /// 根据距离获取颜色
        /// </summary>
        private Color GetDistanceColor(float distance)
        {
            if (distance < 30f) return new Color(1f, 0.3f, 0.3f); // 近距离 - 红色
            if (distance < 100f) return new Color(1f, 0.8f, 0.3f); // 中距离 - 橙色
            return new Color(0.3f, 1f, 0.3f); // 远距离 - 绿色
        }
        
        /// <summary>
        /// 根据血量百分比获取颜色
        /// </summary>
        private Color GetHealthColor(float healthPercent)
        {
            if (healthPercent > 0.7f) return Color.green;
            if (healthPercent > 0.3f) return Color.yellow;
            return Color.red;
        }
        
        /// <summary>
        /// 创建血量条
        /// </summary>
        private string CreateHealthBar(float healthPercent, int length)
        {
            int filled = Mathf.RoundToInt(length * healthPercent);
            
            string bar = "[";
            for (int i = 0; i < length; i++)
            {
                bar += i < filled ? "█" : "░";
            }
            bar += "]";
            
            return bar;
        }
        
        /// <summary>
        /// 截断字符串
        /// </summary>
        private string TruncateString(string str, int maxLength)
        {
            if (string.IsNullOrEmpty(str)) return "";
            return str.Length <= maxLength ? str : str.Substring(0, maxLength - 2) + "..";
        }
        
        #endregion
    }
}
