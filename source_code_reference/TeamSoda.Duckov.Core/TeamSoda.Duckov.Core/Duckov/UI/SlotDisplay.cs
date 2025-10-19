using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Duckov.UI.Animations;
using Duckov.Utilities;
using ItemStatsSystem;
using ItemStatsSystem.Items;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Duckov.UI
{
	// Token: 0x02000399 RID: 921
	public class SlotDisplay : MonoBehaviour, IPoolable, IPointerClickHandler, IEventSystemHandler, IItemDragSource, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler, IPointerEnterHandler, IPointerExitHandler
	{
		// Token: 0x140000DD RID: 221
		// (add) Token: 0x060020B2 RID: 8370 RVA: 0x0007224C File Offset: 0x0007044C
		// (remove) Token: 0x060020B3 RID: 8371 RVA: 0x00072284 File Offset: 0x00070484
		internal event Action<SlotDisplay> onSlotDisplayClicked;

		// Token: 0x140000DE RID: 222
		// (add) Token: 0x060020B4 RID: 8372 RVA: 0x000722BC File Offset: 0x000704BC
		// (remove) Token: 0x060020B5 RID: 8373 RVA: 0x000722F4 File Offset: 0x000704F4
		internal event Action<SlotDisplay> onSlotDisplayDoubleClicked;

		// Token: 0x1700064B RID: 1611
		// (get) Token: 0x060020B6 RID: 8374 RVA: 0x00072329 File Offset: 0x00070529
		// (set) Token: 0x060020B7 RID: 8375 RVA: 0x00072331 File Offset: 0x00070531
		public bool Editable
		{
			get
			{
				return this.editable;
			}
			internal set
			{
				this.editable = value;
			}
		}

		// Token: 0x1700064C RID: 1612
		// (get) Token: 0x060020B8 RID: 8376 RVA: 0x0007233A File Offset: 0x0007053A
		// (set) Token: 0x060020B9 RID: 8377 RVA: 0x00072342 File Offset: 0x00070542
		public bool ContentSelectable
		{
			get
			{
				return this.contentSelectable;
			}
			internal set
			{
				this.contentSelectable = value;
			}
		}

		// Token: 0x1700064D RID: 1613
		// (get) Token: 0x060020BA RID: 8378 RVA: 0x0007234B File Offset: 0x0007054B
		// (set) Token: 0x060020BB RID: 8379 RVA: 0x00072353 File Offset: 0x00070553
		public bool ShowOperationMenu
		{
			get
			{
				return this.showOperationMenu;
			}
			internal set
			{
				this.showOperationMenu = value;
			}
		}

		// Token: 0x1700064E RID: 1614
		// (get) Token: 0x060020BC RID: 8380 RVA: 0x0007235C File Offset: 0x0007055C
		// (set) Token: 0x060020BD RID: 8381 RVA: 0x00072364 File Offset: 0x00070564
		public Slot Target { get; private set; }

		// Token: 0x140000DF RID: 223
		// (add) Token: 0x060020BE RID: 8382 RVA: 0x00072370 File Offset: 0x00070570
		// (remove) Token: 0x060020BF RID: 8383 RVA: 0x000723A4 File Offset: 0x000705A4
		public static event Action<SlotDisplayOperationContext> onOperation;

		// Token: 0x060020C0 RID: 8384 RVA: 0x000723D8 File Offset: 0x000705D8
		private void RegisterEvents()
		{
			this.UnregisterEvents();
			if (!base.isActiveAndEnabled)
			{
				return;
			}
			if (this.Target != null)
			{
				this.Target.onSlotContentChanged += this.OnTargetContentChanged;
			}
			ItemUIUtilities.OnSelectionChanged += this.OnItemSelectionChanged;
			this.itemDisplay.onPointerClick += this.OnItemDisplayClicked;
			this.itemDisplay.onDoubleClicked += this.OnItemDisplayDoubleClicked;
			IItemDragSource.OnStartDragItem += this.OnStartDragItem;
			IItemDragSource.OnEndDragItem += this.OnEndDragItem;
			UIInputManager.OnFastPick += this.OnFastPick;
			UIInputManager.OnDropItem += this.OnFastDrop;
			UIInputManager.OnUseItem += this.OnFastUse;
		}

		// Token: 0x060020C1 RID: 8385 RVA: 0x000724A8 File Offset: 0x000706A8
		private void UnregisterEvents()
		{
			if (this.Target != null)
			{
				this.Target.onSlotContentChanged -= this.OnTargetContentChanged;
			}
			ItemUIUtilities.OnSelectionChanged -= this.OnItemSelectionChanged;
			this.itemDisplay.onPointerClick -= this.OnItemDisplayClicked;
			this.itemDisplay.onDoubleClicked -= this.OnItemDisplayDoubleClicked;
			IItemDragSource.OnStartDragItem -= this.OnStartDragItem;
			IItemDragSource.OnEndDragItem -= this.OnEndDragItem;
			UIInputManager.OnFastPick -= this.OnFastPick;
			UIInputManager.OnDropItem -= this.OnFastDrop;
			UIInputManager.OnUseItem -= this.OnFastUse;
		}

		// Token: 0x060020C2 RID: 8386 RVA: 0x00072568 File Offset: 0x00070768
		private void OnFastDrop(UIInputEventData data)
		{
			if (!base.isActiveAndEnabled)
			{
				return;
			}
			if (!this.hovering)
			{
				return;
			}
			if (this.Target == null)
			{
				return;
			}
			if (this.Target.Content == null)
			{
				return;
			}
			if (!this.Target.Content.CanDrop)
			{
				return;
			}
			if (this.Editable)
			{
				this.Target.Content.Drop(CharacterMainControl.Main, true);
			}
		}

		// Token: 0x060020C3 RID: 8387 RVA: 0x000725D8 File Offset: 0x000707D8
		private void OnFastUse(UIInputEventData data)
		{
			if (!base.isActiveAndEnabled)
			{
				return;
			}
			if (!this.hovering)
			{
				return;
			}
			if (this.Target == null)
			{
				return;
			}
			if (this.Target.Content == null)
			{
				return;
			}
			if (!this.Target.Content.IsUsable(CharacterMainControl.Main))
			{
				return;
			}
			CharacterMainControl.Main.UseItem(this.Target.Content);
		}

		// Token: 0x060020C4 RID: 8388 RVA: 0x00072641 File Offset: 0x00070841
		private void OnFastPick(UIInputEventData data)
		{
			if (!base.isActiveAndEnabled)
			{
				return;
			}
			if (!this.hovering)
			{
				return;
			}
			this.OnItemDisplayDoubleClicked(this.itemDisplay, new PointerEventData(EventSystem.current));
		}

		// Token: 0x060020C5 RID: 8389 RVA: 0x0007266B File Offset: 0x0007086B
		private void OnEndDragItem(Item item)
		{
			this.pluggableIndicator.Hide();
		}

		// Token: 0x060020C6 RID: 8390 RVA: 0x00072678 File Offset: 0x00070878
		private void OnStartDragItem(Item item)
		{
			if (!base.isActiveAndEnabled)
			{
				return;
			}
			if (!this.Editable)
			{
				return;
			}
			if (item != this.Target.Content && this.Target.CanPlug(item))
			{
				this.pluggableIndicator.Show();
				return;
			}
			this.pluggableIndicator.Hide();
		}

		// Token: 0x060020C7 RID: 8391 RVA: 0x000726CF File Offset: 0x000708CF
		private void OnItemDisplayDoubleClicked(ItemDisplay arg1, PointerEventData arg2)
		{
			Action<SlotDisplay> action = this.onSlotDisplayDoubleClicked;
			if (action != null)
			{
				action(this);
			}
			if (!this.ContentSelectable)
			{
				arg2.Use();
			}
		}

		// Token: 0x060020C8 RID: 8392 RVA: 0x000726F4 File Offset: 0x000708F4
		private void OnItemDisplayClicked(ItemDisplay display, PointerEventData data)
		{
			Action<SlotDisplay> action = this.onSlotDisplayClicked;
			if (action != null)
			{
				action(this);
			}
			if (data.button == PointerEventData.InputButton.Left)
			{
				if (Keyboard.current != null && Keyboard.current.altKey.isPressed)
				{
					if (this.Editable && this.Target.Content != null)
					{
						Item content = this.Target.Content;
						content.Detach();
						if (!ItemUtilities.SendToPlayerCharacterInventory(content, false))
						{
							if (PlayerStorage.IsAccessableAndNotFull())
							{
								ItemUtilities.SendToPlayerStorage(content, false);
							}
							else
							{
								ItemUtilities.SendToPlayer(content, false, false);
							}
						}
						data.Use();
						return;
					}
				}
				else if (!this.ContentSelectable)
				{
					data.Use();
					return;
				}
			}
			else if (data.button == PointerEventData.InputButton.Right && this.Editable)
			{
				Slot target = this.Target;
				if (((target != null) ? target.Content : null) != null)
				{
					ItemOperationMenu.Show(this.itemDisplay);
				}
			}
		}

		// Token: 0x060020C9 RID: 8393 RVA: 0x000727D0 File Offset: 0x000709D0
		private void OnTargetContentChanged(Slot slot)
		{
			this.Refresh();
			this.Punch();
		}

		// Token: 0x060020CA RID: 8394 RVA: 0x000727DE File Offset: 0x000709DE
		private void OnItemSelectionChanged()
		{
		}

		// Token: 0x060020CB RID: 8395 RVA: 0x000727E0 File Offset: 0x000709E0
		public void Setup(Slot target)
		{
			this.UnregisterEvents();
			this.Target = target;
			this.label.text = target.DisplayName;
			this.Refresh();
			this.RegisterEvents();
			this.pluggableIndicator.Hide();
		}

		// Token: 0x060020CC RID: 8396 RVA: 0x00072818 File Offset: 0x00070A18
		private void Refresh()
		{
			if (this.Target.Content == null)
			{
				this.slotIcon.gameObject.SetActive(true);
				if (this.Target.SlotIcon != null)
				{
					this.slotIcon.sprite = this.Target.SlotIcon;
				}
				else
				{
					this.slotIcon.sprite = this.defaultSlotIcon;
				}
			}
			else
			{
				this.slotIcon.gameObject.SetActive(false);
			}
			this.itemDisplay.ShowOperationButtons = this.showOperationMenu;
			this.itemDisplay.Setup(this.Target.Content);
		}

		// Token: 0x1700064F RID: 1615
		// (get) Token: 0x060020CD RID: 8397 RVA: 0x000728BF File Offset: 0x00070ABF
		public static PrefabPool<SlotDisplay> Pool
		{
			get
			{
				return GameplayUIManager.Instance.SlotDisplayPool;
			}
		}

		// Token: 0x17000650 RID: 1616
		// (get) Token: 0x060020CE RID: 8398 RVA: 0x000728CB File Offset: 0x00070ACB
		// (set) Token: 0x060020CF RID: 8399 RVA: 0x000728D8 File Offset: 0x00070AD8
		public bool Movable
		{
			get
			{
				return this.itemDisplay.Movable;
			}
			set
			{
				this.itemDisplay.Movable = value;
			}
		}

		// Token: 0x060020D0 RID: 8400 RVA: 0x000728E6 File Offset: 0x00070AE6
		public static SlotDisplay Get()
		{
			return SlotDisplay.Pool.Get(null);
		}

		// Token: 0x060020D1 RID: 8401 RVA: 0x000728F3 File Offset: 0x00070AF3
		public static void Release(SlotDisplay item)
		{
			SlotDisplay.Pool.Release(item);
		}

		// Token: 0x060020D2 RID: 8402 RVA: 0x00072900 File Offset: 0x00070B00
		public void NotifyPooled()
		{
		}

		// Token: 0x060020D3 RID: 8403 RVA: 0x00072902 File Offset: 0x00070B02
		public void NotifyReleased()
		{
			this.UnregisterEvents();
			this.Target = null;
		}

		// Token: 0x060020D4 RID: 8404 RVA: 0x00072911 File Offset: 0x00070B11
		private void Awake()
		{
			this.itemDisplay.onReceiveDrop += this.OnDrop;
		}

		// Token: 0x060020D5 RID: 8405 RVA: 0x0007292B File Offset: 0x00070B2B
		private void OnEnable()
		{
			this.RegisterEvents();
			this.iconInitialColor = this.slotIcon.color;
			GameObject gameObject = this.hoveringIndicator;
			if (gameObject == null)
			{
				return;
			}
			gameObject.SetActive(false);
		}

		// Token: 0x060020D6 RID: 8406 RVA: 0x00072955 File Offset: 0x00070B55
		private void OnDisable()
		{
			this.UnregisterEvents();
		}

		// Token: 0x060020D7 RID: 8407 RVA: 0x00072960 File Offset: 0x00070B60
		public void OnPointerClick(PointerEventData eventData)
		{
			Action<SlotDisplay> action = this.onSlotDisplayClicked;
			if (action != null)
			{
				action(this);
			}
			if (!this.Editable)
			{
				this.Punch();
				eventData.Use();
				return;
			}
			if (eventData.button == PointerEventData.InputButton.Left)
			{
				Item selectedItem = ItemUIUtilities.SelectedItem;
				if (selectedItem == null)
				{
					this.Punch();
					return;
				}
				if (this.Target.Content != null)
				{
					Debug.Log("槽位 " + this.Target.DisplayName + " 中已经有物品。操作已取消。");
					this.DenialPunch();
					return;
				}
				if (!this.Target.CanPlug(selectedItem))
				{
					Debug.Log(string.Concat(new string[]
					{
						"物品 ",
						selectedItem.DisplayName,
						" 未通过槽位 ",
						this.Target.DisplayName,
						" 安装检测。操作已取消。"
					}));
					this.DenialPunch();
					return;
				}
				eventData.Use();
				selectedItem.Detach();
				Item item;
				this.Target.Plug(selectedItem, out item);
				ItemUIUtilities.NotifyPutItem(selectedItem, false);
				if (item != null)
				{
					ItemUIUtilities.RaiseOrphan(item);
				}
				this.Punch();
			}
		}

		// Token: 0x060020D8 RID: 8408 RVA: 0x00072A7C File Offset: 0x00070C7C
		public void Punch()
		{
			if (this.slotIcon != null)
			{
				this.slotIcon.transform.DOKill(false);
				this.slotIcon.color = this.iconInitialColor;
				this.slotIcon.transform.localScale = Vector3.one;
				this.slotIcon.transform.DOPunchScale(Vector3.one * this.slotIconPunchScale, this.punchDuration, 10, 1f);
			}
			if (this.itemDisplay != null)
			{
				this.itemDisplay.Punch();
			}
		}

		// Token: 0x060020D9 RID: 8409 RVA: 0x00072B18 File Offset: 0x00070D18
		public void DenialPunch()
		{
			if (this.slotIcon == null)
			{
				return;
			}
			this.slotIcon.transform.DOKill(false);
			this.slotIcon.color = this.iconInitialColor;
			this.slotIcon.DOColor(this.slotIconDenialColor, this.denialPunchDuration).From<TweenerCore<Color, Color, ColorOptions>>();
			Action<SlotDisplayOperationContext> action = SlotDisplay.onOperation;
			if (action == null)
			{
				return;
			}
			action(new SlotDisplayOperationContext(this, SlotDisplayOperationContext.Operation.Deny, false));
		}

		// Token: 0x060020DA RID: 8410 RVA: 0x00072B8B File Offset: 0x00070D8B
		public bool IsEditable()
		{
			return this.Editable;
		}

		// Token: 0x060020DB RID: 8411 RVA: 0x00072B93 File Offset: 0x00070D93
		public Item GetItem()
		{
			Slot target = this.Target;
			if (target == null)
			{
				return null;
			}
			return target.Content;
		}

		// Token: 0x060020DC RID: 8412 RVA: 0x00072BA8 File Offset: 0x00070DA8
		public void OnDrop(PointerEventData eventData)
		{
			if (!this.Editable)
			{
				return;
			}
			if (eventData.used)
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
			if (this.SetAmmo(item))
			{
				return;
			}
			if (!this.Target.CanPlug(item))
			{
				Debug.Log(string.Concat(new string[]
				{
					"物品 ",
					item.DisplayName,
					" 未通过槽位 ",
					this.Target.DisplayName,
					" 安装检测。操作已取消。"
				}));
				this.DenialPunch();
				return;
			}
			Inventory inInventory = item.InInventory;
			Slot pluggedIntoSlot = item.PluggedIntoSlot;
			if (pluggedIntoSlot == this.Target)
			{
				return;
			}
			ItemUIUtilities.NotifyPutItem(item, false);
			Item item2;
			bool flag = this.Target.Plug(item, out item2);
			if (item2 != null && (!(inInventory != null) || !inInventory.AddAndMerge(item2, 0)))
			{
				Item item3;
				if (pluggedIntoSlot != null && pluggedIntoSlot.CanPlug(item2) && pluggedIntoSlot.Plug(item2, out item3))
				{
					if (item3)
					{
						Debug.LogError("Source slot spit out an unplugged item! " + item3.DisplayName);
					}
				}
				else if (!ItemUtilities.SendToPlayerCharacter(item2, false))
				{
					LootView lootView = View.ActiveView as LootView;
					if (lootView == null || !(lootView.TargetInventory != null) || !lootView.TargetInventory.AddAndMerge(item2, 0))
					{
						if (PlayerStorage.IsAccessableAndNotFull())
						{
							ItemUtilities.SendToPlayerStorage(item2, false);
						}
						else
						{
							item2.Drop(CharacterMainControl.Main, true);
						}
					}
				}
			}
			Action<SlotDisplayOperationContext> action = SlotDisplay.onOperation;
			if (action == null)
			{
				return;
			}
			action(new SlotDisplayOperationContext(this, SlotDisplayOperationContext.Operation.Equip, flag));
		}

		// Token: 0x060020DD RID: 8413 RVA: 0x00072D5D File Offset: 0x00070F5D
		public void OnDrag(PointerEventData eventData)
		{
		}

		// Token: 0x060020DE RID: 8414 RVA: 0x00072D5F File Offset: 0x00070F5F
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

		// Token: 0x060020DF RID: 8415 RVA: 0x00072D7E File Offset: 0x00070F7E
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

		// Token: 0x060020E0 RID: 8416 RVA: 0x00072D98 File Offset: 0x00070F98
		private bool SetAmmo(Item incomming)
		{
			Slot target = this.Target;
			ItemSetting_Gun itemSetting_Gun;
			if (target == null)
			{
				itemSetting_Gun = null;
			}
			else
			{
				Item content = target.Content;
				itemSetting_Gun = ((content != null) ? content.GetComponent<ItemSetting_Gun>() : null);
			}
			ItemSetting_Gun itemSetting_Gun2 = itemSetting_Gun;
			if (itemSetting_Gun2 == null)
			{
				return false;
			}
			if (!itemSetting_Gun2.IsValidBullet(incomming))
			{
				return false;
			}
			if (View.ActiveView is InventoryView || View.ActiveView is LootView)
			{
				View.ActiveView.Close();
			}
			return itemSetting_Gun2.LoadSpecificBullet(incomming);
		}

		// Token: 0x04001648 RID: 5704
		[SerializeField]
		private Sprite defaultSlotIcon;

		// Token: 0x04001649 RID: 5705
		[SerializeField]
		private TextMeshProUGUI label;

		// Token: 0x0400164A RID: 5706
		[SerializeField]
		private ItemDisplay itemDisplay;

		// Token: 0x0400164B RID: 5707
		[SerializeField]
		private Image slotIcon;

		// Token: 0x0400164C RID: 5708
		[SerializeField]
		private FadeGroup pluggableIndicator;

		// Token: 0x0400164D RID: 5709
		[SerializeField]
		private GameObject hoveringIndicator;

		// Token: 0x0400164E RID: 5710
		[SerializeField]
		private bool editable = true;

		// Token: 0x0400164F RID: 5711
		[SerializeField]
		private bool showOperationMenu = true;

		// Token: 0x04001650 RID: 5712
		[SerializeField]
		private bool contentSelectable = true;

		// Token: 0x04001653 RID: 5715
		[SerializeField]
		[Range(0f, 1f)]
		private float punchDuration = 0.1f;

		// Token: 0x04001654 RID: 5716
		[SerializeField]
		[Range(-1f, 1f)]
		private float slotIconPunchScale = -0.1f;

		// Token: 0x04001655 RID: 5717
		[SerializeField]
		[Range(0f, 1f)]
		private float denialPunchDuration = 0.2f;

		// Token: 0x04001656 RID: 5718
		[SerializeField]
		private Color slotIconDenialColor = Color.red;

		// Token: 0x04001657 RID: 5719
		private Color iconInitialColor;

		// Token: 0x0400165A RID: 5722
		private bool hovering;
	}
}
