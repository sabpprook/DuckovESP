using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x0200017E RID: 382
public class MainMenuCamera : MonoBehaviour
{
	// Token: 0x06000B84 RID: 2948 RVA: 0x00030B60 File Offset: 0x0002ED60
	private void Update()
	{
		Vector3 mousePosition = Input.mousePosition;
		float num = (float)Screen.width;
		float num2 = (float)Screen.height;
		float num3 = mousePosition.x / num;
		float num4 = mousePosition.y / num2;
		base.transform.localRotation = quaternion.Euler(0f, Mathf.Lerp(this.yawRange.x, this.yawRange.y, num3) * 0.017453292f, 0f, math.RotationOrder.ZXY);
		if (this.pitchRoot)
		{
			this.pitchRoot.localRotation = quaternion.Euler(Mathf.Lerp(this.pitchRange.x, this.pitchRange.y, num4) * 0.017453292f, 0f, 0f, math.RotationOrder.ZXY);
		}
		base.transform.localPosition = new Vector3(Mathf.Lerp(this.posRangeX.x, this.posRangeX.y, num3), Mathf.Lerp(this.posRangeY.x, this.posRangeY.y, num4), 0f);
	}

	// Token: 0x040009D0 RID: 2512
	public Vector2 yawRange;

	// Token: 0x040009D1 RID: 2513
	public Vector2 pitchRange;

	// Token: 0x040009D2 RID: 2514
	public Transform pitchRoot;

	// Token: 0x040009D3 RID: 2515
	[FormerlySerializedAs("posRange")]
	public Vector2 posRangeX;

	// Token: 0x040009D4 RID: 2516
	public Vector2 posRangeY;
}
