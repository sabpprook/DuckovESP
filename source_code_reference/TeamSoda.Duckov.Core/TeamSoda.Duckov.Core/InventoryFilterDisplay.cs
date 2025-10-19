using System;
using System.Collections.Generic;
using Duckov.UI;
using Duckov.Utilities;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x020001ED RID: 493
public class InventoryFilterDisplay : MonoBehaviour, ISingleSelectionMenu<InventoryFilterDisplayEntry>
{
	// Token: 0x1700029D RID: 669
	// (get) Token: 0x06000E73 RID: 3699 RVA: 0x0003A10C File Offset: 0x0003830C
	private PrefabPool<InventoryFilterDisplayEntry> Pool
	{
		get
		{
			if (this._pool == null)
			{
				this._pool = new PrefabPool<InventoryFilterDisplayEntry>(this.template, null, null, null, null, true, 10, 10000, null);
			}
			return this._pool;
		}
	}

	// Token: 0x06000E74 RID: 3700 RVA: 0x0003A145 File Offset: 0x00038345
	private void Awake()
	{
		this.template.gameObject.SetActive(false);
	}

	// Token: 0x06000E75 RID: 3701 RVA: 0x0003A158 File Offset: 0x00038358
	public void Setup(InventoryDisplay target)
	{
		this.Pool.ReleaseAll();
		this.entries.Clear();
		if (target == null)
		{
			return;
		}
		this.targetDisplay = target;
		this.provider = target.Target.GetComponent<InventoryFilterProvider>();
		if (this.provider == null)
		{
			return;
		}
		foreach (InventoryFilterProvider.FilterEntry filterEntry in this.provider.entries)
		{
			InventoryFilterDisplayEntry inventoryFilterDisplayEntry = this.Pool.Get(null);
			inventoryFilterDisplayEntry.Setup(new Action<InventoryFilterDisplayEntry, PointerEventData>(this.OnEntryClicked), filterEntry);
			this.entries.Add(inventoryFilterDisplayEntry);
		}
		this.selection = null;
	}

	// Token: 0x06000E76 RID: 3702 RVA: 0x0003A201 File Offset: 0x00038401
	private void OnEntryClicked(InventoryFilterDisplayEntry entry, PointerEventData data)
	{
		this.SetSelection(entry);
	}

	// Token: 0x06000E77 RID: 3703 RVA: 0x0003A20B File Offset: 0x0003840B
	internal void Select(int i)
	{
		if (i < 0 || i >= this.entries.Count)
		{
			return;
		}
		this.SetSelection(this.entries[i]);
	}

	// Token: 0x06000E78 RID: 3704 RVA: 0x0003A233 File Offset: 0x00038433
	public InventoryFilterDisplayEntry GetSelection()
	{
		return this.selection;
	}

	// Token: 0x06000E79 RID: 3705 RVA: 0x0003A23C File Offset: 0x0003843C
	public bool SetSelection(InventoryFilterDisplayEntry selection)
	{
		if (selection == null)
		{
			return false;
		}
		this.selection = selection;
		InventoryFilterProvider.FilterEntry filter = selection.Filter;
		this.targetDisplay.SetFilter(filter.GetFunction());
		foreach (InventoryFilterDisplayEntry inventoryFilterDisplayEntry in this.entries)
		{
			inventoryFilterDisplayEntry.NotifySelectionChanged(inventoryFilterDisplayEntry == selection);
		}
		return true;
	}

	// Token: 0x04000BFE RID: 3070
	[SerializeField]
	private InventoryFilterDisplayEntry template;

	// Token: 0x04000BFF RID: 3071
	private PrefabPool<InventoryFilterDisplayEntry> _pool;

	// Token: 0x04000C00 RID: 3072
	private InventoryDisplay targetDisplay;

	// Token: 0x04000C01 RID: 3073
	private InventoryFilterProvider provider;

	// Token: 0x04000C02 RID: 3074
	private List<InventoryFilterDisplayEntry> entries = new List<InventoryFilterDisplayEntry>();

	// Token: 0x04000C03 RID: 3075
	private InventoryFilterDisplayEntry selection;
}
