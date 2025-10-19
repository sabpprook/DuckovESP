using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Cysharp.Threading.Tasks;
using Duckov.Economy;
using Duckov.Utilities;
using ItemStatsSystem;
using Saves;
using UnityEngine;

namespace Duckov.BlackMarkets
{
	// Token: 0x02000304 RID: 772
	public class BlackMarket : MonoBehaviour
	{
		// Token: 0x1700048D RID: 1165
		// (get) Token: 0x06001914 RID: 6420 RVA: 0x0005B1DF File Offset: 0x000593DF
		// (set) Token: 0x06001915 RID: 6421 RVA: 0x0005B1E6 File Offset: 0x000593E6
		public static BlackMarket Instance { get; private set; }

		// Token: 0x1700048E RID: 1166
		// (get) Token: 0x06001916 RID: 6422 RVA: 0x0005B1EE File Offset: 0x000593EE
		// (set) Token: 0x06001917 RID: 6423 RVA: 0x0005B201 File Offset: 0x00059401
		public int RefreshChance
		{
			get
			{
				return Mathf.Min(this.refreshChance, this.MaxRefreshChance);
			}
			set
			{
				this.refreshChance = value;
				Action<BlackMarket> action = BlackMarket.onRefreshChanceChanged;
				if (action == null)
				{
					return;
				}
				action(this);
			}
		}

		// Token: 0x1400009F RID: 159
		// (add) Token: 0x06001918 RID: 6424 RVA: 0x0005B21C File Offset: 0x0005941C
		// (remove) Token: 0x06001919 RID: 6425 RVA: 0x0005B250 File Offset: 0x00059450
		public static event Action<BlackMarket> onRefreshChanceChanged;

		// Token: 0x140000A0 RID: 160
		// (add) Token: 0x0600191A RID: 6426 RVA: 0x0005B284 File Offset: 0x00059484
		// (remove) Token: 0x0600191B RID: 6427 RVA: 0x0005B2B8 File Offset: 0x000594B8
		public static event Action<BlackMarket.OnRequestMaxRefreshChanceEventContext> onRequestMaxRefreshChance;

		// Token: 0x140000A1 RID: 161
		// (add) Token: 0x0600191C RID: 6428 RVA: 0x0005B2EC File Offset: 0x000594EC
		// (remove) Token: 0x0600191D RID: 6429 RVA: 0x0005B320 File Offset: 0x00059520
		public static event Action<BlackMarket.OnRequestRefreshTimeFactorEventContext> onRequestRefreshTime;

		// Token: 0x0600191E RID: 6430 RVA: 0x0005B353 File Offset: 0x00059553
		public static void NotifyMaxRefreshChanceChanged()
		{
			BlackMarket.dirty = true;
		}

		// Token: 0x1700048F RID: 1167
		// (get) Token: 0x0600191F RID: 6431 RVA: 0x0005B35C File Offset: 0x0005955C
		public int MaxRefreshChance
		{
			get
			{
				if (BlackMarket.dirty)
				{
					BlackMarket.OnRequestMaxRefreshChanceEventContext onRequestMaxRefreshChanceEventContext = new BlackMarket.OnRequestMaxRefreshChanceEventContext();
					onRequestMaxRefreshChanceEventContext.Add(1);
					Action<BlackMarket.OnRequestMaxRefreshChanceEventContext> action = BlackMarket.onRequestMaxRefreshChance;
					if (action != null)
					{
						action(onRequestMaxRefreshChanceEventContext);
					}
					this.cachedMaxRefreshChance = onRequestMaxRefreshChanceEventContext.Value;
				}
				return this.cachedMaxRefreshChance;
			}
		}

		// Token: 0x17000490 RID: 1168
		// (get) Token: 0x06001920 RID: 6432 RVA: 0x0005B3A0 File Offset: 0x000595A0
		private TimeSpan TimeToRefresh
		{
			get
			{
				BlackMarket.OnRequestRefreshTimeFactorEventContext onRequestRefreshTimeFactorEventContext = new BlackMarket.OnRequestRefreshTimeFactorEventContext();
				Action<BlackMarket.OnRequestRefreshTimeFactorEventContext> action = BlackMarket.onRequestRefreshTime;
				if (action != null)
				{
					action(onRequestRefreshTimeFactorEventContext);
				}
				float num = Mathf.Max(onRequestRefreshTimeFactorEventContext.Value, 0.01f);
				return TimeSpan.FromTicks((long)((float)this.timeToRefresh * num));
			}
		}

