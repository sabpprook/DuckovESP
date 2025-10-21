using System;
using System.Collections.Generic;
using System.Linq;
using Duckov.Options;
using Sirenix.Utilities;
using SodaCraft.Localizations;
using UnityEngine;

// Token: 0x020001CA RID: 458
public class ResolutionSetter : MonoBehaviour
{
	// Token: 0x06000D8F RID: 3471 RVA: 0x000379C8 File Offset: 0x00035BC8
	private void Test()
	{
		this.debugDisplayRes = new Vector2Int(Display.main.systemWidth, Display.main.systemHeight);
		this.debugmMaxRes = new Vector2Int(ResolutionSetter.MaxResolution.width, ResolutionSetter.MaxResolution.height);
		this.debugScreenRes = new Vector2Int(Screen.currentResolution.width, Screen.currentResolution.height);
		this.testRes = ResolutionSetter.GetResolutions();
	}

	// Token: 0x17000283 RID: 643
	// (get) Token: 0x06000D90 RID: 3472 RVA: 0x00037A44 File Offset: 0x00035C44
	public static DuckovResolution MaxResolution
	{
		get
		{
			Resolution[] resolutions = Screen.resolutions;
			resolutions.Sort(delegate(Resolution A, Resolution B)
			{
				if (A.height > B.height)
				{
					return -1;
				}
				if (A.height < B.height)
				{
					return 1;
				}
				if (A.width > B.width)
				{
					return -1;
				}
				if (A.width < B.width)
				{
					return 1;
				}
				return 0;
			});
			Resolution resolution = default(Resolution);
			resolution.width = Screen.currentResolution.width;
			resolution.height = Screen.currentResolution.height;
			Resolution resolution2 = Screen.resolutions[resolutions.Length - 1];
			DuckovResolution duckovResolution;
			if (resolution.width > resolution2.width)
			{
				duckovResolution = new DuckovResolution(resolution);
			}
			else
			{
				duckovResolution = new DuckovResolution(resolution2);
			}
			if ((float)duckovResolution.width / (float)duckovResolution.height < 1.4f)
			{
				duckovResolution.width = Mathf.RoundToInt((float)(duckovResolution.height * 16 / 9));
			}
			return duckovResolution;
		}
	}

	// Token: 0x06000D91 RID: 3473 RVA: 0x00037B10 File Offset: 0x00035D10
	public static Resolution GetResByHeight(int height, DuckovResolution maxRes)
	{
		return new Resolution
		{
			height = height,
			width = (int)((float)maxRes.width * (float)height / (float)maxRes.height)
		};
	}

	// Token: 0x06000D92 RID: 3474 RVA: 0x00037B48 File Offset: 0x00035D48
	public static DuckovResolution[] GetResolutions()
	{
		DuckovResolution maxResolution = ResolutionSetter.MaxResolution;
		List<Resolution> list = Screen.resolutions.ToList<Resolution>();
		list.Add(ResolutionSetter.GetResByHeight(1080, maxResolution));
		list.Add(ResolutionSetter.GetResByHeight(900, maxResolution));
		list.Add(ResolutionSetter.GetResByHeight(720, maxResolution));
		list.Add(ResolutionSetter.GetResByHeight(540, maxResolution));
		List<DuckovResolution> list2 = new List<DuckovResolution>();
		bool flag = OptionsManager.Load<ResolutionSetter.screenModes>(ResolutionSetter.Key_ScreenMode, ResolutionSetter.screenModes.Window) != ResolutionSetter.screenModes.Window;
		foreach (Resolution resolution in list)
		{
			DuckovResolution duckovResolution = new DuckovResolution(resolution);
			if (!list2.Contains(duckovResolution) && (float)duckovResolution.width / (float)duckovResolution.height >= 1.4f && (!flag || duckovResolution.CheckRotioFit(duckovResolution, maxResolution)))
			{
				list2.Add(duckovResolution);
			}
		}
		list2.Sort(delegate(DuckovResolution A, DuckovResolution B)
		{
			if (A.height > B.height)
			{
				return -1;
			}
			if (A.height < B.height)
			{
				return 1;
			}
			if (A.width > B.width)
			{
				return -1;
			}
			if (A.width < B.width)
			{
				return 1;
			}
			return 0;
		});
		return list2.ToArray();
	}

	// Token: 0x06000D93 RID: 3475 RVA: 0x00037C68 File Offset: 0x00035E68
	private void Update()
	{
		this.UpdateFullScreenCheck();
	}

	// Token: 0x06000D94 RID: 3476 RVA: 0x00037C70 File Offset: 0x00035E70
	private void UpdateFullScreenCheck()
	{
		ResolutionSetter.fullScreenChangeCheckCoolTimer -= Time.unscaledDeltaTime;
		if (ResolutionSetter.fullScreenChangeCheckCoolTimer > 0f)
		{
			return;
		}
		if (ResolutionSetter.currentFullScreen != (Screen.fullScreenMode == FullScreenMode.FullScreenWindow || Screen.fullScreenMode == FullScreenMode.ExclusiveFullScreen))
		{
			ResolutionSetter.currentFullScreen = !ResolutionSetter.currentFullScreen;
			OptionsManager.Save<ResolutionSetter.screenModes>(ResolutionSetter.Key_ScreenMode, ResolutionSetter.currentFullScreen ? ResolutionSetter.screenModes.Borderless : ResolutionSetter.screenModes.Window);
			ResolutionSetter.fullScreenChangeCheckCoolTimer = ResolutionSetter.fullScreenChangeCheckCoolTime;
		}
	}

