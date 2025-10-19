using System;
using UnityEngine;

// Token: 0x020000CA RID: 202
public class SkillHud3D : MonoBehaviour
{
	// Token: 0x0600064C RID: 1612 RVA: 0x0001C6C6 File Offset: 0x0001A8C6
	private void Awake()
	{
		this.HideAll();
	}

	// Token: 0x0600064D RID: 1613 RVA: 0x0001C6CE File Offset: 0x0001A8CE
	private void HideAll()
	{
		this.skillRangeHUD.gameObject.SetActive(false);
		this.projectileLine.gameObject.SetActive(false);
	}

	// Token: 0x0600064E RID: 1614 RVA: 0x0001C6F4 File Offset: 0x0001A8F4
	private void LateUpdate()
	{
		if (!this.character)
		{
			this.character = LevelManager.Instance.MainCharacter;
			return;
		}
		this.currentSkill = null;
		this.currentSkill = this.character.skillAction.CurrentRunningSkill;
		if (this.aiming != (this.currentSkill != null))
		{
			this.aiming = !this.aiming;
			if (this.currentSkill != null)
			{
				this.currentSkill = this.character.skillAction.CurrentRunningSkill;
				this.skillRangeHUD.gameObject.SetActive(true);
				float num = 1f;
				if (this.currentSkill.SkillContext.effectRange > 1f)
				{
					num = this.currentSkill.SkillContext.effectRange;
				}
				this.skillRangeHUD.SetRange(num);
				if (this.currentSkill.SkillContext.isGrenade)
				{
					this.projectileLine.gameObject.SetActive(true);
				}
			}
			else
			{
				this.HideAll();
			}
		}
		Vector3 currentSkillAimPoint = this.character.GetCurrentSkillAimPoint();
		Vector3 one = Vector3.one;
		if (this.projectileLine.gameObject.activeSelf)
		{
			bool flag = this.projectileLine.UpdateLine(this.character.CurrentUsingAimSocket.position, currentSkillAimPoint, this.currentSkill.SkillContext.grenageVerticleSpeed, ref one);
		}
		this.skillRangeHUD.transform.position = currentSkillAimPoint;
		this.skillRangeHUD.SetProgress(this.character.skillAction.GetProgress().progress);
	}

	// Token: 0x04000615 RID: 1557
	private CharacterMainControl character;

	// Token: 0x04000616 RID: 1558
	private bool aiming;

	// Token: 0x04000617 RID: 1559
	public SkillRangeHUD skillRangeHUD;

	// Token: 0x04000618 RID: 1560
	public SkillProjectileLineHUD projectileLine;

	// Token: 0x04000619 RID: 1561
	private SkillBase currentSkill;
}
