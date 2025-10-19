using System;
using System.Collections.Generic;
using System.Linq;
using ItemStatsSystem;
using Saves;
using UnityEngine;

namespace Duckov
{
	// Token: 0x0200022D RID: 557
	public class ItemShortcut : MonoBehaviour
	{
		// Token: 0x17000302 RID: 770
		// (get) Token: 0x06001145 RID: 4421 RVA: 0x000430EC File Offset: 0x000412EC
		private static CharacterMainControl Master
		{
			get
			{
				return CharacterMainControl.Main;
			}
		}

		// Token: 0x17000303 RID: 771
		// (get) Token: 0x06001146 RID: 4422 RVA: 0x000430F3 File Offset: 0x000412F3
		private static Inventory MainInventory
		{
			get
			{
				if (ItemShortcut.Master == null)
				{
					return null;
				}
				if (!ItemShortcut.Master.CharacterItem)
				{
					return null;
				}
				return ItemShortcut.Master.CharacterItem.Inventory;
			}
		}

		// Token: 0x17000304 RID: 772
		// (get) Token: 0x06001147 RID: 4423 RVA: 0x00043126 File Offset: 0x00041326
		public static int MaxIndex
		{
			get
			{
				if (ItemShortcut.Instance == null)
				{
					return 0;
				}
				return ItemShortcut.Instance.maxIndex;
			}
		}

		// Token: 0x06001148 RID: 4424 RVA: 0x00043144 File Offset: 0x00041344
		private void Awake()
		{
			if (ItemShortcut.Instance == null)
			{
				ItemShortcut.Instance = this;
			}
			else
			{
				Debug.LogError("检测到多个ItemShortcut");
			}
			SavesSystem.OnCollectSaveData += this.OnCollectSaveData;
			SavesSystem.OnSetFile += this.OnSetSaveFile;
			LevelManager.OnLevelInitialized += this.OnLevelInitialized;
		}

		// Token: 0x06001149 RID: 4425 RVA: 0x000431A3 File Offset: 0x000413A3
		private void OnDestroy()
		{
			SavesSystem.OnCollectSaveData -= this.OnCollectSaveData;
			SavesSystem.OnSetFile -= this.OnSetSaveFile;
			LevelManager.OnLevelInitialized -= this.OnLevelInitialized;
		}

		// Token: 0x0600114A RID: 4426 RVA: 0x000431D8 File Offset: 0x000413D8
		private void Start()
		{
			this.Load();
		}

		// Token: 0x0600114B RID: 4427 RVA: 0x000431E0 File Offset: 0x000413E0
		private void OnLevelInitialized()
		{
			this.Load();
		}

		// Token: 0x0600114C RID: 4428 RVA: 0x000431E8 File Offset: 0x000413E8
		private void OnSetSaveFile()
		{
			this.Load();
		}

		// Token: 0x0600114D RID: 4429 RVA: 0x000431F0 File Offset: 0x000413F0
		private void OnCollectSaveData()
		{
			this.Save();
		}

		// Token: 0x0600114E RID: 4430 RVA: 0x000431F8 File Offset: 0x000413F8
		private void Load()
		{
			ItemShortcut.SaveData saveData = SavesSystem.Load<ItemShortcut.SaveData>("ItemShortcut_Data");
			if (saveData == null)
			{
				return;
			}
			saveData.ApplyTo(this);
		}

		// Token: 0x0600114F RID: 4431 RVA: 0x00043210 File Offset: 0x00041410
		private void Save()
		{
			ItemShortcut.SaveData saveData = new ItemShortcut.SaveData();
			saveData.Generate(this);
			SavesSystem.Save<ItemShortcut.SaveData>("ItemShortcut_Data", saveData);
		}

		// Token: 0x06001150 RID: 4432 RVA: 0x00043238 File Offset: 0x00041438
		public static bool IsItemValid(Item item)
		{
			return !(item == null) && !(ItemShortcut.MainInventory == null) && !(ItemShortcut.MainInventory != item.InInventory) && !item.Tags.Contains("Weapon");
		}

		// Token: 0x06001151 RID: 4433 RVA: 0x00043288 File Offset: 0x00041488
		private bool Set_Local(int index, Item item)
		{
			if (ItemShortcut.Master == null)
			{
				return false;
			}
			if (index < 0 || index > this.maxIndex)
			{
				return false;
			}
			if (!ItemShortcut.IsItemValid(item))
			{
				return false;
			}
			while (this.items.Count <= index)
			{
				this.items.Add(null);
			}
			while (this.itemTypes.Count <= index)
			{
				this.itemTypes.Add(-1);
			}
			this.items[index] = item;
			this.itemTypes[index] = item.TypeID;
			Action<int> onSetItem = ItemShortcut.OnSetItem;
			if (onSetItem != null)
			{
				onSetItem(index);
			}
			for (int i = 0; i < this.items.Count; i++)
			{
				if (i != index)
				{
					bool flag = false;
					if (this.items[i] == item)
					{
						this.items[i] = null;
						flag = true;
					}
					if (this.itemTypes[i] == item.TypeID)
					{
						this.itemTypes[i] = -1;
						this.items[i] = null;
						flag = true;
					}
					if (flag)
					{
						ItemShortcut.OnSetItem(i);
					}
				}
			}
			return true;
		}

