using System;
using NodeCanvas.Framework;

namespace NodeCanvas.Tasks.Actions
{
	// Token: 0x02000404 RID: 1028
	public class AimToPlayer : ActionTask<AICharacterController>
	{
		// Token: 0x0600252B RID: 9515 RVA: 0x000802BB File Offset: 0x0007E4BB
		protected override string OnInit()
		{
			return null;
		}

		// Token: 0x0600252C RID: 9516 RVA: 0x000802BE File Offset: 0x0007E4BE
		protected override void OnExecute()
		{
		}

		// Token: 0x0600252D RID: 9517 RVA: 0x000802C0 File Offset: 0x0007E4C0
		protected override void OnUpdate()
		{
			if (!this.target)
			{
				this.target = CharacterMainControl.Main;
			}
			base.agent.CharacterMainControl.SetAimPoint(this.target.transform.position);
		}

		// Token: 0x04001953 RID: 6483
		private CharacterMainControl target;
	}
}
