using System;
using Duckov;
using UnityEngine;

// Token: 0x0200004F RID: 79
public class CA_Carry : CharacterActionBase, IProgress
{
	// Token: 0x060001FD RID: 509 RVA: 0x00009BA3 File Offset: 0x00007DA3
	public override CharacterActionBase.ActionPriorities ActionPriority()
	{
		return CharacterActionBase.ActionPriorities.Whatever;
	}

	// Token: 0x060001FE RID: 510 RVA: 0x00009BA6 File Offset: 0x00007DA6
	public override bool CanMove()
	{
		return true;
	}

	// Token: 0x060001FF RID: 511 RVA: 0x00009BA9 File Offset: 0x00007DA9
	public override bool CanRun()
	{
		return false;
	}

	// Token: 0x06000200 RID: 512 RVA: 0x00009BAC File Offset: 0x00007DAC
	public override bool CanUseHand()
	{
		return false;
	}

	// Token: 0x06000201 RID: 513 RVA: 0x00009BAF File Offset: 0x00007DAF
	public override bool CanControlAim()
	{
		return true;
	}

	// Token: 0x06000202 RID: 514 RVA: 0x00009BB2 File Offset: 0x00007DB2
	public override bool IsReady()
	{
		return this.carryTarget != null;
	}

	// Token: 0x06000203 RID: 515 RVA: 0x00009BC0 File Offset: 0x00007DC0
	public float GetWeight()
	{
		if (!base.Running)
		{
			return 0f;
		}
		if (!this.carringTarget)
		{
			return 0f;
		}
		return this.carringTarget.GetWeight();
	}

	// Token: 0x06000204 RID: 516 RVA: 0x00009BF0 File Offset: 0x00007DF0
	public Progress GetProgress()
	{
		return new Progress
		{
			inProgress = false,
			total = 1f,
			current = 1f
		};
	}

	// Token: 0x06000205 RID: 517 RVA: 0x00009C26 File Offset: 0x00007E26
	protected override bool OnStart()
	{
		this.characterController.ChangeHoldItem(null);
		this.carryTarget.Take(this);
		this.carringTarget = this.carryTarget;
		return true;
	}

	// Token: 0x06000206 RID: 518 RVA: 0x00009C4E File Offset: 0x00007E4E
	protected override void OnUpdateAction(float deltaTime)
	{
		if (this.characterController.CurrentHoldItemAgent != null)
		{
			base.StopAction();
		}
		if (this.carryTarget)
		{
			this.carryTarget.OnCarriableUpdate(deltaTime);
		}
	}

	// Token: 0x06000207 RID: 519 RVA: 0x00009C83 File Offset: 0x00007E83
	protected override void OnStop()
	{
		this.carryTarget.Drop();
		this.carringTarget = null;
	}

	// Token: 0x040001BA RID: 442
	[HideInInspector]
	public Carriable carryTarget;

	// Token: 0x040001BB RID: 443
	private Carriable carringTarget;

	// Token: 0x040001BC RID: 444
	public Vector3 carryPoint = new Vector3(0f, 1f, 0.8f);
}
