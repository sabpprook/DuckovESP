using System;
using Duckov.Buildings;
using Duckov.Buildings.UI;
using UnityEngine;

// Token: 0x0200019E RID: 414
public class BuilderViewInvoker : InteractableBase
{
	// Token: 0x06000C35 RID: 3125 RVA: 0x000337B8 File Offset: 0x000319B8
	protected override void OnInteractFinished()
	{
		if (this.buildingArea == null)
		{
			return;
		}
		BuilderView.Show(this.buildingArea);
	}

	// Token: 0x04000A94 RID: 2708
	[SerializeField]
	private BuildingArea buildingArea;
}
