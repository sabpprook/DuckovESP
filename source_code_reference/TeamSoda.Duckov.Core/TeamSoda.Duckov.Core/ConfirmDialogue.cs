using System;
using Cysharp.Threading.Tasks;
using Duckov.UI.Animations;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x0200016D RID: 365
public class ConfirmDialogue : MonoBehaviour
{
	// Token: 0x06000B00 RID: 2816 RVA: 0x0002EECF File Offset: 0x0002D0CF
	private void Awake()
	{
		this.btnConfirm.onClick.AddListener(new UnityAction(this.OnConfirmed));
		this.btnCancel.onClick.AddListener(new UnityAction(this.OnCanceled));
	}

	// Token: 0x06000B01 RID: 2817 RVA: 0x0002EF09 File Offset: 0x0002D109
	private void OnCanceled()
	{
		this.canceled = true;
	}

	// Token: 0x06000B02 RID: 2818 RVA: 0x0002EF12 File Offset: 0x0002D112
	private void OnConfirmed()
	{
		this.confirmed = true;
	}

	// Token: 0x06000B03 RID: 2819 RVA: 0x0002EF1C File Offset: 0x0002D11C
	public async UniTask<bool> Execute()
	{
		bool flag;
		if (this.executing)
		{
			flag = false;
		}
		else
		{
			this.executing = true;
			bool flag2 = await this.DoExecute();
			this.executing = false;
			flag = flag2;
		}
		return flag;
	}

	// Token: 0x06000B04 RID: 2820 RVA: 0x0002EF60 File Offset: 0x0002D160
	private async UniTask<bool> DoExecute()
	{
		Debug.Log("Executing confirm dialogue");
		await this.fadeGroup.ShowAndReturnTask();
		this.canceled = false;
		this.confirmed = false;
		while (!this.canceled && !this.confirmed)
		{
			await UniTask.Yield();
		}
		bool flag = false;
		if (this.confirmed)
		{
			flag = true;
		}
		this.fadeGroup.Hide();
		return flag;
	}

	// Token: 0x06000B05 RID: 2821 RVA: 0x0002EFA3 File Offset: 0x0002D1A3
	internal void SkipHide()
	{
		this.fadeGroup.SkipHide();
	}

	// Token: 0x04000972 RID: 2418
	[SerializeField]
	private FadeGroup fadeGroup;

	// Token: 0x04000973 RID: 2419
	[SerializeField]
	private Button btnConfirm;

	// Token: 0x04000974 RID: 2420
	[SerializeField]
	private Button btnCancel;

	// Token: 0x04000975 RID: 2421
	private bool canceled;

	// Token: 0x04000976 RID: 2422
	private bool confirmed;

	// Token: 0x04000977 RID: 2423
	private bool executing;
}
