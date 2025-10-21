using System;
using System.Runtime.CompilerServices;

namespace YKF_ESP
{
	// Token: 0x02000014 RID: 20
	[NullableContext(1)]
	[Nullable(0)]
	public class MagicBulletRenderer
	{
		// Token: 0x060000AE RID: 174 RVA: 0x00005F10 File Offset: 0x00004110
		public MagicBulletRenderer(ESPSettings settings)
		{
			this.settings = settings;
		}

		// Token: 0x060000AF RID: 175 RVA: 0x00005F1F File Offset: 0x0000411F
		public void UpdateSettings(ESPSettings newSettings)
		{
			this.settings = newSettings;
		}

		// Token: 0x060000B0 RID: 176 RVA: 0x00005F28 File Offset: 0x00004128
		public void Dispose()
		{
		}

		// Token: 0x04000064 RID: 100
		private ESPSettings settings;
	}
}
