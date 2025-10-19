using System;
using Steamworks;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020001E6 RID: 486
public class EventIfSteamChina : MonoBehaviour
{
	// Token: 0x06000E5C RID: 3676 RVA: 0x00039D83 File Offset: 0x00037F83
	private void Start()
	{
		if (!SteamManager.Initialized)
		{
			return;
		}
		if (SteamUtils.IsSteamChinaLauncher())
		{
			this.onStart_IsSteamChina.Invoke();
			return;
		}
		this.onStart_IsNotSteamChina.Invoke();
	}

	// Token: 0x04000BE7 RID: 3047
	public UnityEvent onStart_IsSteamChina;

	// Token: 0x04000BE8 RID: 3048
	public UnityEvent onStart_IsNotSteamChina;
}
