using System;
using ItemStatsSystem.Stats;

namespace Duckov.MiniGames.GoldMiner.Artifacts
{
	// Token: 0x020002BA RID: 698
	public class GMA_017 : GoldMinerArtifactBehaviour
	{
		// Token: 0x06001660 RID: 5728 RVA: 0x00052549 File Offset: 0x00050749
		protected override void OnAttached(GoldMinerArtifact artifact)
		{
			if (base.Run == null)
			{
				return;
			}
			base.Run.goldValueFactor.AddModifier(new Modifier(ModifierType.Add, 0.2f, this));
		}

		// Token: 0x06001661 RID: 5729 RVA: 0x00052570 File Offset: 0x00050770
		protected override void OnDetached(GoldMinerArtifact artifact)
		{
			if (base.Run == null)
			{
				return;
			}
			base.Run.goldValueFactor.RemoveAllModifiersFromSource(this);
		}
	}
}
