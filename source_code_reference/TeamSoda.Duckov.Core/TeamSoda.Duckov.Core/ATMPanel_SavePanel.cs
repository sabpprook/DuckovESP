using System;
using Duckov.UI.Animations;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x02000199 RID: 409
public class ATMPanel_SavePanel : MonoBehaviour
{
	// Token: 0x17000232 RID: 562
	// (get) Token: 0x06000C0F RID: 3087 RVA: 0x00033205 File Offset: 0x00031405
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

	// Token: 0x14000060 RID: 96
	// (add) Token: 0x06000C10 RID: 3088 RVA: 0x00033228 File Offset: 0x00031428
	// (remove) Token: 0x06000C11 RID: 3089 RVA: 0x00033260 File Offset: 0x00031460
	public event Action<ATMPanel_SavePanel> onQuit;

	// Token: 0x06000C12 RID: 3090 RVA: 0x00033295 File Offset: 0x00031495
	private void OnEnable()
	{
		ItemUtilities.OnPlayerItemOperation += this.OnPlayerItemOperation;
		this.RefreshCash();
		this.Refresh();
	}

	// Token: 0x06000C13 RID: 3091 RVA: 0x000332B4 File Offset: 0x000314B4
	private void OnDisable()
	{
		ItemUtilities.OnPlayerItemOperation -= this.OnPlayerItemOperation;
	}

	// Token: 0x06000C14 RID: 3092 RVA: 0x000332C7 File Offset: 0x000314C7
	private void OnPlayerItemOperation()
	{
		this.RefreshCash();
		this.Refresh();
	}

	// Token: 0x06000C15 RID: 3093 RVA: 0x000332D5 File Offset: 0x000314D5
	private void RefreshCash()
	{
		this._cachedCashAmount = ItemUtilities.GetItemCount(451);
	}

	// Token: 0x06000C16 RID: 3094 RVA: 0x000332E8 File Offset: 0x000314E8
	private void Awake()
	{
		this.inputPanel.onInputFieldValueChanged += this.OnInputValueChanged;
		this.inputPanel.maxFunction = () => (long)this.CashAmount;
		this.confirmButton.onClick.AddListener(new UnityAction(this.OnConfirmButtonClicked));
		this.quitButton.onClick.AddListener(new UnityAction(this.OnQuitButtonClicked));
	}

	// Token: 0x06000C17 RID: 3095 RVA: 0x0003335B File Offset: 0x0003155B
	private void OnQuitButtonClicked()
	{
		Action<ATMPanel_SavePanel> action = this.onQuit;
		if (action == null)
		{
			return;
		}
		action(this);
	}

	// Token: 0x06000C18 RID: 3096 RVA: 0x00033370 File Offset: 0x00031570
	private void OnConfirmButtonClicked()
	{
		if (this.inputPanel.Value <= 0L)
		{
			this.inputPanel.Clear();
			return;
		}
		if (this.inputPanel.Value > (long)this.CashAmount)
		{
			return;
		}
		if (ATMPanel.Save(this.inputPanel.Value))
		{
			this.inputPanel.Clear();
		}
	}

	// Token: 0x06000C19 RID: 3097 RVA: 0x000333CA File Offset: 0x000315CA
	private void OnInputValueChanged(string v)
	{
		this.Refresh();
	}

	// Token: 0x06000C1A RID: 3098 RVA: 0x000333D4 File Offset: 0x000315D4
	private void Refresh()
	{
		bool flag = (long)this.CashAmount >= this.inputPanel.Value;
		flag &= this.inputPanel.Value >= 0L;
		this.insufficientIndicator.SetActive(!flag);
	}

	// Token: 0x06000C1B RID: 3099 RVA: 0x0003341D File Offset: 0x0003161D
	internal void Hide(bool skip = false)
	{
		if (skip)
		{
			this.fadeGroup.SkipHide();
			return;
		}
		this.fadeGroup.Hide();
	}

	// Token: 0x06000C1C RID: 3100 RVA: 0x00033439 File Offset: 0x00031639
	internal void Show()
	{
		this.fadeGroup.Show();
	}

	// Token: 0x04000A81 RID: 2689
	private const int CashItemTypeID = 451;

	// Token: 0x04000A82 RID: 2690
	[SerializeField]
	private FadeGroup fadeGroup;

	// Token: 0x04000A83 RID: 2691
	[SerializeField]
	private DigitInputPanel inputPanel;

	// Token: 0x04000A84 RID: 2692
	[SerializeField]
	private Button confirmButton;

	// Token: 0x04000A85 RID: 2693
	[SerializeField]
	private GameObject insufficientIndicator;

	// Token: 0x04000A86 RID: 2694
	[SerializeField]
	private Button quitButton;

	// Token: 0x04000A87 RID: 2695
	private int _cachedCashAmount = -1;
}
