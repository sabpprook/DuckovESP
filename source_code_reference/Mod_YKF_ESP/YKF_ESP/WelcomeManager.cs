using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Duckov.UI.Animations;
using UnityEngine;

namespace YKF_ESP
{
	// Token: 0x02000018 RID: 24
	[NullableContext(1)]
	[Nullable(0)]
	public class WelcomeManager
	{
		// Token: 0x060000CB RID: 203 RVA: 0x00007377 File Offset: 0x00005577
		public WelcomeManager(LogManager logManager, ESPSettings settings, AudioController audioController, SceneLoader sceneLoader)
		{
			this.logManager = logManager;
			this.settings = settings;
			this.audioController = audioController;
			this.sceneLoader = sceneLoader;
		}

		// Token: 0x060000CC RID: 204 RVA: 0x0000739C File Offset: 0x0000559C
		public void OnFinishedLoadingScene(SceneLoadingContext context)
		{
			try
			{
				this.currentSceneName = context.sceneName;
				LogManager logManager = this.logManager;
				if (logManager != null)
				{
					logManager.WriteLog("WelcomeManager: Finished loading scene: " + context.sceneName, false);
				}
				if (context.sceneName == "Base")
				{
					this.hasPlayedBGM = false;
					this.hasShownWelcome = false;
					this.isWaitingForClick = true;
					this.hasClickedToContinue = false;
					LogManager logManager2 = this.logManager;
					if (logManager2 != null)
					{
						logManager2.WriteLog("Base scene loaded, waiting for click to continue...", false);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager logManager3 = this.logManager;
				if (logManager3 != null)
				{
					logManager3.WriteLog(string.Format("Error in WelcomeManager scene handler: {0}", ex), true);
				}
			}
		}

		// Token: 0x060000CD RID: 205 RVA: 0x00007450 File Offset: 0x00005650
		private void ShowWelcomeMessage()
		{
			if (this.hasShownWelcome)
			{
				return;
			}
			try
			{
				CharacterMainControl main = CharacterMainControl.Main;
				if (main != null)
				{
					main.PopText("欢迎使用YKF-ESP，祝你游戏愉快!", 5f);
					this.hasShownWelcome = true;
					LogManager logManager = this.logManager;
					if (logManager != null)
					{
						logManager.WriteLog("Welcome message displayed to player", false);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager logManager2 = this.logManager;
				if (logManager2 != null)
				{
					logManager2.WriteLog(string.Format("Failed to show welcome message: {0}", ex), true);
				}
			}
		}

		// Token: 0x060000CE RID: 206 RVA: 0x000074D8 File Offset: 0x000056D8
		public void PlayRandomBGM()
		{
			if (!this.settings.PlayBGM)
			{
				LogManager logManager = this.logManager;
				if (logManager == null)
				{
					return;
				}
				logManager.WriteLog("BGM playback is disabled in configuration.", false);
				return;
			}
			else
			{
				if (this.hasPlayedBGM)
				{
					return;
				}
				try
				{
					string[] array = new string[] { "dddd", "manbo" };
					string text = array[Random.Range(0, array.Length)];
					this.audioController.PlayCustomAudio(text, 0.8f, "bus:/Master/SFX");
					this.hasPlayedBGM = true;
					LogManager logManager2 = this.logManager;
					if (logManager2 != null)
					{
						logManager2.WriteLog("Playing random BGM after click to continue: " + text, false);
					}
				}
				catch (Exception ex)
				{
					LogManager logManager3 = this.logManager;
					if (logManager3 != null)
					{
						logManager3.WriteLog(string.Format("Failed to play BGM: {0}", ex), true);
					}
				}
				return;
			}
		}

		// Token: 0x060000CF RID: 207 RVA: 0x000075A4 File Offset: 0x000057A4
		public void CheckClickToContinue()
		{
			if (this.sceneLoader == null)
			{
				return;
			}
			try
			{
				FieldInfo field = typeof(SceneLoader).GetField("pointerClickEventRecevier", BindingFlags.Instance | BindingFlags.NonPublic);
				FieldInfo field2 = typeof(SceneLoader).GetField("content", BindingFlags.Instance | BindingFlags.NonPublic);
				FieldInfo field3 = typeof(SceneLoader).GetField("clickIndicator", BindingFlags.Instance | BindingFlags.NonPublic);
				if (field != null && field2 != null && field3 != null)
				{
					field.GetValue(this.sceneLoader);
					FadeGroup fadeGroup = field2.GetValue(this.sceneLoader) as FadeGroup;
					FadeGroup fadeGroup2 = field3.GetValue(this.sceneLoader) as FadeGroup;
					if (fadeGroup != null && fadeGroup2 != null && !fadeGroup.gameObject.activeInHierarchy && !fadeGroup2.gameObject.activeInHierarchy && !this.hasClickedToContinue)
					{
						this.hasClickedToContinue = true;
						this.clickTime = Time.unscaledTime;
						this.isWaitingForClick = false;
						LogManager logManager = this.logManager;
						if (logManager != null)
						{
							logManager.WriteLog("Click to continue completed, starting 3 second delay for BGM and welcome message...", false);
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager logManager2 = this.logManager;
				if (logManager2 != null)
				{
					logManager2.WriteLog(string.Format("Error checking click to continue: {0}", ex), true);
				}
				if (!this.hasClickedToContinue)
				{
					this.hasClickedToContinue = true;
					this.clickTime = Time.unscaledTime;
					this.isWaitingForClick = false;
					LogManager logManager3 = this.logManager;
					if (logManager3 != null)
					{
						logManager3.WriteLog("Fallback: Starting 3 second delay for BGM and welcome message...", false);
					}
				}
			}
		}

		// Token: 0x060000D0 RID: 208 RVA: 0x0000773C File Offset: 0x0000593C
		public void Update()
		{
			if (this.isWaitingForClick && this.currentSceneName == "Base")
			{
				this.CheckClickToContinue();
			}
			if (this.hasClickedToContinue && !this.hasPlayedBGM && !this.hasShownWelcome && Time.unscaledTime - this.clickTime >= 3f)
			{
				this.ShowWelcomeMessage();
				if (this.settings.PlayBGM)
				{
					this.PlayRandomBGM();
				}
			}
		}

		// Token: 0x04000089 RID: 137
		private LogManager logManager;

		// Token: 0x0400008A RID: 138
		private ESPSettings settings;

		// Token: 0x0400008B RID: 139
		private AudioController audioController;

		// Token: 0x0400008C RID: 140
		private SceneLoader sceneLoader;

		// Token: 0x0400008D RID: 141
		private string currentSceneName;

		// Token: 0x0400008E RID: 142
		private bool hasPlayedBGM;

		// Token: 0x0400008F RID: 143
		private bool hasShownWelcome;

		// Token: 0x04000090 RID: 144
		private bool isWaitingForClick;

		// Token: 0x04000091 RID: 145
		private bool hasClickedToContinue;

		// Token: 0x04000092 RID: 146
		private float clickTime;
	}
}
