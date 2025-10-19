using System;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x02000154 RID: 340
public class UIPanelButton_OpenChildPanel : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
{
	// Token: 0x06000A77 RID: 2679 RVA: 0x0002DCCB File Offset: 0x0002BECB
	private void Awake()
	{
		if (this.master == null)
		{
			this.master = base.GetComponentInParent<UIPanel>();
		}
	}

	// Token: 0x06000A78 RID: 2680 RVA: 0x0002DCE7 File Offset: 0x0002BEE7
	public void OnPointerClick(PointerEventData eventData)
	{
		UIPanel uipanel = this.master;
		if (uipanel != null)
		{
			uipanel.OpenChild(this.target);
		}
		eventData.Use();
	}

	// Token: 0x0400091F RID: 2335
	[SerializeField]
	private UIPanel master;

	// Token: 0x04000920 RID: 2336
	[SerializeField]
	private UIPanel target;
}
