using System;
using ItemStatsSystem;
using UnityEngine;

namespace Duckov.Crops
{
	// Token: 0x020002E6 RID: 742
	[Serializable]
	public struct CropInfo
	{
		// Token: 0x1700044A RID: 1098
		// (get) Token: 0x060017C9 RID: 6089 RVA: 0x00057118 File Offset: 0x00055318
		public string DisplayName
		{
			get
			{
				if (this._normalMetaData == null)
				{
					this._normalMetaData = new ItemMetaData?(ItemAssetsCollection.GetMetaData(this.resultNormal));
				}
				return this._normalMetaData.Value.DisplayName;
			}
		}

		// Token: 0x1700044B RID: 1099
		// (get) Token: 0x060017CA RID: 6090 RVA: 0x0005715B File Offset: 0x0005535B
		public TimeSpan GrowTime
		{
			get
			{
				return TimeSpan.FromTicks(this.totalGrowTicks);
			}
		}

		// Token: 0x060017CB RID: 6091 RVA: 0x00057168 File Offset: 0x00055368
		public int GetProduct(ProductRanking ranking)
		{
			int num = 0;
			switch (ranking)
			{
			case ProductRanking.Poor:
				num = this.resultPoor;
				break;
			case ProductRanking.Normal:
				num = this.resultNormal;
				break;
			case ProductRanking.Good:
				num = this.resultGood;
				break;
			}
			if (num == 0)
			{
				if (this.resultNormal != 0)
				{
					return this.resultNormal;
				}
				if (this.resultPoor != 0)
				{
					return this.resultPoor;
				}
			}
			return num;
		}

		// Token: 0x04001159 RID: 4441
		public string id;

		// Token: 0x0400115A RID: 4442
		public GameObject displayPrefab;

		// Token: 0x0400115B RID: 4443
		[ItemTypeID]
		public int resultPoor;

		// Token: 0x0400115C RID: 4444
		[ItemTypeID]
		public int resultNormal;

		// Token: 0x0400115D RID: 4445
		[ItemTypeID]
		public int resultGood;

		// Token: 0x0400115E RID: 4446
		private ItemMetaData? _normalMetaData;

		// Token: 0x0400115F RID: 4447
		public int resultAmount;

		// Token: 0x04001160 RID: 4448
		[TimeSpan]
		public long totalGrowTicks;
	}
}
