using System;
using Duckov;
using UnityEngine;

// Token: 0x02000054 RID: 84
public class CA_Skill : CharacterActionBase, IProgress
{
	// Token: 0x1700006E RID: 110
	// (get) Token: 0x06000235 RID: 565 RVA: 0x0000A440 File Offset: 0x00008640
	public SkillBase CurrentRunningSkill
	{
		get
		{
			if (!base.Running || this.currentRunningSkillKeeper == null)
			{
				return null;
			}
			return this.currentRunningSkillKeeper.Skill;
		}
	}

	// Token: 0x06000236 RID: 566 RVA: 0x0000A45F File Offset: 0x0000865F
	public CharacterSkillKeeper GetSkillKeeper(SkillTypes skillType)
	{
		if (skillType == SkillTypes.itemSkill)
		{
			return this.holdItemSkillKeeper;
		}
		if (skillType != SkillTypes.characterSkill)
		{
			return null;
		}
		return this.characterSkillKeeper;
	}

	// Token: 0x06000237 RID: 567 RVA: 0x0000A479 File Offset: 0x00008679
	public override CharacterActionBase.ActionPriorities ActionPriority()
	{
		return CharacterActionBase.ActionPriorities.Skills;
	}

	// Token: 0x06000238 RID: 568 RVA: 0x0000A47C File Offset: 0x0000867C
	public override bool CanControlAim()
	{
		return true;
	}

	// Token: 0x06000239 RID: 569 RVA: 0x0000A47F File Offset: 0x0000867F
	public override bool CanEditInventory()
	{
		return false;
	}

	// Token: 0x0600023A RID: 570 RVA: 0x0000A482 File Offset: 0x00008682
	public override bool CanMove()
	{
		return !(this.CurrentRunningSkill != null) || this.CurrentRunningSkill.SkillContext.movableWhileAim;
	}

	// Token: 0x0600023B RID: 571 RVA: 0x0000A4A4 File Offset: 0x000086A4
	public override bool CanRun()
	{
		return false;
	}

	// Token: 0x0600023C RID: 572 RVA: 0x0000A4A7 File Offset: 0x000086A7
	public override bool CanUseHand()
	{
		return false;
	}

	// Token: 0x0600023D RID: 573 RVA: 0x0000A4AA File Offset: 0x000086AA
	public override bool IsReady()
	{
		return !base.Running;
	}

	// Token: 0x0600023E RID: 574 RVA: 0x0000A4B7 File Offset: 0x000086B7
	public bool IsSkillHasEnoughStaminaAndCD(SkillBase skill)
	{
		return this.characterController.CurrentStamina >= skill.staminaCost && Time.time - skill.LastReleaseTime >= skill.coolDownTime;
	}

	// Token: 0x0600023F RID: 575 RVA: 0x0000A4E8 File Offset: 0x000086E8
	protected override bool OnStart()
	{
		CharacterSkillKeeper skillKeeper = this.GetSkillKeeper(this.skillTypeToRelease);
		if (skillKeeper != null && skillKeeper.CheckSkillAndBinding())
		{
			if (skillKeeper.Skill != null)
			{
				if (!this.IsSkillHasEnoughStaminaAndCD(skillKeeper.Skill))
				{
					return false;
				}
				SkillContext skillContext = skillKeeper.Skill.SkillContext;
			}
			this.currentRunningSkillKeeper = skillKeeper;
			Debug.Log(string.Format("skillType is {0}", this.skillTypeToRelease));
			return true;
		}
		return false;
	}

	// Token: 0x06000240 RID: 576 RVA: 0x0000A55B File Offset: 0x0000875B
	public void SetNextSkillType(SkillTypes skillType)
	{
		if (base.Running)
		{
			return;
		}
		this.skillTypeToRelease = skillType;
	}

	// Token: 0x06000241 RID: 577 RVA: 0x0000A570 File Offset: 0x00008770
	public bool SetSkillOfType(SkillTypes skillType, SkillBase _skill, GameObject _bindingObject)
	{
		CharacterSkillKeeper skillKeeper = this.GetSkillKeeper(skillType);
		if (skillKeeper == null)
		{
			return false;
		}
		if (base.Running && skillKeeper == this.currentRunningSkillKeeper)
		{
			base.StopAction();
		}
		skillKeeper.SetSkill(_skill, _bindingObject);
		return true;
	}

	// Token: 0x06000242 RID: 578 RVA: 0x0000A5AC File Offset: 0x000087AC
	public bool ReleaseSkill(SkillTypes skillType)
	{
		if (!base.Running)
		{
			return false;
		}
		if (this.CurrentRunningSkill == null)
		{
			base.StopAction();
			return false;
		}
		if (skillType != this.skillTypeToRelease)
		{
			base.StopAction();
			return false;
		}
		if (!this.IsSkillHasEnoughStaminaAndCD(this.CurrentRunningSkill))
		{
			return false;
		}
		if (this.actionTimer < this.CurrentRunningSkill.SkillContext.skillReadyTime)
		{
			base.StopAction();
			return false;
		}
		Vector3 currentSkillAimPoint = this.characterController.GetCurrentSkillAimPoint();
		SkillReleaseContext skillReleaseContext = default(SkillReleaseContext);
		skillReleaseContext.releasePoint = currentSkillAimPoint;
		this.CurrentRunningSkill.ReleaseSkill(skillReleaseContext, this.characterController);
		this.currentRunningSkillKeeper = null;
		base.StopAction();
		return true;
	}

	// Token: 0x06000243 RID: 579 RVA: 0x0000A65A File Offset: 0x0000885A
	protected override void OnStop()
	{
		this.currentRunningSkillKeeper = null;
	}

	// Token: 0x06000244 RID: 580 RVA: 0x0000A663 File Offset: 0x00008863
	protected override void OnUpdateAction(float deltaTime)
	{
		if (this.currentRunningSkillKeeper == null || !this.currentRunningSkillKeeper.CheckSkillAndBinding())
		{
			base.StopAction();
		}
	}

	// Token: 0x06000245 RID: 581 RVA: 0x0000A684 File Offset: 0x00008884
	public Progress GetProgress()
	{
		Progress progress = default(Progress);
		SkillBase currentRunningSkill = this.CurrentRunningSkill;
		if (currentRunningSkill != null)
		{
			progress.total = currentRunningSkill.SkillContext.skillReadyTime;
			progress.current = this.actionTimer;
			progress.inProgress = progress.progress < 1f;
		}
		else
		{
			progress.inProgress = false;
		}
		return progress;
	}

	// Token: 0x040001D2 RID: 466
	[SerializeField]
	public CharacterSkillKeeper holdItemSkillKeeper;

	// Token: 0x040001D3 RID: 467
	[SerializeField]
	public CharacterSkillKeeper characterSkillKeeper;

	// Token: 0x040001D4 RID: 468
	private SkillTypes skillTypeToRelease;

	// Token: 0x040001D5 RID: 469
	private CharacterSkillKeeper currentRunningSkillKeeper;
}
