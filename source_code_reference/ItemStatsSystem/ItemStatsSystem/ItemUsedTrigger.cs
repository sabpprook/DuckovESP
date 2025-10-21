using System;
using UnityEngine;

namespace ItemStatsSystem
{
	// Token: 0x02000016 RID: 22
	public class ItemUsedTrigger : EffectTrigger
	{
		// Token: 0x17000027 RID: 39
		// (get) Token: 0x060000A7 RID: 167 RVA: 0x00003DDD File Offset: 0x00001FDD
		public override string DisplayName
		{
			get
			{
				return "当物品被使用";
			}
		}

		// Token: 0x060000A8 RID: 168 RVA: 0x00003DE4 File Offset: 0x00001FE4
		private void OnEnable()
		{
			if (base.Master != null && base.Master.Item != null)
			{
				base.Master.Item.onUse += this.OnItemUsed;
				return;
			}
			Debug.LogError("因为找不到对象，未能注册物品使用事件。");
		}

		// Token: 0x060000A9 RID: 169 RVA: 0x00003E3C File Offset: 0x0000203C
		private new void OnDisable()
		{
			if (base.Master == null)
			{
				return;
			}
			if (base.Master.Item == null)
			{
				return;
			}
			base.Master.Item.onUse -= this.OnItemUsed;
		}

		// Token: 0x060000AA RID: 170 RVA: 0x00003E88 File Offset: 0x00002088
		private void OnItemUsed(Item item, object user)
		{
			base.Trigger(true);
		}
	}
}
