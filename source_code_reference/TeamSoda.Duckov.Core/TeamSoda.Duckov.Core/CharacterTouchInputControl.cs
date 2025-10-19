using System;
using UnityEngine;

// Token: 0x02000078 RID: 120
public class CharacterTouchInputControl : MonoBehaviour
{
	// Token: 0x0600048A RID: 1162 RVA: 0x00014D91 File Offset: 0x00012F91
	public void SetMoveInput(Vector2 axisInput, bool holding)
	{
		this.characterInputManager.SetMoveInput(axisInput);
	}

	// Token: 0x0600048B RID: 1163 RVA: 0x00014D9F File Offset: 0x00012F9F
	public void SetRunInput(bool holding)
	{
		this.characterInputManager.SetRunInput(holding);
	}

	// Token: 0x0600048C RID: 1164 RVA: 0x00014DAD File Offset: 0x00012FAD
	public void SetAdsInput(bool holding)
	{
		this.characterInputManager.SetAdsInput(holding);
	}

	// Token: 0x0600048D RID: 1165 RVA: 0x00014DBB File Offset: 0x00012FBB
	public void SetGunAimInput(Vector2 axisInput, bool holding)
	{
		this.characterInputManager.SetAimInputUsingJoystick(axisInput);
		this.characterInputManager.SetAimType(AimTypes.normalAim);
	}

	// Token: 0x0600048E RID: 1166 RVA: 0x00014DD5 File Offset: 0x00012FD5
	public void SetCharacterSkillAimInput(Vector2 axisInput, bool holding)
	{
		this.characterInputManager.SetAimInputUsingJoystick(axisInput);
		this.characterInputManager.SetAimType(AimTypes.characterSkill);
	}

	// Token: 0x0600048F RID: 1167 RVA: 0x00014DEF File Offset: 0x00012FEF
	public void StartCharacterSkillAim()
	{
		this.characterInputManager.StartCharacterSkillAim();
	}

	// Token: 0x06000490 RID: 1168 RVA: 0x00014DFC File Offset: 0x00012FFC
	public void CharacterSkillRelease(bool trigger)
	{
		if (!trigger)
		{
			this.characterInputManager.CancleSkill();
			return;
		}
		this.characterInputManager.ReleaseCharacterSkill();
	}

	// Token: 0x06000491 RID: 1169 RVA: 0x00014E19 File Offset: 0x00013019
	public void SetItemSkillAimInput(Vector2 axisInput, bool holding)
	{
		this.characterInputManager.SetAimInputUsingJoystick(axisInput);
		this.characterInputManager.SetAimType(AimTypes.handheldSkill);
	}

	// Token: 0x06000492 RID: 1170 RVA: 0x00014E33 File Offset: 0x00013033
	public void StartItemSkillAim()
	{
		this.characterInputManager.StartItemSkillAim();
	}

	// Token: 0x06000493 RID: 1171 RVA: 0x00014E40 File Offset: 0x00013040
	public void ItemSkillRelease(bool trigger)
	{
		if (!trigger)
		{
			this.characterInputManager.CancleSkill();
			return;
		}
		this.characterInputManager.ReleaseItemSkill();
	}

	// Token: 0x040003D5 RID: 981
	public InputManager characterInputManager;
}
