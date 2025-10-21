using System;
using Duckov.Options;
using UnityEngine;

// Token: 0x020001C9 RID: 457
public class ResolutionOptions : OptionsProviderBase
{
	// Token: 0x17000282 RID: 642
	// (get) Token: 0x06000D8A RID: 3466 RVA: 0x000378DE File Offset: 0x00035ADE
	public override string Key
	{
		get
		{
			return ResolutionSetter.Key_Resolution;
		}
	}

	// Token: 0x06000D8B RID: 3467 RVA: 0x000378E8 File Offset: 0x00035AE8
	public override string GetCurrentOption()
	{
		return OptionsManager.Load<DuckovResolution>(this.Key, new DuckovResolution(Screen.resolutions[Screen.resolutions.Length - 1])).ToString();
	}

	// Token: 0x06000D8C RID: 3468 RVA: 0x00037928 File Offset: 0x00035B28
	public override string[] GetOptions()
	{
		this.avaliableResolutions = ResolutionSetter.GetResolutions();
		string[] array = new string[this.avaliableResolutions.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = this.avaliableResolutions[i].ToString();
		}
		return array;
	}

	// Token: 0x06000D8D RID: 3469 RVA: 0x00037978 File Offset: 0x00035B78
	public override void Set(int index)
	{
		if (this.avaliableResolutions == null || index >= this.avaliableResolutions.Length)
		{
			Debug.Log("设置分辨率流程错误");
			index = 0;
		}
		DuckovResolution duckovResolution = this.avaliableResolutions[index];
		OptionsManager.Save<DuckovResolution>(this.Key, duckovResolution);
	}

	// Token: 0x04000B7F RID: 2943
	private DuckovResolution[] avaliableResolutions;
}
