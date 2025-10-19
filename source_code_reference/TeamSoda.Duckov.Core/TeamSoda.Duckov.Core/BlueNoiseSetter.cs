using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200013A RID: 314
public class BlueNoiseSetter : MonoBehaviour
{
	// Token: 0x06000A10 RID: 2576 RVA: 0x0002B130 File Offset: 0x00029330
	private void Update()
	{
		Shader.SetGlobalTexture("GlobalBlueNoise", this.blueNoises[this.index]);
		this.index++;
		if (this.index >= this.blueNoises.Count)
		{
			this.index = 0;
		}
	}

	// Token: 0x040008CC RID: 2252
	public List<Texture2D> blueNoises;

	// Token: 0x040008CD RID: 2253
	private int index;
}
