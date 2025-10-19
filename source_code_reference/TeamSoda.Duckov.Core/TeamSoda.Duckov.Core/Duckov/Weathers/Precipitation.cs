using System;
using UnityEngine;

namespace Duckov.Weathers
{
	// Token: 0x02000242 RID: 578
	[Serializable]
	public class Precipitation
	{
		// Token: 0x17000327 RID: 807
		// (get) Token: 0x060011FA RID: 4602 RVA: 0x000449E2 File Offset: 0x00042BE2
		public float CloudyThreshold
		{
			get
			{
				return this.cloudyThreshold;
			}
		}

		// Token: 0x17000328 RID: 808
		// (get) Token: 0x060011FB RID: 4603 RVA: 0x000449EA File Offset: 0x00042BEA
		public float RainyThreshold
		{
			get
			{
				return this.rainyThreshold;
			}
		}

		// Token: 0x060011FC RID: 4604 RVA: 0x000449F2 File Offset: 0x00042BF2
		public bool IsRainy(TimeSpan dayAndTime)
		{
			return this.Get(dayAndTime) > this.rainyThreshold;
		}

		// Token: 0x060011FD RID: 4605 RVA: 0x00044A03 File Offset: 0x00042C03
		public bool IsCloudy(TimeSpan dayAndTime)
		{
			return this.Get(dayAndTime) > this.cloudyThreshold;
		}

		// Token: 0x060011FE RID: 4606 RVA: 0x00044A14 File Offset: 0x00042C14
		public float Get(TimeSpan dayAndTime)
		{
			Vector2 perlinNoiseCoord = this.GetPerlinNoiseCoord(dayAndTime);
			return Mathf.Clamp01(((Mathf.PerlinNoise(perlinNoiseCoord.x, perlinNoiseCoord.y) + Mathf.PerlinNoise(perlinNoiseCoord.x + 0.5f + 123.4f, perlinNoiseCoord.y - 567.8f)) / 2f - 0.5f) * this.contrast + 0.5f + this.offset);
		}

		// Token: 0x060011FF RID: 4607 RVA: 0x00044A84 File Offset: 0x00042C84
		public Vector2 GetPerlinNoiseCoord(TimeSpan dayAndTime)
		{
			float num = (float)(dayAndTime.Days % 3650) * 24f + (float)dayAndTime.Hours + (float)dayAndTime.Minutes / 60f;
			int num2 = dayAndTime.Days / 3650;
			return new Vector2(num * this.frequency, (float)(this.seed + num2));
		}

		// Token: 0x06001200 RID: 4608 RVA: 0x00044AE0 File Offset: 0x00042CE0
		internal void SetSeed(int seed)
		{
			this.seed = seed;
		}

		// Token: 0x06001201 RID: 4609 RVA: 0x00044AEC File Offset: 0x00042CEC
		public float Get()
		{
			TimeSpan now = GameClock.Now;
			return this.Get(now);
		}

		// Token: 0x06001202 RID: 4610 RVA: 0x00044B08 File Offset: 0x00042D08
		public bool IsRainy()
		{
			TimeSpan now = GameClock.Now;
			return this.IsRainy(now);
		}

		// Token: 0x06001203 RID: 4611 RVA: 0x00044B24 File Offset: 0x00042D24
		public bool IsCloudy()
		{
			TimeSpan now = GameClock.Now;
			return this.IsCloudy(now);
		}

		// Token: 0x04000DD7 RID: 3543
		[SerializeField]
		private int seed;

		// Token: 0x04000DD8 RID: 3544
		[SerializeField]
		[Range(0f, 1f)]
		private float cloudyThreshold;

		// Token: 0x04000DD9 RID: 3545
		[SerializeField]
		[Range(0f, 1f)]
		private float rainyThreshold;

		// Token: 0x04000DDA RID: 3546
		[SerializeField]
		private float frequency = 1f;

		// Token: 0x04000DDB RID: 3547
		[SerializeField]
		private float offset;

		// Token: 0x04000DDC RID: 3548
		[SerializeField]
		private float contrast = 1f;
	}
}
