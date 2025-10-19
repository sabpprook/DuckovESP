using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Duckov.UI;
using ItemStatsSystem;
using ItemStatsSystem.Data;
using Saves;
using SodaCraft.Localizations;
using SodaCraft.StringUtilities;
using UnityEngine;

// Token: 0x020000F6 RID: 246
public class PlayerStorage : MonoBehaviour, IInitializedQueryHandler
{
	// Token: 0x170001AE RID: 430
	// (get) Token: 0x06000820 RID: 2080 RVA: 0x000247B2 File Offset: 0x000229B2
	// (set) Token: 0x06000821 RID: 2081 RVA: 0x000247B9 File Offset: 0x000229B9
	public static PlayerStorage Instance { get; private set; }

	// Token: 0x14000034 RID: 52
	// (add) Token: 0x06000822 RID: 2082 RVA: 0x000247C4 File Offset: 0x000229C4
	// (remove) Token: 0x06000823 RID: 2083 RVA: 0x000247F8 File Offset: 0x000229F8
	public static event Action<PlayerStorage, Inventory, int> OnPlayerStorageChange;

	// Token: 0x170001AF RID: 431
	// (get) Token: 0x06000824 RID: 2084 RVA: 0x0002482B File Offset: 0x00022A2B
	public static Inventory Inventory
	{
		get
		{
			if (PlayerStorage.Instance == null)
			{
				return null;
			}
			return PlayerStorage.Instance.inventory;
		}
	}

	// Token: 0x170001B0 RID: 432
	// (get) Token: 0x06000825 RID: 2085 RVA: 0x00024846 File Offset: 0x00022A46
	public static List<ItemTreeData> IncomingItemBuffer
	{
		get
		{
			return PlayerStorageBuffer.Buffer;
		}
	}

	// Token: 0x170001B1 RID: 433
	// (get) Token: 0x06000826 RID: 2086 RVA: 0x0002484D File Offset: 0x00022A4D
	public InteractableLootbox InteractableLootBox
	{
		get
		{
			return this.interactable;
		}
	}

	// Token: 0x14000035 RID: 53
	// (add) Token: 0x06000827 RID: 2087 RVA: 0x00024858 File Offset: 0x00022A58
	// (remove) Token: 0x06000828 RID: 2088 RVA: 0x0002488C File Offset: 0x00022A8C
	public static event Action<PlayerStorage.StorageCapacityCalculationHolder> OnRecalculateStorageCapacity;

	// Token: 0x14000036 RID: 54
	// (add) Token: 0x06000829 RID: 2089 RVA: 0x000248C0 File Offset: 0x00022AC0
	// (remove) Token: 0x0600082A RID: 2090 RVA: 0x000248F4 File Offset: 0x00022AF4
	public static event Action OnTakeBufferItem;

	// Token: 0x14000037 RID: 55
	// (add) Token: 0x0600082B RID: 2091 RVA: 0x00024928 File Offset: 0x00022B28
	// (remove) Token: 0x0600082C RID: 2092 RVA: 0x0002495C File Offset: 0x00022B5C
	public static event Action<Item> OnItemAddedToBuffer;

	// Token: 0x14000038 RID: 56
	// (add) Token: 0x0600082D RID: 2093 RVA: 0x00024990 File Offset: 0x00022B90
	// (remove) Token: 0x0600082E RID: 2094 RVA: 0x000249C4 File Offset: 0x00022BC4
	public static event Action OnLoadingFinished;

	// Token: 0x0600082F RID: 2095 RVA: 0x000249F7 File Offset: 0x00022BF7
	public static bool IsAccessableAndNotFull()
	{
		return !(PlayerStorage.Instance == null) && !(PlayerStorage.Inventory == null) && PlayerStorage.Inventory.GetFirstEmptyPosition(0) >= 0;
	}

	// Token: 0x170001B2 RID: 434
	// (get) Token: 0x06000830 RID: 2096 RVA: 0x00024A28 File Offset: 0x00022C28
	public int DefaultCapacity
	{
		get
		{
			return this.defaultCapacity;
		}
	}

	// Token: 0x06000831 RID: 2097 RVA: 0x00024A30 File Offset: 0x00022C30
	public static void NotifyCapacityDirty()
	{
		PlayerStorage.needRecalculateCapacity = true;
	}

