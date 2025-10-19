using System;
using Cysharp.Threading.Tasks;
using Duckov.Scenes;
using Duckov.UI;
using Duckov.Utilities;
using Eflatun.SceneReference;
using UnityEngine;

// Token: 0x02000112 RID: 274
public class SceneLoaderProxy : MonoBehaviour
{
	// Token: 0x0600094A RID: 2378 RVA: 0x00029075 File Offset: 0x00027275
	public void LoadScene()
	{
		if (SceneLoader.Instance == null)
		{
			Debug.LogWarning("没找到SceneLoader实例，已取消加载场景");
			return;
		}
		this.Task().Forget();
	}

	// Token: 0x0600094B RID: 2379 RVA: 0x0002909C File Offset: 0x0002729C
	private async UniTask Task()
	{
		if ("Base" == this.sceneID)
		{
			this.saveToFile = true;
		}
		if (this.showClosure)
		{
			if (this.notifyEvacuation)
			{
				LevelManager instance = LevelManager.Instance;
				if (instance != null)
				{
					instance.NotifyEvacuated(new EvacuationInfo(MultiSceneCore.ActiveSubSceneID, base.transform.position));
				}
			}
			await ClosureView.ShowAndReturnTask(1f);
		}
		if (this.notifyEvacuation)
		{
			this.overrideCurtainScene = GameplayDataSettings.SceneManagement.EvacuateScreenScene;
		}
		if (this.useLocation)
		{
			SceneLoader instance2 = SceneLoader.Instance;
			string text = this.sceneID;
			MultiSceneLocation multiSceneLocation = this.location;
			bool flag = this.notifyEvacuation;
			SceneReference sceneReference = this.overrideCurtainScene;
			bool flag2 = false;
			bool flag3 = this.saveToFile;
			bool flag4 = this.hideTips;
			instance2.LoadScene(text, multiSceneLocation, sceneReference, flag2, flag, this.circleFade, flag3, flag4).Forget();
		}
		else
		{
			SceneLoader instance3 = SceneLoader.Instance;
			string text2 = this.sceneID;
			bool flag4 = this.notifyEvacuation;
			SceneReference sceneReference2 = this.overrideCurtainScene;
			bool flag5 = false;
			bool flag3 = this.saveToFile;
			bool flag = this.hideTips;
			instance3.LoadScene(text2, sceneReference2, flag5, flag4, this.circleFade, false, default(MultiSceneLocation), flag3, flag).Forget();
		}
	}

	// Token: 0x0600094C RID: 2380 RVA: 0x000290DF File Offset: 0x000272DF
	public void LoadMainMenu()
	{
		SceneLoader.LoadMainMenu(this.circleFade);
	}

	// Token: 0x04000844 RID: 2116
	[SceneID]
	[SerializeField]
	private string sceneID;

	// Token: 0x04000845 RID: 2117
	[SerializeField]
	private bool useLocation;

	// Token: 0x04000846 RID: 2118
	[SerializeField]
	private MultiSceneLocation location;

	// Token: 0x04000847 RID: 2119
	[SerializeField]
	private bool showClosure = true;

	// Token: 0x04000848 RID: 2120
	[SerializeField]
	private bool notifyEvacuation = true;

	// Token: 0x04000849 RID: 2121
	[SerializeField]
	private SceneReference overrideCurtainScene;

	// Token: 0x0400084A RID: 2122
	[SerializeField]
	private bool hideTips;

	// Token: 0x0400084B RID: 2123
	[SerializeField]
	private bool circleFade = true;

	// Token: 0x0400084C RID: 2124
	private bool saveToFile;
}
