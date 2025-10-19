using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Duckov.UI;
using Duckov.Utilities;
using ItemStatsSystem;
using Saves;
using SodaCraft.Localizations;
using SodaCraft.StringUtilities;
using UnityEngine;

namespace Duckov.Economy
{
	// Token: 0x02000321 RID: 801
	public class EconomyManager : MonoBehaviour, ISaveDataProvider
	{
		// Token: 0x170004DF RID: 1247
		// (get) Token: 0x06001AAB RID: 6827 RVA: 0x0006068C File Offset: 0x0005E88C
		public static string ItemUnlockNotificationTextMainFormat
		{
			get
			{
				EconomyManager instance = EconomyManager.Instance;
				if (instance == null)
				{
					return null;
				}
				return instance.itemUnlockNotificationTextMainFormat;
			}
		}

		// Token: 0x170004E0 RID: 1248
		// (get) Token: 0x06001AAC RID: 6828 RVA: 0x0006069E File Offset: 0x0005E89E
		public static string ItemUnlockNotificationTextSubFormat
		{
			get
			{
				EconomyManager instance = EconomyManager.Instance;
				if (instance == null)
				{
					return null;
				}
				return instance.itemUnlockNotificationTextSubFormat;
			}
		}

		// Token: 0x140000B2 RID: 178
		// (add) Token: 0x06001AAD RID: 6829 RVA: 0x000606B0 File Offset: 0x0005E8B0
		// (remove) Token: 0x06001AAE RID: 6830 RVA: 0x000606E4 File Offset: 0x0005E8E4
		public static event Action OnEconomyManagerLoaded;

		// Token: 0x170004E1 RID: 1249
		// (get) Token: 0x06001AAF RID: 6831 RVA: 0x00060717 File Offset: 0x0005E917
		// (set) Token: 0x06001AB0 RID: 6832 RVA: 0x0006071E File Offset: 0x0005E91E
		public static EconomyManager Instance { get; private set; }

		// Token: 0x06001AB1 RID: 6833 RVA: 0x00060726 File Offset: 0x0005E926
		private void Awake()
		{
			if (EconomyManager.Instance == null)
			{
				EconomyManager.Instance = this;
			}
			SavesSystem.OnCollectSaveData += this.OnCollectSaveData;
			SavesSystem.OnSetFile += this.OnSetSaveFile;
			this.Load();
		}

		// Token: 0x06001AB2 RID: 6834 RVA: 0x00060763 File Offset: 0x0005E963
		private void OnCollectSaveData()
		{
			this.Save();
		}

		// Token: 0x06001AB3 RID: 6835 RVA: 0x0006076B File Offset: 0x0005E96B
		private void OnSetSaveFile()
		{
			this.Load();
		}