		// Token: 0x17000491 RID: 1169
		// (get) Token: 0x06001921 RID: 6433 RVA: 0x0005B3E4 File Offset: 0x000595E4
		// (set) Token: 0x06001922 RID: 6434 RVA: 0x0005B3F1 File Offset: 0x000595F1
		private DateTime LastRefreshedTime
		{
			get
			{
				return DateTime.FromBinary(this.lastRefreshedTimeRaw);
			}
			set
			{
				this.lastRefreshedTimeRaw = value.ToBinary();
			}
		}

		// Token: 0x17000492 RID: 1170
		// (get) Token: 0x06001923 RID: 6435 RVA: 0x0005B400 File Offset: 0x00059600
		private TimeSpan TimeSinceLastRefreshedTime
		{
			get
			{
				if (DateTime.UtcNow < this.LastRefreshedTime)
				{
					this.LastRefreshedTime = DateTime.UtcNow;
				}
				return DateTime.UtcNow - this.LastRefreshedTime;
			}
		}

		// Token: 0x17000493 RID: 1171
		// (get) Token: 0x06001924 RID: 6436 RVA: 0x0005B42F File Offset: 0x0005962F
		public TimeSpan RemainingTimeBeforeRefresh
		{
			get
			{
				return this.TimeToRefresh - this.TimeSinceLastRefreshedTime;
			}
		}

		// Token: 0x17000494 RID: 1172
		// (get) Token: 0x06001925 RID: 6437 RVA: 0x0005B442 File Offset: 0x00059642
		public ReadOnlyCollection<BlackMarket.DemandSupplyEntry> Demands
		{
			get
			{
				if (this._demands_readonly == null)
				{
					this._demands_readonly = new ReadOnlyCollection<BlackMarket.DemandSupplyEntry>(this.demands);
				}
				return this._demands_readonly;
			}
		}

		// Token: 0x17000495 RID: 1173
		// (get) Token: 0x06001926 RID: 6438 RVA: 0x0005B463 File Offset: 0x00059663
		public ReadOnlyCollection<BlackMarket.DemandSupplyEntry> Supplies
		{
			get
			{
				if (this._supplies_readonly == null)
				{
					this._supplies_readonly = new ReadOnlyCollection<BlackMarket.DemandSupplyEntry>(this.supplies);
				}
				return this._supplies_readonly;
			}
		}

		// Token: 0x140000A2 RID: 162
		// (add) Token: 0x06001927 RID: 6439 RVA: 0x0005B484 File Offset: 0x00059684
		// (remove) Token: 0x06001928 RID: 6440 RVA: 0x0005B4BC File Offset: 0x000596BC
		public event Action onAfterGenerateEntries;

		// Token: 0x06001929 RID: 6441 RVA: 0x0005B4F4 File Offset: 0x000596F4
		private ItemFilter ContructRandomFilter()
		{
			Tag random = this.tags.GetRandom(0f);
			int random2 = this.qualities.GetRandom(0f);
			if (GameMetaData.Instance.IsDemo)
			{
				this.excludeTags.Add(GameplayDataSettings.Tags.LockInDemoTag);
			}
			return new ItemFilter
			{
				requireTags = new Tag[] { random },
				excludeTags = this.excludeTags.ToArray(),
				minQuality = random2,
				maxQuality = random2
			};
		}

		// Token: 0x0600192A RID: 6442 RVA: 0x0005B580 File Offset: 0x00059780
		public async UniTask<bool> Buy(BlackMarket.DemandSupplyEntry entry)
		{
			bool flag;
			if (entry == null)
			{
				flag = false;
			}
			else if (entry.remaining <= 0)
			{
				flag = false;
			}
			else if (!this.supplies.Contains(entry))
			{
				flag = false;
			}
			else if (!entry.BuyCost.Pay(true, true))
			{
				flag = false;
			}
			else
			{
				await entry.SellCost.Return(true, false, 1, null);
				entry.remaining--;
				entry.NotifyChange();
				flag = true;
			}
			return flag;
		}

		// Token: 0x0600192B RID: 6443 RVA: 0x0005B5CC File Offset: 0x000597CC
		public async UniTask<bool> Sell(BlackMarket.DemandSupplyEntry entry)
		{
			bool flag;
			if (entry == null)
			{
				flag = false;
			}
			else if (entry.remaining <= 0)
			{
				flag = false;
			}
			else if (!this.demands.Contains(entry))
			{
				flag = false;
			}
			else if (!entry.SellCost.Pay(true, true))
			{
				flag = false;
			}
			else
			{
				await entry.BuyCost.Return(false, false, 1, null);
				entry.remaining--;
				entry.NotifyChange();
				flag = true;
			}
			return flag;
		}

