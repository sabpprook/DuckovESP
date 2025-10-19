using System;
using ItemStatsSystem.Stats;
using UnityEngine;

namespace Duckov.MiniGames.GoldMiner.Artifacts
{
	// Token: 0x020002C2 RID: 706
	public class GMA_025 : GoldMinerArtifactBehaviour
	{
		// Token: 0x06001677 RID: 5751 RVA: 0x0005280F File Offset: 0x00050A0F
		protected override void OnAttached(GoldMinerArtifact artifact)
		{
			if (base.Run == null)
			{
				return;
			}
			base.Run.emptySpeed.AddModifier(new Modifier(ModifierType.PercentageAdd, this.addAmount, this));
		}

		// Token: 0x06001678 RID: 5752 RVA: 0x00052838 File Offset: 0x00050A38
		protected override void OnDetached(GoldMinerArtifact artifact)
		{
			if (base.Run == null)
			{
				return;
			}
			base.Run.emptySpeed.RemoveAllModifiersFromSource(this);
		}

		// Token: 0x0400107E RID: 4222
		[SerializeField]
		private float addAmount = 1f;
	}
}
