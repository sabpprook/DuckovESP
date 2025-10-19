using System;
using System.Collections.Generic;
using ItemStatsSystem;
using SodaCraft.Localizations;
using SodaCraft.StringUtilities;
using UnityEngine;

namespace Duckov.Quests.Tasks
{
	// Token: 0x02000355 RID: 853
	public class SubmitItems : Task
	{
		// Token: 0x170005AA RID: 1450
		// (get) Token: 0x06001DC1 RID: 7617 RVA: 0x000698F6 File Offset: 0x00067AF6
		public int ItemTypeID
		{
			get
			{
				return this.itemTypeID;
			}
		}

		// Token: 0x170005AB RID: 1451
		// (get) Token: 0x06001DC2 RID: 7618 RVA: 0x00069900 File Offset: 0x00067B00
		private ItemMetaData CachedMeta
		{
			get
			{
				if (this._cachedMeta == null || this._cachedMeta.Value.id != this.itemTypeID)
				{
					this._cachedMeta = new ItemMetaData?(ItemAssetsCollection.GetMetaData(this.itemTypeID));
				}
				return this._cachedMeta.Value;
			}
		}

		// Token: 0x170005AC RID: 1452
		// (get) Token: 0x06001DC3 RID: 7619 RVA: 0x00069953 File Offset: 0x00067B53
		private string descriptionFormatKey
		{
			get
			{
				return "Task_SubmitItems";
			}
		}

		// Token: 0x170005AD RID: 1453
		// (get) Token: 0x06001DC4 RID: 7620 RVA: 0x0006995A File Offset: 0x00067B5A
		private string DescriptionFormat
		{
			get
			{
				return this.descriptionFormatKey.ToPlainText();
			}
		}

		// Token: 0x170005AE RID: 1454
		// (get) Token: 0x06001DC5 RID: 7621 RVA: 0x00069967 File Offset: 0x00067B67
		private string havingAmountFormatKey
		{
			get
			{
				return "Task_SubmitItems_HavingAmount";
			}
		}

		// Token: 0x170005AF RID: 1455
		// (get) Token: 0x06001DC6 RID: 7622 RVA: 0x0006996E File Offset: 0x00067B6E
		private string HavingAmountFormat
		{
			get
			{
				return this.havingAmountFormatKey.ToPlainText();
			}
		}

		// Token: 0x170005B0 RID: 1456
		// (get) Token: 0x06001DC7 RID: 7623 RVA: 0x0006997C File Offset: 0x00067B7C
		public override string Description
		{
			get
			{
				string text = this.DescriptionFormat.Format(new
				{
					ItemDisplayName = this.CachedMeta.DisplayName,
					submittedAmount = this.submittedAmount,
					requiredAmount = this.requiredAmount
				});
				if (!base.IsFinished())
				{
					text = text + " " + this.HavingAmountFormat.Format(new
					{
						amount = ItemUtilities.GetItemCount(this.itemTypeID)
					});
				}
				return text;
			}
		}

		// Token: 0x170005B1 RID: 1457
		// (get) Token: 0x06001DC8 RID: 7624 RVA: 0x000699E4 File Offset: 0x00067BE4
		public override Sprite Icon
		{
			get
			{
				return this.CachedMeta.icon;
			}
		}

		// Token: 0x170005B2 RID: 1458
		// (get) Token: 0x06001DC9 RID: 7625 RVA: 0x000699F1 File Offset: 0x00067BF1
		public override bool Interactable
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170005B3 RID: 1459
		// (get) Token: 0x06001DCA RID: 7626 RVA: 0x000699F4 File Offset: 0x00067BF4
		public override bool PossibleValidInteraction
		{
			get
			{
				return this.CheckItemEnough();
			}
		}

		// Token: 0x170005B4 RID: 1460
		// (get) Token: 0x06001DCB RID: 7627 RVA: 0x000699FC File Offset: 0x00067BFC
		public override string InteractText
		{
			get
			{
				return "Task_SubmitItems_Interact".ToPlainText();
			}
		}

		// Token: 0x170005B5 RID: 1461
		// (get) Token: 0x06001DCC RID: 7628 RVA: 0x00069A08 File Offset: 0x00067C08
		public override bool NeedInspection
		{
			get
			{
				return !base.IsFinished() && this.CheckItemEnough();
			}
		}

		// Token: 0x140000CF RID: 207
		// (add) Token: 0x06001DCD RID: 7629 RVA: 0x00069A1C File Offset: 0x00067C1C
		// (remove) Token: 0x06001DCE RID: 7630 RVA: 0x00069A50 File Offset: 0x00067C50
		public static event Action<SubmitItems> onItemEnough;

		// Token: 0x06001DCF RID: 7631 RVA: 0x00069A83 File Offset: 0x00067C83
		protected override void OnInit()
		{
			base.OnInit();
			PlayerStorage.OnPlayerStorageChange += this.OnPlayerStorageChanged;
			CharacterMainControl.OnMainCharacterInventoryChangedEvent = (Action<CharacterMainControl, Inventory, int>)Delegate.Combine(CharacterMainControl.OnMainCharacterInventoryChangedEvent, new Action<CharacterMainControl, Inventory, int>(this.OnMainCharacterInventoryChanged));
			this.CheckItemEnough();
		}

