using System;
using UnityEngine;
using UnityEngine.UIElements;

// Token: 0x02000206 RID: 518
public class UIToolkitTest : MonoBehaviour
{
	// Token: 0x06000F1D RID: 3869 RVA: 0x0003B9EC File Offset: 0x00039BEC
	private void Awake()
	{
		VisualElement visualElement = this.doc.rootVisualElement.Q("Button", null);
		CallbackEventHandler callbackEventHandler = this.doc.rootVisualElement.Q("Button2", null);
		visualElement.RegisterCallback<ClickEvent>(new EventCallback<ClickEvent>(this.OnButtonClicked), TrickleDown.NoTrickleDown);
		callbackEventHandler.RegisterCallback<ClickEvent>(new EventCallback<ClickEvent>(this.OnButton2Clicked), TrickleDown.NoTrickleDown);
	}

	// Token: 0x06000F1E RID: 3870 RVA: 0x0003BA4B File Offset: 0x00039C4B
	private void OnButton2Clicked(ClickEvent evt)
	{
		Debug.Log("Button 2 Clicked");
	}

	// Token: 0x06000F1F RID: 3871 RVA: 0x0003BA57 File Offset: 0x00039C57
	private void OnButtonClicked(ClickEvent evt)
	{
		Debug.Log("Button Clicked");
	}

	// Token: 0x04000C5A RID: 3162
	[SerializeField]
	private UIDocument doc;
}
