using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Duckov.UI.Animations;
using Duckov.Utilities;
using ItemStatsSystem;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Duckov.UI
{
	// Token: 0x02000379 RID: 889
	public class ItemPicker : MonoBehaviour
	{
		// Token: 0x170005F1 RID: 1521
		// (get) Token: 0x06001ECB RID: 7883 RVA: 0x0006C3A0 File Offset: 0x0006A5A0
		// (set) Token: 0x06001ECC RID: 7884 RVA: 0x0006C3A7 File Offset: 0x0006A5A7
		public static ItemPicker Instance { get; private set; }

		// Token: 0x170005F2 RID: 1522
		// (get) Token: 0x06001ECD RID: 7885 RVA: 0x0006C3B0 File Offset: 0x0006A5B0
		private PrefabPool<ItemPickerEntry> EntryPool
		{
			get
			{
				if (this._entryPool == null)
				{
					this._entryPool = new PrefabPool<ItemPickerEntry>(this.entryPrefab, this.contentParent ? this.contentParent : base.transform, new Action<ItemPickerEntry>(this.OnGetEntry), null, null, true, 10, 10000, null);
				}
				return this._entryPool;
			}
		}

		// Token: 0x06001ECE RID: 7886 RVA: 0x0006C40E File Offset: 0x0006A60E
		private void OnGetEntry(ItemPickerEntry entry)
		{
		}

		// Token: 0x170005F3 RID: 1523
		// (get) Token: 0x06001ECF RID: 7887 RVA: 0x0006C410 File Offset: 0x0006A610
		public bool Picking
		{
			get
			{
				return this.picking;
			}
		}

		// Token: 0x06001ED0 RID: 7888 RVA: 0x0006C418 File Offset: 0x0006A618
		private async UniTask<Item> WaitForUserPick(ICollection<Item> candidates)
		{
			Item item;
			if (this.picking)
			{
				Debug.LogError("选择UI已被占用");
				item = null;
			}
			else
			{
				this.picking = true;
				this.confirmed = false;
				this.canceled = false;
				this.pickedItem = null;
				base.gameObject.SetActive(true);
				this.fadeGroup.gameObject.SetActive(true);
				this.SetupUI(candidates);
				RectTransform rectTransform = base.transform as RectTransform;
				rectTransform.ForceUpdateRectTransforms();
				LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
				await UniTask.NextFrame();
				this.fadeGroup.Show();
				do
				{
					await UniTask.NextFrame();
				}
				while (!this.confirmed && !this.canceled);
				this.picking = false;
				this.fadeGroup.Hide();
				if (this.confirmed)
				{
					if (!candidates.Contains(this.pickedItem))
					{
						Debug.LogError("选出了意料之外的物品。");
					}
					item = this.pickedItem;
				}
				else
				{
					bool flag = this.canceled;
					item = null;
				}
			}
			return item;
		}

		// Token: 0x06001ED1 RID: 7889 RVA: 0x0006C464 File Offset: 0x0006A664
		private void Awake()
		{
			if (ItemPicker.Instance == null)
			{
				ItemPicker.Instance = this;
			}
			else
			{
				Debug.LogError("场景中存在两个ItemPicker，请检查。");
			}
			this.confirmButton.onClick.AddListener(new UnityAction(this.OnConfirmButtonClicked));
			this.cancelButton.onClick.AddListener(new UnityAction(this.OnCancelButtonClicked));
		}

		// Token: 0x06001ED2 RID: 7890 RVA: 0x0006C4C8 File Offset: 0x0006A6C8
		private void OnCancelButtonClicked()
		{
			this.Cancel();
		}

		// Token: 0x06001ED3 RID: 7891 RVA: 0x0006C4D0 File Offset: 0x0006A6D0
		private void OnConfirmButtonClicked()
		{
			this.ConfirmPick(this.pickedItem);
		}

		// Token: 0x06001ED4 RID: 7892 RVA: 0x0006C4DE File Offset: 0x0006A6DE
		private void OnDestroy()
		{
		}

		// Token: 0x06001ED5 RID: 7893 RVA: 0x0006C4E0 File Offset: 0x0006A6E0
		private void Update()
		{
			if (!this.picking && this.fadeGroup.IsShown)
			{
				this.fadeGroup.Hide();
			}
		}

		// Token: 0x06001ED6 RID: 7894 RVA: 0x0006C504 File Offset: 0x0006A704
		public static async UniTask<Item> Pick(ICollection<Item> candidates)
		{
			Item item;
			if (ItemPicker.Instance == null)
			{
				item = null;
			}
			else
			{
				item = await ItemPicker.Instance.WaitForUserPick(candidates);
			}
			return item;
		}

		// Token: 0x06001ED7 RID: 7895 RVA: 0x0006C547 File Offset: 0x0006A747
		public void ConfirmPick(Item item)
		{
			this.confirmed = true;
			this.pickedItem = item;
		}

		// Token: 0x06001ED8 RID: 7896 RVA: 0x0006C557 File Offset: 0x0006A757
		public void Cancel()
		{
			this.canceled = true;
		}

		// Token: 0x06001ED9 RID: 7897 RVA: 0x0006C560 File Offset: 0x0006A760
		private void SetupUI(ICollection<Item> candidates)
		{
			this.EntryPool.ReleaseAll();
			foreach (Item item in candidates)
			{
				if (!(item == null))
				{
					ItemPickerEntry itemPickerEntry = this.EntryPool.Get(null);
					itemPickerEntry.Setup(this, item);
					itemPickerEntry.transform.SetAsLastSibling();
				}
			}
		}

		// Token: 0x06001EDA RID: 7898 RVA: 0x0006C5D4 File Offset: 0x0006A7D4
		internal void NotifyEntryClicked(ItemPickerEntry itemPickerEntry, Item target)
		{
			this.pickedItem = target;
		}

		// Token: 0x0400151D RID: 5405
		[SerializeField]
		private FadeGroup fadeGroup;

		// Token: 0x0400151E RID: 5406
		[SerializeField]
		private ItemPickerEntry entryPrefab;

		// Token: 0x0400151F RID: 5407
		[SerializeField]
		private Transform contentParent;

		// Token: 0x04001520 RID: 5408
		[SerializeField]
		private Button confirmButton;

		// Token: 0x04001521 RID: 5409
		[SerializeField]
		private Button cancelButton;

		// Token: 0x04001522 RID: 5410
		private PrefabPool<ItemPickerEntry> _entryPool;

		// Token: 0x04001523 RID: 5411
		private bool picking;

		// Token: 0x04001524 RID: 5412
		private bool canceled;

		// Token: 0x04001525 RID: 5413
		private bool confirmed;

		// Token: 0x04001526 RID: 5414
		private Item pickedItem;
	}
}
