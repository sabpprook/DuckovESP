using System;
using LeTai.TrueShadow;
using LeTai.TrueShadow.PluginInterfaces;
using UnityEngine;
using UnityEngine.UI.ProceduralImage;

// Token: 0x02000204 RID: 516
[ExecuteInEditMode]
public class ProceduralImageHashProvider : MonoBehaviour, ITrueShadowCustomHashProvider
{
	// Token: 0x06000F14 RID: 3860 RVA: 0x0003B8ED File Offset: 0x00039AED
	private void Awake()
	{
		if (this.trueShadow == null)
		{
			this.trueShadow = base.GetComponent<TrueShadow>();
		}
		if (this.proceduralImage == null)
		{
			this.proceduralImage = base.GetComponent<ProceduralImage>();
		}
	}

	// Token: 0x06000F15 RID: 3861 RVA: 0x0003B923 File Offset: 0x00039B23
	private void Refresh()
	{
		this.trueShadow.CustomHash = this.Hash();
	}

	// Token: 0x06000F16 RID: 3862 RVA: 0x0003B936 File Offset: 0x00039B36
	private void OnValidate()
	{
		if (this.trueShadow == null)
		{
			this.trueShadow = base.GetComponent<TrueShadow>();
		}
		if (this.proceduralImage == null)
		{
			this.proceduralImage = base.GetComponent<ProceduralImage>();
		}
		this.Refresh();
	}

	// Token: 0x06000F17 RID: 3863 RVA: 0x0003B972 File Offset: 0x00039B72
	private void OnRectTransformDimensionsChange()
	{
		this.Refresh();
	}

	// Token: 0x06000F18 RID: 3864 RVA: 0x0003B97C File Offset: 0x00039B7C
	private int Hash()
	{
		return this.proceduralImage.InfoCache.GetHashCode() + this.proceduralImage.color.GetHashCode();
	}

	// Token: 0x04000C57 RID: 3159
	[SerializeField]
	private ProceduralImage proceduralImage;

	// Token: 0x04000C58 RID: 3160
	[SerializeField]
	private TrueShadow trueShadow;
}
