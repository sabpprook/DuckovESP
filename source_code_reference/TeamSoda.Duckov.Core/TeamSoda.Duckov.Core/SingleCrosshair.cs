using System;
using UnityEngine;

// Token: 0x0200007A RID: 122
public class SingleCrosshair : MonoBehaviour
{
	// Token: 0x060004A4 RID: 1188 RVA: 0x000153AC File Offset: 0x000135AC
	public void UpdateScatter(float _scatter)
	{
		this.currentScatter = _scatter;
		RectTransform rectTransform = base.transform as RectTransform;
		rectTransform.localRotation = Quaternion.Euler(0f, 0f, this.rotation);
		Vector3 vector = Vector3.zero;
		if (this.axis != Vector3.zero)
		{
			vector = rectTransform.parent.InverseTransformDirection(rectTransform.TransformDirection(this.axis));
		}
		rectTransform.anchoredPosition = vector * (this.minDistance + this.currentScatter * this.scatterMoveScale);
		if (this.controlRectWidthHeight)
		{
			float num = this.minScale + this.currentScatter * this.scatterScaleFactor;
			rectTransform.sizeDelta = Vector2.one * num;
		}
	}

	// Token: 0x060004A5 RID: 1189 RVA: 0x0001546A File Offset: 0x0001366A
	private void OnValidate()
	{
		this.UpdateScatter(0f);
	}

	// Token: 0x040003E5 RID: 997
	public float rotation;

	// Token: 0x040003E6 RID: 998
	public Vector3 axis;

	// Token: 0x040003E7 RID: 999
	public float minDistance;

	// Token: 0x040003E8 RID: 1000
	public float scatterMoveScale = 5f;

	// Token: 0x040003E9 RID: 1001
	private float currentScatter;

	// Token: 0x040003EA RID: 1002
	public bool controlRectWidthHeight;

	// Token: 0x040003EB RID: 1003
	public float minScale = 100f;

	// Token: 0x040003EC RID: 1004
	public float scatterScaleFactor = 5f;
}
