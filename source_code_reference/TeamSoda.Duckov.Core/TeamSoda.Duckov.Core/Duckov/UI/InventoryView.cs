using System;
using Duckov.UI.Animations;
using ItemStatsSystem;
using UnityEngine;

namespace Duckov.UI
{
	// Token: 0x020003AE RID: 942
	public class InventoryView : View
	{
		// Token: 0x17000679 RID: 1657
		// (get) Token: 0x060021D9 RID: 8665 RVA: 0x00075E41 File Offset: 0x00074041
		private static InventoryView Instance
		{
			get
			{
				return View.GetViewInstance<InventoryView>();
			}
		}

		// Token: 0x1700067A RID: 1658
		// (get) Token: 0x060021DA RID: 8666 RVA: 0x00075E48 File Offset: 0x00074048
		private Item CharacterItem
		{
			get
			{
				LevelManager instance = LevelManager.Instance;
				if (instance == null)
				{
					return null;
				}
				CharacterMainControl mainCharacter = instance.MainCharacter;
				if (mainCharacter == null)
				{
					return null;
				}
				return mainCharacter.CharacterItem;
			}
		}

		// Token: 0x060021DB RID: 8667 RVA: 0x00075E65 File Offset: 0x00074065
		protected override void Awake()
		{
			base.Awake();
		}

		// Token: 0x060021DC RID: 8668 RVA: 0x00075E70 File Offset: 0x00074070
		private void Update()
		{
			bool flag = true;
			this.inventoryDisplay.Editable = flag;
			this.slotDisplay.Editable = flag;
		}

		// Token: 0x060021DD RID: 8669 RVA: 0x00075E98 File Offset: 0x00074098
		protected override void OnOpen()
		{
			this.UnregisterEvents();
			base.OnOpen();
			Item characterItem = this.CharacterItem;
			if (characterItem == null)
			{
				Debug.LogError("物品栏开启失败，角色物体不存在");
				base.Close();
				return;
			}
			base.gameObject.SetActive(true);
			this.slotDisplay.Setup(characterItem, false);
			this.inventoryDisplay.Setup(characterItem.Inventory, null, null, false, null);
			this.RegisterEvents();
			this.fadeGroup.Show();
		}

		// Token: 0x060021DE RID: 8670 RVA: 0x00075F14 File Offset: 0x00074114
		protected override void OnClose()
		{
			this.UnregisterEvents();
			base.OnClose();
			this.fadeGroup.Hide();
			this.itemDetailsFadeGroup.Hide();
			if (SplitDialogue.Instance && SplitDialogue.Instance.isActiveAndEnabled)
			{
				SplitDialogue.Instance.Cancel();
			}
		}

		// Token: 0x060021DF RID: 8671 RVA: 0x00075F65 File Offset: 0x00074165
		private void RegisterEvents()
		{
			ItemUIUtilities.OnSelectionChanged += this.OnItemSelectionChanged;
		}

		// Token: 0x060021E0 RID: 8672 RVA: 0x00075F78 File Offset: 0x00074178
		private void OnItemSelectionChanged()
		{
			if (ItemUIUtilities.SelectedItem != null)
			{
				this.detailsDisplay.Setup(ItemUIUtilities.SelectedItem);
				this.itemDetailsFadeGroup.Show();
				return;
			}
			this.itemDetailsFadeGroup.Hide();
		}

		// Token: 0x060021E1 RID: 8673 RVA: 0x00075FAE File Offset: 0x000741AE
		private void UnregisterEvents()
		{
			ItemUIUtilities.OnSelectionChanged -= this.OnItemSelectionChanged;
		}

		// Token: 0x060021E2 RID: 8674 RVA: 0x00075FC1 File Offset: 0x000741C1
		public static void Show()
		{
			if (!LevelManager.LevelInited)
			{
				return;
			}
			LootView instance = LootView.Instance;
			if (instance != null)
			{
				instance.Show();
			}
			if (LootView.Instance == null)
			{
				Debug.Log("LOOTVIEW INSTANCE IS NULL");
			}
		}

		// Token: 0x060021E3 RID: 8675 RVA: 0x00075FF2 File Offset: 0x000741F2
		public static void Hide()
		{
			LootView instance = LootView.Instance;
			if (instance == null)
			{
				return;
			}
			instance.Close();
		}

		// Token: 0x040016F6 RID: 5878
		[SerializeField]
		private FadeGroup fadeGroup;

		// Token: 0x040016F7 RID: 5879
		[SerializeField]
		private ItemSlotCollectionDisplay slotDisplay;

		// Token: 0x040016F8 RID: 5880
		[SerializeField]
		private InventoryDisplay inventoryDisplay;

		// Token: 0x040016F9 RID: 5881
		[SerializeField]
		private ItemDetailsDisplay detailsDisplay;

		// Token: 0x040016FA RID: 5882
		[SerializeField]
		private FadeGroup itemDetailsFadeGroup;
	}
}
