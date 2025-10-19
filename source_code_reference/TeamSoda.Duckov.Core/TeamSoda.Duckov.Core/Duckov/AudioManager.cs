using System;
using System.Collections.Generic;
using System.Linq;
using Duckov.Options;
using Duckov.Scenes;
using Duckov.UI;
using FMOD.Studio;
using FMODUnity;
using ItemStatsSystem;
using SodaCraft.StringUtilities;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

namespace Duckov
{
	// Token: 0x02000229 RID: 553
	public class AudioManager : MonoBehaviour
	{
		// Token: 0x170002F8 RID: 760
		// (get) Token: 0x06001107 RID: 4359 RVA: 0x00041F92 File Offset: 0x00040192
		public static AudioManager Instance
		{
			get
			{
				return GameManager.AudioManager;
			}
		}

		// Token: 0x170002F9 RID: 761
		// (get) Token: 0x06001108 RID: 4360 RVA: 0x00041F9C File Offset: 0x0004019C
		public static bool IsStingerPlaying
		{
			get
			{
				if (AudioManager.Instance == null)
				{
					return false;
				}
				if (AudioManager.Instance.stingerSource == null)
				{
					return false;
				}
				return AudioManager.Instance.stingerSource.events.Any((EventInstance e) => e.isValid());
			}
		}

		// Token: 0x06001109 RID: 4361 RVA: 0x00041FFF File Offset: 0x000401FF
		private IEnumerable<AudioManager.Bus> AllBueses()
		{
			yield return this.masterBus;
			yield return this.sfxBus;
			yield return this.musicBus;
			yield break;
		}

		// Token: 0x170002FA RID: 762
		// (get) Token: 0x0600110A RID: 4362 RVA: 0x0004200F File Offset: 0x0004020F
		private Transform listener
		{
			get
			{
				return base.transform;
			}
		}

		// Token: 0x170002FB RID: 763
		// (get) Token: 0x0600110B RID: 4363 RVA: 0x00042017 File Offset: 0x00040217
		private static Transform SoundSourceParent
		{
			get
			{
				if (AudioManager._soundSourceParent == null)
				{
					GameObject gameObject = new GameObject("Sound Sources");
					AudioManager._soundSourceParent = gameObject.transform;
					global::UnityEngine.Object.DontDestroyOnLoad(gameObject);
				}
				return AudioManager._soundSourceParent;
			}
		}

		// Token: 0x170002FC RID: 764
		// (get) Token: 0x0600110C RID: 4364 RVA: 0x00042048 File Offset: 0x00040248
		private static ObjectPool<GameObject> SoundSourcePool
		{
			get
			{
				if (AudioManager._soundSourcePool == null)
				{
					AudioManager._soundSourcePool = new ObjectPool<GameObject>(delegate
					{
						GameObject gameObject = new GameObject("SoundSource");
						gameObject.transform.SetParent(AudioManager.SoundSourceParent);
						return gameObject;
					}, delegate(GameObject e)
					{
						e.SetActive(true);
					}, delegate(GameObject e)
					{
						e.SetActive(false);
					}, null, true, 10, 10000);
				}
				return AudioManager._soundSourcePool;
			}
		}

		// Token: 0x0600110D RID: 4365 RVA: 0x000420D4 File Offset: 0x000402D4
		public static EventInstance? Post(string eventName, GameObject gameObject)
		{
			if (string.IsNullOrEmpty(eventName))
			{
				return null;
			}
			if (gameObject == null)
			{
				Debug.LogError(string.Format("Posting event but gameObject is null: {0}", gameObject));
			}
			if (!gameObject.activeSelf)
			{
				Debug.LogError(string.Format("Posting event but gameObject is not active: {0}", gameObject));
			}
			return AudioManager.Instance.MPost(eventName, gameObject);
		}

		// Token: 0x0600110E RID: 4366 RVA: 0x00042130 File Offset: 0x00040330
		public static EventInstance? Post(string eventName)
		{
			if (string.IsNullOrEmpty(eventName))
			{
				return null;
			}
			return AudioManager.Instance.MPost(eventName, null);
		}

