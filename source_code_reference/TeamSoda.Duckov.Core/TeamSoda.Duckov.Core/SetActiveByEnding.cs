using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200018A RID: 394
public class SetActiveByEnding : MonoBehaviour
{
	// Token: 0x06000BBB RID: 3003 RVA: 0x00031C73 File Offset: 0x0002FE73
	private void Start()
	{
		this.target.SetActive(this.endingIndexs.Contains(Ending.endingIndex));
	}

	// Token: 0x04000A14 RID: 2580
	public GameObject target;

	// Token: 0x04000A15 RID: 2581
	public List<int> endingIndexs;
}
