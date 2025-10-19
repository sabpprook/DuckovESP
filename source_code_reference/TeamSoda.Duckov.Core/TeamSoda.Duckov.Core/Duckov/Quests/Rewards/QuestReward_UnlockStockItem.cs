using System;
using Duckov.Economy;
using ItemStatsSystem;
using SodaCraft.Localizations;
using SodaCraft.StringUtilities;
using UnityEngine;

namespace Duckov.Quests.Rewards
{
	// Token: 0x02000358 RID: 856
	public class QuestReward_UnlockStockItem : Reward
	{
		// Token: 0x170005C2 RID: 1474
		// (get) Token: 0x06001DF0 RID: 7664 RVA: 0x00069EDF File Offset: 0x000680DF
		public int UnlockItem
		{
			get
			{
				return this.unlockItem;
			}
		}

		// Token: 0x06001DF1 RID: 7665 RVA: 0x00069EE7 File Offset: 0x000680E7
		private ItemMetaData GetItemMeta()
		{
			return ItemAssetsCollection.GetMetaData(this.unlockItem);
		}

		// Token: 0x170005C3 RID: 1475
		// (get) Token: 0x06001DF2 RID: 7666 RVA: 0x00069EF4 File Offset: 0x000680F4
		public override Sprite Icon
		{
			get
			{
				return ItemAssetsCollection.GetMetaData(this.unlockItem).icon;
			}
		}

		// Token: 0x170005C4 RID: 1476
		// (get) Token: 0x06001DF3 RID: 7667 RVA: 0x00069F06 File Offset: 0x00068106
		private string descriptionFormatKey
		{
			get
			{
				return "Reward_UnlockStockItem";
			}
		}

		// Token: 0x170005C5 RID: 1477
		// (get) Token: 0x06001DF4 RID: 7668 RVA: 0x00069F0D File Offset: 0x0006810D
		private string DescriptionFormat
		{
			get
			{
				return this.descriptionFormatKey.ToPlainText();
			}
		}

		// Token: 0x170005C6 RID: 1478
		// (get) Token: 0x06001DF5 RID: 7669 RVA: 0x00069F1C File Offset: 0x0006811C
		private string ItemDisplayName
		{
			get
			{
				return ItemAssetsCollection.GetMetaData(this.unlockItem).DisplayName;
			}
		}

		// Token: 0x170005C7 RID: 1479
		// (get) Token: 0x06001DF6 RID: 7670 RVA: 0x00069F3C File Offset: 0x0006813C
		public override string Description
		{
			get
			{
				return this.DescriptionFormat.Format(new { this.ItemDisplayName });
			}
		}

		// Token: 0x170005C8 RID: 1480
		// (get) Token: 0x06001DF7 RID: 7671 RVA: 0x00069F54 File Offset: 0x00068154
		public override bool Claimed
		{
			get
			{
				return this.claimed;
			}
		}

		// Token: 0x170005C9 RID: 1481
		// (get) Token: 0x06001DF8 RID: 7672 RVA: 0x00069F5C File Offset: 0x0006815C
		public override bool AutoClaim
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06001DF9 RID: 7673 RVA: 0x00069F5F File Offset: 0x0006815F
		public override object GenerateSaveData()
		{
			return this.claimed;
		}

		// Token: 0x06001DFA RID: 7674 RVA: 0x00069F6C File Offset: 0x0006816C
		public override void OnClaim()
		{
			EconomyManager.Unlock(this.unlockItem, true, true);
			this.claimed = true;
			base.ReportStatusChanged();
		}

		// Token: 0x06001DFB RID: 7675 RVA: 0x00069F88 File Offset: 0x00068188
		public override void SetupSaveData(object data)
		{
			if (data is bool)
			{
				bool flag = (bool)data;
				this.claimed = flag;
			}
		}

		// Token: 0x06001DFC RID: 7676 RVA: 0x00069FAB File Offset: 0x000681AB
		public override void NotifyReload(Quest questInstance)
		{
			if (questInstance.Complete)
			{
				EconomyManager.Unlock(this.unlockItem, true, true);
			}
		}

		// Token: 0x04001488 RID: 5256
		[SerializeField]
		[ItemTypeID]
		private int unlockItem;

		// Token: 0x04001489 RID: 5257
		private bool claimed;
	}
}
