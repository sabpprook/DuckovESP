using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Duckov.UI.Animations;
using Saves;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000164 RID: 356
public class DeleteSaveDataButton : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler
{
	// Token: 0x17000217 RID: 535
	// (get) Token: 0x06000ACF RID: 2767 RVA: 0x0002E946 File Offset: 0x0002CB46
	private float TimeSinceStartedHolding
	{
		get
		{
			return Time.unscaledTime - this.timeWhenStartedHolding;
		}
	}

	// Token: 0x17000218 RID: 536
	// (get) Token: 0x06000AD0 RID: 2768 RVA: 0x0002E954 File Offset: 0x0002CB54
	private float T
	{
		get
		{
			if (this.totalTime <= 0f)
			{
				return 1f;
			}
			return Mathf.Clamp01(this.TimeSinceStartedHolding / this.totalTime);
		}
	}

	// Token: 0x06000AD1 RID: 2769 RVA: 0x0002E97B File Offset: 0x0002CB7B
	public void OnPointerDown(PointerEventData eventData)
	{
		this.holding = true;
		this.timeWhenStartedHolding = Time.unscaledTime;
	}

	// Token: 0x06000AD2 RID: 2770 RVA: 0x0002E98F File Offset: 0x0002CB8F
	public void OnPointerUp(PointerEventData eventData)
	{
		this.holding = false;
		this.timeWhenStartedHolding = float.MaxValue;
		this.RefreshProgressBar();
	}

	// Token: 0x06000AD3 RID: 2771 RVA: 0x0002E9A9 File Offset: 0x0002CBA9
	private void Start()
	{
		this.barFill.fillAmount = 0f;
	}

	// Token: 0x06000AD4 RID: 2772 RVA: 0x0002E9BB File Offset: 0x0002CBBB
	private void Update()
	{
		if (this.holding)
		{
			this.RefreshProgressBar();
			if (this.T >= 1f)
			{
				this.Execute();
			}
		}
	}

	// Token: 0x06000AD5 RID: 2773 RVA: 0x0002E9DE File Offset: 0x0002CBDE
	private void Execute()
	{
		this.holding = false;
		this.DeleteCurrentSaveData();
		this.RefreshProgressBar();
		this.NotifySaveDeleted().Forget();
	}

	// Token: 0x06000AD6 RID: 2774 RVA: 0x0002EA00 File Offset: 0x0002CC00
	private async UniTask NotifySaveDeleted()
	{
		await this.saveDeletedNotifierFadeGroup.ShowAndReturnTask();
		await UniTask.WaitForSeconds(2, false, PlayerLoopTiming.Update, default(CancellationToken), false);
		await this.saveDeletedNotifierFadeGroup.HideAndReturnTask();
	}

	// Token: 0x06000AD7 RID: 2775 RVA: 0x0002EA43 File Offset: 0x0002CC43
	private void DeleteCurrentSaveData()
	{
		SavesSystem.DeleteCurrentSave();
	}

	// Token: 0x06000AD8 RID: 2776 RVA: 0x0002EA4A File Offset: 0x0002CC4A
	private void RefreshProgressBar()
	{
		this.barFill.fillAmount = this.T;
	}

	// Token: 0x04000957 RID: 2391
	[SerializeField]
	private float totalTime = 3f;

	// Token: 0x04000958 RID: 2392
	[SerializeField]
	private Image barFill;

	// Token: 0x04000959 RID: 2393
	[SerializeField]
	private FadeGroup saveDeletedNotifierFadeGroup;

	// Token: 0x0400095A RID: 2394
	private float timeWhenStartedHolding = float.MaxValue;

	// Token: 0x0400095B RID: 2395
	private bool holding;
}
