using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Duckov.Utilities;
using UnityEngine;

namespace Duckov.MiniGames.GoldMiner
{
	// Token: 0x0200028E RID: 654
	public class GoldMinerShop : MiniGameBehaviour
	{
		// Token: 0x06001557 RID: 5463 RVA: 0x0004EFEC File Offset: 0x0004D1EC
		private void Clear()
		{
			this.capacity = this.master.run.shopCapacity;
			for (int i = 0; i < this.stock.Count; i++)
			{
				ShopEntity shopEntity = this.stock[i];
				if (shopEntity != null && (shopEntity.sold || !shopEntity.locked))
				{
					this.stock[i] = null;
				}
			}
			for (int j = this.capacity; j < this.stock.Count; j++)
			{
				if (this.stock[j] == null)
				{
					this.stock.RemoveAt(j);
				}
			}
		}

		// Token: 0x06001558 RID: 5464 RVA: 0x0004F088 File Offset: 0x0004D288
		private void Refill()
		{
			this.capacity = this.master.run.shopCapacity;
			for (int i = 0; i < this.capacity; i++)
			{
				if (this.stock.Count <= i)
				{
					this.stock.Add(null);
				}
				ShopEntity shopEntity = this.stock[i];
				if (shopEntity == null || shopEntity.sold)
				{
					this.stock[i] = this.GenerateNewShopItem();
				}
			}
		}

		// Token: 0x06001559 RID: 5465 RVA: 0x0004F100 File Offset: 0x0004D300
		private void RefreshStock()
		{
			this.Clear();
			this.CacheValidCandiateLists();
			this.Refill();
			Action action = this.onAfterOperation;
			if (action == null)
			{
				return;
			}
			action();
		}

		// Token: 0x0600155A RID: 5466 RVA: 0x0004F124 File Offset: 0x0004D324
		private void CacheValidCandiateLists()
		{
			for (int i = 0; i < 5; i++)
			{
				int num = i + 1;
				List<GoldMinerArtifact> list = this.SearchValidCandidateArtifactIDs(num).ToList<GoldMinerArtifact>();
				this.validCandidateLists[i] = list;
			}
			foreach (ShopEntity shopEntity in this.stock)
			{
				if (shopEntity != null && !(shopEntity.artifact == null) && !shopEntity.artifact.AllowMultiple)
				{
					foreach (List<GoldMinerArtifact> list2 in this.validCandidateLists)
					{
						if (list2 != null)
						{
							list2.Remove(shopEntity.artifact);
						}
					}
				}
			}
		}

		// Token: 0x0600155B RID: 5467 RVA: 0x0004F1F0 File Offset: 0x0004D3F0
		private IEnumerable<GoldMinerArtifact> SearchValidCandidateArtifactIDs(int quality)
		{
			using (IEnumerator<GoldMinerArtifact> enumerator = this.master.ArtifactPrefabs.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					GoldMinerArtifact artifact = enumerator.Current;
					if (artifact.Quality == quality && (artifact.AllowMultiple || (this.master.run.GetArtifactCount(artifact.ID) <= 0 && !this.stock.Any((ShopEntity e) => e != null && !e.sold && e.ID == artifact.ID))))
					{
						yield return artifact;
					}
				}
			}
			IEnumerator<GoldMinerArtifact> enumerator = null;
			yield break;
			yield break;
		}

		// Token: 0x0600155C RID: 5468 RVA: 0x0004F207 File Offset: 0x0004D407
		private List<GoldMinerArtifact> GetValidCandidateArtifactIDs(int q)
		{
			return this.validCandidateLists[q - 1];
		}

		// Token: 0x0600155D RID: 5469 RVA: 0x0004F214 File Offset: 0x0004D414
		private ShopEntity GenerateNewShopItem()
		{
			int num = this.qualityDistribute.GetRandom(0f);
			List<GoldMinerArtifact> list = null;
			for (int i = num; i >= 1; i--)
			{
				list = this.GetValidCandidateArtifactIDs(i);
				if (list.Count > 0)
				{
					num = i;
					break;
				}
			}
			GoldMinerArtifact random = list.GetRandom(this.master.run.shopRandom);
			if (random != null && !random.AllowMultiple)
			{
				List<GoldMinerArtifact> validCandidateArtifactIDs = this.GetValidCandidateArtifactIDs(num);
				if (validCandidateArtifactIDs != null)
				{
					validCandidateArtifactIDs.Remove(random);
				}
			}
			if (random == null)
			{
				Debug.Log(string.Format("{0} failed to generate", num));
			}
			return new ShopEntity
			{
				artifact = random
			};
		}

