using System;
using Duckov.Utilities;
using ItemStatsSystem;

namespace Duckov.Crops
{
	// Token: 0x020002E5 RID: 741
	[Serializable]
	public struct SeedInfo
	{
		// Token: 0x060017C8 RID: 6088 RVA: 0x00057106 File Offset: 0x00055306
		public string GetRandomCropID()
		{
			return this.cropIDs.GetRandom(0f);
		}

		// Token: 0x04001157 RID: 4439
		[ItemTypeID]
		public int itemTypeID;

		// Token: 0x04001158 RID: 4440
		public RandomContainer<string> cropIDs;
	}
}
