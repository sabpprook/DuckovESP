using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Duckov.Economy.UI;
using Duckov.UI;
using Duckov.Utilities;
using ItemStatsSystem;
using Saves;
using SodaCraft.Localizations;
using SodaCraft.StringUtilities;
using UnityEngine;

namespace Duckov.Economy
{
	// Token: 0x02000323 RID: 803
	public class StockShop : MonoBehaviour, IMerchant, ISaveDataProvider
	{
		// Token: 0x170004E8 RID: 1256
		// (get) Token: 0x06001AD8 RID: 6872 RVA: 0x00061074 File Offset: 0x0005F274
		public string MerchantID
		{
			get
			{
				return this.merchantID;
			}
		}

		// Token: 0x170004E9 RID: 1257
		// (get) Token: 0x06001AD9 RID: 6873 RVA: 0x0006107C File Offset: 0x0005F27C
		public string OpinionKey
		{
			get
			{
				return "Opinion_" + this.merchantID;
			}
		}

		// Token: 0x170004EA RID: 1258
		// (get) Token: 0x06001ADA RID: 6874 RVA: 0x0006108E File Offset: 0x0005F28E
		public string DisplayName
		{
			get
			{
				return this.DisplayNameKey.ToPlainText();
			}
		}

		// Token: 0x170004EB RID: 1259
		// (get) Token: 0x06001ADB RID: 6875 RVA: 0x0006109B File Offset: 0x0005F29B
		// (set) Token: 0x06001ADC RID: 6876 RVA: 0x000610B2 File Offset: 0x0005F2B2
		private int Opinion
		{
			get
			{
				return Mathf.Clamp(CommonVariables.GetInt(this.OpinionKey, 0), -100, 100);
			}
			set
			{
				CommonVariables.SetInt(this.OpinionKey, value);
			}
		}

		// Token: 0x170004EC RID: 1260
		// (get) Token: 0x06001ADD RID: 6877 RVA: 0x000610C0 File Offset: 0x0005F2C0
		public string PurchaseNotificationTextFormat
		{
			get
			{
				return this.purchaseNotificationTextFormatKey.ToPlainText();
			}
		}

		// Token: 0x170004ED RID: 1261
		// (get) Token: 0x06001ADE RID: 6878 RVA: 0x000610CD File Offset: 0x0005F2CD
		public bool AccountAvaliable
		{
			get
			{
				return this.accountAvaliable;
			}
		}

		// Token: 0x140000B7 RID: 183
		// (add) Token: 0x06001ADF RID: 6879 RVA: 0x000610D8 File Offset: 0x0005F2D8
		// (remove) Token: 0x06001AE0 RID: 6880 RVA: 0x0006110C File Offset: 0x0005F30C
		public static event Action<StockShop> OnAfterItemSold;

		// Token: 0x140000B8 RID: 184
		// (add) Token: 0x06001AE1 RID: 6881 RVA: 0x00061140 File Offset: 0x0005F340
		// (remove) Token: 0x06001AE2 RID: 6882 RVA: 0x00061174 File Offset: 0x0005F374
		public static event Action<StockShop, Item> OnItemPurchased;

		// Token: 0x140000B9 RID: 185
		// (add) Token: 0x06001AE3 RID: 6883 RVA: 0x000611A8 File Offset: 0x0005F3A8
		// (remove) Token: 0x06001AE4 RID: 6884 RVA: 0x000611DC File Offset: 0x0005F3DC
		public static event Action<StockShop, Item, int> OnItemSoldByPlayer;

		// Token: 0x170004EE RID: 1262
		// (get) Token: 0x06001AE5 RID: 6885 RVA: 0x00061210 File Offset: 0x0005F410
		public TimeSpan TimeSinceLastRefresh
		{
			get
			{
				DateTime dateTime = DateTime.FromBinary(this.lastTimeRefreshedStock);
				if (dateTime > DateTime.UtcNow)
				{
					dateTime = DateTime.UtcNow;
					this.lastTimeRefreshedStock = DateTime.UtcNow.ToBinary();
					GameManager.TimeTravelDetected();
				}
				return DateTime.UtcNow - dateTime;
			}
		}

		// Token: 0x170004EF RID: 1263
		// (get) Token: 0x06001AE6 RID: 6886 RVA: 0x00061260 File Offset: 0x0005F460
		public TimeSpan NextRefreshETA
		{
			get
			{
				TimeSpan timeSinceLastRefresh = this.TimeSinceLastRefresh;
				TimeSpan timeSpan = TimeSpan.FromTicks(this.refreshAfterTimeSpan) - timeSinceLastRefresh;
				if (timeSpan < TimeSpan.Zero)
				{
					timeSpan = TimeSpan.Zero;
				}
				return timeSpan;
			}
		}

