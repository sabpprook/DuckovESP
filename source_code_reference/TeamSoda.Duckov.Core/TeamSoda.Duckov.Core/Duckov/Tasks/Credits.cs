using System;
using Cysharp.Threading.Tasks;
using Duckov.Achievements;
using Duckov.UI.Animations;
using UnityEngine;

namespace Duckov.Tasks
{
	// Token: 0x0200036D RID: 877
	public class Credits : MonoBehaviour, ITaskBehaviour
	{
		// Token: 0x06001E55 RID: 7765 RVA: 0x0006AE52 File Offset: 0x00069052
		private void Awake()
		{
			this.rectTransform = base.transform as RectTransform;
		}

		// Token: 0x06001E56 RID: 7766 RVA: 0x0006AE65 File Offset: 0x00069065
		public void Begin()
		{
			if (this.task.Status == UniTaskStatus.Pending)
			{
				return;
			}
			this.skip = false;
			this.fadeGroup.SkipHide();
			this.fadeGroup.gameObject.SetActive(true);
			this.task = this.Task();
		}

		// Token: 0x06001E57 RID: 7767 RVA: 0x0006AEA4 File Offset: 0x000690A4
		public bool IsPending()
		{
			return this.task.Status == UniTaskStatus.Pending;
		}

		// Token: 0x06001E58 RID: 7768 RVA: 0x0006AEB4 File Offset: 0x000690B4
		public bool IsComplete()
		{
			return !this.IsPending();
		}

		// Token: 0x06001E59 RID: 7769 RVA: 0x0006AEC0 File Offset: 0x000690C0
		private async UniTask Task()
		{
			if (!this.mute)
			{
				AudioManager.PlayBGM("mus_main_theme");
			}
			this.fadeGroup.Show();
			await UniTask.Yield();
			this.content.anchoredPosition = Vector3.zero;
			float height = this.rectTransform.rect.height;
			float height2 = this.content.rect.height;
			float yMax = height * 0.5f + height2;
			float y = 0f;
			while (y < yMax)
			{
				y += Time.deltaTime * this.scrollSpeed * (this.skip ? 20f : 1f);
				this.content.anchoredPosition = Vector3.up * y;
				await UniTask.Yield();
			}
			float holdBuffer = 0f;
			while (!this.skip)
			{
				await UniTask.Yield();
				holdBuffer += Time.unscaledDeltaTime;
				if (holdBuffer > this.holdForSeconds)
				{
					break;
				}
			}
			if (this.fadeOut)
			{
				await this.fadeGroup.HideAndReturnTask();
			}
			if (AchievementManager.Instance != null)
			{
				AchievementManager.Instance.Unlock("Escape_From_Duckov");
			}
			if (!this.mute)
			{
				AudioManager.StopBGM();
			}
		}

		// Token: 0x06001E5A RID: 7770 RVA: 0x0006AF03 File Offset: 0x00069103
		public void Skip()
		{
			this.skip = true;
			if (this.fadeOut && this.fadeGroup.IsFading)
			{
				this.fadeGroup.SkipHide();
			}
			if (!this.mute)
			{
				AudioManager.StopBGM();
			}
		}

		// Token: 0x040014B7 RID: 5303
		private RectTransform rectTransform;

		// Token: 0x040014B8 RID: 5304
		[SerializeField]
		private FadeGroup fadeGroup;

		// Token: 0x040014B9 RID: 5305
		[SerializeField]
		private RectTransform content;

		// Token: 0x040014BA RID: 5306
		[SerializeField]
		private float scrollSpeed;

		// Token: 0x040014BB RID: 5307
		[SerializeField]
		private float holdForSeconds;

		// Token: 0x040014BC RID: 5308
		[SerializeField]
		private bool fadeOut;

		// Token: 0x040014BD RID: 5309
		[SerializeField]
		private bool mute;

		// Token: 0x040014BE RID: 5310
		private UniTask task;

		// Token: 0x040014BF RID: 5311
		private bool skip;
	}
}
