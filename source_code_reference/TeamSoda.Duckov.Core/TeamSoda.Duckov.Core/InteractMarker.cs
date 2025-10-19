using System;
using UnityEngine;

// Token: 0x020000DC RID: 220
public class InteractMarker : MonoBehaviour
{
	// Token: 0x06000706 RID: 1798 RVA: 0x0001FB1C File Offset: 0x0001DD1C
	public void MarkAsUsed()
	{
		if (this.markedAsUsed)
		{
			return;
		}
		this.markedAsUsed = true;
		if (this.hideIfUsedObject)
		{
			this.hideIfUsedObject.SetActive(false);
		}
		if (this.showIfUsedObject)
		{
			this.showIfUsedObject.SetActive(true);
		}
	}

	// Token: 0x040006AC RID: 1708
	private bool markedAsUsed;

	// Token: 0x040006AD RID: 1709
	public GameObject showIfUsedObject;

	// Token: 0x040006AE RID: 1710
	public GameObject hideIfUsedObject;
}
