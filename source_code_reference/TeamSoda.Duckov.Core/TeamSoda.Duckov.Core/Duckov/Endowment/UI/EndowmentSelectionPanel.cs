using System;
using Cysharp.Threading.Tasks;
using Duckov.UI;
using Duckov.UI.Animations;
using Duckov.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Duckov.Endowment.UI
{
	// Token: 0x020002F5 RID: 757
	public class EndowmentSelectionPanel : View
	{
		// Token: 0x1700047C RID: 1148
		// (get) Token: 0x06001894 RID: 6292 RVA: 0x00059684 File Offset: 0x00057884
		private PrefabPool<EndowmentSelectionEntry> Pool
		{
			get
			{
				if (this._pool == null)
				{
					this._pool = new PrefabPool<EndowmentSelectionEntry>(this.entryTemplate, null, null, null, null, true, 10, 10000, delegate(EndowmentSelectionEntry e)
					{
						e.onClicked = (Action<EndowmentSelectionEntry, PointerEventData>)Delegate.Combine(e.onClicked, new Action<EndowmentSelectionEntry, PointerEventData>(this.OnEntryClicked));
					});
				}
				return this._pool;
			}
		}

		// Token: 0x06001895 RID: 6293 RVA: 0x000596C8 File Offset: 0x000578C8
		protected override void Awake()
		{
			base.Awake();
			this.confirmButton.onClick.AddListener(new UnityAction(this.OnConfirmButtonClicked));
			this.cancelButton.onClick.AddListener(new UnityAction(this.OnCancelButtonClicked));
		}

		// Token: 0x06001896 RID: 6294 RVA: 0x00059708 File Offset: 0x00057908
		protected override void OnCancel()
		{
			base.OnCancel();
			this.canceled = true;
		}

		// Token: 0x06001897 RID: 6295 RVA: 0x00059717 File Offset: 0x00057917
		private void OnCancelButtonClicked()
		{
			this.canceled = true;
		}

		// Token: 0x06001898 RID: 6296 RVA: 0x00059720 File Offset: 0x00057920
		private void OnConfirmButtonClicked()
		{
			this.confirmed = true;
		}

		// Token: 0x06001899 RID: 6297 RVA: 0x00059729 File Offset: 0x00057929
		private void OnEntryClicked(EndowmentSelectionEntry entry, PointerEventData data)
		{
			if (entry.Locked)
			{
				return;
			}
			this.Select(entry);
		}

		// Token: 0x0600189A RID: 6298 RVA: 0x0005973C File Offset: 0x0005793C
		private void Select(EndowmentSelectionEntry entry)
		{
			this.Selection = entry;
			foreach (EndowmentSelectionEntry endowmentSelectionEntry in this.Pool.ActiveEntries)
			{
				endowmentSelectionEntry.SetSelection(endowmentSelectionEntry == entry);
			}
			this.RefreshDescription();
		}

		// Token: 0x1700047D RID: 1149
		// (get) Token: 0x0600189B RID: 6299 RVA: 0x000597A0 File Offset: 0x000579A0
		// (set) Token: 0x0600189C RID: 6300 RVA: 0x000597A8 File Offset: 0x000579A8
		public EndowmentSelectionEntry Selection { get; private set; }

		// Token: 0x0600189D RID: 6301 RVA: 0x000597B4 File Offset: 0x000579B4
		public void Setup()
		{
			if (EndowmentManager.Instance == null)
			{
				return;
			}
			this.Pool.ReleaseAll();
			foreach (EndowmentEntry endowmentEntry in EndowmentManager.Instance.Entries)
			{
				if (!(endowmentEntry == null))
				{
					this.Pool.Get(null).Setup(endowmentEntry);
				}
			}
			foreach (EndowmentSelectionEntry endowmentSelectionEntry in this.Pool.ActiveEntries)
			{
				if (endowmentSelectionEntry.Target.Index == EndowmentManager.SelectedIndex)
				{
					this.Select(endowmentSelectionEntry);
					break;
				}
			}
		}

		// Token: 0x0600189E RID: 6302 RVA: 0x00059888 File Offset: 0x00057A88
		private void RefreshDescription()
		{
			if (this.Selection == null)
			{
				this.descriptionText.text = "-";
			}
			this.descriptionText.text = this.Selection.DescriptionAndEffects;
		}

		// Token: 0x0600189F RID: 6303 RVA: 0x000598BE File Offset: 0x00057ABE
		protected override void OnOpen()
		{
			base.OnOpen();
			this.Execute().Forget();
		}

		// Token: 0x060018A0 RID: 6304 RVA: 0x000598D1 File Offset: 0x00057AD1
		protected override void OnClose()
		{
			base.OnClose();
			this.fadeGroup.Hide();
		}

		// Token: 0x060018A1 RID: 6305 RVA: 0x000598E4 File Offset: 0x00057AE4
		public async UniTask Execute()
		{
			this.Setup();
			await this.fadeGroup.ShowAndReturnTask();
			await this.WaitForConfirm();
			if (this.confirmed && this.Selection.Index != EndowmentManager.CurrentIndex)
			{
				EndowmentManager.Instance.SelectIndex(this.Selection.Index);
				SceneLoader.Instance.LoadBaseScene(null, false).Forget();
			}
			base.Close();
		}

		// Token: 0x060018A2 RID: 6306 RVA: 0x00059928 File Offset: 0x00057B28
		private async UniTask WaitForConfirm()
		{
			this.confirmed = false;
			this.canceled = false;
			while ((!this.confirmed || !(this.Selection != null)) && !this.canceled)
			{
				await UniTask.Yield();
			}
		}

		// Token: 0x060018A3 RID: 6307 RVA: 0x0005996B File Offset: 0x00057B6B
		internal void SkipHide()
		{
			if (this.fadeGroup != null)
			{
				this.fadeGroup.SkipHide();
			}
		}

		// Token: 0x060018A4 RID: 6308 RVA: 0x00059988 File Offset: 0x00057B88
		public static void Show()
		{
			EndowmentSelectionPanel viewInstance = View.GetViewInstance<EndowmentSelectionPanel>();
			if (viewInstance == null)
			{
				return;
			}
			viewInstance.Open(null);
		}

		// Token: 0x040011E7 RID: 4583
		[SerializeField]
		private FadeGroup fadeGroup;

		// Token: 0x040011E8 RID: 4584
		[SerializeField]
		private EndowmentSelectionEntry entryTemplate;

		// Token: 0x040011E9 RID: 4585
		[SerializeField]
		private TextMeshProUGUI descriptionText;

		// Token: 0x040011EA RID: 4586
		[SerializeField]
		private Button confirmButton;

		// Token: 0x040011EB RID: 4587
		[SerializeField]
		private Button cancelButton;

		// Token: 0x040011EC RID: 4588
		private PrefabPool<EndowmentSelectionEntry> _pool;

		// Token: 0x040011EE RID: 4590
		private bool confirmed;

		// Token: 0x040011EF RID: 4591
		private bool canceled;
	}
}
