using System;
using System.Runtime.CompilerServices;
using Cysharp.Threading.Tasks;
using Duckov.UI;
using Duckov.UI.Animations;
using Duckov.Utilities;
using ItemStatsSystem;
using SodaCraft.Localizations;
using SodaCraft.StringUtilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Duckov.Economy.UI
{
	// Token: 0x02000325 RID: 805
	public class StockShopView : View, ISingleSelectionMenu<StockShopItemEntry>
	{
		// Token: 0x170004F4 RID: 1268
		// (get) Token: 0x06001B0E RID: 6926 RVA: 0x00061DAB File Offset: 0x0005FFAB
		public static StockShopView Instance
		{
			get
			{
				return View.GetViewInstance<StockShopView>();
			}
		}

		// Token: 0x170004F5 RID: 1269
		// (get) Token: 0x06001B0F RID: 6927 RVA: 0x00061DB2 File Offset: 0x0005FFB2
		private string TextBuy
		{
			get
			{
				return this.textBuy.ToPlainText();
			}
		}

		// Token: 0x170004F6 RID: 1270
		// (get) Token: 0x06001B10 RID: 6928 RVA: 0x00061DBF File Offset: 0x0005FFBF
		private string TextSoldOut
		{
			get
			{
				return this.textSoldOut.ToPlainText();
			}
		}

		// Token: 0x170004F7 RID: 1271
		// (get) Token: 0x06001B11 RID: 6929 RVA: 0x00061DCC File Offset: 0x0005FFCC
		private string TextSell
		{
			get
			{
				return this.textSell.ToPlainText();
			}
		}

		// Token: 0x170004F8 RID: 1272
		// (get) Token: 0x06001B12 RID: 6930 RVA: 0x00061DD9 File Offset: 0x0005FFD9
		private string TextUnlock
		{
			get
			{
				return this.textUnlock.ToPlainText();
			}
		}

		// Token: 0x170004F9 RID: 1273
		// (get) Token: 0x06001B13 RID: 6931 RVA: 0x00061DE6 File Offset: 0x0005FFE6
		private string TextLocked
		{
			get
			{
				return this.textLocked.ToPlainText();
			}
		}

		// Token: 0x170004FA RID: 1274
		// (get) Token: 0x06001B14 RID: 6932 RVA: 0x00061DF4 File Offset: 0x0005FFF4
		private PrefabPool<StockShopItemEntry> EntryPool
		{
			get
			{
				if (this._entryPool == null)
				{
					this._entryPool = new PrefabPool<StockShopItemEntry>(this.entryTemplate, this.entryTemplate.transform.parent, null, null, null, true, 10, 10000, null);
					this.entryTemplate.gameObject.SetActive(false);
				}
				return this._entryPool;
			}
		}

		// Token: 0x170004FB RID: 1275
		// (get) Token: 0x06001B15 RID: 6933 RVA: 0x00061E4D File Offset: 0x0006004D
		private global::UnityEngine.Object Selection
		{
			get
			{
				if (ItemUIUtilities.SelectedItemDisplay != null)
				{
					return ItemUIUtilities.SelectedItemDisplay;
				}
				if (this.selectedItem != null)
				{
					return this.selectedItem;
				}
				return null;
			}
		}

		// Token: 0x170004FC RID: 1276
		// (get) Token: 0x06001B16 RID: 6934 RVA: 0x00061E78 File Offset: 0x00060078
		public StockShop Target
		{
			get
			{
				return this.target;
			}
		}

		// Token: 0x06001B17 RID: 6935 RVA: 0x00061E80 File Offset: 0x00060080
		protected override void Awake()
		{
			base.Awake();
			this.interactionButton.onClick.AddListener(new UnityAction(this.OnInteractionButtonClicked));
			UIInputManager.OnFastPick += this.OnFastPick;
		}

		// Token: 0x06001B18 RID: 6936 RVA: 0x00061EB5 File Offset: 0x000600B5
		protected override void OnDestroy()
		{
			base.OnDestroy();
			UIInputManager.OnFastPick -= this.OnFastPick;
		}

		// Token: 0x06001B19 RID: 6937 RVA: 0x00061ECE File Offset: 0x000600CE
		private void OnFastPick(UIInputEventData data)
		{
			if (!base.isActiveAndEnabled)
			{
				return;
			}
			this.OnInteractionButtonClicked();
		}

		// Token: 0x06001B1A RID: 6938 RVA: 0x00061EDF File Offset: 0x000600DF
		private void FixedUpdate()
		{
			this.RefreshCountDown();
		}

		// Token: 0x06001B1B RID: 6939 RVA: 0x00061EE8 File Offset: 0x000600E8
		private void RefreshCountDown()
		{
			if (this.target == null)
			{
				this.refreshCountDown.text = "-";
			}
			TimeSpan nextRefreshETA = this.target.NextRefreshETA;
			int days = nextRefreshETA.Days;
			int hours = nextRefreshETA.Hours;
			int minutes = nextRefreshETA.Minutes;
			int seconds = nextRefreshETA.Seconds;
			this.refreshCountDown.text = string.Format("{0}{1:00}:{2:00}:{3:00}", new object[]
			{
				(days > 0) ? (days.ToString() + " - ") : "",
				hours,
				minutes,
				seconds
			});
		}

		// Token: 0x06001B1C RID: 6940 RVA: 0x00061F98 File Offset: 0x00060198
		private void OnInteractionButtonClicked()
		{
			if (this.Selection == null)
			{
				return;
			}
			ItemDisplay itemDisplay = this.Selection as ItemDisplay;
			if (itemDisplay != null)
			{
				this.Target.Sell(itemDisplay.Target).Forget();
				AudioManager.Post(this.sfx_Sell);
				ItemUIUtilities.Select(null);
				this.OnSelectionChanged();
				return;
			}
			StockShopItemEntry stockShopItemEntry = this.Selection as StockShopItemEntry;
			if (stockShopItemEntry != null)
			{
				int itemTypeID = stockShopItemEntry.Target.ItemTypeID;
				if (stockShopItemEntry.IsUnlocked())
				{
					this.BuyTask(itemTypeID).Forget();
					return;
				}
				if (EconomyManager.IsWaitingForUnlockConfirm(itemTypeID))
				{
					EconomyManager.ConfirmUnlock(itemTypeID);
				}
			}
		}

		// Token: 0x06001B1D RID: 6941 RVA: 0x00062030 File Offset: 0x00060230
		private async UniTask BuyTask(int itemTypeID)
		{
			UniTask<bool>.Awaiter awaiter = this.Target.Buy(itemTypeID, 1).GetAwaiter();
			if (!awaiter.IsCompleted)
			{
				await awaiter;
				UniTask<bool>.Awaiter awaiter2;
				awaiter = awaiter2;
				awaiter2 = default(UniTask<bool>.Awaiter);
			}
			if (awaiter.GetResult())
			{
				AudioManager.Post(this.sfx_Buy);
				this.clickBlockerFadeGroup.SkipShow();
				await UniTask.NextFrame();
				await this.clickBlockerFadeGroup.HideAndReturnTask();
			}
		}

		// Token: 0x06001B1E RID: 6942 RVA: 0x0006207C File Offset: 0x0006027C
		private void OnEnable()
		{
			ItemUIUtilities.OnSelectionChanged += this.OnItemUIUtilitiesSelectionChanged;
			EconomyManager.OnItemUnlockStateChanged += this.OnItemUnlockStateChanged;
			StockShop.OnAfterItemSold += this.OnAfterItemSold;
			UIInputManager.OnNextPage += this.OnNextPage;
			UIInputManager.OnPreviousPage += this.OnPreviousPage;
		}

		// Token: 0x06001B1F RID: 6943 RVA: 0x000620E0 File Offset: 0x000602E0
		private void OnDisable()
		{
			ItemUIUtilities.OnSelectionChanged -= this.OnItemUIUtilitiesSelectionChanged;
			EconomyManager.OnItemUnlockStateChanged -= this.OnItemUnlockStateChanged;
			StockShop.OnAfterItemSold -= this.OnAfterItemSold;
			UIInputManager.OnNextPage -= this.OnNextPage;
			UIInputManager.OnPreviousPage -= this.OnPreviousPage;
		}

		// Token: 0x06001B20 RID: 6944 RVA: 0x00062142 File Offset: 0x00060342
		private void OnNextPage(UIInputEventData data)
		{
			this.playerStorageDisplay.NextPage();
		}

		// Token: 0x06001B21 RID: 6945 RVA: 0x0006214F File Offset: 0x0006034F
		private void OnPreviousPage(UIInputEventData data)
		{
			this.playerStorageDisplay.PreviousPage();
		}

		// Token: 0x06001B22 RID: 6946 RVA: 0x0006215C File Offset: 0x0006035C
		private void OnAfterItemSold(StockShop shop)
		{
			this.RefreshInteractionButton();
			this.RefreshStockText();
		}

		// Token: 0x06001B23 RID: 6947 RVA: 0x0006216A File Offset: 0x0006036A
		private void OnItemUnlockStateChanged(int itemTypeID)
		{
			if (this.details.Target == null)
			{
				return;
			}
			if (itemTypeID == this.details.Target.TypeID)
			{
				this.RefreshInteractionButton();
				this.RefreshStockText();
			}
		}

		// Token: 0x06001B24 RID: 6948 RVA: 0x0006219F File Offset: 0x0006039F
		private void OnItemUIUtilitiesSelectionChanged()
		{
			if (this.selectedItem != null && ItemUIUtilities.SelectedItemDisplay != null)
			{
				this.selectedItem = null;
			}
			this.OnSelectionChanged();
		}

		// Token: 0x06001B25 RID: 6949 RVA: 0x000621CC File Offset: 0x000603CC
		private void OnSelectionChanged()
		{
			Action action = this.onSelectionChanged;
			if (action != null)
			{
				action();
			}
			if (this.Selection == null)
			{
				this.detailsFadeGroup.Hide();
				return;
			}
			Item item = null;
			StockShopItemEntry stockShopItemEntry = this.Selection as StockShopItemEntry;
			if (stockShopItemEntry != null)
			{
				item = stockShopItemEntry.GetItem();
			}
			else
			{
				ItemDisplay itemDisplay = this.Selection as ItemDisplay;
				if (itemDisplay != null)
				{
					item = itemDisplay.Target;
				}
			}
			if (item == null)
			{
				this.detailsFadeGroup.Hide();
				return;
			}
			this.details.Setup(item);
			this.RefreshStockText();
			this.RefreshInteractionButton();
			this.RefreshCountDown();
			this.detailsFadeGroup.Show();
		}

		// Token: 0x06001B26 RID: 6950 RVA: 0x00062274 File Offset: 0x00060474
		private void RefreshStockText()
		{
			StockShopItemEntry stockShopItemEntry = this.Selection as StockShopItemEntry;
			if (stockShopItemEntry != null)
			{
				this.stockText.gameObject.SetActive(true);
				this.stockText.text = this.stockTextFormat.Format(new
				{
					text = this.stockTextKey.ToPlainText(),
					current = stockShopItemEntry.Target.CurrentStock,
					max = stockShopItemEntry.Target.MaxStock
				});
				return;
			}
			if (this.Selection is ItemDisplay)
			{
				this.stockText.gameObject.SetActive(false);
			}
		}

		// Token: 0x06001B27 RID: 6951 RVA: 0x000622FC File Offset: 0x000604FC
		public StockShopItemEntry GetSelection()
		{
			return this.Selection as StockShopItemEntry;
		}

		// Token: 0x06001B28 RID: 6952 RVA: 0x00062309 File Offset: 0x00060509
		public bool SetSelection(StockShopItemEntry selection)
		{
			if (ItemUIUtilities.SelectedItem != null)
			{
				ItemUIUtilities.Select(null);
			}
			this.selectedItem = selection;
			this.OnSelectionChanged();
			return true;
		}

		// Token: 0x06001B29 RID: 6953 RVA: 0x0006232C File Offset: 0x0006052C
		internal void Setup(StockShop target)
		{
			this.target = target;
			this.detailsFadeGroup.SkipHide();
			this.merchantNameText.text = target.DisplayName;
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
			this.playerInventoryDisplay.Setup(inventory2, null, (Item e) => e == null || e.CanBeSold, false, null);
			if (PetProxy.PetInventory != null)
			{
				this.petInventoryDisplay.Setup(PetProxy.PetInventory, null, (Item e) => e == null || e.CanBeSold, false, null);
				this.petInventoryDisplay.gameObject.SetActive(true);
			}
			else
			{
				this.petInventoryDisplay.gameObject.SetActive(false);
			}
			Inventory inventory3 = PlayerStorage.Inventory;
			if (inventory3 != null)
			{
				this.playerStorageDisplay.gameObject.SetActive(true);
				this.playerStorageDisplay.Setup(inventory3, null, (Item e) => e == null || e.CanBeSold, false, null);
			}
			else
			{
				this.playerStorageDisplay.gameObject.SetActive(false);
			}
			this.EntryPool.ReleaseAll();
			Transform parent = this.entryTemplate.transform.parent;
			foreach (StockShop.Entry entry in target.entries)
			{
				if (entry.Show)
				{
					StockShopItemEntry stockShopItemEntry = this.EntryPool.Get(parent);
					stockShopItemEntry.Setup(this, entry);
					stockShopItemEntry.transform.SetAsLastSibling();
				}
			}
			TradingUIUtilities.ActiveMerchant = target;
		}

		// Token: 0x06001B2A RID: 6954 RVA: 0x00062504 File Offset: 0x00060704
		private void RefreshInteractionButton()
		{
			this.cannotSellIndicator.SetActive(false);
			this.cashOnlyIndicator.SetActive(!this.Target.AccountAvaliable);
			ItemDisplay itemDisplay = this.Selection as ItemDisplay;
			if (itemDisplay != null)
			{
				bool canBeSold = itemDisplay.Target.CanBeSold;
				this.interactionButton.interactable = canBeSold;
				this.priceDisplay.gameObject.SetActive(true);
				this.lockDisplay.gameObject.SetActive(false);
				this.interactionText.text = this.TextSell;
				this.interactionButtonImage.color = this.buttonColor_Interactable;
				this.priceText.text = this.<RefreshInteractionButton>g__GetPriceText|71_1(itemDisplay.Target, true);
				this.cannotSellIndicator.SetActive(!itemDisplay.Target.CanBeSold);
				return;
			}
			StockShopItemEntry stockShopItemEntry = this.Selection as StockShopItemEntry;
			if (stockShopItemEntry != null)
			{
				bool flag = stockShopItemEntry.IsUnlocked();
				bool flag2 = EconomyManager.IsWaitingForUnlockConfirm(stockShopItemEntry.Target.ItemTypeID);
				this.interactionButton.interactable = flag || flag2;
				this.priceDisplay.gameObject.SetActive(flag);
				this.lockDisplay.gameObject.SetActive(!flag);
				this.cannotSellIndicator.SetActive(false);
				if (flag)
				{
					Item item = stockShopItemEntry.GetItem();
					int num = this.<RefreshInteractionButton>g__GetPrice|71_0(item, false);
					bool enough = new Cost((long)num).Enough;
					this.priceText.text = num.ToString("n0");
					if (stockShopItemEntry.Target.CurrentStock > 0)
					{
						this.interactionText.text = this.TextBuy;
						this.interactionButtonImage.color = (enough ? this.buttonColor_Interactable : this.buttonColor_NotInteractable);
						return;
					}
					this.interactionButton.interactable = false;
					this.interactionText.text = this.TextSoldOut;
					this.interactionButtonImage.color = this.buttonColor_NotInteractable;
					return;
				}
				else
				{
					if (flag2)
					{
						this.interactionText.text = this.TextUnlock;
						this.interactionButtonImage.color = this.buttonColor_Interactable;
						return;
					}
					this.interactionText.text = this.TextLocked;
					this.interactionButtonImage.color = this.buttonColor_NotInteractable;
				}
			}
		}

		// Token: 0x06001B2B RID: 6955 RVA: 0x00062739 File Offset: 0x00060939
		protected override void OnOpen()
		{
			base.OnOpen();
			this.fadeGroup.Show();
		}

		// Token: 0x06001B2C RID: 6956 RVA: 0x0006274C File Offset: 0x0006094C
		protected override void OnClose()
		{
			base.OnClose();
			this.fadeGroup.Hide();
		}

		// Token: 0x06001B2D RID: 6957 RVA: 0x0006275F File Offset: 0x0006095F
		internal void SetupAndShow(StockShop stockShop)
		{
			ItemUIUtilities.Select(null);
			this.SetSelection(null);
			this.Setup(stockShop);
			base.Open(null);
		}

		// Token: 0x06001B2F RID: 6959 RVA: 0x000627F6 File Offset: 0x000609F6
		[CompilerGenerated]
		private int <RefreshInteractionButton>g__GetPrice|71_0(Item item, bool selling)
		{
			return this.Target.ConvertPrice(item, selling);
		}

		// Token: 0x06001B30 RID: 6960 RVA: 0x00062808 File Offset: 0x00060A08
		[CompilerGenerated]
		private string <RefreshInteractionButton>g__GetPriceText|71_1(Item item, bool selling)
		{
			return this.<RefreshInteractionButton>g__GetPrice|71_0(item, selling).ToString("n0");
		}

		// Token: 0x04001338 RID: 4920
		[SerializeField]
		private FadeGroup fadeGroup;

		// Token: 0x04001339 RID: 4921
		[SerializeField]
		private FadeGroup detailsFadeGroup;

		// Token: 0x0400133A RID: 4922
		[SerializeField]
		private ItemDetailsDisplay details;

		// Token: 0x0400133B RID: 4923
		[SerializeField]
		private InventoryDisplay playerInventoryDisplay;

		// Token: 0x0400133C RID: 4924
		[SerializeField]
		private InventoryDisplay petInventoryDisplay;

		// Token: 0x0400133D RID: 4925
		[SerializeField]
		private InventoryDisplay playerStorageDisplay;

		// Token: 0x0400133E RID: 4926
		[SerializeField]
		private StockShopItemEntry entryTemplate;

		// Token: 0x0400133F RID: 4927
		[SerializeField]
		private TextMeshProUGUI stockText;

		// Token: 0x04001340 RID: 4928
		[SerializeField]
		[LocalizationKey("Default")]
		private string stockTextKey = "UI_Stock";

		// Token: 0x04001341 RID: 4929
		[SerializeField]
		private string stockTextFormat = "{text} {current}/{max}";

		// Token: 0x04001342 RID: 4930
		[SerializeField]
		private TextMeshProUGUI merchantNameText;

		// Token: 0x04001343 RID: 4931
		[SerializeField]
		private Button interactionButton;

		// Token: 0x04001344 RID: 4932
		[SerializeField]
		private Image interactionButtonImage;

		// Token: 0x04001345 RID: 4933
		[SerializeField]
		private Color buttonColor_Interactable;

		// Token: 0x04001346 RID: 4934
		[SerializeField]
		private Color buttonColor_NotInteractable;

		// Token: 0x04001347 RID: 4935
		[SerializeField]
		private TextMeshProUGUI interactionText;

		// Token: 0x04001348 RID: 4936
		[SerializeField]
		private GameObject cashOnlyIndicator;

		// Token: 0x04001349 RID: 4937
		[SerializeField]
		private GameObject cannotSellIndicator;

		// Token: 0x0400134A RID: 4938
		[LocalizationKey("Default")]
		[SerializeField]
		private string textBuy = "购买";

		// Token: 0x0400134B RID: 4939
		[LocalizationKey("Default")]
		[SerializeField]
		private string textSoldOut = "已售罄";

		// Token: 0x0400134C RID: 4940
		[LocalizationKey("Default")]
		[SerializeField]
		private string textSell = "出售";

		// Token: 0x0400134D RID: 4941
		[LocalizationKey("Default")]
		[SerializeField]
		private string textUnlock = "解锁";

		// Token: 0x0400134E RID: 4942
		[LocalizationKey("Default")]
		[SerializeField]
		private string textLocked = "已锁定";

		// Token: 0x0400134F RID: 4943
		[SerializeField]
		private GameObject priceDisplay;

		// Token: 0x04001350 RID: 4944
		[SerializeField]
		private TextMeshProUGUI priceText;

		// Token: 0x04001351 RID: 4945
		[SerializeField]
		private GameObject lockDisplay;

		// Token: 0x04001352 RID: 4946
		[SerializeField]
		private FadeGroup clickBlockerFadeGroup;

		// Token: 0x04001353 RID: 4947
		[SerializeField]
		private TextMeshProUGUI refreshCountDown;

		// Token: 0x04001354 RID: 4948
		private string sfx_Buy = "UI/buy";

		// Token: 0x04001355 RID: 4949
		private string sfx_Sell = "UI/sell";

		// Token: 0x04001356 RID: 4950
		private PrefabPool<StockShopItemEntry> _entryPool;

		// Token: 0x04001357 RID: 4951
		private StockShop target;

		// Token: 0x04001358 RID: 4952
		private StockShopItemEntry selectedItem;

		// Token: 0x04001359 RID: 4953
		public Action onSelectionChanged;
	}
}
