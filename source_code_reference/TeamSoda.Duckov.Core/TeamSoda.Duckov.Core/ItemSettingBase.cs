using System;
using ItemStatsSystem;
using UnityEngine;

// Token: 0x020000EA RID: 234
public abstract class ItemSettingBase : MonoBehaviour
{
	// Token: 0x170001A2 RID: 418
	// (get) Token: 0x060007C3 RID: 1987 RVA: 0x00022E7C File Offset: 0x0002107C
	public Item Item
	{
		get
		{
			if (this._item == null)
			{
				this._item = base.GetComponent<Item>();
			}
			return this._item;
		}
	}

	// Token: 0x060007C4 RID: 1988 RVA: 0x00022E9E File Offset: 0x0002109E
	public void Awake()
	{
		if (this.Item)
		{
			this.SetMarkerParam(this.Item);
			this.OnInit();
		}
	}

	// Token: 0x060007C5 RID: 1989 RVA: 0x00022EBF File Offset: 0x000210BF
	public virtual void OnInit()
	{
	}

	// Token: 0x060007C6 RID: 1990 RVA: 0x00022EC1 File Offset: 0x000210C1
	public virtual void Start()
	{
	}

	// Token: 0x060007C7 RID: 1991
	public abstract void SetMarkerParam(Item selfItem);

	// Token: 0x04000743 RID: 1859
	protected Item _item;
}
