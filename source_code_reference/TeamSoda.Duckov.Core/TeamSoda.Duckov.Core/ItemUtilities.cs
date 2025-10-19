using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Duckov.Economy;
using Duckov.MasterKeys;
using Duckov.UI;
using Duckov.Utilities;
using ItemStatsSystem;
using ItemStatsSystem.Items;
using UnityEngine;

// Token: 0x020000F5 RID: 245
public static class ItemUtilities
{
	// Token: 0x170001AB RID: 427
	// (get) Token: 0x06000803 RID: 2051 RVA: 0x00023AB3 File Offset: 0x00021CB3
	private static Item CharacterItem
	{
		get
		{
			LevelManager instance = LevelManager.Instance;
			if (instance == null)
			{
				return null;
			}
			CharacterMainControl mainCharacter = instance.MainCharacter;
			if (mainCharacter == null)
			{
				return null;
			}
			return mainCharacter.CharacterItem;
		}
	}

	// Token: 0x170001AC RID: 428
	// (get) Token: 0x06000804 RID: 2052 RVA: 0x00023AD0 File Offset: 0x00021CD0
	private static Inventory CharacterInventory
	{
		get
		{
			Item characterItem = ItemUtilities.CharacterItem;
			if (characterItem == null)
			{
				return null;
			}
			return characterItem.Inventory;
		}
	}

	// Token: 0x170001AD RID: 429
	// (get) Token: 0x06000805 RID: 2053 RVA: 0x00023AE2 File Offset: 0x00021CE2
	private static Inventory PlayerStorageInventory
	{
		get
		{
			return PlayerStorage.Inventory;
		}
	}

	// Token: 0x14000031 RID: 49
	// (add) Token: 0x06000806 RID: 2054 RVA: 0x00023AEC File Offset: 0x00021CEC
	// (remove) Token: 0x06000807 RID: 2055 RVA: 0x00023B20 File Offset: 0x00021D20
	public static event Action OnPlayerItemOperation;

	// Token: 0x14000032 RID: 50
	// (add) Token: 0x06000808 RID: 2056 RVA: 0x00023B54 File Offset: 0x00021D54
	// (remove) Token: 0x06000809 RID: 2057 RVA: 0x00023B88 File Offset: 0x00021D88
	public static event Action<Item> OnItemSentToPlayerInventory;

	// Token: 0x14000033 RID: 51
	// (add) Token: 0x0600080A RID: 2058 RVA: 0x00023BBC File Offset: 0x00021DBC
	// (remove) Token: 0x0600080B RID: 2059 RVA: 0x00023BF0 File Offset: 0x00021DF0
	public static event Action<Item> OnItemSentToPlayerStorage;

	// Token: 0x0600080C RID: 2060 RVA: 0x00023C24 File Offset: 0x00021E24
	public static async UniTask<bool> Decompose(Item item, int count)
	{
		return await DecomposeDatabase.Decompose(item, count);
	}

	// Token: 0x0600080D RID: 2061 RVA: 0x00023C70 File Offset: 0x00021E70
	public static async UniTask<Item> GenerateBullet(Item gunItem)
	{
		Debug.Log("Trying to generate bullet for " + gunItem.DisplayName);
		if (gunItem.GetComponent<ItemSetting_Gun>())
		{
			string @string = gunItem.Constants.GetString("Caliber", null);
			if (!string.IsNullOrEmpty(@string))
			{
				ItemFilter itemFilter = new ItemFilter
				{
					caliber = @string,
					minQuality = 0,
					maxQuality = 1,
					requireTags = new Tag[] { GameplayDataSettings.Tags.Get("Bullet") }
				};
				itemFilter.caliber = @string;
				int[] array = ItemAssetsCollection.Search(itemFilter);
				if (array.Length != 0)
				{
					object obj = await ItemAssetsCollection.InstantiateAsync(array.GetRandom<int>());
					if (obj == null)
					{
						Debug.Log("Bullet generation failed for " + gunItem.DisplayName);
					}
					return obj;
				}
			}
		}
		Debug.Log("Bullet generation failed for " + gunItem.DisplayName);
		return null;
	}

