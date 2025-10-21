using System;
using Cysharp.Threading.Tasks;
using Duckov.Scenes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x0200015A RID: 346
public class ReselectButton : MonoBehaviour
{
	// Token: 0x06000AA0 RID: 2720 RVA: 0x0002E383 File Offset: 0x0002C583
	private void Awake()
	{
		this.button.onClick.AddListener(new UnityAction(this.OnButtonClicked));
	}

	// Token: 0x06000AA1 RID: 2721 RVA: 0x0002E3A1 File Offset: 0x0002C5A1
	private void OnEnable()
	{
		this.setActiveGroup.SetActive(LevelManager.Instance && LevelManager.Instance.IsBaseLevel);
	}

	// Token: 0x06000AA2 RID: 2722 RVA: 0x0002E3C7 File Offset: 0x0002C5C7
	private void OnDisable()
	{
	}

	// Token: 0x06000AA3 RID: 2723 RVA: 0x0002E3CC File Offset: 0x0002C5CC
	private void OnButtonClicked()
	{
		SceneLoader.Instance.LoadScene(this.prepareSceneID, null, false, false, true, false, default(MultiSceneLocation), true, false).Forget();
		if (PauseMenu.Instance && PauseMenu.Instance.Shown)
		{
			PauseMenu.Hide();
		}
	}

	// Token: 0x04000946 RID: 2374
	[SerializeField]
	private GameObject setActiveGroup;

	// Token: 0x04000947 RID: 2375
	[SerializeField]
	private Button button;

	// Token: 0x04000948 RID: 2376
	[SerializeField]
	[SceneID]
	private string prepareSceneID = "Prepare";
}
