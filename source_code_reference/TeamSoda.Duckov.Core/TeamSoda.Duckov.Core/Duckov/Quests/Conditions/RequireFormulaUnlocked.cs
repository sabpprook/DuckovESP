using System;
using ItemStatsSystem;
using UnityEngine;

namespace Duckov.Quests.Conditions
{
	// Token: 0x0200035F RID: 863
	public class RequireFormulaUnlocked : Condition
	{
		// Token: 0x06001E1F RID: 7711 RVA: 0x0006A28D File Offset: 0x0006848D
		public override bool Evaluate()
		{
			return CraftingManager.IsFormulaUnlocked(this.formulaID);
		}

		// Token: 0x04001491 RID: 5265
		[ItemTypeID]
		[SerializeField]
		private int itemID;

		// Token: 0x04001492 RID: 5266
		[SerializeField]
		private string formulaID;

		// Token: 0x04001493 RID: 5267
		public Item setFromItem;
	}
}
