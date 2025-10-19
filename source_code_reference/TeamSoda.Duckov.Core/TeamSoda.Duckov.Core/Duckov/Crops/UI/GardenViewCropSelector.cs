using System;
using Duckov.UI;
using Duckov.UI.Animations;
using ItemStatsSystem;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Duckov.Crops.UI
{
	// Token: 0x020002EE RID: 750
	public class GardenViewCropSelector : MonoBehaviour
	{
		// Token: 0x0600184E RID: 6222 RVA: 0x00058E42 File Offset: 0x00057042
		private void Awake()
		{
			this.btnConfirm.onClick.AddListener(new UnityAction(this.OnConfirm));
		}

		// Token: 0x0600184F RID: 6223 RVA: 0x00058E60 File Offset: 0x00057060
		private void OnConfirm()
		{
			Item selectedItem = ItemUIUtilities.SelectedItem;
			if (selectedItem != null)
			{
				this.master.SelectSeed(selectedItem.TypeID);
			}
			this.Hide();
		}

		// Token: 0x06001850 RID: 6224 RVA: 0x00058E94 File Offset: 0x00057094
		public void Show()
		{
			this.fadeGroup.Show();
			if (LevelManager.Instance == null)
			{
				return;
			}
			ItemUIUtilities.Select(null);
			this.playerInventoryDisplay.Setup(CharacterMainControl.Main.CharacterItem.Inventory, null, null, false, (Item e) => e != null && CropDatabase.IsSeed(e.TypeID));
			this.storageInventoryDisplay.Setup(PlayerStorage.Inventory, null, null, false, (Item e) => e != null && CropDatabase.IsSeed(e.TypeID));
		}

		// Token: 0x06001851 RID: 6225 RVA: 0x00058F2E File Offset: 0x0005712E
		private void OnEnable()
		{
			ItemUIUtilities.OnSelectionChanged += this.OnSelectionChanged;
		}

		// Token: 0x06001852 RID: 6226 RVA: 0x00058F41 File Offset: 0x00057141
		private void OnDisable()
		{
			ItemUIUtilities.OnSelectionChanged -= this.OnSelectionChanged;
		}

		// Token: 0x06001853 RID: 6227 RVA: 0x00058F54 File Offset: 0x00057154
		private void OnSelectionChanged()
		{
		}

		// Token: 0x06001854 RID: 6228 RVA: 0x00058F56 File Offset: 0x00057156
		public void Hide()
		{
			this.fadeGroup.Hide();
		}

		// Token: 0x040011C1 RID: 4545
		[SerializeField]
		private GardenView master;

		// Token: 0x040011C2 RID: 4546
		[SerializeField]
		private FadeGroup fadeGroup;

		// Token: 0x040011C3 RID: 4547
		[SerializeField]
		private Button btnConfirm;

		// Token: 0x040011C4 RID: 4548
		[SerializeField]
		private InventoryDisplay playerInventoryDisplay;

		// Token: 0x040011C5 RID: 4549
		[SerializeField]
		private InventoryDisplay storageInventoryDisplay;
	}
}