	// Token: 0x0600080E RID: 2062 RVA: 0x00023CB4 File Offset: 0x00021EB4
	public static List<Item> FindAllBelongsToPlayer(Predicate<Item> predicate)
	{
		List<Item> list = new List<Item>();
		Inventory playerStorageInventory = ItemUtilities.PlayerStorageInventory;
		if (playerStorageInventory != null)
		{
			List<Item> list2 = playerStorageInventory.FindAll(predicate);
			list.AddRange(list2);
		}
		Inventory characterInventory = ItemUtilities.CharacterInventory;
		if (characterInventory != null)
		{
			List<Item> list3 = characterInventory.FindAll(predicate);
			list.AddRange(list3);
		}
		LevelManager instance = LevelManager.Instance;
		Inventory inventory;
		if (instance == null)
		{
			inventory = null;
		}
		else
		{
			PetProxy petProxy = instance.PetProxy;
			inventory = ((petProxy != null) ? petProxy.Inventory : null);
		}
		Inventory inventory2 = inventory;
		if (inventory2 != null)
		{
			List<Item> list4 = inventory2.FindAll(predicate);
			list.AddRange(list4);
		}
		return list;
	}

	// Token: 0x0600080F RID: 2063 RVA: 0x00023D40 File Offset: 0x00021F40
	public static int GetItemCount(int typeID)
	{
		List<Item> list = ItemUtilities.FindAllBelongsToPlayer((Item e) => e != null && e.TypeID == typeID);
		int num = 0;
		foreach (Item item in list)
		{
			num += item.StackCount;
		}
		return num;
	}

	// Token: 0x06000810 RID: 2064 RVA: 0x00023DB0 File Offset: 0x00021FB0
	public static bool AddAndMerge(this Inventory inventory, Item item, int preferedFirstPosition = 0)
	{
		if (inventory == null)
		{
			return false;
		}
		if (item.Stackable)
		{
			Func<Item, bool> <>9__0;
			while (item.StackCount > 0)
			{
				Func<Item, bool> func;
				if ((func = <>9__0) == null)
				{
					func = (<>9__0 = (Item e) => e.TypeID == item.TypeID && e.MaxStackCount > e.StackCount);
				}
				Item item2 = inventory.FirstOrDefault(func);
				if (item2 == null)
				{
					break;
				}
				item2.Combine(item);
			}
			if (item.StackCount <= 0)
			{
				return true;
			}
		}
		int firstEmptyPosition = inventory.GetFirstEmptyPosition(preferedFirstPosition);
		if (firstEmptyPosition < 0)
		{
			return false;
		}
		item.Detach();
		inventory.AddAt(item, firstEmptyPosition);
		return true;
	}

	// Token: 0x06000811 RID: 2065 RVA: 0x00023E68 File Offset: 0x00022068
	public static bool SendToPlayerCharacter(Item item, bool dontMerge = false)
	{
		if (item == null)
		{
			return false;
		}
		LevelManager instance = LevelManager.Instance;
		Item item2;
		if (instance == null)
		{
			item2 = null;
		}
		else
		{
			CharacterMainControl mainCharacter = instance.MainCharacter;
			item2 = ((mainCharacter != null) ? mainCharacter.CharacterItem : null);
		}
		Item item3 = item2;
		if (item3 == null)
		{
			return false;
		}
		if (item3.TryPlug(item, true, null, 0))
		{
			Action onPlayerItemOperation = ItemUtilities.OnPlayerItemOperation;
			if (onPlayerItemOperation != null)
			{
				onPlayerItemOperation();
			}
			return true;
		}
		return ItemUtilities.SendToPlayerCharacterInventory(item, dontMerge);
	}

	// Token: 0x06000812 RID: 2066 RVA: 0x00023ECE File Offset: 0x000220CE
	public static void SendToPlayer(Item item, bool dontMerge = false, bool sendToStorage = true)
	{
		if (ItemUtilities.SendToPlayerCharacter(item, dontMerge))
		{
			return;
		}
		if (sendToStorage)
		{
			ItemUtilities.SendToPlayerStorage(item, false);
			return;
		}
		item.Drop(CharacterMainControl.Main, true);
	}

