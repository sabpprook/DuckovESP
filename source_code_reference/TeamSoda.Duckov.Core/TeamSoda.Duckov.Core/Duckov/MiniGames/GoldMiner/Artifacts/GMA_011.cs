using System;

namespace Duckov.MiniGames.GoldMiner.Artifacts
{
	// Token: 0x020002B4 RID: 692
	public class GMA_011 : GoldMinerArtifactBehaviour
	{
		// Token: 0x0600164D RID: 5709 RVA: 0x000522DD File Offset: 0x000504DD
		protected override void OnAttached(GoldMinerArtifact artifact)
		{
			if (base.Run == null)
			{
				return;
			}
			base.Run.forceLevelSuccessFuncs.Add(new Func<bool>(this.ForceAndDetach));
		}

		// Token: 0x0600164E RID: 5710 RVA: 0x00052304 File Offset: 0x00050504
		private bool ForceAndDetach()
		{
			base.Run.DetachArtifact(this.master);
			return true;
		}

		// Token: 0x0600164F RID: 5711 RVA: 0x00052319 File Offset: 0x00050519
		protected override void OnDetached(GoldMinerArtifact artifact)
		{
			if (base.Run == null)
			{
				return;
			}
			base.Run.forceLevelSuccessFuncs.Remove(new Func<bool>(this.ForceAndDetach));
		}
	}
}
