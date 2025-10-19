using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Cysharp.Threading.Tasks;
using Duckov.Economy;
using Duckov.UI;
using Duckov.Utilities;
using ItemStatsSystem;
using ItemStatsSystem.Data;
using ItemStatsSystem.Items;
using Saves;
using SodaCraft.Localizations;
using SodaCraft.StringUtilities;
using UnityEngine;

namespace Duckov.DeathLotteries
{
	// Token: 0x020002FF RID: 767
	public class DeathLottery : MonoBehaviour
	{
		// Token: 0x1400009E RID: 158
		// (add) Token: 0x060018CF RID: 6351 RVA: 0x0005A560 File Offset: 0x00058760
		// (remove) Token: 0x060018D0 RID: 6352 RVA: 0x0005A594 File Offset: 0x00058794
		public static event Action<DeathLottery> OnRequestUI;

		// Token: 0x060018D1 RID: 6353 RVA: 0x0005A5C7 File Offset: 0x000587C7
		public void RequestUI()
		{
			Action<DeathLottery> onRequestUI = DeathLottery.OnRequestUI;
			if (onRequestUI == null)
			{
				return;
			}
			onRequestUI(this);
		}

		// Token: 0x1700047F RID: 1151
		// (get) Token: 0x060018D2 RID: 6354 RVA: 0x0005A5D9 File Offset: 0x000587D9
		public int MaxChances
		{
			get
			{
				return this.costs.Length;
			}
		}

		// Token: 0x17000480 RID: 1152
		// (get) Token: 0x060018D3 RID: 6355 RVA: 0x0005A5E3 File Offset: 0x000587E3
		public static uint CurrentDeadCharacterToken
		{
			get
			{
				return SavesSystem.Load<uint>("DeadCharacterToken");
			}
		}

		// Token: 0x17000481 RID: 1153
		// (get) Token: 0x060018D4 RID: 6356 RVA: 0x0005A5EF File Offset: 0x000587EF
		private string SelectNotificationFormat
		{
			get
			{
				return this.selectNotificationFormatKey.ToPlainText();
			}
		}

		// Token: 0x17000482 RID: 1154
		// (get) Token: 0x060018D5 RID: 6357 RVA: 0x0005A5FC File Offset: 0x000587FC
		public DeathLottery.OptionalCosts[] Costs
		{
			get
			{
				return this.costs;
			}
		}

		// Token: 0x17000483 RID: 1155
		// (get) Token: 0x060018D6 RID: 6358 RVA: 0x0005A604 File Offset: 0x00058804
		public List<Item> ItemInstances
		{
			get
			{
				return this.itemInstances;
			}
		}

		// Token: 0x17000484 RID: 1156
		// (get) Token: 0x060018D7 RID: 6359 RVA: 0x0005A60C File Offset: 0x0005880C
		public DeathLottery.Status CurrentStatus
		{
			get
			{
				if (this.loading)
				{
					return default(DeathLottery.Status);
				}
				if (!this.status.valid)
				{
					return default(DeathLottery.Status);
				}
				return this.status;
			}
		}

		// Token: 0x17000485 RID: 1157
		// (get) Token: 0x060018D8 RID: 6360 RVA: 0x0005A648 File Offset: 0x00058848
		public int RemainingChances
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x17000486 RID: 1158
		// (get) Token: 0x060018D9 RID: 6361 RVA: 0x0005A656 File Offset: 0x00058856
		public bool Loading
		{
			get
			{
				return this.loading;
			}
		}

		// Token: 0x060018DA RID: 6362 RVA: 0x0005A660 File Offset: 0x00058860
		private async UniTask Load()
		{
			this.loading = true;
			this.status = SavesSystem.Load<DeathLottery.Status>("DeathLottery/status");
			if (!this.status.valid)
			{
				await this.CreateNewStatus();
			}
			else if (this.status.deadCharacterToken != DeathLottery.CurrentDeadCharacterToken)
			{
				await this.CreateNewStatus();
			}
			else
			{
				await this.LoadItemInstances();
			}
			this.loading = false;
		}

		// Token: 0x060018DB RID: 6363 RVA: 0x0005A6A4 File Offset: 0x000588A4
		private async UniTask LoadItemInstances()
		{
			this.ClearItemInstances();
			foreach (ItemTreeData itemTreeData in this.status.candidates)
			{
				if (itemTreeData != null)
				{
					Item item = await ItemTreeData.InstantiateAsync(itemTreeData);
					this.itemInstances.Add(item);
					item.transform.SetParent(base.transform);
				}
			}
			List<ItemTreeData>.Enumerator enumerator = default(List<ItemTreeData>.Enumerator);
		}

