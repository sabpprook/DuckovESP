using System;

namespace Duckov.MiniGames.GoldMiner.Artifacts
{
	// Token: 0x020002BD RID: 701
	public class GMA_020 : GoldMinerArtifactBehaviour
	{
		// Token: 0x0600166A RID: 5738 RVA: 0x000526D3 File Offset: 0x000508D3
		protected override void OnAttached(GoldMinerArtifact artifact)
		{
			if (base.Run == null)
			{
				return;
			}
			base.Run.strengthPotion += 3;
		}
	}
}
