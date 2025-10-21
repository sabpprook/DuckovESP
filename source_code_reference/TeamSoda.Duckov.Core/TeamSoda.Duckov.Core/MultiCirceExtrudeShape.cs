using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000143 RID: 323
public class MultiCirceExtrudeShape : ShapeProvider
{
	// Token: 0x06000A30 RID: 2608 RVA: 0x0002BAA8 File Offset: 0x00029CA8
	public override PipeRenderer.OrientedPoint[] GenerateShape()
	{
		List<PipeRenderer.OrientedPoint> list = new List<PipeRenderer.OrientedPoint>();
		float num = 360f / (float)this.subdivision;
		float num2 = 1f / (float)(this.subdivision * this.circles.Length);
		for (int i = 0; i < this.circles.Length; i++)
		{
			MultiCirceExtrudeShape.Circle circle = this.circles[i];
			float radius = circle.radius;
			Vector3 origin = circle.origin;
			Vector3 vector = Vector3.up * radius;
			for (int j = 0; j < this.subdivision; j++)
			{
				Quaternion quaternion = Quaternion.AngleAxis(num * (float)j, Vector3.forward);
				Vector3 vector2 = origin + quaternion * vector;
				list.Add(new PipeRenderer.OrientedPoint
				{
					position = vector2,
					rotation = quaternion,
					uv = num2 * (float)(i * this.subdivision + j) * Vector2.one
				});
			}
			list.Add(new PipeRenderer.OrientedPoint
			{
				position = vector,
				rotation = Quaternion.AngleAxis(0f, Vector3.forward),
				uv = num2 * (float)((i + 1) * this.subdivision) * Vector2.one
			});
		}
		return list.ToArray();
	}

	// Token: 0x040008E4 RID: 2276
	public MultiCirceExtrudeShape.Circle[] circles;

	// Token: 0x040008E5 RID: 2277
	public int subdivision = 4;

	// Token: 0x020004A1 RID: 1185
	[Serializable]
	public struct Circle
	{
		// Token: 0x04001C19 RID: 7193
		public Vector3 origin;

		// Token: 0x04001C1A RID: 7194
		public float radius;
	}
}
