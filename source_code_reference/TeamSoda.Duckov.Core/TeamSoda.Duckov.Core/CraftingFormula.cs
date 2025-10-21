using System;
using Duckov.Economy;
using ItemStatsSystem;
using UnityEngine;

// Token: 0x020001A2 RID: 418
[Serializable]
public struct CraftingFormula
{
	// Token: 0x1700023B RID: 571
	// (get) Token: 0x06000C62 RID: 3170 RVA: 0x000344DB File Offset: 0x000326DB
	public bool IDValid
	{
		get
		{
			return !string.IsNullOrEmpty(this.id);
		}
	}

	// Token: 0x04000AC2 RID: 2754
	public string id;

	// Token: 0x04000AC3 RID: 2755
	public CraftingFormula.ItemEntry result;

	// Token: 0x04000AC4 RID: 2756
	public string[] tags;

	// Token: 0x04000AC5 RID: 2757
	[SerializeField]
	public Cost cost;

	// Token: 0x04000AC6 RID: 2758
	public bool unlockByDefault;

	// Token: 0x04000AC7 RID: 2759
	public bool lockInDemo;

	// Token: 0x04000AC8 RID: 2760
	public string requirePerk;

	// Token: 0x04000AC9 RID: 2761
	public bool hideInIndex;

	// Token: 0x020004BC RID: 1212
	[Serializable]
	public struct ItemEntry
	{
		// Token: 0x04001C88 RID: 7304
		[ItemTypeID]
		public int id;

		// Token: 0x04001C89 RID: 7305
		public int amount;
	}
}
