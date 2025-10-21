using System;
using UnityEngine;

namespace Duckov.MiniGames.GoldMiner.Artifacts
{
	// Token: 0x020002AC RID: 684
	public class GMA_003 : GoldMinerArtifactBehaviour
	{
		// Token: 0x0600162D RID: 5677 RVA: 0x00051CF2 File Offset: 0x0004FEF2
		protected override void OnAttached(GoldMinerArtifact artifact)
		{
			if (base.GoldMiner == null)
			{
				return;
			}
			GoldMiner goldMiner = base.GoldMiner;
			goldMiner.onResolveEntity = (Action<GoldMiner, GoldMinerEntity>)Delegate.Combine(goldMiner.onResolveEntity, new Action<GoldMiner, GoldMinerEntity>(this.OnResolveEntity));
		}

		// Token: 0x0600162E RID: 5678 RVA: 0x00051D2A File Offset: 0x0004FF2A
		protected override void OnDetached(GoldMinerArtifact artifact)
		{
			if (base.GoldMiner == null)
			{
				return;
			}
			GoldMiner goldMiner = base.GoldMiner;
			goldMiner.onResolveEntity = (Action<GoldMiner, GoldMinerEntity>)Delegate.Remove(goldMiner.onResolveEntity, new Action<GoldMiner, GoldMinerEntity>(this.OnResolveEntity));
		}

		// Token: 0x0600162F RID: 5679 RVA: 0x00051D64 File Offset: 0x0004FF64
		private void OnResolveEntity(GoldMiner miner, GoldMinerEntity entity)
		{
			if (entity == null)
			{
				return;
			}
			if (base.Run.IsRock(entity))
			{
				Debug.Log("Enity is Rock ", entity);
				this.streak++;
			}
			else
			{
				this.streak = 0;
			}
			if (this.streak > 1)
			{
				base.Run.levelScoreFactor += 0.1f;
			}
		}

		// Token: 0x04001077 RID: 4215
		private int streak;
	}
}
