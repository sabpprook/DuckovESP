using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02000145 RID: 325
public class RoundCornerSquareExtrudeShape : ShapeProvider
{
	// Token: 0x06000A35 RID: 2613 RVA: 0x0002C1D0 File Offset: 0x0002A3D0
	public override PipeRenderer.OrientedPoint[] GenerateShape()
	{
		float num = this.size;
		float num2 = this.bevelSize;
		Vector2 vector = new Vector2(-num / 2f + num2, num / 2f - num2);
		Vector2 vector2 = vector + new Vector2(-1f, 1f).normalized * num2;
		Vector2 vector3 = vector + new Vector2(0f, 1f) * num2;
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
			uv = num2 * Vector2.one
		};
		PipeRenderer.OrientedPoint orientedPoint3 = orientedPoint2;
		orientedPoint3.position.x = -orientedPoint3.position.x;
		orientedPoint3.normal.x = -orientedPoint3.normal.x;
		orientedPoint3.uv = Vector2.one - orientedPoint3.uv;
		PipeRenderer.OrientedPoint orientedPoint4 = orientedPoint;
		orientedPoint4.position.x = -orientedPoint4.position.x;
		orientedPoint4.normal.x = -orientedPoint4.normal.x;
		orientedPoint4.uv = Vector2.one;
		List<PipeRenderer.OrientedPoint> list = new List<PipeRenderer.OrientedPoint>();
		list.Add(orientedPoint);
		list.Add(orientedPoint2);
		list.Add(orientedPoint3);
		list.Add(orientedPoint4);
		for (int i = 1; i <= 3; i++)
		{
			PipeRenderer.OrientedPoint orientedPoint5 = orientedPoint;
			PipeRenderer.OrientedPoint orientedPoint6 = orientedPoint2;
			PipeRenderer.OrientedPoint orientedPoint7 = orientedPoint3;
			PipeRenderer.OrientedPoint orientedPoint8 = orientedPoint4;
			for (int j = 0; j < i; j++)
			{
				orientedPoint5 = RoundCornerSquareExtrudeShape.<GenerateShape>g__Rotate90|2_0(orientedPoint5);
				orientedPoint6 = RoundCornerSquareExtrudeShape.<GenerateShape>g__Rotate90|2_0(orientedPoint6);
				orientedPoint7 = RoundCornerSquareExtrudeShape.<GenerateShape>g__Rotate90|2_0(orientedPoint7);
				orientedPoint8 = RoundCornerSquareExtrudeShape.<GenerateShape>g__Rotate90|2_0(orientedPoint8);
			}
			orientedPoint5.uv += (float)i * Vector2.one;
			orientedPoint6.uv += (float)i * Vector2.one;
			orientedPoint7.uv += (float)i * Vector2.one;
			orientedPoint8.uv += (float)i * Vector2.one;
			list.Add(orientedPoint5);
			list.Add(orientedPoint6);
			list.Add(orientedPoint7);
			list.Add(orientedPoint8);
		}
		list.Reverse();
		return list.ToArray();
	}

	// Token: 0x06000A37 RID: 2615 RVA: 0x0002C4BC File Offset: 0x0002A6BC
	[CompilerGenerated]
	internal static PipeRenderer.OrientedPoint <GenerateShape>g__Rotate90|2_0(PipeRenderer.OrientedPoint original)
	{
		Quaternion quaternion = Quaternion.AngleAxis(-90f, Vector3.forward);
		PipeRenderer.OrientedPoint orientedPoint = original;
		orientedPoint.position = quaternion * original.position;
		orientedPoint.normal = quaternion * original.normal;
		return orientedPoint;
	}

	// Token: 0x040008EA RID: 2282
	public float size = 1f;

	// Token: 0x040008EB RID: 2283
	public float bevelSize = 0.1f;
}
