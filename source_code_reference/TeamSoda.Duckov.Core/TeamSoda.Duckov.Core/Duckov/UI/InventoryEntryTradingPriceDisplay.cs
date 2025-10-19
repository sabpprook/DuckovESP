using System;
using ItemStatsSystem;
using TMPro;
using UnityEngine;

namespace Duckov.UI
{
	// Token: 0x0200039E RID: 926
	public class InventoryEntryTradingPriceDisplay : MonoBehaviour
	{
		// Token: 0x17000653 RID: 1619
		// (get) Token: 0x060020FA RID: 8442 RVA: 0x000730EA File Offset: 0x000712EA
		// (set) Token: 0x060020FB RID: 8443 RVA: 0x000730F2 File Offset: 0x000712F2
		public bool Selling
		{
			get
			{
				return this.selling;
			}
			set
			{
				this.selling = value;
			}
		}

		// Token: 0x060020FC RID: 8444 RVA: 0x000730FB File Offset: 0x000712FB
		private void Awake()
		{
			this.master.onRefresh += this.OnRefresh;
			TradingUIUtilities.OnActiveMerchantChanged += this.OnActiveMerchantChanged;
		}

		// Token: 0x060020FD RID: 8445 RVA: 0x00073125 File Offset: 0x00071325
		private void OnActiveMerchantChanged(IMerchant merchant)
		{
			this.Refresh();
		}

		// Token: 0x060020FE RID: 8446 RVA: 0x0007312D File Offset: 0x0007132D
		private void Start()
		{
			this.Refresh();
		}

		// Token: 0x060020FF RID: 8447 RVA: 0x00073135 File Offset: 0x00071335
		private void OnDestroy()
		{
			if (this.master != null)
			{
				this.master.onRefresh -= this.OnRefresh;
			}
			TradingUIUtilities.OnActiveMerchantChanged -= this.OnActiveMerchantChanged;
		}

		// Token: 0x06002100 RID: 8448 RVA: 0x0007316D File Offset: 0x0007136D
		private void OnRefresh(InventoryEntry entry)
		{
			this.Refresh();
		}

		// Token: 0x06002101 RID: 8449 RVA: 0x00073178 File Offset: 0x00071378
		private void Refresh()
		{
			InventoryEntry inventoryEntry = this.master;
			Item item = ((inventoryEntry != null) ? inventoryEntry.Content : null);
			if (item != null)
			{
				this.canvasGroup.alpha = 1f;
				string text = this.GetPrice(item).ToString(this.moneyFormat);
				this.priceText.text = text;
				return;
			}
			this.canvasGroup.alpha = 0f;
		}

		// Token: 0x06002102 RID: 8450 RVA: 0x000731E4 File Offset: 0x000713E4
		private int GetPrice(Item content)
		{
			if (content == null)
			{
				return 0;
			}
			int value = content.Value;
			if (TradingUIUtilities.ActiveMerchant == null)
			{
				return value;
			}
			return TradingUIUtilities.ActiveMerchant.ConvertPrice(content, this.selling);
		}

		// Token: 0x04001665 RID: 5733
		[SerializeField]
		private InventoryEntry master;

		// Token: 0x04001666 RID: 5734
		[SerializeField]
		private CanvasGroup canvasGroup;

		// Token: 0x04001667 RID: 5735
		[SerializeField]
		private TextMeshProUGUI priceText;

		// Token: 0x04001668 RID: 5736
		[SerializeField]
		private bool selling = true;

		// Token: 0x04001669 RID: 5737
		[SerializeField]
		private string moneyFormat = "n0";
	}
}
