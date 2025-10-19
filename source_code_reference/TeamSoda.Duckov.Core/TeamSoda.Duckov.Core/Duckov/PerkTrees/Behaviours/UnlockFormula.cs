using System;
using System.Collections.Generic;

namespace Duckov.PerkTrees.Behaviours
{
	// Token: 0x02000258 RID: 600
	public class UnlockFormula : PerkBehaviour
	{
		// Token: 0x17000361 RID: 865
		// (get) Token: 0x060012A7 RID: 4775 RVA: 0x0004641F File Offset: 0x0004461F
		private IEnumerable<string> FormulasToUnlock
		{
			get
			{
				if (!CraftingFormulaCollection.Instance)
				{
					yield break;
				}
				string matchKey = base.Master.Master.ID + "/" + base.Master.name;
				foreach (CraftingFormula craftingFormula in CraftingFormulaCollection.Instance.Entries)
				{
					if (craftingFormula.requirePerk == matchKey)
					{
						yield return craftingFormula.id;
					}
				}
				IEnumerator<CraftingFormula> enumerator = null;
				yield break;
				yield break;
			}
		}

		// Token: 0x060012A8 RID: 4776 RVA: 0x00046430 File Offset: 0x00044630
		protected override void OnUnlocked()
		{
			foreach (string text in this.FormulasToUnlock)
			{
				CraftingManager.UnlockFormula(text);
			}
		}
	}
}
