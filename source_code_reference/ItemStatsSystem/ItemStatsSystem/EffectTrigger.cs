using System;
using Duckov.Utilities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ItemStatsSystem
{
	// Token: 0x02000011 RID: 17
	[RequireComponent(typeof(Effect))]
	public class EffectTrigger : EffectComponent, ISelfValidator
	{
		// Token: 0x1700001B RID: 27
		// (get) Token: 0x0600006C RID: 108 RVA: 0x0000332D File Offset: 0x0000152D
		protected override Color ActiveLabelColor
		{
			get
			{
				return DuckovUtilitiesSettings.Colors.EffectTrigger;
			}
		}

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x0600006D RID: 109 RVA: 0x00003339 File Offset: 0x00001539
		public override string DisplayName
		{
			get
			{
				return "未命名触发器(" + base.GetType().Name + ")";
			}
		}

		// Token: 0x0600006E RID: 110 RVA: 0x00003355 File Offset: 0x00001555
		protected void Trigger(bool positive = true)
		{
			base.Master.Trigger(new EffectTriggerEventContext(this, positive));
		}

		// Token: 0x0600006F RID: 111 RVA: 0x00003369 File Offset: 0x00001569
		protected void TriggerPositive()
		{
			this.Trigger(true);
		}

		// Token: 0x06000070 RID: 112 RVA: 0x00003372 File Offset: 0x00001572
		protected void TriggerNegative()
		{
			this.Trigger(false);
		}

		// Token: 0x06000071 RID: 113 RVA: 0x0000337C File Offset: 0x0000157C
		public override void Validate(SelfValidationResult result)
		{
			base.Validate(result);
			if (base.Master != null && !base.Master.Triggers.Contains(this))
			{
				result.AddError("Master 中不包含本 Filter。").WithFix("将此 Filter 添加到 Master 中。", delegate
				{
					base.Master.AddEffectComponent(this);
				}, true);
			}
		}

		// Token: 0x06000072 RID: 114 RVA: 0x000033D4 File Offset: 0x000015D4
		protected virtual void OnDisable()
		{
			this.Trigger(false);
		}

		// Token: 0x06000073 RID: 115 RVA: 0x000033DD File Offset: 0x000015DD
		protected virtual void OnMasterSetTargetItem(Effect effect, Item item)
		{
		}

		// Token: 0x06000074 RID: 116 RVA: 0x000033DF File Offset: 0x000015DF
		internal void NotifySetItem(Effect effect, Item targetItem)
		{
			this.OnMasterSetTargetItem(effect, targetItem);
		}
	}
}
