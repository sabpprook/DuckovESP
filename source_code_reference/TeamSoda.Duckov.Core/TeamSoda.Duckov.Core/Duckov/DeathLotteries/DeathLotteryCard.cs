using System;
using Duckov.Economy;
using Duckov.UI;
using Duckov.UI.Animations;
using ItemStatsSystem;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Duckov.DeathLotteries
{
	// Token: 0x02000302 RID: 770
	public class DeathLotteryCard : MonoBehaviour, IPointerClickHandler, IEventSystemHandler, IPointerEnterHandler, IPointerExitHandler
	{
		// Token: 0x17000487 RID: 1159
		// (get) Token: 0x060018FB RID: 6395 RVA: 0x0005AD68 File Offset: 0x00058F68
		public int Index
		{
			get
			{
				return this.index;
			}
		}

		// Token: 0x060018FC RID: 6396 RVA: 0x0005AD70 File Offset: 0x00058F70
		public void OnPointerClick(PointerEventData eventData)
		{
			if (this.master == null)
			{
				return;
			}
			if (this.master.Target == null)
			{
				return;
			}
			DeathLottery.OptionalCosts cost = this.master.Target.GetCost();
			this.master.NotifyEntryClicked(this, cost.costA);
		}

		// Token: 0x060018FD RID: 6397 RVA: 0x0005ADC4 File Offset: 0x00058FC4
		public void Setup(DeathLotteryVIew master, int index)
		{
			if (master == null)
			{
				return;
			}
			if (master.Target == null)
			{
				return;
			}
			this.master = master;
			this.targetItem = master.Target.ItemInstances[index];
			this.index = index;
			this.itemDisplay.Setup(this.targetItem);
			this.cardDisplay.SetFacing(master.Target.CurrentStatus.selectedItems.Contains(index), true);
			this.Refresh();
		}

		// Token: 0x060018FE RID: 6398 RVA: 0x0005AE48 File Offset: 0x00059048
		public void NotifyFacing(bool uncovered)
		{
			this.cardDisplay.SetFacing(uncovered, false);
			this.Refresh();
		}

		// Token: 0x17000488 RID: 1160
		// (get) Token: 0x060018FF RID: 6399 RVA: 0x0005AE60 File Offset: 0x00059060
		private bool Selected
		{
			get
			{
				return !(this.master == null) && !(this.master.Target == null) && this.master.Target.CurrentStatus.selectedItems != null && this.master.Target.CurrentStatus.selectedItems.Contains(this.Index);
			}
		}

		// Token: 0x06001900 RID: 6400 RVA: 0x0005AECB File Offset: 0x000590CB
		private void Refresh()
		{
			this.selectedIndicator.SetActive(this.Selected);
		}

		// Token: 0x06001901 RID: 6401 RVA: 0x0005AEDE File Offset: 0x000590DE
		private void Awake()
		{
			this.costFade.Hide();
		}

		// Token: 0x06001902 RID: 6402 RVA: 0x0005AEEC File Offset: 0x000590EC
		public void OnPointerEnter(PointerEventData eventData)
		{
			if (this.master.Target.CurrentStatus.SelectedCount >= this.master.Target.MaxChances)
			{
				return;
			}
			Cost costA = this.master.Target.GetCost().costA;
			this.costDisplay.Setup(costA, 1);
			this.freeIndicator.SetActive(costA.IsFree);
			this.costFade.Show();
		}

		// Token: 0x06001903 RID: 6403 RVA: 0x0005AF64 File Offset: 0x00059164
		public void OnPointerExit(PointerEventData eventData)
		{
			this.costFade.Hide();
		}

		// Token: 0x04001229 RID: 4649
		[SerializeField]
		private CardDisplay cardDisplay;

		// Token: 0x0400122A RID: 4650
		[SerializeField]
		private ItemDisplay itemDisplay;

		// Token: 0x0400122B RID: 4651
		[SerializeField]
		private CostDisplay costDisplay;

		// Token: 0x0400122C RID: 4652
		[SerializeField]
		private GameObject freeIndicator;

		// Token: 0x0400122D RID: 4653
		[SerializeField]
		private FadeGroup costFade;

		// Token: 0x0400122E RID: 4654
		[SerializeField]
		private GameObject selectedIndicator;

		// Token: 0x0400122F RID: 4655
		private DeathLotteryVIew master;

		// Token: 0x04001230 RID: 4656
		private int index;

		// Token: 0x04001231 RID: 4657
		private Item targetItem;
	}
}