		// Token: 0x06001AB4 RID: 6836 RVA: 0x00060774 File Offset: 0x0005E974
		private void Load()
		{
			if (SavesSystem.KeyExisits("EconomyData"))
			{
				this.SetupSaveData(SavesSystem.Load<EconomyManager.SaveData>("EconomyData"));
			}
			try
			{
				Action onEconomyManagerLoaded = EconomyManager.OnEconomyManagerLoaded;
				if (onEconomyManagerLoaded != null)
				{
					onEconomyManagerLoaded();
				}
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		// Token: 0x06001AB5 RID: 6837 RVA: 0x000607CC File Offset: 0x0005E9CC
		private void Save()
		{
			SavesSystem.Save<EconomyManager.SaveData>("EconomyData", (EconomyManager.SaveData)this.GenerateSaveData());
		}

		// Token: 0x06001AB6 RID: 6838 RVA: 0x000607E3 File Offset: 0x0005E9E3
		private void OnDestroy()
		{
			SavesSystem.OnCollectSaveData -= this.OnCollectSaveData;
			SavesSystem.OnSetFile -= this.OnSetSaveFile;
		}

		// Token: 0x170004E2 RID: 1250
		// (get) Token: 0x06001AB7 RID: 6839 RVA: 0x00060807 File Offset: 0x0005EA07
		// (set) Token: 0x06001AB8 RID: 6840 RVA: 0x00060824 File Offset: 0x0005EA24
		public static long Money
		{
			get
			{
				if (EconomyManager.Instance == null)
				{
					return 0L;
				}
				return EconomyManager.Instance.money;
			}
			private set
			{
				long num = EconomyManager.Money;
				if (EconomyManager.Instance == null)
				{
					return;
				}
				EconomyManager.Instance.money = value;
				Action<long, long> onMoneyChanged = EconomyManager.OnMoneyChanged;
				if (onMoneyChanged == null)
				{
					return;
				}
				onMoneyChanged(num, value);
			}
		}

		// Token: 0x170004E3 RID: 1251
		// (get) Token: 0x06001AB9 RID: 6841 RVA: 0x00060861 File Offset: 0x0005EA61
		public static long Cash
		{
			get
			{
				return (long)ItemUtilities.GetItemCount(451);
			}
		}

		// Token: 0x170004E4 RID: 1252
		// (get) Token: 0x06001ABA RID: 6842 RVA: 0x0006086E File Offset: 0x0005EA6E
		public ReadOnlyCollection<int> UnlockedItemIds
		{
			get
			{
				return this.unlockedItemIds.AsReadOnly();
			}
		}

		// Token: 0x140000B3 RID: 179
		// (add) Token: 0x06001ABB RID: 6843 RVA: 0x0006087C File Offset: 0x0005EA7C
		// (remove) Token: 0x06001ABC RID: 6844 RVA: 0x000608B0 File Offset: 0x0005EAB0
		public static event Action<long, long> OnMoneyChanged;

		// Token: 0x140000B4 RID: 180
		// (add) Token: 0x06001ABD RID: 6845 RVA: 0x000608E4 File Offset: 0x0005EAE4
		// (remove) Token: 0x06001ABE RID: 6846 RVA: 0x00060918 File Offset: 0x0005EB18
		public static event Action<int> OnItemUnlockStateChanged;

		// Token: 0x140000B5 RID: 181
		// (add) Token: 0x06001ABF RID: 6847 RVA: 0x0006094C File Offset: 0x0005EB4C
		// (remove) Token: 0x06001AC0 RID: 6848 RVA: 0x00060980 File Offset: 0x0005EB80
		public static event Action<long> OnMoneyPaid;

		// Token: 0x140000B6 RID: 182
		// (add) Token: 0x06001AC1 RID: 6849 RVA: 0x000609B4 File Offset: 0x0005EBB4
		// (remove) Token: 0x06001AC2 RID: 6850 RVA: 0x000609E8 File Offset: 0x0005EBE8
		public static event Action<Cost> OnCostPaid;

		// Token: 0x06001AC3 RID: 6851 RVA: 0x00060A1C File Offset: 0x0005EC1C
		private static bool Pay(long amount, bool accountAvaliable = true, bool cashAvaliale = true)
		{
			long num = (accountAvaliable ? EconomyManager.Money : 0L);
			long num2 = (cashAvaliale ? EconomyManager.Cash : 0L);
			if (num + num2 < amount)
			{
				return false;
			}
			long num3 = amount;
			if (accountAvaliable)
			{
				if (num > amount)
				{
					num3 = 0L;
					EconomyManager.Money -= amount;
				}
				else
				{
					num3 -= num;
					EconomyManager.Money = 0L;
				}
			}
			if (cashAvaliale && num3 > 0L)
			{
				ItemUtilities.ConsumeItems(451, num3);
			}
			if (amount > 0L)
			{
				Action<long> onMoneyPaid = EconomyManager.OnMoneyPaid;
				if (onMoneyPaid != null)
				{
					onMoneyPaid(amount);
				}
			}
			return true;
		}

		// Token: 0x06001AC4 RID: 6852 RVA: 0x00060A9E File Offset: 0x0005EC9E
		public static bool Pay(Cost cost, bool accountAvaliable = true, bool cashAvaliale = true)
		{
			if (!EconomyManager.IsEnough(cost, accountAvaliable, true))
			{
				return false;
			}
			if (!EconomyManager.Pay(cost.money, accountAvaliable, cashAvaliale))
			{
				return false;
			}
			if (!ItemUtilities.ConsumeItems(cost))
			{
				return false;
			}
			Action<Cost> onCostPaid = EconomyManager.OnCostPaid;
			if (onCostPaid != null)
			{
				onCostPaid(cost);
			}
			return true;
		}

		// Token: 0x06001AC5 RID: 6853 RVA: 0x00060ADC File Offset: 0x0005ECDC
		public static bool IsEnough(Cost cost, bool accountAvaliable = true, bool cashAvaliale = true)
		{
			long num = (accountAvaliable ? EconomyManager.Money : 0L);
			long num2 = (cashAvaliale ? EconomyManager.Cash : 0L);
			if (num + num2 < cost.money)
			{
				return false;
			}
			if (cost.items != null)
			{
				foreach (Cost.ItemEntry itemEntry in cost.items)
				{
					if ((long)ItemUtilities.GetItemCount(itemEntry.id) < itemEntry.amount)
					{
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x06001AC6 RID: 6854 RVA: 0x00060B4A File Offset: 0x0005ED4A
		public static bool Add(long amount)
		{
			if (EconomyManager.Instance == null)
			{
				return false;
			}
			EconomyManager.Money += amount;
			return true;
		}

		// Token: 0x06001AC7 RID: 6855 RVA: 0x00060B68 File Offset: 0x0005ED68
		public static bool IsWaitingForUnlockConfirm(int itemTypeID)
		{
			return !GameplayDataSettings.Economy.UnlockedItemByDefault.Contains(itemTypeID) && !(EconomyManager.Instance == null) && EconomyManager.Instance.unlockesWaitingForConfirm.Contains(itemTypeID);
		}

		// Token: 0x06001AC8 RID: 6856 RVA: 0x00060B9D File Offset: 0x0005ED9D
		public static bool IsUnlocked(int itemTypeID)
		{
			return GameplayDataSettings.Economy.UnlockedItemByDefault.Contains(itemTypeID) || (!(EconomyManager.Instance == null) && EconomyManager.Instance.UnlockedItemIds.Contains(itemTypeID));
		}

		// Token: 0x06001AC9 RID: 6857 RVA: 0x00060BD4 File Offset: 0x0005EDD4
		public static void Unlock(int itemTypeID, bool needConfirm = true, bool showUI = true)
		{
			if (EconomyManager.Instance == null)
			{
				return;
			}
			if (EconomyManager.Instance.unlockedItemIds.Contains(itemTypeID))
			{
				return;
			}
			if (EconomyManager.Instance.unlockesWaitingForConfirm.Contains(itemTypeID))
			{
				return;
			}
			if (needConfirm)
			{
				EconomyManager.Instance.unlockesWaitingForConfirm.Add(itemTypeID);
			}
			else
			{
				EconomyManager.Instance.unlockedItemIds.Add(itemTypeID);
			}
			Action<int> onItemUnlockStateChanged = EconomyManager.OnItemUnlockStateChanged;
			if (onItemUnlockStateChanged != null)
			{
				onItemUnlockStateChanged(itemTypeID);
			}
			ItemMetaData metaData = ItemAssetsCollection.GetMetaData(itemTypeID);
			Debug.Log(EconomyManager.ItemUnlockNotificationTextMainFormat);
			Debug.Log(metaData.DisplayName);
			if (showUI)
			{
				NotificationText.Push("Notification_StockShoopItemUnlockFormat".ToPlainText().Format(new
				{
					displayName = metaData.DisplayName
				}));
			}
		}

		// Token: 0x06001ACA RID: 6858 RVA: 0x00060C8C File Offset: 0x0005EE8C
		public static void ConfirmUnlock(int itemTypeID)
		{
			if (EconomyManager.Instance == null)
			{
				return;
			}
			EconomyManager.Instance.unlockesWaitingForConfirm.Remove(itemTypeID);
			EconomyManager.Instance.unlockedItemIds.Add(itemTypeID);
			Action<int> onItemUnlockStateChanged = EconomyManager.OnItemUnlockStateChanged;
			if (onItemUnlockStateChanged == null)
			{
				return;
			}
			onItemUnlockStateChanged(itemTypeID);
		}

		// Token: 0x06001ACB RID: 6859 RVA: 0x00060CD8 File Offset: 0x0005EED8
		public object GenerateSaveData()
		{
			return new EconomyManager.SaveData
			{
				money = EconomyManager.Money,
				unlockedItems = this.unlockedItemIds.ToArray(),
				unlockesWaitingForConfirm = this.unlockesWaitingForConfirm.ToArray()
			};
		}

		// Token: 0x06001ACC RID: 6860 RVA: 0x00060D24 File Offset: 0x0005EF24
		public void SetupSaveData(object rawData)
		{
			if (rawData is EconomyManager.SaveData)
			{
				EconomyManager.SaveData saveData = (EconomyManager.SaveData)rawData;
				this.money = saveData.money;
				this.unlockedItemIds.Clear();
				if (saveData.unlockedItems != null)
				{
					this.unlockedItemIds.AddRange(saveData.unlockedItems);
				}
				this.unlockesWaitingForConfirm.Clear();
				if (saveData.unlockesWaitingForConfirm != null)
				{
					this.unlockesWaitingForConfirm.AddRange(saveData.unlockesWaitingForConfirm);
				}
				return;
			}
		}

		// Token: 0x0400130B RID: 4875
		[SerializeField]
		private string itemUnlockNotificationTextMainFormat = "物品 {itemDisplayName} 已解锁";

		// Token: 0x0400130C RID: 4876
		[SerializeField]
		private string itemUnlockNotificationTextSubFormat = "请在对应商店中查看";

		// Token: 0x0400130F RID: 4879
		private const string saveKey = "EconomyData";

		// Token: 0x04001310 RID: 4880
		private long money;

		// Token: 0x04001311 RID: 4881
		[SerializeField]
		private List<int> unlockedItemIds;

		// Token: 0x04001312 RID: 4882
		[SerializeField]
		private List<int> unlockesWaitingForConfirm;

		// Token: 0x04001317 RID: 4887
		public const int CashItemID = 451;

		// Token: 0x020005BD RID: 1469
		[Serializable]
		public struct SaveData
		{
			// Token: 0x04002066 RID: 8294
			public long money;

			// Token: 0x04002067 RID: 8295
			public int[] unlockedItems;

			// Token: 0x04002068 RID: 8296
			public int[] unlockesWaitingForConfirm;
		}
	}
}
