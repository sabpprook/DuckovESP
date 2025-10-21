using System;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.CompilerServices;
using ItemStatsSystem;
using UnityEngine;

namespace Duckov.Quests
{
	// Token: 0x02000336 RID: 822
	public class RewardItem : Reward
	{
		// Token: 0x17000543 RID: 1347
		// (get) Token: 0x06001C3A RID: 7226 RVA: 0x0006612F File Offset: 0x0006432F
		public override bool Claimed
		{
			get
			{
				return this.claimed;
			}
		}

		// Token: 0x17000544 RID: 1348
		// (get) Token: 0x06001C3B RID: 7227 RVA: 0x00066137 File Offset: 0x00064337
		public override bool Claiming
		{
			get
			{
				return this.claiming;
			}
		}

		// Token: 0x17000545 RID: 1349
		// (get) Token: 0x06001C3C RID: 7228 RVA: 0x0006613F File Offset: 0x0006433F
		private ItemMetaData CachedMeta
		{
			get
			{
				if (this._cachedMeta == null)
				{
					this._cachedMeta = new ItemMetaData?(ItemAssetsCollection.GetMetaData(this.itemTypeID));
				}
				return this._cachedMeta.Value;
			}
		}

		// Token: 0x17000546 RID: 1350
		// (get) Token: 0x06001C3D RID: 7229 RVA: 0x0006616F File Offset: 0x0006436F
		public override Sprite Icon
		{
			get
			{
				return this.CachedMeta.icon;
			}
		}

		// Token: 0x17000547 RID: 1351
		// (get) Token: 0x06001C3E RID: 7230 RVA: 0x0006617C File Offset: 0x0006437C
		public override string Description
		{
			get
			{
				return string.Format("{0} x{1}", this.CachedMeta.DisplayName, this.amount);
			}
		}

		// Token: 0x06001C3F RID: 7231 RVA: 0x000661AC File Offset: 0x000643AC
		public override object GenerateSaveData()
		{
			return this.claimed;
		}

		// Token: 0x06001C40 RID: 7232 RVA: 0x000661B9 File Offset: 0x000643B9
		public override void SetupSaveData(object data)
		{
			this.claimed = (bool)data;
		}

		// Token: 0x06001C41 RID: 7233 RVA: 0x000661C7 File Offset: 0x000643C7
		public override void OnClaim()
		{
			if (this.claimed)
			{
				return;
			}
			if (this.claiming)
			{
				return;
			}
			this.claiming = true;
			this.GenerateAndGiveItems().Forget();
		}

		// Token: 0x06001C42 RID: 7234 RVA: 0x000661F0 File Offset: 0x000643F0
		private UniTask GenerateAndGiveItems()
		{
			RewardItem.<GenerateAndGiveItems>d__18 <GenerateAndGiveItems>d__;
			<GenerateAndGiveItems>d__.<>t__builder = AsyncUniTaskMethodBuilder.Create();
			<GenerateAndGiveItems>d__.<>4__this = this;
			<GenerateAndGiveItems>d__.<>1__state = -1;
			<GenerateAndGiveItems>d__.<>t__builder.Start<RewardItem.<GenerateAndGiveItems>d__18>(ref <GenerateAndGiveItems>d__);
			return <GenerateAndGiveItems>d__.<>t__builder.Task;
		}

		// Token: 0x06001C43 RID: 7235 RVA: 0x00066233 File Offset: 0x00064433
		private void SendItemToPlayerStorage(Item item)
		{
			PlayerStorage.Push(item, true);
		}

		// Token: 0x040013BD RID: 5053
		[ItemTypeID]
		public int itemTypeID;

		// Token: 0x040013BE RID: 5054
		public int amount = 1;

		// Token: 0x040013BF RID: 5055
		private bool claimed;

		// Token: 0x040013C0 RID: 5056
		private bool claiming;

		// Token: 0x040013C1 RID: 5057
		private ItemMetaData? _cachedMeta;
	}
}
