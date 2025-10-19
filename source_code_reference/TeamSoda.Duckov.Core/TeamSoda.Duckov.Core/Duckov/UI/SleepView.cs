using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Duckov.UI.Animations;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Duckov.UI
{
	// Token: 0x02000383 RID: 899
	public class SleepView : View
	{
		// Token: 0x17000600 RID: 1536
		// (get) Token: 0x06001F2B RID: 7979 RVA: 0x0006D39D File Offset: 0x0006B59D
		public static SleepView Instance
		{
			get
			{
				return View.GetViewInstance<SleepView>();
			}
		}

		// Token: 0x17000601 RID: 1537
		// (get) Token: 0x06001F2C RID: 7980 RVA: 0x0006D3A4 File Offset: 0x0006B5A4
		private TimeSpan SleepTimeSpan
		{
			get
			{
				return TimeSpan.FromMinutes((double)this.sleepForMinuts);
			}
		}

		// Token: 0x17000602 RID: 1538
		// (get) Token: 0x06001F2D RID: 7981 RVA: 0x0006D3B2 File Offset: 0x0006B5B2
		private TimeSpan WillWakeUpAt
		{
			get
			{
				return GameClock.TimeOfDay + this.SleepTimeSpan;
			}
		}

		// Token: 0x17000603 RID: 1539
		// (get) Token: 0x06001F2E RID: 7982 RVA: 0x0006D3C4 File Offset: 0x0006B5C4
		private bool WillWakeUpNextDay
		{
			get
			{
				return this.WillWakeUpAt.Days > 0;
			}
		}

		// Token: 0x06001F2F RID: 7983 RVA: 0x0006D3E2 File Offset: 0x0006B5E2
		protected override void OnOpen()
		{
			base.OnOpen();
			this.fadeGroup.Show();
		}

		// Token: 0x06001F30 RID: 7984 RVA: 0x0006D3F5 File Offset: 0x0006B5F5
		protected override void OnClose()
		{
			base.OnClose();
			this.fadeGroup.Hide();
		}

		// Token: 0x06001F31 RID: 7985 RVA: 0x0006D408 File Offset: 0x0006B608
		protected override void Awake()
		{
			base.Awake();
			this.slider.onValueChanged.AddListener(new UnityAction<float>(this.OnSliderValueChanged));
			this.confirmButton.onClick.AddListener(new UnityAction(this.OnConfirmButtonClicked));
		}

		// Token: 0x06001F32 RID: 7986 RVA: 0x0006D448 File Offset: 0x0006B648
		private void OnConfirmButtonClicked()
		{
			this.Sleep((float)this.sleepForMinuts).Forget();
		}

		// Token: 0x06001F33 RID: 7987 RVA: 0x0006D45C File Offset: 0x0006B65C
		private async UniTask Sleep(float minuts)
		{
			if (!this.sleeping)
			{
				this.sleeping = true;
				float seconds = minuts * 60f;
				await BlackScreen.ShowAndReturnTask(null, 0f, 0.5f);
				GameClock.Step(seconds);
				await UniTask.WaitForSeconds(0.5f, true, PlayerLoopTiming.Update, default(CancellationToken), false);
				Action onAfterSleep = SleepView.OnAfterSleep;
				if (onAfterSleep != null)
				{
					onAfterSleep();
				}
				if (View.ActiveView == this)
				{
					base.Close();
				}
				await BlackScreen.HideAndReturnTask(null, 0f, 0.5f);
				this.sleeping = false;
			}
		}

		// Token: 0x06001F34 RID: 7988 RVA: 0x0006D4A7 File Offset: 0x0006B6A7
		private void OnGameClockStep()
		{
			this.Refresh();
		}

		// Token: 0x06001F35 RID: 7989 RVA: 0x0006D4AF File Offset: 0x0006B6AF
		private void OnEnable()
		{
			this.InitializeUI();
			GameClock.OnGameClockStep += this.OnGameClockStep;
		}

		// Token: 0x06001F36 RID: 7990 RVA: 0x0006D4C8 File Offset: 0x0006B6C8
		private void OnDisable()
		{
			GameClock.OnGameClockStep -= this.OnGameClockStep;
		}

		// Token: 0x06001F37 RID: 7991 RVA: 0x0006D4DB File Offset: 0x0006B6DB
		private void OnSliderValueChanged(float newValue)
		{
			this.sleepForMinuts = Mathf.RoundToInt(newValue);
			this.Refresh();
		}

		// Token: 0x06001F38 RID: 7992 RVA: 0x0006D4EF File Offset: 0x0006B6EF
		private void InitializeUI()
		{
			this.slider.SetValueWithoutNotify((float)this.sleepForMinuts);
		}

		// Token: 0x06001F39 RID: 7993 RVA: 0x0006D503 File Offset: 0x0006B703
		private void Update()
		{
			this.Refresh();
		}

		// Token: 0x06001F3A RID: 7994 RVA: 0x0006D50C File Offset: 0x0006B70C
		private void Refresh()
		{
			TimeSpan willWakeUpAt = this.WillWakeUpAt;
			this.willWakeUpAtText.text = string.Format("{0:00}:{1:00}", willWakeUpAt.Hours, willWakeUpAt.Minutes);
			TimeSpan sleepTimeSpan = this.SleepTimeSpan;
			this.sleepTimeSpanText.text = string.Format("{0:00} h {1:00} min", (int)sleepTimeSpan.TotalHours, sleepTimeSpan.Minutes);
			this.nextDayIndicator.gameObject.SetActive(willWakeUpAt.Days > 0);
		}

		// Token: 0x06001F3B RID: 7995 RVA: 0x0006D59C File Offset: 0x0006B79C
		public static void Show()
		{
			if (SleepView.Instance == null)
			{
				return;
			}
			SleepView.Instance.Open(null);
		}

		// Token: 0x0400154F RID: 5455
		[SerializeField]
		private FadeGroup fadeGroup;

		// Token: 0x04001550 RID: 5456
		[SerializeField]
		private Slider slider;

		// Token: 0x04001551 RID: 5457
		[SerializeField]
		private TextMeshProUGUI willWakeUpAtText;

		// Token: 0x04001552 RID: 5458
		[SerializeField]
		private TextMeshProUGUI sleepTimeSpanText;

		// Token: 0x04001553 RID: 5459
		[SerializeField]
		private GameObject nextDayIndicator;

		// Token: 0x04001554 RID: 5460
		[SerializeField]
		private Button confirmButton;

		// Token: 0x04001555 RID: 5461
		private int sleepForMinuts;

		// Token: 0x04001556 RID: 5462
		public static Action OnAfterSleep;

		// Token: 0x04001557 RID: 5463
		private bool sleeping;
	}
}
