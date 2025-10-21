using System;
using Saves;

namespace Duckov.Quests.Conditions
{
	// Token: 0x02000361 RID: 865
	public class RequireHasFished : Condition
	{
		// Token: 0x06001E23 RID: 7715 RVA: 0x0006A2E6 File Offset: 0x000684E6
		public override bool Evaluate()
		{
			return RequireHasFished.GetHasFished();
		}

		// Token: 0x06001E24 RID: 7716 RVA: 0x0006A2ED File Offset: 0x000684ED
		public static void SetHasFished()
		{
			SavesSystem.Save<bool>("HasFished", true);
		}

		// Token: 0x06001E25 RID: 7717 RVA: 0x0006A2FA File Offset: 0x000684FA
		public static bool GetHasFished()
		{
			return SavesSystem.Load<bool>("HasFished");
		}
	}
}
