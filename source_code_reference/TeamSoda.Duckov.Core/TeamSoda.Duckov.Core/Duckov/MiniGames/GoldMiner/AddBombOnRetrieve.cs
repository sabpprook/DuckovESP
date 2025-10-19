using System;
using UnityEngine;

namespace Duckov.MiniGames.GoldMiner
{
	// Token: 0x02000293 RID: 659
	public class AddBombOnRetrieve : MiniGameBehaviour
	{
		// Token: 0x0600159D RID: 5533 RVA: 0x000500F8 File Offset: 0x0004E2F8
		private void Awake()
		{
			if (this.master == null)
			{
				this.master = base.GetComponent<GoldMinerEntity>();
			}
			GoldMinerEntity goldMinerEntity = this.master;
			goldMinerEntity.OnResolved = (Action<GoldMinerEntity, GoldMiner>)Delegate.Combine(goldMinerEntity.OnResolved, new Action<GoldMinerEntity, GoldMiner>(this.OnResolved));
		}

		// Token: 0x0600159E RID: 5534 RVA: 0x00050146 File Offset: 0x0004E346
		private void OnResolved(GoldMinerEntity entity, GoldMiner game)
		{
			game.run.bomb += this.amount;
		}

		// Token: 0x04000FFF RID: 4095
		[SerializeField]
		private GoldMinerEntity master;

		// Token: 0x04001000 RID: 4096
		[SerializeField]
		private int amount = 1;
	}
}
