using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Duckov;
using Duckov.Buffs;
using Duckov.Scenes;
using Duckov.UI;
using Duckov.UI.DialogueBubbles;
using Duckov.Utilities;
using ItemStatsSystem;
using ItemStatsSystem.Items;
using SodaCraft.Localizations;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200005C RID: 92
public class CharacterMainControl : MonoBehaviour
{
	// Token: 0x17000072 RID: 114
	// (get) Token: 0x06000292 RID: 658 RVA: 0x0000BA6C File Offset: 0x00009C6C
	// (set) Token: 0x06000293 RID: 659 RVA: 0x0000BA74 File Offset: 0x00009C74
	public AudioManager.VoiceType AudioVoiceType
	{
		get
		{
			return this.audioVoiceType;
		}
		set
		{
			this.audioVoiceType = value;
			if (base.gameObject.activeInHierarchy)
			{
				AudioManager.SetVoiceType(base.gameObject, this.audioVoiceType);
			}
		}
	}

	// Token: 0x17000073 RID: 115
	// (get) Token: 0x06000294 RID: 660 RVA: 0x0000BA9B File Offset: 0x00009C9B
	// (set) Token: 0x06000295 RID: 661 RVA: 0x0000BAA3 File Offset: 0x00009CA3
	public AudioManager.FootStepMaterialType FootStepMaterialType
	{
		get
		{
			return this.footStepMaterialType;
		}
		set
		{
			this.footStepMaterialType = value;
		}
	}

	// Token: 0x17000074 RID: 116
	// (get) Token: 0x06000296 RID: 662 RVA: 0x0000BAAC File Offset: 0x00009CAC
	public static CharacterMainControl Main
	{
		get
		{
			if (LevelManager.Instance == null)
			{
				return null;
			}
			return LevelManager.Instance.MainCharacter;
		}
	}

	// Token: 0x17000075 RID: 117
	// (get) Token: 0x06000297 RID: 663 RVA: 0x0000BAC7 File Offset: 0x00009CC7
	public Teams Team
	{
		get
		{
			return this.team;
		}
	}

	// Token: 0x1400000A RID: 10
	// (add) Token: 0x06000298 RID: 664 RVA: 0x0000BAD0 File Offset: 0x00009CD0
	// (remove) Token: 0x06000299 RID: 665 RVA: 0x0000BB08 File Offset: 0x00009D08
	public event Action<Teams> OnTeamChanged;

	// Token: 0x17000076 RID: 118
	// (get) Token: 0x0600029A RID: 666 RVA: 0x0000BB3D File Offset: 0x00009D3D
	public Item CharacterItem
	{
		get
		{
			return this.characterItem;
		}
	}

	// Token: 0x17000077 RID: 119
	// (get) Token: 0x0600029B RID: 667 RVA: 0x0000BB45 File Offset: 0x00009D45
	public DuckovItemAgent CurrentHoldItemAgent
	{
		get
		{
			return this.agentHolder.CurrentHoldItemAgent;
		}
	}

	// Token: 0x17000078 RID: 120
	// (get) Token: 0x0600029C RID: 668 RVA: 0x0000BB52 File Offset: 0x00009D52
	public bool Hidden
	{
		get
		{
			return this.hidden;
		}
	}

	// Token: 0x1400000B RID: 11
	// (add) Token: 0x0600029D RID: 669 RVA: 0x0000BB5C File Offset: 0x00009D5C
	// (remove) Token: 0x0600029E RID: 670 RVA: 0x0000BB94 File Offset: 0x00009D94
	public event Action<CharacterMainControl, Vector3> OnSetPositionEvent;

	// Token: 0x1400000C RID: 12
	// (add) Token: 0x0600029F RID: 671 RVA: 0x0000BBCC File Offset: 0x00009DCC
	// (remove) Token: 0x060002A0 RID: 672 RVA: 0x0000BC04 File Offset: 0x00009E04
	public event Action<DamageInfo> BeforeCharacterSpawnLootOnDead;

	// Token: 0x17000079 RID: 121
	// (get) Token: 0x060002A1 RID: 673 RVA: 0x0000BC39 File Offset: 0x00009E39
	public Transform CurrentUsingAimSocket
	{
		get
		{
			if (this.agentHolder.CurrentUsingSocket == null || this.GetMeleeWeapon() != null)
			{
				return base.transform;
			}
			return this.agentHolder.CurrentUsingSocket;
		}
	}

	// Token: 0x1700007A RID: 122
	// (get) Token: 0x060002A2 RID: 674 RVA: 0x0000BC6E File Offset: 0x00009E6E
	public Transform RightHandSocket
	{
		get
		{
			if (this.characterModel && this.characterModel.RightHandSocket)
			{
				return this.characterModel.RightHandSocket;
			}
			return null;
		}
	}

	// Token: 0x1700007B RID: 123
	// (get) Token: 0x060002A3 RID: 675 RVA: 0x0000BC9C File Offset: 0x00009E9C
	public Vector3 CurrentAimDirection
	{
		get
		{
			return this.modelRoot.forward;
		}
	}

	// Token: 0x1700007C RID: 124
	// (get) Token: 0x060002A4 RID: 676 RVA: 0x0000BCA9 File Offset: 0x00009EA9
	public Vector3 CurrentMoveDirection
	{
		get
		{
			return this.movementControl.CurrentMoveDirectionXZ;
		}
	}

	// Token: 0x1700007D RID: 125
	// (get) Token: 0x060002A5 RID: 677 RVA: 0x0000BCB6 File Offset: 0x00009EB6
	public float AnimationMoveSpeedValue
	{
		get
		{
			return this.movementControl.GetMoveAnimationValue();
		}
	}

	// Token: 0x1700007E RID: 126
	// (get) Token: 0x060002A6 RID: 678 RVA: 0x0000BCC3 File Offset: 0x00009EC3
	public Vector2 AnimationLocalMoveDirectionValue
	{
		get
		{
			return this.movementControl.GetLocalMoveDirectionAnimationValue();
		}
	}

	// Token: 0x1400000D RID: 13
	// (add) Token: 0x060002A7 RID: 679 RVA: 0x0000BCD0 File Offset: 0x00009ED0
	// (remove) Token: 0x060002A8 RID: 680 RVA: 0x0000BD04 File Offset: 0x00009F04
	public static event Action<Item> OnMainCharacterStartUseItem;

	// Token: 0x1700007F RID: 127
	// (get) Token: 0x060002A9 RID: 681 RVA: 0x0000BD37 File Offset: 0x00009F37
	public bool Running
	{
		get
		{
			return this.movementControl.Running;
		}
	}

	// Token: 0x17000080 RID: 128
	// (get) Token: 0x060002AA RID: 682 RVA: 0x0000BD44 File Offset: 0x00009F44
	public bool IsOnGround
	{
		get
		{
			return this.movementControl.IsOnGround;
		}
	}

	// Token: 0x17000081 RID: 129
	// (get) Token: 0x060002AB RID: 683 RVA: 0x0000BD51 File Offset: 0x00009F51
	public Vector3 Velocity
	{
		get
		{
			return this.movementControl.Velocity;
		}
	}

	// Token: 0x17000082 RID: 130
	// (get) Token: 0x060002AC RID: 684 RVA: 0x0000BD60 File Offset: 0x00009F60
	public bool ThermalOn
	{
		get
		{
			int num = Mathf.RoundToInt(this.NightVisionType);
			return GameManager.NightVision.nightVisionTypes[num].thermalOn;
		}
	}

	// Token: 0x17000083 RID: 131
	// (get) Token: 0x060002AD RID: 685 RVA: 0x0000BD8E File Offset: 0x00009F8E
	public bool IsInAdsInput
	{
		get
		{
			return this.adsInput && (!(this.CurrentAction != null) || !this.CurrentAction.Running) && !this.Running;
		}
	}

	// Token: 0x17000084 RID: 132
	// (get) Token: 0x060002AE RID: 686 RVA: 0x0000BDC4 File Offset: 0x00009FC4
	public float AdsValue
	{
		get
		{
			ItemAgent_Gun gun = this.GetGun();
			if (gun)
			{
				return gun.AdsValue;
			}
			if (this.CurrentAction != null && this.CurrentAction.Running)
			{
				return 0f;
			}
			if (this.Running)
			{
				return 0f;
			}
			return (float)(this.adsInput ? 1 : 0);
		}
	}

	// Token: 0x17000085 RID: 133
	// (get) Token: 0x060002AF RID: 687 RVA: 0x0000BE23 File Offset: 0x0000A023
	public AimTypes AimType
	{
		get
		{
			return this.aimType;
		}
	}

	// Token: 0x060002B0 RID: 688 RVA: 0x0000BE2C File Offset: 0x0000A02C
	public float GetAimRange()
	{
		float num = 8f;
		switch (this.aimType)
		{
		case AimTypes.normalAim:
		{
			ItemAgent_Gun gun = this.GetGun();
			if (gun != null)
			{
				num = gun.BulletDistance;
				num -= 0.4f;
			}
			else
			{
				ItemAgent_MeleeWeapon meleeWeapon = this.GetMeleeWeapon();
				if (meleeWeapon != null)
				{
					num = meleeWeapon.AttackRange;
				}
			}
			break;
		}
		case AimTypes.characterSkill:
		{
			SkillBase skill = this.skillAction.characterSkillKeeper.Skill;
			if (skill)
			{
				num = skill.SkillContext.castRange;
			}
			break;
		}
		case AimTypes.handheldSkill:
		{
			ItemSetting_Skill skill2 = this.agentHolder.Skill;
			if (skill2)
			{
				SkillBase skill3 = skill2.Skill;
				if (skill3)
				{
					num = skill3.SkillContext.castRange;
				}
			}
			break;
		}
		}
		return num;
	}

	// Token: 0x17000086 RID: 134
	// (get) Token: 0x060002B1 RID: 689 RVA: 0x0000BEF6 File Offset: 0x0000A0F6
	public bool NeedToSearchTarget
	{
		get
		{
			return InputManager.InputDevice == InputManager.InputDevices.touch && (this.GetGun() || this.GetMeleeWeapon());
		}
	}

	// Token: 0x060002B2 RID: 690 RVA: 0x0000BF1F File Offset: 0x0000A11F
	public Vector3 GetCurrentAimPoint()
	{
		return this.inputAimPoint;
	}

	// Token: 0x060002B3 RID: 691 RVA: 0x0000BF28 File Offset: 0x0000A128
	public Vector3 GetCurrentSkillAimPoint()
	{
		SkillBase currentRunningSkill = this.skillAction.CurrentRunningSkill;
		if (!currentRunningSkill)
		{
			return this.inputAimPoint;
		}
		float castRange = currentRunningSkill.SkillContext.castRange;
		float y = this.inputAimPoint.y;
		Vector3 vector = this.inputAimPoint - base.transform.position;
		vector.y = 0f;
		float num = vector.magnitude;
		vector.Normalize();
		if (num > castRange)
		{
			num = castRange;
		}
		Vector3 vector2 = base.transform.position + vector * num;
		vector2.y = y;
		return vector2;
	}

	// Token: 0x17000087 RID: 135
	// (get) Token: 0x060002B4 RID: 692 RVA: 0x0000BFC7 File Offset: 0x0000A1C7
	public CharacterActionBase CurrentAction
	{
		get
		{
			return this.currentAction;
		}
	}

	// Token: 0x1400000E RID: 14
	// (add) Token: 0x060002B5 RID: 693 RVA: 0x0000BFD0 File Offset: 0x0000A1D0
	// (remove) Token: 0x060002B6 RID: 694 RVA: 0x0000C008 File Offset: 0x0000A208
	public event Action<CharacterActionBase> OnActionStartEvent;

