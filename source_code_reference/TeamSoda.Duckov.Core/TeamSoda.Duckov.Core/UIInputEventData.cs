using System;
using UnityEngine;

// Token: 0x02000171 RID: 369
public class UIInputEventData
{
	// Token: 0x17000223 RID: 547
	// (get) Token: 0x06000B46 RID: 2886 RVA: 0x0002FDB7 File Offset: 0x0002DFB7
	public bool Used
	{
		get
		{
			return this.used;
		}
	}

	// Token: 0x06000B47 RID: 2887 RVA: 0x0002FDBF File Offset: 0x0002DFBF
	public void Use()
	{
		this.used = true;
	}

	// Token: 0x04000999 RID: 2457
	private bool used;

	// Token: 0x0400099A RID: 2458
	public Vector2 vector;

	// Token: 0x0400099B RID: 2459
	public bool confirm;

	// Token: 0x0400099C RID: 2460
	public bool cancel;
}
