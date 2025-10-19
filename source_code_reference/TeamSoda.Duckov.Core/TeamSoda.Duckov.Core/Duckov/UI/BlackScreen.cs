using System;
using Cysharp.Threading.Tasks;
using Duckov.UI.Animations;
using UnityEngine;
using UnityEngine.UI;

namespace Duckov.UI
{
	// Token: 0x02000377 RID: 887
	public class BlackScreen : MonoBehaviour
	{
		// Token: 0x170005EC RID: 1516
		// (get) Token: 0x06001EA6 RID: 7846 RVA: 0x0006B9F1 File Offset: 0x00069BF1
		public static BlackScreen Instance
		{
			get
			{
				return GameManager.BlackScreen;
			}
		}

		// Token: 0x06001EA7 RID: 7847 RVA: 0x0006B9F8 File Offset: 0x00069BF8
		private void Awake()
		{
			if (BlackScreen.Instance != this)
			{
				Debug.LogError("检测到应当删除的BlackScreen实例", base.gameObject);
			}
		}

		// Token: 0x06001EA8 RID: 7848 RVA: 0x0006BA17 File Offset: 0x00069C17
		private void SetFadeCurve(AnimationCurve curve)
		{
			this.fadeElement.ShowCurve = curve;
			this.fadeElement.HideCurve = curve;
		}

		// Token: 0x06001EA9 RID: 7849 RVA: 0x0006BA31 File Offset: 0x00069C31
		private void SetCircleFade(float circleFade)
		{
			this.fadeImage.material.SetFloat("_CircleFade", circleFade);
		}

		// Token: 0x06001EAA RID: 7850 RVA: 0x0006BA4C File Offset: 0x00069C4C
		private UniTask LShowAndReturnTask(AnimationCurve animationCurve = null, float circleFade = 0f, float duration = -1f)
		{
			this.taskCounter++;
			if (this.taskCounter > 1)
			{
				return UniTask.CompletedTask;
			}
			this.fadeElement.Duration = ((duration > 0f) ? duration : this.defaultDuration);
			if (animationCurve == null)
			{
				this.SetFadeCurve(this.defaultShowCurve);
			}
			else
			{
				this.SetFadeCurve(animationCurve);
			}
			this.SetCircleFade(circleFade);
			return this.fadeGroup.ShowAndReturnTask();
		}

		// Token: 0x06001EAB RID: 7851 RVA: 0x0006BABC File Offset: 0x00069CBC
		private UniTask LHideAndReturnTask(AnimationCurve animationCurve = null, float circleFade = 0f, float duration = -1f)
		{
			int num = this.taskCounter - 1;
			this.taskCounter = num;
			if (num > 0)
			{
				return UniTask.CompletedTask;
			}
			this.fadeElement.Duration = ((duration > 0f) ? duration : this.defaultDuration);
			if (animationCurve == null)
			{
				this.SetFadeCurve(this.defaultHideCurve);
			}
			else
			{
				this.SetFadeCurve(animationCurve);
			}
			this.SetCircleFade(circleFade);
			return this.fadeGroup.HideAndReturnTask();
		}

		// Token: 0x06001EAC RID: 7852 RVA: 0x0006BB29 File Offset: 0x00069D29
		public static UniTask ShowAndReturnTask(AnimationCurve animationCurve = null, float circleFade = 0f, float duration = 0.5f)
		{
			if (BlackScreen.Instance == null)
			{
				return UniTask.CompletedTask;
			}
			return BlackScreen.Instance.LShowAndReturnTask(animationCurve, circleFade, duration);
		}

		// Token: 0x06001EAD RID: 7853 RVA: 0x0006BB4B File Offset: 0x00069D4B
		public static UniTask HideAndReturnTask(AnimationCurve animationCurve = null, float circleFade = 0f, float duration = 0.5f)
		{
			if (BlackScreen.Instance == null)
			{
				return UniTask.CompletedTask;
			}
			return BlackScreen.Instance.LHideAndReturnTask(animationCurve, circleFade, duration);
		}

		// Token: 0x040014F8 RID: 5368
		[SerializeField]
		private FadeGroup fadeGroup;

		// Token: 0x040014F9 RID: 5369
		[SerializeField]
		private MaterialPropertyFade fadeElement;

		// Token: 0x040014FA RID: 5370
		[SerializeField]
		private Image fadeImage;

		// Token: 0x040014FB RID: 5371
		[SerializeField]
		private float defaultDuration = 0.5f;

		// Token: 0x040014FC RID: 5372
		[SerializeField]
		private AnimationCurve defaultShowCurve;

		// Token: 0x040014FD RID: 5373
		[SerializeField]
		private AnimationCurve defaultHideCurve;

		// Token: 0x040014FE RID: 5374
		private int taskCounter;
	}
}
