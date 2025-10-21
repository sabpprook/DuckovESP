using System;
using NodeCanvas.Framework;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions
{
	// Token: 0x0200040E RID: 1038
	public class ReleaseSkill : ActionTask<AICharacterController>
	{
		// Token: 0x06002569 RID: 9577 RVA: 0x00080EED File Offset: 0x0007F0ED
		protected override string OnInit()
		{
			return null;
		}

		// Token: 0x0600256A RID: 9578 RVA: 0x00080EF0 File Offset: 0x0007F0F0
		protected override void OnExecute()
		{
			base.agent.CharacterMainControl.SetSkill(SkillTypes.characterSkill, base.agent.skillInstance, base.agent.skillInstance.gameObject);
			if (!base.agent.CharacterMainControl.StartSkillAim(SkillTypes.characterSkill))
			{
				base.EndAction(false);
				return;
			}
			this.readyTime = base.agent.skillInstance.SkillContext.skillReadyTime;
		}

		// Token: 0x0600256B RID: 9579 RVA: 0x00080F60 File Offset: 0x0007F160
		protected override void OnUpdate()
		{
			if (base.agent.searchedEnemy)
			{
				base.agent.CharacterMainControl.SetAimPoint(base.agent.searchedEnemy.transform.position);
			}
			if (base.elapsedTime <= this.readyTime + 0.1f)
			{
				return;
			}
			if (global::UnityEngine.Random.Range(0f, 1f) < base.agent.skillSuccessChance)
			{
				base.agent.CharacterMainControl.ReleaseSkill(SkillTypes.characterSkill);
				base.EndAction(true);
				return;
			}
			base.agent.CharacterMainControl.CancleSkill();
			base.EndAction(false);
		}

		// Token: 0x0600256C RID: 9580 RVA: 0x00081007 File Offset: 0x0007F207
		protected override void OnStop()
		{
			base.agent.CharacterMainControl.CancleSkill();
			base.agent.CharacterMainControl.SwitchToFirstAvailableWeapon();
		}

		// Token: 0x04001976 RID: 6518
		private float readyTime;

		// Token: 0x04001977 RID: 6519
		private float tryReleaseSkillTimeMarker = -1f;
	}
}
