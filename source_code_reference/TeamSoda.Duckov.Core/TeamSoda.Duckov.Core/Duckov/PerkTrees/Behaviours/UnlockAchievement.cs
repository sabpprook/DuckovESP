using System;
using Duckov.Achievements;
using UnityEngine;

namespace Duckov.PerkTrees.Behaviours
{
	// Token: 0x02000257 RID: 599
	public class UnlockAchievement : PerkBehaviour
	{
		// Token: 0x060012A5 RID: 4773 RVA: 0x000463F2 File Offset: 0x000445F2
		protected override void OnUnlocked()
		{
			if (AchievementManager.Instance == null)
			{
				return;
			}
			AchievementManager.Instance.Unlock(this.achievementKey.Trim());
		}

		// Token: 0x04000E19 RID: 3609
		[SerializeField]
		private string achievementKey;
	}
}
