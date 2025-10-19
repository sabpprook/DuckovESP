using System;
using Duckov.Options;
using SodaCraft.Localizations;
using Umbra;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

// Token: 0x020001D5 RID: 469
public class ShadowSetting : OptionsProviderBase
{
	// Token: 0x1700028D RID: 653
	// (get) Token: 0x06000DEE RID: 3566 RVA: 0x00038B8F File Offset: 0x00036D8F
	public override string Key
	{
		get
		{
			return "ShadowSettings";
		}
	}

	// Token: 0x06000DEF RID: 3567 RVA: 0x00038B96 File Offset: 0x00036D96
	public override string[] GetOptions()
	{
		return new string[]
		{
			this.offKey.ToPlainText(),
			this.lowKey.ToPlainText(),
			this.middleKey.ToPlainText(),
			this.highKey.ToPlainText()
		};
	}

	// Token: 0x06000DF0 RID: 3568 RVA: 0x00038BD8 File Offset: 0x00036DD8
	public override string GetCurrentOption()
	{
		switch (OptionsManager.Load<int>(this.Key, 2))
		{
		case 0:
			return this.offKey.ToPlainText();
		case 1:
			return this.lowKey.ToPlainText();
		case 2:
			return this.middleKey.ToPlainText();
		case 3:
			return this.highKey.ToPlainText();
		default:
			return this.highKey.ToPlainText();
		}
	}

	// Token: 0x06000DF1 RID: 3569 RVA: 0x00038C48 File Offset: 0x00036E48
	private void SetShadow(bool on, int res, float shadowDistance, bool softShadow, bool softShadowDownSample, bool contactShadow, int pointLightCount)
	{
		UniversalRenderPipelineAsset universalRenderPipelineAsset = (UniversalRenderPipelineAsset)GraphicsSettings.currentRenderPipeline;
		if (universalRenderPipelineAsset != null)
		{
			universalRenderPipelineAsset.shadowDistance = (on ? shadowDistance : 0f);
			universalRenderPipelineAsset.mainLightShadowmapResolution = res;
			universalRenderPipelineAsset.additionalLightsShadowmapResolution = res;
			universalRenderPipelineAsset.maxAdditionalLightsCount = pointLightCount;
		}
		if (this.umbraProfile)
		{
			this.umbraProfile.shadowSource = (softShadow ? ShadowSource.UmbraShadows : ShadowSource.UnityShadows);
			this.umbraProfile.downsample = softShadowDownSample;
			this.umbraProfile.contactShadows = contactShadow;
		}
	}

	// Token: 0x06000DF2 RID: 3570 RVA: 0x00038CCC File Offset: 0x00036ECC
	public override void Set(int index)
	{
		switch (index)
		{
		case 0:
			this.SetShadow(false, 512, 0f, false, false, false, 0);
			break;
		case 1:
			this.SetShadow(true, 1024, 70f, false, false, false, 0);
			break;
		case 2:
			this.SetShadow(true, 2048, 80f, true, true, true, 5);
			break;
		case 3:
			this.SetShadow(true, 4096, 90f, true, false, true, 6);
			break;
		}
		OptionsManager.Save<int>(this.Key, index);
	}

	// Token: 0x06000DF3 RID: 3571 RVA: 0x00038D57 File Offset: 0x00036F57
	private void Awake()
	{
		LevelManager.OnLevelInitialized += this.RefreshOnLevelInited;
	}

	// Token: 0x06000DF4 RID: 3572 RVA: 0x00038D6A File Offset: 0x00036F6A
	private void OnDestroy()
	{
		LevelManager.OnLevelInitialized -= this.RefreshOnLevelInited;
	}

	// Token: 0x06000DF5 RID: 3573 RVA: 0x00038D80 File Offset: 0x00036F80
	private void RefreshOnLevelInited()
	{
		int num = OptionsManager.Load<int>(this.Key, 2);
		this.Set(num);
	}

	// Token: 0x04000BA6 RID: 2982
	public UmbraProfile umbraProfile;

	// Token: 0x04000BA7 RID: 2983
	public float onDistance = 100f;

	// Token: 0x04000BA8 RID: 2984
	[LocalizationKey("Default")]
	public string highKey = "Options_High";

	// Token: 0x04000BA9 RID: 2985
	[LocalizationKey("Default")]
	public string middleKey = "Options_Middle";

	// Token: 0x04000BAA RID: 2986
	[LocalizationKey("Default")]
	public string lowKey = "Options_Low";

	// Token: 0x04000BAB RID: 2987
	[LocalizationKey("Default")]
	public string offKey = "Options_Off";
}
