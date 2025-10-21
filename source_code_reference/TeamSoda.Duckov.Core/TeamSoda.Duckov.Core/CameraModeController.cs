using System;
using System.IO;
using System.Threading;
using Cinemachine;
using Cysharp.Threading.Tasks;
using Duckov;
using Duckov.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

// Token: 0x020001A1 RID: 417
public class CameraModeController : MonoBehaviour
{
	// Token: 0x1700023A RID: 570
	// (get) Token: 0x06000C53 RID: 3155 RVA: 0x00033DED File Offset: 0x00031FED
	private static string filePath
	{
		get
		{
			if (GameMetaData.Instance.Platform == Platform.WeGame)
			{
				return Application.streamingAssetsPath + "/ScreenShots";
			}
			return Application.persistentDataPath + "/ScreenShots";
		}
	}

	// Token: 0x06000C54 RID: 3156 RVA: 0x00033E1C File Offset: 0x0003201C
	private void UpdateInput()
	{
		this.moveInput = this.inputActionAsset["CameraModeMove"].ReadValue<Vector2>();
		this.focusInput = this.inputActionAsset["CameraModeFocus"].IsPressed();
		this.upDownInput = this.inputActionAsset["CameraModeUpDown"].ReadValue<float>();
		this.fovInput = this.inputActionAsset["CameraModeFOV"].ReadValue<float>();
		this.aimInput = this.inputActionAsset["CameraModeAim"].ReadValue<Vector2>();
		this.captureInput = this.inputActionAsset["CameraModeCapture"].WasPressedThisFrame();
		this.fastInput = this.inputActionAsset["CameraModeFaster"].IsPressed();
		this.openFolderInput = this.inputActionAsset["CameraModeOpenFolder"].WasPressedThisFrame();
	}

	// Token: 0x06000C55 RID: 3157 RVA: 0x00033F04 File Offset: 0x00032104
	private void Awake()
	{
		CameraMode.OnCameraModeActivated = (Action)Delegate.Combine(CameraMode.OnCameraModeActivated, new Action(this.OnCameraModeActivated));
		CameraMode.OnCameraModeDeactivated = (Action)Delegate.Combine(CameraMode.OnCameraModeDeactivated, new Action(this.OnCameraModeDeactivated));
		this.inputActionAsset.Enable();
		this.vCam.gameObject.SetActive(true);
		base.gameObject.SetActive(false);
	}

	// Token: 0x06000C56 RID: 3158 RVA: 0x00033F7C File Offset: 0x0003217C
	private void Update()
	{
		if (!this.actived)
		{
			return;
		}
		this.UpdateInput();
		if (this.shootting)
		{
			return;
		}
		this.UpdateMove();
		this.UpdateLook();
		this.UpdateFov();
		if (this.captureInput)
		{
			this.Shot().Forget();
		}
		if (this.openFolderInput)
		{
			CameraModeController.OpenFolder();
			this.openFolderInput = false;
		}
	}

	// Token: 0x06000C57 RID: 3159 RVA: 0x00033FDD File Offset: 0x000321DD
	private void LateUpdate()
	{
		this.UpdateFocus();
	}

	// Token: 0x06000C58 RID: 3160 RVA: 0x00033FE8 File Offset: 0x000321E8
	private void UpdateMove()
	{
		Vector3 forward = this.vCam.transform.forward;
		forward.y = 0f;
		forward.Normalize();
		Vector3 right = this.vCam.transform.right;
		right.y = 0f;
		right.Normalize();
		Vector3 vector = right * this.moveInput.x + forward * this.moveInput.y;
		vector.Normalize();
		vector += this.upDownInput * Vector3.up;
		this.vCam.transform.position += Time.unscaledDeltaTime * vector * (this.fastInput ? this.fastMoveSpeed : this.moveSpeed);
	}

