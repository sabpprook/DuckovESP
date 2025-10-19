using System;
using Duckov;
using UnityEngine;

// Token: 0x0200019C RID: 412
public class FmodEventTester : MonoBehaviour
{
	// Token: 0x06000C31 RID: 3121 RVA: 0x00033777 File Offset: 0x00031977
	public void PlayEvent()
	{
		AudioManager.Post(this.e, base.gameObject);
	}

	// Token: 0x04000A92 RID: 2706
	[SerializeField]
	private string e;
}
