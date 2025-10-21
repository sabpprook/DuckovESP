using System;
using System.Runtime.CompilerServices;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.CompilerServices;
using Duckov;
using Duckov.Scenes;
using Duckov.UI.Animations;
using Duckov.Utilities;
using Eflatun.SceneReference;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

// Token: 0x02000129 RID: 297
public class SceneLoader : MonoBehaviour
{
	// Token: 0x170001FE RID: 510
	// (get) Token: 0x060009AB RID: 2475 RVA: 0x00029B29 File Offset: 0x00027D29
	public static SceneLoader Instance
	{
		get
		{
			return GameManager.SceneLoader;
		}
	}

	// Token: 0x170001FF RID: 511
	// (get) Token: 0x060009AC RID: 2476 RVA: 0x00029B30 File Offset: 0x00027D30
	// (set) Token: 0x060009AD RID: 2477 RVA: 0x00029B37 File Offset: 0x00027D37
	public static bool IsSceneLoading { get; private set; }

	// Token: 0x14000047 RID: 71
	// (add) Token: 0x060009AE RID: 2478 RVA: 0x00029B40 File Offset: 0x00027D40
	// (remove) Token: 0x060009AF RID: 2479 RVA: 0x00029B74 File Offset: 0x00027D74
	public static event Action<SceneLoadingContext> onStartedLoadingScene;

	// Token: 0x14000048 RID: 72
	// (add) Token: 0x060009B0 RID: 2480 RVA: 0x00029BA8 File Offset: 0x00027DA8
	// (remove) Token: 0x060009B1 RID: 2481 RVA: 0x00029BDC File Offset: 0x00027DDC
	public static event Action<SceneLoadingContext> onFinishedLoadingScene;

	// Token: 0x14000049 RID: 73
	// (add) Token: 0x060009B2 RID: 2482 RVA: 0x00029C10 File Offset: 0x00027E10
	// (remove) Token: 0x060009B3 RID: 2483 RVA: 0x00029C44 File Offset: 0x00027E44
	public static event Action<SceneLoadingContext> onBeforeSetSceneActive;

	// Token: 0x1400004A RID: 74
	// (add) Token: 0x060009B4 RID: 2484 RVA: 0x00029C78 File Offset: 0x00027E78
	// (remove) Token: 0x060009B5 RID: 2485 RVA: 0x00029CAC File Offset: 0x00027EAC
	public static event Action<SceneLoadingContext> onAfterSceneInitialize;

	// Token: 0x17000200 RID: 512
	// (get) Token: 0x060009B6 RID: 2486 RVA: 0x00029CDF File Offset: 0x00027EDF
	// (set) Token: 0x060009B7 RID: 2487 RVA: 0x00029D07 File Offset: 0x00027F07
	public static string LoadingComment
	{
		get
		{
			if (LevelManager.LevelInitializing)
			{
				return LevelManager.LevelInitializingComment;
			}
			if (SceneLoader.Instance != null)
			{
				return SceneLoader.Instance._loadingComment;
			}
			return null;
		}
		set
		{
			if (SceneLoader.Instance == null)
			{
				return;
			}
			SceneLoader.Instance._loadingComment = value;
			Action<string> onSetLoadingComment = SceneLoader.OnSetLoadingComment;
			if (onSetLoadingComment == null)
			{
				return;
			}
			onSetLoadingComment(value);
		}
	}

	// Token: 0x1400004B RID: 75
	// (add) Token: 0x060009B8 RID: 2488 RVA: 0x00029D34 File Offset: 0x00027F34
	// (remove) Token: 0x060009B9 RID: 2489 RVA: 0x00029D68 File Offset: 0x00027F68
	public static event Action<string> OnSetLoadingComment;

