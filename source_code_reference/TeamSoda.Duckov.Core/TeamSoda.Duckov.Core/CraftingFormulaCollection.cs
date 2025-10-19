using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Duckov.Utilities;
using UnityEngine;

// Token: 0x020001A3 RID: 419
[CreateAssetMenu]
public class CraftingFormulaCollection : ScriptableObject
{
	// Token: 0x1700023C RID: 572
	// (get) Token: 0x06000C63 RID: 3171 RVA: 0x000344EB File Offset: 0x000326EB
	public static CraftingFormulaCollection Instance
	{
		get
		{
			return GameplayDataSettings.CraftingFormulas;
		}
	}

	// Token: 0x1700023D RID: 573
	// (get) Token: 0x06000C64 RID: 3172 RVA: 0x000344F2 File Offset: 0x000326F2
	public ReadOnlyCollection<CraftingFormula> Entries
	{
		get
		{
			if (this._entries_ReadOnly == null)
			{
				this._entries_ReadOnly = new ReadOnlyCollection<CraftingFormula>(this.list);
			}
			return this._entries_ReadOnly;
		}
	}

	// Token: 0x06000C65 RID: 3173 RVA: 0x00034514 File Offset: 0x00032714
	public static bool TryGetFormula(string id, out CraftingFormula formula)
	{
		if (!(CraftingFormulaCollection.Instance == null))
		{
			CraftingFormula craftingFormula = CraftingFormulaCollection.Instance.list.FirstOrDefault((CraftingFormula e) => e.id == id);
			if (!string.IsNullOrEmpty(craftingFormula.id))
			{
				formula = craftingFormula;
				return true;
			}
		}
		formula = default(CraftingFormula);
		return false;
	}

	// Token: 0x04000ACA RID: 2762
	[SerializeField]
	private List<CraftingFormula> list;

	// Token: 0x04000ACB RID: 2763
	private ReadOnlyCollection<CraftingFormula> _entries_ReadOnly;
}