		// Token: 0x0600110F RID: 4367 RVA: 0x0004215C File Offset: 0x0004035C
		public static EventInstance? Post(string eventName, Vector3 position)
		{
			if (string.IsNullOrEmpty(eventName))
			{
				return null;
			}
			return AudioManager.Instance.MPost(eventName, position);
		}

		// Token: 0x06001110 RID: 4368 RVA: 0x00042187 File Offset: 0x00040387
		internal static EventInstance? PostQuak(string soundKey, AudioManager.VoiceType voiceType, GameObject gameObject)
		{
			AudioObject orCreate = AudioObject.GetOrCreate(gameObject);
			orCreate.VoiceType = voiceType;
			return orCreate.PostQuak(soundKey);
		}

		// Token: 0x06001111 RID: 4369 RVA: 0x0004219C File Offset: 0x0004039C
		public static void PostHitMarker(bool crit)
		{
			AudioManager.Post(crit ? "SFX/Combat/Marker/hitmarker_head" : "SFX/Combat/Marker/hitmarker");
		}

		// Token: 0x06001112 RID: 4370 RVA: 0x000421B3 File Offset: 0x000403B3
		public static void PostKillMarker(bool crit = false)
		{
			AudioManager.Post(crit ? "SFX/Combat/Marker/killmarker_head" : "SFX/Combat/Marker/killmarker");
		}

		// Token: 0x06001113 RID: 4371 RVA: 0x000421CC File Offset: 0x000403CC
		private void Awake()
		{
			CharacterSoundMaker.OnFootStepSound = (Action<Vector3, CharacterSoundMaker.FootStepTypes, CharacterMainControl>)Delegate.Combine(CharacterSoundMaker.OnFootStepSound, new Action<Vector3, CharacterSoundMaker.FootStepTypes, CharacterMainControl>(this.OnFootStepSound));
			Projectile.OnBulletFlyByCharacter = (Action<Vector3>)Delegate.Combine(Projectile.OnBulletFlyByCharacter, new Action<Vector3>(this.OnBulletFlyby));
			MultiSceneCore.OnSubSceneLoaded += this.OnSubSceneLoaded;
			ItemUIUtilities.OnPutItem += this.OnPutItem;
			Health.OnDead += this.OnHealthDead;
			LevelManager.OnLevelInitialized += this.OnLevelInitialized;
			SceneLoader.onStartedLoadingScene += this.OnStartedLoadingScene;
			OptionsManager.OnOptionsChanged += this.OnOptionsChanged;
			foreach (AudioManager.Bus bus in this.AllBueses())
			{
				bus.LoadOptions();
			}
		}

		// Token: 0x06001114 RID: 4372 RVA: 0x000422BC File Offset: 0x000404BC
		private void OnDestroy()
		{
			CharacterSoundMaker.OnFootStepSound = (Action<Vector3, CharacterSoundMaker.FootStepTypes, CharacterMainControl>)Delegate.Remove(CharacterSoundMaker.OnFootStepSound, new Action<Vector3, CharacterSoundMaker.FootStepTypes, CharacterMainControl>(this.OnFootStepSound));
			Projectile.OnBulletFlyByCharacter = (Action<Vector3>)Delegate.Remove(Projectile.OnBulletFlyByCharacter, new Action<Vector3>(this.OnBulletFlyby));
			MultiSceneCore.OnSubSceneLoaded -= this.OnSubSceneLoaded;
			ItemUIUtilities.OnPutItem -= this.OnPutItem;
			Health.OnDead -= this.OnHealthDead;
			LevelManager.OnLevelInitialized -= this.OnLevelInitialized;
			SceneLoader.onStartedLoadingScene -= this.OnStartedLoadingScene;
			OptionsManager.OnOptionsChanged -= this.OnOptionsChanged;
		}

		// Token: 0x06001115 RID: 4373 RVA: 0x00042370 File Offset: 0x00040570
		private void OnOptionsChanged(string key)
		{
			foreach (AudioManager.Bus bus in this.AllBueses())
			{
				bus.NotifyOptionsChanged(key);
			}
		}

