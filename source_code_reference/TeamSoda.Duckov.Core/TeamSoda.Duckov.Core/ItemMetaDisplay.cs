using System;
using Duckov.Utilities;
using ItemStatsSystem;
using LeTai.TrueShadow;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020001F1 RID: 497
public class ItemMetaDisplay : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IItemMetaDataProvider
{
	// Token: 0x06000E85 RID: 3717 RVA: 0x0003A3C1 File Offset: 0x000385C1
	public ItemMetaData GetMetaData()
	{
		return this.data;
	}

	// Token: 0x14000066 RID: 102
	// (add) Token: 0x06000E86 RID: 3718 RVA: 0x0003A3CC File Offset: 0x000385CC
	// (remove) Token: 0x06000E87 RID: 3719 RVA: 0x0003A400 File Offset: 0x00038600
	public static event Action<ItemMetaDisplay> OnMouseEnter;

	// Token: 0x14000067 RID: 103
	// (add) Token: 0x06000E88 RID: 3720 RVA: 0x0003A434 File Offset: 0x00038634
	// (remove) Token: 0x06000E89 RID: 3721 RVA: 0x0003A468 File Offset: 0x00038668
	public static event Action<ItemMetaDisplay> OnMouseExit;

	// Token: 0x06000E8A RID: 3722 RVA: 0x0003A49B File Offset: 0x0003869B
	public void OnPointerEnter(PointerEventData eventData)
	{
		Action<ItemMetaDisplay> onMouseEnter = ItemMetaDisplay.OnMouseEnter;
		if (onMouseEnter == null)
		{
			return;
		}
		onMouseEnter(this);
	}

	// Token: 0x06000E8B RID: 3723 RVA: 0x0003A4AD File Offset: 0x000386AD
	public void OnPointerExit(PointerEventData eventData)
	{
		Action<ItemMetaDisplay> onMouseExit = ItemMetaDisplay.OnMouseExit;
		if (onMouseExit == null)
		{
			return;
		}
		onMouseExit(this);
	}

	// Token: 0x06000E8C RID: 3724 RVA: 0x0003A4C0 File Offset: 0x000386C0
	public void Setup(int typeID)
	{
		ItemMetaData metaData = ItemAssetsCollection.GetMetaData(typeID);
		this.Setup(metaData);
	}

	// Token: 0x06000E8D RID: 3725 RVA: 0x0003A4DB File Offset: 0x000386DB
	public void Setup(ItemMetaData data)
	{
		this.data = data;
		this.icon.sprite = data.icon;
		GameplayDataSettings.UIStyle.ApplyDisplayQualityShadow(data.displayQuality, this.displayQualityShadow);
	}

	// Token: 0x06000E8E RID: 3726 RVA: 0x0003A50B File Offset: 0x0003870B
	internal void Setup(object rootTypeID)
	{
		throw new NotImplementedException();
	}

	// Token: 0x04000C0B RID: 3083
	[SerializeField]
	private Image icon;

	// Token: 0x04000C0C RID: 3084
	[SerializeField]
	private TrueShadow displayQualityShadow;

	// Token: 0x04000C0D RID: 3085
	private ItemMetaData data;
}
