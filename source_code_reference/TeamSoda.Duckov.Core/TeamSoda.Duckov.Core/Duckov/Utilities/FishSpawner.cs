using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Duckov.Weathers;
using ItemStatsSystem;
using UnityEngine;

namespace Duckov.Utilities
{
	// Token: 0x020003F5 RID: 1013
	public class FishSpawner : MonoBehaviour
	{
		// Token: 0x060024AA RID: 9386 RVA: 0x0007EC7A File Offset: 0x0007CE7A
		public void CalculateChances()
		{
			this.tags.RefreshPercent();
			this.qualities.RefreshPercent();
		}

		// Token: 0x060024AB RID: 9387 RVA: 0x0007EC92 File Offset: 0x0007CE92
		private void Awake()
		{
			this.excludeTagsReal = new List<Tag>();
		}

		// Token: 0x060024AC RID: 9388 RVA: 0x0007EC9F File Offset: 0x0007CE9F
		private void Start()
		{
		}

		// Token: 0x060024AD RID: 9389 RVA: 0x0007ECA4 File Offset: 0x0007CEA4
		public async UniTask<Item> Spawn(int baitID, float luck)
		{
			int num = -1;
			bool atNight = TimeOfDayController.Instance.AtNight;
			Weather currentWeather = TimeOfDayController.Instance.CurrentWeather;
			foreach (FishSpawner.SpecialPair specialPair in this.specialPairs)
			{
				if (baitID == specialPair.baitID && global::UnityEngine.Random.Range(0f, 1f) < specialPair.chance && this.CheckFishDayNightAndWeather(specialPair.fishID, atNight, currentWeather))
				{
					num = specialPair.fishID;
					break;
				}
			}
			if (num == -1)
			{
				luck = Mathf.Max(luck, 0.1f);
				float num2 = 1f - 1f / luck;
				Tag random = this.tags.GetRandom(0f);
				int random2 = this.qualities.GetRandom(num2);
				this.CalculateTags(atNight, currentWeather);
				int[] array = FishSpawner.Search(new ItemFilter
				{
					requireTags = new Tag[] { random },
					excludeTags = this.excludeTagsReal.ToArray(),
					minQuality = random2,
					maxQuality = random2
				});
				if (array.Length < 1)
				{
					Debug.Log(string.Format("LootBox未找到任何合适的随机物品\n Tag:{0} Quality:{1}", random.DisplayName, random2));
					return null;
				}
				num = array.GetRandom<int>();
			}
			return await ItemAssetsCollection.InstantiateAsync(num);
		}

		// Token: 0x060024AE RID: 9390 RVA: 0x0007ECF7 File Offset: 0x0007CEF7
		public static int[] Search(ItemFilter filter)
		{
			return ItemAssetsCollection.Search(filter);
		}

		// Token: 0x060024AF RID: 9391 RVA: 0x0007ED00 File Offset: 0x0007CF00
		private void CalculateTags(bool atNight, Weather weather)
		{
			this.excludeTagsReal.Clear();
			this.excludeTagsReal.AddRange(this.excludeTags);
			if (atNight)
			{
				this.excludeTagsReal.Add(this.Fish_OnlyDay);
			}
			else
			{
				this.excludeTagsReal.Add(this.Fish_OnlyNight);
			}
			this.excludeTagsReal.Add(this.Fish_OnlySunDay);
			this.excludeTagsReal.Add(this.Fish_OnlyRainDay);
			this.excludeTagsReal.Add(this.Fish_OnlyStorm);
			switch (weather)
			{
			case Weather.Sunny:
				this.excludeTagsReal.Remove(this.Fish_OnlySunDay);
				return;
			case Weather.Cloudy:
				break;
			case Weather.Rainy:
				this.excludeTagsReal.Remove(this.Fish_OnlyRainDay);
				return;
			case Weather.Stormy_I:
				this.excludeTagsReal.Remove(this.Fish_OnlyStorm);
				return;
			case Weather.Stormy_II:
				this.excludeTagsReal.Remove(this.Fish_OnlyStorm);
				break;
			default:
				return;
			}
		}

		// Token: 0x060024B0 RID: 9392 RVA: 0x0007EDEC File Offset: 0x0007CFEC
		private bool CheckFishDayNightAndWeather(int fishID, bool atNight, Weather currentWeather)
		{
			ItemMetaData metaData = ItemAssetsCollection.GetMetaData(fishID);
			return (!metaData.tags.Contains(this.Fish_OnlyNight) || atNight) && (!metaData.tags.Contains(this.Fish_OnlyDay) || !atNight) && (!metaData.tags.Contains(this.Fish_OnlyRainDay) || currentWeather == Weather.Rainy) && (!metaData.tags.Contains(this.Fish_OnlySunDay) || currentWeather == Weather.Sunny) && (!metaData.tags.Contains(this.Fish_OnlyStorm) || currentWeather == Weather.Stormy_I || currentWeather == Weather.Stormy_II);
		}

		// Token: 0x040018FA RID: 6394
		[SerializeField]
		private List<FishSpawner.SpecialPair> specialPairs;

		// Token: 0x040018FB RID: 6395
		[SerializeField]
		private RandomContainer<Tag> tags;

		// Token: 0x040018FC RID: 6396
		[SerializeField]
		private List<Tag> excludeTags;

		// Token: 0x040018FD RID: 6397
		[SerializeField]
		private RandomContainer<int> qualities;

		// Token: 0x040018FE RID: 6398
		private List<Tag> excludeTagsReal;

		// Token: 0x040018FF RID: 6399
		[SerializeField]
		private Tag Fish_OnlyDay;

		// Token: 0x04001900 RID: 6400
		[SerializeField]
		private Tag Fish_OnlyNight;

		// Token: 0x04001901 RID: 6401
		[SerializeField]
		private Tag Fish_OnlySunDay;

		// Token: 0x04001902 RID: 6402
		[SerializeField]
		private Tag Fish_OnlyRainDay;

		// Token: 0x04001903 RID: 6403
		[SerializeField]
		private Tag Fish_OnlyStorm;

		// Token: 0x0200065A RID: 1626
		[Serializable]
		private struct SpecialPair
		{
			// Token: 0x040022DB RID: 8923
			[ItemTypeID]
			public int baitID;

			// Token: 0x040022DC RID: 8924
			[ItemTypeID]
			public int fishID;

			// Token: 0x040022DD RID: 8925
			[Range(0f, 1f)]
			public float chance;
		}
	}
}
