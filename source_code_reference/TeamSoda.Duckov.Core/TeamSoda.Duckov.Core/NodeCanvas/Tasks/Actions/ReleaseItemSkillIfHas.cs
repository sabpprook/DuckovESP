using System;
using NodeCanvas.Framework;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions
{
	// Token: 0x0200040D RID: 1037
	public class ReleaseItemSkillIfHas : ActionTask<AICharacterController>
	{
		// Token: 0x17000730 RID: 1840
		// (get) Token: 0x06002562 RID: 9570 RVA: 0x00080CC2 File Offset: 0x0007EEC2
		private float chance
		{
			get
			{
				if (!base.agent)
				{
					return 0f;
				}
				return base.agent.itemSkillChance;
			}
		}

		// Token: 0x17000731 RID: 1841
		// (get) Token: 0x06002563 RID: 9571 RVA: 0x00080CE2 File Offset: 0x0007EEE2
		public float checkTimeSpace
		{
			get
			{
				if (!base.agent)
				{
					return 999f;
				}
				return base.agent.itemSkillCoolTime;
			}
		}

		// Token: 0x06002564 RID: 9572 RVA: 0x00080D02 File Offset: 0x0007EF02
		protected override string OnInit()
		{
			return null;
		}

		// Token: 0x06002565 RID: 9573 RVA: 0x00080D08 File Offset: 0x0007EF08
		protected override void OnExecute()
		{
			this.skillRefrence = null;
			if (Time.time - this.checkTimeMarker < this.checkTimeSpace)
			{
				base.EndAction(false);
				return;
			}
			this.checkTimeMarker = Time.time;
			if (global::UnityEngine.Random.Range(0f, 1f) > this.chance)
			{
				base.EndAction(false);
				return;
			}
			ItemSetting_Skill itemSkill = base.agent.GetItemSkill(this.random);
			if (!itemSkill)
			{
				base.EndAction(false);
				return;
			}
			if (base.agent.CharacterMainControl.CurrentAction && base.agent.CharacterMainControl.CurrentAction.Running)
			{
				base.EndAction(false);
				return;
			}
			this.skillRefrence = itemSkill;
			base.agent.CharacterMainControl.ChangeHoldItem(itemSkill.Item);
			base.agent.CharacterMainControl.SetSkill(SkillTypes.itemSkill, itemSkill.Skill, itemSkill.gameObject);
			if (!base.agent.CharacterMainControl.StartSkillAim(SkillTypes.itemSkill))
			{
				base.EndAction(false);
				return;
			}
			this.readyTime = itemSkill.Skill.SkillContext.skillReadyTime;
		}

		// Token: 0x06002566 RID: 9574 RVA: 0x00080E28 File Offset: 0x0007F028
		protected override void OnUpdate()
		{
			if (!this.skillRefrence)
			{
				base.EndAction(false);
				return;
			}
			if (base.agent.searchedEnemy)
			{
				base.agent.CharacterMainControl.SetAimPoint(base.agent.searchedEnemy.transform.position);
			}
			if (base.elapsedTime > this.readyTime + 0.1f)
			{
				base.agent.CharacterMainControl.ReleaseSkill(SkillTypes.itemSkill);
				base.EndAction(true);
				return;
			}
		}

		// Token: 0x06002567 RID: 9575 RVA: 0x00080EAF File Offset: 0x0007F0AF
		protected override void OnStop()
		{
			base.agent.CharacterMainControl.CancleSkill();
			base.agent.CharacterMainControl.SwitchToFirstAvailableWeapon();
		}

		// Token: 0x04001972 RID: 6514
		public bool random = true;

		// Token: 0x04001973 RID: 6515
		private float checkTimeMarker = -1f;

		// Token: 0x04001974 RID: 6516
		private float readyTime;

		// Token: 0x04001975 RID: 6517
		private ItemSetting_Skill skillRefrence;
	}
}
