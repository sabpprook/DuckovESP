using System;
using DG.Tweening;
using Duckov;
using Duckov.Achievements;
using Duckov.Modding;
using Duckov.NoteIndexs;
using Duckov.Rules;
using Duckov.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

// Token: 0x0200009B RID: 155
public class GameManager : MonoBehaviour
{
	// Token: 0x1700010E RID: 270
	// (get) Token: 0x0600052A RID: 1322 RVA: 0x00017578 File Offset: 0x00015778
	public static GameManager Instance
	{
		get
		{
			if (!Application.isPlaying)
			{
				return null;
			}
			if (GameManager._instance == null)
			{
				GameManager._instance = global::UnityEngine.Object.FindObjectOfType<GameManager>();
				if (GameManager._instance)
				{
					global::UnityEngine.Object.DontDestroyOnLoad(GameManager._instance.gameObject);
				}
			}
			if (GameManager._instance == null)
			{
				GameObject gameObject = Resources.Load<GameObject>("GameManager");
				if (gameObject == null)
				{
					Debug.LogError("Resources中找不到GameManager的Prefab");
				}
				GameManager component = global::UnityEngine.Object.Instantiate<GameObject>(gameObject).GetComponent<GameManager>();
				if (component == null)
				{
					Debug.LogError("GameManager的prefab上没有GameManager组件");
					return null;
				}
				GameManager._instance = component;
				if (GameManager._instance)
				{
					global::UnityEngine.Object.DontDestroyOnLoad(GameManager._instance.gameObject);
				}
			}
			return GameManager._instance;
		}
	}

	// Token: 0x1700010F RID: 271
	// (get) Token: 0x0600052B RID: 1323 RVA: 0x00017630 File Offset: 0x00015830
	public static bool Paused
	{
		get
		{
			return !(GameManager.Instance == null) && GameManager.Instance.pauseMenu.Shown;
		}
	}

	// Token: 0x17000110 RID: 272
	// (get) Token: 0x0600052C RID: 1324 RVA: 0x00017655 File Offset: 0x00015855
	public static AudioManager AudioManager
	{
		get
		{
			return GameManager.Instance.audioManager;
		}
	}

	// Token: 0x17000111 RID: 273
	// (get) Token: 0x0600052D RID: 1325 RVA: 0x00017661 File Offset: 0x00015861
	public static UIInputManager UiInputManager
	{
		get
		{
			return GameManager.Instance.uiInputManager;
		}
	}

	// Token: 0x17000112 RID: 274
	// (get) Token: 0x0600052E RID: 1326 RVA: 0x0001766D File Offset: 0x0001586D
	public static PauseMenu PauseMenu
	{
		get
		{
			return GameManager.Instance.pauseMenu;
		}
	}

	// Token: 0x17000113 RID: 275
	// (get) Token: 0x0600052F RID: 1327 RVA: 0x00017679 File Offset: 0x00015879
	public static GameRulesManager DifficultyManager
	{
		get
		{
			return GameManager.Instance.difficultyManager;
		}
	}

	// Token: 0x17000114 RID: 276
	// (get) Token: 0x06000530 RID: 1328 RVA: 0x00017685 File Offset: 0x00015885
	public static SceneLoader SceneLoader
	{
		get
		{
			return GameManager.Instance.sceneLoader;
		}
	}

	// Token: 0x17000115 RID: 277
	// (get) Token: 0x06000531 RID: 1329 RVA: 0x00017691 File Offset: 0x00015891
	public static BlackScreen BlackScreen
	{
		get
		{
			return GameManager.Instance.blackScreen;
		}
	}

	// Token: 0x17000116 RID: 278
	// (get) Token: 0x06000532 RID: 1330 RVA: 0x0001769D File Offset: 0x0001589D
	public static EventSystem EventSystem
	{
		get
		{
			return GameManager.Instance.eventSystem;
		}
	}

