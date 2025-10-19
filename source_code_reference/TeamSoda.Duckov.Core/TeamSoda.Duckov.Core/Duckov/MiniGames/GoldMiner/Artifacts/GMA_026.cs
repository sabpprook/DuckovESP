using System;
using ItemStatsSystem.Stats;

namespace Duckov.MiniGames.GoldMiner.Artifacts
{
	// Token: 0x020002C3 RID: 707
	public class GMA_026 : GoldMinerArtifactBehaviour
	{
		// Token: 0x0600167A RID: 5754 RVA: 0x00052868 File Offset: 0x00050A68
		protected override void OnAttached(GoldMinerArtifact artifact)
		{
			if (base.Run == null)
			{
				return;
			}
			base.Run.shopRefreshPrice.AddModifier(new Modifier(ModifierType.PercentageMultiply, -1f, this));
		}

		// Token: 0x0600167B RID: 5755 RVA: 0x00052893 File Offset: 0x00050A93
		protected override void OnDetached(GoldMinerArtifact artifact)
		{
			if (base.Run == null)
			{
				return;
			}
			base.Run.shopRefreshPrice.RemoveAllModifiersFromSource(this);
		}
	}
}
