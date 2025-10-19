using System;
using System.Collections.Generic;
using Duckov.Economy;
using Duckov.UI.Animations;
using Duckov.Utilities;
using ItemStatsSystem;
using ItemStatsSystem.Items;
using LeTai.TrueShadow;
using SodaCraft.Localizations;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Duckov.UI
{
	// Token: 0x020003B2 RID: 946
	public class ItemRepairView : View
	{
		// Token: 0x17000683 RID: 1667
		// (get) Token: 0x06002220 RID: 8736 RVA: 0x00076ED0 File Offset: 0x000750D0
		public static ItemRepairView Instance
		{
			get
			{
				return View.GetViewInstance<ItemRepairView>();
			}
		}

		// Token: 0x17000684 RID: 1668
		// (get) Token: 0x06002221 RID: 8737 RVA: 0x00076ED7 File Offset: 0x000750D7
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

		// Token: 0x17000685 RID: 1669
		// (get) Token: 0x06002222 RID: 8738 RVA: 0x00076EF4 File Offset: 0x000750F4
		private bool CanRepair
		{
			get
			{
				Item selectedItem = ItemUIUtilities.SelectedItem;
				if (selectedItem == null)
				{
					return false;
				}
				if (!selectedItem.UseDurability)
				{
					return false;
				}
				if (selectedItem.MaxDurabilityWithLoss < 1f)
				{
					return false;
				}
				if (!selectedItem.Tags.Contains("Repairable"))
				{
					Debug.Log(selectedItem.DisplayName + " 不包含tag Repairable");
					return false;
				}
				return selectedItem.Durability < selectedItem.MaxDurabilityWithLoss;
			}
		}

		// Token: 0x17000686 RID: 1670
		// (get) Token: 0x06002223 RID: 8739 RVA: 0x00076F64 File Offset: 0x00075164
		private bool NoNeedToRepair
		{
			get
			{
				Item selectedItem = ItemUIUtilities.SelectedItem;
				return !(selectedItem == null) && selectedItem.UseDurability && selectedItem.Durability >= selectedItem.MaxDurabilityWithLoss;
			}
		}

		// Token: 0x17000687 RID: 1671
		// (get) Token: 0x06002224 RID: 8740 RVA: 0x00076FA0 File Offset: 0x000751A0
		private bool Broken
		{
			get
			{
				Item selectedItem = ItemUIUtilities.SelectedItem;
				return !(selectedItem == null) && selectedItem.UseDurability && selectedItem.MaxDurabilityWithLoss < 1f;
			}
		}

		// Token: 0x140000E8 RID: 232
		// (add) Token: 0x06002225 RID: 8741 RVA: 0x00076FD8 File Offset: 0x000751D8
		// (remove) Token: 0x06002226 RID: 8742 RVA: 0x0007700C File Offset: 0x0007520C
		public static event Action OnRepaireOptionDone;

		// Token: 0x06002227 RID: 8743 RVA: 0x0007703F File Offset: 0x0007523F
		protected override void Awake()
		{
			base.Awake();
			this.repairButton.onClick.AddListener(new UnityAction(this.OnRepairButtonClicked));
			this.itemDetailsFadeGroup.SkipHide();
		}

		// Token: 0x06002228 RID: 8744 RVA: 0x00077070 File Offset: 0x00075270
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

		// Token: 0x06002229 RID: 8745 RVA: 0x000770E8 File Offset: 0x000752E8
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
			this.repairButtonFadeGroup.SkipHide();
			this.placeHolderFadeGroup.SkipHide();
			ItemUIUtilities.Select(null);
			this.RefreshSelectedItemInfo();
			this.repairAllPanel.Setup(this);
		}

		// Token: 0x0600222A RID: 8746 RVA: 0x00077189 File Offset: 0x00075389
		protected override void OnClose()
		{
			this.UnregisterEvents();
			base.OnClose();
			this.fadeGroup.Hide();
			this.itemDetailsFadeGroup.Hide();
		}

		// Token: 0x0600222B RID: 8747 RVA: 0x000771AD File Offset: 0x000753AD
		private void RegisterEvents()
		{
			ItemUIUtilities.OnSelectionChanged += this.OnItemSelectionChanged;
		}

		// Token: 0x0600222C RID: 8748 RVA: 0x000771C0 File Offset: 0x000753C0
		private void OnItemSelectionChanged()
		{
			this.RefreshSelectedItemInfo();
		}

		// Token: 0x0600222D RID: 8749 RVA: 0x000771C8 File Offset: 0x000753C8
		private void UnregisterEvents()
		{
			ItemUIUtilities.OnSelectionChanged -= this.OnItemSelectionChanged;
		}

		// Token: 0x0600222E RID: 8750 RVA: 0x000771DB File Offset: 0x000753DB
		public static void Show()
		{
			if (ItemRepairView.Instance == null)
			{
				return;
			}
			ItemRepairView.Instance.Open(null);
		}

		// Token: 0x0600222F RID: 8751 RVA: 0x000771F6 File Offset: 0x000753F6
		public static void Hide()
		{
			if (ItemRepairView.Instance == null)
			{
				return;
			}
			ItemRepairView.Instance.Close();
		}

		// Token: 0x06002230 RID: 8752 RVA: 0x00077210 File Offset: 0x00075410
		private void RefreshSelectedItemInfo()
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
			if (this.CanRepair)
			{
				this.placeHolderFadeGroup.Hide();
				this.repairButtonFadeGroup.Show();
			}
			else
			{
				this.repairButtonFadeGroup.Hide();
				this.placeHolderFadeGroup.Show();
			}
			Item selectedItem = ItemUIUtilities.SelectedItem;
			this.willLoseDurabilityText.text = "";
			if (selectedItem == null)
			{
				this.selectedItemName.text = this.noItemSelectedNameText;
				this.selectedItemIcon.sprite = this.noItemSelectedIconSprite;
				this.selectedItemShadow.enabled = false;
				this.noNeedToRepairIndicator.SetActive(false);
				this.brokenIndicator.SetActive(false);
				this.cannotRepairIndicator.SetActive(false);
				this.selectedItemIcon.color = Color.clear;
				this.barFill.fillAmount = 0f;
				this.lossBarFill.fillAmount = 0f;
				this.durabilityText.text = "-";
				return;
			}
			this.selectedItemShadow.enabled = true;
			this.selectedItemIcon.color = Color.white;
			this.selectedItemName.text = selectedItem.DisplayName;
			this.selectedItemIcon.sprite = selectedItem.Icon;
			GameplayDataSettings.UIStyle.GetDisplayQualityLook(selectedItem.DisplayQuality).Apply(this.selectedItemShadow);
			this.noNeedToRepairIndicator.SetActive(!this.Broken && this.NoNeedToRepair && selectedItem.Repairable);
			this.cannotRepairIndicator.SetActive(selectedItem.UseDurability && !selectedItem.Repairable && !this.Broken);
			this.brokenIndicator.SetActive(this.Broken);
			if (this.CanRepair)
			{
				float num2;
				float num3;
				float num4;
				int num = this.CalculateRepairPrice(selectedItem, out num2, out num3, out num4);
				this.repairPriceText.text = num.ToString();
				this.willLoseDurabilityText.text = "UI_MaxDurability".ToPlainText() + " -" + num3.ToString("0.#");
				this.repairButton.interactable = EconomyManager.Money >= (long)num;
			}
			if (selectedItem.UseDurability)
			{
				float durability = selectedItem.Durability;
				float maxDurability = selectedItem.MaxDurability;
				float maxDurabilityWithLoss = selectedItem.MaxDurabilityWithLoss;
				float num5 = durability / maxDurability;
				this.barFill.fillAmount = num5;
				this.lossBarFill.fillAmount = selectedItem.DurabilityLoss;
				this.durabilityText.text = string.Format("{0:0.#} / {1} ", durability, maxDurabilityWithLoss.ToString("0.#"));
				this.barFill.color = this.barFillColorOverT.Evaluate(num5);
				return;
			}
			this.barFill.fillAmount = 0f;
			this.lossBarFill.fillAmount = 0f;
			this.durabilityText.text = "-";
		}

		// Token: 0x06002231 RID: 8753 RVA: 0x00077514 File Offset: 0x00075714
		private void OnRepairButtonClicked()
		{
			Item selectedItem = ItemUIUtilities.SelectedItem;
			if (selectedItem == null)
			{
				return;
			}
			if (!selectedItem.UseDurability)
			{
				return;
			}
			this.Repair(selectedItem, false);
			this.RefreshSelectedItemInfo();
		}

		// Token: 0x06002232 RID: 8754 RVA: 0x00077548 File Offset: 0x00075748
		private void Repair(Item item, bool prepaied = false)
		{
			float num2;
			float num3;
			float num4;
			int num = this.CalculateRepairPrice(item, out num2, out num3, out num4);
			if (!prepaied && !EconomyManager.Pay(new Cost((long)num), true, true))
			{
				return;
			}
			item.DurabilityLoss += num4;
			item.Durability = item.MaxDurability * (1f - item.DurabilityLoss);
			Action onRepaireOptionDone = ItemRepairView.OnRepaireOptionDone;
			if (onRepaireOptionDone == null)
			{
				return;
			}
			onRepaireOptionDone();
		}

		// Token: 0x06002233 RID: 8755 RVA: 0x000775B0 File Offset: 0x000757B0
		private int CalculateRepairPrice(Item item, out float repairAmount, out float lostAmount, out float lostPercentage)
		{
			repairAmount = 0f;
			lostAmount = 0f;
			lostPercentage = 0f;
			if (item == null)
			{
				return 0;
			}
			if (!item.UseDurability)
			{
				return 0;
			}
			float maxDurability = item.MaxDurability;
			float durabilityLoss = item.DurabilityLoss;
			float num = maxDurability * (1f - durabilityLoss);
			float durability = item.Durability;
			repairAmount = num - durability;
			float repairLossRatio = item.GetRepairLossRatio();
			lostAmount = repairAmount * repairLossRatio;
			repairAmount -= lostAmount;
			if (repairAmount <= 0f)
			{
				return 0;
			}
			lostPercentage = lostAmount / maxDurability;
			float num2 = repairAmount / maxDurability;
			return Mathf.CeilToInt((float)item.Value * num2 * 0.5f);
		}

		// Token: 0x06002234 RID: 8756 RVA: 0x00077650 File Offset: 0x00075850
		public List<Item> GetAllEquippedItems()
		{
			CharacterMainControl main = CharacterMainControl.Main;
			if (main == null)
			{
				return null;
			}
			Item characterItem = main.CharacterItem;
			if (characterItem == null)
			{
				return null;
			}
			SlotCollection slots = characterItem.Slots;
			if (slots == null)
			{
				return null;
			}
			List<Item> list = new List<Item>();
			foreach (Slot slot in slots)
			{
				if (slot != null)
				{
					Item content = slot.Content;
					if (!(content == null))
					{
						list.Add(content);
					}
				}
			}
			return list;
		}

		// Token: 0x06002235 RID: 8757 RVA: 0x000776F4 File Offset: 0x000758F4
		public int CalculateRepairPrice(List<Item> itemsToRepair)
		{
			int num = 0;
			foreach (Item item in itemsToRepair)
			{
				float num2;
				float num3;
				float num4;
				num += this.CalculateRepairPrice(item, out num2, out num3, out num4);
			}
			return num;
		}

		// Token: 0x06002236 RID: 8758 RVA: 0x00077750 File Offset: 0x00075950
		public void RepairItems(List<Item> itemsToRepair)
		{
			if (!EconomyManager.Pay(new Cost((long)this.CalculateRepairPrice(itemsToRepair)), true, true))
			{
				return;
			}
			foreach (Item item in itemsToRepair)
			{
				this.Repair(item, true);
			}
		}

		// Token: 0x04001728 RID: 5928
		[SerializeField]
		private FadeGroup fadeGroup;

		// Token: 0x04001729 RID: 5929
		[SerializeField]
		private ItemSlotCollectionDisplay slotDisplay;

		// Token: 0x0400172A RID: 5930
		[SerializeField]
		private InventoryDisplay inventoryDisplay;

		// Token: 0x0400172B RID: 5931
		[SerializeField]
		private ItemDetailsDisplay detailsDisplay;

		// Token: 0x0400172C RID: 5932
		[SerializeField]
		private FadeGroup itemDetailsFadeGroup;

		// Token: 0x0400172D RID: 5933
		[SerializeField]
		private ItemRepair_RepairAllPanel repairAllPanel;

		// Token: 0x0400172E RID: 5934
		[SerializeField]
		private FadeGroup repairButtonFadeGroup;

		// Token: 0x0400172F RID: 5935
		[SerializeField]
		private FadeGroup placeHolderFadeGroup;

		// Token: 0x04001730 RID: 5936
		[SerializeField]
		private Button repairButton;

		// Token: 0x04001731 RID: 5937
		[SerializeField]
		private TextMeshProUGUI repairPriceText;

		// Token: 0x04001732 RID: 5938
		[SerializeField]
		private TextMeshProUGUI selectedItemName;

		// Token: 0x04001733 RID: 5939
		[SerializeField]
		private Image selectedItemIcon;

		// Token: 0x04001734 RID: 5940
		[SerializeField]
		private TrueShadow selectedItemShadow;

		// Token: 0x04001735 RID: 5941
		[SerializeField]
		private string noItemSelectedNameText = "-";

		// Token: 0x04001736 RID: 5942
		[SerializeField]
		private Sprite noItemSelectedIconSprite;

		// Token: 0x04001737 RID: 5943
		[SerializeField]
		private GameObject noNeedToRepairIndicator;

		// Token: 0x04001738 RID: 5944
		[SerializeField]
		private GameObject brokenIndicator;

		// Token: 0x04001739 RID: 5945
		[SerializeField]
		private GameObject cannotRepairIndicator;

		// Token: 0x0400173A RID: 5946
		[SerializeField]
		private TextMeshProUGUI durabilityText;

		// Token: 0x0400173B RID: 5947
		[SerializeField]
		private TextMeshProUGUI willLoseDurabilityText;

		// Token: 0x0400173C RID: 5948
		[SerializeField]
		private Image barFill;

		// Token: 0x0400173D RID: 5949
		[SerializeField]
		private Image lossBarFill;

		// Token: 0x0400173E RID: 5950
		[SerializeField]
		private Gradient barFillColorOverT;

		// Token: 0x0400173F RID: 5951
		private List<Inventory> avaliableInventories = new List<Inventory>();
	}
}
