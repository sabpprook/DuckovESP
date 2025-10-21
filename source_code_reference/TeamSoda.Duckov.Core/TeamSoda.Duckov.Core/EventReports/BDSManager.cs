using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using Bilibili.BDS;
using Duckov;
using Duckov.Buffs;
using Duckov.Buildings;
using Duckov.Economy;
using Duckov.MasterKeys;
using Duckov.PerkTrees;
using Duckov.Quests;
using Duckov.Rules;
using ItemStatsSystem;
using ItemStatsSystem.Items;
using Saves;
using Steamworks;
using UnityEngine;

namespace EventReports
{
	// Token: 0x02000225 RID: 549
	public class BDSManager : MonoBehaviour
	{
		// Token: 0x060010C8 RID: 4296 RVA: 0x00041010 File Offset: 0x0003F210
		private void Awake()
		{
			if (PlatformInfo.Platform == Platform.Steam)
			{
				if (SteamManager.Initialized && SteamUtils.IsSteamChinaLauncher())
				{
				}
			}
			else
			{
				string.Format("{0}", PlatformInfo.Platform);
			}
			Debug.Log("Player Info:\n" + BDSManager.PlayerInfo.GetCurrent().ToJson());
		}

		// Token: 0x060010C9 RID: 4297 RVA: 0x00041066 File Offset: 0x0003F266
		private void Start()
		{
			this.OnGameStarted();
		}

		// Token: 0x060010CA RID: 4298 RVA: 0x0004106E File Offset: 0x0003F26E
		private void OnDestroy()
		{
		}

		// Token: 0x170002F7 RID: 759
		// (get) Token: 0x060010CB RID: 4299 RVA: 0x00041070 File Offset: 0x0003F270
		private float TimeSinceLastHeartbeat
		{
			get
			{
				return Time.unscaledTime - this.lastTimeHeartbeat;
			}
		}

		// Token: 0x060010CC RID: 4300 RVA: 0x0004107E File Offset: 0x0003F27E
		private void Update()
		{
			bool isPlaying = Application.isPlaying;
		}

		// Token: 0x060010CD RID: 4301 RVA: 0x00041086 File Offset: 0x0003F286
		private void UpdateHeartbeat()
		{
			if (this.TimeSinceLastHeartbeat > 60f)
			{
				this.ReportCustomEvent(BDSManager.EventName.heartbeat, "");
				this.lastTimeHeartbeat = Time.unscaledTime;
			}
		}

		// Token: 0x060010CE RID: 4302 RVA: 0x000410B0 File Offset: 0x0003F2B0
		private void RegisterEvents()
		{
			this.UnregisterEvents();
			SavesSystem.OnSaveDeleted += this.OnSaveDeleted;
			RaidUtilities.OnNewRaid = (Action<RaidUtilities.RaidInfo>)Delegate.Combine(RaidUtilities.OnNewRaid, new Action<RaidUtilities.RaidInfo>(this.OnNewRaid));
			RaidUtilities.OnRaidEnd = (Action<RaidUtilities.RaidInfo>)Delegate.Combine(RaidUtilities.OnRaidEnd, new Action<RaidUtilities.RaidInfo>(this.OnRaidEnd));
			SceneLoader.onStartedLoadingScene += this.OnSceneLoadingStart;
			SceneLoader.onFinishedLoadingScene += this.OnSceneLoadingFinish;
			LevelManager.OnLevelInitialized += this.OnLevelInitialized;
			LevelManager.OnEvacuated += this.OnEvacuated;
			LevelManager.OnMainCharacterDead += this.OnMainCharacterDead;
			Quest.onQuestActivated += this.OnQuestActivated;
			Quest.onQuestCompleted += this.OnQuestCompleted;
			EconomyManager.OnCostPaid += this.OnCostPaid;
			EconomyManager.OnMoneyPaid += this.OnMoneyPaid;
			ItemUtilities.OnItemSentToPlayerInventory += this.OnItemSentToPlayerInventory;
			ItemUtilities.OnItemSentToPlayerStorage += this.OnItemSentToPlayerStorage;
			StockShop.OnItemPurchased += this.OnItemPurchased;
			CraftingManager.OnItemCrafted = (Action<CraftingFormula, Item>)Delegate.Combine(CraftingManager.OnItemCrafted, new Action<CraftingFormula, Item>(this.OnItemCrafted));
			CraftingManager.OnFormulaUnlocked = (Action<string>)Delegate.Combine(CraftingManager.OnFormulaUnlocked, new Action<string>(this.OnFormulaUnlocked));
			Health.OnDead += this.OnHealthDead;
			EXPManager.onLevelChanged = (Action<int, int>)Delegate.Combine(EXPManager.onLevelChanged, new Action<int, int>(this.OnLevelChanged));
			BuildingManager.OnBuildingBuiltComplex += this.OnBuildingBuilt;
			BuildingManager.OnBuildingDestroyedComplex += this.OnBuildingDestroyed;
			Perk.OnPerkUnlockConfirmed += this.OnPerkUnlockConfirmed;
			MasterKeysManager.OnMasterKeyUnlocked += this.OnMasterKeyUnlocked;
			CharacterMainControl.OnMainCharacterSlotContentChangedEvent = (Action<CharacterMainControl, Slot>)Delegate.Combine(CharacterMainControl.OnMainCharacterSlotContentChangedEvent, new Action<CharacterMainControl, Slot>(this.OnMainCharacterSlotContentChanged));
			StockShop.OnItemSoldByPlayer += this.OnItemSold;
			Reward.OnRewardClaimed += this.OnRewardClaimed;
			UsageUtilities.OnItemUsedStaticEvent += this.OnItemUsed;
			InteractableBase.OnInteractStartStaticEvent += this.OnInteractStart;
			LevelManager.OnNewGameReport += this.OnNewGameReport;
			Interact_CustomFace.OnCustomFaceStartEvent += this.OnCustomFaceStart;
			Interact_CustomFace.OnCustomFaceFinishedEvent += this.OnCustomFaceFinish;
			CheatMode.OnCheatModeStatusChanged += this.OnCheatModeStatusChanged;
		}

