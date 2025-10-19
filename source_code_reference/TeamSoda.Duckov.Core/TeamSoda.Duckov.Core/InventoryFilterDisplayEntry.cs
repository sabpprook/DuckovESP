using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020001EE RID: 494
public class InventoryFilterDisplayEntry : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
{
	// Token: 0x1700029E RID: 670
	// (get) Token: 0x06000E7B RID: 3707 RVA: 0x0003A2D3 File Offset: 0x000384D3
	// (set) Token: 0x06000E7C RID: 3708 RVA: 0x0003A2DB File Offset: 0x000384DB
	public InventoryFilterProvider.FilterEntry Filter { get; private set; }

	// Token: 0x06000E7D RID: 3709 RVA: 0x0003A2E4 File Offset: 0x000384E4
	public void OnPointerClick(PointerEventData eventData)
	{
		Action<InventoryFilterDisplayEntry, PointerEventData> action = this.onPointerClick;
		if (action == null)
		{
			return;
		}
		action(this, eventData);
	}

	// Token: 0x06000E7E RID: 3710 RVA: 0x0003A2F8 File Offset: 0x000384F8
	internal void Setup(Action<InventoryFilterDisplayEntry, PointerEventData> onPointerClick, InventoryFilterProvider.FilterEntry filter)
	{
		this.onPointerClick = onPointerClick;
		this.Filter = filter;
		if (this.icon)
		{
			this.icon.sprite = filter.icon;
		}
		if (this.nameDisplay)
		{
			this.nameDisplay.text = filter.DisplayName;
		}
	}

	// Token: 0x06000E7F RID: 3711 RVA: 0x0003A350 File Offset: 0x00038550
	internal void NotifySelectionChanged(bool isThisSelected)
	{
		this.selectedIndicator.SetActive(isThisSelected);
	}

	// Token: 0x04000C04 RID: 3076
	[SerializeField]
	private Image icon;

	// Token: 0x04000C05 RID: 3077
	[SerializeField]
	private TextMeshProUGUI nameDisplay;

	// Token: 0x04000C06 RID: 3078
	[SerializeField]
	private GameObject selectedIndicator;

	// Token: 0x04000C08 RID: 3080
	private Action<InventoryFilterDisplayEntry, PointerEventData> onPointerClick;
}
