using System;
using Duckov.Economy;
using SodaCraft.Localizations;
using SodaCraft.StringUtilities;
using UnityEngine;

namespace Duckov.Quests.Tasks
{
	// Token: 0x02000351 RID: 849
	public class QuestTask_SubmitMoney : Task
	{
		// Token: 0x17000599 RID: 1433
		// (get) Token: 0x06001D90 RID: 7568 RVA: 0x000692FE File Offset: 0x000674FE
		public string DescriptionFormat
		{
			get
			{
				return this.decriptionFormatKey.ToPlainText();
			}
		}

		// Token: 0x1700059A RID: 1434
		// (get) Token: 0x06001D91 RID: 7569 RVA: 0x0006930B File Offset: 0x0006750B
		public override string Description
		{
			get
			{
				return this.DescriptionFormat.Format(new { this.money });
			}
		}

		// Token: 0x1700059B RID: 1435
		// (get) Token: 0x06001D92 RID: 7570 RVA: 0x00069323 File Offset: 0x00067523
		public override bool Interactable
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700059C RID: 1436
		// (get) Token: 0x06001D93 RID: 7571 RVA: 0x00069326 File Offset: 0x00067526
		public override bool PossibleValidInteraction
		{
			get
			{
				return this.CheckMoneyEnough();
			}
		}

		// Token: 0x1700059D RID: 1437
		// (get) Token: 0x06001D94 RID: 7572 RVA: 0x0006932E File Offset: 0x0006752E
		public override string InteractText
		{
			get
			{
				return this.interactTextKey.ToPlainText();
			}
		}

		// Token: 0x06001D95 RID: 7573 RVA: 0x0006933C File Offset: 0x0006753C
		public override void Interact()
		{
			Cost cost = new Cost((long)this.money);
			if (cost.Pay(true, true))
			{
				this.submitted = true;
				base.ReportStatusChanged();
			}
		}

		// Token: 0x06001D96 RID: 7574 RVA: 0x0006936F File Offset: 0x0006756F
		private bool CheckMoneyEnough()
		{
			return EconomyManager.Money >= (long)this.money;
		}

		// Token: 0x06001D97 RID: 7575 RVA: 0x00069382 File Offset: 0x00067582
		public override object GenerateSaveData()
		{
			return this.submitted;
		}

		// Token: 0x06001D98 RID: 7576 RVA: 0x00069390 File Offset: 0x00067590
		public override void SetupSaveData(object data)
		{
			if (data is bool)
			{
				bool flag = (bool)data;
				this.submitted = flag;
			}
		}

		// Token: 0x06001D99 RID: 7577 RVA: 0x000693B3 File Offset: 0x000675B3
		protected override bool CheckFinished()
		{
			return this.submitted;
		}

		// Token: 0x0400146C RID: 5228
		[SerializeField]
		private int money;

		// Token: 0x0400146D RID: 5229
		[SerializeField]
		[LocalizationKey("Default")]
		private string decriptionFormatKey = "QuestTask_SubmitMoney";

		// Token: 0x0400146E RID: 5230
		[SerializeField]
		[LocalizationKey("Default")]
		private string interactTextKey = "QuestTask_SubmitMoney_Interact";

		// Token: 0x0400146F RID: 5231
		private bool submitted;
	}
}
