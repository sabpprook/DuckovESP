using System;
using Duckov.Achievements;
using Duckov.Rules.UI;
using Saves;
using UnityEngine;

// Token: 0x0200009F RID: 159
public class EndingControl : MonoBehaviour
{
	// Token: 0x0600054D RID: 1357 RVA: 0x00017BC8 File Offset: 0x00015DC8
	public void SetEndingIndex()
	{
		Ending.endingIndex = this.endingIndex;
		AchievementManager instance = AchievementManager.Instance;
		bool flag = SavesSystem.Load<bool>(this.MissleLuncherClosedKey);
		DifficultySelection.UnlockRage();
		if (instance)
		{
			if (this.endingIndex == 0)
			{
				if (!flag)
				{
					instance.Unlock("Ending_0");
					return;
				}
				instance.Unlock("Ending_3");
				return;
			}
			else
			{
				if (!flag)
				{
					instance.Unlock("Ending_1");
					return;
				}
				instance.Unlock("Ending_2");
			}
		}
	}

	// Token: 0x040004C3 RID: 1219
	public int endingIndex;

	// Token: 0x040004C4 RID: 1220
	public string MissleLuncherClosedKey = "MissleLuncherClosed";
}
