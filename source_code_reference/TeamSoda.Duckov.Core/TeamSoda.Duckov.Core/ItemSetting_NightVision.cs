using System;
using ItemStatsSystem;

// Token: 0x020000F3 RID: 243
public class ItemSetting_NightVision : ItemSettingBase
{
	// Token: 0x060007F7 RID: 2039 RVA: 0x000238EF File Offset: 0x00021AEF
	public override void OnInit()
	{
		if (this._item)
		{
			this._item.onPluggedIntoSlot += this.OnplugedIntoSlot;
		}
	}

	// Token: 0x060007F8 RID: 2040 RVA: 0x00023915 File Offset: 0x00021B15
	private void OnplugedIntoSlot(Item item)
	{
		this.nightVisionOn = true;
		this.SyncModifiers();
	}

	// Token: 0x060007F9 RID: 2041 RVA: 0x00023924 File Offset: 0x00021B24
	private void OnDestroy()
	{
		if (this._item)
		{
			this._item.onPluggedIntoSlot -= this.OnplugedIntoSlot;
		}
	}

	// Token: 0x060007FA RID: 2042 RVA: 0x0002394A File Offset: 0x00021B4A
	public void ToggleNightVison()
	{
		this.nightVisionOn = !this.nightVisionOn;
		this.SyncModifiers();
	}

	// Token: 0x060007FB RID: 2043 RVA: 0x00023961 File Offset: 0x00021B61
	private void SyncModifiers()
	{
		if (!this._item)
		{
			return;
		}
		this._item.Modifiers.ModifierEnable = this.nightVisionOn;
	}

	// Token: 0x060007FC RID: 2044 RVA: 0x00023987 File Offset: 0x00021B87
	public override void SetMarkerParam(Item selfItem)
	{
		selfItem.SetBool("IsNightVision", true, true);
	}

	// Token: 0x04000767 RID: 1895
	private bool nightVisionOn = true;
}
