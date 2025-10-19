using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000137 RID: 311
public class MapImageToShader : MonoBehaviour
{
	// Token: 0x06000A05 RID: 2565 RVA: 0x0002AEA4 File Offset: 0x000290A4
	private void Start()
	{
	}

	// Token: 0x06000A06 RID: 2566 RVA: 0x0002AEA8 File Offset: 0x000290A8
	private void Update()
	{
		if (!this.material)
		{
			this.material = base.GetComponent<Image>().material;
		}
		if (!this.material)
		{
			return;
		}
		Rect rect = this.rect.rect;
		Vector3 vector = rect.min;
		Vector3 vector2 = rect.max;
		Vector3 vector3 = new Vector3(vector.x, vector.y);
		Vector3 vector4 = new Vector3(vector.x, vector2.y);
		Vector3 vector5 = new Vector3(vector2.x, vector.y);
		Vector3 vector6 = base.transform.TransformPoint(vector3);
		Vector3 vector7 = base.transform.TransformVector(vector4 - vector3);
		Vector3 vector8 = base.transform.TransformVector(vector5 - vector3);
		this.material.SetVector(this.zeroPointID, vector6);
		this.material.SetVector(this.upVectorID, vector7);
		this.material.SetVector(this.rightVectorID, vector8);
		this.material.SetFloat(this.scaleID, this.rect.lossyScale.x);
	}

	// Token: 0x040008C2 RID: 2242
	public RectTransform rect;

	// Token: 0x040008C3 RID: 2243
	private Material material;

	// Token: 0x040008C4 RID: 2244
	private int zeroPointID = Shader.PropertyToID("_ZeroPoint");

	// Token: 0x040008C5 RID: 2245
	private int upVectorID = Shader.PropertyToID("_UpVector");

	// Token: 0x040008C6 RID: 2246
	private int rightVectorID = Shader.PropertyToID("_RightVector");

	// Token: 0x040008C7 RID: 2247
	private int scaleID = Shader.PropertyToID("_Scale");
}
