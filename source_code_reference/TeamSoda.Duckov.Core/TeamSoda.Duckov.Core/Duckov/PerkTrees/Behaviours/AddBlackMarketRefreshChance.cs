using System;
using Duckov.BlackMarkets;
using UnityEngine;

namespace Duckov.PerkTrees.Behaviours
{
	// Token: 0x02000254 RID: 596
	public class AddBlackMarketRefreshChance : PerkBehaviour
	{
		// Token: 0x06001297 RID: 4759 RVA: 0x00046062 File Offset: 0x00044262
		protected override void OnAwake()
		{
			base.OnAwake();
			BlackMarket.onRequestMaxRefreshChance += this.HandleEvent;
		}

		// Token: 0x06001298 RID: 4760 RVA: 0x0004607B File Offset: 0x0004427B
		protected override void OnOnDestroy()
		{
			base.OnOnDestroy();
			BlackMarket.onRequestMaxRefreshChance -= this.HandleEvent;
		}

		// Token: 0x06001299 RID: 4761 RVA: 0x00046094 File Offset: 0x00044294
		private void HandleEvent(BlackMarket.OnRequestMaxRefreshChanceEventContext context)
		{
			if (base.Master == null)
			{
				return;
			}
			if (!base.Master.Unlocked)
			{
				return;
			}
			context.Add(this.addAmount);
		}

		// Token: 0x0600129A RID: 4762 RVA: 0x000460BF File Offset: 0x000442BF
		protected override void OnUnlocked()
		{
			BlackMarket.NotifyMaxRefreshChanceChanged();
		}

		// Token: 0x04000E14 RID: 3604
		[SerializeField]
		private int addAmount = 1;
	}
}
