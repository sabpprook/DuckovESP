using System;
using UnityEngine;

// Token: 0x0200009C RID: 156
public class TimeScaleManager : MonoBehaviour
{
	// Token: 0x0600053D RID: 1341 RVA: 0x00017769 File Offset: 0x00015969
	private void Awake()
	{
	}

	// Token: 0x0600053E RID: 1342 RVA: 0x0001776C File Offset: 0x0001596C
	private void Update()
	{
		float num = 1f;
		if (GameManager.Paused)
		{
			num = 0f;
		}
		if (CameraMode.Active)
		{
			num = 0f;
		}
		Time.timeScale = num;
		Time.fixedDeltaTime = Mathf.Max(0.0005f, Time.timeScale * 0.02f);
	}
}
