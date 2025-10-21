using System;
using UnityEngine;

namespace Duckov.MiniGames.GoldMiner
{
	// Token: 0x02000289 RID: 649
	public abstract class GoldMinerArtifactBehaviour : MiniGameBehaviour
	{
		// Token: 0x170003E1 RID: 993
		// (get) Token: 0x060014F8 RID: 5368 RVA: 0x0004DAAA File Offset: 0x0004BCAA
		protected GoldMinerRunData Run
		{
			get
			{
				if (this.master == null)
				{
					return null;
				}
				if (this.master.Master == null)
				{
					return null;
				}
				return this.master.Master.run;
			}
		}

		// Token: 0x170003E2 RID: 994
		// (get) Token: 0x060014F9 RID: 5369 RVA: 0x0004DAE1 File Offset: 0x0004BCE1
		protected GoldMiner GoldMiner
		{
			get
			{
				if (this.master == null)
				{
					return null;
				}
				return this.master.Master;
			}
		}

		// Token: 0x060014FA RID: 5370 RVA: 0x0004DB00 File Offset: 0x0004BD00
		private void Awake()
		{
			if (!this.master)
			{
				this.master = base.GetComponent<GoldMinerArtifact>();
			}
			GoldMinerArtifact goldMinerArtifact = this.master;
			goldMinerArtifact.OnAttached = (Action<GoldMinerArtifact>)Delegate.Combine(goldMinerArtifact.OnAttached, new Action<GoldMinerArtifact>(this.OnAttached));
			GoldMinerArtifact goldMinerArtifact2 = this.master;
			goldMinerArtifact2.OnDetached = (Action<GoldMinerArtifact>)Delegate.Combine(goldMinerArtifact2.OnDetached, new Action<GoldMinerArtifact>(this.OnDetached));
		}

		// Token: 0x060014FB RID: 5371 RVA: 0x0004DB76 File Offset: 0x0004BD76
		protected virtual void OnAttached(GoldMinerArtifact artifact)
		{
		}

		// Token: 0x060014FC RID: 5372 RVA: 0x0004DB78 File Offset: 0x0004BD78
		protected virtual void OnDetached(GoldMinerArtifact artifact)
		{
		}

		// Token: 0x04000F68 RID: 3944
		[SerializeField]
		protected GoldMinerArtifact master;
	}
}
