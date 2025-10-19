using System;
using ItemStatsSystem;

// Token: 0x020000E8 RID: 232
public static class InventoryExtensions
{
	// Token: 0x060007B9 RID: 1977 RVA: 0x000228F0 File Offset: 0x00020AF0
	private static void Sort(this Inventory inventory, Comparison<Item> comparison)
	{
		inventory.Content.Sort(comparison);
	}
}
