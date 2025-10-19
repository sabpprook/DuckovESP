using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;

// Token: 0x020001C3 RID: 451
public class OpenSaveFolder : MonoBehaviour
{
	// Token: 0x1700027C RID: 636
	// (get) Token: 0x06000D6F RID: 3439 RVA: 0x00037711 File Offset: 0x00035911
	private string filePath
	{
		get
		{
			return Application.persistentDataPath;
		}
	}

	// Token: 0x06000D70 RID: 3440 RVA: 0x00037718 File Offset: 0x00035918
	private void Update()
	{
		if (Keyboard.current.leftCtrlKey.isPressed && Keyboard.current.lKey.isPressed)
		{
			this.OpenFolder();
		}
	}

	// Token: 0x06000D71 RID: 3441 RVA: 0x00037742 File Offset: 0x00035942
	public void OpenFolder()
	{
		Process.Start(new ProcessStartInfo
		{
			FileName = this.filePath,
			UseShellExecute = true
		});
	}
}
