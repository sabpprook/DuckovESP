using System;
using Duckov.Options;
using SodaCraft.Localizations;

// Token: 0x020001D4 RID: 468
public class RunInputOptions : OptionsProviderBase
{
	// Token: 0x1700028C RID: 652
	// (get) Token: 0x06000DE6 RID: 3558 RVA: 0x00038A8F File Offset: 0x00036C8F
	public override string Key
	{
		get
		{
			return "RunInputModeSettings";
		}
	}

	// Token: 0x06000DE7 RID: 3559 RVA: 0x00038A96 File Offset: 0x00036C96
	public override string[] GetOptions()
	{
		return new string[]
		{
			this.holdModeKey.ToPlainText(),
			this.switchModeKey.ToPlainText()
		};
	}

	// Token: 0x06000DE8 RID: 3560 RVA: 0x00038ABC File Offset: 0x00036CBC
	public override string GetCurrentOption()
	{
		int num = OptionsManager.Load<int>(this.Key, 0);
		if (num == 0)
		{
			return this.holdModeKey.ToPlainText();
		}
		if (num != 1)
		{
			return this.holdModeKey.ToPlainText();
		}
		return this.switchModeKey.ToPlainText();
	}

	// Token: 0x06000DE9 RID: 3561 RVA: 0x00038B02 File Offset: 0x00036D02
	public override void Set(int index)
	{
		if (index != 0)
		{
			if (index == 1)
			{
				InputManager.useRunInputBuffer = true;
			}
		}
		else
		{
			InputManager.useRunInputBuffer = false;
		}
		OptionsManager.Save<int>(this.Key, index);
	}

	// Token: 0x06000DEA RID: 3562 RVA: 0x00038B27 File Offset: 0x00036D27
	private void Awake()
	{
		LevelManager.OnLevelInitialized += this.RefreshOnLevelInited;
	}

	// Token: 0x06000DEB RID: 3563 RVA: 0x00038B3A File Offset: 0x00036D3A
	private void OnDestroy()
	{
		LevelManager.OnLevelInitialized -= this.RefreshOnLevelInited;
	}

	// Token: 0x06000DEC RID: 3564 RVA: 0x00038B50 File Offset: 0x00036D50
	private void RefreshOnLevelInited()
	{
		int num = OptionsManager.Load<int>(this.Key, 1);
		this.Set(num);
	}

	// Token: 0x04000BA4 RID: 2980
	[LocalizationKey("Default")]
	public string holdModeKey = "RunInputMode_Hold";

	// Token: 0x04000BA5 RID: 2981
	[LocalizationKey("Default")]
	public string switchModeKey = "RunInputMode_Switch";
}
