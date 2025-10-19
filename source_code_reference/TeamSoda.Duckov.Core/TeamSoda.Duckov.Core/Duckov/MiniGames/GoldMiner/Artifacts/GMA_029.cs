using System;

namespace Duckov.MiniGames.GoldMiner.Artifacts
{
	// Token: 0x020002C6 RID: 710
	public class GMA_029 : GoldMinerArtifactBehaviour
	{
		// Token: 0x06001682 RID: 5762 RVA: 0x0005292A File Offset: 0x00050B2A
		protected override void OnAttached(GoldMinerArtifact artifact)
		{
			if (base.Run == null)
			{
				return;
			}
			if (base.Run.shopCapacity >= 6)
			{
				return;
			}
			base.Run.shopCapacity++;
		}
	}
}
