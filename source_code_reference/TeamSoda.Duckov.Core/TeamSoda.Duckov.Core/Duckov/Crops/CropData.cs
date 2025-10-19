using System;
using UnityEngine;

namespace Duckov.Crops
{
	// Token: 0x020002E8 RID: 744
	[Serializable]
	public struct CropData
	{
		// Token: 0x1700044C RID: 1100
		// (get) Token: 0x060017CC RID: 6092 RVA: 0x000571C6 File Offset: 0x000553C6
		public ProductRanking Ranking
		{
			get
			{
				if (this.score < 33)
				{
					return ProductRanking.Poor;
				}
				if (this.score < 66)
				{
					return ProductRanking.Normal;
				}
				return ProductRanking.Good;
			}
		}

		// Token: 0x1700044D RID: 1101
		// (get) Token: 0x060017CD RID: 6093 RVA: 0x000571E1 File Offset: 0x000553E1
		public TimeSpan GrowTime
		{
			get
			{
				return TimeSpan.FromTicks(this.growTicks);
			}
		}

		// Token: 0x1700044E RID: 1102
		// (get) Token: 0x060017CE RID: 6094 RVA: 0x000571EE File Offset: 0x000553EE
		// (set) Token: 0x060017CF RID: 6095 RVA: 0x000571FB File Offset: 0x000553FB
		public DateTime LastUpdateDateTime
		{
			get
			{
				return DateTime.FromBinary(this.lastUpdateDateTimeRaw);
			}
			set
			{
				this.lastUpdateDateTimeRaw = value.ToBinary();
			}
		}

		// Token: 0x04001165 RID: 4453
		public string gardenID;

		// Token: 0x04001166 RID: 4454
		public Vector2Int coord;

		// Token: 0x04001167 RID: 4455
		public string cropID;

		// Token: 0x04001168 RID: 4456
		public int score;

		// Token: 0x04001169 RID: 4457
		public bool watered;

		// Token: 0x0400116A RID: 4458
		[TimeSpan]
		public long growTicks;

		// Token: 0x0400116B RID: 4459
		[DateTime]
		public long lastUpdateDateTimeRaw;
	}
}