		// Token: 0x060010CF RID: 4303 RVA: 0x00041340 File Offset: 0x0003F540
		private void UnregisterEvents()
		{
			SavesSystem.OnSaveDeleted -= this.OnSaveDeleted;
			RaidUtilities.OnNewRaid = (Action<RaidUtilities.RaidInfo>)Delegate.Remove(RaidUtilities.OnNewRaid, new Action<RaidUtilities.RaidInfo>(this.OnNewRaid));
			RaidUtilities.OnRaidEnd = (Action<RaidUtilities.RaidInfo>)Delegate.Remove(RaidUtilities.OnRaidEnd, new Action<RaidUtilities.RaidInfo>(this.OnRaidEnd));
			SceneLoader.onStartedLoadingScene -= this.OnSceneLoadingStart;
			SceneLoader.onFinishedLoadingScene -= this.OnSceneLoadingFinish;
			LevelManager.OnLevelInitialized -= this.OnLevelInitialized;
			LevelManager.OnEvacuated -= this.OnEvacuated;
			LevelManager.OnMainCharacterDead -= this.OnMainCharacterDead;
			Quest.onQuestActivated -= this.OnQuestActivated;
			Quest.onQuestCompleted -= this.OnQuestCompleted;
			EconomyManager.OnCostPaid -= this.OnCostPaid;
			EconomyManager.OnMoneyPaid -= this.OnMoneyPaid;
			ItemUtilities.OnItemSentToPlayerInventory -= this.OnItemSentToPlayerInventory;
			ItemUtilities.OnItemSentToPlayerStorage -= this.OnItemSentToPlayerStorage;
			StockShop.OnItemPurchased -= this.OnItemPurchased;
			CraftingManager.OnItemCrafted = (Action<CraftingFormula, Item>)Delegate.Remove(CraftingManager.OnItemCrafted, new Action<CraftingFormula, Item>(this.OnItemCrafted));
			CraftingManager.OnFormulaUnlocked = (Action<string>)Delegate.Remove(CraftingManager.OnFormulaUnlocked, new Action<string>(this.OnFormulaUnlocked));
			Health.OnDead -= this.OnHealthDead;
			EXPManager.onLevelChanged = (Action<int, int>)Delegate.Remove(EXPManager.onLevelChanged, new Action<int, int>(this.OnLevelChanged));
			BuildingManager.OnBuildingBuiltComplex -= this.OnBuildingBuilt;
			BuildingManager.OnBuildingDestroyedComplex -= this.OnBuildingDestroyed;
			Perk.OnPerkUnlockConfirmed -= this.OnPerkUnlockConfirmed;
			MasterKeysManager.OnMasterKeyUnlocked -= this.OnMasterKeyUnlocked;
			CharacterMainControl.OnMainCharacterSlotContentChangedEvent = (Action<CharacterMainControl, Slot>)Delegate.Remove(CharacterMainControl.OnMainCharacterSlotContentChangedEvent, new Action<CharacterMainControl, Slot>(this.OnMainCharacterSlotContentChanged));
			StockShop.OnItemSoldByPlayer -= this.OnItemSold;
			Reward.OnRewardClaimed -= this.OnRewardClaimed;
			UsageUtilities.OnItemUsedStaticEvent -= this.OnItemUsed;
			InteractableBase.OnInteractStartStaticEvent -= this.OnInteractStart;
			LevelManager.OnNewGameReport -= this.OnNewGameReport;
			Interact_CustomFace.OnCustomFaceStartEvent -= this.OnCustomFaceStart;
			Interact_CustomFace.OnCustomFaceFinishedEvent -= this.OnCustomFaceFinish;
			CheatMode.OnCheatModeStatusChanged -= this.OnCheatModeStatusChanged;
		}

