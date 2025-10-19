using System;
using NodeCanvas.Framework;

namespace NodeCanvas.Tasks.Actions
{
	// Token: 0x02000416 RID: 1046
	public class StopMoving : ActionTask<AICharacterController>
	{
		// Token: 0x0600259D RID: 9629 RVA: 0x00081833 File Offset: 0x0007FA33
		protected override string OnInit()
		{
			return null;
		}

		// Token: 0x0600259E RID: 9630 RVA: 0x00081836 File Offset: 0x0007FA36
		protected override void OnExecute()
		{
			base.agent.StopMove();
			base.EndAction(true);
		}
	}
}
