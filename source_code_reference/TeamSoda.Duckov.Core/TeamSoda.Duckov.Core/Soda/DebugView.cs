using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using ItemStatsSystem;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

namespace Soda
{
	// Token: 0x02000224 RID: 548
	public class DebugView : MonoBehaviour
	{
		// Token: 0x170002F5 RID: 757
		// (get) Token: 0x060010A9 RID: 4265 RVA: 0x000407F5 File Offset: 0x0003E9F5
		public DebugView Instance
		{
			get
			{
				return this.instance;
			}
		}

		// Token: 0x170002F6 RID: 758
		// (get) Token: 0x060010AA RID: 4266 RVA: 0x000407FD File Offset: 0x0003E9FD
		public bool EdgeLightActive
		{
			get
			{
				return this.edgeLightActive;
			}
		}

		// Token: 0x14000073 RID: 115
		// (add) Token: 0x060010AB RID: 4267 RVA: 0x00040808 File Offset: 0x0003EA08
		// (remove) Token: 0x060010AC RID: 4268 RVA: 0x0004083C File Offset: 0x0003EA3C
		public static event Action<DebugView> OnDebugViewConfigChanged;

		// Token: 0x060010AD RID: 4269 RVA: 0x0004086F File Offset: 0x0003EA6F
		private void Awake()
		{
		}

		// Token: 0x060010AE RID: 4270 RVA: 0x00040871 File Offset: 0x0003EA71
		private void OnDestroy()
		{
			LevelManager.OnLevelInitialized -= this.OnlevelInited;
			SceneManager.activeSceneChanged -= this.OnSceneLoaded;
		}

		// Token: 0x060010AF RID: 4271 RVA: 0x00040898 File Offset: 0x0003EA98
		private void InitFromData()
		{
			if (PlayerPrefs.HasKey("ResMode"))
			{
				this.resMode = (ResModes)PlayerPrefs.GetInt("ResMode");
			}
			else
			{
				this.resMode = ResModes.R720p;
			}
			if (PlayerPrefs.HasKey("TexMode"))
			{
				this.texMode = (TextureModes)PlayerPrefs.GetInt("TexMode");
			}
			else
			{
				this.texMode = TextureModes.High;
			}
			if (PlayerPrefs.HasKey("InputDevice"))
			{
				this.inputDevice = PlayerPrefs.GetInt("InputDevice");
			}
			else
			{
				this.inputDevice = 1;
			}
			if (PlayerPrefs.HasKey("BloomActive"))
			{
				this.bloomActive = PlayerPrefs.GetInt("BloomActive") != 0;
			}
			else
			{
				this.bloomActive = true;
			}
			if (PlayerPrefs.HasKey("EdgeLightActive"))
			{
				this.edgeLightActive = PlayerPrefs.GetInt("EdgeLightActive") != 0;
			}
			else
			{
				this.edgeLightActive = true;
			}
			if (PlayerPrefs.HasKey("AOActive"))
			{
				this.aoActive = PlayerPrefs.GetInt("AOActive") != 0;
			}
			else
			{
				this.aoActive = false;
			}
			if (PlayerPrefs.HasKey("DofActive"))
			{
				this.dofActive = PlayerPrefs.GetInt("DofActive") != 0;
			}
			else
			{
				this.dofActive = false;
			}
			if (PlayerPrefs.HasKey("ReporterActive"))
			{
				this.reporterActive = PlayerPrefs.GetInt("ReporterActive") != 0;
				return;
			}
			this.reporterActive = false;
		}

		// Token: 0x060010B0 RID: 4272 RVA: 0x000409DC File Offset: 0x0003EBDC
		private void Update()
		{
			this.deltaTimes[this.frameIndex] = Time.deltaTime;
			this.frameIndex++;
			if (this.frameIndex >= this.frameSampleCount)
			{
				this.frameIndex = 0;
				float num = 0f;
				for (int i = 0; i < this.frameSampleCount; i++)
				{
					num += this.deltaTimes[i];
				}
				int num2 = Mathf.RoundToInt((float)this.frameSampleCount / Mathf.Max(0.0001f, num));
				this.fpsText1.text = num2.ToString();
				this.fpsText2.text = num2.ToString();
			}
		}