		// Token: 0x06001DD0 RID: 7632 RVA: 0x00069AC3 File Offset: 0x00067CC3
		private void OnDestroy()
		{
			PlayerStorage.OnPlayerStorageChange -= this.OnPlayerStorageChanged;
			CharacterMainControl.OnMainCharacterInventoryChangedEvent = (Action<CharacterMainControl, Inventory, int>)Delegate.Remove(CharacterMainControl.OnMainCharacterInventoryChangedEvent, new Action<CharacterMainControl, Inventory, int>(this.OnMainCharacterInventoryChanged));
		}

		// Token: 0x06001DD1 RID: 7633 RVA: 0x00069AF8 File Offset: 0x00067CF8
		private void OnPlayerStorageChanged(PlayerStorage storage, Inventory inventory, int index)
		{
			if (base.Master.Complete)
			{
				return;
			}
			Item itemAt = inventory.GetItemAt(index);
			if (itemAt == null)
			{
				return;
			}
			if (itemAt.TypeID == this.itemTypeID)
			{
				this.CheckItemEnough();
			}
		}

		// Token: 0x06001DD2 RID: 7634 RVA: 0x00069B3C File Offset: 0x00067D3C
		private void OnMainCharacterInventoryChanged(CharacterMainControl control, Inventory inventory, int index)
		{
			if (base.Master.Complete)
			{
				return;
			}
			Item itemAt = inventory.GetItemAt(index);
			if (itemAt == null)
			{
				return;
			}
			if (itemAt.TypeID == this.itemTypeID)
			{
				this.CheckItemEnough();
			}
		}

		// Token: 0x06001DD3 RID: 7635 RVA: 0x00069B7E File Offset: 0x00067D7E
		private bool CheckItemEnough()
		{
			if (ItemUtilities.GetItemCount(this.itemTypeID) >= this.requiredAmount)
			{
				Action<SubmitItems> action = SubmitItems.onItemEnough;
				if (action != null)
				{
					action(this);
				}
				this.SetMapElementVisable(false);
				return true;
			}
			this.SetMapElementVisable(true);
			return false;
		}

		// Token: 0x06001DD4 RID: 7636 RVA: 0x00069BB5 File Offset: 0x00067DB5
		private void SetMapElementVisable(bool visable)
		{
			if (!this.mapElement)
			{
				return;
			}
			if (visable)
			{
				this.mapElement.name = base.Master.DisplayName;
			}
			this.mapElement.SetVisibility(visable);
		}

		// Token: 0x06001DD5 RID: 7637 RVA: 0x00069BEC File Offset: 0x00067DEC
		public void Submit(Item item)
		{
			if (item.TypeID != this.itemTypeID)
			{
				Debug.LogError("提交的物品类型与需求不一致。");
				return;
			}
			int num = this.requiredAmount - this.submittedAmount;
			if (num <= 0)
			{
				Debug.LogError("目标已达成，不需要继续提交物品");
				return;
			}
			int num2 = this.submittedAmount;
			if (num < item.StackCount)
			{
				item.StackCount -= num;
				this.submittedAmount += num;
			}
			else
			{
				foreach (Item item2 in item.GetAllChildren(false, true))
				{
					item2.Detach();
					if (!ItemUtilities.SendToPlayerCharacter(item2, false))
					{
						item2.Drop(CharacterMainControl.Main, true);
					}
				}
				item.Detach();
				item.DestroyTree();
				this.submittedAmount += item.StackCount;
			}
			Debug.Log("submission done");
			if (num2 != this.submittedAmount)
			{
				base.Master.NotifyTaskFinished(this);
			}
			base.ReportStatusChanged();
		}

		// Token: 0x06001DD6 RID: 7638 RVA: 0x00069D00 File Offset: 0x00067F00
		protected override bool CheckFinished()
		{
			return this.submittedAmount >= this.requiredAmount;
		}

		// Token: 0x06001DD7 RID: 7639 RVA: 0x00069D13 File Offset: 0x00067F13
		public override object GenerateSaveData()
		{
			return this.submittedAmount;
		}

		// Token: 0x06001DD8 RID: 7640 RVA: 0x00069D20 File Offset: 0x00067F20
		public override void SetupSaveData(object data)
		{
			this.submittedAmount = (int)data;
		}

		// Token: 0x06001DD9 RID: 7641 RVA: 0x00069D30 File Offset: 0x00067F30
		public override void Interact()
		{
			if (base.Master == null)
			{
				return;
			}
			List<Item> list = ItemUtilities.FindAllBelongsToPlayer((Item e) => e != null && e.TypeID == this.itemTypeID);
			for (int i = 0; i < list.Count; i++)
			{
				Item item = list[i];
				this.Submit(item);
				if (base.IsFinished())
				{
					break;
				}
			}
		}

		// Token: 0x0400147E RID: 5246
		[ItemTypeID]
		[SerializeField]
		private int itemTypeID;

		// Token: 0x0400147F RID: 5247
		[Range(1f, 100f)]
		[SerializeField]
		private int requiredAmount = 1;

		// Token: 0x04001480 RID: 5248
		[SerializeField]
		private int submittedAmount;

		// Token: 0x04001481 RID: 5249
		private ItemMetaData? _cachedMeta;

		// Token: 0x04001482 RID: 5250
		[SerializeField]
		private MapElementForTask mapElement;
	}
}
