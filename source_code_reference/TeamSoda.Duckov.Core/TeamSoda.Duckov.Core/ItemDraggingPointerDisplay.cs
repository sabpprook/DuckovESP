using System;
using Duckov.UI;
using ItemStatsSystem;
using UnityEngine;
using UnityEngine.InputSystem;

// Token: 0x0200015F RID: 351
public class ItemDraggingPointerDisplay : MonoBehaviour
{
	// Token: 0x06000AB7 RID: 2743 RVA: 0x0002E5F8 File Offset: 0x0002C7F8
	private void Awake()
	{
		this.rectTransform = base.transform as RectTransform;
		this.parentRectTransform = base.transform.parent as RectTransform;
		IItemDragSource.OnStartDragItem += this.OnStartDragItem;
		IItemDragSource.OnEndDragItem += this.OnEndDragItem;
		base.gameObject.SetActive(false);
	}

	// Token: 0x06000AB8 RID: 2744 RVA: 0x0002E65A File Offset: 0x0002C85A
	private void OnDestroy()
	{
		IItemDragSource.OnStartDragItem -= this.OnStartDragItem;
		IItemDragSource.OnEndDragItem -= this.OnEndDragItem;
	}

	// Token: 0x06000AB9 RID: 2745 RVA: 0x0002E67E File Offset: 0x0002C87E
	private void Update()
	{
		this.RefreshPosition();
		if (Mouse.current.leftButton.wasReleasedThisFrame)
		{
			this.OnEndDragItem(null);
		}
	}

	// Token: 0x06000ABA RID: 2746 RVA: 0x0002E6A0 File Offset: 0x0002C8A0
	private unsafe void RefreshPosition()
	{
		Vector2 vector;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(this.rectTransform.parent as RectTransform, *Pointer.current.position.value, null, out vector);
		this.rectTransform.localPosition = vector;
	}

	// Token: 0x06000ABB RID: 2747 RVA: 0x0002E6EB File Offset: 0x0002C8EB
	private void OnStartDragItem(Item item)
	{
		this.target = item;
		if (this.target == null)
		{
			return;
		}
		this.display.Setup(this.target);
		this.RefreshPosition();
		base.gameObject.SetActive(true);
	}

	// Token: 0x06000ABC RID: 2748 RVA: 0x0002E726 File Offset: 0x0002C926
	private void OnEndDragItem(Item item)
	{
		this.target = null;
		base.gameObject.SetActive(false);
	}

	// Token: 0x0400094D RID: 2381
	[SerializeField]
	private RectTransform rectTransform;

	// Token: 0x0400094E RID: 2382
	[SerializeField]
	private RectTransform parentRectTransform;

	// Token: 0x0400094F RID: 2383
	[SerializeField]
	private ItemDisplay display;

	// Token: 0x04000950 RID: 2384
	private Item target;
}
