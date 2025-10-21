using System;

namespace Duckov.MiniGames.GoldMiner.Artifacts
{
	// Token: 0x020002BB RID: 699
	public class GMA_018 : GoldMinerArtifactBehaviour
	{
		// Token: 0x06001663 RID: 5731 RVA: 0x00052598 File Offset: 0x00050798
		protected override void OnAttached(GoldMinerArtifact artifact)
		{
			if (base.GoldMiner == null)
			{
				return;
			}
			GoldMiner goldMiner = base.GoldMiner;
			goldMiner.onLevelBegin = (Action<GoldMiner>)Delegate.Combine(goldMiner.onLevelBegin, new Action<GoldMiner>(this.OnLevelBegin));
			GoldMiner goldMiner2 = base.GoldMiner;
			goldMiner2.onResolveEntity = (Action<GoldMiner, GoldMinerEntity>)Delegate.Combine(goldMiner2.onResolveEntity, new Action<GoldMiner, GoldMinerEntity>(this.OnResolveEntity));
		}

		// Token: 0x06001664 RID: 5732 RVA: 0x00052604 File Offset: 0x00050804
		protected override void OnDetached(GoldMinerArtifact artifact)
		{
			if (base.GoldMiner == null)
			{
				return;
			}
			GoldMiner goldMiner = base.GoldMiner;
			goldMiner.onLevelBegin = (Action<GoldMiner>)Delegate.Remove(goldMiner.onLevelBegin, new Action<GoldMiner>(this.OnLevelBegin));
			GoldMiner goldMiner2 = base.GoldMiner;
			goldMiner2.onResolveEntity = (Action<GoldMiner, GoldMinerEntity>)Delegate.Remove(goldMiner2.onResolveEntity, new Action<GoldMiner, GoldMinerEntity>(this.OnResolveEntity));
		}

		// Token: 0x06001665 RID: 5733 RVA: 0x0005266E File Offset: 0x0005086E
		private void OnLevelBegin(GoldMiner miner)
		{
			this.remaining = 5;
		}

		// Token: 0x06001666 RID: 5734 RVA: 0x00052677 File Offset: 0x00050877
		private void OnResolveEntity(GoldMiner miner, GoldMinerEntity entity)
		{
			if (!entity)
			{
				return;
			}
			if (this.remaining < 1)
			{
				return;
			}
			this.remaining--;
			entity.Value = 200;
		}

		// Token: 0x0400107C RID: 4220
		private int remaining;
	}
}
