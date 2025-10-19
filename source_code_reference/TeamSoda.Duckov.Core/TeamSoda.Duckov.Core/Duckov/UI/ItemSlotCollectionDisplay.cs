using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.CompilerServices;
using Duckov.Utilities;
using ItemStatsSystem;
using ItemStatsSystem.Items;
using UnityEngine;

namespace Duckov.UI
{
	// Token: 0x02000398 RID: 920
	public class ItemSlotCollectionDisplay : MonoBehaviour
	{
		// Token: 0x17000646 RID: 1606
		// (get) Token: 0x0600209E RID: 8350 RVA: 0x00071ED3 File Offset: 0x000700D3
		// (set) Token: 0x0600209F RID: 8351 RVA: 0x00071EDB File Offset: 0x000700DB
		public bool Editable
		{
			get
			{
				return this.editable;
			}
			internal set
			{
				this.editable = value;
			}
		}

		// Token: 0x17000647 RID: 1607
		// (get) Token: 0x060020A0 RID: 8352 RVA: 0x00071EE4 File Offset: 0x000700E4
		// (set) Token: 0x060020A1 RID: 8353 RVA: 0x00071EEC File Offset: 0x000700EC
		public bool ContentSelectable
		{
			get
			{
				return this.contentSelectable;
			}
			set
			{
				this.contentSelectable = value;
			}
		}

		// Token: 0x17000648 RID: 1608
		// (get) Token: 0x060020A2 RID: 8354 RVA: 0x00071EF5 File Offset: 0x000700F5
		public bool ShowOperationMenu
		{
			get
			{
				return this.showOperationMenu;
			}
		}

		// Token: 0x17000649 RID: 1609
		// (get) Token: 0x060020A3 RID: 8355 RVA: 0x00071EFD File Offset: 0x000700FD
		// (set) Token: 0x060020A4 RID: 8356 RVA: 0x00071F05 File Offset: 0x00070105
		public bool Movable { get; private set; }

		// Token: 0x1700064A RID: 1610
		// (get) Token: 0x060020A5 RID: 8357 RVA: 0x00071F0E File Offset: 0x0007010E
		// (set) Token: 0x060020A6 RID: 8358 RVA: 0x00071F16 File Offset: 0x00070116
		public Item Target { get; private set; }

		// Token: 0x140000DB RID: 219
		// (add) Token: 0x060020A7 RID: 8359 RVA: 0x00071F20 File Offset: 0x00070120
		// (remove) Token: 0x060020A8 RID: 8360 RVA: 0x00071F58 File Offset: 0x00070158
		public event Action<ItemSlotCollectionDisplay, SlotDisplay> onElementClicked;

		// Token: 0x140000DC RID: 220
		// (add) Token: 0x060020A9 RID: 8361 RVA: 0x00071F90 File Offset: 0x00070190
		// (remove) Token: 0x060020AA RID: 8362 RVA: 0x00071FC8 File Offset: 0x000701C8
		public event Action<ItemSlotCollectionDisplay, SlotDisplay> onElementDoubleClicked;

		// Token: 0x060020AB RID: 8363 RVA: 0x00072000 File Offset: 0x00070200
		public void Setup(Item target, bool movable = false)
		{
			this.Target = target;
			this.Clear();
			if (this.Target == null)
			{
				return;
			}
			if (this.Target.Slots == null)
			{
				return;
			}
			this.Movable = movable;
			for (int i = 0; i < this.Target.Slots.Count; i++)
			{
				Slot slot = this.Target.Slots[i];
				if (slot != null)
				{
					SlotDisplay slotDisplay = SlotDisplay.Get();
					slotDisplay.onSlotDisplayClicked += this.OnSlotDisplayClicked;
					slotDisplay.onSlotDisplayDoubleClicked += this.OnSlotDisplayDoubleClicked;
					slotDisplay.ShowOperationMenu = this.ShowOperationMenu;
					slotDisplay.Setup(slot);
					slotDisplay.Editable = this.editable;
					slotDisplay.ContentSelectable = this.contentSelectable;
					slotDisplay.transform.SetParent(this.entriesParent, false);
					slotDisplay.Movable = this.Movable;
					this.slots.Add(slotDisplay);
				}
			}
		}

