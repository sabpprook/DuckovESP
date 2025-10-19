using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Duckov.UI;
using Duckov.UI.Animations;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Duckov
{
	// Token: 0x0200023D RID: 573
	public class StrongNotification : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
	{
		// Token: 0x1700031F RID: 799
		// (get) Token: 0x060011CC RID: 4556 RVA: 0x00044448 File Offset: 0x00042648
		// (set) Token: 0x060011CD RID: 4557 RVA: 0x0004444F File Offset: 0x0004264F
		public static StrongNotification Instance { get; private set; }

		// Token: 0x17000320 RID: 800
		// (get) Token: 0x060011CE RID: 4558 RVA: 0x00044457 File Offset: 0x00042657
		private bool showing
		{
			get
			{
				return this.showingTask.Status == UniTaskStatus.Pending;
			}
		}

		// Token: 0x17000321 RID: 801
		// (get) Token: 0x060011CF RID: 4559 RVA: 0x00044467 File Offset: 0x00042667
		public static bool Showing
		{
			get
			{
				return !(StrongNotification.Instance == null) && StrongNotification.Instance.showing;
			}
		}

		// Token: 0x060011D0 RID: 4560 RVA: 0x00044484 File Offset: 0x00042684
		private void Awake()
		{
			if (StrongNotification.Instance == null)
			{
				StrongNotification.Instance = this;
			}
			UIInputManager.OnConfirm += this.OnConfirm;
			UIInputManager.OnCancel += this.OnCancel;
			View.OnActiveViewChanged += this.View_OnActiveViewChanged;
		}

		// Token: 0x060011D1 RID: 4561 RVA: 0x000444D7 File Offset: 0x000426D7
		private void OnDestroy()
		{
			UIInputManager.OnConfirm -= this.OnConfirm;
			UIInputManager.OnCancel -= this.OnCancel;
			View.OnActiveViewChanged -= this.View_OnActiveViewChanged;
		}

		// Token: 0x060011D2 RID: 4562 RVA: 0x0004450C File Offset: 0x0004270C
		private void View_OnActiveViewChanged()
		{
			this.confirmed = true;
		}

		// Token: 0x060011D3 RID: 4563 RVA: 0x00044515 File Offset: 0x00042715
		private void OnCancel(UIInputEventData data)
		{
			this.confirmed = true;
		}

		// Token: 0x060011D4 RID: 4564 RVA: 0x0004451E File Offset: 0x0004271E
		private void OnConfirm(UIInputEventData data)
		{
			this.confirmed = true;
		}

		// Token: 0x060011D5 RID: 4565 RVA: 0x00044527 File Offset: 0x00042727
		private void Update()
		{
			if (!this.showing && StrongNotification.pending.Count > 0)
			{
				this.BeginShow();
			}
		}

		// Token: 0x060011D6 RID: 4566 RVA: 0x00044544 File Offset: 0x00042744
		private void BeginShow()
		{
			this.showingTask = this.ShowTask();
		}

		// Token: 0x060011D7 RID: 4567 RVA: 0x00044554 File Offset: 0x00042754
		private async UniTask ShowTask()
		{
			await this.mainFadeGroup.ShowAndReturnTask();
			await UniTask.WaitForSeconds(this.contentDelay, true, PlayerLoopTiming.Update, default(CancellationToken), false);
			while (StrongNotification.pending.Count > 0)
			{
				StrongNotificationContent strongNotificationContent = StrongNotification.pending[0];
				StrongNotification.pending.RemoveAt(0);
				await this.DisplayContent(strongNotificationContent);
			}
			await this.mainFadeGroup.HideAndReturnTask();
		}

		// Token: 0x060011D8 RID: 4568 RVA: 0x00044598 File Offset: 0x00042798
		private async UniTask DisplayContent(StrongNotificationContent cur)
		{
			if (cur != null)
			{
				this.textMain.text = cur.mainText;
				this.textSub.text = cur.subText;
				if (cur.image != null)
				{
					this.image.sprite = cur.image;
					this.image.gameObject.SetActive(true);
				}
				else
				{
					this.image.gameObject.SetActive(false);
				}
				await this.contentFadeGroup.ShowAndReturnTask();
				this.confirmed = false;
				while (!this.confirmed)
				{
					await UniTask.NextFrame();
				}
				await this.contentFadeGroup.HideAndReturnTask();
			}
		}

		// Token: 0x060011D9 RID: 4569 RVA: 0x000445E3 File Offset: 0x000427E3
		public void OnPointerClick(PointerEventData eventData)
		{
			this.confirmed = true;
		}

		// Token: 0x060011DA RID: 4570 RVA: 0x000445EC File Offset: 0x000427EC
		public static void Push(StrongNotificationContent content)
		{
			StrongNotification.pending.Add(content);
		}

		// Token: 0x060011DB RID: 4571 RVA: 0x000445F9 File Offset: 0x000427F9
		public static void Push(string mainText, string subText = "")
		{
			StrongNotification.pending.Add(new StrongNotificationContent(mainText, subText, null));
		}

		// Token: 0x04000DB6 RID: 3510
		[SerializeField]
		private FadeGroup mainFadeGroup;

		// Token: 0x04000DB7 RID: 3511
		[SerializeField]
		private FadeGroup contentFadeGroup;

		// Token: 0x04000DB8 RID: 3512
		[SerializeField]
		private TextMeshProUGUI textMain;

		// Token: 0x04000DB9 RID: 3513
		[SerializeField]
		private TextMeshProUGUI textSub;

		// Token: 0x04000DBA RID: 3514
		[SerializeField]
		private Image image;

		// Token: 0x04000DBB RID: 3515
		[SerializeField]
		private float contentDelay = 0.5f;

		// Token: 0x04000DBC RID: 3516
		private static List<StrongNotificationContent> pending = new List<StrongNotificationContent>();

		// Token: 0x04000DBE RID: 3518
		private UniTask showingTask;

		// Token: 0x04000DBF RID: 3519
		private bool confirmed;
	}
}
