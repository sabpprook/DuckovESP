using System;
using UnityEngine;
using UnityEngine.UI;

namespace Duckov.UI.Animations
{
	// Token: 0x020003D5 RID: 981
	public class ImageColorLooper : LooperElement
	{
		// Token: 0x060023B6 RID: 9142 RVA: 0x0007C93C File Offset: 0x0007AB3C
		protected override void OnTick(LooperClock clock, float t)
		{
			Color color = this.colorOverT.Evaluate(t);
			float num = this.alphaOverT.Evaluate(t);
			color.a *= num;
			this.image.color = color;
		}

		// Token: 0x0400183C RID: 6204
		[SerializeField]
		private Image image;

		// Token: 0x0400183D RID: 6205
		[GradientUsage(true)]
		[SerializeField]
		private Gradient colorOverT;

		// Token: 0x0400183E RID: 6206
		[SerializeField]
		private AnimationCurve alphaOverT;
	}
}
