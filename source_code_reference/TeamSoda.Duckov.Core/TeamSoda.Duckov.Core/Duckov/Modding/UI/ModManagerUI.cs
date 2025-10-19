using System;
using Cysharp.Threading.Tasks;
using Duckov.UI.Animations;
using Duckov.Utilities;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Duckov.Modding.UI
{
	// Token: 0x0200026C RID: 620
	public class ModManagerUI : MonoBehaviour
	{
		// Token: 0x17000383 RID: 899
		// (get) Token: 0x0600135D RID: 4957 RVA: 0x000482CA File Offset: 0x000464CA
		private ModManager Master
		{
			get
			{
				return ModManager.Instance;
			}
		}

		// Token: 0x17000384 RID: 900
		// (get) Token: 0x0600135E RID: 4958 RVA: 0x000482D4 File Offset: 0x000464D4
		private PrefabPool<ModEntry> Pool
		{
			get
			{
				if (this._pool == null)
				{
					this._pool = new PrefabPool<ModEntry>(this.entryTemplate, null, null, null, null, true, 10, 10000, null);
				}
				return this._pool;
			}
		}

		// Token: 0x0600135F RID: 4959 RVA: 0x00048310 File Offset: 0x00046510
		private void Awake()
		{
			this.agreementBtn.onClick.AddListener(new UnityAction(this.OnAgreementBtnClicked));
			this.quitBtn.onClick.AddListener(new UnityAction(this.Quit));
			this.rejectBtn.onClick.AddListener(new UnityAction(this.OnRejectBtnClicked));
		}

		// Token: 0x06001360 RID: 4960 RVA: 0x00048371 File Offset: 0x00046571
		private void OnRejectBtnClicked()
		{
			ModManager.AllowActivatingMod = false;
			this.Quit();
		}

		// Token: 0x06001361 RID: 4961 RVA: 0x0004837F File Offset: 0x0004657F
		private void OnAgreementBtnClicked()
		{
			ModManager.AllowActivatingMod = true;
			this.agreementFadeGroup.Hide();
			this.contentFadeGroup.Show();
		}

		// Token: 0x06001362 RID: 4962 RVA: 0x0004839D File Offset: 0x0004659D
		private void Show()
		{
			this.mainFadeGroup.Show();
		}

		// Token: 0x06001363 RID: 4963 RVA: 0x000483AC File Offset: 0x000465AC
		private void OnEnable()
		{
			ModManager.Rescan();
			this.Pool.ReleaseAll();
			foreach (ModInfo modInfo in ModManager.modInfos)
			{
				this.Pool.Get(null).Setup(this, modInfo);
			}
			this.uploaderFadeGroup.SkipHide();
			if (!ModManager.AllowActivatingMod)
			{
				this.contentFadeGroup.SkipHide();
				this.agreementFadeGroup.Show();
				return;
			}
			this.agreementFadeGroup.SkipHide();
			this.contentFadeGroup.Show();
		}

		// Token: 0x06001364 RID: 4964 RVA: 0x0004845C File Offset: 0x0004665C
		private void Hide()
		{
			this.mainFadeGroup.Hide();
		}

		// Token: 0x06001365 RID: 4965 RVA: 0x00048469 File Offset: 0x00046669
		private void Quit()
		{
			UnityEvent unityEvent = this.onQuit;
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
			this.Hide();
		}

		// Token: 0x06001366 RID: 4966 RVA: 0x00048484 File Offset: 0x00046684
		internal async UniTask BeginUpload(ModInfo info)
		{
			if (!this.uploading)
			{
				this.uploading = true;
				this.contentFadeGroup.Hide();
				await this.uploadPanel.Execute(info);
				this.contentFadeGroup.Show();
				this.uploading = false;
			}
		}

		// Token: 0x04000E6C RID: 3692
		[SerializeField]
		private FadeGroup mainFadeGroup;

		// Token: 0x04000E6D RID: 3693
		[SerializeField]
		private FadeGroup contentFadeGroup;

		// Token: 0x04000E6E RID: 3694
		[SerializeField]
		private FadeGroup agreementFadeGroup;

		// Token: 0x04000E6F RID: 3695
		[SerializeField]
		private FadeGroup uploaderFadeGroup;

		// Token: 0x04000E70 RID: 3696
		[SerializeField]
		private ModUploadPanel uploadPanel;

		// Token: 0x04000E71 RID: 3697
		[SerializeField]
		private Button rejectBtn;

		// Token: 0x04000E72 RID: 3698
		[SerializeField]
		private Button agreementBtn;

		// Token: 0x04000E73 RID: 3699
		[SerializeField]
		private ModEntry entryTemplate;

		// Token: 0x04000E74 RID: 3700
		[SerializeField]
		private Button quitBtn;

		// Token: 0x04000E75 RID: 3701
		public UnityEvent onQuit;

		// Token: 0x04000E76 RID: 3702
		private PrefabPool<ModEntry> _pool;

		// Token: 0x04000E77 RID: 3703
		private bool uploading;
	}
}
