using System;
using Duckov.Utilities;
using UnityEngine;

namespace Duckov.BlackMarkets.UI
{
	// Token: 0x02000308 RID: 776
	public class SupplyPanel : MonoBehaviour
	{
		// Token: 0x170004A0 RID: 1184
		// (get) Token: 0x06001969 RID: 6505 RVA: 0x0005C14F File Offset: 0x0005A34F
		// (set) Token: 0x0600196A RID: 6506 RVA: 0x0005C157 File Offset: 0x0005A357
		public BlackMarket Target { get; private set; }

		// Token: 0x170004A1 RID: 1185
		// (get) Token: 0x0600196B RID: 6507 RVA: 0x0005C160 File Offset: 0x0005A360
		private PrefabPool<SupplyPanel_Entry> EntryPool
		{
			get
			{
				if (this._entryPool == null)
				{
					this._entryPool = new PrefabPool<SupplyPanel_Entry>(this.entryTemplate, null, null, null, null, true, 10, 10000, new Action<SupplyPanel_Entry>(this.OnCreateEntry));
				}
				return this._entryPool;
			}
		}

		// Token: 0x0600196C RID: 6508 RVA: 0x0005C1A4 File Offset: 0x0005A3A4
		private void OnCreateEntry(SupplyPanel_Entry entry)
		{
			entry.onDealButtonClicked += this.OnEntryClicked;
		}

		// Token: 0x0600196D RID: 6509 RVA: 0x0005C1B8 File Offset: 0x0005A3B8
		private void OnEntryClicked(SupplyPanel_Entry entry)
		{
			Debug.Log("Supply entry clicked");
			this.Target.Buy(entry.Target);
		}

		// Token: 0x0600196E RID: 6510 RVA: 0x0005C1D6 File Offset: 0x0005A3D6
		internal void Setup(BlackMarket target)
		{
			if (target == null)
			{
				Debug.LogError("加载 BlackMarket 的 Supply Panel 失败。Black Market 对象不存在。");
				return;
			}
			this.Target = target;
			this.Refresh();
			if (base.isActiveAndEnabled)
			{
				this.RegisterEvents();
			}
		}

		// Token: 0x0600196F RID: 6511 RVA: 0x0005C207 File Offset: 0x0005A407
		private void OnEnable()
		{
			this.RegisterEvents();
			this.Refresh();
		}

		// Token: 0x06001970 RID: 6512 RVA: 0x0005C215 File Offset: 0x0005A415
		private void OnDisable()
		{
			this.UnregsiterEvents();
		}

		// Token: 0x06001971 RID: 6513 RVA: 0x0005C220 File Offset: 0x0005A420
		private void Refresh()
		{
			if (this.Target == null)
			{
				return;
			}
			this.EntryPool.ReleaseAll();
			foreach (BlackMarket.DemandSupplyEntry demandSupplyEntry in this.Target.Supplies)
			{
				this.EntryPool.Get(null).Setup(demandSupplyEntry);
			}
		}

		// Token: 0x06001972 RID: 6514 RVA: 0x0005C298 File Offset: 0x0005A498
		private void UnregsiterEvents()
		{
			if (this.Target == null)
			{
				return;
			}
			this.Target.onAfterGenerateEntries -= this.OnAfterTargetGenerateEntries;
		}

		// Token: 0x06001973 RID: 6515 RVA: 0x0005C2C0 File Offset: 0x0005A4C0
		private void RegisterEvents()
		{
			if (this.Target == null)
			{
				return;
			}
			this.UnregsiterEvents();
			this.Target.onAfterGenerateEntries += this.OnAfterTargetGenerateEntries;
		}

		// Token: 0x06001974 RID: 6516 RVA: 0x0005C2EE File Offset: 0x0005A4EE
		private void OnAfterTargetGenerateEntries()
		{
			this.Refresh();
		}

		// Token: 0x0400126F RID: 4719
		[SerializeField]
		private SupplyPanel_Entry entryTemplate;

		// Token: 0x04001270 RID: 4720
		private PrefabPool<SupplyPanel_Entry> _entryPool;
	}
}
