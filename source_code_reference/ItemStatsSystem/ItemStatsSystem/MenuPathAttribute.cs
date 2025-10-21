using System;

namespace ItemStatsSystem
{
	// Token: 0x0200000B RID: 11
	public class MenuPathAttribute : Attribute
	{
		// Token: 0x0600003F RID: 63 RVA: 0x00002F2C File Offset: 0x0000112C
		public MenuPathAttribute(string path)
		{
			this.path = path;
		}

		// Token: 0x04000027 RID: 39
		public string path;
	}
}
