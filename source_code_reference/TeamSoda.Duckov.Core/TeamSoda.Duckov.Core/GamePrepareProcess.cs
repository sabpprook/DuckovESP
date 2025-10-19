using System;
using Cysharp.Threading.Tasks;
using Duckov.Rules.UI;
using Duckov.Scenes;
using Eflatun.SceneReference;
using UnityEngine;

// Token: 0x020001E3 RID: 483
public class GamePrepareProcess : MonoBehaviour
{
	// Token: 0x06000E46 RID: 3654 RVA: 0x00039648 File Offset: 0x00037848
	private async UniTask Execute()
	{
		this.difficultySelection.SkipHide();
		await this.difficultySelection.Execute();
		if (this.goToBaseSceneIfVisted && !string.IsNullOrEmpty(this.baseScene) && MultiSceneCore.GetVisited(this.baseScene))
		{
			SceneLoader.Instance.LoadScene(this.baseScene, this.overrideCurtainScene, false, false, true, false, default(MultiSceneLocation), true, false).Forget();
		}
		else if (this.goToBaseSceneIfVisted && !string.IsNullOrEmpty(this.guideScene) && MultiSceneCore.GetVisited(this.guideScene))
		{
			SceneLoader.Instance.LoadScene(this.guideScene, this.overrideCurtainScene, false, false, true, false, default(MultiSceneLocation), true, false).Forget();
		}
		else
		{
			SceneLoader.Instance.LoadScene(this.introScene, this.overrideCurtainScene, false, false, true, false, default(MultiSceneLocation), true, false).Forget();
		}
	}

	// Token: 0x06000E47 RID: 3655 RVA: 0x0003968B File Offset: 0x0003788B
	private void Start()
	{
		this.Execute().Forget();
	}

	// Token: 0x04000BC6 RID: 3014
	[SerializeField]
	private DifficultySelection difficultySelection;

	// Token: 0x04000BC7 RID: 3015
	[SerializeField]
	[SceneID]
	private string introScene;

	// Token: 0x04000BC8 RID: 3016
	[SerializeField]
	[SceneID]
	private string guideScene;

	// Token: 0x04000BC9 RID: 3017
	public bool goToBaseSceneIfVisted;

	// Token: 0x04000BCA RID: 3018
	[SerializeField]
	[SceneID]
	private string baseScene;

	// Token: 0x04000BCB RID: 3019
	public SceneReference overrideCurtainScene;
}
