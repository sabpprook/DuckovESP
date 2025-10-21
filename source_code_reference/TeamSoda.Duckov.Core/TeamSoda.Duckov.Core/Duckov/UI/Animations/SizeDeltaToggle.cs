using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace Duckov.UI.Animations
{
	// Token: 0x020003DF RID: 991
	public class SizeDeltaToggle : ToggleAnimation
	{
		// Token: 0x170006CF RID: 1743
		// (get) Token: 0x060023DA RID: 9178 RVA: 0x0007CFB6 File Offset: 0x0007B1B6
		private RectTransform RectTransform
		{
			get
			{
				if (this._rectTransform == null)
				{
					this._rectTransform = base.GetComponent<RectTransform>();
				}
				return this._rectTransform;
			}
		}

		// Token: 0x060023DB RID: 9179 RVA: 0x0007CFD8 File Offset: 0x0007B1D8
		private void CachePose()
		{
			this.cachedSizeDelta = this.RectTransform.sizeDelta;
		}

		// Token: 0x060023DC RID: 9180 RVA: 0x0007CFEB File Offset: 0x0007B1EB
		private void Awake()
		{
			this.CachePose();
		}

		// Token: 0x060023DD RID: 9181 RVA: 0x0007CFF4 File Offset: 0x0007B1F4
		protected override void OnSetToggle(bool status)
		{
			if (!base.gameObject.activeInHierarchy)
			{
				return;
			}
			Vector2 vector = (status ? this.activeSizeDelta : this.idleSizeDelta);
			this.RectTransform.DOKill(false);
			this.RectTransform.DOSizeDelta(vector, this.duration, false).SetEase(this.animationCurve);
		}

		// Token: 0x0400185D RID: 6237
		public Vector2 idleSizeDelta = Vector2.zero;

		// Token: 0x0400185E RID: 6238
		public Vector2 activeSizeDelta = Vector2.one * 12f;

		// Token: 0x0400185F RID: 6239
		public float duration = 0.1f;

		// Token: 0x04001860 RID: 6240
		public AnimationCurve animationCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x04001861 RID: 6241
		private Vector2 cachedSizeDelta = Vector3.one;

		// Token: 0x04001862 RID: 6242
		private RectTransform _rectTransform;
	}
}
