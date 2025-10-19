using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000144 RID: 324
public class RoundCornerRectExtrudeShape : ShapeProvider
{
	// Token: 0x06000A32 RID: 2610 RVA: 0x0002BC00 File Offset: 0x00029E00
	public override PipeRenderer.OrientedPoint[] GenerateShape()
	{
		float num = this.bevelSize;
		Vector2 vector = new Vector2(-this.size.x / 2f + num, this.size.y / 2f - num);
		Vector2 vector2 = vector + new Vector2(-1f, 1f).normalized * num;
		Vector2 vector3 = vector + new Vector2(0f, 1f) * num;
		PipeRenderer.OrientedPoint orientedPoint = new PipeRenderer.OrientedPoint
		{
			position = vector2,
			normal = new Vector2(-1f, 1f),
			uv = Vector2.zero
		};
		PipeRenderer.OrientedPoint orientedPoint2 = new PipeRenderer.OrientedPoint
		{
			position = vector3,
			normal = new Vector3(0f, 1f),
			uv = num * Vector2.one
		};
		PipeRenderer.OrientedPoint orientedPoint3 = orientedPoint2;
		orientedPoint3.position.x = -orientedPoint3.position.x;
		orientedPoint3.normal.x = -orientedPoint3.normal.x;
		orientedPoint3.uv = Vector2.one - orientedPoint3.uv;
		PipeRenderer.OrientedPoint orientedPoint4 = orientedPoint;
		orientedPoint4.position.x = -orientedPoint4.position.x;
		orientedPoint4.normal.x = -orientedPoint4.normal.x;
		orientedPoint4.uv = Vector2.one;
		PipeRenderer.OrientedPoint orientedPoint5 = orientedPoint4;
		orientedPoint5.uv = Vector2.zero;
		PipeRenderer.OrientedPoint orientedPoint6 = default(PipeRenderer.OrientedPoint);
		orientedPoint6.position = vector;
		orientedPoint6.position.x = -orientedPoint6.position.x;
		orientedPoint6.position.x = orientedPoint6.position.x + num;
		orientedPoint6.normal = new Vector3(1f, 0f);
		orientedPoint6.uv = orientedPoint2.uv;
		PipeRenderer.OrientedPoint orientedPoint7 = orientedPoint6;
		orientedPoint7.position.y = -orientedPoint7.position.y;
		orientedPoint7.uv = orientedPoint3.uv;
		PipeRenderer.OrientedPoint orientedPoint8 = orientedPoint4;
		orientedPoint8.position.y = -orientedPoint8.position.y;
		orientedPoint8.normal = new Vector2(1f, -1f);
		orientedPoint8.uv = Vector2.one;
		PipeRenderer.OrientedPoint orientedPoint9 = orientedPoint8;
		orientedPoint9.uv = Vector2.zero;
		PipeRenderer.OrientedPoint orientedPoint10 = orientedPoint3;
		orientedPoint10.position.y = -orientedPoint10.position.y;
		orientedPoint10.normal = Vector2.down;
		orientedPoint10.uv = orientedPoint6.uv;
		PipeRenderer.OrientedPoint orientedPoint11 = orientedPoint2;
		orientedPoint11.position.y = -orientedPoint11.position.y;
		orientedPoint11.normal = Vector2.down;
		orientedPoint11.uv = orientedPoint3.uv;
		PipeRenderer.OrientedPoint orientedPoint12 = orientedPoint;
		orientedPoint12.position.y = -orientedPoint12.position.y;
		orientedPoint12.normal = new Vector2(-1f, -1f);
		orientedPoint12.uv = Vector2.one;
		PipeRenderer.OrientedPoint orientedPoint13 = orientedPoint12;
		orientedPoint13.uv = Vector2.zero;
		PipeRenderer.OrientedPoint orientedPoint14 = orientedPoint7;
		orientedPoint14.position.x = -orientedPoint14.position.x;
		orientedPoint14.normal = Vector2.left;
		orientedPoint14.uv = orientedPoint2.uv;
		PipeRenderer.OrientedPoint orientedPoint15 = orientedPoint6;
		orientedPoint15.position.x = -orientedPoint15.position.x;
		orientedPoint15.normal = Vector2.left;
		orientedPoint15.uv = orientedPoint3.uv;
		PipeRenderer.OrientedPoint orientedPoint16 = orientedPoint15;
		orientedPoint16.uv = Vector2.zero;
		List<PipeRenderer.OrientedPoint> list = new List<PipeRenderer.OrientedPoint>();
		list.Add(orientedPoint);
		list.Add(orientedPoint2);
		list.Add(orientedPoint3);
		list.Add(orientedPoint4);
		list.Add(orientedPoint5);
		list.Add(orientedPoint6);
		list.Add(orientedPoint7);
		list.Add(orientedPoint8);
		list.Add(orientedPoint9);
		list.Add(orientedPoint10);
		list.Add(orientedPoint11);
		list.Add(orientedPoint12);
		list.Add(orientedPoint13);
		list.Add(orientedPoint14);
		list.Add(orientedPoint15);
		list.Add(orientedPoint16);
		list.Reverse();
		if (this.resample)
		{
			list = this.Resample(list, this.stepLength);
		}
		return list.ToArray();
	}

	// Token: 0x06000A33 RID: 2611 RVA: 0x0002C098 File Offset: 0x0002A298
	private List<PipeRenderer.OrientedPoint> Resample(List<PipeRenderer.OrientedPoint> original, float stepLength)
	{
		if (stepLength < 0.01f)
		{
			return new List<PipeRenderer.OrientedPoint>();
		}
		List<PipeRenderer.OrientedPoint> list = new List<PipeRenderer.OrientedPoint>();
		int i = 0;
		float num = 0f;
		while (i < original.Count)
		{
			PipeRenderer.OrientedPoint orientedPoint = original[i];
			PipeRenderer.OrientedPoint orientedPoint2 = ((i + 1 >= original.Count) ? original[0] : original[i + 1]);
			Vector3 vector = orientedPoint2.position - orientedPoint.position;
			Vector3 normalized = vector.normalized;
			float magnitude = vector.magnitude;
			for (float num2 = 0f; num2 < magnitude; num2 += stepLength)
			{
				Vector3 vector2 = orientedPoint.position + normalized * num2;
				float num3 = num2 / magnitude;
				num += num2;
				PipeRenderer.OrientedPoint orientedPoint3 = new PipeRenderer.OrientedPoint
				{
					position = vector2,
					normal = Vector3.Lerp(orientedPoint.normal, orientedPoint2.normal, num3),
					uv = num * Vector2.one
				};
				list.Add(orientedPoint3);
			}
			i++;
		}
		return list;
	}

	// Token: 0x040008E6 RID: 2278
	public Vector2 size = Vector2.one;

	// Token: 0x040008E7 RID: 2279
	public float bevelSize = 0.1f;

	// Token: 0x040008E8 RID: 2280
	public bool resample;

	// Token: 0x040008E9 RID: 2281
	[Range(0.1f, 1f)]
	public float stepLength = 0.25f;
}
