using System;
using UnityEngine;

// Token: 0x0200012C RID: 300
[Serializable]
public class CharacterSkillKeeper
{
	// Token: 0x17000204 RID: 516
	// (get) Token: 0x060009D0 RID: 2512 RVA: 0x0002A186 File Offset: 0x00028386
	public SkillBase Skill
	{
		get
		{
			return this.skill;
		}
	}

	// Token: 0x060009D1 RID: 2513 RVA: 0x0002A18E File Offset: 0x0002838E
	public void SetSkill(SkillBase _skill, GameObject _bindingObject)
	{
		this.skill = null;
		this.skillBindingObject = null;
		if (_skill != null && _bindingObject != null)
		{
			this.skill = _skill;
			this.skillBindingObject = _bindingObject;
		}
		Action onSkillChanged = this.OnSkillChanged;
		if (onSkillChanged == null)
		{
			return;
		}
		onSkillChanged();
	}

	// Token: 0x060009D2 RID: 2514 RVA: 0x0002A1CE File Offset: 0x000283CE
	public bool CheckSkillAndBinding()
	{
		if (this.skill != null && this.skillBindingObject != null)
		{
			return true;
		}
		this.skill = null;
		this.skillBindingObject = null;
		return false;
	}

	// Token: 0x0400088C RID: 2188
	private SkillBase skill;

	// Token: 0x0400088D RID: 2189
	private GameObject skillBindingObject;

	// Token: 0x0400088E RID: 2190
	public Action OnSkillChanged;
}
