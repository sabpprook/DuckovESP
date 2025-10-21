using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Cysharp.Threading.Tasks;
using ItemStatsSystem;
using UnityEngine;

namespace Duckov.Economy
{
	// Token: 0x02000322 RID: 802
	[Serializable]
	public struct Cost
	{
		// Token: 0x170004E5 RID: 1253
		// (get) Token: 0x06001ACE RID: 6862 RVA: 0x00060DB5 File Offset: 0x0005EFB5
		public bool Enough
		{
			get
			{
				return EconomyManager.IsEnough(this, true, true);
			}
		}

		// Token: 0x170004E6 RID: 1254
		// (get) Token: 0x06001ACF RID: 6863 RVA: 0x00060DC4 File Offset: 0x0005EFC4
		public bool IsFree
		{
			get
			{
				return this.money <= 0L && (this.items == null || this.items.Length == 0);
			}
		}

		// Token: 0x06001AD0 RID: 6864 RVA: 0x00060DE6 File Offset: 0x0005EFE6
		public bool Pay(bool accountAvaliable = true, bool cashAvaliable = true)
		{
			return EconomyManager.Pay(this, accountAvaliable, cashAvaliable);
		}

		// Token: 0x06001AD1 RID: 6865 RVA: 0x00060DF8 File Offset: 0x0005EFF8
		public static Cost FromString(string costDescription)
		{
			int num = 0;
			List<Cost.ItemEntry> list = new List<Cost.ItemEntry>();
			foreach (string text in costDescription.Split(',', StringSplitOptions.None))
			{
				string[] array2 = text.Split(":", StringSplitOptions.None);
				if (array2.Length != 2)
				{
					Debug.LogError("Invalid cost description: " + text + "\n" + costDescription);
				}
				else
				{
					string text2 = array2[0].Trim();
					int num2;
					if (!int.TryParse(array2[1].Trim(), out num2))
					{
						Debug.LogError("Invalid cost description: " + text);
					}
					else if (text2 == "money")
					{
						num = num2;
					}
					else
					{
						int num3 = ItemAssetsCollection.TryGetIDByName(text2);
						if (num3 <= 0)
						{
							Debug.LogError("Invalid item name " + text2);
						}
						else
						{
							list.Add(new Cost.ItemEntry
							{
								id = num3,
								amount = (long)num2
							});
						}
					}
				}
			}
			return new Cost
			{
				money = (long)num,
				items = list.ToArray()
			};
		}

		// Token: 0x170004E7 RID: 1255
		// (get) Token: 0x06001AD2 RID: 6866 RVA: 0x00060F09 File Offset: 0x0005F109
		public static bool TaskPending
		{
			get
			{
				return Cost.ReturnTaskLocks.Count > 0;
			}
		}

		// Token: 0x06001AD3 RID: 6867 RVA: 0x00060F18 File Offset: 0x0005F118
		internal async UniTask Return(bool directToBuffer = false, bool toPlayerInventory = false, int amountFactor = 1, List<Item> generatedItemsBuffer = null)
		{
			object taskLock = new object();
			Cost.ReturnTaskLocks.Add(taskLock);
			List<Item> generatedItems = new List<Item>();
			foreach (Cost.ItemEntry item in this.items)
			{
				long count = item.amount * (long)amountFactor;
				while (count > 0L)
				{
					Item item2 = await ItemAssetsCollection.InstantiateAsync(item.id);
					if (item2.Stackable)
					{
						if (count > (long)item2.MaxStackCount)
						{
							item2.StackCount = item2.MaxStackCount;
						}
						else
						{
							item2.StackCount = (int)count;
						}
						if (item2.StackCount <= 0)
						{
							Debug.LogError(string.Format("物品{0}({1})的StackCount为{2},请检查", item2.DisplayName, item2.TypeID, item2.StackCount));
							count -= 1L;
						}
						else
						{
							count -= (long)item2.StackCount;
						}
					}
					else
					{
						count -= 1L;
					}
					generatedItems.Add(item2);
				}
			}
			Cost.ItemEntry[] array = null;
			foreach (Item item3 in generatedItems)
			{
				if (!toPlayerInventory || !ItemUtilities.SendToPlayerCharacterInventory(item3, false))
				{
					ItemUtilities.SendToPlayerStorage(item3, directToBuffer);
				}
			}
			if (generatedItemsBuffer != null)
			{
				generatedItemsBuffer.AddRange(generatedItems);
			}
			EconomyManager.Add(this.money * (long)amountFactor);
			Cost.ReturnTaskLocks.Remove(taskLock);
		}

		// Token: 0x06001AD4 RID: 6868 RVA: 0x00060F84 File Offset: 0x0005F184
		public Cost(long money, [TupleElementNames(new string[] { "id", "amount" })] ValueTuple<int, long>[] items)
		{
			this.money = money;
			this.items = new Cost.ItemEntry[items.Length];
			for (int i = 0; i < items.Length; i++)
			{
				ValueTuple<int, long> valueTuple = items[i];
				this.items[i] = new Cost.ItemEntry
				{
					id = valueTuple.Item1,
					amount = valueTuple.Item2
				};
			}
		}

		// Token: 0x06001AD5 RID: 6869 RVA: 0x00060FEB File Offset: 0x0005F1EB
		public Cost(long money)
		{
			this.money = money;
			this.items = new Cost.ItemEntry[0];
		}

		// Token: 0x06001AD6 RID: 6870 RVA: 0x00061000 File Offset: 0x0005F200
		public Cost([TupleElementNames(new string[] { "id", "amount" })] params ValueTuple<int, long>[] items)
		{
			this.money = 0L;
			this.items = new Cost.ItemEntry[items.Length];
			for (int i = 0; i < items.Length; i++)
			{
				ValueTuple<int, long> valueTuple = items[i];
				this.items[i] = new Cost.ItemEntry
				{
					id = valueTuple.Item1,
					amount = valueTuple.Item2
				};
			}
		}

		// Token: 0x04001318 RID: 4888
		public long money;

		// Token: 0x04001319 RID: 4889
		public Cost.ItemEntry[] items;

		// Token: 0x0400131A RID: 4890
		private static List<object> ReturnTaskLocks = new List<object>();

		// Token: 0x020005BE RID: 1470
		[Serializable]
		public struct ItemEntry
		{
			// Token: 0x04002069 RID: 8297
			[ItemTypeID]
			public int id;

			// Token: 0x0400206A RID: 8298
			public long amount;
		}
	}
}
