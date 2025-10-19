using System;
using Cysharp.Threading.Tasks;
using Saves;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x0200016B RID: 363
public class Button_LoadMainMenu : MonoBehaviour
{
	// Token: 0x06000AF8 RID: 2808 RVA: 0x0002ED8B File Offset: 0x0002CF8B
	private void Awake()
	{
		this.button.onClick.AddListener(new UnityAction(this.BeginQuitting));
		this.dialogue.SkipHide();
	}

	// Token: 0x06000AF9 RID: 2809 RVA: 0x0002EDB4 File Offset: 0x0002CFB4
	private void BeginQuitting()
	{
		if (this.task.Status == UniTaskStatus.Pending)
		{
			return;
		}
		Debug.Log("Quitting");
		this.task = this.QuitTask();
	}

	// Token: 0x06000AFA RID: 2810 RVA: 0x0002EDDC File Offset: 0x0002CFDC
	private async UniTask QuitTask()
	{
		if (LevelManager.Instance != null && LevelManager.Instance.IsBaseLevel)
		{
			LevelManager.Instance.SaveMainCharacter();
			SavesSystem.CollectSaveData();
			SavesSystem.SaveFile(true);
		}
		else
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
		SceneLoader.LoadMainMenu(true);
		PauseMenu.Hide();
	}

	// Token: 0x0400096C RID: 2412
	[SerializeField]
	private Button button;

	// Token: 0x0400096D RID: 2413
	[SerializeField]
	private ConfirmDialogue dialogue;

	// Token: 0x0400096E RID: 2414
	private UniTask task;
}
