using System;
using Duckov.Quests;
using UnityEngine;

// Token: 0x02000117 RID: 279
public class Condition_TimeOfDay : Condition
{
	// Token: 0x0600096A RID: 2410 RVA: 0x000292C8 File Offset: 0x000274C8
	public override bool Evaluate()
	{
		float num = (float)GameClock.TimeOfDay.TotalHours % 24f;
		return (num >= this.from && num <= this.to) || (this.to < this.from && (num >= this.from || num <= this.to));
	}

	// Token: 0x04000850 RID: 2128
	[Range(0f, 24f)]
	public float from;

	// Token: 0x04000851 RID: 2129
	[Range(0f, 24f)]
	public float to;
}
