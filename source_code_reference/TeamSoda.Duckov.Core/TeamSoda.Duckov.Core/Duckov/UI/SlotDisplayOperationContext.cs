using System;

namespace Duckov.UI
{
	// Token: 0x0200039A RID: 922
	public struct SlotDisplayOperationContext
	{
		// Token: 0x060020E2 RID: 8418 RVA: 0x00072E58 File Offset: 0x00071058
		public SlotDisplayOperationContext(SlotDisplay slotDisplay, SlotDisplayOperationContext.Operation operation, bool succeed)
		{
			this.slotDisplay = slotDisplay;
			this.operation = operation;
			this.succeed = succeed;
		}

		// Token: 0x0400165B RID: 5723
		public SlotDisplay slotDisplay;

		// Token: 0x0400165C RID: 5724
		public SlotDisplayOperationContext.Operation operation;

		// Token: 0x0400165D RID: 5725
		public bool succeed;

		// Token: 0x02000618 RID: 1560
		public enum Operation
		{
			// Token: 0x04002198 RID: 8600
			None,
			// Token: 0x04002199 RID: 8601
			Equip,
			// Token: 0x0400219A RID: 8602
			Unequip,
			// Token: 0x0400219B RID: 8603
			Deny
		}
	}
}
