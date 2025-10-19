using System;
using Duckov.Options;
using SodaCraft.Localizations;

// Token: 0x020001D8 RID: 472
public class SunFogSettings : OptionsProviderBase
{
	// Token: 0x1700028F RID: 655
	// (get) Token: 0x06000E07 RID: 3591 RVA: 0x00038F92 File Offset: 0x00037192
	public override string Key
	{
		get
		{
			return "SunFogSetting";
		}
	}

	// Token: 0x06000E08 RID: 3592 RVA: 0x00038F99 File Offset: 0x00037199
	public override string[] GetOptions()
	{
		return new string[]
		{
			this.offKey.ToPlainText(),
			this.onKey.ToPlainText()
		};
	}

	// Token: 0x06000E09 RID: 3593 RVA: 0x00038FC0 File Offset: 0x000371C0
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

	// Token: 0x06000E0A RID: 3594 RVA: 0x00039006 File Offset: 0x00037206
	public override void Set(int index)
	{
		if (index != 0)
		{
			if (index == 1)
			{
				SunFogEntry.SetEnabled(true);
			}
		}
		else
		{
			SunFogEntry.SetEnabled(false);
		}
		OptionsManager.Save<int>(this.Key, index);
	}

	// Token: 0x06000E0B RID: 3595 RVA: 0x0003902B File Offset: 0x0003722B
	private void Awake()
	{
		LevelManager.OnLevelInitialized += this.RefreshOnLevelInited;
	}

	// Token: 0x06000E0C RID: 3596 RVA: 0x0003903E File Offset: 0x0003723E
	private void OnDestroy()
	{
		LevelManager.OnLevelInitialized -= this.RefreshOnLevelInited;
	}

	// Token: 0x06000E0D RID: 3597 RVA: 0x00039054 File Offset: 0x00037254
	private void RefreshOnLevelInited()
	{
		int num = OptionsManager.Load<int>(this.Key, 1);
		this.Set(num);
	}

	// Token: 0x04000BB0 RID: 2992
	[LocalizationKey("Default")]
	public string onKey = "Options_On";

	// Token: 0x04000BB1 RID: 2993
	[LocalizationKey("Default")]
	public string offKey = "Options_Off";
}
