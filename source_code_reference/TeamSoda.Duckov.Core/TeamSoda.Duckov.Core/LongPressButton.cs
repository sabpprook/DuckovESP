using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000157 RID: 343
public class LongPressButton : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler, IPointerExitHandler
{
	// Token: 0x17000211 RID: 529
	// (get) Token: 0x06000A80 RID: 2688 RVA: 0x0002DE57 File Offset: 0x0002C057
	private float TimeSincePressStarted
	{
		get
		{
			return Time.unscaledTime - this.timeWhenPressStarted;
		}
	}

	// Token: 0x17000212 RID: 530
	// (get) Token: 0x06000A81 RID: 2689 RVA: 0x0002DE65 File Offset: 0x0002C065
	private float Progress
	{
		get
		{
			if (!this.pressed)
			{
				return 0f;
			}
			return this.TimeSincePressStarted / this.pressTime;
		}
	}

	// Token: 0x06000A82 RID: 2690 RVA: 0x0002DE82 File Offset: 0x0002C082
	private void Update()
	{
		this.fill.fillAmount = this.Progress;
		if (this.pressed && this.Progress >= 1f)
		{
			UnityEvent unityEvent = this.onPressFullfilled;
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
			this.pressed = false;
		}
	}

	// Token: 0x06000A83 RID: 2691 RVA: 0x0002DEC2 File Offset: 0x0002C0C2
	public void OnPointerDown(PointerEventData eventData)
	{
		this.pressed = true;
		this.timeWhenPressStarted = Time.unscaledTime;
		UnityEvent unityEvent = this.onPressStarted;
		if (unityEvent == null)
		{
			return;
		}
		unityEvent.Invoke();
	}

	// Token: 0x06000A84 RID: 2692 RVA: 0x0002DEE6 File Offset: 0x0002C0E6
	public void OnPointerExit(PointerEventData eventData)
	{
		if (!this.pressed)
		{
			return;
		}
		this.pressed = false;
		UnityEvent unityEvent = this.onPressCanceled;
		if (unityEvent == null)
		{
			return;
		}
		unityEvent.Invoke();
	}

	// Token: 0x06000A85 RID: 2693 RVA: 0x0002DF08 File Offset: 0x0002C108
	public void OnPointerUp(PointerEventData eventData)
	{
		if (!this.pressed)
		{
			return;
		}
		this.pressed = false;
		UnityEvent unityEvent = this.onPressCanceled;
		if (unityEvent == null)
		{
			return;
		}
		unityEvent.Invoke();
	}

	// Token: 0x04000928 RID: 2344
	[SerializeField]
	private Image fill;

	// Token: 0x04000929 RID: 2345
	[SerializeField]
	private float pressTime = 1f;

	// Token: 0x0400092A RID: 2346
	public UnityEvent onPressStarted;

	// Token: 0x0400092B RID: 2347
	public UnityEvent onPressCanceled;

	// Token: 0x0400092C RID: 2348
	public UnityEvent onPressFullfilled;

	// Token: 0x0400092D RID: 2349
	private float timeWhenPressStarted;

	// Token: 0x0400092E RID: 2350
	private bool pressed;
}
