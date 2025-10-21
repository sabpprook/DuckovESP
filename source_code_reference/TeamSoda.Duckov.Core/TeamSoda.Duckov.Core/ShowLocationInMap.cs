using System;
using UnityEngine;

// Token: 0x0200012B RID: 299
public class ShowLocationInMap : MonoBehaviour
{
	// Token: 0x17000202 RID: 514
	// (get) Token: 0x060009CD RID: 2509 RVA: 0x0002A16E File Offset: 0x0002836E
	public string DisplayName
	{
		get
		{
			return this.displayName;
		}
	}

	// Token: 0x17000203 RID: 515
	// (get) Token: 0x060009CE RID: 2510 RVA: 0x0002A176 File Offset: 0x00028376
	public string DisplayNameRaw
	{
		get
		{
			return this.displayName;
		}
	}

	// Token: 0x0400088B RID: 2187
	[SerializeField]
	private string displayName;
}
