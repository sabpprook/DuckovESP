using System;
using Duckov.Options;
using SodaCraft.Localizations;
using SymmetryBreakStudio.TastyGrassShader;
using UnityEngine.Rendering.Universal;

// Token: 0x020001D1 RID: 465
public class GrassOptions : OptionsProviderBase
{
	// Token: 0x17000288 RID: 648
	// (get) Token: 0x06000DCD RID: 3533 RVA: 0x00038640 File Offset: 0x00036840
	public override string Key
	{
		get
		{
			return "GrassSettings";
		}
	}

	// Token: 0x06000DCE RID: 3534 RVA: 0x00038647 File Offset: 0x00036847
	public override string[] GetOptions()
	{
		return new string[]
		{
			this.offKey.ToPlainText(),
			this.onKey.ToPlainText()
		};
	}

	// Token: 0x06000DCF RID: 3535 RVA: 0x0003866C File Offset: 0x0003686C
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

	// Token: 0x06000DD0 RID: 3536 RVA: 0x000386B2 File Offset: 0x000368B2
	private void Awake()
	{
		LevelManager.OnLevelInitialized += this.RefreshOnLevelInited;
	}

	// Token: 0x06000DD1 RID: 3537 RVA: 0x000386C5 File Offset: 0x000368C5
	private void OnDestroy()
	{
		LevelManager.OnLevelInitialized -= this.RefreshOnLevelInited;
	}

	// Token: 0x06000DD2 RID: 3538 RVA: 0x000386D8 File Offset: 0x000368D8
	private void RefreshOnLevelInited()
	{
		int num = OptionsManager.Load<int>(this.Key, 1);
		this.Set(num);
	}

	// Token: 0x06000DD3 RID: 3539 RVA: 0x000386FC File Offset: 0x000368FC
	public override void Set(int index)
	{
		ScriptableRendererFeature scriptableRendererFeature = this.rendererData.rendererFeatures.Find((ScriptableRendererFeature e) => e is TastyGrassShaderGlobalSettings);
		if (scriptableRendererFeature != null)
		{
			TastyGrassShaderGlobalSettings tastyGrassShaderGlobalSettings = scriptableRendererFeature as TastyGrassShaderGlobalSettings;
			if (index != 0)
			{
				if (index == 1)
				{
					tastyGrassShaderGlobalSettings.SetActive(true);
					TgsManager.Enable = true;
				}
			}
			else
			{
				tastyGrassShaderGlobalSettings.SetActive(false);
				TgsManager.Enable = false;
			}
		}
		OptionsManager.Save<int>(this.Key, index);
	}

	// Token: 0x04000B99 RID: 2969
	[LocalizationKey("Default")]
	public string offKey = "GrassOptions_Off";

	// Token: 0x04000B9A RID: 2970
	[LocalizationKey("Default")]
	public string onKey = "GrassOptions_On";

	// Token: 0x04000B9B RID: 2971
	public UniversalRendererData rendererData;
}
