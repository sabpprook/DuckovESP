using System;
using UnityEngine;

namespace YKF_ESP
{
	// Token: 0x02000016 RID: 22
	public static class UIColorHelper
	{
		// Token: 0x060000BB RID: 187 RVA: 0x00006888 File Offset: 0x00004A88
		public static Color GetDistanceColor(float distance)
		{
			if (distance <= 20f)
			{
				return Color.red;
			}
			if (distance <= 50f)
			{
				return Color.yellow;
			}
			if (distance <= 100f)
			{
				return Color.white;
			}
			return Color.gray;
		}

		// Token: 0x060000BC RID: 188 RVA: 0x000068B9 File Offset: 0x00004AB9
		public static Color GetHealthColor(float healthPercent)
		{
			if (healthPercent > 0.7f)
			{
				return Color.green;
			}
			if (healthPercent > 0.5f)
			{
				return Color.yellow;
			}
			if (healthPercent > 0.3f)
			{
				return new Color(1f, 0.5f, 0f);
			}
			return Color.red;
		}
	}
}
