using System;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x02000125 RID: 293
public class LoadUnitySceneOnStart : MonoBehaviour
{
	// Token: 0x06000994 RID: 2452 RVA: 0x00029897 File Offset: 0x00027A97
	private void Start()
	{
		SceneManager.LoadScene(this.sceneIndex);
	}

	// Token: 0x0400086A RID: 2154
	public int sceneIndex;
}
