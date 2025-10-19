using System;
using System.Linq;

namespace Duckov.MiniGames.GoldMiner.Artifacts
{
	// Token: 0x020002B0 RID: 688
	public class GMA_007 : GoldMinerArtifactBehaviour
	{
		// Token: 0x0600163D RID: 5693 RVA: 0x00051FD2 File Offset: 0x000501D2
		protected override void OnAttached(GoldMinerArtifact artifact)
		{
			if (base.Run == null)
			{
				return;
			}
			base.Run.additionalFactorFuncs.Add(new Func<float>(this.AddFactorIfResolved3DifferentKindsOfGold));
		}

		// Token: 0x0600163E RID: 5694 RVA: 0x00051FF9 File Offset: 0x000501F9
		protected override void OnDetached(GoldMinerArtifact artifact)
		{
			if (base.Run == null)
			{
				return;
			}
			base.Run.additionalFactorFuncs.Remove(new Func<float>(this.AddFactorIfResolved3DifferentKindsOfGold));
		}

		// Token: 0x0600163F RID: 5695 RVA: 0x00052024 File Offset: 0x00050224
		private float AddFactorIfResolved3DifferentKindsOfGold()
		{
			if ((from e in base.GoldMiner.resolvedEntities
				where e != null && e.tags.Contains(GoldMinerEntity.Tag.Gold)
				group e by e.size).Count<IGrouping<GoldMinerEntity.Size, GoldMinerEntity>>() >= 3)
			{
				return 0.5f;
			}
			return 0f;
		}
	}
}
