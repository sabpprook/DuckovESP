using System;
using UnityEngine;

namespace YKF_ESP
{
	// Token: 0x02000011 RID: 17
	public static class UIColorHelper
	{
		// Token: 0x0600007A RID: 122 RVA: 0x000045F6 File Offset: 0x000027F6
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

		// Token: 0x0600007B RID: 123 RVA: 0x00004627 File Offset: 0x00002827
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
