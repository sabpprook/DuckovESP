using System;
using Duckov;
using FMOD.Studio;
using ItemStatsSystem;
using UnityEngine;

// Token: 0x02000055 RID: 85
public class CA_UseItem : CharacterActionBase, IProgress
{
	// Token: 0x06000247 RID: 583 RVA: 0x0000A6F0 File Offset: 0x000088F0
	public override CharacterActionBase.ActionPriorities ActionPriority()
	{
		return CharacterActionBase.ActionPriorities.usingItem;
	}

	// Token: 0x06000248 RID: 584 RVA: 0x0000A6F3 File Offset: 0x000088F3
	public override bool CanMove()
	{
		return true;
	}

	// Token: 0x06000249 RID: 585 RVA: 0x0000A6F6 File Offset: 0x000088F6
	public override bool CanRun()
	{
		return false;
	}

	// Token: 0x0600024A RID: 586 RVA: 0x0000A6F9 File Offset: 0x000088F9
	public override bool CanUseHand()
	{
		return false;
	}

	// Token: 0x0600024B RID: 587 RVA: 0x0000A6FC File Offset: 0x000088FC
	public override bool CanControlAim()
	{
		return true;
	}

	// Token: 0x0600024C RID: 588 RVA: 0x0000A6FF File Offset: 0x000088FF
	public override bool IsReady()
	{
		return true;
	}

	// Token: 0x0600024D RID: 589 RVA: 0x0000A704 File Offset: 0x00008904
	protected override bool OnStart()
	{
		this.agentUsable = null;
		bool flag = false;
		if (this.item.AgentUtilities.ActiveAgent == null)
		{
			if (this.characterController.ChangeHoldItem(this.item) && this.characterController.CurrentHoldItemAgent != null)
			{
				this.agentUsable = this.characterController.CurrentHoldItemAgent as IAgentUsable;
				flag = true;
			}
		}
		else if (this.item.AgentUtilities.ActiveAgent == this.characterController.CurrentHoldItemAgent)
		{
			flag = true;
		}
		if (flag)
		{
			this.PostActionSound();
		}
		return flag;
	}

	// Token: 0x0600024E RID: 590 RVA: 0x0000A7A0 File Offset: 0x000089A0
	protected override void OnStop()
	{
		this.StopSound();
		this.characterController.SwitchToWeaponBeforeUse();
		if (this.item != null && !this.item.IsBeingDestroyed && this.item.GetRoot() != this.characterController.CharacterItem && !this.characterController.PickupItem(this.item))
		{
			this.item.Drop(this.characterController, true);
		}
	}

	// Token: 0x0600024F RID: 591 RVA: 0x0000A81C File Offset: 0x00008A1C
	public void SetUseItem(Item _item)
	{
		this.item = _item;
		this.hasSound = false;
		UsageUtilities component = this.item.GetComponent<UsageUtilities>();
		if (component)
		{
			this.hasSound = component.hasSound;
			this.actionSound = component.actionSound;
			this.useSound = component.useSound;
		}
	}

	// Token: 0x06000250 RID: 592 RVA: 0x0000A870 File Offset: 0x00008A70
	protected override void OnUpdateAction(float deltaTime)
	{
		if (this.item == null)
		{
			base.StopAction();
			return;
		}
		if (this.characterController.CurrentHoldItemAgent == null || this.characterController.CurrentHoldItemAgent.Item == null || this.characterController.CurrentHoldItemAgent.Item != this.item)
		{
			Debug.Log("拿的不统一");
			base.StopAction();
			return;
		}
		if (base.ActionTimer > this.characterController.CurrentHoldItemAgent.Item.UseTime)
		{
			this.OnFinish();
			Debug.Log("Use Finished");
			base.StopAction();
		}
	}

	// Token: 0x06000251 RID: 593 RVA: 0x0000A924 File Offset: 0x00008B24
	private void OnFinish()
	{
		this.item.Use(this.characterController);
		this.PostUseSound();
		if (this.item.Stackable)
		{
			this.item.StackCount = this.item.StackCount - 1;
			return;
		}
		if (this.item.UseDurability)
		{
			if (this.item.Durability <= 0f && !this.item.IsBeingDestroyed)
			{
				this.item.DestroyTree();
				return;
			}
		}
		else
		{
			this.item.DestroyTree();
		}
	}

	// Token: 0x06000252 RID: 594 RVA: 0x0000A9B4 File Offset: 0x00008BB4
	public Progress GetProgress()
	{
		Progress progress = default(Progress);
		if (this.item != null && base.Running)
		{
			progress.inProgress = true;
			progress.total = this.item.UseTime;
			progress.current = this.actionTimer;
			return progress;
		}
		progress.inProgress = false;
		return progress;
	}

	// Token: 0x06000253 RID: 595 RVA: 0x0000AA11 File Offset: 0x00008C11
	private void OnDestroy()
	{
		this.StopSound();
	}

	// Token: 0x06000254 RID: 596 RVA: 0x0000AA19 File Offset: 0x00008C19
	private void OnDisable()
	{
		this.StopSound();
	}

	// Token: 0x06000255 RID: 597 RVA: 0x0000AA21 File Offset: 0x00008C21
	private void PostActionSound()
	{
		if (!this.hasSound)
		{
			return;
		}
		this.soundInstance = AudioManager.Post(this.actionSound, base.gameObject);
	}

	// Token: 0x06000256 RID: 598 RVA: 0x0000AA43 File Offset: 0x00008C43
	private void PostUseSound()
	{
		if (!this.hasSound)
		{
			return;
		}
		AudioManager.Post(this.useSound, base.gameObject);
	}

	// Token: 0x06000257 RID: 599 RVA: 0x0000AA60 File Offset: 0x00008C60
	private void StopSound()
	{
		if (this.soundInstance != null)
		{
			this.soundInstance.Value.stop(STOP_MODE.ALLOWFADEOUT);
		}
	}

	// Token: 0x040001D6 RID: 470
	private Item item;

	// Token: 0x040001D7 RID: 471
	public IAgentUsable agentUsable;

	// Token: 0x040001D8 RID: 472
	public bool hasSound;

	// Token: 0x040001D9 RID: 473
	public string actionSound;

	// Token: 0x040001DA RID: 474
	public string useSound;

	// Token: 0x040001DB RID: 475
	private EventInstance? soundInstance;
}
