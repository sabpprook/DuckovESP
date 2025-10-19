using System;
using Duckov.BlackMarkets;
using UnityEngine;

namespace Duckov.PerkTrees.Behaviours
{
	// Token: 0x02000255 RID: 597
	public class ChangeBlackMarketRefreshTimeFactor : PerkBehaviour
	{
		// Token: 0x0600129C RID: 4764 RVA: 0x000460D5 File Offset: 0x000442D5
		protected override void OnAwake()
		{
			base.OnAwake();
			BlackMarket.onRequestRefreshTime += this.HandleEvent;
		}

		// Token: 0x0600129D RID: 4765 RVA: 0x000460EE File Offset: 0x000442EE
		protected override void OnOnDestroy()
		{
			base.OnOnDestroy();
			BlackMarket.onRequestRefreshTime -= this.HandleEvent;
		}

		// Token: 0x0600129E RID: 4766 RVA: 0x00046107 File Offset: 0x00044307
		private void HandleEvent(BlackMarket.OnRequestRefreshTimeFactorEventContext context)
		{
			if (base.Master == null)
			{
				return;
			}
			if (!base.Master.Unlocked)
			{
				return;
			}
			context.Add(this.amount);
		}

		// Token: 0x04000E15 RID: 3605
		[SerializeField]
		private float amount = -0.1f;
	}
}
