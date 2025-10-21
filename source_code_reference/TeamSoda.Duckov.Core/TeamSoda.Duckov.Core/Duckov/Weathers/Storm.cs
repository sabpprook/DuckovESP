using System;
using UnityEngine;

namespace Duckov.Weathers
{
	// Token: 0x02000241 RID: 577
	[Serializable]
	public class Storm
	{
		// Token: 0x17000326 RID: 806
		// (get) Token: 0x060011F2 RID: 4594 RVA: 0x00044875 File Offset: 0x00042A75
		[TimeSpan]
		private long Period
		{
			get
			{
				return this.sleepTime + this.stage1Time + this.stage2Time;
			}
		}

		// Token: 0x060011F3 RID: 4595 RVA: 0x0004488C File Offset: 0x00042A8C
		public int GetStormLevel(TimeSpan dayAndTime)
		{
			long num = (dayAndTime.Ticks + this.offset) % this.Period;
			if (num < this.sleepTime)
			{
				return 0;
			}
			if (num < this.sleepTime + this.stage1Time)
			{
				return 1;
			}
			return 2;
		}

		// Token: 0x060011F4 RID: 4596 RVA: 0x000448D0 File Offset: 0x00042AD0
		public TimeSpan GetStormETA(TimeSpan dayAndTime)
		{
			long num = (dayAndTime.Ticks + this.offset) % this.Period;
			if (num < this.sleepTime)
			{
				return TimeSpan.FromTicks(this.sleepTime - num);
			}
			return TimeSpan.Zero;
		}

		// Token: 0x060011F5 RID: 4597 RVA: 0x00044910 File Offset: 0x00042B10
		public TimeSpan GetStormIOverETA(TimeSpan dayAndTime)
		{
			long num = (dayAndTime.Ticks + this.offset) % this.Period;
			return TimeSpan.FromTicks(this.sleepTime + this.stage1Time - num);
		}

		// Token: 0x060011F6 RID: 4598 RVA: 0x00044948 File Offset: 0x00042B48
		public TimeSpan GetStormIIOverETA(TimeSpan dayAndTime)
		{
			long num = (dayAndTime.Ticks + this.offset) % this.Period;
			return TimeSpan.FromTicks(this.Period - num);
		}

		// Token: 0x060011F7 RID: 4599 RVA: 0x00044978 File Offset: 0x00042B78
		public float GetSleepPercent(TimeSpan dayAndTime)
		{
			return (float)((dayAndTime.Ticks + this.offset) % this.Period) / (float)this.sleepTime;
		}

		// Token: 0x060011F8 RID: 4600 RVA: 0x00044998 File Offset: 0x00042B98
		public float GetStormRemainPercent(TimeSpan dayAndTime)
		{
			long num = (dayAndTime.Ticks + this.offset) % this.Period - this.sleepTime;
			return 1f - (float)num / ((float)this.stage1Time + (float)this.stage2Time);
		}

		// Token: 0x04000DD3 RID: 3539
		[SerializeField]
		[TimeSpan]
		private long offset;

		// Token: 0x04000DD4 RID: 3540
		[SerializeField]
		[TimeSpan]
		private long sleepTime;

		// Token: 0x04000DD5 RID: 3541
		[SerializeField]
		[TimeSpan]
		private long stage1Time;

		// Token: 0x04000DD6 RID: 3542
		[SerializeField]
		[TimeSpan]
		private long stage2Time;
	}
}
