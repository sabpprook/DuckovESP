using System;
using Duckov.Utilities;
using UnityEngine;

namespace Duckov.MiniGames.GoldMiner
{
	// Token: 0x0200029F RID: 671
	public class GoldMiner_PopText : MiniGameBehaviour
	{
		// Token: 0x17000406 RID: 1030
		// (get) Token: 0x060015D2 RID: 5586 RVA: 0x00050B8C File Offset: 0x0004ED8C
		private PrefabPool<GoldMiner_PopTextEntry> TextPool
		{
			get
			{
				if (this._textPool == null)
				{
					this._textPool = new PrefabPool<GoldMiner_PopTextEntry>(this.textTemplate, null, null, null, null, true, 10, 10000, null);
				}
				return this._textPool;
			}
		}

		// Token: 0x060015D3 RID: 5587 RVA: 0x00050BC5 File Offset: 0x0004EDC5
		public void Pop(string content, Vector3 position)
		{
			this.TextPool.Get(null).Setup(position, content, new Action<GoldMiner_PopTextEntry>(this.ReleaseEntry));
		}

		// Token: 0x060015D4 RID: 5588 RVA: 0x00050BE6 File Offset: 0x0004EDE6
		private void ReleaseEntry(GoldMiner_PopTextEntry entry)
		{
			this.TextPool.Release(entry);
		}

		// Token: 0x0400102F RID: 4143
		[SerializeField]
		private GoldMiner_PopTextEntry textTemplate;

		// Token: 0x04001030 RID: 4144
		private PrefabPool<GoldMiner_PopTextEntry> _textPool;
	}
}
