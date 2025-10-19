using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

// Token: 0x02000169 RID: 361
public class OnPointerClick : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
{
	// Token: 0x06000AF0 RID: 2800 RVA: 0x0002ED0C File Offset: 0x0002CF0C
	void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
	{
		UnityEvent<PointerEventData> unityEvent = this.onPointerClick;
		if (unityEvent == null)
		{
			return;
		}
		unityEvent.Invoke(eventData);
	}

	// Token: 0x0400096B RID: 2411
	public UnityEvent<PointerEventData> onPointerClick;
}