		// Token: 0x060018DC RID: 6364 RVA: 0x0005A6E8 File Offset: 0x000588E8
		private void ClearItemInstances()
		{
			for (int i = 0; i < this.itemInstances.Count; i++)
			{
				Item item = this.itemInstances[i];
				if (!(item.ParentItem != null))
				{
					item.DestroyTree();
				}
			}
			this.itemInstances.Clear();
		}

		// Token: 0x060018DD RID: 6365 RVA: 0x0005A737 File Offset: 0x00058937
		[ContextMenu("ForceCreateNewStatus")]
		private void ForceCreateNewStatus()
		{
			if (this.Loading)
			{
				return;
			}
			this.ForceCreateNewStatusTask().Forget();
		}

		// Token: 0x060018DE RID: 6366 RVA: 0x0005A750 File Offset: 0x00058950
		private async UniTask ForceCreateNewStatusTask()
		{
			this.loading = true;
			await this.CreateNewStatus();
			this.loading = false;
		}

		// Token: 0x060018DF RID: 6367 RVA: 0x0005A794 File Offset: 0x00058994
		private async UniTask CreateNewStatus()
		{
			List<ItemTreeData> candidates = new List<ItemTreeData>();
			Item item = await ItemSavesUtilities.LoadLastDeadCharacterItem();
			Item deadCharacter = item;
			if (!(deadCharacter == null))
			{
				List<Item> list = await this.SelectCandidates(deadCharacter);
				deadCharacter.DestroyTree();
				this.ClearItemInstances();
				foreach (Item item2 in list)
				{
					item2.transform.SetParent(base.transform);
					this.itemInstances.Add(item2);
					candidates.Add(ItemTreeData.FromItem(item2));
				}
				this.status = new DeathLottery.Status
				{
					valid = true,
					deadCharacterToken = DeathLottery.CurrentDeadCharacterToken,
					selectedItems = new List<int>(),
					candidates = candidates
				};
			}
		}

		// Token: 0x060018E0 RID: 6368 RVA: 0x0005A7D7 File Offset: 0x000589D7
		private void Awake()
		{
			SavesSystem.OnCollectSaveData += this.Save;
		}

		// Token: 0x060018E1 RID: 6369 RVA: 0x0005A7EA File Offset: 0x000589EA
		private void Start()
		{
			this.Load().Forget();
		}

		// Token: 0x060018E2 RID: 6370 RVA: 0x0005A7F7 File Offset: 0x000589F7
		private void OnDestroy()
		{
			SavesSystem.OnCollectSaveData -= this.Save;
		}

		// Token: 0x060018E3 RID: 6371 RVA: 0x0005A80A File Offset: 0x00058A0A
		private void Save()
		{
			if (this.loading)
			{
				return;
			}
			SavesSystem.Save<DeathLottery.Status>("DeathLottery/status", this.status);
		}

		// Token: 0x060018E4 RID: 6372 RVA: 0x0005A828 File Offset: 0x00058A28
		private async UniTask<List<Item>> SelectCandidates(Item deadCharacter)
		{
			List<Item> candidates = new List<Item>();
			if (deadCharacter.Slots != null)
			{
				foreach (Slot slot in deadCharacter.Slots)
				{
					if (slot != null)
					{
						Item content = slot.Content;
						if (!(content == null) && this.CanBeACandidate(content))
						{
							content.Detach();
							candidates.Add(content);
							if (candidates.Count >= 8)
							{
								goto IL_0112;
							}
						}
					}
				}
			}
			List<Item> list = new List<Item>();
			foreach (Item item in list)
			{
				item.Detach();
			}
			int num = 8 - candidates.Count;
			Item[] randomSubSet = list.GetRandomSubSet(num);
			if (randomSubSet != null)
			{
				candidates.AddRange(randomSubSet);
			}
			IL_0112:
			int maxAttempts = 100;
			int attempts = 0;
			while (candidates.Count < 8)
			{
				int num2 = attempts;
				attempts = num2 + 1;
				if (attempts > maxAttempts)
				{
					Debug.LogError("无法生成candidate");
					break;
				}
				Item item2 = await ItemAssetsCollection.InstantiateAsync(this.dummyItems.GetRandom(0f).typeID);
				if (!(item2 == null))
				{
					candidates.Add(item2);
				}
			}
			candidates.RandomizeOrder<Item>();
			return candidates;
		}

