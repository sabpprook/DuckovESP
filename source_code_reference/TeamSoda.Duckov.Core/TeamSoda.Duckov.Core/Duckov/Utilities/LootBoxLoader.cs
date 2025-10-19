using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Duckov.Scenes;
using ItemStatsSystem;
using UnityEngine;

namespace Duckov.Utilities
{
	// Token: 0x020003F8 RID: 1016
	public class LootBoxLoader : MonoBehaviour
	{
		// Token: 0x060024CD RID: 9421 RVA: 0x0007F372 File Offset: 0x0007D572
		public void CalculateChances()
		{
			this.randomPool.RefreshPercent();
		}

		// Token: 0x17000712 RID: 1810
		// (get) Token: 0x060024CE RID: 9422 RVA: 0x0007F37F File Offset: 0x0007D57F
		public List<int> FixedItems
		{
			get
			{
				return this.fixedItems;
			}
		}

		// Token: 0x17000713 RID: 1811
		// (get) Token: 0x060024CF RID: 9423 RVA: 0x0007F387 File Offset: 0x0007D587
		[SerializeField]
		private Inventory Inventory
		{
			get
			{
				if (this._lootBox == null)
				{
					this._lootBox = base.GetComponent<InteractableLootbox>();
					if (this._lootBox == null)
					{
						return null;
					}
				}
				return this._lootBox.Inventory;
			}
		}

		// Token: 0x060024D0 RID: 9424 RVA: 0x0007F3BE File Offset: 0x0007D5BE
		public static int[] Search(ItemFilter filter)
		{
			return ItemAssetsCollection.Search(filter);
		}

		// Token: 0x060024D1 RID: 9425 RVA: 0x0007F3C6 File Offset: 0x0007D5C6
		private void Awake()
		{
			if (this._lootBox == null)
			{
				this._lootBox = base.GetComponent<InteractableLootbox>();
			}
			this.RandomActive();
		}

		// Token: 0x060024D2 RID: 9426 RVA: 0x0007F3E8 File Offset: 0x0007D5E8
		private int GetKey()
		{
			Vector3 vector = base.transform.position * 10f;
			int num = Mathf.RoundToInt(vector.x);
			int num2 = Mathf.RoundToInt(vector.y);
			int num3 = Mathf.RoundToInt(vector.z);
			Vector3Int vector3Int = new Vector3Int(num, num2, num3);
			return string.Format("LootBoxLoader_{0}", vector3Int).GetHashCode();
		}

		// Token: 0x060024D3 RID: 9427 RVA: 0x0007F44C File Offset: 0x0007D64C
		private void RandomActive()
		{
			bool flag = false;
			int key = this.GetKey();
			object obj;
			if (MultiSceneCore.Instance.inLevelData.TryGetValue(key, out obj))
			{
				if (obj is bool)
				{
					bool flag2 = (bool)obj;
					flag = flag2;
				}
			}
			else
			{
				flag = global::UnityEngine.Random.Range(0f, 1f) < this.activeChance;
				MultiSceneCore.Instance.inLevelData.Add(key, flag);
			}
			base.gameObject.SetActive(flag);
		}

		// Token: 0x060024D4 RID: 9428 RVA: 0x0007F4C3 File Offset: 0x0007D6C3
		public void StartSetup()
		{
			this.Setup().Forget();
		}

