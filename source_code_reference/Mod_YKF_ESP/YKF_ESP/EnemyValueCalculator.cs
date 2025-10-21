using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ItemStatsSystem;
using ItemStatsSystem.Items;
using UnityEngine;

namespace YKF_ESP
{
	// Token: 0x0200000E RID: 14
	[NullableContext(1)]
	[Nullable(0)]
	public static class EnemyValueCalculator
	{
		// Token: 0x06000054 RID: 84 RVA: 0x0000482C File Offset: 0x00002A2C
		public static long CalculateEnemyInventoryValue(CharacterMainControl enemy)
		{
			if (enemy == null)
			{
				return 0L;
			}
			long num2;
			try
			{
				int instanceID = enemy.GetInstanceID();
				long num;
				if (EnemyValueCalculator.valueCache.TryGetValue(instanceID, out num))
				{
					num2 = num;
				}
				else
				{
					long num3 = EnemyValueCalculator.CalculateEnemyValueInternal(enemy);
					EnemyValueCalculator.valueCache[instanceID] = num3;
					EnemyValueCalculator.PerformCacheCleanupIfNeeded();
					num2 = num3;
				}
			}
			catch (Exception ex)
			{
				Debug.LogWarning("计算敌人价值时出错: " + ex.Message);
				num2 = 0L;
			}
			return num2;
		}

		// Token: 0x06000055 RID: 85 RVA: 0x000048AC File Offset: 0x00002AAC
		public static long ForceRecalculateEnemyValue(CharacterMainControl enemy)
		{
			if (enemy == null)
			{
				return 0L;
			}
			int instanceID = enemy.GetInstanceID();
			EnemyValueCalculator.valueCache.Remove(instanceID);
			return EnemyValueCalculator.CalculateEnemyInventoryValue(enemy);
		}

		// Token: 0x06000056 RID: 86 RVA: 0x000048DE File Offset: 0x00002ADE
		public static void ClearEnemyCache(CharacterMainControl enemy)
		{
			if (enemy != null)
			{
				EnemyValueCalculator.valueCache.Remove(enemy.GetInstanceID());
			}
		}

		// Token: 0x06000057 RID: 87 RVA: 0x000048FA File Offset: 0x00002AFA
		public static void ClearAllCache()
		{
			EnemyValueCalculator.valueCache.Clear();
		}

		// Token: 0x06000058 RID: 88 RVA: 0x00004908 File Offset: 0x00002B08
		private static long CalculateEnemyValueInternal(CharacterMainControl enemy)
		{
			long num = 0L;
			if (enemy.CharacterItem != null)
			{
				num += EnemyValueCalculator.CalculateInventoryValueOptimized(enemy.CharacterItem);
			}
			PetProxy component = enemy.GetComponent<PetProxy>();
			bool flag;
			if (component == null)
			{
				flag = null != null;
			}
			else
			{
				Inventory inventory = component.Inventory;
				flag = ((inventory != null) ? inventory.Content : null) != null;
			}
			if (flag)
			{
				foreach (Item item in component.Inventory.Content)
				{
					if (item != null)
					{
						num += EnemyValueCalculator.CalculateInventoryValueOptimized(item);
					}
				}
			}
			num += EnemyValueCalculator.CalculateCashValueOptimized(enemy);
			return num;
		}

		// Token: 0x06000059 RID: 89 RVA: 0x000049B8 File Offset: 0x00002BB8
		private static long CalculateCashValueOptimized(CharacterMainControl enemy)
		{
			if (((enemy != null) ? enemy.CharacterItem : null) == null)
			{
				return 0L;
			}
			long cashValue = 0L;
			EnemyValueCalculator.TraverseItemsOptimized(enemy.CharacterItem, delegate(Item item)
			{
				if (item.TypeID == 451)
				{
					cashValue += (long)item.StackCount;
				}
			});
			return cashValue;
		}

