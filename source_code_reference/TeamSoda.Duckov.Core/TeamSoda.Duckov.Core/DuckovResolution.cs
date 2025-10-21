using System;
using UnityEngine;

// Token: 0x020001CB RID: 459
[Serializable]
public struct DuckovResolution
{
	// Token: 0x06000D9E RID: 3486 RVA: 0x00037EA8 File Offset: 0x000360A8
	public override bool Equals(object obj)
	{
		if (obj is DuckovResolution)
		{
			DuckovResolution duckovResolution = (DuckovResolution)obj;
			if (duckovResolution.height == this.height && duckovResolution.width == this.width)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000D9F RID: 3487 RVA: 0x00037EE3 File Offset: 0x000360E3
	public override string ToString()
	{
		return string.Format("{0} x {1}", this.width, this.height);
	}

	// Token: 0x06000DA0 RID: 3488 RVA: 0x00037F05 File Offset: 0x00036105
	public DuckovResolution(Resolution res)
	{
		this.height = res.height;
		this.width = res.width;
	}

	// Token: 0x06000DA1 RID: 3489 RVA: 0x00037F21 File Offset: 0x00036121
	public DuckovResolution(int _width, int _height)
	{
		this.height = _height;
		this.width = _width;
	}

	// Token: 0x06000DA2 RID: 3490 RVA: 0x00037F34 File Offset: 0x00036134
	public bool CheckRotioFit(DuckovResolution newRes, DuckovResolution defaultRes)
	{
		float num = (float)newRes.height / (float)newRes.width;
		return Mathf.Abs((float)defaultRes.height - num * (float)defaultRes.width) <= 2f;
	}

	// Token: 0x04000B89 RID: 2953
	public int width;

	// Token: 0x04000B8A RID: 2954
	public int height;
}
