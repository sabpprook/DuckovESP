using System;
using Duckov.MiniGames.GoldMiner.UI;
using TMPro;
using UnityEngine;

namespace Duckov.MiniGames.GoldMiner
{
	// Token: 0x0200029E RID: 670
	public class GoldMinerShopUIRefreshBtn : MonoBehaviour
	{
		// Token: 0x060015CC RID: 5580 RVA: 0x00050A80 File Offset: 0x0004EC80
		private void Awake()
		{
			if (!this.navEntry)
			{
				this.navEntry = base.GetComponent<NavEntry>();
			}
			NavEntry navEntry = this.navEntry;
			navEntry.onInteract = (Action<NavEntry>)Delegate.Combine(navEntry.onInteract, new Action<NavEntry>(this.OnInteract));
			GoldMinerShop goldMinerShop = this.shop;
			goldMinerShop.onAfterOperation = (Action)Delegate.Combine(goldMinerShop.onAfterOperation, new Action(this.OnAfterOperation));
		}

		// Token: 0x060015CD RID: 5581 RVA: 0x00050AF4 File Offset: 0x0004ECF4
		private void OnEnable()
		{
			this.RefreshCostText();
		}

		// Token: 0x060015CE RID: 5582 RVA: 0x00050AFC File Offset: 0x0004ECFC
		private void OnAfterOperation()
		{
			this.RefreshCostText();
		}

		// Token: 0x060015CF RID: 5583 RVA: 0x00050B04 File Offset: 0x0004ED04
		private void RefreshCostText()
		{
			this.costText.text = string.Format("${0}", this.shop.GetRefreshCost());
			this.refreshChanceText.text = string.Format("{0}", this.shop.refreshChance);
			this.noChanceIndicator.SetActive(this.shop.refreshChance < 1);
		}

		// Token: 0x060015D0 RID: 5584 RVA: 0x00050B74 File Offset: 0x0004ED74
		private void OnInteract(NavEntry entry)
		{
			this.shop.TryRefresh();
		}

		// Token: 0x0400102A RID: 4138
		[SerializeField]
		private GoldMinerShop shop;

		// Token: 0x0400102B RID: 4139
		[SerializeField]
		private NavEntry navEntry;

		// Token: 0x0400102C RID: 4140
		[SerializeField]
		private TextMeshProUGUI costText;

		// Token: 0x0400102D RID: 4141
		[SerializeField]
		private TextMeshProUGUI refreshChanceText;

		// Token: 0x0400102E RID: 4142
		[SerializeField]
		private GameObject noChanceIndicator;
	}
}
