using System;
using Saves;
using UnityEngine;

// Token: 0x020000DF RID: 223
public class SetDoorOpenIfSaveData : MonoBehaviour
{
	// Token: 0x0600071C RID: 1820 RVA: 0x0001FF56 File Offset: 0x0001E156
	private void Start()
	{
		if (LevelManager.LevelInited)
		{
			this.OnSet();
			return;
		}
		LevelManager.OnLevelInitialized += this.OnSet;
	}

	// Token: 0x0600071D RID: 1821 RVA: 0x0001FF77 File Offset: 0x0001E177
	private void OnDestroy()
	{
		LevelManager.OnLevelInitialized -= this.OnSet;
	}

	// Token: 0x0600071E RID: 1822 RVA: 0x0001FF8C File Offset: 0x0001E18C
	private void OnSet()
	{
		bool flag = SavesSystem.Load<bool>(this.key);
		Debug.Log(string.Format("Load door data:{0}  {1}", this.key, flag));
		this.door.ForceSetClosed(flag != this.openIfDataTure, false);
	}

	// Token: 0x040006C2 RID: 1730
	public Door door;

	// Token: 0x040006C3 RID: 1731
	public string key;

	// Token: 0x040006C4 RID: 1732
	public bool openIfDataTure = true;
}
