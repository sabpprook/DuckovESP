using System;
using ItemStatsSystem.Stats;
using UnityEngine;

namespace Duckov.MiniGames.GoldMiner.Artifacts
{
	// Token: 0x020002BF RID: 703
	public class GMA_022 : GoldMinerArtifactBehaviour
	{
		// Token: 0x0600166E RID: 5742 RVA: 0x0005271F File Offset: 0x0005091F
		protected override void OnAttached(GoldMinerArtifact artifact)
		{
			if (base.Run == null)
			{
				return;
			}
			base.Run.charm.AddModifier(new Modifier(ModifierType.Add, this.amount, this));
		}

		// Token: 0x0600166F RID: 5743 RVA: 0x00052747 File Offset: 0x00050947
		protected override void OnDetached(GoldMinerArtifact artifact)
		{
			if (base.Run == null)
			{
				return;
			}
			base.Run.charm.RemoveAllModifiersFromSource(this);
		}

		// Token: 0x0400107D RID: 4221
		[SerializeField]
		private float amount = 0.1f;
	}
}
