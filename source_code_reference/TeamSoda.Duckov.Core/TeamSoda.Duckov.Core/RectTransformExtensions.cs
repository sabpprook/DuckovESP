using System;
using UnityEngine;

// Token: 0x02000099 RID: 153
public static class RectTransformExtensions
{
	// Token: 0x06000526 RID: 1318 RVA: 0x000174B2 File Offset: 0x000156B2
	public static Camera GetUICamera()
	{
		return null;
	}

	// Token: 0x06000527 RID: 1319 RVA: 0x000174B8 File Offset: 0x000156B8
	public static void MatchWorldPosition(this RectTransform rectTransform, Vector3 worldPosition, Vector3 worldSpaceOffset = default(Vector3))
	{
		RectTransform rectTransform2 = rectTransform.parent as RectTransform;
		if (rectTransform2 == null)
		{
			return;
		}
		worldPosition += worldSpaceOffset;
		Vector2 vector = RectTransformUtility.WorldToScreenPoint(Camera.main, worldPosition);
		Vector2 vector2;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform2, vector, RectTransformExtensions.GetUICamera(), out vector2);
		rectTransform.localPosition = vector2;
	}
}
