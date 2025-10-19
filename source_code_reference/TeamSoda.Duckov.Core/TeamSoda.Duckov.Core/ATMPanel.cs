using System;
using Cysharp.Threading.Tasks;
using Duckov.Economy;
using Duckov.UI.Animations;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x02000197 RID: 407
public class ATMPanel : MonoBehaviour
{
	// Token: 0x17000231 RID: 561
	// (get) Token: 0x06000BF0 RID: 3056 RVA: 0x00032D0F File Offset: 0x00030F0F
	private int CashAmount
	{
		get
		{
			if (this._cachedCashAmount < 0)
			{
				this._cachedCashAmount = ItemUtilities.GetItemCount(451);
			}
			return this._cachedCashAmount;
		}
	}

	// Token: 0x06000BF1 RID: 3057 RVA: 0x00032D30 File Offset: 0x00030F30
	private void Awake()
	{
		this.btnSelectSave.onClick.AddListener(new UnityAction(this.ShowSavePanel));
		this.btnSelectDraw.onClick.AddListener(new UnityAction(this.ShowDrawPanel));
		this.savePanel.onQuit += this.SavePanel_onQuit;
		this.drawPanel.onQuit += this.DrawPanel_onQuit;
	}

	// Token: 0x06000BF2 RID: 3058 RVA: 0x00032DA3 File Offset: 0x00030FA3
	private void DrawPanel_onQuit(ATMPanel_DrawPanel panel)
	{
		this.ShowSelectPanel(false);
	}

	// Token: 0x06000BF3 RID: 3059 RVA: 0x00032DAC File Offset: 0x00030FAC
	private void SavePanel_onQuit(ATMPanel_SavePanel obj)
	{
		this.ShowSelectPanel(false);
	}

	// Token: 0x06000BF4 RID: 3060 RVA: 0x00032DB5 File Offset: 0x00030FB5
	private void HideAllPanels(bool skip = false)
	{
		if (skip)
		{
			this.selectPanel.SkipHide();
		}
		else
		{
			this.selectPanel.Hide();
		}
		this.savePanel.Hide(skip);
		this.drawPanel.Hide(skip);
	}

	// Token: 0x06000BF5 RID: 3061 RVA: 0x00032DEA File Offset: 0x00030FEA
	public void ShowSelectPanel(bool skipHideOthers = false)
	{
		this.HideAllPanels(skipHideOthers);
		this.selectPanel.Show();
	}

	// Token: 0x06000BF6 RID: 3062 RVA: 0x00032DFE File Offset: 0x00030FFE
	public void ShowDrawPanel()
	{
		this.HideAllPanels(false);
		this.drawPanel.Show();
	}

	// Token: 0x06000BF7 RID: 3063 RVA: 0x00032E12 File Offset: 0x00031012
	public void ShowSavePanel()
	{
		this.HideAllPanels(false);
		this.savePanel.Show();
	}

	// Token: 0x06000BF8 RID: 3064 RVA: 0x00032E26 File Offset: 0x00031026
	private void OnEnable()
	{
		EconomyManager.OnMoneyChanged += this.OnMoneyChanged;
		ItemUtilities.OnPlayerItemOperation += this.OnPlayerItemOperation;
		this.RefreshCash();
		this.RefreshBalance();
		this.ShowSelectPanel(false);
	}

	// Token: 0x06000BF9 RID: 3065 RVA: 0x00032E5D File Offset: 0x0003105D
	private void OnDisable()
	{
		EconomyManager.OnMoneyChanged -= this.OnMoneyChanged;
		ItemUtilities.OnPlayerItemOperation -= this.OnPlayerItemOperation;
	}

	// Token: 0x06000BFA RID: 3066 RVA: 0x00032E81 File Offset: 0x00031081
	private void OnPlayerItemOperation()
	{
		this.RefreshCash();
	}

	// Token: 0x06000BFB RID: 3067 RVA: 0x00032E89 File Offset: 0x00031089
	private void OnMoneyChanged(long oldMoney, long changedMoney)
	{
		this.RefreshBalance();
	}

	// Token: 0x06000BFC RID: 3068 RVA: 0x00032E91 File Offset: 0x00031091
	private void RefreshCash()
	{
		this._cachedCashAmount = ItemUtilities.GetItemCount(451);
		this.cashAmountText.text = string.Format("{0:n0}", this.CashAmount);
	}

	// Token: 0x06000BFD RID: 3069 RVA: 0x00032EC3 File Offset: 0x000310C3
	private void RefreshBalance()
	{
		this.balanceAmountText.text = string.Format("{0:n0}", EconomyManager.Money);
	}

	// Token: 0x06000BFE RID: 3070 RVA: 0x00032EE4 File Offset: 0x000310E4
	public static async UniTask<bool> Draw(long amount)
	{
		bool flag;
		if (ATMPanel.drawingMoney)
		{
			flag = false;
		}
		else
		{
			if (amount > 10000000L)
			{
				Debug.LogError(string.Format("Drawing amount {0} greater than max draw amount {1}. Clamping draw amount down.", amount, 10000000L));
				amount = 10000000L;
			}
			ATMPanel.drawingMoney = true;
			try
			{
				Cost cost = new Cost(amount);
				if (!cost.Enough)
				{
					return 0;
				}
				Cost cost2 = new Cost(new ValueTuple<int, long>[]
				{
					new ValueTuple<int, long>(451, amount)
				});
				await cost2.Return(false, true, 1, null);
				cost.Pay(true, false);
				cost = default(Cost);
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
			ATMPanel.drawingMoney = false;
			flag = true;
		}
		return flag;
	}

	// Token: 0x06000BFF RID: 3071 RVA: 0x00032F28 File Offset: 0x00031128
	public static bool Save(long amount)
	{
		Cost cost = new Cost(0L, new ValueTuple<int, long>[]
		{
			new ValueTuple<int, long>(451, amount)
		});
		if (!cost.Pay(false, true))
		{
			return false;
		}
		EconomyManager.Add(amount);
		return true;
	}

	// Token: 0x04000A70 RID: 2672
	private const int CashItemTypeID = 451;

	// Token: 0x04000A71 RID: 2673
	[SerializeField]
	private TextMeshProUGUI balanceAmountText;

	// Token: 0x04000A72 RID: 2674
	[SerializeField]
	private TextMeshProUGUI cashAmountText;

	// Token: 0x04000A73 RID: 2675
	[SerializeField]
	private Button btnSelectSave;

	// Token: 0x04000A74 RID: 2676
	[SerializeField]
	private Button btnSelectDraw;

	// Token: 0x04000A75 RID: 2677
	[SerializeField]
	private FadeGroup selectPanel;

	// Token: 0x04000A76 RID: 2678
	[SerializeField]
	private ATMPanel_SavePanel savePanel;

	// Token: 0x04000A77 RID: 2679
	[SerializeField]
	private ATMPanel_DrawPanel drawPanel;

	// Token: 0x04000A78 RID: 2680
	private int _cachedCashAmount = -1;

	// Token: 0x04000A79 RID: 2681
	private static bool drawingMoney;

	// Token: 0x04000A7A RID: 2682
	public const long MaxDrawAmount = 10000000L;
}