		// Token: 0x060010D0 RID: 4304 RVA: 0x000415C8 File Offset: 0x0003F7C8
		private void OnCheatModeStatusChanged(bool value)
		{
			this.ReportCustomEvent<BDSManager.CheatModeStatusChangeContext>(BDSManager.EventName.cheat_mode_changed, new BDSManager.CheatModeStatusChangeContext
			{
				cheatModeActive = value
			});
		}

		// Token: 0x060010D1 RID: 4305 RVA: 0x000415EE File Offset: 0x0003F7EE
		private void OnCustomFaceFinish()
		{
			this.ReportCustomEvent(BDSManager.EventName.face_customize_finish, "");
		}

		// Token: 0x060010D2 RID: 4306 RVA: 0x000415FD File Offset: 0x0003F7FD
		private void OnCustomFaceStart()
		{
			this.ReportCustomEvent(BDSManager.EventName.face_customize_begin, "");
		}

		// Token: 0x060010D3 RID: 4307 RVA: 0x0004160C File Offset: 0x0003F80C
		private void OnNewGameReport()
		{
			this.ReportCustomEvent(BDSManager.EventName.begin_new_game, "");
		}

		// Token: 0x060010D4 RID: 4308 RVA: 0x0004161C File Offset: 0x0003F81C
		private void OnInteractStart(InteractableBase target)
		{
			if (target == null)
			{
				return;
			}
			this.ReportCustomEvent<BDSManager.InteractEventContext>(BDSManager.EventName.interact_start, new BDSManager.InteractEventContext
			{
				interactGameObjectName = target.name,
				typeName = target.GetType().Name
			});
		}

		// Token: 0x060010D5 RID: 4309 RVA: 0x00041664 File Offset: 0x0003F864
		private void OnItemUsed(Item item)
		{
			this.ReportCustomEvent<BDSManager.ItemUseEventContext>(BDSManager.EventName.item_use, new BDSManager.ItemUseEventContext
			{
				itemTypeID = item.TypeID
			});
		}

		// Token: 0x060010D6 RID: 4310 RVA: 0x00041690 File Offset: 0x0003F890
		private void OnRewardClaimed(Reward reward)
		{
			int num = ((reward.Master != null) ? reward.Master.ID : (-1));
			this.ReportCustomEvent<BDSManager.RewardClaimEventContext>(BDSManager.EventName.reward_claimed, new BDSManager.RewardClaimEventContext
			{
				questID = num,
				rewardID = reward.ID
			});
		}

		// Token: 0x060010D7 RID: 4311 RVA: 0x000416E0 File Offset: 0x0003F8E0
		private void OnItemSold(StockShop shop, Item item, int price)
		{
			if (item == null)
			{
				return;
			}
			string text = ((shop != null) ? shop.MerchantID : null);
			this.ReportCustomEvent<BDSManager.ItemSoldEventContext>(BDSManager.EventName.item_sold, new BDSManager.ItemSoldEventContext
			{
				stockShopID = text,
				itemID = item.TypeID,
				price = price
			});
		}

		// Token: 0x060010D8 RID: 4312 RVA: 0x00041734 File Offset: 0x0003F934
		private void OnMainCharacterSlotContentChanged(CharacterMainControl control, Slot slot)
		{
			if (control == null || slot == null)
			{
				return;
			}
			if (slot.Content == null)
			{
				return;
			}
			this.ReportCustomEvent<BDSManager.EquipEventContext>(BDSManager.EventName.role_equip, new BDSManager.EquipEventContext
			{
				slotKey = slot.Key,
				contentItemTypeID = slot.Content.TypeID
			});
		}

		// Token: 0x060010D9 RID: 4313 RVA: 0x00041790 File Offset: 0x0003F990
		private void OnMasterKeyUnlocked(int id)
		{
			this.ReportCustomEvent<BDSManager.MasterKeyUnlockContext>(BDSManager.EventName.masterkey_unlocked, new BDSManager.MasterKeyUnlockContext
			{
				keyID = id
			});
		}

		// Token: 0x060010DA RID: 4314 RVA: 0x000417B8 File Offset: 0x0003F9B8
		private void OnPerkUnlockConfirmed(Perk perk)
		{
			if (perk == null)
			{
				return;
			}
			BDSManager.EventName eventName = BDSManager.EventName.perk_unlocked;
			BDSManager.PerkInfo perkInfo = default(BDSManager.PerkInfo);
			PerkTree master = perk.Master;
			perkInfo.perkTreeID = ((master != null) ? master.ID : null);
			perkInfo.perkName = perk.name;
			this.ReportCustomEvent<BDSManager.PerkInfo>(eventName, perkInfo);
		}

