using System;
using UnityEngine;

namespace Duckov.MiniGames.GoldMiner
{
	// Token: 0x02000294 RID: 660
	public class AddEagleEyePotionOnRetrieve : MiniGameBehaviour
	{
		// Token: 0x060015A0 RID: 5536 RVA: 0x00050170 File Offset: 0x0004E370
		private void Awake()
		{
			if (this.master == null)
			{
				this.master = base.GetComponent<GoldMinerEntity>();
			}
			GoldMinerEntity goldMinerEntity = this.master;
			goldMinerEntity.OnResolved = (Action<GoldMinerEntity, GoldMiner>)Delegate.Combine(goldMinerEntity.OnResolved, new Action<GoldMinerEntity, GoldMiner>(this.OnResolved));
		}

		// Token: 0x060015A1 RID: 5537 RVA: 0x000501BE File Offset: 0x0004E3BE
		private void OnResolved(GoldMinerEntity entity, GoldMiner game)
		{
			game.run.eagleEyePotion += this.amount;
		}

		// Token: 0x04001001 RID: 4097
		[SerializeField]
		private GoldMinerEntity master;

		// Token: 0x04001002 RID: 4098
		[SerializeField]
		private int amount = 1;
	}
}
