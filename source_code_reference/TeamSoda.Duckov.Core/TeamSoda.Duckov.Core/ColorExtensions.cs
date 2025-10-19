using System;
using UnityEngine;

// Token: 0x02000098 RID: 152
public static class ColorExtensions
{
	// Token: 0x06000525 RID: 1317 RVA: 0x00017438 File Offset: 0x00015638
	public static string ToHexString(this Color color)
	{
		return ((byte)(color.r * 255f)).ToString("X2") + ((byte)(color.g * 255f)).ToString("X2") + ((byte)(color.b * 255f)).ToString("X2") + ((byte)(color.a * 255f)).ToString("X2");
	}
}
