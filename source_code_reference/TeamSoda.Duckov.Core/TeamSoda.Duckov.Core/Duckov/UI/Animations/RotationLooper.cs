using System;
using UnityEngine;

namespace Duckov.UI.Animations
{
	// Token: 0x020003D9 RID: 985
	public class RotationLooper : LooperElement
	{
		// Token: 0x060023C4 RID: 9156 RVA: 0x0007CB38 File Offset: 0x0007AD38
		protected override void OnTick(LooperClock clock, float t)
		{
			if (base.transform == null)
			{
				return;
			}
			Vector3 vector = Vector3.Lerp(this.eulerRotationA, this.eulerRotationB, this.curve.Evaluate(t));
			base.transform.localRotation = Quaternion.Euler(vector);
		}

		// Token: 0x04001846 RID: 6214
		[SerializeField]
		private Vector3 eulerRotationA;

		// Token: 0x04001847 RID: 6215
		[SerializeField]
		private Vector3 eulerRotationB;

		// Token: 0x04001848 RID: 6216
		[SerializeField]
		private AnimationCurve curve;
	}
}
