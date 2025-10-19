using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace Duckov.UI.Animations
{
	// Token: 0x020003D0 RID: 976
	public class RectTransformFade : FadeElement
	{
		// Token: 0x170006C1 RID: 1729
		// (get) Token: 0x0600236C RID: 9068 RVA: 0x0007BCA8 File Offset: 0x00079EA8
		private Vector2 TargetAnchoredPosition
		{
			get
			{
				return this.cachedAnchordPosition + this.offset;
			}
		}

		// Token: 0x170006C2 RID: 1730
		// (get) Token: 0x0600236D RID: 9069 RVA: 0x0007BCBB File Offset: 0x00079EBB
		private Vector3 TargetScale
		{
			get
			{
				return this.cachedScale + Vector3.one * this.uniformScale;
			}
		}

		// Token: 0x170006C3 RID: 1731
		// (get) Token: 0x0600236E RID: 9070 RVA: 0x0007BCD8 File Offset: 0x00079ED8
		private Vector3 TargetRotation
		{
			get
			{
				return this.cachedRotation + Vector3.forward * this.rotateZ;
			}
		}

		// Token: 0x0600236F RID: 9071 RVA: 0x0007BCF5 File Offset: 0x00079EF5
		private void Initialize()
		{
			if (this.initialized)
			{
				Debug.LogError("Object Initialized Twice, aborting");
				return;
			}
			this.CachePose();
			this.initialized = true;
		}

		// Token: 0x06002370 RID: 9072 RVA: 0x0007BD18 File Offset: 0x00079F18
		private void CachePose()
		{
			if (this.rectTransform == null)
			{
				return;
			}
			this.cachedAnchordPosition = this.rectTransform.anchoredPosition;
			this.cachedScale = this.rectTransform.localScale;
			this.cachedRotation = this.rectTransform.localRotation.eulerAngles;
		}

		// Token: 0x06002371 RID: 9073 RVA: 0x0007BD70 File Offset: 0x00079F70
		private void Awake()
		{
			if (this.rectTransform == null || this.rectTransform.gameObject != base.gameObject)
			{
				this.rectTransform = base.GetComponent<RectTransform>();
			}
			if (!this.initialized)
			{
				this.Initialize();
			}
		}

		// Token: 0x06002372 RID: 9074 RVA: 0x0007BDBD File Offset: 0x00079FBD
		private void OnValidate()
		{
			if (this.rectTransform == null || this.rectTransform.gameObject != base.gameObject)
			{
				this.rectTransform = base.GetComponent<RectTransform>();
			}
		}

		// Token: 0x06002373 RID: 9075 RVA: 0x0007BDF4 File Offset: 0x00079FF4
		protected override async UniTask HideTask(int token)
		{
			if (!this.initialized)
			{
				this.Initialize();
			}
			UniTask uniTask = this.rectTransform.DOAnchorPos(this.TargetAnchoredPosition, this.duration, false).SetEase(this.hidingAnimationCurve).OnComplete(delegate
			{
				this.rectTransform.anchoredPosition = this.TargetAnchoredPosition;
			})
				.ToUniTask(TweenCancelBehaviour.Kill, default(CancellationToken));
			UniTask uniTask2 = this.rectTransform.DOScale(this.TargetScale, this.duration).SetEase(this.showingAnimationCurve).OnComplete(delegate
			{
				this.rectTransform.localScale = this.TargetScale;
			})
				.ToUniTask(TweenCancelBehaviour.Kill, default(CancellationToken));
			UniTask uniTask3 = this.rectTransform.DOLocalRotate(this.TargetRotation, this.duration, RotateMode.Fast).SetEase(this.showingAnimationCurve).OnComplete(delegate
			{
				this.rectTransform.localRotation = Quaternion.Euler(this.TargetRotation);
			})
				.ToUniTask(TweenCancelBehaviour.Kill, default(CancellationToken));
			await UniTask.WhenAll(new UniTask[] { uniTask, uniTask2, uniTask3 });
		}

		// Token: 0x06002374 RID: 9076 RVA: 0x0007BE38 File Offset: 0x0007A038
		protected override async UniTask ShowTask(int token)
		{
			if (!this.initialized)
			{
				this.Initialize();
			}
			UniTask uniTask = this.rectTransform.DOAnchorPos(this.cachedAnchordPosition, this.duration, false).SetEase(this.showingAnimationCurve).OnComplete(delegate
			{
				this.rectTransform.anchoredPosition = this.cachedAnchordPosition;
				if (this.debug)
				{
					Debug.Log(string.Format("Move Complete {0}", base.gameObject.activeInHierarchy));
				}
			})
				.ToUniTask(TweenCancelBehaviour.Kill, default(CancellationToken));
			UniTask uniTask2 = this.rectTransform.DOScale(this.cachedScale, this.duration).SetEase(this.showingAnimationCurve).OnComplete(delegate
			{
				this.rectTransform.localScale = this.cachedScale;
			})
				.ToUniTask(TweenCancelBehaviour.Kill, default(CancellationToken));
			UniTask uniTask3 = this.rectTransform.DOLocalRotate(this.cachedRotation, this.duration, RotateMode.Fast).SetEase(this.showingAnimationCurve).OnComplete(delegate
			{
				this.rectTransform.localRotation = Quaternion.Euler(this.cachedRotation);
			})
				.ToUniTask(TweenCancelBehaviour.Kill, default(CancellationToken));
			await UniTask.WhenAll(new UniTask[] { uniTask, uniTask2, uniTask3 });
			if (this.debug)
			{
				Debug.Log("Ending Show Task");
			}
		}

		// Token: 0x06002375 RID: 9077 RVA: 0x0007BE7C File Offset: 0x0007A07C
		protected override void OnSkipHide()
		{
			if (this.debug)
			{
				Debug.Log("OnSkipHide");
			}
			if (!this.initialized)
			{
				this.Initialize();
			}
			this.rectTransform.anchoredPosition = this.TargetAnchoredPosition;
			this.rectTransform.localScale = this.TargetScale;
			this.rectTransform.localRotation = Quaternion.Euler(this.TargetRotation);
		}

		// Token: 0x06002376 RID: 9078 RVA: 0x0007BEE1 File Offset: 0x0007A0E1
		private void OnDestroy()
		{
			RectTransform rectTransform = this.rectTransform;
			if (rectTransform == null)
			{
				return;
			}
			rectTransform.DOKill(false);
		}

		// Token: 0x06002377 RID: 9079 RVA: 0x0007BEF8 File Offset: 0x0007A0F8
		protected override void OnSkipShow()
		{
			if (this.debug)
			{
				Debug.Log("OnSkipShow");
			}
			if (!this.initialized)
			{
				this.Initialize();
			}
			this.rectTransform.anchoredPosition = this.cachedAnchordPosition;
			this.rectTransform.localScale = this.cachedScale;
			this.rectTransform.localRotation = Quaternion.Euler(this.cachedRotation);
		}

		// Token: 0x04001810 RID: 6160
		[SerializeField]
		private bool debug;

		// Token: 0x04001811 RID: 6161
		[SerializeField]
		private RectTransform rectTransform;

		// Token: 0x04001812 RID: 6162
		[SerializeField]
		private float duration = 0.4f;

		// Token: 0x04001813 RID: 6163
		[SerializeField]
		private Vector2 offset = Vector2.left * 10f;

		// Token: 0x04001814 RID: 6164
		[SerializeField]
		[Range(-1f, 1f)]
		private float uniformScale;

		// Token: 0x04001815 RID: 6165
		[SerializeField]
		[Range(-180f, 180f)]
		private float rotateZ;

		// Token: 0x04001816 RID: 6166
		[SerializeField]
		private AnimationCurve showingAnimationCurve;

		// Token: 0x04001817 RID: 6167
		[SerializeField]
		private AnimationCurve hidingAnimationCurve;

		// Token: 0x04001818 RID: 6168
		private Vector2 cachedAnchordPosition = Vector2.zero;

		// Token: 0x04001819 RID: 6169
		private Vector3 cachedScale = Vector3.one;

		// Token: 0x0400181A RID: 6170
		private Vector3 cachedRotation = Vector3.zero;

		// Token: 0x0400181B RID: 6171
		private bool initialized;
	}
}
