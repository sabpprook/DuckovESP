using System;
using System.Collections.Generic;
using ItemStatsSystem.Items;
using UnityEngine;

// Token: 0x0200005D RID: 93
public class CharacterModel : MonoBehaviour
{
	// Token: 0x14000015 RID: 21
	// (add) Token: 0x06000362 RID: 866 RVA: 0x0000EDA0 File Offset: 0x0000CFA0
	// (remove) Token: 0x06000363 RID: 867 RVA: 0x0000EDD8 File Offset: 0x0000CFD8
	public event Action<CharacterModel> OnDestroyEvent;

	// Token: 0x170000C2 RID: 194
	// (get) Token: 0x06000364 RID: 868 RVA: 0x0000EE0D File Offset: 0x0000D00D
	public Transform LefthandSocket
	{
		get
		{
			return this.lefthandSocket;
		}
	}

	// Token: 0x170000C3 RID: 195
	// (get) Token: 0x06000365 RID: 869 RVA: 0x0000EE15 File Offset: 0x0000D015
	public Transform RightHandSocket
	{
		get
		{
			return this.rightHandSocket;
		}
	}

	// Token: 0x170000C4 RID: 196
	// (get) Token: 0x06000366 RID: 870 RVA: 0x0000EE1D File Offset: 0x0000D01D
	public Transform ArmorSocket
	{
		get
		{
			return this.armorSocket;
		}
	}

	// Token: 0x170000C5 RID: 197
	// (get) Token: 0x06000367 RID: 871 RVA: 0x0000EE25 File Offset: 0x0000D025
	public Transform HelmatSocket
	{
		get
		{
			return this.helmatSocket;
		}
	}

	// Token: 0x170000C6 RID: 198
	// (get) Token: 0x06000368 RID: 872 RVA: 0x0000EE2D File Offset: 0x0000D02D
	public Transform FaceMaskSocket
	{
		get
		{
			if (this.faceSocket)
			{
				return this.faceSocket;
			}
			return this.helmatSocket;
		}
	}

	// Token: 0x170000C7 RID: 199
	// (get) Token: 0x06000369 RID: 873 RVA: 0x0000EE49 File Offset: 0x0000D049
	public Transform BackpackSocket
	{
		get
		{
			return this.backpackSocket;
		}
	}

	// Token: 0x170000C8 RID: 200
	// (get) Token: 0x0600036A RID: 874 RVA: 0x0000EE51 File Offset: 0x0000D051
	public Transform MeleeWeaponSocket
	{
		get
		{
			return this.meleeWeaponSocket;
		}
	}

	// Token: 0x170000C9 RID: 201
	// (get) Token: 0x0600036B RID: 875 RVA: 0x0000EE59 File Offset: 0x0000D059
	public Transform PopTextSocket
	{
		get
		{
			return this.popTextSocket;
		}
	}

	// Token: 0x170000CA RID: 202
	// (get) Token: 0x0600036C RID: 876 RVA: 0x0000EE61 File Offset: 0x0000D061
	public CustomFaceInstance CustomFace
	{
		get
		{
			return this.customFace;
		}
	}

	// Token: 0x170000CB RID: 203
	// (get) Token: 0x0600036D RID: 877 RVA: 0x0000EE69 File Offset: 0x0000D069
	public bool Hidden
	{
		get
		{
			return this.characterMainControl.Hidden;
		}
	}

	// Token: 0x14000016 RID: 22
	// (add) Token: 0x0600036E RID: 878 RVA: 0x0000EE78 File Offset: 0x0000D078
	// (remove) Token: 0x0600036F RID: 879 RVA: 0x0000EEB0 File Offset: 0x0000D0B0
	public event Action OnCharacterSetEvent;

	// Token: 0x14000017 RID: 23
	// (add) Token: 0x06000370 RID: 880 RVA: 0x0000EEE8 File Offset: 0x0000D0E8
	// (remove) Token: 0x06000371 RID: 881 RVA: 0x0000EF20 File Offset: 0x0000D120
	public event Action OnAttackOrShootEvent;

