using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000148 RID: 328
public class MultipleBezierShape : ShapeProvider
{
	// Token: 0x06000A47 RID: 2631 RVA: 0x0002D31C File Offset: 0x0002B51C
	public override PipeRenderer.OrientedPoint[] GenerateShape()
	{
		List<PipeRenderer.OrientedPoint> list = new List<PipeRenderer.OrientedPoint>();
		for (int i = 0; i < this.points.Length / 4; i++)
		{
			Vector3 vector = this.points[i * 4];
			Vector3 vector2 = this.points[i * 4 + 1];
			Vector3 vector3 = this.points[i * 4 + 2];
			Vector3 vector4 = this.points[i * 4 + 3];
			PipeRenderer.OrientedPoint[] array = BezierSpline.GenerateShape(vector, vector2, vector3, vector4, this.subdivisions);
			if (list.Count > 0)
			{
				list.RemoveAt(list.Count - 1);
			}
			list.AddRange(array);
		}
		PipeRenderer.OrientedPoint[] array2 = list.ToArray();
		PipeHelperFunctions.RecalculateNormals(ref array2);
		PipeHelperFunctions.RecalculateUvs(ref array2, 1f, 0f);
		PipeHelperFunctions.RotatePoints(ref array2, this.rotationOffset, this.twist);
		return array2;
	}

	// Token: 0x040008FE RID: 2302
	public Vector3[] points;

	// Token: 0x040008FF RID: 2303
	public int subdivisions = 16;

	// Token: 0x04000900 RID: 2304
	public bool lockedHandles;

	// Token: 0x04000901 RID: 2305
	public float rotationOffset;

	// Token: 0x04000902 RID: 2306
	public float twist;

	// Token: 0x04000903 RID: 2307
	public bool edit;
}
