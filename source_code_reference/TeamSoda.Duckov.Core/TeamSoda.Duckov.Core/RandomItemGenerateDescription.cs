using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Duckov.Utilities;
using ItemStatsSystem;
using UnityEngine;

// Token: 0x020000F8 RID: 248
[Serializable]
public struct RandomItemGenerateDescription
{
	// Token: 0x0600084E RID: 2126 RVA: 0x00024EF4 File Offset: 0x000230F4
	public async UniTask<List<Item>> Generate(int count = -1)
	{
		List<Item> items = new List<Item>();
		if (count < 0)
		{
			count = global::UnityEngine.Random.Range(this.randomCount.x, this.randomCount.y + 1);
		}
		List<Item> list;
		if (count < 1)
		{
			list = items;
		}
		else
		{
			List<int> list2 = new List<int>();
			if (this.randomFromPool)
			{
				if (global::UnityEngine.Random.Range(0f, 1f) > this.chance)
				{
					list = items;
				}
				else
				{
					for (int i = 0; i < count; i++)
					{
						Item item = await ItemAssetsCollection.InstantiateAsync(this.itemPool.GetRandom(0f).itemTypeID);
						if (!(item == null))
						{
							items.Add(item);
							this.SetDurabilityIfNeeded(item);
						}
					}
					list = items;
				}
			}
			else
			{
				if (!this.excludeTags.Contains(GameplayDataSettings.Tags.Special))
				{
					this.excludeTags.Add(GameplayDataSettings.Tags.Special);
				}
				if (!LevelManager.Rule.AdvancedDebuffMode && !this.excludeTags.Contains(GameplayDataSettings.Tags.AdvancedDebuffMode))
				{
					this.excludeTags.Add(GameplayDataSettings.Tags.AdvancedDebuffMode);
				}
				for (int j = 0; j < count; j++)
				{
					if (global::UnityEngine.Random.Range(0f, 1f) <= this.chance)
					{
						Tag random = this.tags.GetRandom(0f);
						int random2 = this.qualities.GetRandom(0f);
						List<Tag> list3 = new List<Tag>();
						if (random != null)
						{
							list3.Add(random);
						}
						if (this.addtionalRequireTags.Count > 0)
						{
							list3.AddRange(this.addtionalRequireTags);
						}
						int[] array = ItemAssetsCollection.Search(new ItemFilter
						{
							requireTags = list3.ToArray(),
							excludeTags = this.excludeTags.ToArray(),
							minQuality = random2,
							maxQuality = random2
						});
						if (array.Length >= 1)
						{
							list2.Add(array.GetRandom<int>());
						}
					}
				}
				foreach (int num in list2)
				{
					Item item2 = await ItemAssetsCollection.InstantiateAsync(num);
					if (!(item2 == null))
					{
						items.Add(item2);
						this.SetDurabilityIfNeeded(item2);
					}
				}
				List<int>.Enumerator enumerator = default(List<int>.Enumerator);
				list = items;
			}
		}
		return list;
	}

	// Token: 0x0600084F RID: 2127 RVA: 0x00024F44 File Offset: 0x00023144
	private void SetDurabilityIfNeeded(Item targetItem)
	{
		if (targetItem == null)
		{
			return;
		}
		if (this.controlDurability && targetItem.UseDurability)
		{
			float num = global::UnityEngine.Random.Range(this.durabilityIntegrity.x, this.durabilityIntegrity.y);
			targetItem.DurabilityLoss = 1f - num;
			float num2 = global::UnityEngine.Random.Range(this.durability.x, this.durability.y);
			if (num2 > num)
			{
				num2 = num;
			}
			targetItem.Durability = targetItem.MaxDurability * num2;
		}
	}

	// Token: 0x06000850 RID: 2128 RVA: 0x00024FC4 File Offset: 0x000231C4
	private void RefreshPercent()
	{
		this.itemPool.RefreshPercent();
	}

	// Token: 0x0400077E RID: 1918
	[TextArea]
	[SerializeField]
	private string comment;

	// Token: 0x0400077F RID: 1919
	[Range(0f, 1f)]
	public float chance;

	// Token: 0x04000780 RID: 1920
	public Vector2Int randomCount;

	// Token: 0x04000781 RID: 1921
	public bool controlDurability;

	// Token: 0x04000782 RID: 1922
	public Vector2 durability;

	// Token: 0x04000783 RID: 1923
	public Vector2 durabilityIntegrity;

	// Token: 0x04000784 RID: 1924
	public bool randomFromPool;

	// Token: 0x04000785 RID: 1925
	[SerializeField]
	public RandomContainer<RandomItemGenerateDescription.Entry> itemPool;

	// Token: 0x04000786 RID: 1926
	public RandomContainer<Tag> tags;

	// Token: 0x04000787 RID: 1927
	public List<Tag> addtionalRequireTags;

	// Token: 0x04000788 RID: 1928
	public List<Tag> excludeTags;

	// Token: 0x04000789 RID: 1929
	public RandomContainer<int> qualities;

	// Token: 0x0200047C RID: 1148
	[Serializable]
	public struct Entry
	{
		// Token: 0x04001B66 RID: 7014
		[ItemTypeID]
		[SerializeField]
		public int itemTypeID;
	}
}
