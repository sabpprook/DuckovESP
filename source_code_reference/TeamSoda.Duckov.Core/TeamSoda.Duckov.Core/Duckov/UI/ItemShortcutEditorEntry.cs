using System;
using Duckov.Utilities;
using ItemStatsSystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Duckov.UI
{
	// Token: 0x020003A7 RID: 935
	public class ItemShortcutEditorEntry : MonoBehaviour, IPointerClickHandler, IEventSystemHandler, IDropHandler, IPointerEnterHandler, IPointerExitHandler, IItemDragSource, IBeginDragHandler, IEndDragHandler, IDragHandler
	{
		// Token: 0x1700066F RID: 1647
		// (get) Token: 0x0600218B RID: 8587 RVA: 0x00075148 File Offset: 0x00073348
		private Item TargetItem
		{
			get
			{
				return ItemShortcut.Get(this.index);
			}
		}

		// Token: 0x0600218C RID: 8588 RVA: 0x00075158 File Offset: 0x00073358
		private void Awake()
		{
			this.itemDisplay.onPointerClick += this.OnItemDisplayClicked;
			this.itemDisplay.onReceiveDrop += this.OnDrop;
			ItemShortcut.OnSetItem += this.OnSetItem;
			this.hoveringIndicator.SetActive(false);
		}

		// Token: 0x0600218D RID: 8589 RVA: 0x000751B1 File Offset: 0x000733B1
		private void OnSetItem(int index)
		{
			if (index == this.index)
			{
				this.Refresh();
			}
		}

		// Token: 0x0600218E RID: 8590 RVA: 0x000751C2 File Offset: 0x000733C2
		private void OnItemDisplayClicked(ItemDisplay display, PointerEventData data)
		{
			this.OnPointerClick(data);
			data.Use();
		}

		// Token: 0x0600218F RID: 8591 RVA: 0x000751D1 File Offset: 0x000733D1
		public void OnPointerClick(PointerEventData eventData)
		{
			if (ItemUIUtilities.SelectedItem != null && ItemShortcut.Set(this.index, ItemUIUtilities.SelectedItem))
			{
				this.Refresh();
			}
		}

		// Token: 0x06002190 RID: 8592 RVA: 0x000751F8 File Offset: 0x000733F8
		internal void Refresh()
		{
			this.UnregisterEvents();
			if (this.displayingItem != this.TargetItem)
			{
				this.itemDisplay.Punch();
			}
			this.displayingItem = this.TargetItem;
			this.itemDisplay.Setup(this.displayingItem);
			this.itemDisplay.ShowOperationButtons = false;
			this.RegisterEvents();
		}

		// Token: 0x06002191 RID: 8593 RVA: 0x00075258 File Offset: 0x00073458
		private void RegisterEvents()
		{
			if (this.displayingItem != null)
			{
				this.displayingItem.onParentChanged += this.OnTargetParentChanged;
				this.displayingItem.onSetStackCount += this.OnTargetStackCountChanged;
			}
		}

		// Token: 0x06002192 RID: 8594 RVA: 0x00075296 File Offset: 0x00073496
		private void UnregisterEvents()
		{
			if (this.displayingItem != null)
			{
				this.displayingItem.onParentChanged -= this.OnTargetParentChanged;
				this.displayingItem.onSetStackCount -= this.OnTargetStackCountChanged;
			}
		}

		// Token: 0x06002193 RID: 8595 RVA: 0x000752D4 File Offset: 0x000734D4
		private void OnTargetStackCountChanged(Item item)
		{
			this.SetDirty();
		}

		// Token: 0x06002194 RID: 8596 RVA: 0x000752DC File Offset: 0x000734DC
		private void OnTargetParentChanged(Item item)
		{
			this.SetDirty();
		}

		// Token: 0x06002195 RID: 8597 RVA: 0x000752E4 File Offset: 0x000734E4
		private void SetDirty()
		{
			this.dirty = true;
		}

		// Token: 0x06002196 RID: 8598 RVA: 0x000752ED File Offset: 0x000734ED
		private void Update()
		{
			if (this.dirty)
			{
				this.Refresh();
			}
		}

		// Token: 0x06002197 RID: 8599 RVA: 0x000752FD File Offset: 0x000734FD
		private void OnDestroy()
		{
			this.UnregisterEvents();
			ItemShortcut.OnSetItem -= this.OnSetItem;
		}

		// Token: 0x06002198 RID: 8600 RVA: 0x00075318 File Offset: 0x00073518
		internal void Setup(int i)
		{
			this.index = i;
			this.Refresh();
			InputActionReference inputActionReference = InputActionReference.Create(GameplayDataSettings.InputActions[string.Format("Character/ItemShortcut{0}", i + 3)]);
			this.indicator.Setup(inputActionReference, -1);
		}

		// Token: 0x06002199 RID: 8601 RVA: 0x00075364 File Offset: 0x00073564
		public void OnDrop(PointerEventData eventData)
		{
			eventData.Use();
			IItemDragSource component = eventData.pointerDrag.gameObject.GetComponent<IItemDragSource>();
			if (component == null)
			{
				return;
			}
			if (!component.IsEditable())
			{
				return;
			}
			Item item = component.GetItem();
			if (item == null)
			{
				return;
			}
			if (!item.IsInPlayerCharacter())
			{
				ItemUtilities.SendToPlayer(item, false, false);
			}
			if (ItemShortcut.Set(this.index, item))
			{
				this.Refresh();
				AudioManager.Post("UI/click");
			}
		}

		// Token: 0x0600219A RID: 8602 RVA: 0x000753D5 File Offset: 0x000735D5
		public void OnPointerEnter(PointerEventData eventData)
		{
			this.hoveringIndicator.SetActive(true);
		}

		// Token: 0x0600219B RID: 8603 RVA: 0x000753E3 File Offset: 0x000735E3
		public void OnPointerExit(PointerEventData eventData)
		{
			this.hoveringIndicator.SetActive(false);
		}

		// Token: 0x0600219C RID: 8604 RVA: 0x000753F1 File Offset: 0x000735F1
		public bool IsEditable()
		{
			return this.TargetItem != null;
		}

		// Token: 0x0600219D RID: 8605 RVA: 0x000753FF File Offset: 0x000735FF
		public Item GetItem()
		{
			return this.TargetItem;
		}

		// Token: 0x0600219E RID: 8606 RVA: 0x00075407 File Offset: 0x00073607
		public void OnDrag(PointerEventData eventData)
		{
		}

		// Token: 0x040016B7 RID: 5815
		[SerializeField]
		private ItemDisplay itemDisplay;

		// Token: 0x040016B8 RID: 5816
		[SerializeField]
		private GameObject hoveringIndicator;

		// Token: 0x040016B9 RID: 5817
		[SerializeField]
		private int index;

		// Token: 0x040016BA RID: 5818
		[SerializeField]
		private InputIndicator indicator;

		// Token: 0x040016BB RID: 5819
		private Item displayingItem;

		// Token: 0x040016BC RID: 5820
		private bool dirty;
	}
}
