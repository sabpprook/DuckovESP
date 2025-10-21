using System;
using Saves;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Duckov.UI.SavesRestore
{
	// Token: 0x020003E6 RID: 998
	public class SavesBackupRestorePanelEntry : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
	{
		// Token: 0x170006D6 RID: 1750
		// (get) Token: 0x0600240F RID: 9231 RVA: 0x0007D777 File Offset: 0x0007B977
		public SavesSystem.BackupInfo Info
		{
			get
			{
				return this.info;
			}
		}

		// Token: 0x06002410 RID: 9232 RVA: 0x0007D77F File Offset: 0x0007B97F
		public void OnPointerClick(PointerEventData eventData)
		{
			this.master.NotifyClicked(this);
		}

		// Token: 0x06002411 RID: 9233 RVA: 0x0007D790 File Offset: 0x0007B990
		internal void Setup(SavesBackupRestorePanel master, SavesSystem.BackupInfo info)
		{
			this.master = master;
			this.info = info;
			if (info.time_raw <= 0L)
			{
				this.timeText.text = "???";
				return;
			}
			this.timeText.text = info.Time.ToLocalTime().ToString("yyyy/MM/dd HH:mm:ss");
		}

		// Token: 0x04001885 RID: 6277
		[SerializeField]
		private TextMeshProUGUI timeText;

		// Token: 0x04001886 RID: 6278
		private SavesBackupRestorePanel master;

		// Token: 0x04001887 RID: 6279
		private SavesSystem.BackupInfo info;
	}
}
