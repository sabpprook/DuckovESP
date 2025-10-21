using System;
using Duckov.UI;
using UnityEngine;

// Token: 0x020001A0 RID: 416
public class CameraMode : MonoBehaviour
{
	// Token: 0x17000238 RID: 568
	// (get) Token: 0x06000C46 RID: 3142 RVA: 0x00033BBC File Offset: 0x00031DBC
	// (set) Token: 0x06000C47 RID: 3143 RVA: 0x00033BC3 File Offset: 0x00031DC3
	public static CameraMode Instance { get; private set; }

	// Token: 0x17000239 RID: 569
	// (get) Token: 0x06000C48 RID: 3144 RVA: 0x00033BCB File Offset: 0x00031DCB
	public static bool Active
	{
		get
		{
			return !(CameraMode.Instance == null) && CameraMode.Instance.active;
		}
	}

	// Token: 0x06000C49 RID: 3145 RVA: 0x00033BE8 File Offset: 0x00031DE8
	private void Awake()
	{
		if (CameraMode.Instance != null)
		{
			Debug.LogError("检测到多个Camera Mode", base.gameObject);
			return;
		}
		Shader.SetGlobalFloat("CameraModeOn", 0f);
		CameraMode.Instance = this;
		UIInputManager.OnToggleCameraMode += this.OnToggleCameraMode;
		UIInputManager.OnCancel += this.OnUICancel;
		ManagedUIElement.onOpen += this.OnViewOpen;
	}

	// Token: 0x06000C4A RID: 3146 RVA: 0x00033C5C File Offset: 0x00031E5C
	private void OnDestroy()
	{
		Shader.SetGlobalFloat("CameraModeOn", 0f);
		UIInputManager.OnToggleCameraMode -= this.OnToggleCameraMode;
		UIInputManager.OnCancel -= this.OnUICancel;
		ManagedUIElement.onOpen -= this.OnViewOpen;
		Shader.SetGlobalFloat("CameraModeOn", 0f);
	}

	// Token: 0x06000C4B RID: 3147 RVA: 0x00033CBA File Offset: 0x00031EBA
	private void OnViewOpen(ManagedUIElement element)
	{
		if (CameraMode.Active)
		{
			CameraMode.Deactivate();
		}
	}

	// Token: 0x06000C4C RID: 3148 RVA: 0x00033CC8 File Offset: 0x00031EC8
	private void OnUICancel(UIInputEventData data)
	{
		if (data.Used)
		{
			return;
		}
		if (CameraMode.Active)
		{
			CameraMode.Deactivate();
			data.Use();
		}
	}

	// Token: 0x06000C4D RID: 3149 RVA: 0x00033CE5 File Offset: 0x00031EE5
	private void OnToggleCameraMode(UIInputEventData data)
	{
		if (CameraMode.Active)
		{
			CameraMode.Deactivate();
		}
		else
		{
			CameraMode.Activate();
		}
		data.Use();
	}

	// Token: 0x06000C4E RID: 3150 RVA: 0x00033D00 File Offset: 0x00031F00
	private void MActivate()
	{
		if (View.ActiveView != null)
		{
			return;
		}
		this.active = true;
		Shader.SetGlobalFloat("CameraModeOn", 1f);
		Action onCameraModeActivated = CameraMode.OnCameraModeActivated;
		if (onCameraModeActivated != null)
		{
			onCameraModeActivated();
		}
		Action<bool> onCameraModeChanged = CameraMode.OnCameraModeChanged;
		if (onCameraModeChanged == null)
		{
			return;
		}
		onCameraModeChanged(this.active);
	}

	// Token: 0x06000C4F RID: 3151 RVA: 0x00033D56 File Offset: 0x00031F56
	private void MDeactivate()
	{
		this.active = false;
		Shader.SetGlobalFloat("CameraModeOn", 0f);
		Action onCameraModeDeactivated = CameraMode.OnCameraModeDeactivated;
		if (onCameraModeDeactivated != null)
		{
			onCameraModeDeactivated();
		}
		Action<bool> onCameraModeChanged = CameraMode.OnCameraModeChanged;
		if (onCameraModeChanged == null)
		{
			return;
		}
		onCameraModeChanged(this.active);
	}

	// Token: 0x06000C50 RID: 3152 RVA: 0x00033D93 File Offset: 0x00031F93
	public static void Activate()
	{
		if (CameraMode.Instance == null)
		{
			return;
		}
		Shader.SetGlobalFloat("CameraModeOn", 1f);
		CameraMode.Instance.MActivate();
	}

	// Token: 0x06000C51 RID: 3153 RVA: 0x00033DBC File Offset: 0x00031FBC
	public static void Deactivate()
	{
		Shader.SetGlobalFloat("CameraModeOn", 0f);
		if (CameraMode.Instance == null)
		{
			return;
		}
		CameraMode.Instance.MDeactivate();
	}

	// Token: 0x04000AA0 RID: 2720
	public static Action OnCameraModeActivated;

	// Token: 0x04000AA1 RID: 2721
	public static Action OnCameraModeDeactivated;

	// Token: 0x04000AA2 RID: 2722
	public static Action<bool> OnCameraModeChanged;

	// Token: 0x04000AA3 RID: 2723
	private bool active;
}
