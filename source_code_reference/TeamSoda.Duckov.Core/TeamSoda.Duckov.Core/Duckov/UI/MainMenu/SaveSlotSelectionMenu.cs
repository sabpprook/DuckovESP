using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Duckov.UI.Animations;
using Saves;
using UnityEngine;

namespace Duckov.UI.MainMenu
{
	// Token: 0x020003EA RID: 1002
	public class SaveSlotSelectionMenu : MonoBehaviour
	{
		// Token: 0x0600242C RID: 9260 RVA: 0x0007DD61 File Offset: 0x0007BF61
		private void OnEnable()
		{
			UIInputManager.OnCancel += this.OnCancel;
		}

		// Token: 0x0600242D RID: 9261 RVA: 0x0007DD74 File Offset: 0x0007BF74
		private void OnDisable()
		{
			UIInputManager.OnCancel -= this.OnCancel;
		}

		// Token: 0x0600242E RID: 9262 RVA: 0x0007DD87 File Offset: 0x0007BF87
		private void OnCancel(UIInputEventData data)
		{
			data.Use();
			this.Finish();
		}

		// Token: 0x0600242F RID: 9263 RVA: 0x0007DD98 File Offset: 0x0007BF98
		internal async UniTask Execute()
		{
			this.finished = false;
			this.oldSaveIndicator.SetActive(SavesSystem.IsOldSave(SavesSystem.CurrentSlot));
			await UniTask.WaitForSeconds(0.25f, true, PlayerLoopTiming.Update, default(CancellationToken), false);
			this.fadeGroup.Show();
			while (!this.finished)
			{
				await UniTask.NextFrame();
			}
			this.oldSaveIndicator.SetActive(SavesSystem.IsOldSave(SavesSystem.CurrentSlot));
			await UniTask.WaitForSeconds(0.05f, true, PlayerLoopTiming.Update, default(CancellationToken), false);
			this.fadeGroup.Hide();
			await UniTask.WaitForSeconds(0.25f, true, PlayerLoopTiming.Update, default(CancellationToken), false);
		}

		// Token: 0x06002430 RID: 9264 RVA: 0x0007DDDB File Offset: 0x0007BFDB
		public void Finish()
		{
			this.finished = true;
		}

		// Token: 0x040018A1 RID: 6305
		[SerializeField]
		private FadeGroup fadeGroup;

		// Token: 0x040018A2 RID: 6306
		[SerializeField]
		private GameObject oldSaveIndicator;

		// Token: 0x040018A3 RID: 6307
		internal bool finished;
	}
}
