using System;
using Duckov.Utilities;
using UnityEngine;

namespace Duckov.UI
{
	// Token: 0x020003A8 RID: 936
	public class ItemShortcutEditorPanel : MonoBehaviour
	{
		// Token: 0x17000670 RID: 1648
		// (get) Token: 0x060021A0 RID: 8608 RVA: 0x00075414 File Offset: 0x00073614
		private PrefabPool<ItemShortcutEditorEntry> EntryPool
		{
			get
			{
				if (this._entryPool == null)
				{
					this._entryPool = new PrefabPool<ItemShortcutEditorEntry>(this.entryTemplate, this.entryTemplate.transform.parent, null, null, null, true, 10, 10000, null);
					this.entryTemplate.gameObject.SetActive(false);
				}
				return this._entryPool;
			}
		}

		// Token: 0x060021A1 RID: 8609 RVA: 0x0007546D File Offset: 0x0007366D
		private void OnEnable()
		{
			this.Setup();
		}

		// Token: 0x060021A2 RID: 8610 RVA: 0x00075478 File Offset: 0x00073678
		private void Setup()
		{
			this.EntryPool.ReleaseAll();
			for (int i = 0; i <= ItemShortcut.MaxIndex; i++)
			{
				ItemShortcutEditorEntry itemShortcutEditorEntry = this.EntryPool.Get(this.entryTemplate.transform.parent);
				itemShortcutEditorEntry.Setup(i);
				itemShortcutEditorEntry.transform.SetAsLastSibling();
			}
		}

		// Token: 0x040016BD RID: 5821
		[SerializeField]
		private ItemShortcutEditorEntry entryTemplate;

		// Token: 0x040016BE RID: 5822
		private PrefabPool<ItemShortcutEditorEntry> _entryPool;
	}
}
