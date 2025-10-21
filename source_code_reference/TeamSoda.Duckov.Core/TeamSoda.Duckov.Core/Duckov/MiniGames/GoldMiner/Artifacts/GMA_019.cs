using System;

namespace Duckov.MiniGames.GoldMiner.Artifacts
{
	// Token: 0x020002BC RID: 700
	public class GMA_019 : GoldMinerArtifactBehaviour
	{
		// Token: 0x06001668 RID: 5736 RVA: 0x000526AD File Offset: 0x000508AD
		protected override void OnAttached(GoldMinerArtifact artifact)
		{
			if (base.Run == null)
			{
				return;
			}
			base.Run.bomb += 3;
		}
	}
}
