using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ItemStatsSystem;
using ItemStatsSystem.Items;

namespace YKF_ESP
{
	// Token: 0x0200000B RID: 11
	[NullableContext(1)]
	[Nullable(0)]
	public static class EnemyValueCalculator
	{
		// Token: 0x0600003A RID: 58 RVA: 0x000033CC File Offset: 0x000015CC
		public static long CalculateEnemyInventoryValue(CharacterMainControl enemy)
		{
			if (enemy == null)
			{
				return 0L;
			}
			long num;
			try
			{
				Item characterItem = enemy.CharacterItem;
				if (characterItem == null)
				{
					num = 0L;
				}
				else
				{
					num = EnemyValueCalculator.CalculateInventoryValue(characterItem);
				}
			}
			catch (Exception)
			{
				num = 0L;
			}
			return num;
		}

		// Token: 0x0600003B RID: 59 RVA: 0x0000341C File Offset: 0x0000161C
		private static long CalculateInventoryValue(Item rootItem)
		{
			if (rootItem == null)
			{
				return 0L;
			}
			long num = (long)rootItem.GetTotalRawValue();
			Stack<Item> stack = new Stack<Item>();
			stack.Push(rootItem);
			while (stack.Count > 0)
			{
				Item item = stack.Pop();
				if (item.Slots != null)
				{
					foreach (Slot slot in item.Slots)
					{
						if (((slot != null) ? slot.Content : null) != null)
						{
							num += (long)slot.Content.GetTotalRawValue();
							stack.Push(slot.Content);
						}
					}
				}
			}
			return num;
		}
	}
}
