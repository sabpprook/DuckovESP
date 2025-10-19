using System;
using Duckov.UI;
using ItemStatsSystem;
using UnityEngine;

// Token: 0x020001F4 RID: 500
public class DebugUISetup : MonoBehaviour
{
	// Token: 0x170002A1 RID: 673
	// (get) Token: 0x06000EA6 RID: 3750 RVA: 0x0003A87D File Offset: 0x00038A7D
	private CharacterMainControl Character
	{
		get
		{
			return LevelManager.Instance.MainCharacter;
		}
	}

	// Token: 0x170002A2 RID: 674
	// (get) Token: 0x06000EA7 RID: 3751 RVA: 0x0003A889 File Offset: 0x00038A89
	private Item CharacterItem
	{
		get
		{
			return this.Character.CharacterItem;
		}
	}

	// Token: 0x06000EA8 RID: 3752 RVA: 0x0003A896 File Offset: 0x00038A96
	public void Setup()
	{
		this.slotCollectionDisplay.Setup(this.CharacterItem, false);
		this.inventoryDisplay.Setup(this.CharacterItem.Inventory, null, null, false, null);
	}

	// Token: 0x04000C1C RID: 3100
	[SerializeField]
	private ItemSlotCollectionDisplay slotCollectionDisplay;

	// Token: 0x04000C1D RID: 3101
	[SerializeField]
	private InventoryDisplay inventoryDisplay;
}
