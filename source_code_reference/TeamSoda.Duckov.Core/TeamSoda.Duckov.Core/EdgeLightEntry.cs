using System;
using UnityEngine;

// Token: 0x020001CD RID: 461
public class EdgeLightEntry : MonoBehaviour
{
	// Token: 0x14000064 RID: 100
	// (add) Token: 0x06000DAB RID: 3499 RVA: 0x00038070 File Offset: 0x00036270
	// (remove) Token: 0x06000DAC RID: 3500 RVA: 0x000380A4 File Offset: 0x000362A4
	private static event Action OnSettingChangedEvent;

	// Token: 0x06000DAD RID: 3501 RVA: 0x000380D7 File Offset: 0x000362D7
	public static void SetEnabled(bool enabled)
	{
		EdgeLightEntry.settingEnabled = enabled;
		Action onSettingChangedEvent = EdgeLightEntry.OnSettingChangedEvent;
		if (onSettingChangedEvent == null)
		{
			return;
		}
		onSettingChangedEvent();
	}

	// Token: 0x06000DAE RID: 3502 RVA: 0x000380EE File Offset: 0x000362EE
	private void Awake()
	{
		EdgeLightEntry.OnSettingChangedEvent += this.OnSettingChanged;
		base.gameObject.SetActive(EdgeLightEntry.settingEnabled);
	}

	// Token: 0x06000DAF RID: 3503 RVA: 0x00038111 File Offset: 0x00036311
	private void OnDestroy()
	{
		EdgeLightEntry.OnSettingChangedEvent -= this.OnSettingChanged;
	}

	// Token: 0x06000DB0 RID: 3504 RVA: 0x00038124 File Offset: 0x00036324
	private void OnSettingChanged()
	{
		base.gameObject.SetActive(EdgeLightEntry.settingEnabled);
	}

	// Token: 0x04000B8E RID: 2958
	private static bool settingEnabled = true;
}
