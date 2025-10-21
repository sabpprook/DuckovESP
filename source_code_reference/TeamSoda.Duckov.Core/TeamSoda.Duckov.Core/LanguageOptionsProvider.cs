using System;
using SodaCraft.Localizations;
using UnityEngine;

// Token: 0x020001C6 RID: 454
public class LanguageOptionsProvider : OptionsProviderBase
{
	// Token: 0x1700027F RID: 639
	// (get) Token: 0x06000D7B RID: 3451 RVA: 0x0003781B File Offset: 0x00035A1B
	public override string Key
	{
		get
		{
			return "Language";
		}
	}

	// Token: 0x06000D7C RID: 3452 RVA: 0x00037822 File Offset: 0x00035A22
	public override string GetCurrentOption()
	{
		return LocalizationManager.CurrentLanguageDisplayName;
	}

	// Token: 0x06000D7D RID: 3453 RVA: 0x0003782C File Offset: 0x00035A2C
	public override string[] GetOptions()
	{
		LocalizationDatabase instance = LocalizationDatabase.Instance;
		if (instance == null)
		{
			return new string[] { "?" };
		}
		string[] languageDisplayNameList = instance.GetLanguageDisplayNameList();
		this.cache = languageDisplayNameList;
		return languageDisplayNameList;
	}

	// Token: 0x06000D7E RID: 3454 RVA: 0x00037866 File Offset: 0x00035A66
	public override void Set(int index)
	{
		if (this.cache == null)
		{
			this.GetOptions();
		}
		if (index < 0 || index >= this.cache.Length)
		{
			Debug.LogError("语言越界");
			return;
		}
		LocalizationManager.SetLanguage(index);
	}

	// Token: 0x04000B7E RID: 2942
	private string[] cache;
}
