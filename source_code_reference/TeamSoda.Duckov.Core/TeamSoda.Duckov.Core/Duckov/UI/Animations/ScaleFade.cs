using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace Duckov.UI.Animations
{
	// Token: 0x020003D1 RID: 977
	public class ScaleFade : FadeElement
	{
		// Token: 0x170006C4 RID: 1732
		// (get) Token: 0x0600237F RID: 9087 RVA: 0x0007C057 File Offset: 0x0007A257
		private Vector3 HiddenScale
		{
			get
			{
				return Vector3.one + Vector3.one * this.uniformScale + this.scale;
			}
		}

		// Token: 0x06002380 RID: 9088 RVA: 0x0007C07E File Offset: 0x0007A27E
		private void CachePose()
		{
			this.cachedScale = base.transform.localScale;
		}

		// Token: 0x06002381 RID: 9089 RVA: 0x0007C091 File Offset: 0x0007A291
		private void RestorePose()
		{
			base.transform.localScale = this.cachedScale;
		}

		// Token: 0x06002382 RID: 9090 RVA: 0x0007C0A4 File Offset: 0x0007A2A4
		private void Initialize()
		{
			if (this.initialized)
			{
				return;
			}
			this.initialized = true;
			this.CachePose();
		}

		// Token: 0x06002383 RID: 9091 RVA: 0x0007C0BC File Offset: 0x0007A2BC
		protected override UniTask HideTask(int token)
		{
			if (!this.initialized)
			{
				this.Initialize();
			}
			if (!base.transform)
			{
				return UniTask.CompletedTask;
			}
			return base.transform.DOScale(this.HiddenScale, this.duration).SetEase(this.hideCurve).ToUniTask(TweenCancelBehaviour.Kill, default(CancellationToken));
		}

		// Token: 0x06002384 RID: 9092 RVA: 0x0007C11B File Offset: 0x0007A31B
		protected override void OnSkipHide()
		{
			if (!this.initialized)
			{
				this.Initialize();
			}
			base.transform.localScale = this.HiddenScale;
		}

		// Token: 0x06002385 RID: 9093 RVA: 0x0007C13C File Offset: 0x0007A33C
		protected override void OnSkipShow()
		{
			if (!this.initialized)
			{
				this.Initialize();
			}
			this.RestorePose();
		}

		// Token: 0x06002386 RID: 9094 RVA: 0x0007C154 File Offset: 0x0007A354
		protected override UniTask ShowTask(int token)
		{
			if (!this.initialized)
			{
				this.Initialize();
			}
			return base.transform.DOScale(this.cachedScale, this.duration).SetEase(this.showCurve).OnComplete(new TweenCallback(this.RestorePose))
				.ToUniTask(TweenCancelBehaviour.Kill, default(CancellationToken));
		}

		// Token: 0x0400181C RID: 6172
		[SerializeField]
		private float duration = 0.1f;

		// Token: 0x0400181D RID: 6173
		[SerializeField]
		private Vector3 scale = Vector3.zero;

		// Token: 0x0400181E RID: 6174
		[SerializeField]
		[Range(-1f, 1f)]
		private float uniformScale;

		// Token: 0x0400181F RID: 6175
		[SerializeField]
		private AnimationCurve showCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x04001820 RID: 6176
		[SerializeField]
		private AnimationCurve hideCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x04001821 RID: 6177
		private Vector3 cachedScale = Vector3.one;

		// Token: 0x04001822 RID: 6178
		private bool initialized;
	}
}