	// Token: 0x06000813 RID: 2067 RVA: 0x00023EF4 File Offset: 0x000220F4
	public static bool SendToPlayerCharacterInventory(Item item, bool dontMerge = false)
	{
		if (item == null)
		{
			return false;
		}
		LevelManager instance = LevelManager.Instance;
		Inventory inventory;
		if (instance == null)
		{
			inventory = null;
		}
		else
		{
			CharacterMainControl mainCharacter = instance.MainCharacter;
			if (mainCharacter == null)
			{
				inventory = null;
			}
			else
			{
				Item characterItem = mainCharacter.CharacterItem;
				inventory = ((characterItem != null) ? characterItem.Inventory : null);
			}
		}
		Inventory inventory2 = inventory;
		if (inventory2 == null)
		{
			return false;
		}
		int num = 0;
		bool flag;
		if (dontMerge)
		{
			flag = inventory2.AddItem(item);
		}
		else
		{
			flag = inventory2.AddAndMerge(item, num);
		}
		if (!flag)
		{
			return false;
		}
		Action onPlayerItemOperation = ItemUtilities.OnPlayerItemOperation;
		if (onPlayerItemOperation != null)
		{
			onPlayerItemOperation();
		}
		Action<Item> onItemSentToPlayerInventory = ItemUtilities.OnItemSentToPlayerInventory;
		if (onItemSentToPlayerInventory != null)
		{
			onItemSentToPlayerInventory(item);
		}
		return true;
	}

	// Token: 0x06000814 RID: 2068 RVA: 0x00023F82 File Offset: 0x00022182
	public static void SendToPlayerStorage(Item item, bool directToBuffer = false)
	{
		item.Detach();
		PlayerStorage.Push(item, directToBuffer);
		Action<Item> onItemSentToPlayerStorage = ItemUtilities.OnItemSentToPlayerStorage;
		if (onItemSentToPlayerStorage == null)
		{
			return;
		}
		onItemSentToPlayerStorage(item);
	}

	// Token: 0x06000815 RID: 2069 RVA: 0x00023FA4 File Offset: 0x000221A4
	public static bool IsInPlayerCharacter(this Item item)
	{
		ItemUtilities.<>c__DisplayClass24_0 CS$<>8__locals1 = new ItemUtilities.<>c__DisplayClass24_0();
		ItemUtilities.<>c__DisplayClass24_0 CS$<>8__locals2 = CS$<>8__locals1;
		LevelManager instance = LevelManager.Instance;
		Item item2;
		if (instance == null)
		{
			item2 = null;
		}
		else
		{
			CharacterMainControl mainCharacter = instance.MainCharacter;
			item2 = ((mainCharacter != null) ? mainCharacter.CharacterItem : null);
		}
		CS$<>8__locals2.characterItem = item2;
		return !(CS$<>8__locals1.characterItem == null) && item.GetAllParents(false).Any((Item e) => e == CS$<>8__locals1.characterItem);
	}

	// Token: 0x06000816 RID: 2070 RVA: 0x00024004 File Offset: 0x00022204
	public static bool IsInPlayerStorage(this Item item)
	{
		Inventory playerStorageInventory = PlayerStorage.Inventory;
		return !(playerStorageInventory == null) && item.GetAllParents(false).Any((Item e) => e.InInventory == playerStorageInventory);
	}

	// Token: 0x06000817 RID: 2071 RVA: 0x0002404A File Offset: 0x0002224A
	public static bool IsRegistered(this Item item)
	{
		return !(item == null) && (MasterKeysManager.IsActive(item.TypeID) || CraftingManager.IsFormulaUnlocked(FormulasRegisterView.GetFormulaID(item)));
	}

