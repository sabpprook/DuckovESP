using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.Mathematics;

namespace UnityEngine.Splines
{
	// Token: 0x02000209 RID: 521
	[ExecuteInEditMode]
	[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
	[AddComponentMenu("Splines/Spline Profile Extrude")]
	public class SplineProfileExtrude : MonoBehaviour
	{
		// Token: 0x170002B7 RID: 695
		// (get) Token: 0x06000F44 RID: 3908 RVA: 0x0003C0C7 File Offset: 0x0003A2C7
		[Obsolete("Use Container instead.", false)]
		public SplineContainer container
		{
			get
			{
				return this.Container;
			}
		}

		// Token: 0x170002B8 RID: 696
		// (get) Token: 0x06000F45 RID: 3909 RVA: 0x0003C0CF File Offset: 0x0003A2CF
		// (set) Token: 0x06000F46 RID: 3910 RVA: 0x0003C0D7 File Offset: 0x0003A2D7
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

		// Token: 0x170002B9 RID: 697
		// (get) Token: 0x06000F47 RID: 3911 RVA: 0x0003C0E0 File Offset: 0x0003A2E0
		[Obsolete("Use RebuildOnSplineChange instead.", false)]
		public bool rebuildOnSplineChange
		{
			get
			{
				return this.RebuildOnSplineChange;
			}
		}

		// Token: 0x170002BA RID: 698
		// (get) Token: 0x06000F48 RID: 3912 RVA: 0x0003C0E8 File Offset: 0x0003A2E8
		// (set) Token: 0x06000F49 RID: 3913 RVA: 0x0003C0F0 File Offset: 0x0003A2F0
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

		// Token: 0x170002BB RID: 699
		// (get) Token: 0x06000F4A RID: 3914 RVA: 0x0003C0F9 File Offset: 0x0003A2F9
		// (set) Token: 0x06000F4B RID: 3915 RVA: 0x0003C101 File Offset: 0x0003A301
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

		// Token: 0x170002BC RID: 700
		// (get) Token: 0x06000F4C RID: 3916 RVA: 0x0003C110 File Offset: 0x0003A310
		// (set) Token: 0x06000F4D RID: 3917 RVA: 0x0003C118 File Offset: 0x0003A318
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

		// Token: 0x170002BD RID: 701
		// (get) Token: 0x06000F4E RID: 3918 RVA: 0x0003C12B File Offset: 0x0003A32B
		// (set) Token: 0x06000F4F RID: 3919 RVA: 0x0003C133 File Offset: 0x0003A333
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

		// Token: 0x170002BE RID: 702
		// (get) Token: 0x06000F50 RID: 3920 RVA: 0x0003C146 File Offset: 0x0003A346
		public int ProfileSeg
		{
			get
			{
				return this.profile.Length;
			}
		}

		// Token: 0x170002BF RID: 703
		// (get) Token: 0x06000F51 RID: 3921 RVA: 0x0003C150 File Offset: 0x0003A350
		// (set) Token: 0x06000F52 RID: 3922 RVA: 0x0003C158 File Offset: 0x0003A358
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

		// Token: 0x170002C0 RID: 704
		// (get) Token: 0x06000F53 RID: 3923 RVA: 0x0003C161 File Offset: 0x0003A361
		// (set) Token: 0x06000F54 RID: 3924 RVA: 0x0003C169 File Offset: 0x0003A369
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

		// Token: 0x170002C1 RID: 705
		// (get) Token: 0x06000F55 RID: 3925 RVA: 0x0003C198 File Offset: 0x0003A398
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

		// Token: 0x170002C2 RID: 706
		// (get) Token: 0x06000F56 RID: 3926 RVA: 0x0003C1AB File Offset: 0x0003A3AB
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

		// Token: 0x06000F57 RID: 3927 RVA: 0x0003C1C0 File Offset: 0x0003A3C0
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

		// Token: 0x06000F58 RID: 3928 RVA: 0x0003C238 File Offset: 0x0003A438
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

		// Token: 0x06000F59 RID: 3929 RVA: 0x0003C297 File Offset: 0x0003A497
		private void OnEnable()
		{
			Spline.Changed += this.OnSplineChanged;
		}

		// Token: 0x06000F5A RID: 3930 RVA: 0x0003C2AA File Offset: 0x0003A4AA
		private void OnDisable()
		{
			Spline.Changed -= this.OnSplineChanged;
		}

		// Token: 0x06000F5B RID: 3931 RVA: 0x0003C2BD File Offset: 0x0003A4BD
		private void OnSplineChanged(Spline spline, int knotIndex, SplineModification modificationType)
		{
			if (this.m_Container != null && this.Splines.Contains(spline) && this.m_RebuildOnSplineChange)
			{
				this.m_RebuildRequested = true;
			}
		}

		// Token: 0x06000F5C RID: 3932 RVA: 0x0003C2EA File Offset: 0x0003A4EA
		private void Update()
		{
			if (this.m_RebuildRequested && Time.time >= this.m_NextScheduledRebuild)
			{
				this.Rebuild();
			}
		}

		// Token: 0x06000F5D RID: 3933 RVA: 0x0003C308 File Offset: 0x0003A508
		public void Rebuild()
		{
			if ((this.m_Mesh = base.GetComponent<MeshFilter>().sharedMesh) == null)
			{
				return;
			}
			this.Extrude<Spline>(this.Splines[0], this.profile, this.m_Mesh, this.m_SegmentsPerUnit, this.m_Range);
			this.m_NextScheduledRebuild = Time.time + 1f / (float)this.m_RebuildFrequency;
		}

		// Token: 0x06000F5E RID: 3934 RVA: 0x0003C37C File Offset: 0x0003A57C
		private void Extrude<T>(T spline, SplineProfileExtrude.Vertex[] profile, Mesh mesh, float segmentsPerUnit, float2 range) where T : ISpline
		{
			int num = profile.Length;
			if (num < 2)
			{
				return;
			}
			float num2 = Mathf.Abs(range.y - range.x);
			int num3 = Mathf.Max((int)Mathf.Ceil(spline.GetLength() * num2 * segmentsPerUnit), 1);
			float num4 = 0f;
			List<Vector3> list = new List<Vector3>();
			List<Vector3> list2 = new List<Vector3>();
			List<Vector2> list3 = new List<Vector2>();
			Vector3 vector = Vector3.zero;
			for (int i = 0; i < num3; i++)
			{
				float num5 = math.lerp(range.x, range.y, (float)i / ((float)num3 - 1f));
				if (num5 > 1f)
				{
					num5 = 1f;
				}
				if (num5 < 1E-07f)
				{
					num5 = 1E-07f;
				}
				float3 @float;
				float3 float2;
				float3 float3;
				spline.Evaluate(num5, out @float, out float2, out float3);
				Vector3 normalized = float2.normalized;
				Vector3 normalized2 = float3.normalized;
				Vector3 vector2 = Vector3.Cross(normalized, normalized2);
				float num6 = 1f / (float)(num - 1);
				if (i > 0)
				{
					num4 += (@float - vector).magnitude;
				}
				for (int j = 0; j < num; j++)
				{
					SplineProfileExtrude.Vertex vertex = profile[j];
					float u = vertex.u;
					float y = vertex.position.y;
					float x = vertex.position.x;
					float z = vertex.position.z;
					Vector3 vector3 = Quaternion.FromToRotation(Vector3.up, normalized2) * vertex.normal;
					Vector3 vector4 = @float + x * vector2 + y * normalized2 + z * normalized;
					list.Add(vector4);
					list3.Add(new Vector2(u * this.uFactor, num4 * this.vFactor));
					list2.Add(vector3);
				}
				vector = @float;
			}
			SplineProfileExtrude.<>c__DisplayClass53_0<T> CS$<>8__locals1;
			CS$<>8__locals1.triangles = new List<int>();
			for (int k = 0; k < num3 - 1; k++)
			{
				int num7 = k * num;
				for (int l = 0; l < num - 1; l++)
				{
					int num8 = num7 + l;
					SplineProfileExtrude.<Extrude>g__AddTriangles|53_0<T>(new int[]
					{
						num8,
						num8 + 1,
						num8 + num
					}, ref CS$<>8__locals1);
					SplineProfileExtrude.<Extrude>g__AddTriangles|53_0<T>(new int[]
					{
						num8 + 1,
						num8 + 1 + num,
						num8 + num
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

		// Token: 0x06000F5F RID: 3935 RVA: 0x0003C641 File Offset: 0x0003A841
		private void OnValidate()
		{
			this.Rebuild();
		}

		// Token: 0x06000F60 RID: 3936 RVA: 0x0003C649 File Offset: 0x0003A849
		internal Mesh CreateMeshAsset()
		{
			return new Mesh
			{
				name = base.name
			};
		}

		// Token: 0x06000F61 RID: 3937 RVA: 0x0003C65C File Offset: 0x0003A85C
		private void FlattenSpline()
		{
		}

		// Token: 0x06000F63 RID: 3939 RVA: 0x0003C6CE File Offset: 0x0003A8CE
		[CompilerGenerated]
		internal static void <Extrude>g__AddTriangles|53_0<T>(int[] indicies, ref SplineProfileExtrude.<>c__DisplayClass53_0<T> A_1) where T : ISpline
		{
			A_1.triangles.AddRange(indicies);
		}

		// Token: 0x04000C69 RID: 3177
		[SerializeField]
		[Tooltip("The Spline to extrude.")]
		private SplineContainer m_Container;

		// Token: 0x04000C6A RID: 3178
		[SerializeField]
		private SplineProfileExtrude.Vertex[] profile;

		// Token: 0x04000C6B RID: 3179
		[SerializeField]
		[Tooltip("Enable to regenerate the extruded mesh when the target Spline is modified. Disable this option if the Spline will not be modified at runtime.")]
		private bool m_RebuildOnSplineChange;

		// Token: 0x04000C6C RID: 3180
		[SerializeField]
		[Tooltip("The maximum number of times per-second that the mesh will be rebuilt.")]
		private int m_RebuildFrequency = 30;

		// Token: 0x04000C6D RID: 3181
		[SerializeField]
		[Tooltip("Automatically update any Mesh, Box, or Sphere collider components when the mesh is extruded.")]
		private bool m_UpdateColliders = true;

		// Token: 0x04000C6E RID: 3182
		[SerializeField]
		[Tooltip("The number of edge loops that comprise the length of one unit of the mesh. The total number of sections is equal to \"Spline.GetLength() * segmentsPerUnit\".")]
		private float m_SegmentsPerUnit = 4f;

		// Token: 0x04000C6F RID: 3183
		[SerializeField]
		[Tooltip("The radius of the extruded mesh.")]
		private float m_Width = 0.25f;

		// Token: 0x04000C70 RID: 3184
		[SerializeField]
		private float m_Height = 0.05f;

		// Token: 0x04000C71 RID: 3185
		[SerializeField]
		[Tooltip("The section of the Spline to extrude.")]
		private Vector2 m_Range = new Vector2(0f, 1f);

		// Token: 0x04000C72 RID: 3186
		[SerializeField]
		private float uFactor = 1f;

		// Token: 0x04000C73 RID: 3187
		[SerializeField]
		private float vFactor = 1f;

		// Token: 0x04000C74 RID: 3188
		private Mesh m_Mesh;

		// Token: 0x04000C75 RID: 3189
		private bool m_RebuildRequested;

		// Token: 0x04000C76 RID: 3190
		private float m_NextScheduledRebuild;

		// Token: 0x020004E5 RID: 1253
		[Serializable]
		private struct Vertex
		{
			// Token: 0x04001D35 RID: 7477
			public Vector3 position;

			// Token: 0x04001D36 RID: 7478
			public Vector3 normal;

			// Token: 0x04001D37 RID: 7479
			public float u;
		}
	}
}
