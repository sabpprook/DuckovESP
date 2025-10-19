using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Duckov.UI;
using Duckov.UI.Animations;
using Duckov.Utilities;
using ItemStatsSystem;
using SodaCraft.Localizations;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Fishing.UI
{
	// Token: 0x02000214 RID: 532
	public class BaitSelectPanel : MonoBehaviour, ISingleSelectionMenu<BaitSelectPanelEntry>
	{
		// Token: 0x170002DB RID: 731
		// (get) Token: 0x06000FD2 RID: 4050 RVA: 0x0003E084 File Offset: 0x0003C284
		private PrefabPool<BaitSelectPanelEntry> EntryPool
		{
			get
			{
				if (this._entryPool == null)
				{
					this._entryPool = new PrefabPool<BaitSelectPanelEntry>(this.entry, null, null, null, null, true, 10, 10000, null);
				}
				return this._entryPool;
			}
		}

		// Token: 0x1400006D RID: 109
		// (add) Token: 0x06000FD3 RID: 4051 RVA: 0x0003E0C0 File Offset: 0x0003C2C0
		// (remove) Token: 0x06000FD4 RID: 4052 RVA: 0x0003E0F8 File Offset: 0x0003C2F8
		internal event Action onSetSelection;

		// Token: 0x06000FD5 RID: 4053 RVA: 0x0003E130 File Offset: 0x0003C330
		internal async UniTask DoBaitSelection(ICollection<Item> availableBaits, Func<Item, bool> baitSelectionResultCallback)
		{
			this.detailsFadeGroup.SkipHide();
			this.Setup(availableBaits);
			this.Open();
			Item item = await this.WaitForSelection();
			baitSelectionResultCallback(item);
			this.Close();
		}

		// Token: 0x06000FD6 RID: 4054 RVA: 0x0003E183 File Offset: 0x0003C383
		private void Open()
		{
			this.fadeGroup.Show();
		}

		// Token: 0x06000FD7 RID: 4055 RVA: 0x0003E190 File Offset: 0x0003C390
		private void Close()
		{
			this.fadeGroup.Hide();
		}

		// Token: 0x170002DC RID: 732
		// (get) Token: 0x06000FD8 RID: 4056 RVA: 0x0003E19D File Offset: 0x0003C39D
		private Item SelectedItem
		{
			get
			{
				BaitSelectPanelEntry baitSelectPanelEntry = this.selectedEntry;
				if (baitSelectPanelEntry == null)
				{
					return null;
				}
				return baitSelectPanelEntry.Target;
			}
		}

		// Token: 0x06000FD9 RID: 4057 RVA: 0x0003E1B0 File Offset: 0x0003C3B0
		private async UniTask<Item> WaitForSelection()
		{
			this.selectedEntry = null;
			this.canceled = false;
			this.confirmed = false;
			while (base.gameObject.activeInHierarchy && !this.confirmed && !this.canceled)
			{
				await UniTask.Yield();
			}
			Item item;
			if (this.canceled)
			{
				item = null;
			}
			else
			{
				item = this.SelectedItem;
			}
			return item;
		}

		// Token: 0x06000FDA RID: 4058 RVA: 0x0003E1F4 File Offset: 0x0003C3F4
		private void Setup(ICollection<Item> availableBaits)
		{
			this.selectedEntry = null;
			this.EntryPool.ReleaseAll();
			foreach (Item item in availableBaits)
			{
				this.EntryPool.Get(null).Setup(this, item);
			}
		}

		// Token: 0x06000FDB RID: 4059 RVA: 0x0003E25C File Offset: 0x0003C45C
		internal void NotifyStop()
		{
			this.Close();
		}

		// Token: 0x06000FDC RID: 4060 RVA: 0x0003E264 File Offset: 0x0003C464
		private void Awake()
		{
			this.confirmButton.onClick.AddListener(new UnityAction(this.OnConfirmButtonClicked));
			this.cancelButton.onClick.AddListener(new UnityAction(this.OnCancelButtonClicked));
		}

		// Token: 0x06000FDD RID: 4061 RVA: 0x0003E29E File Offset: 0x0003C49E
		private void OnConfirmButtonClicked()
		{
			if (this.SelectedItem == null)
			{
				NotificationText.Push("Fishing_PleaseSelectBait".ToPlainText());
				return;
			}
			this.confirmed = true;
		}

		// Token: 0x06000FDE RID: 4062 RVA: 0x0003E2C5 File Offset: 0x0003C4C5
		private void OnCancelButtonClicked()
		{
			this.canceled = true;
		}

		// Token: 0x06000FDF RID: 4063 RVA: 0x0003E2CE File Offset: 0x0003C4CE
		internal void NotifySelect(BaitSelectPanelEntry baitSelectPanelEntry)
		{
			this.SetSelection(baitSelectPanelEntry);
			if (this.SelectedItem != null)
			{
				this.details.Setup(this.SelectedItem);
				this.detailsFadeGroup.Show();
				return;
			}
			this.detailsFadeGroup.SkipHide();
		}

		// Token: 0x06000FE0 RID: 4064 RVA: 0x0003E30E File Offset: 0x0003C50E
		public BaitSelectPanelEntry GetSelection()
		{
			return this.selectedEntry;
		}

		// Token: 0x06000FE1 RID: 4065 RVA: 0x0003E316 File Offset: 0x0003C516
		public bool SetSelection(BaitSelectPanelEntry selection)
		{
			this.selectedEntry = selection;
			Action action = this.onSetSelection;
			if (action != null)
			{
				action();
			}
			return true;
		}

		// Token: 0x04000CB7 RID: 3255
		[SerializeField]
		private FadeGroup fadeGroup;

		// Token: 0x04000CB8 RID: 3256
		[SerializeField]
		private Button confirmButton;

		// Token: 0x04000CB9 RID: 3257
		[SerializeField]
		private Button cancelButton;

		// Token: 0x04000CBA RID: 3258
		[SerializeField]
		private ItemDetailsDisplay details;

		// Token: 0x04000CBB RID: 3259
		[SerializeField]
		private FadeGroup detailsFadeGroup;

		// Token: 0x04000CBC RID: 3260
		[SerializeField]
		private BaitSelectPanelEntry entry;

		// Token: 0x04000CBD RID: 3261
		private PrefabPool<BaitSelectPanelEntry> _entryPool;

		// Token: 0x04000CBF RID: 3263
		private BaitSelectPanelEntry selectedEntry;

		// Token: 0x04000CC0 RID: 3264
		private bool canceled;

		// Token: 0x04000CC1 RID: 3265
		private bool confirmed;
	}
}
