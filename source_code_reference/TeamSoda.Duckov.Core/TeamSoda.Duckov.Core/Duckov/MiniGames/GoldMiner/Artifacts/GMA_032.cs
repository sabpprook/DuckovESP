using System;
using UnityEngine;

namespace Duckov.MiniGames.GoldMiner.Artifacts
{
	// Token: 0x020002C9 RID: 713
	public class GMA_032 : GoldMinerArtifactBehaviour
	{
		// Token: 0x06001688 RID: 5768 RVA: 0x000529CF File Offset: 0x00050BCF
		protected override void OnAttached(GoldMinerArtifact artifact)
		{
			if (base.Run == null)
			{
				return;
			}
			base.Run.extraDiamond = Mathf.MoveTowards(base.Run.extraDiamond, 5f, 0.5f);
		}
	}
}
