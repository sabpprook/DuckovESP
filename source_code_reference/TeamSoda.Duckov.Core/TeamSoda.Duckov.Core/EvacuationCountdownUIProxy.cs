using System;
using UnityEngine;

// Token: 0x02000110 RID: 272
public class EvacuationCountdownUIProxy : MonoBehaviour
{
	// Token: 0x06000945 RID: 2373 RVA: 0x0002902F File Offset: 0x0002722F
	public void Request(CountDownArea target)
	{
		EvacuationCountdownUI.Request(target);
	}

	// Token: 0x06000946 RID: 2374 RVA: 0x00029037 File Offset: 0x00027237
	public void Release(CountDownArea target)
	{
		EvacuationCountdownUI.Release(target);
	}
}
