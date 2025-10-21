using System;
using Duckov.Utilities;
using ItemStatsSystem;
using UnityEngine;

namespace Duckov.UI
{
	// Token: 0x02000394 RID: 916
	public class UsageUtilitiesDisplay : MonoBehaviour
	{
		// Token: 0x1700062B RID: 1579
		// (get) Token: 0x06002030 RID: 8240 RVA: 0x0007062B File Offset: 0x0006E82B
		// (set) Token: 0x06002031 RID: 8241 RVA: 0x00070633 File Offset: 0x0006E833
		public UsageUtilities Target { get; private set; }

		// Token: 0x1700062C RID: 1580
		// (get) Token: 0x06002032 RID: 8242 RVA: 0x0007063C File Offset: 0x0006E83C
		private PrefabPool<UsageUtilitiesDisplay_Entry> EntryPool
		{
			get
			{
				if (this._entryPool == null)
				{
					this._entryPool = new PrefabPool<UsageUtilitiesDisplay_Entry>(this.entryTemplate, null, null, null, null, true, 10, 10000, null);
				}
				return this._entryPool;
			}
		}

		// Token: 0x06002033 RID: 8243 RVA: 0x00070678 File Offset: 0x0006E878
		public void Setup(Item item)
		{
			if (!(item == null))
			{
				UsageUtilities component = item.GetComponent<UsageUtilities>();
				if (!(component == null))
				{
					this.Target = component;
					base.gameObject.SetActive(true);
					this.Refresh();
					return;
				}
			}
			base.gameObject.SetActive(false);
		}

		// Token: 0x06002034 RID: 8244 RVA: 0x000706C4 File Offset: 0x0006E8C4
		private void Refresh()
		{
			this.EntryPool.ReleaseAll();
			foreach (UsageBehavior usageBehavior in this.Target.behaviors)
			{
				if (!(usageBehavior == null) && usageBehavior.DisplaySettings.display)
				{
					this.EntryPool.Get(null).Setup(usageBehavior);
				}
			}
			if (this.EntryPool.ActiveEntries.Count <= 0)
			{
				base.gameObject.SetActive(false);
			}
		}

		// Token: 0x040015F6 RID: 5622
		[SerializeField]
		private UsageUtilitiesDisplay_Entry entryTemplate;

		// Token: 0x040015F7 RID: 5623
		private PrefabPool<UsageUtilitiesDisplay_Entry> _entryPool;
	}
}
