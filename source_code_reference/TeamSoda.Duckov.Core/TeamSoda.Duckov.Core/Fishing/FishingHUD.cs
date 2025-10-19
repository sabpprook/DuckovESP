using System;
using Cysharp.Threading.Tasks;
using Duckov.UI.Animations;
using ItemStatsSystem;
using UnityEngine;
using UnityEngine.UI;

namespace Fishing
{
	// Token: 0x02000213 RID: 531
	public class FishingHUD : MonoBehaviour
	{
		// Token: 0x06000FC9 RID: 4041 RVA: 0x0003DF11 File Offset: 0x0003C111
		private void Awake()
		{
			Action_Fishing.OnPlayerStartCatching += this.OnStartCatching;
			Action_Fishing.OnPlayerStopCatching += this.OnStopCatching;
			Action_Fishing.OnPlayerStopFishing += this.OnStopFishing;
		}

		// Token: 0x06000FCA RID: 4042 RVA: 0x0003DF46 File Offset: 0x0003C146
		private void OnDestroy()
		{
			Action_Fishing.OnPlayerStartCatching -= this.OnStartCatching;
			Action_Fishing.OnPlayerStopCatching -= this.OnStopCatching;
			Action_Fishing.OnPlayerStopFishing -= this.OnStopFishing;
		}

		// Token: 0x06000FCB RID: 4043 RVA: 0x0003DF7B File Offset: 0x0003C17B
		private void OnStopFishing(Action_Fishing fishing)
		{
			this.fadeGroup.Hide();
		}

		// Token: 0x06000FCC RID: 4044 RVA: 0x0003DF88 File Offset: 0x0003C188
		private void OnStopCatching(Action_Fishing fishing, Item item, Action<bool> action)
		{
			this.StopCatchingTask(item, action).Forget();
		}

		// Token: 0x06000FCD RID: 4045 RVA: 0x0003DF97 File Offset: 0x0003C197
		private void OnStartCatching(Action_Fishing fishing, float totalTime, Func<float> currentTimeGetter)
		{
			this.CatchingTask(fishing, totalTime, currentTimeGetter).Forget();
		}

		// Token: 0x06000FCE RID: 4046 RVA: 0x0003DFA8 File Offset: 0x0003C1A8
		private async UniTask CatchingTask(Action_Fishing fishing, float totalTime, Func<float> currentTimeGetter)
		{
			this.succeedIndicator.SkipHide();
			this.failIndicator.SkipHide();
			this.fadeGroup.Show();
			while (fishing.Running && fishing.FishingState == Action_Fishing.FishingStates.catching)
			{
				this.UpdateBar(totalTime, currentTimeGetter());
				await UniTask.Yield();
			}
			if (!fishing.Running)
			{
				this.fadeGroup.Hide();
			}
		}

		// Token: 0x06000FCF RID: 4047 RVA: 0x0003E004 File Offset: 0x0003C204
		private void UpdateBar(float totalTime, float currentTime)
		{
			if (totalTime <= 0f)
			{
				return;
			}
			float num = 1f - currentTime / totalTime;
			this.countDownFill.fillAmount = num;
		}

		// Token: 0x06000FD0 RID: 4048 RVA: 0x0003E030 File Offset: 0x0003C230
		private async UniTask StopCatchingTask(Item item, Action<bool> confirmCallback)
		{
			if (item == null)
			{
				this.failIndicator.Show();
			}
			else
			{
				this.succeedIndicator.Show();
			}
			this.fadeGroup.Hide();
		}

		// Token: 0x04000CB3 RID: 3251
		[SerializeField]
		private FadeGroup fadeGroup;

		// Token: 0x04000CB4 RID: 3252
		[SerializeField]
		private Image countDownFill;

		// Token: 0x04000CB5 RID: 3253
		[SerializeField]
		private FadeGroup succeedIndicator;

		// Token: 0x04000CB6 RID: 3254
		[SerializeField]
		private FadeGroup failIndicator;
	}
}
