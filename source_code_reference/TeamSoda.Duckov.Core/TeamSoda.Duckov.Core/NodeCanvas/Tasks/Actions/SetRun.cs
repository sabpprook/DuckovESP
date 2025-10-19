using System;
using NodeCanvas.Framework;

namespace NodeCanvas.Tasks.Actions
{
	// Token: 0x02000413 RID: 1043
	public class SetRun : ActionTask<AICharacterController>
	{
		// Token: 0x17000734 RID: 1844
		// (get) Token: 0x06002589 RID: 9609 RVA: 0x00081592 File Offset: 0x0007F792
		protected override string info
		{
			get
			{
				return string.Format("SetRun:{0}", this.run.value);
			}
		}

		// Token: 0x0600258A RID: 9610 RVA: 0x000815AE File Offset: 0x0007F7AE
		protected override string OnInit()
		{
			return null;
		}

		// Token: 0x0600258B RID: 9611 RVA: 0x000815B1 File Offset: 0x0007F7B1
		protected override void OnExecute()
		{
			base.agent.CharacterMainControl.SetRunInput(this.run.value);
			base.EndAction(true);
		}

		// Token: 0x0600258C RID: 9612 RVA: 0x000815D5 File Offset: 0x0007F7D5
		protected override void OnStop()
		{
		}

		// Token: 0x0600258D RID: 9613 RVA: 0x000815D7 File Offset: 0x0007F7D7
		protected override void OnPause()
		{
		}

		// Token: 0x04001991 RID: 6545
		public BBParameter<bool> run;
	}
}
