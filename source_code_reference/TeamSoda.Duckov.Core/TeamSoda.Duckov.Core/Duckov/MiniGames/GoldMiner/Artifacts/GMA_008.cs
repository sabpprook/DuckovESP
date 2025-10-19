using System;

namespace Duckov.MiniGames.GoldMiner.Artifacts
{
	// Token: 0x020002B1 RID: 689
	public class GMA_008 : GoldMinerArtifactBehaviour
	{
		// Token: 0x06001641 RID: 5697 RVA: 0x000520A0 File Offset: 0x000502A0
		protected override void OnAttached(GoldMinerArtifact artifact)
		{
			if (base.GoldMiner == null)
			{
				return;
			}
			GoldMiner goldMiner = base.GoldMiner;
			goldMiner.onLevelBegin = (Action<GoldMiner>)Delegate.Combine(goldMiner.onLevelBegin, new Action<GoldMiner>(this.OnLevelBegin));
			GoldMiner goldMiner2 = base.GoldMiner;
			goldMiner2.onAfterResolveEntity = (Action<GoldMiner, GoldMinerEntity>)Delegate.Combine(goldMiner2.onAfterResolveEntity, new Action<GoldMiner, GoldMinerEntity>(this.OnAfterResolveEntity));
		}

		// Token: 0x06001642 RID: 5698 RVA: 0x0005210C File Offset: 0x0005030C
		protected override void OnDetached(GoldMinerArtifact artifact)
		{
			if (base.GoldMiner == null)
			{
				return;
			}
			GoldMiner goldMiner = base.GoldMiner;
			goldMiner.onLevelBegin = (Action<GoldMiner>)Delegate.Remove(goldMiner.onLevelBegin, new Action<GoldMiner>(this.OnLevelBegin));
			GoldMiner goldMiner2 = base.GoldMiner;
			goldMiner2.onAfterResolveEntity = (Action<GoldMiner, GoldMinerEntity>)Delegate.Remove(goldMiner2.onAfterResolveEntity, new Action<GoldMiner, GoldMinerEntity>(this.OnAfterResolveEntity));
		}

		// Token: 0x06001643 RID: 5699 RVA: 0x00052176 File Offset: 0x00050376
		private void OnLevelBegin(GoldMiner miner)
		{
			this.triggered = false;
		}

		// Token: 0x06001644 RID: 5700 RVA: 0x00052180 File Offset: 0x00050380
		private void OnAfterResolveEntity(GoldMiner miner, GoldMinerEntity entity)
		{
			if (this.triggered)
			{
				return;
			}
			if (base.GoldMiner.activeEntities.Count <= 0)
			{
				this.triggered = true;
				base.Run.charm.BaseValue += 0.5f;
			}
		}

		// Token: 0x04001079 RID: 4217
		private bool triggered;
	}
}
