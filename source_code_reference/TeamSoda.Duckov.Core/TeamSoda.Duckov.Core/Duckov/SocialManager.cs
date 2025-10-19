using System;
using Duckov.Achievements;
using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace Duckov
{
	// Token: 0x02000239 RID: 569
	public class SocialManager : MonoBehaviour
	{
		// Token: 0x0600119E RID: 4510 RVA: 0x00043DFE File Offset: 0x00041FFE
		private void Awake()
		{
			AchievementManager.OnAchievementUnlocked += this.UnlockAchievement;
		}

		// Token: 0x0600119F RID: 4511 RVA: 0x00043E11 File Offset: 0x00042011
		private void Start()
		{
			Social.localUser.Authenticate(new Action<bool>(this.ProcessAuthentication));
		}

		// Token: 0x060011A0 RID: 4512 RVA: 0x00043E29 File Offset: 0x00042029
		private void ProcessAuthentication(bool success)
		{
			if (success)
			{
				this.initialized = true;
				Social.LoadAchievements(new Action<IAchievement[]>(this.ProcessLoadedAchievements));
			}
		}

		// Token: 0x060011A1 RID: 4513 RVA: 0x00043E46 File Offset: 0x00042046
		private void ProcessLoadedAchievements(IAchievement[] loadedAchievements)
		{
			this._achievement_cache = loadedAchievements;
		}

		// Token: 0x060011A2 RID: 4514 RVA: 0x00043E4F File Offset: 0x0004204F
		private void UnlockAchievement(string id)
		{
			if (this.initialized)
			{
				return;
			}
			Social.ReportProgress(id, 100.0, new Action<bool>(this.OnReportProgressResult));
		}

		// Token: 0x060011A3 RID: 4515 RVA: 0x00043E75 File Offset: 0x00042075
		private void OnReportProgressResult(bool success)
		{
			Social.LoadAchievements(new Action<IAchievement[]>(this.ProcessLoadedAchievements));
		}

		// Token: 0x04000DA3 RID: 3491
		private bool initialized;

		// Token: 0x04000DA4 RID: 3492
		private IAchievement[] _achievement_cache;
	}
}