	// Token: 0x17000117 RID: 279
	// (get) Token: 0x06000533 RID: 1331 RVA: 0x000176A9 File Offset: 0x000158A9
	public static NightVisionVisual NightVision
	{
		get
		{
			return GameManager.Instance.nightVision;
		}
	}

	// Token: 0x17000118 RID: 280
	// (get) Token: 0x06000534 RID: 1332 RVA: 0x000176B5 File Offset: 0x000158B5
	public static bool BloodFxOn
	{
		get
		{
			return GameMetaData.BloodFxOn;
		}
	}

	// Token: 0x17000119 RID: 281
	// (get) Token: 0x06000535 RID: 1333 RVA: 0x000176BC File Offset: 0x000158BC
	public static PlayerInput MainPlayerInput
	{
		get
		{
			return GameManager.Instance.mainPlayerInput;
		}
	}

	// Token: 0x1700011A RID: 282
	// (get) Token: 0x06000536 RID: 1334 RVA: 0x000176C8 File Offset: 0x000158C8
	public static ModManager ModManager
	{
		get
		{
			return GameManager.Instance.modManager;
		}
	}

	// Token: 0x1700011B RID: 283
	// (get) Token: 0x06000537 RID: 1335 RVA: 0x000176D4 File Offset: 0x000158D4
	public static NoteIndex NoteIndex
	{
		get
		{
			return GameManager.Instance.noteIndex;
		}
	}

	// Token: 0x1700011C RID: 284
	// (get) Token: 0x06000538 RID: 1336 RVA: 0x000176E0 File Offset: 0x000158E0
	public static AchievementManager AchievementManager
	{
		get
		{
			return GameManager.Instance.achievementManager;
		}
	}

	// Token: 0x06000539 RID: 1337 RVA: 0x000176EC File Offset: 0x000158EC
	private void Awake()
	{
		if (GameManager._instance == null)
		{
			GameManager._instance = this;
			global::UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		}
		else if (GameManager._instance != this)
		{
			global::UnityEngine.Object.Destroy(base.gameObject);
		}
		DOTween.defaultTimeScaleIndependent = true;
		DebugManager.instance.enableRuntimeUI = false;
		DebugManager.instance.displayRuntimeUI = false;
	}

	// Token: 0x0600053A RID: 1338 RVA: 0x0001774D File Offset: 0x0001594D
	private void Update()
	{
		bool isEditor = Application.isEditor;
	}

	// Token: 0x0600053B RID: 1339 RVA: 0x00017755 File Offset: 0x00015955
	public static void TimeTravelDetected()
	{
		Debug.Log("检测到穿越者");
	}

	// Token: 0x040004A6 RID: 1190
	private static GameManager _instance;

	// Token: 0x040004A7 RID: 1191
	[SerializeField]
	private AudioManager audioManager;

	// Token: 0x040004A8 RID: 1192
	[SerializeField]
	private UIInputManager uiInputManager;

	// Token: 0x040004A9 RID: 1193
	[SerializeField]
	private GameRulesManager difficultyManager;

	// Token: 0x040004AA RID: 1194
	[SerializeField]
	private PauseMenu pauseMenu;

	// Token: 0x040004AB RID: 1195
	[SerializeField]
	private SceneLoader sceneLoader;

	// Token: 0x040004AC RID: 1196
	[SerializeField]
	private BlackScreen blackScreen;

	// Token: 0x040004AD RID: 1197
	[SerializeField]
	private EventSystem eventSystem;

	// Token: 0x040004AE RID: 1198
	[SerializeField]
	private PlayerInput mainPlayerInput;

	// Token: 0x040004AF RID: 1199
	[SerializeField]
	private NightVisionVisual nightVision;

	// Token: 0x040004B0 RID: 1200
	[SerializeField]
	private ModManager modManager;

	// Token: 0x040004B1 RID: 1201
	[SerializeField]
	private NoteIndex noteIndex;

	// Token: 0x040004B2 RID: 1202
	[SerializeField]
	private AchievementManager achievementManager;

	// Token: 0x040004B3 RID: 1203
	public static bool newBoot;
}
