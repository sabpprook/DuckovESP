using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace ItemStatsSystem
{
	// Token: 0x02000005 RID: 5
	[CreateAssetMenu(menuName = "Items/Item Asset Collection")]
	public class ItemAssetsCollection : ScriptableObject, ISelfValidator
	{
		// Token: 0x17000002 RID: 2
		// (get) Token: 0x0600000B RID: 11 RVA: 0x00002380 File Offset: 0x00000580
		public static ItemAssetsCollection Instance
		{
			get
			{
				if (ItemAssetsCollection.instanceCache)
				{
					return ItemAssetsCollection.instanceCache;
				}
				ItemAssetsCollection.instanceCache = Resources.Load<ItemAssetsCollection>("ItemAssetsCollection");
				return ItemAssetsCollection.instanceCache;
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x0600000C RID: 12 RVA: 0x000023A8 File Offset: 0x000005A8
		public int NextTypeID
		{
			get
			{
				int num = this.entries.Max((ItemAssetsCollection.Entry e) => e.typeID);
				if (this.nextTypeID <= num)
				{
					this.nextTypeID = num + 1;
				}
				return this.nextTypeID;
			}
		}

		// Token: 0x0600000D RID: 13 RVA: 0x000023F8 File Offset: 0x000005F8
		private static bool TryGetDynamicEntry(int typeID, out ItemAssetsCollection.DynamicEntry entry)
		{
			return ItemAssetsCollection.dynamicDic.TryGetValue(typeID, out entry) && entry != null;
		}

		// Token: 0x0600000E RID: 14 RVA: 0x00002414 File Offset: 0x00000614
		public static bool AddDynamicEntry(Item prefab)
		{
			if (prefab == null)
			{
				return false;
			}
			if (ItemAssetsCollection.Instance == null)
			{
				return false;
			}
			int typeID = prefab.TypeID;
			if (ItemAssetsCollection.Instance.entries.Any((ItemAssetsCollection.Entry e) => e != null && e.typeID == typeID))
			{
				Debug.LogWarning(string.Format("Warning from Dynamic Item:{0}\nDynamic Item Type ID collides with the main game. This will override the main game's item. Please make sure this is intentional, or avoid it.", typeID));
			}
			ItemAssetsCollection.DynamicEntry dynamicEntry;
			if (ItemAssetsCollection.TryGetDynamicEntry(typeID, out dynamicEntry))
			{
				Debug.LogWarning(string.Format("Warning from Dynamic Item:{0}\nDynamic Item Overwrite detected! May cause some of the mod work incorrectly. Please avoid colliding item type ids.", typeID));
			}
			ItemAssetsCollection.DynamicEntry dynamicEntry2 = new ItemAssetsCollection.DynamicEntry
			{
				typeID = typeID,
				prefab = prefab
			};
			ItemAssetsCollection.dynamicDic[typeID] = dynamicEntry2;
			return true;
		}

		// Token: 0x0600000F RID: 15 RVA: 0x000024D8 File Offset: 0x000006D8
		public static bool RemoveDynamicEntry(Item prefab)
		{
			if (prefab == null)
			{
				return false;
			}
			if (ItemAssetsCollection.Instance == null)
			{
				return false;
			}
			int typeID = prefab.TypeID;
			ItemAssetsCollection.DynamicEntry dynamicEntry;
			return ItemAssetsCollection.TryGetDynamicEntry(typeID, out dynamicEntry) && !(dynamicEntry.prefab != prefab) && ItemAssetsCollection.dynamicDic.Remove(typeID);
		}

		// Token: 0x06000010 RID: 16 RVA: 0x00002530 File Offset: 0x00000730
		private ItemAssetsCollection.Entry GetEntry(int typeID)
		{
			if (this.dic == null)
			{
				this.dic = new Dictionary<int, ItemAssetsCollection.Entry>();
				foreach (ItemAssetsCollection.Entry entry in this.entries)
				{
					this.dic[entry.typeID] = entry;
				}
			}
			ItemAssetsCollection.Entry entry2;
			if (this.dic.TryGetValue(typeID, out entry2))
			{
				return entry2;
			}
			return null;
		}

		// Token: 0x06000011 RID: 17 RVA: 0x000025B4 File Offset: 0x000007B4
		public async UniTask<Item> InstantiateAsync_Local(int typeID)
		{
			ItemAssetsCollection.DynamicEntry dynamicEntry;
			Item item;
			if (ItemAssetsCollection.TryGetDynamicEntry(typeID, out dynamicEntry))
			{
				item = global::UnityEngine.Object.Instantiate<Item>(dynamicEntry.prefab);
			}
			else
			{
				ItemAssetsCollection.Entry entry = this.GetEntry(typeID);
				if (entry == null)
				{
					Debug.LogWarning(string.Format("在 ItemAssetCollection 中找不到 Item ID:{0} 的项目。", typeID));
					item = null;
				}
				else if (entry.prefab == null)
				{
					Debug.LogWarning(string.Format("在 ItemAssetCollection 中未配置 Item ID:{0} 的 Asset。", typeID));
					item = null;
				}
				else
				{
					item = global::UnityEngine.Object.Instantiate<Item>(entry.prefab);
				}
			}
			return item;
		}

		// Token: 0x06000012 RID: 18 RVA: 0x00002600 File Offset: 0x00000800
		public static async UniTask<Item> InstantiateAsync(int typeID)
		{
			Item item;
			if (ItemAssetsCollection.Instance == null)
			{
				Debug.LogError("Instance of ItemAssetsCollection not found");
				item = null;
			}
			else
			{
				item = await ItemAssetsCollection.Instance.InstantiateAsync_Local(typeID);
			}
			return item;
		}

		// Token: 0x06000013 RID: 19 RVA: 0x00002644 File Offset: 0x00000844
		public static ItemMetaData GetMetaData(int typeID)
		{
			ItemAssetsCollection.DynamicEntry dynamicEntry;
			if (ItemAssetsCollection.TryGetDynamicEntry(typeID, out dynamicEntry))
			{
				return dynamicEntry.MetaData;
			}
			ItemAssetsCollection.Entry entry = ItemAssetsCollection.Instance.GetEntry(typeID);
			if (entry == null)
			{
				return default(ItemMetaData);
			}
			return entry.metaData;
		}

		// Token: 0x06000014 RID: 20 RVA: 0x00002684 File Offset: 0x00000884
		public static Item GetPrefab(int typeID)
		{
			ItemAssetsCollection.Entry entry = ItemAssetsCollection.Instance.GetEntry(typeID);
			if (entry == null)
			{
				return null;
			}
			return entry.prefab;
		}

		// Token: 0x06000015 RID: 21 RVA: 0x000026A8 File Offset: 0x000008A8
		public void Validate(SelfValidationResult result)
		{
		}

		// Token: 0x06000016 RID: 22 RVA: 0x000026AA File Offset: 0x000008AA
		public void Collect()
		{
		}

		// Token: 0x06000017 RID: 23 RVA: 0x000026AC File Offset: 0x000008AC
		private void SetFolderTag(Item item)
		{
		}

		// Token: 0x06000018 RID: 24 RVA: 0x000026B0 File Offset: 0x000008B0
		public void RefreshMeta()
		{
			foreach (ItemAssetsCollection.Entry entry in this.entries)
			{
				entry.RefreshMetaData();
			}
		}

		// Token: 0x06000019 RID: 25 RVA: 0x00002700 File Offset: 0x00000900
		public static int[] GetAllTypeIds(ItemFilter filter)
		{
			if (ItemAssetsCollection.Instance == null)
			{
				return null;
			}
			bool matchCaliber = !string.IsNullOrEmpty(filter.caliber);
			IEnumerable<int> enumerable = from e in ItemAssetsCollection.Instance.entries.FindAll(delegate(ItemAssetsCollection.Entry entry)
				{
					ItemMetaData metaData = entry.metaData;
					return base.<GetAllTypeIds>g__EvaluateFilter|0(metaData, filter);
				})
				select e.typeID;
			IEnumerable<int> enumerable2 = from e in ItemAssetsCollection.dynamicDic.Where(delegate(KeyValuePair<int, ItemAssetsCollection.DynamicEntry> e)
				{
					ItemAssetsCollection.DynamicEntry value = e.Value;
					return !(value.prefab == null) && base.<GetAllTypeIds>g__EvaluateFilter|0(value.MetaData, filter);
				})
				select e.Key;
			HashSet<int> hashSet = new HashSet<int>(enumerable);
			hashSet.AddRange(enumerable2);
			return hashSet.ToArray<int>();
		}

		// Token: 0x0600001A RID: 26 RVA: 0x000027D0 File Offset: 0x000009D0
		public static int[] Search(ItemFilter filter)
		{
			ItemAssetsCollection.<>c__DisplayClass26_0 CS$<>8__locals1;
			CS$<>8__locals1.filter = filter;
			int[] array;
			if (ItemAssetsCollection.cachedSearchResults.TryGetValue(CS$<>8__locals1.filter.GetHashCode(), out array))
			{
				return array;
			}
			CS$<>8__locals1.result = ItemAssetsCollection.GetAllTypeIds(CS$<>8__locals1.filter);
			while (CS$<>8__locals1.result.Length < 1)
			{
				ItemAssetsCollection.<Search>g__DownGradeSearch|26_0(ref CS$<>8__locals1);
				if (CS$<>8__locals1.filter.maxQuality < 0 || CS$<>8__locals1.filter.minQuality < 0)
				{
					break;
				}
			}
			ItemAssetsCollection.cachedSearchResults[CS$<>8__locals1.filter.GetHashCode()] = CS$<>8__locals1.result;
			return CS$<>8__locals1.result;
		}

		// Token: 0x0600001B RID: 27 RVA: 0x00002874 File Offset: 0x00000A74
		public static int TryGetIDByName(string name)
		{
			if (ItemAssetsCollection.Instance == null)
			{
				return -1;
			}
			ItemAssetsCollection.Entry entry = ItemAssetsCollection.Instance.entries.Find((ItemAssetsCollection.Entry e) => e.metaData.Name == name);
			if (entry == null)
			{
				return -1;
			}
			return entry.typeID;
		}

		// Token: 0x0600001E RID: 30 RVA: 0x000028E4 File Offset: 0x00000AE4
		[CompilerGenerated]
		internal static void <Search>g__DownGradeSearch|26_0(ref ItemAssetsCollection.<>c__DisplayClass26_0 A_0)
		{
			int num = Mathf.Min(A_0.filter.maxQuality, A_0.filter.minQuality) - 1;
			A_0.filter.maxQuality = num;
			A_0.filter.minQuality = num;
			if (num < 0)
			{
				return;
			}
			A_0.result = ItemAssetsCollection.GetAllTypeIds(A_0.filter);
		}

		// Token: 0x04000007 RID: 7
		private static ItemAssetsCollection instanceCache;

		// Token: 0x04000008 RID: 8
		private bool editNextTypeID;

		// Token: 0x04000009 RID: 9
		[SerializeField]
		private int nextTypeID;

		// Token: 0x0400000A RID: 10
		public List<ItemAssetsCollection.Entry> entries;

		// Token: 0x0400000B RID: 11
		private Dictionary<int, ItemAssetsCollection.Entry> dic;

		// Token: 0x0400000C RID: 12
		private static Dictionary<int, ItemAssetsCollection.DynamicEntry> dynamicDic = new Dictionary<int, ItemAssetsCollection.DynamicEntry>();

		// Token: 0x0400000D RID: 13
		private static Dictionary<int, int[]> cachedSearchResults = new Dictionary<int, int[]>();

		// Token: 0x02000030 RID: 48
		[Serializable]
		public class Entry : ISelfValidator
		{
			// Token: 0x0600023F RID: 575 RVA: 0x00008CB9 File Offset: 0x00006EB9
			public void RefreshMetaData()
			{
			}

			// Token: 0x06000240 RID: 576 RVA: 0x00008CBB File Offset: 0x00006EBB
			public void Validate(SelfValidationResult result)
			{
			}

			// Token: 0x040000DA RID: 218
			public int typeID;

			// Token: 0x040000DB RID: 219
			public Item prefab;

			// Token: 0x040000DC RID: 220
			public ItemMetaData metaData;
		}

		// Token: 0x02000031 RID: 49
		public class DynamicEntry
		{
			// Token: 0x1700009E RID: 158
			// (get) Token: 0x06000242 RID: 578 RVA: 0x00008CC8 File Offset: 0x00006EC8
			public ItemMetaData MetaData
			{
				get
				{
					if (this.prefab == null)
					{
						return default(ItemMetaData);
					}
					if (this._metaData == null)
					{
						this._metaData = new ItemMetaData?(new ItemMetaData(this.prefab));
					}
					return this._metaData.Value;
				}
			}

			// Token: 0x040000DD RID: 221
			public int typeID;

			// Token: 0x040000DE RID: 222
			public Item prefab;

			// Token: 0x040000DF RID: 223
			private ItemMetaData? _metaData;
		}
	}
}
