using System;
using Duckov.UI.Animations;
using Saves;
using UnityEngine;

// Token: 0x02000123 RID: 291
public class RestoreFailureDetectedIndicator : MonoBehaviour
{
	// Token: 0x0600098B RID: 2443 RVA: 0x0002978E File Offset: 0x0002798E
	private void OnEnable()
	{
		SavesSystem.OnRestoreFailureDetected += this.Refresh;
		SavesSystem.OnSetFile += this.Refresh;
		this.Refresh();
	}

	// Token: 0x0600098C RID: 2444 RVA: 0x000297B8 File Offset: 0x000279B8
	private void OnDisable()
	{
		SavesSystem.OnRestoreFailureDetected -= this.Refresh;
		SavesSystem.OnSetFile -= this.Refresh;
	}

	// Token: 0x0600098D RID: 2445 RVA: 0x000297DC File Offset: 0x000279DC
	private void Refresh()
	{
		if (SavesSystem.RestoreFailureMarker)
		{
			this.fadeGroup.Show();
			return;
		}
		this.fadeGroup.Hide();
	}

	// Token: 0x04000868 RID: 2152
	[SerializeField]
	private FadeGroup fadeGroup;
}
