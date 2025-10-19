using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Duckov.UI.Animations
{
	// Token: 0x020003D2 RID: 978
	public abstract class FadeElement : MonoBehaviour
	{
		// Token: 0x170006C5 RID: 1733
		// (get) Token: 0x06002388 RID: 9096 RVA: 0x0007C226 File Offset: 0x0007A426
		public UniTask ActiveTask
		{
			get
			{
				return this.activeTask;
			}
		}

		// Token: 0x170006C6 RID: 1734
		// (get) Token: 0x06002389 RID: 9097 RVA: 0x0007C22E File Offset: 0x0007A42E
		protected int ActiveTaskToken
		{
			get
			{
				return this.activeTaskToken;
			}
		}

		// Token: 0x170006C7 RID: 1735
		// (get) Token: 0x0600238A RID: 9098 RVA: 0x0007C236 File Offset: 0x0007A436
		protected bool ManageGameObjectActive
		{
			get
			{
				return this.manageGameObjectActive;
			}
		}

		// Token: 0x0600238B RID: 9099 RVA: 0x0007C23E File Offset: 0x0007A43E
		private void CacheNewTaskToken()
		{
			this.activeTaskToken = global::UnityEngine.Random.Range(1, int.MaxValue);
		}

		// Token: 0x170006C8 RID: 1736
		// (get) Token: 0x0600238C RID: 9100 RVA: 0x0007C251 File Offset: 0x0007A451
		// (set) Token: 0x0600238D RID: 9101 RVA: 0x0007C259 File Offset: 0x0007A459
		public bool IsFading { get; private set; }

		// Token: 0x0600238E RID: 9102 RVA: 0x0007C264 File Offset: 0x0007A464
		public async UniTask Show(float delay = 0f)
		{
			this.CacheNewTaskToken();
			this.activeTask = this.WrapShowTask(this.ActiveTaskToken, delay);
			await this.activeTask;
			this.isShown = true;
		}

		// Token: 0x0600238F RID: 9103 RVA: 0x0007C2B0 File Offset: 0x0007A4B0
		public async UniTask Hide()
		{
			this.CacheNewTaskToken();
			this.activeTask = this.WrapHideTask(this.ActiveTaskToken, this.delay);
			this.isShown = false;
			await this.activeTask;
		}

		// Token: 0x06002390 RID: 9104 RVA: 0x0007C2F4 File Offset: 0x0007A4F4
		private async UniTask WrapShowTask(int token, float delay = 0f)
		{
			await UniTask.WaitForSeconds(this.delay + delay, true, PlayerLoopTiming.Update, default(CancellationToken), false);
			if (!(this == null))
			{
				if (this.ActiveTaskToken == token && this.manageGameObjectActive)
				{
					base.gameObject.SetActive(true);
				}
				this.IsFading = true;
				if (!string.IsNullOrWhiteSpace(this.sfx_Show))
				{
					AudioManager.Post(this.sfx_Show);
				}
				await UniTask.NextFrame();
				await this.ShowTask(token);
				if (this.ActiveTaskToken == token)
				{
					this.IsFading = false;
				}
			}
		}

		// Token: 0x06002391 RID: 9105 RVA: 0x0007C348 File Offset: 0x0007A548
		private async UniTask WrapHideTask(int token, float delay = 0f)
		{
			if (!string.IsNullOrWhiteSpace(this.sfx_Hide) && this.isShown)
			{
				AudioManager.Post(this.sfx_Hide);
			}
			this.IsFading = true;
			await UniTask.WaitForSeconds(this.delay + delay, true, PlayerLoopTiming.Update, default(CancellationToken), false);
			await this.HideTask(token);
			if (!(this == null))
			{
				if (this.ActiveTaskToken == token && this.manageGameObjectActive)
				{
					GameObject gameObject = base.gameObject;
					if (gameObject != null)
					{
						gameObject.SetActive(false);
					}
				}
				if (this.ActiveTaskToken == token)
				{
					this.IsFading = false;
				}
			}
		}

		// Token: 0x06002392 RID: 9106
		protected abstract UniTask ShowTask(int token);

		// Token: 0x06002393 RID: 9107
		protected abstract UniTask HideTask(int token);

		// Token: 0x06002394 RID: 9108
		protected abstract void OnSkipHide();

		// Token: 0x06002395 RID: 9109
		protected abstract void OnSkipShow();

		// Token: 0x06002396 RID: 9110 RVA: 0x0007C39B File Offset: 0x0007A59B
		public void SkipHide()
		{
			this.activeTaskToken = 0;
			this.OnSkipHide();
			if (this.ManageGameObjectActive)
			{
				base.gameObject.SetActive(false);
			}
		}

		// Token: 0x06002397 RID: 9111 RVA: 0x0007C3BE File Offset: 0x0007A5BE
		internal void SkipShow()
		{
			this.activeTaskToken = 0;
			this.OnSkipShow();
			if (this.ManageGameObjectActive)
			{
				base.gameObject.SetActive(true);
			}
		}

		// Token: 0x04001823 RID: 6179
		protected UniTask activeTask;

		// Token: 0x04001824 RID: 6180
		private int activeTaskToken;

		// Token: 0x04001825 RID: 6181
		[SerializeField]
		private bool manageGameObjectActive;

		// Token: 0x04001826 RID: 6182
		[SerializeField]
		private float delay;

		// Token: 0x04001827 RID: 6183
		[SerializeField]
		private string sfx_Show;

		// Token: 0x04001828 RID: 6184
		[SerializeField]
		private string sfx_Hide;

		// Token: 0x0400182A RID: 6186
		private bool isShown;
	}
}
