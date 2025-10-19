using System;
using NodeCanvas.Framework;

namespace NodeCanvas.Tasks.Conditions
{
	// Token: 0x02000400 RID: 1024
	public class CheckNoticed : ConditionTask<AICharacterController>
	{
		// Token: 0x0600251C RID: 9500 RVA: 0x00080141 File Offset: 0x0007E341
		protected override string OnInit()
		{
			return null;
		}

		// Token: 0x0600251D RID: 9501 RVA: 0x00080144 File Offset: 0x0007E344
		protected override void OnEnable()
		{
		}

		// Token: 0x0600251E RID: 9502 RVA: 0x00080146 File Offset: 0x0007E346
		protected override void OnDisable()
		{
		}

		// Token: 0x0600251F RID: 9503 RVA: 0x00080148 File Offset: 0x0007E348
		protected override bool OnCheck()
		{
			bool flag = base.agent.isNoticing(this.noticedTimeThreshold);
			if (this.resetNotice)
			{
				base.agent.noticed = false;
			}
			return flag;
		}

		// Token: 0x04001948 RID: 6472
		public float noticedTimeThreshold = 0.2f;

		// Token: 0x04001949 RID: 6473
		public bool resetNotice;
	}
}
