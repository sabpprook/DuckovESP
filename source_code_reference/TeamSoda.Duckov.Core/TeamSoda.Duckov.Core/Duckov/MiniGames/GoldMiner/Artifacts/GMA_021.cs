using System;

namespace Duckov.MiniGames.GoldMiner.Artifacts
{
	// Token: 0x020002BE RID: 702
	public class GMA_021 : GoldMinerArtifactBehaviour
	{
		// Token: 0x0600166C RID: 5740 RVA: 0x000526F9 File Offset: 0x000508F9
		protected override void OnAttached(GoldMinerArtifact artifact)
		{
			if (base.Run == null)
			{
				return;
			}
			base.Run.eagleEyePotion += 3;
		}
	}
}
