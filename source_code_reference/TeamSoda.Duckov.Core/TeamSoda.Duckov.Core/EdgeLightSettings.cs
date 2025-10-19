using System;
using Duckov.Options;
using SodaCraft.Localizations;

// Token: 0x020001CE RID: 462
public class EdgeLightSettings : OptionsProviderBase
{
	// Token: 0x17000285 RID: 645
	// (get) Token: 0x06000DB3 RID: 3507 RVA: 0x00038146 File Offset: 0x00036346
	public override string Key
	{
		get
		{
			return "EdgeLightSetting";
		}
	}

	// Token: 0x06000DB4 RID: 3508 RVA: 0x0003814D File Offset: 0x0003634D
	public override string[] GetOptions()
	{
		return new string[]
		{
			this.offKey.ToPlainText(),
			this.onKey.ToPlainText()
		};
	}

	// Token: 0x06000DB5 RID: 3509 RVA: 0x00038174 File Offset: 0x00036374
	public override string GetCurrentOption()
	{
		int num = OptionsManager.Load<int>(this.Key, 1);
		if (num == 0)
		{
			return this.offKey.ToPlainText();
		}
		if (num != 1)
		{
			return this.onKey.ToPlainText();
		}
		return this.onKey.ToPlainText();
	}

	// Token: 0x06000DB6 RID: 3510 RVA: 0x000381BA File Offset: 0x000363BA
	public override void Set(int index)
	{
		if (index != 0)
		{
			if (index == 1)
			{
				EdgeLightEntry.SetEnabled(true);
			}
		}
		else
		{
			EdgeLightEntry.SetEnabled(false);
		}
		OptionsManager.Save<int>(this.Key, index);
	}

	// Token: 0x06000DB7 RID: 3511 RVA: 0x000381DF File Offset: 0x000363DF
	private void Awake()
	{
		LevelManager.OnLevelInitialized += this.RefreshOnLevelInited;
	}

	// Token: 0x06000DB8 RID: 3512 RVA: 0x000381F2 File Offset: 0x000363F2
	private void OnDestroy()
	{
		LevelManager.OnLevelInitialized -= this.RefreshOnLevelInited;
	}

	// Token: 0x06000DB9 RID: 3513 RVA: 0x00038208 File Offset: 0x00036408
	private void RefreshOnLevelInited()
	{
		int num = OptionsManager.Load<int>(this.Key, 1);
		this.Set(num);
	}

	// Token: 0x04000B90 RID: 2960
	[LocalizationKey("Default")]
	public string onKey = "Options_On";

	// Token: 0x04000B91 RID: 2961
	[LocalizationKey("Default")]
	public string offKey = "Options_Off";
}
