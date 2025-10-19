using System;
using NodeCanvas.Framework;

namespace NodeCanvas.Tasks.Conditions
{
	// Token: 0x020003FF RID: 1023
	public class CheckHurt : ConditionTask<AICharacterController>
	{
		// Token: 0x06002517 RID: 9495 RVA: 0x000800BC File Offset: 0x0007E2BC
		protected override string OnInit()
		{
			return null;
		}

		// Token: 0x06002518 RID: 9496 RVA: 0x000800BF File Offset: 0x0007E2BF
		protected override void OnEnable()
		{
		}

		// Token: 0x06002519 RID: 9497 RVA: 0x000800C1 File Offset: 0x0007E2C1
		protected override void OnDisable()
		{
		}

		// Token: 0x0600251A RID: 9498 RVA: 0x000800C4 File Offset: 0x0007E2C4
		protected override bool OnCheck()
		{
			if (base.agent == null || this.cacheFromCharacterDmgReceiver == null)
			{
				return false;
			}
			bool flag = false;
			DamageInfo damageInfo = default(DamageInfo);
			if (base.agent.IsHurt(this.hurtTimeThreshold, this.damageThreshold, ref damageInfo))
			{
				this.cacheFromCharacterDmgReceiver.value = damageInfo.fromCharacter.mainDamageReceiver;
				flag = true;
			}
			return flag;
		}

		// Token: 0x04001945 RID: 6469
		public float hurtTimeThreshold = 0.2f;

		// Token: 0x04001946 RID: 6470
		public int damageThreshold = 3;

		// Token: 0x04001947 RID: 6471
		public BBParameter<DamageReceiver> cacheFromCharacterDmgReceiver;
	}
}