		// Token: 0x060010B1 RID: 4273 RVA: 0x00040A80 File Offset: 0x0003EC80
		public void SetInputDevice(int type)
		{
			if (!true)
			{
				InputManager.SetInputDevice(InputManager.InputDevices.touch);
				this.inputDeviceText.text = "触摸";
				PlayerPrefs.SetInt("InputDevice", 0);
				return;
			}
			InputManager.SetInputDevice(InputManager.InputDevices.mouseKeyboard);
			this.inputDeviceText.text = "键鼠";
			PlayerPrefs.SetInt("InputDevice", 1);
		}

		// Token: 0x060010B2 RID: 4274 RVA: 0x00040AD6 File Offset: 0x0003ECD6
		public void SetRes(int resModeIndex)
		{
			this.SetRes((ResModes)resModeIndex);
		}

		// Token: 0x060010B3 RID: 4275 RVA: 0x00040AE0 File Offset: 0x0003ECE0
		public void SetRes(ResModes mode)
		{
			this.resMode = mode;
			this.screenRes.x = (float)Display.main.systemWidth;
			this.screenRes.y = (float)Display.main.systemHeight;
			PlayerPrefs.SetInt("ResMode", (int)mode);
			int num = 1;
			int num2 = 1;
			switch (this.resMode)
			{
			case ResModes.Source:
				num = Mathf.RoundToInt(this.screenRes.x);
				num2 = Mathf.RoundToInt(this.screenRes.y);
				break;
			case ResModes.HalfRes:
				num = Mathf.RoundToInt(this.screenRes.x / 2f);
				num2 = Mathf.RoundToInt(this.screenRes.y / 2f);
				break;
			case ResModes.R720p:
				num = Mathf.RoundToInt(this.screenRes.x / this.screenRes.y * 720f);
				num2 = 720;
				break;
			case ResModes.R480p:
				num = Mathf.RoundToInt(this.screenRes.x / this.screenRes.y * 480f);
				num2 = 480;
				break;
			}
			this.resText.text = string.Format("{0}x{1}", num, num2);
			Screen.SetResolution(num, num2, FullScreenMode.FullScreenWindow);
			Action<DebugView> onDebugViewConfigChanged = DebugView.OnDebugViewConfigChanged;
			if (onDebugViewConfigChanged == null)
			{
				return;
			}
			onDebugViewConfigChanged(this);
		}

		// Token: 0x060010B4 RID: 4276 RVA: 0x00040C35 File Offset: 0x0003EE35
		public void SetTexture(int texModeIndex)
		{
			this.SetTexture((TextureModes)texModeIndex);
		}

		// Token: 0x060010B5 RID: 4277 RVA: 0x00040C40 File Offset: 0x0003EE40
		public void SetTexture(TextureModes mode)
		{
			this.texMode = mode;
			QualitySettings.globalTextureMipmapLimit = (int)this.texMode;
			switch (this.texMode)
			{
			case TextureModes.High:
				this.texText.text = "高";
				break;
			case TextureModes.Middle:
				this.texText.text = "中";
				break;
			case TextureModes.Low:
				this.texText.text = "低";
				break;
			case TextureModes.VeryLow:
				this.texText.text = "极低";
				break;
			}
			PlayerPrefs.SetInt("TexMode", (int)this.texMode);
			Action<DebugView> onDebugViewConfigChanged = DebugView.OnDebugViewConfigChanged;
			if (onDebugViewConfigChanged == null)
			{
				return;
			}
			onDebugViewConfigChanged(this);
		}

		// Token: 0x060010B6 RID: 4278 RVA: 0x00040CE4 File Offset: 0x0003EEE4
		private void OnlevelInited()
		{
			this.SetInvincible(this.invincible);
		}

		// Token: 0x060010B7 RID: 4279 RVA: 0x00040CF4 File Offset: 0x0003EEF4
		private void OnSceneLoaded(Scene s1, Scene s2)
		{
			this.SetShadow().Forget();
		}

