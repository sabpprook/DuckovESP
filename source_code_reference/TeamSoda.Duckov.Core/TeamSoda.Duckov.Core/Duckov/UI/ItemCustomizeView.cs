using System;
using System.Collections.Generic;
using Duckov.UI.Animations;
using Duckov.Utilities;
using ItemStatsSystem;
using ItemStatsSystem.Items;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Duckov.UI
{
	// Token: 0x020003B0 RID: 944
	public class ItemCustomizeView : View, ISingleSelectionMenu<SlotDisplay>
	{
		// Token: 0x1700067E RID: 1662
		// (get) Token: 0x060021F4 RID: 8692 RVA: 0x000763EB File Offset: 0x000745EB
		public static ItemCustomizeView Instance
		{
			get
			{
				return View.GetViewInstance<ItemCustomizeView>();
			}
		}

		// Token: 0x1700067F RID: 1663
		// (get) Token: 0x060021F5 RID: 8693 RVA: 0x000763F4 File Offset: 0x000745F4
		private PrefabPool<ItemDisplay> ItemDisplayPool
		{
			get
			{
				if (this._itemDisplayPool == null)
				{
					this.itemDisplayTemplate.gameObject.SetActive(false);
					this._itemDisplayPool = new PrefabPool<ItemDisplay>(this.itemDisplayTemplate, this.itemDisplayTemplate.transform.parent, null, null, null, true, 10, 10000, null);
				}
				return this._itemDisplayPool;
			}
		}

		// Token: 0x060021F6 RID: 8694 RVA: 0x0007644D File Offset: 0x0007464D
		private void OnGetInventoryDisplay(InventoryDisplay display)
		{
			display.onDisplayDoubleClicked += this.OnInventoryDoubleClicked;
			display.ShowOperationButtons = false;
		}

		// Token: 0x060021F7 RID: 8695 RVA: 0x00076468 File Offset: 0x00074668
		private void OnReleaseInventoryDisplay(InventoryDisplay display)
		{
			display.onDisplayDoubleClicked -= this.OnInventoryDoubleClicked;
		}

		// Token: 0x060021F8 RID: 8696 RVA: 0x0007647C File Offset: 0x0007467C
		private void OnInventoryDoubleClicked(InventoryDisplay display, InventoryEntry entry, PointerEventData data)
		{
			if (entry.Item != null)
			{
				this.target.TryPlug(entry.Item, false, entry.Master.Target, 0);
				data.Use();
			}
		}

		// Token: 0x17000680 RID: 1664
		// (get) Token: 0x060021F9 RID: 8697 RVA: 0x000764B1 File Offset: 0x000746B1
		public Item Target
		{
			get
			{
				return this.target;
			}
		}

		// Token: 0x060021FA RID: 8698 RVA: 0x000764B9 File Offset: 0x000746B9
		public void Setup(Item target, List<Inventory> avaliableInventories)
		{
			this.target = target;
			this.customizingTargetDisplay.Setup(target);
			this.avaliableInventories.Clear();
			this.avaliableInventories.AddRange(avaliableInventories);
		}

		// Token: 0x060021FB RID: 8699 RVA: 0x000764E5 File Offset: 0x000746E5
		public void DebugSetup(Item target, Inventory inventory1, Inventory inventory2)
		{
			this.Setup(target, new List<Inventory> { inventory1, inventory2 });
		}

		// Token: 0x060021FC RID: 8700 RVA: 0x00076501 File Offset: 0x00074701
		protected override void OnOpen()
		{
			base.OnOpen();
			ItemUIUtilities.Select(null);
			ItemUIUtilities.OnSelectionChanged += this.OnItemSelectionChanged;
			this.fadeGroup.Show();
			this.SetSelection(null);
			this.RefreshDetails();
		}

		// Token: 0x060021FD RID: 8701 RVA: 0x00076539 File Offset: 0x00074739
		protected override void OnClose()
		{
			ItemUIUtilities.OnSelectionChanged -= this.OnItemSelectionChanged;
			base.OnClose();
			this.fadeGroup.Hide();
			this.selectedItemDisplayFadeGroup.Hide();
		}

		// Token: 0x060021FE RID: 8702 RVA: 0x00076568 File Offset: 0x00074768
		private void OnItemSelectionChanged()
		{
			this.RefreshDetails();
		}

		// Token: 0x060021FF RID: 8703 RVA: 0x00076570 File Offset: 0x00074770
		private void RefreshDetails()
		{
			if (ItemUIUtilities.SelectedItem != null)
			{
				this.selectedItemDisplayFadeGroup.Show();
				this.selectedItemDisplay.Setup(ItemUIUtilities.SelectedItem);
				Item item = this.selectedItemDisplay.Target;
				bool flag = this.selectedSlotDisplay.Target.Content != item;
				this.equipButton.gameObject.SetActive(flag);
				this.unequipButton.gameObject.SetActive(!flag);
				return;
			}
			this.selectedItemDisplayFadeGroup.Hide();
			this.equipButton.gameObject.SetActive(false);
			this.unequipButton.gameObject.SetActive(false);
		}

		// Token: 0x06002200 RID: 8704 RVA: 0x0007661C File Offset: 0x0007481C
		protected override void Awake()
		{
			base.Awake();
			this.equipButton.onClick.AddListener(new UnityAction(this.OnEquipButtonClicked));
			this.unequipButton.onClick.AddListener(new UnityAction(this.OnUnequipButtonClicked));
			this.customizingTargetDisplay.SlotCollectionDisplay.onElementClicked += this.OnSlotElementClicked;
		}

		// Token: 0x06002201 RID: 8705 RVA: 0x00076684 File Offset: 0x00074884
		private void OnUnequipButtonClicked()
		{
			if (this.selectedSlotDisplay == null)
			{
				return;
			}
			if (this.selectedItemDisplay == null)
			{
				return;
			}
			Slot slot = this.selectedSlotDisplay.Target;
			if (slot.Content != null)
			{
				Item item = slot.Unplug();
				this.HandleUnpluggledItem(item);
			}
			this.RefreshAvaliableItems();
		}

		// Token: 0x06002202 RID: 8706 RVA: 0x000766E0 File Offset: 0x000748E0
		private void OnEquipButtonClicked()
		{
			if (this.selectedSlotDisplay == null)
			{
				return;
			}
			if (this.selectedItemDisplay == null)
			{
				return;
			}
			Slot slot = this.selectedSlotDisplay.Target;
			Item item = this.selectedItemDisplay.Target;
			if (slot == null)
			{
				return;
			}
			if (item == null)
			{
				return;
			}
			if (slot.Content != null)
			{
				Item item2 = slot.Unplug();
				this.HandleUnpluggledItem(item2);
			}
			item.Detach();
			Item item3;
			if (!slot.Plug(item, out item3))
			{
				Debug.LogError("装备失败！");
				this.HandleUnpluggledItem(item);
			}
			this.RefreshAvaliableItems();
		}

		// Token: 0x06002203 RID: 8707 RVA: 0x00076775 File Offset: 0x00074975
		private void HandleUnpluggledItem(Item item)
		{
			if (PlayerStorage.Inventory)
			{
				ItemUtilities.SendToPlayerStorage(item, false);
				return;
			}
			if (!ItemUtilities.SendToPlayerCharacterInventory(item, false))
			{
				ItemUtilities.SendToPlayerStorage(item, false);
			}
		}

		// Token: 0x06002204 RID: 8708 RVA: 0x0007679B File Offset: 0x0007499B
		private void OnSlotElementClicked(ItemSlotCollectionDisplay collection, SlotDisplay slot)
		{
			this.SetSelection(slot);
		}

		// Token: 0x06002205 RID: 8709 RVA: 0x000767A5 File Offset: 0x000749A5
		public SlotDisplay GetSelection()
		{
			return this.selectedSlotDisplay;
		}

		// Token: 0x06002206 RID: 8710 RVA: 0x000767AD File Offset: 0x000749AD
		public bool SetSelection(SlotDisplay selection)
		{
			this.selectedSlotDisplay = selection;
			this.RefreshSelectionIndicator();
			this.OnSlotSelectionChanged();
			return true;
		}

		// Token: 0x06002207 RID: 8711 RVA: 0x000767C4 File Offset: 0x000749C4
		private void RefreshSelectionIndicator()
		{
			this.slotSelectionIndicator.gameObject.SetActive(this.selectedSlotDisplay);
			if (this.selectedSlotDisplay != null)
			{
				this.slotSelectionIndicator.position = this.selectedSlotDisplay.transform.position;
			}
		}

		// Token: 0x06002208 RID: 8712 RVA: 0x00076815 File Offset: 0x00074A15
		private void OnSlotSelectionChanged()
		{
			ItemUIUtilities.Select(null);
			this.RefreshAvaliableItems();
		}

		// Token: 0x06002209 RID: 8713 RVA: 0x00076824 File Offset: 0x00074A24
		private void RefreshAvaliableItems()
		{
			this.avaliableItems.Clear();
			if (!(this.selectedSlotDisplay == null))
			{
				Slot slot = this.selectedSlotDisplay.Target;
				if (!(this.selectedSlotDisplay == null))
				{
					foreach (Inventory inventory in this.avaliableInventories)
					{
						foreach (Item item in inventory)
						{
							if (!(item == null) && slot.CanPlug(item))
							{
								this.avaliableItems.Add(item);
							}
						}
					}
				}
			}
			this.RefreshItemListGraphics();
		}

		// Token: 0x0600220A RID: 8714 RVA: 0x000768F8 File Offset: 0x00074AF8
		private void RefreshItemListGraphics()
		{
			Debug.Log("Refreshing Item List Graphics");
			bool flag = this.selectedSlotDisplay != null;
			bool flag2 = this.avaliableItems.Count > 0;
			this.selectSlotPlaceHolder.SetActive(!flag);
			this.noAvaliableItemPlaceHolder.SetActive(flag && !flag2);
			this.avaliableItemsContainer.SetActive(flag2);
			this.ItemDisplayPool.ReleaseAll();
			if (flag2)
			{
				foreach (Item item in this.avaliableItems)
				{
					if (!(item == null))
					{
						ItemDisplay itemDisplay = this.ItemDisplayPool.Get(null);
						itemDisplay.ShowOperationButtons = false;
						itemDisplay.Setup(item);
					}
				}
			}
		}

		// Token: 0x0400170B RID: 5899
		[SerializeField]
		private FadeGroup fadeGroup;

		// Token: 0x0400170C RID: 5900
		[SerializeField]
		private Button equipButton;

		// Token: 0x0400170D RID: 5901
		[SerializeField]
		private Button unequipButton;

		// Token: 0x0400170E RID: 5902
		[SerializeField]
		private ItemDetailsDisplay customizingTargetDisplay;

		// Token: 0x0400170F RID: 5903
		[SerializeField]
		private ItemDetailsDisplay selectedItemDisplay;

		// Token: 0x04001710 RID: 5904
		[SerializeField]
		private FadeGroup selectedItemDisplayFadeGroup;

		// Token: 0x04001711 RID: 5905
		[SerializeField]
		private RectTransform slotSelectionIndicator;

		// Token: 0x04001712 RID: 5906
		[SerializeField]
		private GameObject selectSlotPlaceHolder;

		// Token: 0x04001713 RID: 5907
		[SerializeField]
		private GameObject avaliableItemsContainer;

		// Token: 0x04001714 RID: 5908
		[SerializeField]
		private GameObject noAvaliableItemPlaceHolder;

		// Token: 0x04001715 RID: 5909
		[SerializeField]
		private ItemDisplay itemDisplayTemplate;

		// Token: 0x04001716 RID: 5910
		private PrefabPool<ItemDisplay> _itemDisplayPool;

		// Token: 0x04001717 RID: 5911
		private Item target;

		// Token: 0x04001718 RID: 5912
		private SlotDisplay selectedSlotDisplay;

		// Token: 0x04001719 RID: 5913
		private List<Inventory> avaliableInventories = new List<Inventory>();

		// Token: 0x0400171A RID: 5914
		private List<Item> avaliableItems = new List<Item>();
	}
}
