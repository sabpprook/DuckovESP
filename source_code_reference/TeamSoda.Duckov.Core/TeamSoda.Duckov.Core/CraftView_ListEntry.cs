using System;
using ItemStatsSystem;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020001AA RID: 426
public class CraftView_ListEntry : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
{
	// Token: 0x17000248 RID: 584
	// (get) Token: 0x06000C9F RID: 3231 RVA: 0x00034FC6 File Offset: 0x000331C6
	// (set) Token: 0x06000CA0 RID: 3232 RVA: 0x00034FCE File Offset: 0x000331CE
	public CraftView Master { get; private set; }

	// Token: 0x17000249 RID: 585
	// (get) Token: 0x06000CA1 RID: 3233 RVA: 0x00034FD7 File Offset: 0x000331D7
	// (set) Token: 0x06000CA2 RID: 3234 RVA: 0x00034FDF File Offset: 0x000331DF
	public CraftingFormula Formula { get; private set; }

	// Token: 0x06000CA3 RID: 3235 RVA: 0x00034FE8 File Offset: 0x000331E8
	public void Setup(CraftView master, CraftingFormula formula)
	{
		this.Master = master;
		this.Formula = formula;
		ItemMetaData metaData = ItemAssetsCollection.GetMetaData(this.Formula.result.id);
		this.icon.sprite = metaData.icon;
		this.nameText.text = string.Format("{0} x{1}", metaData.DisplayName, formula.result.amount);
		this.Refresh();
	}

	// Token: 0x06000CA4 RID: 3236 RVA: 0x0003505C File Offset: 0x0003325C
	public void OnPointerClick(PointerEventData eventData)
	{
		CraftView master = this.Master;
		if (master == null)
		{
			return;
		}
		master.SetSelection(this);
	}

	// Token: 0x06000CA5 RID: 3237 RVA: 0x00035070 File Offset: 0x00033270
	internal void NotifyUnselected()
	{
		this.Refresh();
	}

	// Token: 0x06000CA6 RID: 3238 RVA: 0x00035078 File Offset: 0x00033278
	internal void NotifySelected()
	{
		this.Refresh();
	}

	// Token: 0x06000CA7 RID: 3239 RVA: 0x00035080 File Offset: 0x00033280
	private void Refresh()
	{
		if (this.Master == null)
		{
			return;
		}
		bool flag = this.Master.GetSelection() == this;
		this.background.color = (flag ? this.selectedColor : this.normalColor);
	}

	// Token: 0x04000AF3 RID: 2803
	[SerializeField]
	private Color normalColor;

	// Token: 0x04000AF4 RID: 2804
	[SerializeField]
	private Color selectedColor;

	// Token: 0x04000AF5 RID: 2805
	[SerializeField]
	private Image icon;

	// Token: 0x04000AF6 RID: 2806
	[SerializeField]
	private Image background;

	// Token: 0x04000AF7 RID: 2807
	[SerializeField]
	private TextMeshProUGUI nameText;
}
