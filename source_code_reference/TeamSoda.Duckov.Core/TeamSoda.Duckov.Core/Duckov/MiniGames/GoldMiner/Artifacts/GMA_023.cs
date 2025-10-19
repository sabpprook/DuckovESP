using System;
using ItemStatsSystem.Stats;

namespace Duckov.MiniGames.GoldMiner.Artifacts
{
	// Token: 0x020002C0 RID: 704
	public class GMA_023 : GoldMinerArtifactBehaviour
	{
		// Token: 0x06001671 RID: 5745 RVA: 0x00052777 File Offset: 0x00050977
		protected override void OnAttached(GoldMinerArtifact artifact)
		{
			if (base.Run == null)
			{
				return;
			}
			base.Run.strength.AddModifier(new Modifier(ModifierType.Add, 10f, this));
		}

		// Token: 0x06001672 RID: 5746 RVA: 0x0005279E File Offset: 0x0005099E
		protected override void OnDetached(GoldMinerArtifact artifact)
		{
			if (base.Run == null)
			{
				return;
			}
			base.Run.strength.RemoveAllModifiersFromSource(this);
		}
	}
}
