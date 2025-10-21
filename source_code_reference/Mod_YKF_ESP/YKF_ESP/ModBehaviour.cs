using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Duckov.Modding;
using Duckov.UI;
using UnityEngine;

namespace YKF_ESP
{
	// Token: 0x02000015 RID: 21
	[NullableContext(1)]
	[Nullable(0)]
	public class ModBehaviour : ModBehaviour
	{
		// Token: 0x060000B1 RID: 177 RVA: 0x00005F2C File Offset: 0x0000412C
		private void Awake()
		{
			string environmentVariable = Environment.GetEnvironmentVariable("YKF_ESP_AUTHOR", EnvironmentVariableTarget.User);
			if (!string.IsNullOrEmpty(environmentVariable) && environmentVariable.ToLower() == "true")
			{
				Debug.Log("[YKF_ESP] Author mode detected, mod disabled");
				Object.Destroy(this);
				return;
			}
			try
			{
				string directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
				this.configManager = new ConfigManager(directoryName);
				this.settings = this.configManager.LoadSettings(null);
				this.logManager = new LogManager(directoryName, this.settings.LogToFile);
				this.logManager.WriteLog("YKF_ESP Mod Awake! Version: " + this.settings.Version, false);
				SceneLoader instance = SceneLoader.Instance;
				this.ValidateToggleKey();
				this.magicBulletManager = new MagicBulletManager(this.settings, this.logManager);
				this.magicBulletRenderer = new MagicBulletRenderer(this.settings);
				this.ValidateMagicBulletKey();
				this.cacheManager = new CacheManager();
				this.enemyDetector = new EnemyDetector(this.settings, this.logManager);
				this.espRenderer = new ESPRenderer(this.settings, this.logManager);
				this.uiManager = new UIManager(this.settings, this.logManager, this.configManager);
				this.audioController = new AudioController(this.logManager);
				this.welcomeManager = new WelcomeManager(this.logManager, this.settings, this.audioController, instance);
				SceneLoader.onStartedLoadingScene += this.OnStartedLoadingScene;
				SceneLoader.onFinishedLoadingScene += this.welcomeManager.OnFinishedLoadingScene;
				if (this.magicBulletManager == null || this.cacheManager == null || this.enemyDetector == null || this.espRenderer == null || this.uiManager == null || this.audioController == null || this.welcomeManager == null)
				{
					throw new Exception("Critical component initialization failed");
				}
				base.StartCoroutine(this.cacheManager.UpdateCaches());
				this.isInitialized = true;
				this.logManager.WriteLog("Mod initialized successfully.", false);
			}
			catch (Exception ex)
			{
				this.hasError = true;
				try
				{
					LogManager logManager = this.logManager;
					if (logManager != null)
					{
						logManager.WriteLog(string.Format("Initialization failed: {0}", ex), true);
					}
				}
				catch
				{
					Debug.LogError(string.Format("[YKF_ESP] Initialization failed: {0}", ex));
				}
			}
		}

		// Token: 0x060000B2 RID: 178 RVA: 0x0000619C File Offset: 0x0000439C
		private void OnStartedLoadingScene(SceneLoadingContext context)
		{
			try
			{
				LogManager logManager = this.logManager;
				if (logManager != null)
				{
					logManager.WriteLog("Loading scene: " + context.sceneName, false);
				}
			}
			catch (Exception ex)
			{
				LogManager logManager2 = this.logManager;
				if (logManager2 != null)
				{
					logManager2.WriteLog(string.Format("Error in scene loading handler: {0}", ex), true);
				}
			}
		}

