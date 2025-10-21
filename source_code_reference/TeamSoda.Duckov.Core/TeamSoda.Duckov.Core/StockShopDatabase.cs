using System;
using System.Collections.Generic;
using Duckov.Utilities;
using ItemStatsSystem;
using UnityEngine;

// Token: 0x02000151 RID: 337
[CreateAssetMenu(menuName = "Duckov/Stock Shop Database")]
public class StockShopDatabase : ScriptableObject
{
	// Token: 0x1700020F RID: 527
	// (get) Token: 0x06000A63 RID: 2659 RVA: 0x0002DA6C File Offset: 0x0002BC6C
	public static StockShopDatabase Instance
	{
		get
		{
			return GameplayDataSettings.StockshopDatabase;
		}
	}

	// Token: 0x06000A64 RID: 2660 RVA: 0x0002DA74 File Offset: 0x0002BC74
	public StockShopDatabase.MerchantProfile GetMerchantProfile(string merchantID)
	{
		return this.merchantProfiles.Find((StockShopDatabase.MerchantProfile e) => e.merchantID == merchantID);
	}

	// Token: 0x04000918 RID: 2328
	public List<StockShopDatabase.MerchantProfile> merchantProfiles;

	// Token: 0x020004A5 RID: 1189
	[Serializable]
	public class MerchantProfile
	{
		// Token: 0x04001C27 RID: 7207
		public string merchantID;

		// Token: 0x04001C28 RID: 7208
		public List<StockShopDatabase.ItemEntry> entries = new List<StockShopDatabase.ItemEntry>();
	}

	// Token: 0x020004A6 RID: 1190
	[Serializable]
	public class ItemEntry
	{
		// Token: 0x04001C29 RID: 7209
		[ItemTypeID]
		public int typeID;

		// Token: 0x04001C2A RID: 7210
		public int maxStock;

		// Token: 0x04001C2B RID: 7211
		public bool forceUnlock;

		// Token: 0x04001C2C RID: 7212
		public float priceFactor;

		// Token: 0x04001C2D RID: 7213
		public float possibility;

		// Token: 0x04001C2E RID: 7214
		public bool lockInDemo;
	}
}
