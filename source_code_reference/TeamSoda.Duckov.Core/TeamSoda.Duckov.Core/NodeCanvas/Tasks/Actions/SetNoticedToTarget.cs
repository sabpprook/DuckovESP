using System;
using NodeCanvas.Framework;

namespace NodeCanvas.Tasks.Actions
{
	// Token: 0x02000412 RID: 1042
	public class SetNoticedToTarget : ActionTask<AICharacterController>
	{
		// Token: 0x06002583 RID: 9603 RVA: 0x0008155D File Offset: 0x0007F75D
		protected override string OnInit()
		{
			return null;
		}

		// Token: 0x17000733 RID: 1843
		// (get) Token: 0x06002584 RID: 9604 RVA: 0x00081560 File Offset: 0x0007F760
		protected override string info
		{
			get
			{
				return "set noticed to";
			}
		}

		// Token: 0x06002585 RID: 9605 RVA: 0x00081567 File Offset: 0x0007F767
		protected override void OnExecute()
		{
			base.agent.SetNoticedToTarget(this.target.value);
			base.EndAction(true);
		}

		// Token: 0x06002586 RID: 9606 RVA: 0x00081586 File Offset: 0x0007F786
		protected override void OnStop()
		{
		}

		// Token: 0x06002587 RID: 9607 RVA: 0x00081588 File Offset: 0x0007F788
		protected override void OnPause()
		{
		}

		// Token: 0x04001990 RID: 6544
		public BBParameter<DamageReceiver> target;
	}
}
