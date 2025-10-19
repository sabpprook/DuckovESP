using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000147 RID: 327
public class BezierSpline : ShapeProvider
{
	// Token: 0x06000A40 RID: 2624 RVA: 0x0002CD48 File Offset: 0x0002AF48
	public static Vector3 GetPoint(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, float t)
	{
		float num = Mathf.Pow(1f - t, 3f);
		float num2 = 3f * t * (1f - t) * (1f - t);
		float num3 = 3f * t * t * (1f - t);
		float num4 = t * t * t;
		return num * p1 + num2 * p2 + num3 * p3 + num4 * p4;
	}

	// Token: 0x06000A41 RID: 2625 RVA: 0x0002CDCC File Offset: 0x0002AFCC
	public static Vector3 GetTangent(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, float t)
	{
		float num = -3f * (1f - t) * (1f - t);
		float num2 = 3f * (1f - t) * (1f - t) - 6f * t * (1f - t);
		float num3 = 6f * t * (1f - t) - 3f * t * t;
		float num4 = 3f * t * t;
		return num * p1 + num2 * p2 + num3 * p3 + num4 * p4;
	}

	// Token: 0x06000A42 RID: 2626 RVA: 0x0002CE70 File Offset: 0x0002B070
	public static Vector3 GetNormal(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, float t)
	{
		Vector3 tangent = BezierSpline.GetTangent(p1, p2, p3, p4, t);
		Vector3 vector = 6f * (1f - t) * p1 - (6f * (1f - t) + (6f - 12f * t)) * p2 + (6f - 12f * t - 6f * t) * p3 + 6f * t * p4;
		Vector3 normalized = tangent.normalized;
		Vector3 normalized2 = (normalized + vector).normalized;
		return Vector3.Cross(Vector3.Cross(normalized, normalized2), normalized).normalized;
	}

	// Token: 0x06000A43 RID: 2627 RVA: 0x0002CF2C File Offset: 0x0002B12C
	public static PipeRenderer.OrientedPoint[] GenerateShape(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, int subdivisions)
	{
		List<PipeRenderer.OrientedPoint> list = new List<PipeRenderer.OrientedPoint>();
		float num = 1f / (float)subdivisions;
		float num2 = 0f;
		Vector3 vector = Vector3.zero;
		for (int i = 0; i <= subdivisions; i++)
		{
			float num3 = (float)i * num;
			Vector3 point = BezierSpline.GetPoint(p0, p1, p2, p3, num3);
			Vector3 tangent = BezierSpline.GetTangent(p0, p1, p2, p3, num3);
			Vector3 normal = BezierSpline.GetNormal(p0, p1, p2, p3, num3);
			if (i > 0)
			{
				num2 += (point - vector).magnitude;
			}
			Quaternion quaternion = Quaternion.identity;
			quaternion = Quaternion.LookRotation(tangent, normal);
			PipeRenderer.OrientedPoint orientedPoint = new PipeRenderer.OrientedPoint
			{
				position = point,
				tangent = tangent,
				normal = normal,
				rotation = quaternion,
				rotationalAxisVector = Vector3.forward,
				uv = Vector2.one * num2
			};
			list.Add(orientedPoint);
			vector = point;
		}
		PipeRenderer.OrientedPoint[] array = list.ToArray();
		PipeHelperFunctions.RecalculateNormals(ref array);
		return array;
	}

	// Token: 0x06000A44 RID: 2628 RVA: 0x0002D030 File Offset: 0x0002B230
	public override PipeRenderer.OrientedPoint[] GenerateShape()
	{
		List<PipeRenderer.OrientedPoint> list = new List<PipeRenderer.OrientedPoint>();
		float num = 1f / (float)this.subdivisions;
		Vector3 vector = this.points[0];
		Vector3 vector2 = this.points[1];
		Vector3 vector3 = this.points[2];
		Vector3 vector4 = this.points[3];
		float num2 = 0f;
		Vector3 vector5 = Vector3.zero;
		for (int i = 0; i <= this.subdivisions; i++)
		{
			float num3 = (float)i * num;
			Vector3 point = BezierSpline.GetPoint(vector, vector2, vector3, vector4, num3);
			Vector3 tangent = BezierSpline.GetTangent(vector, vector2, vector3, vector4, num3);
			Vector3 normal = BezierSpline.GetNormal(vector, vector2, vector3, vector4, num3);
			if (i > 0)
			{
				num2 += (point - vector5).magnitude;
			}
			Quaternion quaternion = Quaternion.identity;
			quaternion = Quaternion.LookRotation(tangent, normal);
			PipeRenderer.OrientedPoint orientedPoint = new PipeRenderer.OrientedPoint
			{
				position = point,
				tangent = tangent,
				normal = normal,
				rotation = quaternion,
				rotationalAxisVector = Vector3.forward,
				uv = Vector2.one * num2
			};
			list.Add(orientedPoint);
			vector5 = point;
		}
		PipeRenderer.OrientedPoint[] array = list.ToArray();
		PipeHelperFunctions.RecalculateNormals(ref array);
		return array;
	}

	// Token: 0x06000A45 RID: 2629 RVA: 0x0002D180 File Offset: 0x0002B380
	private void OnDrawGizmosSelected()
	{
		if (this.drawGizmos)
		{
			Matrix4x4 localToWorldMatrix = base.transform.localToWorldMatrix;
			for (int i = 0; i < this.points.Length; i++)
			{
				Gizmos.DrawWireCube(localToWorldMatrix.MultiplyPoint(this.points[i]), Vector3.one * 0.1f);
			}
			float num = 1f / (float)this.subdivisions;
			for (int j = 0; j < this.subdivisions; j++)
			{
				Vector3 vector = BezierSpline.GetPoint(this.points[0], this.points[1], this.points[2], this.points[3], num * (float)j);
				Vector3 vector2 = BezierSpline.GetPoint(this.points[0], this.points[1], this.points[2], this.points[3], num * (float)(j + 1));
				vector = localToWorldMatrix.MultiplyPoint(vector);
				vector2 = localToWorldMatrix.MultiplyPoint(vector2);
				Gizmos.DrawLine(vector, vector2);
				Vector3 vector3 = BezierSpline.GetTangent(this.points[0], this.points[1], this.points[2], this.points[3], num * (float)j);
				vector3 = localToWorldMatrix.MultiplyVector(vector3);
				Vector3 vector4 = vector + vector3 * 0.1f;
				Gizmos.DrawLine(vector, vector4);
			}
		}
	}

	// Token: 0x040008FA RID: 2298
	public PipeRenderer pipeRenderer;

	// Token: 0x040008FB RID: 2299
	public Vector3[] points = new Vector3[4];

	// Token: 0x040008FC RID: 2300
	public int subdivisions = 12;

	// Token: 0x040008FD RID: 2301
	public bool drawGizmos;
}