	// Token: 0x1400000F RID: 15
	// (add) Token: 0x060002B7 RID: 695 RVA: 0x0000C040 File Offset: 0x0000A240
	// (remove) Token: 0x060002B8 RID: 696 RVA: 0x0000C078 File Offset: 0x0000A278
	public event Action<CharacterActionBase> OnActionProgressFinishEvent;

	// Token: 0x14000010 RID: 16
	// (add) Token: 0x060002B9 RID: 697 RVA: 0x0000C0B0 File Offset: 0x0000A2B0
	// (remove) Token: 0x060002BA RID: 698 RVA: 0x0000C0E8 File Offset: 0x0000A2E8
	public event Action<DuckovItemAgent> OnHoldAgentChanged;

	// Token: 0x14000011 RID: 17
	// (add) Token: 0x060002BB RID: 699 RVA: 0x0000C120 File Offset: 0x0000A320
	// (remove) Token: 0x060002BC RID: 700 RVA: 0x0000C158 File Offset: 0x0000A358
	public event Action<DuckovItemAgent> OnShootEvent;

	// Token: 0x14000012 RID: 18
	// (add) Token: 0x060002BD RID: 701 RVA: 0x0000C190 File Offset: 0x0000A390
	// (remove) Token: 0x060002BE RID: 702 RVA: 0x0000C1C8 File Offset: 0x0000A3C8
	public event Action TryCatchFishInputEvent;

	// Token: 0x17000088 RID: 136
	// (get) Token: 0x060002BF RID: 703 RVA: 0x0000C1FD File Offset: 0x0000A3FD
	public Health Health
	{
		get
		{
			return this.health;
		}
	}

	// Token: 0x17000089 RID: 137
	// (get) Token: 0x060002C0 RID: 704 RVA: 0x0000C205 File Offset: 0x0000A405
	public Vector3 MoveInput
	{
		get
		{
			return this.movementControl.MoveInput;
		}
	}

	// Token: 0x1700008A RID: 138
	// (get) Token: 0x060002C1 RID: 705 RVA: 0x0000C212 File Offset: 0x0000A412
	public CharacterEquipmentController EquipmentController
	{
		get
		{
			return this.equipmentController;
		}
	}

	// Token: 0x14000013 RID: 19
	// (add) Token: 0x060002C2 RID: 706 RVA: 0x0000C21C File Offset: 0x0000A41C
	// (remove) Token: 0x060002C3 RID: 707 RVA: 0x0000C254 File Offset: 0x0000A454
	public event Action<DuckovItemAgent> OnAttackEvent;

	// Token: 0x14000014 RID: 20
	// (add) Token: 0x060002C4 RID: 708 RVA: 0x0000C28C File Offset: 0x0000A48C
	// (remove) Token: 0x060002C5 RID: 709 RVA: 0x0000C2C4 File Offset: 0x0000A4C4
	public event Action OnSkillStartReleaseEvent;

	// Token: 0x1700008B RID: 139
	// (get) Token: 0x060002C6 RID: 710 RVA: 0x0000C2F9 File Offset: 0x0000A4F9
	public bool Dashing
	{
		get
		{
			return this.dashAction != null && this.dashAction.Running;
		}
	}

	// Token: 0x060002C7 RID: 711 RVA: 0x0000C316 File Offset: 0x0000A516
	private void OnMainCharacterInventoryChanged(Inventory inventory, int index)
	{
		ItemUtilities.NotifyPlayerItemOperation();
		Action<CharacterMainControl, Inventory, int> onMainCharacterInventoryChangedEvent = CharacterMainControl.OnMainCharacterInventoryChangedEvent;
		if (onMainCharacterInventoryChangedEvent == null)
		{
			return;
		}
		onMainCharacterInventoryChangedEvent(this, inventory, index);
	}

	// Token: 0x060002C8 RID: 712 RVA: 0x0000C32F File Offset: 0x0000A52F
	private void OnMainCharacterSlotContentChanged(Item item, Slot slot)
	{
		Action<CharacterMainControl, Slot> onMainCharacterSlotContentChangedEvent = CharacterMainControl.OnMainCharacterSlotContentChangedEvent;
		if (onMainCharacterSlotContentChangedEvent != null)
		{
			onMainCharacterSlotContentChangedEvent(this, slot);
		}
		this.CheckTakeoutWeaponWhileEquip(slot);
	}

	// Token: 0x060002C9 RID: 713 RVA: 0x0000C34C File Offset: 0x0000A54C
	private void CheckTakeoutWeaponWhileEquip(Slot slot)
	{
		if (slot.Content == null)
		{
			return;
		}
		if (this.CurrentHoldItemAgent != null)
		{
			return;
		}
		if (!slot.Content.Tags.Contains("Weapon"))
		{
			return;
		}
		this.agentHolder.ChangeHoldItem(slot.Content);
	}

	// Token: 0x060002CA RID: 714 RVA: 0x0000C3A4 File Offset: 0x0000A5A4
	public void SwitchWeapon(int dir)
	{
		this.weaponsTemp.Clear();
		this.weaponSwitchIndex = -1;
		Item item = null;
		if (this.CurrentHoldItemAgent != null)
		{
			item = this.CurrentHoldItemAgent.Item;
		}
		Item item2 = this.PrimWeaponSlot().Content;
		if (item2)
		{
			this.weaponsTemp.Add(item2);
			if (item == item2)
			{
				this.weaponSwitchIndex = this.weaponsTemp.Count - 1;
			}
		}
		item2 = this.SecWeaponSlot().Content;
		if (item2)
		{
			this.weaponsTemp.Add(item2);
			if (item == item2)
			{
				this.weaponSwitchIndex = this.weaponsTemp.Count - 1;
			}
		}
		item2 = this.MeleeWeaponSlot().Content;
		if (item2)
		{
			this.weaponsTemp.Add(item2);
			if (item == item2)
			{
				this.weaponSwitchIndex = this.weaponsTemp.Count - 1;
			}
		}
		if (this.weaponsTemp.Count <= 0)
		{
			return;
		}
		this.weaponSwitchIndex -= dir;
		if (this.weaponSwitchIndex < 0)
		{
			this.weaponSwitchIndex = this.weaponsTemp.Count - 1;
		}
		if (this.weaponSwitchIndex >= this.weaponsTemp.Count)
		{
			this.weaponSwitchIndex = 0;
		}
		this.ChangeHoldItem(this.weaponsTemp[this.weaponSwitchIndex]);
	}

	// Token: 0x1700008C RID: 140
	// (get) Token: 0x060002CB RID: 715 RVA: 0x0000C4FE File Offset: 0x0000A6FE
	public bool IsMainCharacter
	{
		get
		{
			return !(LevelManager.Instance == null) && LevelManager.Instance.MainCharacter == this;
		}
	}

	// Token: 0x060002CC RID: 716 RVA: 0x0000C51F File Offset: 0x0000A71F
	public bool CanEditInventory()
	{
		return !this.currentAction || !this.currentAction.Running || this.currentAction.CanEditInventory();
	}

	// Token: 0x060002CD RID: 717 RVA: 0x0000C54B File Offset: 0x0000A74B
	public void SetMoveInput(Vector3 moveInput)
	{
		this.movementControl.SetMoveInput(moveInput);
	}

	// Token: 0x060002CE RID: 718 RVA: 0x0000C559 File Offset: 0x0000A759
	public Slot MeleeWeaponSlot()
	{
		return this.GetSlot(this.meleeWeaponSlotHash);
	}

	// Token: 0x060002CF RID: 719 RVA: 0x0000C567 File Offset: 0x0000A767
	public Slot PrimWeaponSlot()
	{
		return this.GetSlot(this.primWeaponSlotHash);
	}

	// Token: 0x060002D0 RID: 720 RVA: 0x0000C575 File Offset: 0x0000A775
	public Slot SecWeaponSlot()
	{
		return this.GetSlot(this.secWeaponSlotHash);
	}

	// Token: 0x060002D1 RID: 721 RVA: 0x0000C583 File Offset: 0x0000A783
	public Slot GetSlot(int hash)
	{
		if (this.characterItem == null)
		{
			return null;
		}
		return this.characterItem.Slots.GetSlot(hash);
	}

	// Token: 0x060002D2 RID: 722 RVA: 0x0000C5A6 File Offset: 0x0000A7A6
	private void Awake()
	{
		this.nearByHalfObsticles = new HashSet<GameObject>();
		this.agentHolder.OnHoldAgentChanged += this.OnChangeItemAgentChangedFunc;
	}

	// Token: 0x060002D3 RID: 723 RVA: 0x0000C5CC File Offset: 0x0000A7CC
	private void StoreHoldWeaponBeforeUse()
	{
		if (this.agentHolder.CurrentHoldItemAgent)
		{
			Item item = this.agentHolder.CurrentHoldItemAgent.Item;
			if (item == this.MeleeWeaponSlot().Content)
			{
				this.holdWeaponBeforeUse = -1;
				return;
			}
			if (item == this.PrimWeaponSlot().Content)
			{
				this.holdWeaponBeforeUse = 0;
				return;
			}
			if (item == this.SecWeaponSlot().Content)
			{
				this.holdWeaponBeforeUse = 1;
				return;
			}
		}
	}

	// Token: 0x060002D4 RID: 724 RVA: 0x0000C64D File Offset: 0x0000A84D
	public bool SwitchToFirstAvailableWeapon()
	{
		return this.SwitchToWeapon(0) || this.SwitchToWeapon(1) || this.SwitchToWeapon(-1);
	}

	// Token: 0x060002D5 RID: 725 RVA: 0x0000C66C File Offset: 0x0000A86C
	public bool SwitchToWeapon(int index)
	{
		Slot slot = null;
		if (index == -1)
		{
			slot = this.MeleeWeaponSlot();
		}
		if (index == 0)
		{
			slot = this.PrimWeaponSlot();
		}
		else if (index == 1)
		{
			slot = this.SecWeaponSlot();
		}
		if (slot == null)
		{
			return false;
		}
		Item content = slot.Content;
		if (content == null)
		{
			return false;
		}
		this.ChangeHoldItem(content);
		return true;
	}

	// Token: 0x060002D6 RID: 726 RVA: 0x0000C6C0 File Offset: 0x0000A8C0
	public void ToggleNightVision()
	{
		Item faceMaskItem = this.GetFaceMaskItem();
		if (!faceMaskItem)
		{
			return;
		}
		ItemSetting_NightVision component = faceMaskItem.GetComponent<ItemSetting_NightVision>();
		if (!component)
		{
			return;
		}
		component.ToggleNightVison();
	}

	// Token: 0x060002D7 RID: 727 RVA: 0x0000C6F4 File Offset: 0x0000A8F4
	public void Dash()
	{
		if (this.dashAction == null)
		{
			return;
		}
		if (this.attackAction.Running && !this.attackAction.DamageDealed)
		{
			return;
		}
		if (this.StartAction(this.dashAction) && !this.DashCanControl && this.disableTriggerTimer < 0.6f)
		{
			this.disableTriggerTimer = 0.6f;
		}
	}

	// Token: 0x060002D8 RID: 728 RVA: 0x0000C75C File Offset: 0x0000A95C
	public void TryCatchFishInput()
	{
		if (!this.currentAction || !this.currentAction.Running)
		{
			return;
		}
		Action_FishingV2 action_FishingV = this.currentAction as Action_FishingV2;
		if (action_FishingV)
		{
			action_FishingV.TryCatch();
		}
	}

