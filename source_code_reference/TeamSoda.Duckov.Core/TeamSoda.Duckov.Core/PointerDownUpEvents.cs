using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

// Token: 0x02000203 RID: 515
public class PointerDownUpEvents : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler
{
	// Token: 0x06000F11 RID: 3857 RVA: 0x0003B8BF File Offset: 0x00039ABF
	public void OnPointerDown(PointerEventData eventData)
	{
		UnityEvent<PointerEventData> unityEvent = this.onPointerDown;
		if (unityEvent == null)
		{
			return;
		}
		unityEvent.Invoke(eventData);
	}

	// Token: 0x06000F12 RID: 3858 RVA: 0x0003B8D2 File Offset: 0x00039AD2
	public void OnPointerUp(PointerEventData eventData)
	{
		UnityEvent<PointerEventData> unityEvent = this.onPointerUp;
		if (unityEvent == null)
		{
			return;
		}
		unityEvent.Invoke(eventData);
	}

	// Token: 0x04000C55 RID: 3157
	public UnityEvent<PointerEventData> onPointerDown;

	// Token: 0x04000C56 RID: 3158
	public UnityEvent<PointerEventData> onPointerUp;
}
