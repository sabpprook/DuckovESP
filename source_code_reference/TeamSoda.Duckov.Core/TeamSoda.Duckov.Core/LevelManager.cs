using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Cysharp.Threading.Tasks;
using Duckov;
using Duckov.MiniMaps;
using Duckov.Rules;
using Duckov.Scenes;
using Duckov.UI;
using Duckov.Utilities;
using ItemStatsSystem;
using ItemStatsSystem.Items;
using Saves;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

// Token: 0x02000106 RID: 262
public class LevelManager : MonoBehaviour
{
	// Token: 0x170001D0 RID: 464
	// (get) Token: 0x060008CA RID: 2250 RVA: 0x000277BD File Offset: 0x000259BD
	public static LevelManager Instance
	{
		get
		{
			if (!LevelManager.instance)
			{
				LevelManager.SetInstance();
			}
			return LevelManager.instance;
		}
	}

	// Token: 0x170001D1 RID: 465
	// (get) Token: 0x060008CB RID: 2251 RVA: 0x000277D8 File Offset: 0x000259D8
	public static Transform LootBoxInventoriesParent
	{
		get
		{
			if (LevelManager.Instance._lootBoxInventoriesParent == null)
			{
				GameObject gameObject = new GameObject("Loot Box Inventories");
				gameObject.transform.SetParent(LevelManager.Instance.transform);
				LevelManager.Instance._lootBoxInventoriesParent = gameObject.transform;
				LevelManager.LootBoxInventories.Clear();
			}
			return LevelManager.Instance._lootBoxInventoriesParent;
		}
	}

	// Token: 0x170001D2 RID: 466
	// (get) Token: 0x060008CC RID: 2252 RVA: 0x0002783B File Offset: 0x00025A3B
	public static Dictionary<int, Inventory> LootBoxInventories
	{
		get
		{
			if (LevelManager.Instance._lootBoxInventories == null)
			{
				LevelManager.Instance._lootBoxInventories = new Dictionary<int, Inventory>();
			}
			return LevelManager.Instance._lootBoxInventories;
		}
	}

	// Token: 0x170001D3 RID: 467
	// (get) Token: 0x060008CD RID: 2253 RVA: 0x00027862 File Offset: 0x00025A62
	public bool IsRaidMap
	{
		get
		{
			return LevelConfig.IsRaidMap;
		}
	}

	// Token: 0x170001D4 RID: 468
	// (get) Token: 0x060008CE RID: 2254 RVA: 0x00027869 File Offset: 0x00025A69
	public bool IsBaseLevel
	{
		get
		{
			return LevelConfig.IsBaseLevel;
		}
	}

	// Token: 0x170001D5 RID: 469
	// (get) Token: 0x060008CF RID: 2255 RVA: 0x00027870 File Offset: 0x00025A70
	public InputManager InputManager
	{
		get
		{
			return this.inputManager;
		}
	}

	// Token: 0x170001D6 RID: 470
	// (get) Token: 0x060008D0 RID: 2256 RVA: 0x00027878 File Offset: 0x00025A78
	public CharacterCreator CharacterCreator
	{
		get
		{
			return this.characterCreator;
		}
	}

	// Token: 0x170001D7 RID: 471
	// (get) Token: 0x060008D1 RID: 2257 RVA: 0x00027880 File Offset: 0x00025A80
	public ExitCreator ExitCreator
	{
		get
		{
			return this.exitCreator;
		}
	}

	// Token: 0x170001D8 RID: 472
	// (get) Token: 0x060008D2 RID: 2258 RVA: 0x00027888 File Offset: 0x00025A88
	public ExplosionManager ExplosionManager
	{
		get
		{
			return this.explosionManager;
		}
	}

	// Token: 0x170001D9 RID: 473
	// (get) Token: 0x060008D3 RID: 2259 RVA: 0x00027890 File Offset: 0x00025A90
	private int characterItemTypeID
	{
		get
		{
			return GameplayDataSettings.ItemAssets.DefaultCharacterItemTypeID;
		}
	}

	// Token: 0x170001DA RID: 474
	// (get) Token: 0x060008D4 RID: 2260 RVA: 0x0002789C File Offset: 0x00025A9C
	public CharacterMainControl MainCharacter
	{
		get
		{
			return this.mainCharacter;
		}
	}

	// Token: 0x170001DB RID: 475
	// (get) Token: 0x060008D5 RID: 2261 RVA: 0x000278A4 File Offset: 0x00025AA4
	public CharacterMainControl PetCharacter
	{
		get
		{
			return this.petCharacter;
		}
	}

