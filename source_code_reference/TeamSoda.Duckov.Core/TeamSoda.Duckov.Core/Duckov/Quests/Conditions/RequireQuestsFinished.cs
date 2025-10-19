using System;
using UnityEngine;

namespace Duckov.Quests.Conditions
{
	// Token: 0x02000364 RID: 868
	public class RequireQuestsFinished : Condition
	{
		// Token: 0x170005DC RID: 1500
		// (get) Token: 0x06001E2D RID: 7725 RVA: 0x0006A3E9 File Offset: 0x000685E9
		public int[] RequiredQuestIDs
		{
			get
			{
				return this.requiredQuestIDs;
			}
		}

		// Token: 0x06001E2E RID: 7726 RVA: 0x0006A3F1 File Offset: 0x000685F1
		public override bool Evaluate()
		{
			return QuestManager.AreQuestFinished(this.requiredQuestIDs);
		}

		// Token: 0x04001499 RID: 5273
		[SerializeField]
		private int[] requiredQuestIDs;
	}
}
