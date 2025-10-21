using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using ItemStatsSystem;
using Saves;
using UnityEngine;

// Token: 0x02000109 RID: 265
public class PetProxy : MonoBehaviour
{
	// Token: 0x170001E9 RID: 489
	// (get) Token: 0x06000911 RID: 2321 RVA: 0x000284DE File Offset: 0x000266DE
	public static PetProxy Instance
	{
		get
		{
			if (LevelManager.Instance == null)
			{
				return null;
			}
			return LevelManager.Instance.PetProxy;
		}
	}

	// Token: 0x170001EA RID: 490
	// (get) Token: 0x06000912 RID: 2322 RVA: 0x000284F9 File Offset: 0x000266F9
	public static Inventory PetInventory
	{
		get
		{
			if (PetProxy.Instance == null)
			{
				return null;
			}
			return PetProxy.Instance.Inventory;
		}
	}

	// Token: 0x170001EB RID: 491
	// (get) Token: 0x06000913 RID: 2323 RVA: 0x00028514 File Offset: 0x00026714
	public Inventory Inventory
	{
		get
		{
			return this.inventory;
		}
	}

	// Token: 0x06000914 RID: 2324 RVA: 0x0002851C File Offset: 0x0002671C
	private void Start()
	{
		SavesSystem.OnCollectSaveData += this.OnCollectSaveData;
		ItemSavesUtilities.LoadInventory("Inventory_Safe", this.inventory).Forget();
	}

	// Token: 0x06000915 RID: 2325 RVA: 0x00028544 File Offset: 0x00026744
	private void OnDestroy()
	{
		SavesSystem.OnCollectSaveData -= this.OnCollectSaveData;
	}

	// Token: 0x06000916 RID: 2326 RVA: 0x00028557 File Offset: 0x00026757
	private void OnCollectSaveData()
	{
		this.inventory.Save("Inventory_Safe");
	}

	// Token: 0x06000917 RID: 2327 RVA: 0x0002856C File Offset: 0x0002676C
	public void DestroyItemInBase()
	{
		if (!this.Inventory)
		{
			return;
		}
		List<Item> list = new List<Item>();
		foreach (Item item in this.Inventory)
		{
			list.Add(item);
		}
		foreach (Item item2 in list)
		{
			if (item2.Tags.Contains("DestroyInBase"))
			{
				item2.DestroyTree();
			}
		}
	}

	// Token: 0x06000918 RID: 2328 RVA: 0x00028620 File Offset: 0x00026820
	private void Update()
	{
		if (!LevelManager.LevelInited)
		{
			return;
		}
		if (LevelManager.Instance.PetCharacter == null)
		{
			return;
		}
		base.transform.position = LevelManager.Instance.PetCharacter.transform.position;
		if (this.checkTimer > 0f)
		{
			this.checkTimer -= Time.unscaledDeltaTime;
			return;
		}
		if (CharacterMainControl.Main.PetCapcity != this.inventory.Capacity)
		{
			this.inventory.SetCapacity(CharacterMainControl.Main.PetCapcity);
		}
		this.checkTimer = 1f;
	}

	// Token: 0x04000829 RID: 2089
	[SerializeField]
	private Inventory inventory;

	// Token: 0x0400082A RID: 2090
	private float checkTimer = 0.02f;
}
