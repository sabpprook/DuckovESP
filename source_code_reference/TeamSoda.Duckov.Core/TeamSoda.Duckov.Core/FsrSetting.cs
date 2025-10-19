using System;
using Duckov.MiniGames;
using Duckov.Options;
using SodaCraft.Localizations;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

// Token: 0x020001D0 RID: 464
public class FsrSetting : OptionsProviderBase
{
	// Token: 0x17000287 RID: 647
	// (get) Token: 0x06000DC3 RID: 3523 RVA: 0x000383C8 File Offset: 0x000365C8
	public override string Key
	{
		get
		{
			return "FsrSetting";
		}
	}

	// Token: 0x06000DC4 RID: 3524 RVA: 0x000383D0 File Offset: 0x000365D0
	public override string[] GetOptions()
	{
		return new string[]
		{
			this.offKey.ToPlainText(),
			this.qualityKey.ToPlainText(),
			this.balancedKey.ToPlainText(),
			this.performanceKey.ToPlainText(),
			this.ultraPerformanceKey.ToPlainText()
		};
	}

	// Token: 0x06000DC5 RID: 3525 RVA: 0x0003842C File Offset: 0x0003662C
	public override string GetCurrentOption()
	{
		switch (OptionsManager.Load<int>(this.Key, 0))
		{
		case 0:
			return this.offKey.ToPlainText();
		case 1:
			return this.qualityKey.ToPlainText();
		case 2:
			return this.balancedKey.ToPlainText();
		case 3:
			return this.performanceKey.ToPlainText();
		case 4:
			return this.ultraPerformanceKey.ToPlainText();
		default:
			return this.offKey.ToPlainText();
		}
	}

	// Token: 0x06000DC6 RID: 3526 RVA: 0x000384AC File Offset: 0x000366AC
	public override void Set(int index)
	{
		UniversalRenderPipelineAsset universalRenderPipelineAsset = (UniversalRenderPipelineAsset)GraphicsSettings.currentRenderPipeline;
		int num = index;
		if (FsrSetting.gameOn)
		{
			num = 0;
		}
		switch (num)
		{
		case 0:
			if (universalRenderPipelineAsset != null)
			{
				universalRenderPipelineAsset.renderScale = 1f;
				universalRenderPipelineAsset.upscalingFilter = UpscalingFilterSelection.Linear;
			}
			break;
		case 1:
			if (universalRenderPipelineAsset != null)
			{
				universalRenderPipelineAsset.renderScale = 0.67f;
				universalRenderPipelineAsset.upscalingFilter = UpscalingFilterSelection.FSR;
			}
			break;
		case 2:
			if (universalRenderPipelineAsset != null)
			{
				universalRenderPipelineAsset.renderScale = 0.58f;
				universalRenderPipelineAsset.upscalingFilter = UpscalingFilterSelection.FSR;
			}
			break;
		case 3:
			if (universalRenderPipelineAsset != null)
			{
				universalRenderPipelineAsset.renderScale = 0.5f;
				universalRenderPipelineAsset.upscalingFilter = UpscalingFilterSelection.FSR;
			}
			break;
		case 4:
			if (universalRenderPipelineAsset != null)
			{
				universalRenderPipelineAsset.renderScale = 0.33f;
				universalRenderPipelineAsset.upscalingFilter = UpscalingFilterSelection.FSR;
			}
			break;
		}
		OptionsManager.Save<int>(this.Key, index);
	}

	// Token: 0x06000DC7 RID: 3527 RVA: 0x0003858C File Offset: 0x0003678C
	private void Awake()
	{
		this.RefreshOnLevelInited();
		LevelManager.OnLevelInitialized += this.RefreshOnLevelInited;
		GamingConsole.OnGamingConsoleInteractChanged += this.OnGamingConsoleInteractChanged;
	}

	// Token: 0x06000DC8 RID: 3528 RVA: 0x000385B6 File Offset: 0x000367B6
	private void OnGamingConsoleInteractChanged(bool _gameOn)
	{
		FsrSetting.gameOn = _gameOn;
		this.SyncSetting();
	}

	// Token: 0x06000DC9 RID: 3529 RVA: 0x000385C4 File Offset: 0x000367C4
	private void OnDestroy()
	{
		LevelManager.OnLevelInitialized -= this.RefreshOnLevelInited;
	}

	// Token: 0x06000DCA RID: 3530 RVA: 0x000385D8 File Offset: 0x000367D8
	private void SyncSetting()
	{
		int num = OptionsManager.Load<int>(this.Key, 0);
		this.Set(num);
	}

	// Token: 0x06000DCB RID: 3531 RVA: 0x000385F9 File Offset: 0x000367F9
	private void RefreshOnLevelInited()
	{
		this.SyncSetting();
	}

	// Token: 0x04000B93 RID: 2963
	[LocalizationKey("Default")]
	public string offKey = "fsr_Off";

	// Token: 0x04000B94 RID: 2964
	[LocalizationKey("Default")]
	public string qualityKey = "fsr_Quality";

	// Token: 0x04000B95 RID: 2965
	[LocalizationKey("Default")]
	public string balancedKey = "fsr_Balanced";

	// Token: 0x04000B96 RID: 2966
	[LocalizationKey("Default")]
	public string performanceKey = "fsr_Performance";

	// Token: 0x04000B97 RID: 2967
	[LocalizationKey("Default")]
	public string ultraPerformanceKey = "fsr_UltraPerformance";

	// Token: 0x04000B98 RID: 2968
	private static bool gameOn;
}
