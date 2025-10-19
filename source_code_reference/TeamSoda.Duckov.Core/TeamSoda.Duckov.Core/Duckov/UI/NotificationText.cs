using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Duckov.UI.Animations;
using TMPro;
using UnityEngine;

namespace Duckov.UI
{
	// Token: 0x02000376 RID: 886
	public class NotificationText : MonoBehaviour
	{
		// Token: 0x06001E9F RID: 7839 RVA: 0x0006B921 File Offset: 0x00069B21
		public static void Push(string text)
		{
			if (NotificationText.pendingTexts.Count > 0 && NotificationText.pendingTexts.Peek() == text)
			{
				return;
			}
			NotificationText.pendingTexts.Enqueue(text);
		}

		// Token: 0x06001EA0 RID: 7840 RVA: 0x0006B94E File Offset: 0x00069B4E
		private static string Pop()
		{
			return NotificationText.pendingTexts.Dequeue();
		}

		// Token: 0x170005EB RID: 1515
		// (get) Token: 0x06001EA1 RID: 7841 RVA: 0x0006B95A File Offset: 0x00069B5A
		private int PendingCount
		{
			get
			{
				return NotificationText.pendingTexts.Count;
			}
		}

		// Token: 0x06001EA2 RID: 7842 RVA: 0x0006B966 File Offset: 0x00069B66
		private void Update()
		{
			if (!this.showing && this.PendingCount > 0)
			{
				this.ShowNext().Forget();
			}
		}

		// Token: 0x06001EA3 RID: 7843 RVA: 0x0006B984 File Offset: 0x00069B84
		private async UniTask ShowNext()
		{
			if (this.PendingCount != 0)
			{
				this.showing = true;
				string text = NotificationText.Pop();
				this.text.text = text;
				this.fadeGroup.Show();
				await UniTask.WaitForSeconds((this.PendingCount > 0) ? this.durationIfPending : this.duration, true, PlayerLoopTiming.Update, default(CancellationToken), false);
				await this.fadeGroup.HideAndReturnTask();
				this.showing = false;
			}
		}

		// Token: 0x040014F2 RID: 5362
		[SerializeField]
		private FadeGroup fadeGroup;

		// Token: 0x040014F3 RID: 5363
		[SerializeField]
		private TextMeshProUGUI text;

		// Token: 0x040014F4 RID: 5364
		[SerializeField]
		private float duration = 1.2f;

		// Token: 0x040014F5 RID: 5365
		[SerializeField]
		private float durationIfPending = 0.65f;

		// Token: 0x040014F6 RID: 5366
		private static Queue<string> pendingTexts = new Queue<string>();

		// Token: 0x040014F7 RID: 5367
		private bool showing;
	}
}
