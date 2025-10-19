using System;
using UnityEngine;

namespace Duckov.MiniGames.GoldMiner.Artifacts
{
	// Token: 0x020002C8 RID: 712
	public class GMA_031 : GoldMinerArtifactBehaviour
	{
		// Token: 0x06001686 RID: 5766 RVA: 0x00052997 File Offset: 0x00050B97
		protected override void OnAttached(GoldMinerArtifact artifact)
		{
			if (base.Run == null)
			{
				return;
			}
			base.Run.extraGold = Mathf.MoveTowards(base.Run.extraGold, 5f, 1f);
		}
	}
}
