using System;
using Duckov.UI;
using ItemStatsSystem;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Fishing.UI
{
	// Token: 0x02000215 RID: 533
	public class BaitSelectPanelEntry : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
	{
		// Token: 0x170002DD RID: 733
		// (get) Token: 0x06000FE3 RID: 4067 RVA: 0x0003E339 File Offset: 0x0003C539
		public Item Target
		{
			get
			{
				return this.targetItem;
			}
		}

		// Token: 0x170002DE RID: 734
		// (get) Token: 0x06000FE4 RID: 4068 RVA: 0x0003E341 File Offset: 0x0003C541
		private bool Selected
		{
			get
			{
				return !(this.master == null) && this.master.GetSelection() == this;
			}
		}

		// Token: 0x06000FE5 RID: 4069 RVA: 0x0003E364 File Offset: 0x0003C564
		internal void Setup(BaitSelectPanel master, Item cur)
		{
			this.UnregisterEvents();
			this.master = master;
			this.targetItem = cur;
			this.itemDisplay.Setup(this.targetItem);
			this.RegisterEvents();
			this.Refresh();
		}

		// Token: 0x06000FE6 RID: 4070 RVA: 0x0003E397 File Offset: 0x0003C597
		private void RegisterEvents()
		{
			if (this.master == null)
			{
				return;
			}
			this.master.onSetSelection += this.Refresh;
		}

		// Token: 0x06000FE7 RID: 4071 RVA: 0x0003E3BF File Offset: 0x0003C5BF
		private void UnregisterEvents()
		{
			if (this.master == null)
			{
				return;
			}
			this.master.onSetSelection -= this.Refresh;
		}

		// Token: 0x06000FE8 RID: 4072 RVA: 0x0003E3E7 File Offset: 0x0003C5E7
		private void Refresh()
		{
			this.selectedIndicator.SetActive(this.Selected);
		}

		// Token: 0x06000FE9 RID: 4073 RVA: 0x0003E3FA File Offset: 0x0003C5FA
		private void Awake()
		{
			this.itemDisplay.onPointerClick += this.OnPointerClick;
		}

		// Token: 0x06000FEA RID: 4074 RVA: 0x0003E413 File Offset: 0x0003C613
		public void OnPointerClick(PointerEventData eventData)
		{
			eventData.Use();
			this.master.NotifySelect(this);
		}

		// Token: 0x06000FEB RID: 4075 RVA: 0x0003E427 File Offset: 0x0003C627
		private void OnPointerClick(ItemDisplay display, PointerEventData data)
		{
			this.OnPointerClick(data);
		}

		// Token: 0x04000CC2 RID: 3266
		[SerializeField]
		private GameObject selectedIndicator;

		// Token: 0x04000CC3 RID: 3267
		[SerializeField]
		private ItemDisplay itemDisplay;

		// Token: 0x04000CC4 RID: 3268
		private BaitSelectPanel master;

		// Token: 0x04000CC5 RID: 3269
		private Item targetItem;
	}
}
