using System;
using UnityEngine;

namespace Debugging
{
	// Token: 0x0200021D RID: 541
	public class InstantiateTiming : MonoBehaviour
	{
		// Token: 0x06001041 RID: 4161 RVA: 0x0003F0EB File Offset: 0x0003D2EB
		public void InstantiatePrefab()
		{
			Debug.Log("Start Instantiate");
			global::UnityEngine.Object.Instantiate<GameObject>(this.prefab);
			Debug.Log("Instantiated");
		}

		// Token: 0x06001042 RID: 4162 RVA: 0x0003F10D File Offset: 0x0003D30D
		private void Awake()
		{
			Debug.Log("Awake");
		}

		// Token: 0x06001043 RID: 4163 RVA: 0x0003F119 File Offset: 0x0003D319
		private void Start()
		{
			Debug.Log("Start");
		}

		// Token: 0x04000CF9 RID: 3321
		public GameObject prefab;
	}
}
