using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Duckov.MiniMaps;
using Duckov.Scenes;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x02000100 RID: 256
public class ExitCreator : MonoBehaviour
{
	// Token: 0x170001BA RID: 442
	// (get) Token: 0x06000872 RID: 2162 RVA: 0x000258D9 File Offset: 0x00023AD9
	private int minExitCount
	{
		get
		{
			return LevelConfig.MinExitCount;
		}
	}

	// Token: 0x170001BB RID: 443
	// (get) Token: 0x06000873 RID: 2163 RVA: 0x000258E0 File Offset: 0x00023AE0
	private int maxExitCount
	{
		get
		{
			return LevelConfig.MaxExitCount;
		}
	}

	// Token: 0x06000874 RID: 2164 RVA: 0x000258E8 File Offset: 0x00023AE8
	public void Spawn()
	{
		int num = global::UnityEngine.Random.Range(this.minExitCount, this.maxExitCount + 1);
		if (MultiSceneCore.Instance == null)
		{
			return;
		}
		List<ValueTuple<string, SubSceneEntry.Location>> list = new List<ValueTuple<string, SubSceneEntry.Location>>();
		foreach (SubSceneEntry subSceneEntry in MultiSceneCore.Instance.SubScenes)
		{
			foreach (SubSceneEntry.Location location in subSceneEntry.cachedLocations)
			{
				if (this.IsPathCompitable(location))
				{
					list.Add(new ValueTuple<string, SubSceneEntry.Location>(subSceneEntry.sceneID, location));
				}
			}
		}
		list.Sort(new Comparison<ValueTuple<string, SubSceneEntry.Location>>(this.compareExit));
		if (num > list.Count)
		{
			num = list.Count;
		}
		Vector3 vector;
		MiniMapSettings.TryGetMinimapPosition(LevelManager.Instance.MainCharacter.transform.position, out vector);
		int num2 = Mathf.RoundToInt((float)list.Count * 0.8f);
		if (num > num2)
		{
			num2 = num;
		}
		for (int i = 0; i < num; i++)
		{
			int num3 = global::UnityEngine.Random.Range(0, num2);
			num2--;
			ValueTuple<string, SubSceneEntry.Location> valueTuple = list[num3];
			list.RemoveAt(num3);
			SceneInfoEntry sceneInfo = SceneInfoCollection.GetSceneInfo(valueTuple.Item1);
			this.CreateExit(valueTuple.Item2.position, sceneInfo.BuildIndex, i);
		}
	}

	// Token: 0x06000875 RID: 2165 RVA: 0x00025A70 File Offset: 0x00023C70
	private int compareExit([TupleElementNames(new string[] { "sceneID", "locationData" })] ValueTuple<string, SubSceneEntry.Location> a, [TupleElementNames(new string[] { "sceneID", "locationData" })] ValueTuple<string, SubSceneEntry.Location> b)
	{
		Vector3 vector;
		if (!MiniMapSettings.TryGetMinimapPosition(LevelManager.Instance.MainCharacter.transform.position, out vector))
		{
			return -1;
		}
		Vector3 vector2;
		if (!MiniMapSettings.TryGetMinimapPosition(a.Item2.position, a.Item1, out vector2))
		{
			return -1;
		}
		Vector3 vector3;
		if (!MiniMapSettings.TryGetMinimapPosition(b.Item2.position, b.Item1, out vector3))
		{
			return -1;
		}
		float num = Vector3.Distance(vector, vector2);
		float num2 = Vector3.Distance(vector, vector3);
		if (num > num2)
		{
			return -1;
		}
		return 1;
	}

	// Token: 0x06000876 RID: 2166 RVA: 0x00025AEC File Offset: 0x00023CEC
	private bool IsPathCompitable(SubSceneEntry.Location location)
	{
		string path = location.path;
		int num = path.IndexOf('/');
		return num != -1 && path.Substring(0, num) == "Exits";
	}

	// Token: 0x06000877 RID: 2167 RVA: 0x00025B24 File Offset: 0x00023D24
	private void CreateExit(Vector3 position, int sceneBuildIndex, int debugIndex)
	{
		GameObject gameObject = global::UnityEngine.Object.Instantiate<GameObject>(this.exitPrefab, position, Quaternion.identity);
		if (MultiSceneCore.Instance)
		{
			MultiSceneCore.MoveToActiveWithScene(gameObject, sceneBuildIndex);
		}
		this.SpawnMapElement(position, sceneBuildIndex, debugIndex);
	}

	// Token: 0x06000878 RID: 2168 RVA: 0x00025B60 File Offset: 0x00023D60
	private void SpawnMapElement(Vector3 position, int sceneBuildIndex, int debugIndex)
	{
		SimplePointOfInterest simplePointOfInterest = new GameObject("MapElement").AddComponent<SimplePointOfInterest>();
		simplePointOfInterest.transform.position = position;
		if (MultiSceneCore.Instance != null)
		{
			simplePointOfInterest.Color = this.iconColor;
			simplePointOfInterest.ShadowColor = this.shadowColor;
			simplePointOfInterest.ShadowDistance = this.shadowDistance;
			simplePointOfInterest.IsArea = false;
			simplePointOfInterest.ScaleFactor = 1f;
			string sceneID = SceneInfoCollection.GetSceneID(sceneBuildIndex);
			simplePointOfInterest.Setup(this.icon, this.exitNameKey, false, sceneID);
			SceneManager.MoveGameObjectToScene(simplePointOfInterest.gameObject, MultiSceneCore.MainScene.Value);
		}
	}

	// Token: 0x040007A9 RID: 1961
	public GameObject exitPrefab;

	// Token: 0x040007AA RID: 1962
	[LocalizationKey("Default")]
	public string exitNameKey;

	// Token: 0x040007AB RID: 1963
	[SerializeField]
	private Sprite icon;

	// Token: 0x040007AC RID: 1964
	[SerializeField]
	private Color iconColor = Color.white;

	// Token: 0x040007AD RID: 1965
	[SerializeField]
	private Color shadowColor = Color.white;

	// Token: 0x040007AE RID: 1966
	[SerializeField]
	private float shadowDistance;
}