		// Token: 0x060018E5 RID: 6373 RVA: 0x0005A874 File Offset: 0x00058A74
		private bool CanBeACandidate(Item item)
		{
			if (item == null)
			{
				return false;
			}
			foreach (Tag tag in this.excludeTags)
			{
				if (item.Tags.Contains(tag))
				{
					return false;
				}
			}
			foreach (Tag tag2 in this.requireTags)
			{
				if (item.Tags.Contains(tag2))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060018E6 RID: 6374 RVA: 0x0005A8E0 File Offset: 0x00058AE0
		public async UniTask<bool> Select(int index, Cost payWhenSucceed)
		{
			bool flag;
			if (this.loading)
			{
				flag = false;
			}
			else if (!this.status.valid)
			{
				flag = false;
			}
			else if (this.status.SelectedCount >= this.MaxChances)
			{
				flag = false;
			}
			else if (this.status.selectedItems.Contains(index))
			{
				flag = false;
			}
			else
			{
				Item item = await ItemTreeData.InstantiateAsync(this.status.candidates[index]);
				Item instance = item;
				if (instance == null)
				{
					flag = false;
				}
				else if (!payWhenSucceed.Enough)
				{
					flag = false;
				}
				else
				{
					if (instance.GetComponent<ItemSetting_Gun>())
					{
						Item item2 = await ItemUtilities.GenerateBullet(instance);
						if (item2 != null)
						{
							DeathLottery.<Select>g__SendToPlayer|44_0(item2);
						}
					}
					DeathLottery.<Select>g__SendToPlayer|44_0(instance);
					this.status.selectedItems.Add(index);
					payWhenSucceed.Pay(true, true);
					NotificationText.Push(this.SelectNotificationFormat.Format(new
					{
						itemName = instance.DisplayName
					}));
					flag = true;
				}
			}
			return flag;
		}

		// Token: 0x060018E7 RID: 6375 RVA: 0x0005A934 File Offset: 0x00058B34
		internal DeathLottery.OptionalCosts GetCost()
		{
			if (!this.status.valid)
			{
				return default(DeathLottery.OptionalCosts);
			}
			if (this.status.SelectedCount >= this.Costs.Length)
			{
				return default(DeathLottery.OptionalCosts);
			}
			return this.Costs[this.status.SelectedCount];
		}

		// Token: 0x060018E9 RID: 6377 RVA: 0x0005A9AB File Offset: 0x00058BAB
		[CompilerGenerated]
		internal static void <Select>g__SendToPlayer|44_0(Item item)
		{
			if (item == null)
			{
				return;
			}
			if (!ItemUtilities.SendToPlayerCharacter(item, false))
			{
				ItemUtilities.SendToPlayerStorage(item, false);
			}
		}

		// Token: 0x04001212 RID: 4626
		public const int MaxCandidateCount = 8;

		// Token: 0x04001213 RID: 4627
		[SerializeField]
		[LocalizationKey("Default")]
		private string selectNotificationFormatKey = "DeathLottery_SelectNotification";

		// Token: 0x04001214 RID: 4628
		[SerializeField]
		private Tag[] requireTags;

		// Token: 0x04001215 RID: 4629
		[SerializeField]
		private Tag[] excludeTags;

		// Token: 0x04001216 RID: 4630
		[SerializeField]
		private RandomContainer<DeathLottery.dummyItemEntry> dummyItems;

		// Token: 0x04001217 RID: 4631
		[SerializeField]
		private DeathLottery.OptionalCosts[] costs;

		// Token: 0x04001218 RID: 4632
		private DeathLottery.Status status;

		// Token: 0x04001219 RID: 4633
		private List<Item> itemInstances = new List<Item>();

		// Token: 0x0400121A RID: 4634
		private bool loading;

		// Token: 0x02000589 RID: 1417
		[Serializable]
		private struct dummyItemEntry
		{
			// Token: 0x04001FC0 RID: 8128
			[ItemTypeID]
			public int typeID;
		}

		// Token: 0x0200058A RID: 1418
		[Serializable]
		public struct OptionalCosts
		{
			// Token: 0x04001FC1 RID: 8129
			[SerializeField]
			public Cost costA;

			// Token: 0x04001FC2 RID: 8130
			[SerializeField]
			public bool useCostB;

			// Token: 0x04001FC3 RID: 8131
			[SerializeField]
			public Cost costB;
		}

		// Token: 0x0200058B RID: 1419
		[Serializable]
		public struct Status
		{
			// Token: 0x17000768 RID: 1896
			// (get) Token: 0x06002856 RID: 10326 RVA: 0x00094C87 File Offset: 0x00092E87
			public int SelectedCount
			{
				get
				{
					return this.selectedItems.Count;
				}
			}

			// Token: 0x04001FC4 RID: 8132
			public bool valid;

			// Token: 0x04001FC5 RID: 8133
			public uint deadCharacterToken;

			// Token: 0x04001FC6 RID: 8134
			public List<int> selectedItems;

			// Token: 0x04001FC7 RID: 8135
			public List<ItemTreeData> candidates;
		}
	}
}
