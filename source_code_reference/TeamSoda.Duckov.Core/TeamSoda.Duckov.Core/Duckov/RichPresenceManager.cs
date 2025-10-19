using System;
using Duckov.Scenes;
using UnityEngine;

namespace Duckov
{
	// Token: 0x02000232 RID: 562
	public class RichPresenceManager : MonoBehaviour
	{
		// Token: 0x1700030D RID: 781
		// (get) Token: 0x06001180 RID: 4480 RVA: 0x00043AE5 File Offset: 0x00041CE5
		public bool isPlaying
		{
			get
			{
				return !this.isMainMenu;
			}
		}

		// Token: 0x06001181 RID: 4481 RVA: 0x00043AF0 File Offset: 0x00041CF0
		private void InvokeChangeEvent()
		{
			Action<RichPresenceManager> onInstanceChanged = RichPresenceManager.OnInstanceChanged;
			if (onInstanceChanged == null)
			{
				return;
			}
			onInstanceChanged(this);
		}

		// Token: 0x06001182 RID: 4482 RVA: 0x00043B04 File Offset: 0x00041D04
		private void Awake()
		{
			MainMenu.OnMainMenuAwake = (Action)Delegate.Combine(MainMenu.OnMainMenuAwake, new Action(this.OnMainMenuAwake));
			MainMenu.OnMainMenuDestroy = (Action)Delegate.Combine(MainMenu.OnMainMenuDestroy, new Action(this.OnMainMenuDestroy));
			MultiSceneCore.OnInstanceAwake += this.OnMultiSceneCoreInstanceAwake;
			MultiSceneCore.OnInstanceDestroy += this.OnMultiSceneCoreInstanceDestroy;
		}

		// Token: 0x06001183 RID: 4483 RVA: 0x00043B74 File Offset: 0x00041D74
		private void OnDestroy()
		{
			MainMenu.OnMainMenuAwake = (Action)Delegate.Remove(MainMenu.OnMainMenuAwake, new Action(this.OnMainMenuAwake));
			MainMenu.OnMainMenuDestroy = (Action)Delegate.Remove(MainMenu.OnMainMenuDestroy, new Action(this.OnMainMenuDestroy));
			MultiSceneCore.OnInstanceAwake -= this.OnMultiSceneCoreInstanceAwake;
			MultiSceneCore.OnInstanceDestroy -= this.OnMultiSceneCoreInstanceDestroy;
		}

		// Token: 0x06001184 RID: 4484 RVA: 0x00043BE3 File Offset: 0x00041DE3
		private void OnMainMenuAwake()
		{
			this.isMainMenu = true;
			this.InvokeChangeEvent();
		}

		// Token: 0x06001185 RID: 4485 RVA: 0x00043BF2 File Offset: 0x00041DF2
		private void OnMainMenuDestroy()
		{
			this.isMainMenu = false;
			this.InvokeChangeEvent();
		}

		// Token: 0x06001186 RID: 4486 RVA: 0x00043C01 File Offset: 0x00041E01
		private void OnMultiSceneCoreInstanceAwake(MultiSceneCore core)
		{
			this.levelDisplayNameRaw = core.DisplaynameRaw;
			this.isInLevel = true;
			this.InvokeChangeEvent();
		}

		// Token: 0x06001187 RID: 4487 RVA: 0x00043C1C File Offset: 0x00041E1C
		private void OnMultiSceneCoreInstanceDestroy(MultiSceneCore core)
		{
			this.isInLevel = false;
			this.InvokeChangeEvent();
		}

		// Token: 0x06001188 RID: 4488 RVA: 0x00043C2B File Offset: 0x00041E2B
		internal string GetSteamDisplay()
		{
			if (Application.isEditor)
			{
				return "#Status_UnityEditor";
			}
			if (!this.isMainMenu)
			{
				return "#Status_Playing";
			}
			return "#Status_MainMenu";
		}

		// Token: 0x04000D89 RID: 3465
		public bool isMainMenu = true;

		// Token: 0x04000D8A RID: 3466
		public bool isInLevel;

		// Token: 0x04000D8B RID: 3467
		public string levelDisplayNameRaw;

		// Token: 0x04000D8C RID: 3468
		public static Action<RichPresenceManager> OnInstanceChanged;
	}
}
