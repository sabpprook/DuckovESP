using System;
using Duckov.Options;
using SodaCraft.Localizations;

// Token: 0x020001D6 RID: 470
public class SoftShadowOptions : OptionsProviderBase
{
	// Token: 0x1700028E RID: 654
	// (get) Token: 0x06000DF7 RID: 3575 RVA: 0x00038DE0 File Offset: 0x00036FE0
	public override string Key
	{
		get
		{
			return "SoftShadowSettings";
		}
	}

	// Token: 0x06000DF8 RID: 3576 RVA: 0x00038DE7 File Offset: 0x00036FE7
	public override string[] GetOptions()
	{
		return new string[]
		{
			this.offKey.ToPlainText(),
			this.onKey.ToPlainText()
		};
	}

	// Token: 0x06000DF9 RID: 3577 RVA: 0x00038E0C File Offset: 0x0003700C
	public override string GetCurrentOption()
	{
		int num = OptionsManager.Load<int>(this.Key, 1);
		if (num == 0)
		{
			return this.offKey.ToPlainText();
		}
		if (num != 1)
		{
			return this.offKey.ToPlainText();
		}
		return this.onKey.ToPlainText();
	}

	// Token: 0x06000DFA RID: 3578 RVA: 0x00038E52 File Offset: 0x00037052
	private void Awake()
	{
		LevelManager.OnLevelInitialized += this.RefreshOnLevelInited;
	}

	// Token: 0x06000DFB RID: 3579 RVA: 0x00038E65 File Offset: 0x00037065
	private void OnDestroy()
	{
		LevelManager.OnLevelInitialized -= this.RefreshOnLevelInited;
	}

	// Token: 0x06000DFC RID: 3580 RVA: 0x00038E78 File Offset: 0x00037078
	private void RefreshOnLevelInited()
	{
		int num = OptionsManager.Load<int>(this.Key, 1);
		this.Set(num);
	}

	// Token: 0x06000DFD RID: 3581 RVA: 0x00038E99 File Offset: 0x00037099
	public override void Set(int index)
	{
	}

	// Token: 0x04000BAC RID: 2988
	[LocalizationKey("Default")]
	public string offKey = "SoftShadowOptions_Off";

	// Token: 0x04000BAD RID: 2989
	[LocalizationKey("Default")]
	public string onKey = "SoftShadowOptions_On";
}
