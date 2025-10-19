using System;
using UnityEngine;

// Token: 0x02000142 RID: 322
[RequireComponent(typeof(PipeRenderer))]
public class CircularExtrudeShape : ShapeProvider
{
	// Token: 0x06000A2D RID: 2605 RVA: 0x0002B920 File Offset: 0x00029B20
	public override PipeRenderer.OrientedPoint[] GenerateShape()
	{
		Vector3 vector = Vector3.up * this.radius;
		float num = 360f / (float)this.subdivision;
		float num2 = 1f / (float)this.subdivision;
		PipeRenderer.OrientedPoint[] array = new PipeRenderer.OrientedPoint[this.subdivision + 1];
		for (int i = 0; i < this.subdivision; i++)
		{
			Quaternion quaternion = Quaternion.AngleAxis(num * (float)i, Vector3.forward);
			Vector3 vector2 = quaternion * vector + this.offset;
			array[i] = new PipeRenderer.OrientedPoint
			{
				position = vector2,
				rotation = quaternion,
				uv = num2 * (float)i * Vector2.one
			};
		}
		array[this.subdivision] = new PipeRenderer.OrientedPoint
		{
			position = vector + this.offset,
			rotation = Quaternion.AngleAxis(0f, Vector3.forward),
			uv = Vector2.one
		};
		return array;
	}

	// Token: 0x06000A2E RID: 2606 RVA: 0x0002BA2C File Offset: 0x00029C2C
	private void OnDrawGizmosSelected()
	{
		if (this.pipeRenderer == null)
		{
			this.pipeRenderer = base.GetComponent<PipeRenderer>();
		}
		if (this.pipeRenderer != null && this.pipeRenderer.extrudeShapeProvider == null)
		{
			this.pipeRenderer.extrudeShapeProvider = this;
		}
	}

	// Token: 0x040008E0 RID: 2272
	public PipeRenderer pipeRenderer;

	// Token: 0x040008E1 RID: 2273
	public float radius = 1f;

	// Token: 0x040008E2 RID: 2274
	public int subdivision = 12;

	// Token: 0x040008E3 RID: 2275
	public Vector3 offset = Vector3.zero;
}
