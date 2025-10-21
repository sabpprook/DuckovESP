using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Duckov.UI;
using Duckov.UI.Animations;
using ItemStatsSystem;
using ItemStatsSystem.Items;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Duckov.MasterKeys.UI
{
	// Token: 0x020002DF RID: 735
	public class MasterKeysRegisterView : View
	{
		// Token: 0x17000438 RID: 1080
		// (get) Token: 0x06001778 RID: 6008 RVA: 0x0005609C File Offset: 0x0005429C
		public static MasterKeysRegisterView Instance
		{
			get
			{
				return View.GetViewInstance<MasterKeysRegisterView>();
			}
		}

		// Token: 0x17000439 RID: 1081
		// (get) Token: 0x06001779 RID: 6009 RVA: 0x000560A3 File Offset: 0x000542A3
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

		// Token: 0x1700043A RID: 1082
		// (get) Token: 0x0600177A RID: 6010 RVA: 0x000560C0 File Offset: 0x000542C0
		private Slot KeySlot
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
				return this.keySlotItem.Slots[this.keySlotKey];
			}
		}

		// Token: 0x0600177B RID: 6011 RVA: 0x00056100 File Offset: 0x00054300
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

		// Token: 0x0600177C RID: 6012 RVA: 0x0005618C File Offset: 0x0005438C
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
			if (!this.KeySlot.CanPlug(item))
			{
				return;
			}
			item.Detach();
			Item item2;
			this.KeySlot.Plug(item, out item2);
			if (item2 != null)
			{
				ItemUtilities.SendToPlayer(item2, false, true);
			}
		}

		// Token: 0x0600177D RID: 6013 RVA: 0x000561E8 File Offset: 0x000543E8
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

		// Token: 0x0600177E RID: 6014 RVA: 0x00056214 File Offset: 0x00054414
		private void OnSubmitButtonClicked()
		{
			if (this.KeySlot != null && this.KeySlot.Content != null && MasterKeysManager.SubmitAndActivate(this.KeySlot.Content))
			{
				this.IndicateSuccess();
			}
		}

		// Token: 0x0600177F RID: 6015 RVA: 0x00056249 File Offset: 0x00054449
		private void IndicateSuccess()
		{
			this.SuccessIndicationTask().Forget();
		}

		// Token: 0x06001780 RID: 6016 RVA: 0x00056258 File Offset: 0x00054458
		private async UniTask SuccessIndicationTask()
		{
			this.succeedIndicator.Show();
			AudioManager.Post(this.sfx_Register);
			await UniTask.WaitForSeconds(this.successIndicationTime, true, PlayerLoopTiming.Update, default(CancellationToken), false);
			this.succeedIndicator.Hide();
		}

		// Token: 0x06001781 RID: 6017 RVA: 0x0005629B File Offset: 0x0005449B
		private void HideSuccessIndication()
		{
			this.succeedIndicator.Hide();
		}

		// Token: 0x06001782 RID: 6018 RVA: 0x000562A8 File Offset: 0x000544A8
		private bool EntryFunc_ShouldHighligh(Item e)
		{
			return !(e == null) && this.KeySlot.CanPlug(e) && !MasterKeysManager.IsActive(e.TypeID);
		}

		// Token: 0x06001783 RID: 6019 RVA: 0x000562D5 File Offset: 0x000544D5
		private bool EntryFunc_CanOperate(Item e)
		{
			return e == null || this.KeySlot.CanPlug(e);
		}

		// Token: 0x06001784 RID: 6020 RVA: 0x000562F0 File Offset: 0x000544F0
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
			this.inventoryDisplay.Setup(characterItem.Inventory, new Func<Item, bool>(this.EntryFunc_ShouldHighligh), new Func<Item, bool>(this.EntryFunc_CanOperate), false, null);
			if (PlayerStorage.Inventory != null)
			{
				this.playerStorageInventoryDisplay.ShowOperationButtons = false;
				this.playerStorageInventoryDisplay.gameObject.SetActive(true);
				this.playerStorageInventoryDisplay.Setup(PlayerStorage.Inventory, new Func<Item, bool>(this.EntryFunc_ShouldHighligh), new Func<Item, bool>(this.EntryFunc_CanOperate), false, null);
			}
			else
			{
				this.playerStorageInventoryDisplay.gameObject.SetActive(false);
			}
			this.registerSlotDisplay.Setup(this.KeySlot);
			this.RefreshRecordExistsIndicator();
			this.RegisterEvents();
			this.fadeGroup.Show();
		}

		// Token: 0x06001785 RID: 6021 RVA: 0x000563FC File Offset: 0x000545FC
		protected override void OnClose()
		{
			this.UnregisterEvents();
			base.OnClose();
			this.fadeGroup.Hide();
			this.detailsFadeGroup.Hide();
			if (this.KeySlot != null && this.KeySlot.Content != null)
			{
				Item content = this.KeySlot.Content;
				content.Detach();
				ItemUtilities.SendToPlayerCharacterInventory(content, false);
			}
		}

		// Token: 0x06001786 RID: 6022 RVA: 0x0005645E File Offset: 0x0005465E
		private void RegisterEvents()
		{
			this.KeySlot.onSlotContentChanged += this.OnSlotContentChanged;
			ItemUIUtilities.OnSelectionChanged += this.OnItemSelectionChanged;
		}

		// Token: 0x06001787 RID: 6023 RVA: 0x00056488 File Offset: 0x00054688
		private void UnregisterEvents()
		{
			this.KeySlot.onSlotContentChanged -= this.OnSlotContentChanged;
			ItemUIUtilities.OnSelectionChanged -= this.OnItemSelectionChanged;
		}

		// Token: 0x06001788 RID: 6024 RVA: 0x000564B2 File Offset: 0x000546B2
		private void OnSlotContentChanged(Slot slot)
		{
			this.RefreshRecordExistsIndicator();
			this.HideSuccessIndication();
			if (((slot != null) ? slot.Content : null) != null)
			{
				AudioManager.PlayPutItemSFX(slot.Content, false);
			}
		}

		// Token: 0x06001789 RID: 6025 RVA: 0x000564E0 File Offset: 0x000546E0
		private void RefreshRecordExistsIndicator()
		{
			Item content = this.KeySlot.Content;
			if (content == null)
			{
				this.recordExistsIndicator.SetActive(false);
				return;
			}
			bool flag = MasterKeysManager.IsActive(content.TypeID);
			this.recordExistsIndicator.SetActive(flag);
		}

		// Token: 0x0600178A RID: 6026 RVA: 0x00056527 File Offset: 0x00054727
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

		// Token: 0x0600178B RID: 6027 RVA: 0x0005655D File Offset: 0x0005475D
		public static void Show()
		{
			if (MasterKeysRegisterView.Instance == null)
			{
				return;
			}
			MasterKeysRegisterView.Instance.Open(null);
		}

		// Token: 0x04001129 RID: 4393
		[SerializeField]
		private FadeGroup fadeGroup;

		// Token: 0x0400112A RID: 4394
		[SerializeField]
		private InventoryDisplay inventoryDisplay;

		// Token: 0x0400112B RID: 4395
		[SerializeField]
		private InventoryDisplay playerStorageInventoryDisplay;

		// Token: 0x0400112C RID: 4396
		[SerializeField]
		private ItemDetailsDisplay detailsDisplay;

		// Token: 0x0400112D RID: 4397
		[SerializeField]
		private FadeGroup detailsFadeGroup;

		// Token: 0x0400112E RID: 4398
		[SerializeField]
		private Button submitButton;

		// Token: 0x0400112F RID: 4399
		[SerializeField]
		private Item keySlotItem;

		// Token: 0x04001130 RID: 4400
		[SerializeField]
		private string keySlotKey = "Key";

		// Token: 0x04001131 RID: 4401
		[SerializeField]
		private SlotDisplay registerSlotDisplay;

		// Token: 0x04001132 RID: 4402
		[SerializeField]
		private GameObject recordExistsIndicator;

		// Token: 0x04001133 RID: 4403
		[SerializeField]
		private FadeGroup succeedIndicator;

		// Token: 0x04001134 RID: 4404
		[SerializeField]
		private float successIndicationTime = 1.5f;

		// Token: 0x04001135 RID: 4405
		private string sfx_Register = "UI/register";
	}
}
