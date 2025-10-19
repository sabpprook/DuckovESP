using System;
using System.Collections.Generic;
using Duckov.Scenes;
using Duckov.Utilities;
using Eflatun.SceneReference;
using ItemStatsSystem;
using SodaCraft.Localizations;
using SodaCraft.StringUtilities;
using UnityEngine;

namespace Duckov.Quests.Tasks
{
	// Token: 0x0200034F RID: 847
	public class QuestTask_KillCount : Task
	{
		// Token: 0x1700057F RID: 1407
		// (get) Token: 0x06001D58 RID: 7512 RVA: 0x00068CAF File Offset: 0x00066EAF
		// (set) Token: 0x06001D59 RID: 7513 RVA: 0x00068CB6 File Offset: 0x00066EB6
		[LocalizationKey("TasksAndRewards")]
		private string defaultEnemyNameKey
		{
			get
			{
				return "Task_Desc_AnyEnemy";
			}
			set
			{
			}
		}

		// Token: 0x17000580 RID: 1408
		// (get) Token: 0x06001D5A RID: 7514 RVA: 0x00068CB8 File Offset: 0x00066EB8
		// (set) Token: 0x06001D5B RID: 7515 RVA: 0x00068CBF File Offset: 0x00066EBF
		[LocalizationKey("TasksAndRewards")]
		private string defaultWeaponNameKey
		{
			get
			{
				return "Task_Desc_AnyWeapon";
			}
			set
			{
			}
		}

		// Token: 0x17000581 RID: 1409
		// (get) Token: 0x06001D5C RID: 7516 RVA: 0x00068CC4 File Offset: 0x00066EC4
		private string weaponName
		{
			get
			{
				if (this.withWeapon)
				{
					return ItemAssetsCollection.GetMetaData(this.weaponTypeID).DisplayName;
				}
				return this.defaultWeaponNameKey.ToPlainText();
			}
		}

		// Token: 0x17000582 RID: 1410
		// (get) Token: 0x06001D5D RID: 7517 RVA: 0x00068CF8 File Offset: 0x00066EF8
		private string enemyName
		{
			get
			{
				if (this.requireEnemyType == null)
				{
					return this.defaultEnemyNameKey.ToPlainText();
				}
				return this.requireEnemyType.DisplayName;
			}
		}

		// Token: 0x17000583 RID: 1411
		// (get) Token: 0x06001D5E RID: 7518 RVA: 0x00068D1F File Offset: 0x00066F1F
		// (set) Token: 0x06001D5F RID: 7519 RVA: 0x00068D26 File Offset: 0x00066F26
		[LocalizationKey("TasksAndRewards")]
		private string descriptionFormatKey
		{
			get
			{
				return "Task_KillCount";
			}
			set
			{
			}
		}

		// Token: 0x17000584 RID: 1412
		// (get) Token: 0x06001D60 RID: 7520 RVA: 0x00068D28 File Offset: 0x00066F28
		// (set) Token: 0x06001D61 RID: 7521 RVA: 0x00068D2F File Offset: 0x00066F2F
		[LocalizationKey("TasksAndRewards")]
		private string withWeaponDescriptionFormatKey
		{
			get
			{
				return "Task_Desc_WithWeapon";
			}
			set
			{
			}
		}

		// Token: 0x17000585 RID: 1413
		// (get) Token: 0x06001D62 RID: 7522 RVA: 0x00068D31 File Offset: 0x00066F31
		// (set) Token: 0x06001D63 RID: 7523 RVA: 0x00068D38 File Offset: 0x00066F38
		[LocalizationKey("TasksAndRewards")]
		private string requireSceneDescriptionFormatKey
		{
			get
			{
				return "Task_Desc_RequireScene";
			}
			set
			{
			}
		}

		// Token: 0x17000586 RID: 1414
		// (get) Token: 0x06001D64 RID: 7524 RVA: 0x00068D3A File Offset: 0x00066F3A
		// (set) Token: 0x06001D65 RID: 7525 RVA: 0x00068D41 File Offset: 0x00066F41
		[LocalizationKey("TasksAndRewards")]
		private string RequireHeadShotDescriptionKey
		{
			get
			{
				return "Task_Desc_RequireHeadShot";
			}
			set
			{
			}
		}

		// Token: 0x17000587 RID: 1415
		// (get) Token: 0x06001D66 RID: 7526 RVA: 0x00068D43 File Offset: 0x00066F43
		// (set) Token: 0x06001D67 RID: 7527 RVA: 0x00068D4A File Offset: 0x00066F4A
		[LocalizationKey("TasksAndRewards")]
		private string WithoutHeadShotDescriptionKey
		{
			get
			{
				return "Task_Desc_WithoutHeadShot";
			}
			set
			{
			}
		}

