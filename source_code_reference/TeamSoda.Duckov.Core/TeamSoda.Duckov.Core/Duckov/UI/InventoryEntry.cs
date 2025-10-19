using System;
using Duckov.Utilities;
using ItemStatsSystem;
using SodaCraft.Localizations;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Duckov.UI
{
	// Token: 0x0200038C RID: 908
	public class InventoryEntry : MonoBehaviour, IPoolable, IPointerClickHandler, IEventSystemHandler, IDropHandler, IItemDragSource, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler
	{
		// Token: 0x17000618 RID: 1560
		// (get) Token: 0x06001FB9 RID: 8121 RVA: 0x0006ED8A File Offset: 0x0006CF8A
		// (set) Token: 0x06001FBA RID: 8122 RVA: 0x0006ED92 File Offset: 0x0006CF92
		public InventoryDisplay Master { get; private set; }

		// Token: 0x17000619 RID: 1561
		// (get) Token: 0x06001FBB RID: 8123 RVA: 0x0006ED9B File Offset: 0x0006CF9B
		public int Index
		{
			get
			{
				return this.index;
			}
		}

		// Token: 0x1700061A RID: 1562
		// (get) Token: 0x06001FBC RID: 8124 RVA: 0x0006EDA3 File Offset: 0x0006CFA3
		// (set) Token: 0x06001FBD RID: 8125 RVA: 0x0006EDAB File Offset: 0x0006CFAB
		public bool Disabled
		{
			get
			{
				return this.disabled;
			}
			set
			{
				this.disabled = value;
				this.Refresh();
			}
		}

		// Token: 0x1700061B RID: 1563
		// (get) Token: 0x06001FBE RID: 8126 RVA: 0x0006EDBC File Offset: 0x0006CFBC
		public Item Content
		{
			get
			{
				InventoryDisplay master = this.Master;
				Inventory inventory = ((master != null) ? master.Target : null);
				if (inventory == null)
				{
					return null;
				}
				if (this.index >= inventory.Capacity)
				{
					return null;
				}
				InventoryDisplay master2 = this.Master;
				if (master2 == null)
				{
					return null;
				}
				Inventory target = master2.Target;
				if (target == null)
				{
					return null;
				}
				return target.GetItemAt(this.index);
			}
		}

		// Token: 0x1700061C RID: 1564
		// (get) Token: 0x06001FBF RID: 8127 RVA: 0x0006EE1C File Offset: 0x0006D01C
		public bool ShouldHighlight
		{
			get
			{
				return !(this.Master == null) && !(this.Content == null) && (this.Master.EvaluateShouldHighlight(this.Content) || (this.Editable && ItemUIUtilities.IsGunSelected && !this.cacheContentIsGun && this.IsCaliberMatchItemSelected()));
			}
		}

		// Token: 0x06001FC0 RID: 8128 RVA: 0x0006EE7D File Offset: 0x0006D07D
		private bool IsCaliberMatchItemSelected()
		{
			return !(this.Content == null) && ItemUIUtilities.SelectedItemCaliber == this.cachedMeta.caliber;
		}

		// Token: 0x1700061D RID: 1565
		// (get) Token: 0x06001FC1 RID: 8129 RVA: 0x0006EEA4 File Offset: 0x0006D0A4
		public bool CanOperate
		{
			get
			{
				return !(this.Master == null) && this.Master.Func_CanOperate(this.Content);
			}
		}

		// Token: 0x1700061E RID: 1566
		// (get) Token: 0x06001FC2 RID: 8130 RVA: 0x0006EECC File Offset: 0x0006D0CC
		public bool Editable
		{
			get
			{
				return !(this.Master == null) && this.Master.Editable && this.CanOperate;
			}
		}

		// Token: 0x1700061F RID: 1567
		// (get) Token: 0x06001FC3 RID: 8131 RVA: 0x0006EEF3 File Offset: 0x0006D0F3
		public bool Movable
		{
			get
			{
				return !(this.Master == null) && this.Master.Movable;
			}
		}

		// Token: 0x140000D5 RID: 213
		// (add) Token: 0x06001FC4 RID: 8132 RVA: 0x0006EF10 File Offset: 0x0006D110
		// (remove) Token: 0x06001FC5 RID: 8133 RVA: 0x0006EF48 File Offset: 0x0006D148
		public event Action<InventoryEntry> onRefresh;

		// Token: 0x06001FC6 RID: 8134 RVA: 0x0006EF80 File Offset: 0x0006D180
		private void Awake()
		{
			this.itemDisplay.onPointerClick += this.OnItemDisplayPointerClicked;
			this.itemDisplay.onDoubleClicked += this.OnDisplayDoubleClicked;
			this.itemDisplay.onReceiveDrop += this.OnDrop;
			GameObject gameObject = this.hoveringIndicator;
			if (gameObject != null)
			{
				gameObject.SetActive(false);
			}
			UIInputManager.OnFastPick += this.OnFastPick;
			UIInputManager.OnDropItem += this.OnDropItemButton;
			UIInputManager.OnUseItem += this.OnUseItemButton;
		}

		// Token: 0x06001FC7 RID: 8135 RVA: 0x0006F018 File Offset: 0x0006D218
		private void OnEnable()
		{
			ItemUIUtilities.OnSelectionChanged += this.OnSelectionChanged;
			UIInputManager.OnLockInventoryIndex += this.OnInputLockInventoryIndex;
			UIInputManager.OnShortcutInput += this.OnShortcutInput;
		}

		// Token: 0x06001FC8 RID: 8136 RVA: 0x0006F050 File Offset: 0x0006D250
		private void OnDisable()
		{
			this.hovering = false;
			GameObject gameObject = this.hoveringIndicator;
			if (gameObject != null)
			{
				gameObject.SetActive(false);
			}
			ItemUIUtilities.OnSelectionChanged -= this.OnSelectionChanged;
			UIInputManager.OnLockInventoryIndex -= this.OnInputLockInventoryIndex;
			UIInputManager.OnShortcutInput -= this.OnShortcutInput;
		}

		// Token: 0x06001FC9 RID: 8137 RVA: 0x0006F0A9 File Offset: 0x0006D2A9
		private void OnShortcutInput(UIInputEventData data, int shortcutIndex)
		{
			if (!this.hovering)
			{
				return;
			}
			if (this.Item == null)
			{
				return;
			}
			ItemShortcut.Set(shortcutIndex, this.Item);
			ItemUIUtilities.NotifyPutItem(this.Item, false);
		}

		// Token: 0x06001FCA RID: 8138 RVA: 0x0006F0DC File Offset: 0x0006D2DC
		private void OnInputLockInventoryIndex(UIInputEventData data)
		{
			if (!this.hovering)
			{
				return;
			}
			this.ToggleLock();
		}

		// Token: 0x06001FCB RID: 8139 RVA: 0x0006F0ED File Offset: 0x0006D2ED
		private void OnSelectionChanged()
		{
			this.highlightIndicator.SetActive(this.ShouldHighlight);
			if (ItemUIUtilities.SelectedItemDisplay == this.itemDisplay)
			{
				this.Refresh();
			}
		}

		// Token: 0x06001FCC RID: 8140 RVA: 0x0006F118 File Offset: 0x0006D318
		private void OnDestroy()
		{
			UIInputManager.OnFastPick -= this.OnFastPick;
			UIInputManager.OnDropItem -= this.OnDropItemButton;
			UIInputManager.OnUseItem -= this.OnUseItemButton;
			if (this.itemDisplay != null)
			{
				this.itemDisplay.onPointerClick -= this.OnItemDisplayPointerClicked;
				this.itemDisplay.onDoubleClicked -= this.OnDisplayDoubleClicked;
				this.itemDisplay.onReceiveDrop -= this.OnDrop;
			}
		}

		// Token: 0x06001FCD RID: 8141 RVA: 0x0006F1AC File Offset: 0x0006D3AC
		private void OnFastPick(UIInputEventData data)
		{
			if (data.Used)
			{
				return;
			}
			if (!base.isActiveAndEnabled)
			{
				return;
			}
			if (!this.hovering)
			{
				return;
			}
			this.Master.NotifyItemDoubleClicked(this, new PointerEventData(EventSystem.current));
			data.Use();
		}

		// Token: 0x06001FCE RID: 8142 RVA: 0x0006F1E8 File Offset: 0x0006D3E8
		private void OnDropItemButton(UIInputEventData data)
		{
			if (!base.isActiveAndEnabled)
			{
				return;
			}
			if (!this.hovering)
			{
				return;
			}
			if (this.Item == null)
			{
				return;
			}
			if (!this.Item.CanDrop)
			{
				return;
			}
			if (this.CanOperate)
			{
				this.Item.Drop(CharacterMainControl.Main, true);
			}
		}

		// Token: 0x06001FCF RID: 8143 RVA: 0x0006F240 File Offset: 0x0006D440
		private void OnUseItemButton(UIInputEventData data)
		{
			if (!base.isActiveAndEnabled)
			{
				return;
			}
			if (!this.hovering)
			{
				return;
			}
			if (this.Item == null)
			{
				return;
			}
			if (!this.Item.IsUsable(CharacterMainControl.Main))
			{
				return;
			}
			if (this.CanOperate)
			{
				CharacterMainControl.Main.UseItem(this.Item);
			}
		}

		// Token: 0x06001FD0 RID: 8144 RVA: 0x0006F29C File Offset: 0x0006D49C
		private void OnItemDisplayPointerClicked(ItemDisplay display, PointerEventData data)
		{
			if (!base.isActiveAndEnabled)
			{
				return;
			}
			if (this.disabled || !this.CanOperate)
			{
				data.Use();
				return;
			}
			if (!this.Editable)
			{
				return;
			}
			if (data.button == PointerEventData.InputButton.Left)
			{
				if (this.Content == null)
				{
					return;
				}
				if (Keyboard.current != null && Keyboard.current.altKey.isPressed)
				{
					data.Use();
					if (ItemUIUtilities.SelectedItem != null)
					{
						ItemUIUtilities.SelectedItem.TryPlug(this.Content, false, null, 0);
					}
					CharacterMainControl.Main.CharacterItem.TryPlug(this.Content, false, null, 0);
					return;
				}
				if (ItemUIUtilities.SelectedItem == null)
				{
					return;
				}
				if (this.Content.Stackable && ItemUIUtilities.SelectedItem != this.Content && ItemUIUtilities.SelectedItem.TypeID == this.Content.TypeID)
				{
					ItemUIUtilities.SelectedItem.CombineInto(this.Content);
					return;
				}
			}
			else if (data.button == PointerEventData.InputButton.Right && this.Editable && this.Content != null)
			{
				ItemOperationMenu.Show(this.itemDisplay);
			}
		}

		// Token: 0x06001FD1 RID: 8145 RVA: 0x0006F3C4 File Offset: 0x0006D5C4
		private void OnDisplayDoubleClicked(ItemDisplay display, PointerEventData data)
		{
			this.Master.NotifyItemDoubleClicked(this, data);
		}

		// Token: 0x06001FD2 RID: 8146 RVA: 0x0006F3D3 File Offset: 0x0006D5D3
		public void Setup(InventoryDisplay master, int index, bool disabled = false)
		{
			this.Master = master;
			this.index = index;
			this.disabled = disabled;
			this.Refresh();
		}

		// Token: 0x06001FD3 RID: 8147 RVA: 0x0006F3F0 File Offset: 0x0006D5F0
		internal void Refresh()
		{
			Item content = this.Content;
			if (content != null)
			{
				this.cachedMeta = ItemAssetsCollection.GetMetaData(content.TypeID);
				this.cacheContentIsGun = content.Tags.Contains("Gun");
			}
			else
			{
				this.cacheContentIsGun = false;
				this.cachedMeta = default(ItemMetaData);
			}
			this.itemDisplay.Setup(content);
			this.itemDisplay.CanDrop = this.CanOperate;
			this.itemDisplay.Movable = this.Movable;
			this.itemDisplay.Editable = this.Editable && this.CanOperate;
			this.itemDisplay.CanLockSort = true;
			if (!this.Master.Target.NeedInspection && content != null)
			{
				content.Inspected = true;
			}
			this.itemDisplay.ShowOperationButtons = this.Master.ShowOperationButtons;
			this.shortcutIndicator.gameObject.SetActive(this.Master.IsShortcut(this.index));
			this.disabledIndicator.SetActive(this.disabled || !this.CanOperate);
			this.highlightIndicator.SetActive(this.ShouldHighlight);
			bool flag = this.Master.Target.IsIndexLocked(this.Index);
			this.lockIndicator.SetActive(flag);
			Action<InventoryEntry> action = this.onRefresh;
			if (action == null)
			{
				return;
			}
			action(this);
		}

		// Token: 0x17000620 RID: 1568
		// (get) Token: 0x06001FD4 RID: 8148 RVA: 0x0006F55C File Offset: 0x0006D75C
		public static PrefabPool<InventoryEntry> Pool
		{
			get
			{
				return GameplayUIManager.Instance.InventoryEntryPool;
			}
		}

		// Token: 0x17000621 RID: 1569
		// (get) Token: 0x06001FD5 RID: 8149 RVA: 0x0006F568 File Offset: 0x0006D768
		public Item Item
		{
			get
			{
				if (this.itemDisplay != null && this.itemDisplay.isActiveAndEnabled)
				{
					return this.itemDisplay.Target;
				}
				return null;
			}
		}

		// Token: 0x06001FD6 RID: 8150 RVA: 0x0006F592 File Offset: 0x0006D792
		public static InventoryEntry Get()
		{
			return InventoryEntry.Pool.Get(null);
		}

		// Token: 0x06001FD7 RID: 8151 RVA: 0x0006F59F File Offset: 0x0006D79F
		public static void Release(InventoryEntry item)
		{
			InventoryEntry.Pool.Release(item);
		}

		// Token: 0x06001FD8 RID: 8152 RVA: 0x0006F5AC File Offset: 0x0006D7AC
		public void NotifyPooled()
		{
		}

		// Token: 0x06001FD9 RID: 8153 RVA: 0x0006F5AE File Offset: 0x0006D7AE
		public void NotifyReleased()
		{
			this.Master = null;
		}

		// Token: 0x06001FDA RID: 8154 RVA: 0x0006F5B8 File Offset: 0x0006D7B8
		public void OnPointerClick(PointerEventData eventData)
		{
			this.Punch();
			if (eventData.button == PointerEventData.InputButton.Left)
			{
				this.lastClickTime = eventData.clickTime;
				if (this.Editable)
				{
					Item selectedItem = ItemUIUtilities.SelectedItem;
					if (!(selectedItem == null))
					{
						if (this.Content != null)
						{
							Debug.Log(string.Format("{0}(Inventory) 的 {1} 已经有物品。操作已取消。", this.Master.Target.name, this.index));
						}
						else
						{
							eventData.Use();
							selectedItem.Detach();
							this.Master.Target.AddAt(selectedItem, this.index);
							ItemUIUtilities.NotifyPutItem(selectedItem, false);
						}
					}
				}
				this.lastClickTime = eventData.clickTime;
			}
		}

		// Token: 0x06001FDB RID: 8155 RVA: 0x0006F66A File Offset: 0x0006D86A
		internal void Punch()
		{
			this.itemDisplay.Punch();
		}

		// Token: 0x06001FDC RID: 8156 RVA: 0x0006F677 File Offset: 0x0006D877
		public void OnDrag(PointerEventData eventData)
		{
		}

		// Token: 0x06001FDD RID: 8157 RVA: 0x0006F67C File Offset: 0x0006D87C
		public void OnDrop(PointerEventData eventData)
		{
			if (eventData.used)
			{
				return;
			}
			if (!this.Editable)
			{
				return;
			}
			if (eventData.button != PointerEventData.InputButton.Left)
			{
				return;
			}
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
			if (item.Sticky && !this.Master.Target.AcceptSticky)
			{
				return;
			}
			if (Keyboard.current != null && Keyboard.current.ctrlKey.isPressed)
			{
				if (this.Content != null)
				{
					NotificationText.Push("UI_Inventory_TargetOccupiedCannotSplit".ToPlainText());
					return;
				}
				Debug.Log("SPLIT");
				SplitDialogue.SetupAndShow(item, this.Master.Target, this.index);
				return;
			}
			else
			{
				ItemUIUtilities.NotifyPutItem(item, false);
				if (this.Content == null)
				{
					item.Detach();
					this.Master.Target.AddAt(item, this.index);
					return;
				}
				if (this.Content.TypeID == item.TypeID && this.Content.Stackable)
				{
					this.Content.Combine(item);
					return;
				}
				Inventory inInventory = item.InInventory;
				Inventory target = this.Master.Target;
				if (inInventory != null)
				{
					int num = inInventory.GetIndex(item);
					int num2 = this.index;
					Item content = this.Content;
					if (content != item)
					{
						item.Detach();
						content.Detach();
						inInventory.AddAt(content, num);
						target.AddAt(item, num2);
					}
				}
				return;
			}
		}

		// Token: 0x06001FDE RID: 8158 RVA: 0x0006F80B File Offset: 0x0006DA0B
		public bool IsEditable()
		{
			return !(this.Content == null) && !this.Content.NeedInspection && this.Editable;
		}

		// Token: 0x06001FDF RID: 8159 RVA: 0x0006F832 File Offset: 0x0006DA32
		public Item GetItem()
		{
			return this.Content;
		}

		// Token: 0x06001FE0 RID: 8160 RVA: 0x0006F83A File Offset: 0x0006DA3A
		public void OnPointerEnter(PointerEventData eventData)
		{
			this.hovering = true;
			GameObject gameObject = this.hoveringIndicator;
			if (gameObject == null)
			{
				return;
			}
			gameObject.SetActive(this.Editable);
		}

		// Token: 0x06001FE1 RID: 8161 RVA: 0x0006F859 File Offset: 0x0006DA59
		public void OnPointerExit(PointerEventData eventData)
		{
			this.hovering = false;
			GameObject gameObject = this.hoveringIndicator;
			if (gameObject == null)
			{
				return;
			}
			gameObject.SetActive(false);
		}

		// Token: 0x06001FE2 RID: 8162 RVA: 0x0006F873 File Offset: 0x0006DA73
		public void ToggleLock()
		{
			this.Master.Target.ToggleLockIndex(this.Index);
		}

		// Token: 0x040015B0 RID: 5552
		[SerializeField]
		private ItemDisplay itemDisplay;

		// Token: 0x040015B1 RID: 5553
		[SerializeField]
		private GameObject shortcutIndicator;

		// Token: 0x040015B2 RID: 5554
		[SerializeField]
		private GameObject disabledIndicator;

		// Token: 0x040015B3 RID: 5555
		[SerializeField]
		private GameObject hoveringIndicator;

		// Token: 0x040015B4 RID: 5556
		[SerializeField]
		private GameObject highlightIndicator;

		// Token: 0x040015B5 RID: 5557
		[SerializeField]
		private GameObject lockIndicator;

		// Token: 0x040015B7 RID: 5559
		[SerializeField]
		private int index;

		// Token: 0x040015B8 RID: 5560
		[SerializeField]
		private bool disabled;

		// Token: 0x040015BA RID: 5562
		private bool cacheContentIsGun;

		// Token: 0x040015BB RID: 5563
		private ItemMetaData cachedMeta;

		// Token: 0x040015BC RID: 5564
		public const float doubleClickTimeThreshold = 0.3f;

		// Token: 0x040015BD RID: 5565
		private float lastClickTime;

		// Token: 0x040015BE RID: 5566
		private bool hovering;
	}
}
