using System;
using Duckov.Utilities;
using UnityEngine;

// Token: 0x020000CB RID: 203
public class SkillProjectileLineHUD : MonoBehaviour
{
	// Token: 0x06000650 RID: 1616 RVA: 0x0001C893 File Offset: 0x0001AA93
	private void Awake()
	{
		this.obsticleLayers = GameplayDataSettings.Layers.wallLayerMask | GameplayDataSettings.Layers.groundLayerMask | GameplayDataSettings.Layers.fowBlockLayers;
	}

	// Token: 0x06000651 RID: 1617 RVA: 0x0001C8D0 File Offset: 0x0001AAD0
	public bool UpdateLine(Vector3 start, Vector3 target, float verticleSpeed, ref Vector3 hitPoint)
	{
		float magnitude = Physics.gravity.magnitude;
		if (this.line.points.Length != this.fragmentCount + 1)
		{
			this.line.points = new Vector3[this.fragmentCount + 1];
			this.line.colors = new Color[this.fragmentCount + 1];
		}
		float num = verticleSpeed / magnitude;
		float num2 = Mathf.Sqrt(2f * (num * verticleSpeed * 0.5f + start.y - target.y) / magnitude);
		float num3 = num + num2;
		Vector3 vector = start;
		vector.y = 0f;
		Vector3 vector2 = target;
		vector2.y = 0f;
		float num4 = Vector3.Distance(vector, vector2);
		float num5 = 0f;
		Vector3 vector3 = vector2 - vector;
		if (vector3.magnitude > 0f)
		{
			vector3 = vector3.normalized;
			num5 = num4 / num3;
		}
		else
		{
			vector3 = Vector3.zero;
		}
		float num6 = num3 / (float)this.fragmentCount;
		bool flag = false;
		for (int i = 0; i < this.fragmentCount + 1; i++)
		{
			float num7 = num6 * (float)i;
			this.line.points[i] = start + Vector3.up * (verticleSpeed - magnitude * num7 * 0.5f) * num7 + vector3 * num5 * num7;
			Vector3 vector4 = this.line.points[i];
			if (i > 0 && i < this.line.points.Length - 1 && !flag)
			{
				Vector3 vector5 = this.line.points[i - 1];
				flag = this.CheckObsticle(vector5, vector4, ref hitPoint);
				hitPoint = vector5 + (vector4 - vector5).normalized * (hitPoint - vector5).magnitude;
			}
			if (flag)
			{
				this.line.colors[i] = this.obsticleColor;
			}
			else
			{
				this.line.colors[i] = this.lineColor;
			}
		}
		this.line.hitObsticle = flag;
		if (flag)
		{
			this.line.hitPoint = hitPoint;
		}
		this.line.DrawLine();
		return flag;
	}

	// Token: 0x06000652 RID: 1618 RVA: 0x0001CB34 File Offset: 0x0001AD34
	private bool CheckObsticle(Vector3 from, Vector3 to, ref Vector3 hitPoint)
	{
		if (this.hits == null)
		{
			this.hits = new RaycastHit[3];
		}
		if (Physics.SphereCastNonAlloc(from, 0.2f, (to - from).normalized, this.hits, (to - from).magnitude, this.obsticleLayers) > 0)
		{
			hitPoint = this.hits[0].point;
			return true;
		}
		return false;
	}

	// Token: 0x0400061A RID: 1562
	public ShapesSkillLine line;

	// Token: 0x0400061B RID: 1563
	public int fragmentCount = 20;

	// Token: 0x0400061C RID: 1564
	[ColorUsage(true, true)]
	public Color lineColor;

	// Token: 0x0400061D RID: 1565
	[ColorUsage(true, true)]
	public Color obsticleColor;

	// Token: 0x0400061E RID: 1566
	private LayerMask obsticleLayers;

	// Token: 0x0400061F RID: 1567
	private RaycastHit[] hits;
}