		// Token: 0x06001152 RID: 4434 RVA: 0x000433A4 File Offset: 0x000415A4
		private Item Get_Local(int index)
		{
			if (index >= this.items.Count)
			{
				return null;
			}
			Item item = this.items[index];
			if (item == null)
			{
				item = ItemShortcut.MainInventory.Find(this.itemTypes[index]);
				if (item != null)
				{
					this.items[index] = item;
				}
			}
			if (!ItemShortcut.IsItemValid(item))
			{
				this.SetDirty(index);
				return null;
			}
			return item;
		}

		// Token: 0x06001153 RID: 4435 RVA: 0x00043416 File Offset: 0x00041616
		private void SetDirty(int index)
		{
			this.dirtyIndexes.Add(index);
		}

		// Token: 0x06001154 RID: 4436 RVA: 0x00043428 File Offset: 0x00041628
		private void Update()
		{
			if (this.dirtyIndexes.Count > 0)
			{
				foreach (int num in this.dirtyIndexes.ToArray<int>())
				{
					if (num < this.items.Count && !ItemShortcut.IsItemValid(this.items[num]))
					{
						this.items[num] = null;
						Action<int> onSetItem = ItemShortcut.OnSetItem;
						if (onSetItem != null)
						{
							onSetItem(num);
						}
					}
				}
				this.dirtyIndexes.Clear();
			}
		}

		// Token: 0x14000074 RID: 116
		// (add) Token: 0x06001155 RID: 4437 RVA: 0x000434AC File Offset: 0x000416AC
		// (remove) Token: 0x06001156 RID: 4438 RVA: 0x000434E0 File Offset: 0x000416E0
		public static event Action<int> OnSetItem;

		// Token: 0x06001157 RID: 4439 RVA: 0x00043513 File Offset: 0x00041713
		public static Item Get(int index)
		{
			if (ItemShortcut.Instance == null)
			{
				return null;
			}
			return ItemShortcut.Instance.Get_Local(index);
		}

		// Token: 0x06001158 RID: 4440 RVA: 0x0004352F File Offset: 0x0004172F
		public static bool Set(int index, Item item)
		{
			return !(ItemShortcut.Instance == null) && ItemShortcut.Instance.Set_Local(index, item);
		}

		// Token: 0x04000D70 RID: 3440
		public static ItemShortcut Instance;

		// Token: 0x04000D71 RID: 3441
		[SerializeField]
		private int maxIndex = 3;

		// Token: 0x04000D72 RID: 3442
		[SerializeField]
		private List<Item> items = new List<Item>();

		// Token: 0x04000D73 RID: 3443
		[SerializeField]
		private List<int> itemTypes = new List<int>();

		// Token: 0x04000D74 RID: 3444
		private const string SaveKey = "ItemShortcut_Data";

		// Token: 0x04000D75 RID: 3445
		private HashSet<int> dirtyIndexes = new HashSet<int>();

		// Token: 0x02000524 RID: 1316
		[Serializable]
		private class SaveData
		{
			// Token: 0x17000752 RID: 1874
			// (get) Token: 0x06002780 RID: 10112 RVA: 0x0009034F File Offset: 0x0008E54F
			public int Count
			{
				get
				{
					return this.inventoryIndexes.Count;
				}
			}

			// Token: 0x06002781 RID: 10113 RVA: 0x0009035C File Offset: 0x0008E55C
			public void Generate(ItemShortcut shortcut)
			{
				this.inventoryIndexes.Clear();
				Inventory mainInventory = ItemShortcut.MainInventory;
				if (mainInventory == null)
				{
					return;
				}
				for (int i = 0; i < shortcut.items.Count; i++)
				{
					Item item = shortcut.items[i];
					int index = mainInventory.GetIndex(item);
					this.inventoryIndexes.Add(index);
				}
			}

			// Token: 0x06002782 RID: 10114 RVA: 0x000903BC File Offset: 0x0008E5BC
			public void ApplyTo(ItemShortcut shortcut)
			{
				Inventory mainInventory = ItemShortcut.MainInventory;
				if (mainInventory == null)
				{
					return;
				}
				for (int i = 0; i < this.inventoryIndexes.Count; i++)
				{
					int num = this.inventoryIndexes[i];
					if (num >= 0)
					{
						Item itemAt = mainInventory.GetItemAt(num);
						shortcut.Set_Local(i, itemAt);
					}
				}
			}

			// Token: 0x04001E41 RID: 7745
			[SerializeField]
			internal List<int> inventoryIndexes = new List<int>();
		}
	}
}
