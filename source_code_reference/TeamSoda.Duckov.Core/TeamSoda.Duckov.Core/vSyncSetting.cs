using System;
using Duckov.Options;
using SodaCraft.Localizations;
using UnityEngine;

// Token: 0x020001DA RID: 474
public class vSyncSetting : OptionsProviderBase
{
	// Token: 0x17000294 RID: 660
	// (get) Token: 0x06000E19 RID: 3609 RVA: 0x00039123 File Offset: 0x00037323
	public override string Key
	{
		get
		{
			return "GSyncSetting";
		}
	}

	// Token: 0x06000E1A RID: 3610 RVA: 0x0003912A File Offset: 0x0003732A
	public override string[] GetOptions()
	{
		return new string[]
		{
			this.onKey.ToPlainText(),
			this.offKey.ToPlainText()
		};
	}

	// Token: 0x06000E1B RID: 3611 RVA: 0x00039150 File Offset: 0x00037350
	public override string GetCurrentOption()
	{
		int num = OptionsManager.Load<int>(this.Key, 1);
		if (num == 0)
		{
			this.SyncObjectActive(true);
			return this.onKey.ToPlainText();
		}
		if (num != 1)
		{
			return this.offKey.ToPlainText();
		}
		this.SyncObjectActive(false);
		return this.offKey.ToPlainText();
	}

	// Token: 0x06000E1C RID: 3612 RVA: 0x000391A4 File Offset: 0x000373A4
	public override void Set(int index)
	{
		if (index != 0)
		{
			if (index == 1)
			{
				QualitySettings.vSyncCount = 0;
				this.SyncObjectActive(false);
			}
		}
		else
		{
			QualitySettings.vSyncCount = 1;
			this.SyncObjectActive(true);
		}
		OptionsManager.Save<int>(this.Key, index);
	}

	// Token: 0x06000E1D RID: 3613 RVA: 0x000391D7 File Offset: 0x000373D7
	private void SyncObjectActive(bool active)
	{
		if (this.setActiveIfOn)
		{
			this.setActiveIfOn.SetActive(active);
		}
	}

	// Token: 0x06000E1E RID: 3614 RVA: 0x000391F2 File Offset: 0x000373F2
	private void Awake()
	{
		LevelManager.OnLevelInitialized += this.RefreshOnLevelInited;
	}

	// Token: 0x06000E1F RID: 3615 RVA: 0x00039205 File Offset: 0x00037405
	private void OnDestroy()
	{
		LevelManager.OnLevelInitialized -= this.RefreshOnLevelInited;
	}

	// Token: 0x06000E20 RID: 3616 RVA: 0x00039218 File Offset: 0x00037418
	private void RefreshOnLevelInited()
	{
		int num = OptionsManager.Load<int>(this.Key, 1);
		this.Set(num);
	}

	// Token: 0x04000BB3 RID: 2995
	[LocalizationKey("Default")]
	public string onKey = "gSync_On";

	// Token: 0x04000BB4 RID: 2996
	[LocalizationKey("Default")]
	public string offKey = "gSync_Off";

	// Token: 0x04000BB5 RID: 2997
	public GameObject setActiveIfOn;
}