	// Token: 0x06000818 RID: 2072 RVA: 0x00024078 File Offset: 0x00022278
	public static bool TryPlug(this Item main, Item part, bool emptyOnly = false, Inventory backupInventory = null, int preferredFirstIndex = 0)
	{
		if (main == null)
		{
			return false;
		}
		if (part == null)
		{
			return false;
		}
		if (main.Slots == null)
		{
			return false;
		}
		bool flag = main.IsInPlayerCharacter();
		bool flag2 = part.IsInPlayerCharacter();
		bool flag3 = main.IsInPlayerStorage();
		bool flag4 = part.IsInPlayerStorage();
		bool flag5 = flag || flag2 || flag3 || flag4;
		Slot slot = null;
		Slot pluggedIntoSlot = part.PluggedIntoSlot;
		if (backupInventory == null)
		{
			if (part.InInventory)
			{
				backupInventory = part.InInventory;
			}
			else if (main.InInventory)
			{
				backupInventory = main.InInventory;
			}
			else if (part.PluggedIntoSlot != null)
			{
				Item characterItem = part.GetCharacterItem();
				if (characterItem != null)
				{
					backupInventory = characterItem.Inventory;
				}
			}
			if (backupInventory == null)
			{
				Item characterItem2 = main.GetCharacterItem();
				if (characterItem2 != null)
				{
					backupInventory = characterItem2.Inventory;
				}
			}
		}
		IEnumerable<Slot> enumerable = main.Slots.Where((Slot e) => e != null && e.CanPlug(part));
		if (part.PluggedIntoSlot != null)
		{
			foreach (Slot slot2 in enumerable)
			{
				if (part.PluggedIntoSlot == slot2)
				{
					Debug.Log("什么也没做，因为已经在这个物体上了。");
					return false;
				}
			}
		}
		if (part.Stackable)
		{
			foreach (Slot slot3 in enumerable)
			{
				Item content = slot3.Content;
				if (!(content == null) && content.TypeID == part.TypeID)
				{
					content.Combine(part);
					if (part.StackCount <= 0)
					{
						return true;
					}
				}
			}
		}
		Slot slot4 = enumerable.FirstOrDefault((Slot e) => e.Content == null);
		if (slot4 != null)
		{
			slot = slot4;
		}
		else if (!emptyOnly)
		{
			slot = enumerable.FirstOrDefault<Slot>();
		}
		if (slot == null)
		{
			return false;
		}
		Item item;
		slot.Plug(part, out item);
		if (item != null)
		{
			bool flag6 = false;
			if (pluggedIntoSlot != null && pluggedIntoSlot.Content == null)
			{
				Item item2;
				flag6 = pluggedIntoSlot.Plug(item, out item2);
			}
			if (!flag6 && backupInventory != null)
			{
				flag6 = backupInventory.AddAndMerge(item, preferredFirstIndex);
			}
			if (!flag6)
			{
				if (flag5)
				{
					item.Drop(CharacterMainControl.Main, true);
				}
				else
				{
					item.Drop(Vector3.down * 1000f, false, Vector3.up, 0f);
				}
			}
		}
		return true;
	}

	// Token: 0x06000819 RID: 2073 RVA: 0x00024374 File Offset: 0x00022574
	public static CharacterMainControl GetCharacterMainControl(this Item item)
	{
		Item root = item.GetRoot();
		if (root == null)
		{
			return null;
		}
		return root.GetComponentInParent<CharacterMainControl>();
	}

	// Token: 0x0600081A RID: 2074 RVA: 0x00024388 File Offset: 0x00022588
	internal static IEnumerable<Inventory> GetPlayerInventories()
	{
		HashSet<Inventory> hashSet = new HashSet<Inventory>();
		LevelManager instance = LevelManager.Instance;
		Inventory inventory;
		if (instance == null)
		{
			inventory = null;
		}
		else
		{
			CharacterMainControl mainCharacter = instance.MainCharacter;
			if (mainCharacter == null)
			{
				inventory = null;
			}
			else
			{
				Item characterItem = mainCharacter.CharacterItem;
				inventory = ((characterItem != null) ? characterItem.Inventory : null);
			}
		}
		Inventory inventory2 = inventory;
		if (inventory2)
		{
			hashSet.Add(inventory2);
		}
		if (PlayerStorage.Inventory != null)
		{
			hashSet.Add(PlayerStorage.Inventory);
		}
		return hashSet;
	}

