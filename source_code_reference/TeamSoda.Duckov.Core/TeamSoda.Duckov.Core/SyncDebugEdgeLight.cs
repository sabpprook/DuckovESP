using System;
using Soda;
using UnityEngine;

// Token: 0x020000CD RID: 205
public class SyncDebugEdgeLight : MonoBehaviour
{
	// Token: 0x06000657 RID: 1623 RVA: 0x0001CC28 File Offset: 0x0001AE28
	private void Awake()
	{
		DebugView.OnDebugViewConfigChanged += this.OnDebugConfigChanged;
	}

	// Token: 0x06000658 RID: 1624 RVA: 0x0001CC3B File Offset: 0x0001AE3B
	private void OnDestroy()
	{
		DebugView.OnDebugViewConfigChanged -= this.OnDebugConfigChanged;
	}

	// Token: 0x06000659 RID: 1625 RVA: 0x0001CC4E File Offset: 0x0001AE4E
	private void OnDebugConfigChanged(DebugView debugView)
	{
		if (debugView == null)
		{
			return;
		}
		base.gameObject.SetActive(debugView.EdgeLightActive);
	}
}