		// Token: 0x0600155E RID: 5470 RVA: 0x0004F2C0 File Offset: 0x0004D4C0
		public bool Buy(ShopEntity entity)
		{
			if (!this.stock.Contains(entity))
			{
				Debug.LogError("Buying entity that doesn't exist in shop stock");
				return false;
			}
			if (entity.sold)
			{
				return false;
			}
			bool flag;
			int num = this.CalculateDealPrice(entity, out flag);
			if (this.master.run.shopTicket > 0)
			{
				this.master.run.shopTicket--;
			}
			else if (!this.master.PayMoney(num))
			{
				return false;
			}
			this.master.run.AttachArtifactFromPrefab(entity.artifact);
			entity.sold = true;
			Action action = this.onAfterOperation;
			if (action != null)
			{
				action();
			}
			return true;
		}

		// Token: 0x0600155F RID: 5471 RVA: 0x0004F368 File Offset: 0x0004D568
		public int CalculateDealPrice(ShopEntity entity, out bool useTicket)
		{
			useTicket = false;
			if (entity == null)
			{
				return 0;
			}
			if (this.master.run.shopTicket > 0)
			{
				useTicket = true;
				return 0;
			}
			GoldMinerArtifact artifact = entity.artifact;
			if (artifact == null)
			{
				return 0;
			}
			return Mathf.CeilToInt((float)artifact.BasePrice * entity.priceFactor * this.master.GlobalPriceFactor);
		}

		// Token: 0x170003F8 RID: 1016
		// (get) Token: 0x06001560 RID: 5472 RVA: 0x0004F3C6 File Offset: 0x0004D5C6
		// (set) Token: 0x06001561 RID: 5473 RVA: 0x0004F3CE File Offset: 0x0004D5CE
		public int refreshChance { get; private set; }

		// Token: 0x06001562 RID: 5474 RVA: 0x0004F3D8 File Offset: 0x0004D5D8
		public async UniTask Execute()
		{
			this.RefreshStock();
			if (this.stock.Count > 0)
			{
				this.stock[0].priceFactor = this.master.run.shopPriceCut.Value;
			}
			this.refreshPrice = Mathf.RoundToInt(this.master.run.shopRefreshPrice.Value);
			this.refreshChance = Mathf.RoundToInt(this.master.run.shopRefreshChances.Value);
			this.complete = false;
			this.ui.gameObject.SetActive(true);
			this.ui.enableInput = false;
			this.ui.Setup(this);
			await UniTask.WaitForSeconds(0.1f, false, PlayerLoopTiming.Update, default(CancellationToken), false);
			this.ui.enableInput = true;
			while (!this.complete)
			{
				await UniTask.Yield();
			}
			this.ui.gameObject.SetActive(false);
		}

		// Token: 0x06001563 RID: 5475 RVA: 0x0004F41B File Offset: 0x0004D61B
		internal void Continue()
		{
			this.complete = true;
		}

		// Token: 0x06001564 RID: 5476 RVA: 0x0004F424 File Offset: 0x0004D624
		internal bool TryRefresh()
		{
			if (this.refreshChance <= 0)
			{
				return false;
			}
			int refreshCost = this.GetRefreshCost();
			if (!this.master.PayMoney(refreshCost))
			{
				return false;
			}
			int refreshChance = this.refreshChance;
			this.refreshChance = refreshChance - 1;
			this.refreshPrice += Mathf.RoundToInt(this.master.run.shopRefreshPriceIncrement.Value);
			this.RefreshStock();
			return true;
		}

		// Token: 0x06001565 RID: 5477 RVA: 0x0004F491 File Offset: 0x0004D691
		internal int GetRefreshCost()
		{
			return this.refreshPrice;
		}

		// Token: 0x04000FCB RID: 4043
		[SerializeField]
		private GoldMiner master;

		// Token: 0x04000FCC RID: 4044
		[SerializeField]
		private GoldMinerShopUI ui;

		// Token: 0x04000FCD RID: 4045
		[SerializeField]
		private RandomContainer<int> qualityDistribute;

		// Token: 0x04000FCE RID: 4046
		public List<ShopEntity> stock = new List<ShopEntity>();

		// Token: 0x04000FCF RID: 4047
		public Action onAfterOperation;

		// Token: 0x04000FD0 RID: 4048
		private int capacity;

		// Token: 0x04000FD1 RID: 4049
		private List<GoldMinerArtifact>[] validCandidateLists = new List<GoldMinerArtifact>[5];

		// Token: 0x04000FD2 RID: 4050
		private bool complete;

		// Token: 0x04000FD3 RID: 4051
		private int refreshPrice = 100;
	}
}
