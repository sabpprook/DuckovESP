using System;
using DG.Tweening;
using Duckov.Utilities;
using ItemStatsSystem;
using ItemStatsSystem.Items;
using LeTai.TrueShadow;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Duckov.UI
{
	// Token: 0x02000396 RID: 918
	public class ItemDisplay : MonoBehaviour, IPoolable, IPointerClickHandler, IEventSystemHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IDropHandler
	{
		// Token: 0x1700062E RID: 1582
		// (get) Token: 0x0600203A RID: 8250 RVA: 0x000707B2 File Offset: 0x0006E9B2
		private Sprite FallbackIcon
		{
			get
			{
				return GameplayDataSettings.UIStyle.FallbackItemIcon;
			}
		}

		// Token: 0x1700062F RID: 1583
		// (get) Token: 0x0600203B RID: 8251 RVA: 0x000707BE File Offset: 0x0006E9BE
		// (set) Token: 0x0600203C RID: 8252 RVA: 0x000707C6 File Offset: 0x0006E9C6
		public Item Target { get; private set; }

		// Token: 0x17000630 RID: 1584
		// (get) Token: 0x0600203D RID: 8253 RVA: 0x000707CF File Offset: 0x0006E9CF
		// (set) Token: 0x0600203E RID: 8254 RVA: 0x000707D7 File Offset: 0x0006E9D7
		internal Action releaseAction { get; set; }

		// Token: 0x140000D6 RID: 214
		// (add) Token: 0x0600203F RID: 8255 RVA: 0x000707E0 File Offset: 0x0006E9E0
		// (remove) Token: 0x06002040 RID: 8256 RVA: 0x00070818 File Offset: 0x0006EA18
		internal event Action<ItemDisplay, PointerEventData> onDoubleClicked;

		// Token: 0x140000D7 RID: 215
		// (add) Token: 0x06002041 RID: 8257 RVA: 0x00070850 File Offset: 0x0006EA50
		// (remove) Token: 0x06002042 RID: 8258 RVA: 0x00070888 File Offset: 0x0006EA88
		public event Action<PointerEventData> onReceiveDrop;

		// Token: 0x17000631 RID: 1585
		// (get) Token: 0x06002043 RID: 8259 RVA: 0x000708BD File Offset: 0x0006EABD
		public bool Selected
		{
			get
			{
				return ItemUIUtilities.SelectedItemDisplay == this;
			}
		}

		// Token: 0x17000632 RID: 1586
		// (get) Token: 0x06002044 RID: 8260 RVA: 0x000708CC File Offset: 0x0006EACC
		private PrefabPool<SlotIndicator> SlotIndicatorPool
		{
			get
			{
				if (this._slotIndicatorPool == null)
				{
					if (this.slotIndicatorTemplate == null)
					{
						Debug.LogError("SI is null", base.gameObject);
					}
					this._slotIndicatorPool = new PrefabPool<SlotIndicator>(this.slotIndicatorTemplate, null, null, null, null, true, 10, 10000, null);
				}
				return this._slotIndicatorPool;
			}
		}

		// Token: 0x140000D8 RID: 216
		// (add) Token: 0x06002045 RID: 8261 RVA: 0x00070924 File Offset: 0x0006EB24
		// (remove) Token: 0x06002046 RID: 8262 RVA: 0x00070958 File Offset: 0x0006EB58
		public static event Action<ItemDisplay> OnPointerEnterItemDisplay;

		// Token: 0x140000D9 RID: 217
		// (add) Token: 0x06002047 RID: 8263 RVA: 0x0007098C File Offset: 0x0006EB8C
		// (remove) Token: 0x06002048 RID: 8264 RVA: 0x000709C0 File Offset: 0x0006EBC0
		public static event Action<ItemDisplay> OnPointerExitItemDisplay;

		// Token: 0x06002049 RID: 8265 RVA: 0x000709F4 File Offset: 0x0006EBF4
		public void Setup(Item target)
		{
			this.UnregisterEvents();
			this.Target = target;
			this.Clear();
			this.slotIndicatorTemplate.gameObject.SetActive(false);
			if (target == null)
			{
				this.SetupEmpty();
			}
			else
			{
				this.icon.color = Color.white;
				this.icon.sprite = target.Icon;
				if (this.icon.sprite == null)
				{
					this.icon.sprite = this.FallbackIcon;
				}
				this.icon.gameObject.SetActive(true);
				ValueTuple<float, Color, bool> shadowOffsetAndColorOfQuality = GameplayDataSettings.UIStyle.GetShadowOffsetAndColorOfQuality(target.DisplayQuality);
				this.displayQualityShadow.OffsetDistance = shadowOffsetAndColorOfQuality.Item1;
				this.displayQualityShadow.Color = shadowOffsetAndColorOfQuality.Item2;
				this.displayQualityShadow.Inset = shadowOffsetAndColorOfQuality.Item3;
				bool stackable = this.Target.Stackable;
				this.countGameObject.SetActive(stackable);
				this.nameText.text = this.Target.DisplayName;
				if (target.Slots != null)
				{
					foreach (Slot slot in target.Slots)
					{
						this.SlotIndicatorPool.Get(null).Setup(slot);
					}
				}
			}
			this.Refresh();
			if (base.isActiveAndEnabled)
			{
				this.RegisterEvents();
			}
		}

		// Token: 0x140000DA RID: 218
		// (add) Token: 0x0600204A RID: 8266 RVA: 0x00070B70 File Offset: 0x0006ED70
		// (remove) Token: 0x0600204B RID: 8267 RVA: 0x00070BA8 File Offset: 0x0006EDA8
		public event Action<ItemDisplay, PointerEventData> onPointerClick;

		// Token: 0x0600204C RID: 8268 RVA: 0x00070BE0 File Offset: 0x0006EDE0
		private void RegisterEvents()
		{
			this.UnregisterEvents();
			ItemUIUtilities.OnSelectionChanged += this.OnItemUtilitiesSelectionChanged;
			ItemWishlist.OnWishlistChanged += this.OnWishlistChanged;
			if (this.Target == null)
			{
				return;
			}
			this.Target.onDestroy += this.OnTargetDestroy;
			this.Target.onSetStackCount += this.OnTargetSetStackCount;
			this.Target.onInspectionStateChanged += this.OnTargetInspectionStateChanged;
			this.Target.onDurabilityChanged += this.OnTargetDurabilityChanged;
		}

		// Token: 0x0600204D RID: 8269 RVA: 0x00070C80 File Offset: 0x0006EE80
		private void UnregisterEvents()
		{
			ItemUIUtilities.OnSelectionChanged -= this.OnItemUtilitiesSelectionChanged;
			ItemWishlist.OnWishlistChanged -= this.OnWishlistChanged;
			if (this.Target == null)
			{
				return;
			}
			this.Target.onDestroy -= this.OnTargetDestroy;
			this.Target.onSetStackCount -= this.OnTargetSetStackCount;
			this.Target.onInspectionStateChanged -= this.OnTargetInspectionStateChanged;
			this.Target.onDurabilityChanged -= this.OnTargetDurabilityChanged;
		}

		// Token: 0x0600204E RID: 8270 RVA: 0x00070D1A File Offset: 0x0006EF1A
		private void OnWishlistChanged(int type)
		{
			if (this.Target == null)
			{
				return;
			}
			if (this.Target.TypeID == type)
			{
				this.RefreshWishlistInfo();
			}
		}

		// Token: 0x0600204F RID: 8271 RVA: 0x00070D3F File Offset: 0x0006EF3F
		private void OnTargetDurabilityChanged(Item item)
		{
			this.Refresh();
		}

		// Token: 0x06002050 RID: 8272 RVA: 0x00070D47 File Offset: 0x0006EF47
		private void OnTargetDestroy(Item item)
		{
		}

		// Token: 0x06002051 RID: 8273 RVA: 0x00070D49 File Offset: 0x0006EF49
		private void OnTargetSetStackCount(Item item)
		{
			if (item != this.Target)
			{
				Debug.LogError("触发事件的Item不匹配!");
			}
			this.Refresh();
		}

		// Token: 0x06002052 RID: 8274 RVA: 0x00070D69 File Offset: 0x0006EF69
		private void OnItemUtilitiesSelectionChanged()
		{
			this.Refresh();
		}

		// Token: 0x06002053 RID: 8275 RVA: 0x00070D71 File Offset: 0x0006EF71
		private void OnTargetInspectionStateChanged(Item item)
		{
			this.Refresh();
			this.Punch();
		}

		// Token: 0x06002054 RID: 8276 RVA: 0x00070D7F File Offset: 0x0006EF7F
		private void Clear()
		{
			this.SlotIndicatorPool.ReleaseAll();
		}

		// Token: 0x06002055 RID: 8277 RVA: 0x00070D8C File Offset: 0x0006EF8C
		private void SetupEmpty()
		{
			this.icon.sprite = EmptySprite.Get();
			this.icon.color = Color.clear;
			this.countText.text = string.Empty;
			this.nameText.text = string.Empty;
			this.durabilityFill.fillAmount = 0f;
			this.durabilityLoss.fillAmount = 0f;
			this.durabilityZeroIndicator.gameObject.SetActive(false);
		}

		// Token: 0x06002056 RID: 8278 RVA: 0x00070E0C File Offset: 0x0006F00C
		private void Refresh()
		{
			if (this == null)
			{
				Debug.Log("NULL");
				return;
			}
			if (this.isBeingDestroyed)
			{
				return;
			}
			if (this.Target == null)
			{
				this.HideMainContentAndDisableControl();
				this.HideInspectionElements();
				if (ItemUIUtilities.SelectedItemDisplayRaw == this)
				{
					ItemUIUtilities.Select(null);
				}
			}
			else if (this.Target.NeedInspection)
			{
				this.HideMainContentAndDisableControl();
				this.ShowInspectionElements();
			}
			else
			{
				this.HideInspectionElements();
				this.ShowMainContentAndEnableControl();
			}
			this.selectionIndicator.gameObject.SetActive(this.Selected);
			this.RefreshWishlistInfo();
		}

		// Token: 0x06002057 RID: 8279 RVA: 0x00070EA8 File Offset: 0x0006F0A8
		private void RefreshWishlistInfo()
		{
			if (this.Target == null || this.Target.NeedInspection)
			{
				this.wishlistedIndicator.SetActive(false);
				this.questRequiredIndicator.SetActive(false);
				this.buildingRequiredIndicator.SetActive(false);
				return;
			}
			ItemWishlist.WishlistInfo wishlistInfo = ItemWishlist.GetWishlistInfo(this.Target.TypeID);
			this.wishlistedIndicator.SetActive(wishlistInfo.isManuallyWishlisted);
			this.questRequiredIndicator.SetActive(wishlistInfo.isQuestRequired);
			this.buildingRequiredIndicator.SetActive(wishlistInfo.isBuildingRequired);
		}

		// Token: 0x06002058 RID: 8280 RVA: 0x00070F3C File Offset: 0x0006F13C
		private void HideMainContentAndDisableControl()
		{
			this.mainContentShown = false;
			if (this.mainContentShown && ItemUIUtilities.SelectedItemDisplay == this)
			{
				ItemUIUtilities.Select(null);
			}
			this.interactionEventReceiver.raycastTarget = false;
			this.icon.gameObject.SetActive(false);
			this.countGameObject.SetActive(false);
			this.durabilityGameObject.SetActive(false);
			this.durabilityZeroIndicator.gameObject.SetActive(false);
			this.nameContainer.SetActive(false);
			this.slotIndicatorContainer.SetActive(false);
		}

		// Token: 0x06002059 RID: 8281 RVA: 0x00070FCC File Offset: 0x0006F1CC
		private void ShowMainContentAndEnableControl()
		{
			this.mainContentShown = true;
			this.interactionEventReceiver.raycastTarget = true;
			this.icon.gameObject.SetActive(true);
			this.nameContainer.SetActive(true);
			this.countText.text = (this.Target.Stackable ? this.Target.StackCount.ToString() : string.Empty);
			bool useDurability = this.Target.UseDurability;
			if (useDurability)
			{
				float num = this.Target.Durability / this.Target.MaxDurability;
				this.durabilityFill.fillAmount = num;
				this.durabilityFill.color = this.durabilityFillColorOverT.Evaluate(num);
				this.durabilityZeroIndicator.SetActive(this.Target.Durability <= 0f);
				this.durabilityLoss.fillAmount = this.Target.DurabilityLoss;
			}
			else
			{
				this.durabilityZeroIndicator.gameObject.SetActive(false);
			}
			this.countGameObject.SetActive(this.Target.Stackable);
			this.durabilityGameObject.SetActive(useDurability);
			this.slotIndicatorContainer.SetActive(true);
		}

		// Token: 0x0600205A RID: 8282 RVA: 0x000710FC File Offset: 0x0006F2FC
		private void ShowInspectionElements()
		{
			this.inspectionElementRoot.gameObject.SetActive(true);
			bool inspecting = this.Target.Inspecting;
			if (this.inspectingElement)
			{
				this.inspectingElement.SetActive(inspecting);
			}
			if (this.notInspectingElement)
			{
				this.notInspectingElement.SetActive(!inspecting);
			}
		}

		// Token: 0x0600205B RID: 8283 RVA: 0x0007115B File Offset: 0x0006F35B
		private void HideInspectionElements()
		{
			this.inspectionElementRoot.gameObject.SetActive(false);
		}

		// Token: 0x0600205C RID: 8284 RVA: 0x0007116E File Offset: 0x0006F36E
		private void OnEnable()
		{
			this.RegisterEvents();
		}

		// Token: 0x0600205D RID: 8285 RVA: 0x00071176 File Offset: 0x0006F376
		private void OnDisable()
		{
			ItemUIUtilities.OnSelectionChanged -= this.OnItemUtilitiesSelectionChanged;
			if (this.Selected)
			{
				ItemUIUtilities.Select(null);
			}
			this.UnregisterEvents();
		}

		// Token: 0x0600205E RID: 8286 RVA: 0x0007119D File Offset: 0x0006F39D
		private void OnDestroy()
		{
			this.UnregisterEvents();
			ItemUIUtilities.OnSelectionChanged -= this.OnItemUtilitiesSelectionChanged;
			this.isBeingDestroyed = true;
		}

		// Token: 0x17000633 RID: 1587
		// (get) Token: 0x0600205F RID: 8287 RVA: 0x000711BD File Offset: 0x0006F3BD
		public static PrefabPool<ItemDisplay> Pool
		{
			get
			{
				return GameplayUIManager.Instance.ItemDisplayPool;
			}
		}

		// Token: 0x17000634 RID: 1588
		// (get) Token: 0x06002060 RID: 8288 RVA: 0x000711C9 File Offset: 0x0006F3C9
		// (set) Token: 0x06002061 RID: 8289 RVA: 0x000711D1 File Offset: 0x0006F3D1
		public bool ShowOperationButtons
		{
			get
			{
				return this.showOperationButtons;
			}
			internal set
			{
				this.showOperationButtons = value;
			}
		}

		// Token: 0x17000635 RID: 1589
		// (get) Token: 0x06002062 RID: 8290 RVA: 0x000711DA File Offset: 0x0006F3DA
		// (set) Token: 0x06002063 RID: 8291 RVA: 0x000711E2 File Offset: 0x0006F3E2
		public bool Editable { get; set; }

		// Token: 0x17000636 RID: 1590
		// (get) Token: 0x06002064 RID: 8292 RVA: 0x000711EB File Offset: 0x0006F3EB
		// (set) Token: 0x06002065 RID: 8293 RVA: 0x000711F3 File Offset: 0x0006F3F3
		public bool Movable { get; set; }

		// Token: 0x17000637 RID: 1591
		// (get) Token: 0x06002066 RID: 8294 RVA: 0x000711FC File Offset: 0x0006F3FC
		// (set) Token: 0x06002067 RID: 8295 RVA: 0x00071204 File Offset: 0x0006F404
		public bool CanDrop { get; set; }

		// Token: 0x17000638 RID: 1592
		// (get) Token: 0x06002068 RID: 8296 RVA: 0x0007120D File Offset: 0x0006F40D
		// (set) Token: 0x06002069 RID: 8297 RVA: 0x00071215 File Offset: 0x0006F415
		public bool IsStockshopSample { get; set; }

		// Token: 0x17000639 RID: 1593
		// (get) Token: 0x0600206A RID: 8298 RVA: 0x0007121E File Offset: 0x0006F41E
		public bool CanUse
		{
			get
			{
				return !(this.Target == null) && this.Editable && this.Target.IsUsable(CharacterMainControl.Main);
			}
		}

		// Token: 0x1700063A RID: 1594
		// (get) Token: 0x0600206B RID: 8299 RVA: 0x0007124F File Offset: 0x0006F44F
		public bool CanSplit
		{
			get
			{
				return !(this.Target == null) && this.Editable && (this.Movable && this.Target.StackCount > 1);
			}
		}

		// Token: 0x1700063B RID: 1595
		// (get) Token: 0x0600206C RID: 8300 RVA: 0x00071284 File Offset: 0x0006F484
		// (set) Token: 0x0600206D RID: 8301 RVA: 0x0007128C File Offset: 0x0006F48C
		public bool CanLockSort { get; internal set; }

		// Token: 0x1700063C RID: 1596
		// (get) Token: 0x0600206E RID: 8302 RVA: 0x00071295 File Offset: 0x0006F495
		public bool CanSetShortcut
		{
			get
			{
				return !(this.Target == null) && this.showOperationButtons && ItemShortcut.IsItemValid(this.Target);
			}
		}

		// Token: 0x0600206F RID: 8303 RVA: 0x000712C1 File Offset: 0x0006F4C1
		public static ItemDisplay Get()
		{
			return ItemDisplay.Pool.Get(null);
		}

		// Token: 0x06002070 RID: 8304 RVA: 0x000712CE File Offset: 0x0006F4CE
		public static void Release(ItemDisplay item)
		{
			ItemDisplay.Pool.Release(item);
		}

		// Token: 0x06002071 RID: 8305 RVA: 0x000712DB File Offset: 0x0006F4DB
		public void NotifyPooled()
		{
		}

		// Token: 0x06002072 RID: 8306 RVA: 0x000712DD File Offset: 0x0006F4DD
		public void NotifyReleased()
		{
			this.UnregisterEvents();
			this.Target = null;
			this.SetupEmpty();
		}

		// Token: 0x06002073 RID: 8307 RVA: 0x000712F2 File Offset: 0x0006F4F2
		[ContextMenu("Select")]
		private void Select()
		{
			ItemUIUtilities.Select(this);
		}

		// Token: 0x06002074 RID: 8308 RVA: 0x000712FA File Offset: 0x0006F4FA
		public void NotifySelected()
		{
		}

		// Token: 0x06002075 RID: 8309 RVA: 0x000712FC File Offset: 0x0006F4FC
		public void NotifyUnselected()
		{
			KontextMenu.Hide(this);
		}

		// Token: 0x06002076 RID: 8310 RVA: 0x00071304 File Offset: 0x0006F504
		public void OnPointerClick(PointerEventData eventData)
		{
			Action<ItemDisplay, PointerEventData> action = this.onPointerClick;
			if (action != null)
			{
				action(this, eventData);
			}
			if (!eventData.used && eventData.button == PointerEventData.InputButton.Left)
			{
				if (eventData.clickTime - this.lastClickTime <= 0.3f && !this.doubleClickInvoked)
				{
					this.doubleClickInvoked = true;
					Action<ItemDisplay, PointerEventData> action2 = this.onDoubleClicked;
					if (action2 != null)
					{
						action2(this, eventData);
					}
				}
				if (!eventData.used && (!this.Target || !this.Target.NeedInspection))
				{
					if (ItemUIUtilities.SelectedItemDisplay != this)
					{
						this.Select();
						eventData.Use();
					}
					else
					{
						ItemUIUtilities.Select(null);
						eventData.Use();
					}
				}
			}
			if (eventData.clickTime - this.lastClickTime > 0.3f)
			{
				this.doubleClickInvoked = false;
			}
			this.lastClickTime = eventData.clickTime;
			this.Punch();
		}

		// Token: 0x06002077 RID: 8311 RVA: 0x000713E4 File Offset: 0x0006F5E4
		public void Punch()
		{
			this.selectionIndicator.transform.DOKill(false);
			this.icon.transform.DOKill(false);
			this.backgroundRing.transform.DOKill(false);
			this.selectionIndicator.transform.localScale = Vector3.one;
			this.icon.transform.localScale = Vector3.one;
			this.backgroundRing.transform.localScale = Vector3.one;
			this.selectionIndicator.transform.DOPunchScale(Vector3.one * this.selectionRingPunchScale, this.punchDuration, 10, 1f);
			this.icon.transform.DOPunchScale(Vector3.one * this.iconPunchScale, this.punchDuration, 10, 1f);
			this.backgroundRing.transform.DOPunchScale(Vector3.one * this.backgroundRingPunchScale, this.punchDuration, 10, 1f);
		}

		// Token: 0x06002078 RID: 8312 RVA: 0x000714F0 File Offset: 0x0006F6F0
		public void OnPointerDown(PointerEventData eventData)
		{
		}

		// Token: 0x06002079 RID: 8313 RVA: 0x000714F2 File Offset: 0x0006F6F2
		public void OnPointerUp(PointerEventData eventData)
		{
		}

		// Token: 0x0600207A RID: 8314 RVA: 0x000714F4 File Offset: 0x0006F6F4
		public void OnPointerExit(PointerEventData eventData)
		{
			if (this.Target == null)
			{
				return;
			}
			Action<ItemDisplay> onPointerExitItemDisplay = ItemDisplay.OnPointerExitItemDisplay;
			if (onPointerExitItemDisplay == null)
			{
				return;
			}
			onPointerExitItemDisplay(this);
		}

		// Token: 0x0600207B RID: 8315 RVA: 0x00071515 File Offset: 0x0006F715
		public void OnPointerEnter(PointerEventData eventData)
		{
			if (this.Target == null)
			{
				return;
			}
			Action<ItemDisplay> onPointerEnterItemDisplay = ItemDisplay.OnPointerEnterItemDisplay;
			if (onPointerEnterItemDisplay == null)
			{
				return;
			}
			onPointerEnterItemDisplay(this);
		}

		// Token: 0x0600207C RID: 8316 RVA: 0x00071536 File Offset: 0x0006F736
		public void OnDrop(PointerEventData eventData)
		{
			this.HandleDirectDrop(eventData);
			if (eventData.used)
			{
				return;
			}
			Action<PointerEventData> action = this.onReceiveDrop;
			if (action == null)
			{
				return;
			}
			action(eventData);
		}

		// Token: 0x0600207D RID: 8317 RVA: 0x0007155C File Offset: 0x0006F75C
		private void HandleDirectDrop(PointerEventData eventData)
		{
			if (this.Target == null)
			{
				return;
			}
			if (eventData.button != PointerEventData.InputButton.Left)
			{
				return;
			}
			if (this.IsStockshopSample)
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
			if (!this.Target.TryPlug(item, false, null, 0))
			{
				return;
			}
			ItemUIUtilities.NotifyPutItem(item, false);
			eventData.Use();
		}

		// Token: 0x040015FA RID: 5626
		[SerializeField]
		private Image icon;

		// Token: 0x040015FB RID: 5627
		[SerializeField]
		private TrueShadow displayQualityShadow;

		// Token: 0x040015FC RID: 5628
		[SerializeField]
		private GameObject countGameObject;

		// Token: 0x040015FD RID: 5629
		[SerializeField]
		private TextMeshProUGUI countText;

		// Token: 0x040015FE RID: 5630
		[SerializeField]
		private GameObject selectionIndicator;

		// Token: 0x040015FF RID: 5631
		[SerializeField]
		private Graphic interactionEventReceiver;

		// Token: 0x04001600 RID: 5632
		[SerializeField]
		private GameObject backgroundRing;

		// Token: 0x04001601 RID: 5633
		[SerializeField]
		private GameObject inspectionElementRoot;

		// Token: 0x04001602 RID: 5634
		[SerializeField]
		private GameObject inspectingElement;

		// Token: 0x04001603 RID: 5635
		[SerializeField]
		private GameObject notInspectingElement;

		// Token: 0x04001604 RID: 5636
		[SerializeField]
		private GameObject nameContainer;

		// Token: 0x04001605 RID: 5637
		[SerializeField]
		private TextMeshProUGUI nameText;

		// Token: 0x04001606 RID: 5638
		[SerializeField]
		private GameObject durabilityGameObject;

		// Token: 0x04001607 RID: 5639
		[SerializeField]
		private Image durabilityFill;

		// Token: 0x04001608 RID: 5640
		[SerializeField]
		private Gradient durabilityFillColorOverT;

		// Token: 0x04001609 RID: 5641
		[SerializeField]
		private GameObject durabilityZeroIndicator;

		// Token: 0x0400160A RID: 5642
		[SerializeField]
		private Image durabilityLoss;

		// Token: 0x0400160B RID: 5643
		[SerializeField]
		private GameObject slotIndicatorContainer;

		// Token: 0x0400160C RID: 5644
		[SerializeField]
		private SlotIndicator slotIndicatorTemplate;

		// Token: 0x0400160D RID: 5645
		[SerializeField]
		private GameObject wishlistedIndicator;

		// Token: 0x0400160E RID: 5646
		[SerializeField]
		private GameObject questRequiredIndicator;

		// Token: 0x0400160F RID: 5647
		[SerializeField]
		private GameObject buildingRequiredIndicator;

		// Token: 0x04001610 RID: 5648
		[SerializeField]
		[Range(0f, 1f)]
		private float punchDuration = 0.2f;

		// Token: 0x04001611 RID: 5649
		[SerializeField]
		[Range(-1f, 1f)]
		private float selectionRingPunchScale = 0.1f;

		// Token: 0x04001612 RID: 5650
		[SerializeField]
		[Range(-1f, 1f)]
		private float backgroundRingPunchScale = 0.2f;

		// Token: 0x04001613 RID: 5651
		[SerializeField]
		[Range(-1f, 1f)]
		private float iconPunchScale = 0.1f;

		// Token: 0x04001618 RID: 5656
		public const float doubleClickTimeThreshold = 0.3f;

		// Token: 0x04001619 RID: 5657
		private PrefabPool<SlotIndicator> _slotIndicatorPool;

		// Token: 0x0400161D RID: 5661
		private bool mainContentShown = true;

		// Token: 0x0400161E RID: 5662
		private bool isBeingDestroyed;

		// Token: 0x0400161F RID: 5663
		[SerializeField]
		private bool showOperationButtons = true;

		// Token: 0x04001625 RID: 5669
		private float lastClickTime;

		// Token: 0x04001626 RID: 5670
		private bool doubleClickInvoked;
	}
}
