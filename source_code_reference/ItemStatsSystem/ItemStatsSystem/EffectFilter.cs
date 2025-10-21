using System;
using Duckov.Utilities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ItemStatsSystem
{
	// Token: 0x02000010 RID: 16
	[RequireComponent(typeof(Effect))]
	public class EffectFilter : EffectComponent, ISelfValidator
	{
		// Token: 0x17000018 RID: 24
		// (get) Token: 0x06000063 RID: 99 RVA: 0x00003257 File Offset: 0x00001457
		protected override Color ActiveLabelColor
		{
			get
			{
				return DuckovUtilitiesSettings.Colors.EffectFilter;
			}
		}

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x06000064 RID: 100 RVA: 0x00003263 File Offset: 0x00001463
		public override string DisplayName
		{
			get
			{
				return "未命名过滤器(" + base.GetType().Name + ")";
			}
		}

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x06000065 RID: 101 RVA: 0x0000327F File Offset: 0x0000147F
		// (set) Token: 0x06000066 RID: 102 RVA: 0x00003287 File Offset: 0x00001487
		protected bool IgnoreNegativeTrigger
		{
			get
			{
				return this.ignoreNegativeTrigger;
			}
			set
			{
				this.ignoreNegativeTrigger = value;
			}
		}

		// Token: 0x06000067 RID: 103 RVA: 0x00003290 File Offset: 0x00001490
		public bool Evaluate(EffectTriggerEventContext context)
		{
			return !base.enabled || (!context.positive && this.IgnoreNegativeTrigger) || this.OnEvaluate(context);
		}

		// Token: 0x06000068 RID: 104 RVA: 0x000032B5 File Offset: 0x000014B5
		protected virtual bool OnEvaluate(EffectTriggerEventContext context)
		{
			return true;
		}

		// Token: 0x06000069 RID: 105 RVA: 0x000032B8 File Offset: 0x000014B8
		public override void Validate(SelfValidationResult result)
		{
			base.Validate(result);
			if (base.Master != null && !base.Master.Filters.Contains(this))
			{
				result.AddError("Master 中不包含本 Filter。").WithFix("将此 Filter 添加到 Master 中。", delegate
				{
					base.Master.AddEffectComponent(this);
				}, true);
			}
		}

		// Token: 0x0400002C RID: 44
		[SerializeField]
		private bool ignoreNegativeTrigger = true;
	}
}
