using System;
using UnityEngine;

namespace Duckov.MiniGames.GoldMiner.Artifacts
{
	// Token: 0x020002B8 RID: 696
	public class GMA_015 : GoldMinerArtifactBehaviour
	{
		// Token: 0x06001659 RID: 5721 RVA: 0x00052450 File Offset: 0x00050650
		protected override void OnAttached(GoldMinerArtifact artifact)
		{
			if (base.GoldMiner == null)
			{
				return;
			}
			GoldMiner goldMiner = base.GoldMiner;
			goldMiner.onResolveEntity = (Action<GoldMiner, GoldMinerEntity>)Delegate.Combine(goldMiner.onResolveEntity, new Action<GoldMiner, GoldMinerEntity>(this.OnResolveEntity));
		}

		// Token: 0x0600165A RID: 5722 RVA: 0x00052488 File Offset: 0x00050688
		protected override void OnDetached(GoldMinerArtifact artifact)
		{
			if (base.GoldMiner == null)
			{
				return;
			}
			GoldMiner goldMiner = base.GoldMiner;
			goldMiner.onResolveEntity = (Action<GoldMiner, GoldMinerEntity>)Delegate.Remove(goldMiner.onResolveEntity, new Action<GoldMiner, GoldMinerEntity>(this.OnResolveEntity));
		}

		// Token: 0x0600165B RID: 5723 RVA: 0x000524C0 File Offset: 0x000506C0
		private void OnResolveEntity(GoldMiner miner, GoldMinerEntity entity)
		{
			if (base.Run == null)
			{
				return;
			}
			if (!base.Run.IsPig(entity))
			{
				return;
			}
			entity.Value += this.amount;
		}

		// Token: 0x0400107B RID: 4219
		[SerializeField]
		private int amount = 20;
	}
}
