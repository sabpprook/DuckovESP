using System;
using UnityEngine;

namespace Duckov.Quests.Conditions
{
	// Token: 0x02000363 RID: 867
	public class RequireQuestsActive : Condition
	{
		// Token: 0x170005DB RID: 1499
		// (get) Token: 0x06001E2A RID: 7722 RVA: 0x0006A3CC File Offset: 0x000685CC
		public int[] RequiredQuestIDs
		{
			get
			{
				return this.requiredQuestIDs;
			}
		}

		// Token: 0x06001E2B RID: 7723 RVA: 0x0006A3D4 File Offset: 0x000685D4
		public override bool Evaluate()
		{
			return QuestManager.AreQuestsActive(this.requiredQuestIDs);
		}

		// Token: 0x04001498 RID: 5272
		[SerializeField]
		private int[] requiredQuestIDs;
	}
}
