using System;
using UnityEngine;

namespace Duckov
{
	// Token: 0x0200022C RID: 556
	public struct Progress
	{
		// Token: 0x17000301 RID: 769
		// (get) Token: 0x06001144 RID: 4420 RVA: 0x000430C5 File Offset: 0x000412C5
		public float progress
		{
			get
			{
				if (this.total > 0f)
				{
					return Mathf.Clamp01(this.current / this.total);
				}
				return 1f;
			}
		}

		// Token: 0x04000D6C RID: 3436
		public bool inProgress;

		// Token: 0x04000D6D RID: 3437
		public float total;

		// Token: 0x04000D6E RID: 3438
		public float current;

		// Token: 0x04000D6F RID: 3439
		public string progressName;
	}
}
