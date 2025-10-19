using System;
using UnityEngine;

// Token: 0x0200017B RID: 379
public class HandheldSprite : MonoBehaviour
{
	// Token: 0x06000B7C RID: 2940 RVA: 0x00030AEB File Offset: 0x0002ECEB
	private void Start()
	{
		if (this.agent.Item)
		{
			this.spriteRenderer.sprite = this.agent.Item.Icon;
		}
	}

	// Token: 0x040009CC RID: 2508
	public DuckovItemAgent agent;

	// Token: 0x040009CD RID: 2509
	public SpriteRenderer spriteRenderer;
}