		// Token: 0x060020AC RID: 8364 RVA: 0x000720F9 File Offset: 0x000702F9
		private void OnSlotDisplayDoubleClicked(SlotDisplay display)
		{
			Action<ItemSlotCollectionDisplay, SlotDisplay> action = this.onElementDoubleClicked;
			if (action == null)
			{
				return;
			}
			action(this, display);
		}

		// Token: 0x060020AD RID: 8365 RVA: 0x00072110 File Offset: 0x00070310
		private void Clear()
		{
			foreach (SlotDisplay slotDisplay in this.slots)
			{
				slotDisplay.onSlotDisplayClicked -= this.OnSlotDisplayClicked;
				SlotDisplay.Release(slotDisplay);
			}
			this.slots.Clear();
			this.entriesParent.DestroyAllChildren();
		}

		// Token: 0x060020AE RID: 8366 RVA: 0x00072188 File Offset: 0x00070388
		private void OnSlotDisplayClicked(SlotDisplay display)
		{
			Action<ItemSlotCollectionDisplay, SlotDisplay> action = this.onElementClicked;
			if (action != null)
			{
				action(this, display);
			}
			if (!this.editable && this.notifyNotEditable)
			{
				this.ShowNotEditableIndicator().Forget();
			}
		}

		// Token: 0x060020AF RID: 8367 RVA: 0x000721B8 File Offset: 0x000703B8
		private UniTask ShowNotEditableIndicator()
		{
			ItemSlotCollectionDisplay.<ShowNotEditableIndicator>d__36 <ShowNotEditableIndicator>d__;
			<ShowNotEditableIndicator>d__.<>t__builder = AsyncUniTaskMethodBuilder.Create();
			<ShowNotEditableIndicator>d__.<>4__this = this;
			<ShowNotEditableIndicator>d__.<>1__state = -1;
			<ShowNotEditableIndicator>d__.<>t__builder.Start<ItemSlotCollectionDisplay.<ShowNotEditableIndicator>d__36>(ref <ShowNotEditableIndicator>d__);
			return <ShowNotEditableIndicator>d__.<>t__builder.Task;
		}

		// Token: 0x060020B1 RID: 8369 RVA: 0x00072239 File Offset: 0x00070439
		[CompilerGenerated]
		private bool <ShowNotEditableIndicator>g__TokenChanged|36_0(ref ItemSlotCollectionDisplay.<>c__DisplayClass36_0 A_1)
		{
			return A_1.token != this.currentToken;
		}

		// Token: 0x0400163A RID: 5690
		[SerializeField]
		private Transform entriesParent;

		// Token: 0x0400163B RID: 5691
		[SerializeField]
		private CanvasGroup notEditableIndicator;

		// Token: 0x0400163C RID: 5692
		[SerializeField]
		private bool editable = true;

		// Token: 0x0400163D RID: 5693
		[SerializeField]
		private bool contentSelectable = true;

		// Token: 0x0400163E RID: 5694
		[SerializeField]
		private bool showOperationMenu = true;

		// Token: 0x0400163F RID: 5695
		[SerializeField]
		private bool notifyNotEditable;

		// Token: 0x04001640 RID: 5696
		[SerializeField]
		private float fadeDuration = 1f;

		// Token: 0x04001641 RID: 5697
		[SerializeField]
		private float sustainDuration = 1f;

		// Token: 0x04001644 RID: 5700
		private List<SlotDisplay> slots = new List<SlotDisplay>();

		// Token: 0x04001647 RID: 5703
		private int currentToken;
	}
}