		// Token: 0x0600192C RID: 6444 RVA: 0x0005B618 File Offset: 0x00059818
		private void GenerateDemandsAndSupplies()
		{
			this.demands.Clear();
			this.supplies.Clear();
			int num = 0;
			for (int i = 0; i < this.demandsCount; i++)
			{
				num++;
				if (num > 100)
				{
					Debug.LogError("黑市构建需求失败。尝试次数超过100次。");
					break;
				}
				int[] array = ItemAssetsCollection.Search(this.ContructRandomFilter());
				if (array.Length == 0)
				{
					i--;
				}
				else
				{
					int random = array.GetRandom<int>();
					ItemAssetsCollection.GetMetaData(random);
					int random2 = this.demandAmountRand.GetRandom(0f);
					float random3 = this.demandFactorRand.GetRandom(0f);
					int random4 = this.demandBatchCountRand.GetRandom(0f);
					BlackMarket.DemandSupplyEntry demandSupplyEntry = new BlackMarket.DemandSupplyEntry
					{
						itemID = random,
						remaining = random2,
						priceFactor = random3,
						batchCount = random4
					};
					this.demands.Add(demandSupplyEntry);
				}
			}
			num = 0;
			for (int j = 0; j < this.suppliesCount; j++)
			{
				num++;
				if (num > 100)
				{
					Debug.LogError("黑市构建供应失败。尝试次数超过100次。");
					break;
				}
				int[] array2 = ItemAssetsCollection.Search(this.ContructRandomFilter());
				if (array2.Length == 0)
				{
					j--;
				}
				else
				{
					int candidate = array2.GetRandom<int>();
					if (this.demands.Any((BlackMarket.DemandSupplyEntry e) => e.ItemID == candidate))
					{
						j--;
					}
					else
					{
						ItemAssetsCollection.GetMetaData(candidate);
						int random5 = this.supplyAmountRand.GetRandom(0f);
						float random6 = this.supplyFactorRand.GetRandom(0f);
						int random7 = this.supplyBatchCountRand.GetRandom(0f);
						BlackMarket.DemandSupplyEntry demandSupplyEntry2 = new BlackMarket.DemandSupplyEntry
						{
							itemID = candidate,
							remaining = random5,
							priceFactor = random6,
							batchCount = random7
						};
						this.supplies.Add(demandSupplyEntry2);
					}
				}
			}
			Action action = this.onAfterGenerateEntries;
			if (action != null)
			{
				action();
			}
			if (!LevelManager.LevelInited)
			{
				return;
			}
			LevelManager.Instance.SaveMainCharacter();
			SavesSystem.CollectSaveData();
			SavesSystem.SaveFile(true);
		}

		// Token: 0x0600192D RID: 6445 RVA: 0x0005B82C File Offset: 0x00059A2C
		public void PayAndRegenerate()
		{
			if (this.RefreshChance <= 0)
			{
				return;
			}
			int num = this.RefreshChance;
			this.RefreshChance = num - 1;
			this.GenerateDemandsAndSupplies();
		}

		// Token: 0x0600192E RID: 6446 RVA: 0x0005B85C File Offset: 0x00059A5C
		private void FixedUpdate()
		{
			if (this.RefreshChance >= this.MaxRefreshChance)
			{
				this.LastRefreshedTime = DateTime.UtcNow;
				return;
			}
			TimeSpan timeSpan = this.TimeSinceLastRefreshedTime;
			if (timeSpan > this.TimeToRefresh)
			{
				while (timeSpan > this.TimeToRefresh)
				{
					timeSpan -= this.TimeToRefresh;
					this.RefreshChance++;
					if (this.RefreshChance >= this.MaxRefreshChance)
					{
						break;
					}
				}
				if (timeSpan > TimeSpan.Zero)
				{
					this.LastRefreshedTime = DateTime.UtcNow - timeSpan;
				}
			}
		}

		// Token: 0x0600192F RID: 6447 RVA: 0x0005B8EF File Offset: 0x00059AEF
		private void Awake()
		{
			BlackMarket.Instance = this;
			SavesSystem.OnCollectSaveData += this.Save;
		}

		// Token: 0x06001930 RID: 6448 RVA: 0x0005B908 File Offset: 0x00059B08
		private void Start()
		{
			this.Load();
		}

		// Token: 0x06001931 RID: 6449 RVA: 0x0005B910 File Offset: 0x00059B10
		private void OnDestroy()
		{
			SavesSystem.OnCollectSaveData -= this.Save;
		}

