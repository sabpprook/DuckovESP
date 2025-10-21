using System;
using Duckov.Options;
using SodaCraft.Localizations;
using UnityEngine;

// Token: 0x020001CF RID: 463
public class FrameRateSetting : OptionsProviderBase
{
	// Token: 0x17000286 RID: 646
	// (get) Token: 0x06000DBB RID: 3515 RVA: 0x00038247 File Offset: 0x00036447
	public override string Key
	{
		get
		{
			return "FrameRateSetting";
		}
	}

	// Token: 0x06000DBC RID: 3516 RVA: 0x0003824E File Offset: 0x0003644E
	public override string[] GetOptions()
	{
		return new string[]
		{
			"60",
			"90",
			"120",
			"144",
			"240",
			this.optionUnlimitKey.ToPlainText()
		};
	}

	// Token: 0x06000DBD RID: 3517 RVA: 0x0003828C File Offset: 0x0003648C
	public override string GetCurrentOption()
	{
		switch (OptionsManager.Load<int>(this.Key, 0))
		{
		case 0:
			return "60";
		case 1:
			return "90";
		case 2:
			return "120";
		case 3:
			return "144";
		case 4:
			return "240";
		case 5:
			return this.optionUnlimitKey.ToPlainText();
		default:
			return "60";
		}
	}

	// Token: 0x06000DBE RID: 3518 RVA: 0x000382F8 File Offset: 0x000364F8
	public override void Set(int index)
	{
		switch (index)
		{
		case 0:
			Application.targetFrameRate = 60;
			break;
		case 1:
			Application.targetFrameRate = 90;
			break;
		case 2:
			Application.targetFrameRate = 120;
			break;
		case 3:
			Application.targetFrameRate = 144;
			break;
		case 4:
			Application.targetFrameRate = 240;
			break;
		case 5:
			Application.targetFrameRate = 500;
			break;
		}
		OptionsManager.Save<int>(this.Key, index);
	}

	// Token: 0x06000DBF RID: 3519 RVA: 0x0003836E File Offset: 0x0003656E
	private void Awake()
	{
		LevelManager.OnLevelInitialized += this.RefreshOnLevelInited;
	}

	// Token: 0x06000DC0 RID: 3520 RVA: 0x00038381 File Offset: 0x00036581
	private void OnDestroy()
	{
		LevelManager.OnLevelInitialized -= this.RefreshOnLevelInited;
	}

	// Token: 0x06000DC1 RID: 3521 RVA: 0x00038394 File Offset: 0x00036594
	private void RefreshOnLevelInited()
	{
		int num = OptionsManager.Load<int>(this.Key, 0);
		this.Set(num);
	}

	// Token: 0x04000B92 RID: 2962
	[LocalizationKey("Default")]
	public string optionUnlimitKey = "FrameRateUnlimit";
}
