using System;
using Duckov.Economy;
using Duckov.PerkTrees;
using ItemStatsSystem;
using SodaCraft.Localizations;
using SodaCraft.StringUtilities;
using UnityEngine;

// Token: 0x020001E0 RID: 480
public class UnlockStockShopItem : PerkBehaviour
{
	// Token: 0x17000297 RID: 663
	// (get) Token: 0x06000E39 RID: 3641 RVA: 0x00039519 File Offset: 0x00037719
	private string DescriptionFormat
	{
		get
		{
			return "PerkBehaviour_UnlockStockShopItem".ToPlainText();
		}
	}

	// Token: 0x17000298 RID: 664
	// (get) Token: 0x06000E3A RID: 3642 RVA: 0x00039525 File Offset: 0x00037725
	public override string Description
	{
		get
		{
			return this.DescriptionFormat.Format(new { this.ItemDisplayName });
		}
	}

	// Token: 0x17000299 RID: 665
	// (get) Token: 0x06000E3B RID: 3643 RVA: 0x00039540 File Offset: 0x00037740
	private string ItemDisplayName
	{
		get
		{
			return ItemAssetsCollection.GetMetaData(this.itemTypeID).DisplayName;
		}
	}

	// Token: 0x06000E3C RID: 3644 RVA: 0x00039560 File Offset: 0x00037760
	private void Start()
	{
		if (base.Master.Unlocked && !EconomyManager.IsUnlocked(this.itemTypeID))
		{
			EconomyManager.Unlock(this.itemTypeID, false, false);
		}
	}

	// Token: 0x06000E3D RID: 3645 RVA: 0x00039589 File Offset: 0x00037789
	protected override void OnUnlocked()
	{
		base.OnUnlocked();
		EconomyManager.Unlock(this.itemTypeID, false, true);
	}

	// Token: 0x04000BC3 RID: 3011
	[ItemTypeID]
	[SerializeField]
	private int itemTypeID;
}
