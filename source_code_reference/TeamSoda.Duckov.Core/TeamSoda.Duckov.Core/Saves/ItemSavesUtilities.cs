using System;
using Cysharp.Threading.Tasks;
using ItemStatsSystem;
using ItemStatsSystem.Data;
using UnityEngine;

namespace Saves
{
	// Token: 0x02000222 RID: 546
	public static class ItemSavesUtilities
	{
		// Token: 0x06001061 RID: 4193 RVA: 0x0003F8B0 File Offset: 0x0003DAB0
		public static void SaveAsLastDeadCharacter(Item item)
		{
			uint num = SavesSystem.Load<uint>("DeadCharacterToken");
			uint num2 = num;
			do
			{
				num2 += 1U;
			}
			while (num2 == num);
			SavesSystem.Save<uint>("DeadCharacterToken", num2);
			item.Save("LastDeadCharacter");
		}

		// Token: 0x06001062 RID: 4194 RVA: 0x0003F8E8 File Offset: 0x0003DAE8
		public static async UniTask<Item> LoadLastDeadCharacterItem()
		{
			return await ItemSavesUtilities.LoadItem("LastDeadCharacter");
		}

		// Token: 0x06001063 RID: 4195 RVA: 0x0003F924 File Offset: 0x0003DB24
		public static void Save(this Item item, string key)
		{
			ItemTreeData itemTreeData = ItemTreeData.FromItem(item);
			SavesSystem.Save<ItemTreeData>("Item/", key, itemTreeData);
		}

		// Token: 0x06001064 RID: 4196 RVA: 0x0003F944 File Offset: 0x0003DB44
		public static void Save(this Inventory inventory, string key)
		{
			InventoryData inventoryData = InventoryData.FromInventory(inventory);
			SavesSystem.Save<InventoryData>("Inventory/", key, inventoryData);
		}

		// Token: 0x06001065 RID: 4197 RVA: 0x0003F964 File Offset: 0x0003DB64
		public static async UniTask<Item> LoadItem(string key)
		{
			return await ItemTreeData.InstantiateAsync(SavesSystem.Load<ItemTreeData>("Item/", key));
		}

		// Token: 0x06001066 RID: 4198 RVA: 0x0003F9A8 File Offset: 0x0003DBA8
		public static async UniTask LoadInventory(string key, Inventory inventoryInstance)
		{
			if (!(inventoryInstance == null))
			{
				inventoryInstance.Loading = true;
				InventoryData inventoryData = SavesSystem.Load<InventoryData>("Inventory/", key);
				if (inventoryData == null)
				{
					Debug.LogWarning("Key Doesn't exist " + key + ", aborting operation");
					inventoryInstance.Loading = false;
				}
				else
				{
					inventoryInstance.DestroyAllContent();
					await InventoryData.LoadIntoInventory(inventoryData, inventoryInstance);
					inventoryInstance.Loading = false;
				}
			}
		}

		// Token: 0x04000D0D RID: 3341
		private const string InventoryPrefix = "Inventory/";

		// Token: 0x04000D0E RID: 3342
		private const string ItemPrefix = "Item/";
	}
}
