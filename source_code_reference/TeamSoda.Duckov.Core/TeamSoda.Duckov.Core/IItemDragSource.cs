using System;
using Duckov.UI;
using ItemStatsSystem;
using UnityEngine.EventSystems;

// Token: 0x0200015E RID: 350
public interface IItemDragSource : IBeginDragHandler, IEventSystemHandler, IEndDragHandler, IDragHandler
{
	// Token: 0x1400004E RID: 78
	// (add) Token: 0x06000AAF RID: 2735 RVA: 0x0002E4B0 File Offset: 0x0002C6B0
	// (remove) Token: 0x06000AB0 RID: 2736 RVA: 0x0002E4E4 File Offset: 0x0002C6E4
	public static event Action<Item> OnStartDragItem;

	// Token: 0x1400004F RID: 79
	// (add) Token: 0x06000AB1 RID: 2737 RVA: 0x0002E518 File Offset: 0x0002C718
	// (remove) Token: 0x06000AB2 RID: 2738 RVA: 0x0002E54C File Offset: 0x0002C74C
	public static event Action<Item> OnEndDragItem;

	// Token: 0x06000AB3 RID: 2739
	bool IsEditable();

	// Token: 0x06000AB4 RID: 2740
	Item GetItem();

	// Token: 0x06000AB5 RID: 2741 RVA: 0x0002E580 File Offset: 0x0002C780
	void OnBeginDrag(PointerEventData eventData)
	{
		if (!this.IsEditable())
		{
			return;
		}
		if (eventData.button != PointerEventData.InputButton.Left)
		{
			return;
		}
		Item item = this.GetItem();
		Action<Item> onStartDragItem = IItemDragSource.OnStartDragItem;
		if (onStartDragItem != null)
		{
			onStartDragItem(item);
		}
		if (item == null)
		{
			return;
		}
		ItemUIUtilities.NotifyPutItem(item, true);
	}

	// Token: 0x06000AB6 RID: 2742 RVA: 0x0002E5C8 File Offset: 0x0002C7C8
	void OnEndDrag(PointerEventData eventData)
	{
		if (eventData.button != PointerEventData.InputButton.Left)
		{
			return;
		}
		Item item = this.GetItem();
		Action<Item> onEndDragItem = IItemDragSource.OnEndDragItem;
		if (onEndDragItem == null)
		{
			return;
		}
		onEndDragItem(item);
	}
}
