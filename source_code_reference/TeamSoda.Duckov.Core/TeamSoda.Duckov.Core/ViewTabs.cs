using System;
using Duckov.UI;
using Duckov.UI.Animations;
using UnityEngine;

// Token: 0x02000200 RID: 512
public class ViewTabs : MonoBehaviour
{
	// Token: 0x06000F03 RID: 3843 RVA: 0x0003B758 File Offset: 0x00039958
	public void Show()
	{
		this.fadeGroup.Show();
	}

	// Token: 0x06000F04 RID: 3844 RVA: 0x0003B765 File Offset: 0x00039965
	public void Hide()
	{
		this.fadeGroup.Hide();
	}

	// Token: 0x06000F05 RID: 3845 RVA: 0x0003B772 File Offset: 0x00039972
	private void Update()
	{
		if (this.fadeGroup.IsShown && View.ActiveView == null)
		{
			this.Hide();
		}
	}

	// Token: 0x04000C51 RID: 3153
	[SerializeField]
	private FadeGroup fadeGroup;
}
