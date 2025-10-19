using System;
using Duckov.UI.Animations;
using Duckov.UI.SavesRestore;
using Saves;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x02000167 RID: 359
public class SavesBackupRestoreInvoker : MonoBehaviour
{
	// Token: 0x06000AE3 RID: 2787 RVA: 0x0002EB38 File Offset: 0x0002CD38
	private void Awake()
	{
		this.mainButton.onClick.AddListener(new UnityAction(this.OnMainButtonClicked));
		this.buttonSlot1.onClick.AddListener(delegate
		{
			this.OnButtonClicked(1);
		});
		this.buttonSlot2.onClick.AddListener(delegate
		{
			this.OnButtonClicked(2);
		});
		this.buttonSlot3.onClick.AddListener(delegate
		{
			this.OnButtonClicked(3);
		});
	}

	// Token: 0x06000AE4 RID: 2788 RVA: 0x0002EBB5 File Offset: 0x0002CDB5
	private void OnMainButtonClicked()
	{
		this.menuFadeGroup.Toggle();
	}

	// Token: 0x06000AE5 RID: 2789 RVA: 0x0002EBC2 File Offset: 0x0002CDC2
	private void OnButtonClicked(int index)
	{
		this.menuFadeGroup.Hide();
		SavesSystem.SetFile(index);
		this.restorePanel.Open(index);
	}

	// Token: 0x04000961 RID: 2401
	[SerializeField]
	private Button mainButton;

	// Token: 0x04000962 RID: 2402
	[SerializeField]
	private FadeGroup menuFadeGroup;

	// Token: 0x04000963 RID: 2403
	[SerializeField]
	private Button buttonSlot1;

	// Token: 0x04000964 RID: 2404
	[SerializeField]
	private Button buttonSlot2;

	// Token: 0x04000965 RID: 2405
	[SerializeField]
	private Button buttonSlot3;

	// Token: 0x04000966 RID: 2406
	[SerializeField]
	private SavesBackupRestorePanel restorePanel;
}
