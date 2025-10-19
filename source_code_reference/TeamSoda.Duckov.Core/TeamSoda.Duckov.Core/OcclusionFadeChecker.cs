using System;
using UnityEngine;

// Token: 0x02000184 RID: 388
public class OcclusionFadeChecker : MonoBehaviour
{
	// Token: 0x06000B9D RID: 2973 RVA: 0x000313B0 File Offset: 0x0002F5B0
	private void OnTriggerEnter(Collider other)
	{
		OcclusionFadeTrigger component = other.GetComponent<OcclusionFadeTrigger>();
		if (!component)
		{
			return;
		}
		component.Enter();
	}

	// Token: 0x06000B9E RID: 2974 RVA: 0x000313D4 File Offset: 0x0002F5D4
	private void OnTriggerExit(Collider other)
	{
		OcclusionFadeTrigger component = other.GetComponent<OcclusionFadeTrigger>();
		if (!component)
		{
			return;
		}
		component.Leave();
	}
}
