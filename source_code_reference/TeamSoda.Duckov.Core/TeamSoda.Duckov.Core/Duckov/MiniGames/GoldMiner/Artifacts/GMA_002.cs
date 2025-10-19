using System;
using ItemStatsSystem.Stats;

namespace Duckov.MiniGames.GoldMiner.Artifacts
{
	// Token: 0x020002AB RID: 683
	public class GMA_002 : GoldMinerArtifactBehaviour
	{
		// Token: 0x06001627 RID: 5671 RVA: 0x00051B38 File Offset: 0x0004FD38
		protected override void OnAttached(GoldMinerArtifact artifact)
		{
			if (this.master == null)
			{
				return;
			}
			if (base.GoldMiner == null)
			{
				return;
			}
			this.modifer = new Modifier(ModifierType.PercentageMultiply, -0.5f, this);
			GoldMiner goldMiner = base.GoldMiner;
			goldMiner.onResolveEntity = (Action<GoldMiner, GoldMinerEntity>)Delegate.Combine(goldMiner.onResolveEntity, new Action<GoldMiner, GoldMinerEntity>(this.OnResolveEntity));
			GoldMiner goldMiner2 = base.GoldMiner;
			goldMiner2.onHookBeginRetrieve = (Action<GoldMiner, Hook>)Delegate.Combine(goldMiner2.onHookBeginRetrieve, new Action<GoldMiner, Hook>(this.OnBeginRetrieve));
			GoldMiner goldMiner3 = base.GoldMiner;
			goldMiner3.onHookEndRetrieve = (Action<GoldMiner, Hook>)Delegate.Combine(goldMiner3.onHookEndRetrieve, new Action<GoldMiner, Hook>(this.OnEndRetrieve));
		}

		// Token: 0x06001628 RID: 5672 RVA: 0x00051BF0 File Offset: 0x0004FDF0
		protected override void OnDetached(GoldMinerArtifact artifact)
		{
			if (base.GoldMiner == null)
			{
				return;
			}
			GoldMiner goldMiner = base.GoldMiner;
			goldMiner.onResolveEntity = (Action<GoldMiner, GoldMinerEntity>)Delegate.Remove(goldMiner.onResolveEntity, new Action<GoldMiner, GoldMinerEntity>(this.OnResolveEntity));
			GoldMiner goldMiner2 = base.GoldMiner;
			goldMiner2.onHookBeginRetrieve = (Action<GoldMiner, Hook>)Delegate.Remove(goldMiner2.onHookBeginRetrieve, new Action<GoldMiner, Hook>(this.OnBeginRetrieve));
			GoldMiner goldMiner3 = base.GoldMiner;
			goldMiner3.onHookEndRetrieve = (Action<GoldMiner, Hook>)Delegate.Remove(goldMiner3.onHookEndRetrieve, new Action<GoldMiner, Hook>(this.OnEndRetrieve));
			if (base.Run != null)
			{
				base.Run.staminaDrain.RemoveModifier(this.modifer);
			}
		}

		// Token: 0x06001629 RID: 5673 RVA: 0x00051CA0 File Offset: 0x0004FEA0
		private void OnBeginRetrieve(GoldMiner miner, Hook hook)
		{
			if (!this.effectActive)
			{
				return;
			}
			base.Run.staminaDrain.AddModifier(this.modifer);
		}

		// Token: 0x0600162A RID: 5674 RVA: 0x00051CC1 File Offset: 0x0004FEC1
		private void OnEndRetrieve(GoldMiner miner, Hook hook)
		{
			base.Run.staminaDrain.RemoveModifier(this.modifer);
			this.effectActive = false;
		}

		// Token: 0x0600162B RID: 5675 RVA: 0x00051CE1 File Offset: 0x0004FEE1
		private void OnResolveEntity(GoldMiner miner, GoldMinerEntity entity)
		{
			this.effectActive = true;
		}

		// Token: 0x04001075 RID: 4213
		private Modifier modifer;

		// Token: 0x04001076 RID: 4214
		private bool effectActive;
	}
}