		// Token: 0x06001116 RID: 4374 RVA: 0x000423BC File Offset: 0x000405BC
		public static AudioManager.Bus GetBus(string name)
		{
			if (AudioManager.Instance == null)
			{
				return null;
			}
			foreach (AudioManager.Bus bus in AudioManager.Instance.AllBueses())
			{
				if (bus.Name == name)
				{
					return bus;
				}
			}
			return null;
		}

		// Token: 0x06001117 RID: 4375 RVA: 0x0004242C File Offset: 0x0004062C
		private void OnStartedLoadingScene(SceneLoadingContext context)
		{
			if (this.ambientSource)
			{
				this.ambientSource.StopAll(FMOD.Studio.STOP_MODE.IMMEDIATE);
			}
		}

		// Token: 0x06001118 RID: 4376 RVA: 0x00042447 File Offset: 0x00040647
		private void OnLevelInitialized()
		{
		}

		// Token: 0x06001119 RID: 4377 RVA: 0x00042449 File Offset: 0x00040649
		private void Start()
		{
			this.UpdateBuses();
		}

		// Token: 0x0600111A RID: 4378 RVA: 0x00042451 File Offset: 0x00040651
		private void OnHealthDead(Health health, DamageInfo info)
		{
			if (health.TryGetCharacter() == CharacterMainControl.Main)
			{
				AudioManager.StopBGM();
				AudioManager.Post("Music/Stinger/stg_death");
			}
		}

		// Token: 0x0600111B RID: 4379 RVA: 0x00042475 File Offset: 0x00040675
		private void OnPutItem(Item item, bool pickup = false)
		{
			AudioManager.PlayPutItemSFX(item, pickup);
		}

		// Token: 0x0600111C RID: 4380 RVA: 0x0004247E File Offset: 0x0004067E
		public static void PlayPutItemSFX(Item item, bool pickup = false)
		{
			if (item == null)
			{
				return;
			}
			if (!LevelManager.LevelInited)
			{
				return;
			}
			AudioManager.Post((pickup ? "SFX/Item/pickup_{soundkey}" : "SFX/Item/put_{soundkey}").Format(new
			{
				soundkey = item.SoundKey.ToLower()
			}));
		}

		// Token: 0x0600111D RID: 4381 RVA: 0x000424BC File Offset: 0x000406BC
		private void OnSubSceneLoaded(MultiSceneCore core, Scene scene)
		{
			LevelManager.LevelInitializingComment = "Opening ears";
			SubSceneEntry subSceneInfo = core.GetSubSceneInfo(scene);
			if (subSceneInfo == null)
			{
				return;
			}
			if (this.ambientSource)
			{
				LevelManager.LevelInitializingComment = "Hearing Ambient";
				this.ambientSource.StopAll(FMOD.Studio.STOP_MODE.IMMEDIATE);
				this.ambientSource.Post("Amb/amb_{soundkey}".Format(new
				{
					soundkey = subSceneInfo.AmbientSound.ToLower()
				}), true);
			}
			LevelManager.LevelInitializingComment = "Hearing Buses";
			this.ApplyBuses();
		}

		// Token: 0x170002FD RID: 765
		// (get) Token: 0x0600111E RID: 4382 RVA: 0x00042539 File Offset: 0x00040739
		public static bool PlayingBGM
		{
			get
			{
				return AudioManager.playingBGM;
			}
		}

		// Token: 0x170002FE RID: 766
		// (get) Token: 0x0600111F RID: 4383 RVA: 0x00042540 File Offset: 0x00040740
		private static bool LogEvent
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06001120 RID: 4384 RVA: 0x00042544 File Offset: 0x00040744
		public static bool TryCreateEventInstance(string eventPath, out EventInstance eventInstance)
		{
			eventInstance = default(EventInstance);
			if (AudioManager.Instance.useArchivedSound)
			{
				eventPath = "Archived/" + eventPath;
			}
			string text = "event:/" + eventPath;
			try
			{
				eventInstance = RuntimeManager.CreateInstance(text);
				return true;
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
				if (AudioManager.LogEvent)
				{
					Debug.LogError("[AudioEvent][Failed] " + text);
				}
			}
			return false;
		}