		// Token: 0x060010B8 RID: 4280 RVA: 0x00040D10 File Offset: 0x0003EF10
		private async UniTaskVoid SetShadow()
		{
			await UniTask.WaitForEndOfFrame(this);
			await UniTask.WaitForEndOfFrame(this);
			await UniTask.WaitForSeconds(0.2f, true, PlayerLoopTiming.Update, default(CancellationToken), false);
			this.light = RenderSettings.sun;
			if (this.light)
			{
				this.light.shadows = (this.edgeLightActive ? LightShadows.Soft : LightShadows.None);
			}
		}

		// Token: 0x060010B9 RID: 4281 RVA: 0x00040D53 File Offset: 0x0003EF53
		public void ToggleBloom()
		{
			this.bloomActive = !this.bloomActive;
			this.SetBloom(this.bloomActive);
		}

		// Token: 0x060010BA RID: 4282 RVA: 0x00040D70 File Offset: 0x0003EF70
		private void SetBloom(bool active)
		{
			Bloom bloom;
			bool flag = this.volumeProfile.TryGet<Bloom>(out bloom);
			this.bloomText.text = (active ? "开" : "关");
			if (flag)
			{
				bloom.active = active;
			}
			this.bloomActive = active;
			PlayerPrefs.SetInt("BloomActive", this.bloomActive ? 1 : 0);
			Action<DebugView> onDebugViewConfigChanged = DebugView.OnDebugViewConfigChanged;
			if (onDebugViewConfigChanged == null)
			{
				return;
			}
			onDebugViewConfigChanged(this);
		}

		// Token: 0x060010BB RID: 4283 RVA: 0x00040DDA File Offset: 0x0003EFDA
		public void ToggleEdgeLight()
		{
			this.edgeLightActive = !this.edgeLightActive;
			this.SetEdgeLight(this.edgeLightActive);
		}

		// Token: 0x060010BC RID: 4284 RVA: 0x00040DF8 File Offset: 0x0003EFF8
		private void SetEdgeLight(bool active)
		{
			this.edgeLightText.text = (active ? "开" : "关");
			this.edgeLightActive = active;
			PlayerPrefs.SetInt("EdgeLightActive", this.edgeLightActive ? 1 : 0);
			UniversalRenderPipelineAsset universalRenderPipelineAsset = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
			if (universalRenderPipelineAsset != null)
			{
				universalRenderPipelineAsset.supportsCameraDepthTexture = active;
			}
			this.SetShadow();
			Action<DebugView> onDebugViewConfigChanged = DebugView.OnDebugViewConfigChanged;
			if (onDebugViewConfigChanged == null)
			{
				return;
			}
			onDebugViewConfigChanged(this);
		}

		// Token: 0x060010BD RID: 4285 RVA: 0x00040E74 File Offset: 0x0003F074
		public void ToggleAO()
		{
			this.aoActive = !this.aoActive;
			this.SetAO(this.aoActive);
		}

		// Token: 0x060010BE RID: 4286 RVA: 0x00040E91 File Offset: 0x0003F091
		public void ToggleDof()
		{
			this.dofActive = !this.dofActive;
			this.SetDof(this.dofActive);
		}

		// Token: 0x060010BF RID: 4287 RVA: 0x00040EAE File Offset: 0x0003F0AE
		public void ToggleInvincible()
		{
			this.invincible = !this.invincible;
			this.SetInvincible(this.invincible);
		}

		// Token: 0x060010C0 RID: 4288 RVA: 0x00040ECB File Offset: 0x0003F0CB
		private void SetReporter(bool active)
		{
		}

		// Token: 0x060010C1 RID: 4289 RVA: 0x00040ECD File Offset: 0x0003F0CD
		public void ToggleReporter()
		{
			this.SetReporter(!this.reporterActive);
		}

		// Token: 0x060010C2 RID: 4290 RVA: 0x00040EE0 File Offset: 0x0003F0E0
		private void SetAO(bool active)
		{
			ScriptableRendererFeature scriptableRendererFeature = this.rendererData.rendererFeatures.Find((ScriptableRendererFeature a) => a.name == "ScreenSpaceAmbientOcclusion");
			if (scriptableRendererFeature != null)
			{
				scriptableRendererFeature.SetActive(active);
				this.aoText.text = (active ? "开" : "关");
				PlayerPrefs.SetInt("AOActive", active ? 1 : 0);
			}
			Action<DebugView> onDebugViewConfigChanged = DebugView.OnDebugViewConfigChanged;
			if (onDebugViewConfigChanged == null)
			{
				return;
			}
			onDebugViewConfigChanged(this);
		}

