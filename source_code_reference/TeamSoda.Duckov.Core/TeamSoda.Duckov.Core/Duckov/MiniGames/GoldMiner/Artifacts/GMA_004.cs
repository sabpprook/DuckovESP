using System;

namespace Duckov.MiniGames.GoldMiner.Artifacts
{
	// Token: 0x020002AD RID: 685
	public class GMA_004 : GoldMinerArtifactBehaviour
	{
		// Token: 0x06001631 RID: 5681 RVA: 0x00051DD3 File Offset: 0x0004FFD3
		protected override void OnAttached(GoldMinerArtifact artifact)
		{
			if (base.GoldMiner == null)
			{
				return;
			}
			GoldMiner goldMiner = base.GoldMiner;
			goldMiner.onResolveEntity = (Action<GoldMiner, GoldMinerEntity>)Delegate.Combine(goldMiner.onResolveEntity, new Action<GoldMiner, GoldMinerEntity>(this.OnResolveEntity));
		}

		// Token: 0x06001632 RID: 5682 RVA: 0x00051E0B File Offset: 0x0005000B
		protected override void OnDetached(GoldMinerArtifact artifact)
		{
			if (base.GoldMiner == null)
			{
				return;
			}
			GoldMiner goldMiner = base.GoldMiner;
			goldMiner.onResolveEntity = (Action<GoldMiner, GoldMinerEntity>)Delegate.Remove(goldMiner.onResolveEntity, new Action<GoldMiner, GoldMinerEntity>(this.OnResolveEntity));
		}

		// Token: 0x06001633 RID: 5683 RVA: 0x00051E43 File Offset: 0x00050043
		private void OnResolveEntity(GoldMiner miner, GoldMinerEntity entity)
		{
			if (entity == null)
			{
				return;
			}
			if (base.Run.IsRock(entity) && entity.size > GoldMinerEntity.Size.M)
			{
				base.Run.levelScoreFactor += 0.3f;
			}
		}
	}
}
