using System;
using UnityEngine;

namespace Duckov.UI.Animations
{
	// Token: 0x020003D4 RID: 980
	public class AnchoredPositionLooper : LooperElement
	{
		// Token: 0x060023B3 RID: 9139 RVA: 0x0007C8D9 File Offset: 0x0007AAD9
		private void Awake()
		{
			this.rectTransform = base.transform as RectTransform;
		}

		// Token: 0x060023B4 RID: 9140 RVA: 0x0007C8EC File Offset: 0x0007AAEC
		protected override void OnTick(LooperClock clock, float t)
		{
			if (this.rectTransform == null)
			{
				return;
			}
			Vector2 vector = Vector2.Lerp(this.anchoredPositionA, this.anchoredPositionB, this.curve.Evaluate(t));
			this.rectTransform.anchoredPosition = vector;
		}

		// Token: 0x04001838 RID: 6200
		[SerializeField]
		private Vector2 anchoredPositionA;

		// Token: 0x04001839 RID: 6201
		[SerializeField]
		private Vector2 anchoredPositionB;

		// Token: 0x0400183A RID: 6202
		[SerializeField]
		private AnimationCurve curve;

		// Token: 0x0400183B RID: 6203
		private RectTransform rectTransform;
	}
}
