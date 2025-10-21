using System;
using UnityEngine;

// Token: 0x0200010B RID: 267
public class PlayerPositionBackupProxy : MonoBehaviour
{
	// Token: 0x06000926 RID: 2342 RVA: 0x00028A56 File Offset: 0x00026C56
	public void StartRecoverInteract()
	{
		PauseMenu.Instance.Close();
		PlayerPositionBackupManager.StartRecover();
	}
}