	// Token: 0x060009BA RID: 2490 RVA: 0x00029D9C File Offset: 0x00027F9C
	private void Awake()
	{
		if (SceneLoader.Instance != this)
		{
			Debug.LogError(base.gameObject.scene.name + " 场景中出现了应当删除的Scene Loader");
			global::UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		this.pointerClickEventRecevier.onPointerClick.AddListener(new UnityAction<PointerEventData>(this.NotifyPointerClick));
		this.pointerClickEventRecevier.gameObject.SetActive(false);
		this.content.Hide();
	}

	// Token: 0x060009BB RID: 2491 RVA: 0x00029E1C File Offset: 0x0002801C
	public async UniTask LoadScene(string sceneID, MultiSceneLocation location, SceneReference overrideCurtainScene = null, bool clickToConinue = false, bool notifyEvacuation = false, bool doCircleFade = true, bool saveToFile = true, bool hideTips = false)
	{
		await this.LoadScene(sceneID, overrideCurtainScene, clickToConinue, notifyEvacuation, doCircleFade, true, location, saveToFile, hideTips);
	}

	// Token: 0x060009BC RID: 2492 RVA: 0x00029EA4 File Offset: 0x000280A4
	public async UniTask LoadScene(string sceneID, SceneReference overrideCurtainScene = null, bool clickToConinue = false, bool notifyEvacuation = false, bool doCircleFade = true, bool useLocation = false, MultiSceneLocation location = default(MultiSceneLocation), bool saveToFile = true, bool hideTips = false)
	{
		SceneInfoEntry sceneInfo = SceneInfoCollection.GetSceneInfo(sceneID);
		if (sceneInfo != null)
		{
			if (sceneInfo.SceneReference.UnsafeReason == SceneReferenceUnsafeReason.None)
			{
				await this.LoadScene(sceneInfo.SceneReference, overrideCurtainScene, clickToConinue, notifyEvacuation, doCircleFade, useLocation, location, saveToFile, hideTips);
			}
		}
	}

	// Token: 0x17000201 RID: 513
	// (get) Token: 0x060009BD RID: 2493 RVA: 0x00029F35 File Offset: 0x00028135
	// (set) Token: 0x060009BE RID: 2494 RVA: 0x00029F3C File Offset: 0x0002813C
	public static bool HideTips { get; private set; }

	// Token: 0x060009BF RID: 2495 RVA: 0x00029F44 File Offset: 0x00028144
	public UniTask LoadScene(SceneReference sceneReference, SceneReference overrideCurtainScene = null, bool clickToConinue = false, bool notifyEvacuation = false, bool doCircleFade = true, bool useLocation = false, MultiSceneLocation location = default(MultiSceneLocation), bool saveToFile = true, bool hideTips = false)
	{
		SceneLoader.<LoadScene>d__45 <LoadScene>d__;
		<LoadScene>d__.<>t__builder = AsyncUniTaskMethodBuilder.Create();
		<LoadScene>d__.<>4__this = this;
		<LoadScene>d__.sceneReference = sceneReference;
		<LoadScene>d__.overrideCurtainScene = overrideCurtainScene;
		<LoadScene>d__.clickToConinue = clickToConinue;
		<LoadScene>d__.notifyEvacuation = notifyEvacuation;
		<LoadScene>d__.doCircleFade = doCircleFade;
		<LoadScene>d__.useLocation = useLocation;
		<LoadScene>d__.location = location;
		<LoadScene>d__.saveToFile = saveToFile;
		<LoadScene>d__.hideTips = hideTips;
		<LoadScene>d__.<>1__state = -1;
		<LoadScene>d__.<>t__builder.Start<SceneLoader.<LoadScene>d__45>(ref <LoadScene>d__);
		return <LoadScene>d__.<>t__builder.Task;
	}

	// Token: 0x060009C0 RID: 2496 RVA: 0x00029FD8 File Offset: 0x000281D8
	public void LoadTarget()
	{
		this.LoadScene(this.target, null, false, false, true, false, default(MultiSceneLocation), true, false).Forget();
	}

	// Token: 0x060009C1 RID: 2497 RVA: 0x0002A008 File Offset: 0x00028208
	public async UniTask LoadBaseScene(SceneReference overrideCurtainScene = null, bool doCircleFade = true)
	{
		GameplayDataSettings.SceneManagementData sceneManagement = GameplayDataSettings.SceneManagement;
		SceneReference sceneReference = ((sceneManagement != null) ? sceneManagement.BaseScene : null);
		if (sceneReference == null)
		{
			Debug.LogError("未配置基地场景(GameplayDataSettings/SceneManagement/BaseScene)");
		}
		await this.LoadScene(sceneReference, overrideCurtainScene, true, false, doCircleFade, false, default(MultiSceneLocation), true, false);
	}

	// Token: 0x060009C2 RID: 2498 RVA: 0x0002A05B File Offset: 0x0002825B
	public void NotifyPointerClick(PointerEventData eventData)
	{
		this.clicked = true;
		AudioManager.Post("UI/sceneloader_click");
	}

	// Token: 0x060009C3 RID: 2499 RVA: 0x0002A06F File Offset: 0x0002826F
	internal static void StaticLoadSingle(SceneReference sceneReference)
	{
		SceneManager.LoadScene(sceneReference.Name, LoadSceneMode.Single);
	}

	// Token: 0x060009C4 RID: 2500 RVA: 0x0002A07D File Offset: 0x0002827D
	internal static void StaticLoadSingle(string sceneID)
	{
		SceneManager.LoadScene(SceneInfoCollection.GetBuildIndex(sceneID), LoadSceneMode.Single);
	}

	// Token: 0x060009C5 RID: 2501 RVA: 0x0002A08C File Offset: 0x0002828C
	public static void LoadMainMenu(bool circleFade = true)
	{
		if (SceneLoader.Instance)
		{
			SceneLoader.Instance.LoadScene(GameplayDataSettings.SceneManagement.MainMenuScene, null, false, false, circleFade, false, default(MultiSceneLocation), true, false).Forget();
		}
	}

	// Token: 0x060009C7 RID: 2503 RVA: 0x0002A0EC File Offset: 0x000282EC
	[CompilerGenerated]
	internal static float <LoadScene>g__TimeSinceLoadingStarted|45_0(ref SceneLoader.<>c__DisplayClass45_0 A_0)
	{
		return Time.unscaledTime - A_0.timeWhenLoadingStarted;
	}

	// Token: 0x04000874 RID: 2164
	public SceneReference defaultCurtainScene;

	// Token: 0x04000875 RID: 2165
	[SerializeField]
	private OnPointerClick pointerClickEventRecevier;

	// Token: 0x04000876 RID: 2166
	[SerializeField]
	private float minimumLoadingTime = 1f;

	// Token: 0x04000877 RID: 2167
	[SerializeField]
	private float waitAfterSceneLoaded = 1f;

	// Token: 0x04000878 RID: 2168
	[SerializeField]
	private FadeGroup content;

	// Token: 0x04000879 RID: 2169
	[SerializeField]
	private FadeGroup loadingIndicator;

	// Token: 0x0400087A RID: 2170
	[SerializeField]
	private FadeGroup clickIndicator;

	// Token: 0x0400087B RID: 2171
	[SerializeField]
	private AnimationCurve fadeCurve1;

	// Token: 0x0400087C RID: 2172
	[SerializeField]
	private AnimationCurve fadeCurve2;

	// Token: 0x0400087D RID: 2173
	[SerializeField]
	private AnimationCurve fadeCurve3;

	// Token: 0x0400087E RID: 2174
	[SerializeField]
	private AnimationCurve fadeCurve4;

	// Token: 0x04000884 RID: 2180
	private string _loadingComment;

	// Token: 0x04000886 RID: 2182
	[SerializeField]
	private SceneReference target;

	// Token: 0x04000887 RID: 2183
	private bool clicked;
}
