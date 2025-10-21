using System;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.Tasks.Conditions
{
	// Token: 0x02000402 RID: 1026
	public class CheckTargetDistance : ConditionTask<AICharacterController>
	{
		// Token: 0x17000729 RID: 1833
		// (get) Token: 0x06002523 RID: 9507 RVA: 0x000801C6 File Offset: 0x0007E3C6
		protected override string info
		{
			get
			{
				return "is target in range";
			}
		}

		// Token: 0x06002524 RID: 9508 RVA: 0x000801D0 File Offset: 0x0007E3D0
		protected override bool OnCheck()
		{
			if (this.useTransform && this.targetTransform.value == null)
			{
				return false;
			}
			Vector3 vector = (this.useTransform ? this.targetTransform.value.position : this.targetPoint.value);
			float num;
			if (this.useShootRange)
			{
				num = base.agent.CharacterMainControl.GetAimRange() * this.shootRangeMultiplier.value;
			}
			else
			{
				num = this.distance.value;
			}
			return Vector3.Distance(base.agent.transform.position, vector) <= num;
		}

		// Token: 0x0400194A RID: 6474
		public bool useTransform;

		// Token: 0x0400194B RID: 6475
		[ShowIf("useTransform", 1)]
		public BBParameter<Transform> targetTransform;

		// Token: 0x0400194C RID: 6476
		[ShowIf("useTransform", 0)]
		public BBParameter<Vector3> targetPoint;

		// Token: 0x0400194D RID: 6477
		public bool useShootRange;

		// Token: 0x0400194E RID: 6478
		[ShowIf("useShootRange", 1)]
		public BBParameter<float> shootRangeMultiplier = 1f;

		// Token: 0x0400194F RID: 6479
		[ShowIf("useShootRange", 0)]
		public BBParameter<float> distance;
	}
}
