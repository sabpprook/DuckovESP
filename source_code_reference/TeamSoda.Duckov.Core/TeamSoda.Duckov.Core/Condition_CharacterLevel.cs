using System;
using Duckov;
using Duckov.Quests;
using SodaCraft.Localizations;
using SodaCraft.StringUtilities;
using UnityEngine;

// Token: 0x02000114 RID: 276
public class Condition_CharacterLevel : Condition
{
	// Token: 0x170001F1 RID: 497
	// (get) Token: 0x06000961 RID: 2401 RVA: 0x000291D0 File Offset: 0x000273D0
	[LocalizationKey("Default")]
	private string DisplayTextFormatKey
	{
		get
		{
			switch (this.relation)
			{
			case Condition_CharacterLevel.Relation.LessThan:
				return "Condition_CharacterLevel_LessThan";
			case Condition_CharacterLevel.Relation.Equals:
				return "Condition_CharacterLevel_Equals";
			case Condition_CharacterLevel.Relation.GreaterThan:
				return "Condition_CharacterLevel_GreaterThan";
			}
			return "";
		}
	}

	// Token: 0x170001F2 RID: 498
	// (get) Token: 0x06000962 RID: 2402 RVA: 0x00029215 File Offset: 0x00027415
	private string DisplayTextFormat
	{
		get
		{
			return this.DisplayTextFormatKey.ToPlainText();
		}
	}

	// Token: 0x170001F3 RID: 499
	// (get) Token: 0x06000963 RID: 2403 RVA: 0x00029222 File Offset: 0x00027422
	public override string DisplayText
	{
		get
		{
			return this.DisplayTextFormat.Format(new { this.level });
		}
	}

	// Token: 0x06000964 RID: 2404 RVA: 0x0002923C File Offset: 0x0002743C
	public override bool Evaluate()
	{
		int num = EXPManager.Level;
		switch (this.relation)
		{
		case Condition_CharacterLevel.Relation.LessThan:
			return num <= this.level;
		case Condition_CharacterLevel.Relation.Equals:
			return num == this.level;
		case Condition_CharacterLevel.Relation.GreaterThan:
			return num >= this.level;
		}
		return false;
	}

	// Token: 0x0400084D RID: 2125
	[SerializeField]
	private Condition_CharacterLevel.Relation relation;

	// Token: 0x0400084E RID: 2126
	[SerializeField]
	private int level;

	// Token: 0x02000493 RID: 1171
	private enum Relation
	{
		// Token: 0x04001BC8 RID: 7112
		LessThan = 1,
		// Token: 0x04001BC9 RID: 7113
		Equals,
		// Token: 0x04001BCA RID: 7114
		GreaterThan = 4
	}
}
