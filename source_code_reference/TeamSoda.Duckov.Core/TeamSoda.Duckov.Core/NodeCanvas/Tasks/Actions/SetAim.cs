using System;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions
{
	// Token: 0x02000411 RID: 1041
	public class SetAim : ActionTask<AICharacterController>
	{
		// Token: 0x17000732 RID: 1842
		// (get) Token: 0x0600257C RID: 9596 RVA: 0x00081450 File Offset: 0x0007F650
		protected override string info
		{
			get
			{
				if (this.useTransfom && string.IsNullOrEmpty(this.aimTarget.name))
				{
					return "Set aim to null";
				}
				if (!this.useTransfom)
				{
					return "Set aim to " + this.aimPos.name;
				}
				return "Set aim to " + this.aimTarget.name;
			}
		}

		// Token: 0x0600257D RID: 9597 RVA: 0x000814B0 File Offset: 0x0007F6B0
		protected override string OnInit()
		{
			return null;
		}

		// Token: 0x0600257E RID: 9598 RVA: 0x000814B4 File Offset: 0x0007F6B4
		protected override void OnExecute()
		{
			base.agent.SetTarget(this.aimTarget.value);
			if (!this.useTransfom || !(this.aimTarget.value != null))
			{
				if (!this.useTransfom)
				{
					base.agent.SetAimInput((this.aimPos.value - base.agent.transform.position).normalized, AimTypes.normalAim);
				}
				else
				{
					base.agent.SetAimInput(Vector3.zero, AimTypes.normalAim);
				}
			}
			base.EndAction(true);
		}

		// Token: 0x0600257F RID: 9599 RVA: 0x00081548 File Offset: 0x0007F748
		protected override void OnUpdate()
		{
		}

		// Token: 0x06002580 RID: 9600 RVA: 0x0008154A File Offset: 0x0007F74A
		protected override void OnStop()
		{
		}

		// Token: 0x06002581 RID: 9601 RVA: 0x0008154C File Offset: 0x0007F74C
		protected override void OnPause()
		{
		}

		// Token: 0x0400198C RID: 6540
		public bool useTransfom = true;

		// Token: 0x0400198D RID: 6541
		[ShowIf("useTransfom", 1)]
		public BBParameter<Transform> aimTarget;

		// Token: 0x0400198E RID: 6542
		[ShowIf("useTransfom", 0)]
		public BBParameter<Vector3> aimPos;

		// Token: 0x0400198F RID: 6543
		private bool waitingSearchResult;
	}
}
