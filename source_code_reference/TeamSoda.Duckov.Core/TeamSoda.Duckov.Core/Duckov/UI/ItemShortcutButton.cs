using System;
using DG.Tweening;
using ItemStatsSystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Duckov.UI
{
	// Token: 0x020003A6 RID: 934
	public class ItemShortcutButton : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
	{
		// Token: 0x17000669 RID: 1641
		// (get) Token: 0x0600216A RID: 8554 RVA: 0x00074BB0 File Offset: 0x00072DB0
		// (set) Token: 0x0600216B RID: 8555 RVA: 0x00074BB8 File Offset: 0x00072DB8
		public int Index { get; private set; }

		// Token: 0x1700066A RID: 1642
		// (get) Token: 0x0600216C RID: 8556 RVA: 0x00074BC1 File Offset: 0x00072DC1
		// (set) Token: 0x0600216D RID: 8557 RVA: 0x00074BC9 File Offset: 0x00072DC9
		public ItemShortcutPanel Master { get; private set; }

		// Token: 0x1700066B RID: 1643
		// (get) Token: 0x0600216E RID: 8558 RVA: 0x00074BD2 File Offset: 0x00072DD2
		// (set) Token: 0x0600216F RID: 8559 RVA: 0x00074BDA File Offset: 0x00072DDA
		public Inventory Inventory { get; private set; }

		// Token: 0x1700066C RID: 1644
		// (get) Token: 0x06002170 RID: 8560 RVA: 0x00074BE3 File Offset: 0x00072DE3
		// (set) Token: 0x06002171 RID: 8561 RVA: 0x00074BEB File Offset: 0x00072DEB
		public CharacterMainControl Character { get; private set; }

		// Token: 0x1700066D RID: 1645
		// (get) Token: 0x06002172 RID: 8562 RVA: 0x00074BF4 File Offset: 0x00072DF4
		// (set) Token: 0x06002173 RID: 8563 RVA: 0x00074BFC File Offset: 0x00072DFC
		public Item TargetItem { get; private set; }

		// Token: 0x06002174 RID: 8564 RVA: 0x00074C05 File Offset: 0x00072E05
		private Item GetTargetItem()
		{
			return ItemShortcut.Get(this.Index);
		}

		// Token: 0x1700066E RID: 1646
		// (get) Token: 0x06002175 RID: 8565 RVA: 0x00074C14 File Offset: 0x00072E14
		private bool Interactable
		{
			get
			{
				Item targetItem = this.TargetItem;
				return ((targetItem != null) ? targetItem.UsageUtilities : null) || (this.TargetItem && this.TargetItem.HasHandHeldAgent) || (this.TargetItem && this.TargetItem.GetBool("IsSkill", false));
			}
		}

		// Token: 0x06002176 RID: 8566 RVA: 0x00074C7C File Offset: 0x00072E7C
		public void OnPointerClick(PointerEventData eventData)
		{
			if (!this.Interactable)
			{
				this.denialIndicator.color = this.denialColor;
				this.denialIndicator.DOColor(Color.clear, 0.1f);
				return;
			}
			if (this.Character && this.TargetItem && this.TargetItem.UsageUtilities && this.TargetItem.UsageUtilities.IsUsable(this.TargetItem, this.Character))
			{
				this.Character.UseItem(this.TargetItem);
				return;
			}
			if (this.Character && this.TargetItem && this.TargetItem.GetBool("IsSkill", false))
			{
				this.Character.ChangeHoldItem(this.TargetItem);
				return;
			}
			if (this.Character && this.TargetItem && this.TargetItem.HasHandHeldAgent)
			{
				this.Character.ChangeHoldItem(this.TargetItem);
				return;
			}
			this.AnimateDenial();
		}

		// Token: 0x06002177 RID: 8567 RVA: 0x00074D95 File Offset: 0x00072F95
		public void AnimateDenial()
		{
			this.denialIndicator.DOKill(false);
			this.denialIndicator.color = this.denialColor;
			this.denialIndicator.DOColor(Color.clear, 0.1f);
		}

		// Token: 0x06002178 RID: 8568 RVA: 0x00074DCB File Offset: 0x00072FCB
		private void Awake()
		{
			ItemShortcutButton.OnRequireAnimateDenial += this.OnStaticAnimateDenial;
		}

		// Token: 0x06002179 RID: 8569 RVA: 0x00074DDE File Offset: 0x00072FDE
		private void OnDestroy()
		{
			ItemShortcutButton.OnRequireAnimateDenial -= this.OnStaticAnimateDenial;
			this.isBeingDestroyed = true;
			this.UnregisterEvents();
		}

		// Token: 0x0600217A RID: 8570 RVA: 0x00074DFE File Offset: 0x00072FFE
		private void OnStaticAnimateDenial(int index)
		{
			if (!base.isActiveAndEnabled)
			{
				return;
			}
			if (index == this.Index)
			{
				this.AnimateDenial();
			}
		}

		// Token: 0x140000E6 RID: 230
		// (add) Token: 0x0600217B RID: 8571 RVA: 0x00074E18 File Offset: 0x00073018
		// (remove) Token: 0x0600217C RID: 8572 RVA: 0x00074E4C File Offset: 0x0007304C
		private static event Action<int> OnRequireAnimateDenial;

		// Token: 0x0600217D RID: 8573 RVA: 0x00074E7F File Offset: 0x0007307F
		public static void AnimateDenial(int index)
		{
			Action<int> onRequireAnimateDenial = ItemShortcutButton.OnRequireAnimateDenial;
			if (onRequireAnimateDenial == null)
			{
				return;
			}
			onRequireAnimateDenial(index);
		}

		// Token: 0x0600217E RID: 8574 RVA: 0x00074E94 File Offset: 0x00073094
		internal void Initialize(ItemShortcutPanel itemShortcutPanel, int index)
		{
			this.UnregisterEvents();
			this.Master = itemShortcutPanel;
			this.Inventory = this.Master.Target;
			this.Index = index;
			this.Character = this.Master.Character;
			this.Refresh();
			this.RegisterEvents();
		}

		// Token: 0x0600217F RID: 8575 RVA: 0x00074EE4 File Offset: 0x000730E4
		private void Refresh()
		{
			if (this.isBeingDestroyed)
			{
				return;
			}
			this.UnregisterEvents();
			this.TargetItem = this.GetTargetItem();
			if (this.TargetItem == null)
			{
				this.SetupEmpty();
			}
			else
			{
				this.SetupItem(this.TargetItem);
			}
			this.RegisterEvents();
			this.requireRefresh = false;
		}

		// Token: 0x06002180 RID: 8576 RVA: 0x00074F3C File Offset: 0x0007313C
		private void SetupItem(Item targetItem)
		{
			if (this.notInteractableIndicator)
			{
				this.notInteractableIndicator.gameObject.SetActive(false);
			}
			this.itemDisplay.Setup(targetItem);
			this.itemDisplay.gameObject.SetActive(true);
			this.notInteractableIndicator.gameObject.SetActive(!this.Interactable);
		}

		// Token: 0x06002181 RID: 8577 RVA: 0x00074F9D File Offset: 0x0007319D
		private void SetupEmpty()
		{
			this.itemDisplay.gameObject.SetActive(false);
		}

		// Token: 0x06002182 RID: 8578 RVA: 0x00074FB0 File Offset: 0x000731B0
		private void RegisterEvents()
		{
			ItemShortcut.OnSetItem += this.OnItemShortcutSetItem;
			if (this.Inventory != null)
			{
				this.Inventory.onContentChanged += this.OnContentChanged;
			}
			if (this.TargetItem != null)
			{
				this.TargetItem.onSetStackCount += this.OnItemStackCountChanged;
			}
		}

		// Token: 0x06002183 RID: 8579 RVA: 0x00075018 File Offset: 0x00073218
		private void UnregisterEvents()
		{
			ItemShortcut.OnSetItem -= this.OnItemShortcutSetItem;
			if (this.Inventory != null)
			{
				this.Inventory.onContentChanged -= this.OnContentChanged;
			}
			if (this.TargetItem != null)
			{
				this.TargetItem.onSetStackCount -= this.OnItemStackCountChanged;
			}
		}

		// Token: 0x06002184 RID: 8580 RVA: 0x00075080 File Offset: 0x00073280
		private void OnItemShortcutSetItem(int obj)
		{
			this.Refresh();
		}

		// Token: 0x06002185 RID: 8581 RVA: 0x00075088 File Offset: 0x00073288
		private void OnItemStackCountChanged(Item item)
		{
			if (item != this.TargetItem)
			{
				return;
			}
			this.requireRefresh = true;
		}

		// Token: 0x06002186 RID: 8582 RVA: 0x000750A0 File Offset: 0x000732A0
		private void OnContentChanged(Inventory inventory, int index)
		{
			this.requireRefresh = true;
		}

		// Token: 0x06002187 RID: 8583 RVA: 0x000750AC File Offset: 0x000732AC
		private void Update()
		{
			if (this.requireRefresh)
			{
				this.Refresh();
			}
			bool flag = this.TargetItem != null && this.Character.CurrentHoldItemAgent != null && this.TargetItem == this.Character.CurrentHoldItemAgent.Item;
			if (flag && !this.lastFrameUsing)
			{
				this.OnStartedUsing();
			}
			else if (!flag && this.lastFrameUsing)
			{
				this.OnStoppedUsing();
			}
			this.usingIndicator.gameObject.SetActive(flag);
		}

		// Token: 0x06002188 RID: 8584 RVA: 0x0007513C File Offset: 0x0007333C
		private void OnStartedUsing()
		{
		}

		// Token: 0x06002189 RID: 8585 RVA: 0x0007513E File Offset: 0x0007333E
		private void OnStoppedUsing()
		{
		}

		// Token: 0x040016A9 RID: 5801
		[SerializeField]
		private ItemDisplay itemDisplay;

		// Token: 0x040016AA RID: 5802
		[SerializeField]
		private GameObject usingIndicator;

		// Token: 0x040016AB RID: 5803
		[SerializeField]
		private GameObject notInteractableIndicator;

		// Token: 0x040016AC RID: 5804
		[SerializeField]
		private Image denialIndicator;

		// Token: 0x040016AD RID: 5805
		[SerializeField]
		private Color denialColor;

		// Token: 0x040016B4 RID: 5812
		private bool isBeingDestroyed;

		// Token: 0x040016B5 RID: 5813
		private bool requireRefresh;

		// Token: 0x040016B6 RID: 5814
		private bool lastFrameUsing;
	}
}
