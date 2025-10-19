using System.Collections.Generic;
using UnityEngine;

namespace DuckovESP
{
    /// <summary>
    /// 敌人ESP渲染器
    /// 负责绘制3D ESP标记和连线
    /// </summary>
    public class EnemyESPRenderer
    {
        private readonly ESPConfig _config;
        private Material _lineMaterial;
        private static Texture2D _whiteTexture;
        
        public EnemyESPRenderer(ESPConfig config)
        {
            _config = config;
            InitializeLineMaterial();
        }
        
        /// <summary>
        /// 初始化GL线条材质
        /// </summary>
        private void InitializeLineMaterial()
        {
            if (_lineMaterial == null)
            {
                var shader = Shader.Find("Hidden/Internal-Colored") ?? Shader.Find("Sprites/Default");
                if (shader != null)
                {
                    _lineMaterial = new Material(shader)
                    {
                        hideFlags = HideFlags.HideAndDontSave
                    };
                    _lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    _lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    _lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
                    _lineMaterial.SetInt("_ZWrite", 0);
                }
            }
            
            if (_whiteTexture == null)
            {
                _whiteTexture = new Texture2D(1, 1);
                _whiteTexture.SetPixel(0, 0, Color.white);
                _whiteTexture.Apply();
            }
        }
        
        /// <summary>
        /// 绘制3D ESP标记（在OnGUI中调用）
        /// </summary>
        public void DrawESPLabels(List<EnemyInfo> enemies, Camera camera, GUIStyle textStyle)
        {
            if (!_config.EnableEnemyESP || enemies == null || camera == null) return;
            
            foreach (var enemy in enemies)
            {
                if (!enemy.IsAlive) continue;
                
                Vector3 screenPos = camera.WorldToScreenPoint(enemy.HeadPosition);
                
                // 检查是否在屏幕前方
                if (screenPos.z <= 0) continue;
                
                // 转换为GUI坐标系（左上角为原点）
                screenPos.y = Screen.height - screenPos.y;
                
                // 绘制ESP文本
                DrawEnemyLabel(enemy, screenPos, textStyle);
            }
        }
        
        /// <summary>
        /// 绘制单个敌人的ESP标签
        /// </summary>
        private void DrawEnemyLabel(EnemyInfo enemy, Vector3 screenPos, GUIStyle textStyle)
        {
            // 构建显示文本
            var lines = new List<string>();
            
            // 名字和距离
            lines.Add($"{enemy.Name} [{enemy.Distance:F0}m]");
            
            // 血量条和数值
            if (_config.ShowEnemyHealth)
            {
                string healthBar = CreateHealthBar(enemy.HealthPercent);
                lines.Add($"{healthBar} {enemy.CurrentHealth:F0}/{enemy.MaxHealth:F0}");
            }
            
            // 武器
            if (_config.ShowEnemyWeapon && !string.IsNullOrEmpty(enemy.WeaponName) && enemy.WeaponName != "无")
            {
                lines.Add($"🔫 {enemy.WeaponName}");
            }
            
            // 库存价值
            if (_config.ShowEnemyValue && enemy.InventoryValue > 0)
            {
                lines.Add($"💰 ¥{enemy.InventoryValue:N0}");
            }
            
            // 瞄准警告
            if (enemy.IsAimingAtPlayer)
            {
                lines.Add("⚠️ 正在瞄准你!");
            }
            
            // 绘制背景框
            float totalHeight = lines.Count * (textStyle.fontSize + 2);
            float maxWidth = 0f;
            
            foreach (var line in lines)
            {
                Vector2 size = textStyle.CalcSize(new GUIContent(line));
                if (size.x > maxWidth) maxWidth = size.x;
            }
            
            Rect boxRect = new Rect(
                screenPos.x - maxWidth / 2 - 5,
                screenPos.y - 5,
                maxWidth + 10,
                totalHeight + 10
            );
            
            // 绘制半透明背景
            Color originalColor = GUI.color;
            GUI.color = new Color(0, 0, 0, 0.7f);
            GUI.DrawTexture(boxRect, _whiteTexture);
            GUI.color = originalColor;
            
            // 绘制文本
            float yOffset = screenPos.y;
            foreach (var line in lines)
            {
                // 根据内容设置颜色
                Color textColor = enemy.DisplayColor;
                
                if (line.Contains("⚠️"))
                {
                    textColor = Color.red;
                }
                else if (line.Contains("💰") && enemy.InventoryValue >= _config.HighValueThreshold)
                {
                    textColor = new Color(1f, 0.843f, 0f); // 金色
                }
                else if (line.Contains("🔫"))
                {
                    textColor = Color.white;
                }
                
                DrawTextWithOutline(line, screenPos.x, yOffset, textStyle, textColor);
                yOffset += textStyle.fontSize + 2;
            }
        }
        
