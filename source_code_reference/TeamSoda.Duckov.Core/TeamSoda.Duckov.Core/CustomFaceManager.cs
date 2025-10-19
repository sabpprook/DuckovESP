using System;
using Duckov.Utilities;
using Saves;
using UnityEngine;

// Token: 0x020000FF RID: 255
public class CustomFaceManager : MonoBehaviour
{
	// Token: 0x0600086D RID: 2157 RVA: 0x00025875 File Offset: 0x00023A75
	public void SaveSettingToMainCharacter(CustomFaceSettingData setting)
	{
		this.SaveSetting("CustomFace_MainCharacter", setting);
	}

	// Token: 0x0600086E RID: 2158 RVA: 0x00025883 File Offset: 0x00023A83
	public CustomFaceSettingData LoadMainCharacterSetting()
	{
		return this.LoadSetting("CustomFace_MainCharacter");
	}

	// Token: 0x0600086F RID: 2159 RVA: 0x00025890 File Offset: 0x00023A90
	private void SaveSetting(string key, CustomFaceSettingData setting)
	{
		setting.savedSetting = true;
		SavesSystem.Save<CustomFaceSettingData>(key, setting);
	}

	// Token: 0x06000870 RID: 2160 RVA: 0x000258A4 File Offset: 0x00023AA4
	private CustomFaceSettingData LoadSetting(string key)
	{
		CustomFaceSettingData customFaceSettingData = SavesSystem.Load<CustomFaceSettingData>(key);
		if (!customFaceSettingData.savedSetting)
		{
			customFaceSettingData = GameplayDataSettings.CustomFaceData.DefaultPreset.settings;
		}
		return customFaceSettingData;
	}
}