		// Token: 0x060010DB RID: 4315 RVA: 0x00041808 File Offset: 0x0003FA08
		private void OnBuildingBuilt(int guid, BuildingInfo info)
		{
			this.ReportCustomEvent<BDSManager.BuildingEventContext>(BDSManager.EventName.building_built, new BDSManager.BuildingEventContext
			{
				buildingID = info.id
			});
		}

		// Token: 0x060010DC RID: 4316 RVA: 0x00041834 File Offset: 0x0003FA34
		private void OnBuildingDestroyed(int guid, BuildingInfo info)
		{
			this.ReportCustomEvent<BDSManager.BuildingEventContext>(BDSManager.EventName.building_destroyed, new BDSManager.BuildingEventContext
			{
				buildingID = info.id
			});
		}

		// Token: 0x060010DD RID: 4317 RVA: 0x0004185F File Offset: 0x0003FA5F
		private void OnLevelChanged(int from, int to)
		{
			this.ReportCustomEvent<BDSManager.LevelChangedEventContext>(BDSManager.EventName.role_level_changed, new BDSManager.LevelChangedEventContext(from, to));
		}

		// Token: 0x060010DE RID: 4318 RVA: 0x00041870 File Offset: 0x0003FA70
		private void OnHealthDead(Health health, DamageInfo info)
		{
			if (health == null)
			{
				return;
			}
			Teams team = health.team;
			bool flag = false;
			if (info.fromCharacter != null && info.fromCharacter.IsMainCharacter())
			{
				flag = true;
			}
			if (flag)
			{
				this.ReportCustomEvent<BDSManager.EnemyKillInfo>(BDSManager.EventName.enemy_kill, new BDSManager.EnemyKillInfo
				{
					enemyPresetName = BDSManager.<OnHealthDead>g__GetPresetName|36_0(health),
					damageInfo = info
				});
			}
		}

		// Token: 0x060010DF RID: 4319 RVA: 0x000418D6 File Offset: 0x0003FAD6
		private void OnFormulaUnlocked(string formulaID)
		{
			this.ReportCustomEvent(BDSManager.EventName.craft_formula_unlock, StrJson.Create(new string[] { "id", formulaID }));
		}

		// Token: 0x060010E0 RID: 4320 RVA: 0x000418F7 File Offset: 0x0003FAF7
		private void OnItemCrafted(CraftingFormula formula, Item item)
		{
			this.ReportCustomEvent<CraftingFormula>(BDSManager.EventName.craft_craft, formula);
		}

		// Token: 0x060010E1 RID: 4321 RVA: 0x00041904 File Offset: 0x0003FB04
		private void OnItemPurchased(StockShop shop, Item item)
		{
			if (shop == null || item == null)
			{
				return;
			}
			this.ReportCustomEvent<BDSManager.PurchaseInfo>(BDSManager.EventName.shop_purchased, new BDSManager.PurchaseInfo
			{
				shopID = shop.MerchantID,
				itemTypeID = item.TypeID,
				itemAmount = item.StackCount
			});
		}

		// Token: 0x060010E2 RID: 4322 RVA: 0x0004195C File Offset: 0x0003FB5C
		private void OnItemSentToPlayerStorage(Item item)
		{
			if (item == null)
			{
				return;
			}
			this.ReportCustomEvent<BDSManager.ItemInfo>(BDSManager.EventName.item_to_storage, new BDSManager.ItemInfo
			{
				itemId = item.TypeID,
				amount = item.StackCount
			});
		}

		// Token: 0x060010E3 RID: 4323 RVA: 0x000419A0 File Offset: 0x0003FBA0
		private void OnItemSentToPlayerInventory(Item item)
		{
			if (item == null)
			{
				return;
			}
			this.ReportCustomEvent<BDSManager.ItemInfo>(BDSManager.EventName.item_to_inventory, new BDSManager.ItemInfo
			{
				itemId = item.TypeID,
				amount = item.StackCount
			});
		}

		// Token: 0x060010E4 RID: 4324 RVA: 0x000419E4 File Offset: 0x0003FBE4
		private void OnMoneyPaid(long money)
		{
			this.ReportCustomEvent<Cost>(BDSManager.EventName.pay_money, new Cost
			{
				money = money,
				items = new Cost.ItemEntry[0]
			});
		}

		// Token: 0x060010E5 RID: 4325 RVA: 0x00041A17 File Offset: 0x0003FC17
		private void OnCostPaid(Cost cost)
		{
			this.ReportCustomEvent<Cost>(BDSManager.EventName.pay_cost, cost);
		}

		// Token: 0x060010E6 RID: 4326 RVA: 0x00041A22 File Offset: 0x0003FC22
		private void OnQuestActivated(Quest quest)
		{
			if (quest == null)
			{
				return;
			}
			this.ReportCustomEvent<Quest.QuestInfo>(BDSManager.EventName.quest_activate, quest.GetInfo());
		}