		// Token: 0x06001AE7 RID: 6887 RVA: 0x0006129C File Offset: 0x0005F49C
		private async UniTask<Item> GetItemInstance(int typeID)
		{
			Item item;
			Item item2;
			if (this.itemInstances.TryGetValue(typeID, out item))
			{
				item2 = item;
			}
			else
			{
				Item item3 = await ItemAssetsCollection.InstantiateAsync(typeID);
				item3.transform.SetParent(base.transform);
				item3.gameObject.SetActive(false);
				this.itemInstances[typeID] = item3;
				item2 = item3;
			}
			return item2;
		}

		// Token: 0x06001AE8 RID: 6888 RVA: 0x000612E8 File Offset: 0x0005F4E8
		public Item GetItemInstanceDirect(int typeID)
		{
			Item item;
			if (this.itemInstances.TryGetValue(typeID, out item))
			{
				return item;
			}
			return null;
		}

		// Token: 0x06001AE9 RID: 6889 RVA: 0x00061308 File Offset: 0x0005F508
		private void Awake()
		{
			this.InitializeEntries();
			SavesSystem.OnCollectSaveData += this.Save;
			SavesSystem.OnSetFile += this.Load;
			this.Load();
		}

		// Token: 0x06001AEA RID: 6890 RVA: 0x00061338 File Offset: 0x0005F538
		private void InitializeEntries()
		{
			StockShopDatabase.MerchantProfile merchantProfile = StockShopDatabase.Instance.GetMerchantProfile(this.merchantID);
			if (merchantProfile == null)
			{
				Debug.Log("未配置商人 " + this.merchantID);
				return;
			}
			foreach (StockShopDatabase.ItemEntry itemEntry in merchantProfile.entries)
			{
				this.entries.Add(new StockShop.Entry(itemEntry));
			}
		}

		// Token: 0x170004F0 RID: 1264
		// (get) Token: 0x06001AEB RID: 6891 RVA: 0x000613C0 File Offset: 0x0005F5C0
		private string SaveKey
		{
			get
			{
				return "StockShop_" + this.merchantID;
			}
		}

		// Token: 0x06001AEC RID: 6892 RVA: 0x000613D4 File Offset: 0x0005F5D4
		private void Load()
		{
			if (!SavesSystem.KeyExisits(this.SaveKey))
			{
				return;
			}
			StockShop.SaveData saveData = SavesSystem.Load<StockShop.SaveData>(this.SaveKey);
			this.SetupSaveData(saveData);
		}

		// Token: 0x06001AED RID: 6893 RVA: 0x00061404 File Offset: 0x0005F604
		private void Save()
		{
			StockShop.SaveData saveData = this.GenerateSaveData() as StockShop.SaveData;
			if (saveData == null)
			{
				Debug.LogError("没法正确生成StockShop的SaveData");
				return;
			}
			SavesSystem.Save<StockShop.SaveData>(this.SaveKey, saveData);
		}

		// Token: 0x06001AEE RID: 6894 RVA: 0x00061437 File Offset: 0x0005F637
		private void OnDestroy()
		{
			SavesSystem.OnCollectSaveData -= this.Save;
			SavesSystem.OnSetFile -= this.Load;
		}

		// Token: 0x06001AEF RID: 6895 RVA: 0x0006145C File Offset: 0x0005F65C
		private void Start()
		{
			this.CacheItemInstances().Forget();
			if (this.refreshStockOnStart)
			{
				this.DoRefreshStock();
				this.lastTimeRefreshedStock = DateTime.UtcNow.ToBinary();
			}
		}

		// Token: 0x06001AF0 RID: 6896 RVA: 0x00061498 File Offset: 0x0005F698
		private async UniTask CacheItemInstances()
		{
			List<UniTask> list = new List<UniTask>();
			foreach (StockShop.Entry entry in this.entries)
			{
				UniTask<Item> itemInstance = this.GetItemInstance(entry.ItemTypeID);
				list.Add(itemInstance);
			}
			await UniTask.WhenAll(list);
		}