	// Token: 0x170001DC RID: 476
	// (get) Token: 0x060008D6 RID: 2262 RVA: 0x000278AC File Offset: 0x00025AAC
	public GameCamera GameCamera
	{
		get
		{
			return this.gameCamera;
		}
	}

	// Token: 0x170001DD RID: 477
	// (get) Token: 0x060008D7 RID: 2263 RVA: 0x000278B4 File Offset: 0x00025AB4
	public FogOfWarManager FogOfWarManager
	{
		get
		{
			return this.fowManager;
		}
	}

	// Token: 0x170001DE RID: 478
	// (get) Token: 0x060008D8 RID: 2264 RVA: 0x000278BC File Offset: 0x00025ABC
	public TimeOfDayController TimeOfDayController
	{
		get
		{
			return this.timeOfDayController;
		}
	}

	// Token: 0x1400003E RID: 62
	// (add) Token: 0x060008D9 RID: 2265 RVA: 0x000278C4 File Offset: 0x00025AC4
	// (remove) Token: 0x060008DA RID: 2266 RVA: 0x000278F8 File Offset: 0x00025AF8
	public static event Action OnLevelBeginInitializing;

	// Token: 0x1400003F RID: 63
	// (add) Token: 0x060008DB RID: 2267 RVA: 0x0002792C File Offset: 0x00025B2C
	// (remove) Token: 0x060008DC RID: 2268 RVA: 0x00027960 File Offset: 0x00025B60
	public static event Action OnLevelInitialized;

	// Token: 0x14000040 RID: 64
	// (add) Token: 0x060008DD RID: 2269 RVA: 0x00027994 File Offset: 0x00025B94
	// (remove) Token: 0x060008DE RID: 2270 RVA: 0x000279C8 File Offset: 0x00025BC8
	public static event Action OnAfterLevelInitialized;

	// Token: 0x170001DF RID: 479
	// (get) Token: 0x060008DF RID: 2271 RVA: 0x000279FB File Offset: 0x00025BFB
	public AIMainBrain AIMainBrain
	{
		get
		{
			return this.aiMainBrain;
		}
	}

	// Token: 0x170001E0 RID: 480
	// (get) Token: 0x060008E0 RID: 2272 RVA: 0x00027A03 File Offset: 0x00025C03
	public static bool LevelInitializing
	{
		get
		{
			return !(LevelManager.Instance == null) && LevelManager.Instance.initingLevel;
		}
	}

	// Token: 0x170001E1 RID: 481
	// (get) Token: 0x060008E1 RID: 2273 RVA: 0x00027A1E File Offset: 0x00025C1E
	public static bool AfterInit
	{
		get
		{
			return !(LevelManager.Instance == null) && LevelManager.Instance.afterInit;
		}
	}

	// Token: 0x170001E2 RID: 482
	// (get) Token: 0x060008E2 RID: 2274 RVA: 0x00027A39 File Offset: 0x00025C39
	public PetProxy PetProxy
	{
		get
		{
			return this.petProxy;
		}
	}

	// Token: 0x170001E3 RID: 483
	// (get) Token: 0x060008E3 RID: 2275 RVA: 0x00027A41 File Offset: 0x00025C41
	public BulletPool BulletPool
	{
		get
		{
			return this.bulletPool;
		}
	}

	// Token: 0x170001E4 RID: 484
	// (get) Token: 0x060008E4 RID: 2276 RVA: 0x00027A49 File Offset: 0x00025C49
	public CustomFaceManager CustomFaceManager
	{
		get
		{
			return this.customFaceManager;
		}
	}

	// Token: 0x170001E5 RID: 485
	// (get) Token: 0x060008E5 RID: 2277 RVA: 0x00027A51 File Offset: 0x00025C51
	// (set) Token: 0x060008E6 RID: 2278 RVA: 0x00027A6C File Offset: 0x00025C6C
	public static string LevelInitializingComment
	{
		get
		{
			if (LevelManager.Instance == null)
			{
				return null;
			}
			return LevelManager.Instance._levelInitializingComment;
		}
		set
		{
			if (LevelManager.Instance == null)
			{
				return;
			}
			LevelManager.Instance._levelInitializingComment = value;
			Action<string> onLevelInitializingCommentChanged = LevelManager.OnLevelInitializingCommentChanged;
			if (onLevelInitializingCommentChanged != null)
			{
				onLevelInitializingCommentChanged(value);
			}
			Debug.Log("[Level Initialization] " + value);
		}
	}

