using System;
using System.Collections.Generic;
using Duckov.Utilities;
using Eflatun.SceneReference;
using UnityEngine;

// Token: 0x02000126 RID: 294
[CreateAssetMenu]
public class SceneInfoCollection : ScriptableObject
{
	// Token: 0x170001F5 RID: 501
	// (get) Token: 0x06000996 RID: 2454 RVA: 0x000298AC File Offset: 0x00027AAC
	internal static SceneInfoCollection Instance
	{
		get
		{
			GameplayDataSettings.SceneManagementData sceneManagement = GameplayDataSettings.SceneManagement;
			if (sceneManagement == null)
			{
				return null;
			}
			return sceneManagement.SceneInfoCollection;
		}
	}

	// Token: 0x170001F6 RID: 502
	// (get) Token: 0x06000997 RID: 2455 RVA: 0x000298BE File Offset: 0x00027ABE
	public static List<SceneInfoEntry> Entries
	{
		get
		{
			if (SceneInfoCollection.Instance == null)
			{
				return null;
			}
			return SceneInfoCollection.Instance.entries;
		}
	}

	// Token: 0x06000998 RID: 2456 RVA: 0x000298DC File Offset: 0x00027ADC
	public SceneInfoEntry InstanceGetSceneInfo(string id)
	{
		return this.entries.Find((SceneInfoEntry e) => e.ID == id);
	}

	// Token: 0x06000999 RID: 2457 RVA: 0x00029910 File Offset: 0x00027B10
	public string InstanceGetSceneID(int buildIndex)
	{
		SceneInfoEntry sceneInfoEntry = this.entries.Find((SceneInfoEntry e) => e != null && e.SceneReference.UnsafeReason == SceneReferenceUnsafeReason.None && e.SceneReference.BuildIndex == buildIndex);
		if (sceneInfoEntry == null)
		{
			return null;
		}
		return sceneInfoEntry.ID;
	}

	// Token: 0x0600099A RID: 2458 RVA: 0x0002994D File Offset: 0x00027B4D
	internal string InstanceGetSceneID(SceneReference sceneRef)
	{
		if (sceneRef.UnsafeReason != SceneReferenceUnsafeReason.None)
		{
			return null;
		}
		return this.InstanceGetSceneID(sceneRef.BuildIndex);
	}

	// Token: 0x0600099B RID: 2459 RVA: 0x00029968 File Offset: 0x00027B68
	internal SceneReference InstanceGetSceneReferencce(string requireSceneID)
	{
		SceneInfoEntry sceneInfoEntry = this.InstanceGetSceneInfo(requireSceneID);
		if (sceneInfoEntry == null)
		{
			return null;
		}
		return sceneInfoEntry.SceneReference;
	}

	// Token: 0x0600099C RID: 2460 RVA: 0x00029988 File Offset: 0x00027B88
	public static SceneInfoEntry GetSceneInfo(string sceneID)
	{
		if (SceneInfoCollection.Instance == null)
		{
			return null;
		}
		return SceneInfoCollection.Instance.InstanceGetSceneInfo(sceneID);
	}

	// Token: 0x0600099D RID: 2461 RVA: 0x000299A4 File Offset: 0x00027BA4
	public static string GetSceneID(SceneReference sceneRef)
	{
		if (SceneInfoCollection.Instance == null)
		{
			return null;
		}
		return SceneInfoCollection.Instance.InstanceGetSceneID(sceneRef);
	}

	// Token: 0x0600099E RID: 2462 RVA: 0x000299C0 File Offset: 0x00027BC0
	public static string GetSceneID(int buildIndex)
	{
		if (SceneInfoCollection.Instance == null)
		{
			return null;
		}
		return SceneInfoCollection.Instance.InstanceGetSceneID(buildIndex);
	}

	// Token: 0x0600099F RID: 2463 RVA: 0x000299DC File Offset: 0x00027BDC
	internal static int GetBuildIndex(string overrideSceneID)
	{
		if (SceneInfoCollection.Instance == null)
		{
			return -1;
		}
		SceneInfoEntry sceneInfoEntry = SceneInfoCollection.Instance.InstanceGetSceneInfo(overrideSceneID);
		if (sceneInfoEntry == null)
		{
			return -1;
		}
		return sceneInfoEntry.BuildIndex;
	}

	// Token: 0x060009A0 RID: 2464 RVA: 0x00029A10 File Offset: 0x00027C10
	internal static SceneInfoEntry GetSceneInfo(int sceneBuildIndex)
	{
		if (SceneInfoCollection.Instance == null)
		{
			return null;
		}
		return SceneInfoCollection.Instance.entries.Find((SceneInfoEntry e) => e.BuildIndex == sceneBuildIndex);
	}

	// Token: 0x0400086B RID: 2155
	public const string BaseSceneID = "Base";

	// Token: 0x0400086C RID: 2156
	[SerializeField]
	private List<SceneInfoEntry> entries;
}
