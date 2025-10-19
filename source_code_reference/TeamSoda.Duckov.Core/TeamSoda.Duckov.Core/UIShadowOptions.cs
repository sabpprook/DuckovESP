using System;
using Duckov.Options;
using LeTai.TrueShadow;
using SodaCraft.Localizations;

// Token: 0x020001D9 RID: 473
public class UIShadowOptions : OptionsProviderBase
{
	// Token: 0x17000290 RID: 656
	// (get) Token: 0x06000E0F RID: 3599 RVA: 0x00039093 File Offset: 0x00037293
	public override string Key
	{
		get
		{
			return "UIShadow";
		}
	}

	// Token: 0x17000291 RID: 657
	// (get) Token: 0x06000E10 RID: 3600 RVA: 0x0003909A File Offset: 0x0003729A
	// (set) Token: 0x06000E11 RID: 3601 RVA: 0x000390A7 File Offset: 0x000372A7
	public static bool Active
	{
		get
		{
			return OptionsManager.Load<bool>("UIShadow", true);
		}
		set
		{
			OptionsManager.Save<bool>("UIShadow", value);
		}
	}

	// Token: 0x06000E12 RID: 3602 RVA: 0x000390B4 File Offset: 0x000372B4
	public static void Apply()
	{
		TrueShadow.ExternalActive = UIShadowOptions.Active;
	}

	// Token: 0x17000292 RID: 658
	// (get) Token: 0x06000E13 RID: 3603 RVA: 0x000390C0 File Offset: 0x000372C0
	public string ActiveText
	{
		get
		{
			return "Options_On".ToPlainText();
		}
	}

	// Token: 0x17000293 RID: 659
	// (get) Token: 0x06000E14 RID: 3604 RVA: 0x000390CC File Offset: 0x000372CC
	public string InactiveText
	{
		get
		{
			return "Options_Off".ToPlainText();
		}
	}

	// Token: 0x06000E15 RID: 3605 RVA: 0x000390D8 File Offset: 0x000372D8
	public override string GetCurrentOption()
	{
		if (UIShadowOptions.Active)
		{
			return this.ActiveText;
		}
		return this.InactiveText;
	}

	// Token: 0x06000E16 RID: 3606 RVA: 0x000390EE File Offset: 0x000372EE
	public override string[] GetOptions()
	{
		return new string[] { this.InactiveText, this.ActiveText };
	}

	// Token: 0x06000E17 RID: 3607 RVA: 0x00039108 File Offset: 0x00037308
	public override void Set(int index)
	{
		if (index <= 0)
		{
			UIShadowOptions.Active = false;
			return;
		}
		UIShadowOptions.Active = true;
	}

	// Token: 0x04000BB2 RID: 2994
	private const string key = "UIShadow";
}
