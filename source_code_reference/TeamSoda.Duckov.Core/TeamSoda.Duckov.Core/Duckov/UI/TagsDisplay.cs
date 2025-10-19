using System;
using Duckov.Utilities;
using ItemStatsSystem;
using UnityEngine;

namespace Duckov.UI
{
	// Token: 0x0200039C RID: 924
	public class TagsDisplay : MonoBehaviour
	{
		// Token: 0x17000652 RID: 1618
		// (get) Token: 0x060020EF RID: 8431 RVA: 0x00072F80 File Offset: 0x00071180
		private PrefabPool<TagsDisplayEntry> EntryPool
		{
			get
			{
				if (this._entryPool == null)
				{
					this._entryPool = new PrefabPool<TagsDisplayEntry>(this.entryTemplate, null, null, null, null, true, 10, 10000, null);
				}
				return this._entryPool;
			}
		}

		// Token: 0x060020F0 RID: 8432 RVA: 0x00072FB9 File Offset: 0x000711B9
		private void Awake()
		{
			this.entryTemplate.gameObject.SetActive(false);
		}

		// Token: 0x060020F1 RID: 8433 RVA: 0x00072FCC File Offset: 0x000711CC
		public void Setup(Item item)
		{
			this.EntryPool.ReleaseAll();
			if (item == null)
			{
				return;
			}
			foreach (Tag tag in item.Tags)
			{
				if (!(tag == null) && tag.Show)
				{
					this.EntryPool.Get(null).Setup(tag);
				}
			}
		}

		// Token: 0x060020F2 RID: 8434 RVA: 0x0007304C File Offset: 0x0007124C
		internal void Clear()
		{
			this.EntryPool.ReleaseAll();
		}

		// Token: 0x04001660 RID: 5728
		[SerializeField]
		private TagsDisplayEntry entryTemplate;

		// Token: 0x04001661 RID: 5729
		private PrefabPool<TagsDisplayEntry> _entryPool;
	}
}
