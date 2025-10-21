using System;
using UnityEngine;

namespace Duckov
{
	// Token: 0x0200023E RID: 574
	public class StrongNotificationContent
	{
		// Token: 0x060011DE RID: 4574 RVA: 0x0004462C File Offset: 0x0004282C
		public StrongNotificationContent(string mainText, string subText = "", Sprite image = null)
		{
			this.mainText = mainText;
			this.subText = subText;
			this.image = image;
		}

		// Token: 0x04000DC0 RID: 3520
		public string mainText;

		// Token: 0x04000DC1 RID: 3521
		public string subText;

		// Token: 0x04000DC2 RID: 3522
		public Sprite image;
	}
}
