using System;

namespace ItemStatsSystem
{
	// Token: 0x02000013 RID: 19
	public struct EffectTriggerEventContext
	{
		// Token: 0x060000A1 RID: 161 RVA: 0x00003D77 File Offset: 0x00001F77
		public EffectTriggerEventContext(EffectTrigger source, bool positive)
		{
			this.source = source;
			this.positive = positive;
		}

		// Token: 0x04000038 RID: 56
		public EffectTrigger source;

		// Token: 0x04000039 RID: 57
		public bool positive;
	}
}
