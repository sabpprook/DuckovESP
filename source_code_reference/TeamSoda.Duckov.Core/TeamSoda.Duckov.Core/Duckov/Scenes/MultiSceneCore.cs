using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Duckov.Economy;
using Duckov.MiniMaps;
using Duckov.UI;
using Duckov.Utilities;
using Eflatun.SceneReference;
using Saves;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Duckov.Scenes
{
	// Token: 0x02000328 RID: 808
	public class MultiSceneCore : MonoBehaviour
	{
		// Token: 0x170004FE RID: 1278
		// (get) Token: 0x06001B36 RID: 6966 RVA: 0x00062F80 File Offset: 0x00061180
		// (set) Token: 0x06001B37 RID: 6967 RVA: 0x00062F87 File Offset: 0x00061187
		public static MultiSceneCore Instance { get; private set; }

		// Token: 0x170004FF RID: 1279
		// (get) Token: 0x06001B38 RID: 6968 RVA: 0x00062F8F File Offset: 0x0006118F
		public List<SubSceneEntry> SubScenes
		{
			get
			{
				return this.subScenes;
			}
		}

		// Token: 0x17000500 RID: 1280
		// (get) Token: 0x06001B39 RID: 6969 RVA: 0x00062F98 File Offset: 0x00061198
		public static Scene? MainScene
		{
			get
			{
				if (MultiSceneCore.Instance == null)
				{
					return null;
				}
				return new Scene?(MultiSceneCore.Instance.gameObject.scene);
			}
		}

		// Token: 0x17000501 RID: 1281
		// (get) Token: 0x06001B3A RID: 6970 RVA: 0x00062FD0 File Offset: 0x000611D0
		public static string ActiveSubSceneID
		{
			get
			{
				if (MultiSceneCore.ActiveSubScene == null)
				{
					return null;
				}
				if (MultiSceneCore.Instance == null)
				{
					return null;
				}
				SubSceneEntry subSceneEntry = MultiSceneCore.Instance.SubScenes.Find((SubSceneEntry e) => e != null && MultiSceneCore.ActiveSubScene.Value.buildIndex == e.Info.BuildIndex);
				if (subSceneEntry == null)
				{
					return null;
				}
				return subSceneEntry.sceneID;
			}
		}

		// Token: 0x17000502 RID: 1282
		// (get) Token: 0x06001B3B RID: 6971 RVA: 0x00063038 File Offset: 0x00061238
		public static Scene? ActiveSubScene
		{
			get
			{
				if (MultiSceneCore.Instance == null)
				{
					return null;
				}
				if (MultiSceneCore.Instance.isLoading)
				{
					return null;
				}
				return new Scene?(MultiSceneCore.Instance.activeSubScene);
			}
		}

		// Token: 0x140000BA RID: 186
		// (add) Token: 0x06001B3C RID: 6972 RVA: 0x00063084 File Offset: 0x00061284
		// (remove) Token: 0x06001B3D RID: 6973 RVA: 0x000630B8 File Offset: 0x000612B8
		public static event Action<MultiSceneCore, Scene> OnSubSceneWillBeUnloaded;

		// Token: 0x140000BB RID: 187
		// (add) Token: 0x06001B3E RID: 6974 RVA: 0x000630EC File Offset: 0x000612EC
		// (remove) Token: 0x06001B3F RID: 6975 RVA: 0x00063120 File Offset: 0x00061320
		public static event Action<MultiSceneCore, Scene> OnSubSceneLoaded;

		// Token: 0x17000503 RID: 1283
		// (get) Token: 0x06001B40 RID: 6976 RVA: 0x00063154 File Offset: 0x00061354
		public SceneInfoEntry SceneInfo
		{
			get
			{
				return SceneInfoCollection.GetSceneInfo(base.gameObject.scene.buildIndex);
			}
		}

		// Token: 0x17000504 RID: 1284
		// (get) Token: 0x06001B41 RID: 6977 RVA: 0x0006317C File Offset: 0x0006137C
		public string DisplayName
		{
			get
			{
				SceneInfoEntry sceneInfo = SceneInfoCollection.GetSceneInfo(base.gameObject.scene.buildIndex);
				if (sceneInfo == null)
				{
					return "?";
				}
				return sceneInfo.DisplayName;
			}
		}

		// Token: 0x17000505 RID: 1285
		// (get) Token: 0x06001B42 RID: 6978 RVA: 0x000631B4 File Offset: 0x000613B4
		public string DisplaynameRaw
		{
			get
			{
				SceneInfoEntry sceneInfo = SceneInfoCollection.GetSceneInfo(base.gameObject.scene.buildIndex);
				if (sceneInfo == null)
				{
					return "?";
				}
				return sceneInfo.DisplayNameRaw;
			}
		}

		// Token: 0x06001B43 RID: 6979 RVA: 0x000631EC File Offset: 0x000613EC
		public static void MoveToActiveWithScene(GameObject go, int sceneBuildIndex)
		{
			if (MultiSceneCore.Instance == null)
			{
				return;
			}
			Transform setActiveWithSceneParent = MultiSceneCore.Instance.GetSetActiveWithSceneParent(sceneBuildIndex);
			go.transform.SetParent(setActiveWithSceneParent);
		}

		// Token: 0x06001B44 RID: 6980 RVA: 0x00063220 File Offset: 0x00061420
		public static void MoveToActiveWithScene(GameObject go)
		{
			int buildIndex = go.scene.buildIndex;
			MultiSceneCore.MoveToActiveWithScene(go, buildIndex);
		}

		// Token: 0x06001B45 RID: 6981 RVA: 0x00063244 File Offset: 0x00061444
		public Transform GetSetActiveWithSceneParent(int sceneBuildIndex)
		{
			GameObject gameObject;
			if (this.setActiveWithSceneObjects.TryGetValue(sceneBuildIndex, out gameObject))
			{
				return gameObject.transform;
			}
			SceneInfoEntry sceneInfoEntry = SceneInfoCollection.GetSceneInfo(sceneBuildIndex);
			if (sceneInfoEntry == null)
			{
				sceneInfoEntry = new SceneInfoEntry();
				Debug.LogWarning(string.Format("BuildIndex {0} 的sceneInfo不存在", sceneBuildIndex));
			}
			GameObject gameObject2 = new GameObject(sceneInfoEntry.ID);
			gameObject2.transform.SetParent(base.transform);
			this.setActiveWithSceneObjects.Add(sceneBuildIndex, gameObject2);
			gameObject2.SetActive(sceneInfoEntry.IsLoaded);
			return gameObject2.transform;
		}

		// Token: 0x140000BC RID: 188
		// (add) Token: 0x06001B46 RID: 6982 RVA: 0x000632CC File Offset: 0x000614CC
		// (remove) Token: 0x06001B47 RID: 6983 RVA: 0x00063300 File Offset: 0x00061500
		public static event Action<MultiSceneCore> OnInstanceAwake;

		// Token: 0x140000BD RID: 189
		// (add) Token: 0x06001B48 RID: 6984 RVA: 0x00063334 File Offset: 0x00061534
		// (remove) Token: 0x06001B49 RID: 6985 RVA: 0x00063368 File Offset: 0x00061568
		public static event Action<MultiSceneCore> OnInstanceDestroy;

		// Token: 0x140000BE RID: 190
		// (add) Token: 0x06001B4A RID: 6986 RVA: 0x0006339C File Offset: 0x0006159C
		// (remove) Token: 0x06001B4B RID: 6987 RVA: 0x000633D0 File Offset: 0x000615D0
		public static event Action<string> OnSetSceneVisited;

		// Token: 0x06001B4C RID: 6988 RVA: 0x00063404 File Offset: 0x00061604
		private void Awake()
		{
			if (MultiSceneCore.Instance == null)
			{
				MultiSceneCore.Instance = this;
			}
			else
			{
				Debug.LogError("Multiple Multi Scene Core detected!");
			}
			Action<MultiSceneCore> onInstanceAwake = MultiSceneCore.OnInstanceAwake;
			if (onInstanceAwake != null)
			{
				onInstanceAwake(this);
			}
			if (this.playAfterLevelInit)
			{
				if (LevelManager.AfterInit)
				{
					this.PlayStinger();
					return;
				}
				LevelManager.OnAfterLevelInitialized += this.OnAfterLevelInitialized;
			}
		}

		// Token: 0x06001B4D RID: 6989 RVA: 0x00063468 File Offset: 0x00061668
		private void OnDestroy()
		{
			Action<MultiSceneCore> onInstanceDestroy = MultiSceneCore.OnInstanceDestroy;
			if (onInstanceDestroy != null)
			{
				onInstanceDestroy(this);
			}
			LevelManager.OnAfterLevelInitialized -= this.OnAfterLevelInitialized;
		}

		// Token: 0x06001B4E RID: 6990 RVA: 0x0006348C File Offset: 0x0006168C
		private void OnAfterLevelInitialized()
		{
			if (this.playAfterLevelInit)
			{
				this.PlayStinger();
			}
		}

		// Token: 0x06001B4F RID: 6991 RVA: 0x0006349C File Offset: 0x0006169C
		public void PlayStinger()
		{
			if (!string.IsNullOrWhiteSpace(this.playStinger))
			{
				AudioManager.PlayStringer(this.playStinger);
			}
		}

		// Token: 0x06001B50 RID: 6992 RVA: 0x000634B8 File Offset: 0x000616B8
		private void Start()
		{
			this.CreatePointsOfInterestsForLocations();
			AudioManager.StopBGM();
			AudioManager.SetState("Level", this.levelStateName);
			if (this.SceneInfo != null && !string.IsNullOrEmpty(this.SceneInfo.ID))
			{
				MultiSceneCore.SetVisited(this.SceneInfo.ID);
			}
		}

		// Token: 0x06001B51 RID: 6993 RVA: 0x0006350A File Offset: 0x0006170A
		public static void SetVisited(string sceneID)
		{
			SavesSystem.Save<bool>("MultiSceneCore_Visited_" + sceneID, true);
			Action<string> onSetSceneVisited = MultiSceneCore.OnSetSceneVisited;
			if (onSetSceneVisited == null)
			{
				return;
			}
			onSetSceneVisited(sceneID);
		}

		// Token: 0x06001B52 RID: 6994 RVA: 0x0006352D File Offset: 0x0006172D
		public static bool GetVisited(string sceneID)
		{
			return SavesSystem.Load<bool>("MultiSceneCore_Visited_" + sceneID);
		}

		// Token: 0x06001B53 RID: 6995 RVA: 0x00063540 File Offset: 0x00061740
		private void CreatePointsOfInterestsForLocations()
		{
			foreach (SubSceneEntry subSceneEntry in this.SubScenes)
			{
				foreach (SubSceneEntry.Location location in subSceneEntry.cachedLocations)
				{
					if (location.showInMap)
					{
						SimplePointOfInterest.Create(location.position, subSceneEntry.sceneID, location.DisplayNameRaw, null, true);
					}
				}
			}
		}

		// Token: 0x06001B54 RID: 6996 RVA: 0x000635EC File Offset: 0x000617EC
		private void CreatePointsOfInterestsForTeleporters()
		{
			foreach (SubSceneEntry subSceneEntry in this.SubScenes)
			{
				foreach (SubSceneEntry.TeleporterInfo teleporterInfo in subSceneEntry.cachedTeleporters)
				{
					SimplePointOfInterest.Create(teleporterInfo.position, subSceneEntry.sceneID, "", GameplayDataSettings.UIStyle.DefaultTeleporterIcon, false).ScaleFactor = GameplayDataSettings.UIStyle.TeleporterIconScale;
				}
			}
		}

		// Token: 0x06001B55 RID: 6997 RVA: 0x000636A4 File Offset: 0x000618A4
		public void BeginLoadSubScene(SceneReference reference)
		{
			this.LoadSubScene(reference, true).Forget<bool>();
		}

		// Token: 0x17000506 RID: 1286
		// (get) Token: 0x06001B56 RID: 6998 RVA: 0x000636B3 File Offset: 0x000618B3
		public bool IsLoading
		{
			get
			{
				return this.isLoading;
			}
		}

		// Token: 0x17000507 RID: 1287
		// (get) Token: 0x06001B57 RID: 6999 RVA: 0x000636BC File Offset: 0x000618BC
		public static string MainSceneID
		{
			get
			{
				return SceneInfoCollection.GetSceneID(MultiSceneCore.MainScene.Value.buildIndex);
			}
		}

		// Token: 0x06001B58 RID: 7000 RVA: 0x000636E4 File Offset: 0x000618E4
		private SceneReference GetSubSceneReference(string sceneID)
		{
			SubSceneEntry subSceneEntry = this.subScenes.Find((SubSceneEntry e) => e.sceneID == sceneID);
			if (subSceneEntry == null)
			{
				return null;
			}
			return subSceneEntry.SceneReference;
		}

		// Token: 0x06001B59 RID: 7001 RVA: 0x00063724 File Offset: 0x00061924
		private async UniTask<bool> LoadSubScene(SceneReference targetScene, bool withBlackScreen = true)
		{
			bool flag;
			if (SceneLoader.IsSceneLoading)
			{
				Debug.LogWarning("已经在加载子场景了");
				flag = false;
			}
			else if (this.isLoading)
			{
				Debug.LogWarning("已经在加载子场景了");
				flag = false;
			}
			else if (targetScene == null)
			{
				Debug.LogWarning("目标场景为空");
				flag = false;
			}
			else
			{
				this.isLoading = true;
				if (withBlackScreen)
				{
					await BlackScreen.ShowAndReturnTask(null, 0f, 0.5f);
				}
				if (Cost.TaskPending)
				{
					Debug.LogError("MultiSceneCore: 检测到正在返还物品");
				}
				float unscaledTime = Time.unscaledTime;
				Scene currentMainScene = base.gameObject.scene;
				SceneManager.SetActiveScene(base.gameObject.scene);
				List<UniTask> list = new List<UniTask>();
				if (this.activeSubScene.isLoaded)
				{
					Action<MultiSceneCore, Scene> onSubSceneWillBeUnloaded = MultiSceneCore.OnSubSceneWillBeUnloaded;
					if (onSubSceneWillBeUnloaded != null)
					{
						onSubSceneWillBeUnloaded(this, this.activeSubScene);
					}
					this.LocalOnSubSceneWillBeUnloaded(this.activeSubScene);
					list.Add(SceneManager.UnloadSceneAsync(this.activeSubScene).ToUniTask(null, PlayerLoopTiming.Update, default(CancellationToken), false));
				}
				UniTask uniTask = SceneManager.LoadSceneAsync(targetScene.BuildIndex, LoadSceneMode.Additive).ToUniTask(null, PlayerLoopTiming.Update, default(CancellationToken), false);
				list.Add(uniTask);
				await UniTask.WhenAll(list);
				if (currentMainScene != SceneManager.GetActiveScene())
				{
					Debug.LogError("Sub-scene loading failed because the Active Scene has Changed during this process!");
					await SceneManager.UnloadSceneAsync(targetScene.BuildIndex);
					flag = false;
				}
				else
				{
					this.activeSubScene = targetScene.LoadedScene;
					this.cachedSubsceneEntry = this.GetSubSceneInfo(this.activeSubScene);
					AudioManager.SetState("GameStatus", "Playing");
					string sceneID = SceneInfoCollection.GetSceneID(targetScene.BuildIndex);
					if (!string.IsNullOrEmpty(sceneID))
					{
						MultiSceneCore.SetVisited(sceneID);
					}
					this.LocalOnSubSceneLoaded(this.activeSubScene);
					SceneManager.SetActiveScene(this.activeSubScene);
					await UniTask.NextFrame();
					bool playingBGM = AudioManager.PlayingBGM;
					BlackScreen.HideAndReturnTask(null, 0f, 0.5f).Forget();
					this.isLoading = false;
					Action<MultiSceneCore, Scene> onSubSceneLoaded = MultiSceneCore.OnSubSceneLoaded;
					if (onSubSceneLoaded != null)
					{
						onSubSceneLoaded(this, this.activeSubScene);
					}
					flag = true;
				}
			}
			return flag;
		}

		// Token: 0x06001B5A RID: 7002 RVA: 0x00063778 File Offset: 0x00061978
		private void LocalOnSubSceneWillBeUnloaded(Scene scene)
		{
			this.subScenes.Find((SubSceneEntry e) => e != null && e.Info.BuildIndex == scene.buildIndex);
			Transform setActiveWithSceneParent = this.GetSetActiveWithSceneParent(scene.buildIndex);
			Debug.Log(string.Format("Setting Active False {0}  {1}", setActiveWithSceneParent.name, scene.buildIndex));
			setActiveWithSceneParent.gameObject.SetActive(false);
		}

		// Token: 0x06001B5B RID: 7003 RVA: 0x000637F0 File Offset: 0x000619F0
		private void LocalOnSubSceneLoaded(Scene scene)
		{
			this.subScenes.Find((SubSceneEntry e) => e != null && e.Info.BuildIndex == scene.buildIndex);
			this.GetSetActiveWithSceneParent(scene.buildIndex).gameObject.SetActive(true);
		}

		// Token: 0x06001B5C RID: 7004 RVA: 0x00063840 File Offset: 0x00061A40
		public async UniTask<bool> LoadAndTeleport(MultiSceneLocation location)
		{
			bool flag;
			if (!this.SubScenes.Any((SubSceneEntry e) => e.sceneID == location.SceneID))
			{
				flag = false;
			}
			else
			{
				UniTask<bool>.Awaiter awaiter = this.LoadSubScene(location.Scene, true).GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					await awaiter;
					UniTask<bool>.Awaiter awaiter2;
					awaiter = awaiter2;
					awaiter2 = default(UniTask<bool>.Awaiter);
				}
				if (!awaiter.GetResult())
				{
					flag = false;
				}
				else
				{
					Transform locationTransform = location.GetLocationTransform();
					if (locationTransform == null)
					{
						Debug.LogError("Location Not Found: " + location.Scene.Name + "/" + location.LocationName);
					}
					LevelManager.Instance.MainCharacter.SetPosition(locationTransform.position);
					flag = true;
				}
			}
			return flag;
		}

		// Token: 0x06001B5D RID: 7005 RVA: 0x0006388C File Offset: 0x00061A8C
		public async UniTask<bool> LoadAndTeleport(string sceneID, Vector3 position, bool subSceneLocation = false)
		{
			bool flag;
			if (!this.SubScenes.Any((SubSceneEntry e) => e.sceneID == sceneID))
			{
				flag = false;
			}
			else
			{
				SceneReference subSceneReference = this.GetSubSceneReference(sceneID);
				UniTask<bool>.Awaiter awaiter = this.LoadSubScene(subSceneReference, true).GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					await awaiter;
					UniTask<bool>.Awaiter awaiter2;
					awaiter = awaiter2;
					awaiter2 = default(UniTask<bool>.Awaiter);
				}
				if (!awaiter.GetResult())
				{
					flag = false;
				}
				else
				{
					CharacterMainControl mainCharacter = LevelManager.Instance.MainCharacter;
					Vector3 vector = position;
					if (subSceneLocation && !MiniMapSettings.TryGetWorldPosition(position, sceneID, out vector))
					{
						flag = false;
					}
					else
					{
						mainCharacter.SetPosition(vector);
						flag = true;
					}
				}
			}
			return flag;
		}

		// Token: 0x06001B5E RID: 7006 RVA: 0x000638E8 File Offset: 0x00061AE8
		public static void MoveToMainScene(GameObject gameObject)
		{
			if (MultiSceneCore.Instance == null)
			{
				Debug.LogError("移动到主场景失败，因为MultiSceneCore不存在");
				return;
			}
			SceneManager.MoveGameObjectToScene(gameObject, MultiSceneCore.MainScene.Value);
		}

		// Token: 0x06001B5F RID: 7007 RVA: 0x00063920 File Offset: 0x00061B20
		public void CacheLocations()
		{
		}

		// Token: 0x06001B60 RID: 7008 RVA: 0x00063922 File Offset: 0x00061B22
		public void CacheTeleporters()
		{
		}

		// Token: 0x06001B61 RID: 7009 RVA: 0x00063924 File Offset: 0x00061B24
		private Vector3 GetClosestTeleporterPosition(Vector3 pos)
		{
			float num = float.MaxValue;
			Vector3 vector = pos;
			foreach (SubSceneEntry subSceneEntry in this.subScenes)
			{
				foreach (SubSceneEntry.TeleporterInfo teleporterInfo in subSceneEntry.cachedTeleporters)
				{
					float magnitude = (teleporterInfo.position - pos).magnitude;
					if (magnitude < num)
					{
						num = magnitude;
						vector = teleporterInfo.position;
					}
				}
			}
			return vector;
		}

		// Token: 0x06001B62 RID: 7010 RVA: 0x000639DC File Offset: 0x00061BDC
		internal bool TryGetCachedPosition(MultiSceneLocation location, out Vector3 result)
		{
			return this.TryGetCachedPosition(location.SceneID, location.LocationName, out result);
		}

		// Token: 0x06001B63 RID: 7011 RVA: 0x000639F4 File Offset: 0x00061BF4
		internal bool TryGetCachedPosition(string sceneID, string locationName, out Vector3 result)
		{
			result = default(Vector3);
			SubSceneEntry subSceneEntry = this.subScenes.Find((SubSceneEntry e) => e != null && e.sceneID == sceneID);
			return subSceneEntry != null && subSceneEntry.TryGetCachedPosition(locationName, out result);
		}

		// Token: 0x06001B64 RID: 7012 RVA: 0x00063A40 File Offset: 0x00061C40
		internal SubSceneEntry GetSubSceneInfo(Scene scene)
		{
			return this.subScenes.Find((SubSceneEntry e) => e != null && e.Info != null && e.Info.BuildIndex == scene.buildIndex);
		}

		// Token: 0x06001B65 RID: 7013 RVA: 0x00063A71 File Offset: 0x00061C71
		public SubSceneEntry GetSubSceneInfo()
		{
			return this.cachedSubsceneEntry;
		}

		// Token: 0x04001366 RID: 4966
		[SerializeField]
		private string levelStateName = "None";

		// Token: 0x04001367 RID: 4967
		[SerializeField]
		private string playStinger = "";

		// Token: 0x04001368 RID: 4968
		[SerializeField]
		private bool playAfterLevelInit;

		// Token: 0x04001369 RID: 4969
		[SerializeField]
		private List<SubSceneEntry> subScenes;

		// Token: 0x0400136A RID: 4970
		private Scene activeSubScene;

		// Token: 0x0400136B RID: 4971
		[HideInInspector]
		public List<int> usedCreatorIds = new List<int>();

		// Token: 0x0400136C RID: 4972
		[HideInInspector]
		public Dictionary<int, object> inLevelData = new Dictionary<int, object>();

		// Token: 0x0400136F RID: 4975
		[SerializeField]
		private bool teleportToRandomOnLevelInitialized;

		// Token: 0x04001370 RID: 4976
		private Dictionary<int, GameObject> setActiveWithSceneObjects = new Dictionary<int, GameObject>();

		// Token: 0x04001374 RID: 4980
		private bool isLoading;

		// Token: 0x04001375 RID: 4981
		private SubSceneEntry cachedSubsceneEntry;
	}
}
