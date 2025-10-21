using System;
using NodeCanvas.Framework;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions
{
	// Token: 0x0200040F RID: 1039
	public class RotateAim : ActionTask<AICharacterController>
	{
		// Token: 0x0600256E RID: 9582 RVA: 0x0008103E File Offset: 0x0007F23E
		protected override string OnInit()
		{
			return null;
		}

		// Token: 0x0600256F RID: 9583 RVA: 0x00081044 File Offset: 0x0007F244
		protected override void OnExecute()
		{
			this.time = global::UnityEngine.Random.Range(this.timeRange.value.x, this.timeRange.value.y);
			this.startDir = base.agent.CharacterMainControl.CurrentAimDirection;
			base.agent.SetTarget(null);
			if (this.shoot)
			{
				base.agent.CharacterMainControl.Trigger(true, true, false);
			}
		}

		// Token: 0x06002570 RID: 9584 RVA: 0x000810BC File Offset: 0x0007F2BC
		protected override void OnUpdate()
		{
			this.currentAngle = this.angle * base.elapsedTime / this.time;
			Vector3 vector = Quaternion.Euler(0f, this.currentAngle, 0f) * this.startDir;
			base.agent.CharacterMainControl.SetAimPoint(base.agent.CharacterMainControl.transform.position + vector * 100f);
			if (this.shoot)
			{
				base.agent.CharacterMainControl.Trigger(true, true, false);
			}
			if (base.elapsedTime > this.time)
			{
				base.EndAction(true);
			}
		}

		// Token: 0x06002571 RID: 9585 RVA: 0x00081169 File Offset: 0x0007F369
		protected override void OnStop()
		{
			base.agent.CharacterMainControl.Trigger(false, false, false);
		}

		// Token: 0x06002572 RID: 9586 RVA: 0x0008117E File Offset: 0x0007F37E
		protected override void OnPause()
		{
			base.agent.CharacterMainControl.Trigger(false, false, false);
		}

		// Token: 0x04001978 RID: 6520
		private Vector3 startDir;

		// Token: 0x04001979 RID: 6521
		public float angle;

		// Token: 0x0400197A RID: 6522
		private float currentAngle;

		// Token: 0x0400197B RID: 6523
		public BBParameter<Vector2> timeRange;

		// Token: 0x0400197C RID: 6524
		private float time;

		// Token: 0x0400197D RID: 6525
		public bool shoot;
	}
}
