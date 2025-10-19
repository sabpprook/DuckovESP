using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Duckov.UI.Animations
{
	// Token: 0x020003D3 RID: 979
	public class FadeGroup : MonoBehaviour
	{
		// Token: 0x140000EC RID: 236
		// (add) Token: 0x06002399 RID: 9113 RVA: 0x0007C3EC File Offset: 0x0007A5EC
		// (remove) Token: 0x0600239A RID: 9114 RVA: 0x0007C424 File Offset: 0x0007A624
		public event Action<FadeGroup> OnFadeComplete;

		// Token: 0x140000ED RID: 237
		// (add) Token: 0x0600239B RID: 9115 RVA: 0x0007C45C File Offset: 0x0007A65C
		// (remove) Token: 0x0600239C RID: 9116 RVA: 0x0007C494 File Offset: 0x0007A694
		public event Action<FadeGroup> OnShowComplete;

		// Token: 0x140000EE RID: 238
		// (add) Token: 0x0600239D RID: 9117 RVA: 0x0007C4CC File Offset: 0x0007A6CC
		// (remove) Token: 0x0600239E RID: 9118 RVA: 0x0007C504 File Offset: 0x0007A704
		public event Action<FadeGroup> OnHideComplete;

		// Token: 0x170006C9 RID: 1737
		// (get) Token: 0x0600239F RID: 9119 RVA: 0x0007C539 File Offset: 0x0007A739
		public bool IsHidingInProgress
		{
			get
			{
				return this.isHidingInProgress;
			}
		}

		// Token: 0x170006CA RID: 1738
		// (get) Token: 0x060023A0 RID: 9120 RVA: 0x0007C541 File Offset: 0x0007A741
		public bool IsShowingInProgress
		{
			get
			{
				return this.isShowingInProgress;
			}
		}

		// Token: 0x170006CB RID: 1739
		// (get) Token: 0x060023A1 RID: 9121 RVA: 0x0007C549 File Offset: 0x0007A749
		public bool IsShown
		{
			get
			{
				return this.isShown;
			}
		}

		// Token: 0x170006CC RID: 1740
		// (get) Token: 0x060023A2 RID: 9122 RVA: 0x0007C551 File Offset: 0x0007A751
		public bool IsHidden
		{
			get
			{
				return !this.isShown;
			}
		}

		// Token: 0x060023A3 RID: 9123 RVA: 0x0007C55C File Offset: 0x0007A75C
		private void Start()
		{
			if (this.skipHideOnStart)
			{
				this.SkipHide();
			}
		}

		// Token: 0x060023A4 RID: 9124 RVA: 0x0007C56C File Offset: 0x0007A76C
		private void OnEnable()
		{
			if (this.showOnEnable)
			{
				this.Show();
			}
		}

		// Token: 0x060023A5 RID: 9125 RVA: 0x0007C57C File Offset: 0x0007A77C
		[ContextMenu("Show")]
		public void Show()
		{
			if (this.debug)
			{
				Debug.Log("Fadegroup SHOW " + base.name);
			}
			this.skipHideOnStart = false;
			if (this.manageGameObjectActive)
			{
				base.gameObject.SetActive(true);
			}
			this.ShowTask().Forget();
		}

		// Token: 0x060023A6 RID: 9126 RVA: 0x0007C5CC File Offset: 0x0007A7CC
		[ContextMenu("Hide")]
		public void Hide()
		{
			if (this.debug)
			{
				Debug.Log("Fadegroup HIDE " + base.name, base.gameObject);
			}
			this.HideTask().Forget();
		}

		// Token: 0x060023A7 RID: 9127 RVA: 0x0007C5FC File Offset: 0x0007A7FC
		public void Toggle()
		{
			if (this.IsShown)
			{
				this.Hide();
				return;
			}
			if (this.IsHidden)
			{
				this.Show();
			}
		}

		// Token: 0x060023A8 RID: 9128 RVA: 0x0007C61B File Offset: 0x0007A81B
		public UniTask ShowAndReturnTask()
		{
			if (this.skipHideBeforeShow)
			{
				this.SkipHide();
			}
			if (this.manageGameObjectActive)
			{
				base.gameObject.SetActive(true);
			}
			return this.ShowTask();
		}

		// Token: 0x060023A9 RID: 9129 RVA: 0x0007C645 File Offset: 0x0007A845
		public UniTask HideAndReturnTask()
		{
			return this.HideTask();
		}

		// Token: 0x060023AA RID: 9130 RVA: 0x0007C64D File Offset: 0x0007A84D
		private int CacheNewTaskToken()
		{
			this.activeTaskToken = global::UnityEngine.Random.Range(0, int.MaxValue);
			return this.activeTaskToken;
		}

		// Token: 0x060023AB RID: 9131 RVA: 0x0007C668 File Offset: 0x0007A868
		public async UniTask ShowTask()
		{
			this.isHidingInProgress = false;
			this.isShowingInProgress = true;
			this.isShown = true;
			int token = this.CacheNewTaskToken();
			List<UniTask> list = new List<UniTask>();
			foreach (FadeElement fadeElement in this.fadeElements)
			{
				if (fadeElement == null)
				{
					Debug.LogWarning("Element in fade group " + base.name + " is null");
				}
				else
				{
					list.Add(fadeElement.Show(0f));
				}
			}
			await UniTask.WhenAll(list);
			if (token == this.activeTaskToken)
			{
				this.ShowComplete();
			}
		}

		// Token: 0x060023AC RID: 9132 RVA: 0x0007C6AC File Offset: 0x0007A8AC
		public async UniTask HideTask()
		{
			this.isShowingInProgress = false;
			this.isHidingInProgress = true;
			this.isShown = false;
			int token = this.CacheNewTaskToken();
			List<UniTask> list = new List<UniTask>();
			foreach (FadeElement fadeElement in this.fadeElements)
			{
				if (!(fadeElement == null))
				{
					list.Add(fadeElement.Hide());
				}
			}
			await UniTask.WhenAll(list);
			if (token == this.activeTaskToken)
			{
				this.HideComplete();
			}
		}

		// Token: 0x060023AD RID: 9133 RVA: 0x0007C6EF File Offset: 0x0007A8EF
		private void ShowComplete()
		{
			this.isShowingInProgress = false;
			Action<FadeGroup> onFadeComplete = this.OnFadeComplete;
			if (onFadeComplete != null)
			{
				onFadeComplete(this);
			}
			Action<FadeGroup> onShowComplete = this.OnShowComplete;
			if (onShowComplete == null)
			{
				return;
			}
			onShowComplete(this);
		}

		// Token: 0x060023AE RID: 9134 RVA: 0x0007C71C File Offset: 0x0007A91C
		private void HideComplete()
		{
			this.isHidingInProgress = false;
			Action<FadeGroup> onFadeComplete = this.OnFadeComplete;
			if (onFadeComplete != null)
			{
				onFadeComplete(this);
			}
			Action<FadeGroup> onHideComplete = this.OnHideComplete;
			if (onHideComplete != null)
			{
				onHideComplete(this);
			}
			if (this == null)
			{
				return;
			}
			if (this.manageGameObjectActive)
			{
				base.gameObject.SetActive(false);
			}
		}

		// Token: 0x060023AF RID: 9135 RVA: 0x0007C774 File Offset: 0x0007A974
		public void SkipHide()
		{
			foreach (FadeElement fadeElement in this.fadeElements)
			{
				if (fadeElement == null)
				{
					Debug.LogWarning("Element in fade group " + base.name + " is null");
				}
				else
				{
					fadeElement.SkipHide();
				}
			}
			if (this.manageGameObjectActive)
			{
				base.gameObject.SetActive(false);
			}
		}

		// Token: 0x170006CD RID: 1741
		// (get) Token: 0x060023B0 RID: 9136 RVA: 0x0007C800 File Offset: 0x0007AA00
		public bool IsFading
		{
			get
			{
				return this.fadeElements.Any((FadeElement e) => e != null && e.IsFading);
			}
		}

		// Token: 0x060023B1 RID: 9137 RVA: 0x0007C82C File Offset: 0x0007AA2C
		internal void SkipShow()
		{
			foreach (FadeElement fadeElement in this.fadeElements)
			{
				if (fadeElement == null)
				{
					Debug.LogWarning("Element in fade group " + base.name + " is null");
				}
				else
				{
					fadeElement.SkipShow();
				}
			}
			if (this.manageGameObjectActive)
			{
				base.gameObject.SetActive(true);
			}
		}

		// Token: 0x0400182B RID: 6187
		[SerializeField]
		private List<FadeElement> fadeElements = new List<FadeElement>();

		// Token: 0x0400182C RID: 6188
		[SerializeField]
		private bool skipHideOnStart = true;

		// Token: 0x0400182D RID: 6189
		[SerializeField]
		private bool showOnEnable;

		// Token: 0x0400182E RID: 6190
		[SerializeField]
		private bool skipHideBeforeShow = true;

		// Token: 0x04001832 RID: 6194
		public bool manageGameObjectActive;

		// Token: 0x04001833 RID: 6195
		private bool isHidingInProgress;

		// Token: 0x04001834 RID: 6196
		private bool isShowingInProgress;

		// Token: 0x04001835 RID: 6197
		private bool isShown;

		// Token: 0x04001836 RID: 6198
		public bool debug;

		// Token: 0x04001837 RID: 6199
		private int activeTaskToken;
	}
}