	// Token: 0x14000041 RID: 65
	// (add) Token: 0x060008E7 RID: 2279 RVA: 0x00027AA8 File Offset: 0x00025CA8
	// (remove) Token: 0x060008E8 RID: 2280 RVA: 0x00027ADC File Offset: 0x00025CDC
	public static event Action<string> OnLevelInitializingCommentChanged;

	// Token: 0x170001E6 RID: 486
	// (get) Token: 0x060008E9 RID: 2281 RVA: 0x00027B0F File Offset: 0x00025D0F
	public static bool LevelInited
	{
		get
		{
			return !(LevelManager.instance == null) && LevelManager.instance.levelInited;
		}
	}

	// Token: 0x14000042 RID: 66
	// (add) Token: 0x060008EA RID: 2282 RVA: 0x00027B2C File Offset: 0x00025D2C
	// (remove) Token: 0x060008EB RID: 2283 RVA: 0x00027B60 File Offset: 0x00025D60
	public static event Action<EvacuationInfo> OnEvacuated;

	// Token: 0x14000043 RID: 67
	// (add) Token: 0x060008EC RID: 2284 RVA: 0x00027B94 File Offset: 0x00025D94
	// (remove) Token: 0x060008ED RID: 2285 RVA: 0x00027BC8 File Offset: 0x00025DC8
	public static event Action<DamageInfo> OnMainCharacterDead;

	// Token: 0x170001E7 RID: 487
	// (get) Token: 0x060008EE RID: 2286 RVA: 0x00027BFB File Offset: 0x00025DFB
	public float LevelTime
	{
		get
		{
			return Time.time - this.levelStartTime;
		}
	}

	// Token: 0x14000044 RID: 68
	// (add) Token: 0x060008EF RID: 2287 RVA: 0x00027C0C File Offset: 0x00025E0C
	// (remove) Token: 0x060008F0 RID: 2288 RVA: 0x00027C40 File Offset: 0x00025E40
	public static event Action OnNewGameReport;

	// Token: 0x170001E8 RID: 488
	// (get) Token: 0x060008F1 RID: 2289 RVA: 0x00027C73 File Offset: 0x00025E73
	public static Ruleset Rule
	{
		get
		{
			return LevelManager.rule;
		}
	}

	// Token: 0x060008F2 RID: 2290 RVA: 0x00027C7A File Offset: 0x00025E7A
	public static void RegisterWaitForInitialization<T>(T toWait) where T : class, IInitializedQueryHandler
	{
		if (toWait == null)
		{
			return;
		}
		if (toWait == null)
		{
			return;
		}
		LevelManager.waitForInitializationList.Add(toWait);
	}

	// Token: 0x060008F3 RID: 2291 RVA: 0x00027C9E File Offset: 0x00025E9E
	public static bool UnregisterWaitForInitialization<T>(T obj) where T : class
	{
		return LevelManager.waitForInitializationList.Remove(obj);
	}

	// Token: 0x060008F4 RID: 2292 RVA: 0x00027CB0 File Offset: 0x00025EB0
	private void Start()
	{
		if (!SceneLoader.IsSceneLoading)
		{
			this.StartInit(default(SceneLoadingContext));
		}
		else
		{
			SceneLoader.onFinishedLoadingScene += this.StartInit;
		}
		if (!SavesSystem.Load<bool>("NewGameReported"))
		{
			SavesSystem.Save<bool>("NewGameReported", true);
			Action onNewGameReport = LevelManager.OnNewGameReport;
			if (onNewGameReport != null)
			{
				onNewGameReport();
			}
		}
		if (GameManager.newBoot)
		{
			this.OnNewBoot();
			GameManager.newBoot = false;
		}
	}

	// Token: 0x060008F5 RID: 2293 RVA: 0x00027D20 File Offset: 0x00025F20
	private void OnDestroy()
	{
		SceneLoader.onFinishedLoadingScene -= this.StartInit;
		CharacterMainControl characterMainControl = this.mainCharacter;
		if (characterMainControl == null)
		{
			return;
		}
		Health health = characterMainControl.Health;
		if (health == null)
		{
			return;
		}
		health.OnDeadEvent.RemoveListener(new UnityAction<DamageInfo>(this.OnMainCharacterDie));
	}

	// Token: 0x060008F6 RID: 2294 RVA: 0x00027D5E File Offset: 0x00025F5E
	private void OnNewBoot()
	{
		Debug.Log("New boot");
		GameClock.Instance.StepTimeTil(new TimeSpan(7, 0, 0));
	}

	// Token: 0x060008F7 RID: 2295 RVA: 0x00027D7C File Offset: 0x00025F7C
	private void StartInit(SceneLoadingContext context)
	{
		this.InitLevel(context).Forget();
	}

