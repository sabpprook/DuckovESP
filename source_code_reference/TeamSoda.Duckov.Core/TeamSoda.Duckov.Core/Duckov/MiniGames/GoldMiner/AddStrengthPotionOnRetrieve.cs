using System;
using UnityEngine;

namespace Duckov.MiniGames.GoldMiner
{
	// Token: 0x02000295 RID: 661
	public class AddStrengthPotionOnRetrieve : MiniGameBehaviour
	{
		// Token: 0x060015A3 RID: 5539 RVA: 0x000501E8 File Offset: 0x0004E3E8
		private void Awake()
		{
			if (this.master == null)
			{
				this.master = base.GetComponent<GoldMinerEntity>();
			}
			GoldMinerEntity goldMinerEntity = this.master;
			goldMinerEntity.OnResolved = (Action<GoldMinerEntity, GoldMiner>)Delegate.Combine(goldMinerEntity.OnResolved, new Action<GoldMinerEntity, GoldMiner>(this.OnResolved));
		}

		// Token: 0x060015A4 RID: 5540 RVA: 0x00050236 File Offset: 0x0004E436
		private void OnResolved(GoldMinerEntity entity, GoldMiner game)
		{
			game.run.strengthPotion += this.amount;
		}

		// Token: 0x04001003 RID: 4099
		[SerializeField]
		private GoldMinerEntity master;

		// Token: 0x04001004 RID: 4100
		[SerializeField]
		private int amount = 1;
	}
}
