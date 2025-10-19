using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace YKF_ESP
{
	// Token: 0x02000012 RID: 18
	[NullableContext(1)]
	[Nullable(0)]
	public class UIManager
	{
		// Token: 0x0600007C RID: 124 RVA: 0x00004668 File Offset: 0x00002868
		public UIManager(ESPSettings settings, LogManager logManager, ConfigManager configManager = null)
		{
			this.settings = settings;
			this.logManager = logManager;
			this.configManager = configManager;
			this.windowRect = new Rect(settings.WindowPosX, settings.WindowPosY, settings.WindowWidth * 1.1f, settings.WindowHeight * 1.1f);
		}

		// Token: 0x0600007D RID: 125 RVA: 0x000046DC File Offset: 0x000028DC
		public void UpdateSettings(ESPSettings newSettings)
		{
			newSettings.WindowPosX = this.windowRect.x;
			newSettings.WindowPosY = this.windowRect.y;
			this.settings = newSettings;
			this.stylesInitialized = false;
			this.windowRect.width = this.settings.WindowWidth * 1.1f;
			this.windowRect.height = this.settings.WindowHeight * 1.1f;
		}

		// Token: 0x0600007E RID: 126 RVA: 0x00004751 File Offset: 0x00002951
		public void ToggleWindow()
		{
			this.showWindow = !this.showWindow;
		}

		// Token: 0x0600007F RID: 127 RVA: 0x00004762 File Offset: 0x00002962
		public void UpdateEnemyData(List<EnemyInfo> newEnemyInfoList)
		{
			this.enemyInfoList = newEnemyInfoList;
		}

		// Token: 0x06000080 RID: 128 RVA: 0x0000476C File Offset: 0x0000296C
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
				GUI.Label(new Rect(10f, (float)(Screen.height - 30), 300f, 20f), "按" + this.settings.ToggleKey + "键切换ESP窗口", GUI.skin.label);
			}
		}

		// Token: 0x06000081 RID: 129 RVA: 0x000048A0 File Offset: 0x00002AA0
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

		// Token: 0x06000082 RID: 130 RVA: 0x00004930 File Offset: 0x00002B30
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

		// Token: 0x06000083 RID: 131 RVA: 0x00004B58 File Offset: 0x00002D58
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

		// Token: 0x06000084 RID: 132 RVA: 0x00004B88 File Offset: 0x00002D88
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

		// Token: 0x06000085 RID: 133 RVA: 0x00004C53 File Offset: 0x00002E53
		private void DrawSeparator()
		{
			GUILayout.Box("", new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(true),
				GUILayout.Height(1f)
			});
		}

		// Token: 0x06000086 RID: 134 RVA: 0x00004C7C File Offset: 0x00002E7C
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

		// Token: 0x06000087 RID: 135 RVA: 0x00004D08 File Offset: 0x00002F08
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

		// Token: 0x06000088 RID: 136 RVA: 0x00004EEC File Offset: 0x000030EC
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

		// Token: 0x06000089 RID: 137 RVA: 0x00004F54 File Offset: 0x00003154
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
		}

		// Token: 0x04000049 RID: 73
		private ESPSettings settings;

		// Token: 0x0400004A RID: 74
		private LogManager logManager;

		// Token: 0x0400004B RID: 75
		private ConfigManager configManager;

		// Token: 0x0400004C RID: 76
		private List<EnemyInfo> enemyInfoList = new List<EnemyInfo>();

		// Token: 0x0400004D RID: 77
		private bool showWindow = true;

		// Token: 0x0400004E RID: 78
		private Vector2 scrollPosition = Vector2.zero;

		// Token: 0x0400004F RID: 79
		private Rect windowRect;

		// Token: 0x04000050 RID: 80
		private GUIStyle headerStyle;

		// Token: 0x04000051 RID: 81
		private GUIStyle nameStyle;

		// Token: 0x04000052 RID: 82
		private GUIStyle valueStyle;

		// Token: 0x04000053 RID: 83
		private GUIStyle distanceStyle;

		// Token: 0x04000054 RID: 84
		private GUIStyle weaponStyle;

		// Token: 0x04000055 RID: 85
		private GUIStyle healthStyle;

		// Token: 0x04000056 RID: 86
		private GUIStyle windowStyle;

		// Token: 0x04000057 RID: 87
		private bool stylesInitialized;

		// Token: 0x04000058 RID: 88
		private float lastWindowSaveTime;

		// Token: 0x04000059 RID: 89
		private const float windowSaveInterval = 2f;
	}
}
