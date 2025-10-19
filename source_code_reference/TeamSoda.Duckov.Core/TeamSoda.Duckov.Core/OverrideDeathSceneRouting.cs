using System;
using Duckov.Scenes;
using UnityEngine;

// Token: 0x02000132 RID: 306
public class OverrideDeathSceneRouting : MonoBehaviour
{
	// Token: 0x17000208 RID: 520
	// (get) Token: 0x060009E1 RID: 2529 RVA: 0x0002A738 File Offset: 0x00028938
	// (set) Token: 0x060009E2 RID: 2530 RVA: 0x0002A73F File Offset: 0x0002893F
	public static OverrideDeathSceneRouting Instance { get; private set; }

	// Token: 0x060009E3 RID: 2531 RVA: 0x0002A747 File Offset: 0x00028947
	private void OnEnable()
	{
		if (OverrideDeathSceneRouting.Instance != null)
		{
			Debug.LogError("存在多个OverrideDeathSceneRouting实例");
		}
		OverrideDeathSceneRouting.Instance = this;
	}

	// Token: 0x060009E4 RID: 2532 RVA: 0x0002A766 File Offset: 0x00028966
	private void OnDisable()
	{
		if (OverrideDeathSceneRouting.Instance == this)
		{
			OverrideDeathSceneRouting.Instance = null;
		}
	}

	// Token: 0x060009E5 RID: 2533 RVA: 0x0002A77B File Offset: 0x0002897B
	public string GetSceneID()
	{
		return this.sceneID;
	}

	// Token: 0x040008B3 RID: 2227
	[SceneID]
	[SerializeField]
	private string sceneID;
}
