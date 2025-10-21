using System;
using NodeCanvas.Framework;

namespace NodeCanvas.Tasks.Conditions
{
	// Token: 0x020003FE RID: 1022
	public class CheckHoldGun : ConditionTask<AICharacterController>
	{
		// Token: 0x06002512 RID: 9490 RVA: 0x00080095 File Offset: 0x0007E295
		protected override string OnInit()
		{
			return null;
		}

		// Token: 0x06002513 RID: 9491 RVA: 0x00080098 File Offset: 0x0007E298
		protected override void OnEnable()
		{
		}

		// Token: 0x06002514 RID: 9492 RVA: 0x0008009A File Offset: 0x0007E29A
		protected override void OnDisable()
		{
		}

		// Token: 0x06002515 RID: 9493 RVA: 0x0008009C File Offset: 0x0007E29C
		protected override bool OnCheck()
		{
			return base.agent.CharacterMainControl.GetGun() != null;
		}
	}
}
