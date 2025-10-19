using System;
using Duckov;
using UnityEngine;

// Token: 0x02000051 RID: 81
public class CA_Interact : CharacterActionBase, IProgress
{
	// Token: 0x1700006A RID: 106
	// (get) Token: 0x06000215 RID: 533 RVA: 0x00009F13 File Offset: 0x00008113
	public InteractableBase MasterInteractableAround
	{
		get
		{
			return this.masterInteractableAround;
		}
	}

	// Token: 0x1700006B RID: 107
	// (get) Token: 0x06000216 RID: 534 RVA: 0x00009F1B File Offset: 0x0000811B
	public InteractableBase InteractTarget
	{
		get
		{
			if (this.masterInteractableAround)
			{
				return this.masterInteractableAround.GetInteractableInGroup(this.interactIndexInGroup);
			}
			return null;
		}
	}

	// Token: 0x1700006C RID: 108
	// (get) Token: 0x06000217 RID: 535 RVA: 0x00009F3D File Offset: 0x0000813D
	public int InteractIndexInGroup
	{
		get
		{
			return this.interactIndexInGroup;
		}
	}

	// Token: 0x1700006D RID: 109
	// (get) Token: 0x06000218 RID: 536 RVA: 0x00009F45 File Offset: 0x00008145
	public InteractableBase InteractingTarget
	{
		get
		{
			return this.interactingTarget;
		}
	}

	// Token: 0x06000219 RID: 537 RVA: 0x00009F4D File Offset: 0x0000814D
	private void Awake()
	{
		this.interactLayers = 1 << LayerMask.NameToLayer("Interactable");
		this.colliders = new Collider[5];
	}

	// Token: 0x0600021A RID: 538 RVA: 0x00009F78 File Offset: 0x00008178
	public void SearchInteractableAround()
	{
		if (!this.characterController.IsMainCharacter)
		{
			return;
		}
		InteractableBase interactableBase = this.masterInteractableAround;
		int num = Physics.OverlapSphereNonAlloc(base.transform.position + Vector3.up * 0.5f + this.characterController.CurrentAimDirection * 0.2f, 0.3f, this.colliders, this.interactLayers);
		if (num <= 0)
		{
			this.masterInteractableAround = null;
			return;
		}
		float num2 = 999f;
		this.minDistanceInteractable = null;
		for (int i = 0; i < num; i++)
		{
			Collider collider = this.colliders[i];
			float num3 = Vector3.Distance(base.transform.position, collider.transform.position);
			if (num3 < num2)
			{
				InteractableBase interactableBase2 = null;
				if (this.masterInteractableAround == null || this.masterInteractableAround.gameObject != collider.gameObject)
				{
					interactableBase2 = collider.GetComponent<InteractableBase>();
				}
				else if (this.masterInteractableAround != null && this.masterInteractableAround.gameObject == collider.gameObject)
				{
					interactableBase2 = this.masterInteractableAround;
				}
				if (!(interactableBase2 == null) && interactableBase2.CheckInteractable())
				{
					this.minDistanceInteractable = interactableBase2;
					num2 = num3;
				}
			}
		}
		this.masterInteractableAround = this.minDistanceInteractable;
		if (interactableBase != this.masterInteractableAround || interactableBase == null)
		{
			this.interactIndexInGroup = 0;
		}
	}

	// Token: 0x0600021B RID: 539 RVA: 0x0000A100 File Offset: 0x00008300
	public void SwitchInteractable(int dir)
	{
		if (this.MasterInteractableAround == null || !this.MasterInteractableAround.interactableGroup)
		{
			this.interactIndexInGroup = 0;
			return;
		}
		this.interactIndexInGroup += dir;
		int num = 1;
		if (this.MasterInteractableAround.interactableGroup)
		{
			num = this.MasterInteractableAround.GetInteractableList().Count;
		}
		if (this.interactIndexInGroup >= num)
		{
			this.interactIndexInGroup = 0;
			return;
		}
		if (this.interactIndexInGroup < 0)
		{
			this.interactIndexInGroup = num - 1;
		}
	}

	// Token: 0x0600021C RID: 540 RVA: 0x0000A181 File Offset: 0x00008381
	public void SetInteractableTarget(InteractableBase interactable)
	{
		this.masterInteractableAround = interactable;
		this.interactIndexInGroup = 0;
	}

	// Token: 0x0600021D RID: 541 RVA: 0x0000A194 File Offset: 0x00008394
	protected override bool OnStart()
	{
		InteractableBase interactTarget = this.InteractTarget;
		if (!interactTarget)
		{
			return false;
		}
		if (interactTarget.StartInteract(this.characterController))
		{
			this.interactingTarget = interactTarget;
			return true;
		}
		return false;
	}

	// Token: 0x0600021E RID: 542 RVA: 0x0000A1CA File Offset: 0x000083CA
	protected override void OnUpdateAction(float deltaTime)
	{
		if (this.interactingTarget == null)
		{
			base.StopAction();
			return;
		}
		if (!this.interactingTarget.Interacting)
		{
			base.StopAction();
			return;
		}
		this.interactingTarget.UpdateInteract(this.characterController, deltaTime);
	}

	// Token: 0x0600021F RID: 543 RVA: 0x0000A209 File Offset: 0x00008409
	public override CharacterActionBase.ActionPriorities ActionPriority()
	{
		return CharacterActionBase.ActionPriorities.Interact;
	}

	// Token: 0x06000220 RID: 544 RVA: 0x0000A20C File Offset: 0x0000840C
	public override bool CanMove()
	{
		return false;
	}

	// Token: 0x06000221 RID: 545 RVA: 0x0000A20F File Offset: 0x0000840F
	protected override void OnStop()
	{
		if (this.interactingTarget && this.interactingTarget.Interacting)
		{
			this.interactingTarget.InternalStopInteract();
		}
	}

	// Token: 0x06000222 RID: 546 RVA: 0x0000A236 File Offset: 0x00008436
	public override bool CanRun()
	{
		return false;
	}

	// Token: 0x06000223 RID: 547 RVA: 0x0000A239 File Offset: 0x00008439
	public override bool CanUseHand()
	{
		return false;
	}

	// Token: 0x06000224 RID: 548 RVA: 0x0000A23C File Offset: 0x0000843C
	public override bool CanControlAim()
	{
		return false;
	}

	// Token: 0x06000225 RID: 549 RVA: 0x0000A23F File Offset: 0x0000843F
	public override bool IsReady()
	{
		return !base.Running && this.InteractTarget != null;
	}

	// Token: 0x06000226 RID: 550 RVA: 0x0000A257 File Offset: 0x00008457
	public Progress GetProgress()
	{
		if (this.interactingTarget != null)
		{
			this.progress = this.interactingTarget.GetProgress();
		}
		else
		{
			this.progress.inProgress = false;
		}
		return this.progress;
	}

	// Token: 0x040001C6 RID: 454
	private InteractableBase masterInteractableAround;

	// Token: 0x040001C7 RID: 455
	private int interactIndexInGroup;

	// Token: 0x040001C8 RID: 456
	private InteractableBase interactingTarget;

	// Token: 0x040001C9 RID: 457
	private LayerMask interactLayers;

	// Token: 0x040001CA RID: 458
	private InteractableBase minDistanceInteractable;

	// Token: 0x040001CB RID: 459
	private Collider[] colliders;

	// Token: 0x040001CC RID: 460
	private Progress progress;
}