		// Token: 0x060010E7 RID: 4327 RVA: 0x00041A3C File Offset: 0x0003FC3C
		private void OnQuestCompleted(Quest quest)
		{
			if (quest == null)
			{
				return;
			}
			this.ReportCustomEvent<Quest.QuestInfo>(BDSManager.EventName.quest_complete, quest.GetInfo());
		}

		// Token: 0x060010E8 RID: 4328 RVA: 0x00041A58 File Offset: 0x0003FC58
		private void OnMainCharacterDead(DamageInfo info)
		{
			string text = "None";
			string text2 = "None";
			if (info.fromCharacter)
			{
				CharacterRandomPreset characterPreset = info.fromCharacter.characterPreset;
				if (characterPreset != null)
				{
					text = characterPreset.name;
					text2 = characterPreset.nameKey;
				}
			}
			this.ReportCustomEvent<BDSManager.CharacterDeathContext>(BDSManager.EventName.main_character_dead, new BDSManager.CharacterDeathContext
			{
				damageInfo = info,
				levelInfo = LevelManager.GetCurrentLevelInfo(),
				fromCharacterPresetName = text,
				fromCharacterNameKey = text2
			});
		}

		// Token: 0x060010E9 RID: 4329 RVA: 0x00041AD8 File Offset: 0x0003FCD8
		private void OnEvacuated(EvacuationInfo evacuationInfo)
		{
			LevelManager.LevelInfo currentLevelInfo = LevelManager.GetCurrentLevelInfo();
			RaidUtilities.RaidInfo currentRaid = RaidUtilities.CurrentRaid;
			BDSManager.PlayerStatus playerStatus = BDSManager.PlayerStatus.CreateFromCurrent();
			this.ReportCustomEvent<BDSManager.EvacuationEventData>(BDSManager.EventName.level_evacuated, new BDSManager.EvacuationEventData
			{
				evacuationInfo = evacuationInfo,
				mapID = currentLevelInfo.activeSubSceneID,
				raidInfo = currentRaid,
				playerStatus = playerStatus
			});
		}

		// Token: 0x060010EA RID: 4330 RVA: 0x00041B2D File Offset: 0x0003FD2D
		private void OnLevelInitialized()
		{
			this.ReportCustomEvent<LevelManager.LevelInfo>(BDSManager.EventName.level_initialized, LevelManager.GetCurrentLevelInfo());
		}

		// Token: 0x060010EB RID: 4331 RVA: 0x00041B3B File Offset: 0x0003FD3B
		private void OnSceneLoadingFinish(SceneLoadingContext context)
		{
			this.ReportCustomEvent<SceneLoadingContext>(BDSManager.EventName.scene_load_start, context);
		}

		// Token: 0x060010EC RID: 4332 RVA: 0x00041B45 File Offset: 0x0003FD45
		private void OnSceneLoadingStart(SceneLoadingContext context)
		{
			this.ReportCustomEvent<SceneLoadingContext>(BDSManager.EventName.scene_load_finish, context);
		}

		// Token: 0x060010ED RID: 4333 RVA: 0x00041B4F File Offset: 0x0003FD4F
		private void OnRaidEnd(RaidUtilities.RaidInfo info)
		{
			this.ReportCustomEvent<RaidUtilities.RaidInfo>(BDSManager.EventName.raid_end, info);
		}

		// Token: 0x060010EE RID: 4334 RVA: 0x00041B59 File Offset: 0x0003FD59
		private void OnNewRaid(RaidUtilities.RaidInfo info)
		{
			this.ReportCustomEvent<RaidUtilities.RaidInfo>(BDSManager.EventName.raid_new, info);
		}

		// Token: 0x060010EF RID: 4335 RVA: 0x00041B63 File Offset: 0x0003FD63
		private void OnSaveDeleted()
		{
			this.ReportCustomEvent(BDSManager.EventName.delete_save_data, StrJson.Create(new string[]
			{
				"slot",
				string.Format("{0}", SavesSystem.CurrentSlot)
			}));
		}

		// Token: 0x060010F0 RID: 4336 RVA: 0x00041B98 File Offset: 0x0003FD98
		private void OnGameStarted()
		{
			int @int = PlayerPrefs.GetInt("AppStartCount", 0);
			this.sessionInfo = new BDSManager.SessionInfo
			{
				startCount = @int,
				isFirstTimeStart = (@int <= 0),
				session_id = DateTime.Now.ToBinary().GetHashCode()
			};
			this.sessionStartTime = DateTime.Now;
			this.ReportCustomEvent<BDSManager.SessionInfo>(BDSManager.EventName.app_start, this.sessionInfo);
			PlayerPrefs.SetInt("AppStartCount", @int + 1);
			PlayerPrefs.Save();
		}