		// Token: 0x06001AF1 RID: 6897 RVA: 0x000614DC File Offset: 0x0005F6DC
		internal void RefreshIfNeeded()
		{
			TimeSpan timeSpan = TimeSpan.FromTicks(this.refreshAfterTimeSpan);
			DateTime dateTime = DateTime.FromBinary(this.lastTimeRefreshedStock);
			if (dateTime > DateTime.UtcNow)
			{
				dateTime = DateTime.UtcNow;
				this.lastTimeRefreshedStock = dateTime.ToBinary();
			}
			DateTime dateTime2 = DateTime.UtcNow - TimeSpan.FromDays(2.0);
			if (dateTime < dateTime2)
			{
				this.lastTimeRefreshedStock = dateTime2.ToBinary();
			}
			if (DateTime.UtcNow - dateTime > timeSpan)
			{
				this.DoRefreshStock();
				this.lastTimeRefreshedStock = DateTime.UtcNow.ToBinary();
			}
		}

		// Token: 0x06001AF2 RID: 6898 RVA: 0x0006157C File Offset: 0x0005F77C
		private void DoRefreshStock()
		{
			bool advancedDebuffMode = LevelManager.Rule.AdvancedDebuffMode;
			foreach (StockShop.Entry entry in this.entries)
			{
				if (entry.Possibility > 0f && entry.Possibility < 1f && global::UnityEngine.Random.Range(0f, 1f) > entry.Possibility)
				{
					entry.Show = false;
					entry.CurrentStock = 0;
				}
				else
				{
					ItemMetaData metaData = ItemAssetsCollection.GetMetaData(entry.ItemTypeID);
					if (!advancedDebuffMode && metaData.tags.Contains(GameplayDataSettings.Tags.AdvancedDebuffMode))
					{
						entry.Show = false;
						entry.CurrentStock = 0;
					}
					else
					{
						entry.Show = true;
						entry.CurrentStock = entry.MaxStock;
					}
				}
			}
		}

		// Token: 0x170004F1 RID: 1265
		// (get) Token: 0x06001AF3 RID: 6899 RVA: 0x00061664 File Offset: 0x0005F864
		public bool Busy
		{
			get
			{
				return this.buying || this.selling;
			}
		}

		// Token: 0x06001AF4 RID: 6900 RVA: 0x0006167C File Offset: 0x0005F87C
		public async UniTask<bool> Buy(int itemTypeID, int amount = 1)
		{
			bool flag;
			if (this.Busy)
			{
				flag = false;
			}
			else
			{
				this.buying = true;
				bool flag2 = await this.BuyTask(itemTypeID, amount);
				this.buying = false;
				flag = flag2;
			}
			return flag;
		}

		// Token: 0x06001AF5 RID: 6901 RVA: 0x000616D0 File Offset: 0x0005F8D0
		private async UniTask<bool> BuyTask(int itemTypeID, int amount = 1)
		{
			StockShop.Entry found = this.entries.First((StockShop.Entry e) => e != null && e.ItemTypeID == itemTypeID);
			bool flag;
			if (found == null)
			{
				flag = false;
			}
			else if (found.CurrentStock < 1)
			{
				flag = false;
			}
			else
			{
				Item itemInstanceDirect = this.GetItemInstanceDirect(itemTypeID);
				if (!itemInstanceDirect.Stackable)
				{
					amount = 1;
				}
				if (found.CurrentStock < amount)
				{
					flag = false;
				}
				else if (itemInstanceDirect == null)
				{
					flag = false;
				}
				else if (!EconomyManager.Pay(new Cost((long)this.ConvertPrice(itemInstanceDirect, false)), this.accountAvaliable, true))
				{
					flag = false;
				}
				else
				{
					Item item = await ItemAssetsCollection.InstantiateAsync(itemTypeID);
					if (!ItemUtilities.SendToPlayerCharacterInventory(item, false))
					{
						Debug.Log("玩家身上没地儿了，发送到玩家仓储处");
						ItemUtilities.SendToPlayerStorage(item, false);
					}
					found.CurrentStock -= amount;
					Action<StockShop> onAfterItemSold = StockShop.OnAfterItemSold;
					if (onAfterItemSold != null)
					{
						onAfterItemSold(this);
					}
					Action<StockShop, Item> onItemPurchased = StockShop.OnItemPurchased;
					if (onItemPurchased != null)
					{
						onItemPurchased(this, item);
					}
					NotificationText.Push(this.PurchaseNotificationTextFormat.Format(new
					{
						itemDisplayName = item.DisplayName
					}));
					flag = true;
				}
			}
			return flag;
		}

