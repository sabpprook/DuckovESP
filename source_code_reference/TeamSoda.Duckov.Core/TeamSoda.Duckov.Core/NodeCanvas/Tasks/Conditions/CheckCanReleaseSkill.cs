using System;
using NodeCanvas.Framework;
using UnityEngine;

namespace NodeCanvas.Tasks.Conditions
{
	// Token: 0x020003FC RID: 1020
	public class CheckCanReleaseSkill : ConditionTask<AICharacterController>
	{
		// Token: 0x0600250B RID: 9483 RVA: 0x0007FEE8 File Offset: 0x0007E0E8
		protected override bool OnCheck()
		{
			if (base.agent == null)
			{
				return false;
			}
			if (!base.agent.hasSkill)
			{
				return false;
			}
			if (!base.agent.skillInstance)
			{
				return false;
			}
			if (Time.time < base.agent.nextReleaseSkillTimeMarker)
			{
				return false;
			}
			if (!base.agent.CharacterMainControl.skillAction.IsSkillHasEnoughStaminaAndCD(base.agent.skillInstance))
			{
				return false;
			}
			if (base.agent.CharacterMainControl.CurrentAction && base.agent.CharacterMainControl.CurrentAction.Running)
			{
				return false;
			}
			base.agent.nextReleaseSkillTimeMarker = Time.time + global::UnityEngine.Random.Range(base.agent.skillCoolTimeRange.x, base.agent.skillCoolTimeRange.y);
			return global::UnityEngine.Random.Range(0f, 1f) <= base.agent.skillSuccessChance;
		}
	}
}
