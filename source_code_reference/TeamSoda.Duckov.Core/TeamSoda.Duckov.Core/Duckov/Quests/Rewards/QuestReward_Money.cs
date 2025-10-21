using System;
using Duckov.Economy;
using SodaCraft.Localizations;
using SodaCraft.StringUtilities;
using UnityEngine;

namespace Duckov.Quests.Rewards
{
	// Token: 0x02000357 RID: 855
	public class QuestReward_Money : Reward
	{
		// Token: 0x170005BC RID: 1468
		// (get) Token: 0x06001DE6 RID: 7654 RVA: 0x00069E47 File Offset: 0x00068047
		public int Amount
		{
			get
			{
				return this.amount;
			}
		}

		// Token: 0x170005BD RID: 1469
		// (get) Token: 0x06001DE7 RID: 7655 RVA: 0x00069E4F File Offset: 0x0006804F
		public override bool Claimed
		{
			get
			{
				return this.claimed;
			}
		}

		// Token: 0x170005BE RID: 1470
		// (get) Token: 0x06001DE8 RID: 7656 RVA: 0x00069E57 File Offset: 0x00068057
		[SerializeField]
		private string descriptionFormatKey
		{
			get
			{
				return "Reward_Money";
			}
		}

		// Token: 0x170005BF RID: 1471
		// (get) Token: 0x06001DE9 RID: 7657 RVA: 0x00069E5E File Offset: 0x0006805E
		private string DescriptionFormat
		{
			get
			{
				return this.descriptionFormatKey.ToPlainText();
			}
		}

		// Token: 0x170005C0 RID: 1472
		// (get) Token: 0x06001DEA RID: 7658 RVA: 0x00069E6B File Offset: 0x0006806B
		public override bool AutoClaim
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170005C1 RID: 1473
		// (get) Token: 0x06001DEB RID: 7659 RVA: 0x00069E6E File Offset: 0x0006806E
		public override string Description
		{
			get
			{
				return this.DescriptionFormat.Format(new { this.amount });
			}
		}

		// Token: 0x06001DEC RID: 7660 RVA: 0x00069E86 File Offset: 0x00068086
		public override object GenerateSaveData()
		{
			return this.claimed;
		}

		// Token: 0x06001DED RID: 7661 RVA: 0x00069E93 File Offset: 0x00068093
		public override void OnClaim()
		{
			if (this.Claimed)
			{
				return;
			}
			if (!EconomyManager.Add((long)this.amount))
			{
				return;
			}
			this.claimed = true;
		}

		// Token: 0x06001DEE RID: 7662 RVA: 0x00069EB4 File Offset: 0x000680B4
		public override void SetupSaveData(object data)
		{
			if (data is bool)
			{
				bool flag = (bool)data;
				this.claimed = flag;
			}
		}

		// Token: 0x04001486 RID: 5254
		[Min(0f)]
		[SerializeField]
		private int amount;

		// Token: 0x04001487 RID: 5255
		[SerializeField]
		private bool claimed;
	}
}
