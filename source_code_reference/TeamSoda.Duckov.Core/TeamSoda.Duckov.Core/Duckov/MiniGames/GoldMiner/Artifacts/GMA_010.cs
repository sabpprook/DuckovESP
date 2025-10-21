using System;

namespace Duckov.MiniGames.GoldMiner.Artifacts
{
	// Token: 0x020002B3 RID: 691
	public class GMA_010 : GoldMinerArtifactBehaviour
	{
		// Token: 0x0600164A RID: 5706 RVA: 0x000522A3 File Offset: 0x000504A3
		protected override void OnAttached(GoldMinerArtifact artifact)
		{
			if (base.Run == null)
			{
				return;
			}
			base.Run.minMoneySum = 1000;
		}

		// Token: 0x0600164B RID: 5707 RVA: 0x000522BE File Offset: 0x000504BE
		protected override void OnDetached(GoldMinerArtifact artifact)
		{
			if (base.Run == null)
			{
				return;
			}
			base.Run.minMoneySum = 0;
		}
	}
}
