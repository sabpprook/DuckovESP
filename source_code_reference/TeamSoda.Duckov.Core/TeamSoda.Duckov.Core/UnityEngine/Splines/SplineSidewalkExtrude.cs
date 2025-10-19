using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.Mathematics;

namespace UnityEngine.Splines
{
	// Token: 0x0200020A RID: 522
	[ExecuteInEditMode]
	[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
	[AddComponentMenu("Splines/Spline Sidewalk Extrude")]
	public class SplineSidewalkExtrude : MonoBehaviour
	{
		// Token: 0x170002C3 RID: 707
		// (get) Token: 0x06000F64 RID: 3940 RVA: 0x0003C6DC File Offset: 0x0003A8DC
		[Obsolete("Use Container instead.", false)]
		public SplineContainer container
		{
			get
			{
				return this.Container;
			}
		}

		// Token: 0x170002C4 RID: 708
		// (get) Token: 0x06000F65 RID: 3941 RVA: 0x0003C6E4 File Offset: 0x0003A8E4
		// (set) Token: 0x06000F66 RID: 3942 RVA: 0x0003C6EC File Offset: 0x0003A8EC
		public SplineContainer Container
		{
			get
			{
				return this.m_Container;
			}
			set
			{
				this.m_Container = value;
			}
		}

		// Token: 0x170002C5 RID: 709
		// (get) Token: 0x06000F67 RID: 3943 RVA: 0x0003C6F5 File Offset: 0x0003A8F5
		[Obsolete("Use RebuildOnSplineChange instead.", false)]
		public bool rebuildOnSplineChange
		{
			get
			{
				return this.RebuildOnSplineChange;
			}
		}

		// Token: 0x170002C6 RID: 710
		// (get) Token: 0x06000F68 RID: 3944 RVA: 0x0003C6FD File Offset: 0x0003A8FD
		// (set) Token: 0x06000F69 RID: 3945 RVA: 0x0003C705 File Offset: 0x0003A905
		public bool RebuildOnSplineChange
		{
			get
			{
				return this.m_RebuildOnSplineChange;
			}
			set
			{
				this.m_RebuildOnSplineChange = value;
			}
		}

		// Token: 0x170002C7 RID: 711
		// (get) Token: 0x06000F6A RID: 3946 RVA: 0x0003C70E File Offset: 0x0003A90E
		// (set) Token: 0x06000F6B RID: 3947 RVA: 0x0003C716 File Offset: 0x0003A916
		public int RebuildFrequency
		{
			get
			{
				return this.m_RebuildFrequency;
			}
			set
			{
				this.m_RebuildFrequency = Mathf.Max(value, 1);
			}
		}

		// Token: 0x170002C8 RID: 712
		// (get) Token: 0x06000F6C RID: 3948 RVA: 0x0003C725 File Offset: 0x0003A925
		// (set) Token: 0x06000F6D RID: 3949 RVA: 0x0003C72D File Offset: 0x0003A92D
		public float SegmentsPerUnit
		{
			get
			{
				return this.m_SegmentsPerUnit;
			}
			set
			{
				this.m_SegmentsPerUnit = Mathf.Max(value, 0.0001f);
			}
		}

		// Token: 0x170002C9 RID: 713
		// (get) Token: 0x06000F6E RID: 3950 RVA: 0x0003C740 File Offset: 0x0003A940
		// (set) Token: 0x06000F6F RID: 3951 RVA: 0x0003C748 File Offset: 0x0003A948
		public float Width
		{
			get
			{
				return this.m_Width;
			}
			set
			{
				this.m_Width = Mathf.Max(value, 1E-05f);
			}
		}

		// Token: 0x170002CA RID: 714
		// (get) Token: 0x06000F70 RID: 3952 RVA: 0x0003C75B File Offset: 0x0003A95B
		// (set) Token: 0x06000F71 RID: 3953 RVA: 0x0003C763 File Offset: 0x0003A963
		public float Height
		{
			get
			{
				return this.m_Height;
			}
			set
			{
				this.m_Height = value;
			}
		}

		// Token: 0x170002CB RID: 715
		// (get) Token: 0x06000F72 RID: 3954 RVA: 0x0003C76C File Offset: 0x0003A96C
		// (set) Token: 0x06000F73 RID: 3955 RVA: 0x0003C774 File Offset: 0x0003A974
		public Vector2 Range
		{
			get
			{
				return this.m_Range;
			}
			set
			{
				this.m_Range = new Vector2(Mathf.Min(value.x, value.y), Mathf.Max(value.x, value.y));
			}
		}

		// Token: 0x170002CC RID: 716
		// (get) Token: 0x06000F74 RID: 3956 RVA: 0x0003C7A3 File Offset: 0x0003A9A3
		public Spline Spline
		{
			get
			{
				SplineContainer container = this.m_Container;
				if (container == null)
				{
					return null;
				}
				return container.Spline;
			}
		}

		// Token: 0x170002CD RID: 717
		// (get) Token: 0x06000F75 RID: 3957 RVA: 0x0003C7B6 File Offset: 0x0003A9B6
		public IReadOnlyList<Spline> Splines
		{
			get
			{
				SplineContainer container = this.m_Container;
				if (container == null)
				{
					return null;
				}
				return container.Splines;
			}
		}

		// Token: 0x06000F76 RID: 3958 RVA: 0x0003C7CC File Offset: 0x0003A9CC
		internal void Reset()
		{
			base.TryGetComponent<SplineContainer>(out this.m_Container);
			MeshFilter meshFilter;
			if (base.TryGetComponent<MeshFilter>(out meshFilter))
			{
				meshFilter.sharedMesh = (this.m_Mesh = this.CreateMeshAsset());
			}
			MeshRenderer meshRenderer;
			if (base.TryGetComponent<MeshRenderer>(out meshRenderer) && meshRenderer.sharedMaterial == null)
			{
				GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
				Material sharedMaterial = gameObject.GetComponent<MeshRenderer>().sharedMaterial;
				Object.DestroyImmediate(gameObject);
				meshRenderer.sharedMaterial = sharedMaterial;
			}
			this.Rebuild();
		}

		// Token: 0x06000F77 RID: 3959 RVA: 0x0003C844 File Offset: 0x0003AA44
		private void Start()
		{
			if (this.m_Container == null || this.m_Container.Spline == null)
			{
				Debug.LogError("Spline Extrude does not have a valid SplineContainer set.");
				return;
			}
			if ((this.m_Mesh = base.GetComponent<MeshFilter>().sharedMesh) == null)
			{
				Debug.LogError("SplineExtrude.createMeshInstance is disabled, but there is no valid mesh assigned. Please create or assign a writable mesh asset.");
			}
			this.Rebuild();
		}

		// Token: 0x06000F78 RID: 3960 RVA: 0x0003C8A3 File Offset: 0x0003AAA3
		private void OnEnable()
		{
			Spline.Changed += this.OnSplineChanged;
		}

		// Token: 0x06000F79 RID: 3961 RVA: 0x0003C8B6 File Offset: 0x0003AAB6
		private void OnDisable()
		{
			Spline.Changed -= this.OnSplineChanged;
		}

		// Token: 0x06000F7A RID: 3962 RVA: 0x0003C8C9 File Offset: 0x0003AAC9
		private void OnSplineChanged(Spline spline, int knotIndex, SplineModification modificationType)
		{
			if (this.m_Container != null && this.Splines.Contains(spline) && this.m_RebuildOnSplineChange)
			{
				this.m_RebuildRequested = true;
			}
		}

		// Token: 0x06000F7B RID: 3963 RVA: 0x0003C8F6 File Offset: 0x0003AAF6
		private void Update()
		{
			if (this.m_RebuildRequested && Time.time >= this.m_NextScheduledRebuild)
			{
				this.Rebuild();
			}
		}

		// Token: 0x06000F7C RID: 3964 RVA: 0x0003C914 File Offset: 0x0003AB14
		public void Rebuild()
		{
			if ((this.m_Mesh = base.GetComponent<MeshFilter>().sharedMesh) == null)
			{
				return;
			}
			this.Extrude<Spline>(this.Splines[0], this.m_Mesh, this.m_SegmentsPerUnit, this.m_Range);
			this.m_NextScheduledRebuild = Time.time + 1f / (float)this.m_RebuildFrequency;
		}

		// Token: 0x06000F7D RID: 3965 RVA: 0x0003C980 File Offset: 0x0003AB80
		private void Extrude<T>(T spline, Mesh mesh, float segmentsPerUnit, float2 range) where T : ISpline
		{
			SplineSidewalkExtrude.<>c__DisplayClass55_0<T> CS$<>8__locals1;
			CS$<>8__locals1.<>4__this = this;
			mesh.Clear();
			if (this.sides == SplineSidewalkExtrude.Sides.None)
			{
				return;
			}
			float num = Mathf.Abs(range.y - range.x);
			int num2 = Mathf.Max((int)Mathf.Ceil(spline.GetLength() * num * segmentsPerUnit), 1);
			CS$<>8__locals1.v = 0f;
			CS$<>8__locals1.verts = new List<Vector3>();
			CS$<>8__locals1.n = new List<Vector3>();
			CS$<>8__locals1.uv = new List<Vector2>();
			CS$<>8__locals1.triangles = new List<int>();
			Vector3 vector = Vector3.zero;
			SplineSidewalkExtrude.ProfileLine[] array = this.GenerateProfile();
			CS$<>8__locals1.profileVertexCount = array.Length * 2;
			for (int i = 0; i < num2; i++)
			{
				SplineSidewalkExtrude.<>c__DisplayClass55_1<T> CS$<>8__locals2;
				CS$<>8__locals2.isLastSegment = i == num2 - 1;
				float num3 = math.lerp(range.x, range.y, (float)i / ((float)num2 - 1f));
				if (num3 > 1f)
				{
					num3 = 1f;
				}
				if (num3 < 1E-07f)
				{
					num3 = 1E-07f;
				}
				float3 @float;
				float3 float2;
				spline.Evaluate(num3, out CS$<>8__locals2.center, out @float, out float2);
				CS$<>8__locals2.forward = @float.normalized;
				CS$<>8__locals2.up = float2.normalized;
				CS$<>8__locals2.right = Vector3.Cross(CS$<>8__locals2.forward, CS$<>8__locals2.up);
				if (i > 0)
				{
					CS$<>8__locals1.v += (CS$<>8__locals2.center - vector).magnitude;
				}
				foreach (SplineSidewalkExtrude.ProfileLine profileLine in array)
				{
					this.<Extrude>g__DrawLine|55_2<T>(profileLine.start, profileLine.end, profileLine.u0, profileLine.u1, ref CS$<>8__locals1, ref CS$<>8__locals2);
				}
				vector = CS$<>8__locals2.center;
			}
			mesh.vertices = CS$<>8__locals1.verts.ToArray();
			mesh.uv = CS$<>8__locals1.uv.ToArray();
			mesh.triangles = CS$<>8__locals1.triangles.ToArray();
			mesh.RecalculateNormals();
			mesh.RecalculateBounds();
		}

		// Token: 0x06000F7E RID: 3966 RVA: 0x0003CBB4 File Offset: 0x0003ADB4
		private SplineSidewalkExtrude.ProfileLine[] GenerateProfile()
		{
			SplineSidewalkExtrude.<>c__DisplayClass57_0 CS$<>8__locals1;
			CS$<>8__locals1.lines = new List<SplineSidewalkExtrude.ProfileLine>();
			float num = this.height - this.bevel;
			float num2 = Mathf.Sqrt(2f * this.bevel * this.bevel);
			float num3 = this.width - 2f * this.bevel;
			CS$<>8__locals1.uFactor = num + num2 + num3 + num2 + num;
			if ((this.sides | SplineSidewalkExtrude.Sides.Left) == this.sides)
			{
				SplineSidewalkExtrude.<GenerateProfile>g__Add|57_0(-this.offset - this.width, 0f, -this.offset - this.width, this.height - this.bevel, 0f, num, ref CS$<>8__locals1);
				SplineSidewalkExtrude.<GenerateProfile>g__Add|57_0(-this.offset - this.width + this.bevel, this.height, -this.offset - this.bevel, this.height, num + num2, num + num2 + num3, ref CS$<>8__locals1);
				SplineSidewalkExtrude.<GenerateProfile>g__Add|57_0(-this.offset, this.height - this.bevel, -this.offset, 0f, num + num2 + num3 + num2, num + num2 + num3 + num2 + num, ref CS$<>8__locals1);
				if (this.bevel > 0f)
				{
					SplineSidewalkExtrude.<GenerateProfile>g__Add|57_0(-this.offset - this.width, this.height - this.bevel, -this.offset - this.width + this.bevel, this.height, num, num + num2, ref CS$<>8__locals1);
					SplineSidewalkExtrude.<GenerateProfile>g__Add|57_0(-this.offset - this.bevel, this.height, -this.offset, this.height - this.bevel, num + num2 + num3, num + num2 + num3 + num2, ref CS$<>8__locals1);
				}
			}
			if ((this.sides | SplineSidewalkExtrude.Sides.Right) == this.sides)
			{
				SplineSidewalkExtrude.<GenerateProfile>g__Add|57_0(this.offset, 0f, this.offset, this.height - this.bevel, num + num2 + num3 + num2 + num, num + num2 + num3 + num2, ref CS$<>8__locals1);
				SplineSidewalkExtrude.<GenerateProfile>g__Add|57_0(this.offset + this.bevel, this.height, this.offset + this.width - this.bevel, this.height, num + num2 + num3, num + num2, ref CS$<>8__locals1);
				SplineSidewalkExtrude.<GenerateProfile>g__Add|57_0(this.offset + this.width, this.height - this.bevel, this.offset + this.width, 0f, num, 0f, ref CS$<>8__locals1);
				if (this.bevel > 0f)
				{
					SplineSidewalkExtrude.<GenerateProfile>g__Add|57_0(this.offset, this.height - this.bevel, this.offset + this.bevel, this.height, num + num2 + num3 + num2, num + num2 + num3, ref CS$<>8__locals1);
					SplineSidewalkExtrude.<GenerateProfile>g__Add|57_0(this.offset + this.width - this.bevel, this.height, this.offset + this.width, this.height - this.bevel, num + num2, num, ref CS$<>8__locals1);
				}
			}
			return CS$<>8__locals1.lines.ToArray();
		}

		// Token: 0x06000F7F RID: 3967 RVA: 0x0003CEB1 File Offset: 0x0003B0B1
		private void OnValidate()
		{
			this.Rebuild();
		}

		// Token: 0x06000F80 RID: 3968 RVA: 0x0003CEB9 File Offset: 0x0003B0B9
		internal Mesh CreateMeshAsset()
		{
			return new Mesh
			{
				name = base.name
			};
		}

		// Token: 0x06000F81 RID: 3969 RVA: 0x0003CECC File Offset: 0x0003B0CC
		private void FlattenSpline()
		{
		}

		// Token: 0x06000F83 RID: 3971 RVA: 0x0003CF48 File Offset: 0x0003B148
		[CompilerGenerated]
		internal static Vector3 <Extrude>g__ProfileToObject|55_1<T>(Vector3 profilePos, ref SplineSidewalkExtrude.<>c__DisplayClass55_1<T> A_1) where T : ISpline
		{
			return A_1.center + profilePos.x * A_1.right + profilePos.y * A_1.up + profilePos.z * A_1.forward;
		}

		// Token: 0x06000F84 RID: 3972 RVA: 0x0003CFA4 File Offset: 0x0003B1A4
		[CompilerGenerated]
		private void <Extrude>g__DrawLine|55_2<T>(Vector3 p0, Vector3 p1, float u0, float u1, ref SplineSidewalkExtrude.<>c__DisplayClass55_0<T> A_5, ref SplineSidewalkExtrude.<>c__DisplayClass55_1<T> A_6) where T : ISpline
		{
			Vector3 vector = SplineSidewalkExtrude.<Extrude>g__ProfileToObject|55_1<T>(p0, ref A_6);
			Vector3 vector2 = SplineSidewalkExtrude.<Extrude>g__ProfileToObject|55_1<T>(p1, ref A_6);
			Vector3 vector3 = Vector3.Cross(vector2 - vector, A_6.forward);
			int count = A_5.verts.Count;
			A_5.verts.Add(vector);
			A_5.verts.Add(vector2);
			A_5.n.Add(vector3);
			A_5.n.Add(vector3);
			A_5.uv.Add(new Vector2(u0 * this.uFactor, A_5.v * this.vFactor));
			A_5.uv.Add(new Vector2(u1 * this.uFactor, A_5.v * this.vFactor));
			if (!A_6.isLastSegment)
			{
				this.<Extrude>g__AddTriangles|55_0<T>(new int[]
				{
					count,
					count + 1,
					count + A_5.profileVertexCount
				}, ref A_5);
				this.<Extrude>g__AddTriangles|55_0<T>(new int[]
				{
					count + 1,
					count + 1 + A_5.profileVertexCount,
					count + A_5.profileVertexCount
				}, ref A_5);
			}
		}

		// Token: 0x06000F85 RID: 3973 RVA: 0x0003D0C2 File Offset: 0x0003B2C2
		[CompilerGenerated]
		private void <Extrude>g__AddTriangles|55_0<T>(int[] indicies, ref SplineSidewalkExtrude.<>c__DisplayClass55_0<T> A_2) where T : ISpline
		{
			A_2.triangles.AddRange(indicies);
		}

		// Token: 0x06000F86 RID: 3974 RVA: 0x0003D0D0 File Offset: 0x0003B2D0
		[CompilerGenerated]
		internal static void <GenerateProfile>g__Add|57_0(float x0, float y0, float x1, float y1, float u0, float u1, ref SplineSidewalkExtrude.<>c__DisplayClass57_0 A_6)
		{
			A_6.lines.Add(new SplineSidewalkExtrude.ProfileLine(new Vector3(x0, y0), new Vector3(x1, y1), u0 / A_6.uFactor, u1 / A_6.uFactor));
		}

		// Token: 0x04000C77 RID: 3191
		[SerializeField]
		[Tooltip("The Spline to extrude.")]
		private SplineContainer m_Container;

		// Token: 0x04000C78 RID: 3192
		[SerializeField]
		private float offset;

		// Token: 0x04000C79 RID: 3193
		[SerializeField]
		private float height;

		// Token: 0x04000C7A RID: 3194
		[SerializeField]
		private float width;

		// Token: 0x04000C7B RID: 3195
		[SerializeField]
		private float bevel;

		// Token: 0x04000C7C RID: 3196
		[SerializeField]
		private SplineSidewalkExtrude.Sides sides = SplineSidewalkExtrude.Sides.Both;

		// Token: 0x04000C7D RID: 3197
		[SerializeField]
		[Tooltip("Enable to regenerate the extruded mesh when the target Spline is modified. Disable this option if the Spline will not be modified at runtime.")]
		private bool m_RebuildOnSplineChange;

		// Token: 0x04000C7E RID: 3198
		[SerializeField]
		[Tooltip("The maximum number of times per-second that the mesh will be rebuilt.")]
		private int m_RebuildFrequency = 30;

		// Token: 0x04000C7F RID: 3199
		[SerializeField]
		[Tooltip("Automatically update any Mesh, Box, or Sphere collider components when the mesh is extruded.")]
		private bool m_UpdateColliders = true;

		// Token: 0x04000C80 RID: 3200
		[SerializeField]
		[Tooltip("The number of edge loops that comprise the length of one unit of the mesh. The total number of sections is equal to \"Spline.GetLength() * segmentsPerUnit\".")]
		private float m_SegmentsPerUnit = 4f;

		// Token: 0x04000C81 RID: 3201
		[SerializeField]
		[Tooltip("The radius of the extruded mesh.")]
		private float m_Width = 0.25f;

		// Token: 0x04000C82 RID: 3202
		[SerializeField]
		private float m_Height = 0.05f;

		// Token: 0x04000C83 RID: 3203
		[SerializeField]
		[Tooltip("The section of the Spline to extrude.")]
		private Vector2 m_Range = new Vector2(0f, 0.999f);

		// Token: 0x04000C84 RID: 3204
		[SerializeField]
		private float uFactor = 1f;

		// Token: 0x04000C85 RID: 3205
		[SerializeField]
		private float vFactor = 1f;

		// Token: 0x04000C86 RID: 3206
		private Mesh m_Mesh;

		// Token: 0x04000C87 RID: 3207
		private bool m_RebuildRequested;

		// Token: 0x04000C88 RID: 3208
		private float m_NextScheduledRebuild;

		// Token: 0x020004E7 RID: 1255
		[Flags]
		public enum Sides
		{
			// Token: 0x04001D3A RID: 7482
			None = 0,
			// Token: 0x04001D3B RID: 7483
			Left = 1,
			// Token: 0x04001D3C RID: 7484
			Right = 2,
			// Token: 0x04001D3D RID: 7485
			Both = 3
		}

		// Token: 0x020004E8 RID: 1256
		private struct ProfileLine
		{
			// Token: 0x06002721 RID: 10017 RVA: 0x0008E4D7 File Offset: 0x0008C6D7
			public ProfileLine(Vector3 start, Vector3 end, float u0, float u1)
			{
				this.start = start;
				this.end = end;
				this.u0 = u0;
				this.u1 = u1;
			}

			// Token: 0x04001D3E RID: 7486
			public Vector3 start;

			// Token: 0x04001D3F RID: 7487
			public Vector3 end;

			// Token: 0x04001D40 RID: 7488
			public float u0;

			// Token: 0x04001D41 RID: 7489
			public float u1;
		}
	}
}
