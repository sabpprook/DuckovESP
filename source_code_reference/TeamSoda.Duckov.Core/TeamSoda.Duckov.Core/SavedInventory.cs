using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using ItemStatsSystem;
using Saves;
using UnityEngine;

// Token: 0x020000E7 RID: 231
public class SavedInventory : MonoBehaviour
{
	// Token: 0x060007B0 RID: 1968 RVA: 0x000227E3 File Offset: 0x000209E3
	private void Awake()
	{
		if (this.inventory == null)
		{
			this.inventory = base.GetComponent<Inventory>();
		}
		this.Register();
	}

	// Token: 0x060007B1 RID: 1969 RVA: 0x00022805 File Offset: 0x00020A05
	private void Start()
	{
		if (this.registered)
		{
			this.Load();
		}
	}

	// Token: 0x060007B2 RID: 1970 RVA: 0x00022815 File Offset: 0x00020A15
	private void OnDestroy()
	{
		this.Unregsister();
	}

	// Token: 0x060007B3 RID: 1971 RVA: 0x00022820 File Offset: 0x00020A20
	private void Register()
	{
		SavedInventory savedInventory;
		if (SavedInventory.activeInventories.TryGetValue(this.key, out savedInventory))
		{
			Debug.LogError("存在多个带有相同Key的Saved Inventory: " + this.key, base.gameObject);
			return;
		}
		SavesSystem.OnCollectSaveData += this.Save;
		this.registered = true;
	}

	// Token: 0x060007B4 RID: 1972 RVA: 0x00022875 File Offset: 0x00020A75
	private void Unregsister()
	{
		SavesSystem.OnCollectSaveData -= this.Save;
	}

	// Token: 0x060007B5 RID: 1973 RVA: 0x00022888 File Offset: 0x00020A88
	private void Save()
	{
		this.inventory.Save(this.key);
	}

	// Token: 0x060007B6 RID: 1974 RVA: 0x0002289B File Offset: 0x00020A9B
	private void Load()
	{
		if (this.inventory.Loading)
		{
			Debug.LogError("Inventory is already loading.", base.gameObject);
			return;
		}
		ItemSavesUtilities.LoadInventory(this.key, this.inventory).Forget();
	}

	// Token: 0x0400073D RID: 1853
	[SerializeField]
	private Inventory inventory;

	// Token: 0x0400073E RID: 1854
	[SerializeField]
	private string key = "DefaultSavedInventory";

	// Token: 0x0400073F RID: 1855
	private static Dictionary<string, SavedInventory> activeInventories = new Dictionary<string, SavedInventory>();

	// Token: 0x04000740 RID: 1856
	private bool registered;
}
