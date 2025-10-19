using System;
using Duckov;
using ItemStatsSystem;

// Token: 0x02000052 RID: 82
public class CA_Reload : CharacterActionBase, IProgress
{
	// Token: 0x06000228 RID: 552 RVA: 0x0000A294 File Offset: 0x00008494
	public override CharacterActionBase.ActionPriorities ActionPriority()
	{
		return CharacterActionBase.ActionPriorities.Reload;
	}

	// Token: 0x06000229 RID: 553 RVA: 0x0000A297 File Offset: 0x00008497
	public override bool CanMove()
	{
		return true;
	}

	// Token: 0x0600022A RID: 554 RVA: 0x0000A29A File Offset: 0x0000849A
	public override bool CanRun()
	{
		return true;
	}

	// Token: 0x0600022B RID: 555 RVA: 0x0000A29D File Offset: 0x0000849D
	public override bool CanUseHand()
	{
		return false;
	}

	// Token: 0x0600022C RID: 556 RVA: 0x0000A2A0 File Offset: 0x000084A0
	public override bool CanControlAim()
	{
		return true;
	}

	// Token: 0x0600022D RID: 557 RVA: 0x0000A2A3 File Offset: 0x000084A3
	public override bool IsReady()
	{
		this.currentGun = this.characterController.agentHolder.CurrentHoldGun;
		return this.currentGun && !this.currentGun.IsReloading();
	}

	// Token: 0x0600022E RID: 558 RVA: 0x0000A2DC File Offset: 0x000084DC
	protected override bool OnStart()
	{
		this.currentGun = null;
		if (!this.characterController || !this.characterController.CurrentHoldItemAgent)
		{
			return false;
		}
		this.currentGun = this.characterController.agentHolder.CurrentHoldGun;
		this.currentGun.GunItemSetting.PreferdBulletsToLoad = this.preferedBulletToReload;
		this.preferedBulletToReload = null;
		return this.currentGun != null && this.currentGun.BeginReload();
	}

	// Token: 0x0600022F RID: 559 RVA: 0x0000A362 File Offset: 0x00008562
	protected override void OnStop()
	{
		if (this.currentGun != null)
		{
			this.currentGun.CancleReload();
		}
	}

	// Token: 0x06000230 RID: 560 RVA: 0x0000A380 File Offset: 0x00008580
	public bool GetGunReloadable()
	{
		if (this.currentGun == null)
		{
			this.currentGun = this.characterController.agentHolder.CurrentHoldGun;
			return false;
		}
		return !base.Running && !this.currentGun.IsFull();
	}

	// Token: 0x06000231 RID: 561 RVA: 0x0000A3CF File Offset: 0x000085CF
	public override bool CanEditInventory()
	{
		return true;
	}

	// Token: 0x06000232 RID: 562 RVA: 0x0000A3D2 File Offset: 0x000085D2
	protected override void OnUpdateAction(float deltaTime)
	{
		if (this.currentGun == null)
		{
			base.StopAction();
			return;
		}
		if (!this.currentGun.IsReloading())
		{
			base.StopAction();
		}
	}

	// Token: 0x06000233 RID: 563 RVA: 0x0000A400 File Offset: 0x00008600
	public Progress GetProgress()
	{
		if (this.currentGun != null)
		{
			return this.currentGun.GetReloadProgress();
		}
		return new Progress
		{
			inProgress = false
		};
	}

	// Token: 0x040001CD RID: 461
	public ItemAgent_Gun currentGun;

	// Token: 0x040001CE RID: 462
	public Item preferedBulletToReload;
}
