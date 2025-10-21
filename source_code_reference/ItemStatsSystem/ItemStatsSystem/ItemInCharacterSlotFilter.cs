using System;

namespace ItemStatsSystem
{
	// Token: 0x02000015 RID: 21
	public class ItemInCharacterSlotFilter : EffectFilter
	{
		// Token: 0x060000A5 RID: 165 RVA: 0x00003D9E File Offset: 0x00001F9E
		protected override bool OnEvaluate(EffectTriggerEventContext context)
		{
			return !(base.Master == null) && !(base.Master.Item == null) && base.Master.Item.IsInCharacterSlot();
		}
	}
}
