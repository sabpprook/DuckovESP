using System;
using ItemStatsSystem;
using SodaCraft.Localizations;
using SodaCraft.StringUtilities;
using UnityEngine;

namespace Duckov.Quests.Tasks
{
	// Token: 0x02000354 RID: 852
	public class QuestTask_UseItem : Task
	{
		// Token: 0x170005A4 RID: 1444
		// (get) Token: 0x06001DB2 RID: 7602 RVA: 0x0006975A File Offset: 0x0006795A
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

		// Token: 0x170005A5 RID: 1445
		// (get) Token: 0x06001DB3 RID: 7603 RVA: 0x0006978A File Offset: 0x0006798A
		private string descriptionFormatKey
		{
			get
			{
				return "Task_UseItem";
			}
		}

		// Token: 0x170005A6 RID: 1446
		// (get) Token: 0x06001DB4 RID: 7604 RVA: 0x00069791 File Offset: 0x00067991
		private string DescriptionFormat
		{
			get
			{
				return this.descriptionFormatKey.ToPlainText();
			}
		}

		// Token: 0x170005A7 RID: 1447
		// (get) Token: 0x06001DB5 RID: 7605 RVA: 0x000697A0 File Offset: 0x000679A0
		private string ItemDisplayName
		{
			get
			{
				return this.CachedMeta.DisplayName;
			}
		}

		// Token: 0x170005A8 RID: 1448
		// (get) Token: 0x06001DB6 RID: 7606 RVA: 0x000697BB File Offset: 0x000679BB
		public override string Description
		{
			get
			{
				return this.DescriptionFormat.Format(new { this.ItemDisplayName, this.amount, this.requireAmount });
			}
		}

		// Token: 0x170005A9 RID: 1449
		// (get) Token: 0x06001DB7 RID: 7607 RVA: 0x000697DF File Offset: 0x000679DF
		public override Sprite Icon
		{
			get
			{
				return this.CachedMeta.icon;
			}
		}

		// Token: 0x06001DB8 RID: 7608 RVA: 0x000697EC File Offset: 0x000679EC
		private void OnEnable()
		{
			Item.onUseStatic += this.OnItemUsed;
			LevelManager.OnLevelInitialized += this.OnLevelInitialized;
		}

		// Token: 0x06001DB9 RID: 7609 RVA: 0x00069810 File Offset: 0x00067A10
		private void OnDisable()
		{
			Item.onUseStatic -= this.OnItemUsed;
			LevelManager.OnLevelInitialized -= this.OnLevelInitialized;
		}

		// Token: 0x06001DBA RID: 7610 RVA: 0x00069834 File Offset: 0x00067A34
		private void OnLevelInitialized()
		{
			if (this.resetOnLevelInitialized)
			{
				this.amount = 0;
			}
		}

		// Token: 0x06001DBB RID: 7611 RVA: 0x00069845 File Offset: 0x00067A45
		private void OnItemUsed(Item item, object user)
		{
			if (!LevelManager.Instance)
			{
				return;
			}
			if (user as CharacterMainControl == LevelManager.Instance.MainCharacter && item.TypeID == this.itemTypeID)
			{
				this.AddCount();
			}
		}

		// Token: 0x06001DBC RID: 7612 RVA: 0x0006987F File Offset: 0x00067A7F
		private void AddCount()
		{
			if (this.amount < this.requireAmount)
			{
				this.amount++;
				base.ReportStatusChanged();
			}
		}

		// Token: 0x06001DBD RID: 7613 RVA: 0x000698A3 File Offset: 0x00067AA3
		public override object GenerateSaveData()
		{
			return this.amount;
		}

		// Token: 0x06001DBE RID: 7614 RVA: 0x000698B0 File Offset: 0x00067AB0
		protected override bool CheckFinished()
		{
			return this.amount >= this.requireAmount;
		}

		// Token: 0x06001DBF RID: 7615 RVA: 0x000698C4 File Offset: 0x00067AC4
		public override void SetupSaveData(object data)
		{
			if (data is int)
			{
				int num = (int)data;
				this.amount = num;
			}
		}

		// Token: 0x04001479 RID: 5241
		[SerializeField]
		private int requireAmount = 1;

		// Token: 0x0400147A RID: 5242
		[ItemTypeID]
		[SerializeField]
		private int itemTypeID;

		// Token: 0x0400147B RID: 5243
		[SerializeField]
		private bool resetOnLevelInitialized;

		// Token: 0x0400147C RID: 5244
		[SerializeField]
		private int amount;

		// Token: 0x0400147D RID: 5245
		private ItemMetaData? _cachedMeta;
	}
}
