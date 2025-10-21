using System;
using ItemStatsSystem;
using UnityEngine;

namespace Fishing
{
	// Token: 0x02000212 RID: 530
	[Serializable]
	internal struct FishingPoolEntry
	{
		// Token: 0x170002D9 RID: 729
		// (get) Token: 0x06000FC7 RID: 4039 RVA: 0x0003DF01 File Offset: 0x0003C101
		public int ID
		{
			get
			{
				return this.id;
			}
		}

		// Token: 0x170002DA RID: 730
		// (get) Token: 0x06000FC8 RID: 4040 RVA: 0x0003DF09 File Offset: 0x0003C109
		public float Weight
		{
			get
			{
				return this.weight;
			}
		}

		// Token: 0x04000CB1 RID: 3249
		[SerializeField]
		[ItemTypeID]
		private int id;

		// Token: 0x04000CB2 RID: 3250
		[SerializeField]
		private float weight;
	}
}
