using System;
using ItemStatsSystem;
using UnityEngine.Serialization;

namespace Duckov.ItemUsage
{
	// Token: 0x02000369 RID: 873
	public class RemoveBuff : UsageBehavior
	{
		// Token: 0x06001E44 RID: 7748 RVA: 0x0006AA40 File Offset: 0x00068C40
		public override bool CanBeUsed(Item item, object user)
		{
			if (!item)
			{
				return false;
			}
			if (this.useDurability && item.Durability < (float)this.durabilityUsage)
			{
				return false;
			}
			CharacterMainControl characterMainControl = user as CharacterMainControl;
			return !(characterMainControl == null) && characterMainControl.HasBuff(this.buffID);
		}

		// Token: 0x06001E45 RID: 7749 RVA: 0x0006AA90 File Offset: 0x00068C90
		protected override void OnUse(Item item, object user)
		{
			CharacterMainControl characterMainControl = user as CharacterMainControl;
			if (characterMainControl == null)
			{
				return;
			}
			if (!this.litmitRemoveLayerCount)
			{
				characterMainControl.RemoveBuff(this.buffID, false);
			}
			for (int i = 0; i < this.removeLayerCount; i++)
			{
				characterMainControl.RemoveBuff(this.buffID, this.litmitRemoveLayerCount);
			}
			if (this.useDurability && item.Durability > 0f)
			{
				item.Durability -= (float)this.durabilityUsage;
			}
		}

		// Token: 0x040014AC RID: 5292
		public int buffID;

		// Token: 0x040014AD RID: 5293
		[FormerlySerializedAs("removeOneLayer")]
		public bool litmitRemoveLayerCount;

		// Token: 0x040014AE RID: 5294
		public int removeLayerCount = 2;

		// Token: 0x040014AF RID: 5295
		public bool useDurability;

		// Token: 0x040014B0 RID: 5296
		public int durabilityUsage = 1;
	}
}
