using System;
using Duckov.UI.Animations;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x02000165 RID: 357
public class FadeGroupButton : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
{
	// Token: 0x06000ADA RID: 2778 RVA: 0x0002EA7B File Offset: 0x0002CC7B
	private void OnEnable()
	{
		UIInputManager.OnCancel += this.OnCancel;
	}

	// Token: 0x06000ADB RID: 2779 RVA: 0x0002EA8E File Offset: 0x0002CC8E
	private void OnDisable()
	{
		UIInputManager.OnCancel -= this.OnCancel;
	}

	// Token: 0x06000ADC RID: 2780 RVA: 0x0002EAA1 File Offset: 0x0002CCA1
	private void OnCancel(UIInputEventData data)
	{
		if (!base.isActiveAndEnabled)
		{
			return;
		}
		if (data.Used)
		{
			return;
		}
		if (!this.triggerWhenCancel)
		{
			return;
		}
		this.Execute();
		data.Use();
	}

	// Token: 0x06000ADD RID: 2781 RVA: 0x0002EACA File Offset: 0x0002CCCA
	public void OnPointerClick(PointerEventData eventData)
	{
		this.Execute();
	}

	// Token: 0x06000ADE RID: 2782 RVA: 0x0002EAD2 File Offset: 0x0002CCD2
	private void Execute()
	{
		if (this.closeOnClick)
		{
			this.closeOnClick.Hide();
		}
		if (this.openOnClick)
		{
			this.openOnClick.Show();
		}
	}

	// Token: 0x0400095C RID: 2396
	[SerializeField]
	private FadeGroup closeOnClick;

	// Token: 0x0400095D RID: 2397
	[SerializeField]
	private FadeGroup openOnClick;

	// Token: 0x0400095E RID: 2398
	[SerializeField]
	private bool triggerWhenCancel;
}