	// Token: 0x06000832 RID: 2098 RVA: 0x00024A38 File Offset: 0x00022C38
	private void Awake()
	{
		if (PlayerStorage.Instance == null)
		{
			PlayerStorage.Instance = this;
		}
		if (PlayerStorage.Instance != this)
		{
			Debug.LogError("发现了多个Player Storage!");
			return;
		}
		if (this.interactable == null)
		{
			this.interactable = base.GetComponent<InteractableLootbox>();
		}
		this.inventory.onContentChanged += this.OnInventoryContentChanged;
		SavesSystem.OnCollectSaveData += this.SavesSystem_OnCollectSaveData;
		LevelManager.RegisterWaitForInitialization<PlayerStorage>(this);
	}

	// Token: 0x06000833 RID: 2099 RVA: 0x00024AB8 File Offset: 0x00022CB8
	private void Start()
	{
		this.Load().Forget();
	}

	// Token: 0x06000834 RID: 2100 RVA: 0x00024AC5 File Offset: 0x00022CC5
	private void OnDestroy()
	{
		this.inventory.onContentChanged -= this.OnInventoryContentChanged;
		SavesSystem.OnCollectSaveData -= this.SavesSystem_OnCollectSaveData;
		LevelManager.UnregisterWaitForInitialization<PlayerStorage>(this);
	}

	// Token: 0x06000835 RID: 2101 RVA: 0x00024AF6 File Offset: 0x00022CF6
	private void SavesSystem_OnSetFile()
	{
		this.Load().Forget();
	}

	// Token: 0x06000836 RID: 2102 RVA: 0x00024B03 File Offset: 0x00022D03
	private void SavesSystem_OnCollectSaveData()
	{
		this.Save();
	}

	// Token: 0x06000837 RID: 2103 RVA: 0x00024B0B File Offset: 0x00022D0B
	private void OnInventoryContentChanged(Inventory inventory, int index)
	{
		Action<PlayerStorage, Inventory, int> onPlayerStorageChange = PlayerStorage.OnPlayerStorageChange;
		if (onPlayerStorageChange == null)
		{
			return;
		}
		onPlayerStorageChange(this, inventory, index);
	}

	// Token: 0x06000838 RID: 2104 RVA: 0x00024B20 File Offset: 0x00022D20
	public static void Push(Item item, bool toBufferDirectly = false)
	{
		if (item == null)
		{
			return;
		}
		if (!toBufferDirectly && PlayerStorage.Inventory != null)
		{
			if (item.Stackable)
			{
				Func<Item, bool> <>9__0;
				while (item.StackCount > 0)
				{
					IEnumerable<Item> enumerable = PlayerStorage.Inventory;
					Func<Item, bool> func;
					if ((func = <>9__0) == null)
					{
						func = (<>9__0 = (Item e) => e.TypeID == item.TypeID && e.MaxStackCount > e.StackCount);
					}
					Item item2 = enumerable.FirstOrDefault(func);
					if (item2 == null)
					{
						break;
					}
					item2.Combine(item);
				}
			}
			if (item != null && item.StackCount > 0)
			{
				int firstEmptyPosition = PlayerStorage.Inventory.GetFirstEmptyPosition(0);
				if (firstEmptyPosition >= 0)
				{
					PlayerStorage.Inventory.AddAt(item, firstEmptyPosition);
					return;
				}
			}
		}
		NotificationText.Push("PlayerStorage_Notification_ItemAddedToBuffer".ToPlainText().Format(new
		{
			displayName = item.DisplayName
		}));
		PlayerStorage.IncomingItemBuffer.Add(ItemTreeData.FromItem(item));
		item.Detach();
		item.DestroyTree();
		Action<Item> onItemAddedToBuffer = PlayerStorage.OnItemAddedToBuffer;
		if (onItemAddedToBuffer == null)
		{
			return;
		}
		onItemAddedToBuffer(item);
	}

	// Token: 0x06000839 RID: 2105 RVA: 0x00024C5E File Offset: 0x00022E5E
	private void Save()
	{
		if (PlayerStorage.Loading)
		{
			return;
		}
		this.inventory.Save("PlayerStorage");
	}

	// Token: 0x170001B3 RID: 435
	// (get) Token: 0x0600083A RID: 2106 RVA: 0x00024C78 File Offset: 0x00022E78
	// (set) Token: 0x0600083B RID: 2107 RVA: 0x00024C7F File Offset: 0x00022E7F
	public static bool Loading { get; private set; }

