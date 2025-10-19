using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200014A RID: 330
public class UVCylinder : MonoBehaviour
{
	// Token: 0x06000A50 RID: 2640 RVA: 0x0002D7BC File Offset: 0x0002B9BC
	private void Generate()
	{
		if (this.mesh == null)
		{
			this.mesh = new Mesh();
		}
		this.mesh.Clear();
		new List<Vector3>();
		new List<Vector2>();
		new List<Vector3>();
		new List<int>();
		for (int i = 0; i < this.subdivision; i++)
		{
		}
	}

	// Token: 0x04000904 RID: 2308
	public float radius = 1f;

	// Token: 0x04000905 RID: 2309
	public float height = 2f;

	// Token: 0x04000906 RID: 2310
	public int subdivision = 16;

	// Token: 0x04000907 RID: 2311
	private Mesh mesh;
}