		// Token: 0x06001121 RID: 4385 RVA: 0x000425C0 File Offset: 0x000407C0
		public static void PlayBGM(string name)
		{
			AudioManager.StopBGM();
			if (AudioManager.Instance == null)
			{
				return;
			}
			AudioManager.playingBGM = true;
			if (string.IsNullOrWhiteSpace(name))
			{
				return;
			}
			string text = "Music/Loop/{soundkey}".Format(new
			{
				soundkey = name
			});
			if (AudioManager.Instance.bgmSource.Post(text, true) == null)
			{
				AudioManager.currentBGMName = null;
				return;
			}
			AudioManager.currentBGMName = name;
		}

		// Token: 0x06001122 RID: 4386 RVA: 0x00042628 File Offset: 0x00040828
		public static void StopBGM()
		{
			if (AudioManager.Instance == null)
			{
				return;
			}
			AudioManager.Instance.bgmSource.StopAll(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
			AudioManager.currentBGMName = null;
		}

		// Token: 0x06001123 RID: 4387 RVA: 0x00042650 File Offset: 0x00040850
		public static void PlayStringer(string key)
		{
			string text = "Music/Stinger/{key}".Format(new { key });
			AudioManager.Instance.stingerSource.Post(text, true);
		}

		// Token: 0x06001124 RID: 4388 RVA: 0x00042680 File Offset: 0x00040880
		private void OnBulletFlyby(Vector3 vector)
		{
			AudioManager.Post("SFX/Combat/Bullet/flyby", vector);
		}

		// Token: 0x06001125 RID: 4389 RVA: 0x0004268E File Offset: 0x0004088E
		public static void SetState(string stateGroup, string state)
		{
			AudioManager.globalStates[stateGroup] = state;
		}

		// Token: 0x06001126 RID: 4390 RVA: 0x0004269C File Offset: 0x0004089C
		public static string GetState(string stateGroup)
		{
			string text;
			if (AudioManager.globalStates.TryGetValue(stateGroup, out text))
			{
				return text;
			}
			return null;
		}

		// Token: 0x06001127 RID: 4391 RVA: 0x000426BB File Offset: 0x000408BB
		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Backslash))
			{
				this.useArchivedSound = !this.useArchivedSound;
				Debug.Log(string.Format("USE ARCHIVED SOUND:{0}", this.useArchivedSound));
			}
			this.UpdateListener();
			this.UpdateBuses();
		}

		// Token: 0x06001128 RID: 4392 RVA: 0x000426FC File Offset: 0x000408FC
		private void UpdateListener()
		{
			if (LevelManager.Instance == null)
			{
				Camera main = Camera.main;
				if (main != null)
				{
					this.listener.transform.position = main.transform.position;
					this.listener.transform.rotation = main.transform.rotation;
				}
				return;
			}
			GameCamera gameCamera = LevelManager.Instance.GameCamera;
			if (gameCamera != null)
			{
				if (CharacterMainControl.Main != null)
				{
					this.listener.transform.position = CharacterMainControl.Main.transform.position + Vector3.up * 2f;
				}
				else
				{
					this.listener.transform.position = gameCamera.renderCamera.transform.position;
				}
				this.listener.transform.rotation = gameCamera.renderCamera.transform.rotation;
			}
		}

		// Token: 0x06001129 RID: 4393 RVA: 0x000427F8 File Offset: 0x000409F8
		private void UpdateBuses()
		{
			foreach (AudioManager.Bus bus in this.AllBueses())
			{
				if (bus.Dirty)
				{
					bus.Apply();
				}
			}
		}

		// Token: 0x0600112A RID: 4394 RVA: 0x0004284C File Offset: 0x00040A4C
		private void ApplyBuses()
		{
			foreach (AudioManager.Bus bus in this.AllBueses())
			{
				bus.Apply();
			}
		}

		// Token: 0x0600112B RID: 4395 RVA: 0x00042898 File Offset: 0x00040A98
		private void OnFootStepSound(Vector3 position, CharacterSoundMaker.FootStepTypes type, CharacterMainControl character)
		{
			if (character == null)
			{
				return;
			}
			GameObject gameObject = character.gameObject;
			string text = "floor";
			this.MSetParameter(gameObject, "terrain", text);
			string text2 = character.FootStepMaterialType.ToString();
			string text3 = "light";
			switch (type)
			{
			case CharacterSoundMaker.FootStepTypes.walkLight:
			case CharacterSoundMaker.FootStepTypes.runLight:
				text3 = "light";
				break;
			case CharacterSoundMaker.FootStepTypes.walkHeavy:
			case CharacterSoundMaker.FootStepTypes.runHeavy:
				text3 = "heavy";
				break;
			}
			AudioManager.Post("Char/Footstep/footstep_{charaType}_{strengthType}".Format(new
			{
				charaType = text2,
				strengthType = text3
			}), character.gameObject);
		}

		// Token: 0x170002FF RID: 767
		// (get) Token: 0x0600112C RID: 4396 RVA: 0x00042928 File Offset: 0x00040B28
		public static bool Initialized
		{
			get
			{
				return RuntimeManager.IsInitialized;
			}
		}

		// Token: 0x0600112D RID: 4397 RVA: 0x0004292F File Offset: 0x00040B2F
		private void MSetParameter(GameObject gameObject, string parameterName, string value)
		{
			if (gameObject == null)
			{
				Debug.LogError("Game Object must exist");
				return;
			}
			AudioObject.GetOrCreate(gameObject).SetParameterByNameWithLabel(parameterName, value);
		}

		// Token: 0x0600112E RID: 4398 RVA: 0x00042954 File Offset: 0x00040B54
		private EventInstance? MPost(string eventName, GameObject gameObject = null)
		{
			if (!AudioManager.Initialized)
			{
				return null;
			}
			if (string.IsNullOrWhiteSpace(eventName))
			{
				return null;
			}
			if (gameObject == null)
			{
				gameObject = AudioManager.Instance.gameObject;
			}
			else if (!gameObject.activeInHierarchy)
			{
				Debug.LogWarning("Posting event on inactive object, canceled");
				return null;
			}
			return AudioObject.GetOrCreate(gameObject).Post(eventName ?? "", true);
		}

		// Token: 0x0600112F RID: 4399 RVA: 0x000429D0 File Offset: 0x00040BD0
		private EventInstance? MPost(string eventName, Vector3 position)
		{
			AudioManager.SoundSourcePool.Get().transform.position = position;
			EventInstance eventInstance;
			if (!AudioManager.TryCreateEventInstance(eventName ?? "", out eventInstance))
			{
				return null;
			}
			eventInstance.set3DAttributes(position.To3DAttributes());
			eventInstance.start();
			eventInstance.release();
			return new EventInstance?(eventInstance);
		}

		// Token: 0x06001130 RID: 4400 RVA: 0x00042A33 File Offset: 0x00040C33
		public static void StopAll(GameObject gameObject, FMOD.Studio.STOP_MODE mode = FMOD.Studio.STOP_MODE.IMMEDIATE)
		{
			AudioObject.GetOrCreate(gameObject).StopAll(mode);
		}

		// Token: 0x06001131 RID: 4401 RVA: 0x00042A44 File Offset: 0x00040C44
		internal void MSetRTPC(string key, float value, GameObject gameObject = null)
		{
			if (gameObject == null)
			{
				RuntimeManager.StudioSystem.setParameterByName("parameter:/" + key, value, false);
				if (AudioManager.LogEvent)
				{
					Debug.Log(string.Format("[AudioEvent][Parameter][Global] {0} = {1}", key, value));
					return;
				}
			}
			else
			{
				AudioObject.GetOrCreate(gameObject).SetParameterByName("parameter:/" + key, value);
				if (AudioManager.LogEvent)
				{
					Debug.Log(string.Format("[AudioEvent][Parameter][GameObject] {0} = {1}", key, value), gameObject);
				}
			}
		}

		// Token: 0x06001132 RID: 4402 RVA: 0x00042AC8 File Offset: 0x00040CC8
		internal static void SetRTPC(string key, float value, GameObject gameObject = null)
		{
			if (AudioManager.Instance == null)
			{
				return;
			}
			AudioManager.Instance.MSetRTPC(key, value, gameObject);
		}

		// Token: 0x06001133 RID: 4403 RVA: 0x00042AE5 File Offset: 0x00040CE5
		public static void SetVoiceType(GameObject gameObject, AudioManager.VoiceType voiceType)
		{
			if (gameObject == null)
			{
				return;
			}
			AudioObject.GetOrCreate(gameObject).VoiceType = voiceType;
		}

		// Token: 0x04000D48 RID: 3400
		private bool useArchivedSound;

		// Token: 0x04000D49 RID: 3401
		[SerializeField]
		private AudioObject ambientSource;

		// Token: 0x04000D4A RID: 3402
		[SerializeField]
		private AudioObject bgmSource;

		// Token: 0x04000D4B RID: 3403
		[SerializeField]
		private AudioObject stingerSource;

		// Token: 0x04000D4C RID: 3404
		[SerializeField]
		private AudioManager.Bus masterBus = new AudioManager.Bus("Master");

		// Token: 0x04000D4D RID: 3405
		[SerializeField]
		private AudioManager.Bus sfxBus = new AudioManager.Bus("Master/SFX");

		// Token: 0x04000D4E RID: 3406
		[SerializeField]
		private AudioManager.Bus musicBus = new AudioManager.Bus("Master/Music");

		// Token: 0x04000D4F RID: 3407
		private static Transform _soundSourceParent;

		// Token: 0x04000D50 RID: 3408
		private static ObjectPool<GameObject> _soundSourcePool;

		// Token: 0x04000D51 RID: 3409
		private const string path_hitmarker_norm = "SFX/Combat/Marker/hitmarker";

		// Token: 0x04000D52 RID: 3410
		private const string path_hitmarker_crit = "SFX/Combat/Marker/hitmarker_head";

		// Token: 0x04000D53 RID: 3411
		private const string path_killmarker_norm = "SFX/Combat/Marker/killmarker";

		// Token: 0x04000D54 RID: 3412
		private const string path_killmarker_crit = "SFX/Combat/Marker/killmarker_head";

		// Token: 0x04000D55 RID: 3413
		private const string path_music_death = "Music/Stinger/stg_death";

		// Token: 0x04000D56 RID: 3414
		private const string path_bullet_flyby = "SFX/Combat/Bullet/flyby";

		// Token: 0x04000D57 RID: 3415
		private const string path_pickup_item_fmt_soundkey = "SFX/Item/pickup_{soundkey}";

		// Token: 0x04000D58 RID: 3416
		private const string path_put_item_fmt_soundkey = "SFX/Item/put_{soundkey}";

		// Token: 0x04000D59 RID: 3417
		private const string path_ambient_fmt_soundkey = "Amb/amb_{soundkey}";

		// Token: 0x04000D5A RID: 3418
		private const string path_music_loop_fmt_soundkey = "Music/Loop/{soundkey}";

		// Token: 0x04000D5B RID: 3419
		private const string path_footstep_fmt_soundkey = "Char/Footstep/footstep_{charaType}_{strengthType}";

		// Token: 0x04000D5C RID: 3420
		public const string path_reload_fmt_soundkey = "SFX/Combat/Gun/Reload/{soundkey}";

		// Token: 0x04000D5D RID: 3421
		public const string path_shoot_fmt_gunkey = "SFX/Combat/Gun/Shoot/{soundkey}";

		// Token: 0x04000D5E RID: 3422
		public const string path_task_finished = "UI/mission_small";

		// Token: 0x04000D5F RID: 3423
		public const string path_building_built = "UI/building_up";

		// Token: 0x04000D60 RID: 3424
		public const string path_gun_unload = "SFX/Combat/Gun/unload";

		// Token: 0x04000D61 RID: 3425
		public const string path_stinger_fmt_key = "Music/Stinger/{key}";

		// Token: 0x04000D62 RID: 3426
		private static bool playingBGM;

		// Token: 0x04000D63 RID: 3427
		private static EventInstance bgmEvent;

		// Token: 0x04000D64 RID: 3428
		private static string currentBGMName;

		// Token: 0x04000D65 RID: 3429
		private static Dictionary<string, string> globalStates = new Dictionary<string, string>();

		// Token: 0x04000D66 RID: 3430
		private static Dictionary<int, AudioManager.VoiceType> gameObjectVoiceTypes = new Dictionary<int, AudioManager.VoiceType>();

		// Token: 0x0200051E RID: 1310
		[Serializable]
		public class Bus
		{
			// Token: 0x1700074B RID: 1867
			// (get) Token: 0x06002764 RID: 10084 RVA: 0x000900A0 File Offset: 0x0008E2A0
			public string Name
			{
				get
				{
					return this.volumeRTPC;
				}
			}

			// Token: 0x1700074C RID: 1868
			// (get) Token: 0x06002765 RID: 10085 RVA: 0x000900A8 File Offset: 0x0008E2A8
			// (set) Token: 0x06002766 RID: 10086 RVA: 0x000900B0 File Offset: 0x0008E2B0
			public float Volume
			{
				get
				{
					return this.volume;
				}
				set
				{
					this.volume = value;
					this.Apply();
				}
			}

			// Token: 0x1700074D RID: 1869
			// (get) Token: 0x06002767 RID: 10087 RVA: 0x000900BF File Offset: 0x0008E2BF
			// (set) Token: 0x06002768 RID: 10088 RVA: 0x000900C7 File Offset: 0x0008E2C7
			public bool Mute
			{
				get
				{
					return this.mute;
				}
				set
				{
					this.mute = value;
					this.Apply();
				}
			}

			// Token: 0x1700074E RID: 1870
			// (get) Token: 0x06002769 RID: 10089 RVA: 0x000900D6 File Offset: 0x0008E2D6
			public bool Dirty
			{
				get
				{
					return this.appliedVolume != this.Volume;
				}
			}

			// Token: 0x0600276A RID: 10090 RVA: 0x000900EC File Offset: 0x0008E2EC
			public void Apply()
			{
				try
				{
					FMOD.Studio.Bus bus = RuntimeManager.GetBus("bus:/" + this.volumeRTPC);
					bus.setVolume(this.Volume);
					bus.setMute(this.Mute);
				}
				catch (Exception ex)
				{
					Debug.LogException(ex);
				}
				this.appliedVolume = this.Volume;
				OptionsManager.Save<float>(this.SaveKey, this.volume);
			}

			// Token: 0x1700074F RID: 1871
			// (get) Token: 0x0600276B RID: 10091 RVA: 0x00090164 File Offset: 0x0008E364
			private string SaveKey
			{
				get
				{
					return "Audio/" + this.volumeRTPC;
				}
			}

			// Token: 0x0600276C RID: 10092 RVA: 0x00090176 File Offset: 0x0008E376
			public Bus(string rtpc)
			{
				this.volumeRTPC = rtpc;
			}

			// Token: 0x0600276D RID: 10093 RVA: 0x000901A6 File Offset: 0x0008E3A6
			internal void LoadOptions()
			{
				this.volume = OptionsManager.Load<float>(this.SaveKey, 1f);
			}

			// Token: 0x0600276E RID: 10094 RVA: 0x000901BE File Offset: 0x0008E3BE
			internal void NotifyOptionsChanged(string key)
			{
				if (key == this.SaveKey)
				{
					this.LoadOptions();
				}
			}

			// Token: 0x04001E27 RID: 7719
			[SerializeField]
			private string volumeRTPC = "Master";

			// Token: 0x04001E28 RID: 7720
			[HideInInspector]
			[SerializeField]
			private float volume = 1f;

			// Token: 0x04001E29 RID: 7721
			[HideInInspector]
			[SerializeField]
			private bool mute;

			// Token: 0x04001E2A RID: 7722
			private float appliedVolume = float.MinValue;
		}

		// Token: 0x0200051F RID: 1311
		public enum FootStepMaterialType
		{
			// Token: 0x04001E2C RID: 7724
			organic,
			// Token: 0x04001E2D RID: 7725
			mech,
			// Token: 0x04001E2E RID: 7726
			danger
		}

		// Token: 0x02000520 RID: 1312
		public enum VoiceType
		{
			// Token: 0x04001E30 RID: 7728
			Duck,
			// Token: 0x04001E31 RID: 7729
			Robot,
			// Token: 0x04001E32 RID: 7730
			Wolf,
			// Token: 0x04001E33 RID: 7731
			Chicken,
			// Token: 0x04001E34 RID: 7732
			Crow,
			// Token: 0x04001E35 RID: 7733
			Eagle
		}
	}
}
