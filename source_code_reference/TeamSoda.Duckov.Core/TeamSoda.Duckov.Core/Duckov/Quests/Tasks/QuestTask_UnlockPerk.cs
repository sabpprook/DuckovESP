using System;
using System.Collections.Generic;
using Duckov.PerkTrees;
using SodaCraft.Localizations;
using SodaCraft.StringUtilities;
using UnityEngine;

namespace Duckov.Quests.Tasks
{
	// Token: 0x02000353 RID: 851
	public class QuestTask_UnlockPerk : Task
	{
		// Token: 0x170005A0 RID: 1440
		// (get) Token: 0x06001DA5 RID: 7589 RVA: 0x000694DE File Offset: 0x000676DE
		private string DescriptionFormat
		{
			get
			{
				return this.descriptionFormatKey.ToPlainText();
			}
		}

		// Token: 0x170005A1 RID: 1441
		// (get) Token: 0x06001DA6 RID: 7590 RVA: 0x000694EB File Offset: 0x000676EB
		private string PerkDisplayName
		{
			get
			{
				if (this.perk == null)
				{
					this.BindPerk();
				}
				if (this.perk == null)
				{
					return this.perkObjectName.ToPlainText();
				}
				return this.perk.DisplayName;
			}
		}

		// Token: 0x170005A2 RID: 1442
		// (get) Token: 0x06001DA7 RID: 7591 RVA: 0x00069527 File Offset: 0x00067727
		public override string Description
		{
			get
			{
				return this.DescriptionFormat.Format(new { this.PerkDisplayName });
			}
		}

		// Token: 0x170005A3 RID: 1443
		// (get) Token: 0x06001DA8 RID: 7592 RVA: 0x0006953F File Offset: 0x0006773F
		public override Sprite Icon
		{
			get
			{
				if (this.perk != null)
				{
					return this.perk.Icon;
				}
				return null;
			}
		}

		// Token: 0x06001DA9 RID: 7593 RVA: 0x0006955C File Offset: 0x0006775C
		protected override void OnInit()
		{
			if (LevelManager.LevelInited)
			{
				this.BindPerk();
				return;
			}
			LevelManager.OnLevelInitialized += this.OnLevelInitialized;
		}

		// Token: 0x06001DAA RID: 7594 RVA: 0x00069580 File Offset: 0x00067780
		private bool BindPerk()
		{
			if (this.perk)
			{
				if (!this.unlocked && this.perk.Unlocked)
				{
					this.OnPerkUnlockStateChanged(this.perk, true);
				}
				return false;
			}
			PerkTree perkTree = PerkTreeManager.GetPerkTree(this.perkTreeID);
			if (perkTree)
			{
				using (List<Perk>.Enumerator enumerator = perkTree.perks.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Perk perk = enumerator.Current;
						if (perk.gameObject.name == this.perkObjectName)
						{
							this.perk = perk;
							if (this.perk.Unlocked)
							{
								this.OnPerkUnlockStateChanged(this.perk, true);
							}
							this.perk.onUnlockStateChanged += this.OnPerkUnlockStateChanged;
							return true;
						}
					}
					goto IL_00E6;
				}
			}
			Debug.LogError("PerkTree Not Found " + this.perkTreeID, base.gameObject);
			IL_00E6:
			Debug.LogError("Perk Not Found: " + this.perkTreeID + "/" + this.perkObjectName, base.gameObject);
			return false;
		}

		// Token: 0x06001DAB RID: 7595 RVA: 0x000696AC File Offset: 0x000678AC
		private void OnPerkUnlockStateChanged(Perk _perk, bool _unlocked)
		{
			if (base.Master.Complete)
			{
				return;
			}
			if (_unlocked)
			{
				this.unlocked = true;
				base.ReportStatusChanged();
			}
		}

		// Token: 0x06001DAC RID: 7596 RVA: 0x000696CC File Offset: 0x000678CC
		private void OnDestroy()
		{
			if (this.perk)
			{
				this.perk.onUnlockStateChanged -= this.OnPerkUnlockStateChanged;
			}
			LevelManager.OnLevelInitialized -= this.OnLevelInitialized;
		}

		// Token: 0x06001DAD RID: 7597 RVA: 0x00069703 File Offset: 0x00067903
		private void OnLevelInitialized()
		{
			this.BindPerk();
		}

		// Token: 0x06001DAE RID: 7598 RVA: 0x0006970C File Offset: 0x0006790C
		public override object GenerateSaveData()
		{
			return this.unlocked;
		}

		// Token: 0x06001DAF RID: 7599 RVA: 0x00069719 File Offset: 0x00067919
		protected override bool CheckFinished()
		{
			return this.unlocked;
		}

		// Token: 0x06001DB0 RID: 7600 RVA: 0x00069724 File Offset: 0x00067924
		public override void SetupSaveData(object data)
		{
			if (data is bool)
			{
				bool flag = (bool)data;
				this.unlocked = flag;
			}
		}

		// Token: 0x04001474 RID: 5236
		[SerializeField]
		private string perkTreeID;

		// Token: 0x04001475 RID: 5237
		[SerializeField]
		private string perkObjectName;

		// Token: 0x04001476 RID: 5238
		private Perk perk;

		// Token: 0x04001477 RID: 5239
		[NonSerialized]
		private bool unlocked;

		// Token: 0x04001478 RID: 5240
		private string descriptionFormatKey = "Task_UnlockPerk";
	}
}
