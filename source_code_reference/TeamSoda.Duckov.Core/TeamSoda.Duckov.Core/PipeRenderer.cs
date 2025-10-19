using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000146 RID: 326
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class PipeRenderer : MonoBehaviour
{
	// Token: 0x1700020C RID: 524
	// (get) Token: 0x06000A38 RID: 2616 RVA: 0x0002C504 File Offset: 0x0002A704
	public PipeRenderer.OrientedPoint[] extrudeShape
	{
		get
		{
			if (this.extrudeShapeProvider == null)
			{
				Debug.LogWarning("Extrude shape is null, please add an extrude shape such as \"CircularExtrudeShape\"");
				return new PipeRenderer.OrientedPoint[]
				{
					new PipeRenderer.OrientedPoint
					{
						position = Vector3.zero
					}
				};
			}
			return this.extrudeShapeProvider.GenerateShape();
		}
	}

	// Token: 0x1700020D RID: 525
	// (get) Token: 0x06000A39 RID: 2617 RVA: 0x0002C558 File Offset: 0x0002A758
	public PipeRenderer.OrientedPoint[] splineShape
	{
		get
		{
			if (this.splineShapeProvider == null)
			{
				Debug.LogWarning("Spline shape is null, please add a spline shape such as \"Beveled Line Shape\"");
				return new PipeRenderer.OrientedPoint[]
				{
					new PipeRenderer.OrientedPoint
					{
						position = Vector3.zero
					}
				};
			}
			return this.splineShapeProvider.GenerateShape();
		}
	}

	// Token: 0x06000A3A RID: 2618 RVA: 0x0002C5AB File Offset: 0x0002A7AB
	public float GetTotalLength()
	{
		return PipeHelperFunctions.GetTotalLength(this.splineInUse);
	}

	// Token: 0x06000A3B RID: 2619 RVA: 0x0002C5B8 File Offset: 0x0002A7B8
	public static Mesh GeneratePipeMesh(PipeRenderer.OrientedPoint[] extrudeShape, PipeRenderer.OrientedPoint[] splineShape, Color vertexColor, float uvTwist = 0f, float extrudeShapeScale = 1f, AnimationCurve extrudeShapeScaleCurve = null, float sectionLength = 0f, bool caps = false, bool recalculateNormal = true, bool revertFaces = false)
	{
		List<Vector3> list = new List<Vector3>();
		List<Vector3> list2 = new List<Vector3>();
		List<int> list3 = new List<int>();
		List<Vector2> list4 = new List<Vector2>();
		float num = 0f;
		float totalLength = PipeHelperFunctions.GetTotalLength(splineShape);
		if (sectionLength <= 0f)
		{
			sectionLength = totalLength;
		}
		for (int i = 0; i < splineShape.Length; i++)
		{
			PipeRenderer.OrientedPoint orientedPoint = splineShape[i];
			Vector3 position = orientedPoint.position;
			Quaternion rotation = orientedPoint.rotation;
			if (i > 0)
			{
				PipeRenderer.OrientedPoint orientedPoint2 = splineShape[i - 1];
				num += (position - orientedPoint2.position).magnitude;
			}
			float num2 = num % sectionLength / sectionLength;
			float num3 = ((extrudeShapeScaleCurve != null) ? extrudeShapeScaleCurve.Evaluate(num2) : 1f);
			foreach (PipeRenderer.OrientedPoint orientedPoint3 in extrudeShape)
			{
				Vector3 vector = position + extrudeShapeScale * num3 * (rotation * orientedPoint3.position);
				Vector3 vector2 = (recalculateNormal ? (vector - position).normalized : (rotation * orientedPoint3.normal));
				Vector2 vector3 = new Vector2(orientedPoint3.uv.y + num * uvTwist, orientedPoint.uv.x);
				list.Add(vector);
				list2.Add(revertFaces ? (-vector2) : vector2);
				list4.Add(vector3);
			}
			if (i > 0)
			{
				int num4 = i * extrudeShape.Length;
				for (int k = 0; k < extrudeShape.Length - 1; k++)
				{
					int num5 = num4 + k;
					int num6 = num4 + k + 1;
					int num7 = num6 - extrudeShape.Length;
					int num8 = num5 - extrudeShape.Length;
					if (revertFaces)
					{
						list3.Add(num6);
						list3.Add(num8);
						list3.Add(num5);
						list3.Add(num7);
						list3.Add(num8);
						list3.Add(num6);
					}
					else
					{
						list3.Add(num5);
						list3.Add(num8);
						list3.Add(num6);
						list3.Add(num6);
						list3.Add(num8);
						list3.Add(num7);
					}
				}
			}
		}
		if (caps)
		{
			Vector3 vector4 = -splineShape[0].tangent;
			int num9 = 0;
			int[] array = new int[extrudeShape.Length];
			for (int l = 0; l < extrudeShape.Length; l++)
			{
				int num10 = num9 + l;
				list.Add(list[num10]);
				list2.Add(vector4);
				list4.Add(list4[num10]);
				array[l] = list.Count - 1;
			}
			Vector3 vector5 = Vector3.zero;
			for (int m = 0; m < array.Length; m++)
			{
				vector5 += list[m];
			}
			vector5 /= (float)array.Length;
			list.Add(vector5);
			list2.Add(vector4);
			list4.Add(Vector2.one * 0f / 5f);
			int num11 = list.Count - 1;
			for (int n = 0; n < array.Length - 1; n++)
			{
				list3.Add(num11);
				list3.Add(array[n + 1]);
				list3.Add(array[n]);
			}
			vector4 = splineShape[splineShape.Length - 1].tangent;
			num9 = extrudeShape.Length * (splineShape.Length - 1);
			for (int num12 = 0; num12 < extrudeShape.Length; num12++)
			{
				int num13 = num9 + num12;
				list.Add(list[num13]);
				list2.Add(vector4);
				list4.Add(list4[num13]);
				array[num12] = list.Count - 1;
			}
			vector5 = Vector3.zero;
			for (int num14 = 0; num14 < array.Length; num14++)
			{
				vector5 += list[array[num14]];
			}
			vector5 /= (float)array.Length;
			list.Add(vector5);
			list2.Add(vector4);
			list4.Add(Vector2.one * 0f / 5f);
			num11 = list.Count - 1;
			for (int num15 = 0; num15 < array.Length - 1; num15++)
			{
				list3.Add(array[num15]);
				list3.Add(array[num15 + 1]);
				list3.Add(num11);
			}
		}
		Color[] array2 = new Color[list.Count];
		for (int num16 = 0; num16 < array2.Length; num16++)
		{
			array2[num16] = vertexColor;
		}
		Mesh mesh = new Mesh();
		mesh.vertices = list.ToArray();
		mesh.uv = list4.ToArray();
		mesh.triangles = list3.ToArray();
		mesh.normals = list2.ToArray();
		mesh.RecalculateTangents();
		mesh.colors = array2;
		mesh.name = "Generated Mesh";
		return mesh;
	}

	// Token: 0x06000A3C RID: 2620 RVA: 0x0002CA80 File Offset: 0x0002AC80
	private void OnDrawGizmosSelected()
	{
		if (this.meshFilter == null)
		{
			this.meshFilter = base.GetComponent<MeshFilter>();
		}
		this.splineInUse = this.splineShape;
		this.meshFilter.mesh = PipeRenderer.GeneratePipeMesh(this.extrudeShape, this.splineInUse, this.vertexColor, this.uvTwist, this.extrudeShapeScale, this.useExtrudeShapeScaleCurve ? this.extrudeShapeScaleCurve : null, this.sectionLength, this.caps, this.recalculateNormal, this.revertFaces);
		if (this.drawSplinePoints)
		{
			Matrix4x4 localToWorldMatrix = base.transform.localToWorldMatrix;
			for (int i = 0; i < this.splineInUse.Length; i++)
			{
				PipeRenderer.OrientedPoint orientedPoint = this.splineInUse[i];
				Vector3 vector = localToWorldMatrix.MultiplyPoint(orientedPoint.position);
				Gizmos.DrawWireCube(vector, Vector3.one * 0.01f);
				Vector3 vector2 = localToWorldMatrix.MultiplyVector(orientedPoint.tangent);
				Gizmos.DrawLine(vector, vector + vector2 * 0.02f);
			}
		}
	}

	// Token: 0x06000A3D RID: 2621 RVA: 0x0002CB84 File Offset: 0x0002AD84
	private void Start()
	{
		if (this.meshFilter == null)
		{
			this.meshFilter = base.GetComponent<MeshFilter>();
		}
	}

	// Token: 0x06000A3E RID: 2622 RVA: 0x0002CBA0 File Offset: 0x0002ADA0
	public Vector3 GetPositionByOffset(float offset, out Quaternion rotation)
	{
		if (this.splineInUse == null || this.splineInUse.Length < 1)
		{
			rotation = Quaternion.identity;
			return Vector3.zero;
		}
		if (offset <= 0f)
		{
			rotation = this.splineInUse[0].rotation;
			return this.splineInUse[0].position;
		}
		float num = 0f;
		for (int i = 1; i < this.splineInUse.Length; i++)
		{
			PipeRenderer.OrientedPoint orientedPoint = this.splineInUse[i];
			PipeRenderer.OrientedPoint orientedPoint2 = this.splineInUse[i - 1];
			float num2 = num;
			float magnitude = (orientedPoint.position - orientedPoint2.position).magnitude;
			num += magnitude;
			float num3 = num;
			if (num3 > offset)
			{
				float num4 = num3 - num2;
				float num5 = (offset - num2) / num4;
				rotation = Quaternion.Lerp(orientedPoint2.rotation, orientedPoint.rotation, num5);
				return orientedPoint2.position + (orientedPoint.position - orientedPoint2.position) * num5;
			}
		}
		rotation = this.splineInUse[this.splineInUse.Length - 1].rotation;
		return this.splineInUse[this.splineInUse.Length - 1].position;
	}

	// Token: 0x040008EC RID: 2284
	public MeshFilter meshFilter;

	// Token: 0x040008ED RID: 2285
	public ShapeProvider splineShapeProvider;

	// Token: 0x040008EE RID: 2286
	public ShapeProvider extrudeShapeProvider;

	// Token: 0x040008EF RID: 2287
	[Header("UV")]
	public float uvTwist;

	// Token: 0x040008F0 RID: 2288
	[Header("Options")]
	public float extrudeShapeScale = 1f;

	// Token: 0x040008F1 RID: 2289
	public float sectionLength = 10f;

	// Token: 0x040008F2 RID: 2290
	public bool caps;

	// Token: 0x040008F3 RID: 2291
	public bool useExtrudeShapeScaleCurve;

	// Token: 0x040008F4 RID: 2292
	public AnimationCurve extrudeShapeScaleCurve = AnimationCurve.Constant(0f, 1f, 1f);

	// Token: 0x040008F5 RID: 2293
	public Color vertexColor = Color.white;

	// Token: 0x040008F6 RID: 2294
	public bool recalculateNormal = true;

	// Token: 0x040008F7 RID: 2295
	public bool revertFaces;

	// Token: 0x040008F8 RID: 2296
	[Header("Gizmos")]
	public bool drawSplinePoints;

	// Token: 0x040008F9 RID: 2297
	public PipeRenderer.OrientedPoint[] splineInUse;

	// Token: 0x020004A2 RID: 1186
	[Serializable]
	public struct OrientedPoint
	{
		// Token: 0x04001C1B RID: 7195
		public Vector3 position;

		// Token: 0x04001C1C RID: 7196
		public Quaternion rotation;

		// Token: 0x04001C1D RID: 7197
		public Vector3 tangent;

		// Token: 0x04001C1E RID: 7198
		public Vector3 rotationalAxisVector;

		// Token: 0x04001C1F RID: 7199
		public Vector3 normal;

		// Token: 0x04001C20 RID: 7200
		public Vector2 uv;
	}
}
