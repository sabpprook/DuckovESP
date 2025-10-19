using System;
using ItemStatsSystem;
using ItemStatsSystem.Items;
using UnityEngine;

// Token: 0x020000E4 RID: 228
public class FishingRod : MonoBehaviour
{
	// Token: 0x1700014D RID: 333
	// (get) Token: 0x0600072A RID: 1834 RVA: 0x00020163 File Offset: 0x0001E363
	private ItemAgent selfAgent
	{
		get
		{
			if (this._selfAgent == null)
			{
				this._selfAgent = base.GetComponent<ItemAgent>();
			}
			return this._selfAgent;
		}
	}

	// Token: 0x1700014E RID: 334
	// (get) Token: 0x0600072B RID: 1835 RVA: 0x00020185 File Offset: 0x0001E385
	public Item Bait
	{
		get
		{
			if (this.baitSlot == null)
			{
				this.baitSlot = this.selfAgent.Item.Slots.GetSlot("Bait");
			}
			if (this.baitSlot != null)
			{
				return this.baitSlot.Content;
			}
			return null;
		}
	}

	// Token: 0x0600072C RID: 1836 RVA: 0x000201C4 File Offset: 0x0001E3C4
	public bool UseBait()
	{
		Item bait = this.Bait;
		if (bait == null)
		{
			return false;
		}
		if (bait.Stackable)
		{
			bait.StackCount--;
		}
		else
		{
			bait.DestroyTree();
		}
		return true;
	}

	// Token: 0x040006D6 RID: 1750
	[SerializeField]
	private ItemAgent _selfAgent;

	// Token: 0x040006D7 RID: 1751
	private Slot baitSlot;

	// Token: 0x040006D8 RID: 1752
	public Transform lineStart;
}
