using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace Duckov.UI.Animations
{
	// Token: 0x020003DE RID: 990
	public class ScaleToggle : ToggleAnimation
	{
		// Token: 0x060023D6 RID: 9174 RVA: 0x0007CEC6 File Offset: 0x0007B0C6
		private void CachePose()
		{
			this.cachedScale = this.rectTransform.localScale;
		}

		// Token: 0x060023D7 RID: 9175 RVA: 0x0007CED9 File Offset: 0x0007B0D9
		private void Awake()
		{
			this.rectTransform = base.transform as RectTransform;
			this.CachePose();
		}

		// Token: 0x060023D8 RID: 9176 RVA: 0x0007CEF4 File Offset: 0x0007B0F4
		protected override void OnSetToggle(bool status)
		{
			float num = (status ? this.activeScale : this.idleScale);
			num * this.cachedScale;
			this.rectTransform.DOKill(false);
			this.rectTransform.DOScale(this.cachedScale * num, this.duration).SetEase(this.animationCurve);
		}

		// Token: 0x04001857 RID: 6231
		public float idleScale = 1f;

		// Token: 0x04001858 RID: 6232
		public float activeScale = 0.9f;

		// Token: 0x04001859 RID: 6233
		public float duration = 0.1f;

		// Token: 0x0400185A RID: 6234
		public AnimationCurve animationCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x0400185B RID: 6235
		private Vector3 cachedScale = Vector3.one;

		// Token: 0x0400185C RID: 6236
		private RectTransform rectTransform;
	}
}
