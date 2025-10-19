using System;
using Duckov.Options;
using HorizonBasedAmbientOcclusion.Universal;
using SodaCraft.Localizations;
using UnityEngine.Rendering;

// Token: 0x020001D2 RID: 466
public class HBAOSettings : OptionsProviderBase
{
	// Token: 0x17000289 RID: 649
	// (get) Token: 0x06000DD5 RID: 3541 RVA: 0x00038798 File Offset: 0x00036998
	public override string Key
	{
		get
		{
			return "HBAOSettings";
		}
	}

	// Token: 0x06000DD6 RID: 3542 RVA: 0x0003879F File Offset: 0x0003699F
	public override string[] GetOptions()
	{
		return new string[]
		{
			this.offKey.ToPlainText(),
			this.lowKey.ToPlainText(),
			this.normalKey.ToPlainText(),
			this.highKey.ToPlainText()
		};
	}

	// Token: 0x06000DD7 RID: 3543 RVA: 0x000387E0 File Offset: 0x000369E0
	public override string GetCurrentOption()
	{
		switch (OptionsManager.Load<int>(this.Key, 2))
		{
		case 0:
			return this.offKey.ToPlainText();
		case 1:
			return this.lowKey.ToPlainText();
		case 2:
			return this.normalKey.ToPlainText();
		case 3:
			return this.highKey.ToPlainText();
		default:
			return this.offKey.ToPlainText();
		}
	}

	// Token: 0x06000DD8 RID: 3544 RVA: 0x00038850 File Offset: 0x00036A50
	public override void Set(int index)
	{
		HBAO hbao;
		if (this.GlobalVolumePorfile.TryGet<HBAO>(out hbao))
		{
			switch (index)
			{
			case 0:
				hbao.EnableHBAO(false);
				break;
			case 1:
				hbao.EnableHBAO(true);
				hbao.resolution = new HBAO.ResolutionParameter(HBAO.Resolution.Half, false);
				hbao.bias.value = 64f;
				break;
			case 2:
				hbao.EnableHBAO(true);
				hbao.resolution = new HBAO.ResolutionParameter(HBAO.Resolution.Half, false);
				hbao.bias.value = 128f;
				break;
			case 3:
				hbao.EnableHBAO(true);
				hbao.resolution = new HBAO.ResolutionParameter(HBAO.Resolution.Full, false);
				hbao.bias.value = 128f;
				break;
			}
		}
		OptionsManager.Save<int>(this.Key, index);
	}

	// Token: 0x06000DD9 RID: 3545 RVA: 0x0003890C File Offset: 0x00036B0C
	private void Awake()
	{
		LevelManager.OnLevelInitialized += this.RefreshOnLevelInited;
	}

	// Token: 0x06000DDA RID: 3546 RVA: 0x0003891F File Offset: 0x00036B1F
	private void OnDestroy()
	{
		LevelManager.OnLevelInitialized -= this.RefreshOnLevelInited;
	}

	// Token: 0x06000DDB RID: 3547 RVA: 0x00038934 File Offset: 0x00036B34
	private void RefreshOnLevelInited()
	{
		int num = OptionsManager.Load<int>(this.Key, 1);
		this.Set(num);
	}

	// Token: 0x04000B9C RID: 2972
	[LocalizationKey("Default")]
	public string offKey = "HBAOSettings_Off";

	// Token: 0x04000B9D RID: 2973
	[LocalizationKey("Default")]
	public string lowKey = "HBAOSettings_Low";

	// Token: 0x04000B9E RID: 2974
	[LocalizationKey("Default")]
	public string normalKey = "HBAOSettings_Normal";

	// Token: 0x04000B9F RID: 2975
	[LocalizationKey("Default")]
	public string highKey = "HBAOSettings_High";

	// Token: 0x04000BA0 RID: 2976
	public VolumeProfile GlobalVolumePorfile;
}