		// Token: 0x17000588 RID: 1416
		// (get) Token: 0x06001D68 RID: 7528 RVA: 0x00068D4C File Offset: 0x00066F4C
		// (set) Token: 0x06001D69 RID: 7529 RVA: 0x00068D53 File Offset: 0x00066F53
		[LocalizationKey("TasksAndRewards")]
		private string RequireBuffDescriptionFormatKey
		{
			get
			{
				return "Task_Desc_WithBuff";
			}
			set
			{
			}
		}

		// Token: 0x17000589 RID: 1417
		// (get) Token: 0x06001D6A RID: 7530 RVA: 0x00068D55 File Offset: 0x00066F55
		private string DescriptionFormat
		{
			get
			{
				return this.descriptionFormatKey.ToPlainText();
			}
		}

		// Token: 0x1700058A RID: 1418
		// (get) Token: 0x06001D6B RID: 7531 RVA: 0x00068D64 File Offset: 0x00066F64
		public override string[] ExtraDescriptsions
		{
			get
			{
				List<string> list = new List<string>();
				if (this.withWeapon)
				{
					list.Add(this.WithWeaponDescription);
				}
				if (!string.IsNullOrEmpty(this.requireSceneID))
				{
					list.Add(this.RequireSceneDescription);
				}
				if (this.requireHeadShot)
				{
					list.Add(this.RequireHeadShotDescription);
				}
				if (this.withoutHeadShot)
				{
					list.Add(this.WithoutHeadShotDescription);
				}
				if (this.requireBuff)
				{
					list.Add(this.RequireBuffDescription);
				}
				return list.ToArray();
			}
		}

		// Token: 0x1700058B RID: 1419
		// (get) Token: 0x06001D6C RID: 7532 RVA: 0x00068DE6 File Offset: 0x00066FE6
		private string WithWeaponDescription
		{
			get
			{
				return this.withWeaponDescriptionFormatKey.ToPlainText().Format(new { this.weaponName });
			}
		}

		// Token: 0x1700058C RID: 1420
		// (get) Token: 0x06001D6D RID: 7533 RVA: 0x00068E03 File Offset: 0x00067003
		private string RequireSceneDescription
		{
			get
			{
				return this.requireSceneDescriptionFormatKey.ToPlainText().Format(new { this.requireSceneName });
			}
		}

		// Token: 0x1700058D RID: 1421
		// (get) Token: 0x06001D6E RID: 7534 RVA: 0x00068E20 File Offset: 0x00067020
		private string RequireHeadShotDescription
		{
			get
			{
				return this.RequireHeadShotDescriptionKey.ToPlainText();
			}
		}

		// Token: 0x1700058E RID: 1422
		// (get) Token: 0x06001D6F RID: 7535 RVA: 0x00068E2D File Offset: 0x0006702D
		private string WithoutHeadShotDescription
		{
			get
			{
				return this.WithoutHeadShotDescriptionKey.ToPlainText();
			}
		}

		// Token: 0x1700058F RID: 1423
		// (get) Token: 0x06001D70 RID: 7536 RVA: 0x00068E3C File Offset: 0x0006703C
		private string RequireBuffDescription
		{
			get
			{
				string buffDisplayName = GameplayDataSettings.Buffs.GetBuffDisplayName(this.requireBuffID);
				return this.RequireBuffDescriptionFormatKey.ToPlainText().Format(new
				{
					buffName = buffDisplayName
				});
			}
		}

		// Token: 0x17000590 RID: 1424
		// (get) Token: 0x06001D71 RID: 7537 RVA: 0x00068E70 File Offset: 0x00067070
		public override string Description
		{
			get
			{
				return this.DescriptionFormat.Format(new { this.weaponName, this.enemyName, this.requireAmount, this.amount, this.requireSceneName });
			}
		}

		// Token: 0x17000591 RID: 1425
		// (get) Token: 0x06001D72 RID: 7538 RVA: 0x00068EA0 File Offset: 0x000670A0
		public SceneInfoEntry RequireSceneInfo
		{
			get
			{
				return SceneInfoCollection.GetSceneInfo(this.requireSceneID);
			}
		}

		// Token: 0x17000592 RID: 1426
		// (get) Token: 0x06001D73 RID: 7539 RVA: 0x00068EB0 File Offset: 0x000670B0
		public SceneReference RequireScene
		{
			get
			{
				SceneInfoEntry requireSceneInfo = this.RequireSceneInfo;
				if (requireSceneInfo == null)
				{
					return null;
				}
				return requireSceneInfo.SceneReference;
			}
		}

		// Token: 0x17000593 RID: 1427
		// (get) Token: 0x06001D74 RID: 7540 RVA: 0x00068ECF File Offset: 0x000670CF
		public string requireSceneName
		{
			get
			{
				if (string.IsNullOrEmpty(this.requireSceneID))
				{
					return "Task_Desc_AnyScene".ToPlainText();
				}
				return this.RequireSceneInfo.DisplayName;
			}
		}

