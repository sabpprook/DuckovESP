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
	// Token: 0x02000309 RID: 777
	public class SupplyPanel_Entry : MonoBehaviour, IPoolable
	{
		// Token: 0x170004A2 RID: 1186
		// (get) Token: 0x06001976 RID: 6518 RVA: 0x0005C2FE File Offset: 0x0005A4FE
		// (set) Token: 0x06001977 RID: 6519 RVA: 0x0005C306 File Offset: 0x0005A506
		public BlackMarket.DemandSupplyEntry Target { get; private set; }

		// Token: 0x140000A4 RID: 164
		// (add) Token: 0x06001978 RID: 6520 RVA: 0x0005C310 File Offset: 0x0005A510
		// (remove) Token: 0x06001979 RID: 6521 RVA: 0x0005C348 File Offset: 0x0005A548
		public event Action<SupplyPanel_Entry> onDealButtonClicked;

		// Token: 0x170004A3 RID: 1187
		// (get) Token: 0x0600197A RID: 6522 RVA: 0x0005C37D File Offset: 0x0005A57D
		private string TitleFormatKey
		{
			get
			{
				if (this.Target == null)
				{
					return "?";
				}
				if (this.Target.priceFactor <= 0.9f)
				{
					return this.titleFormatKey_Low;
				}
				return this.titleFormatKey_Normal;
			}
		}

		// Token: 0x170004A4 RID: 1188
		// (get) Token: 0x0600197B RID: 6523 RVA: 0x0005C3AC File Offset: 0x0005A5AC
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

		// Token: 0x0600197C RID: 6524 RVA: 0x0005C3DC File Offset: 0x0005A5DC
		private bool CanInteract()
		{
			return this.Target != null && this.Target.remaining > 0 && this.Target.BuyCost.Enough;
		}

		// Token: 0x0600197D RID: 6525 RVA: 0x0005C416 File Offset: 0x0005A616
		public void NotifyPooled()
		{
		}

		// Token: 0x0600197E RID: 6526 RVA: 0x0005C418 File Offset: 0x0005A618
		public void NotifyReleased()
		{
			if (this.Target != null)
			{
				this.Target.onChanged -= this.OnChanged;
			}
		}

		// Token: 0x0600197F RID: 6527 RVA: 0x0005C439 File Offset: 0x0005A639
		private void OnChanged(BlackMarket.DemandSupplyEntry entry)
		{
			this.Refresh();
		}

		// Token: 0x06001980 RID: 6528 RVA: 0x0005C444 File Offset: 0x0005A644
		internal void Setup(BlackMarket.DemandSupplyEntry target)
		{
			if (target == null)
			{
				Debug.LogError("找不到对象", base.gameObject);
				return;
			}
			this.Target = target;
			this.costDisplay.Setup(target.BuyCost, 1);
			this.resultDisplay.Setup(target.ItemID, (long)target.ItemMetaData.defaultStackCount);
			this.titleDisplay.text = this.TitleText;
			this.Refresh();
			target.onChanged += this.OnChanged;
		}

		// Token: 0x06001981 RID: 6529 RVA: 0x0005C4C4 File Offset: 0x0005A6C4
		private void OnEnable()
		{
			ItemUtilities.OnPlayerItemOperation += this.Refresh;
		}

		// Token: 0x06001982 RID: 6530 RVA: 0x0005C4D7 File Offset: 0x0005A6D7
		private void OnDisable()
		{
			ItemUtilities.OnPlayerItemOperation -= this.Refresh;
		}

		// Token: 0x06001983 RID: 6531 RVA: 0x0005C4EA File Offset: 0x0005A6EA
		private void Awake()
		{
			this.dealButton.onClick.AddListener(new UnityAction(this.OnDealButtonClicked));
		}

		// Token: 0x06001984 RID: 6532 RVA: 0x0005C508 File Offset: 0x0005A708
		private void OnDealButtonClicked()
		{
			Action<SupplyPanel_Entry> action = this.onDealButtonClicked;
			if (action == null)
			{
				return;
			}
			action(this);
		}

		// Token: 0x06001985 RID: 6533 RVA: 0x0005C51C File Offset: 0x0005A71C
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

		// Token: 0x04001271 RID: 4721
		[SerializeField]
		private TextMeshProUGUI titleDisplay;

		// Token: 0x04001272 RID: 4722
		[SerializeField]
		private CostDisplay costDisplay;

		// Token: 0x04001273 RID: 4723
		[SerializeField]
		private ItemAmountDisplay resultDisplay;

		// Token: 0x04001274 RID: 4724
		[SerializeField]
		private GameObject remainingInfoContainer;

		// Token: 0x04001275 RID: 4725
		[SerializeField]
		private TextMeshProUGUI remainingAmountDisplay;

		// Token: 0x04001276 RID: 4726
		[SerializeField]
		private GameObject canInteractIndicator;

		// Token: 0x04001277 RID: 4727
		[SerializeField]
		private GameObject outOfStockIndicator;

		// Token: 0x04001278 RID: 4728
		[SerializeField]
		[LocalizationKey("UIText")]
		private string titleFormatKey_Normal = "BlackMarket_Supply_Title_Normal";

		// Token: 0x04001279 RID: 4729
		[SerializeField]
		[LocalizationKey("UIText")]
		private string titleFormatKey_Low = "BlackMarket_Supply_Title_Low";

		// Token: 0x0400127A RID: 4730
		[SerializeField]
		private Button dealButton;
	}
}