		// Token: 0x06001AF6 RID: 6902 RVA: 0x00061724 File Offset: 0x0005F924
		internal async UniTask Sell(Item target)
		{
			if (!this.Busy)
			{
				if (!(target == null))
				{
					if (target.CanBeSold)
					{
						this.selling = true;
						int sellPrice = this.ConvertPrice(target, true);
						target.Detach();
						target.DestroyTree();
						if (this.returnCash)
						{
							Cost cost = new Cost(new ValueTuple<int, long>[]
							{
								new ValueTuple<int, long>(451, (long)sellPrice)
							});
							await cost.Return(false, true, 1, null);
						}
						else
						{
							EconomyManager.Add((long)sellPrice);
						}
						Action<StockShop, Item, int> onItemSoldByPlayer = StockShop.OnItemSoldByPlayer;
						if (onItemSoldByPlayer != null)
						{
							onItemSoldByPlayer(this, target, sellPrice);
						}
						this.selling = false;
					}
				}
			}
		}

		// Token: 0x06001AF7 RID: 6903 RVA: 0x0006176F File Offset: 0x0005F96F
		public void ShowUI()
		{
			if (!StockShopView.Instance)
			{
				return;
			}
			this.RefreshIfNeeded();
			StockShopView.Instance.SetupAndShow(this);
		}

		// Token: 0x06001AF8 RID: 6904 RVA: 0x00061790 File Offset: 0x0005F990
		public int ConvertPrice(Item item, bool selling = false)
		{
			int num = item.GetTotalRawValue();
			if (!selling)
			{
				StockShop.Entry entry = this.entries.Find((StockShop.Entry e) => e != null && e.ItemTypeID == item.TypeID);
				if (entry != null)
				{
					num = Mathf.FloorToInt((float)num * entry.PriceFactor);
				}
			}
			if (selling)
			{
				float factor = this.sellFactor;
				StockShop.OverrideSellingPriceEntry overrideSellingPriceEntry = this.overrideSellingPrice.Find((StockShop.OverrideSellingPriceEntry e) => e.typeID == item.TypeID);
				if (overrideSellingPriceEntry != null)
				{
					factor = overrideSellingPriceEntry.factor;
				}
				return Mathf.FloorToInt((float)num * factor);
			}
			return num;
		}

		// Token: 0x06001AF9 RID: 6905 RVA: 0x00061820 File Offset: 0x0005FA20
		public object GenerateSaveData()
		{
			StockShop.SaveData saveData = new StockShop.SaveData();
			saveData.lastTimeRefreshedStock = this.lastTimeRefreshedStock;
			foreach (StockShop.Entry entry in this.entries)
			{
				saveData.stockCounts.Add(new StockShop.SaveData.StockCountEntry
				{
					itemTypeID = entry.ItemTypeID,
					stock = entry.CurrentStock
				});
			}
			return saveData;
		}

		// Token: 0x06001AFA RID: 6906 RVA: 0x000618A8 File Offset: 0x0005FAA8
		public void SetupSaveData(object dataRaw)
		{
			StockShop.SaveData saveData = dataRaw as StockShop.SaveData;
			if (saveData == null)
			{
				return;
			}
			this.lastTimeRefreshedStock = saveData.lastTimeRefreshedStock;
			using (List<StockShop.Entry>.Enumerator enumerator = this.entries.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					StockShop.Entry cur = enumerator.Current;
					StockShop.SaveData.StockCountEntry stockCountEntry = saveData.stockCounts.Find((StockShop.SaveData.StockCountEntry e) => e != null && e.itemTypeID == cur.ItemTypeID);
					if (stockCountEntry != null)
					{
						cur.Show = stockCountEntry.stock > 0;
						cur.CurrentStock = stockCountEntry.stock;
					}
				}
			}
		}

		// Token: 0x0400131B RID: 4891
		[SerializeField]
		private string merchantID = "Albert";

		// Token: 0x0400131C RID: 4892
		[LocalizationKey("Default")]
		public string DisplayNameKey;

		// Token: 0x0400131D RID: 4893
		[TimeSpan]
		[SerializeField]
		private long refreshAfterTimeSpan;

		// Token: 0x0400131E RID: 4894
		[SerializeField]
		private string purchaseNotificationTextFormatKey = "UI_StockShop_PurchasedNotification";

		// Token: 0x0400131F RID: 4895
		[SerializeField]
		private bool accountAvaliable;

		// Token: 0x04001320 RID: 4896
		[SerializeField]
		private bool returnCash;

		// Token: 0x04001321 RID: 4897
		[SerializeField]
		private bool refreshStockOnStart;

		// Token: 0x04001322 RID: 4898
		public float sellFactor = 0.5f;

		// Token: 0x04001323 RID: 4899
		public List<StockShop.Entry> entries = new List<StockShop.Entry>();

		// Token: 0x04001324 RID: 4900
		public List<StockShop.OverrideSellingPriceEntry> overrideSellingPrice = new List<StockShop.OverrideSellingPriceEntry>();

