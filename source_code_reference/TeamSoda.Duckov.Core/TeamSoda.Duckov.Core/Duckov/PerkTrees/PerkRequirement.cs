using System;
using Duckov.Economy;
using ItemStatsSystem;

namespace Duckov.PerkTrees
{
	// Token: 0x02000248 RID: 584
	[Serializable]
	public class PerkRequirement
	{
		// Token: 0x1700033D RID: 829
		// (get) Token: 0x0600123C RID: 4668 RVA: 0x00045574 File Offset: 0x00043774
		public TimeSpan RequireTime
		{
			get
			{
				return TimeSpan.FromTicks(this.requireTime);
			}
		}

		// Token: 0x0600123D RID: 4669 RVA: 0x00045581 File Offset: 0x00043781
		internal bool AreSatisfied()
		{
			return this.level <= EXPManager.Level && this.cost.Enough;
		}

		// Token: 0x04000DFC RID: 3580
		public int level;

		// Token: 0x04000DFD RID: 3581
		public Cost cost;

		// Token: 0x04000DFE RID: 3582
		[TimeSpan]
		public long requireTime;

		// Token: 0x0200052F RID: 1327
		[Serializable]
		public class RequireItemEntry
		{
			// Token: 0x04001E65 RID: 7781
			[ItemTypeID]
			public int id = 1;

			// Token: 0x04001E66 RID: 7782
			public int amount = 1;
		}
	}
}
