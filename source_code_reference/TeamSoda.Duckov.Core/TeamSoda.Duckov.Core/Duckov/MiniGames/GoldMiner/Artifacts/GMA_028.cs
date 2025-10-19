using System;

namespace Duckov.MiniGames.GoldMiner.Artifacts
{
	// Token: 0x020002C5 RID: 709
	public class GMA_028 : GoldMinerArtifactBehaviour
	{
		// Token: 0x06001680 RID: 5760 RVA: 0x00052904 File Offset: 0x00050B04
		protected override void OnAttached(GoldMinerArtifact artifact)
		{
			if (base.Run == null)
			{
				return;
			}
			base.Run.shopTicket++;
		}
	}
}
