using System;
using Duckov.Economy;
using ItemStatsSystem;

// Token: 0x020001A6 RID: 422
[Serializable]
public struct DecomposeFormula
{
	// Token: 0x04000AD3 RID: 2771
	[ItemTypeID]
	public int item;

	// Token: 0x04000AD4 RID: 2772
	public bool valid;

	// Token: 0x04000AD5 RID: 2773
	public Cost result;
}
