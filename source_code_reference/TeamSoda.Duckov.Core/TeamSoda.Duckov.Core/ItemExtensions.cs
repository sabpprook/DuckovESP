using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Duckov;
using Duckov.Scenes;
using Duckov.Utilities;
using ItemStatsSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x020000E9 RID: 233
public static class ItemExtensions
{
	// Token: 0x060007BA RID: 1978 RVA: 0x00022900 File Offset: 0x00020B00
	private static ItemAgent CreatePickupAgent(this Item itemInstance, Vector3 pos)
	{
		if (itemInstance.ActiveAgent != null)
		{
			Debug.LogError("创建pickup agent失败,已有agent:" + itemInstance.ActiveAgent.name);
			return null;
		}
		ItemAgent itemAgent = itemInstance.AgentUtilities.GetPrefab(ItemExtensions.PickupHash);
		if (itemAgent == null)
		{
			itemAgent = GameplayDataSettings.Prefabs.PickupAgentPrefab;
		}
		ItemAgent itemAgent2 = itemInstance.AgentUtilities.CreateAgent(itemAgent, ItemAgent.AgentTypes.pickUp);
		itemAgent2.transform.position = pos;
		return itemAgent2;
	}

	// Token: 0x060007BB RID: 1979 RVA: 0x00022978 File Offset: 0x00020B78
	public static ItemAgent CreateHandheldAgent(this Item itemInstance)
	{
		if (itemInstance.ActiveAgent != null)
		{
			Debug.LogError("创建pickup agent失败,已有agent");
			return null;
		}
		ItemAgent itemAgent = itemInstance.AgentUtilities.GetPrefab(ItemExtensions.HandheldHash);
		if (itemAgent == null)
		{
			itemAgent = GameplayDataSettings.Prefabs.HandheldAgentPrefab;
		}
		return itemInstance.AgentUtilities.CreateAgent(itemAgent, ItemAgent.AgentTypes.handheld);
	}

	// Token: 0x060007BC RID: 1980 RVA: 0x000229D4 File Offset: 0x00020BD4
	public static DuckovItemAgent Drop(this Item item, Vector3 pos, bool createRigidbody, Vector3 dropDirection, float randomAngle)
	{
		if (item == null)
		{
			Debug.Log("尝试丢弃不存在的物体");
			return null;
		}
		item.Detach();
		if (MultiSceneCore.MainScene != null)
		{
			item.gameObject.transform.SetParent(null);
			SceneManager.MoveGameObjectToScene(item.gameObject, MultiSceneCore.MainScene.Value);
		}
		ItemAgent itemAgent = item.CreatePickupAgent(pos);
		if (MultiSceneCore.Instance)
		{
			if (itemAgent == null)
			{
				Debug.Log("创建的agent是null");
			}
			MultiSceneCore.MoveToActiveWithScene(itemAgent.gameObject, SceneManager.GetActiveScene().buildIndex);
		}
		InteractablePickup component = itemAgent.GetComponent<InteractablePickup>();
		if (createRigidbody && component != null)
		{
			component.Throw(dropDirection, randomAngle);
		}
		else
		{
			component.transform.rotation = Quaternion.Euler(0f, global::UnityEngine.Random.Range(-randomAngle, randomAngle) * 0.5f, 0f);
		}
		return itemAgent as DuckovItemAgent;
	}

	// Token: 0x060007BD RID: 1981 RVA: 0x00022AC4 File Offset: 0x00020CC4
	public static void Drop(this Item item, CharacterMainControl character, bool createRigidbody)
	{
		if (item == null)
		{
			return;
		}
		(global::UnityEngine.Random.insideUnitSphere * 1f).y = 0f;
		item.Drop(character.transform.position, createRigidbody, character.CurrentAimDirection, 45f);
		if (character.IsMainCharacter && LevelManager.LevelInited)
		{
			AudioManager.Post("SFX/Item/put_" + item.SoundKey, character.gameObject);
		}
	}

