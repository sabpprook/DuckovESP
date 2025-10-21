using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000057 RID: 87
public class CharacterAnimationControl : MonoBehaviour
{
	// Token: 0x0600026A RID: 618 RVA: 0x0000AB30 File Offset: 0x00008D30
	private void InitHash()
	{
		foreach (AnimatorControllerParameter animatorControllerParameter in this.animator.parameters)
		{
			this.animatorHashes.Add(animatorControllerParameter.nameHash);
		}
	}

	// Token: 0x0600026B RID: 619 RVA: 0x0000AB6D File Offset: 0x00008D6D
	private void SetAnimatorBool(int hash, bool value)
	{
		if (!this.animatorHashes.Contains(hash))
		{
			return;
		}
		this.animator.SetBool(hash, value);
	}

	// Token: 0x0600026C RID: 620 RVA: 0x0000AB8B File Offset: 0x00008D8B
	private void SetAnimatorFloat(int hash, float value)
	{
		if (!this.animatorHashes.Contains(hash))
		{
			return;
		}
		this.animator.SetFloat(hash, value);
	}

	// Token: 0x0600026D RID: 621 RVA: 0x0000ABA9 File Offset: 0x00008DA9
	private void SetAnimatorInteger(int hash, int value)
	{
		if (!this.animatorHashes.Contains(hash))
		{
			return;
		}
		this.animator.SetInteger(hash, value);
	}

	// Token: 0x0600026E RID: 622 RVA: 0x0000ABC7 File Offset: 0x00008DC7
	private void SetAnimatorTrigger(int hash)
	{
		if (!this.animatorHashes.Contains(hash))
		{
			return;
		}
		this.animator.SetTrigger(hash);
	}

	// Token: 0x0600026F RID: 623 RVA: 0x0000ABE4 File Offset: 0x00008DE4
	private void Awake()
	{
		if (!this.characterModel)
		{
			this.characterModel = base.GetComponent<CharacterModel>();
		}
		this.characterModel.OnCharacterSetEvent += this.OnCharacterSet;
		if (this.characterModel.characterMainControl)
		{
			this.characterMainControl = this.characterModel.characterMainControl;
		}
		this.characterModel.OnAttackOrShootEvent += this.OnAttack;
		this.InitHash();
	}

	// Token: 0x06000270 RID: 624 RVA: 0x0000AC61 File Offset: 0x00008E61
	private void OnDestroy()
	{
		if (this.characterModel)
		{
			this.characterModel.OnCharacterSetEvent -= this.OnCharacterSet;
			this.characterModel.OnAttackOrShootEvent -= this.OnAttack;
		}
	}

	// Token: 0x06000271 RID: 625 RVA: 0x0000AC9E File Offset: 0x00008E9E
	private void OnCharacterSet()
	{
		this.characterMainControl = this.characterModel.characterMainControl;
	}

	// Token: 0x06000272 RID: 626 RVA: 0x0000ACB1 File Offset: 0x00008EB1
	private void Start()
	{
		if (this.attackLayer < 0)
		{
			this.attackLayer = this.animator.GetLayerIndex("MeleeAttack");
		}
	}

	// Token: 0x06000273 RID: 627 RVA: 0x0000ACD2 File Offset: 0x00008ED2
	private void SetAttackLayerWeight(float weight)
	{
		if (this.attackLayer < 0)
		{
			return;
		}
		this.animator.SetLayerWeight(this.attackLayer, weight);
	}

