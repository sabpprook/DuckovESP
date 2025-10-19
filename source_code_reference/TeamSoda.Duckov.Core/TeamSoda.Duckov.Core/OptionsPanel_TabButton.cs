using System;
using Duckov.Options.UI;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x020001DD RID: 477
public class OptionsPanel_TabButton : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
{
	// Token: 0x06000E2E RID: 3630 RVA: 0x00039407 File Offset: 0x00037607
	public void OnPointerClick(PointerEventData eventData)
	{
		Action<OptionsPanel_TabButton, PointerEventData> action = this.onClicked;
		if (action == null)
		{
			return;
		}
		action(this, eventData);
	}

	// Token: 0x06000E2F RID: 3631 RVA: 0x0003941C File Offset: 0x0003761C
	internal void NotifySelectionChanged(OptionsPanel optionsPanel, OptionsPanel_TabButton selection)
	{
		bool flag = selection == this;
		this.tab.SetActive(flag);
		this.selectedIndicator.SetActive(flag);
	}

	// Token: 0x04000BBE RID: 3006
	[SerializeField]
	private GameObject selectedIndicator;

	// Token: 0x04000BBF RID: 3007
	[SerializeField]
	private GameObject tab;

	// Token: 0x04000BC0 RID: 3008
	public Action<OptionsPanel_TabButton, PointerEventData> onClicked;
}
