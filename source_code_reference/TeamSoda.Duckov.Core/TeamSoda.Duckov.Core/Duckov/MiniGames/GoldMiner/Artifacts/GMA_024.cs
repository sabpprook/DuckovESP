using System;
using ItemStatsSystem.Stats;

namespace Duckov.MiniGames.GoldMiner.Artifacts
{
	// Token: 0x020002C1 RID: 705
	public class GMA_024 : GoldMinerArtifactBehaviour
	{
		// Token: 0x06001674 RID: 5748 RVA: 0x000527C3 File Offset: 0x000509C3
		protected override void OnAttached(GoldMinerArtifact artifact)
		{
			if (base.Run == null)
			{
				return;
			}
			base.Run.maxStamina.AddModifier(new Modifier(ModifierType.Add, 1.5f, this));
		}

		// Token: 0x06001675 RID: 5749 RVA: 0x000527EA File Offset: 0x000509EA
		protected override void OnDetached(GoldMinerArtifact artifact)
		{
			if (base.Run == null)
			{
				return;
			}
			base.Run.maxStamina.RemoveAllModifiersFromSource(this);
		}
	}
}