        /// <summary>
        /// 创建血量条
        /// </summary>
        private string CreateHealthBar(float healthPercent)
        {
            int barLength = 10;
            int filled = Mathf.RoundToInt(barLength * healthPercent);
            
            string bar = "[";
            for (int i = 0; i < barLength; i++)
            {
                bar += i < filled ? "█" : "░";
            }
            bar += "]";
            
            return bar;
        }
        
        /// <summary>
        /// 绘制带描边的文本
        /// </summary>
        private void DrawTextWithOutline(string text, float x, float y, GUIStyle style, Color textColor)
        {
            Vector2 size = style.CalcSize(new GUIContent(text));
            Rect rect = new Rect(x - size.x / 2, y, size.x, size.y);
            
            // 描边
            Color originalColor = style.normal.textColor;
            style.normal.textColor = Color.black;
            
            for (int ox = -1; ox <= 1; ox++)
            {
                for (int oy = -1; oy <= 1; oy++)
                {
                    if (ox == 0 && oy == 0) continue;
                    GUI.Label(new Rect(rect.x + ox, rect.y + oy, rect.width, rect.height), text, style);
                }
            }
            
            // 主文本
            style.normal.textColor = textColor;
            GUI.Label(rect, text, style);
            
            style.normal.textColor = originalColor;
        }
        
        /// <summary>
        /// 绘制从玩家到敌人的连线（在OnRenderObject中调用）
        /// </summary>
        public void DrawLines(List<EnemyInfo> enemies, CharacterMainControl player, Camera camera)
        {
            if (!_config.EnableEnemyLines || enemies == null || player?.transform == null || _lineMaterial == null) 
                return;
            
            Vector3 playerPos = player.transform.position + Vector3.up * 1.5f;
            
            GL.PushMatrix();
            _lineMaterial.SetPass(0);
            GL.LoadOrtho();
            GL.Begin(GL.LINES);
            
            foreach (var enemy in enemies)
            {
                if (!enemy.IsAlive) continue;
                if (enemy.Distance > _config.MaxEnemyLineDistance) continue;
                
                Vector3 playerScreen = camera.WorldToScreenPoint(playerPos);
                Vector3 enemyScreen = camera.WorldToScreenPoint(enemy.HeadPosition);
                
                // 检查是否在屏幕前方
                if (playerScreen.z <= 0 || enemyScreen.z <= 0) continue;
                
                // 转换为GL坐标 (0-1范围)
                Vector2 p1 = new Vector2(playerScreen.x / Screen.width, playerScreen.y / Screen.height);
                Vector2 p2 = new Vector2(enemyScreen.x / Screen.width, enemyScreen.y / Screen.height);
                
                // 设置线条颜色
                Color lineColor = enemy.IsAimingAtPlayer ? Color.red : enemy.DisplayColor;
                lineColor.a = 0.6f; // 半透明
                GL.Color(lineColor);
                
                // 绘制粗线条（多重绘制）
                DrawThickLine(p1, p2, _config.EnemyLineWidth);
            }
            
            GL.End();
            GL.PopMatrix();
        }
        
        /// <summary>
        /// 绘制粗线条
        /// </summary>
        private void DrawThickLine(Vector2 p1, Vector2 p2, float width)
        {
            float offset = width / Screen.width;
            
            // 主线
            GL.Vertex3(p1.x, p1.y, 0f);
            GL.Vertex3(p2.x, p2.y, 0f);
            
            // 增加厚度（绘制多条偏移线）
            for (int i = 1; i <= 2; i++)
            {
                float o = offset * i;
                
                // 水平偏移
                GL.Vertex3(p1.x + o, p1.y, 0f);
                GL.Vertex3(p2.x + o, p2.y, 0f);
                
                GL.Vertex3(p1.x - o, p1.y, 0f);
                GL.Vertex3(p2.x - o, p2.y, 0f);
                
                // 垂直偏移
                GL.Vertex3(p1.x, p1.y + o, 0f);
                GL.Vertex3(p2.x, p2.y + o, 0f);
                
                GL.Vertex3(p1.x, p1.y - o, 0f);
                GL.Vertex3(p2.x, p2.y - o, 0f);
            }
        }
        
        /// <summary>
        /// 清理资源
        /// </summary>
        public void Dispose()
        {
            if (_lineMaterial != null)
            {
                Object.DestroyImmediate(_lineMaterial);
                _lineMaterial = null;
            }
        }
    }
}
