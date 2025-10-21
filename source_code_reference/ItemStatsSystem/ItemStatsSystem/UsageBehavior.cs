using System;
using UnityEngine;

namespace ItemStatsSystem
{
	// Token: 0x02000025 RID: 37
	public abstract class UsageBehavior : MonoBehaviour
	{
		// Token: 0x1700008A RID: 138
		// (get) Token: 0x060001F7 RID: 503 RVA: 0x00007DD4 File Offset: 0x00005FD4
		public virtual UsageBehavior.DisplaySettingsData DisplaySettings
		{
			get
			{
				return default(UsageBehavior.DisplaySettingsData);
			}
		}

		// Token: 0x060001F8 RID: 504
		public abstract bool CanBeUsed(Item item, object user);

		// Token: 0x060001F9 RID: 505
		protected abstract void OnUse(Item item, object user);

		// Token: 0x060001FA RID: 506 RVA: 0x00007DEA File Offset: 0x00005FEA
		public void Use(Item item, object user)
		{
			this.OnUse(item, user);
		}

		// Token: 0x0200004A RID: 74
		public struct DisplaySettingsData
		{
			// Token: 0x170000A2 RID: 162
			// (get) Token: 0x06000289 RID: 649 RVA: 0x0000966B File Offset: 0x0000786B
			public string Description
			{
				get
				{
					return this.description;
				}
			}

			// Token: 0x04000124 RID: 292
			public bool display;

			// Token: 0x04000125 RID: 293
			public string description;
		}
	}
}
