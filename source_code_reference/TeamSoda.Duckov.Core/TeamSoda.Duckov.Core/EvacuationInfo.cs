using System;
using UnityEngine;

// Token: 0x02000107 RID: 263
[Serializable]
public struct EvacuationInfo
{
	// Token: 0x0600090F RID: 2319 RVA: 0x000284C6 File Offset: 0x000266C6
	public EvacuationInfo(string subsceneID, Vector3 position)
	{
		this.subsceneID = subsceneID;
		this.position = position;
	}

	// Token: 0x04000827 RID: 2087
	public string subsceneID;

	// Token: 0x04000828 RID: 2088
	public Vector3 position;
}
