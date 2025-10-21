using System;
using UnityEngine;

namespace Duckov.MiniGames.GoldMiner.Artifacts
{
	// Token: 0x020002C7 RID: 711
	public class GMA_030 : GoldMinerArtifactBehaviour
	{
		// Token: 0x06001684 RID: 5764 RVA: 0x0005295F File Offset: 0x00050B5F
		protected override void OnAttached(GoldMinerArtifact artifact)
		{
			if (base.Run == null)
			{
				return;
			}
			base.Run.extraRocks = Mathf.MoveTowards(base.Run.extraRocks, 5f, 1f);
		}
	}
}
