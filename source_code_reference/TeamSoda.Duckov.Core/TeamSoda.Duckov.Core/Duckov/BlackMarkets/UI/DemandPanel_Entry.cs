using System;
using Duckov.Utilities;
using SodaCraft.Localizations;
using SodaCraft.StringUtilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Duckov.BlackMarkets.UI
{
	// Token: 0x02000307 RID: 775
	public class DemandPanel_Entry : MonoBehaviour, IPoolable
	{
		// Token: 0x1700049D RID: 1181
		// (get) Token: 0x06001958 RID: 6488 RVA: 0x0005BE7E File Offset: 0x0005A07E
		// (set) Token: 0x06001959 RID: 6489 RVA: 0x0005BE86 File Offset: 0x0005A086
		public BlackMarket.DemandSupplyEntry Target { get; private set; }

		// Token: 0x140000A3 RID: 163
		// (add) Token: 0x0600195A RID: 6490 RVA: 0x0005BE90 File Offset: 0x0005A090
		// (remove) Token: 0x0600195B RID: 6491 RVA: 0x0005BEC8 File Offset: 0x0005A0C8
		public event Action<DemandPanel_Entry> onDealButtonClicked;

		// Token: 0x1700049E RID: 1182
		// (get) Token: 0x0600195C RID: 6492 RVA: 0x0005BEFD File Offset: 0x0005A0FD
		private string TitleFormatKey
		{
			get
			{
				if (this.Target == null)
				{
					return "?";
				}
				if (this.Target.priceFactor >= 1.9f)
				{
					return this.titleFormatKey_High;
				}
				return this.titleFormatKey_Normal;
			}
		}

		// Token: 0x1700049F RID: 1183
		// (get) Token: 0x0600195D RID: 6493 RVA: 0x0005BF2C File Offset: 0x0005A12C
		private string TitleText
		{
			get
			{
				if (this.Target == null)
				{
					return "?";
				}
				return this.TitleFormatKey.ToPlainText().Format(new
				{
					itemName = this.Target.ItemDisplayName
				});
			}
		}

		// Token: 0x0600195E RID: 6494 RVA: 0x0005BF5C File Offset: 0x0005A15C
		private bool CanInteract()
		{
			return this.Target != null && this.Target.remaining > 0 && this.Target.SellCost.Enough;
		}

		// Token: 0x0600195F RID: 6495 RVA: 0x0005BF96 File Offset: 0x0005A196
		public void NotifyPooled()
		{
		}

		// Token: 0x06001960 RID: 6496 RVA: 0x0005BF98 File Offset: 0x0005A198
		public void NotifyReleased()
		{
			if (this.Target != null)
			{
				this.Target.onChanged -= this.OnChanged;
			}
		}

		// Token: 0x06001961 RID: 6497 RVA: 0x0005BFB9 File Offset: 0x0005A1B9
		private void OnChanged(BlackMarket.DemandSupplyEntry entry)
		{
			this.Refresh();
		}

		// Token: 0x06001962 RID: 6498 RVA: 0x0005BFC1 File Offset: 0x0005A1C1
		public void OnDealButtonClicked()
		{
			Action<DemandPanel_Entry> action = this.onDealButtonClicked;
			if (action == null)
			{
				return;
			}
			action(this);
		}

		// Token: 0x06001963 RID: 6499 RVA: 0x0005BFD4 File Offset: 0x0005A1D4
		internal void Setup(BlackMarket.DemandSupplyEntry target)
		{
			if (target == null)
			{
				Debug.LogError("找不到对象", base.gameObject);
				return;
			}
			this.Target = target;
			this.costDisplay.Setup(target.SellCost, 1);
			this.moneyDisplay.text = string.Format("{0}", target.TotalPrice);
			this.titleDisplay.text = this.TitleText;
			this.Refresh();
			target.onChanged += this.OnChanged;
		}

		// Token: 0x06001964 RID: 6500 RVA: 0x0005C057 File Offset: 0x0005A257
		private void OnEnable()
		{
			ItemUtilities.OnPlayerItemOperation += this.Refresh;
		}

		// Token: 0x06001965 RID: 6501 RVA: 0x0005C06A File Offset: 0x0005A26A
		private void OnDisable()
		{
			ItemUtilities.OnPlayerItemOperation -= this.Refresh;
		}

		// Token: 0x06001966 RID: 6502 RVA: 0x0005C07D File Offset: 0x0005A27D
		private void Awake()
		{
			this.dealButton.onClick.AddListener(new UnityAction(this.OnDealButtonClicked));
		}

		// Token: 0x06001967 RID: 6503 RVA: 0x0005C09C File Offset: 0x0005A29C
		private void Refresh()
		{
			if (this.Target == null)
			{
				Debug.LogError("找不到对象", base.gameObject);
				return;
			}
			this.remainingAmountDisplay.text = string.Format("{0}", this.Target.Remaining);
			bool flag = this.CanInteract();
			this.canInteractIndicator.SetActive(flag);
			bool flag2 = this.Target.Remaining <= 0;
			this.outOfStockIndicator.SetActive(flag2);
			this.remainingInfoContainer.SetActive(this.Target.remaining > 1);
		}

		// Token: 0x04001262 RID: 4706
		[SerializeField]
		private TextMeshProUGUI titleDisplay;

		// Token: 0x04001263 RID: 4707
		[SerializeField]
		private CostDisplay costDisplay;

		// Token: 0x04001264 RID: 4708
		[SerializeField]
		private TextMeshProUGUI moneyDisplay;

		// Token: 0x04001265 RID: 4709
		[SerializeField]
		private GameObject remainingInfoContainer;

		// Token: 0x04001266 RID: 4710
		[SerializeField]
		private TextMeshProUGUI remainingAmountDisplay;

		// Token: 0x04001267 RID: 4711
		[SerializeField]
		private GameObject canInteractIndicator;

		// Token: 0x04001268 RID: 4712
		[SerializeField]
		private GameObject outOfStockIndicator;

		// Token: 0x04001269 RID: 4713
		[SerializeField]
		[LocalizationKey("UIText")]
		private string titleFormatKey_Normal = "BlackMarket_Demand_Title_Normal";

		// Token: 0x0400126A RID: 4714
		[SerializeField]
		[LocalizationKey("UIText")]
		private string titleFormatKey_High = "BlackMarket_Demand_Title_High";

		// Token: 0x0400126B RID: 4715
		[SerializeField]
		private Button dealButton;
	}
}
