using System;
using Duckov.Utilities;
using ItemStatsSystem;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Duckov.UI
{
	// Token: 0x0200037A RID: 890
	public class ItemPickerEntry : MonoBehaviour, IPoolable
	{
		// Token: 0x06001EDC RID: 7900 RVA: 0x0006C5E5 File Offset: 0x0006A7E5
		private void Awake()
		{
			this.itemDisplay.onPointerClick += this.OnItemDisplayClicked;
		}

		// Token: 0x06001EDD RID: 7901 RVA: 0x0006C5FE File Offset: 0x0006A7FE
		private void OnDestroy()
		{
			this.itemDisplay.onPointerClick -= this.OnItemDisplayClicked;
		}

		// Token: 0x06001EDE RID: 7902 RVA: 0x0006C617 File Offset: 0x0006A817
		private void OnItemDisplayClicked(ItemDisplay display, PointerEventData eventData)
		{
			this.master.NotifyEntryClicked(this, this.target);
		}

		// Token: 0x06001EDF RID: 7903 RVA: 0x0006C62C File Offset: 0x0006A82C
		public void Setup(ItemPicker master, Item item)
		{
			this.master = master;
			this.target = item;
			if (this.target != null)
			{
				this.itemDisplay.Setup(this.target);
			}
			else
			{
				Debug.LogError("Item Picker不应当展示空的Item。");
			}
			this.itemDisplay.ShowOperationButtons = false;
		}

		// Token: 0x06001EE0 RID: 7904 RVA: 0x0006C67E File Offset: 0x0006A87E
		public void NotifyPooled()
		{
		}

		// Token: 0x06001EE1 RID: 7905 RVA: 0x0006C680 File Offset: 0x0006A880
		public void NotifyReleased()
		{
		}

		// Token: 0x04001527 RID: 5415
		[SerializeField]
		private ItemDisplay itemDisplay;

		// Token: 0x04001528 RID: 5416
		private ItemPicker master;

		// Token: 0x04001529 RID: 5417
		private Item target;
	}
}
