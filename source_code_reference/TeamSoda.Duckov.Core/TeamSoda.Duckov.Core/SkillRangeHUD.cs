using System;
using UnityEngine;

// Token: 0x020000CC RID: 204
public class SkillRangeHUD : MonoBehaviour
{
	// Token: 0x06000654 RID: 1620 RVA: 0x0001CBBC File Offset: 0x0001ADBC
	public void SetRange(float range)
	{
		this.rangeTarget.localScale = Vector3.one * range;
	}

	// Token: 0x06000655 RID: 1621 RVA: 0x0001CBD4 File Offset: 0x0001ADD4
	public void SetProgress(float progress)
	{
		if (this.rangeMat == null)
		{
			this.rangeMat = this.rangeRenderer.material;
		}
		if (this.rangeMat == null)
		{
			return;
		}
		this.rangeMat.SetFloat("_Progress", progress);
	}

	// Token: 0x04000620 RID: 1568
	public Transform rangeTarget;

	// Token: 0x04000621 RID: 1569
	public Renderer rangeRenderer;

	// Token: 0x04000622 RID: 1570
	private Material rangeMat;
}
