using System;
using Duckov.PerkTrees;
using UnityEngine;

namespace Duckov.Quests.Conditions
{
	// Token: 0x02000362 RID: 866
	public class RequirePerkUnlocked : Condition
	{
		// Token: 0x06001E27 RID: 7719 RVA: 0x0006A30E File Offset: 0x0006850E
		public override bool Evaluate()
		{
			return this.GetUnlocked();
		}

		// Token: 0x06001E28 RID: 7720 RVA: 0x0006A318 File Offset: 0x00068518
		private bool GetUnlocked()
		{
			if (this.perk)
			{
				return this.perk.Unlocked;
			}
			PerkTree perkTree = PerkTreeManager.GetPerkTree(this.perkTreeID);
			if (perkTree)
			{
				foreach (Perk perk in perkTree.perks)
				{
					if (perk.gameObject.name == this.perkObjectName)
					{
						this.perk = perk;
						return this.perk.Unlocked;
					}
				}
				return false;
			}
			return false;
		}

		// Token: 0x04001495 RID: 5269
		[SerializeField]
		private string perkTreeID;

		// Token: 0x04001496 RID: 5270
		[SerializeField]
		private string perkObjectName;

		// Token: 0x04001497 RID: 5271
		private Perk perk;
	}
}
