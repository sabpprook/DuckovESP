using System;
using Duckov.Utilities;
using UnityEngine;

namespace Duckov.MasterKeys.UI
{
	// Token: 0x020002DE RID: 734
	public class MasterKeysIndexList : MonoBehaviour, ISingleSelectionMenu<MasterKeysIndexEntry>
	{
		// Token: 0x17000437 RID: 1079
		// (get) Token: 0x0600176C RID: 5996 RVA: 0x00055EEC File Offset: 0x000540EC
		private PrefabPool<MasterKeysIndexEntry> Pool
		{
			get
			{
				if (this._pool == null)
				{
					this._pool = new PrefabPool<MasterKeysIndexEntry>(this.entryPrefab, this.entryContainer, new Action<MasterKeysIndexEntry>(this.OnGetEntry), new Action<MasterKeysIndexEntry>(this.OnReleaseEntry), null, true, 10, 10000, null);
				}
				return this._pool;
			}
		}

		// Token: 0x14000098 RID: 152
		// (add) Token: 0x0600176D RID: 5997 RVA: 0x00055F40 File Offset: 0x00054140
		// (remove) Token: 0x0600176E RID: 5998 RVA: 0x00055F78 File Offset: 0x00054178
		internal event Action<MasterKeysIndexEntry> onEntryPointerClicked;

		// Token: 0x0600176F RID: 5999 RVA: 0x00055FAD File Offset: 0x000541AD
		private void OnGetEntry(MasterKeysIndexEntry entry)
		{
			entry.onPointerClicked += this.OnEntryPointerClicked;
		}

		// Token: 0x06001770 RID: 6000 RVA: 0x00055FC1 File Offset: 0x000541C1
		private void OnReleaseEntry(MasterKeysIndexEntry entry)
		{
			entry.onPointerClicked -= this.OnEntryPointerClicked;
		}

		// Token: 0x06001771 RID: 6001 RVA: 0x00055FD5 File Offset: 0x000541D5
		private void OnEntryPointerClicked(MasterKeysIndexEntry entry)
		{
			Action<MasterKeysIndexEntry> action = this.onEntryPointerClicked;
			if (action == null)
			{
				return;
			}
			action(entry);
		}

		// Token: 0x06001772 RID: 6002 RVA: 0x00055FE8 File Offset: 0x000541E8
		private void Awake()
		{
			this.entryPrefab.gameObject.SetActive(false);
		}

		// Token: 0x06001773 RID: 6003 RVA: 0x00055FFC File Offset: 0x000541FC
		internal void Refresh()
		{
			this.Pool.ReleaseAll();
			foreach (int num in MasterKeysManager.AllPossibleKeys)
			{
				this.Populate(num);
			}
		}

		// Token: 0x06001774 RID: 6004 RVA: 0x0005605C File Offset: 0x0005425C
		private void Populate(int itemID)
		{
			MasterKeysIndexEntry masterKeysIndexEntry = this.Pool.Get(this.entryContainer);
			masterKeysIndexEntry.gameObject.SetActive(true);
			masterKeysIndexEntry.Setup(itemID, this);
		}

		// Token: 0x06001775 RID: 6005 RVA: 0x00056082 File Offset: 0x00054282
		public MasterKeysIndexEntry GetSelection()
		{
			return this.selection;
		}

		// Token: 0x06001776 RID: 6006 RVA: 0x0005608A File Offset: 0x0005428A
		public bool SetSelection(MasterKeysIndexEntry selection)
		{
			this.selection = selection;
			return true;
		}

		// Token: 0x04001124 RID: 4388
		[SerializeField]
		private MasterKeysIndexEntry entryPrefab;

		// Token: 0x04001125 RID: 4389
		[SerializeField]
		private RectTransform entryContainer;

		// Token: 0x04001126 RID: 4390
		private PrefabPool<MasterKeysIndexEntry> _pool;

		// Token: 0x04001128 RID: 4392
		private MasterKeysIndexEntry selection;
	}
}
