using System;
using Steamworks;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001E7 RID: 487
public class SteamLogoImage : MonoBehaviour
{
	// Token: 0x06000E5E RID: 3678 RVA: 0x00039DB3 File Offset: 0x00037FB3
	private void Start()
	{
		this.Refresh();
	}

	// Token: 0x06000E5F RID: 3679 RVA: 0x00039DBC File Offset: 0x00037FBC
	private void Refresh()
	{
		if (!SteamManager.Initialized)
		{
			this.image.sprite = this.steamLogo;
			return;
		}
		if (SteamUtils.IsSteamChinaLauncher())
		{
			this.image.sprite = this.steamChinaLogo;
			return;
		}
		this.image.sprite = this.steamLogo;
	}

	// Token: 0x04000BE9 RID: 3049
	[SerializeField]
	private Image image;

	// Token: 0x04000BEA RID: 3050
	[SerializeField]
	private Sprite steamLogo;

	// Token: 0x04000BEB RID: 3051
	[SerializeField]
	private Sprite steamChinaLogo;
}
