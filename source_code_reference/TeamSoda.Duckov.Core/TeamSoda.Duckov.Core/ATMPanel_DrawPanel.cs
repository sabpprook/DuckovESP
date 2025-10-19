using System;
using Cysharp.Threading.Tasks;
using Duckov.Economy;
using Duckov.UI.Animations;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x02000198 RID: 408
public class ATMPanel_DrawPanel : MonoBehaviour
{
	// Token: 0x1400005F RID: 95
	// (add) Token: 0x06000C01 RID: 3073 RVA: 0x00032F7C File Offset: 0x0003117C
	// (remove) Token: 0x06000C02 RID: 3074 RVA: 0x00032FB4 File Offset: 0x000311B4
	public event Action<ATMPanel_DrawPanel> onQuit;

	// Token: 0x06000C03 RID: 3075 RVA: 0x00032FE9 File Offset: 0x000311E9
	private void OnEnable()
	{
		EconomyManager.OnMoneyChanged += this.OnMoneyChanged;
		this.Refresh();
	}

	// Token: 0x06000C04 RID: 3076 RVA: 0x00033002 File Offset: 0x00031202
	private void OnDisable()
	{
		EconomyManager.OnMoneyChanged -= this.OnMoneyChanged;
	}

	// Token: 0x06000C05 RID: 3077 RVA: 0x00033018 File Offset: 0x00031218
	private void Awake()
	{
		this.inputPanel.onInputFieldValueChanged += this.OnInputValueChanged;
		this.inputPanel.maxFunction = delegate
		{
			long num = EconomyManager.Money;
			if (num > 10000000L)
			{
				num = 10000000L;
			}
			return num;
		};
		this.confirmButton.onClick.AddListener(new UnityAction(this.OnConfirmButtonClicked));
		this.quitButton.onClick.AddListener(new UnityAction(this.OnQuitButtonClicked));
	}

	// Token: 0x06000C06 RID: 3078 RVA: 0x0003309E File Offset: 0x0003129E
	private void OnQuitButtonClicked()
	{
		Action<ATMPanel_DrawPanel> action = this.onQuit;
		if (action == null)
		{
			return;
		}
		action(this);
	}

	// Token: 0x06000C07 RID: 3079 RVA: 0x000330B1 File Offset: 0x000312B1
	private void OnMoneyChanged(long arg1, long arg2)
	{
		this.Refresh();
	}

	// Token: 0x06000C08 RID: 3080 RVA: 0x000330BC File Offset: 0x000312BC
	private void OnConfirmButtonClicked()
	{
		if (this.inputPanel.Value <= 0L)
		{
			this.inputPanel.Clear();
			return;
		}
		long num = EconomyManager.Money;
		if (num > 10000000L)
		{
			num = 10000000L;
		}
		if (this.inputPanel.Value > num)
		{
			return;
		}
		this.DrawTask(this.inputPanel.Value).Forget();
	}

	// Token: 0x06000C09 RID: 3081 RVA: 0x00033120 File Offset: 0x00031320
	private async UniTask DrawTask(long value)
	{
		UniTask<bool>.Awaiter awaiter = ATMPanel.Draw(value).GetAwaiter();
		if (!awaiter.IsCompleted)
		{
			await awaiter;
			UniTask<bool>.Awaiter awaiter2;
			awaiter = awaiter2;
			awaiter2 = default(UniTask<bool>.Awaiter);
		}
		if (awaiter.GetResult())
		{
			this.inputPanel.Clear();
		}
	}

	// Token: 0x06000C0A RID: 3082 RVA: 0x0003316B File Offset: 0x0003136B
	private void OnInputValueChanged(string v)
	{
		this.Refresh();
	}

	// Token: 0x06000C0B RID: 3083 RVA: 0x00033174 File Offset: 0x00031374
	private void Refresh()
	{
		bool flag = EconomyManager.Money >= this.inputPanel.Value;
		flag &= this.inputPanel.Value <= 10000000L;
		flag &= this.inputPanel.Value >= 0L;
		this.insufficientIndicator.SetActive(!flag);
	}

	// Token: 0x06000C0C RID: 3084 RVA: 0x000331D4 File Offset: 0x000313D4
	internal void Show()
	{
		this.fadeGroup.Show();
	}

	// Token: 0x06000C0D RID: 3085 RVA: 0x000331E1 File Offset: 0x000313E1
	internal void Hide(bool skip)
	{
		if (skip)
		{
			this.fadeGroup.SkipHide();
			return;
		}
		this.fadeGroup.Hide();
	}

	// Token: 0x04000A7B RID: 2683
	[SerializeField]
	private FadeGroup fadeGroup;

	// Token: 0x04000A7C RID: 2684
	[SerializeField]
	private DigitInputPanel inputPanel;

	// Token: 0x04000A7D RID: 2685
	[SerializeField]
	private Button confirmButton;

	// Token: 0x04000A7E RID: 2686
	[SerializeField]
	private GameObject insufficientIndicator;

	// Token: 0x04000A7F RID: 2687
	[SerializeField]
	private Button quitButton;
}
