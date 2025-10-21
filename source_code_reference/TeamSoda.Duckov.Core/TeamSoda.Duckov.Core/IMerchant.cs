using System;
using ItemStatsSystem;

// Token: 0x020001FD RID: 509
public interface IMerchant
{
	// Token: 0x06000EEC RID: 3820
	int ConvertPrice(Item item, bool selling = false);
}
