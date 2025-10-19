using System;
using UnityEngine;

// Token: 0x0200017D RID: 381
public class MainCharacterFace : MonoBehaviour
{
	// Token: 0x06000B81 RID: 2945 RVA: 0x00030B30 File Offset: 0x0002ED30
	private void Start()
	{
		CustomFaceSettingData customFaceSettingData = this.customFaceManager.LoadMainCharacterSetting();
		this.customFace.LoadFromData(customFaceSettingData);
	}

	// Token: 0x06000B82 RID: 2946 RVA: 0x00030B55 File Offset: 0x0002ED55
	private void Update()
	{
	}

	// Token: 0x040009CE RID: 2510
	public CustomFaceManager customFaceManager;

	// Token: 0x040009CF RID: 2511
	public CustomFaceInstance customFace;
}
