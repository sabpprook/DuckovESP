using System;
using SodaCraft.Localizations;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020001A9 RID: 425
public class CraftViewFilterBtnEntry : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
{
	// Token: 0x06000C9C RID: 3228 RVA: 0x00034F46 File Offset: 0x00033146
	public void OnPointerClick(PointerEventData eventData)
	{
		if (this.master == null)
		{
			return;
		}
		this.master.SetFilter(this.index);
	}

	// Token: 0x06000C9D RID: 3229 RVA: 0x00034F68 File Offset: 0x00033168
	public void Setup(CraftView master, CraftView.FilterInfo filterInfo, int index, bool selected)
	{
		this.master = master;
		this.info = filterInfo;
		this.index = index;
		this.icon.sprite = filterInfo.icon;
		this.displayNameText.text = filterInfo.displayNameKey.ToPlainText();
		this.selectedIndicator.SetActive(selected);
	}

	// Token: 0x04000AED RID: 2797
	[SerializeField]
	private Image icon;

	// Token: 0x04000AEE RID: 2798
	[SerializeField]
	private TextMeshProUGUI displayNameText;

	// Token: 0x04000AEF RID: 2799
	[SerializeField]
	private GameObject selectedIndicator;

	// Token: 0x04000AF0 RID: 2800
	private CraftView.FilterInfo info;

	// Token: 0x04000AF1 RID: 2801
	private CraftView master;

	// Token: 0x04000AF2 RID: 2802
	private int index;
}