	// Token: 0x06000372 RID: 882 RVA: 0x0000EF55 File Offset: 0x0000D155
	private void Awake()
	{
		this.defaultRightHandLocalRotation = this.rightHandSocket.localRotation;
	}

	// Token: 0x06000373 RID: 883 RVA: 0x0000EF68 File Offset: 0x0000D168
	private void Start()
	{
		CharacterSubVisuals component = base.GetComponent<CharacterSubVisuals>();
		if (component != null)
		{
			if (this.subVisuals.Contains(component))
			{
				this.RemoveVisual(component);
			}
			this.AddSubVisuals(component);
		}
	}

	// Token: 0x06000374 RID: 884 RVA: 0x0000EFA1 File Offset: 0x0000D1A1
	private void LateUpdate()
	{
		if (this.autoSyncRightHandRotation)
		{
			this.SyncRightHandRotation();
		}
	}

	// Token: 0x06000375 RID: 885 RVA: 0x0000EFB4 File Offset: 0x0000D1B4
	public void OnMainCharacterSetted(CharacterMainControl _characterMainControl)
	{
		this.characterMainControl = _characterMainControl;
		if (!this.characterMainControl)
		{
			return;
		}
		if (this.characterMainControl.attackAction)
		{
			this.characterMainControl.attackAction.OnAttack += this.OnAttack;
		}
		this.characterMainControl.OnShootEvent += this.OnShoot;
		this.characterMainControl.EquipmentController.OnHelmatSlotContentChanged += this.OnHelmatSlotContentChange;
		this.characterMainControl.EquipmentController.OnFaceMaskSlotContentChanged += this.OnFaceMaskSlotContentChange;
		if (_characterMainControl.mainDamageReceiver != null)
		{
			CapsuleCollider component = _characterMainControl.mainDamageReceiver.GetComponent<CapsuleCollider>();
			if (component != null)
			{
				component.radius = this.damageReceiverRadius;
				if (this.damageReceiverRadius <= 0f)
				{
					component.enabled = false;
				}
			}
		}
		Action onCharacterSetEvent = this.OnCharacterSetEvent;
		if (onCharacterSetEvent != null)
		{
			onCharacterSetEvent();
		}
		this.hurtVisual.SetHealth(_characterMainControl.Health);
	}

	// Token: 0x06000376 RID: 886 RVA: 0x0000F0B8 File Offset: 0x0000D2B8
	private void CharacterMainControl_OnShootEvent(DuckovItemAgent obj)
	{
		throw new NotImplementedException();
	}

	// Token: 0x06000377 RID: 887 RVA: 0x0000F0C0 File Offset: 0x0000D2C0
	private void OnHelmatSlotContentChange(Slot slot)
	{
		if (slot == null)
		{
			return;
		}
		this.helmatShowHair = slot.Content == null || slot.Content.Constants.GetBool(this.showHairHash, false);
		this.helmatShowMouth = slot.Content == null || slot.Content.Constants.GetBool(this.showMouthHash, true);
		if (this.customFace && this.customFace.hairSocket)
		{
			this.customFace.hairSocket.gameObject.SetActive(this.helmatShowHair && this.faceMaskShowHair);
		}
		if (this.customFace && this.customFace.mouthPart.socket)
		{
			this.customFace.mouthPart.socket.gameObject.SetActive(this.helmatShowMouth && this.faceMaskShowMouth);
		}
	}