	// Token: 0x06000D95 RID: 3477 RVA: 0x00037CE0 File Offset: 0x00035EE0
	public static void UpdateResolutionAndScreenMode()
	{
		ResolutionSetter.fullScreenChangeCheckCoolTimer = ResolutionSetter.fullScreenChangeCheckCoolTime;
		DuckovResolution duckovResolution = OptionsManager.Load<DuckovResolution>(ResolutionSetter.Key_Resolution, new DuckovResolution(Screen.resolutions[Screen.resolutions.Length - 1]));
		if ((float)duckovResolution.width / (float)duckovResolution.height < 1.3666667f)
		{
			duckovResolution.width = Mathf.RoundToInt((float)(duckovResolution.height * 16 / 9));
		}
		ResolutionSetter.screenModes screenModes = OptionsManager.Load<ResolutionSetter.screenModes>(ResolutionSetter.Key_ScreenMode, ResolutionSetter.screenModes.Borderless);
		ResolutionSetter.currentFullScreen = screenModes == ResolutionSetter.screenModes.Borderless;
		Screen.SetResolution(duckovResolution.width, duckovResolution.height, ResolutionSetter.ScreenModeToFullScreenMode(screenModes));
	}

	// Token: 0x06000D96 RID: 3478 RVA: 0x00037D75 File Offset: 0x00035F75
	private static FullScreenMode ScreenModeToFullScreenMode(ResolutionSetter.screenModes screenMode)
	{
		if (screenMode == ResolutionSetter.screenModes.Borderless)
		{
			return FullScreenMode.FullScreenWindow;
		}
		if (screenMode != ResolutionSetter.screenModes.Window)
		{
			return FullScreenMode.ExclusiveFullScreen;
		}
		return FullScreenMode.Windowed;
	}

	// Token: 0x06000D97 RID: 3479 RVA: 0x00037D88 File Offset: 0x00035F88
	public static string[] GetScreenModes()
	{
		return new string[]
		{
			("Option_ScreenMode_" + ResolutionSetter.screenModes.Borderless.ToString()).ToPlainText(),
			("Option_ScreenMode_" + ResolutionSetter.screenModes.Window.ToString()).ToPlainText()
		};
	}

	// Token: 0x06000D98 RID: 3480 RVA: 0x00037DDD File Offset: 0x00035FDD
	public static string ScreenModeToName(ResolutionSetter.screenModes mode)
	{
		return ("Option_ScreenMode_" + mode.ToString()).ToPlainText();
	}

	// Token: 0x06000D99 RID: 3481 RVA: 0x00037DFB File Offset: 0x00035FFB
	private void Awake()
	{
		ResolutionSetter.UpdateResolutionAndScreenMode();
		OptionsManager.OnOptionsChanged += this.OnOptionsChanged;
	}

	// Token: 0x06000D9A RID: 3482 RVA: 0x00037E13 File Offset: 0x00036013
	private void OnDestroy()
	{
		OptionsManager.OnOptionsChanged -= this.OnOptionsChanged;
	}

	// Token: 0x06000D9B RID: 3483 RVA: 0x00037E26 File Offset: 0x00036026
	private void OnOptionsChanged(string key)
	{
		if (key == ResolutionSetter.Key_Resolution || key == ResolutionSetter.Key_ScreenMode)
		{
			ResolutionSetter.UpdateResolutionAndScreenMode();
		}
	}

	// Token: 0x04000B80 RID: 2944
	public static string Key_Resolution = "Resolution";

	// Token: 0x04000B81 RID: 2945
	public static string Key_ScreenMode = "ScreenMode";

	// Token: 0x04000B82 RID: 2946
	public static bool currentFullScreen = false;

	// Token: 0x04000B83 RID: 2947
	private static float fullScreenChangeCheckCoolTimer = 1f;

	// Token: 0x04000B84 RID: 2948
	private static float fullScreenChangeCheckCoolTime = 1f;

	// Token: 0x04000B85 RID: 2949
	public Vector2Int debugDisplayRes = new Vector2Int(0, 0);

	// Token: 0x04000B86 RID: 2950
	public Vector2Int debugScreenRes = new Vector2Int(0, 0);

	// Token: 0x04000B87 RID: 2951
	public Vector2Int debugmMaxRes = new Vector2Int(0, 0);

	// Token: 0x04000B88 RID: 2952
	public DuckovResolution[] testRes;

	// Token: 0x020004D3 RID: 1235
	public enum screenModes
	{
		// Token: 0x04001CE8 RID: 7400
		Borderless,
		// Token: 0x04001CE9 RID: 7401
		Window
	}
}
