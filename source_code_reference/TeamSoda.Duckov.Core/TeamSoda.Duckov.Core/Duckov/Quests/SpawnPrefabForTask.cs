using System;
using System.Collections.Generic;
using Duckov.Quests.Tasks;
using Duckov.Scenes;
using Duckov.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Duckov.Quests
{
	// Token: 0x0200033D RID: 829
	public class SpawnPrefabForTask : MonoBehaviour
	{
		// Token: 0x17000555 RID: 1365
		// (get) Token: 0x06001C85 RID: 7301 RVA: 0x00066B57 File Offset: 0x00064D57
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

		// Token: 0x06001C86 RID: 7302 RVA: 0x00066B79 File Offset: 0x00064D79
		private void Awake()
		{
			SceneLoader.onFinishedLoadingScene += this.OnFinishedLoadingScene;
			MultiSceneCore.OnSubSceneLoaded += this.OnSubSceneLoaded;
		}

		// Token: 0x06001C87 RID: 7303 RVA: 0x00066B9D File Offset: 0x00064D9D
		private void Start()
		{
			this.SpawnIfNeeded();
		}

		// Token: 0x06001C88 RID: 7304 RVA: 0x00066BA5 File Offset: 0x00064DA5
		private void OnDestroy()
		{
			SceneLoader.onFinishedLoadingScene -= this.OnFinishedLoadingScene;
			MultiSceneCore.OnSubSceneLoaded -= this.OnSubSceneLoaded;
		}

		// Token: 0x06001C89 RID: 7305 RVA: 0x00066BC9 File Offset: 0x00064DC9
		private void OnSubSceneLoaded(MultiSceneCore core, Scene scene)
		{
			LevelManager.LevelInitializingComment = "Spawning prefabs for task";
			this.SpawnIfNeeded();
		}

		// Token: 0x06001C8A RID: 7306 RVA: 0x00066BDB File Offset: 0x00064DDB
		private void OnFinishedLoadingScene(SceneLoadingContext context)
		{
			this.SpawnIfNeeded();
		}

		// Token: 0x06001C8B RID: 7307 RVA: 0x00066BE4 File Offset: 0x00064DE4
		private void SpawnIfNeeded()
		{
			if (this.prefab == null)
			{
				return;
			}
			if (this.task == null)
			{
				Debug.LogWarning("未配置Task");
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

		// Token: 0x17000556 RID: 1366
		// (get) Token: 0x06001C8C RID: 7308 RVA: 0x00066C38 File Offset: 0x00064E38
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

		// Token: 0x06001C8D RID: 7309 RVA: 0x00066C98 File Offset: 0x00064E98
		private bool IsSpawned()
		{
			object obj;
			return this.spawned || (!(MultiSceneCore.Instance == null) && MultiSceneCore.Instance.inLevelData.TryGetValue(this.SpawnKey, out obj) && obj is bool && (bool)obj);
		}

		// Token: 0x06001C8E RID: 7310 RVA: 0x00066CEC File Offset: 0x00064EEC
		private void Spawn()
		{
			Vector3 vector;
			if (!this.locations.GetRandom<MultiSceneLocation>().TryGetLocationPosition(out vector))
			{
				return;
			}
			GameObject gameObject = global::UnityEngine.Object.Instantiate<GameObject>(this.prefab, vector, Quaternion.identity);
			QuestTask_TaskEvent questTask_TaskEvent = this.task as QuestTask_TaskEvent;
			if (questTask_TaskEvent)
			{
				TaskEventEmitter component = gameObject.GetComponent<TaskEventEmitter>();
				if (component)
				{
					component.SetKey(questTask_TaskEvent.EventKey);
				}
			}
			if (MultiSceneCore.Instance)
			{
				MultiSceneCore.MoveToActiveWithScene(gameObject, base.transform.gameObject.scene.buildIndex);
				MultiSceneCore.Instance.inLevelData[this.SpawnKey] = true;
			}
			this.spawned = true;
		}

		// Token: 0x040013DC RID: 5084
		[SerializeField]
		private string componentID = "SpawnPrefabForTask";

		// Token: 0x040013DD RID: 5085
		private Task _taskCache;

		// Token: 0x040013DE RID: 5086
		[SerializeField]
		private List<MultiSceneLocation> locations;

		// Token: 0x040013DF RID: 5087
		[SerializeField]
		private GameObject prefab;

		// Token: 0x040013E0 RID: 5088
		private bool spawned;
	}
}
