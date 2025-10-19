using System;
using Duckov.UI;
using UnityEngine;

// Token: 0x02000201 RID: 513
public class ViewTabDisplayEntry : MonoBehaviour
{
	// Token: 0x06000F07 RID: 3847 RVA: 0x0003B79C File Offset: 0x0003999C
	private void Awake()
	{
		ManagedUIElement.onOpen += this.OnViewOpen;
		ManagedUIElement.onClose += this.OnViewClose;
		this.HideIndicator();
	}

	// Token: 0x06000F08 RID: 3848 RVA: 0x0003B7C6 File Offset: 0x000399C6
	private void OnDestroy()
	{
		ManagedUIElement.onOpen -= this.OnViewOpen;
		ManagedUIElement.onClose -= this.OnViewClose;
	}

	// Token: 0x06000F09 RID: 3849 RVA: 0x0003B7EA File Offset: 0x000399EA
	private void Start()
	{
		if (View.ActiveView != null && View.ActiveView.GetType().Name == this.viewTypeName)
		{
			this.ShowIndicator();
		}
	}

	// Token: 0x06000F0A RID: 3850 RVA: 0x0003B81B File Offset: 0x00039A1B
	private void OnViewClose(ManagedUIElement element)
	{
		if (element.GetType().Name == this.viewTypeName)
		{
			this.HideIndicator();
		}
	}

	// Token: 0x06000F0B RID: 3851 RVA: 0x0003B83B File Offset: 0x00039A3B
	private void OnViewOpen(ManagedUIElement element)
	{
		if (element.GetType().Name == this.viewTypeName)
		{
			this.ShowIndicator();
		}
	}

	// Token: 0x06000F0C RID: 3852 RVA: 0x0003B85B File Offset: 0x00039A5B
	private void ShowIndicator()
	{
		this.indicator.SetActive(true);
		this.punch.Punch();
	}

	// Token: 0x06000F0D RID: 3853 RVA: 0x0003B874 File Offset: 0x00039A74
	private void HideIndicator()
	{
		this.indicator.SetActive(false);
	}

	// Token: 0x04000C52 RID: 3154
	[SerializeField]
	private string viewTypeName;

	// Token: 0x04000C53 RID: 3155
	[SerializeField]
	private GameObject indicator;

	// Token: 0x04000C54 RID: 3156
	[SerializeField]
	private PunchReceiver punch;
}
