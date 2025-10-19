using System;
using Duckov.Utilities;
using ItemStatsSystem.Items;
using UnityEngine;

namespace Duckov.UI
{
	// Token: 0x0200039B RID: 923
	public class SlotIndicator : MonoBehaviour, IPoolable
	{
		// Token: 0x17000651 RID: 1617
		// (get) Token: 0x060020E3 RID: 8419 RVA: 0x00072E6F File Offset: 0x0007106F
		// (set) Token: 0x060020E4 RID: 8420 RVA: 0x00072E77 File Offset: 0x00071077
		public Slot Target { get; private set; }

		// Token: 0x060020E5 RID: 8421 RVA: 0x00072E80 File Offset: 0x00071080
		public void Setup(Slot target)
		{
			this.UnregisterEvents();
			this.Target = target;
			this.RegisterEvents();
			this.Refresh();
		}

		// Token: 0x060020E6 RID: 8422 RVA: 0x00072E9B File Offset: 0x0007109B
		private void RegisterEvents()
		{
			if (this.Target == null)
			{
				return;
			}
			this.UnregisterEvents();
			this.Target.onSlotContentChanged += this.OnSlotContentChanged;
		}

		// Token: 0x060020E7 RID: 8423 RVA: 0x00072EC3 File Offset: 0x000710C3
		private void UnregisterEvents()
		{
			if (this.Target == null)
			{
				return;
			}
			this.Target.onSlotContentChanged -= this.OnSlotContentChanged;
		}

		// Token: 0x060020E8 RID: 8424 RVA: 0x00072EE5 File Offset: 0x000710E5
		private void OnSlotContentChanged(Slot slot)
		{
			if (slot != this.Target)
			{
				Debug.LogError("Slot内容改变事件触发了，但它来自别的Slot。这说明Slot Indicator注册的事件发生了泄露，请检查代码。");
				return;
			}
			this.Refresh();
		}

		// Token: 0x060020E9 RID: 8425 RVA: 0x00072F01 File Offset: 0x00071101
		private void Refresh()
		{
			if (this.contentIndicator == null)
			{
				return;
			}
			if (this.Target == null)
			{
				return;
			}
			this.contentIndicator.SetActive(this.Target.Content);
		}

		// Token: 0x060020EA RID: 8426 RVA: 0x00072F36 File Offset: 0x00071136
		public void NotifyPooled()
		{
			this.RegisterEvents();
			this.Refresh();
		}

		// Token: 0x060020EB RID: 8427 RVA: 0x00072F44 File Offset: 0x00071144
		public void NotifyReleased()
		{
			this.UnregisterEvents();
			this.Target = null;
			this.contentIndicator.SetActive(false);
		}

		// Token: 0x060020EC RID: 8428 RVA: 0x00072F5F File Offset: 0x0007115F
		private void OnEnable()
		{
			this.RegisterEvents();
			this.Refresh();
		}

		// Token: 0x060020ED RID: 8429 RVA: 0x00072F6D File Offset: 0x0007116D
		private void OnDisable()
		{
			this.UnregisterEvents();
		}

		// Token: 0x0400165F RID: 5727
		[SerializeField]
		private GameObject contentIndicator;
	}
}