		// Token: 0x17000594 RID: 1428
		// (get) Token: 0x06001D75 RID: 7541 RVA: 0x00068EF4 File Offset: 0x000670F4
		public bool SceneRequirementSatisfied
		{
			get
			{
				if (string.IsNullOrEmpty(this.requireSceneID))
				{
					return true;
				}
				SceneReference requireScene = this.RequireScene;
				return requireScene == null || requireScene.UnsafeReason == SceneReferenceUnsafeReason.Empty || requireScene.UnsafeReason != SceneReferenceUnsafeReason.None || requireScene.LoadedScene.isLoaded;
			}
		}

		// Token: 0x06001D76 RID: 7542 RVA: 0x00068F3F File Offset: 0x0006713F
		private void OnEnable()
		{
			Health.OnDead += this.Health_OnDead;
			LevelManager.OnLevelInitialized += this.OnLevelInitialized;
		}

		// Token: 0x06001D77 RID: 7543 RVA: 0x00068F63 File Offset: 0x00067163
		private void OnDisable()
		{
			Health.OnDead -= this.Health_OnDead;
			LevelManager.OnLevelInitialized -= this.OnLevelInitialized;
		}

		// Token: 0x06001D78 RID: 7544 RVA: 0x00068F87 File Offset: 0x00067187
		private void OnLevelInitialized()
		{
			if (this.resetOnLevelInitialized)
			{
				this.amount = 0;
			}
		}

		// Token: 0x06001D79 RID: 7545 RVA: 0x00068F98 File Offset: 0x00067198
		private void Health_OnDead(Health health, DamageInfo info)
		{
			if (health.team == Teams.player)
			{
				return;
			}
			bool flag = false;
			CharacterMainControl fromCharacter = info.fromCharacter;
			if (fromCharacter != null && info.fromCharacter.IsMainCharacter())
			{
				flag = true;
			}
			if (!flag)
			{
				return;
			}
			if (this.withWeapon && info.fromWeaponItemID != this.weaponTypeID)
			{
				return;
			}
			if (!this.SceneRequirementSatisfied)
			{
				return;
			}
			if (this.requireHeadShot && info.crit <= 0)
			{
				return;
			}
			if (this.withoutHeadShot && info.crit > 0)
			{
				return;
			}
			if (this.requireBuff && !fromCharacter.HasBuff(this.requireBuffID))
			{
				return;
			}
			if (this.requireEnemyType != null)
			{
				CharacterMainControl characterMainControl = health.TryGetCharacter();
				if (characterMainControl == null)
				{
					return;
				}
				CharacterRandomPreset characterPreset = characterMainControl.characterPreset;
				if (characterPreset == null)
				{
					return;
				}
				if (characterPreset.nameKey != this.requireEnemyType.nameKey)
				{
					return;
				}
			}
			this.AddCount();
		}

		// Token: 0x06001D7A RID: 7546 RVA: 0x0006907D File Offset: 0x0006727D
		private void AddCount()
		{
			if (this.amount < this.requireAmount)
			{
				this.amount++;
				base.ReportStatusChanged();
			}
		}

		// Token: 0x06001D7B RID: 7547 RVA: 0x000690A1 File Offset: 0x000672A1
		public override object GenerateSaveData()
		{
			return this.amount;
		}

		// Token: 0x06001D7C RID: 7548 RVA: 0x000690AE File Offset: 0x000672AE
		protected override bool CheckFinished()
		{
			return this.amount >= this.requireAmount;
		}

		// Token: 0x06001D7D RID: 7549 RVA: 0x000690C4 File Offset: 0x000672C4
		public override void SetupSaveData(object data)
		{
			if (data is int)
			{
				int num = (int)data;
				this.amount = num;
			}
		}

		// Token: 0x0400145C RID: 5212
		[SerializeField]
		private int requireAmount = 1;

		// Token: 0x0400145D RID: 5213
		[SerializeField]
		private bool resetOnLevelInitialized;

		// Token: 0x0400145E RID: 5214
		[SerializeField]
		private int amount;

		// Token: 0x0400145F RID: 5215
		[SerializeField]
		private bool withWeapon;

		// Token: 0x04001460 RID: 5216
		[SerializeField]
		[ItemTypeID]
		private int weaponTypeID;

		// Token: 0x04001461 RID: 5217
		[SerializeField]
		private bool requireHeadShot;

		// Token: 0x04001462 RID: 5218
		[SerializeField]
		private bool withoutHeadShot;

		// Token: 0x04001463 RID: 5219
		[SerializeField]
		private bool requireBuff;

		// Token: 0x04001464 RID: 5220
		[SerializeField]
		private int requireBuffID;

		// Token: 0x04001465 RID: 5221
		[SerializeField]
		private CharacterRandomPreset requireEnemyType;

		// Token: 0x04001466 RID: 5222
		[SceneID]
		[SerializeField]
		private string requireSceneID;
	}
}
