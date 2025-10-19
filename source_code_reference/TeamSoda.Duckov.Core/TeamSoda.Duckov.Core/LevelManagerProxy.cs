using System;
using Duckov.Scenes;
using UnityEngine;

// Token: 0x02000111 RID: 273
public class LevelManagerProxy : MonoBehaviour
{
	// Token: 0x06000948 RID: 2376 RVA: 0x00029047 File Offset: 0x00027247
	public void NotifyEvacuated()
	{
		LevelManager instance = LevelManager.Instance;
		if (instance == null)
		{
			return;
		}
		instance.NotifyEvacuated(new EvacuationInfo(MultiSceneCore.ActiveSubSceneID, base.transform.position));
	}
}
