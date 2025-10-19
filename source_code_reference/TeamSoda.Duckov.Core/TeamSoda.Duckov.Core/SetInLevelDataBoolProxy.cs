using System;
using Duckov.Scenes;
using UnityEngine;

// Token: 0x020000A5 RID: 165
public class SetInLevelDataBoolProxy : MonoBehaviour
{
	// Token: 0x060005A1 RID: 1441 RVA: 0x000193B9 File Offset: 0x000175B9
	public void SetToTarget()
	{
		this.SetTo(this.targetValue);
	}

	// Token: 0x060005A2 RID: 1442 RVA: 0x000193C8 File Offset: 0x000175C8
	public void SetTo(bool target)
	{
		if (this.keyString == "")
		{
			return;
		}
		if (!this.keyInited)
		{
			this.InitKey();
		}
		if (MultiSceneCore.Instance)
		{
			MultiSceneCore.Instance.inLevelData[this.keyHash] = target;
		}
	}

	// Token: 0x060005A3 RID: 1443 RVA: 0x0001941D File Offset: 0x0001761D
	private void InitKey()
	{
		this.keyHash = this.keyString.GetHashCode();
		this.keyInited = true;
	}

	// Token: 0x04000520 RID: 1312
	public bool targetValue = true;

	// Token: 0x04000521 RID: 1313
	public string keyString = "";

	// Token: 0x04000522 RID: 1314
	private int keyHash;

	// Token: 0x04000523 RID: 1315
	private bool keyInited;
}
