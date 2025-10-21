using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Duckov.Utilities;
using ItemStatsSystem.Items;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ItemStatsSystem.Data
{
	// Token: 0x0200002B RID: 43
	[Serializable]
	public class ItemTreeData
	{
		// Token: 0x14000018 RID: 24
		// (add) Token: 0x06000231 RID: 561 RVA: 0x00008784 File Offset: 0x00006984
		// (remove) Token: 0x06000232 RID: 562 RVA: 0x000087B8 File Offset: 0x000069B8
		public static event Action<Item> OnItemLoaded;

		// Token: 0x1700009C RID: 156
		// (get) Token: 0x06000233 RID: 563 RVA: 0x000087EC File Offset: 0x000069EC
		public ItemTreeData.DataEntry RootData
		{
			get
			{
				ItemTreeData.DataEntry dataEntry = this.entries.Find((ItemTreeData.DataEntry e) => e.instanceID == this.rootInstanceID);
				if (dataEntry == null)
				{
					return null;
				}
				return dataEntry;
			}
		}

		// Token: 0x1700009D RID: 157
		// (get) Token: 0x06000234 RID: 564 RVA: 0x00008818 File Offset: 0x00006A18
		public int RootTypeID
		{
			get
			{
				ItemTreeData.DataEntry dataEntry = this.entries.Find((ItemTreeData.DataEntry e) => e.instanceID == this.rootInstanceID);
				if (dataEntry == null)
				{
					return 0;
				}
				return dataEntry.typeID;
			}
		}

		// Token: 0x06000235 RID: 565 RVA: 0x00008848 File Offset: 0x00006A48
		public static ItemTreeData FromItem(Item item)
		{
			ItemTreeData itemTreeData = new ItemTreeData();
			Dictionary<int, ItemTreeData.DataEntry> dictionary = new Dictionary<int, ItemTreeData.DataEntry>();
			itemTreeData.rootInstanceID = item.GetInstanceID();
			List<Item> allChildren = item.GetAllChildren(true, false);
			foreach (Item item2 in allChildren)
			{
				ItemTreeData.DataEntry dataEntry = new ItemTreeData.DataEntry
				{
					instanceID = item2.GetInstanceID(),
					typeID = item2.TypeID
				};
				foreach (CustomData customData in item2.Variables)
				{
					dataEntry.variables.Add(new CustomData(customData));
				}
				if (item2.Inventory != null)
				{
					int lastItemPosition = item2.Inventory.GetLastItemPosition();
					for (int i = 0; i <= lastItemPosition; i++)
					{
						Item item3 = item2.Inventory[i];
						if (item3 != null)
						{
							dataEntry.inventory.Add(new ItemTreeData.InventoryDataEntry(i, item3.GetInstanceID()));
						}
					}
					dataEntry.inventorySortLocks = new List<int>(item2.Inventory.lockedIndexes);
				}
				dictionary.Add(dataEntry.instanceID, dataEntry);
				itemTreeData.entries.Add(dataEntry);
			}
			foreach (Item item4 in allChildren)
			{
				ItemTreeData.DataEntry dataEntry2 = dictionary[item4.GetInstanceID()];
				if (!(item4.Slots == null))
				{
					foreach (Slot slot in item4.Slots)
					{
						if (slot.Content != null)
						{
							dataEntry2.slotContents.Add(new ItemTreeData.SlotInstanceIDPair(slot.Key, slot.Content.GetInstanceID()));
						}
					}
				}
			}
			return itemTreeData;
		}

		// Token: 0x06000236 RID: 566 RVA: 0x00008AB8 File Offset: 0x00006CB8
		public static async UniTask<Item> InstantiateAsync(ItemTreeData data)
		{
			Item item;
			if (data == null)
			{
				item = null;
			}
			else
			{
				Dictionary<int, Item> instanceDic = new Dictionary<int, Item>();
				Scene beginningScene = SceneManager.GetActiveScene();
				bool beginningSceneLoaded = beginningScene.isLoaded;
				bool playing = Application.isPlaying;
				bool abort = false;
				foreach (ItemTreeData.DataEntry curData in data.entries)
				{
					if ((beginningSceneLoaded && !beginningScene.isLoaded) || Application.isPlaying != playing)
					{
						abort = true;
						break;
					}
					Item item2 = await ItemAssetsCollection.InstantiateAsync(curData.typeID);
					if (item2 == null)
					{
						Debug.LogError(string.Format("Failed to create item {0}, type:{1}", data.rootInstanceID, curData.typeID));
					}
					else
					{
						instanceDic.Add(curData.instanceID, item2);
						foreach (CustomData customData in curData.variables)
						{
							item2.Variables.SetRaw(customData.Key, customData.DataType, customData.GetRawCopied(), true);
						}
						Action<Item> onItemLoaded = ItemTreeData.OnItemLoaded;
						if (onItemLoaded != null)
						{
							onItemLoaded(item2);
						}
						curData = null;
					}
				}
				List<ItemTreeData.DataEntry>.Enumerator enumerator = default(List<ItemTreeData.DataEntry>.Enumerator);
				if (abort)
				{
					Debug.LogWarning("Item Instantiate Aborted");
					Item[] array = instanceDic.Values.ToArray<Item>();
					for (int i = 0; i < array.Length; i++)
					{
						global::UnityEngine.Object.Destroy(array[i].gameObject);
					}
					item = null;
				}
				else
				{
					foreach (ItemTreeData.DataEntry dataEntry in data.entries)
					{
						Item item3;
						if (instanceDic.TryGetValue(dataEntry.instanceID, out item3))
						{
							foreach (ItemTreeData.SlotInstanceIDPair slotInstanceIDPair in dataEntry.slotContents)
							{
								if (item3.Slots == null)
								{
									Debug.LogError(string.Format("Trying to plug item to slot {0}({1}-{2})/{3}, but the slot doesn't exist.", new object[] { item3.name, item3.TypeID, item3.DisplayName, slotInstanceIDPair.slot }));
									break;
								}
								Slot slot = item3.Slots[slotInstanceIDPair.slot];
								Item item4;
								instanceDic.TryGetValue(slotInstanceIDPair.instanceID, out item4);
								if (slot != null && !(item4 == null))
								{
									Item item5;
									slot.Plug(item4, out item5);
									if (item5 != null)
									{
										Debug.LogError("Found Unplugged Item while Loading Item Tree!");
									}
								}
							}
							if (dataEntry.inventory.Count > 0)
							{
								if (item3.Inventory == null)
								{
									Debug.LogError("尝试加载Inventory数据，但物品的Inventory不存在。");
								}
								else
								{
									foreach (ItemTreeData.InventoryDataEntry inventoryDataEntry in dataEntry.inventory)
									{
										Item item6;
										if (instanceDic.TryGetValue(inventoryDataEntry.instanceID, out item6))
										{
											item3.Inventory.AddAt(item6, inventoryDataEntry.position);
										}
										else
										{
											Debug.LogError(string.Format("加载Inventory时找不到物品实例 {0}", inventoryDataEntry.instanceID));
										}
									}
								}
							}
							if (item3.Inventory && dataEntry.inventorySortLocks != null)
							{
								item3.Inventory.lockedIndexes.Clear();
								item3.Inventory.lockedIndexes.AddRange(dataEntry.inventorySortLocks);
							}
						}
					}
					Item item7;
					if (instanceDic.TryGetValue(data.rootInstanceID, out item7))
					{
						item = item7;
					}
					else
					{
						Debug.LogError(string.Format("Missing Item {0} \n {1}", data.rootInstanceID, data.ToString()));
						item = null;
					}
				}
			}
			return item;
		}

		// Token: 0x06000237 RID: 567 RVA: 0x00008AFC File Offset: 0x00006CFC
		public ItemTreeData.DataEntry GetEntry(int instanceID)
		{
			return this.entries.Find((ItemTreeData.DataEntry e) => e.instanceID == instanceID);
		}

		// Token: 0x06000238 RID: 568 RVA: 0x00008B30 File Offset: 0x00006D30
		public override string ToString()
		{
			ItemTreeData.<>c__DisplayClass15_0 CS$<>8__locals1 = new ItemTreeData.<>c__DisplayClass15_0();
			CS$<>8__locals1.<>4__this = this;
			ItemTreeData.DataEntry dataEntry = this.entries.Find((ItemTreeData.DataEntry e) => e.instanceID == CS$<>8__locals1.<>4__this.rootInstanceID);
			if (dataEntry == null)
			{
				Debug.LogError("No Root Entry in Tree");
				return "Invalid Item Tree";
			}
			CS$<>8__locals1.indent = 0;
			CS$<>8__locals1.result = "";
			CS$<>8__locals1.<ToString>g__PrintEntry|1(dataEntry);
			return CS$<>8__locals1.result;
		}

		// Token: 0x040000CB RID: 203
		public int rootInstanceID;

		// Token: 0x040000CC RID: 204
		public List<ItemTreeData.DataEntry> entries = new List<ItemTreeData.DataEntry>();

		// Token: 0x0200004D RID: 77
		[Serializable]
		public class DataEntry
		{
			// Token: 0x170000A3 RID: 163
			// (get) Token: 0x0600028F RID: 655 RVA: 0x000096B1 File Offset: 0x000078B1
			public string TypeName
			{
				get
				{
					return string.Format("TYPE_{0}", this.typeID);
				}
			}

			// Token: 0x170000A4 RID: 164
			// (get) Token: 0x06000290 RID: 656 RVA: 0x000096C8 File Offset: 0x000078C8
			public int StackCount
			{
				get
				{
					CustomData customData = this.variables.Find((CustomData e) => e.Key == "Count");
					if (customData == null)
					{
						return 1;
					}
					if (customData.DataType != CustomDataType.Int)
					{
						return 1;
					}
					return customData.GetInt();
				}
			}

			// Token: 0x04000128 RID: 296
			public int instanceID;

			// Token: 0x04000129 RID: 297
			public int typeID;

			// Token: 0x0400012A RID: 298
			public List<CustomData> variables = new List<CustomData>();

			// Token: 0x0400012B RID: 299
			public List<ItemTreeData.SlotInstanceIDPair> slotContents = new List<ItemTreeData.SlotInstanceIDPair>();

			// Token: 0x0400012C RID: 300
			public List<ItemTreeData.InventoryDataEntry> inventory = new List<ItemTreeData.InventoryDataEntry>();

			// Token: 0x0400012D RID: 301
			public List<int> inventorySortLocks = new List<int>();
		}

		// Token: 0x0200004E RID: 78
		public class SlotInstanceIDPair
		{
			// Token: 0x06000292 RID: 658 RVA: 0x0000974A File Offset: 0x0000794A
			public SlotInstanceIDPair(string slot, int instanceID)
			{
				this.slot = slot;
				this.instanceID = instanceID;
			}

			// Token: 0x0400012E RID: 302
			public string slot;

			// Token: 0x0400012F RID: 303
			public int instanceID;
		}

		// Token: 0x0200004F RID: 79
		public class InventoryDataEntry
		{
			// Token: 0x06000293 RID: 659 RVA: 0x00009760 File Offset: 0x00007960
			public InventoryDataEntry(int position, int instanceID)
			{
				this.position = position;
				this.instanceID = instanceID;
			}

			// Token: 0x04000130 RID: 304
			public int position;

			// Token: 0x04000131 RID: 305
			public int instanceID;
		}
	}
}
