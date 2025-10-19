using System;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x02000163 RID: 355
public class CopyTextOnClick : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
{
	// Token: 0x17000216 RID: 534
	// (get) Token: 0x06000ACC RID: 2764 RVA: 0x0002E920 File Offset: 0x0002CB20
	[SerializeField]
	private string content
	{
		get
		{
			return Path.Combine(Application.persistentDataPath, "Saves");
		}
	}

	// Token: 0x06000ACD RID: 2765 RVA: 0x0002E931 File Offset: 0x0002CB31
	public void OnPointerClick(PointerEventData eventData)
	{
		GUIUtility.systemCopyBuffer = this.content;
	}
}
