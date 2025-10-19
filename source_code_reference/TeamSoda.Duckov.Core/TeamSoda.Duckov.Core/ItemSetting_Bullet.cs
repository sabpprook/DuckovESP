using System;
using ItemStatsSystem;

// Token: 0x020000EC RID: 236
public class ItemSetting_Bullet : ItemSettingBase
{
	// Token: 0x060007D2 RID: 2002 RVA: 0x00023086 File Offset: 0x00021286
	public override void SetMarkerParam(Item selfItem)
	{
		selfItem.SetBool("IsBullet", true, true);
	}
}