		// Token: 0x060010F1 RID: 4337 RVA: 0x00041C1E File Offset: 0x0003FE1E
		private void ReportCustomEvent(BDSManager.EventName eventName, StrJson customParameters)
		{
			this.ReportCustomEvent(eventName, customParameters.ToString());
		}

		// Token: 0x060010F2 RID: 4338 RVA: 0x00041C30 File Offset: 0x0003FE30
		private void ReportCustomEvent<T>(BDSManager.EventName eventName, T customParameters)
		{
			string text = ((customParameters != null) ? JsonUtility.ToJson(customParameters) : "");
			this.ReportCustomEvent(eventName, text);
		}

		// Token: 0x060010F3 RID: 4339 RVA: 0x00041C60 File Offset: 0x0003FE60
		private void ReportCustomEvent(BDSManager.EventName eventName, string customParameters = "")
		{
			string text = BDSManager.PlayerInfo.GetCurrent().ToJson();
			SDK.ReportCustomEvent(eventName.ToString(), text, "", customParameters);
			try
			{
				Action<string, string> onReportCustomEvent = BDSManager.OnReportCustomEvent;
				if (onReportCustomEvent != null)
				{
					onReportCustomEvent(eventName.ToString(), customParameters);
				}
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		// Token: 0x060010F5 RID: 4341 RVA: 0x00041CD4 File Offset: 0x0003FED4
		[CompilerGenerated]
		internal static string <OnHealthDead>g__GetPresetName|36_0(Health health)
		{
			CharacterMainControl characterMainControl = health.TryGetCharacter();
			if (characterMainControl == null)
			{
				return "None";
			}
			CharacterRandomPreset characterPreset = characterMainControl.characterPreset;
			if (characterPreset == null)
			{
				return "None";
			}
			return characterPreset.Name;
		}

		// Token: 0x04000D3D RID: 3389
		private float lastTimeHeartbeat;

		// Token: 0x04000D3E RID: 3390
		private int sessionID;

		// Token: 0x04000D3F RID: 3391
		private DateTime sessionStartTime;

		// Token: 0x04000D40 RID: 3392
		private BDSManager.SessionInfo sessionInfo;

		// Token: 0x04000D41 RID: 3393
		public static Action<string, string> OnReportCustomEvent;

		// Token: 0x0200050B RID: 1291
		private enum EventName
		{
			// Token: 0x04001DCD RID: 7629
			none,
			// Token: 0x04001DCE RID: 7630
			app_start,
			// Token: 0x04001DCF RID: 7631
			begin_new_game,
			// Token: 0x04001DD0 RID: 7632
			delete_save_data,
			// Token: 0x04001DD1 RID: 7633
			raid_new,
			// Token: 0x04001DD2 RID: 7634
			raid_end,
			// Token: 0x04001DD3 RID: 7635
			scene_load_start,
			// Token: 0x04001DD4 RID: 7636
			scene_load_finish,
			// Token: 0x04001DD5 RID: 7637
			level_initialized,
			// Token: 0x04001DD6 RID: 7638
			level_evacuated,
			// Token: 0x04001DD7 RID: 7639
			main_character_dead,
			// Token: 0x04001DD8 RID: 7640
			quest_activate,
			// Token: 0x04001DD9 RID: 7641
			quest_complete,
			// Token: 0x04001DDA RID: 7642
			pay_money,
			// Token: 0x04001DDB RID: 7643
			pay_cost,
			// Token: 0x04001DDC RID: 7644
			item_to_inventory,
			// Token: 0x04001DDD RID: 7645
			item_to_storage,
			// Token: 0x04001DDE RID: 7646
			shop_purchased,
			// Token: 0x04001DDF RID: 7647
			craft_craft,
			// Token: 0x04001DE0 RID: 7648
			craft_formula_unlock,
			// Token: 0x04001DE1 RID: 7649
			enemy_kill,
			// Token: 0x04001DE2 RID: 7650
			role_level_changed,
			// Token: 0x04001DE3 RID: 7651
			building_built,
			// Token: 0x04001DE4 RID: 7652
			building_destroyed,
			// Token: 0x04001DE5 RID: 7653
			perk_unlocked,
			// Token: 0x04001DE6 RID: 7654
			masterkey_unlocked,
			// Token: 0x04001DE7 RID: 7655
			role_equip,
			// Token: 0x04001DE8 RID: 7656
			item_sold,
			// Token: 0x04001DE9 RID: 7657
			reward_claimed,
			// Token: 0x04001DEA RID: 7658
			item_use,
			// Token: 0x04001DEB RID: 7659
			interact_start,
			// Token: 0x04001DEC RID: 7660
			face_customize_begin,
			// Token: 0x04001DED RID: 7661
			face_customize_finish,
			// Token: 0x04001DEE RID: 7662
			heartbeat,
			// Token: 0x04001DEF RID: 7663
			cheat_mode_changed,
			// Token: 0x04001DF0 RID: 7664
			app_end
		}

		// Token: 0x0200050C RID: 1292
		private struct CheatModeStatusChangeContext
		{
			// Token: 0x04001DF1 RID: 7665
			public bool cheatModeActive;
		}

		// Token: 0x0200050D RID: 1293
		private struct InteractEventContext
		{
			// Token: 0x04001DF2 RID: 7666
			public string interactGameObjectName;

			// Token: 0x04001DF3 RID: 7667
			public string typeName;
		}

		// Token: 0x0200050E RID: 1294
		private struct ItemUseEventContext
		{
			// Token: 0x04001DF4 RID: 7668
			public int itemTypeID;
		}

		// Token: 0x0200050F RID: 1295
		private struct RewardClaimEventContext
		{
			// Token: 0x04001DF5 RID: 7669
			public int questID;

			// Token: 0x04001DF6 RID: 7670
			public int rewardID;
		}

		// Token: 0x02000510 RID: 1296
		private struct ItemSoldEventContext
		{
			// Token: 0x04001DF7 RID: 7671
			public string stockShopID;

			// Token: 0x04001DF8 RID: 7672
			public int itemID;

			// Token: 0x04001DF9 RID: 7673
			public int price;
		}

		// Token: 0x02000511 RID: 1297
		private struct EquipEventContext
		{
			// Token: 0x04001DFA RID: 7674
			public string slotKey;

			// Token: 0x04001DFB RID: 7675
			public int contentItemTypeID;
		}

		// Token: 0x02000512 RID: 1298
		private struct MasterKeyUnlockContext
		{
			// Token: 0x04001DFC RID: 7676
			public int keyID;
		}

		// Token: 0x02000513 RID: 1299
		private struct PerkInfo
		{
			// Token: 0x04001DFD RID: 7677
			public string perkTreeID;

			// Token: 0x04001DFE RID: 7678
			public string perkName;
		}

		// Token: 0x02000514 RID: 1300
		private struct BuildingEventContext
		{
			// Token: 0x04001DFF RID: 7679
			public string buildingID;
		}

		// Token: 0x02000515 RID: 1301
		private struct LevelChangedEventContext
		{
			// Token: 0x0600275E RID: 10078 RVA: 0x0008FDCE File Offset: 0x0008DFCE
			public LevelChangedEventContext(int from, int to)
			{
				this.from = from;
				this.to = to;
			}

			// Token: 0x04001E00 RID: 7680
			public int from;

			// Token: 0x04001E01 RID: 7681
			public int to;
		}

		// Token: 0x02000516 RID: 1302
		private struct EnemyKillInfo
		{
			// Token: 0x04001E02 RID: 7682
			public string enemyPresetName;

			// Token: 0x04001E03 RID: 7683
			public DamageInfo damageInfo;
		}

		// Token: 0x02000517 RID: 1303
		[Serializable]
		public struct PurchaseInfo
		{
			// Token: 0x04001E04 RID: 7684
			public string shopID;

			// Token: 0x04001E05 RID: 7685
			public int itemTypeID;

			// Token: 0x04001E06 RID: 7686
			public int itemAmount;
		}

		// Token: 0x02000518 RID: 1304
		private struct ItemInfo
		{
			// Token: 0x04001E07 RID: 7687
			public int itemId;

			// Token: 0x04001E08 RID: 7688
			public int amount;
		}

		// Token: 0x02000519 RID: 1305
		public struct CharacterDeathContext
		{
			// Token: 0x04001E09 RID: 7689
			public DamageInfo damageInfo;

			// Token: 0x04001E0A RID: 7690
			public string fromCharacterPresetName;

			// Token: 0x04001E0B RID: 7691
			public string fromCharacterNameKey;

			// Token: 0x04001E0C RID: 7692
			public LevelManager.LevelInfo levelInfo;
		}

		// Token: 0x0200051A RID: 1306
		[Serializable]
		private struct PlayerStatus
		{
			// Token: 0x0600275F RID: 10079 RVA: 0x0008FDE0 File Offset: 0x0008DFE0
			public static BDSManager.PlayerStatus CreateFromCurrent()
			{
				CharacterMainControl main = CharacterMainControl.Main;
				if (main == null)
				{
					return default(BDSManager.PlayerStatus);
				}
				Health health = main.Health;
				if (health == null)
				{
					return default(BDSManager.PlayerStatus);
				}
				CharacterBuffManager buffManager = main.GetBuffManager();
				if (buffManager == null)
				{
					return default(BDSManager.PlayerStatus);
				}
				if (main.CharacterItem == null)
				{
					return default(BDSManager.PlayerStatus);
				}
				string[] array = new string[buffManager.Buffs.Count];
				for (int i = 0; i < buffManager.Buffs.Count; i++)
				{
					Buff buff = buffManager.Buffs[i];
					if (!(buff == null))
					{
						array[i] = string.Format("{0} {1}", buff.ID, buff.DisplayNameKey);
					}
				}
				int totalRawValue = main.CharacterItem.GetTotalRawValue();
				return new BDSManager.PlayerStatus
				{
					valid = true,
					healthMax = health.MaxHealth,
					health = main.CurrentEnergy,
					water = main.CurrentWater,
					food = main.CurrentEnergy,
					waterMax = main.MaxWater,
					foodMax = main.MaxEnergy,
					totalItemValue = totalRawValue
				};
			}

			// Token: 0x04001E0D RID: 7693
			public bool valid;

			// Token: 0x04001E0E RID: 7694
			public float healthMax;

			// Token: 0x04001E0F RID: 7695
			public float health;

			// Token: 0x04001E10 RID: 7696
			public float waterMax;

			// Token: 0x04001E11 RID: 7697
			public float foodMax;

			// Token: 0x04001E12 RID: 7698
			public float water;

			// Token: 0x04001E13 RID: 7699
			public float food;

			// Token: 0x04001E14 RID: 7700
			public string[] activeEffects;

			// Token: 0x04001E15 RID: 7701
			public int totalItemValue;
		}

		// Token: 0x0200051B RID: 1307
		private struct EvacuationEventData
		{
			// Token: 0x04001E16 RID: 7702
			public EvacuationInfo evacuationInfo;

			// Token: 0x04001E17 RID: 7703
			public string mapID;

			// Token: 0x04001E18 RID: 7704
			public RaidUtilities.RaidInfo raidInfo;

			// Token: 0x04001E19 RID: 7705
			public BDSManager.PlayerStatus playerStatus;
		}

		// Token: 0x0200051C RID: 1308
		[Serializable]
		private struct SessionInfo
		{
			// Token: 0x04001E1A RID: 7706
			public int startCount;

			// Token: 0x04001E1B RID: 7707
			public bool isFirstTimeStart;

			// Token: 0x04001E1C RID: 7708
			public int session_id;

			// Token: 0x04001E1D RID: 7709
			public int session_duration_seconds;
		}

		// Token: 0x0200051D RID: 1309
		public struct PlayerInfo
		{
			// Token: 0x06002760 RID: 10080 RVA: 0x0008FF34 File Offset: 0x0008E134
			public PlayerInfo(int level, string steamAccountID, int saveSlot, string location, string language, string displayName, string difficulty, string platform, string version, string system)
			{
				this.role_name = displayName;
				this.profession_type = language;
				this.gender = version;
				this.level = string.Format("{0}", level);
				this.b_account_id = steamAccountID;
				this.b_role_id = string.Format("{0}|{1}", saveSlot, difficulty);
				this.b_tour_indicator = "0";
				this.b_zone_id = location;
				this.b_sdk_uid = platform + "|" + system;
			}

			// Token: 0x06002761 RID: 10081 RVA: 0x0008FFB8 File Offset: 0x0008E1B8
			public static BDSManager.PlayerInfo GetCurrent()
			{
				string id = PlatformInfo.GetID();
				string displayName = PlatformInfo.GetDisplayName();
				return new BDSManager.PlayerInfo(EXPManager.Level, id, SavesSystem.CurrentSlot, RegionInfo.CurrentRegion.Name, Application.systemLanguage.ToString(), displayName, GameRulesManager.Current.displayNameKey, PlatformInfo.Platform.ToString(), GameMetaData.Instance.Version.ToString(), Environment.OSVersion.Platform.ToString())
				{
					gender = GameMetaData.Instance.Version.ToString()
				};
			}

			// Token: 0x06002762 RID: 10082 RVA: 0x00090074 File Offset: 0x0008E274
			public static string GetCurrentJson()
			{
				return BDSManager.PlayerInfo.GetCurrent().ToJson();
			}

			// Token: 0x06002763 RID: 10083 RVA: 0x0009008E File Offset: 0x0008E28E
			public string ToJson()
			{
				return JsonUtility.ToJson(this);
			}

			// Token: 0x04001E1E RID: 7710
			public string role_name;

			// Token: 0x04001E1F RID: 7711
			public string profession_type;

			// Token: 0x04001E20 RID: 7712
			public string gender;

			// Token: 0x04001E21 RID: 7713
			public string level;

			// Token: 0x04001E22 RID: 7714
			public string b_account_id;

			// Token: 0x04001E23 RID: 7715
			public string b_role_id;

			// Token: 0x04001E24 RID: 7716
			public string b_tour_indicator;

			// Token: 0x04001E25 RID: 7717
			public string b_zone_id;

			// Token: 0x04001E26 RID: 7718
			public string b_sdk_uid;
		}
	}
}
