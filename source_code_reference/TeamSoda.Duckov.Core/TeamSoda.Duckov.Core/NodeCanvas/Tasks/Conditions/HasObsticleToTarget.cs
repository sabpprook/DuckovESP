using System;
using NodeCanvas.Framework;

namespace NodeCanvas.Tasks.Conditions
{
	// Token: 0x02000403 RID: 1027
	public class HasObsticleToTarget : ConditionTask<AICharacterController>
	{
		// Token: 0x06002526 RID: 9510 RVA: 0x0008028D File Offset: 0x0007E48D
		protected override string OnInit()
		{
			return null;
		}

		// Token: 0x06002527 RID: 9511 RVA: 0x00080290 File Offset: 0x0007E490
		protected override void OnEnable()
		{
		}

		// Token: 0x06002528 RID: 9512 RVA: 0x00080292 File Offset: 0x0007E492
		protected override void OnDisable()
		{
		}

		// Token: 0x06002529 RID: 9513 RVA: 0x00080294 File Offset: 0x0007E494
		protected override bool OnCheck()
		{
			return base.agent.hasObsticleToTarget;
		}

		// Token: 0x04001950 RID: 6480
		public float hurtTimeThreshold = 0.2f;

		// Token: 0x04001951 RID: 6481
		public int damageThreshold = 3;

		// Token: 0x04001952 RID: 6482
		public BBParameter<DamageReceiver> cacheFromCharacterDmgReceiver;
	}
}
