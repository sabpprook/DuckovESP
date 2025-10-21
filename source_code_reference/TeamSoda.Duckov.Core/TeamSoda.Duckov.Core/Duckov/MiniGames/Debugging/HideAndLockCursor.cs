using System;
using UnityEngine;

namespace Duckov.MiniGames.Debugging
{
	// Token: 0x020002CA RID: 714
	public class HideAndLockCursor : MonoBehaviour
	{
		// Token: 0x0600168A RID: 5770 RVA: 0x00052A07 File Offset: 0x00050C07
		private void OnEnable()
		{
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
		}

		// Token: 0x0600168B RID: 5771 RVA: 0x00052A15 File Offset: 0x00050C15
		private void OnDisable()
		{
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
		}

		// Token: 0x0600168C RID: 5772 RVA: 0x00052A23 File Offset: 0x00050C23
		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				base.enabled = false;
			}
		}
	}
}
