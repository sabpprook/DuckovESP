using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace YKF_ESP
{
	// Token: 0x02000017 RID: 23
	[NullableContext(1)]
	[Nullable(0)]
	public class UIManager
	{
		// Token: 0x060000BD RID: 189 RVA: 0x000068FC File Offset: 0x00004AFC
		public UIManager(ESPSettings settings, LogManager logManager, ConfigManager configManager = null)
		{
			this.settings = settings;
			this.logManager = logManager;
			this.configManager = configManager;
			this.windowRect = new Rect(settings.WindowPosX, settings.WindowPosY, settings.WindowWidth * 1.1f, settings.WindowHeight * 1.1f);
		}

		// Token: 0x060000BE RID: 190 RVA: 0x00006970 File Offset: 0x00004B70
		public void UpdateSettings(ESPSettings newSettings)
		{
			newSettings.WindowPosX = this.windowRect.x;
			newSettings.WindowPosY = this.windowRect.y;
			this.settings = newSettings;
			this.stylesInitialized = false;
			this.windowRect.width = this.settings.WindowWidth * 1.1f;
			this.windowRect.height = this.settings.WindowHeight * 1.1f;
		}

		// Token: 0x060000BF RID: 191 RVA: 0x000069E5 File Offset: 0x00004BE5
		public void ToggleWindow()
		{
			this.showWindow = !this.showWindow;
		}

		// Token: 0x060000C0 RID: 192 RVA: 0x000069F6 File Offset: 0x00004BF6
		public void UpdateEnemyData(List<EnemyInfo> newEnemyInfoList)
		{
			this.enemyInfoList = newEnemyInfoList;
		}

		// Token: 0x060000C1 RID: 193 RVA: 0x00006A00 File Offset: 0x00004C00
		public void OnGUI()
		{
			this.InitializeGUIStyles();
			if (this.showWindow && this.settings.ShowEnemyList && this.enemyInfoList.Count > 0)
			{
				Color color = GUI.color;
				GUI.color = new Color(color.r, color.g, color.b, this.settings.WindowOpacity);
				Rect rect = GUI.Window(12345, this.windowRect, new GUI.WindowFunction(this.DrawEnemyWindow), "YKF ESP - 敌人列表 (" + this.settings.ToggleKey + "切换)", this.windowStyle);
				if (rect.x != this.windowRect.x || rect.y != this.windowRect.y)
				{
					this.windowRect = rect;
					this.SaveWindowPositionIfNeeded();
				}
				GUI.color = color;
			}
			if (this.showWindow)
			{
				string text = "按" + this.settings.ToggleKey + "键切换ESP窗口";
				text = text + " | 按" + this.settings.MagicBulletToggleKey + "键切换魔法子弹";
				GUI.Label(new Rect(10f, (float)(Screen.height - 30), 500f, 20f), text, GUI.skin.label);
			}
		}

		// Token: 0x060000C2 RID: 194 RVA: 0x00006B50 File Offset: 0x00004D50
		private void SaveWindowPositionIfNeeded()
		{
			if (this.configManager != null && Time.time - this.lastWindowSaveTime > 2f)
			{
				this.lastWindowSaveTime = Time.time;
				float num = Mathf.Clamp(this.windowRect.x, 0f, (float)Screen.width - this.windowRect.width);
				float num2 = Mathf.Clamp(this.windowRect.y, 0f, (float)Screen.height - this.windowRect.height);
				this.configManager.SaveWindowPosition(num, num2);
			}
		}

		// Token: 0x060000C3 RID: 195 RVA: 0x00006BE0 File Offset: 0x00004DE0
		private void InitializeGUIStyles()
		{
			if (this.stylesInitialized)
			{
				return;
			}
			try
			{
				this.windowStyle = new GUIStyle(GUI.skin.window);
				this.windowStyle.fontSize = 14;
				this.windowStyle.fontStyle = 1;
				this.headerStyle = new GUIStyle(GUI.skin.label);
				this.headerStyle.fontSize = 16;
				this.headerStyle.fontStyle = 1;
				this.headerStyle.normal.textColor = Color.white;
				this.headerStyle.alignment = 4;
				this.nameStyle = new GUIStyle(GUI.skin.label);
				this.nameStyle.fontSize = this.settings.EnemyListDistanceFontSize;
				this.nameStyle.fontStyle = 1;
				this.nameStyle.alignment = 3;
				this.valueStyle = new GUIStyle(GUI.skin.label);
				this.valueStyle.fontSize = this.settings.EnemyListDistanceFontSize;
				this.valueStyle.fontStyle = 1;
				this.valueStyle.alignment = 5;
				this.distanceStyle = new GUIStyle(GUI.skin.label);
				this.distanceStyle.fontSize = this.settings.EnemyListDistanceFontSize;
				this.distanceStyle.fontStyle = 0;
				this.distanceStyle.alignment = 4;
				this.weaponStyle = new GUIStyle(GUI.skin.label);
				this.weaponStyle.fontSize = this.settings.EnemyListDistanceFontSize - 2;
				this.weaponStyle.fontStyle = 0;
				this.weaponStyle.alignment = 3;
				this.healthStyle = new GUIStyle(GUI.skin.label);
				this.healthStyle.fontSize = this.settings.EnemyListDistanceFontSize - 1;
				this.healthStyle.fontStyle = 0;
				this.healthStyle.alignment = 4;
				this.stylesInitialized = true;
			}
			catch (Exception ex)
			{
				this.logManager.WriteLog(string.Format("Error initializing GUI styles: {0}", ex), true);
			}
		}

		// Token: 0x060000C4 RID: 196 RVA: 0x00006E08 File Offset: 0x00005008
		private void DrawEnemyWindow(int windowID)
		{
			GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
			this.DrawHeader();
			this.DrawSeparator();
			this.DrawEnemyList();
			this.DrawStatistics();
			GUILayout.EndVertical();
			GUI.DragWindow();
		}

		// Token: 0x060000C5 RID: 197 RVA: 0x00006E38 File Offset: 0x00005038
		private void DrawHeader()
		{
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label("名字", this.headerStyle, new GUILayoutOption[] { GUILayout.Width(80f) });
			GUILayout.Label("价值", this.headerStyle, new GUILayoutOption[] { GUILayout.Width(100f) });
			GUILayout.Label("距离", this.headerStyle, new GUILayoutOption[] { GUILayout.Width(60f) });
			GUILayout.Label("武器", this.headerStyle, new GUILayoutOption[] { GUILayout.Width(120f) });
			GUILayout.Label("血量", this.headerStyle, new GUILayoutOption[] { GUILayout.Width(80f) });
			GUILayout.EndHorizontal();
		}

		// Token: 0x060000C6 RID: 198 RVA: 0x00006F03 File Offset: 0x00005103
		private void DrawSeparator()
		{
			GUILayout.Box("", new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(true),
				GUILayout.Height(1f)
			});
		}

		// Token: 0x060000C7 RID: 199 RVA: 0x00006F2C File Offset: 0x0000512C
		private void DrawEnemyList()
		{
			float num = this.windowRect.height - 120f;
			this.scrollPosition = GUILayout.BeginScrollView(this.scrollPosition, new GUILayoutOption[] { GUILayout.Height(num) });
			foreach (EnemyInfo enemyInfo in this.enemyInfoList)
			{
				this.DrawEnemyRow(enemyInfo);
			}
			GUILayout.EndScrollView();
		}

		// Token: 0x060000C8 RID: 200 RVA: 0x00006FB8 File Offset: 0x000051B8
		private void DrawEnemyRow(EnemyInfo enemyInfo)
		{
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			this.nameStyle.normal.textColor = enemyInfo.SpecialColor;
			GUILayout.Label(enemyInfo.Name, this.nameStyle, new GUILayoutOption[] { GUILayout.Width(80f) });
			Color color = ((enemyInfo.Value >= this.settings.HighValueThreshold) ? new Color(1f, 0.843f, 0f) : enemyInfo.SpecialColor);
			this.valueStyle.normal.textColor = color;
			GUILayout.Label(string.Format("¥{0:N0}", enemyInfo.Value), this.valueStyle, new GUILayoutOption[] { GUILayout.Width(100f) });
			this.distanceStyle.normal.textColor = UIColorHelper.GetDistanceColor(enemyInfo.Distance);
			GUILayout.Label(string.Format("{0:F1}m", enemyInfo.Distance), this.distanceStyle, new GUILayoutOption[] { GUILayout.Width(60f) });
			this.weaponStyle.normal.textColor = enemyInfo.SpecialColor;
			GUILayout.Label((enemyInfo.Weapon.Length > 15) ? (enemyInfo.Weapon.Substring(0, 15) + "...") : enemyInfo.Weapon, this.weaponStyle, new GUILayoutOption[] { GUILayout.Width(120f) });
			this.healthStyle.normal.textColor = UIColorHelper.GetHealthColor(enemyInfo.HealthPercent);
			GUILayout.Label(string.Format("{0:F0}/{1:F0}", enemyInfo.CurrentHealth, enemyInfo.MaxHealth), this.healthStyle, new GUILayoutOption[] { GUILayout.Width(80f) });
			GUILayout.EndHorizontal();
			if (enemyInfo.IsAimingAtPlayer)
			{
				this.DrawAimingWarning();
			}
		}

		// Token: 0x060000C9 RID: 201 RVA: 0x0000719C File Offset: 0x0000539C
		private void DrawAimingWarning()
		{
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Space(10f);
			GUILayout.Label("⚠ 正在瞄准你!", new GUIStyle(GUI.skin.label)
			{
				normal = 
				{
					textColor = Color.red
				},
				fontStyle = 1,
				fontSize = 12
			}, Array.Empty<GUILayoutOption>());
			GUILayout.EndHorizontal();
		}

		// Token: 0x060000CA RID: 202 RVA: 0x00007204 File Offset: 0x00005404
		private void DrawStatistics()
		{
			GUIStyle guistyle = new GUIStyle(GUI.skin.label);
			guistyle.fontSize = 12;
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label(string.Format("总敌人数: {0}", this.enemyInfoList.Count), guistyle, Array.Empty<GUILayoutOption>());
			GUILayout.FlexibleSpace();
			long num = this.enemyInfoList.Sum((EnemyInfo e) => e.Value);
			GUILayout.Label(string.Format("总价值: ¥{0:N0}", num), guistyle, Array.Empty<GUILayoutOption>());
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUIStyle guistyle2 = new GUIStyle(GUI.skin.label);
			guistyle2.fontSize = 11;
			guistyle2.normal.textColor = (this.settings.MagicBulletActive ? Color.green : Color.gray);
			string text = (this.settings.MagicBulletActive ? "启用" : "禁用");
			GUILayout.Label(string.Concat(new string[]
			{
				"魔法子弹: ",
				text,
				" (",
				this.settings.MagicBulletToggleKey,
				")"
			}), guistyle2, Array.Empty<GUILayoutOption>());
			GUILayout.FlexibleSpace();
			GUILayout.Label(string.Format("范围: {0:F0}m", this.settings.MagicBulletRange), guistyle2, Array.Empty<GUILayoutOption>());
			GUILayout.EndHorizontal();
		}

		// Token: 0x04000078 RID: 120
		private ESPSettings settings;

		// Token: 0x04000079 RID: 121
		private LogManager logManager;

		// Token: 0x0400007A RID: 122
		private ConfigManager configManager;

		// Token: 0x0400007B RID: 123
		private List<EnemyInfo> enemyInfoList = new List<EnemyInfo>();

		// Token: 0x0400007C RID: 124
		private bool showWindow = true;

		// Token: 0x0400007D RID: 125
		private Vector2 scrollPosition = Vector2.zero;

		// Token: 0x0400007E RID: 126
		private Rect windowRect;

		// Token: 0x0400007F RID: 127
		private GUIStyle headerStyle;

		// Token: 0x04000080 RID: 128
		private GUIStyle nameStyle;

		// Token: 0x04000081 RID: 129
		private GUIStyle valueStyle;

		// Token: 0x04000082 RID: 130
		private GUIStyle distanceStyle;

		// Token: 0x04000083 RID: 131
		private GUIStyle weaponStyle;

		// Token: 0x04000084 RID: 132
		private GUIStyle healthStyle;

		// Token: 0x04000085 RID: 133
		private GUIStyle windowStyle;

		// Token: 0x04000086 RID: 134
		private bool stylesInitialized;

		// Token: 0x04000087 RID: 135
		private float lastWindowSaveTime;

		// Token: 0x04000088 RID: 136
		private const float windowSaveInterval = 2f;
	}
}
