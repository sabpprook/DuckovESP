using System;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions
{
	// Token: 0x02000417 RID: 1047
	public class TraceTarget : ActionTask<AICharacterController>
	{
		// Token: 0x060025A0 RID: 9632 RVA: 0x00081852 File Offset: 0x0007FA52
		protected override string OnInit()
		{
			return null;
		}

		// Token: 0x060025A1 RID: 9633 RVA: 0x00081858 File Offset: 0x0007FA58
		protected override void OnExecute()
		{
			if (base.agent == null || (this.traceTargetTransform && this.centerTransform.value == null))
			{
				base.EndAction(false);
				return;
			}
			Vector3 vector = (this.traceTargetTransform ? this.centerTransform.value.position : this.centerPosition.value);
			base.agent.MoveToPos(vector);
		}

		// Token: 0x060025A2 RID: 9634 RVA: 0x000818C8 File Offset: 0x0007FAC8
		protected override void OnUpdate()
		{
			if (base.agent == null)
			{
				base.EndAction(false);
				return;
			}
			Vector3 vector = ((this.traceTargetTransform && this.centerTransform.value != null) ? this.centerTransform.value.position : this.centerPosition.value);
			if (base.elapsedTime > this.overTime.value)
			{
				base.EndAction(this.overTimeReturnSuccess);
				return;
			}
			if (Vector3.Distance(vector, base.agent.transform.position) < this.stopDistance.value)
			{
				base.EndAction(true);
				return;
			}
			this.recalculatePathTimer -= Time.deltaTime;
			if (this.recalculatePathTimer <= 0f)
			{
				this.recalculatePathTimer = this.recalculatePathTimeSpace;
				base.agent.MoveToPos(vector);
			}
			else if (!base.agent.WaitingForPathResult())
			{
				if (!base.agent.IsMoving() || base.agent.ReachedEndOfPath())
				{
					base.EndAction(true);
					return;
				}
				if (!base.agent.HasPath())
				{
					if (!this.failIfNoPath && this.retryIfNotFound)
					{
						base.agent.MoveToPos(vector);
						return;
					}
					base.EndAction(!this.failIfNoPath);
					return;
				}
			}
			if (this.syncDirectionIfNoAimTarget && base.agent.aimTarget == null)
			{
				Vector3 currentMoveDirection = base.agent.CharacterMainControl.CurrentMoveDirection;
				if (currentMoveDirection.magnitude > 0f)
				{
					base.agent.CharacterMainControl.SetAimPoint(base.agent.CharacterMainControl.transform.position + currentMoveDirection * 1000f);
				}
			}
		}

		// Token: 0x060025A3 RID: 9635 RVA: 0x00081A80 File Offset: 0x0007FC80
		protected override void OnStop()
		{
			base.agent.StopMove();
		}

		// Token: 0x060025A4 RID: 9636 RVA: 0x00081A8D File Offset: 0x0007FC8D
		protected override void OnPause()
		{
		}

		// Token: 0x04001997 RID: 6551
		public bool traceTargetTransform = true;

		// Token: 0x04001998 RID: 6552
		[ShowIf("traceTargetTransform", 0)]
		public BBParameter<Vector3> centerPosition;

		// Token: 0x04001999 RID: 6553
		[ShowIf("traceTargetTransform", 1)]
		public BBParameter<Transform> centerTransform;

		// Token: 0x0400199A RID: 6554
		public BBParameter<float> stopDistance;

		// Token: 0x0400199B RID: 6555
		public BBParameter<float> overTime = 8f;

		// Token: 0x0400199C RID: 6556
		public bool overTimeReturnSuccess = true;

		// Token: 0x0400199D RID: 6557
		private Vector3 targetPoint;

		// Token: 0x0400199E RID: 6558
		public bool failIfNoPath;

		// Token: 0x0400199F RID: 6559
		[ShowIf("failIfNoPath", 0)]
		public bool retryIfNotFound;

		// Token: 0x040019A0 RID: 6560
		private float recalculatePathTimeSpace = 0.15f;

		// Token: 0x040019A1 RID: 6561
		private float recalculatePathTimer = 0.15f;

		// Token: 0x040019A2 RID: 6562
		public bool syncDirectionIfNoAimTarget = true;
	}
}
