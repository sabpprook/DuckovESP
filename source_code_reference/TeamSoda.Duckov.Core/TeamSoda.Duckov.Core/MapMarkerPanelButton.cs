using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x020001BF RID: 447
public class MapMarkerPanelButton : MonoBehaviour
{
	// Token: 0x17000274 RID: 628
	// (get) Token: 0x06000D55 RID: 3413 RVA: 0x0003715F File Offset: 0x0003535F
	public Image Image
	{
		get
		{
			return this.image;
		}
	}

	// Token: 0x06000D56 RID: 3414 RVA: 0x00037167 File Offset: 0x00035367
	public void Setup(UnityAction action, bool selected)
	{
		this.button.onClick.RemoveAllListeners();
		this.button.onClick.AddListener(action);
		this.selectionIndicator.gameObject.SetActive(selected);
	}

	// Token: 0x04000B58 RID: 2904
	[SerializeField]
	private GameObject selectionIndicator;

	// Token: 0x04000B59 RID: 2905
	[SerializeField]
	private Image image;

	// Token: 0x04000B5A RID: 2906
	[SerializeField]
	private Button button;
}