		// Token: 0x060024D5 RID: 9429 RVA: 0x0007F4D0 File Offset: 0x0007D6D0
		public async UniTask Setup()
		{
			if (!(this.Inventory == null))
			{
				if (GameMetaData.Instance.IsDemo)
				{
					this.excludeTags.Add(GameplayDataSettings.Tags.LockInDemoTag);
				}
				if (!this.excludeTags.Contains(GameplayDataSettings.Tags.Special))
				{
					this.excludeTags.Add(GameplayDataSettings.Tags.Special);
				}
				if (!LevelManager.Rule.AdvancedDebuffMode && !this.excludeTags.Contains(GameplayDataSettings.Tags.AdvancedDebuffMode))
				{
					this.excludeTags.Add(GameplayDataSettings.Tags.AdvancedDebuffMode);
				}
				int num = Mathf.RoundToInt(global::UnityEngine.Random.Range((float)this.randomCount.x - 0.5f, (float)this.randomCount.y + 0.5f) * LevelConfig.Instance.LootboxItemCountMultiplier);
				if (this.randomCount.x <= 0 && this.randomCount.y <= 0)
				{
					num = 0;
				}
				List<int> list = new List<int>();
				if (global::UnityEngine.Random.Range(0f, 1f) < this.fixedItemSpawnChance && this.fixedItems.Count > 0)
				{
					list.AddRange(this.fixedItems);
				}
				if (this.randomFromPool)
				{
					for (int i = 0; i < num; i++)
					{
						LootBoxLoader.Entry random = this.randomPool.GetRandom(0f);
						list.Add(random.itemTypeID);
					}
				}
				else
				{
					float lootBoxQualityLowPercent = LevelConfig.Instance.LootBoxQualityLowPercent;
					for (int j = 0; j < num; j++)
					{
						Tag random2 = this.tags.GetRandom(0f);
						int random3 = this.qualities.GetRandom(lootBoxQualityLowPercent);
						int[] array = LootBoxLoader.Search(new ItemFilter
						{
							requireTags = new Tag[] { random2 },
							excludeTags = this.excludeTags.ToArray(),
							minQuality = random3,
							maxQuality = random3
						});
						if (array.Length >= 1)
						{
							int random4 = array.GetRandom<int>();
							list.Add(random4);
						}
					}
				}
				if (this.inventorySize < list.Count)
				{
					this.inventorySize = list.Count;
				}
				this.Inventory.SetCapacity(this.inventorySize);
				this.Inventory.Loading = true;
				foreach (int num2 in list)
				{
					if (num2 > 0)
					{
						Item item = await ItemAssetsCollection.InstantiateAsync(num2);
						if (this.dropOnSpawnItem || !this.Inventory.AddItem(item))
						{
							item.Drop(base.transform.position + Vector3.up, true, (global::UnityEngine.Random.insideUnitSphere + Vector3.up) * 2f, 45f);
						}
					}
				}
				List<int>.Enumerator enumerator = default(List<int>.Enumerator);
				await this.CreateCash();
				this.Inventory.Loading = false;
				this._lootBox.CheckHideIfEmpty();
			}
		}

		// Token: 0x060024D6 RID: 9430 RVA: 0x0007F514 File Offset: 0x0007D714
		private async UniTask CreateCash()
		{
			if (global::UnityEngine.Random.Range(0f, 1f) <= this.GenrateCashChance)
			{
				int cashCount = global::UnityEngine.Random.Range(1, this.maxRandomCash);
				int firstEmptyPosition = this.Inventory.GetFirstEmptyPosition(0);
				int capacity = this.Inventory.Capacity;
				if (firstEmptyPosition >= capacity)
				{
					this.Inventory.SetCapacity(capacity + 1);
				}
				Item item = await ItemAssetsCollection.InstantiateAsync(GameplayDataSettings.ItemAssets.CashItemTypeID);
				item.StackCount = cashCount;
				if (this.dropOnSpawnItem || !this.Inventory.AddItem(item))
				{
					item.Drop(base.transform.position + Vector3.up, true, (global::UnityEngine.Random.insideUnitSphere + Vector3.up) * 2f, 45f);
				}
			}
		}

		// Token: 0x060024D7 RID: 9431 RVA: 0x0007F557 File Offset: 0x0007D757
		private void OnValidate()
		{
			this.tags.RefreshPercent();
			this.qualities.RefreshPercent();
		}

		// Token: 0x04001916 RID: 6422
		public bool autoSetup = true;

		// Token: 0x04001917 RID: 6423
		public bool dropOnSpawnItem;

		// Token: 0x04001918 RID: 6424
		[SerializeField]
		[Range(0f, 1f)]
		private float activeChance = 1f;

		// Token: 0x04001919 RID: 6425
		[SerializeField]
		private int inventorySize = 8;

		// Token: 0x0400191A RID: 6426
		[SerializeField]
		private Vector2Int randomCount = new Vector2Int(1, 1);

		// Token: 0x0400191B RID: 6427
		public bool randomFromPool;

		// Token: 0x0400191C RID: 6428
		[SerializeField]
		private RandomContainer<Tag> tags;

		// Token: 0x0400191D RID: 6429
		[SerializeField]
		private List<Tag> excludeTags;

		// Token: 0x0400191E RID: 6430
		[SerializeField]
		private RandomContainer<int> qualities;

		// Token: 0x0400191F RID: 6431
		[SerializeField]
		private RandomContainer<LootBoxLoader.Entry> randomPool;

		// Token: 0x04001920 RID: 6432
		[Range(0f, 1f)]
		public float GenrateCashChance;

		// Token: 0x04001921 RID: 6433
		public int maxRandomCash;

		// Token: 0x04001922 RID: 6434
		[ItemTypeID]
		[SerializeField]
		private List<int> fixedItems;

		// Token: 0x04001923 RID: 6435
		[Range(0f, 1f)]
		[SerializeField]
		private float fixedItemSpawnChance = 1f;

		// Token: 0x04001924 RID: 6436
		[SerializeField]
		private InteractableLootbox _lootBox;

		// Token: 0x0200065F RID: 1631
		[Serializable]
		private struct Entry
		{
			// Token: 0x040022EE RID: 8942
			[ItemTypeID]
			[SerializeField]
			public int itemTypeID;
		}
	}
}