		// Token: 0x04001328 RID: 4904
		[DateTime]
		[SerializeField]
		private long lastTimeRefreshedStock;

		// Token: 0x04001329 RID: 4905
		private Dictionary<int, Item> itemInstances = new Dictionary<int, Item>();

		// Token: 0x0400132A RID: 4906
		private bool buying;

		// Token: 0x0400132B RID: 4907
		private bool selling;

		// Token: 0x020005C0 RID: 1472
		public class Entry
		{
			// Token: 0x060028D7 RID: 10455 RVA: 0x000972DA File Offset: 0x000954DA
			public Entry(StockShopDatabase.ItemEntry cur)
			{
				this.entry = cur;
			}

			// Token: 0x17000781 RID: 1921
			// (get) Token: 0x060028D8 RID: 10456 RVA: 0x000972F0 File Offset: 0x000954F0
			public int MaxStock
			{
				get
				{
					if (this.entry.maxStock < 1)
					{
						this.entry.maxStock = 1;
					}
					return this.entry.maxStock;
				}
			}

			// Token: 0x17000782 RID: 1922
			// (get) Token: 0x060028D9 RID: 10457 RVA: 0x00097317 File Offset: 0x00095517
			public int ItemTypeID
			{
				get
				{
					return this.entry.typeID;
				}
			}

			// Token: 0x17000783 RID: 1923
			// (get) Token: 0x060028DA RID: 10458 RVA: 0x00097324 File Offset: 0x00095524
			public bool ForceUnlock
			{
				get
				{
					return (!GameMetaData.Instance.IsDemo || !this.entry.lockInDemo) && this.entry.forceUnlock;
				}
			}

			// Token: 0x17000784 RID: 1924
			// (get) Token: 0x060028DB RID: 10459 RVA: 0x0009734C File Offset: 0x0009554C
			public float PriceFactor
			{
				get
				{
					return this.entry.priceFactor;
				}
			}

			// Token: 0x17000785 RID: 1925
			// (get) Token: 0x060028DC RID: 10460 RVA: 0x00097359 File Offset: 0x00095559
			public float Possibility
			{
				get
				{
					return this.entry.possibility;
				}
			}

			// Token: 0x17000786 RID: 1926
			// (get) Token: 0x060028DD RID: 10461 RVA: 0x00097366 File Offset: 0x00095566
			// (set) Token: 0x060028DE RID: 10462 RVA: 0x0009736E File Offset: 0x0009556E
			public bool Show
			{
				get
				{
					return this.show;
				}
				set
				{
					this.show = value;
				}
			}

			// Token: 0x17000787 RID: 1927
			// (get) Token: 0x060028DF RID: 10463 RVA: 0x00097377 File Offset: 0x00095577
			// (set) Token: 0x060028E0 RID: 10464 RVA: 0x0009737F File Offset: 0x0009557F
			public int CurrentStock
			{
				get
				{
					return this.currentStock;
				}
				set
				{
					this.currentStock = value;
					Action<StockShop.Entry> action = this.onStockChanged;
					if (action == null)
					{
						return;
					}
					action(this);
				}
			}

			// Token: 0x140000F8 RID: 248
			// (add) Token: 0x060028E1 RID: 10465 RVA: 0x0009739C File Offset: 0x0009559C
			// (remove) Token: 0x060028E2 RID: 10466 RVA: 0x000973D4 File Offset: 0x000955D4
			public event Action<StockShop.Entry> onStockChanged;

			// Token: 0x04002079 RID: 8313
			private StockShopDatabase.ItemEntry entry;

			// Token: 0x0400207A RID: 8314
			[SerializeField]
			private bool show = true;

			// Token: 0x0400207B RID: 8315
			[SerializeField]
			private int currentStock;
		}

		// Token: 0x020005C1 RID: 1473
		[Serializable]
		public class OverrideSellingPriceEntry
		{
			// Token: 0x0400207D RID: 8317
			[ItemTypeID]
			public int typeID;

			// Token: 0x0400207E RID: 8318
			public float factor = 0.5f;
		}

		// Token: 0x020005C2 RID: 1474
		[Serializable]
		private class SaveData
		{
			// Token: 0x0400207F RID: 8319
			[DateTime]
			public long lastTimeRefreshedStock;

			// Token: 0x04002080 RID: 8320
			public List<StockShop.SaveData.StockCountEntry> stockCounts = new List<StockShop.SaveData.StockCountEntry>();

			// Token: 0x02000671 RID: 1649
			public class StockCountEntry
			{
				// Token: 0x04002320 RID: 8992
				public int itemTypeID;

				// Token: 0x04002321 RID: 8993
				public int stock;
			}
		}
	}
}
