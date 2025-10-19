using System;
using ItemStatsSystem.Stats;

namespace Duckov.MiniGames.GoldMiner.Artifacts
{
	// Token: 0x020002B5 RID: 693
	public class GMA_012 : GoldMinerArtifactBehaviour
	{
		// Token: 0x06001651 RID: 5713 RVA: 0x00052349 File Offset: 0x00050549
		protected override void OnAttached(GoldMinerArtifact artifact)
		{
			if (base.Run == null)
			{
				return;
			}
			base.Run.defuse.AddModifier(new Modifier(ModifierType.Add, 1f, this));
		}

		// Token: 0x06001652 RID: 5714 RVA: 0x00052370 File Offset: 0x00050570
		protected override void OnDetached(GoldMinerArtifact artifact)
		{
			if (base.Run == null)
			{
				return;
			}
			base.Run.defuse.RemoveAllModifiersFromSource(this);
		}
	}
}