	// Token: 0x0600081B RID: 2075 RVA: 0x000243F0 File Offset: 0x000225F0
	internal static bool ConsumeItems(Cost cost)
	{
		ItemUtilities.<>c__DisplayClass30_0 CS$<>8__locals1 = new ItemUtilities.<>c__DisplayClass30_0();
		List<Action> list = new List<Action>();
		CS$<>8__locals1.detachedItems = new List<Item>();
		if (cost.items != null)
		{
			Cost.ItemEntry[] items2 = cost.items;
			for (int i = 0; i < items2.Length; i++)
			{
				ItemUtilities.<>c__DisplayClass30_1 CS$<>8__locals2 = new ItemUtilities.<>c__DisplayClass30_1();
				CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
				CS$<>8__locals2.cur = items2[i];
				List<Item> items = ItemUtilities.FindAllBelongsToPlayer((Item e) => e != null && e.TypeID == CS$<>8__locals2.cur.id);
				int count = ItemUtilities.Count(items);
				if ((long)count < CS$<>8__locals2.cur.amount)
				{
					return false;
				}
				list.Add(delegate
				{
					long num = CS$<>8__locals2.cur.amount;
					for (int j = 0; j < count; j++)
					{
						Item item2 = items[j];
						if (!(item2 == null))
						{
							if (item2.Slots != null)
							{
								foreach (Slot slot in item2.Slots)
								{
									if (slot != null)
									{
										Item content = slot.Content;
										if (!(content == null))
										{
											content.Detach();
											CS$<>8__locals2.CS$<>8__locals1.detachedItems.Add(content);
										}
									}
								}
							}
							if ((long)item2.StackCount <= num)
							{
								num -= (long)item2.StackCount;
								item2.Detach();
								item2.DestroyTree();
							}
							else
							{
								item2.StackCount -= (int)num;
								num = 0L;
							}
							if (num <= 0L)
							{
								break;
							}
						}
					}
				});
			}
		}
		foreach (Action action in list)
		{
			action();
		}
		foreach (Item item in CS$<>8__locals1.detachedItems)
		{
			if (!(item == null))
			{
				ItemUtilities.SendToPlayer(item, false, PlayerStorage.Inventory != null);
			}
		}
		Action onPlayerItemOperation = ItemUtilities.OnPlayerItemOperation;
		if (onPlayerItemOperation != null)
		{
			onPlayerItemOperation();
		}
		return true;
	}

	// Token: 0x0600081C RID: 2076 RVA: 0x00024574 File Offset: 0x00022774
	internal static bool ConsumeItems(int itemTypeID, long amount)
	{
		List<Item> list = ItemUtilities.FindAllBelongsToPlayer((Item e) => e != null && e.TypeID == itemTypeID);
		if ((long)ItemUtilities.Count(list) < amount)
		{
			return false;
		}
		List<Item> list2 = new List<Item>();
		long num = amount;
		for (int i = 0; i < list.Count; i++)
		{
			Item item = list[i];
			if (!(item == null))
			{
				if (item.Slots != null)
				{
					foreach (Slot slot in item.Slots)
					{
						if (slot != null)
						{
							Item content = slot.Content;
							if (!(content == null))
							{
								content.Detach();
								list2.Add(content);
							}
						}
					}
				}
				if ((long)item.StackCount <= num)
				{
					num -= (long)item.StackCount;
					item.Detach();
					item.DestroyTree();
				}
				else
				{
					item.StackCount -= (int)num;
					num = 0L;
				}
				if (num <= 0L)
				{
					break;
				}
			}
		}
		foreach (Item item2 in list2)
		{
			if (!(item2 == null))
			{
				ItemUtilities.SendToPlayer(item2, false, PlayerStorage.Inventory != null);
			}
		}
		return true;
	}

	// Token: 0x0600081D RID: 2077 RVA: 0x000246E8 File Offset: 0x000228E8
	internal static int Count(IEnumerable<Item> items)
	{
		int num = 0;
		foreach (Item item in items)
		{
			if (item.Stackable)
			{
				num += item.StackCount;
			}
			else
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x0600081E RID: 2078 RVA: 0x00024744 File Offset: 0x00022944
	public static float GetRepairLossRatio(this Item item)
	{
		if (item == null)
		{
			return 0f;
		}
		float num = 0.14f;
		float num2 = item.Constants.GetFloat("RepairLossRatio", num);
		if (item.Tags.Contains("Weapon"))
		{
			float num3 = CharacterMainControl.WeaponRepairLossFactor();
			num2 *= num3;
		}
		else
		{
			float num4 = CharacterMainControl.EquipmentRepairLossFactor();
			num2 *= num4;
		}
		return num2;
	}

	// Token: 0x0600081F RID: 2079 RVA: 0x000247A1 File Offset: 0x000229A1
	internal static void NotifyPlayerItemOperation()
	{
		Action onPlayerItemOperation = ItemUtilities.OnPlayerItemOperation;
		if (onPlayerItemOperation == null)
		{
			return;
		}
		onPlayerItemOperation();
	}
}
