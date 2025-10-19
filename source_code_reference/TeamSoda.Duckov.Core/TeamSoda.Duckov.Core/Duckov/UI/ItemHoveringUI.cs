using System;
using Duckov.UI.Animations;
using ItemStatsSystem;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Duckov.UI
{
	// Token: 0x02000378 RID: 888
	public class ItemHoveringUI : MonoBehaviour
	{
		// Token: 0x170005ED RID: 1517
		// (get) Token: 0x06001EAF RID: 7855 RVA: 0x0006BB80 File Offset: 0x00069D80
		// (set) Token: 0x06001EB0 RID: 7856 RVA: 0x0006BB87 File Offset: 0x00069D87
		public static ItemHoveringUI Instance { get; private set; }

		// Token: 0x170005EE RID: 1518
		// (get) Token: 0x06001EB1 RID: 7857 RVA: 0x0006BB8F File Offset: 0x00069D8F
		public RectTransform LayoutParent
		{
			get
			{
				return this.layoutParent;
			}
		}

		// Token: 0x140000D1 RID: 209
		// (add) Token: 0x06001EB2 RID: 7858 RVA: 0x0006BB98 File Offset: 0x00069D98
		// (remove) Token: 0x06001EB3 RID: 7859 RVA: 0x0006BBCC File Offset: 0x00069DCC
		public static event Action<ItemHoveringUI, ItemMetaData> onSetupMeta;

		// Token: 0x140000D2 RID: 210
		// (add) Token: 0x06001EB4 RID: 7860 RVA: 0x0006BC00 File Offset: 0x00069E00
		// (remove) Token: 0x06001EB5 RID: 7861 RVA: 0x0006BC34 File Offset: 0x00069E34
		public static event Action<ItemHoveringUI, Item> onSetupItem;

		// Token: 0x170005EF RID: 1519
		// (get) Token: 0x06001EB6 RID: 7862 RVA: 0x0006BC67 File Offset: 0x00069E67
		// (set) Token: 0x06001EB7 RID: 7863 RVA: 0x0006BC6E File Offset: 0x00069E6E
		public static int DisplayingItemID { get; private set; }

		// Token: 0x170005F0 RID: 1520
		// (get) Token: 0x06001EB8 RID: 7864 RVA: 0x0006BC76 File Offset: 0x00069E76
		public static bool Shown
		{
			get
			{
				return !(ItemHoveringUI.Instance == null) && ItemHoveringUI.Instance.fadeGroup.IsShown;
			}
		}

		// Token: 0x06001EB9 RID: 7865 RVA: 0x0006BC98 File Offset: 0x00069E98
		private void Awake()
		{
			ItemHoveringUI.Instance = this;
			if (this.rectTransform == null)
			{
				this.rectTransform = base.GetComponent<RectTransform>();
			}
			ItemDisplay.OnPointerEnterItemDisplay += this.OnPointerEnterItemDisplay;
			ItemDisplay.OnPointerExitItemDisplay += this.OnPointerExitItemDisplay;
			ItemAmountDisplay.OnMouseEnter += this.OnMouseEnterItemAmountDisplay;
			ItemAmountDisplay.OnMouseExit += this.OnMouseExitItemAmountDisplay;
			ItemMetaDisplay.OnMouseEnter += this.OnMouseEnterMetaDisplay;
			ItemMetaDisplay.OnMouseExit += this.OnMouseExitMetaDisplay;
		}

		// Token: 0x06001EBA RID: 7866 RVA: 0x0006BD2C File Offset: 0x00069F2C
		private void OnDestroy()
		{
			ItemDisplay.OnPointerEnterItemDisplay -= this.OnPointerEnterItemDisplay;
			ItemDisplay.OnPointerExitItemDisplay -= this.OnPointerExitItemDisplay;
			ItemAmountDisplay.OnMouseEnter -= this.OnMouseEnterItemAmountDisplay;
			ItemAmountDisplay.OnMouseExit -= this.OnMouseExitItemAmountDisplay;
			ItemMetaDisplay.OnMouseEnter -= this.OnMouseEnterMetaDisplay;
			ItemMetaDisplay.OnMouseExit -= this.OnMouseExitMetaDisplay;
		}

		// Token: 0x06001EBB RID: 7867 RVA: 0x0006BD9F File Offset: 0x00069F9F
		private void OnMouseExitMetaDisplay(ItemMetaDisplay display)
		{
			if (this.target == display)
			{
				this.Hide();
			}
		}

		// Token: 0x06001EBC RID: 7868 RVA: 0x0006BDB5 File Offset: 0x00069FB5
		private void OnMouseEnterMetaDisplay(ItemMetaDisplay display)
		{
			this.SetupAndShowMeta<ItemMetaDisplay>(display);
		}

		// Token: 0x06001EBD RID: 7869 RVA: 0x0006BDBE File Offset: 0x00069FBE
		private void OnMouseExitItemAmountDisplay(ItemAmountDisplay display)
		{
			if (this.target == display)
			{
				this.Hide();
			}
		}

		// Token: 0x06001EBE RID: 7870 RVA: 0x0006BDD4 File Offset: 0x00069FD4
		private void OnMouseEnterItemAmountDisplay(ItemAmountDisplay display)
		{
			this.SetupAndShowMeta<ItemAmountDisplay>(display);
		}

		// Token: 0x06001EBF RID: 7871 RVA: 0x0006BDDD File Offset: 0x00069FDD
		private void OnPointerExitItemDisplay(ItemDisplay display)
		{
			if (this.target == display)
			{
				this.Hide();
			}
		}

		// Token: 0x06001EC0 RID: 7872 RVA: 0x0006BDF3 File Offset: 0x00069FF3
		private void OnPointerEnterItemDisplay(ItemDisplay display)
		{
			this.SetupAndShow(display);
		}

		// Token: 0x06001EC1 RID: 7873 RVA: 0x0006BDFC File Offset: 0x00069FFC
		private void SetupAndShow(ItemDisplay display)
		{
			if (display == null)
			{
				return;
			}
			Item item = display.Target;
			if (item == null)
			{
				return;
			}
			if (item.NeedInspection)
			{
				return;
			}
			this.registeredIndicator.SetActive(false);
			this.target = display;
			this.itemName.text = item.DisplayName ?? "";
			this.itemDescription.text = item.Description ?? "";
			this.weightDisplay.gameObject.SetActive(true);
			this.weightDisplay.text = string.Format("{0:0.#} kg", item.TotalWeight);
			this.itemID.text = string.Format("#{0}", item.TypeID);
			ItemHoveringUI.DisplayingItemID = item.TypeID;
			this.itemProperties.gameObject.SetActive(true);
			this.itemProperties.Setup(item);
			this.interactionIndicatorsContainer.SetActive(true);
			this.interactionIndicator_Menu.SetActive(display.ShowOperationButtons);
			this.interactionIndicator_Move.SetActive(display.Movable);
			this.interactionIndicator_Drop.SetActive(display.CanDrop);
			this.interactionIndicator_Use.SetActive(display.CanUse);
			this.interactionIndicator_Split.SetActive(display.CanSplit);
			this.interactionIndicator_LockSort.SetActive(display.CanLockSort);
			this.interactionIndicator_Shortcut.SetActive(display.CanSetShortcut);
			this.usageUtilitiesDisplay.Setup(item);
			this.SetupWishlistInfos(item.TypeID);
			this.SetupBulletDisplay();
			try
			{
				Action<ItemHoveringUI, Item> action = ItemHoveringUI.onSetupItem;
				if (action != null)
				{
					action(this, item);
				}
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
			this.RefreshPosition();
			this.SetupRegisteredInfo(item);
			this.fadeGroup.Show();
		}

		// Token: 0x06001EC2 RID: 7874 RVA: 0x0006BFD4 File Offset: 0x0006A1D4
		private void SetupRegisteredInfo(Item item)
		{
			if (item == null)
			{
				return;
			}
			if (item.IsRegistered())
			{
				this.registeredIndicator.SetActive(true);
			}
		}

		// Token: 0x06001EC3 RID: 7875 RVA: 0x0006BFF4 File Offset: 0x0006A1F4
		private void SetupAndShowMeta<T>(T dataProvider) where T : MonoBehaviour, IItemMetaDataProvider
		{
			if (dataProvider == null)
			{
				return;
			}
			this.registeredIndicator.SetActive(false);
			this.target = dataProvider;
			ItemMetaData metaData = dataProvider.GetMetaData();
			this.itemName.text = metaData.DisplayName;
			this.itemID.text = string.Format("{0}", metaData.id);
			ItemHoveringUI.DisplayingItemID = metaData.id;
			this.itemDescription.text = metaData.Description;
			this.interactionIndicatorsContainer.SetActive(true);
			this.weightDisplay.gameObject.SetActive(false);
			this.bulletTypeDisplay.gameObject.SetActive(false);
			this.itemProperties.gameObject.SetActive(false);
			this.interactionIndicator_Menu.gameObject.SetActive(false);
			this.interactionIndicator_Move.gameObject.SetActive(false);
			this.interactionIndicator_Drop.gameObject.SetActive(false);
			this.interactionIndicator_Use.gameObject.SetActive(false);
			this.usageUtilitiesDisplay.gameObject.SetActive(false);
			this.interactionIndicator_Split.SetActive(false);
			this.interactionIndicator_Shortcut.SetActive(false);
			this.SetupWishlistInfos(metaData.id);
			try
			{
				Action<ItemHoveringUI, ItemMetaData> action = ItemHoveringUI.onSetupMeta;
				if (action != null)
				{
					action(this, metaData);
				}
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
			this.RefreshPosition();
			this.fadeGroup.Show();
		}

		// Token: 0x06001EC4 RID: 7876 RVA: 0x0006C178 File Offset: 0x0006A378
		private void SetupBulletDisplay()
		{
			ItemDisplay itemDisplay = this.target as ItemDisplay;
			if (itemDisplay == null)
			{
				return;
			}
			ItemSetting_Gun component = itemDisplay.Target.GetComponent<ItemSetting_Gun>();
			if (component == null)
			{
				this.bulletTypeDisplay.gameObject.SetActive(false);
				return;
			}
			this.bulletTypeDisplay.gameObject.SetActive(true);
			this.bulletTypeDisplay.Setup(component.TargetBulletID);
		}

		// Token: 0x06001EC5 RID: 7877 RVA: 0x0006C1E4 File Offset: 0x0006A3E4
		private unsafe void RefreshPosition()
		{
			Vector2 vector = *Mouse.current.position.value;
			Vector2 vector2;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(this.rectTransform, vector, null, out vector2);
			float xMax = this.contents.rect.xMax;
			float yMin = this.contents.rect.yMin;
			float num = this.rectTransform.rect.xMax - xMax;
			float num2 = this.rectTransform.rect.yMin - yMin;
			vector2.x = Mathf.Min(vector2.x, num);
			vector2.y = Mathf.Max(vector2.y, num2);
			this.contents.anchoredPosition = vector2;
		}

		// Token: 0x06001EC6 RID: 7878 RVA: 0x0006C2A4 File Offset: 0x0006A4A4
		private void Hide()
		{
			this.fadeGroup.Hide();
			ItemHoveringUI.DisplayingItemID = -1;
		}

		// Token: 0x06001EC7 RID: 7879 RVA: 0x0006C2B8 File Offset: 0x0006A4B8
		private void Update()
		{
			if (this.fadeGroup.IsShown)
			{
				if (this.target == null || !this.target.isActiveAndEnabled)
				{
					this.Hide();
				}
				ItemDisplay itemDisplay = this.target as ItemDisplay;
				if (itemDisplay != null && itemDisplay.Target == null)
				{
					this.Hide();
				}
			}
			this.RefreshPosition();
		}

		// Token: 0x06001EC8 RID: 7880 RVA: 0x0006C31C File Offset: 0x0006A51C
		private void SetupWishlistInfos(int itemTypeID)
		{
			ItemWishlist.WishlistInfo wishlistInfo = ItemWishlist.GetWishlistInfo(itemTypeID);
			bool isManuallyWishlisted = wishlistInfo.isManuallyWishlisted;
			bool isBuildingRequired = wishlistInfo.isBuildingRequired;
			bool isQuestRequired = wishlistInfo.isQuestRequired;
			bool flag = isManuallyWishlisted || isBuildingRequired || isQuestRequired;
			this.wishlistIndicator.SetActive(isManuallyWishlisted);
			this.buildingIndicator.SetActive(isBuildingRequired);
			this.questIndicator.SetActive(isQuestRequired);
			this.wishlistInfoParent.SetActive(flag);
		}

		// Token: 0x06001EC9 RID: 7881 RVA: 0x0006C379 File Offset: 0x0006A579
		internal static void NotifyRefreshWishlistInfo()
		{
			if (ItemHoveringUI.Instance == null)
			{
				return;
			}
			ItemHoveringUI.Instance.SetupWishlistInfos(ItemHoveringUI.DisplayingItemID);
		}

		// Token: 0x04001500 RID: 5376
		[SerializeField]
		private RectTransform rectTransform;

		// Token: 0x04001501 RID: 5377
		[SerializeField]
		private RectTransform layoutParent;

		// Token: 0x04001502 RID: 5378
		[SerializeField]
		private RectTransform contents;

		// Token: 0x04001503 RID: 5379
		[SerializeField]
		private FadeGroup fadeGroup;

		// Token: 0x04001504 RID: 5380
		[SerializeField]
		private TextMeshProUGUI itemName;

		// Token: 0x04001505 RID: 5381
		[SerializeField]
		private TextMeshProUGUI weightDisplay;

		// Token: 0x04001506 RID: 5382
		[SerializeField]
		private TextMeshProUGUI itemDescription;

		// Token: 0x04001507 RID: 5383
		[SerializeField]
		private TextMeshProUGUI itemID;

		// Token: 0x04001508 RID: 5384
		[SerializeField]
		private ItemPropertiesDisplay itemProperties;

		// Token: 0x04001509 RID: 5385
		[SerializeField]
		private BulletTypeDisplay bulletTypeDisplay;

		// Token: 0x0400150A RID: 5386
		[SerializeField]
		private UsageUtilitiesDisplay usageUtilitiesDisplay;

		// Token: 0x0400150B RID: 5387
		[SerializeField]
		private GameObject interactionIndicatorsContainer;

		// Token: 0x0400150C RID: 5388
		[SerializeField]
		private GameObject interactionIndicator_Move;

		// Token: 0x0400150D RID: 5389
		[SerializeField]
		private GameObject interactionIndicator_Menu;

		// Token: 0x0400150E RID: 5390
		[SerializeField]
		private GameObject interactionIndicator_Drop;

		// Token: 0x0400150F RID: 5391
		[SerializeField]
		private GameObject interactionIndicator_Use;

		// Token: 0x04001510 RID: 5392
		[SerializeField]
		private GameObject interactionIndicator_Split;

		// Token: 0x04001511 RID: 5393
		[SerializeField]
		private GameObject interactionIndicator_LockSort;

		// Token: 0x04001512 RID: 5394
		[SerializeField]
		private GameObject interactionIndicator_Shortcut;

		// Token: 0x04001513 RID: 5395
		[SerializeField]
		private GameObject wishlistInfoParent;

		// Token: 0x04001514 RID: 5396
		[SerializeField]
		private GameObject wishlistIndicator;

		// Token: 0x04001515 RID: 5397
		[SerializeField]
		private GameObject buildingIndicator;

		// Token: 0x04001516 RID: 5398
		[SerializeField]
		private GameObject questIndicator;

		// Token: 0x04001517 RID: 5399
		[SerializeField]
		private GameObject registeredIndicator;

		// Token: 0x0400151B RID: 5403
		private MonoBehaviour target;
	}
}