		// Token: 0x06001932 RID: 6450 RVA: 0x0005B924 File Offset: 0x00059B24
		private void Save()
		{
			BlackMarket.SaveData saveData = new BlackMarket.SaveData(this);
			SavesSystem.Save<BlackMarket.SaveData>("BlackMarket_Data", saveData);
		}

		// Token: 0x06001933 RID: 6451 RVA: 0x0005B944 File Offset: 0x00059B44
		private void Load()
		{
			BlackMarket.SaveData saveData = SavesSystem.Load<BlackMarket.SaveData>("BlackMarket_Data");
			if (!saveData.valid)
			{
				this.GenerateDemandsAndSupplies();
				return;
			}
			this.demands.Clear();
			this.demands.AddRange(saveData.demands);
			this.supplies.Clear();
			this.supplies.AddRange(saveData.supplies);
			this.lastRefreshedTimeRaw = saveData.lastRefreshedTimeRaw;
			this.refreshChance = saveData.refreshChance;
		}

		// Token: 0x0400123B RID: 4667
		[SerializeField]
		private int demandsCount = 3;

		// Token: 0x0400123C RID: 4668
		[SerializeField]
		private int suppliesCount = 3;

		// Token: 0x0400123D RID: 4669
		[SerializeField]
		private List<Tag> excludeTags;

		// Token: 0x0400123E RID: 4670
		[SerializeField]
		private RandomContainer<Tag> tags;

		// Token: 0x0400123F RID: 4671
		[SerializeField]
		private RandomContainer<int> qualities;

		// Token: 0x04001240 RID: 4672
		[SerializeField]
		private RandomContainer<int> demandAmountRand;

		// Token: 0x04001241 RID: 4673
		[SerializeField]
		private RandomContainer<float> demandFactorRand;

		// Token: 0x04001242 RID: 4674
		[SerializeField]
		private RandomContainer<int> demandBatchCountRand;

		// Token: 0x04001243 RID: 4675
		[SerializeField]
		private RandomContainer<int> supplyAmountRand;

		// Token: 0x04001244 RID: 4676
		[SerializeField]
		private RandomContainer<float> supplyFactorRand;

		// Token: 0x04001245 RID: 4677
		[SerializeField]
		private RandomContainer<int> supplyBatchCountRand;

		// Token: 0x04001246 RID: 4678
		[SerializeField]
		[TimeSpan]
		private long timeToRefresh;

		// Token: 0x04001247 RID: 4679
		[SerializeField]
		private int refreshChance;

		// Token: 0x0400124B RID: 4683
		private static bool dirty = true;

		// Token: 0x0400124C RID: 4684
		private int cachedMaxRefreshChance = -1;

		// Token: 0x0400124D RID: 4685
		[DateTime]
		private long lastRefreshedTimeRaw;

		// Token: 0x0400124E RID: 4686
		private List<BlackMarket.DemandSupplyEntry> demands = new List<BlackMarket.DemandSupplyEntry>();

		// Token: 0x0400124F RID: 4687
		private List<BlackMarket.DemandSupplyEntry> supplies = new List<BlackMarket.DemandSupplyEntry>();

		// Token: 0x04001250 RID: 4688
		private ReadOnlyCollection<BlackMarket.DemandSupplyEntry> _demands_readonly;

		// Token: 0x04001251 RID: 4689
		private ReadOnlyCollection<BlackMarket.DemandSupplyEntry> _supplies_readonly;

		// Token: 0x04001253 RID: 4691
		private const string SaveKey = "BlackMarket_Data";

		// Token: 0x02000593 RID: 1427
		public class OnRequestMaxRefreshChanceEventContext
		{
			// Token: 0x17000769 RID: 1897
			// (get) Token: 0x06002865 RID: 10341 RVA: 0x0009593A File Offset: 0x00093B3A
			public int Value
			{
				get
				{
					return this.value;
				}
			}

			// Token: 0x06002866 RID: 10342 RVA: 0x00095942 File Offset: 0x00093B42
			public void Add(int count = 1)
			{
				this.value += count;
			}

			// Token: 0x04001FF1 RID: 8177
			private int value;
		}

		// Token: 0x02000594 RID: 1428
		public class OnRequestRefreshTimeFactorEventContext
		{
			// Token: 0x1700076A RID: 1898
			// (get) Token: 0x06002868 RID: 10344 RVA: 0x0009595A File Offset: 0x00093B5A
			public float Value
			{
				get
				{
					return this.value;
				}
			}

			// Token: 0x06002869 RID: 10345 RVA: 0x00095962 File Offset: 0x00093B62
			public void Add(float count = -0.1f)
			{
				this.value += count;
			}

