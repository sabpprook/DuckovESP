using System;
using UnityEngine;

// Token: 0x02000062 RID: 98
public class HalfObsticleTrigger : MonoBehaviour
{
	// Token: 0x06000399 RID: 921 RVA: 0x0000FD95 File Offset: 0x0000DF95
	private void OnTriggerEnter(Collider other)
	{
		this.parent.OnTriggerEnter(other);
	}

	// Token: 0x0600039A RID: 922 RVA: 0x0000FDA3 File Offset: 0x0000DFA3
	private void OnTriggerExit(Collider other)
	{
		this.parent.OnTriggerExit(other);
	}

	// Token: 0x040002BE RID: 702
	public HalfObsticle parent;
}
