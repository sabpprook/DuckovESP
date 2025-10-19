using System;
using SodaCraft.Localizations;
using SodaCraft.StringUtilities;
using UnityEngine;

namespace Duckov.Quests.Rewards
{
	// Token: 0x02000356 RID: 854
	public class QuestReward_EXP : Reward
	{
		// Token: 0x170005B6 RID: 1462
		// (get) Token: 0x06001DDC RID: 7644 RVA: 0x00069DB0 File Offset: 0x00067FB0
		public int Amount
		{
			get
			{
				return this.amount;
			}
		}

		// Token: 0x170005B7 RID: 1463
		// (get) Token: 0x06001DDD RID: 7645 RVA: 0x00069DB8 File Offset: 0x00067FB8
		public override bool Claimed
		{
			get
			{
				return this.claimed;
			}
		}

		// Token: 0x170005B8 RID: 1464
		// (get) Token: 0x06001DDE RID: 7646 RVA: 0x00069DC0 File Offset: 0x00067FC0
		private string descriptionFormatKey
		{
			get
			{
				return "Reward_Exp";
			}
		}

		// Token: 0x170005B9 RID: 1465
		// (get) Token: 0x06001DDF RID: 7647 RVA: 0x00069DC7 File Offset: 0x00067FC7
		private string DescriptionFormat
		{
			get
			{
				return this.descriptionFormatKey.ToPlainText();
			}
		}

		// Token: 0x170005BA RID: 1466
		// (get) Token: 0x06001DE0 RID: 7648 RVA: 0x00069DD4 File Offset: 0x00067FD4
		public override string Description
		{
			get
			{
				return this.DescriptionFormat.Format(new { this.amount });
			}
		}

		// Token: 0x170005BB RID: 1467
		// (get) Token: 0x06001DE1 RID: 7649 RVA: 0x00069DEC File Offset: 0x00067FEC
		public override bool AutoClaim
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06001DE2 RID: 7650 RVA: 0x00069DEF File Offset: 0x00067FEF
		public override object GenerateSaveData()
		{
			return this.claimed;
		}

		// Token: 0x06001DE3 RID: 7651 RVA: 0x00069DFC File Offset: 0x00067FFC
		public override void OnClaim()
		{
			if (this.Claimed)
			{
				return;
			}
			if (!EXPManager.AddExp(this.amount))
			{
				return;
			}
			this.claimed = true;
		}

		// Token: 0x06001DE4 RID: 7652 RVA: 0x00069E1C File Offset: 0x0006801C
		public override void SetupSaveData(object data)
		{
			if (data is bool)
			{
				bool flag = (bool)data;
				this.claimed = flag;
			}
		}

		// Token: 0x04001484 RID: 5252
		[SerializeField]
		private int amount;

		// Token: 0x04001485 RID: 5253
		[SerializeField]
		private bool claimed;
	}
}
