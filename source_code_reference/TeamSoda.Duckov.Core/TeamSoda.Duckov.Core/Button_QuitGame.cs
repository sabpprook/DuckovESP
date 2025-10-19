using System;
using Cysharp.Threading.Tasks;
using Saves;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x0200016C RID: 364
public class Button_QuitGame : MonoBehaviour
{
	// Token: 0x06000AFC RID: 2812 RVA: 0x0002EE27 File Offset: 0x0002D027
	private void Awake()
	{
		this.button.onClick.AddListener(new UnityAction(this.BeginQuitting));
		if (this.dialogue)
		{
			this.dialogue.SkipHide();
		}
	}

	// Token: 0x06000AFD RID: 2813 RVA: 0x0002EE5D File Offset: 0x0002D05D
	private void BeginQuitting()
	{
		if (this.task.Status == UniTaskStatus.Pending)
		{
			return;
		}
		Debug.Log("Quitting");
		this.task = this.QuitTask();
	}

	// Token: 0x06000AFE RID: 2814 RVA: 0x0002EE84 File Offset: 0x0002D084
	private async UniTask QuitTask()
	{
		if (LevelManager.Instance != null && LevelManager.Instance.IsBaseLevel)
		{
			LevelManager.Instance.SaveMainCharacter();
			SavesSystem.CollectSaveData();
			SavesSystem.SaveFile(true);
		}
		else if (this.dialogue)
		{
			UniTask<bool>.Awaiter awaiter = this.dialogue.Execute().GetAwaiter();
			if (!awaiter.IsCompleted)
			{
				await awaiter;
				UniTask<bool>.Awaiter awaiter2;
				awaiter = awaiter2;
				awaiter2 = default(UniTask<bool>.Awaiter);
			}
			if (!awaiter.GetResult())
			{
				return;
			}
		}
		while (SavesSystem.IsSaving)
		{
			await UniTask.Yield();
		}
		if (Application.isEditor)
		{
			Debug.Log("即将调用Application.Quit()。但因为是Editor，不会真的退出。");
		}
		Application.Quit();
	}

	// Token: 0x0400096F RID: 2415
	[SerializeField]
	private Button button;

	// Token: 0x04000970 RID: 2416
	[SerializeField]
	private ConfirmDialogue dialogue;

	// Token: 0x04000971 RID: 2417
	private UniTask task;
}
