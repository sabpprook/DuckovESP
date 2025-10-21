using System;
using NodeCanvas.Framework;
using UnityEngine;

namespace NodeCanvas.Tasks.Conditions
{
	// Token: 0x020003FD RID: 1021
	public class CheckHeathLessThan : ConditionTask<AICharacterController>
	{
		// Token: 0x0600250D RID: 9485 RVA: 0x0007FFED File Offset: 0x0007E1ED
		protected override string OnInit()
		{
			return null;
		}

		// Token: 0x0600250E RID: 9486 RVA: 0x0007FFF0 File Offset: 0x0007E1F0
		protected override void OnEnable()
		{
		}

		// Token: 0x0600250F RID: 9487 RVA: 0x0007FFF2 File Offset: 0x0007E1F2
		protected override void OnDisable()
		{
		}

		// Token: 0x06002510 RID: 9488 RVA: 0x0007FFF4 File Offset: 0x0007E1F4
		protected override bool OnCheck()
		{
			if (Time.time - this.checkTimeMarker < this.checkTimeSpace)
			{
				return false;
			}
			this.checkTimeMarker = Time.time;
			if (!base.agent || !base.agent.CharacterMainControl)
			{
				return false;
			}
			Health health = base.agent.CharacterMainControl.Health;
			return health && health.CurrentHealth / health.MaxHealth <= this.percent;
		}

		// Token: 0x04001942 RID: 6466
		public float percent;

		// Token: 0x04001943 RID: 6467
		private float checkTimeMarker = -1f;

		// Token: 0x04001944 RID: 6468
		public float checkTimeSpace = 1.5f;
	}
}
