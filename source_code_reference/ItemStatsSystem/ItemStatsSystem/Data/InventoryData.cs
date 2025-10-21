using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ItemStatsSystem.Data
{
	// Token: 0x0200002C RID: 44
	[Serializable]
	public class InventoryData
	{
		// Token: 0x0600023C RID: 572 RVA: 0x00008BC8 File Offset: 0x00006DC8
		public static InventoryData FromInventory(Inventory inventory)
		{
			InventoryData inventoryData = new InventoryData();
			inventoryData.capacity = inventory.Capacity;
			int lastItemPosition = inventory.GetLastItemPosition();
			for (int i = 0; i <= lastItemPosition; i++)
			{
				Item itemAt = inventory.GetItemAt(i);
				if (!(itemAt == null))
				{
					InventoryData.Entry entry = new InventoryData.Entry();
					entry.inventoryPosition = i;
					entry.itemTreeData = ItemTreeData.FromItem(itemAt);
					inventoryData.entries.Add(entry);
				}
			}
			inventoryData.lockedIndexes = new List<int>(inventory.lockedIndexes);
			return inventoryData;
		}

		// Token: 0x0600023D RID: 573 RVA: 0x00008C48 File Offset: 0x00006E48
		public static async UniTask LoadIntoInventory(InventoryData data, Inventory inventoryInstance)
		{
			if (data != null)
			{
				foreach (InventoryData.Entry entry in data.entries)
				{
					int position = entry.inventoryPosition;
					Item item = await ItemTreeData.InstantiateAsync(entry.itemTreeData);
					if (item == null)
					{
						Debug.LogError("物品加载失败");
					}
					else if (!inventoryInstance.AddAt(item, position))
					{
						Debug.LogError("向 Inventory " + inventoryInstance.name + " 中添加物品失败。");
					}
				}
				List<InventoryData.Entry>.Enumerator enumerator = default(List<InventoryData.Entry>.Enumerator);
				if (data.lockedIndexes != null)
				{
					inventoryInstance.lockedIndexes.Clear();
					inventoryInstance.lockedIndexes.AddRange(data.lockedIndexes);
				}
			}
		}

		// Token: 0x040000CD RID: 205
		public int capacity = 16;

		// Token: 0x040000CE RID: 206
		public List<InventoryData.Entry> entries = new List<InventoryData.Entry>();

		// Token: 0x040000CF RID: 207
		public List<int> lockedIndexes = new List<int>();

		// Token: 0x02000053 RID: 83
		[Serializable]
		public class Entry
		{
			// Token: 0x04000141 RID: 321
			public int inventoryPosition;

			// Token: 0x04000142 RID: 322
			public ItemTreeData itemTreeData;
		}
	}
}