	// Token: 0x170001B4 RID: 436
	// (get) Token: 0x0600083C RID: 2108 RVA: 0x00024C87 File Offset: 0x00022E87
	// (set) Token: 0x0600083D RID: 2109 RVA: 0x00024C8E File Offset: 0x00022E8E
	public static bool TakingItem { get; private set; }

	// Token: 0x0600083E RID: 2110 RVA: 0x00024C98 File Offset: 0x00022E98
	private async UniTask Load()
	{
		PlayerStorage.Loading = true;
		this.inventory.SetCapacity(16384);
		await ItemSavesUtilities.LoadInventory("PlayerStorage", this.inventory);
		PlayerStorage.RecalculateStorageCapacity();
		PlayerStorage.Loading = false;
		Action onLoadingFinished = PlayerStorage.OnLoadingFinished;
		if (onLoadingFinished != null)
		{
			onLoadingFinished();
		}
		this.initialized = true;
	}

	// Token: 0x0600083F RID: 2111 RVA: 0x00024CDB File Offset: 0x00022EDB
	private void Update()
	{
		if (PlayerStorage.needRecalculateCapacity)
		{
			PlayerStorage.RecalculateStorageCapacity();
		}
	}

	// Token: 0x06000840 RID: 2112 RVA: 0x00024CEC File Offset: 0x00022EEC
	public static int RecalculateStorageCapacity()
	{
		if (PlayerStorage.Instance == null)
		{
			return 0;
		}
		PlayerStorage.StorageCapacityCalculationHolder storageCapacityCalculationHolder = new PlayerStorage.StorageCapacityCalculationHolder();
		storageCapacityCalculationHolder.capacity = PlayerStorage.Instance.DefaultCapacity;
		Action<PlayerStorage.StorageCapacityCalculationHolder> onRecalculateStorageCapacity = PlayerStorage.OnRecalculateStorageCapacity;
		if (onRecalculateStorageCapacity != null)
		{
			onRecalculateStorageCapacity(storageCapacityCalculationHolder);
		}
		int capacity = storageCapacityCalculationHolder.capacity;
		PlayerStorage.Instance.SetCapacity(capacity);
		PlayerStorage.needRecalculateCapacity = false;
		return capacity;
	}

	// Token: 0x06000841 RID: 2113 RVA: 0x00024D48 File Offset: 0x00022F48
	private void SetCapacity(int capacity)
	{
		this.inventory.SetCapacity(capacity);
	}

	// Token: 0x06000842 RID: 2114 RVA: 0x00024D58 File Offset: 0x00022F58
	public static async UniTask TakeBufferItem(int index)
	{
		if (!PlayerStorage.Loading)
		{
			if (!PlayerStorage.TakingItem)
			{
				if (index >= 0)
				{
					if (index <= PlayerStorage.IncomingItemBuffer.Count)
					{
						if (PlayerStorage.IsAccessableAndNotFull())
						{
							ItemTreeData itemTreeData = PlayerStorage.IncomingItemBuffer[index];
							if (itemTreeData != null)
							{
								PlayerStorage.TakingItem = true;
								Item item = await ItemTreeData.InstantiateAsync(itemTreeData);
								PlayerStorage.Inventory.AddAndMerge(item, 0);
								PlayerStorage.IncomingItemBuffer.RemoveAt(index);
								Action onTakeBufferItem = PlayerStorage.OnTakeBufferItem;
								if (onTakeBufferItem != null)
								{
									onTakeBufferItem();
								}
								PlayerStorage.TakingItem = false;
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x06000843 RID: 2115 RVA: 0x00024D9B File Offset: 0x00022F9B
	public bool HasInitialized()
	{
		return this.initialized;
	}

	// Token: 0x0400076F RID: 1903
	[SerializeField]
	private Inventory inventory;

	// Token: 0x04000770 RID: 1904
	[SerializeField]
	private InteractableLootbox interactable;

	// Token: 0x04000775 RID: 1909
	[SerializeField]
	private int defaultCapacity = 32;

	// Token: 0x04000776 RID: 1910
	private static bool needRecalculateCapacity;

	// Token: 0x04000777 RID: 1911
	private const string inventorySaveKey = "PlayerStorage";

	// Token: 0x0400077A RID: 1914
	private bool initialized;

	// Token: 0x02000478 RID: 1144
	public class StorageCapacityCalculationHolder
	{
		// Token: 0x04001B5B RID: 7003
		public int capacity;
	}
}
