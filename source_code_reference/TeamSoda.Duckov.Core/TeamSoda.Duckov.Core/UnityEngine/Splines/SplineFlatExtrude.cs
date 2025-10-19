using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.Mathematics;

namespace UnityEngine.Splines
{
	// Token: 0x02000208 RID: 520
	[ExecuteInEditMode]
	[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
	[AddComponentMenu("Splines/Spline Flat Extrude")]
	public class SplineFlatExtrude : MonoBehaviour
	{
		// Token: 0x170002AB RID: 683
		// (get) Token: 0x06000F23 RID: 3875 RVA: 0x0003BAE2 File Offset: 0x00039CE2
		[Obsolete("Use Container instead.", false)]
		public SplineContainer container
		{
			get
			{
				return this.Container;
			}
		}

		// Token: 0x170002AC RID: 684
		// (get) Token: 0x06000F24 RID: 3876 RVA: 0x0003BAEA File Offset: 0x00039CEA
		// (set) Token: 0x06000F25 RID: 3877 RVA: 0x0003BAF2 File Offset: 0x00039CF2
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

		// Token: 0x170002AD RID: 685
		// (get) Token: 0x06000F26 RID: 3878 RVA: 0x0003BAFB File Offset: 0x00039CFB
		[Obsolete("Use RebuildOnSplineChange instead.", false)]
		public bool rebuildOnSplineChange
		{
			get
			{
				return this.RebuildOnSplineChange;
			}
		}

		// Token: 0x170002AE RID: 686
		// (get) Token: 0x06000F27 RID: 3879 RVA: 0x0003BB03 File Offset: 0x00039D03
		// (set) Token: 0x06000F28 RID: 3880 RVA: 0x0003BB0B File Offset: 0x00039D0B
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

		// Token: 0x170002AF RID: 687
		// (get) Token: 0x06000F29 RID: 3881 RVA: 0x0003BB14 File Offset: 0x00039D14
		// (set) Token: 0x06000F2A RID: 3882 RVA: 0x0003BB1C File Offset: 0x00039D1C
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

		// Token: 0x170002B0 RID: 688
		// (get) Token: 0x06000F2B RID: 3883 RVA: 0x0003BB2B File Offset: 0x00039D2B
		// (set) Token: 0x06000F2C RID: 3884 RVA: 0x0003BB33 File Offset: 0x00039D33
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

		// Token: 0x170002B1 RID: 689
		// (get) Token: 0x06000F2D RID: 3885 RVA: 0x0003BB46 File Offset: 0x00039D46
		// (set) Token: 0x06000F2E RID: 3886 RVA: 0x0003BB4E File Offset: 0x00039D4E
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

		// Token: 0x170002B2 RID: 690
		// (get) Token: 0x06000F2F RID: 3887 RVA: 0x0003BB61 File Offset: 0x00039D61
		// (set) Token: 0x06000F30 RID: 3888 RVA: 0x0003BB69 File Offset: 0x00039D69
		public int ProfileSeg
		{
			get
			{
				return this.m_ProfileSeg;
			}
			set
			{
				this.m_ProfileSeg = value;
			}
		}

		// Token: 0x170002B3 RID: 691
		// (get) Token: 0x06000F31 RID: 3889 RVA: 0x0003BB72 File Offset: 0x00039D72
		// (set) Token: 0x06000F32 RID: 3890 RVA: 0x0003BB7A File Offset: 0x00039D7A
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

		// Token: 0x170002B4 RID: 692
		// (get) Token: 0x06000F33 RID: 3891 RVA: 0x0003BB83 File Offset: 0x00039D83
		// (set) Token: 0x06000F34 RID: 3892 RVA: 0x0003BB8B File Offset: 0x00039D8B
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

		// Token: 0x170002B5 RID: 693
		// (get) Token: 0x06000F35 RID: 3893 RVA: 0x0003BBBA File Offset: 0x00039DBA
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

		// Token: 0x170002B6 RID: 694
		// (get) Token: 0x06000F36 RID: 3894 RVA: 0x0003BBCD File Offset: 0x00039DCD
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

		// Token: 0x06000F37 RID: 3895 RVA: 0x0003BBE0 File Offset: 0x00039DE0
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

		// Token: 0x06000F38 RID: 3896 RVA: 0x0003BC58 File Offset: 0x00039E58
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

		// Token: 0x06000F39 RID: 3897 RVA: 0x0003BCB7 File Offset: 0x00039EB7
		private void OnEnable()
		{
			Spline.Changed += this.OnSplineChanged;
		}

		// Token: 0x06000F3A RID: 3898 RVA: 0x0003BCCA File Offset: 0x00039ECA
		private void OnDisable()
		{
			Spline.Changed -= this.OnSplineChanged;
		}

		// Token: 0x06000F3B RID: 3899 RVA: 0x0003BCDD File Offset: 0x00039EDD
		private void OnSplineChanged(Spline spline, int knotIndex, SplineModification modificationType)
		{
			if (this.m_Container != null && this.Splines.Contains(spline) && this.m_RebuildOnSplineChange)
			{
				this.m_RebuildRequested = true;
			}
		}

		// Token: 0x06000F3C RID: 3900 RVA: 0x0003BD0A File Offset: 0x00039F0A
		private void Update()
		{
			if (this.m_RebuildRequested && Time.time >= this.m_NextScheduledRebuild)
			{
				this.Rebuild();
			}
		}

		// Token: 0x06000F3D RID: 3901 RVA: 0x0003BD28 File Offset: 0x00039F28
		public void Rebuild()
		{
			if ((this.m_Mesh = base.GetComponent<MeshFilter>().sharedMesh) == null)
			{
				return;
			}
			this.Extrude<Spline>(this.Splines[0], this.m_Mesh, this.m_Width, this.m_ProfileSeg, this.m_Height, this.m_SegmentsPerUnit, this.m_Range);
			this.m_NextScheduledRebuild = Time.time + 1f / (float)this.m_RebuildFrequency;
		}

		// Token: 0x06000F3E RID: 3902 RVA: 0x0003BDA8 File Offset: 0x00039FA8
		private void Extrude<T>(T spline, Mesh mesh, float width, int profileSegments, float height, float segmentsPerUnit, float2 range) where T : ISpline
		{
			if (profileSegments < 2)
			{
				return;
			}
			float num = Mathf.Abs(range.y - range.x);
			int num2 = Mathf.Max((int)Mathf.Ceil(spline.GetLength() * num * segmentsPerUnit), 1);
			float num3 = 0f;
			List<Vector3> list = new List<Vector3>();
			List<Vector3> list2 = new List<Vector3>();
			List<Vector2> list3 = new List<Vector2>();
			Vector3 vector = Vector3.zero;
			for (int i = 0; i < num2; i++)
			{
				float num4 = math.lerp(range.x, range.y, (float)i / ((float)num2 - 1f));
				if (num4 > 1f)
				{
					num4 = 1f;
				}
				float3 @float;
				float3 float2;
				float3 float3;
				spline.Evaluate(num4, out @float, out float2, out float3);
				Vector3 normalized = float2.normalized;
				Vector3 normalized2 = float3.normalized;
				Vector3 vector2 = Vector3.Cross(normalized, normalized2);
				float num5 = 1f / (float)(profileSegments - 1);
				if (i > 0)
				{
					num3 += (@float - vector).magnitude;
				}
				for (int j = 0; j < profileSegments; j++)
				{
					float num6 = num5 * (float)j;
					float num7 = (num6 - 0.5f) * 2f;
					float num8 = Mathf.Cos(num7 * 3.1415927f * 0.5f) * height;
					float num9 = num7 * width;
					Vector3 vector3 = @float + num9 * vector2 + num8 * normalized2;
					list.Add(vector3);
					list3.Add(new Vector2(num6 * this.uFactor, num3 * this.vFactor));
					list2.Add(normalized2);
				}
				vector = @float;
			}
			SplineFlatExtrude.<>c__DisplayClass53_0<T> CS$<>8__locals1;
			CS$<>8__locals1.triangles = new List<int>();
			for (int k = 0; k < num2 - 1; k++)
			{
				int num10 = k * profileSegments;
				for (int l = 0; l < profileSegments - 1; l++)
				{
					int num11 = num10 + l;
					SplineFlatExtrude.<Extrude>g__AddTriangles|53_0<T>(new int[]
					{
						num11,
						num11 + 1,
						num11 + profileSegments
					}, ref CS$<>8__locals1);
					SplineFlatExtrude.<Extrude>g__AddTriangles|53_0<T>(new int[]
					{
						num11 + 1,
						num11 + 1 + profileSegments,
						num11 + profileSegments
					}, ref CS$<>8__locals1);
				}
			}
			mesh.Clear();
			mesh.vertices = list.ToArray();
			mesh.uv = list3.ToArray();
			mesh.triangles = CS$<>8__locals1.triangles.ToArray();
			mesh.RecalculateNormals();
			mesh.RecalculateBounds();
		}

		// Token: 0x06000F3F RID: 3903 RVA: 0x0003C026 File Offset: 0x0003A226
		private void OnValidate()
		{
			this.Rebuild();
		}

		// Token: 0x06000F40 RID: 3904 RVA: 0x0003C02E File Offset: 0x0003A22E
		internal Mesh CreateMeshAsset()
		{
			return new Mesh
			{
				name = base.name
			};
		}

		// Token: 0x06000F41 RID: 3905 RVA: 0x0003C041 File Offset: 0x0003A241
		private void FlattenSpline()
		{
		}

		// Token: 0x06000F43 RID: 3907 RVA: 0x0003C0B9 File Offset: 0x0003A2B9
		[CompilerGenerated]
		internal static void <Extrude>g__AddTriangles|53_0<T>(int[] indicies, ref SplineFlatExtrude.<>c__DisplayClass53_0<T> A_1) where T : ISpline
		{
			A_1.triangles.AddRange(indicies);
		}

		// Token: 0x04000C5B RID: 3163
		[SerializeField]
		[Tooltip("The Spline to extrude.")]
		private SplineContainer m_Container;

		// Token: 0x04000C5C RID: 3164
		[SerializeField]
		[Tooltip("Enable to regenerate the extruded mesh when the target Spline is modified. Disable this option if the Spline will not be modified at runtime.")]
		private bool m_RebuildOnSplineChange;

		// Token: 0x04000C5D RID: 3165
		[SerializeField]
		[Tooltip("The maximum number of times per-second that the mesh will be rebuilt.")]
		private int m_RebuildFrequency = 30;

		// Token: 0x04000C5E RID: 3166
		[SerializeField]
		[Tooltip("Automatically update any Mesh, Box, or Sphere collider components when the mesh is extruded.")]
		private bool m_UpdateColliders = true;

		// Token: 0x04000C5F RID: 3167
		[SerializeField]
		[Tooltip("The number of edge loops that comprise the length of one unit of the mesh. The total number of sections is equal to \"Spline.GetLength() * segmentsPerUnit\".")]
		private float m_SegmentsPerUnit = 4f;

		// Token: 0x04000C60 RID: 3168
		[SerializeField]
		[Tooltip("The radius of the extruded mesh.")]
		private float m_Width = 0.25f;

		// Token: 0x04000C61 RID: 3169
		[SerializeField]
		private int m_ProfileSeg = 2;

		// Token: 0x04000C62 RID: 3170
		[SerializeField]
		private float m_Height = 0.05f;

		// Token: 0x04000C63 RID: 3171
		[SerializeField]
		[Tooltip("The section of the Spline to extrude.")]
		private Vector2 m_Range = new Vector2(0f, 0.999f);

		// Token: 0x04000C64 RID: 3172
		[SerializeField]
		private float uFactor = 1f;

		// Token: 0x04000C65 RID: 3173
		[SerializeField]
		private float vFactor = 1f;

		// Token: 0x04000C66 RID: 3174
		private Mesh m_Mesh;

		// Token: 0x04000C67 RID: 3175
		private bool m_RebuildRequested;

		// Token: 0x04000C68 RID: 3176
		private float m_NextScheduledRebuild;
	}
}
