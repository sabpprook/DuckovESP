using System;
using Duckov;
using UnityEngine;

// Token: 0x0200004E RID: 78
public class CA_Attack : CharacterActionBase, IProgress
{
	// Token: 0x14000007 RID: 7
	// (add) Token: 0x060001EE RID: 494 RVA: 0x0000983C File Offset: 0x00007A3C
	// (remove) Token: 0x060001EF RID: 495 RVA: 0x00009874 File Offset: 0x00007A74
	public event Action OnAttack;

	// Token: 0x17000068 RID: 104
	// (get) Token: 0x060001F0 RID: 496 RVA: 0x000098A9 File Offset: 0x00007AA9
	public bool DamageDealed
	{
		get
		{
			return this.damageDealed;
		}
	}

	// Token: 0x060001F1 RID: 497 RVA: 0x000098B1 File Offset: 0x00007AB1
	public override CharacterActionBase.ActionPriorities ActionPriority()
	{
		return CharacterActionBase.ActionPriorities.Attack;
	}

	// Token: 0x060001F2 RID: 498 RVA: 0x000098B4 File Offset: 0x00007AB4
	public override bool CanMove()
	{
		return true;
	}

	// Token: 0x060001F3 RID: 499 RVA: 0x000098B7 File Offset: 0x00007AB7
	public override bool CanRun()
	{
		return false;
	}

	// Token: 0x060001F4 RID: 500 RVA: 0x000098BA File Offset: 0x00007ABA
	public override bool CanUseHand()
	{
		return false;
	}

	// Token: 0x060001F5 RID: 501 RVA: 0x000098BD File Offset: 0x00007ABD
	public override bool CanControlAim()
	{
		return true;
	}

	// Token: 0x060001F6 RID: 502 RVA: 0x000098C0 File Offset: 0x00007AC0
	public Progress GetProgress()
	{
		Progress progress = default(Progress);
		if (base.Running)
		{
			progress.inProgress = true;
			progress.total = this.attackActionTime;
			progress.current = this.actionTimer;
		}
		else
		{
			progress.inProgress = false;
		}
		return progress;
	}

	// Token: 0x060001F7 RID: 503 RVA: 0x0000990C File Offset: 0x00007B0C
	public override bool IsReady()
	{
		if (Time.time - this.lastAttackTime < this.cd)
		{
			return false;
		}
		this.meleeWeapon = this.characterController.GetMeleeWeapon();
		return !(this.meleeWeapon == null) && this.meleeWeapon.StaminaCost <= this.characterController.CurrentStamina && !base.Running;
	}

	// Token: 0x060001F8 RID: 504 RVA: 0x00009974 File Offset: 0x00007B74
	protected override bool OnStart()
	{
		if (!this.characterController.CurrentHoldItemAgent)
		{
			return false;
		}
		this.meleeWeapon = this.characterController.GetMeleeWeapon();
		if (!this.meleeWeapon)
		{
			return false;
		}
		this.characterController.UseStamina(this.meleeWeapon.StaminaCost);
		this.dealDamageTime = this.meleeWeapon.DealDamageTime;
		this.damageDealed = false;
		Action onAttack = this.OnAttack;
		if (onAttack != null)
		{
			onAttack();
		}
		this.CreateAttackSound();
		this.lastAttackTime = Time.time;
		this.cd = 1f / this.meleeWeapon.AttackSpeed;
		this.slashFxDelayTime = this.meleeWeapon.slashFxDelayTime;
		this.slashFxSpawned = false;
		return true;
	}

	// Token: 0x060001F9 RID: 505 RVA: 0x00009A35 File Offset: 0x00007C35
	private void CreateAttackSound()
	{
		AudioManager.Post("SFX/Combat/Melee/attack_" + this.meleeWeapon.SoundKey.ToLower(), base.gameObject);
	}

	// Token: 0x060001FA RID: 506 RVA: 0x00009A5D File Offset: 0x00007C5D
	protected override void OnStop()
	{
	}

	// Token: 0x060001FB RID: 507 RVA: 0x00009A60 File Offset: 0x00007C60
	protected override void OnUpdateAction(float deltaTime)
	{
		if ((this.actionTimer > this.attackActionTime || !base.Running || this.meleeWeapon == null) && base.StopAction())
		{
			return;
		}
		if (!this.slashFxSpawned && base.ActionTimer > this.slashFxDelayTime && this.meleeWeapon && this.meleeWeapon.slashFx)
		{
			this.slashFxSpawned = true;
			Vector3 position = this.characterController.transform.position;
			position.y = this.meleeWeapon.transform.position.y;
			global::UnityEngine.Object.Instantiate<GameObject>(this.meleeWeapon.slashFx, position, Quaternion.LookRotation(this.characterController.modelRoot.forward, Vector3.up)).transform.SetParent(base.transform);
		}
		if (!this.damageDealed && base.ActionTimer > this.dealDamageTime)
		{
			this.damageDealed = true;
			this.meleeWeapon.CheckAndDealDamage();
		}
	}

	// Token: 0x040001B1 RID: 433
	private float attackActionTime = 0.25f;

	// Token: 0x040001B2 RID: 434
	private ItemAgent_MeleeWeapon meleeWeapon;

	// Token: 0x040001B4 RID: 436
	private float dealDamageTime = 0.1f;

	// Token: 0x040001B5 RID: 437
	private bool damageDealed;

	// Token: 0x040001B6 RID: 438
	private float lastAttackTime = -999f;

	// Token: 0x040001B7 RID: 439
	private float cd = -1f;

	// Token: 0x040001B8 RID: 440
	private bool slashFxSpawned;

	// Token: 0x040001B9 RID: 441
	private float slashFxDelayTime;
}
