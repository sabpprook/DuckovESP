using System;

namespace Duckov
{
	// Token: 0x02000234 RID: 564
	[Serializable]
	public struct VersionData
	{
		// Token: 0x06001192 RID: 4498 RVA: 0x00043CF4 File Offset: 0x00041EF4
		public override string ToString()
		{
			return string.Format("{0}.{1}.{2}{3}", new object[] { this.mainVersion, this.subVersion, this.buildVersion, this.suffix });
		}

		// Token: 0x04000D93 RID: 3475
		public int mainVersion;

		// Token: 0x04000D94 RID: 3476
		public int subVersion;

		// Token: 0x04000D95 RID: 3477
		public int buildVersion;

		// Token: 0x04000D96 RID: 3478
		public string suffix;
	}
}
