using System;
using Duckov.MiniGames.GoldMiner.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Duckov.MiniGames.GoldMiner
{
	// Token: 0x0200029D RID: 669
	public class GoldMinerShopUIEntry : MonoBehaviour
	{
		// Token: 0x060015C5 RID: 5573 RVA: 0x000508C8 File Offset: 0x0004EAC8
		private void Awake()
		{
			if (!this.navEntry)
			{
				this.navEntry = base.GetComponent<NavEntry>();
			}
			NavEntry navEntry = this.navEntry;
			navEntry.onInteract = (Action<NavEntry>)Delegate.Combine(navEntry.onInteract, new Action<NavEntry>(this.OnInteract));
			this.VCT = base.GetComponent<VirtualCursorTarget>();
			if (this.VCT)
			{
				this.VCT.onEnter.AddListener(new UnityAction(this.OnVCTEnter));
			}
		}

		// Token: 0x060015C6 RID: 5574 RVA: 0x0005094A File Offset: 0x0004EB4A
		private void OnVCTEnter()
		{
			this.master.hoveringEntry = this;
		}

		// Token: 0x060015C7 RID: 5575 RVA: 0x00050958 File Offset: 0x0004EB58
		private void OnInteract(NavEntry entry)
		{
			this.master.target.Buy(this.target);
		}

		// Token: 0x060015C8 RID: 5576 RVA: 0x00050974 File Offset: 0x0004EB74
		internal void Setup(GoldMinerShopUI master, ShopEntity target)
		{
			this.master = master;
			this.target = target;
			if (target == null || target.artifact == null)
			{
				this.SetupEmpty();
				return;
			}
			this.mainLayout.SetActive(true);
			this.nameText.text = target.artifact.DisplayName;
			this.icon.sprite = target.artifact.Icon;
			this.Refresh();
		}

		// Token: 0x060015C9 RID: 5577 RVA: 0x000509E8 File Offset: 0x0004EBE8
		private void Refresh()
		{
			bool flag;
			int num = this.master.target.CalculateDealPrice(this.target, out flag);
			this.priceText.text = num.ToString(this.priceFormat);
			this.priceIndicator.SetActive(num > 0);
			this.freeIndicator.SetActive(num <= 0);
			this.soldIndicator.SetActive(this.target.sold);
		}

		// Token: 0x060015CA RID: 5578 RVA: 0x00050A5D File Offset: 0x0004EC5D
		private void SetupEmpty()
		{
			this.mainLayout.SetActive(false);
		}

		// Token: 0x0400101E RID: 4126
		[SerializeField]
		private NavEntry navEntry;

		// Token: 0x0400101F RID: 4127
		[SerializeField]
		private VirtualCursorTarget VCT;

		// Token: 0x04001020 RID: 4128
		[SerializeField]
		private GameObject mainLayout;

		// Token: 0x04001021 RID: 4129
		[SerializeField]
		private TextMeshProUGUI nameText;

		// Token: 0x04001022 RID: 4130
		[SerializeField]
		private TextMeshProUGUI priceText;

		// Token: 0x04001023 RID: 4131
		[SerializeField]
		private string priceFormat = "0";

		// Token: 0x04001024 RID: 4132
		[SerializeField]
		private GameObject priceIndicator;

		// Token: 0x04001025 RID: 4133
		[SerializeField]
		private GameObject freeIndicator;

		// Token: 0x04001026 RID: 4134
		[SerializeField]
		private Image icon;

		// Token: 0x04001027 RID: 4135
		[SerializeField]
		private GameObject soldIndicator;

		// Token: 0x04001028 RID: 4136
		private GoldMinerShopUI master;

		// Token: 0x04001029 RID: 4137
		public ShopEntity target;
	}
}