	// Token: 0x060007BE RID: 1982 RVA: 0x00022B40 File Offset: 0x00020D40
	public static async UniTask<List<Item>> GetItemsOfAmount(this Inventory inventory, int itemTypeID, int amount)
	{
		List<Item> list = inventory.FindAll((Item e) => e != null && e.TypeID == itemTypeID);
		List<Item> result = new List<Item>();
		int count = 0;
		foreach (Item item in list)
		{
			int remainingCount = amount - count;
			if (item.StackCount > remainingCount)
			{
				Item item2 = await item.Split(remainingCount);
				result.Add(item2);
				count += remainingCount;
			}
			else
			{
				item.Detach();
				result.Add(item);
				count += item.StackCount;
			}
			if (count >= amount)
			{
				break;
			}
		}
		List<Item>.Enumerator enumerator = default(List<Item>.Enumerator);
		return result;
	}

	// Token: 0x060007BF RID: 1983 RVA: 0x00022B94 File Offset: 0x00020D94
	public static bool TryFindItemsOfAmount(this IEnumerable<Inventory> inventories, int itemTypeID, int requiredAmount, out List<Item> result)
	{
		result = new List<Item>();
		int num = 0;
		foreach (Inventory inventory in inventories)
		{
			foreach (Item item in inventory)
			{
				if (item.TypeID == itemTypeID)
				{
					result.Add(item);
					num += item.StackCount;
					if (num >= requiredAmount)
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	// Token: 0x060007C0 RID: 1984 RVA: 0x00022C34 File Offset: 0x00020E34
	public static void ConsumeItemsOfAmount(this IEnumerable<Item> itemsToBeConsumed, int amount)
	{
		List<Item> list = new List<Item>();
		int num = 0;
		foreach (Item item in itemsToBeConsumed)
		{
			list.Add(item);
			num += item.StackCount;
			if (num >= amount)
			{
				break;
			}
		}
		num = 0;
		for (int i = 0; i < list.Count; i++)
		{
			Item item2 = list[i];
			int num2 = amount - num;
			if (num2 < item2.StackCount)
			{
				item2.StackCount -= num2;
				return;
			}
			item2.Detach();
			item2.DestroyTree();
		}
	}

	// Token: 0x060007C1 RID: 1985 RVA: 0x00022CE8 File Offset: 0x00020EE8
	private static bool TryMerge(IEnumerable<Item> itemsOfSameTypeID, out List<Item> result)
	{
		result = null;
		List<Item> list = itemsOfSameTypeID.ToList<Item>();
		list.RemoveAll((Item e) => e == null);
		if (list.Count <= 0)
		{
			return false;
		}
		int typeID = list[0].TypeID;
		foreach (Item item in list)
		{
			if (typeID != item.TypeID)
			{
				Debug.LogError("尝试融合的Item具有不同的TypeID,已取消");
				return false;
			}
		}
		if (!list[0].Stackable)
		{
			Debug.LogError("此类物品不可堆叠，已取消");
			return false;
		}
		result = new List<Item>();
		Stack<Item> stack = new Stack<Item>(list);
		Item item2 = null;
		while (stack.Count > 0)
		{
			if (item2 == null)
			{
				item2 = stack.Pop();
			}
			if (stack.Count <= 0)
			{
				result.Add(item2);
				break;
			}
			Item item3 = null;
			while (item2.StackCount < item2.MaxStackCount && stack.Count > 0)
			{
				item3 = stack.Pop();
				item2.Combine(item3);
			}
			result.Add(item2);
			if (item3 != null && item3.StackCount > 0)
			{
				if (stack.Count <= 0)
				{
					result.Add(item3);
					break;
				}
				item2 = item3;
			}
			else
			{
				item2 = null;
			}
		}
		return true;
	}

	// Token: 0x04000741 RID: 1857
	public static readonly int PickupHash = "Pickup".GetHashCode();

	// Token: 0x04000742 RID: 1858
	public static readonly int HandheldHash = "Handheld".GetHashCode();
}
