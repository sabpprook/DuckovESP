using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Duckov.Economy;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020000D3 RID: 211
public class CostTaker : InteractableBase
{
	// Token: 0x17000132 RID: 306
	// (get) Token: 0x0600067E RID: 1662 RVA: 0x0001D69F File Offset: 0x0001B89F
	public Cost Cost
	{
		get
		{
			return this.cost;
		}
	}

	// Token: 0x14000028 RID: 40
	// (add) Token: 0x0600067F RID: 1663 RVA: 0x0001D6A8 File Offset: 0x0001B8A8
	// (remove) Token: 0x06000680 RID: 1664 RVA: 0x0001D6E0 File Offset: 0x0001B8E0
	public event Action<CostTaker> onPayed;

	// Token: 0x06000681 RID: 1665 RVA: 0x0001D715 File Offset: 0x0001B915
	protected override bool IsInteractable()
	{
		return this.cost.Enough;
	}

	// Token: 0x06000682 RID: 1666 RVA: 0x0001D724 File Offset: 0x0001B924
	protected override void OnInteractFinished()
	{
		if (!this.cost.Enough)
		{
			return;
		}
		if (this.cost.Pay(true, true))
		{
			Action<CostTaker> action = this.onPayed;
			if (action != null)
			{
				action(this);
			}
			UnityEvent<CostTaker> unityEvent = this.onPayedUnityEvent;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke(this);
		}
	}

	// Token: 0x06000683 RID: 1667 RVA: 0x0001D771 File Offset: 0x0001B971
	private void OnEnable()
	{
		CostTaker.Register(this);
	}

	// Token: 0x06000684 RID: 1668 RVA: 0x0001D779 File Offset: 0x0001B979
	private void OnDisable()
	{
		CostTaker.Unregister(this);
	}

	// Token: 0x17000133 RID: 307
	// (get) Token: 0x06000685 RID: 1669 RVA: 0x0001D781 File Offset: 0x0001B981
	public static ReadOnlyCollection<CostTaker> ActiveCostTakers
	{
		get
		{
			if (CostTaker._activeCostTakers_ReadOnly == null)
			{
				CostTaker._activeCostTakers_ReadOnly = new ReadOnlyCollection<CostTaker>(CostTaker.activeCostTakers);
			}
			return CostTaker._activeCostTakers_ReadOnly;
		}
	}

	// Token: 0x14000029 RID: 41
	// (add) Token: 0x06000686 RID: 1670 RVA: 0x0001D7A0 File Offset: 0x0001B9A0
	// (remove) Token: 0x06000687 RID: 1671 RVA: 0x0001D7D4 File Offset: 0x0001B9D4
	public static event Action<CostTaker> OnCostTakerRegistered;

	// Token: 0x1400002A RID: 42
	// (add) Token: 0x06000688 RID: 1672 RVA: 0x0001D808 File Offset: 0x0001BA08
	// (remove) Token: 0x06000689 RID: 1673 RVA: 0x0001D83C File Offset: 0x0001BA3C
	public static event Action<CostTaker> OnCostTakerUnregistered;

	// Token: 0x0600068A RID: 1674 RVA: 0x0001D86F File Offset: 0x0001BA6F
	public static void Register(CostTaker costTaker)
	{
		CostTaker.activeCostTakers.Add(costTaker);
		Action<CostTaker> onCostTakerRegistered = CostTaker.OnCostTakerRegistered;
		if (onCostTakerRegistered == null)
		{
			return;
		}
		onCostTakerRegistered(costTaker);
	}

	// Token: 0x0600068B RID: 1675 RVA: 0x0001D88C File Offset: 0x0001BA8C
	public static void Unregister(CostTaker costTaker)
	{
		if (CostTaker.activeCostTakers.Remove(costTaker))
		{
			Action<CostTaker> onCostTakerUnregistered = CostTaker.OnCostTakerUnregistered;
			if (onCostTakerUnregistered == null)
			{
				return;
			}
			onCostTakerUnregistered(costTaker);
		}
	}

	// Token: 0x0600068C RID: 1676 RVA: 0x0001D8AB File Offset: 0x0001BAAB
	public void SetCost(Cost cost)
	{
		CostTaker.Unregister(this);
		this.cost = cost;
		if (base.isActiveAndEnabled)
		{
			CostTaker.Register(this);
		}
	}

	// Token: 0x04000652 RID: 1618
	[SerializeField]
	private Cost cost;

	// Token: 0x04000654 RID: 1620
	public UnityEvent<CostTaker> onPayedUnityEvent;

	// Token: 0x04000655 RID: 1621
	private static List<CostTaker> activeCostTakers = new List<CostTaker>();

	// Token: 0x04000656 RID: 1622
	private static ReadOnlyCollection<CostTaker> _activeCostTakers_ReadOnly;
}
