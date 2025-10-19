using System;
using NodeCanvas.Framework;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions
{
	// Token: 0x02000414 RID: 1044
	public class Shoot : ActionTask<AICharacterController>
	{
		// Token: 0x0600258F RID: 9615 RVA: 0x000815E1 File Offset: 0x0007F7E1
		protected override string OnInit()
		{
			return null;
		}

		// Token: 0x17000735 RID: 1845
		// (get) Token: 0x06002590 RID: 9616 RVA: 0x000815E4 File Offset: 0x0007F7E4
		protected override string info
		{
			get
			{
				return string.Format("Shoot {0}to{1} sec.", this.shootTimeRange.value.x, this.shootTimeRange.value.y);
			}
		}

		// Token: 0x06002591 RID: 9617 RVA: 0x0008161C File Offset: 0x0007F81C
		protected override void OnExecute()
		{
			this.semiTimer = this.semiTimeSpace;
			base.agent.CharacterMainControl.Trigger(true, true, false);
			if (!base.agent.shootCanMove)
			{
				base.agent.StopMove();
			}
			this.shootTime = global::UnityEngine.Random.Range(this.shootTimeRange.value.x, this.shootTimeRange.value.y);
			if (this.shootTime <= 0f)
			{
				base.EndAction(true);
			}
		}

		// Token: 0x06002592 RID: 9618 RVA: 0x000816A0 File Offset: 0x0007F8A0
		protected override void OnUpdate()
		{
			bool flag = false;
			this.semiTimer += Time.deltaTime;
			if (!base.agent.shootCanMove)
			{
				base.agent.StopMove();
			}
			if (this.semiTimer >= this.semiTimeSpace)
			{
				this.semiTimer = 0f;
				flag = true;
			}
			base.agent.CharacterMainControl.Trigger(true, flag, false);
			if (base.elapsedTime >= this.shootTime)
			{
				base.EndAction(true);
			}
		}

		// Token: 0x06002593 RID: 9619 RVA: 0x0008171C File Offset: 0x0007F91C
		protected override void OnStop()
		{
			base.agent.CharacterMainControl.Trigger(false, false, false);
		}

		// Token: 0x06002594 RID: 9620 RVA: 0x00081731 File Offset: 0x0007F931
		protected override void OnPause()
		{
			base.agent.CharacterMainControl.Trigger(false, false, false);
		}

		// Token: 0x04001992 RID: 6546
		public BBParameter<Vector2> shootTimeRange;

		// Token: 0x04001993 RID: 6547
		private float shootTime;

		// Token: 0x04001994 RID: 6548
		public float semiTimeSpace = 0.35f;

		// Token: 0x04001995 RID: 6549
		private float semiTimer;
	}
}
