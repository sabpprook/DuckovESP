using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Duckov.Scenes;
using ItemStatsSystem;
using UnityEngine;

namespace Duckov.Utilities
{
	// Token: 0x020003F6 RID: 1014
	[RequireComponent(typeof(Points))]
	public class LootSpawner : MonoBehaviour
	{
		// Token: 0x1700070D RID: 1805
		// (get) Token: 0x060024B2 RID: 9394 RVA: 0x0007EE86 File Offset: 0x0007D086
		public bool RandomFromPool
		{
			get
			{
				return this.randomGenrate && this.randomFromPool;
			}
		}

		// Token: 0x1700070E RID: 1806
		// (get) Token: 0x060024B3 RID: 9395 RVA: 0x0007EE98 File Offset: 0x0007D098
		public bool RandomButNotFromPool
		{
			get
			{
				return this.randomGenrate && !this.randomFromPool;
			}
		}

		// Token: 0x060024B4 RID: 9396 RVA: 0x0007EEAD File Offset: 0x0007D0AD
		public void CalculateChances()
		{
			this.tags.RefreshPercent();
			this.qualities.RefreshPercent();
			this.randomPool.RefreshPercent();
		}

		// Token: 0x060024B5 RID: 9397 RVA: 0x0007EED0 File Offset: 0x0007D0D0
		private void Start()
		{
			if (this.points == null)
			{
				this.points = base.GetComponent<Points>();
			}
			bool flag = false;
			int key = this.GetKey();
			object obj;
			if (MultiSceneCore.Instance.inLevelData.TryGetValue(key, out obj) && obj is bool)
			{
				bool flag2 = (bool)obj;
				flag = flag2;
			}
			if (!flag)
			{
				if (global::UnityEngine.Random.Range(0f, 1f) <= this.spawnChance)
				{
					this.Setup().Forget();
				}
				MultiSceneCore.Instance.inLevelData.Add(key, true);
			}
		}

		// Token: 0x060024B6 RID: 9398 RVA: 0x0007EF64 File Offset: 0x0007D164
		private int GetKey()
		{
			Transform transform = base.transform.parent;
			string text = base.transform.GetSiblingIndex().ToString();
			while (transform != null)
			{
				text = string.Format("{0}/{1}", transform.GetSiblingIndex(), text);
				transform = transform.parent;
			}
			text = string.Format("{0}/{1}", base.gameObject.scene.buildIndex, text);
			return text.GetHashCode();
		}

		// Token: 0x060024B7 RID: 9399 RVA: 0x0007EFE4 File Offset: 0x0007D1E4
		public async UniTask Setup()
		{
			this.typeIds.Clear();
			if (this.randomGenrate && !this.randomFromPool)
			{
				int num = global::UnityEngine.Random.Range(this.randomCount.x, this.randomCount.y);
				if (!this.excludeTags.Contains(GameplayDataSettings.Tags.Special))
				{
					this.excludeTags.Add(GameplayDataSettings.Tags.Special);
				}
				if (!LevelManager.Rule.AdvancedDebuffMode && !this.excludeTags.Contains(GameplayDataSettings.Tags.AdvancedDebuffMode))
				{
					this.excludeTags.Add(GameplayDataSettings.Tags.AdvancedDebuffMode);
				}
				for (int j = 0; j < num; j++)
				{
					Tag random = this.tags.GetRandom(0f);
					int random2 = this.qualities.GetRandom(0f);
					int[] array = LootSpawner.Search(new ItemFilter
					{
						requireTags = new Tag[] { random },
						excludeTags = this.excludeTags.ToArray(),
						minQuality = random2,
						maxQuality = random2
					});
					if (array.Length >= 1)
					{
						int random3 = array.GetRandom<int>();
						this.typeIds.Add(random3);
					}
				}
			}
			else if (this.randomGenrate && this.randomFromPool)
			{
				int num2 = global::UnityEngine.Random.Range(this.randomCount.x, this.randomCount.y);
				for (int k = 0; k < num2; k++)
				{
					LootSpawner.Entry random4 = this.randomPool.GetRandom(0f);
					this.typeIds.Add(random4.itemTypeID);
				}
			}
			else
			{
				this.typeIds.AddRange(this.fixedItems);
			}
			this.loading = true;
			int idCount = this.typeIds.Count;
			List<Vector3> spawnPoints = this.points.GetRandomPoints(idCount);
			for (int i = 0; i < idCount; i++)
			{
				(await ItemAssetsCollection.InstantiateAsync(this.typeIds[i])).Drop(spawnPoints[i] + Vector3.up * 0.5f, false, Vector3.up, 360f);
			}
			this.loading = false;
		}

		// Token: 0x060024B8 RID: 9400 RVA: 0x0007F027 File Offset: 0x0007D227
		public static int[] Search(ItemFilter filter)
		{
			return ItemAssetsCollection.Search(filter);
		}

		// Token: 0x060024B9 RID: 9401 RVA: 0x0007F02F File Offset: 0x0007D22F
		private void OnValidate()
		{
			if (this.points == null)
			{
				this.points = base.GetComponent<Points>();
			}
		}

		// Token: 0x04001904 RID: 6404
		[Range(0f, 1f)]
		public float spawnChance = 1f;

		// Token: 0x04001905 RID: 6405
		public bool randomGenrate = true;

		// Token: 0x04001906 RID: 6406
		public bool randomFromPool;

		// Token: 0x04001907 RID: 6407
		[SerializeField]
		private Vector2Int randomCount = new Vector2Int(1, 1);

		// Token: 0x04001908 RID: 6408
		[SerializeField]
		private RandomContainer<Tag> tags;

		// Token: 0x04001909 RID: 6409
		[SerializeField]
		private List<Tag> excludeTags;

		// Token: 0x0400190A RID: 6410
		[SerializeField]
		private RandomContainer<int> qualities;

		// Token: 0x0400190B RID: 6411
		[SerializeField]
		private RandomContainer<LootSpawner.Entry> randomPool;

		// Token: 0x0400190C RID: 6412
		[ItemTypeID]
		[SerializeField]
		private List<int> fixedItems;

		// Token: 0x0400190D RID: 6413
		[SerializeField]
		private Points points;

		// Token: 0x0400190E RID: 6414
		private bool loading;

		// Token: 0x0400190F RID: 6415
		[SerializeField]
		[ItemTypeID]
		private List<int> typeIds;

		// Token: 0x0200065C RID: 1628
		[Serializable]
		private struct Entry
		{
			// Token: 0x040022E4 RID: 8932
			[ItemTypeID]
			[SerializeField]
			public int itemTypeID;
		}
	}
}
