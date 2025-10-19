using System;

namespace Duckov.MiniGames.GoldMiner.Artifacts
{
	// Token: 0x020002AE RID: 686
	public class GMA_005 : GoldMinerArtifactBehaviour
	{
		// Token: 0x06001635 RID: 5685 RVA: 0x00051E85 File Offset: 0x00050085
		protected override void OnAttached(GoldMinerArtifact artifact)
		{
			if (base.GoldMiner == null)
			{
				return;
			}
			GoldMiner goldMiner = base.GoldMiner;
			goldMiner.onResolveEntity = (Action<GoldMiner, GoldMinerEntity>)Delegate.Combine(goldMiner.onResolveEntity, new Action<GoldMiner, GoldMinerEntity>(this.OnResolveEntity));
		}

		// Token: 0x06001636 RID: 5686 RVA: 0x00051EBD File Offset: 0x000500BD
		protected override void OnDetached(GoldMinerArtifact artifact)
		{
			if (base.GoldMiner == null)
			{
				return;
			}
			GoldMiner goldMiner = base.GoldMiner;
			goldMiner.onResolveEntity = (Action<GoldMiner, GoldMinerEntity>)Delegate.Remove(goldMiner.onResolveEntity, new Action<GoldMiner, GoldMinerEntity>(this.OnResolveEntity));
		}

		// Token: 0x06001637 RID: 5687 RVA: 0x00051EF8 File Offset: 0x000500F8
		private void OnResolveEntity(GoldMiner miner, GoldMinerEntity entity)
		{
			if (this.remaining < 1)
			{
				return;
			}
			if (entity == null)
			{
				return;
			}
			if (base.Run.IsRock(entity) && entity.size < GoldMinerEntity.Size.M)
			{
				entity.Value += 500;
				this.remaining--;
			}
		}

		// Token: 0x04001078 RID: 4216
		private int remaining = 3;
	}
}
