using System;
using Duckov.Options;
using SodaCraft.Localizations;

// Token: 0x020001CC RID: 460
public class DisableCameraOffset : OptionsProviderBase
{
	// Token: 0x17000284 RID: 644
	// (get) Token: 0x06000DA3 RID: 3491 RVA: 0x00037F71 File Offset: 0x00036171
	public override string Key
	{
		get
		{
			return "DisableCameraOffset";
		}
	}

	// Token: 0x06000DA4 RID: 3492 RVA: 0x00037F78 File Offset: 0x00036178
	public override string[] GetOptions()
	{
		return new string[]
		{
			this.onKey.ToPlainText(),
			this.offKey.ToPlainText()
		};
	}

	// Token: 0x06000DA5 RID: 3493 RVA: 0x00037F9C File Offset: 0x0003619C
	public override string GetCurrentOption()
	{
		int num = OptionsManager.Load<int>(this.Key, 1);
		if (num == 0)
		{
			return this.onKey.ToPlainText();
		}
		if (num != 1)
		{
			return this.offKey.ToPlainText();
		}
		return this.offKey.ToPlainText();
	}

	// Token: 0x06000DA6 RID: 3494 RVA: 0x00037FE2 File Offset: 0x000361E2
	public override void Set(int index)
	{
		if (index != 0)
		{
			if (index == 1)
			{
				DisableCameraOffset.disableCameraOffset = false;
			}
		}
		else
		{
			DisableCameraOffset.disableCameraOffset = true;
		}
		OptionsManager.Save<int>(this.Key, index);
	}

	// Token: 0x06000DA7 RID: 3495 RVA: 0x00038007 File Offset: 0x00036207
	private void Awake()
	{
		LevelManager.OnLevelInitialized += this.RefreshOnLevelInited;
	}

	// Token: 0x06000DA8 RID: 3496 RVA: 0x0003801A File Offset: 0x0003621A
	private void OnDestroy()
	{
		LevelManager.OnLevelInitialized -= this.RefreshOnLevelInited;
	}

	// Token: 0x06000DA9 RID: 3497 RVA: 0x00038030 File Offset: 0x00036230
	private void RefreshOnLevelInited()
	{
		int num = OptionsManager.Load<int>(this.Key, 1);
		this.Set(num);
	}

	// Token: 0x04000B8B RID: 2955
	[LocalizationKey("Default")]
	public string onKey = "Options_On";

	// Token: 0x04000B8C RID: 2956
	[LocalizationKey("Default")]
	public string offKey = "Options_Off";

	// Token: 0x04000B8D RID: 2957
	public static bool disableCameraOffset;
}
