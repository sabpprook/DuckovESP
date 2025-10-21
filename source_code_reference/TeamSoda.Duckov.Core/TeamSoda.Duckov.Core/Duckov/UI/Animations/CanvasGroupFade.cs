using System;
using System.Runtime.CompilerServices;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.CompilerServices;
using UnityEngine;

namespace Duckov.UI.Animations
{
	// Token: 0x020003CE RID: 974
	[RequireComponent(typeof(CanvasGroup))]
	public class CanvasGroupFade : FadeElement
	{
		// Token: 0x170006BB RID: 1723
		// (get) Token: 0x06002351 RID: 9041 RVA: 0x0007B7E0 File Offset: 0x000799E0
		private float ShowingDuration
		{
			get
			{
				return this.fadeDuration;
			}
		}

		// Token: 0x170006BC RID: 1724
		// (get) Token: 0x06002352 RID: 9042 RVA: 0x0007B7E8 File Offset: 0x000799E8
		private float HidingDuration
		{
			get
			{
				return this.fadeDuration;
			}
		}

		// Token: 0x06002353 RID: 9043 RVA: 0x0007B7F0 File Offset: 0x000799F0
		private void Awake()
		{
			if (this.canvasGroup == null || this.canvasGroup.gameObject != base.gameObject)
			{
				this.canvasGroup = base.GetComponent<CanvasGroup>();
			}
			this.awaked = true;
		}

		// Token: 0x06002354 RID: 9044 RVA: 0x0007B82B File Offset: 0x00079A2B
		private void OnValidate()
		{
			if (this.canvasGroup == null || this.canvasGroup.gameObject != base.gameObject)
			{
				this.canvasGroup = base.GetComponent<CanvasGroup>();
			}
		}

		// Token: 0x06002355 RID: 9045 RVA: 0x0007B860 File Offset: 0x00079A60
		protected override UniTask ShowTask(int taskToken)
		{
			if (this.canvasGroup == null)
			{
				return default(UniTask);
			}
			if (!this.awaked)
			{
				this.canvasGroup.alpha = 0f;
			}
			if (this.manageBlockRaycast)
			{
				this.canvasGroup.blocksRaycasts = true;
			}
			return this.FadeTask(taskToken, base.IsFading ? this.canvasGroup.alpha : 0f, 1f, this.showingCurve, this.ShowingDuration);
		}

		// Token: 0x06002356 RID: 9046 RVA: 0x0007B8E4 File Offset: 0x00079AE4
		protected override UniTask HideTask(int taskToken)
		{
			if (this.canvasGroup == null)
			{
				return default(UniTask);
			}
			if (this.manageBlockRaycast)
			{
				this.canvasGroup.blocksRaycasts = false;
			}
			return this.FadeTask(taskToken, base.IsFading ? this.canvasGroup.alpha : 1f, 0f, this.hidingCurve, this.HidingDuration);
		}

		// Token: 0x06002357 RID: 9047 RVA: 0x0007B950 File Offset: 0x00079B50
		private UniTask FadeTask(int token, float beginAlpha, float targetAlpha, AnimationCurve animationCurve, float duration)
		{
			CanvasGroupFade.<FadeTask>d__14 <FadeTask>d__;
			<FadeTask>d__.<>t__builder = AsyncUniTaskMethodBuilder.Create();
			<FadeTask>d__.<>4__this = this;
			<FadeTask>d__.token = token;
			<FadeTask>d__.beginAlpha = beginAlpha;
			<FadeTask>d__.targetAlpha = targetAlpha;
			<FadeTask>d__.animationCurve = animationCurve;
			<FadeTask>d__.duration = duration;
			<FadeTask>d__.<>1__state = -1;
			<FadeTask>d__.<>t__builder.Start<CanvasGroupFade.<FadeTask>d__14>(ref <FadeTask>d__);
			return <FadeTask>d__.<>t__builder.Task;
		}

		// Token: 0x06002358 RID: 9048 RVA: 0x0007B9BD File Offset: 0x00079BBD
		protected override void OnSkipHide()
		{
			if (this.canvasGroup != null)
			{
				this.canvasGroup.alpha = 0f;
			}
			if (this.manageBlockRaycast)
			{
				this.canvasGroup.blocksRaycasts = false;
			}
		}

		// Token: 0x06002359 RID: 9049 RVA: 0x0007B9F1 File Offset: 0x00079BF1
		protected override void OnSkipShow()
		{
			if (this.canvasGroup != null)
			{
				this.canvasGroup.alpha = 1f;
			}
			if (this.manageBlockRaycast)
			{
				this.canvasGroup.blocksRaycasts = true;
			}
		}

		// Token: 0x0600235B RID: 9051 RVA: 0x0007BA38 File Offset: 0x00079C38
		[CompilerGenerated]
		private bool <FadeTask>g__CheckTaskValid|14_0(ref CanvasGroupFade.<>c__DisplayClass14_0 A_1)
		{
			return this.canvasGroup != null && A_1.token == base.ActiveTaskToken;
		}

		// Token: 0x04001803 RID: 6147
		[SerializeField]
		private CanvasGroup canvasGroup;

		// Token: 0x04001804 RID: 6148
		[SerializeField]
		private AnimationCurve showingCurve;

		// Token: 0x04001805 RID: 6149
		[SerializeField]
		private AnimationCurve hidingCurve;

		// Token: 0x04001806 RID: 6150
		[SerializeField]
		private float fadeDuration = 0.2f;

		// Token: 0x04001807 RID: 6151
		[SerializeField]
		private bool manageBlockRaycast;

		// Token: 0x04001808 RID: 6152
		private bool awaked;
	}
}
