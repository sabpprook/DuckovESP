using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Duckov.Utilities
{
	// Token: 0x020003F7 RID: 1015
	public class SetActiveByPlayerDistance : MonoBehaviour
	{
		// Token: 0x1700070F RID: 1807
		// (get) Token: 0x060024BB RID: 9403 RVA: 0x0007F072 File Offset: 0x0007D272
		// (set) Token: 0x060024BC RID: 9404 RVA: 0x0007F079 File Offset: 0x0007D279
		public static SetActiveByPlayerDistance Instance { get; private set; }

		// Token: 0x060024BD RID: 9405 RVA: 0x0007F084 File Offset: 0x0007D284
		private static List<GameObject> GetListByScene(int sceneBuildIndex, bool createIfNotExist = true)
		{
			List<GameObject> list;
			if (SetActiveByPlayerDistance.listsOfScenes.TryGetValue(sceneBuildIndex, out list))
			{
				return list;
			}
			if (createIfNotExist)
			{
				List<GameObject> list2 = new List<GameObject>();
				SetActiveByPlayerDistance.listsOfScenes[sceneBuildIndex] = list2;
				return list2;
			}
			return null;
		}

		// Token: 0x060024BE RID: 9406 RVA: 0x0007F0BA File Offset: 0x0007D2BA
		private static List<GameObject> GetListByScene(Scene scene, bool createIfNotExist = true)
		{
			return SetActiveByPlayerDistance.GetListByScene(scene.buildIndex, createIfNotExist);
		}

		// Token: 0x060024BF RID: 9407 RVA: 0x0007F0C9 File Offset: 0x0007D2C9
		public static void Register(GameObject gameObject, int sceneBuildIndex)
		{
			SetActiveByPlayerDistance.GetListByScene(sceneBuildIndex, true).Add(gameObject);
		}

		// Token: 0x060024C0 RID: 9408 RVA: 0x0007F0D8 File Offset: 0x0007D2D8
		public static bool Unregister(GameObject gameObject, int sceneBuildIndex)
		{
			List<GameObject> listByScene = SetActiveByPlayerDistance.GetListByScene(sceneBuildIndex, false);
			return listByScene != null && listByScene.Remove(gameObject);
		}

		// Token: 0x060024C1 RID: 9409 RVA: 0x0007F0F9 File Offset: 0x0007D2F9
		public static void Register(GameObject gameObject, Scene scene)
		{
			SetActiveByPlayerDistance.Register(gameObject, scene.buildIndex);
		}

		// Token: 0x060024C2 RID: 9410 RVA: 0x0007F108 File Offset: 0x0007D308
		public static void Unregister(GameObject gameObject, Scene scene)
		{
			SetActiveByPlayerDistance.Unregister(gameObject, scene.buildIndex);
		}

		// Token: 0x17000710 RID: 1808
		// (get) Token: 0x060024C3 RID: 9411 RVA: 0x0007F118 File Offset: 0x0007D318
		public float Distance
		{
			get
			{
				return this.distance;
			}
		}

		// Token: 0x060024C4 RID: 9412 RVA: 0x0007F120 File Offset: 0x0007D320
		private void Awake()
		{
			if (SetActiveByPlayerDistance.Instance == null)
			{
				SetActiveByPlayerDistance.Instance = this;
			}
			this.CleanUp();
			SceneManager.activeSceneChanged += this.OnActiveSceneChanged;
			this.cachedActiveScene = SceneManager.GetActiveScene();
			this.RefreshCache();
		}

		// Token: 0x060024C5 RID: 9413 RVA: 0x0007F160 File Offset: 0x0007D360
		private void CleanUp()
		{
			List<int> list = new List<int>();
			foreach (KeyValuePair<int, List<GameObject>> keyValuePair in SetActiveByPlayerDistance.listsOfScenes)
			{
				List<GameObject> value = keyValuePair.Value;
				value.RemoveAll((GameObject e) => e == null);
				if (value == null || value.Count < 1)
				{
					list.Add(keyValuePair.Key);
				}
			}
			foreach (int num in list)
			{
				SetActiveByPlayerDistance.listsOfScenes.Remove(num);
			}
		}

		// Token: 0x060024C6 RID: 9414 RVA: 0x0007F240 File Offset: 0x0007D440
		private void OnActiveSceneChanged(Scene prev, Scene cur)
		{
			this.RefreshCache();
		}

		// Token: 0x060024C7 RID: 9415 RVA: 0x0007F248 File Offset: 0x0007D448
		private void RefreshCache()
		{
			this.cachedActiveScene = SceneManager.GetActiveScene();
			this.cachedListRef = SetActiveByPlayerDistance.GetListByScene(this.cachedActiveScene, true);
		}

		// Token: 0x17000711 RID: 1809
		// (get) Token: 0x060024C8 RID: 9416 RVA: 0x0007F267 File Offset: 0x0007D467
		private Transform PlayerTransform
		{
			get
			{
				if (!this.cachedPlayerTransform)
				{
					CharacterMainControl main = CharacterMainControl.Main;
					this.cachedPlayerTransform = ((main != null) ? main.transform : null);
				}
				return this.cachedPlayerTransform;
			}
		}

		// Token: 0x060024C9 RID: 9417 RVA: 0x0007F294 File Offset: 0x0007D494
		private void FixedUpdate()
		{
			if (this.PlayerTransform == null)
			{
				return;
			}
			if (this.cachedListRef == null)
			{
				return;
			}
			foreach (GameObject gameObject in this.cachedListRef)
			{
				if (!(gameObject == null))
				{
					bool flag = (this.PlayerTransform.position - gameObject.transform.position).sqrMagnitude < this.distance * this.distance;
					gameObject.gameObject.SetActive(flag);
				}
			}
		}

		// Token: 0x060024CA RID: 9418 RVA: 0x0007F340 File Offset: 0x0007D540
		private void DebugRegister(GameObject go)
		{
			SetActiveByPlayerDistance.Register(go, go.gameObject.scene);
		}

		// Token: 0x04001911 RID: 6417
		private static Dictionary<int, List<GameObject>> listsOfScenes = new Dictionary<int, List<GameObject>>();

		// Token: 0x04001912 RID: 6418
		[SerializeField]
		private float distance = 100f;

		// Token: 0x04001913 RID: 6419
		private Scene cachedActiveScene;

		// Token: 0x04001914 RID: 6420
		private List<GameObject> cachedListRef;

		// Token: 0x04001915 RID: 6421
		private Transform cachedPlayerTransform;
	}
}
