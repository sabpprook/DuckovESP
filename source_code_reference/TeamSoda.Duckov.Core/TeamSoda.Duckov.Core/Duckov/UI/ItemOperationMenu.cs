using System;
using System.Collections.Generic;
using System.Linq;
using Duckov.UI.Animations;
using ItemStatsSystem;
using ItemStatsSystem.Items;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Duckov.UI
{
	// Token: 0x02000397 RID: 919
	public class ItemOperationMenu : ManagedUIElement
	{
		// Token: 0x1700063D RID: 1597
		// (get) Token: 0x0600207F RID: 8319 RVA: 0x0007161D File Offset: 0x0006F81D
		// (set) Token: 0x06002080 RID: 8320 RVA: 0x00071624 File Offset: 0x0006F824
		public static ItemOperationMenu Instance { get; private set; }

		// Token: 0x1700063E RID: 1598
		// (get) Token: 0x06002081 RID: 8321 RVA: 0x0007162C File Offset: 0x0006F82C
		private Item TargetItem
		{
			get
			{
				ItemDisplay targetDisplay = this.TargetDisplay;
				if (targetDisplay == null)
				{
					return null;
				}
				return targetDisplay.Target;
			}
		}

		// Token: 0x06002082 RID: 8322 RVA: 0x0007163F File Offset: 0x0006F83F
		protected override void Awake()
		{
			base.Awake();
			ItemOperationMenu.Instance = this;
			if (this.rectTransform == null)
			{
				this.rectTransform = base.GetComponent<RectTransform>();
			}
			this.Initialize();
		}

		// Token: 0x06002083 RID: 8323 RVA: 0x0007166D File Offset: 0x0006F86D
		protected override void OnDestroy()
		{
			base.OnDestroy();
		}

		// Token: 0x06002084 RID: 8324 RVA: 0x00071678 File Offset: 0x0006F878
		private void Update()
		{
			if (this.fadeGroup.IsHidingInProgress)
			{
				return;
			}
			if (!this.fadeGroup.IsShown)
			{
				return;
			}
			if (!Mouse.current.leftButton.wasReleasedThisFrame && !(this.targetView == null) && this.targetView.open)
			{
				if (this.fadeGroup.IsShowingInProgress)
				{
					return;
				}
				if (!Mouse.current.rightButton.wasReleasedThisFrame)
				{
					return;
				}
			}
			base.Close();
		}

		// Token: 0x06002085 RID: 8325 RVA: 0x000716F4 File Offset: 0x0006F8F4
		private void Initialize()
		{
			this.btn_Use.onClick.AddListener(new UnityAction(this.Use));
			this.btn_Split.onClick.AddListener(new UnityAction(this.Split));
			this.btn_Dump.onClick.AddListener(new UnityAction(this.Dump));
			this.btn_Equip.onClick.AddListener(new UnityAction(this.Equip));
			this.btn_Modify.onClick.AddListener(new UnityAction(this.Modify));
			this.btn_Unload.onClick.AddListener(new UnityAction(this.Unload));
			this.btn_Wishlist.onClick.AddListener(new UnityAction(this.Wishlist));
		}

		// Token: 0x06002086 RID: 8326 RVA: 0x000717C8 File Offset: 0x0006F9C8
		private void Wishlist()
		{
			if (this.TargetItem == null)
			{
				return;
			}
			int typeID = this.TargetItem.TypeID;
			if (ItemWishlist.GetWishlistInfo(typeID).isManuallyWishlisted)
			{
				ItemWishlist.RemoveFromWishlist(typeID);
				return;
			}
			ItemWishlist.AddToWishList(this.TargetItem.TypeID);
		}

		// Token: 0x06002087 RID: 8327 RVA: 0x00071815 File Offset: 0x0006FA15
		private void Use()
		{
			LevelManager instance = LevelManager.Instance;
			if (instance != null)
			{
				CharacterMainControl mainCharacter = instance.MainCharacter;
				if (mainCharacter != null)
				{
					mainCharacter.UseItem(this.TargetItem);
				}
			}
			InventoryView.Hide();
			base.Close();
		}

		// Token: 0x06002088 RID: 8328 RVA: 0x00071843 File Offset: 0x0006FA43
		private void Split()
		{
			SplitDialogue.SetupAndShow(this.TargetItem);
			base.Close();
		}

		// Token: 0x06002089 RID: 8329 RVA: 0x00071856 File Offset: 0x0006FA56
		private void Dump()
		{
			LevelManager instance = LevelManager.Instance;
			if ((instance != null) ? instance.MainCharacter : null)
			{
				this.TargetItem.Drop(LevelManager.Instance.MainCharacter, true);
			}
			base.Close();
		}

		// Token: 0x0600208A RID: 8330 RVA: 0x0007188C File Offset: 0x0006FA8C
		private void Modify()
		{
			if (this.TargetItem == null)
			{
				return;
			}
			ItemCustomizeView instance = ItemCustomizeView.Instance;
			if (instance == null)
			{
				return;
			}
			List<Inventory> list = new List<Inventory>();
			LevelManager instance2 = LevelManager.Instance;
			Inventory inventory;
			if (instance2 == null)
			{
				inventory = null;
			}
			else
			{
				CharacterMainControl mainCharacter = instance2.MainCharacter;
				if (mainCharacter == null)
				{
					inventory = null;
				}
				else
				{
					Item characterItem = mainCharacter.CharacterItem;
					inventory = ((characterItem != null) ? characterItem.Inventory : null);
				}
			}
			Inventory inventory2 = inventory;
			if (inventory2)
			{
				list.Add(inventory2);
			}
			instance.Setup(this.TargetItem, list);
			instance.Open(null);
			base.Close();
		}

		// Token: 0x0600208B RID: 8331 RVA: 0x00071911 File Offset: 0x0006FB11
		private void Equip()
		{
			LevelManager instance = LevelManager.Instance;
			if (instance != null)
			{
				CharacterMainControl mainCharacter = instance.MainCharacter;
				if (mainCharacter != null)
				{
					Item characterItem = mainCharacter.CharacterItem;
					if (characterItem != null)
					{
						characterItem.TryPlug(this.TargetItem, false, null, 0);
					}
				}
			}
			base.Close();
		}

		// Token: 0x0600208C RID: 8332 RVA: 0x0007194C File Offset: 0x0006FB4C
		private void Unload()
		{
			Item targetItem = this.TargetItem;
			ItemSetting_Gun itemSetting_Gun = ((targetItem != null) ? targetItem.GetComponent<ItemSetting_Gun>() : null);
			if (itemSetting_Gun == null)
			{
				return;
			}
			AudioManager.Post("SFX/Combat/Gun/unload");
			itemSetting_Gun.TakeOutAllBullets();
		}

		// Token: 0x0600208D RID: 8333 RVA: 0x00071987 File Offset: 0x0006FB87
		protected override void OnOpen()
		{
			this.fadeGroup.Show();
		}

		// Token: 0x0600208E RID: 8334 RVA: 0x00071994 File Offset: 0x0006FB94
		protected override void OnClose()
		{
			this.fadeGroup.Hide();
			this.displayingItem = null;
		}

		// Token: 0x0600208F RID: 8335 RVA: 0x000719A8 File Offset: 0x0006FBA8
		public static void Show(ItemDisplay id)
		{
			if (ItemOperationMenu.Instance == null)
			{
				return;
			}
			ItemOperationMenu.Instance.MShow(id);
		}

		// Token: 0x06002090 RID: 8336 RVA: 0x000719C3 File Offset: 0x0006FBC3
		private void MShow(ItemDisplay targetDisplay)
		{
			if (targetDisplay == null)
			{
				return;
			}
			this.TargetDisplay = targetDisplay;
			this.targetView = targetDisplay.GetComponentInParent<View>();
			this.Setup();
			base.Open(null);
		}

		// Token: 0x06002091 RID: 8337 RVA: 0x000719F0 File Offset: 0x0006FBF0
		private void Setup()
		{
			if (this.TargetItem == null)
			{
				return;
			}
			this.displayingItem = this.TargetItem;
			this.icon.sprite = this.TargetItem.Icon;
			this.nameText.text = this.TargetItem.DisplayName;
			this.btn_Use.gameObject.SetActive(this.Usable);
			this.btn_Use.interactable = this.UseButtonInteractable;
			this.btn_Split.gameObject.SetActive(this.Splittable);
			this.btn_Dump.gameObject.SetActive(this.Dumpable);
			this.btn_Equip.gameObject.SetActive(this.Equipable);
			this.btn_Modify.gameObject.SetActive(this.Modifyable);
			this.btn_Unload.gameObject.SetActive(this.Unloadable);
			this.RefreshWeightText();
			this.RefreshPosition();
		}

		// Token: 0x06002092 RID: 8338 RVA: 0x00071AE8 File Offset: 0x0006FCE8
		private void RefreshPosition()
		{
			RectTransform rectTransform = this.TargetDisplay.transform as RectTransform;
			Rect rect = rectTransform.rect;
			Vector2 min = rect.min;
			Vector2 max = rect.max;
			Vector3 vector = rectTransform.localToWorldMatrix.MultiplyPoint(min);
			Vector3 vector2 = rectTransform.localToWorldMatrix.MultiplyPoint(max);
			Vector3 vector3 = this.rectTransform.worldToLocalMatrix.MultiplyPoint(vector);
			Vector3 vector4 = this.rectTransform.worldToLocalMatrix.MultiplyPoint(vector2);
			Vector2[] array = new Vector2[]
			{
				new Vector2(vector3.x, vector3.y),
				new Vector2(vector3.x, vector4.y),
				new Vector2(vector4.x, vector3.y),
				new Vector2(vector4.x, vector4.y)
			};
			int num = 0;
			float num2 = float.MaxValue;
			Vector2 center = this.rectTransform.rect.center;
			for (int i = 0; i < array.Length; i++)
			{
				float sqrMagnitude = (array[i] - center).sqrMagnitude;
				if (sqrMagnitude < num2)
				{
					num = i;
					num2 = sqrMagnitude;
				}
			}
			bool flag = (num & 2) > 0;
			bool flag2 = (num & 1) > 0;
			float num3 = (flag ? vector4.x : vector3.x);
			float num4 = (flag2 ? vector3.y : vector4.y);
			this.contentRectTransform.pivot = new Vector2((float)(flag ? 0 : 1), (float)(flag2 ? 0 : 1));
			this.contentRectTransform.localPosition = new Vector2(num3, num4);
		}

		// Token: 0x06002093 RID: 8339 RVA: 0x00071CBC File Offset: 0x0006FEBC
		private void RefreshWeightText()
		{
			if (this.displayingItem == null)
			{
				return;
			}
			this.weightText.text = string.Format(this.weightTextFormat, this.displayingItem.TotalWeight);
		}

		// Token: 0x06002094 RID: 8340 RVA: 0x00071CF3 File Offset: 0x0006FEF3
		public void OnPointerClick(PointerEventData eventData)
		{
			base.Close();
		}

		// Token: 0x1700063F RID: 1599
		// (get) Token: 0x06002095 RID: 8341 RVA: 0x00071CFB File Offset: 0x0006FEFB
		private bool Usable
		{
			get
			{
				return this.TargetItem.UsageUtilities != null;
			}
		}

		// Token: 0x17000640 RID: 1600
		// (get) Token: 0x06002096 RID: 8342 RVA: 0x00071D0E File Offset: 0x0006FF0E
		private bool UseButtonInteractable
		{
			get
			{
				if (this.TargetItem)
				{
					Item targetItem = this.TargetItem;
					LevelManager instance = LevelManager.Instance;
					return targetItem.IsUsable((instance != null) ? instance.MainCharacter : null);
				}
				return false;
			}
		}

		// Token: 0x17000641 RID: 1601
		// (get) Token: 0x06002097 RID: 8343 RVA: 0x00071D3C File Offset: 0x0006FF3C
		private bool Splittable
		{
			get
			{
				CharacterMainControl main = CharacterMainControl.Main;
				return (main == null || main.CharacterItem.Inventory.GetFirstEmptyPosition(0) >= 0) && (this.TargetItem && this.TargetItem.Stackable) && this.TargetItem.StackCount > 1;
			}
		}

		// Token: 0x17000642 RID: 1602
		// (get) Token: 0x06002098 RID: 8344 RVA: 0x00071D98 File Offset: 0x0006FF98
		private bool Dumpable
		{
			get
			{
				if (!this.TargetItem.CanDrop)
				{
					return false;
				}
				LevelManager instance = LevelManager.Instance;
				Item item;
				if (instance == null)
				{
					item = null;
				}
				else
				{
					CharacterMainControl mainCharacter = instance.MainCharacter;
					item = ((mainCharacter != null) ? mainCharacter.CharacterItem : null);
				}
				Item item2 = item;
				return this.TargetItem.GetRoot() == item2;
			}
		}

		// Token: 0x17000643 RID: 1603
		// (get) Token: 0x06002099 RID: 8345 RVA: 0x00071DE8 File Offset: 0x0006FFE8
		private bool Equipable
		{
			get
			{
				if (this.TargetItem == null)
				{
					return false;
				}
				if (this.TargetItem.PluggedIntoSlot != null)
				{
					return false;
				}
				LevelManager instance = LevelManager.Instance;
				bool? flag;
				if (instance == null)
				{
					flag = null;
				}
				else
				{
					CharacterMainControl mainCharacter = instance.MainCharacter;
					if (mainCharacter == null)
					{
						flag = null;
					}
					else
					{
						Item characterItem = mainCharacter.CharacterItem;
						flag = ((characterItem != null) ? new bool?(characterItem.Slots.Any((Slot e) => e.CanPlug(this.TargetItem))) : null);
					}
				}
				bool? flag2 = flag;
				return flag2 != null && flag2.Value;
			}
		}

		// Token: 0x17000644 RID: 1604
		// (get) Token: 0x0600209A RID: 8346 RVA: 0x00071E7E File Offset: 0x0007007E
		private bool Modifyable
		{
			get
			{
				return this.alwaysModifyable;
			}
		}

		// Token: 0x17000645 RID: 1605
		// (get) Token: 0x0600209B RID: 8347 RVA: 0x00071E8B File Offset: 0x0007008B
		private bool Unloadable
		{
			get
			{
				return !(this.TargetItem == null) && this.TargetItem.GetComponent<ItemSetting_Gun>();
			}
		}

		// Token: 0x04001628 RID: 5672
		[SerializeField]
		private FadeGroup fadeGroup;

		// Token: 0x04001629 RID: 5673
		[SerializeField]
		private RectTransform rectTransform;

		// Token: 0x0400162A RID: 5674
		[SerializeField]
		private RectTransform contentRectTransform;

		// Token: 0x0400162B RID: 5675
		[SerializeField]
		private Image icon;

		// Token: 0x0400162C RID: 5676
		[SerializeField]
		private TextMeshProUGUI nameText;

		// Token: 0x0400162D RID: 5677
		[SerializeField]
		private TextMeshProUGUI weightText;

		// Token: 0x0400162E RID: 5678
		[SerializeField]
		private string weightTextFormat = "{0:0.#}kg";

		// Token: 0x0400162F RID: 5679
		[SerializeField]
		private Button btn_Use;

		// Token: 0x04001630 RID: 5680
		[SerializeField]
		private Button btn_Split;

		// Token: 0x04001631 RID: 5681
		[SerializeField]
		private Button btn_Dump;

		// Token: 0x04001632 RID: 5682
		[SerializeField]
		private Button btn_Equip;

		// Token: 0x04001633 RID: 5683
		[SerializeField]
		private Button btn_Modify;

		// Token: 0x04001634 RID: 5684
		[SerializeField]
		private Button btn_Unload;

		// Token: 0x04001635 RID: 5685
		[SerializeField]
		private Button btn_Wishlist;

		// Token: 0x04001636 RID: 5686
		[SerializeField]
		private bool alwaysModifyable;

		// Token: 0x04001637 RID: 5687
		private View targetView;

		// Token: 0x04001638 RID: 5688
		private ItemDisplay TargetDisplay;

		// Token: 0x04001639 RID: 5689
		private Item displayingItem;
	}
}
