using System;
using Duckov.Options;

// Token: 0x020001C8 RID: 456
public class FullScreenOptions : OptionsProviderBase
{
	// Token: 0x17000281 RID: 641
	// (get) Token: 0x06000D85 RID: 3461 RVA: 0x000378A7 File Offset: 0x00035AA7
	public override string Key
	{
		get
		{
			return ResolutionSetter.Key_ScreenMode;
		}
	}

	// Token: 0x06000D86 RID: 3462 RVA: 0x000378AE File Offset: 0x00035AAE
	public override string GetCurrentOption()
	{
		return ResolutionSetter.ScreenModeToName(OptionsManager.Load<ResolutionSetter.screenModes>(this.Key, ResolutionSetter.screenModes.Borderless));
	}

	// Token: 0x06000D87 RID: 3463 RVA: 0x000378C1 File Offset: 0x00035AC1
	public override string[] GetOptions()
	{
		return ResolutionSetter.GetScreenModes();
	}

	// Token: 0x06000D88 RID: 3464 RVA: 0x000378C8 File Offset: 0x00035AC8
	public override void Set(int index)
	{
		OptionsManager.Save<ResolutionSetter.screenModes>(this.Key, (ResolutionSetter.screenModes)index);
	}
}
