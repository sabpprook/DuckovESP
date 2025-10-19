using System;
using ItemStatsSystem.Stats;

namespace Duckov.MiniGames.GoldMiner.Artifacts
{
	// Token: 0x020002C4 RID: 708
	public class GMA_027 : GoldMinerArtifactBehaviour
	{
		// Token: 0x0600167D RID: 5757 RVA: 0x000528B8 File Offset: 0x00050AB8
		protected override void OnAttached(GoldMinerArtifact artifact)
		{
			if (base.Run == null)
			{
				return;
			}
			base.Run.shopRefreshChances.AddModifier(new Modifier(ModifierType.Add, 1f, this));
		}

		// Token: 0x0600167E RID: 5758 RVA: 0x000528DF File Offset: 0x00050ADF
		protected override void OnDetached(GoldMinerArtifact artifact)
		{
			if (base.Run == null)
			{
				return;
			}
			base.Run.shopRefreshChances.RemoveAllModifiersFromSource(this);
		}
	}
}
