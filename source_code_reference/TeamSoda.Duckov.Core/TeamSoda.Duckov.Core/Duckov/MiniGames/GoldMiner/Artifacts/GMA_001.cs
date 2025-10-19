using System;
using ItemStatsSystem.Stats;

namespace Duckov.MiniGames.GoldMiner.Artifacts
{
	// Token: 0x020002AA RID: 682
	public class GMA_001 : GoldMinerArtifactBehaviour
	{
		// Token: 0x06001624 RID: 5668 RVA: 0x00051A84 File Offset: 0x0004FC84
		protected override void OnAttached(GoldMinerArtifact artifact)
		{
			if (base.Run == null)
			{
				return;
			}
			this.cachedRun = base.Run;
			this.staminaModifier = new Modifier(ModifierType.Add, 1f, this);
			this.scoreFactorModifier = new Modifier(ModifierType.Add, 1f, this);
			this.cachedRun.staminaDrain.AddModifier(this.staminaModifier);
			this.cachedRun.scoreFactorBase.AddModifier(this.scoreFactorModifier);
		}

		// Token: 0x06001625 RID: 5669 RVA: 0x00051AF6 File Offset: 0x0004FCF6
		protected override void OnDetached(GoldMinerArtifact artifact)
		{
			if (this.cachedRun == null)
			{
				return;
			}
			this.cachedRun.staminaDrain.RemoveModifier(this.staminaModifier);
			this.cachedRun.scoreFactorBase.RemoveModifier(this.scoreFactorModifier);
		}

		// Token: 0x04001072 RID: 4210
		private Modifier staminaModifier;

		// Token: 0x04001073 RID: 4211
		private Modifier scoreFactorModifier;

		// Token: 0x04001074 RID: 4212
		private GoldMinerRunData cachedRun;
	}
}
