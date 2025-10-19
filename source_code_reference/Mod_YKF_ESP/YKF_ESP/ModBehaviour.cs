using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Duckov.Modding;
using Duckov.UI;
using UnityEngine;

namespace YKF_ESP
{
	// Token: 0x02000010 RID: 16
	[NullableContext(1)]
	[Nullable(0)]
	public class ModBehaviour : ModBehaviour
	{
		// Token: 0x06000072 RID: 114 RVA: 0x00004150 File Offset: 0x00002350
		private void Awake()
		{
			try
			{
				string directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
				this.configManager = new ConfigManager(directoryName);
				this.settings = this.configManager.LoadSettings();
				this.logManager = new LogManager(directoryName, this.settings.LogToFile);
				this.logManager.WriteLog("YKF_ESP Mod Awake!", false);
				this.ValidateToggleKey();
				this.cacheManager = new CacheManager();
				this.enemyDetector = new EnemyDetector(this.settings, this.logManager);
				this.espRenderer = new ESPRenderer(this.settings, this.logManager);
				this.uiManager = new UIManager(this.settings, this.logManager, this.configManager);
				base.StartCoroutine(this.cacheManager.UpdateCaches());
				this.isInitialized = true;
				this.logManager.WriteLog("Mod initialized successfully.", false);
			}
			catch (Exception ex)
			{
				if (this.logManager != null)
				{
					this.logManager.WriteLog(string.Format("Initialization failed: {0}", ex), true);
				}
			}
		}

		// Token: 0x06000073 RID: 115 RVA: 0x0000426C File Offset: 0x0000246C
		private void Update()
		{
			if (!this.isInitialized || this.hasError)
			{
				return;
			}
			try
			{
				if (this.toggleKeyValid && Input.GetKeyDown(this.toggleKey))
				{
					this.uiManager.ToggleWindow();
				}
				if (this.configManager.ShouldReloadConfig())
				{
					this.settings = this.configManager.LoadSettings();
					this.ValidateToggleKey();
					this.UpdateComponentSettings();
				}
				if (this.mainCamera == null)
				{
					this.mainCamera = Camera.main;
					if (this.mainCamera == null)
					{
						return;
					}
				}
				if (LevelManager.LevelInited && !(CharacterMainControl.Main == null))
				{
					if (this.player == null)
					{
						this.player = CharacterMainControl.Main;
					}
					if (!this.cacheManager.InventoryDisplayCache.Any((InventoryDisplay display) => display != null && display.gameObject.activeInHierarchy && display.enabled))
					{
						this.enemyDetector.Update(this.player, this.cacheManager.AllCharactersCache);
						this.uiManager.UpdateEnemyData(this.enemyDetector.GetEnemyInfoList());
					}
				}
			}
			catch (Exception ex)
			{
				this.hasError = true;
				this.logManager.WriteLog(string.Format("Critical error in Update: {0}", ex), true);
			}
		}

		// Token: 0x06000074 RID: 116 RVA: 0x000043D8 File Offset: 0x000025D8
		private void OnGUI()
		{
			if (!this.isInitialized || this.hasError || this.player == null || this.mainCamera == null)
			{
				return;
			}
			this.uiManager.OnGUI();
		}

		// Token: 0x06000075 RID: 117 RVA: 0x00004414 File Offset: 0x00002614
		private void OnRenderObject()
		{
			if (!this.isInitialized || this.hasError || this.player == null || this.mainCamera == null)
			{
				return;
			}
			this.espRenderer.DrawLines(this.enemyDetector.GetEnemyInfoList(), this.player, this.mainCamera);
		}

		// Token: 0x06000076 RID: 118 RVA: 0x00004470 File Offset: 0x00002670
		private void OnDestroy()
		{
			LogManager logManager = this.logManager;
			if (logManager != null)
			{
				logManager.WriteLog("Mod destroyed.", false);
			}
			LogManager logManager2 = this.logManager;
			if (logManager2 != null)
			{
				logManager2.Dispose();
			}
			ESPRenderer esprenderer = this.espRenderer;
			if (esprenderer != null)
			{
				esprenderer.Dispose();
			}
			base.StopAllCoroutines();
		}

		// Token: 0x06000077 RID: 119 RVA: 0x000044BC File Offset: 0x000026BC
		private void ValidateToggleKey()
		{
			KeyCode keyCode;
			if (KeyCodeHelper.TryParseKeyCode(this.settings.ToggleKey, out keyCode))
			{
				this.toggleKey = keyCode;
				this.toggleKeyValid = true;
				LogManager logManager = this.logManager;
				if (logManager == null)
				{
					return;
				}
				logManager.WriteLog(string.Format("Toggle key set to: {0} ({1})", this.settings.ToggleKey, keyCode), false);
				return;
			}
			else
			{
				this.toggleKey = KeyCode.F1;
				this.toggleKeyValid = false;
				LogManager logManager2 = this.logManager;
				if (logManager2 == null)
				{
					return;
				}
				logManager2.WriteLog("Invalid toggle key '" + this.settings.ToggleKey + "', using default F1", true);
				return;
			}
		}

		// Token: 0x06000078 RID: 120 RVA: 0x00004558 File Offset: 0x00002758
		private void UpdateComponentSettings()
		{
			EnemyDetector enemyDetector = this.enemyDetector;
			if (enemyDetector != null)
			{
				enemyDetector.UpdateSettings(this.settings);
			}
			ESPRenderer esprenderer = this.espRenderer;
			if (esprenderer != null)
			{
				esprenderer.UpdateSettings(this.settings);
			}
			UIManager uimanager = this.uiManager;
			if (uimanager != null)
			{
				uimanager.UpdateSettings(this.settings);
			}
			LogManager logManager = this.logManager;
			if (logManager != null)
			{
				logManager.UpdateSettings(this.settings.LogToFile);
			}
			LogManager logManager2 = this.logManager;
			if (logManager2 == null)
			{
				return;
			}
			logManager2.WriteLog("Settings updated for all components.", false);
		}

		// Token: 0x0400003C RID: 60
		private ConfigManager configManager;

		// Token: 0x0400003D RID: 61
		private ESPSettings settings;

		// Token: 0x0400003E RID: 62
		private EnemyDetector enemyDetector;

		// Token: 0x0400003F RID: 63
		private ESPRenderer espRenderer;

		// Token: 0x04000040 RID: 64
		private UIManager uiManager;

		// Token: 0x04000041 RID: 65
		private LogManager logManager;

		// Token: 0x04000042 RID: 66
		private CacheManager cacheManager;

		// Token: 0x04000043 RID: 67
		private CharacterMainControl player;

		// Token: 0x04000044 RID: 68
		private bool isInitialized;

		// Token: 0x04000045 RID: 69
		private bool hasError;

		// Token: 0x04000046 RID: 70
		private Camera mainCamera;

		// Token: 0x04000047 RID: 71
		private KeyCode toggleKey = KeyCode.F1;

		// Token: 0x04000048 RID: 72
		private bool toggleKeyValid = true;
	}
}
