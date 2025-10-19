using System;
using Duckov.UI;
using Duckov.UI.Animations;
using UnityEngine;

// Token: 0x0200019A RID: 410
public class ATMView : View
{
	// Token: 0x17000233 RID: 563
	// (get) Token: 0x06000C1F RID: 3103 RVA: 0x0003345E File Offset: 0x0003165E
	public static ATMView Instance
	{
		get
		{
			return View.GetViewInstance<ATMView>();
		}
	}

	// Token: 0x06000C20 RID: 3104 RVA: 0x00033465 File Offset: 0x00031665
	protected override void Awake()
	{
		base.Awake();
	}

	// Token: 0x06000C21 RID: 3105 RVA: 0x00033470 File Offset: 0x00031670
	public static void Show()
	{
		ATMView instance = ATMView.Instance;
		if (instance == null)
		{
			return;
		}
		instance.Open(null);
	}

	// Token: 0x06000C22 RID: 3106 RVA: 0x00033494 File Offset: 0x00031694
	protected override void OnOpen()
	{
		base.OnOpen();
		this.fadeGroup.Show();
		this.atmPanel.ShowSelectPanel(true);
	}

	// Token: 0x06000C23 RID: 3107 RVA: 0x000334B3 File Offset: 0x000316B3
	protected override void OnClose()
	{
		base.OnClose();
		this.fadeGroup.Hide();
	}

	// Token: 0x04000A89 RID: 2697
	[SerializeField]
	private FadeGroup fadeGroup;

	// Token: 0x04000A8A RID: 2698
	[SerializeField]
	private ATMPanel atmPanel;
}
