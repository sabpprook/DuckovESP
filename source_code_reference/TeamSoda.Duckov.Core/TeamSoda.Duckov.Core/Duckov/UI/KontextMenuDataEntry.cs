using System;
using UnityEngine;

namespace Duckov.UI
{
	// Token: 0x020003C4 RID: 964
	public class KontextMenuDataEntry
	{
		// Token: 0x06002319 RID: 8985 RVA: 0x0007AF5D File Offset: 0x0007915D
		public void Invoke()
		{
			Action action = this.action;
			if (action == null)
			{
				return;
			}
			action();
		}

		// Token: 0x040017E0 RID: 6112
		public Sprite icon;

		// Token: 0x040017E1 RID: 6113
		public string text;

		// Token: 0x040017E2 RID: 6114
		public Action action;
	}
}