	// Token: 0x060008F8 RID: 2296 RVA: 0x00027D98 File Offset: 0x00025F98
	private async UniTaskVoid InitLevel(SceneLoadingContext context)
	{
		if (!this.initingLevel)
		{
			LevelManager.LevelInitializingComment = "Starting up...";
			LevelManager.instance = this;
			GameManager gameManager = GameManager.Instance;
			this.initingLevel = true;
			LevelManager.LevelInitializingComment = "Invoking Beginning Event...";
			Action onLevelBeginInitializing = LevelManager.OnLevelBeginInitializing;
			if (onLevelBeginInitializing != null)
			{
				onLevelBeginInitializing();
			}
			await UniTask.Yield();
			LevelManager.LevelInitializingComment = "Setting up rule...";
			LevelManager.rule = GameRulesManager.Current;
			Debug.Log(string.Format("Rule is:{0},Recoil:{1}", LevelManager.rule.DisplayName, LevelManager.rule.RecoilMultiplier));
			Vector3 startPos = this.defaultStartPos.position;
			if (context.useLocation && MultiSceneCore.Instance != null)
			{
				LevelManager.LevelInitializingComment = "Finding location for spawning...";
				MultiSceneLocation location = context.location;
				LevelManager.LevelInitializingComment = "Creating Character...";
				await this.CreateMainCharacterAsync(startPos, Quaternion.identity);
				LevelManager.LevelInitializingComment = "Teleporting to location...";
				await MultiSceneCore.Instance.LoadAndTeleport(location);
				startPos = location.GetLocationTransform().position;
				location = default(MultiSceneLocation);
			}
			else if (MultiSceneCore.Instance != null)
			{
				LevelManager.LevelInitializingComment = "Getting location...";
				ValueTuple<string, SubSceneEntry.Location> playerStartLocation = this.GetPlayerStartLocation();
				if (playerStartLocation.Item2 != null)
				{
					LevelManager.LevelInitializingComment = "Creating location info...";
					MultiSceneLocation location = new MultiSceneLocation
					{
						SceneID = playerStartLocation.Item1,
						LocationName = playerStartLocation.Item2.path
					};
					LevelManager.LevelInitializingComment = "Setting start position...";
					startPos = playerStartLocation.Item2.position;
					LevelManager.LevelInitializingComment = "Creating character at location...";
					await this.CreateMainCharacterAsync(playerStartLocation.Item2.position, Quaternion.identity);
					LevelManager.LevelInitializingComment = "Teleporting to location...";
					await MultiSceneCore.Instance.LoadAndTeleport(location);
					location = default(MultiSceneLocation);
				}
				else
				{
					LevelManager.LevelInitializingComment = "Setting default start position...";
					startPos = this.defaultStartPos.position;
					LevelManager.LevelInitializingComment = "Creating character at default position...";
					await this.CreateMainCharacterAsync(this.defaultStartPos.position, Quaternion.identity);
				}
			}
			else
			{
				LevelManager.LevelInitializingComment = "Creating character...";
				await this.CreateMainCharacterAsync(this.defaultStartPos.position, Quaternion.identity);
			}
			LevelManager.LevelInitializingComment = "Setting up character status...";
			this.mainCharacter.Health.OnDeadEvent.AddListener(new UnityAction<DamageInfo>(this.OnMainCharacterDie));
			this.RefreshMainCharacterFace();
			LevelManager.LevelInitializingComment = "Setting up pet...";
			this.petCharacter = await this.petPreset.CreateCharacterAsync(this.mainCharacter.transform.position + Vector3.one * 99f, Vector3.forward, MultiSceneCore.MainScene.Value.buildIndex, null, false);
			if (this.IsBaseLevel && this.petProxy != null)
			{
				this.petProxy.DestroyItemInBase();
			}
			this.petCharacter.Health.showHealthBar = false;
			this.petCharacter.Health.SetInvincible(true);
			LevelManager.LevelInitializingComment = "Setting character items...";
			this.SetCharacterItemsInspected();
			LevelManager.LevelInitializingComment = "Waiting for other initialization...";
			await this.WaitForOtherInitialization();
			this.mainCharacter.SwitchToFirstAvailableWeapon();
			if (MultiSceneCore.Instance != null)
			{
				while (MultiSceneCore.Instance.IsLoading)
				{
					await UniTask.Yield();
				}
			}
			this.initingLevel = false;
			this.levelInited = true;
			this.levelStartTime = Time.time;
			LevelManager.LevelInitializingComment = "Handling raid initialization...";
			this.HandleRaidInitialization();
			LevelManager.LevelInitializingComment = "Invoking initialized event...";
			Action onLevelInitialized = LevelManager.OnLevelInitialized;
			if (onLevelInitialized != null)
			{
				onLevelInitialized();
			}
			LevelManager.LevelInitializingComment = "Healing...";
			float num = SavesSystem.Load<float>("MainCharacterHealth");
			this.mainCharacter.Health.SetHealth(num);
			if (this.IsBaseLevel || this.isNewRaidLevel)
			{
				this.mainCharacter.AddHealth(this.mainCharacter.Health.MaxHealth);
			}
			LevelManager.LevelInitializingComment = "Spawing exits...";
			if (MultiSceneCore.Instance != null)
			{
				this.exitCreator.Spawn();
			}
			LevelManager.LevelInitializingComment = "Creating map element...";
			this.CreateMainCharacterMapElement();
			LevelManager.LevelInitializingComment = "Setting character position...";
			await UniTask.WaitForSeconds(0.25f, false, PlayerLoopTiming.Update, default(CancellationToken), false);
			this.mainCharacter.SetPosition(startPos);
			LevelManager.LevelInitializingComment = "Done!";
			try
			{
				Action onAfterLevelInitialized = LevelManager.OnAfterLevelInitialized;
				if (onAfterLevelInitialized != null)
				{
					onAfterLevelInitialized();
				}
				this.afterInit = true;
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}
	}

	// Token: 0x060008F9 RID: 2297 RVA: 0x00027DE4 File Offset: 0x00025FE4
	private async UniTask CreateMate()
	{
		if (this.matePreset)
		{
			AICharacterController componentInChildren = (await this.matePreset.CreateCharacterAsync(this.mainCharacter.transform.position + Vector3.one, Vector3.forward, MultiSceneCore.MainScene.Value.buildIndex, null, false)).GetComponentInChildren<AICharacterController>();
			if (componentInChildren)
			{
				componentInChildren.leader = this.mainCharacter;
			}
		}
	}

	// Token: 0x060008FA RID: 2298 RVA: 0x00027E28 File Offset: 0x00026028
	private async UniTask WaitForOtherInitialization()
	{
		for (;;)
		{
			if (!LevelManager.waitForInitializationList.Any(delegate(object e)
			{
				if (e == null)
				{
					return false;
				}
				IInitializedQueryHandler initializedQueryHandler = e as IInitializedQueryHandler;
				if (initializedQueryHandler == null)
				{
					return false;
				}
				if (!initializedQueryHandler.HasInitialized())
				{
					LevelManager.LevelInitializingComment = "Waiting for " + e.GetType().Name + "...";
					return true;
				}
				return false;
			}))
			{
				break;
			}
			await UniTask.Yield();
		}
	}

	// Token: 0x060008FB RID: 2299 RVA: 0x00027E64 File Offset: 0x00026064
	private void HandleRaidInitialization()
	{
		RaidUtilities.RaidInfo currentRaid = RaidUtilities.CurrentRaid;
		if (this.IsRaidMap)
		{
			if (currentRaid.ended)
			{
				RaidUtilities.NewRaid();
				this.isNewRaidLevel = true;
				return;
			}
		}
		else if (this.IsBaseLevel && !currentRaid.ended)
		{
			RaidUtilities.NotifyEnd();
		}
	}

	// Token: 0x060008FC RID: 2300 RVA: 0x00027EAC File Offset: 0x000260AC
	public void RefreshMainCharacterFace()
	{
		if (this.mainCharacter.characterModel.CustomFace)
		{
			CustomFaceSettingData customFaceSettingData = this.customFaceManager.LoadMainCharacterSetting();
			this.mainCharacter.characterModel.CustomFace.LoadFromData(customFaceSettingData);
		}
	}

	// Token: 0x060008FD RID: 2301 RVA: 0x00027EF4 File Offset: 0x000260F4
	private async UniTask CreateMainCharacterAsync(Vector3 position, Quaternion rotation)
	{
		Item item = await this.LoadOrCreateCharacterItemInstance();
		this.mainCharacter = await this.characterCreator.CreateCharacter(item, this.characterModel, position, rotation);
		if (!(this.mainCharacter == null))
		{
			if (this.IsBaseLevel)
			{
				this.mainCharacter.DestroyItemsThatNeededToBeDestriedInBase();
			}
			this.mainCharacter.SetTeam(Teams.player);
			this.mainCharacter.CharacterItem.Inventory.AcceptSticky = true;
			if (this.defaultSkill != null)
			{
				SkillBase skillBase = global::UnityEngine.Object.Instantiate<SkillBase>(this.defaultSkill);
				skillBase.transform.SetParent(this.mainCharacter.transform, false);
				this.mainCharacter.SetSkill(SkillTypes.characterSkill, skillBase, skillBase.gameObject);
			}
			this.inputManager.characterMainControl = this.mainCharacter;
			this.inputManager.SwitchItemAgent(1);
			this.gameCamera.SetTarget(this.mainCharacter);
		}
	}

	// Token: 0x060008FE RID: 2302 RVA: 0x00027F48 File Offset: 0x00026148
	private void SetCharacterItemsInspected()
	{
		foreach (Slot slot in this.mainCharacter.CharacterItem.Slots)
		{
			if (slot.Content != null)
			{
				slot.Content.Inspected = true;
			}
		}
		foreach (Item item in this.mainCharacter.CharacterItem.Inventory)
		{
			if (item != null)
			{
				item.Inspected = true;
			}
		}
		foreach (Item item2 in this.petProxy.Inventory)
		{
			if (item2 != null)
			{
				item2.Inspected = true;
			}
		}
	}

	// Token: 0x060008FF RID: 2303 RVA: 0x00028050 File Offset: 0x00026250
	private static void SetInstance()
	{
		if (LevelManager.instance)
		{
			return;
		}
		LevelManager.instance = global::UnityEngine.Object.FindFirstObjectByType<LevelManager>();
		LevelManager.instance;
	}

	// Token: 0x06000900 RID: 2304 RVA: 0x00028074 File Offset: 0x00026274
	private async UniTask<Item> LoadOrCreateCharacterItemInstance()
	{
		Item item = await ItemSavesUtilities.LoadItem("MainCharacterItemData");
		if (item == null)
		{
			item = await ItemAssetsCollection.InstantiateAsync(this.characterItemTypeID);
			Debug.LogWarning("Item Loading failed");
		}
		return item;
	}

	// Token: 0x06000901 RID: 2305 RVA: 0x000280B7 File Offset: 0x000262B7
	public void NotifyEvacuated(EvacuationInfo info)
	{
		this.mainCharacter.Health.SetInvincible(true);
		Action<EvacuationInfo> onEvacuated = LevelManager.OnEvacuated;
		if (onEvacuated != null)
		{
			onEvacuated(info);
		}
		this.SaveMainCharacter();
		SavesSystem.CollectSaveData();
		SavesSystem.SaveFile(true);
	}

	// Token: 0x06000902 RID: 2306 RVA: 0x000280EC File Offset: 0x000262EC
	public void NotifySaveBeforeLoadScene(bool saveToFile)
	{
		this.SaveMainCharacter();
		SavesSystem.CollectSaveData();
		if (saveToFile)
		{
			SavesSystem.SaveFile(true);
		}
	}

	// Token: 0x06000903 RID: 2307 RVA: 0x00028104 File Offset: 0x00026304
	private void OnMainCharacterDie(DamageInfo dmgInfo)
	{
		if (this.dieTask)
		{
			return;
		}
		this.dieTask = true;
		this.CharacterDieTask(dmgInfo).Forget();
		Action<DamageInfo> onMainCharacterDead = LevelManager.OnMainCharacterDead;
		if (onMainCharacterDead == null)
		{
			return;
		}
		onMainCharacterDead(dmgInfo);
	}

	// Token: 0x06000904 RID: 2308 RVA: 0x00028140 File Offset: 0x00026340
	private async UniTaskVoid CharacterDieTask(DamageInfo dmgInfo)
	{
		if (this.IsRaidMap)
		{
			RaidUtilities.NotifyDead();
			DeadBodyManager.RecordDeath(this.mainCharacter);
		}
		ItemSavesUtilities.SaveAsLastDeadCharacter(this.mainCharacter.CharacterItem);
		if (LevelConfig.SpawnTomb)
		{
			InteractableLootbox.CreateFromItem(this.mainCharacter.CharacterItem, this.mainCharacter.transform.position, this.mainCharacter.transform.rotation, true, GameplayDataSettings.Prefabs.LootBoxPrefab_Tomb, true);
		}
		else
		{
			CharacterMainControl characterMainControl = this.mainCharacter;
			if (characterMainControl != null)
			{
				characterMainControl.DropAllItems();
			}
		}
		CharacterMainControl characterMainControl2 = this.mainCharacter;
		if (characterMainControl2 != null)
		{
			characterMainControl2.DestroyAllItem();
		}
		this.SaveMainCharacter();
		SavesSystem.CollectSaveData();
		SavesSystem.SaveFile(true);
		await UniTask.WaitForSeconds(2.5f, true, PlayerLoopTiming.Update, default(CancellationToken), false);
		await ClosureView.ShowAndReturnTask(dmgInfo, 0.5f);
		if (OverrideDeathSceneRouting.Instance != null)
		{
			Debug.Log("死亡后的目标场景已被特殊脚本修改");
			SceneLoader.Instance.LoadScene(OverrideDeathSceneRouting.Instance.GetSceneID(), GameplayDataSettings.SceneManagement.FailLoadingScreenScene, false, false, true, false, default(MultiSceneLocation), true, false).Forget();
		}
		else
		{
			SceneLoader.Instance.LoadBaseScene(GameplayDataSettings.SceneManagement.FailLoadingScreenScene, true).Forget();
		}
	}

	// Token: 0x06000905 RID: 2309 RVA: 0x0002818B File Offset: 0x0002638B
	internal void SaveMainCharacter()
	{
		this.mainCharacter.CharacterItem.Save("MainCharacterItemData");
		SavesSystem.Save<float>("MainCharacterHealth", this.MainCharacter.Health.CurrentHealth);
	}

	// Token: 0x06000906 RID: 2310 RVA: 0x000281BC File Offset: 0x000263BC
	[return: TupleElementNames(new string[] { "sceneID", "locationData" })]
	private ValueTuple<string, SubSceneEntry.Location> GetPlayerStartLocation()
	{
		List<ValueTuple<string, SubSceneEntry.Location>> list = new List<ValueTuple<string, SubSceneEntry.Location>>();
		string text = "StartPoints";
		if (LevelManager.loadLevelBeaconIndex > 0)
		{
			text = text + "_" + LevelManager.loadLevelBeaconIndex.ToString();
			LevelManager.loadLevelBeaconIndex = 0;
		}
		foreach (SubSceneEntry subSceneEntry in MultiSceneCore.Instance.SubScenes)
		{
			foreach (SubSceneEntry.Location location in subSceneEntry.cachedLocations)
			{
				if (this.IsPathCompatible(location, text))
				{
					list.Add(new ValueTuple<string, SubSceneEntry.Location>(subSceneEntry.sceneID, location));
				}
			}
		}
		if (list.Count == 0)
		{
			text = "StartPoints";
			foreach (SubSceneEntry subSceneEntry2 in MultiSceneCore.Instance.SubScenes)
			{
				foreach (SubSceneEntry.Location location2 in subSceneEntry2.cachedLocations)
				{
					if (this.IsPathCompatible(location2, text))
					{
						list.Add(new ValueTuple<string, SubSceneEntry.Location>(subSceneEntry2.sceneID, location2));
					}
				}
			}
		}
		return list.GetRandom<ValueTuple<string, SubSceneEntry.Location>>();
	}

	// Token: 0x06000907 RID: 2311 RVA: 0x0002834C File Offset: 0x0002654C
	private void CreateMainCharacterMapElement()
	{
		if (MultiSceneCore.Instance != null)
		{
			SimplePointOfInterest simplePointOfInterest = this.mainCharacter.gameObject.AddComponent<SimplePointOfInterest>();
			simplePointOfInterest.Color = this.characterMapIconColor;
			simplePointOfInterest.ShadowColor = this.characterMapShadowColor;
			simplePointOfInterest.ShadowDistance = 0f;
			simplePointOfInterest.Setup(this.characterMapIcon, "You", true, null);
		}
	}

	// Token: 0x06000908 RID: 2312 RVA: 0x000283AB File Offset: 0x000265AB
	private void OnSubSceneLoaded()
	{
	}

	// Token: 0x06000909 RID: 2313 RVA: 0x000283B0 File Offset: 0x000265B0
	private bool IsPathCompatible(SubSceneEntry.Location location, string keyWord)
	{
		string path = location.path;
		int num = path.IndexOf('/');
		return num != -1 && path.Substring(0, num) == keyWord;
	}

	// Token: 0x0600090A RID: 2314 RVA: 0x000283E4 File Offset: 0x000265E4
	public void TestTeleport()
	{
		MultiSceneCore.Instance.LoadAndTeleport(this.testTeleportTarget).Forget<bool>();
	}

	// Token: 0x0600090B RID: 2315 RVA: 0x000283FC File Offset: 0x000265FC
	private LevelManager.LevelInfo mGetInfo()
	{
		Scene? activeSubScene = MultiSceneCore.ActiveSubScene;
		string text = ((activeSubScene != null) ? activeSubScene.Value.name : "");
		return new LevelManager.LevelInfo
		{
			isBaseLevel = this.IsBaseLevel,
			sceneName = base.gameObject.scene.name,
			activeSubSceneID = text
		};
	}

	// Token: 0x0600090C RID: 2316 RVA: 0x00028468 File Offset: 0x00026668
	public static LevelManager.LevelInfo GetCurrentLevelInfo()
	{
		if (LevelManager.Instance == null)
		{
			return default(LevelManager.LevelInfo);
		}
		return LevelManager.Instance.mGetInfo();
	}

	// Token: 0x040007FB RID: 2043
	private Transform _lootBoxInventoriesParent;

	// Token: 0x040007FC RID: 2044
	private Dictionary<int, Inventory> _lootBoxInventories;

	// Token: 0x040007FD RID: 2045
	[SerializeField]
	private Transform defaultStartPos;

	// Token: 0x040007FE RID: 2046
	private static LevelManager instance;

	// Token: 0x040007FF RID: 2047
	[SerializeField]
	private InputManager inputManager;

	// Token: 0x04000800 RID: 2048
	[SerializeField]
	private CharacterCreator characterCreator;

	// Token: 0x04000801 RID: 2049
	[SerializeField]
	private ExitCreator exitCreator;

	// Token: 0x04000802 RID: 2050
	[SerializeField]
	private ExplosionManager explosionManager;

	// Token: 0x04000803 RID: 2051
	[SerializeField]
	private CharacterModel characterModel;

	// Token: 0x04000804 RID: 2052
	private CharacterMainControl mainCharacter;

	// Token: 0x04000805 RID: 2053
	private CharacterMainControl petCharacter;

	// Token: 0x04000806 RID: 2054
	[SerializeField]
	private GameCamera gameCamera;

	// Token: 0x04000807 RID: 2055
	[SerializeField]
	private FogOfWarManager fowManager;

	// Token: 0x04000808 RID: 2056
	[SerializeField]
	private TimeOfDayController timeOfDayController;

	// Token: 0x0400080C RID: 2060
	[SerializeField]
	private AIMainBrain aiMainBrain;

	// Token: 0x0400080D RID: 2061
	[SerializeField]
	private CharacterRandomPreset matePreset;

	// Token: 0x0400080E RID: 2062
	private bool initingLevel;

	// Token: 0x0400080F RID: 2063
	private bool isNewRaidLevel;

	// Token: 0x04000810 RID: 2064
	private bool afterInit;

	// Token: 0x04000811 RID: 2065
	[SerializeField]
	private CharacterRandomPreset petPreset;

	// Token: 0x04000812 RID: 2066
	[SerializeField]
	private Sprite characterMapIcon;

	// Token: 0x04000813 RID: 2067
	[SerializeField]
	private Color characterMapIconColor;

	// Token: 0x04000814 RID: 2068
	[SerializeField]
	private Color characterMapShadowColor;

	// Token: 0x04000815 RID: 2069
	[SerializeField]
	private MultiSceneLocation testTeleportTarget;

	// Token: 0x04000816 RID: 2070
	[SerializeField]
	public SkillBase defaultSkill;

	// Token: 0x04000817 RID: 2071
	[SerializeField]
	private PetProxy petProxy;

	// Token: 0x04000818 RID: 2072
	[SerializeField]
	private CustomFaceManager customFaceManager;

	// Token: 0x04000819 RID: 2073
	[SerializeField]
	private BulletPool bulletPool;

	// Token: 0x0400081A RID: 2074
	private string _levelInitializingComment = "";

	// Token: 0x0400081B RID: 2075
	public static int loadLevelBeaconIndex = 0;

	// Token: 0x0400081D RID: 2077
	private bool levelInited;

	// Token: 0x0400081E RID: 2078
	public const string MainCharacterItemSaveKey = "MainCharacterItemData";

	// Token: 0x0400081F RID: 2079
	public const string MainCharacterHealthSaveKey = "MainCharacterHealth";

	// Token: 0x04000822 RID: 2082
	private float levelStartTime = -0.1f;

	// Token: 0x04000824 RID: 2084
	private static Ruleset rule;

	// Token: 0x04000825 RID: 2085
	private static List<object> waitForInitializationList = new List<object>();

	// Token: 0x04000826 RID: 2086
	private bool dieTask;

	// Token: 0x02000486 RID: 1158
	[Serializable]
	public struct LevelInfo
	{
		// Token: 0x04001B95 RID: 7061
		public bool isBaseLevel;

		// Token: 0x04001B96 RID: 7062
		public string sceneName;

		// Token: 0x04001B97 RID: 7063
		public string activeSubSceneID;
	}
}
