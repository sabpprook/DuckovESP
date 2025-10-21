using System;
using Cysharp.Threading.Tasks;
using ItemStatsSystem;
using ItemStatsSystem.Data;
using Saves;
using UnityEngine;

namespace Duckov.Bitcoins
{
	// Token: 0x0200030A RID: 778
	public class BitcoinMiner : MonoBehaviour
	{
		// Token: 0x170004A5 RID: 1189
		// (get) Token: 0x06001987 RID: 6535 RVA: 0x0005C5CF File Offset: 0x0005A7CF
		// (set) Token: 0x06001988 RID: 6536 RVA: 0x0005C5D6 File Offset: 0x0005A7D6
		public static BitcoinMiner Instance { get; private set; }

		// Token: 0x170004A6 RID: 1190
		// (get) Token: 0x06001989 RID: 6537 RVA: 0x0005C5DE File Offset: 0x0005A7DE
		private double Progress
		{
			get
			{
				return this.work;
			}
		}

		// Token: 0x170004A7 RID: 1191
		// (get) Token: 0x0600198A RID: 6538 RVA: 0x0005C5E6 File Offset: 0x0005A7E6
		private static double K_1_12
		{
			get
			{
				if (BitcoinMiner._cached_k == null)
				{
					BitcoinMiner._cached_k = new double?((BitcoinMiner.wps_12 - BitcoinMiner.wps_1) / 11.0);
				}
				return BitcoinMiner._cached_k.Value;
			}
		}

		// Token: 0x170004A8 RID: 1192
		// (get) Token: 0x0600198B RID: 6539 RVA: 0x0005C620 File Offset: 0x0005A820
		public double WorkPerSecond
		{
			get
			{
				if (this.IsInventoryFull)
				{
					return 0.0;
				}
				if (this.cachedPerformance < 1f)
				{
					return (double)this.cachedPerformance * BitcoinMiner.wps_1;
				}
				return BitcoinMiner.wps_1 + (double)(this.cachedPerformance - 1f) * BitcoinMiner.K_1_12;
			}
		}

		// Token: 0x170004A9 RID: 1193
		// (get) Token: 0x0600198C RID: 6540 RVA: 0x0005C673 File Offset: 0x0005A873
		public double HoursPerCoin
		{
			get
			{
				return this.workPerCoin / 3600.0 / this.WorkPerSecond;
			}
		}

		// Token: 0x170004AA RID: 1194
		// (get) Token: 0x0600198D RID: 6541 RVA: 0x0005C68C File Offset: 0x0005A88C
		public bool IsInventoryFull
		{
			get
			{
				return !(this.item == null) && this.item.Inventory.GetFirstEmptyPosition(0) < 0;
			}
		}

		// Token: 0x170004AB RID: 1195
		// (get) Token: 0x0600198E RID: 6542 RVA: 0x0005C6B2 File Offset: 0x0005A8B2
		public TimeSpan TimePerCoin
		{
			get
			{
				if (this.WorkPerSecond > 0.0)
				{
					return TimeSpan.FromSeconds(this.workPerCoin / this.WorkPerSecond);
				}
				return TimeSpan.MaxValue;
			}
		}

		// Token: 0x170004AC RID: 1196
		// (get) Token: 0x0600198F RID: 6543 RVA: 0x0005C6DD File Offset: 0x0005A8DD
		public TimeSpan RemainingTime
		{
			get
			{
				if (this.WorkPerSecond > 0.0)
				{
					return TimeSpan.FromSeconds((this.workPerCoin - this.work) / this.WorkPerSecond);
				}
				return TimeSpan.MaxValue;
			}
		}

		// Token: 0x170004AD RID: 1197
		// (get) Token: 0x06001990 RID: 6544 RVA: 0x0005C710 File Offset: 0x0005A910
		// (set) Token: 0x06001991 RID: 6545 RVA: 0x0005C755 File Offset: 0x0005A955
		private DateTime LastUpdateDateTime
		{
			get
			{
				DateTime dateTime = DateTime.FromBinary(this.lastUpdateDateTimeRaw);
				if (dateTime > DateTime.UtcNow)
				{
					this.lastUpdateDateTimeRaw = DateTime.UtcNow.ToBinary();
					dateTime = DateTime.UtcNow;
					GameManager.TimeTravelDetected();
				}
				return dateTime;
			}
			set
			{
				this.lastUpdateDateTimeRaw = value.ToBinary();
			}
		}

