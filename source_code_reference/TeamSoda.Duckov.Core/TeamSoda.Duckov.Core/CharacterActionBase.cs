using System;
using UnityEngine;

// Token: 0x02000056 RID: 86
public abstract class CharacterActionBase : MonoBehaviour
{
	// Token: 0x1700006F RID: 111
	// (get) Token: 0x06000259 RID: 601 RVA: 0x0000AA97 File Offset: 0x00008C97
	public bool Running
	{
		get
		{
			return this.running;
		}
	}

	// Token: 0x17000070 RID: 112
	// (get) Token: 0x0600025A RID: 602 RVA: 0x0000AA9F File Offset: 0x00008C9F
	public float ActionTimer
	{
		get
		{
			return this.actionTimer;
		}
	}

	// Token: 0x0600025B RID: 603
	public abstract CharacterActionBase.ActionPriorities ActionPriority();

	// Token: 0x0600025C RID: 604
	public abstract bool CanMove();

	// Token: 0x0600025D RID: 605
	public abstract bool CanRun();

	// Token: 0x0600025E RID: 606
	public abstract bool CanUseHand();

	// Token: 0x0600025F RID: 607
	public abstract bool CanControlAim();

	// Token: 0x06000260 RID: 608 RVA: 0x0000AAA7 File Offset: 0x00008CA7
	public virtual bool CanEditInventory()
	{
		return false;
	}

	// Token: 0x06000261 RID: 609 RVA: 0x0000AAAA File Offset: 0x00008CAA
	public void UpdateAction(float deltaTime)
	{
		this.actionTimer += deltaTime;
		this.OnUpdateAction(deltaTime);
	}

	// Token: 0x06000262 RID: 610 RVA: 0x0000AAC1 File Offset: 0x00008CC1
	protected virtual void OnUpdateAction(float deltaTime)
	{
	}

	// Token: 0x06000263 RID: 611 RVA: 0x0000AAC3 File Offset: 0x00008CC3
	protected virtual bool OnStart()
	{
		return true;
	}

	// Token: 0x06000264 RID: 612 RVA: 0x0000AAC6 File Offset: 0x00008CC6
	public virtual bool IsStopable()
	{
		return true;
	}

	// Token: 0x06000265 RID: 613 RVA: 0x0000AAC9 File Offset: 0x00008CC9
	protected virtual void OnStop()
	{
	}

	// Token: 0x06000266 RID: 614
	public abstract bool IsReady();

	// Token: 0x06000267 RID: 615 RVA: 0x0000AACB File Offset: 0x00008CCB
	public bool StartActionByCharacter(CharacterMainControl _character)
	{
		if (!this.IsReady())
		{
			return false;
		}
		this.characterController = _character;
		if (this.OnStart())
		{
			this.actionTimer = 0f;
			this.running = true;
			return true;
		}
		return false;
	}

	// Token: 0x06000268 RID: 616 RVA: 0x0000AAFB File Offset: 0x00008CFB
	public bool StopAction()
	{
		if (!this.running)
		{
			return true;
		}
		if (this.IsStopable())
		{
			this.running = false;
			this.OnStop();
			return true;
		}
		return false;
	}

	// Token: 0x040001DC RID: 476
	private bool running;

	// Token: 0x040001DD RID: 477
	protected float actionTimer;

	// Token: 0x040001DE RID: 478
	public bool progressHUD = true;

	// Token: 0x040001DF RID: 479
	public CharacterMainControl characterController;

	// Token: 0x0200042B RID: 1067
	public enum ActionPriorities
	{
		// Token: 0x040019FD RID: 6653
		Whatever,
		// Token: 0x040019FE RID: 6654
		Reload,
		// Token: 0x040019FF RID: 6655
		Attack,
		// Token: 0x04001A00 RID: 6656
		usingItem,
		// Token: 0x04001A01 RID: 6657
		Dash,
		// Token: 0x04001A02 RID: 6658
		Skills,
		// Token: 0x04001A03 RID: 6659
		Fishing,
		// Token: 0x04001A04 RID: 6660
		Interact
	}
}
