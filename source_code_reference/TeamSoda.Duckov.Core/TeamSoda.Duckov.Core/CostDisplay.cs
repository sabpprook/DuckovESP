using System;
using Duckov.Economy;
using Duckov.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000158 RID: 344
public class CostDisplay : MonoBehaviour
{
	// Token: 0x17000213 RID: 531
	// (get) Token: 0x06000A87 RID: 2695 RVA: 0x0002DF40 File Offset: 0x0002C140
	private PrefabPool<ItemAmountDisplay> ItemPool
	{
		get
		{
			if (this._itemPool == null)
			{
				this._itemPool = new PrefabPool<ItemAmountDisplay>(this.itemAmountTemplate, null, null, null, null, true, 10, 10000, null);
			}
			return this._itemPool;
		}
	}

	// Token: 0x06000A88 RID: 2696 RVA: 0x0002DF79 File Offset: 0x0002C179
	private void OnEnable()
	{
		EconomyManager.OnMoneyChanged += this.OnMoneyChanged;
		ItemUtilities.OnPlayerItemOperation += this.OnItemOperation;
		LevelManager.OnLevelInitialized += this.OnLevelInitialized;
	}

	// Token: 0x06000A89 RID: 2697 RVA: 0x0002DFAE File Offset: 0x0002C1AE
	private void OnDisable()
	{
		EconomyManager.OnMoneyChanged -= this.OnMoneyChanged;
		ItemUtilities.OnPlayerItemOperation -= this.OnItemOperation;
		LevelManager.OnLevelInitialized -= this.OnLevelInitialized;
	}

	// Token: 0x06000A8A RID: 2698 RVA: 0x0002DFE3 File Offset: 0x0002C1E3
	private void OnLevelInitialized()
	{
		this.RefreshBackground();
	}

	// Token: 0x06000A8B RID: 2699 RVA: 0x0002DFEB File Offset: 0x0002C1EB
	private void OnItemOperation()
	{
		this.RefreshBackground();
	}

	// Token: 0x06000A8C RID: 2700 RVA: 0x0002DFF3 File Offset: 0x0002C1F3
	private void RefreshBackground()
	{
		if (this.background == null)
		{
			return;
		}
		this.background.color = (this.cost.Enough ? this.enoughColor : this.normalColor);
	}

	// Token: 0x06000A8D RID: 2701 RVA: 0x0002E02A File Offset: 0x0002C22A
	private void OnMoneyChanged(long arg1, long arg2)
	{
		this.RefreshMoneyBackground();
		this.RefreshBackground();
	}

	// Token: 0x06000A8E RID: 2702 RVA: 0x0002E038 File Offset: 0x0002C238
	public void Setup(Cost cost, int multiplier = 1)
	{
		this.cost = cost;
		this.moneyContainer.SetActive(cost.money > 0L);
		this.money.text = (cost.money * (long)multiplier).ToString("n0");
		this.itemsContainer.SetActive(cost.items != null && cost.items.Length != 0);
		this.ItemPool.ReleaseAll();
		if (cost.items != null)
		{
			foreach (Cost.ItemEntry itemEntry in cost.items)
			{
				ItemAmountDisplay itemAmountDisplay = this.ItemPool.Get(null);
				itemAmountDisplay.Setup(itemEntry.id, itemEntry.amount * (long)multiplier);
				itemAmountDisplay.transform.SetAsLastSibling();
			}
		}
		this.RefreshMoneyBackground();
		this.RefreshBackground();
	}

	// Token: 0x06000A8F RID: 2703 RVA: 0x0002E10C File Offset: 0x0002C30C
	private void RefreshMoneyBackground()
	{
		bool flag = this.cost.money <= EconomyManager.Money;
		this.moneyBackground.color = (flag ? this.money_enoughColor : this.money_normalColor);
	}

	// Token: 0x06000A90 RID: 2704 RVA: 0x0002E14B File Offset: 0x0002C34B
	internal void Clear()
	{
		this.cost = default(Cost);
		this.moneyContainer.SetActive(false);
		this.ItemPool.ReleaseAll();
	}

	// Token: 0x0400092F RID: 2351
	[SerializeField]
	private GameObject moneyContainer;

	// Token: 0x04000930 RID: 2352
	[SerializeField]
	private GameObject itemsContainer;

	// Token: 0x04000931 RID: 2353
	[SerializeField]
	private Image background;

	// Token: 0x04000932 RID: 2354
	[SerializeField]
	private Image moneyBackground;

	// Token: 0x04000933 RID: 2355
	[SerializeField]
	private TextMeshProUGUI money;

	// Token: 0x04000934 RID: 2356
	[SerializeField]
	private ItemAmountDisplay itemAmountTemplate;

	// Token: 0x04000935 RID: 2357
	[SerializeField]
	private Color normalColor;

	// Token: 0x04000936 RID: 2358
	[SerializeField]
	private Color enoughColor;

	// Token: 0x04000937 RID: 2359
	[SerializeField]
	private Color money_normalColor;

	// Token: 0x04000938 RID: 2360
	[SerializeField]
	private Color money_enoughColor;

	// Token: 0x04000939 RID: 2361
	private PrefabPool<ItemAmountDisplay> _itemPool;

	// Token: 0x0400093A RID: 2362
	private Cost cost;
}
