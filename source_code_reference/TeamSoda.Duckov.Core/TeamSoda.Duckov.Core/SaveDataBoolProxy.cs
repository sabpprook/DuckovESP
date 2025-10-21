using System;
using Saves;
using UnityEngine;

// Token: 0x020000AA RID: 170
public class SaveDataBoolProxy : MonoBehaviour
{
	// Token: 0x060005B4 RID: 1460 RVA: 0x000198AC File Offset: 0x00017AAC
	public void Save()
	{
		SavesSystem.Save<bool>(this.key, this.value);
		Debug.Log(string.Format("SetSaveData:{0} to {1}", this.key, this.value));
	}

	// Token: 0x04000536 RID: 1334
	public string key;

	// Token: 0x04000537 RID: 1335
	public bool value;
}