		// Token: 0x060010C3 RID: 4291 RVA: 0x00040F68 File Offset: 0x0003F168
		private void SetDof(bool active)
		{
		}

		// Token: 0x060010C4 RID: 4292 RVA: 0x00040F6A File Offset: 0x0003F16A
		private void SetInvincible(bool active)
		{
			this.invincibleText.text = (active ? "开" : "关");
			this.invincible = active;
			Action<DebugView> onDebugViewConfigChanged = DebugView.OnDebugViewConfigChanged;
			if (onDebugViewConfigChanged == null)
			{
				return;
			}
			onDebugViewConfigChanged(this);
		}

		// Token: 0x060010C5 RID: 4293 RVA: 0x00040FA0 File Offset: 0x0003F1A0
		public void CreateItem()
		{
			this.CreateItemTask().Forget();
		}

		// Token: 0x060010C6 RID: 4294 RVA: 0x00040FBC File Offset: 0x0003F1BC
		private async UniTaskVoid CreateItemTask()
		{
			if (CharacterMainControl.Main != null)
			{
				Item item = await ItemAssetsCollection.InstantiateAsync(this.createItemID);
				if (!(item == null))
				{
					item.Drop(CharacterMainControl.Main, true);
				}
			}
		}

		// Token: 0x04000D1D RID: 3357
		private DebugView instance;

		// Token: 0x04000D1E RID: 3358
		private Vector2 screenRes;

		// Token: 0x04000D1F RID: 3359
		private ResModes resMode;

		// Token: 0x04000D20 RID: 3360
		private TextureModes texMode;

		// Token: 0x04000D21 RID: 3361
		public TextMeshProUGUI resText;

		// Token: 0x04000D22 RID: 3362
		public TextMeshProUGUI texText;

		// Token: 0x04000D23 RID: 3363
		public TextMeshProUGUI fpsText1;

		// Token: 0x04000D24 RID: 3364
		public TextMeshProUGUI fpsText2;

		// Token: 0x04000D25 RID: 3365
		public TextMeshProUGUI inputDeviceText;

		// Token: 0x04000D26 RID: 3366
		public TextMeshProUGUI bloomText;

		// Token: 0x04000D27 RID: 3367
		public TextMeshProUGUI edgeLightText;

		// Token: 0x04000D28 RID: 3368
		public TextMeshProUGUI aoText;

		// Token: 0x04000D29 RID: 3369
		public TextMeshProUGUI dofText;

		// Token: 0x04000D2A RID: 3370
		public TextMeshProUGUI invincibleText;

		// Token: 0x04000D2B RID: 3371
		public TextMeshProUGUI reporterText;

		// Token: 0x04000D2C RID: 3372
		public UniversalRendererData rendererData;

		// Token: 0x04000D2D RID: 3373
		private float[] deltaTimes;

		// Token: 0x04000D2E RID: 3374
		private int frameIndex;

		// Token: 0x04000D2F RID: 3375
		public int frameSampleCount = 30;

		// Token: 0x04000D30 RID: 3376
		public GameObject openButton;

		// Token: 0x04000D31 RID: 3377
		public GameObject panel;

		// Token: 0x04000D32 RID: 3378
		public VolumeProfile volumeProfile;

		// Token: 0x04000D33 RID: 3379
		private bool bloomActive;

		// Token: 0x04000D34 RID: 3380
		private bool edgeLightActive;

		// Token: 0x04000D35 RID: 3381
		private bool aoActive;

		// Token: 0x04000D36 RID: 3382
		private int inputDevice;

		// Token: 0x04000D37 RID: 3383
		private bool dofActive;

		// Token: 0x04000D38 RID: 3384
		private bool invincible;

		// Token: 0x04000D39 RID: 3385
		private bool reporterActive;

		// Token: 0x04000D3A RID: 3386
		private Light light;

		// Token: 0x04000D3B RID: 3387
		[ItemTypeID]
		public int createItemID;
	}
}