	// Token: 0x060002D9 RID: 729 RVA: 0x0000C7A0 File Offset: 0x0000A9A0
	public bool HasNearByHalfObsticle()
	{
		if (this.nearByHalfObsticles.Count <= 0)
		{
			return false;
		}
		using (HashSet<GameObject>.Enumerator enumerator = this.nearByHalfObsticles.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current != null)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x060002DA RID: 730 RVA: 0x0000C80C File Offset: 0x0000AA0C
	public void SwitchToWeaponBeforeUse()
	{
		this.SwitchToWeapon(this.holdWeaponBeforeUse);
		this.holdWeaponBeforeUse = -1;
	}

	// Token: 0x060002DB RID: 731 RVA: 0x0000C822 File Offset: 0x0000AA22
	public void SetForceMoveVelocity(Vector3 _velocity)
	{
		this.movementControl.SetForceMoveVelocity(_velocity);
	}

	// Token: 0x060002DC RID: 732 RVA: 0x0000C830 File Offset: 0x0000AA30
	public void SetAimPoint(Vector3 _aimPoint)
	{
		this.inputAimPoint = _aimPoint;
	}

	// Token: 0x060002DD RID: 733 RVA: 0x0000C83C File Offset: 0x0000AA3C
	public bool Attack()
	{
		if (this.GetMeleeWeapon() == null)
		{
			return false;
		}
		if (!this.attackAction.IsReady())
		{
			return false;
		}
		bool flag = this.StartAction(this.attackAction);
		Action<DuckovItemAgent> onAttackEvent = this.OnAttackEvent;
		if (onAttackEvent == null)
		{
			return flag;
		}
		onAttackEvent(this.GetMeleeWeapon());
		return flag;
	}

	// Token: 0x060002DE RID: 734 RVA: 0x0000C88A File Offset: 0x0000AA8A
	public void SetAimType(AimTypes _aimType)
	{
		this.aimType = _aimType;
	}

	// Token: 0x060002DF RID: 735 RVA: 0x0000C893 File Offset: 0x0000AA93
	public void SetRunInput(bool _runInput)
	{
		this.runInput = _runInput;
	}

	// Token: 0x060002E0 RID: 736 RVA: 0x0000C89C File Offset: 0x0000AA9C
	public void SetAdsInput(bool _adsInput)
	{
		this.adsInput = _adsInput;
	}

	// Token: 0x060002E1 RID: 737 RVA: 0x0000C8A5 File Offset: 0x0000AAA5
	public bool TryToReload(Item preferedBulletToLoad = null)
	{
		this.reloadAction.preferedBulletToReload = preferedBulletToLoad;
		bool flag = this.StartAction(this.reloadAction);
		if (!flag)
		{
			this.reloadAction.preferedBulletToReload = null;
		}
		return flag;
	}

	// Token: 0x060002E2 RID: 738 RVA: 0x0000C8CE File Offset: 0x0000AACE
	public bool SetSkill(SkillTypes skillType, SkillBase skill, GameObject bindingObject)
	{
		return this.skillAction.SetSkillOfType(skillType, skill, bindingObject);
	}

	// Token: 0x060002E3 RID: 739 RVA: 0x0000C8DE File Offset: 0x0000AADE
	public bool StartSkillAim(SkillTypes skillType)
	{
		if (this.skillAction.Running)
		{
			return false;
		}
		this.skillAction.SetNextSkillType(skillType);
		return this.StartAction(this.skillAction);
	}

	// Token: 0x060002E4 RID: 740 RVA: 0x0000C907 File Offset: 0x0000AB07
	public bool ReleaseSkill(SkillTypes skillType)
	{
		Action onSkillStartReleaseEvent = this.OnSkillStartReleaseEvent;
		if (onSkillStartReleaseEvent != null)
		{
			onSkillStartReleaseEvent();
		}
		return this.skillAction.ReleaseSkill(skillType);
	}

	// Token: 0x060002E5 RID: 741 RVA: 0x0000C926 File Offset: 0x0000AB26
	public bool CancleSkill()
	{
		return this.skillAction.StopAction();
	}

	// Token: 0x060002E6 RID: 742 RVA: 0x0000C933 File Offset: 0x0000AB33
	public SkillBase GetCurrentRunningSkill()
	{
		if (!this.skillAction.Running)
		{
			return null;
		}
		return this.skillAction.CurrentRunningSkill;
	}

	// Token: 0x060002E7 RID: 743 RVA: 0x0000C94F File Offset: 0x0000AB4F
	public bool GetGunReloadable()
	{
		return this.reloadAction.GetGunReloadable();
	}

	// Token: 0x060002E8 RID: 744 RVA: 0x0000C95C File Offset: 0x0000AB5C
	public bool CanUseHand()
	{
		return !(this.currentAction != null) || this.currentAction.CanUseHand();
	}

	// Token: 0x060002E9 RID: 745 RVA: 0x0000C97C File Offset: 0x0000AB7C
	public bool CanControlAim()
	{
		return !(this.currentAction != null) || this.currentAction.CanControlAim();
	}

	// Token: 0x060002EA RID: 746 RVA: 0x0000C99C File Offset: 0x0000AB9C
	public bool StartAction(CharacterActionBase newAction)
	{
		if (!newAction.IsReady())
		{
			return false;
		}
		bool flag = true;
		if (this.currentAction && this.currentAction.Running)
		{
			flag = newAction.ActionPriority() > this.currentAction.ActionPriority() && this.currentAction.StopAction();
		}
		if (flag)
		{
			this.currentAction = null;
			if (newAction.StartActionByCharacter(this))
			{
				this.currentAction = newAction;
				Action<CharacterActionBase> onActionStartEvent = this.OnActionStartEvent;
				if (onActionStartEvent != null)
				{
					onActionStartEvent(this.currentAction);
				}
				return true;
			}
		}
		return false;
	}

	// Token: 0x060002EB RID: 747 RVA: 0x0000CA26 File Offset: 0x0000AC26
	public void SwitchHoldAgentInSlot(int slotHash)
	{
		Slot slot = this.characterItem.Slots.GetSlot(slotHash);
		this.ChangeHoldItem((slot != null) ? slot.Content : null);
	}

	// Token: 0x060002EC RID: 748 RVA: 0x0000CA4C File Offset: 0x0000AC4C
	public void SwitchInteractSelection(int dir)
	{
		this.interactAction.SwitchInteractable(dir);
	}

	// Token: 0x060002ED RID: 749 RVA: 0x0000CA5C File Offset: 0x0000AC5C
	public void SetTeam(Teams _team)
	{
		this.team = _team;
		this.health.team = this.team;
		Action<Teams> onTeamChanged = this.OnTeamChanged;
		if (onTeamChanged != null)
		{
			onTeamChanged(_team);
		}
		if (CharacterMainControl.Main == this)
		{
			this.characterItem.Inventory.onContentChanged -= this.OnMainCharacterInventoryChanged;
			this.characterItem.Inventory.onContentChanged += this.OnMainCharacterInventoryChanged;
			this.characterItem.onSlotContentChanged -= this.OnMainCharacterSlotContentChanged;
			this.characterItem.onSlotContentChanged += this.OnMainCharacterSlotContentChanged;
		}
		if (this.characterModel)
		{
			this.characterModel.SyncHiddenToMainCharacter();
		}
	}

	// Token: 0x060002EE RID: 750 RVA: 0x0000CB1E File Offset: 0x0000AD1E
	public ItemAgent_Gun GetGun()
	{
		return this.agentHolder.CurrentHoldGun;
	}

	// Token: 0x060002EF RID: 751 RVA: 0x0000CB2B File Offset: 0x0000AD2B
	public ItemAgent_MeleeWeapon GetMeleeWeapon()
	{
		return this.agentHolder.CurrentHoldMeleeWeapon;
	}

	// Token: 0x060002F0 RID: 752 RVA: 0x0000CB38 File Offset: 0x0000AD38
	public bool ChangeHoldItem(Item item)
	{
		if (!this.CanEditInventory())
		{
			return false;
		}
		if (this.agentHolder.CurrentHoldItemAgent != null && item == this.agentHolder.CurrentHoldItemAgent.Item)
		{
			return false;
		}
		this.agentHolder.ChangeHoldItem(item);
		return true;
	}

	// Token: 0x060002F1 RID: 753 RVA: 0x0000CB8A File Offset: 0x0000AD8A
	private void OnChangeItemAgentChangedFunc(DuckovItemAgent agent)
	{
		Action<DuckovItemAgent> onHoldAgentChanged = this.OnHoldAgentChanged;
		if (onHoldAgentChanged != null)
		{
			onHoldAgentChanged(agent);
		}
		if (this.IsMainCharacter)
		{
			Action<CharacterMainControl, DuckovItemAgent> onMainCharacterChangeHoldItemAgentEvent = CharacterMainControl.OnMainCharacterChangeHoldItemAgentEvent;
			if (onMainCharacterChangeHoldItemAgentEvent == null)
			{
				return;
			}
			onMainCharacterChangeHoldItemAgentEvent(this, agent);
		}
	}

	// Token: 0x060002F2 RID: 754 RVA: 0x0000CBB8 File Offset: 0x0000ADB8
	private void Update()
	{
		if (!LevelManager.LevelInited)
		{
			return;
		}
		this.interactAction.SearchInteractableAround();
		this.UpdateAction(Time.deltaTime);
		this.movementControl.UpdateMovement();
		this.UpdateStats(Time.deltaTime);
		this.TickVariables(Time.deltaTime, 1f);
		if (this.IsMainCharacter)
		{
			this.UpdateThirstyAndStarve();
			this.UpdateWeightState();
		}
		this.disableTriggerTimer -= Time.deltaTime;
	}

	// Token: 0x060002F3 RID: 755 RVA: 0x0000CC2F File Offset: 0x0000AE2F
	private void LateUpdate()
	{
		this.UpdateInventoryCapacity();
	}

	// Token: 0x060002F4 RID: 756 RVA: 0x0000CC38 File Offset: 0x0000AE38
	public void SetItem(Item _item)
	{
		if (_item == null)
		{
			return;
		}
		this.characterItem = _item;
		_item.transform.SetParent(base.transform, false);
		this.currentStamina = this.MaxStamina;
		this.health.SetItemAndCharacter(_item, this);
		this.health.OnDeadEvent.AddListener(new UnityAction<DamageInfo>(this.OnDead));
		this.equipmentController.SetItem(_item);
		_item.Inventory.SetCapacity(Mathf.RoundToInt(this.InventoryCapacity));
		this.health.Init();
	}

	// Token: 0x060002F5 RID: 757 RVA: 0x0000CCCC File Offset: 0x0000AECC
	private void UpdateInventoryCapacity()
	{
		if (LevelManager.Instance.MainCharacter != this)
		{
			return;
		}
		if (this.characterItem == null || this.characterItem.Inventory == null || this.characterItem.Inventory.Loading)
		{
			return;
		}
		int num = Mathf.RoundToInt(this.InventoryCapacity);
		int capacity = this.characterItem.Inventory.Capacity;
		if (capacity == num)
		{
			return;
		}
		this.characterItem.Inventory.SetCapacity(num);
		if (capacity > num)
		{
			int count = this.characterItem.Inventory.Content.Count;
			if (count >= num)
			{
				List<Item> list = new List<Item>();
				for (int i = num; i < count; i++)
				{
					Item item = this.characterItem.Inventory.Content[i];
					if (item != null)
					{
						list.Add(item);
						item.Detach();
					}
				}
				foreach (Item item2 in list)
				{
					if (!this.characterItem.Inventory.AddAndMerge(item2, 0))
					{
						item2.Drop(base.transform.position, true, Vector3.forward, 360f);
					}
				}
			}
		}
	}

	// Token: 0x060002F6 RID: 758 RVA: 0x0000CE30 File Offset: 0x0000B030
	private void OnDead(DamageInfo dmgInfo)
	{
		if (LevelManager.Instance.MainCharacter != this)
		{
			Quaternion quaternion = Quaternion.identity;
			if (this.characterModel)
			{
				quaternion = this.characterModel.transform.rotation;
			}
			if (dmgInfo.fromCharacter && dmgInfo.fromCharacter.IsMainCharacter && this.characterPreset && this.characterPreset.nameKey != "")
			{
				SavesCounter.AddKillCount(this.characterPreset.nameKey);
			}
			Action<DamageInfo> beforeCharacterSpawnLootOnDead = this.BeforeCharacterSpawnLootOnDead;
			if (beforeCharacterSpawnLootOnDead != null)
			{
				beforeCharacterSpawnLootOnDead(dmgInfo);
			}
			InteractableLootbox.CreateFromItem(this.characterItem, base.transform.position + Vector3.up * 0.1f, quaternion, true, this.deadLootBoxPrefab, this.IsMainCharacter);
		}
		if (this.relatedScene != -1)
		{
			SetActiveByPlayerDistance.Unregister(base.gameObject, this.relatedScene);
		}
	}

	// Token: 0x060002F7 RID: 759 RVA: 0x0000CF2C File Offset: 0x0000B12C
	public void Trigger(bool trigger, bool triggerThisFrame, bool releaseThisFrame)
	{
		if (this.Running || this.disableTriggerTimer > 0f)
		{
			trigger = false;
			triggerThisFrame = false;
		}
		else if (trigger && this.CharacterMoveability > 0.5f)
		{
			this.movementControl.ForceSetAimDirectionToAimPoint();
		}
		this.agentHolder.SetTrigger(trigger, triggerThisFrame, releaseThisFrame);
	}

	// Token: 0x060002F8 RID: 760 RVA: 0x0000CF7F File Offset: 0x0000B17F
	public bool CanMove()
	{
		return (!(this.currentAction != null) || this.currentAction.CanMove()) && this.CharacterWalkSpeed > 0f;
	}

	// Token: 0x060002F9 RID: 761 RVA: 0x0000CFB0 File Offset: 0x0000B1B0
	public void PopText(string text, float speed = -1f)
	{
		if (!LevelManager.LevelInited || !CharacterMainControl.Main)
		{
			return;
		}
		if (Vector3.Distance(base.transform.position, CharacterMainControl.Main.transform.position) > 55f)
		{
			return;
		}
		float num = 2f;
		if (this.characterModel && this.characterModel.HelmatSocket)
		{
			num = Vector3.Distance(base.transform.position, this.characterModel.HelmatSocket.position) + 0.5f;
		}
		DialogueBubblesManager.Show(text, base.transform, num, false, false, speed, 2f).Forget();
	}

	// Token: 0x060002FA RID: 762 RVA: 0x0000D060 File Offset: 0x0000B260
	public bool CanRun()
	{
		if (this.currentAction != null && !this.currentAction.CanRun())
		{
			return false;
		}
		float num = this.currentStamina / this.MaxStamina;
		return (num >= 0.2f || this.Running) && num > 0f && this.runInput;
	}

	// Token: 0x060002FB RID: 763 RVA: 0x0000D0BA File Offset: 0x0000B2BA
	public bool IsAiming()
	{
		return !this.movementControl.Running && (this.currentAction == null || !this.currentAction.Running || this.currentAction.CanControlAim());
	}

	// Token: 0x060002FC RID: 764 RVA: 0x0000D0F4 File Offset: 0x0000B2F4
	public void DestroyCharacter()
	{
		global::UnityEngine.Object.Destroy(base.gameObject);
	}

	// Token: 0x060002FD RID: 765 RVA: 0x0000D101 File Offset: 0x0000B301
	public void TriggerShootEvent(DuckovItemAgent shootByAgent)
	{
		Action<DuckovItemAgent> onShootEvent = this.OnShootEvent;
		if (onShootEvent == null)
		{
			return;
		}
		onShootEvent(shootByAgent);
	}

	// Token: 0x060002FE RID: 766 RVA: 0x0000D114 File Offset: 0x0000B314
	public void SetCharacterModel(CharacterModel _characterModel)
	{
		bool flag = true;
		if (this.characterModel != null)
		{
			flag = false;
			global::UnityEngine.Object.Destroy(this.characterModel.gameObject);
		}
		this.characterModel = _characterModel;
		_characterModel.OnMainCharacterSetted(this);
		_characterModel.transform.SetParent(this.modelRoot, false);
		_characterModel.transform.localPosition = Vector3.zero;
		_characterModel.transform.localRotation = quaternion.identity;
		Transform helmatSocket = _characterModel.HelmatSocket;
		if (helmatSocket)
		{
			HeadCollider headCollider = global::UnityEngine.Object.Instantiate<HeadCollider>(GameplayDataSettings.Prefabs.HeadCollider, helmatSocket);
			headCollider.transform.localPosition = Vector3.zero;
			headCollider.transform.localScale = Vector3.one;
			headCollider.Init(this);
			CapsuleCollider component = this.mainDamageReceiver.GetComponent<CapsuleCollider>();
			if (component)
			{
				float num = headCollider.transform.localScale.y * 0.5f + headCollider.transform.position.y - base.transform.position.y + 0.5f;
				component.height = num;
				component.center = Vector3.up * num * 0.5f;
			}
		}
		if (LevelManager.LevelInited && !flag && this.characterItem != null)
		{
			foreach (Slot slot in this.characterItem.Slots)
			{
				if (slot.Content)
				{
					slot.ForceInvokeSlotContentChangedEvent();
				}
			}
		}
	}

	// Token: 0x060002FF RID: 767 RVA: 0x0000D2BC File Offset: 0x0000B4BC
	private void OnDestroy()
	{
		if (this.characterItem && this.characterItem.Inventory)
		{
			this.characterItem.Inventory.onContentChanged -= this.OnMainCharacterInventoryChanged;
		}
		if (this.characterItem)
		{
			this.characterItem.DestroyTree();
		}
		if (this.health)
		{
			this.health.OnDeadEvent.RemoveListener(new UnityAction<DamageInfo>(this.OnDead));
		}
		if (this.relatedScene != -1)
		{
			SetActiveByPlayerDistance.Unregister(base.gameObject, this.relatedScene);
		}
	}

	// Token: 0x06000300 RID: 768 RVA: 0x0000D360 File Offset: 0x0000B560
	private void UpdateAction(float deltaTime)
	{
		if (this.currentAction)
		{
			this.currentAction.UpdateAction(deltaTime);
			if (this.currentAction && !this.currentAction.Running)
			{
				this.currentAction = null;
			}
		}
	}

	// Token: 0x06000301 RID: 769 RVA: 0x0000D39C File Offset: 0x0000B59C
	private void UpdateStats(float deltaTime)
	{
		if (this.movementControl.Running)
		{
			this.UseStamina(this.StaminaDrainRate * deltaTime);
			return;
		}
		this.staminaRecoverTimer += deltaTime;
		if (this.staminaRecoverTimer >= this.StaminaRecoverTime)
		{
			this.currentStamina = Mathf.MoveTowards(this.currentStamina, this.MaxStamina, this.StaminaRecoverRate * deltaTime);
		}
	}

	// Token: 0x06000302 RID: 770 RVA: 0x0000D400 File Offset: 0x0000B600
	public void TickVariables(float deltaTime, float tickTime)
	{
		this.variableTickTimer += deltaTime;
		if (this.variableTickTimer < tickTime)
		{
			return;
		}
		this.variableTickTimer = 0f;
		if (!this.IsMainCharacter)
		{
			return;
		}
		float num = this.CurrentEnergy;
		if (!LevelManager.Instance.IsRaidMap || this.health.Invincible)
		{
			num += 10f * this.WaterEnergyRecoverMultiplier * tickTime / 60f;
			if (num < this.MaxEnergy * 0.25f)
			{
				num = this.MaxEnergy * 0.25f;
			}
			else if (num > this.MaxEnergy)
			{
				num = this.MaxEnergy;
			}
		}
		else
		{
			num -= this.EnergyCostPerMin * tickTime / 60f;
			if (num < 0f)
			{
				num = 0f;
			}
		}
		this.CurrentEnergy = num;
		float num2 = this.CurrentWater;
		if (!LevelManager.Instance.IsRaidMap || this.health.Invincible)
		{
			num2 += 10f * this.WaterEnergyRecoverMultiplier * tickTime / 60f;
			if (num2 < this.MaxWater * 0.25f)
			{
				num2 = this.MaxWater * 0.25f;
			}
			else if (num2 > this.MaxWater)
			{
				num2 = this.MaxWater;
			}
		}
		else
		{
			num2 -= this.WaterCostPerMin * tickTime / 60f;
			if (num2 < 0f)
			{
				num2 = 0f;
			}
		}
		this.CurrentWater = num2;
	}

	// Token: 0x06000303 RID: 771 RVA: 0x0000D554 File Offset: 0x0000B754
	public void UpdateThirstyAndStarve()
	{
		if (this.CurrentWater <= 0f != this.thirsty)
		{
			this.thirsty = !this.thirsty;
			if (this.thirsty)
			{
				this.AddBuff(GameplayDataSettings.Buffs.Thirsty, this, 0);
			}
			else
			{
				this.RemoveBuffsByTag(Buff.BuffExclusiveTags.Thirsty, false);
			}
		}
		if (this.CurrentEnergy <= 0f != this.starve)
		{
			this.starve = !this.starve;
			if (this.starve)
			{
				this.AddBuff(GameplayDataSettings.Buffs.Starve, this, 0);
				return;
			}
			this.RemoveBuffsByTag(Buff.BuffExclusiveTags.Starve, false);
		}
	}

	// Token: 0x06000304 RID: 772 RVA: 0x0000D5F8 File Offset: 0x0000B7F8
	public void UpdateWeightState()
	{
		float num = this.CharacterItem.TotalWeight;
		if (this.carryAction.Running)
		{
			num += this.carryAction.GetWeight();
		}
		float num2 = num / this.MaxWeight;
		CharacterMainControl.WeightStates weightStates = CharacterMainControl.WeightStates.light;
		if (!LevelManager.Instance.IsRaidMap)
		{
			weightStates = CharacterMainControl.WeightStates.normal;
		}
		else if (num2 > 1f)
		{
			weightStates = CharacterMainControl.WeightStates.overWeight;
		}
		else if (num2 > 0.75f)
		{
			weightStates = CharacterMainControl.WeightStates.superHeavy;
		}
		else if (num2 > 0.25f)
		{
			weightStates = CharacterMainControl.WeightStates.normal;
		}
		if (weightStates != this.weightState)
		{
			this.weightState = weightStates;
			this.RemoveBuffsByTag(Buff.BuffExclusiveTags.Weight, false);
			switch (weightStates)
			{
			case CharacterMainControl.WeightStates.light:
				this.AddBuff(GameplayDataSettings.Buffs.Weight_Light, this, 0);
				return;
			case CharacterMainControl.WeightStates.normal:
				break;
			case CharacterMainControl.WeightStates.heavy:
				this.AddBuff(GameplayDataSettings.Buffs.Weight_Heavy, this, 0);
				return;
			case CharacterMainControl.WeightStates.superHeavy:
				this.AddBuff(GameplayDataSettings.Buffs.Weight_SuperHeavy, this, 0);
				return;
			case CharacterMainControl.WeightStates.overWeight:
				this.AddBuff(GameplayDataSettings.Buffs.Weight_Overweight, this, 0);
				break;
			default:
				return;
			}
		}
	}

	// Token: 0x06000305 RID: 773 RVA: 0x0000D6E7 File Offset: 0x0000B8E7
	public bool PickupItem(Item item)
	{
		if (this.health.IsDead)
		{
			return false;
		}
		item.Inspected = true;
		return this.itemControl.PickupItem(item);
	}

	// Token: 0x06000306 RID: 774 RVA: 0x0000D70B File Offset: 0x0000B90B
	public InteractableBase GetInteractableTargetToInteract()
	{
		if (this.currentAction && this.currentAction.ActionPriority() >= this.interactAction.ActionPriority())
		{
			return null;
		}
		return this.interactAction.InteractTarget;
	}

	// Token: 0x06000307 RID: 775 RVA: 0x0000D73F File Offset: 0x0000B93F
	public void Interact(InteractableBase _target)
	{
		if (this.currentAction && this.currentAction.ActionPriority() >= this.interactAction.ActionPriority())
		{
			return;
		}
		this.interactAction.SetInteractableTarget(_target);
		this.Interact();
	}

	// Token: 0x06000308 RID: 776 RVA: 0x0000D77C File Offset: 0x0000B97C
	public void Interact()
	{
		if (this.health.IsDead)
		{
			return;
		}
		if (this.carryAction.Running)
		{
			this.carryAction.StopAction();
			return;
		}
		if (this.currentAction)
		{
			return;
		}
		if (this.GetInteractableTargetToInteract() != null)
		{
			this.StartAction(this.interactAction);
		}
	}

	// Token: 0x06000309 RID: 777 RVA: 0x0000D7DA File Offset: 0x0000B9DA
	public void AddHealth(float healthValue)
	{
		this.health.AddHealth(healthValue * (1f + this.HealGain));
	}

	// Token: 0x0600030A RID: 778 RVA: 0x0000D7F5 File Offset: 0x0000B9F5
	public void SetRelatedScene(int _relatedScene, bool setActiveByPlayerDistance = true)
	{
		this.relatedScene = _relatedScene;
		if (MultiSceneCore.Instance)
		{
			MultiSceneCore.MoveToActiveWithScene(base.gameObject, _relatedScene);
			if (setActiveByPlayerDistance)
			{
				SetActiveByPlayerDistance.Register(base.gameObject, this.relatedScene);
			}
		}
	}

	// Token: 0x0600030B RID: 779 RVA: 0x0000D82C File Offset: 0x0000BA2C
	public void Carry(Carriable target)
	{
		if (!this.carryAction)
		{
			return;
		}
		if (this.currentAction != null && this.currentAction.Running)
		{
			return;
		}
		this.carryAction.carryTarget = target;
		this.StartAction(this.carryAction);
	}

	// Token: 0x0600030C RID: 780 RVA: 0x0000D87C File Offset: 0x0000BA7C
	public void AddEnergy(float energyValue)
	{
		float num = this.CurrentEnergy;
		num += energyValue * (1f + this.FoodGain);
		if (num > this.MaxEnergy)
		{
			num = this.MaxEnergy;
		}
		if (num < 0f)
		{
			num = 0f;
		}
		this.CurrentEnergy = num;
	}

	// Token: 0x0600030D RID: 781 RVA: 0x0000D8C8 File Offset: 0x0000BAC8
	public void AddWater(float waterValue)
	{
		float num = this.CurrentWater;
		num += waterValue * (1f + this.FoodGain);
		if (num > this.MaxWater)
		{
			num = this.MaxWater;
		}
		if (num < 0f)
		{
			num = 0f;
		}
		this.CurrentWater = num;
	}

	// Token: 0x0600030E RID: 782 RVA: 0x0000D914 File Offset: 0x0000BB14
	public void DropAllItems()
	{
		if (this.characterItem == null)
		{
			return;
		}
		List<Item> list = new List<Item>();
		if (this.characterItem.Inventory != null)
		{
			foreach (Item item in this.characterItem.Inventory)
			{
				if ((!this.IsMainCharacter || !item.Tags.Contains(GameplayDataSettings.Tags.DontDropOnDeadInSlot)) && (!this.IsMainCharacter || !item.Sticky))
				{
					list.Add(item);
				}
			}
		}
		foreach (Slot slot in this.characterItem.Slots)
		{
			if (slot.Content != null && (!this.IsMainCharacter || !slot.Content.Tags.Contains(GameplayDataSettings.Tags.DontDropOnDeadInSlot)) && (!this.IsMainCharacter || !slot.Content.Sticky))
			{
				list.Add(slot.Content);
			}
		}
		foreach (Item item2 in list)
		{
			if (!this.IsMainCharacter || !item2.Sticky)
			{
				item2.Drop(base.transform.position, true, Vector3.forward, 360f);
			}
		}
	}

	// Token: 0x0600030F RID: 783 RVA: 0x0000DAB8 File Offset: 0x0000BCB8
	public void DestroyAllItem()
	{
		if (this.characterItem == null)
		{
			return;
		}
		List<Item> list = new List<Item>();
		if (this.characterItem.Inventory != null)
		{
			foreach (Item item in this.characterItem.Inventory)
			{
				list.Add(item);
			}
		}
		foreach (Slot slot in this.characterItem.Slots)
		{
			if (slot.Content != null && (!this.IsMainCharacter || !slot.Content.Tags.Contains(GameplayDataSettings.Tags.DontDropOnDeadInSlot)) && (!this.IsMainCharacter || !slot.Content.Sticky))
			{
				list.Add(slot.Content);
			}
		}
		foreach (Item item2 in list)
		{
			if (!this.IsMainCharacter || !item2.Sticky)
			{
				item2.DestroyTree();
			}
		}
	}

	// Token: 0x06000310 RID: 784 RVA: 0x0000DC14 File Offset: 0x0000BE14
	public void DestroyItemsThatNeededToBeDestriedInBase()
	{
		if (this.characterItem == null)
		{
			return;
		}
		List<Item> list = new List<Item>();
		if (this.characterItem.Inventory != null)
		{
			foreach (Item item in this.characterItem.Inventory)
			{
				list.Add(item);
			}
		}
		foreach (Slot slot in this.characterItem.Slots)
		{
			if (slot.Content != null)
			{
				list.Add(slot.Content);
			}
		}
		foreach (Item item2 in list)
		{
			if (item2.Tags.Contains("DestroyInBase"))
			{
				item2.DestroyTree();
			}
		}
	}

	// Token: 0x06000311 RID: 785 RVA: 0x0000DD38 File Offset: 0x0000BF38
	public void AddSubVisuals(CharacterSubVisuals subVisuals)
	{
		if (!this.characterModel)
		{
			return;
		}
		this.characterModel.AddSubVisuals(subVisuals);
	}

	// Token: 0x06000312 RID: 786 RVA: 0x0000DD54 File Offset: 0x0000BF54
	public void RemoveVisual(CharacterSubVisuals subVisuals)
	{
		if (!this.characterModel)
		{
			return;
		}
		this.characterModel.RemoveVisual(subVisuals);
	}

	// Token: 0x06000313 RID: 787 RVA: 0x0000DD70 File Offset: 0x0000BF70
	public void Hide()
	{
		if (this.hidden)
		{
			return;
		}
		this.hidden = true;
		if (!this.characterModel)
		{
			return;
		}
		this.characterModel.SyncHiddenToMainCharacter();
	}

	// Token: 0x06000314 RID: 788 RVA: 0x0000DD9B File Offset: 0x0000BF9B
	public void Show()
	{
		Health health = this.health;
		if (health != null)
		{
			health.RequestHealthBar();
		}
		if (!this.hidden)
		{
			return;
		}
		this.hidden = false;
		if (!this.characterModel)
		{
			return;
		}
		this.characterModel.SyncHiddenToMainCharacter();
	}

	// Token: 0x06000315 RID: 789 RVA: 0x0000DDD7 File Offset: 0x0000BFD7
	private void OnEnable()
	{
		if (this.IsMainCharacter && this.health)
		{
			this.health.showHealthBar = true;
			this.health.RequestHealthBar();
		}
		AudioManager.SetVoiceType(base.gameObject, this.audioVoiceType);
	}

	// Token: 0x06000316 RID: 790 RVA: 0x0000DE16 File Offset: 0x0000C016
	public bool IsNearByHalfObsticle(GameObject target)
	{
		return !(target == null) && this.nearByHalfObsticles.Count != 0 && this.nearByHalfObsticles.Contains(target);
	}

	// Token: 0x06000317 RID: 791 RVA: 0x0000DE3C File Offset: 0x0000C03C
	public GameObject[] GetNearByHalfObsticles()
	{
		this.nearByHalfObsticles.RemoveWhere((GameObject go) => go == null);
		return this.nearByHalfObsticles.ToArray<GameObject>();
	}

	// Token: 0x06000318 RID: 792 RVA: 0x0000DE74 File Offset: 0x0000C074
	public void AddnearByHalfObsticles(List<GameObject> objs)
	{
		foreach (GameObject gameObject in objs)
		{
			if (!(gameObject == null) && !this.nearByHalfObsticles.Contains(gameObject))
			{
				this.nearByHalfObsticles.Add(gameObject);
			}
		}
	}

	// Token: 0x06000319 RID: 793 RVA: 0x0000DEE0 File Offset: 0x0000C0E0
	public void RemoveNearByHalfObsticles(List<GameObject> objs)
	{
		foreach (GameObject gameObject in objs)
		{
			if (!(gameObject == null) && this.nearByHalfObsticles.Contains(gameObject))
			{
				this.nearByHalfObsticles.Remove(gameObject);
			}
		}
	}

	// Token: 0x0600031A RID: 794 RVA: 0x0000DF4C File Offset: 0x0000C14C
	public void UseItem(Item item)
	{
		if (this.IsMainCharacter && !item.UsageUtilities.IsUsable(item, this))
		{
			NotificationText.Push("UI_Item_NotUsable".ToPlainText());
			return;
		}
		this.StoreHoldWeaponBeforeUse();
		if (item.GetRoot() != this.characterItem)
		{
			Debug.Log("pick fail");
			item.Detach();
			item.AgentUtilities.ReleaseActiveAgent();
			item.transform.SetParent(base.transform);
		}
		if (this.interactAction.Running && this.interactAction.InteractingTarget is InteractableLootbox)
		{
			this.interactAction.StopAction();
		}
		this.useItemAction.SetUseItem(item);
		bool flag = this.StartAction(this.useItemAction);
		Debug.Log(string.Format("UseItemSuccess:{0}", flag));
		if (flag && this.IsMainCharacter)
		{
			Action<Item> onMainCharacterStartUseItem = CharacterMainControl.OnMainCharacterStartUseItem;
			if (onMainCharacterStartUseItem == null)
			{
				return;
			}
			onMainCharacterStartUseItem(item);
		}
	}

	// Token: 0x0600031B RID: 795 RVA: 0x0000E039 File Offset: 0x0000C239
	public CharacterBuffManager GetBuffManager()
	{
		return this.buffManager;
	}

	// Token: 0x0600031C RID: 796 RVA: 0x0000E044 File Offset: 0x0000C244
	public void AddBuff(Buff buffPrefab, CharacterMainControl fromWho = null, int overrideWeaponID = 0)
	{
		if (!buffPrefab)
		{
			return;
		}
		if (this.buffResist.Contains(buffPrefab.ExclusiveTag))
		{
			return;
		}
		Buff.BuffExclusiveTags exclusiveTag = buffPrefab.ExclusiveTag;
		if (exclusiveTag != Buff.BuffExclusiveTags.NotExclusive)
		{
			Buff buffByTag = this.buffManager.GetBuffByTag(exclusiveTag);
			if (buffByTag != null && buffByTag.ID != buffPrefab.ID)
			{
				if (buffByTag.ExclusiveTagPriority > buffPrefab.ExclusiveTagPriority)
				{
					return;
				}
				if (buffByTag.ExclusiveTagPriority == buffPrefab.ExclusiveTagPriority && buffByTag.LimitedLifeTime && buffPrefab.LimitedLifeTime && buffByTag.CurrentLifeTime > buffPrefab.TotalLifeTime)
				{
					buffByTag.fromWho = fromWho;
					if (overrideWeaponID > 0)
					{
						buffByTag.fromWeaponID = overrideWeaponID;
					}
					return;
				}
				this.buffManager.RemoveBuff(buffByTag, false);
			}
		}
		this.buffManager.AddBuff(buffPrefab, fromWho, overrideWeaponID);
	}

	// Token: 0x0600031D RID: 797 RVA: 0x0000E105 File Offset: 0x0000C305
	public void RemoveBuff(int buffID, bool removeOneLayer)
	{
		this.buffManager.RemoveBuff(buffID, removeOneLayer);
	}

	// Token: 0x0600031E RID: 798 RVA: 0x0000E114 File Offset: 0x0000C314
	public void RemoveBuffsByTag(Buff.BuffExclusiveTags tag, bool removeOneLayer)
	{
		this.buffManager.RemoveBuffsByTag(tag, removeOneLayer);
	}

	// Token: 0x0600031F RID: 799 RVA: 0x0000E123 File Offset: 0x0000C323
	public bool HasBuff(int buffID)
	{
		return this.buffManager.HasBuff(buffID);
	}

	// Token: 0x06000320 RID: 800 RVA: 0x0000E131 File Offset: 0x0000C331
	public void SetPosition(Vector3 pos)
	{
		this.movementControl.ForceSetPosition(pos);
		Action<CharacterMainControl, Vector3> onSetPositionEvent = this.OnSetPositionEvent;
		if (onSetPositionEvent == null)
		{
			return;
		}
		onSetPositionEvent(this, pos);
	}

	// Token: 0x06000321 RID: 801 RVA: 0x0000E154 File Offset: 0x0000C354
	public Item GetArmorItem()
	{
		Slot slot = this.characterItem.Slots["Armor"];
		if (slot == null)
		{
			return null;
		}
		return slot.Content;
	}

	// Token: 0x06000322 RID: 802 RVA: 0x0000E184 File Offset: 0x0000C384
	public Item GetHelmatItem()
	{
		Slot slot = this.characterItem.Slots["Helmat"];
		if (slot == null)
		{
			return null;
		}
		return slot.Content;
	}

	// Token: 0x06000323 RID: 803 RVA: 0x0000E1B4 File Offset: 0x0000C3B4
	public Item GetFaceMaskItem()
	{
		Slot slot = this.characterItem.Slots["FaceMask"];
		if (slot == null)
		{
			return null;
		}
		return slot.Content;
	}

	// Token: 0x06000324 RID: 804 RVA: 0x0000E1E4 File Offset: 0x0000C3E4
	public static float WeaponRepairLossFactor()
	{
		if (!CharacterMainControl.Main || CharacterMainControl.Main.characterItem == null)
		{
			return 1f;
		}
		return CharacterMainControl.Main.characterItem.Constants.GetFloat("WeaponRepairLossFactor", 1f);
	}

	// Token: 0x06000325 RID: 805 RVA: 0x0000E234 File Offset: 0x0000C434
	public static float EquipmentRepairLossFactor()
	{
		if (!CharacterMainControl.Main || CharacterMainControl.Main.characterItem == null)
		{
			return 1f;
		}
		return CharacterMainControl.Main.characterItem.Constants.GetFloat("EquipmentRepairLossFactor", 1f);
	}

	// Token: 0x06000326 RID: 806 RVA: 0x0000E283 File Offset: 0x0000C483
	private float GetFloatStatValue(int hash)
	{
		if (this.characterItem)
		{
			return this.characterItem.GetStatValue(hash);
		}
		return 0f;
	}

	// Token: 0x1700008D RID: 141
	// (get) Token: 0x06000327 RID: 807 RVA: 0x0000E2A4 File Offset: 0x0000C4A4
	public float CharacterWalkSpeed
	{
		get
		{
			float num = this.GetFloatStatValue(this.walkSpeedHash);
			num *= this.CharacterMoveability;
			ItemAgent_Gun gun = this.GetGun();
			if (gun)
			{
				float moveSpeedMultiplier = gun.MoveSpeedMultiplier;
				if (moveSpeedMultiplier > 0f)
				{
					num *= moveSpeedMultiplier;
				}
			}
			else
			{
				ItemAgent_MeleeWeapon meleeWeapon = this.GetMeleeWeapon();
				if (meleeWeapon)
				{
					float moveSpeedMultiplier2 = meleeWeapon.MoveSpeedMultiplier;
					if (moveSpeedMultiplier2 > 0f)
					{
						num *= moveSpeedMultiplier2;
					}
				}
			}
			return num;
		}
	}

	// Token: 0x1700008E RID: 142
	// (get) Token: 0x06000328 RID: 808 RVA: 0x0000E314 File Offset: 0x0000C514
	public float AdsWalkSpeedMultiplier
	{
		get
		{
			ItemAgent_Gun gun = this.GetGun();
			if (gun)
			{
				return gun.AdsWalkSpeedMultiplier;
			}
			return 0.5f;
		}
	}

	// Token: 0x1700008F RID: 143
	// (get) Token: 0x06000329 RID: 809 RVA: 0x0000E33C File Offset: 0x0000C53C
	public float CharacterOriginWalkSpeed
	{
		get
		{
			if (this.characterItem)
			{
				return this.characterItem.GetStat(this.walkSpeedHash).BaseValue;
			}
			return 0f;
		}
	}

	// Token: 0x17000090 RID: 144
	// (get) Token: 0x0600032A RID: 810 RVA: 0x0000E368 File Offset: 0x0000C568
	public float CharacterRunSpeed
	{
		get
		{
			float num = this.GetFloatStatValue(this.runSpeedHash);
			num *= this.CharacterMoveability;
			ItemAgent_Gun gun = this.GetGun();
			if (gun)
			{
				float moveSpeedMultiplier = gun.MoveSpeedMultiplier;
				if (moveSpeedMultiplier > 0f)
				{
					num *= moveSpeedMultiplier;
				}
			}
			else
			{
				ItemAgent_MeleeWeapon meleeWeapon = this.GetMeleeWeapon();
				if (meleeWeapon)
				{
					float moveSpeedMultiplier2 = meleeWeapon.MoveSpeedMultiplier;
					if (moveSpeedMultiplier2 > 0f)
					{
						num *= moveSpeedMultiplier2;
					}
				}
			}
			return num;
		}
	}

	// Token: 0x17000091 RID: 145
	// (get) Token: 0x0600032B RID: 811 RVA: 0x0000E3D5 File Offset: 0x0000C5D5
	public float StormProtection
	{
		get
		{
			return this.GetFloatStatValue(this.stormProtectionHash);
		}
	}

	// Token: 0x17000092 RID: 146
	// (get) Token: 0x0600032C RID: 812 RVA: 0x0000E3E3 File Offset: 0x0000C5E3
	public float WaterEnergyRecoverMultiplier
	{
		get
		{
			return this.GetFloatStatValue(this.waterEnergyRecoverMultiplierHash);
		}
	}

	// Token: 0x17000093 RID: 147
	// (get) Token: 0x0600032D RID: 813 RVA: 0x0000E3F1 File Offset: 0x0000C5F1
	public float GunDistanceMultiplier
	{
		get
		{
			return this.GetFloatStatValue(this.gunDistanceMultiplierHash);
		}
	}

	// Token: 0x17000094 RID: 148
	// (get) Token: 0x0600032E RID: 814 RVA: 0x0000E3FF File Offset: 0x0000C5FF
	public float CharacterMoveability
	{
		get
		{
			return this.GetFloatStatValue(this.moveabilityHash);
		}
	}

	// Token: 0x17000095 RID: 149
	// (get) Token: 0x0600032F RID: 815 RVA: 0x0000E40D File Offset: 0x0000C60D
	public float CharacterRunAcc
	{
		get
		{
			return this.GetFloatStatValue(this.runAccHash);
		}
	}

	// Token: 0x17000096 RID: 150
	// (get) Token: 0x06000330 RID: 816 RVA: 0x0000E41B File Offset: 0x0000C61B
	public float CharacterTurnSpeed
	{
		get
		{
			return this.GetFloatStatValue(this.turnSpeedHash) * this.CharacterMoveability;
		}
	}

	// Token: 0x17000097 RID: 151
	// (get) Token: 0x06000331 RID: 817 RVA: 0x0000E430 File Offset: 0x0000C630
	public float CharacterAimTurnSpeed
	{
		get
		{
			return this.GetFloatStatValue(this.aimTurnSpeedHash) * this.CharacterMoveability;
		}
	}

	// Token: 0x17000098 RID: 152
	// (get) Token: 0x06000332 RID: 818 RVA: 0x0000E445 File Offset: 0x0000C645
	public float DashSpeed
	{
		get
		{
			return this.GetFloatStatValue(this.dashSpeedHash);
		}
	}

	// Token: 0x17000099 RID: 153
	// (get) Token: 0x06000333 RID: 819 RVA: 0x0000E453 File Offset: 0x0000C653
	public int PetCapcity
	{
		get
		{
			return Mathf.RoundToInt(this.GetFloatStatValue(this.PetCapcityHash));
		}
	}

	// Token: 0x1700009A RID: 154
	// (get) Token: 0x06000334 RID: 820 RVA: 0x0000E466 File Offset: 0x0000C666
	// (set) Token: 0x06000335 RID: 821 RVA: 0x0000E48F File Offset: 0x0000C68F
	public bool DashCanControl
	{
		get
		{
			return !this.characterItem || this.characterItem.GetStatValue(this.dashCanControlHash) > 0f;
		}
		set
		{
			this.characterItem.GetStat(this.dashCanControlHash).BaseValue = (float)(value ? 1 : 0);
		}
	}

	// Token: 0x1700009B RID: 155
	// (get) Token: 0x06000336 RID: 822 RVA: 0x0000E4AF File Offset: 0x0000C6AF
	public float MaxStamina
	{
		get
		{
			if (this.characterItem)
			{
				return this.characterItem.GetStatValue(this.maxStaminaHash);
			}
			return 0f;
		}
	}

	// Token: 0x1700009C RID: 156
	// (get) Token: 0x06000337 RID: 823 RVA: 0x0000E4D5 File Offset: 0x0000C6D5
	public float CurrentStamina
	{
		get
		{
			return this.currentStamina;
		}
	}

	// Token: 0x1700009D RID: 157
	// (get) Token: 0x06000338 RID: 824 RVA: 0x0000E4DD File Offset: 0x0000C6DD
	public float StaminaDrainRate
	{
		get
		{
			if (this.characterItem)
			{
				return this.characterItem.GetStatValue(this.staminaDrainRateHash);
			}
			return 0f;
		}
	}

	// Token: 0x1700009E RID: 158
	// (get) Token: 0x06000339 RID: 825 RVA: 0x0000E503 File Offset: 0x0000C703
	public float StaminaRecoverRate
	{
		get
		{
			if (this.characterItem)
			{
				return this.characterItem.GetStatValue(this.staminaRecoverRateHash);
			}
			return 0f;
		}
	}

	// Token: 0x1700009F RID: 159
	// (get) Token: 0x0600033A RID: 826 RVA: 0x0000E529 File Offset: 0x0000C729
	public float StaminaRecoverTime
	{
		get
		{
			if (this.characterItem)
			{
				return this.characterItem.GetStatValue(this.staminaRecoverTimeHash);
			}
			return 0f;
		}
	}

	// Token: 0x170000A0 RID: 160
	// (get) Token: 0x0600033B RID: 827 RVA: 0x0000E54F File Offset: 0x0000C74F
	public float CharacterWalkAcc
	{
		get
		{
			return this.GetFloatStatValue(this.walkAccHash);
		}
	}

	// Token: 0x170000A1 RID: 161
	// (get) Token: 0x0600033C RID: 828 RVA: 0x0000E55D File Offset: 0x0000C75D
	public float VisableDistanceFactor
	{
		get
		{
			return this.GetFloatStatValue(this.visableDistanceFactorHash);
		}
	}

	// Token: 0x170000A2 RID: 162
	// (get) Token: 0x0600033D RID: 829 RVA: 0x0000E56B File Offset: 0x0000C76B
	public float MaxWeight
	{
		get
		{
			if (this.characterItem)
			{
				return this.characterItem.GetStatValue(this.maxWeightHash);
			}
			return 0f;
		}
	}

	// Token: 0x170000A3 RID: 163
	// (get) Token: 0x0600033E RID: 830 RVA: 0x0000E591 File Offset: 0x0000C791
	public float FoodGain
	{
		get
		{
			if (this.characterItem)
			{
				return this.characterItem.GetStatValue(this.foodGainHash);
			}
			return 0f;
		}
	}

	// Token: 0x170000A4 RID: 164
	// (get) Token: 0x0600033F RID: 831 RVA: 0x0000E5B7 File Offset: 0x0000C7B7
	public float HealGain
	{
		get
		{
			if (this.characterItem)
			{
				return this.characterItem.GetStatValue(this.healGainHash);
			}
			return 0f;
		}
	}

	// Token: 0x170000A5 RID: 165
	// (get) Token: 0x06000340 RID: 832 RVA: 0x0000E5DD File Offset: 0x0000C7DD
	public float MaxEnergy
	{
		get
		{
			if (this.characterItem)
			{
				return this.characterItem.GetStatValue(this.maxEnergyHash);
			}
			return 0f;
		}
	}

	// Token: 0x170000A6 RID: 166
	// (get) Token: 0x06000341 RID: 833 RVA: 0x0000E603 File Offset: 0x0000C803
	public float EnergyCostPerMin
	{
		get
		{
			if (this.characterItem)
			{
				return this.characterItem.GetStatValue(this.energyCostPerMinHash);
			}
			return 0f;
		}
	}

	// Token: 0x170000A7 RID: 167
	// (get) Token: 0x06000342 RID: 834 RVA: 0x0000E629 File Offset: 0x0000C829
	// (set) Token: 0x06000343 RID: 835 RVA: 0x0000E659 File Offset: 0x0000C859
	public float CurrentEnergy
	{
		get
		{
			if (this.characterItem)
			{
				return this.characterItem.Variables.GetFloat(this.currentEnergyHash, 0f);
			}
			return 0f;
		}
		set
		{
			if (this.characterItem)
			{
				this.characterItem.Variables.SetFloat(this.currentEnergyHash, value);
			}
		}
	}

	// Token: 0x170000A8 RID: 168
	// (get) Token: 0x06000344 RID: 836 RVA: 0x0000E67F File Offset: 0x0000C87F
	public float MaxWater
	{
		get
		{
			if (this.characterItem)
			{
				return this.characterItem.GetStatValue(this.maxWaterHash);
			}
			return 0f;
		}
	}

	// Token: 0x170000A9 RID: 169
	// (get) Token: 0x06000345 RID: 837 RVA: 0x0000E6A5 File Offset: 0x0000C8A5
	public float WaterCostPerMin
	{
		get
		{
			if (this.characterItem)
			{
				return this.characterItem.GetStatValue(this.waterCostPerMinHash);
			}
			return 0f;
		}
	}

	// Token: 0x170000AA RID: 170
	// (get) Token: 0x06000346 RID: 838 RVA: 0x0000E6CB File Offset: 0x0000C8CB
	// (set) Token: 0x06000347 RID: 839 RVA: 0x0000E6FB File Offset: 0x0000C8FB
	public float CurrentWater
	{
		get
		{
			if (this.characterItem)
			{
				return this.characterItem.Variables.GetFloat(this.currentWaterHash, 0f);
			}
			return 0f;
		}
		set
		{
			if (this.characterItem)
			{
				this.characterItem.Variables.SetFloat(this.currentWaterHash, value);
			}
		}
	}

	// Token: 0x170000AB RID: 171
	// (get) Token: 0x06000348 RID: 840 RVA: 0x0000E721 File Offset: 0x0000C921
	public float NightVisionAbility
	{
		get
		{
			return this.GetFloatStatValue(this.NightVisionAbilityHash);
		}
	}

	// Token: 0x170000AC RID: 172
	// (get) Token: 0x06000349 RID: 841 RVA: 0x0000E72F File Offset: 0x0000C92F
	public float NightVisionType
	{
		get
		{
			return this.GetFloatStatValue(this.NightVisionTypeHash);
		}
	}

	// Token: 0x170000AD RID: 173
	// (get) Token: 0x0600034A RID: 842 RVA: 0x0000E73D File Offset: 0x0000C93D
	public float HearingAbility
	{
		get
		{
			return this.GetFloatStatValue(this.HearingAbilityHash);
		}
	}

	// Token: 0x170000AE RID: 174
	// (get) Token: 0x0600034B RID: 843 RVA: 0x0000E74B File Offset: 0x0000C94B
	public float SoundVisable
	{
		get
		{
			return this.GetFloatStatValue(this.SoundVisableHash);
		}
	}

	// Token: 0x170000AF RID: 175
	// (get) Token: 0x0600034C RID: 844 RVA: 0x0000E759 File Offset: 0x0000C959
	public float ViewAngle
	{
		get
		{
			return this.GetFloatStatValue(this.viewAngleHash);
		}
	}

	// Token: 0x170000B0 RID: 176
	// (get) Token: 0x0600034D RID: 845 RVA: 0x0000E767 File Offset: 0x0000C967
	public float ViewDistance
	{
		get
		{
			return this.GetFloatStatValue(this.viewDistanceHash);
		}
	}

	// Token: 0x170000B1 RID: 177
	// (get) Token: 0x0600034E RID: 846 RVA: 0x0000E775 File Offset: 0x0000C975
	public float SenseRange
	{
		get
		{
			return this.GetFloatStatValue(this.senseRangeHash);
		}
	}

	// Token: 0x170000B2 RID: 178
	// (get) Token: 0x0600034F RID: 847 RVA: 0x0000E783 File Offset: 0x0000C983
	public float MeleeDamageMultiplier
	{
		get
		{
			if (this.characterItem)
			{
				return this.characterItem.GetStatValue(CharacterMainControl.meleeDamageMultiplierHash);
			}
			return 0f;
		}
	}

	// Token: 0x170000B3 RID: 179
	// (get) Token: 0x06000350 RID: 848 RVA: 0x0000E7A8 File Offset: 0x0000C9A8
	public float MeleeCritRateGain
	{
		get
		{
			if (this.characterItem)
			{
				return this.characterItem.GetStatValue(CharacterMainControl.meleeCritRateGainHash);
			}
			return 0f;
		}
	}

	// Token: 0x170000B4 RID: 180
	// (get) Token: 0x06000351 RID: 849 RVA: 0x0000E7CD File Offset: 0x0000C9CD
	public float MeleeCritDamageGain
	{
		get
		{
			if (this.characterItem)
			{
				return this.characterItem.GetStatValue(CharacterMainControl.meleeCritDamageGainHash);
			}
			return 0f;
		}
	}

	// Token: 0x170000B5 RID: 181
	// (get) Token: 0x06000352 RID: 850 RVA: 0x0000E7F2 File Offset: 0x0000C9F2
	public float GunDamageMultiplier
	{
		get
		{
			if (this.characterItem)
			{
				return this.characterItem.GetStatValue(CharacterMainControl.gunDamageMultiplierHash);
			}
			return 0f;
		}
	}

	// Token: 0x170000B6 RID: 182
	// (get) Token: 0x06000353 RID: 851 RVA: 0x0000E817 File Offset: 0x0000CA17
	public float ReloadSpeedGain
	{
		get
		{
			if (this.characterItem)
			{
				return this.characterItem.GetStatValue(CharacterMainControl.reloadSpeedGainHash);
			}
			return 0f;
		}
	}

	// Token: 0x170000B7 RID: 183
	// (get) Token: 0x06000354 RID: 852 RVA: 0x0000E83C File Offset: 0x0000CA3C
	public float GunCritRateGain
	{
		get
		{
			if (this.characterItem)
			{
				return this.characterItem.GetStatValue(CharacterMainControl.gunCritRateGainHash);
			}
			return 0f;
		}
	}

	// Token: 0x170000B8 RID: 184
	// (get) Token: 0x06000355 RID: 853 RVA: 0x0000E861 File Offset: 0x0000CA61
	public float GunCritDamageGain
	{
		get
		{
			if (this.characterItem)
			{
				return this.characterItem.GetStatValue(CharacterMainControl.gunCritDamageGainHash);
			}
			return 0f;
		}
	}

	// Token: 0x170000B9 RID: 185
	// (get) Token: 0x06000356 RID: 854 RVA: 0x0000E886 File Offset: 0x0000CA86
	public float GunBulletSpeedMultiplier
	{
		get
		{
			if (this.characterItem)
			{
				return this.characterItem.GetStatValue(CharacterMainControl.gunBulletSpeedMultiplierHash);
			}
			return 1f;
		}
	}

	// Token: 0x170000BA RID: 186
	// (get) Token: 0x06000357 RID: 855 RVA: 0x0000E8AB File Offset: 0x0000CAAB
	public float RecoilControl
	{
		get
		{
			if (this.characterItem)
			{
				return this.characterItem.GetStatValue(CharacterMainControl.recoilControlHash);
			}
			return 1f;
		}
	}

	// Token: 0x170000BB RID: 187
	// (get) Token: 0x06000358 RID: 856 RVA: 0x0000E8D0 File Offset: 0x0000CAD0
	public float GunScatterMultiplier
	{
		get
		{
			if (this.characterItem)
			{
				return this.characterItem.GetStatValue(CharacterMainControl.GunScatterMultiplierHash);
			}
			return 1f;
		}
	}

	// Token: 0x170000BC RID: 188
	// (get) Token: 0x06000359 RID: 857 RVA: 0x0000E8F5 File Offset: 0x0000CAF5
	public float InventoryCapacity
	{
		get
		{
			if (this.characterItem)
			{
				return this.characterItem.GetStatValue(CharacterMainControl.InventoryCapacityHash);
			}
			return 16f;
		}
	}

	// Token: 0x170000BD RID: 189
	// (get) Token: 0x0600035A RID: 858 RVA: 0x0000E91C File Offset: 0x0000CB1C
	public bool HasGasMask
	{
		get
		{
			float num = 0f;
			if (this.characterItem)
			{
				num = this.characterItem.GetStatValue(CharacterMainControl.GasMaskHash);
			}
			return num > 0.1f;
		}
	}

	// Token: 0x0600035B RID: 859 RVA: 0x0000E958 File Offset: 0x0000CB58
	public void UseStamina(float value)
	{
		if (!LevelManager.Instance || LevelManager.Instance.IsBaseLevel)
		{
			return;
		}
		if (value <= 0f)
		{
			return;
		}
		this.staminaRecoverTimer = 0f;
		this.currentStamina -= value;
		if (this.currentStamina < 0f)
		{
			this.currentStamina = 0f;
		}
	}

	// Token: 0x170000BE RID: 190
	// (get) Token: 0x0600035C RID: 860 RVA: 0x0000E9B8 File Offset: 0x0000CBB8
	public float WalkSoundRange
	{
		get
		{
			if (this.characterItem)
			{
				return this.characterItem.GetStatValue(this.walkSoundRangeHash);
			}
			return 0f;
		}
	}

	// Token: 0x170000BF RID: 191
	// (get) Token: 0x0600035D RID: 861 RVA: 0x0000E9DE File Offset: 0x0000CBDE
	public float RunSoundRange
	{
		get
		{
			if (this.characterItem)
			{
				return this.characterItem.GetStatValue(this.runSoundRangeHash);
			}
			return 0f;
		}
	}

	// Token: 0x170000C0 RID: 192
	// (get) Token: 0x0600035E RID: 862 RVA: 0x0000EA04 File Offset: 0x0000CC04
	public bool FlashLight
	{
		get
		{
			return this.CurrentHoldItemAgent && this.CurrentHoldItemAgent.Item.GetStatValue(CharacterMainControl.flashLightHash) > 0f;
		}
	}

	// Token: 0x170000C1 RID: 193
	// (get) Token: 0x0600035F RID: 863 RVA: 0x0000EA31 File Offset: 0x0000CC31
	public string SoundKey
	{
		get
		{
			return "Default";
		}
	}

	// Token: 0x04000219 RID: 537
	public CharacterRandomPreset characterPreset;

	// Token: 0x0400021A RID: 538
	private AudioManager.VoiceType audioVoiceType;

	// Token: 0x0400021B RID: 539
	private AudioManager.FootStepMaterialType footStepMaterialType;

	// Token: 0x0400021C RID: 540
	[SerializeField]
	private Teams team;

	// Token: 0x0400021E RID: 542
	private Item characterItem;

	// Token: 0x0400021F RID: 543
	public Movement movementControl;

	// Token: 0x04000220 RID: 544
	public ItemAgentHolder agentHolder;

	// Token: 0x04000221 RID: 545
	public CA_Carry carryAction;

	// Token: 0x04000222 RID: 546
	public CharacterModel characterModel;

	// Token: 0x04000223 RID: 547
	private bool hidden;

	// Token: 0x04000225 RID: 549
	public InteractableLootbox deadLootBoxPrefab;

	// Token: 0x04000227 RID: 551
	public Transform modelRoot;

	// Token: 0x04000229 RID: 553
	private Vector3 moveInput;

	// Token: 0x0400022A RID: 554
	private bool runInput;

	// Token: 0x0400022B RID: 555
	private bool adsInput;

	// Token: 0x0400022C RID: 556
	private const float defaultAimRange = 8f;

	// Token: 0x0400022D RID: 557
	private AimTypes aimType;

	// Token: 0x0400022E RID: 558
	private float disableTriggerTimer;

	// Token: 0x0400022F RID: 559
	public List<Buff.BuffExclusiveTags> buffResist;

	// Token: 0x04000230 RID: 560
	private Vector3 inputAimPoint;

	// Token: 0x04000231 RID: 561
	private CharacterActionBase currentAction;

	// Token: 0x04000232 RID: 562
	public CA_Reload reloadAction;

	// Token: 0x04000233 RID: 563
	public CA_Skill skillAction;

	// Token: 0x04000234 RID: 564
	public CA_UseItem useItemAction;

	// Token: 0x04000239 RID: 569
	public static Action<CharacterMainControl, Inventory, int> OnMainCharacterInventoryChangedEvent;

	// Token: 0x0400023A RID: 570
	public static Action<CharacterMainControl, Slot> OnMainCharacterSlotContentChangedEvent;

	// Token: 0x0400023C RID: 572
	private int relatedScene;

	// Token: 0x0400023D RID: 573
	[SerializeField]
	private Health health;

	// Token: 0x0400023E RID: 574
	[SerializeField]
	private CharacterItemControl itemControl;

	// Token: 0x0400023F RID: 575
	public CA_Interact interactAction;

	// Token: 0x04000240 RID: 576
	[SerializeField]
	private CharacterEquipmentController equipmentController;

	// Token: 0x04000241 RID: 577
	public static Action<CharacterMainControl, DuckovItemAgent> OnMainCharacterChangeHoldItemAgentEvent;

	// Token: 0x04000242 RID: 578
	[SerializeField]
	private CharacterBuffManager buffManager;

	// Token: 0x04000243 RID: 579
	private int holdWeaponBeforeUse;

	// Token: 0x04000244 RID: 580
	public CA_Dash dashAction;

	// Token: 0x04000245 RID: 581
	public CA_Attack attackAction;

	// Token: 0x04000248 RID: 584
	public DamageReceiver mainDamageReceiver;

	// Token: 0x04000249 RID: 585
	private HashSet<GameObject> nearByHalfObsticles;

	// Token: 0x0400024A RID: 586
	private List<Item> weaponsTemp = new List<Item>();

	// Token: 0x0400024B RID: 587
	private int weaponSwitchIndex;

	// Token: 0x0400024C RID: 588
	private CharacterMainControl.WeightStates weightState = CharacterMainControl.WeightStates.normal;

	// Token: 0x0400024D RID: 589
	private int meleeWeaponSlotHash = "MeleeWeapon".GetHashCode();

	// Token: 0x0400024E RID: 590
	private int primWeaponSlotHash = "PrimaryWeapon".GetHashCode();

	// Token: 0x0400024F RID: 591
	private int secWeaponSlotHash = "SecondaryWeapon".GetHashCode();

	// Token: 0x04000250 RID: 592
	private float staminaRecoverTimer;

	// Token: 0x04000251 RID: 593
	private float variableTickTimer;

	// Token: 0x04000252 RID: 594
	public const float weightThreshold_Light = 0.25f;

	// Token: 0x04000253 RID: 595
	public const float weightThreshold_Heavy = 0.5f;

	// Token: 0x04000254 RID: 596
	public const float weightThreshold_superWeight = 0.75f;

	// Token: 0x04000255 RID: 597
	private string hideShowRecorder;

	// Token: 0x04000256 RID: 598
	private int walkSpeedHash = "WalkSpeed".GetHashCode();

	// Token: 0x04000257 RID: 599
	private int walkAccHash = "WalkAcc".GetHashCode();

	// Token: 0x04000258 RID: 600
	private int runSpeedHash = "RunSpeed".GetHashCode();

	// Token: 0x04000259 RID: 601
	private int stormProtectionHash = "StormProtection".GetHashCode();

	// Token: 0x0400025A RID: 602
	private int waterEnergyRecoverMultiplierHash = "WaterEnergyRecoverMultiplier".GetHashCode();

	// Token: 0x0400025B RID: 603
	private int gunDistanceMultiplierHash = "GunDistanceMultiplier".GetHashCode();

	// Token: 0x0400025C RID: 604
	private int moveabilityHash = "Moveability".GetHashCode();

	// Token: 0x0400025D RID: 605
	private int runAccHash = "RunAcc".GetHashCode();

	// Token: 0x0400025E RID: 606
	private int turnSpeedHash = "TurnSpeed".GetHashCode();

	// Token: 0x0400025F RID: 607
	private int aimTurnSpeedHash = "AimTurnSpeed".GetHashCode();

	// Token: 0x04000260 RID: 608
	private int dashSpeedHash = "DashSpeed".GetHashCode();

	// Token: 0x04000261 RID: 609
	private int dashCanControlHash = "DashCanControl".GetHashCode();

	// Token: 0x04000262 RID: 610
	private int PetCapcityHash = "PetCapcity".GetHashCode();

	// Token: 0x04000263 RID: 611
	private int maxStaminaHash = "Stamina".GetHashCode();

	// Token: 0x04000264 RID: 612
	private float currentStamina;

	// Token: 0x04000265 RID: 613
	private int staminaDrainRateHash = "StaminaDrainRate".GetHashCode();

	// Token: 0x04000266 RID: 614
	private int staminaRecoverRateHash = "StaminaRecoverRate".GetHashCode();

	// Token: 0x04000267 RID: 615
	private int staminaRecoverTimeHash = "StaminaRecoverTime".GetHashCode();

	// Token: 0x04000268 RID: 616
	private int visableDistanceFactorHash = "VisableDistanceFactor".GetHashCode();

	// Token: 0x04000269 RID: 617
	private int maxWeightHash = "MaxWeight".GetHashCode();

	// Token: 0x0400026A RID: 618
	private int foodGainHash = "FoodGain".GetHashCode();

	// Token: 0x0400026B RID: 619
	private int healGainHash = "HealGain".GetHashCode();

	// Token: 0x0400026C RID: 620
	private int maxEnergyHash = "MaxEnergy".GetHashCode();

	// Token: 0x0400026D RID: 621
	private int energyCostPerMinHash = "EnergyCost".GetHashCode();

	// Token: 0x0400026E RID: 622
	private int currentEnergyHash = "CurrentEnergy".GetHashCode();

	// Token: 0x0400026F RID: 623
	private int maxWaterHash = "MaxWater".GetHashCode();

	// Token: 0x04000270 RID: 624
	private int waterCostPerMinHash = "WaterCost".GetHashCode();

	// Token: 0x04000271 RID: 625
	private bool starve;

	// Token: 0x04000272 RID: 626
	private bool thirsty;

	// Token: 0x04000273 RID: 627
	private int currentWaterHash = "CurrentWater".GetHashCode();

	// Token: 0x04000274 RID: 628
	private int NightVisionAbilityHash = "NightVisionAbility".GetHashCode();

	// Token: 0x04000275 RID: 629
	private int NightVisionTypeHash = "NightVisionType".GetHashCode();

	// Token: 0x04000276 RID: 630
	private int HearingAbilityHash = "HearingAbility".GetHashCode();

	// Token: 0x04000277 RID: 631
	private int SoundVisableHash = "SoundVisable".GetHashCode();

	// Token: 0x04000278 RID: 632
	private int viewAngleHash = "ViewAngle".GetHashCode();

	// Token: 0x04000279 RID: 633
	private int viewDistanceHash = "ViewDistance".GetHashCode();

	// Token: 0x0400027A RID: 634
	private int senseRangeHash = "SenseRange".GetHashCode();

	// Token: 0x0400027B RID: 635
	private static int meleeDamageMultiplierHash = "MeleeDamageMultiplier".GetHashCode();

	// Token: 0x0400027C RID: 636
	private static int meleeCritRateGainHash = "MeleeCritRateGain".GetHashCode();

	// Token: 0x0400027D RID: 637
	private static int meleeCritDamageGainHash = "MeleeCritDamageGain".GetHashCode();

	// Token: 0x0400027E RID: 638
	private static int gunDamageMultiplierHash = "GunDamageMultiplier".GetHashCode();

	// Token: 0x0400027F RID: 639
	private static int reloadSpeedGainHash = "ReloadSpeedGain".GetHashCode();

	// Token: 0x04000280 RID: 640
	private static int gunCritRateGainHash = "GunCritRateGain".GetHashCode();

	// Token: 0x04000281 RID: 641
	private static int gunBulletSpeedMultiplierHash = "BulletSpeedMultiplier".GetHashCode();

	// Token: 0x04000282 RID: 642
	private static int gunCritDamageGainHash = "GunCritDamageGain".GetHashCode();

	// Token: 0x04000283 RID: 643
	private static int recoilControlHash = "RecoilControl".GetHashCode();

	// Token: 0x04000284 RID: 644
	private static int GunScatterMultiplierHash = "GunScatterMultiplier".GetHashCode();

	// Token: 0x04000285 RID: 645
	private static int InventoryCapacityHash = "InventoryCapacity".GetHashCode();

	// Token: 0x04000286 RID: 646
	private static int GasMaskHash = "GasMask".GetHashCode();

	// Token: 0x04000287 RID: 647
	private int walkSoundRangeHash = "WalkSoundRange".GetHashCode();

	// Token: 0x04000288 RID: 648
	private int runSoundRangeHash = "RunSoundRange".GetHashCode();

	// Token: 0x04000289 RID: 649
	private static int flashLightHash = "FlashLight".GetHashCode();

	// Token: 0x0200042D RID: 1069
	public enum WeightStates
	{
		// Token: 0x04001A07 RID: 6663
		light,
		// Token: 0x04001A08 RID: 6664
		normal,
		// Token: 0x04001A09 RID: 6665
		heavy,
		// Token: 0x04001A0A RID: 6666
		superHeavy,
		// Token: 0x04001A0B RID: 6667
		overWeight
	}
}
