using System;
using System.Collections.Generic;
using Duckov.UI.Animations;
using Duckov.Utilities;
using ItemStatsSystem;
using LeTai.TrueShadow;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Duckov.UI
{
	// Token: 0x020003AF RID: 943
	public class ItemCustomizeSelectionView : View
	{
		// Token: 0x1700067B RID: 1659
		// (get) Token: 0x060021E5 RID: 8677 RVA: 0x0007600B File Offset: 0x0007420B
		public static ItemCustomizeSelectionView Instance
		{
			get
			{
				return View.GetViewInstance<ItemCustomizeSelectionView>();
			}
		}

		// Token: 0x1700067C RID: 1660
		// (get) Token: 0x060021E6 RID: 8678 RVA: 0x00076012 File Offset: 0x00074212
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

		// Token: 0x1700067D RID: 1661
		// (get) Token: 0x060021E7 RID: 8679 RVA: 0x00076030 File Offset: 0x00074230
		private bool CanCustomize
		{
			get
			{
				Item selectedItem = ItemUIUtilities.SelectedItem;
				return !(selectedItem == null) && !(selectedItem.Slots == null) && selectedItem.Slots.Count >= 1;
			}
		}

		// Token: 0x060021E8 RID: 8680 RVA: 0x0007606F File Offset: 0x0007426F
		protected override void Awake()
		{
			base.Awake();
			this.beginCustomizeButton.onClick.AddListener(new UnityAction(this.OnBeginCustomizeButtonClicked));
		}

		// Token: 0x060021E9 RID: 8681 RVA: 0x00076094 File Offset: 0x00074294
		private void OnBeginCustomizeButtonClicked()
		{
			Item selectedItem = ItemUIUtilities.SelectedItem;
			ItemCustomizeView instance = ItemCustomizeView.Instance;
			if (instance == null)
			{
				return;
			}
			instance.Setup(ItemUIUtilities.SelectedItem, this.GetAvaliableInventories());
			instance.Open(null);
		}

		// Token: 0x060021EA RID: 8682 RVA: 0x000760D0 File Offset: 0x000742D0
		private List<Inventory> GetAvaliableInventories()
		{
			this.avaliableInventories.Clear();
			LevelManager instance = LevelManager.Instance;
			Inventory inventory;
			if (instance == null)
			{
				inventory = null;
			}
			else
			{
				CharacterMainControl mainCharacter = instance.MainCharacter;
				if (mainCharacter == null)
				{
					inventory = null;
				}
				else
				{
					Item characterItem = mainCharacter.CharacterItem;
					inventory = ((characterItem != null) ? characterItem.Inventory : null);
				}
			}
			Inventory inventory2 = inventory;
			if (inventory2 != null)
			{
				this.avaliableInventories.Add(inventory2);
			}
			Inventory inventory3 = PlayerStorage.Inventory;
			if (inventory3 != null)
			{
				this.avaliableInventories.Add(inventory3);
			}
			return this.avaliableInventories;
		}

		// Token: 0x060021EB RID: 8683 RVA: 0x00076148 File Offset: 0x00074348
		protected override void OnOpen()
		{
			this.UnregisterEvents();
			base.OnOpen();
			Item characterItem = this.CharacterItem;
			if (characterItem == null)
			{
				Debug.LogError("物品栏开启失败，角色物体不存在");
				return;
			}
			base.gameObject.SetActive(true);
			this.slotDisplay.Setup(characterItem, false);
			this.inventoryDisplay.Setup(characterItem.Inventory, null, null, false, null);
			this.RegisterEvents();
			this.fadeGroup.Show();
			this.customizeButtonFadeGroup.SkipHide();
			this.placeHolderFadeGroup.SkipHide();
			ItemUIUtilities.Select(null);
			this.RefreshSelectedItemInfo();
		}

		// Token: 0x060021EC RID: 8684 RVA: 0x000761DD File Offset: 0x000743DD
		protected override void OnClose()
		{
			this.UnregisterEvents();
			base.OnClose();
			this.fadeGroup.Hide();
			this.itemDetailsFadeGroup.Hide();
		}

		// Token: 0x060021ED RID: 8685 RVA: 0x00076201 File Offset: 0x00074401
		private void RegisterEvents()
		{
			ItemUIUtilities.OnSelectionChanged += this.OnItemSelectionChanged;
		}

		// Token: 0x060021EE RID: 8686 RVA: 0x00076214 File Offset: 0x00074414
		private void OnItemSelectionChanged()
		{
			if (ItemUIUtilities.SelectedItem != null)
			{
				this.detailsDisplay.Setup(ItemUIUtilities.SelectedItem);
				this.itemDetailsFadeGroup.Show();
			}
			else
			{
				this.itemDetailsFadeGroup.Hide();
			}
			if (this.CanCustomize)
			{
				this.placeHolderFadeGroup.Hide();
				this.customizeButtonFadeGroup.Show();
			}
			else
			{
				this.customizeButtonFadeGroup.Hide();
				this.placeHolderFadeGroup.Show();
			}
			this.RefreshSelectedItemInfo();
		}

		// Token: 0x060021EF RID: 8687 RVA: 0x00076292 File Offset: 0x00074492
		private void UnregisterEvents()
		{
			ItemUIUtilities.OnSelectionChanged -= this.OnItemSelectionChanged;
		}

		// Token: 0x060021F0 RID: 8688 RVA: 0x000762A5 File Offset: 0x000744A5
		public static void Show()
		{
			if (ItemCustomizeSelectionView.Instance == null)
			{
				return;
			}
			ItemCustomizeSelectionView.Instance.Open(null);
		}

		// Token: 0x060021F1 RID: 8689 RVA: 0x000762C0 File Offset: 0x000744C0
		public static void Hide()
		{
			if (ItemCustomizeSelectionView.Instance == null)
			{
				return;
			}
			ItemCustomizeSelectionView.Instance.Close();
		}

		// Token: 0x060021F2 RID: 8690 RVA: 0x000762DC File Offset: 0x000744DC
		private void RefreshSelectedItemInfo()
		{
			Item selectedItem = ItemUIUtilities.SelectedItem;
			if (selectedItem == null)
			{
				this.selectedItemName.text = this.noItemSelectedNameText;
				this.selectedItemIcon.sprite = this.noItemSelectedIconSprite;
				this.selectedItemShadow.enabled = false;
				this.customizableIndicator.SetActive(false);
				this.uncustomizableIndicator.SetActive(false);
				this.selectedItemIcon.color = Color.clear;
				return;
			}
			this.selectedItemShadow.enabled = true;
			this.selectedItemIcon.color = Color.white;
			this.selectedItemName.text = selectedItem.DisplayName;
			this.selectedItemIcon.sprite = selectedItem.Icon;
			GameplayDataSettings.UIStyle.GetDisplayQualityLook(selectedItem.DisplayQuality).Apply(this.selectedItemShadow);
			this.customizableIndicator.SetActive(this.CanCustomize);
			this.uncustomizableIndicator.SetActive(!this.CanCustomize);
		}

		// Token: 0x040016FB RID: 5883
		[SerializeField]
		private FadeGroup fadeGroup;

		// Token: 0x040016FC RID: 5884
		[SerializeField]
		private ItemSlotCollectionDisplay slotDisplay;

		// Token: 0x040016FD RID: 5885
		[SerializeField]
		private InventoryDisplay inventoryDisplay;

		// Token: 0x040016FE RID: 5886
		[SerializeField]
		private ItemDetailsDisplay detailsDisplay;

		// Token: 0x040016FF RID: 5887
		[SerializeField]
		private FadeGroup itemDetailsFadeGroup;

		// Token: 0x04001700 RID: 5888
		[SerializeField]
		private FadeGroup customizeButtonFadeGroup;

		// Token: 0x04001701 RID: 5889
		[SerializeField]
		private FadeGroup placeHolderFadeGroup;

		// Token: 0x04001702 RID: 5890
		[SerializeField]
		private Button beginCustomizeButton;

		// Token: 0x04001703 RID: 5891
		[SerializeField]
		private TextMeshProUGUI selectedItemName;

		// Token: 0x04001704 RID: 5892
		[SerializeField]
		private Image selectedItemIcon;

		// Token: 0x04001705 RID: 5893
		[SerializeField]
		private TrueShadow selectedItemShadow;

		// Token: 0x04001706 RID: 5894
		[SerializeField]
		private GameObject customizableIndicator;

		// Token: 0x04001707 RID: 5895
		[SerializeField]
		private GameObject uncustomizableIndicator;

		// Token: 0x04001708 RID: 5896
		[SerializeField]
		private string noItemSelectedNameText = "-";

		// Token: 0x04001709 RID: 5897
		[SerializeField]
		private Sprite noItemSelectedIconSprite;

		// Token: 0x0400170A RID: 5898
		private List<Inventory> avaliableInventories = new List<Inventory>();
	}
}