			// Token: 0x04001FF2 RID: 8178
			private float value = 1f;
		}

		// Token: 0x02000595 RID: 1429
		[Serializable]
		public class DemandSupplyEntry
		{
			// Token: 0x1700076B RID: 1899
			// (get) Token: 0x0600286B RID: 10347 RVA: 0x00095985 File Offset: 0x00093B85
			public int ItemID
			{
				get
				{
					return this.itemID;
				}
			}

			// Token: 0x1700076C RID: 1900
			// (get) Token: 0x0600286C RID: 10348 RVA: 0x0009598D File Offset: 0x00093B8D
			internal ItemMetaData ItemMetaData
			{
				get
				{
					return ItemAssetsCollection.GetMetaData(this.itemID);
				}
			}

			// Token: 0x1700076D RID: 1901
			// (get) Token: 0x0600286D RID: 10349 RVA: 0x0009599A File Offset: 0x00093B9A
			public int Remaining
			{
				get
				{
					return this.remaining;
				}
			}

			// Token: 0x1700076E RID: 1902
			// (get) Token: 0x0600286E RID: 10350 RVA: 0x000959A2 File Offset: 0x00093BA2
			public int TotalPrice
			{
				get
				{
					return Mathf.FloorToInt((float)this.ItemMetaData.priceEach * this.priceFactor * (float)this.ItemMetaData.defaultStackCount * (float)this.batchCount);
				}
			}

			// Token: 0x1700076F RID: 1903
			// (get) Token: 0x0600286F RID: 10351 RVA: 0x000959D1 File Offset: 0x00093BD1
			public Cost BuyCost
			{
				get
				{
					return new Cost((long)this.TotalPrice);
				}
			}

			// Token: 0x17000770 RID: 1904
			// (get) Token: 0x06002870 RID: 10352 RVA: 0x000959DF File Offset: 0x00093BDF
			public Cost SellCost
			{
				get
				{
					return new Cost(new ValueTuple<int, long>[]
					{
						new ValueTuple<int, long>(this.ItemMetaData.id, (long)(this.ItemMetaData.defaultStackCount * this.batchCount))
					});
				}
			}

			// Token: 0x17000771 RID: 1905
			// (get) Token: 0x06002871 RID: 10353 RVA: 0x00095A18 File Offset: 0x00093C18
			public string ItemDisplayName
			{
				get
				{
					return this.ItemMetaData.DisplayName;
				}
			}

			// Token: 0x140000F7 RID: 247
			// (add) Token: 0x06002872 RID: 10354 RVA: 0x00095A34 File Offset: 0x00093C34
			// (remove) Token: 0x06002873 RID: 10355 RVA: 0x00095A6C File Offset: 0x00093C6C
			public event Action<BlackMarket.DemandSupplyEntry> onChanged;

			// Token: 0x06002874 RID: 10356 RVA: 0x00095AA1 File Offset: 0x00093CA1
			internal void NotifyChange()
			{
				Action<BlackMarket.DemandSupplyEntry> action = this.onChanged;
				if (action == null)
				{
					return;
				}
				action(this);
			}

			// Token: 0x04001FF3 RID: 8179
			[SerializeField]
			[ItemTypeID]
			internal int itemID;

			// Token: 0x04001FF4 RID: 8180
			[SerializeField]
			internal int remaining;

			// Token: 0x04001FF5 RID: 8181
			[SerializeField]
			internal float priceFactor;

			// Token: 0x04001FF6 RID: 8182
			[SerializeField]
			internal int batchCount;
		}

		// Token: 0x02000596 RID: 1430
		[Serializable]
		public struct SaveData
		{
			// Token: 0x06002876 RID: 10358 RVA: 0x00095ABC File Offset: 0x00093CBC
			public SaveData(BlackMarket blackMarket)
			{
				this.valid = true;
				this.lastRefreshedTimeRaw = blackMarket.lastRefreshedTimeRaw;
				this.demands = blackMarket.demands.ToArray();
				this.supplies = blackMarket.supplies.ToArray();
				this.refreshChance = blackMarket.refreshChance;
			}

			// Token: 0x04001FF8 RID: 8184
			public bool valid;

			// Token: 0x04001FF9 RID: 8185
			public long lastRefreshedTimeRaw;

			// Token: 0x04001FFA RID: 8186
			public int refreshChance;

			// Token: 0x04001FFB RID: 8187
			public BlackMarket.DemandSupplyEntry[] demands;

			// Token: 0x04001FFC RID: 8188
			public BlackMarket.DemandSupplyEntry[] supplies;
		}
	}
}
