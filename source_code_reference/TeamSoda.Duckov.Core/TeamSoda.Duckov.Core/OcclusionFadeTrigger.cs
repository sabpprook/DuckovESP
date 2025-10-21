using System;
using UnityEngine;

// Token: 0x02000188 RID: 392
public class OcclusionFadeTrigger : MonoBehaviour
{
	// Token: 0x06000BB6 RID: 2998 RVA: 0x00031C32 File Offset: 0x0002FE32
	private void Awake()
	{
		base.gameObject.layer = LayerMask.NameToLayer("VisualOcclusion");
	}

	// Token: 0x06000BB7 RID: 2999 RVA: 0x00031C49 File Offset: 0x0002FE49
	public void Enter()
	{
		this.parent.OnEnter();
	}

	// Token: 0x06000BB8 RID: 3000 RVA: 0x00031C56 File Offset: 0x0002FE56
	public void Leave()
	{
		this.parent.OnLeave();
	}

	// Token: 0x04000A12 RID: 2578
	public OcclusionFadeObject parent;
}