	// Token: 0x06000C59 RID: 3161 RVA: 0x000340C4 File Offset: 0x000322C4
	private void UpdateLook()
	{
		this.pitch += -this.aimInput.y * this.aimSpeed * Time.unscaledDeltaTime;
		this.pitch = Mathf.Clamp(this.pitch, -89.9f, 89.9f);
		this.yaw += this.aimInput.x * this.aimSpeed * Time.unscaledDeltaTime;
		this.vCam.transform.localRotation = Quaternion.Euler(this.pitch, this.yaw, 0f);
	}

	// Token: 0x06000C5A RID: 3162 RVA: 0x00034160 File Offset: 0x00032360
	private void UpdateFocus()
	{
		if (this.focusInput)
		{
			RaycastHit raycastHit;
			if (Physics.Raycast(this.vCam.transform.position, this.vCam.transform.forward, out raycastHit, 100f, this.dofLayerMask))
			{
				this.dofTargetPoint = raycastHit.point + this.vCam.transform.forward * -0.2f;
				this.dofTarget.position = this.dofTargetPoint;
			}
			this.focusMeshTimer = this.focusMeshAppearTime;
			if (!this.focusMesh.gameObject.activeSelf)
			{
				this.focusMesh.gameObject.SetActive(true);
			}
		}
		else if (this.focusMeshTimer > 0f)
		{
			this.focusMeshTimer -= Time.unscaledDeltaTime;
			if (this.focusMeshTimer <= 0f)
			{
				this.focusMeshTimer = 0f;
				this.focusMesh.gameObject.SetActive(false);
			}
		}
		if (this.focusMesh.gameObject.activeSelf)
		{
			this.focusMesh.transform.localScale = Vector3.one * this.focusMeshSize * this.focusMeshTimer / this.focusMeshAppearTime;
		}
	}

	// Token: 0x06000C5B RID: 3163 RVA: 0x000342B0 File Offset: 0x000324B0
	private void UpdateFov()
	{
		float num = this.vCam.m_Lens.FieldOfView;
		num += -this.fovChangeSpeed * this.fovInput;
		num = Mathf.Clamp(num, this.fovRange.x, this.fovRange.y);
		this.vCam.m_Lens.FieldOfView = num;
	}

	// Token: 0x06000C5C RID: 3164 RVA: 0x00034310 File Offset: 0x00032510
	private void OnDestroy()
	{
		CameraMode.OnCameraModeActivated = (Action)Delegate.Remove(CameraMode.OnCameraModeActivated, new Action(this.OnCameraModeActivated));
		CameraMode.OnCameraModeDeactivated = (Action)Delegate.Remove(CameraMode.OnCameraModeDeactivated, new Action(this.OnCameraModeDeactivated));
	}

	// Token: 0x06000C5D RID: 3165 RVA: 0x00034360 File Offset: 0x00032560
	private void OnCameraModeActivated()
	{
		GameCamera instance = GameCamera.Instance;
		if (instance != null)
		{
			CameraArm mianCameraArm = instance.mianCameraArm;
			this.yaw = mianCameraArm.yaw;
			this.pitch = mianCameraArm.pitch;
			this.vCam.transform.position = instance.renderCamera.transform.position;
			this.dofTargetPoint = instance.target.transform.position;
			this.actived = true;
			this.vCam.m_Lens.FieldOfView = instance.renderCamera.fieldOfView;
			base.gameObject.SetActive(true);
		}
	}

	// Token: 0x06000C5E RID: 3166 RVA: 0x00034402 File Offset: 0x00032602
	public static void OpenFolder()
	{
		GUIUtility.systemCopyBuffer = CameraModeController.filePath;
		NotificationText.Push(CameraModeController.filePath ?? "");
	}

	// Token: 0x06000C5F RID: 3167 RVA: 0x00034421 File Offset: 0x00032621
	private void OnCameraModeDeactivated()
	{
		this.actived = false;
		base.gameObject.SetActive(false);
	}

