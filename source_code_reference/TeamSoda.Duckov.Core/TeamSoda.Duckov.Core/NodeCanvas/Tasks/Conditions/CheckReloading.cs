using System;
using NodeCanvas.Framework;

namespace NodeCanvas.Tasks.Conditions
{
	// Token: 0x02000401 RID: 1025
	public class CheckReloading : ConditionTask<AICharacterController>
	{
		// Token: 0x06002521 RID: 9505 RVA: 0x00080182 File Offset: 0x0007E382
		protected override bool OnCheck()
		{
			return !(base.agent == null) && !(base.agent.CharacterMainControl == null) && base.agent.CharacterMainControl.reloadAction.Running;
		}
	}
}
