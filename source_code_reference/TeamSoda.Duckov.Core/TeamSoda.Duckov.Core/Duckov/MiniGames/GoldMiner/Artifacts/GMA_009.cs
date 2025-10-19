using System;

namespace Duckov.MiniGames.GoldMiner.Artifacts
{
	// Token: 0x020002B2 RID: 690
	public class GMA_009 : GoldMinerArtifactBehaviour
	{
		// Token: 0x06001646 RID: 5702 RVA: 0x000521D4 File Offset: 0x000503D4
		protected override void OnAttached(GoldMinerArtifact artifact)
		{
			if (base.GoldMiner == null)
			{
				return;
			}
			GoldMiner goldMiner = base.GoldMiner;
			goldMiner.onResolveEntity = (Action<GoldMiner, GoldMinerEntity>)Delegate.Combine(goldMiner.onResolveEntity, new Action<GoldMiner, GoldMinerEntity>(this.OnResolveEntity));
		}

		// Token: 0x06001647 RID: 5703 RVA: 0x0005220C File Offset: 0x0005040C
		protected override void OnDetached(GoldMinerArtifact artifact)
		{
			if (base.GoldMiner == null)
			{
				return;
			}
			GoldMiner goldMiner = base.GoldMiner;
			goldMiner.onResolveEntity = (Action<GoldMiner, GoldMinerEntity>)Delegate.Remove(goldMiner.onResolveEntity, new Action<GoldMiner, GoldMinerEntity>(this.OnResolveEntity));
		}

		// Token: 0x06001648 RID: 5704 RVA: 0x00052244 File Offset: 0x00050444
		private void OnResolveEntity(GoldMiner miner, GoldMinerEntity entity)
		{
			if (entity == null)
			{
				return;
			}
			if (base.Run.IsRock(entity))
			{
				this.effectActive = true;
			}
			if (this.effectActive && base.Run.IsGold(entity))
			{
				this.effectActive = false;
				entity.Value *= 2;
			}
		}

		// Token: 0x0400107A RID: 4218
		private bool effectActive;
	}
}
