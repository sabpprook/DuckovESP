using System;
using System.Collections.Generic;
using Duckov.MiniGames.GoldMiner.UI;
using UnityEngine;

namespace Duckov.MiniGames.GoldMiner
{
	// Token: 0x020002A2 RID: 674
	public class NavGroup : MiniGameBehaviour
	{
		// Token: 0x17000407 RID: 1031
		// (get) Token: 0x060015E3 RID: 5603 RVA: 0x00050E83 File Offset: 0x0004F083
		// (set) Token: 0x060015E4 RID: 5604 RVA: 0x00050E8A File Offset: 0x0004F08A
		public static NavGroup ActiveNavGroup { get; private set; }

		// Token: 0x17000408 RID: 1032
		// (get) Token: 0x060015E5 RID: 5605 RVA: 0x00050E92 File Offset: 0x0004F092
		public bool active
		{
			get
			{
				return NavGroup.ActiveNavGroup == this;
			}
		}

		// Token: 0x060015E6 RID: 5606 RVA: 0x00050EA0 File Offset: 0x0004F0A0
		public void SetAsActiveNavGroup()
		{
			NavGroup activeNavGroup = NavGroup.ActiveNavGroup;
			NavGroup.ActiveNavGroup = this;
			this.RefreshAll();
			if (activeNavGroup != null)
			{
				activeNavGroup.RefreshAll();
			}
			Action onNavGroupChanged = NavGroup.OnNavGroupChanged;
			if (onNavGroupChanged == null)
			{
				return;
			}
			onNavGroupChanged();
		}

		// Token: 0x17000409 RID: 1033
		// (get) Token: 0x060015E7 RID: 5607 RVA: 0x00050EDD File Offset: 0x0004F0DD
		// (set) Token: 0x060015E8 RID: 5608 RVA: 0x00050EE8 File Offset: 0x0004F0E8
		public int NavIndex
		{
			get
			{
				return this._navIndex;
			}
			set
			{
				int navIndex = this._navIndex;
				this._navIndex = value;
				this.CleanupIndex();
				int navIndex2 = this._navIndex;
				this.RefreshEntry(navIndex);
				this.RefreshEntry(navIndex2);
			}
		}

		// Token: 0x060015E9 RID: 5609 RVA: 0x00050F1E File Offset: 0x0004F11E
		protected override void OnEnable()
		{
			base.OnEnable();
			this.RefreshAll();
		}

		// Token: 0x060015EA RID: 5610 RVA: 0x00050F2C File Offset: 0x0004F12C
		private void CleanupIndex()
		{
			if (this._navIndex < 0)
			{
				this._navIndex = this.entries.Count - 1;
			}
			if (this._navIndex >= this.entries.Count)
			{
				this._navIndex = 0;
			}
		}

		// Token: 0x060015EB RID: 5611 RVA: 0x00050F64 File Offset: 0x0004F164
		private void RefreshAll()
		{
			for (int i = 0; i < this.entries.Count; i++)
			{
				this.RefreshEntry(i);
			}
		}

		// Token: 0x060015EC RID: 5612 RVA: 0x00050F8E File Offset: 0x0004F18E
		private void RefreshEntry(int index)
		{
			if (index < 0 || index >= this.entries.Count)
			{
				return;
			}
			this.entries[index].NotifySelectionState(this.active && this.NavIndex == index);
		}

		// Token: 0x060015ED RID: 5613 RVA: 0x00050FC8 File Offset: 0x0004F1C8
		public NavEntry GetSelectedEntry()
		{
			if (this.NavIndex < 0 || this.NavIndex >= this.entries.Count)
			{
				return null;
			}
			return this.entries[this.NavIndex];
		}

		// Token: 0x060015EE RID: 5614 RVA: 0x00050FFC File Offset: 0x0004F1FC
		private void Awake()
		{
			if (this.master == null)
			{
				this.master = base.GetComponentInParent<GoldMiner>();
			}
			GoldMiner goldMiner = this.master;
			goldMiner.onLevelBegin = (Action<GoldMiner>)Delegate.Combine(goldMiner.onLevelBegin, new Action<GoldMiner>(this.OnLevelBegin));
		}

		// Token: 0x060015EF RID: 5615 RVA: 0x0005104A File Offset: 0x0004F24A
		private void OnLevelBegin(GoldMiner miner)
		{
			this.RefreshAll();
		}

		// Token: 0x060015F0 RID: 5616 RVA: 0x00051052 File Offset: 0x0004F252
		internal void Remove(NavEntry navEntry)
		{
			this.entries.Remove(navEntry);
			this.CleanupIndex();
			this.RefreshAll();
		}

		// Token: 0x060015F1 RID: 5617 RVA: 0x0005106D File Offset: 0x0004F26D
		internal void Add(NavEntry navEntry)
		{
			this.entries.Add(navEntry);
			this.CleanupIndex();
			this.RefreshAll();
		}

		// Token: 0x060015F2 RID: 5618 RVA: 0x00051088 File Offset: 0x0004F288
		internal void TrySelect(NavEntry navEntry)
		{
			if (!this.entries.Contains(navEntry))
			{
				return;
			}
			int num = this.entries.IndexOf(navEntry);
			this.SetAsActiveNavGroup();
			this.NavIndex = num;
		}

		// Token: 0x04001047 RID: 4167
		[SerializeField]
		private GoldMiner master;

		// Token: 0x04001048 RID: 4168
		[SerializeField]
		public List<NavEntry> entries;

		// Token: 0x0400104A RID: 4170
		public static Action OnNavGroupChanged;

		// Token: 0x0400104B RID: 4171
		private int _navIndex;
	}
}
