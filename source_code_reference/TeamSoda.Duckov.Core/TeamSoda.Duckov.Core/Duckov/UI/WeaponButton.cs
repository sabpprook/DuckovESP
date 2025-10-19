using System;
using Duckov.Utilities;
using ItemStatsSystem;
using ItemStatsSystem.Items;
using LeTai.TrueShadow;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Duckov.UI
{
	// Token: 0x0200039F RID: 927
	public class WeaponButton : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
	{
		// Token: 0x140000E0 RID: 224
		// (add) Token: 0x06002104 RID: 8452 RVA: 0x00073238 File Offset: 0x00071438
		// (remove) Token: 0x06002105 RID: 8453 RVA: 0x0007326C File Offset: 0x0007146C
		public static event Action<WeaponButton> OnWeaponButtonSelected;

		// Token: 0x17000654 RID: 1620
		// (get) Token: 0x06002106 RID: 8454 RVA: 0x0007329F File Offset: 0x0007149F
		private CharacterMainControl Character
		{
			get
			{
				return this._character;
			}
		}

		// Token: 0x17000655 RID: 1621
		// (get) Token: 0x06002107 RID: 8455 RVA: 0x000732A7 File Offset: 0x000714A7
		private Slot TargetSlot
		{
			get
			{
				return this._targetSlot;
			}
		}

		// Token: 0x17000656 RID: 1622
		// (get) Token: 0x06002108 RID: 8456 RVA: 0x000732AF File Offset: 0x000714AF
		private Item TargetItem
		{
			get
			{
				Slot targetSlot = this.TargetSlot;
				if (targetSlot == null)
				{
					return null;
				}
				return targetSlot.Content;
			}
		}

		// Token: 0x17000657 RID: 1623
		// (get) Token: 0x06002109 RID: 8457 RVA: 0x000732C4 File Offset: 0x000714C4
		private bool IsSelected
		{
			get
			{
				Item targetItem = this.TargetItem;
				if (((targetItem != null) ? targetItem.ActiveAgent : null) != null)
				{
					global::UnityEngine.Object activeAgent = this.TargetItem.ActiveAgent;
					ItemAgentHolder agentHolder = this._character.agentHolder;
					return activeAgent == ((agentHolder != null) ? agentHolder.CurrentHoldItemAgent : null);
				}
				return false;
			}
		}

		// Token: 0x0600210A RID: 8458 RVA: 0x00073314 File Offset: 0x00071514
		private void Awake()
		{
			this.RegisterStaticEvents();
			LevelManager instance = LevelManager.Instance;
			if (((instance != null) ? instance.MainCharacter : null) != null)
			{
				this.Initialize(LevelManager.Instance.MainCharacter);
			}
		}

		// Token: 0x0600210B RID: 8459 RVA: 0x00073345 File Offset: 0x00071545
		private void OnDestroy()
		{
			this.UnregisterStaticEvents();
			this.isBeingDestroyed = true;
		}

		// Token: 0x0600210C RID: 8460 RVA: 0x00073354 File Offset: 0x00071554
		private void RegisterStaticEvents()
		{
			LevelManager.OnLevelInitialized += this.OnLevelInitialized;
			CharacterMainControl.OnMainCharacterChangeHoldItemAgentEvent = (Action<CharacterMainControl, DuckovItemAgent>)Delegate.Combine(CharacterMainControl.OnMainCharacterChangeHoldItemAgentEvent, new Action<CharacterMainControl, DuckovItemAgent>(this.OnMainCharacterChangeHoldItemAgent));
		}

		// Token: 0x0600210D RID: 8461 RVA: 0x00073387 File Offset: 0x00071587
		private void UnregisterStaticEvents()
		{
			LevelManager.OnLevelInitialized -= this.OnLevelInitialized;
			CharacterMainControl.OnMainCharacterChangeHoldItemAgentEvent = (Action<CharacterMainControl, DuckovItemAgent>)Delegate.Remove(CharacterMainControl.OnMainCharacterChangeHoldItemAgentEvent, new Action<CharacterMainControl, DuckovItemAgent>(this.OnMainCharacterChangeHoldItemAgent));
		}

		// Token: 0x0600210E RID: 8462 RVA: 0x000733BA File Offset: 0x000715BA
		private void OnMainCharacterChangeHoldItemAgent(CharacterMainControl control, DuckovItemAgent agent)
		{
			if (this._character && control == this._character)
			{
				this.Refresh();
			}
		}

		// Token: 0x0600210F RID: 8463 RVA: 0x000733DD File Offset: 0x000715DD
		private void OnLevelInitialized()
		{
			LevelManager instance = LevelManager.Instance;
			this.Initialize((instance != null) ? instance.MainCharacter : null);
		}

		// Token: 0x06002110 RID: 8464 RVA: 0x000733F8 File Offset: 0x000715F8
		private void Initialize(CharacterMainControl character)
		{
			this.UnregisterEvents();
			this._character = character;
			if (character == null)
			{
				Debug.LogError("Character 不存在，初始化失败");
			}
			if (character.CharacterItem == null)
			{
				Debug.LogError("Character item 不存在，初始化失败");
			}
			this._targetSlot = character.CharacterItem.Slots.GetSlot(this.targetSlotKey);
			if (this._targetSlot == null)
			{
				Debug.LogError("Slot " + this.targetSlotKey + " 不存在，初始化失败");
			}
			this.RegisterEvents();
			this.Refresh();
		}

		// Token: 0x06002111 RID: 8465 RVA: 0x00073487 File Offset: 0x00071687
		private void RegisterEvents()
		{
			if (this._targetSlot == null)
			{
				return;
			}
			this._targetSlot.onSlotContentChanged += this.OnSlotContentChanged;
		}

		// Token: 0x06002112 RID: 8466 RVA: 0x000734A9 File Offset: 0x000716A9
		private void UnregisterEvents()
		{
			if (this._targetSlot == null)
			{
				return;
			}
			this._targetSlot.onSlotContentChanged -= this.OnSlotContentChanged;
		}

		// Token: 0x06002113 RID: 8467 RVA: 0x000734CB File Offset: 0x000716CB
		private void OnSlotContentChanged(Slot slot)
		{
			this.Refresh();
		}

		// Token: 0x06002114 RID: 8468 RVA: 0x000734D4 File Offset: 0x000716D4
		private void Refresh()
		{
			if (this.isBeingDestroyed)
			{
				return;
			}
			this.displayParent.SetActive(this.TargetItem);
			bool isSelected = this.IsSelected;
			if (this.TargetItem)
			{
				this.icon.sprite = this.TargetItem.Icon;
				ValueTuple<float, Color, bool> shadowOffsetAndColorOfQuality = GameplayDataSettings.UIStyle.GetShadowOffsetAndColorOfQuality(this.TargetItem.DisplayQuality);
				this.iconShadow.Inset = shadowOffsetAndColorOfQuality.Item3;
				this.iconShadow.Color = shadowOffsetAndColorOfQuality.Item2;
				this.iconShadow.OffsetDistance = shadowOffsetAndColorOfQuality.Item1;
				this.selectionFrame.SetActive(isSelected);
			}
			UnityEvent<WeaponButton> unityEvent = this.onRefresh;
			if (unityEvent != null)
			{
				unityEvent.Invoke(this);
			}
			if (isSelected)
			{
				UnityEvent<WeaponButton> unityEvent2 = this.onSelected;
				if (unityEvent2 != null)
				{
					unityEvent2.Invoke(this);
				}
				Action<WeaponButton> onWeaponButtonSelected = WeaponButton.OnWeaponButtonSelected;
				if (onWeaponButtonSelected == null)
				{
					return;
				}
				onWeaponButtonSelected(this);
			}
		}

		// Token: 0x06002115 RID: 8469 RVA: 0x000735B6 File Offset: 0x000717B6
		public void OnPointerClick(PointerEventData eventData)
		{
			if (this.Character == null)
			{
				return;
			}
			UnityEvent<WeaponButton> unityEvent = this.onClick;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke(this);
		}

		// Token: 0x0400166A RID: 5738
		[SerializeField]
		private string targetSlotKey = "PrimaryWeapon";

		// Token: 0x0400166B RID: 5739
		[SerializeField]
		private GameObject displayParent;

		// Token: 0x0400166C RID: 5740
		[SerializeField]
		private Image icon;

		// Token: 0x0400166D RID: 5741
		[SerializeField]
		private TrueShadow iconShadow;

		// Token: 0x0400166E RID: 5742
		[SerializeField]
		private GameObject selectionFrame;

		// Token: 0x0400166F RID: 5743
		public UnityEvent<WeaponButton> onClick;

		// Token: 0x04001670 RID: 5744
		public UnityEvent<WeaponButton> onRefresh;

		// Token: 0x04001671 RID: 5745
		public UnityEvent<WeaponButton> onSelected;

		// Token: 0x04001673 RID: 5747
		private CharacterMainControl _character;

		// Token: 0x04001674 RID: 5748
		private Slot _targetSlot;

		// Token: 0x04001675 RID: 5749
		private bool isBeingDestroyed;
	}
}
