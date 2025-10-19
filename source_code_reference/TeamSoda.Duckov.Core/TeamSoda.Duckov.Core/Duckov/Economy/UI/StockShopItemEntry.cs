using System;
using DG.Tweening;
using Duckov.UI;
using ItemStatsSystem;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Duckov.Economy.UI
{
	// Token: 0x02000324 RID: 804
	public class StockShopItemEntry : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
	{
		// Token: 0x170004F2 RID: 1266
		// (get) Token: 0x06001AFC RID: 6908 RVA: 0x000619AD File Offset: 0x0005FBAD
		private StockShop stockShop
		{
			get
			{
				StockShopView stockShopView = this.master;
				if (stockShopView == null)
				{
					return null;
				}
				return stockShopView.Target;
			}
		}

		// Token: 0x170004F3 RID: 1267
		// (get) Token: 0x06001AFD RID: 6909 RVA: 0x000619C0 File Offset: 0x0005FBC0
		public StockShop.Entry Target
		{
			get
			{
				return this.target;
			}
		}

		// Token: 0x06001AFE RID: 6910 RVA: 0x000619C8 File Offset: 0x0005FBC8
		private void Awake()
		{
			this.itemDisplay.onPointerClick += this.OnItemDisplayPointerClick;
		}

		// Token: 0x06001AFF RID: 6911 RVA: 0x000619E1 File Offset: 0x0005FBE1
		private void OnItemDisplayPointerClick(ItemDisplay display, PointerEventData data)
		{
			this.OnPointerClick(data);
		}

		// Token: 0x06001B00 RID: 6912 RVA: 0x000619EA File Offset: 0x0005FBEA
		public Item GetItem()
		{
			return this.stockShop.GetItemInstanceDirect(this.target.ItemTypeID);
		}

		// Token: 0x06001B01 RID: 6913 RVA: 0x00061A04 File Offset: 0x0005FC04
		internal void Setup(StockShopView master, StockShop.Entry entry)
		{
			this.UnregisterEvents();
			this.master = master;
			this.target = entry;
			Item itemInstanceDirect = this.stockShop.GetItemInstanceDirect(this.target.ItemTypeID);
			this.itemDisplay.Setup(itemInstanceDirect);
			this.itemDisplay.ShowOperationButtons = false;
			this.itemDisplay.IsStockshopSample = true;
			int stackCount = itemInstanceDirect.StackCount;
			int num = this.stockShop.ConvertPrice(itemInstanceDirect, false);
			this.priceText.text = num.ToString(this.moneyFormat);
			this.Refresh();
			this.RegisterEvents();
		}

		// Token: 0x06001B02 RID: 6914 RVA: 0x00061A9C File Offset: 0x0005FC9C
		private void RegisterEvents()
		{
			if (this.master != null)
			{
				StockShopView stockShopView = this.master;
				stockShopView.onSelectionChanged = (Action)Delegate.Combine(stockShopView.onSelectionChanged, new Action(this.OnMasterSelectionChanged));
			}
			if (this.target != null)
			{
				this.target.onStockChanged += this.OnTargetStockChanged;
			}
		}

		// Token: 0x06001B03 RID: 6915 RVA: 0x00061B00 File Offset: 0x0005FD00
		private void UnregisterEvents()
		{
			if (this.master != null)
			{
				StockShopView stockShopView = this.master;
				stockShopView.onSelectionChanged = (Action)Delegate.Remove(stockShopView.onSelectionChanged, new Action(this.OnMasterSelectionChanged));
			}
			if (this.target != null)
			{
				this.target.onStockChanged -= this.OnTargetStockChanged;
			}
		}

		// Token: 0x06001B04 RID: 6916 RVA: 0x00061B61 File Offset: 0x0005FD61
		private void OnMasterSelectionChanged()
		{
			this.Refresh();
		}

		// Token: 0x06001B05 RID: 6917 RVA: 0x00061B69 File Offset: 0x0005FD69
		private void OnTargetStockChanged(StockShop.Entry entry)
		{
			this.Refresh();
		}

		// Token: 0x06001B06 RID: 6918 RVA: 0x00061B71 File Offset: 0x0005FD71
		public bool IsUnlocked()
		{
			return this.target != null && (this.target.ForceUnlock || EconomyManager.IsUnlocked(this.target.ItemTypeID));
		}

		// Token: 0x06001B07 RID: 6919 RVA: 0x00061B9C File Offset: 0x0005FD9C
		private void Refresh()
		{
			if (!base.gameObject.activeSelf)
			{
				return;
			}
			bool flag = this.master.GetSelection() == this;
			this.selectionIndicator.SetActive(flag);
			bool flag2 = EconomyManager.IsUnlocked(this.target.ItemTypeID);
			bool flag3 = EconomyManager.IsWaitingForUnlockConfirm(this.target.ItemTypeID);
			if (this.target.ForceUnlock)
			{
				flag2 = true;
				flag3 = false;
			}
			this.lockedIndicator.SetActive(!flag2 && !flag3);
			this.waitingForUnlockIndicator.SetActive(!flag2 && flag3);
			base.gameObject.SetActive(flag2 || flag3);
			this.outOfStockIndicator.SetActive(this.Target.CurrentStock <= 0);
		}

		// Token: 0x06001B08 RID: 6920 RVA: 0x00061C58 File Offset: 0x0005FE58
		public void OnPointerClick(PointerEventData eventData)
		{
			this.Punch();
			if (this.master == null)
			{
				return;
			}
			eventData.Use();
			if (EconomyManager.IsWaitingForUnlockConfirm(this.target.ItemTypeID))
			{
				EconomyManager.ConfirmUnlock(this.target.ItemTypeID);
			}
			if (this.master.GetSelection() == this)
			{
				this.master.SetSelection(null);
				return;
			}
			this.master.SetSelection(this);
		}

		// Token: 0x06001B09 RID: 6921 RVA: 0x00061CD0 File Offset: 0x0005FED0
		public void Punch()
		{
			this.selectionIndicator.transform.DOKill(false);
			this.selectionIndicator.transform.localScale = Vector3.one;
			this.selectionIndicator.transform.DOPunchScale(Vector3.one * this.selectionRingPunchScale, this.punchDuration, 10, 1f);
		}

		// Token: 0x06001B0A RID: 6922 RVA: 0x00061D32 File Offset: 0x0005FF32
		private void OnEnable()
		{
			EconomyManager.OnItemUnlockStateChanged += this.OnItemUnlockStateChanged;
		}

		// Token: 0x06001B0B RID: 6923 RVA: 0x00061D45 File Offset: 0x0005FF45
		private void OnDisable()
		{
			EconomyManager.OnItemUnlockStateChanged -= this.OnItemUnlockStateChanged;
		}

		// Token: 0x06001B0C RID: 6924 RVA: 0x00061D58 File Offset: 0x0005FF58
		private void OnItemUnlockStateChanged(int itemTypeID)
		{
			if (this.target == null)
			{
				return;
			}
			if (itemTypeID == this.target.ItemTypeID)
			{
				this.Refresh();
			}
		}

		// Token: 0x0400132C RID: 4908
		[SerializeField]
		private string moneyFormat = "n0";

		// Token: 0x0400132D RID: 4909
		[SerializeField]
		private ItemDisplay itemDisplay;

		// Token: 0x0400132E RID: 4910
		[SerializeField]
		private TextMeshProUGUI priceText;

		// Token: 0x0400132F RID: 4911
		[SerializeField]
		private GameObject selectionIndicator;

		// Token: 0x04001330 RID: 4912
		[SerializeField]
		private GameObject lockedIndicator;

		// Token: 0x04001331 RID: 4913
		[SerializeField]
		private GameObject waitingForUnlockIndicator;

		// Token: 0x04001332 RID: 4914
		[SerializeField]
		private GameObject outOfStockIndicator;

		// Token: 0x04001333 RID: 4915
		[SerializeField]
		[Range(0f, 1f)]
		private float punchDuration = 0.2f;

		// Token: 0x04001334 RID: 4916
		[SerializeField]
		[Range(-1f, 1f)]
		private float selectionRingPunchScale = 0.1f;

		// Token: 0x04001335 RID: 4917
		[SerializeField]
		[Range(-1f, 1f)]
		private float iconPunchScale = 0.1f;

		// Token: 0x04001336 RID: 4918
		private StockShopView master;

		// Token: 0x04001337 RID: 4919
		private StockShop.Entry target;
	}
}