		// Token: 0x06001992 RID: 6546 RVA: 0x0005C764 File Offset: 0x0005A964
		private void Awake()
		{
			if (BitcoinMiner.Instance != null)
			{
				Debug.LogError("存在多个BitcoinMiner");
				return;
			}
			BitcoinMiner.Instance = this;
			SavesSystem.OnCollectSaveData += this.Save;
		}

		// Token: 0x06001993 RID: 6547 RVA: 0x0005C795 File Offset: 0x0005A995
		private void OnDestroy()
		{
			SavesSystem.OnCollectSaveData -= this.Save;
		}

		// Token: 0x06001994 RID: 6548 RVA: 0x0005C7A8 File Offset: 0x0005A9A8
		private void Start()
		{
			this.Load();
		}

		// Token: 0x170004AE RID: 1198
		// (get) Token: 0x06001995 RID: 6549 RVA: 0x0005C7B0 File Offset: 0x0005A9B0
		// (set) Token: 0x06001996 RID: 6550 RVA: 0x0005C7B8 File Offset: 0x0005A9B8
		public bool Loading { get; private set; }

		// Token: 0x170004AF RID: 1199
		// (get) Token: 0x06001997 RID: 6551 RVA: 0x0005C7C1 File Offset: 0x0005A9C1
		// (set) Token: 0x06001998 RID: 6552 RVA: 0x0005C7C9 File Offset: 0x0005A9C9
		public bool Initialized { get; private set; }

		// Token: 0x06001999 RID: 6553 RVA: 0x0005C7D4 File Offset: 0x0005A9D4
		private async UniTask Setup(BitcoinMiner.SaveData data)
		{
			if (this.Loading)
			{
				Debug.LogError("已经在加载中");
			}
			else
			{
				this.Loading = true;
				Item item = await ItemTreeData.InstantiateAsync(data.itemData);
				this.item = item;
				this.item.transform.SetParent(base.transform);
				this.work = data.work;
				this.lastUpdateDateTimeRaw = data.lastUpdateDateTimeRaw;
				this.cachedPerformance = data.cachedPerformance;
				this.Loading = false;
				this.Initialized = true;
			}
		}

		// Token: 0x0600199A RID: 6554 RVA: 0x0005C820 File Offset: 0x0005AA20
		private async UniTask Initialize()
		{
			if (this.Loading)
			{
				Debug.LogError("已经在加载中");
			}
			else
			{
				this.Loading = true;
				Item item = await ItemAssetsCollection.InstantiateAsync(this.minerItemID);
				this.item = item;
				this.item.transform.SetParent(base.transform);
				this.work = 0.0;
				this.cachedPerformance = 0f;
				this.LastUpdateDateTime = DateTime.UtcNow;
				this.Loading = false;
				this.Initialized = true;
			}
		}

		// Token: 0x0600199B RID: 6555 RVA: 0x0005C864 File Offset: 0x0005AA64
		private void Load()
		{
			if (SavesSystem.KeyExisits("BitcoinMiner_Data"))
			{
				BitcoinMiner.SaveData saveData = SavesSystem.Load<BitcoinMiner.SaveData>("BitcoinMiner_Data");
				this.Setup(saveData).Forget();
				return;
			}
			this.Initialize().Forget();
		}

		// Token: 0x0600199C RID: 6556 RVA: 0x0005C8A0 File Offset: 0x0005AAA0
		private void Save()
		{
			if (this.Loading)
			{
				return;
			}
			if (!this.Initialized)
			{
				return;
			}
			BitcoinMiner.SaveData saveData = new BitcoinMiner.SaveData
			{
				itemData = ItemTreeData.FromItem(this.item),
				work = this.work,
				lastUpdateDateTimeRaw = this.lastUpdateDateTimeRaw,
				cachedPerformance = this.cachedPerformance
			};
			SavesSystem.Save<BitcoinMiner.SaveData>("BitcoinMiner_Data", saveData);
		}

