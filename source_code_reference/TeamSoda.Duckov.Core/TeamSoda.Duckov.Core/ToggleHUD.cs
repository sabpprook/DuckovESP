using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000150 RID: 336
public class ToggleHUD : MonoBehaviour
{
	// Token: 0x06000A61 RID: 2657 RVA: 0x0002DA00 File Offset: 0x0002BC00
	private void Awake()
	{
		foreach (GameObject gameObject in this.toggleTargets)
		{
			if (gameObject != null && !gameObject.activeInHierarchy)
			{
				gameObject.SetActive(true);
			}
		}
	}

	// Token: 0x04000917 RID: 2327
	public List<GameObject> toggleTargets;
}
