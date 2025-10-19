using System;
using Duckov.Rules;
using SodaCraft.Localizations;
using TMPro;
using UnityEngine;

// Token: 0x0200015B RID: 347
public class RuleIndexDisplay : MonoBehaviour
{
	// Token: 0x06000AA5 RID: 2725 RVA: 0x0002E42E File Offset: 0x0002C62E
	private void Awake()
	{
		LocalizationManager.OnSetLanguage += this.OnLanguageChanged;
	}

	// Token: 0x06000AA6 RID: 2726 RVA: 0x0002E441 File Offset: 0x0002C641
	private void OnDestroy()
	{
		LocalizationManager.OnSetLanguage -= this.OnLanguageChanged;
	}

	// Token: 0x06000AA7 RID: 2727 RVA: 0x0002E454 File Offset: 0x0002C654
	private void OnLanguageChanged(SystemLanguage language)
	{
		this.Refresh();
	}

	// Token: 0x06000AA8 RID: 2728 RVA: 0x0002E45C File Offset: 0x0002C65C
	private void OnEnable()
	{
		this.Refresh();
	}

	// Token: 0x06000AA9 RID: 2729 RVA: 0x0002E464 File Offset: 0x0002C664
	private void Refresh()
	{
		this.text.text = string.Format("Rule_{0}", GameRulesManager.SelectedRuleIndex).ToPlainText();
	}

	// Token: 0x04000949 RID: 2377
	[SerializeField]
	private TextMeshProUGUI text;
}
