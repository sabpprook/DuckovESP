using System;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions
{
	// Token: 0x02000409 RID: 1033
	public class MoveToRandomPos : ActionTask<AICharacterController>
	{
		// Token: 0x06002549 RID: 9545 RVA: 0x0008073D File Offset: 0x0007E93D
		protected override string OnInit()
		{
			return null;
		}

		// Token: 0x0600254A RID: 9546 RVA: 0x00080740 File Offset: 0x0007E940
		protected override void OnExecute()
		{
			if (base.agent == null)
			{
				base.EndAction(false);
				return;
			}
			this.targetPoint = this.RandomPoint();
			base.agent.MoveToPos(this.targetPoint);
		}

		// Token: 0x0600254B RID: 9547 RVA: 0x00080778 File Offset: 0x0007E978
		protected override void OnUpdate()
		{
			if (base.agent == null)
			{
				base.EndAction(false);
				return;
			}
			if (base.elapsedTime > this.overTime.value)
			{
				base.EndAction(this.overTimeReturnSuccess);
				return;
			}
			if (this.useTransform && this.centerTransform.value == null)
			{
				base.EndAction(false);
				return;
			}
			if (this.syncDirectionIfNoAimTarget && base.agent.aimTarget == null)
			{
				if (this.setAimToPos && this.aimPos.isDefined)
				{
					base.agent.CharacterMainControl.SetAimPoint(this.aimPos.value);
				}
				else
				{
					Vector3 currentMoveDirection = base.agent.CharacterMainControl.CurrentMoveDirection;
					if (currentMoveDirection.magnitude > 0f)
					{
						base.agent.CharacterMainControl.SetAimPoint(base.agent.CharacterMainControl.transform.position + currentMoveDirection * 1000f);
					}
				}
			}
			if (!base.agent.WaitingForPathResult())
			{
				if (base.agent.ReachedEndOfPath() || !base.agent.IsMoving())
				{
					base.EndAction(true);
					return;
				}
				if (!base.agent.HasPath())
				{
					if (!this.failIfNoPath && this.retryIfNotFound)
					{
						this.targetPoint = this.RandomPoint();
						base.agent.MoveToPos(this.targetPoint);
						return;
					}
					base.EndAction(!this.failIfNoPath);
					return;
				}
			}
		}

		// Token: 0x0600254C RID: 9548 RVA: 0x000808FF File Offset: 0x0007EAFF
		protected override void OnStop()
		{
			base.agent.StopMove();
		}

		// Token: 0x0600254D RID: 9549 RVA: 0x0008090C File Offset: 0x0007EB0C
		protected override void OnPause()
		{
		}

		// Token: 0x0600254E RID: 9550 RVA: 0x00080910 File Offset: 0x0007EB10
		private Vector3 RandomPoint()
		{
			Vector3 vector = base.agent.CharacterMainControl.transform.position;
			if (this.useTransform)
			{
				if (this.centerTransform.isDefined)
				{
					vector = this.centerTransform.value.position;
				}
			}
			else
			{
				vector = this.centerPos.value;
			}
			Vector3 vector2 = vector - base.agent.transform.position;
			vector2.y = 0f;
			if (vector2.magnitude < 0.1f)
			{
				vector2 = global::UnityEngine.Random.insideUnitSphere;
				vector2.y = 0f;
			}
			vector2 = vector2.normalized;
			float num = global::UnityEngine.Random.Range(-0.5f * this.randomAngle, 0.5f * this.randomAngle);
			float num2 = global::UnityEngine.Random.Range(this.avoidRadius.value, this.radius.value);
			vector2 = Quaternion.Euler(0f, num, 0f) * -vector2;
			return vector + vector2 * num2;
		}

		// Token: 0x04001961 RID: 6497
		public bool useTransform;

		// Token: 0x04001962 RID: 6498
		public bool setAimToPos;

		// Token: 0x04001963 RID: 6499
		[ShowIf("setAimToPos", 1)]
		public BBParameter<Vector3> aimPos;

		// Token: 0x04001964 RID: 6500
		[ShowIf("useTransform", 0)]
		public BBParameter<Vector3> centerPos;

		// Token: 0x04001965 RID: 6501
		[ShowIf("useTransform", 1)]
		public BBParameter<Transform> centerTransform;

		// Token: 0x04001966 RID: 6502
		public BBParameter<float> radius;

		// Token: 0x04001967 RID: 6503
		public BBParameter<float> avoidRadius;

		// Token: 0x04001968 RID: 6504
		public float randomAngle = 360f;

		// Token: 0x04001969 RID: 6505
		public BBParameter<float> overTime = 8f;

		// Token: 0x0400196A RID: 6506
		public bool overTimeReturnSuccess = true;

		// Token: 0x0400196B RID: 6507
		private Vector3 targetPoint;

		// Token: 0x0400196C RID: 6508
		public bool failIfNoPath;

		// Token: 0x0400196D RID: 6509
		[ShowIf("failIfNoPath", 0)]
		public bool retryIfNotFound;

		// Token: 0x0400196E RID: 6510
		public bool syncDirectionIfNoAimTarget = true;
	}
}
