using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000205 RID: 517
public class ScrollViewEventReceiver : MonoBehaviour, IScrollHandler, IEventSystemHandler
{
	// Token: 0x06000F1A RID: 3866 RVA: 0x0003B9C4 File Offset: 0x00039BC4
	private void Awake()
	{
		if (this.scrollRect == null)
		{
			this.scrollRect = base.GetComponent<ScrollRect>();
		}
	}

	// Token: 0x06000F1B RID: 3867 RVA: 0x0003B9E0 File Offset: 0x00039BE0
	public void OnScroll(PointerEventData eventData)
	{
	}

	// Token: 0x04000C59 RID: 3161
	[SerializeField]
	private ScrollRect scrollRect;
}
