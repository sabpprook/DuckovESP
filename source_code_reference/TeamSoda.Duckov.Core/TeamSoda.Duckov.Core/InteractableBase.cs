using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Duckov;
using Duckov.MasterKeys;
using Duckov.Scenes;
using Duckov.Utilities;
using ItemStatsSystem;
using ItemStatsSystem.Items;
using SodaCraft.Localizations;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020000D8 RID: 216
public class InteractableBase : MonoBehaviour, IProgress
{
	// Token: 0x060006B5 RID: 1717 RVA: 0x0001E260 File Offset: 0x0001C460
	public List<InteractableBase> GetInteractableList()
	{
		this._interactbleList.Clear();
		this._interactbleList.Add(this);
		if (!this.interactableGroup || this.otherInterablesInGroup.Count <= 0)
		{
			return this._interactbleList;
		}
		foreach (InteractableBase interactableBase in this.otherInterablesInGroup)
		{
			if (!(interactableBase == null) && interactableBase.gameObject.activeInHierarchy)
			{
				this._interactbleList.Add(interactableBase);
			}
		}
		return this._interactbleList;
	}

	// Token: 0x17000139 RID: 313
	// (get) Token: 0x060006B6 RID: 1718 RVA: 0x0001E308 File Offset: 0x0001C508
	public float InteractTime
	{
		get
		{
			if (this.requireItem && !this.requireItemUsed)
			{
				return this.interactTime + this.unlockTime;
			}
			return this.interactTime;
		}
	}

	// Token: 0x1700013A RID: 314
	// (get) Token: 0x060006B7 RID: 1719 RVA: 0x0001E32E File Offset: 0x0001C52E
	// (set) Token: 0x060006B8 RID: 1720 RVA: 0x0001E34F File Offset: 0x0001C54F
	public string InteractName
	{
		get
		{
			if (this.overrideInteractName)
			{
				return this._overrideInteractNameKey.ToPlainText();
			}
			return this.defaultInteractNameKey.ToPlainText();
		}
		set
		{
			this.overrideInteractName = true;
			this._overrideInteractNameKey = value;
		}
	}

	// Token: 0x1700013B RID: 315
	// (get) Token: 0x060006B9 RID: 1721 RVA: 0x0001E35F File Offset: 0x0001C55F
	private bool ShowBaseInteractName
	{
		get
		{
			return this.overrideInteractName && this.ShowBaseInteractNameInspector;
		}
	}

	// Token: 0x1700013C RID: 316
	// (get) Token: 0x060006BA RID: 1722 RVA: 0x0001E371 File Offset: 0x0001C571
	protected virtual bool ShowBaseInteractNameInspector
	{
		get
		{
			return true;
		}
	}

	// Token: 0x1700013D RID: 317
	// (get) Token: 0x060006BB RID: 1723 RVA: 0x0001E374 File Offset: 0x0001C574
	private ItemMetaData CachedMeta
	{
		get
		{
			if (this._cachedMeta == null)
			{
				this._cachedMeta = new ItemMetaData?(ItemAssetsCollection.GetMetaData(this.requireItemId));
			}
			return this._cachedMeta.Value;
		}
	}

	// Token: 0x1400002B RID: 43
	// (add) Token: 0x060006BC RID: 1724 RVA: 0x0001E3A4 File Offset: 0x0001C5A4
	// (remove) Token: 0x060006BD RID: 1725 RVA: 0x0001E3D8 File Offset: 0x0001C5D8
	public static event Action<InteractableBase> OnInteractStartStaticEvent;

	// Token: 0x1700013E RID: 318
	// (get) Token: 0x060006BE RID: 1726 RVA: 0x0001E40B File Offset: 0x0001C60B
	protected virtual bool ShowUnityEvents
	{
		get
		{
			return true;
		}
	}

	// Token: 0x1700013F RID: 319
	// (get) Token: 0x060006BF RID: 1727 RVA: 0x0001E40E File Offset: 0x0001C60E
	public bool Interacting
	{
		get
		{
			return this.interactCharacter != null;
		}
	}

