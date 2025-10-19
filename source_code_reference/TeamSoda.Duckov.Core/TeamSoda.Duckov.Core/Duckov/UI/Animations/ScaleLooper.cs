using System;
using UnityEngine;

namespace Duckov.UI.Animations
{
	// Token: 0x020003DA RID: 986
	public class ScaleLooper : LooperElement
	{
		// Token: 0x060023C6 RID: 9158 RVA: 0x0007CB8C File Offset: 0x0007AD8C
		protected override void OnTick(LooperClock clock, float t)
		{
			float num = this.xOverT.Evaluate(t);
			float num2 = this.yOverT.Evaluate(t);
			float num3 = this.zOverT.Evaluate(t);
			float num4 = this.uniformScaleOverT.Evaluate(t);
			num *= num4;
			num2 *= num4;
			num3 *= num4;
			base.transform.localScale = new Vector3(num, num2, num3);
		}

		// Token: 0x04001849 RID: 6217
		[SerializeField]
		private AnimationCurve uniformScaleOverT = AnimationCurve.Linear(0f, 1f, 1f, 1f);

		// Token: 0x0400184A RID: 6218
		[SerializeField]
		private AnimationCurve xOverT = AnimationCurve.Linear(0f, 1f, 1f, 1f);

		// Token: 0x0400184B RID: 6219
		[SerializeField]
		private AnimationCurve yOverT = AnimationCurve.Linear(0f, 1f, 1f, 1f);

		// Token: 0x0400184C RID: 6220
		[SerializeField]
		private AnimationCurve zOverT = AnimationCurve.Linear(0f, 1f, 1f, 1f);
	}
}