	// Token: 0x06000378 RID: 888 RVA: 0x0000F1C4 File Offset: 0x0000D3C4
	private void OnFaceMaskSlotContentChange(Slot slot)
	{
		if (slot == null)
		{
			return;
		}
		this.faceMaskShowHair = slot.Content == null || slot.Content.Constants.GetBool(this.showHairHash, true);
		this.faceMaskShowMouth = slot.Content == null || slot.Content.Constants.GetBool(this.showMouthHash, true);
		if (this.customFace && this.customFace.hairSocket)
		{
			this.customFace.hairSocket.gameObject.SetActive(this.helmatShowHair && this.faceMaskShowHair);
		}
		if (this.customFace && this.customFace.mouthPart.socket)
		{
			this.customFace.mouthPart.socket.gameObject.SetActive(this.helmatShowMouth && this.faceMaskShowMouth);
		}
	}

	// Token: 0x06000379 RID: 889 RVA: 0x0000F2C8 File Offset: 0x0000D4C8
	private void OnDestroy()
	{
		if (this.destroied)
		{
			return;
		}
		this.destroied = true;
		Action<CharacterModel> onDestroyEvent = this.OnDestroyEvent;
		if (onDestroyEvent != null)
		{
			onDestroyEvent(this);
		}
		if (this.characterMainControl)
		{
			if (this.characterMainControl.attackAction)
			{
				this.characterMainControl.attackAction.OnAttack -= this.OnAttack;
			}
			this.characterMainControl.OnShootEvent -= this.OnShoot;
			this.characterMainControl.EquipmentController.OnHelmatSlotContentChanged -= this.OnHelmatSlotContentChange;
			this.characterMainControl.EquipmentController.OnFaceMaskSlotContentChanged -= this.OnFaceMaskSlotContentChange;
		}
	}

	// Token: 0x0600037A RID: 890 RVA: 0x0000F384 File Offset: 0x0000D584
	private void SyncRightHandRotation()
	{
		if (!this.characterMainControl)
		{
			return;
		}
		bool flag = true;
		bool flag2 = false;
		if (this.characterMainControl.Running)
		{
			flag = false;
		}
		Quaternion quaternion;
		if (flag)
		{
			quaternion = Quaternion.LookRotation(this.characterMainControl.CurrentAimDirection, Vector3.up);
		}
		else
		{
			quaternion = this.rightHandSocket.parent.transform.rotation * this.defaultRightHandLocalRotation;
		}
		float num = 999f;
		if (!flag2)
		{
			num = 360f * Time.deltaTime;
		}
		this.rightHandSocket.rotation = Quaternion.RotateTowards(this.rightHandSocket.rotation, quaternion, num);
	}

	// Token: 0x0600037B RID: 891 RVA: 0x0000F420 File Offset: 0x0000D620
	public void AddSubVisuals(CharacterSubVisuals visuals)
	{
		visuals.mainModel = this;
		if (this.subVisuals.Contains(visuals))
		{
			return;
		}
		this.subVisuals.Add(visuals);
		this.renderers.AddRange(visuals.renderers);
		this.hurtVisual.SetRenderers(this.renderers);
		visuals.SetRenderersHidden(this.Hidden);
	}

	// Token: 0x0600037C RID: 892 RVA: 0x0000F480 File Offset: 0x0000D680
	public void RemoveVisual(CharacterSubVisuals _subVisuals)
	{
		this.subVisuals.Remove(_subVisuals);
		foreach (Renderer renderer in _subVisuals.renderers)
		{
			this.renderers.Remove(renderer);
		}
		this.hurtVisual.SetRenderers(this.renderers);
	}

	// Token: 0x0600037D RID: 893 RVA: 0x0000F4F8 File Offset: 0x0000D6F8
	public void SyncHiddenToMainCharacter()
	{
		bool flag = this.Hidden;
		if (!Team.IsEnemy(Teams.player, this.characterMainControl.Team))
		{
			flag = false;
		}
		if (this.subVisuals.Count > 0)
		{
			foreach (CharacterSubVisuals characterSubVisuals in this.subVisuals)
			{
				if (!(characterSubVisuals == null))
				{
					characterSubVisuals.SetRenderersHidden(flag);
				}
			}
		}
	}

