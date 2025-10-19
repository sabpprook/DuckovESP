using System;
using System.Collections.Generic;
using System.Linq;
using Duckov.Economy;
using Duckov.UI;
using Duckov.Utilities;
using ItemStatsSystem;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020001FF RID: 511
public class ItemRepair_RepairAllPanel : MonoBehaviour
{
	// Token: 0x170002AA RID: 682
	// (get) Token: 0x06000EF6 RID: 3830 RVA: 0x0003B4F4 File Offset: 0x000396F4
	private PrefabPool<ItemDisplay> Pool
	{
		get
		{
			if (this._pool == null)
			{
				this._pool = new PrefabPool<ItemDisplay>(this.itemDisplayTemplate, null, null, null, null, true, 10, 10000, delegate(ItemDisplay e)
				{
					e.onPointerClick += this.OnPointerClickEntry;
				});
			}
			return this._pool;
		}
	}

	// Token: 0x06000EF7 RID: 3831 RVA: 0x0003B538 File Offset: 0x00039738
	private void OnPointerClickEntry(ItemDisplay display, PointerEventData data)
	{
		data.Use();
	}

	// Token: 0x06000EF8 RID: 3832 RVA: 0x0003B540 File Offset: 0x00039740
	private void Awake()
	{
		this.itemDisplayTemplate.gameObject.SetActive(false);
		this.button.onClick.AddListener(new UnityAction(this.OnButtonClicked));
	}

	// Token: 0x06000EF9 RID: 3833 RVA: 0x0003B570 File Offset: 0x00039770
	private void OnButtonClicked()
	{
		if (this.master == null)
		{
			return;
		}
		List<Item> allEquippedItems = this.master.GetAllEquippedItems();
		this.master.RepairItems(allEquippedItems);
		this.needsRefresh = true;
	}

	// Token: 0x06000EFA RID: 3834 RVA: 0x0003B5AB File Offset: 0x000397AB
	private void OnEnable()
	{
		ItemUtilities.OnPlayerItemOperation += this.OnPlayerItemOperation;
		ItemRepairView.OnRepaireOptionDone += this.OnRepairOptionDone;
	}

	// Token: 0x06000EFB RID: 3835 RVA: 0x0003B5CF File Offset: 0x000397CF
	private void OnDisable()
	{
		ItemUtilities.OnPlayerItemOperation -= this.OnPlayerItemOperation;
		ItemRepairView.OnRepaireOptionDone -= this.OnRepairOptionDone;
	}

	// Token: 0x06000EFC RID: 3836 RVA: 0x0003B5F3 File Offset: 0x000397F3
	public void Setup(ItemRepairView master)
	{
		this.master = master;
		this.Refresh();
	}

	// Token: 0x06000EFD RID: 3837 RVA: 0x0003B602 File Offset: 0x00039802
	private void OnPlayerItemOperation()
	{
		this.needsRefresh = true;
	}

	// Token: 0x06000EFE RID: 3838 RVA: 0x0003B60B File Offset: 0x0003980B
	private void OnRepairOptionDone()
	{
		this.needsRefresh = true;
	}

	// Token: 0x06000EFF RID: 3839 RVA: 0x0003B614 File Offset: 0x00039814
	private void Refresh()
	{
		this.needsRefresh = false;
		this.Pool.ReleaseAll();
		List<Item> list = (from e in this.master.GetAllEquippedItems()
			where e.Durability < e.MaxDurabilityWithLoss
			select e).ToList<Item>();
		int num = 0;
		if (list != null && list.Count > 0)
		{
			foreach (Item item in list)
			{
				this.Pool.Get(null).Setup(item);
			}
			num = this.master.CalculateRepairPrice(list);
			this.placeholder.SetActive(false);
			Cost cost = new Cost((long)num);
			bool enough = cost.Enough;
			this.button.interactable = enough;
		}
		else
		{
			this.placeholder.SetActive(true);
			this.button.interactable = false;
		}
		this.priceDisplay.text = num.ToString();
	}

	// Token: 0x06000F00 RID: 3840 RVA: 0x0003B72C File Offset: 0x0003992C
	private void Update()
	{
		if (this.needsRefresh)
		{
			this.Refresh();
		}
	}

	// Token: 0x04000C4A RID: 3146
	[SerializeField]
	private ItemRepairView master;

	// Token: 0x04000C4B RID: 3147
	[SerializeField]
	private TextMeshProUGUI priceDisplay;

	// Token: 0x04000C4C RID: 3148
	[SerializeField]
	private ItemDisplay itemDisplayTemplate;

	// Token: 0x04000C4D RID: 3149
	[SerializeField]
	private Button button;

	// Token: 0x04000C4E RID: 3150
	[SerializeField]
	private GameObject placeholder;

	// Token: 0x04000C4F RID: 3151
	private PrefabPool<ItemDisplay> _pool;

	// Token: 0x04000C50 RID: 3152
	private bool needsRefresh;
}
