using System;
using System.Collections.Generic;
using Duckov.Bitcoins;
using ItemStatsSystem;
using ItemStatsSystem.Items;
using UnityEngine;

// Token: 0x02000180 RID: 384
public class MiningMachineVisual : MonoBehaviour
{
	// Token: 0x06000B88 RID: 2952 RVA: 0x00030CF4 File Offset: 0x0002EEF4
	private void Update()
	{
		if (!this.inited && BitcoinMiner.Instance && BitcoinMiner.Instance.Item != null)
		{
			this.inited = true;
			this.minnerItem = BitcoinMiner.Instance.Item;
			this.minnerItem.onSlotContentChanged += this.OnSlotContentChanged;
			this.slots = this.minnerItem.Slots;
			this.OnSlotContentChanged(this.minnerItem, null);
			return;
		}
	}

	// Token: 0x06000B89 RID: 2953 RVA: 0x00030D74 File Offset: 0x0002EF74
	private void OnDestroy()
	{
		if (this.minnerItem)
		{
			this.minnerItem.onSlotContentChanged -= this.OnSlotContentChanged;
		}
	}

	// Token: 0x06000B8A RID: 2954 RVA: 0x00030D9C File Offset: 0x0002EF9C
	private void OnSlotContentChanged(Item minnerItem, Slot changedSlot)
	{
		for (int i = 0; i < this.slots.Count; i++)
		{
			if (!(this.cardsDisplay[i] == null))
			{
				Item content = this.slots[i].Content;
				MiningMachineCardDisplay.CardTypes cardTypes = MiningMachineCardDisplay.CardTypes.normal;
				if (content != null)
				{
					ItemSetting_GPU component = content.GetComponent<ItemSetting_GPU>();
					if (component)
					{
						cardTypes = component.cardType;
					}
				}
				this.cardsDisplay[i].SetVisualActive(content != null, cardTypes);
			}
		}
	}

	// Token: 0x040009D9 RID: 2521
	public List<MiningMachineCardDisplay> cardsDisplay;

	// Token: 0x040009DA RID: 2522
	private bool inited;

	// Token: 0x040009DB RID: 2523
	private SlotCollection slots;

	// Token: 0x040009DC RID: 2524
	private Item minnerItem;
}
