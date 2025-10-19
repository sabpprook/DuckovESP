using System;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions
{
	// Token: 0x02000407 RID: 1031
	public class Dash : ActionTask<AICharacterController>
	{
		// Token: 0x0600253D RID: 9533 RVA: 0x000804FD File Offset: 0x0007E6FD
		protected override string OnInit()
		{
			this.dashTimeSpace = global::UnityEngine.Random.Range(this.dashTimeSpaceRange.value.x, this.dashTimeSpaceRange.value.y);
			return null;
		}

		// Token: 0x1700072B RID: 1835
		// (get) Token: 0x0600253E RID: 9534 RVA: 0x0008052B File Offset: 0x0007E72B
		protected override string info
		{
			get
			{
				return string.Format("Dash", Array.Empty<object>());
			}
		}

		// Token: 0x0600253F RID: 9535 RVA: 0x0008053C File Offset: 0x0007E73C
		protected override void OnExecute()
		{
			if (Time.time - this.lastDashTime < this.dashTimeSpace)
			{
				base.EndAction();
				return;
			}
			this.lastDashTime = Time.time;
			this.dashTimeSpace = global::UnityEngine.Random.Range(this.dashTimeSpaceRange.value.x, this.dashTimeSpaceRange.value.y);
			Vector3 vector = Vector3.forward;
			Dash.DashDirectionModes dashDirectionModes = this.directionMode;
			if (dashDirectionModes != Dash.DashDirectionModes.random)
			{
				if (dashDirectionModes == Dash.DashDirectionModes.targetTransform)
				{
					if (this.targetTransform.value == null)
					{
						base.EndAction();
						return;
					}
					vector = this.targetTransform.value.position - base.agent.transform.position;
					vector.y = 0f;
					vector.Normalize();
					if (this.verticle)
					{
						vector = Vector3.Cross(vector, Vector3.up) * ((global::UnityEngine.Random.Range(0f, 1f) > 0.5f) ? 1f : (-1f));
					}
				}
			}
			else
			{
				vector = global::UnityEngine.Random.insideUnitCircle;
				vector.z = vector.y;
				vector.y = 0f;
				vector.Normalize();
			}
			base.agent.CharacterMainControl.SetMoveInput(vector);
			base.agent.CharacterMainControl.Dash();
			base.EndAction(true);
		}

		// Token: 0x06002540 RID: 9536 RVA: 0x00080699 File Offset: 0x0007E899
		protected override void OnStop()
		{
		}

		// Token: 0x06002541 RID: 9537 RVA: 0x0008069B File Offset: 0x0007E89B
		protected override void OnPause()
		{
		}

		// Token: 0x0400195A RID: 6490
		public Dash.DashDirectionModes directionMode;

		// Token: 0x0400195B RID: 6491
		[ShowIf("directionMode", 1)]
		public BBParameter<Transform> targetTransform;

		// Token: 0x0400195C RID: 6492
		[ShowIf("directionMode", 1)]
		public bool verticle;

		// Token: 0x0400195D RID: 6493
		public BBParameter<Vector2> dashTimeSpaceRange;

		// Token: 0x0400195E RID: 6494
		private float dashTimeSpace;

		// Token: 0x0400195F RID: 6495
		private float lastDashTime = -999f;

		// Token: 0x0200066B RID: 1643
		public enum DashDirectionModes
		{
			// Token: 0x04002315 RID: 8981
			random,
			// Token: 0x04002316 RID: 8982
			targetTransform
		}
	}
}
