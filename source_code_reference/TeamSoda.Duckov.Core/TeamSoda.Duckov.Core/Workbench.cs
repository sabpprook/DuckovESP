using System;
using Duckov.UI;

// Token: 0x02000195 RID: 405
public class Workbench : InteractableBase
{
	// Token: 0x06000BE8 RID: 3048 RVA: 0x0003298B File Offset: 0x00030B8B
	protected override void OnInteractFinished()
	{
		ItemCustomizeSelectionView.Show();
	}
}
