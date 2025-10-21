using System;
using ItemStatsSystem;

// Token: 0x020000F2 RID: 242
public class ItemSetting_MeleeWeapon : ItemSettingBase
{
	// Token: 0x060007F4 RID: 2036 RVA: 0x000238D0 File Offset: 0x00021AD0
	public override void Start()
	{
		base.Start();
	}

	// Token: 0x060007F5 RID: 2037 RVA: 0x000238D8 File Offset: 0x00021AD8
	public override void SetMarkerParam(Item selfItem)
	{
		selfItem.SetBool("IsMeleeWeapon", true, true);
	}

	// Token: 0x04000766 RID: 1894
	public bool dealExplosionDamage;
}