	// Token: 0x06000C60 RID: 3168 RVA: 0x00034438 File Offset: 0x00032638
	private async UniTaskVoid Shot()
	{
		if (!this.shootting)
		{
			this.indicatorGroup.alpha = 0f;
			await UniTask.WaitForEndOfFrame(this);
			this.shootting = true;
			int num = 0;
			int height = Screen.currentResolution.height;
			if (PlayerPrefs.HasKey("ScreenShotIndex"))
			{
				num = PlayerPrefs.GetInt("ScreenShotIndex");
			}
			if (!Directory.Exists(CameraModeController.filePath))
			{
				Directory.CreateDirectory(CameraModeController.filePath);
			}
			ScreenCapture.CaptureScreenshot(string.Format("{0}/ScreenShot_{1:0000}.png", CameraModeController.filePath, num), 2);
			num++;
			PlayerPrefs.SetInt("ScreenShotIndex", num);
			await UniTask.WaitForEndOfFrame(this);
			await UniTask.WaitForEndOfFrame(this);
			await UniTask.WaitForEndOfFrame(this);
			this.indicatorGroup.alpha = 1f;
			this.colorPunch.Punch();
			UnityEvent onCapturedEvent = this.OnCapturedEvent;
			if (onCapturedEvent != null)
			{
				onCapturedEvent.Invoke();
			}
			await UniTask.WaitForSeconds(0.3f, true, PlayerLoopTiming.Update, default(CancellationToken), false);
			this.shootting = false;
		}
	}

	// Token: 0x04000AA4 RID: 2724
	public CinemachineVirtualCamera vCam;

	// Token: 0x04000AA5 RID: 2725
	private bool actived;

	// Token: 0x04000AA6 RID: 2726
	public Transform dofTarget;

	// Token: 0x04000AA7 RID: 2727
	private Vector3 dofTargetPoint;

	// Token: 0x04000AA8 RID: 2728
	public InputActionAsset inputActionAsset;

	// Token: 0x04000AA9 RID: 2729
	public LayerMask dofLayerMask;

	// Token: 0x04000AAA RID: 2730
	private Vector2 moveInput;

	// Token: 0x04000AAB RID: 2731
	private float upDownInput;

	// Token: 0x04000AAC RID: 2732
	private bool focusInput;

	// Token: 0x04000AAD RID: 2733
	private bool captureInput;

	// Token: 0x04000AAE RID: 2734
	private bool fastInput;

	// Token: 0x04000AAF RID: 2735
	private bool openFolderInput;

	// Token: 0x04000AB0 RID: 2736
	public GameObject focusMesh;

	// Token: 0x04000AB1 RID: 2737
	public float focusMeshSize = 0.3f;

	// Token: 0x04000AB2 RID: 2738
	private float focusMeshCurrentSize = 0.3f;

	// Token: 0x04000AB3 RID: 2739
	public float focusMeshAppearTime = 1f;

	// Token: 0x04000AB4 RID: 2740
	private float focusMeshTimer = 0.3f;

	// Token: 0x04000AB5 RID: 2741
	private float fovInput;

	// Token: 0x04000AB6 RID: 2742
	private Vector2 aimInput;

	// Token: 0x04000AB7 RID: 2743
	public float moveSpeed;

	// Token: 0x04000AB8 RID: 2744
	public float fastMoveSpeed;

	// Token: 0x04000AB9 RID: 2745
	public float aimSpeed;

	// Token: 0x04000ABA RID: 2746
	private float yaw;

	// Token: 0x04000ABB RID: 2747
	private float pitch;

	// Token: 0x04000ABC RID: 2748
	private bool shootting;

	// Token: 0x04000ABD RID: 2749
	public ColorPunch colorPunch;

	// Token: 0x04000ABE RID: 2750
	public Vector2 fovRange = new Vector2(5f, 60f);

	// Token: 0x04000ABF RID: 2751
	[Range(0.01f, 0.5f)]
	public float fovChangeSpeed = 10f;

	// Token: 0x04000AC0 RID: 2752
	public CanvasGroup indicatorGroup;

	// Token: 0x04000AC1 RID: 2753
	public UnityEvent OnCapturedEvent;
}
