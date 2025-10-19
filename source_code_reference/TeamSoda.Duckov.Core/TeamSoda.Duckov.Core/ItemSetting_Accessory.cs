using System;
using ItemStatsSystem;
using ItemStatsSystem.Items;
using UnityEngine;

// Token: 0x020000EB RID: 235
public class ItemSetting_Accessory : ItemSettingBase
{
	// Token: 0x060007C9 RID: 1993 RVA: 0x00022ECB File Offset: 0x000210CB
	public override void SetMarkerParam(Item selfItem)
	{
		selfItem.SetBool("IsBullet", true, true);
	}

	// Token: 0x060007CA RID: 1994 RVA: 0x00022EDA File Offset: 0x000210DA
	public override void OnInit()
	{
		base.Item.onPluggedIntoSlot += this.OnPluggedIntoSlot;
		base.Item.onUnpluggedFromSlot += this.OnUnpluggedIntoSlot;
	}

	// Token: 0x060007CB RID: 1995 RVA: 0x00022F0C File Offset: 0x0002110C
	private void OnPluggedIntoSlot(Item selfItem)
	{
		Slot pluggedIntoSlot = selfItem.PluggedIntoSlot;
		if (pluggedIntoSlot == null)
		{
			return;
		}
		this.masterItem = pluggedIntoSlot.Master;
		if (!this.masterItem)
		{
			return;
		}
		this.masterItem.AgentUtilities.onCreateAgent += this.OnMasterCreateAgent;
		this.CreateAccessory(this.masterItem.AgentUtilities.ActiveAgent as DuckovItemAgent);
	}

	// Token: 0x060007CC RID: 1996 RVA: 0x00022F75 File Offset: 0x00021175
	private void OnUnpluggedIntoSlot(Item selfItem)
	{
		if (this.masterItem)
		{
			this.masterItem.AgentUtilities.onCreateAgent -= this.OnMasterCreateAgent;
		}
		this.DestroyAccessory();
	}

	// Token: 0x060007CD RID: 1997 RVA: 0x00022FA6 File Offset: 0x000211A6
	private void OnDestroy()
	{
		if (this.masterItem)
		{
			this.masterItem.AgentUtilities.onCreateAgent -= this.OnMasterCreateAgent;
		}
		this.DestroyAccessory();
	}

	// Token: 0x060007CE RID: 1998 RVA: 0x00022FD7 File Offset: 0x000211D7
	private void OnMasterCreateAgent(Item _masterItem, ItemAgent newAgnet)
	{
		if (this.masterItem != _masterItem)
		{
			Debug.LogError("缓存了错误的Item");
		}
		if (newAgnet.AgentType != ItemAgent.AgentTypes.handheld)
		{
			return;
		}
		this.CreateAccessory(newAgnet as DuckovItemAgent);
	}

	// Token: 0x060007CF RID: 1999 RVA: 0x00023008 File Offset: 0x00021208
	private void CreateAccessory(DuckovItemAgent parentAgent)
	{
		this.DestroyAccessory();
		if (this.accessoryPfb == null || parentAgent == null || parentAgent.AgentType != ItemAgent.AgentTypes.handheld)
		{
			return;
		}
		this.accessoryInstance = global::UnityEngine.Object.Instantiate<AccessoryBase>(this.accessoryPfb);
		this.accessoryInstance.Init(parentAgent, base.Item);
	}

	// Token: 0x060007D0 RID: 2000 RVA: 0x0002305F File Offset: 0x0002125F
	private void DestroyAccessory()
	{
		if (this.accessoryInstance)
		{
			global::UnityEngine.Object.Destroy(this.accessoryInstance.gameObject);
		}
	}

	// Token: 0x04000744 RID: 1860
	[SerializeField]
	private AccessoryBase accessoryPfb;

	// Token: 0x04000745 RID: 1861
	public ADSAimMarker overrideAdsAimMarker;

	// Token: 0x04000746 RID: 1862
	private AccessoryBase accessoryInstance;

	// Token: 0x04000747 RID: 1863
	private bool created;

	// Token: 0x04000748 RID: 1864
	private Item masterItem;
}
