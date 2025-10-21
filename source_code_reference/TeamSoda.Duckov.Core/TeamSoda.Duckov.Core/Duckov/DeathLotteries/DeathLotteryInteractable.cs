using System;
using UnityEngine;

namespace Duckov.DeathLotteries
{
	// Token: 0x02000300 RID: 768
	public class DeathLotteryInteractable : InteractableBase
	{
		// Token: 0x060018EA RID: 6378 RVA: 0x0005A9C7 File Offset: 0x00058BC7
		protected override bool IsInteractable()
		{
			return !(this.deathLottery == null) && this.deathLottery.CurrentStatus.valid && !this.deathLottery.Loading;
		}

		// Token: 0x060018EB RID: 6379 RVA: 0x0005A9FD File Offset: 0x00058BFD
		protected override void OnInteractFinished()
		{
			this.deathLottery.RequestUI();
		}

		// Token: 0x0400121B RID: 4635
		[SerializeField]
		private DeathLottery deathLottery;
	}
}
