using System;
using Duckov.Utilities;
using UnityEngine;

// Token: 0x020000D4 RID: 212
public class CostTakerHUD : MonoBehaviour
{
	// Token: 0x17000134 RID: 308
	// (get) Token: 0x0600068F RID: 1679 RVA: 0x0001D8DC File Offset: 0x0001BADC
	private PrefabPool<CostTakerHUD_Entry> EntryPool
	{
		get
		{
			if (this._entryPool == null)
			{
				this._entryPool = new PrefabPool<CostTakerHUD_Entry>(this.entryTemplate, null, null, null, null, true, 10, 10000, null);
			}
			return this._entryPool;
		}
	}

	// Token: 0x06000690 RID: 1680 RVA: 0x0001D915 File Offset: 0x0001BB15
	private void Awake()
	{
		this.entryTemplate.gameObject.SetActive(false);
		this.ShowAll();
		CostTaker.OnCostTakerRegistered += this.OnCostTakerRegistered;
		CostTaker.OnCostTakerUnregistered += this.OnCostTakerUnregistered;
	}

	// Token: 0x06000691 RID: 1681 RVA: 0x0001D950 File Offset: 0x0001BB50
	private void OnDestroy()
	{
		CostTaker.OnCostTakerRegistered -= this.OnCostTakerRegistered;
		CostTaker.OnCostTakerUnregistered -= this.OnCostTakerUnregistered;
	}

	// Token: 0x06000692 RID: 1682 RVA: 0x0001D974 File Offset: 0x0001BB74
	private void OnCostTakerRegistered(CostTaker taker)
	{
		this.ShowHUD(taker);
	}

	// Token: 0x06000693 RID: 1683 RVA: 0x0001D97D File Offset: 0x0001BB7D
	private void OnCostTakerUnregistered(CostTaker taker)
	{
		this.HideHUD(taker);
	}

	// Token: 0x06000694 RID: 1684 RVA: 0x0001D986 File Offset: 0x0001BB86
	private void Start()
	{
	}

	// Token: 0x06000695 RID: 1685 RVA: 0x0001D988 File Offset: 0x0001BB88
	private void ShowAll()
	{
		this.EntryPool.ReleaseAll();
		foreach (CostTaker costTaker in CostTaker.ActiveCostTakers)
		{
			this.ShowHUD(costTaker);
		}
	}

	// Token: 0x06000696 RID: 1686 RVA: 0x0001D9E0 File Offset: 0x0001BBE0
	private void ShowHUD(CostTaker costTaker)
	{
		this.EntryPool.Get(null).Setup(costTaker);
	}

	// Token: 0x06000697 RID: 1687 RVA: 0x0001D9F4 File Offset: 0x0001BBF4
	private void HideHUD(CostTaker costTaker)
	{
		CostTakerHUD_Entry costTakerHUD_Entry = this.EntryPool.Find((CostTakerHUD_Entry e) => e.gameObject.activeSelf && e.Target == costTaker);
		if (costTakerHUD_Entry == null)
		{
			return;
		}
		this.EntryPool.Release(costTakerHUD_Entry);
	}

	// Token: 0x04000659 RID: 1625
	[SerializeField]
	private CostTakerHUD_Entry entryTemplate;

	// Token: 0x0400065A RID: 1626
	private PrefabPool<CostTakerHUD_Entry> _entryPool;
}
