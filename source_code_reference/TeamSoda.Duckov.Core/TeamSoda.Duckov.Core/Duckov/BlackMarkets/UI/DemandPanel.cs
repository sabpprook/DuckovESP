using System;
using Duckov.Utilities;
using UnityEngine;

namespace Duckov.BlackMarkets.UI
{
	// Token: 0x02000306 RID: 774
	public class DemandPanel : MonoBehaviour
	{
		// Token: 0x1700049B RID: 1179
		// (get) Token: 0x0600194B RID: 6475 RVA: 0x0005BCD9 File Offset: 0x00059ED9
		// (set) Token: 0x0600194C RID: 6476 RVA: 0x0005BCE1 File Offset: 0x00059EE1
		public BlackMarket Target { get; private set; }

		// Token: 0x1700049C RID: 1180
		// (get) Token: 0x0600194D RID: 6477 RVA: 0x0005BCEC File Offset: 0x00059EEC
		private PrefabPool<DemandPanel_Entry> EntryPool
		{
			get
			{
				if (this._entryPool == null)
				{
					this._entryPool = new PrefabPool<DemandPanel_Entry>(this.entryTemplate, null, null, null, null, true, 10, 10000, new Action<DemandPanel_Entry>(this.OnCreateEntry));
				}
				return this._entryPool;
			}
		}

		// Token: 0x0600194E RID: 6478 RVA: 0x0005BD30 File Offset: 0x00059F30
		private void OnCreateEntry(DemandPanel_Entry entry)
		{
			entry.onDealButtonClicked += this.OnEntryClicked;
		}

		// Token: 0x0600194F RID: 6479 RVA: 0x0005BD44 File Offset: 0x00059F44
		private void OnEntryClicked(DemandPanel_Entry entry)
		{
			this.Target.Sell(entry.Target);
		}

		// Token: 0x06001950 RID: 6480 RVA: 0x0005BD58 File Offset: 0x00059F58
		internal void Setup(BlackMarket target)
		{
			if (target == null)
			{
				Debug.LogError("加载 BlackMarket 的 DemandPanel 失败。Black Market 对象不存在。");
				return;
			}
			this.Target = target;
			this.Refresh();
			if (base.isActiveAndEnabled)
			{
				this.RegisterEvents();
			}
		}

		// Token: 0x06001951 RID: 6481 RVA: 0x0005BD89 File Offset: 0x00059F89
		private void OnEnable()
		{
			this.RegisterEvents();
			this.Refresh();
		}

		// Token: 0x06001952 RID: 6482 RVA: 0x0005BD97 File Offset: 0x00059F97
		private void OnDisable()
		{
			this.UnregsiterEvents();
		}

		// Token: 0x06001953 RID: 6483 RVA: 0x0005BDA0 File Offset: 0x00059FA0
		private void Refresh()
		{
			if (this.Target == null)
			{
				return;
			}
			this.EntryPool.ReleaseAll();
			foreach (BlackMarket.DemandSupplyEntry demandSupplyEntry in this.Target.Demands)
			{
				this.EntryPool.Get(null).Setup(demandSupplyEntry);
			}
		}

		// Token: 0x06001954 RID: 6484 RVA: 0x0005BE18 File Offset: 0x0005A018
		private void UnregsiterEvents()
		{
			if (this.Target == null)
			{
				return;
			}
			this.Target.onAfterGenerateEntries -= this.OnAfterTargetGenerateEntries;
		}

		// Token: 0x06001955 RID: 6485 RVA: 0x0005BE40 File Offset: 0x0005A040
		private void RegisterEvents()
		{
			if (this.Target == null)
			{
				return;
			}
			this.UnregsiterEvents();
			this.Target.onAfterGenerateEntries += this.OnAfterTargetGenerateEntries;
		}

		// Token: 0x06001956 RID: 6486 RVA: 0x0005BE6E File Offset: 0x0005A06E
		private void OnAfterTargetGenerateEntries()
		{
			this.Refresh();
		}

		// Token: 0x04001260 RID: 4704
		[SerializeField]
		private DemandPanel_Entry entryTemplate;

		// Token: 0x04001261 RID: 4705
		private PrefabPool<DemandPanel_Entry> _entryPool;
	}
}
