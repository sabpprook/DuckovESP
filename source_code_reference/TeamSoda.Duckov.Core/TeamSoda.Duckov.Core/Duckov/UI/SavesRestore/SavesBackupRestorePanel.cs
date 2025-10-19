using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Duckov.UI.Animations;
using Duckov.Utilities;
using Saves;
using TMPro;
using UnityEngine;

namespace Duckov.UI.SavesRestore
{
	// Token: 0x020003E5 RID: 997
	public class SavesBackupRestorePanel : MonoBehaviour
	{
		// Token: 0x170006D5 RID: 1749
		// (get) Token: 0x06002405 RID: 9221 RVA: 0x0007D5E0 File Offset: 0x0007B7E0
		private PrefabPool<SavesBackupRestorePanelEntry> Pool
		{
			get
			{
				if (this._pool == null)
				{
					this._pool = new PrefabPool<SavesBackupRestorePanelEntry>(this.template, null, null, null, null, true, 10, 10000, null);
				}
				return this._pool;
			}
		}

		// Token: 0x06002406 RID: 9222 RVA: 0x0007D619 File Offset: 0x0007B819
		private void Awake()
		{
		}

		// Token: 0x06002407 RID: 9223 RVA: 0x0007D61B File Offset: 0x0007B81B
		public void Open(int savesSlot)
		{
			this.slot = savesSlot;
			this.Refresh();
			this.fadeGroup.Show();
		}

		// Token: 0x06002408 RID: 9224 RVA: 0x0007D635 File Offset: 0x0007B835
		public void Close()
		{
			this.fadeGroup.Hide();
		}

		// Token: 0x06002409 RID: 9225 RVA: 0x0007D642 File Offset: 0x0007B842
		public void Confirm()
		{
			this.confirm = true;
		}

		// Token: 0x0600240A RID: 9226 RVA: 0x0007D64B File Offset: 0x0007B84B
		public void Cancel()
		{
			this.cancel = true;
		}

		// Token: 0x0600240B RID: 9227 RVA: 0x0007D654 File Offset: 0x0007B854
		private void Refresh()
		{
			this.Pool.ReleaseAll();
			List<SavesSystem.BackupInfo> list = SavesSystem.GetBackupList(this.slot).ToList<SavesSystem.BackupInfo>();
			list.Sort(delegate(SavesSystem.BackupInfo a, SavesSystem.BackupInfo b)
			{
				if (a.Time < b.Time)
				{
					return 1;
				}
				return -1;
			});
			int num = 0;
			for (int i = 0; i < list.Count; i++)
			{
				SavesSystem.BackupInfo backupInfo = list[i];
				if (backupInfo.exists)
				{
					this.Pool.Get(null).Setup(this, backupInfo);
					num++;
				}
			}
			this.noBackupIndicator.SetActive(num <= 0);
		}

		// Token: 0x0600240C RID: 9228 RVA: 0x0007D6F0 File Offset: 0x0007B8F0
		internal void NotifyClicked(SavesBackupRestorePanelEntry button)
		{
			if (this.recovering)
			{
				return;
			}
			SavesSystem.BackupInfo info = button.Info;
			if (!info.exists)
			{
				return;
			}
			this.RecoverTask(info).Forget();
		}

		// Token: 0x0600240D RID: 9229 RVA: 0x0007D724 File Offset: 0x0007B924
		private async UniTask RecoverTask(SavesSystem.BackupInfo info)
		{
			this.recovering = true;
			this.confirm = false;
			this.cancel = false;
			TextMeshProUGUI[] array = this.slotIndexTexts;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].text = string.Format("{0}", info.slot);
			}
			foreach (TextMeshProUGUI textMeshProUGUI in this.backupTimeTexts)
			{
				if (info.time_raw <= 0L)
				{
					textMeshProUGUI.text = "???";
				}
				textMeshProUGUI.text = info.Time.ToLocalTime().ToString("yyyy/MM/dd HH:mm");
			}
			this.confirmFadeGroup.Show();
			while (!this.confirm && !this.cancel)
			{
				await UniTask.Yield();
			}
			if (this.cancel)
			{
				this.confirmFadeGroup.Hide();
				this.recovering = false;
			}
			else
			{
				SavesSystem.RestoreIndexedBackup(info.slot, info.index);
				this.confirmFadeGroup.Hide();
				this.confirm = false;
				this.resultFadeGroup.Show();
				while (!this.confirm)
				{
					await UniTask.Yield();
				}
				this.confirmFadeGroup.Hide();
				this.resultFadeGroup.Hide();
				this.recovering = false;
				this.Close();
			}
		}

		// Token: 0x04001879 RID: 6265
		[SerializeField]
		private FadeGroup fadeGroup;

		// Token: 0x0400187A RID: 6266
		[SerializeField]
		private FadeGroup confirmFadeGroup;

		// Token: 0x0400187B RID: 6267
		[SerializeField]
		private FadeGroup resultFadeGroup;

		// Token: 0x0400187C RID: 6268
		[SerializeField]
		private TextMeshProUGUI[] slotIndexTexts;

		// Token: 0x0400187D RID: 6269
		[SerializeField]
		private TextMeshProUGUI[] backupTimeTexts;

		// Token: 0x0400187E RID: 6270
		[SerializeField]
		private SavesBackupRestorePanelEntry template;

		// Token: 0x0400187F RID: 6271
		[SerializeField]
		private GameObject noBackupIndicator;

		// Token: 0x04001880 RID: 6272
		private PrefabPool<SavesBackupRestorePanelEntry> _pool;

		// Token: 0x04001881 RID: 6273
		private int slot;

		// Token: 0x04001882 RID: 6274
		private bool recovering;

		// Token: 0x04001883 RID: 6275
		private bool confirm;

		// Token: 0x04001884 RID: 6276
		private bool cancel;
	}
}
