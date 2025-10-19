using System;
using Duckov;
using UnityEngine;

// Token: 0x02000050 RID: 80
public class CA_Dash : CharacterActionBase, IProgress
{
	// Token: 0x17000069 RID: 105
	// (get) Token: 0x06000209 RID: 521 RVA: 0x00009CB9 File Offset: 0x00007EB9
	private string sfx
	{
		get
		{
			if (string.IsNullOrWhiteSpace(this.overrideSFX))
			{
				return "Char/Footstep/dash";
			}
			return this.overrideSFX;
		}
	}

	// Token: 0x0600020A RID: 522 RVA: 0x00009CD4 File Offset: 0x00007ED4
	public override CharacterActionBase.ActionPriorities ActionPriority()
	{
		return CharacterActionBase.ActionPriorities.Dash;
	}

	// Token: 0x0600020B RID: 523 RVA: 0x00009CD7 File Offset: 0x00007ED7
	public override bool CanMove()
	{
		return false;
	}

	// Token: 0x0600020C RID: 524 RVA: 0x00009CDA File Offset: 0x00007EDA
	public override bool CanRun()
	{
		return false;
	}

	// Token: 0x0600020D RID: 525 RVA: 0x00009CDD File Offset: 0x00007EDD
	public override bool CanUseHand()
	{
		return this.dashCanControl;
	}

	// Token: 0x0600020E RID: 526 RVA: 0x00009CE5 File Offset: 0x00007EE5
	public override bool CanControlAim()
	{
		return this.dashCanControl;
	}

	// Token: 0x0600020F RID: 527 RVA: 0x00009CF0 File Offset: 0x00007EF0
	public Progress GetProgress()
	{
		Progress progress = default(Progress);
		if (base.Running)
		{
			progress.inProgress = true;
			progress.total = this.dashTime;
			progress.current = this.actionTimer;
		}
		else
		{
			progress.inProgress = false;
		}
		return progress;
	}

	// Token: 0x06000210 RID: 528 RVA: 0x00009D3A File Offset: 0x00007F3A
	public override bool IsReady()
	{
		return Time.time - this.lastEndTime >= this.coolTime && !base.Running;
	}

	// Token: 0x06000211 RID: 529 RVA: 0x00009D5C File Offset: 0x00007F5C
	protected override bool OnStart()
	{
		if (this.characterController.CurrentStamina < this.staminaCost)
		{
			return false;
		}
		this.characterController.UseStamina(this.staminaCost);
		this.dashSpeed = this.characterController.DashSpeed;
		this.dashCanControl = this.characterController.DashCanControl;
		if (this.characterController.MoveInput.magnitude > 0f)
		{
			this.dashDirection = this.characterController.MoveInput.normalized;
		}
		else
		{
			this.dashDirection = this.characterController.CurrentAimDirection;
		}
		this.characterController.SetForceMoveVelocity(this.dashSpeed * this.speedCurve.Evaluate(0f) * this.dashDirection);
		if (!this.dashCanControl)
		{
			this.characterController.movementControl.ForceTurnTo(this.dashDirection);
		}
		AudioManager.Post(this.sfx, base.gameObject);
		return true;
	}

	// Token: 0x06000212 RID: 530 RVA: 0x00009E55 File Offset: 0x00008055
	protected override void OnStop()
	{
		this.characterController.SetForceMoveVelocity(this.characterController.CharacterRunSpeed * this.dashDirection);
		this.lastEndTime = Time.time;
	}

	// Token: 0x06000213 RID: 531 RVA: 0x00009E84 File Offset: 0x00008084
	protected override void OnUpdateAction(float deltaTime)
	{
		if ((this.actionTimer > this.dashTime || !base.Running) && base.StopAction())
		{
			return;
		}
		this.characterController.SetForceMoveVelocity(this.dashSpeed * this.speedCurve.Evaluate(Mathf.Clamp01(this.actionTimer / this.dashTime)) * this.dashDirection);
	}

	// Token: 0x040001BD RID: 445
	private float dashSpeed;

	// Token: 0x040001BE RID: 446
	private bool dashCanControl;

	// Token: 0x040001BF RID: 447
	public AnimationCurve speedCurve;

	// Token: 0x040001C0 RID: 448
	public float dashTime;

	// Token: 0x040001C1 RID: 449
	public float coolTime = 0.5f;

	// Token: 0x040001C2 RID: 450
	private Vector3 dashDirection;

	// Token: 0x040001C3 RID: 451
	public float staminaCost = 10f;

	// Token: 0x040001C4 RID: 452
	private float lastEndTime = -999f;

	// Token: 0x040001C5 RID: 453
	[SerializeField]
	private string overrideSFX;
}
