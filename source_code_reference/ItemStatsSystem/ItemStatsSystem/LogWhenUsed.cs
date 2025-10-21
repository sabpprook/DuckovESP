using System;
using UnityEngine;

namespace ItemStatsSystem
{
	// Token: 0x02000024 RID: 36
	public class LogWhenUsed : UsageBehavior
	{
		// Token: 0x060001F4 RID: 500 RVA: 0x00007DBA File Offset: 0x00005FBA
		public override bool CanBeUsed(Item item, object user)
		{
			return true;
		}

		// Token: 0x060001F5 RID: 501 RVA: 0x00007DBD File Offset: 0x00005FBD
		protected override void OnUse(Item item, object user)
		{
			Debug.Log(item.name);
		}
	}
}
