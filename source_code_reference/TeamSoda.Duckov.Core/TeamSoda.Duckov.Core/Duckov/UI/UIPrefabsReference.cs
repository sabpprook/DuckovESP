using System;
using UnityEngine;

namespace Duckov.UI
{
	// Token: 0x02000372 RID: 882
	[CreateAssetMenu]
	public class UIPrefabsReference : ScriptableObject
	{
		// Token: 0x170005E2 RID: 1506
		// (get) Token: 0x06001E75 RID: 7797 RVA: 0x0006B219 File Offset: 0x00069419
		public ItemDisplay ItemDisplay
		{
			get
			{
				return this.itemDisplay;
			}
		}

		// Token: 0x170005E3 RID: 1507
		// (get) Token: 0x06001E76 RID: 7798 RVA: 0x0006B221 File Offset: 0x00069421
		public SlotIndicator SlotIndicator
		{
			get
			{
				return this.slotIndicator;
			}
		}

		// Token: 0x170005E4 RID: 1508
		// (get) Token: 0x06001E77 RID: 7799 RVA: 0x0006B229 File Offset: 0x00069429
		public SlotDisplay SlotDisplay
		{
			get
			{
				return this.slotDisplay;
			}
		}

		// Token: 0x170005E5 RID: 1509
		// (get) Token: 0x06001E78 RID: 7800 RVA: 0x0006B231 File Offset: 0x00069431
		public InventoryEntry InventoryEntry
		{
			get
			{
				return this.inventoryEntry;
			}
		}

		// Token: 0x040014D5 RID: 5333
		[SerializeField]
		private ItemDisplay itemDisplay;

		// Token: 0x040014D6 RID: 5334
		[SerializeField]
		private SlotIndicator slotIndicator;

		// Token: 0x040014D7 RID: 5335
		[SerializeField]
		private SlotDisplay slotDisplay;

		// Token: 0x040014D8 RID: 5336
		[SerializeField]
		private InventoryEntry inventoryEntry;
	}
}
