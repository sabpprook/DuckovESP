using System;
using ItemStatsSystem;

// Token: 0x020000F4 RID: 244
public class ItemSetting_Skill : ItemSettingBase
{
	// Token: 0x060007FE RID: 2046 RVA: 0x000239A8 File Offset: 0x00021BA8
	public override void OnInit()
	{
		if (this.Skill)
		{
			SkillBase skill = this.Skill;
			skill.OnSkillReleasedEvent = (Action)Delegate.Combine(skill.OnSkillReleasedEvent, new Action(this.OnSkillReleased));
			this.Skill.fromItem = base.Item;
		}
	}

	// Token: 0x060007FF RID: 2047 RVA: 0x000239FC File Offset: 0x00021BFC
	private void OnSkillReleased()
	{
		ItemSetting_Skill.OnReleaseAction onReleaseAction = this.onRelease;
		if (onReleaseAction != ItemSetting_Skill.OnReleaseAction.none && onReleaseAction == ItemSetting_Skill.OnReleaseAction.reduceCount && (!LevelManager.Instance || !LevelManager.Instance.IsBaseLevel))
		{
			if (base.Item.Stackable)
			{
				base.Item.StackCount--;
				return;
			}
			base.Item.Detach();
			base.Item.DestroyTree();
		}
	}

	// Token: 0x06000800 RID: 2048 RVA: 0x00023A66 File Offset: 0x00021C66
	private void OnDestroy()
	{
		if (this.Skill)
		{
			SkillBase skill = this.Skill;
			skill.OnSkillReleasedEvent = (Action)Delegate.Remove(skill.OnSkillReleasedEvent, new Action(this.OnSkillReleased));
		}
	}

	// Token: 0x06000801 RID: 2049 RVA: 0x00023A9C File Offset: 0x00021C9C
	public override void SetMarkerParam(Item selfItem)
	{
		selfItem.SetBool("IsSkill", true, true);
	}

	// Token: 0x04000768 RID: 1896
	public ItemSetting_Skill.OnReleaseAction onRelease;

	// Token: 0x04000769 RID: 1897
	public SkillBase Skill;

	// Token: 0x0200046B RID: 1131
	public enum OnReleaseAction
	{
		// Token: 0x04001B41 RID: 6977
		none,
		// Token: 0x04001B42 RID: 6978
		reduceCount
	}
}
