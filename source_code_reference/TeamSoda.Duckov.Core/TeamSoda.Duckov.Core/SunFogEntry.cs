using System;
using UnityEngine;

// Token: 0x020001D7 RID: 471
public class SunFogEntry : MonoBehaviour
{
	// Token: 0x14000065 RID: 101
	// (add) Token: 0x06000DFF RID: 3583 RVA: 0x00038EBC File Offset: 0x000370BC
	// (remove) Token: 0x06000E00 RID: 3584 RVA: 0x00038EF0 File Offset: 0x000370F0
	private static event Action OnSettingChangedEvent;

	// Token: 0x06000E01 RID: 3585 RVA: 0x00038F23 File Offset: 0x00037123
	public static void SetEnabled(bool enabled)
	{
		SunFogEntry.settingEnabled = enabled;
		Action onSettingChangedEvent = SunFogEntry.OnSettingChangedEvent;
		if (onSettingChangedEvent == null)
		{
			return;
		}
		onSettingChangedEvent();
	}

	// Token: 0x06000E02 RID: 3586 RVA: 0x00038F3A File Offset: 0x0003713A
	private void Awake()
	{
		SunFogEntry.OnSettingChangedEvent += this.OnSettingChanged;
		base.gameObject.SetActive(SunFogEntry.settingEnabled);
	}

	// Token: 0x06000E03 RID: 3587 RVA: 0x00038F5D File Offset: 0x0003715D
	private void OnDestroy()
	{
		SunFogEntry.OnSettingChangedEvent -= this.OnSettingChanged;
	}

	// Token: 0x06000E04 RID: 3588 RVA: 0x00038F70 File Offset: 0x00037170
	private void OnSettingChanged()
	{
		base.gameObject.SetActive(SunFogEntry.settingEnabled);
	}

	// Token: 0x04000BAE RID: 2990
	private static bool settingEnabled = true;
}