		// Token: 0x0600005A RID: 90 RVA: 0x00004A08 File Offset: 0x00002C08
		private static long CalculateInventoryValueOptimized(Item rootItem)
		{
			if (rootItem == null)
			{
				return 0L;
			}
			long totalValue = 0L;
			EnemyValueCalculator.TraverseItemsOptimized(rootItem, delegate(Item item)
			{
				totalValue += (long)item.GetTotalRawValue();
			});
			return totalValue;
		}

		// Token: 0x0600005B RID: 91 RVA: 0x00004A48 File Offset: 0x00002C48
		private static void TraverseItemsOptimized(Item rootItem, Action<Item> itemProcessor)
		{
			if (rootItem == null || itemProcessor == null)
			{
				return;
			}
			Stack<Item> stack = new Stack<Item>(32);
			stack.Push(rootItem);
			while (stack.Count > 0)
			{
				Item item = stack.Pop();
				if (!(item == null))
				{
					itemProcessor(item);
					if (item.Slots != null)
					{
						foreach (Slot slot in item.Slots)
						{
							if (((slot != null) ? slot.Content : null) != null)
							{
								stack.Push(slot.Content);
							}
						}
					}
					Inventory inventory = item.Inventory;
					if (((inventory != null) ? inventory.Content : null) != null)
					{
						foreach (Item item2 in item.Inventory.Content)
						{
							if (item2 != null)
							{
								stack.Push(item2);
							}
						}
					}
				}
			}
		}

		// Token: 0x0600005C RID: 92 RVA: 0x00004B6C File Offset: 0x00002D6C
		private static void PerformCacheCleanupIfNeeded()
		{
			float time = Time.time;
			if (time - EnemyValueCalculator.lastCacheCleanupTime > EnemyValueCalculator.cacheCleanupInterval)
			{
				EnemyValueCalculator.CleanupInvalidCacheEntries();
				EnemyValueCalculator.lastCacheCleanupTime = time;
			}
		}

		// Token: 0x0600005D RID: 93 RVA: 0x00004B98 File Offset: 0x00002D98
		private static void CleanupInvalidCacheEntries()
		{
			try
			{
				List<int> list = new List<int>();
				foreach (KeyValuePair<int, long> keyValuePair in EnemyValueCalculator.valueCache)
				{
					if (list.Count > 100)
					{
						break;
					}
				}
				if (EnemyValueCalculator.valueCache.Count > 1000)
				{
					EnemyValueCalculator.valueCache.Clear();
					Debug.Log("[EnemyValueCalculator] Cache cleared due to size limit");
				}
			}
			catch (Exception ex)
			{
				Debug.LogWarning("缓存清理时出错: " + ex.Message);
			}
		}

		// Token: 0x0600005E RID: 94 RVA: 0x00004C40 File Offset: 0x00002E40
		public static string GetCacheStats()
		{
			return string.Format("价值缓存条目数: {0}", EnemyValueCalculator.valueCache.Count);
		}

		// Token: 0x0600005F RID: 95 RVA: 0x00004C5C File Offset: 0x00002E5C
		private static List<Item> FindItemsByTypeIDOptimized(CharacterMainControl enemy, int typeID)
		{
			List<Item> foundItems = new List<Item>();
			if (((enemy != null) ? enemy.CharacterItem : null) == null)
			{
				return foundItems;
			}
			EnemyValueCalculator.TraverseItemsOptimized(enemy.CharacterItem, delegate(Item item)
			{
				if (item.TypeID == typeID)
				{
					foundItems.Add(item);
				}
			});
			return foundItems;
		}

		// Token: 0x04000034 RID: 52
		private static Dictionary<int, long> valueCache = new Dictionary<int, long>();

		// Token: 0x04000035 RID: 53
		private static float lastCacheCleanupTime = 0f;

		// Token: 0x04000036 RID: 54
		private static float cacheCleanupInterval = 60f;
	}
}
