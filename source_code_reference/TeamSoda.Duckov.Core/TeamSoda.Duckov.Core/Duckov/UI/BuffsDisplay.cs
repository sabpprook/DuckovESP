using System;
using System.Collections.Generic;
using Duckov.Buffs;
using Duckov.Utilities;
using UnityEngine;

namespace Duckov.UI
{
	// Token: 0x02000374 RID: 884
	public class BuffsDisplay : MonoBehaviour
	{
		// Token: 0x170005E8 RID: 1512
		// (get) Token: 0x06001E86 RID: 7814 RVA: 0x0006B4BC File Offset: 0x000696BC
		private PrefabPool<BuffsDisplayEntry> EntryPool
		{
			get
			{
				if (this._entryPool == null)
				{
					this._entryPool = new PrefabPool<BuffsDisplayEntry>(this.prefab, base.transform, delegate(BuffsDisplayEntry e)
					{
						this.activeEntries.Add(e);
					}, delegate(BuffsDisplayEntry e)
					{
						this.activeEntries.Remove(e);
					}, null, true, 10, 10000, null);
				}
				return this._entryPool;
			}
		}

		// Token: 0x06001E87 RID: 7815 RVA: 0x0006B510 File Offset: 0x00069710
		public void ReleaseEntry(BuffsDisplayEntry entry)
		{
			this.EntryPool.Release(entry);
		}

		// Token: 0x06001E88 RID: 7816 RVA: 0x0006B51E File Offset: 0x0006971E
		private void Awake()
		{
			LevelManager.OnLevelInitialized += this.OnLevelInitialized;
			if (LevelManager.LevelInited)
			{
				this.OnLevelInitialized();
			}
		}

		// Token: 0x06001E89 RID: 7817 RVA: 0x0006B53E File Offset: 0x0006973E
		private void OnDestroy()
		{
			this.UnregisterEvents();
			LevelManager.OnLevelInitialized -= this.OnLevelInitialized;
		}

		// Token: 0x06001E8A RID: 7818 RVA: 0x0006B558 File Offset: 0x00069758
		private void OnLevelInitialized()
		{
			this.UnregisterEvents();
			this.buffManager = LevelManager.Instance.MainCharacter.GetBuffManager();
			foreach (Buff buff in this.buffManager.Buffs)
			{
				this.OnAddBuff(this.buffManager, buff);
			}
			this.RegisterEvents();
		}

		// Token: 0x06001E8B RID: 7819 RVA: 0x0006B5D4 File Offset: 0x000697D4
		private void RegisterEvents()
		{
			if (this.buffManager == null)
			{
				return;
			}
			this.buffManager.onAddBuff += this.OnAddBuff;
			this.buffManager.onRemoveBuff += this.OnRemoveBuff;
		}

		// Token: 0x06001E8C RID: 7820 RVA: 0x0006B613 File Offset: 0x00069813
		private void UnregisterEvents()
		{
			if (this.buffManager == null)
			{
				return;
			}
			this.buffManager.onAddBuff -= this.OnAddBuff;
			this.buffManager.onRemoveBuff -= this.OnRemoveBuff;
		}

		// Token: 0x06001E8D RID: 7821 RVA: 0x0006B652 File Offset: 0x00069852
		private void OnAddBuff(CharacterBuffManager manager, Buff buff)
		{
			if (buff.Hide)
			{
				return;
			}
			this.EntryPool.Get(null).Setup(this, buff);
		}

		// Token: 0x06001E8E RID: 7822 RVA: 0x0006B670 File Offset: 0x00069870
		private void OnRemoveBuff(CharacterBuffManager manager, Buff buff)
		{
			BuffsDisplayEntry buffsDisplayEntry = this.activeEntries.Find((BuffsDisplayEntry e) => e.Target == buff);
			if (buffsDisplayEntry == null)
			{
				return;
			}
			buffsDisplayEntry.Release();
		}

		// Token: 0x040014E2 RID: 5346
		[SerializeField]
		private BuffsDisplayEntry prefab;

		// Token: 0x040014E3 RID: 5347
		private PrefabPool<BuffsDisplayEntry> _entryPool;

		// Token: 0x040014E4 RID: 5348
		private List<BuffsDisplayEntry> activeEntries = new List<BuffsDisplayEntry>();

		// Token: 0x040014E5 RID: 5349
		private CharacterBuffManager buffManager;
	}
}