		// Token: 0x060000B3 RID: 179 RVA: 0x00006200 File Offset: 0x00004400
		private void Update()
		{
			if (!this.isInitialized || this.hasError)
			{
				return;
			}
			try
			{
				this.welcomeManager.Update();
				if (this.toggleKeyValid && Input.GetKeyDown(this.toggleKey))
				{
					UIManager uimanager = this.uiManager;
					if (uimanager != null)
					{
						uimanager.ToggleWindow();
					}
				}
				if (this.magicBulletKeyValid && Input.GetKeyDown(this.magicBulletToggleKey))
				{
					MagicBulletManager magicBulletManager = this.magicBulletManager;
					if (magicBulletManager != null)
					{
						magicBulletManager.ToggleMagicBullet(this.player);
					}
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
					bool flag = true;
					CacheManager cacheManager = this.cacheManager;
					if (((cacheManager != null) ? cacheManager.InventoryDisplayCache : null) != null)
					{
						flag = !this.cacheManager.InventoryDisplayCache.Any((InventoryDisplay display) => display != null && display.gameObject.activeInHierarchy && display.enabled);
					}
					if (flag)
					{
						EnemyDetector enemyDetector = this.enemyDetector;
						if (enemyDetector != null)
						{
							CharacterMainControl characterMainControl = this.player;
							CacheManager cacheManager2 = this.cacheManager;
							enemyDetector.Update(characterMainControl, (cacheManager2 != null) ? cacheManager2.AllCharactersCache : null);
						}
						EnemyDetector enemyDetector2 = this.enemyDetector;
						List<EnemyInfo> list = ((enemyDetector2 != null) ? enemyDetector2.GetEnemyInfoList() : null);
						if (list != null)
						{
							UIManager uimanager2 = this.uiManager;
							if (uimanager2 != null)
							{
								uimanager2.UpdateEnemyData(list);
							}
							if (this.magicBulletManager != null && this.player != null)
							{
								this.magicBulletManager.Update(this.player, list);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				this.hasError = true;
				try
				{
					LogManager logManager = this.logManager;
					if (logManager != null)
					{
						logManager.WriteLog(string.Format("Critical error in Update: {0}", ex), true);
					}
				}
				catch
				{
					Debug.LogError(string.Format("[YKF_ESP] Update error: {0}", ex));
				}
			}
		}

		// Token: 0x060000B4 RID: 180 RVA: 0x00006414 File Offset: 0x00004614
		private void OnGUI()
		{
			if (!this.isInitialized || this.hasError || this.player == null || this.mainCamera == null)
			{
				return;
			}
			UIManager uimanager = this.uiManager;
			if (uimanager == null)
			{
				return;
			}
			uimanager.OnGUI();
		}

		// Token: 0x060000B5 RID: 181 RVA: 0x00006454 File Offset: 0x00004654
		private void OnRenderObject()
		{
			if (!this.isInitialized || this.hasError || this.player == null || this.mainCamera == null)
			{
				return;
			}
			EnemyDetector enemyDetector = this.enemyDetector;
			List<EnemyInfo> list = ((enemyDetector != null) ? enemyDetector.GetEnemyInfoList() : null);
			if (list != null && this.espRenderer != null)
			{
				this.espRenderer.DrawLines(list, this.player, this.mainCamera);
			}
		}

		// Token: 0x060000B6 RID: 182 RVA: 0x000064C4 File Offset: 0x000046C4
		private void OnDestroy()
		{
			try
			{
				SceneLoader.onStartedLoadingScene -= this.OnStartedLoadingScene;
				SceneLoader.onFinishedLoadingScene -= this.welcomeManager.OnFinishedLoadingScene;
				EnemyDetector enemyDetector = this.enemyDetector;
				if (enemyDetector != null)
				{
					enemyDetector.Dispose();
				}
				AudioController audioController = this.audioController;
				if (audioController != null)
				{
					audioController.Dispose();
				}
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
				MagicBulletRenderer magicBulletRenderer = this.magicBulletRenderer;
				if (magicBulletRenderer != null)
				{
					magicBulletRenderer.Dispose();
				}
				base.StopAllCoroutines();
			}
			catch (Exception ex)
			{
				Debug.LogError(string.Format("[YKF_ESP] OnDestroy error: {0}", ex));
			}
		}

		// Token: 0x060000B7 RID: 183 RVA: 0x00006590 File Offset: 0x00004790
		private void ValidateMagicBulletKey()
		{
			try
			{
				KeyCode keyCode;
				if (this.settings != null && KeyCodeHelper.TryParseKeyCode(this.settings.MagicBulletToggleKey, out keyCode))
				{
					this.magicBulletToggleKey = keyCode;
					this.magicBulletKeyValid = true;
					LogManager logManager = this.logManager;
					if (logManager != null)
					{
						logManager.WriteLog(string.Format("Magic Bullet toggle key set to: {0} ({1})", this.settings.MagicBulletToggleKey, keyCode), false);
					}
				}
				else
				{
					this.magicBulletToggleKey = KeyCode.F2;
					this.magicBulletKeyValid = false;
					LogManager logManager2 = this.logManager;
					if (logManager2 != null)
					{
						string text = "Invalid magic bullet key '";
						ESPSettings espsettings = this.settings;
						logManager2.WriteLog(text + ((espsettings != null) ? espsettings.MagicBulletToggleKey : null) + "', using default F2", true);
					}
				}
			}
			catch (Exception ex)
			{
				this.magicBulletToggleKey = KeyCode.F2;
				this.magicBulletKeyValid = false;
				LogManager logManager3 = this.logManager;
				if (logManager3 != null)
				{
					logManager3.WriteLog(string.Format("Error validating magic bullet key: {0}", ex), true);
				}
			}
		}

		// Token: 0x060000B8 RID: 184 RVA: 0x00006680 File Offset: 0x00004880
		private void ValidateToggleKey()
		{
			try
			{
				KeyCode keyCode;
				if (this.settings != null && KeyCodeHelper.TryParseKeyCode(this.settings.ToggleKey, out keyCode))
				{
					this.toggleKey = keyCode;
					this.toggleKeyValid = true;
					LogManager logManager = this.logManager;
					if (logManager != null)
					{
						logManager.WriteLog(string.Format("Toggle key set to: {0} ({1})", this.settings.ToggleKey, keyCode), false);
					}
				}
				else
				{
					this.toggleKey = KeyCode.F1;
					this.toggleKeyValid = false;
					LogManager logManager2 = this.logManager;
					if (logManager2 != null)
					{
						string text = "Invalid toggle key '";
						ESPSettings espsettings = this.settings;
						logManager2.WriteLog(text + ((espsettings != null) ? espsettings.ToggleKey : null) + "', using default F1", true);
					}
				}
			}
			catch (Exception ex)
			{
				this.toggleKey = KeyCode.F1;
				this.toggleKeyValid = false;
				LogManager logManager3 = this.logManager;
				if (logManager3 != null)
				{
					logManager3.WriteLog(string.Format("Error validating toggle key: {0}", ex), true);
				}
			}
		}

		// Token: 0x060000B9 RID: 185 RVA: 0x00006770 File Offset: 0x00004970
		private void UpdateComponentSettings()
		{
			try
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
				MagicBulletManager magicBulletManager = this.magicBulletManager;
				if (magicBulletManager != null)
				{
					magicBulletManager.UpdateSettings(this.settings);
				}
				MagicBulletRenderer magicBulletRenderer = this.magicBulletRenderer;
				if (magicBulletRenderer != null)
				{
					magicBulletRenderer.UpdateSettings(this.settings);
				}
				LogManager logManager = this.logManager;
				if (logManager != null)
				{
					ESPSettings espsettings = this.settings;
					logManager.UpdateSettings(espsettings != null && espsettings.LogToFile);
				}
				LogManager logManager2 = this.logManager;
				if (logManager2 != null)
				{
					logManager2.WriteLog("Settings updated for all components.", false);
				}
			}
			catch (Exception ex)
			{
				LogManager logManager3 = this.logManager;
				if (logManager3 != null)
				{
					logManager3.WriteLog(string.Format("Error updating component settings: {0}", ex), true);
				}
			}
		}

		// Token: 0x04000065 RID: 101
		private ConfigManager configManager;

		// Token: 0x04000066 RID: 102
		private ESPSettings settings;

		// Token: 0x04000067 RID: 103
		private EnemyDetector enemyDetector;

		// Token: 0x04000068 RID: 104
		private ESPRenderer espRenderer;

		// Token: 0x04000069 RID: 105
		private UIManager uiManager;

		// Token: 0x0400006A RID: 106
		private LogManager logManager;

		// Token: 0x0400006B RID: 107
		private CacheManager cacheManager;

		// Token: 0x0400006C RID: 108
		private CharacterMainControl player;

		// Token: 0x0400006D RID: 109
		private bool isInitialized;

		// Token: 0x0400006E RID: 110
		private bool hasError;

		// Token: 0x0400006F RID: 111
		private Camera mainCamera;

		// Token: 0x04000070 RID: 112
		private KeyCode toggleKey = KeyCode.F1;

		// Token: 0x04000071 RID: 113
		private bool toggleKeyValid = true;

		// Token: 0x04000072 RID: 114
		private MagicBulletManager magicBulletManager;

		// Token: 0x04000073 RID: 115
		private MagicBulletRenderer magicBulletRenderer;

		// Token: 0x04000074 RID: 116
		private KeyCode magicBulletToggleKey = KeyCode.F2;

		// Token: 0x04000075 RID: 117
		private bool magicBulletKeyValid = true;

		// Token: 0x04000076 RID: 118
		private AudioController audioController;

		// Token: 0x04000077 RID: 119
		private WelcomeManager welcomeManager;
	}
}
