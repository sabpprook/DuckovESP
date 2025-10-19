using System;
using Cysharp.Threading.Tasks;
using Duckov.Economy;
using Duckov.UI;
using Duckov.UI.Animations;
using SodaCraft.Localizations;
using SodaCraft.StringUtilities;
using TMPro;
using UnityEngine;

namespace Duckov.DeathLotteries
{
	// Token: 0x02000303 RID: 771
	public class DeathLotteryVIew : View
	{
		// Token: 0x17000489 RID: 1161
		// (get) Token: 0x06001905 RID: 6405 RVA: 0x0005AF79 File Offset: 0x00059179
		private string RemainingTextFormat
		{
			get
			{
				return this.remainingTextFormatKey.ToPlainText();
			}
		}

		// Token: 0x1700048A RID: 1162
		// (get) Token: 0x06001906 RID: 6406 RVA: 0x0005AF86 File Offset: 0x00059186
		public DeathLottery Target
		{
			get
			{
				return this.target;
			}
		}

		// Token: 0x1700048B RID: 1163
		// (get) Token: 0x06001907 RID: 6407 RVA: 0x0005AF8E File Offset: 0x0005918E
		public int RemainingChances
		{
			get
			{
				if (this.Target == null)
				{
					return 0;
				}
				return this.Target.RemainingChances;
			}
		}

		// Token: 0x06001908 RID: 6408 RVA: 0x0005AFAB File Offset: 0x000591AB
		protected override void OnOpen()
		{
			base.OnOpen();
			this.fadeGroup.Show();
			this.selectionBusyIndicator.SkipHide();
		}

		// Token: 0x06001909 RID: 6409 RVA: 0x0005AFC9 File Offset: 0x000591C9
		protected override void OnClose()
		{
			base.OnClose();
			this.fadeGroup.Hide();
		}

		// Token: 0x0600190A RID: 6410 RVA: 0x0005AFDC File Offset: 0x000591DC
		protected override void Awake()
		{
			base.Awake();
			DeathLottery.OnRequestUI += this.Show;
		}

		// Token: 0x0600190B RID: 6411 RVA: 0x0005AFF5 File Offset: 0x000591F5
		protected override void OnDestroy()
		{
			base.OnDestroy();
			DeathLottery.OnRequestUI -= this.Show;
		}

		// Token: 0x0600190C RID: 6412 RVA: 0x0005B00E File Offset: 0x0005920E
		private void Show(DeathLottery target)
		{
			this.target = target;
			this.Setup();
			base.Open(null);
		}

		// Token: 0x0600190D RID: 6413 RVA: 0x0005B024 File Offset: 0x00059224
		private void RefreshTexts()
		{
			this.remainingCountText.text = ((this.RemainingChances > 0) ? this.RemainingTextFormat.Format(new
			{
				amount = this.RemainingChances
			}) : this.noRemainingChances.ToPlainText());
		}

		// Token: 0x0600190E RID: 6414 RVA: 0x0005B060 File Offset: 0x00059260
		private void Setup()
		{
			if (this.target == null)
			{
				return;
			}
			if (this.target.Loading)
			{
				return;
			}
			DeathLottery.Status currentStatus = this.target.CurrentStatus;
			if (!currentStatus.valid)
			{
				return;
			}
			for (int i = 0; i < currentStatus.candidates.Count; i++)
			{
				this.cards[i].Setup(this, i);
			}
			this.RefreshTexts();
			this.HandleRemaining();
		}

		// Token: 0x0600190F RID: 6415 RVA: 0x0005B0D0 File Offset: 0x000592D0
		internal void NotifyEntryClicked(DeathLotteryCard deathLotteryCard, Cost cost)
		{
			if (deathLotteryCard == null)
			{
				return;
			}
			if (this.ProcessingSelection)
			{
				return;
			}
			if (this.RemainingChances <= 0)
			{
				return;
			}
			int index = deathLotteryCard.Index;
			if (this.target.CurrentStatus.selectedItems.Contains(index))
			{
				return;
			}
			this.selectTask = this.SelectTask(index, cost);
		}

		// Token: 0x1700048C RID: 1164
		// (get) Token: 0x06001910 RID: 6416 RVA: 0x0005B128 File Offset: 0x00059328
		private bool ProcessingSelection
		{
			get
			{
				return this.selectTask.Status == UniTaskStatus.Pending;
			}
		}

		// Token: 0x06001911 RID: 6417 RVA: 0x0005B138 File Offset: 0x00059338
		private async UniTask SelectTask(int index, Cost cost)
		{
			this.selectionBusyIndicator.Show();
			bool flag = await this.target.Select(index, cost);
			this.cards[index].NotifyFacing(flag);
			this.RefreshTexts();
			this.selectionBusyIndicator.Hide();
			this.HandleRemaining();
		}

		// Token: 0x06001912 RID: 6418 RVA: 0x0005B18C File Offset: 0x0005938C
		private void HandleRemaining()
		{
			if (this.RemainingChances > 0)
			{
				return;
			}
			DeathLotteryCard[] array = this.cards;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].NotifyFacing(true);
			}
		}

		// Token: 0x04001232 RID: 4658
		[SerializeField]
		private FadeGroup fadeGroup;

		// Token: 0x04001233 RID: 4659
		[LocalizationKey("Default")]
		[SerializeField]
		private string remainingTextFormatKey = "DeathLottery_Remaining";

		// Token: 0x04001234 RID: 4660
		[LocalizationKey("Default")]
		[SerializeField]
		private string noRemainingChances = "DeathLottery_NoRemainingChances";

		// Token: 0x04001235 RID: 4661
		[SerializeField]
		private TextMeshProUGUI remainingCountText;

		// Token: 0x04001236 RID: 4662
		[SerializeField]
		private DeathLotteryCard[] cards;

		// Token: 0x04001237 RID: 4663
		[SerializeField]
		private FadeGroup selectionBusyIndicator;

		// Token: 0x04001238 RID: 4664
		private DeathLottery target;

		// Token: 0x04001239 RID: 4665
		private UniTask selectTask;
	}
}
