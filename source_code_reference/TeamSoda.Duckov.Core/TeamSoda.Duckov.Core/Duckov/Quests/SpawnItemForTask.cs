using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Duckov.Scenes;
using Duckov.Utilities;
using ItemStatsSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Duckov.Quests
{
	// Token: 0x0200033C RID: 828
	public class SpawnItemForTask : MonoBehaviour
	{
		// Token: 0x17000553 RID: 1363
		// (get) Token: 0x06001C78 RID: 7288 RVA: 0x000668AC File Offset: 0x00064AAC
		private Task task
		{
			get
			{
				if (this._taskCache == null)
				{
					this._taskCache = base.GetComponent<Task>();
				}
				return this._taskCache;
			}
		}

		// Token: 0x06001C79 RID: 7289 RVA: 0x000668CE File Offset: 0x00064ACE
		private void Awake()
		{
			SceneLoader.onFinishedLoadingScene += this.OnFinishedLoadingScene;
			MultiSceneCore.OnSubSceneLoaded += this.OnSubSceneLoaded;
		}

		// Token: 0x06001C7A RID: 7290 RVA: 0x000668F2 File Offset: 0x00064AF2
		private void Start()
		{
			this.SpawnIfNeeded();
		}

		// Token: 0x06001C7B RID: 7291 RVA: 0x000668FA File Offset: 0x00064AFA
		private void OnDestroy()
		{
			SceneLoader.onFinishedLoadingScene -= this.OnFinishedLoadingScene;
			MultiSceneCore.OnSubSceneLoaded -= this.OnSubSceneLoaded;
		}

		// Token: 0x06001C7C RID: 7292 RVA: 0x0006691E File Offset: 0x00064B1E
		private void OnSubSceneLoaded(MultiSceneCore core, Scene scene)
		{
			LevelManager.LevelInitializingComment = "Spawning item for task";
			this.SpawnIfNeeded();
		}

		// Token: 0x06001C7D RID: 7293 RVA: 0x00066930 File Offset: 0x00064B30
		private void OnFinishedLoadingScene(SceneLoadingContext context)
		{
			this.SpawnIfNeeded();
		}

		// Token: 0x06001C7E RID: 7294 RVA: 0x00066938 File Offset: 0x00064B38
		private void SpawnIfNeeded()
		{
			if (this.itemID < 0)
			{
				return;
			}
			if (this.task == null)
			{
				Debug.Log("spawn item task is null");
				return;
			}
			if (this.task.IsFinished())
			{
				return;
			}
			if (this.IsSpawned())
			{
				return;
			}
			this.Spawn();
		}

		// Token: 0x17000554 RID: 1364
		// (get) Token: 0x06001C7F RID: 7295 RVA: 0x00066988 File Offset: 0x00064B88
		private int SpawnKey
		{
			get
			{
				return string.Format("{0}/{1}/{2}/{3}", new object[]
				{
					"SpawnPrefabForTask",
					this.task.Master.ID,
					this.task.ID,
					this.componentID
				}).GetHashCode();
			}
		}

		// Token: 0x06001C80 RID: 7296 RVA: 0x000669E8 File Offset: 0x00064BE8
		private bool IsSpawned()
		{
			object obj;
			return this.spawned || (!(MultiSceneCore.Instance == null) && MultiSceneCore.Instance.inLevelData.TryGetValue(this.SpawnKey, out obj) && obj is bool && (bool)obj);
		}

		// Token: 0x06001C81 RID: 7297 RVA: 0x00066A3C File Offset: 0x00064C3C
		private void Spawn()
		{
			MultiSceneLocation random = this.locations.GetRandom<MultiSceneLocation>();
			Vector3 vector;
			if (!random.TryGetLocationPosition(out vector))
			{
				return;
			}
			if (MultiSceneCore.Instance)
			{
				MultiSceneCore.Instance.inLevelData[this.SpawnKey] = true;
			}
			this.spawned = true;
			this.SpawnItem(vector, base.transform.gameObject.scene, random).Forget();
		}

		// Token: 0x06001C82 RID: 7298 RVA: 0x00066AB0 File Offset: 0x00064CB0
		private async UniTaskVoid SpawnItem(Vector3 pos, Scene scene, MultiSceneLocation location)
		{
			Item item = await ItemAssetsCollection.InstantiateAsync(this.itemID);
			if (!(item == null))
			{
				item.Drop(pos, false, Vector3.zero, 0f);
				if (this.mapElement)
				{
					this.mapElement.SetVisibility(false);
					this.mapElement.locations.Clear();
					this.mapElement.locations.Add(location);
					if (this.task)
					{
						this.mapElement.name = this.task.Master.DisplayName;
					}
					this.mapElement.SetVisibility(true);
					item.onItemTreeChanged += this.OnItemTreeChanged;
				}
			}
		}

		// Token: 0x06001C83 RID: 7299 RVA: 0x00066B03 File Offset: 0x00064D03
		private void OnItemTreeChanged(Item selfItem)
		{
			if (this.mapElement && selfItem.ParentItem)
			{
				this.mapElement.SetVisibility(false);
				selfItem.onItemTreeChanged -= this.OnItemTreeChanged;
			}
		}

		// Token: 0x040013D6 RID: 5078
		[SerializeField]
		private string componentID = "SpawnItemForTask";

		// Token: 0x040013D7 RID: 5079
		private Task _taskCache;

		// Token: 0x040013D8 RID: 5080
		[SerializeField]
		private List<MultiSceneLocation> locations;

		// Token: 0x040013D9 RID: 5081
		[ItemTypeID]
		[SerializeField]
		private int itemID = -1;

		// Token: 0x040013DA RID: 5082
		[SerializeField]
		private MapElementForTask mapElement;

		// Token: 0x040013DB RID: 5083
		private bool spawned;
	}
}
