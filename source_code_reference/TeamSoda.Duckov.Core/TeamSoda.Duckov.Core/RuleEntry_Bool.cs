using System;
using System.Reflection;
using Duckov.Rules;
using SodaCraft.Localizations;
using UnityEngine;

// Token: 0x020001DE RID: 478
public class RuleEntry_Bool : OptionsProviderBase
{
	// Token: 0x17000296 RID: 662
	// (get) Token: 0x06000E31 RID: 3633 RVA: 0x00039451 File Offset: 0x00037651
	public override string Key
	{
		get
		{
			return this.fieldName;
		}
	}

	// Token: 0x06000E32 RID: 3634 RVA: 0x00039459 File Offset: 0x00037659
	private void Awake()
	{
		this.field = typeof(Ruleset).GetField(this.fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
	}

	// Token: 0x06000E33 RID: 3635 RVA: 0x00039478 File Offset: 0x00037678
	public override string GetCurrentOption()
	{
		Ruleset ruleset = GameRulesManager.Current;
		if ((bool)this.field.GetValue(ruleset))
		{
			return "Options_On".ToPlainText();
		}
		return "Options_Off".ToPlainText();
	}

	// Token: 0x06000E34 RID: 3636 RVA: 0x000394B3 File Offset: 0x000376B3
	public override string[] GetOptions()
	{
		return new string[]
		{
			"Options_Off".ToPlainText(),
			"Options_On".ToPlainText()
		};
	}

	// Token: 0x06000E35 RID: 3637 RVA: 0x000394D8 File Offset: 0x000376D8
	public override void Set(int index)
	{
		bool flag = index > 0;
		Ruleset ruleset = GameRulesManager.Current;
		this.field.SetValue(ruleset, flag);
	}

	// Token: 0x04000BC1 RID: 3009
	[SerializeField]
	private string fieldName;

	// Token: 0x04000BC2 RID: 3010
	private FieldInfo field;
}
