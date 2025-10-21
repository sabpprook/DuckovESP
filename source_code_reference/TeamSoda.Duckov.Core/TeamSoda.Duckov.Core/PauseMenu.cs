using System;

// Token: 0x0200016A RID: 362
public class PauseMenu : UIPanel
{
	// Token: 0x17000219 RID: 537
	// (get) Token: 0x06000AF2 RID: 2802 RVA: 0x0002ED27 File Offset: 0x0002CF27
	public static PauseMenu Instance
	{
		get
		{
			return GameManager.PauseMenu;
		}
	}

	// Token: 0x1700021A RID: 538
	// (get) Token: 0x06000AF3 RID: 2803 RVA: 0x0002ED2E File Offset: 0x0002CF2E
	public bool Shown
	{
		get
		{
			return !(this.fadeGroup == null) && this.fadeGroup.IsShown;
		}
	}

	// Token: 0x06000AF4 RID: 2804 RVA: 0x0002ED4B File Offset: 0x0002CF4B
	public static void Show()
	{
		PauseMenu.Instance.Open(null, true);
	}

	// Token: 0x06000AF5 RID: 2805 RVA: 0x0002ED59 File Offset: 0x0002CF59
	public static void Hide()
	{
		PauseMenu.Instance.Close();
	}

	// Token: 0x06000AF6 RID: 2806 RVA: 0x0002ED65 File Offset: 0x0002CF65
	public static void Toggle()
	{
		if (PauseMenu.Instance.fadeGroup.IsShown)
		{
			PauseMenu.Hide();
			return;
		}
		PauseMenu.Show();
	}
}
