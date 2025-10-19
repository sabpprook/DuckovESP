using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

// Token: 0x0200015C RID: 348
public class DragHandler : MonoBehaviour, IDragHandler, IEventSystemHandler
{
	// Token: 0x06000AAB RID: 2731 RVA: 0x0002E492 File Offset: 0x0002C692
	public void OnDrag(PointerEventData eventData)
	{
		UnityEvent<PointerEventData> unityEvent = this.onDrag;
		if (unityEvent == null)
		{
			return;
		}
		unityEvent.Invoke(eventData);
	}

	// Token: 0x0400094A RID: 2378
	public UnityEvent<PointerEventData> onDrag;
}