	// Token: 0x06000274 RID: 628 RVA: 0x0000ACF0 File Offset: 0x00008EF0
	private void Update()
	{
		if (!this.characterMainControl)
		{
			return;
		}
		this.SetAnimatorFloat(this.hash_MoveSpeed, this.characterMainControl.AnimationMoveSpeedValue);
		Vector2 animationLocalMoveDirectionValue = this.characterMainControl.AnimationLocalMoveDirectionValue;
		this.SetAnimatorFloat(this.hash_MoveDirX, animationLocalMoveDirectionValue.x);
		this.SetAnimatorFloat(this.hash_MoveDirY, animationLocalMoveDirectionValue.y);
		bool flag = true;
		if (this.characterMainControl.CurrentHoldItemAgent == null)
		{
			flag = false;
		}
		else if (!this.characterMainControl.CurrentHoldItemAgent.gameObject.activeSelf)
		{
			flag = false;
		}
		else if (this.characterMainControl.reloadAction.Running)
		{
			flag = false;
		}
		this.SetAnimatorBool(this.hash_RightHandOut, flag);
		bool flag2 = this.characterMainControl.Dashing;
		if (flag2 && !this.hasAnimationIfDashCanControl && this.characterMainControl.DashCanControl)
		{
			flag2 = false;
		}
		this.SetAnimatorBool(this.hash_Dashing, flag2);
		int num = 0;
		if (!this.holdAgent)
		{
			this.holdAgent = this.characterMainControl.CurrentHoldItemAgent;
		}
		if (this.holdAgent != null)
		{
			num = (int)this.holdAgent.handAnimationType;
		}
		this.SetAnimatorInteger(this.hash_HandState, num);
		this.UpdateAttackLayerWeight();
	}

	// Token: 0x06000275 RID: 629 RVA: 0x0000AE28 File Offset: 0x00009028
	private void UpdateAttackLayerWeight()
	{
		if (!this.attacking)
		{
			if (this.weight > 0f)
			{
				this.weight = 0f;
				this.SetAttackLayerWeight(this.weight);
			}
			return;
		}
		this.attackTimer += Time.deltaTime;
		this.weight = this.attackLayerWeightCurve.Evaluate(this.attackTimer / this.attackTime);
		if (this.attackTimer >= this.attackTime)
		{
			this.attacking = false;
			this.weight = 0f;
		}
		this.SetAttackLayerWeight(this.weight);
	}

	// Token: 0x06000276 RID: 630 RVA: 0x0000AEC0 File Offset: 0x000090C0
	public void OnAttack()
	{
		if (!this.characterMainControl || !this.holdAgent || this.holdAgent.handAnimationType != HandheldAnimationType.meleeWeapon)
		{
			this.attacking = false;
			return;
		}
		this.attacking = true;
		if (this.attackLayer < 0)
		{
			this.attackLayer = this.animator.GetLayerIndex("MeleeAttack");
		}
		this.SetAnimatorTrigger(this.hash_Attack);
		this.attackTimer = 0f;
	}

	// Token: 0x040001E0 RID: 480
	public CharacterMainControl characterMainControl;

	// Token: 0x040001E1 RID: 481
	public CharacterModel characterModel;

	// Token: 0x040001E2 RID: 482
	public Animator animator;

	// Token: 0x040001E3 RID: 483
	public float attackTime = 0.3f;

	// Token: 0x040001E4 RID: 484
	private int attackLayer = -1;

	// Token: 0x040001E5 RID: 485
	private bool attacking;

	// Token: 0x040001E6 RID: 486
	private float attackTimer;

	// Token: 0x040001E7 RID: 487
	private bool hasAnimationIfDashCanControl;

	// Token: 0x040001E8 RID: 488
	public AnimationCurve attackLayerWeightCurve;

	// Token: 0x040001E9 RID: 489
	private int hash_MoveSpeed = Animator.StringToHash("MoveSpeed");

	// Token: 0x040001EA RID: 490
	private int hash_MoveDirX = Animator.StringToHash("MoveDirX");

	// Token: 0x040001EB RID: 491
	private int hash_MoveDirY = Animator.StringToHash("MoveDirY");

	// Token: 0x040001EC RID: 492
	private int hash_RightHandOut = Animator.StringToHash("RightHandOut");

	// Token: 0x040001ED RID: 493
	private int hash_HandState = Animator.StringToHash("HandState");

	// Token: 0x040001EE RID: 494
	private int hash_Dashing = Animator.StringToHash("Dashing");

	// Token: 0x040001EF RID: 495
	private int hash_Attack = Animator.StringToHash("Attack");

	// Token: 0x040001F0 RID: 496
	private HashSet<int> animatorHashes = new HashSet<int>();

	// Token: 0x040001F1 RID: 497
	private float weight;

	// Token: 0x040001F2 RID: 498
	private DuckovItemAgent holdAgent;
}