		// Token: 0x0600199D RID: 6557 RVA: 0x0005C910 File Offset: 0x0005AB10
		private void UpdateWork()
		{
			if (this.Loading)
			{
				return;
			}
			if (!this.Initialized)
			{
				return;
			}
			double totalSeconds = (DateTime.UtcNow - this.LastUpdateDateTime).TotalSeconds;
			double num = this.WorkPerSecond * totalSeconds;
			bool isInventoryFull = this.IsInventoryFull;
			if (this.work < 0.0)
			{
				this.work = 0.0;
			}
			this.work += num;
			if (this.work >= this.workPerCoin && !this.CreatingCoin)
			{
				if (!isInventoryFull)
				{
					this.CreateCoin().Forget();
				}
				else
				{
					this.work = this.workPerCoin;
				}
			}
			this.cachedPerformance = this.item.GetStatValue("Performance".GetHashCode());
			this.LastUpdateDateTime = DateTime.UtcNow;
		}

		// Token: 0x170004B0 RID: 1200
		// (get) Token: 0x0600199E RID: 6558 RVA: 0x0005C9DE File Offset: 0x0005ABDE
		// (set) Token: 0x0600199F RID: 6559 RVA: 0x0005C9E6 File Offset: 0x0005ABE6
		public bool CreatingCoin { get; private set; }

		// Token: 0x170004B1 RID: 1201
		// (get) Token: 0x060019A0 RID: 6560 RVA: 0x0005C9EF File Offset: 0x0005ABEF
		public Item Item
		{
			get
			{
				return this.item;
			}
		}

		// Token: 0x170004B2 RID: 1202
		// (get) Token: 0x060019A1 RID: 6561 RVA: 0x0005C9F7 File Offset: 0x0005ABF7
		public float NormalizedProgress
		{
			get
			{
				return (float)(this.work / this.workPerCoin);
			}
		}

		// Token: 0x170004B3 RID: 1203
		// (get) Token: 0x060019A2 RID: 6562 RVA: 0x0005CA07 File Offset: 0x0005AC07
		public double Performance
		{
			get
			{
				if (this.Item == null)
				{
					return 0.0;
				}
				return (double)this.Item.GetStatValue("Performance".GetHashCode());
			}
		}

		// Token: 0x060019A3 RID: 6563 RVA: 0x0005CA38 File Offset: 0x0005AC38
		private async UniTask CreateCoin()
		{
			if (!this.CreatingCoin)
			{
				this.CreatingCoin = true;
				Item item = await ItemAssetsCollection.InstantiateAsync(this.coinItemID);
				this.item.Inventory.AddAndMerge(item, 0);
				this.work -= this.workPerCoin;
				this.CreatingCoin = false;
			}
		}

		// Token: 0x060019A4 RID: 6564 RVA: 0x0005CA7B File Offset: 0x0005AC7B
		private void FixedUpdate()
		{
			this.UpdateWork();
		}

		// Token: 0x0400127E RID: 4734
		[SerializeField]
		[ItemTypeID]
		private int minerItemID = 397;

		// Token: 0x0400127F RID: 4735
		[SerializeField]
		[ItemTypeID]
		private int coinItemID = 388;

		// Token: 0x04001280 RID: 4736
		[SerializeField]
		private double workPerCoin = 1.0;

		// Token: 0x04001281 RID: 4737
		private Item item;

		// Token: 0x04001282 RID: 4738
		private double work;

		// Token: 0x04001283 RID: 4739
		private static readonly double wps_1 = 2.3148148148148147E-05;

		// Token: 0x04001284 RID: 4740
		private static readonly double wps_12 = 5.555555555555556E-05;

		// Token: 0x04001285 RID: 4741
		private static double? _cached_k;

		// Token: 0x04001286 RID: 4742
		[DateTime]
		private long lastUpdateDateTimeRaw;

		// Token: 0x04001287 RID: 4743
		private float cachedPerformance;

		// Token: 0x0400128A RID: 4746
		public const string SaveKey = "BitcoinMiner_Data";

		// Token: 0x0400128B RID: 4747
		private const string PerformaceStatKey = "Performance";

		// Token: 0x0200059B RID: 1435
		[Serializable]
		private struct SaveData
		{
			// Token: 0x0400200C RID: 8204
			public ItemTreeData itemData;

			// Token: 0x0400200D RID: 8205
			public double work;

			// Token: 0x0400200E RID: 8206
			public float cachedPerformance;

			// Token: 0x0400200F RID: 8207
			public long lastUpdateDateTimeRaw;
		}
	}
}