	// Token: 0x0600037E RID: 894 RVA: 0x0000F580 File Offset: 0x0000D780
	public void SetFaceFromPreset(CustomFacePreset preset)
	{
		if (preset == null)
		{
			return;
		}
		if (!this.customFace)
		{
			return;
		}
		this.customFace.LoadFromData(preset.settings);
	}

	// Token: 0x0600037F RID: 895 RVA: 0x0000F5AB File Offset: 0x0000D7AB
	public void SetFaceFromData(CustomFaceSettingData data)
	{
		if (!this.customFace)
		{
			return;
		}
		this.customFace.LoadFromData(data);
	}

	// Token: 0x06000380 RID: 896 RVA: 0x0000F5C7 File Offset: 0x0000D7C7
	private void OnAttack()
	{
		Action onAttackOrShootEvent = this.OnAttackOrShootEvent;
		if (onAttackOrShootEvent == null)
		{
			return;
		}
		onAttackOrShootEvent();
	}

	// Token: 0x06000381 RID: 897 RVA: 0x0000F5D9 File Offset: 0x0000D7D9
	public void ForcePlayAttackAnimation()
	{
		this.OnAttack();
	}

	// Token: 0x06000382 RID: 898 RVA: 0x0000F5E1 File Offset: 0x0000D7E1
	private void OnShoot(DuckovItemAgent agent)
	{
		Action onAttackOrShootEvent = this.OnAttackOrShootEvent;
		if (onAttackOrShootEvent == null)
		{
			return;
		}
		onAttackOrShootEvent();
	}

	// Token: 0x0400028A RID: 650
	public CharacterMainControl characterMainControl;

	// Token: 0x0400028B RID: 651
	public bool invisable;

	// Token: 0x0400028D RID: 653
	[SerializeField]
	private Transform lefthandSocket;

	// Token: 0x0400028E RID: 654
	[SerializeField]
	private Transform rightHandSocket;

	// Token: 0x0400028F RID: 655
	private Quaternion defaultRightHandLocalRotation;

	// Token: 0x04000290 RID: 656
	[SerializeField]
	private HurtVisual hurtVisual;

	// Token: 0x04000291 RID: 657
	[SerializeField]
	private Transform armorSocket;

	// Token: 0x04000292 RID: 658
	[SerializeField]
	private Transform helmatSocket;

	// Token: 0x04000293 RID: 659
	[SerializeField]
	private Transform faceSocket;

	// Token: 0x04000294 RID: 660
	[SerializeField]
	private Transform backpackSocket;

	// Token: 0x04000295 RID: 661
	[SerializeField]
	private Transform meleeWeaponSocket;

	// Token: 0x04000296 RID: 662
	[SerializeField]
	private Transform popTextSocket;

	// Token: 0x04000297 RID: 663
	[SerializeField]
	private List<CharacterSubVisuals> subVisuals;

	// Token: 0x04000298 RID: 664
	[SerializeField]
	private List<Renderer> renderers;

	// Token: 0x04000299 RID: 665
	[SerializeField]
	private CustomFaceInstance customFace;

	// Token: 0x0400029A RID: 666
	public bool autoSyncRightHandRotation = true;

	// Token: 0x0400029B RID: 667
	public float damageReceiverRadius = 0.45f;

	// Token: 0x0400029C RID: 668
	private int showHairHash = "ShowHair".GetHashCode();

	// Token: 0x0400029D RID: 669
	private int showMouthHash = "ShowMouth".GetHashCode();

	// Token: 0x040002A0 RID: 672
	private bool helmatShowMouth = true;

	// Token: 0x040002A1 RID: 673
	private bool helmatShowHair = true;

	// Token: 0x040002A2 RID: 674
	private bool faceMaskShowHair = true;

	// Token: 0x040002A3 RID: 675
	private bool faceMaskShowMouth = true;

	// Token: 0x040002A4 RID: 676
	private bool destroied;
}
