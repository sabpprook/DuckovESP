using System;
using Duckov.UI;
using Duckov.UI.Animations;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Duckov.BlackMarkets.UI
{
	// Token: 0x02000305 RID: 773
	public class BlackMarketView : View
	{
		// Token: 0x17000496 RID: 1174
		// (get) Token: 0x06001936 RID: 6454 RVA: 0x0005B9F6 File Offset: 0x00059BF6
		public static BlackMarketView Instance
		{
			get
			{
				return View.GetViewInstance<BlackMarketView>();
			}
		}

		// Token: 0x17000497 RID: 1175
		// (get) Token: 0x06001937 RID: 6455 RVA: 0x0005B9FD File Offset: 0x00059BFD
		protected override bool ShowOpenCloseButtons
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06001938 RID: 6456 RVA: 0x0005BA00 File Offset: 0x00059C00
		protected override void Awake()
		{
			base.Awake();
			this.btn_demandPanel.onClick.AddListener(delegate
			{
				this.SetMode(BlackMarketView.Mode.Demand);
			});
			this.btn_supplyPanel.onClick.AddListener(delegate
			{
				this.SetMode(BlackMarketView.Mode.Supply);
			});
			this.btn_refresh.onClick.AddListener(new UnityAction(this.OnRefreshBtnClicked));
		}

		// Token: 0x06001939 RID: 6457 RVA: 0x0005BA67 File Offset: 0x00059C67
		private void OnEnable()
		{
			BlackMarket.onRefreshChanceChanged += this.OnRefreshChanceChanced;
		}

		// Token: 0x0600193A RID: 6458 RVA: 0x0005BA7A File Offset: 0x00059C7A
		private void OnDisable()
		{
			BlackMarket.onRefreshChanceChanged -= this.OnRefreshChanceChanced;
		}

		// Token: 0x0600193B RID: 6459 RVA: 0x0005BA8D File Offset: 0x00059C8D
		private void OnRefreshChanceChanced(BlackMarket market)
		{
			this.RefreshRefreshButton();
		}

		// Token: 0x0600193C RID: 6460 RVA: 0x0005BA98 File Offset: 0x00059C98
		private void RefreshRefreshButton()
		{
			if (this.Target == null)
			{
				this.refreshChanceText.text = "ERROR";
				this.refreshInteractableIndicator.SetActive(false);
			}
			int refreshChance = this.Target.RefreshChance;
			int maxRefreshChance = this.Target.MaxRefreshChance;
			this.refreshChanceText.text = string.Format("{0}/{1}", refreshChance, maxRefreshChance);
			this.refreshInteractableIndicator.SetActive(refreshChance > 0);
		}

		// Token: 0x0600193D RID: 6461 RVA: 0x0005BB17 File Offset: 0x00059D17
		private void OnRefreshBtnClicked()
		{
			if (this.Target == null)
			{
				return;
			}
			this.Target.PayAndRegenerate();
		}

		// Token: 0x17000498 RID: 1176
		// (get) Token: 0x0600193E RID: 6462 RVA: 0x0005BB33 File Offset: 0x00059D33
		// (set) Token: 0x0600193F RID: 6463 RVA: 0x0005BB3B File Offset: 0x00059D3B
		public BlackMarket Target { get; private set; }

		// Token: 0x17000499 RID: 1177
		// (get) Token: 0x06001940 RID: 6464 RVA: 0x0005BB44 File Offset: 0x00059D44
		private bool ShowDemand
		{
			get
			{
				return (BlackMarketView.Mode.Demand | this.mode) == this.mode;
			}
		}

		// Token: 0x1700049A RID: 1178
		// (get) Token: 0x06001941 RID: 6465 RVA: 0x0005BB56 File Offset: 0x00059D56
		private bool ShowSupply
		{
			get
			{
				return (BlackMarketView.Mode.Supply | this.mode) == this.mode;
			}
		}

		// Token: 0x06001942 RID: 6466 RVA: 0x0005BB68 File Offset: 0x00059D68
		public static void Show(BlackMarketView.Mode mode)
		{
			if (BlackMarketView.Instance == null)
			{
				return;
			}
			if (BlackMarket.Instance == null)
			{
				return;
			}
			BlackMarketView.Instance.Setup(BlackMarket.Instance, mode);
			BlackMarketView.Instance.Open(null);
		}

		// Token: 0x06001943 RID: 6467 RVA: 0x0005BBA1 File Offset: 0x00059DA1
		private void Setup(BlackMarket target, BlackMarketView.Mode mode)
		{
			this.Target = target;
			this.demandPanel.Setup(target);
			this.supplyPanel.Setup(target);
			this.RefreshRefreshButton();
			this.SetMode(mode);
			base.Open(null);
		}

		// Token: 0x06001944 RID: 6468 RVA: 0x0005BBD6 File Offset: 0x00059DD6
		private void SetMode(BlackMarketView.Mode mode)
		{
			this.mode = mode;
			this.demandPanel.gameObject.SetActive(this.ShowDemand);
			this.supplyPanel.gameObject.SetActive(this.ShowSupply);
		}

		// Token: 0x06001945 RID: 6469 RVA: 0x0005BC0B File Offset: 0x00059E0B
		protected override void OnOpen()
		{
			base.OnOpen();
			this.fadeGroup.Show();
		}

		// Token: 0x06001946 RID: 6470 RVA: 0x0005BC1E File Offset: 0x00059E1E
		protected override void OnClose()
		{
			base.OnClose();
			this.fadeGroup.Hide();
		}

		// Token: 0x06001947 RID: 6471 RVA: 0x0005BC34 File Offset: 0x00059E34
		private void Update()
		{
			if (this.Target == null)
			{
				return;
			}
			int refreshChance = this.Target.RefreshChance;
			int maxRefreshChance = this.Target.MaxRefreshChance;
			string text;
			if (refreshChance < maxRefreshChance)
			{
				TimeSpan remainingTimeBeforeRefresh = this.Target.RemainingTimeBeforeRefresh;
				text = string.Format("{0:00}:{1:00}:{2:00}", Mathf.FloorToInt((float)remainingTimeBeforeRefresh.TotalHours), remainingTimeBeforeRefresh.Minutes, remainingTimeBeforeRefresh.Seconds);
			}
			else
			{
				text = "--:--:--";
			}
			this.refreshETAText.text = text;
		}

		// Token: 0x04001254 RID: 4692
		[SerializeField]
		private FadeGroup fadeGroup;

		// Token: 0x04001255 RID: 4693
		[SerializeField]
		private DemandPanel demandPanel;

		// Token: 0x04001256 RID: 4694
		[SerializeField]
		private SupplyPanel supplyPanel;

		// Token: 0x04001257 RID: 4695
		[SerializeField]
		private TextMeshProUGUI refreshETAText;

		// Token: 0x04001258 RID: 4696
		[SerializeField]
		private TextMeshProUGUI refreshChanceText;

		// Token: 0x04001259 RID: 4697
		[SerializeField]
		private Button btn_demandPanel;

		// Token: 0x0400125A RID: 4698
		[SerializeField]
		private Button btn_supplyPanel;

		// Token: 0x0400125B RID: 4699
		[SerializeField]
		private Button btn_refresh;

		// Token: 0x0400125C RID: 4700
		[SerializeField]
		private GameObject refreshInteractableIndicator;

		// Token: 0x0400125E RID: 4702
		private BlackMarketView.Mode mode;

		// Token: 0x0200059A RID: 1434
		public enum Mode
		{
			// Token: 0x04002009 RID: 8201
			None,
			// Token: 0x0400200A RID: 8202
			Demand,
			// Token: 0x0400200B RID: 8203
			Supply
		}
	}
}
