using System;
using System.Linq;

namespace Duckov.MiniGames.GoldMiner.Artifacts
{
	// Token: 0x020002AF RID: 687
	public class GMA_006 : GoldMinerArtifactBehaviour
	{
		// Token: 0x06001639 RID: 5689 RVA: 0x00051F5F File Offset: 0x0005015F
		protected override void OnAttached(GoldMinerArtifact artifact)
		{
			if (base.Run == null)
			{
				return;
			}
			base.Run.isGoldPredicators.Add(new Func<GoldMinerEntity, bool>(this.SmallRockIsGold));
		}

		// Token: 0x0600163A RID: 5690 RVA: 0x00051F86 File Offset: 0x00050186
		private bool SmallRockIsGold(GoldMinerEntity entity)
		{
			return entity.tags.Contains(GoldMinerEntity.Tag.Rock) && entity.size < GoldMinerEntity.Size.M;
		}

		// Token: 0x0600163B RID: 5691 RVA: 0x00051FA2 File Offset: 0x000501A2
		protected override void OnDetached(GoldMinerArtifact artifact)
		{
			if (base.Run == null)
			{
				return;
			}
			base.Run.isGoldPredicators.Remove(new Func<GoldMinerEntity, bool>(this.SmallRockIsGold));
		}
	}
}
