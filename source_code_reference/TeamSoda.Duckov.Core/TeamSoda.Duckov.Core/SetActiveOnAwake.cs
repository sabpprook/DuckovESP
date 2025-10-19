using System;
using UnityEngine;

// Token: 0x0200014F RID: 335
public class SetActiveOnAwake : MonoBehaviour
{
	// Token: 0x06000A5F RID: 2655 RVA: 0x0002D9E8 File Offset: 0x0002BBE8
	private void Awake()
	{
		this.target.SetActive(true);
	}

	// Token: 0x04000916 RID: 2326
	public GameObject target;
}
