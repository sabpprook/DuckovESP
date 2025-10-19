using System;
using UnityEngine;

namespace Duckov.UI.Animations
{
	// Token: 0x020003D6 RID: 982
	public class LocalPositionLooper : LooperElement
	{
		// Token: 0x060023B8 RID: 9144 RVA: 0x0007C984 File Offset: 0x0007AB84
		protected override void OnTick(LooperClock clock, float t)
		{
			if (base.transform == null)
			{
				return;
			}
			Vector2 vector = Vector2.Lerp(this.localPositionA, this.localPositionB, this.curve.Evaluate(t));
			base.transform.localPosition = vector;
		}

		// Token: 0x0400183F RID: 6207
		[SerializeField]
		private Vector3 localPositionA;

		// Token: 0x04001840 RID: 6208
		[SerializeField]
		private Vector3 localPositionB;

		// Token: 0x04001841 RID: 6209
		[SerializeField]
		private AnimationCurve curve;
	}
}
