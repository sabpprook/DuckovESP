using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Duckov.UI.Animations;
using Duckov.Utilities;
using ItemStatsSystem;
using ItemStatsSystem.Items;
using SodaCraft.Localizations;
using SodaCraft.StringUtilities;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Duckov.UI
{
	// Token: 0x02000382 RID: 898
	public class FormulasRegisterView : View
	{
		// Token: 0x170005FC RID: 1532
		// (get) Token: 0x06001F12 RID: 7954 RVA: 0x0006CD4F File Offset: 0x0006AF4F
		public static FormulasRegisterView Instance
		{
			get
			{
				return View.GetViewInstance<FormulasRegisterView>();
			}
		}

		// Token: 0x170005FD RID: 1533
		// (get) Token: 0x06001F13 RID: 7955 RVA: 0x0006CD56 File Offset: 0x0006AF56
		private string FormulaUnlockedNotificationFormat
		{
			get
			{
				return this.formulaUnlockedFormatKey.ToPlainText();
			}
		}

		// Token: 0x170005FE RID: 1534
		// (get) Token: 0x06001F14 RID: 7956 RVA: 0x0006CD63 File Offset: 0x0006AF63
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

		// Token: 0x170005FF RID: 1535
		// (get) Token: 0x06001F15 RID: 7957 RVA: 0x0006CD80 File Offset: 0x0006AF80
		private Slot SubmitItemSlot
		{
			get
			{
				if (this.keySlotItem == null)
				{
					return null;
				}
				if (this.keySlotItem.Slots == null)
				{
					return null;
				}
				return this.keySlotItem.Slots[this.slotKey];
			}
		}

		// Token: 0x06001F16 RID: 7958 RVA: 0x0006CDC0 File Offset: 0x0006AFC0
		protected override void Awake()
		{
			base.Awake();
			this.submitButton.onClick.AddListener(new UnityAction(this.OnSubmitButtonClicked));
			this.succeedIndicator.SkipHide();
			this.detailsFadeGroup.SkipHide();
			this.registerSlotDisplay.onSlotDisplayDoubleClicked += this.OnSlotDoubleClicked;
			this.inventoryDisplay.onDisplayDoubleClicked += this.OnInventoryItemDoubleClicked;
			this.playerStorageInventoryDisplay.onDisplayDoubleClicked += this.OnInventoryItemDoubleClicked;
		}

		// Token: 0x06001F17 RID: 7959 RVA: 0x0006CE4C File Offset: 0x0006B04C
		private void OnInventoryItemDoubleClicked(InventoryDisplay display, InventoryEntry entry, PointerEventData data)
		{
			if (!entry.Editable)
			{
				return;
			}
			Item item = entry.Item;
			if (item == null)
			{
				return;
			}
			if (!this.SubmitItemSlot.CanPlug(item))
			{
				return;
			}
			item.Detach();
			Item item2;
			this.SubmitItemSlot.Plug(item, out item2);
			if (item2 != null)
			{
				ItemUtilities.SendToPlayer(item2, false, true);
			}
		}

		// Token: 0x06001F18 RID: 7960 RVA: 0x0006CEA8 File Offset: 0x0006B0A8
		private void OnSlotDoubleClicked(SlotDisplay display)
		{
			Item item = display.GetItem();
			if (item == null)
			{
				return;
			}
			item.Detach();
			ItemUtilities.SendToPlayer(item, false, true);
		}

		// Token: 0x06001F19 RID: 7961 RVA: 0x0006CED4 File Offset: 0x0006B0D4
		private void OnSubmitButtonClicked()
		{
			if (this.SubmitItemSlot != null && this.SubmitItemSlot.Content != null)
			{
				Item content = this.SubmitItemSlot.Content;
				string formulaID = FormulasRegisterView.GetFormulaID(content);
				if (string.IsNullOrEmpty(formulaID))
				{
					return;
				}
				if (CraftingManager.IsFormulaUnlocked(formulaID))
				{
					return;
				}
				CraftingManager.UnlockFormula(formulaID);
				CraftingFormula formula = CraftingManager.GetFormula(formulaID);
				if (formula.IDValid)
				{
					ItemMetaData metaData = ItemAssetsCollection.GetMetaData(formula.result.id);
					string text = this.FormulaUnlockedNotificationFormat.Format(new
					{
						itemDisplayName = metaData.DisplayName
					});
					Sprite icon = metaData.icon;
					StrongNotification.Push(new StrongNotificationContent(text, "", icon));
				}
				content.Detach();
				content.DestroyTreeImmediate();
				this.IndicateSuccess();
			}
		}

		// Token: 0x06001F1A RID: 7962 RVA: 0x0006CF90 File Offset: 0x0006B190
		private void IndicateSuccess()
		{
			this.SuccessIndicationTask().Forget();
		}

		// Token: 0x06001F1B RID: 7963 RVA: 0x0006CFA0 File Offset: 0x0006B1A0
		private async UniTask SuccessIndicationTask()
		{
			this.succeedIndicator.Show();
			AudioManager.Post(this.sfx_Register);
			await UniTask.WaitForSeconds(this.successIndicationTime, true, PlayerLoopTiming.Update, default(CancellationToken), false);
			this.succeedIndicator.Hide();
		}

		// Token: 0x06001F1C RID: 7964 RVA: 0x0006CFE3 File Offset: 0x0006B1E3
		private void HideSuccessIndication()
		{
			this.succeedIndicator.Hide();
		}

		// Token: 0x06001F1D RID: 7965 RVA: 0x0006CFF0 File Offset: 0x0006B1F0
		private bool EntryFunc_ShouldHighligh(Item e)
		{
			return !(e == null) && this.SubmitItemSlot.CanPlug(e) && !CraftingManager.IsFormulaUnlocked(FormulasRegisterView.GetFormulaID(e));
		}

		// Token: 0x06001F1E RID: 7966 RVA: 0x0006D01D File Offset: 0x0006B21D
		private bool EntryFunc_CanOperate(Item e)
		{
			return e == null || this.SubmitItemSlot.CanPlug(e);
		}

		// Token: 0x06001F1F RID: 7967 RVA: 0x0006D038 File Offset: 0x0006B238
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
			this.inventoryDisplay.ShowOperationButtons = false;
			this.inventoryDisplay.Setup(characterItem.Inventory, new Func<Item, bool>(this.EntryFunc_ShouldHighligh), new Func<Item, bool>(this.EntryFunc_CanOperate), true, null);
			if (PlayerStorage.Inventory != null)
			{
				this.playerStorageInventoryDisplay.ShowOperationButtons = false;
				this.playerStorageInventoryDisplay.gameObject.SetActive(true);
				this.playerStorageInventoryDisplay.Setup(PlayerStorage.Inventory, new Func<Item, bool>(this.EntryFunc_ShouldHighligh), new Func<Item, bool>(this.EntryFunc_CanOperate), true, null);
			}
			else
			{
				this.playerStorageInventoryDisplay.gameObject.SetActive(false);
			}
			this.registerSlotDisplay.Setup(this.SubmitItemSlot);
			this.RefreshRecordExistsIndicator();
			this.RegisterEvents();
			this.fadeGroup.Show();
		}

		// Token: 0x06001F20 RID: 7968 RVA: 0x0006D144 File Offset: 0x0006B344
		protected override void OnClose()
		{
			this.UnregisterEvents();
			base.OnClose();
			this.fadeGroup.Hide();
			this.detailsFadeGroup.Hide();
			if (this.SubmitItemSlot != null && this.SubmitItemSlot.Content != null)
			{
				Item content = this.SubmitItemSlot.Content;
				content.Detach();
				ItemUtilities.SendToPlayerCharacterInventory(content, false);
			}
		}

		// Token: 0x06001F21 RID: 7969 RVA: 0x0006D1A6 File Offset: 0x0006B3A6
		private void RegisterEvents()
		{
			this.SubmitItemSlot.onSlotContentChanged += this.OnSlotContentChanged;
			ItemUIUtilities.OnSelectionChanged += this.OnItemSelectionChanged;
		}

		// Token: 0x06001F22 RID: 7970 RVA: 0x0006D1D0 File Offset: 0x0006B3D0
		private void UnregisterEvents()
		{
			this.SubmitItemSlot.onSlotContentChanged -= this.OnSlotContentChanged;
			ItemUIUtilities.OnSelectionChanged -= this.OnItemSelectionChanged;
		}

		// Token: 0x06001F23 RID: 7971 RVA: 0x0006D1FA File Offset: 0x0006B3FA
		private void OnSlotContentChanged(Slot slot)
		{
			this.RefreshRecordExistsIndicator();
			this.HideSuccessIndication();
			if (((slot != null) ? slot.Content : null) != null)
			{
				AudioManager.PlayPutItemSFX(slot.Content, false);
			}
		}

		// Token: 0x06001F24 RID: 7972 RVA: 0x0006D228 File Offset: 0x0006B428
		private void RefreshRecordExistsIndicator()
		{
			Item content = this.SubmitItemSlot.Content;
			if (content == null)
			{
				this.recordExistsIndicator.SetActive(false);
				return;
			}
			bool flag = CraftingManager.IsFormulaUnlocked(FormulasRegisterView.GetFormulaID(content));
			this.recordExistsIndicator.SetActive(flag);
		}

		// Token: 0x06001F25 RID: 7973 RVA: 0x0006D26F File Offset: 0x0006B46F
		private bool IsFormulaItem(Item item)
		{
			return !(item == null) && item.GetComponent<ItemSetting_Formula>() != null;
		}

		// Token: 0x06001F26 RID: 7974 RVA: 0x0006D288 File Offset: 0x0006B488
		public static string GetFormulaID(Item item)
		{
			if (item == null)
			{
				return null;
			}
			ItemSetting_Formula component = item.GetComponent<ItemSetting_Formula>();
			if (component == null)
			{
				return null;
			}
			return component.formulaID;
		}

		// Token: 0x06001F27 RID: 7975 RVA: 0x0006D2B8 File Offset: 0x0006B4B8
		private void OnItemSelectionChanged()
		{
			if (ItemUIUtilities.SelectedItem != null)
			{
				this.detailsDisplay.Setup(ItemUIUtilities.SelectedItem);
				this.detailsFadeGroup.Show();
				return;
			}
			this.detailsFadeGroup.Hide();
		}

		// Token: 0x06001F28 RID: 7976 RVA: 0x0006D2EE File Offset: 0x0006B4EE
		public static void Show(ICollection<Tag> requireTags = null)
		{
			if (FormulasRegisterView.Instance == null)
			{
				return;
			}
			FormulasRegisterView.SetupTags(requireTags);
			FormulasRegisterView.Instance.Open(null);
		}

		// Token: 0x06001F29 RID: 7977 RVA: 0x0006D310 File Offset: 0x0006B510
		private static void SetupTags(ICollection<Tag> requireTags = null)
		{
			if (FormulasRegisterView.Instance == null)
			{
				return;
			}
			Slot submitItemSlot = FormulasRegisterView.Instance.SubmitItemSlot;
			if (submitItemSlot == null)
			{
				return;
			}
			submitItemSlot.requireTags.Clear();
			submitItemSlot.requireTags.Add(FormulasRegisterView.Instance.formulaTag);
			if (requireTags != null)
			{
				submitItemSlot.requireTags.AddRange(requireTags);
			}
		}

		// Token: 0x04001540 RID: 5440
		[SerializeField]
		private FadeGroup fadeGroup;

		// Token: 0x04001541 RID: 5441
		[SerializeField]
		private InventoryDisplay inventoryDisplay;

		// Token: 0x04001542 RID: 5442
		[SerializeField]
		private InventoryDisplay playerStorageInventoryDisplay;

		// Token: 0x04001543 RID: 5443
		[SerializeField]
		private ItemDetailsDisplay detailsDisplay;

		// Token: 0x04001544 RID: 5444
		[SerializeField]
		private FadeGroup detailsFadeGroup;

		// Token: 0x04001545 RID: 5445
		[SerializeField]
		private Button submitButton;

		// Token: 0x04001546 RID: 5446
		[SerializeField]
		private Tag formulaTag;

		// Token: 0x04001547 RID: 5447
		[SerializeField]
		private Item keySlotItem;

		// Token: 0x04001548 RID: 5448
		[SerializeField]
		private string slotKey = "SubmitItem";

		// Token: 0x04001549 RID: 5449
		[SerializeField]
		private SlotDisplay registerSlotDisplay;

		// Token: 0x0400154A RID: 5450
		[SerializeField]
		private GameObject recordExistsIndicator;

		// Token: 0x0400154B RID: 5451
		[SerializeField]
		private FadeGroup succeedIndicator;

		// Token: 0x0400154C RID: 5452
		[SerializeField]
		private float successIndicationTime = 1.5f;

		// Token: 0x0400154D RID: 5453
		private string sfx_Register = "UI/register";

		// Token: 0x0400154E RID: 5454
		[LocalizationKey("Default")]
		[SerializeField]
		private string formulaUnlockedFormatKey = "UI_Formulas_RegisterSucceedFormat";
	}
}
