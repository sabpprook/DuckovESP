using System;
using UnityEngine;

// Token: 0x02000136 RID: 310
public class LogOnEnableAndDisable : MonoBehaviour
{
	// Token: 0x06000A02 RID: 2562 RVA: 0x0002AE84 File Offset: 0x00029084
	private void OnEnable()
	{
		Debug.Log("OnEnable");
	}

	// Token: 0x06000A03 RID: 2563 RVA: 0x0002AE90 File Offset: 0x00029090
	private void OnDisable()
	{
		Debug.Log("OnDisable");
	}
}
