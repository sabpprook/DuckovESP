using System;
using ItemStatsSystem.Stats;

namespace Duckov.MiniGames.GoldMiner.Artifacts
{
	// Token: 0x020002B9 RID: 697
	public class GMA_016 : GoldMinerArtifactBehaviour
	{
		// Token: 0x0600165D RID: 5725 RVA: 0x000524FD File Offset: 0x000506FD
		protected override void OnAttached(GoldMinerArtifact artifact)
		{
			if (base.Run == null)
			{
				return;
			}
			base.Run.rockValueFactor.AddModifier(new Modifier(ModifierType.Add, 1f, this));
		}

		// Token: 0x0600165E RID: 5726 RVA: 0x00052524 File Offset: 0x00050724
		protected override void OnDetached(GoldMinerArtifact artifact)
		{
			if (base.Run == null)
			{
				return;
			}
			base.Run.rockValueFactor.RemoveAllModifiersFromSource(this);
		}
	}
}
