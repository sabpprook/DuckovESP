using System;

namespace ItemStatsSystem
{
	// Token: 0x02000018 RID: 24
	public class TriggerOnSetItem : EffectTrigger
	{
		// Token: 0x060000B3 RID: 179 RVA: 0x00003F97 File Offset: 0x00002197
		protected override void OnMasterSetTargetItem(Effect effect, Item item)
		{
			base.Trigger(true);
		}
	}
}