	// Token: 0x17000140 RID: 320
	// (get) Token: 0x060006C0 RID: 1728 RVA: 0x0001E41C File Offset: 0x0001C61C
	// (set) Token: 0x060006C1 RID: 1729 RVA: 0x0001E424 File Offset: 0x0001C624
	public bool MarkerActive
	{
		get
		{
			return this.interactMarkerVisible;
		}
		set
		{
			if (!base.enabled)
			{
				return;
			}
			this.interactMarkerVisible = value;
			if (value)
			{
				this.ActiveMarker();
				return;
			}
			if (this.markerObject)
			{
				this.markerObject.gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x060006C2 RID: 1730 RVA: 0x0001E460 File Offset: 0x0001C660
	protected virtual void Awake()
	{
		this.requireItemDataKeyCached = this.GetKey();
		if (this.interactCollider == null)
		{
			this.interactCollider = base.GetComponent<Collider>();
			if (this.interactCollider == null)
			{
				this.interactCollider = base.gameObject.AddComponent<BoxCollider>();
				this.interactCollider.enabled = false;
			}
		}
		if (this.interactCollider != null)
		{
			this.interactCollider.gameObject.layer = LayerMask.NameToLayer("Interactable");
		}
		foreach (InteractableBase interactableBase in this.otherInterablesInGroup)
		{
			if (interactableBase)
			{
				interactableBase.MarkerActive = false;
				interactableBase.transform.position = base.transform.position;
				interactableBase.transform.rotation = base.transform.rotation;
				interactableBase.interactMarkerOffset = this.interactMarkerOffset;
			}
		}
		this._interactbleList = new List<InteractableBase>();
	}

	// Token: 0x060006C3 RID: 1731 RVA: 0x0001E578 File Offset: 0x0001C778
	protected virtual void Start()
	{
		object obj;
		if (this.requireItem && MultiSceneCore.Instance && MultiSceneCore.Instance.inLevelData.TryGetValue(this.requireItemDataKeyCached, out obj) && obj is bool)
		{
			bool flag = (bool)obj;
			if (flag)
			{
				this.requireItem = false;
				this.requireItemUsed = true;
				UnityEvent onRequiredItemUsedEvent = this.OnRequiredItemUsedEvent;
				if (onRequiredItemUsedEvent != null)
				{
					onRequiredItemUsedEvent.Invoke();
				}
			}
		}
		this.MarkerActive = this.interactMarkerVisible;
	}

	// Token: 0x060006C4 RID: 1732 RVA: 0x0001E5F0 File Offset: 0x0001C7F0
	private void ActiveMarker()
	{
		if (this.markerObject)
		{
			if (!this.markerObject.gameObject.activeInHierarchy)
			{
				this.markerObject.gameObject.SetActive(true);
			}
			return;
		}
		this.markerObject = global::UnityEngine.Object.Instantiate<InteractMarker>(GameplayDataSettings.Prefabs.InteractMarker, base.transform);
		this.markerObject.transform.localPosition = this.interactMarkerOffset;
		this.CheckInteractable();
	}

	// Token: 0x060006C5 RID: 1733 RVA: 0x0001E666 File Offset: 0x0001C866
	public void SetMarkerUsed()
	{
		if (!this.markerObject)
		{
			return;
		}
		this.markerObject.MarkAsUsed();
	}

	// Token: 0x060006C6 RID: 1734 RVA: 0x0001E684 File Offset: 0x0001C884
	public bool StartInteract(CharacterMainControl _interactCharacter)
	{
		if (!_interactCharacter)
		{
			return false;
		}
		if (this.requireItem && !this.TryGetRequiredItem(_interactCharacter).Item1)
		{
			return false;
		}
		if (this.interactCharacter == _interactCharacter)
		{
			return false;
		}
		if (!this.CheckInteractable())
		{
			return false;
		}
		if (this.requireItem && this.whenToUseRequireItem == InteractableBase.WhenToUseRequireItemTypes.OnStartInteract && !this.UseRequiredItem(_interactCharacter))
		{
			this.StopInteract();
			return false;
		}
		this.interactCharacter = _interactCharacter;
		this.interactTimer = 0f;
		this.timeOut = false;
		UnityEvent<CharacterMainControl, InteractableBase> onInteractStartEvent = this.OnInteractStartEvent;
		if (onInteractStartEvent != null)
		{
			onInteractStartEvent.Invoke(_interactCharacter, this);
		}
		Action<InteractableBase> onInteractStartStaticEvent = InteractableBase.OnInteractStartStaticEvent;
		if (onInteractStartStaticEvent != null)
		{
			onInteractStartStaticEvent(this);
		}
		try
		{
			this.OnInteractStart(_interactCharacter);
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
			if (CharacterMainControl.Main)
			{
				CharacterMainControl.Main.PopText("OnInteractStart开始失败，Log Error", -1f);
			}
			return false;
		}
		return true;
	}

	// Token: 0x060006C7 RID: 1735 RVA: 0x0001E774 File Offset: 0x0001C974
	public InteractableBase GetInteractableInGroup(int index)
	{
		if (index == 0)
		{
			return this;
		}
		List<InteractableBase> interactableList = this.GetInteractableList();
		if (index >= interactableList.Count)
		{
			return null;
		}
		return interactableList[index];
	}

	// Token: 0x060006C8 RID: 1736 RVA: 0x0001E79F File Offset: 0x0001C99F
	public void InternalStopInteract()
	{
		this.interactCharacter = null;
		this.lastStopTime = Time.time;
		this.OnInteractStop();
	}

	// Token: 0x060006C9 RID: 1737 RVA: 0x0001E7BC File Offset: 0x0001C9BC
	public void StopInteract()
	{
		CharacterMainControl characterMainControl = this.interactCharacter;
		if (characterMainControl && characterMainControl.interactAction.Running && characterMainControl.interactAction.InteractingTarget == this)
		{
			this.interactCharacter.interactAction.StopAction();
			return;
		}
		this.InternalStopInteract();
	}

	// Token: 0x060006CA RID: 1738 RVA: 0x0001E810 File Offset: 0x0001CA10
	public void UpdateInteract(CharacterMainControl _interactCharacter, float deltaTime)
	{
		this.interactTimer += deltaTime;
		this.OnUpdate(_interactCharacter, deltaTime);
		if (!this.timeOut && this.interactTimer >= this.InteractTime)
		{
			if (this.requireItem && this.whenToUseRequireItem == InteractableBase.WhenToUseRequireItemTypes.OnTimeOut && !this.UseRequiredItem(_interactCharacter))
			{
				this.StopInteract();
				return;
			}
			if (this.requireItem && this.whenToUseRequireItem == InteractableBase.WhenToUseRequireItemTypes.None && !this.requireItemUsed)
			{
				this.requireItemUsed = true;
				UnityEvent onRequiredItemUsedEvent = this.OnRequiredItemUsedEvent;
				if (onRequiredItemUsedEvent != null)
				{
					onRequiredItemUsedEvent.Invoke();
				}
				if (MultiSceneCore.Instance)
				{
					MultiSceneCore.Instance.inLevelData[this.requireItemDataKeyCached] = true;
					Debug.Log("设置使用过物品为true");
				}
			}
			this.timeOut = true;
			this.OnTimeOut();
			UnityEvent<CharacterMainControl, InteractableBase> onInteractTimeoutEvent = this.OnInteractTimeoutEvent;
			if (onInteractTimeoutEvent != null)
			{
				onInteractTimeoutEvent.Invoke(_interactCharacter, this);
			}
			if (this.finishWhenTimeOut)
			{
				this.FinishInteract(_interactCharacter);
			}
		}
	}

	// Token: 0x060006CB RID: 1739 RVA: 0x0001E900 File Offset: 0x0001CB00
	public void FinishInteract(CharacterMainControl _interactCharacter)
	{
		if (this.requireItem && this.whenToUseRequireItem == InteractableBase.WhenToUseRequireItemTypes.OnFinshed && !this.UseRequiredItem(_interactCharacter))
		{
			this.StopInteract();
			return;
		}
		try
		{
			this.OnInteractFinished();
			UnityEvent<CharacterMainControl, InteractableBase> onInteractFinishedEvent = this.OnInteractFinishedEvent;
			if (onInteractFinishedEvent != null)
			{
				onInteractFinishedEvent.Invoke(_interactCharacter, this);
			}
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
		}
		this.StopInteract();
		if (this.disableOnFinish)
		{
			base.enabled = false;
			if (this.markerObject)
			{
				this.markerObject.gameObject.SetActive(false);
			}
			if (this.interactCollider)
			{
				this.interactCollider.enabled = false;
			}
		}
	}

	// Token: 0x060006CC RID: 1740 RVA: 0x0001E9B0 File Offset: 0x0001CBB0
	protected virtual void OnUpdate(CharacterMainControl _interactCharacter, float deltaTime)
	{
	}

	// Token: 0x060006CD RID: 1741 RVA: 0x0001E9B2 File Offset: 0x0001CBB2
	protected virtual void OnTimeOut()
	{
	}

	// Token: 0x060006CE RID: 1742 RVA: 0x0001E9B4 File Offset: 0x0001CBB4
	private bool UseRequiredItem(CharacterMainControl interactCharacter)
	{
		Debug.Log("尝试使用");
		ValueTuple<bool, Item> valueTuple = this.TryGetRequiredItem(interactCharacter);
		Item item = valueTuple.Item2;
		if (!valueTuple.Item1 || valueTuple.Item2 == null)
		{
			return false;
		}
		if (item.UseDurability)
		{
			Debug.Log("尝试消耗耐久");
			item.Durability -= 1f;
			if (item.Durability <= 0f)
			{
				item.Detach();
				item.DestroyTree();
			}
		}
		else if (!item.Stackable)
		{
			Debug.Log("尝试直接消耗掉");
			item.Detach();
			item.DestroyTree();
		}
		else
		{
			Debug.Log("尝试消耗堆叠");
			item.StackCount--;
		}
		if (this.requireOnce)
		{
			this.requireItem = false;
			this.requireItemUsed = true;
			UnityEvent onRequiredItemUsedEvent = this.OnRequiredItemUsedEvent;
			if (onRequiredItemUsedEvent != null)
			{
				onRequiredItemUsedEvent.Invoke();
			}
			if (MultiSceneCore.Instance)
			{
				MultiSceneCore.Instance.inLevelData[this.requireItemDataKeyCached] = true;
				Debug.Log("设置使用过物品为true");
			}
		}
		return true;
	}

	// Token: 0x060006CF RID: 1743 RVA: 0x0001EAC4 File Offset: 0x0001CCC4
	public bool CheckInteractable()
	{
		if (this.interactCharacter != null)
		{
			if (!(this.interactCharacter.interactAction.InteractingTarget != this))
			{
				return false;
			}
			this.StopInteract();
		}
		return (Time.time - this.lastStopTime >= this.coolTime || this.coolTime <= 0f || this.lastStopTime <= 0f) && this.IsInteractable();
	}

	// Token: 0x060006D0 RID: 1744 RVA: 0x0001EB37 File Offset: 0x0001CD37
	protected virtual bool IsInteractable()
	{
		return true;
	}

	// Token: 0x060006D1 RID: 1745 RVA: 0x0001EB3A File Offset: 0x0001CD3A
	protected virtual void OnInteractStart(CharacterMainControl interactCharacter)
	{
	}

	// Token: 0x060006D2 RID: 1746 RVA: 0x0001EB3C File Offset: 0x0001CD3C
	protected virtual void OnInteractStop()
	{
	}

	// Token: 0x060006D3 RID: 1747 RVA: 0x0001EB3E File Offset: 0x0001CD3E
	protected virtual void OnInteractFinished()
	{
	}

	// Token: 0x060006D4 RID: 1748 RVA: 0x0001EB40 File Offset: 0x0001CD40
	public string GetInteractName()
	{
		if (this.overrideInteractName)
		{
			return this.InteractName;
		}
		return "UI_Interact".ToPlainText();
	}

	// Token: 0x060006D5 RID: 1749 RVA: 0x0001EB5C File Offset: 0x0001CD5C
	public string GetRequiredItemName()
	{
		if (!this.requireItem)
		{
			return null;
		}
		return this.CachedMeta.DisplayName;
	}

	// Token: 0x060006D6 RID: 1750 RVA: 0x0001EB81 File Offset: 0x0001CD81
	public Sprite GetRequireditemIcon()
	{
		if (!this.requireItem)
		{
			return null;
		}
		return this.CachedMeta.icon;
	}

	// Token: 0x060006D7 RID: 1751 RVA: 0x0001EB98 File Offset: 0x0001CD98
	protected virtual void OnDestroy()
	{
		if (this.Interacting)
		{
			this.StopInteract();
		}
	}

	// Token: 0x060006D8 RID: 1752 RVA: 0x0001EBA8 File Offset: 0x0001CDA8
	public virtual Progress GetProgress()
	{
		Progress progress = default(Progress);
		if (this.Interacting && this.InteractTime > 0f)
		{
			progress.inProgress = true;
			progress.total = this.InteractTime;
			progress.current = this.interactTimer;
		}
		else
		{
			progress.inProgress = false;
		}
		return progress;
	}

	// Token: 0x060006D9 RID: 1753 RVA: 0x0001EC00 File Offset: 0x0001CE00
	[return: TupleElementNames(new string[] { "hasItem", "ItemInstance" })]
	public ValueTuple<bool, Item> TryGetRequiredItem(CharacterMainControl fromCharacter)
	{
		if (!this.requireItem)
		{
			return new ValueTuple<bool, Item>(false, null);
		}
		if (!fromCharacter)
		{
			return new ValueTuple<bool, Item>(false, null);
		}
		if (MasterKeysManager.IsActive(this.requireItemId))
		{
			return new ValueTuple<bool, Item>(true, null);
		}
		foreach (Slot slot in fromCharacter.CharacterItem.Slots)
		{
			if (slot.Content && slot.Content.TypeID == this.requireItemId)
			{
				return new ValueTuple<bool, Item>(true, slot.Content);
			}
		}
		foreach (Item item in fromCharacter.CharacterItem.Inventory)
		{
			if (item.TypeID == this.requireItemId)
			{
				return new ValueTuple<bool, Item>(true, item);
			}
			if (item.Slots != null && item.Slots.Count > 0)
			{
				foreach (Slot slot2 in item.Slots)
				{
					if (slot2.Content != null && slot2.Content.TypeID == this.requireItemId)
					{
						return new ValueTuple<bool, Item>(true, slot2.Content);
					}
				}
			}
		}
		foreach (Item item2 in LevelManager.Instance.PetProxy.Inventory)
		{
			if (item2.TypeID == this.requireItemId)
			{
				return new ValueTuple<bool, Item>(true, item2);
			}
			if (item2.Slots && item2.Slots.Count > 0)
			{
				foreach (Slot slot3 in item2.Slots)
				{
					if (slot3.Content != null && slot3.Content.TypeID == this.requireItemId)
					{
						return new ValueTuple<bool, Item>(true, slot3.Content);
					}
				}
			}
		}
		return new ValueTuple<bool, Item>(false, null);
	}

	// Token: 0x060006DA RID: 1754 RVA: 0x0001EE90 File Offset: 0x0001D090
	private int GetKey()
	{
		if (this.overrideItemUsedKey)
		{
			return this.overrideItemUsedSaveKey.GetHashCode();
		}
		Vector3 vector = base.transform.position * 10f;
		int num = Mathf.RoundToInt(vector.x);
		int num2 = Mathf.RoundToInt(vector.y);
		int num3 = Mathf.RoundToInt(vector.z);
		Vector3Int vector3Int = new Vector3Int(num, num2, num3);
		return string.Format("Intact_{0}", vector3Int).GetHashCode();
	}

	// Token: 0x060006DB RID: 1755 RVA: 0x0001EF08 File Offset: 0x0001D108
	public void InteractWithMainCharacter()
	{
		CharacterMainControl main = CharacterMainControl.Main;
		if (main == null)
		{
			return;
		}
		main.Interact(this);
	}

	// Token: 0x060006DC RID: 1756 RVA: 0x0001EF1A File Offset: 0x0001D11A
	private void OnDrawGizmos()
	{
		if (!this.interactMarkerVisible)
		{
			return;
		}
		Gizmos.color = Color.yellow;
		Gizmos.DrawSphere(base.transform.TransformPoint(this.interactMarkerOffset), 0.1f);
	}

	// Token: 0x04000676 RID: 1654
	public bool interactableGroup;

	// Token: 0x04000677 RID: 1655
	[SerializeField]
	private List<InteractableBase> otherInterablesInGroup;

	// Token: 0x04000678 RID: 1656
	public bool zoomIn = true;

	// Token: 0x04000679 RID: 1657
	private List<InteractableBase> _interactbleList = new List<InteractableBase>();

	// Token: 0x0400067A RID: 1658
	[SerializeField]
	private float interactTime;

	// Token: 0x0400067B RID: 1659
	public bool finishWhenTimeOut = true;

	// Token: 0x0400067C RID: 1660
	private float interactTimer;

	// Token: 0x0400067D RID: 1661
	public Vector3 interactMarkerOffset;

	// Token: 0x0400067E RID: 1662
	public bool overrideInteractName;

	// Token: 0x0400067F RID: 1663
	[LocalizationKey("Default")]
	private string defaultInteractNameKey = "UI_Interact";

	// Token: 0x04000680 RID: 1664
	[LocalizationKey("Interact")]
	public string _overrideInteractNameKey;

	// Token: 0x04000681 RID: 1665
	public Collider interactCollider;

	// Token: 0x04000682 RID: 1666
	public bool requireItem;

	// Token: 0x04000683 RID: 1667
	public bool requireOnce = true;

	// Token: 0x04000684 RID: 1668
	[ItemTypeID]
	public int requireItemId;

	// Token: 0x04000685 RID: 1669
	public float unlockTime;

	// Token: 0x04000686 RID: 1670
	public bool overrideItemUsedKey;

	// Token: 0x04000687 RID: 1671
	public string overrideItemUsedSaveKey;

	// Token: 0x04000688 RID: 1672
	public InteractableBase.WhenToUseRequireItemTypes whenToUseRequireItem;

	// Token: 0x04000689 RID: 1673
	public UnityEvent OnRequiredItemUsedEvent;

	// Token: 0x0400068A RID: 1674
	private int requireItemDataKeyCached;

	// Token: 0x0400068B RID: 1675
	private bool requireItemUsed;

	// Token: 0x0400068C RID: 1676
	private ItemMetaData? _cachedMeta;

	// Token: 0x0400068D RID: 1677
	public UnityEvent<CharacterMainControl, InteractableBase> OnInteractStartEvent;

	// Token: 0x0400068E RID: 1678
	public UnityEvent<CharacterMainControl, InteractableBase> OnInteractTimeoutEvent;

	// Token: 0x0400068F RID: 1679
	public UnityEvent<CharacterMainControl, InteractableBase> OnInteractFinishedEvent;

	// Token: 0x04000691 RID: 1681
	public bool disableOnFinish;

	// Token: 0x04000692 RID: 1682
	public float coolTime;

	// Token: 0x04000693 RID: 1683
	private float lastStopTime = -1f;

	// Token: 0x04000694 RID: 1684
	protected CharacterMainControl interactCharacter;

	// Token: 0x04000695 RID: 1685
	private bool timeOut;

	// Token: 0x04000696 RID: 1686
	[SerializeField]
	private bool interactMarkerVisible = true;

	// Token: 0x04000697 RID: 1687
	private InteractMarker markerObject;

	// Token: 0x0200045F RID: 1119
	public enum WhenToUseRequireItemTypes
	{
		// Token: 0x04001B0A RID: 6922
		None,
		// Token: 0x04001B0B RID: 6923
		OnFinshed,
		// Token: 0x04001B0C RID: 6924
		OnTimeOut,
		// Token: 0x04001B0D RID: 6925
		OnStartInteract
	}
}
