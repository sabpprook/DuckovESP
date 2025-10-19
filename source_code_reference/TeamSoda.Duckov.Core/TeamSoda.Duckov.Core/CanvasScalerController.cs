using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000152 RID: 338
public class CanvasScalerController : MonoBehaviour
{
	// Token: 0x06000A66 RID: 2662 RVA: 0x0002DAAD File Offset: 0x0002BCAD
	private void OnValidate()
	{
		if (this.canvasScaler == null)
		{
			this.canvasScaler = base.GetComponent<CanvasScaler>();
		}
	}

	// Token: 0x06000A67 RID: 2663 RVA: 0x0002DAC9 File Offset: 0x0002BCC9
	private void Awake()
	{
		this.OnValidate();
	}

	// Token: 0x06000A68 RID: 2664 RVA: 0x0002DAD1 File Offset: 0x0002BCD1
	private void OnEnable()
	{
		this.Refresh();
	}

	// Token: 0x06000A69 RID: 2665 RVA: 0x0002DADC File Offset: 0x0002BCDC
	private void Refresh()
	{
		if (this.canvasScaler == null)
		{
			return;
		}
		Vector2Int currentResolution = this.GetCurrentResolution();
		float num = (float)currentResolution.x / (float)currentResolution.y;
		Vector2 referenceResolution = this.canvasScaler.referenceResolution;
		float num2 = referenceResolution.x / referenceResolution.y;
		if (num > num2)
		{
			this.canvasScaler.matchWidthOrHeight = 1f;
		}
		else
		{
			this.canvasScaler.matchWidthOrHeight = 0f;
		}
		this.cachedResolution = currentResolution;
	}

	// Token: 0x06000A6A RID: 2666 RVA: 0x0002DB57 File Offset: 0x0002BD57
	private void FixedUpdate()
	{
		if (!this.ResolutionMatch())
		{
			this.Refresh();
		}
	}

	// Token: 0x06000A6B RID: 2667 RVA: 0x0002DB68 File Offset: 0x0002BD68
	private bool ResolutionMatch()
	{
		Vector2Int currentResolution = this.GetCurrentResolution();
		return this.cachedResolution.x == currentResolution.x && this.cachedResolution.y == currentResolution.y;
	}

	// Token: 0x06000A6C RID: 2668 RVA: 0x0002DBA6 File Offset: 0x0002BDA6
	private Vector2Int GetCurrentResolution()
	{
		return new Vector2Int(Display.main.renderingWidth, Display.main.renderingHeight);
	}

	// Token: 0x04000919 RID: 2329
	[SerializeField]
	private CanvasScaler canvasScaler;

	// Token: 0x0400091A RID: 2330
	private Vector2Int cachedResolution;
}
