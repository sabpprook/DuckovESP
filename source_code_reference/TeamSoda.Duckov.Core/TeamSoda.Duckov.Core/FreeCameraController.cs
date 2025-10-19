using System;
using CameraSystems;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

// Token: 0x020001B5 RID: 437
public class FreeCameraController : MonoBehaviour
{
	// Token: 0x17000258 RID: 600
	// (get) Token: 0x06000CE8 RID: 3304 RVA: 0x00035BC4 File Offset: 0x00033DC4
	private Gamepad Gamepad
	{
		get
		{
			return Gamepad.current;
		}
	}

	// Token: 0x06000CE9 RID: 3305 RVA: 0x00035BCB File Offset: 0x00033DCB
	private void Awake()
	{
		if (!this.propertiesControl)
		{
			this.propertiesControl = base.GetComponent<CameraPropertiesControl>();
		}
	}

	// Token: 0x06000CEA RID: 3306 RVA: 0x00035BE6 File Offset: 0x00033DE6
	private void OnEnable()
	{
		this.SetRotation(base.transform.rotation);
		this.SnapToMainCamera();
	}

	// Token: 0x06000CEB RID: 3307 RVA: 0x00035C00 File Offset: 0x00033E00
	public void SetRotation(Quaternion rotation)
	{
		Vector3 eulerAngles = rotation.eulerAngles;
		this.yaw = eulerAngles.y;
		this.pitch = eulerAngles.x;
		this.yawTarget = this.yaw;
		this.pitchTarget = this.pitch;
		if (this.pitch > 180f)
		{
			this.pitch -= 360f;
		}
		if (this.pitch < -180f)
		{
			this.pitch += 360f;
		}
		this.pitch = Mathf.Clamp(this.pitch, -89f, 89f);
		base.transform.rotation = Quaternion.Euler(this.pitch, this.yaw, 0f);
	}

	// Token: 0x06000CEC RID: 3308 RVA: 0x00035CC0 File Offset: 0x00033EC0
	private unsafe void Update()
	{
		if (this.Gamepad == null)
		{
			return;
		}
		bool isPressed = this.Gamepad.rightShoulder.isPressed;
		float num = this.moveSpeed * (float)(isPressed ? 2 : 1);
		CharacterMainControl main = CharacterMainControl.Main;
		Vector2 vector = *this.Gamepad.leftStick.value;
		float num2 = *this.Gamepad.rightTrigger.value - *this.Gamepad.leftTrigger.value;
		Vector3 vector2 = new Vector3(vector.x * num, 0f, vector.y * num) * Time.unscaledDeltaTime;
		Vector3 vector3 = (this.projectMovementOnXZPlane ? Vector3.ProjectOnPlane(base.transform.forward, Vector3.up).normalized : base.transform.forward);
		Vector3 vector4 = (this.projectMovementOnXZPlane ? Vector3.ProjectOnPlane(base.transform.right, Vector3.up).normalized : base.transform.right);
		Vector3 vector5 = num2 * Vector3.up * num * 0.5f * Time.unscaledDeltaTime;
		Vector3 vector6 = vector3 * vector2.z + vector4 * vector2.x + vector5;
		if (!this.followCharacter || main == null)
		{
			this.worldPosTarget += vector6;
			base.transform.position = Vector3.SmoothDamp(base.transform.position, this.worldPosTarget, ref this.velocityWorldSpace, this.smoothTime, 20f, 10f * Time.unscaledDeltaTime);
			if (main == null)
			{
				this.followCharacter = false;
			}
		}
		else
		{
			this.offsetFromCharacter += vector6;
			base.transform.position = Vector3.SmoothDamp(base.transform.position, main.transform.position + this.offsetFromCharacter, ref this.velocityLocalSpace, this.smoothTime, 20f, 10f * Time.unscaledDeltaTime);
		}
		Vector3 vector7 = *this.Gamepad.rightStick.value * this.rotateSpeed * this.vCamera.m_Lens.FieldOfView / 60f;
		this.yawTarget += vector7.x * Time.unscaledDeltaTime;
		this.yaw = Mathf.SmoothDamp(this.yaw, this.yawTarget, ref this.yawVelocity, this.smoothTime, 20f, 10f * Time.unscaledDeltaTime);
		this.pitchTarget += -vector7.y * Time.unscaledDeltaTime;
		this.pitch = Mathf.SmoothDamp(this.pitch, this.pitchTarget, ref this.pitchVelocity, this.smoothTime, 20f, 10f * Time.unscaledDeltaTime);
		this.pitch = Mathf.Clamp(this.pitch, -89f, 89f);
		base.transform.rotation = Quaternion.Euler(this.pitch, this.yaw, 0f);
		if (this.Gamepad.buttonNorth.wasPressedThisFrame)
		{
			this.SnapToMainCamera();
		}
		if (this.Gamepad.buttonEast.wasPressedThisFrame)
		{
			this.ToggleFollowTarget();
		}
	}

	// Token: 0x06000CED RID: 3309 RVA: 0x00036036 File Offset: 0x00034236
	private void OnDestroy()
	{
	}

	// Token: 0x06000CEE RID: 3310 RVA: 0x00036038 File Offset: 0x00034238
	private void ToggleFollowTarget()
	{
		CharacterMainControl main = CharacterMainControl.Main;
		if (main == null)
		{
			return;
		}
		this.followCharacter = !this.followCharacter;
		if (this.followCharacter)
		{
			this.offsetFromCharacter = base.transform.position - main.transform.position;
		}
		this.worldPosTarget = base.transform.position;
	}

	// Token: 0x06000CEF RID: 3311 RVA: 0x000360A0 File Offset: 0x000342A0
	private void SnapToMainCamera()
	{
		if (GameCamera.Instance == null)
		{
			return;
		}
		Camera renderCamera = GameCamera.Instance.renderCamera;
		if (renderCamera == null)
		{
			return;
		}
		base.transform.position = renderCamera.transform.position;
		this.worldPosTarget = renderCamera.transform.position;
		this.vCamera.m_Lens.FieldOfView = renderCamera.fieldOfView;
		this.SetRotation(renderCamera.transform.rotation);
		CharacterMainControl main = CharacterMainControl.Main;
		if (main != null && this.followCharacter)
		{
			this.offsetFromCharacter = base.transform.position - main.transform.position;
		}
	}

	// Token: 0x04000B24 RID: 2852
	[SerializeField]
	private CameraPropertiesControl propertiesControl;

	// Token: 0x04000B25 RID: 2853
	[SerializeField]
	private float moveSpeed = 10f;

	// Token: 0x04000B26 RID: 2854
	[SerializeField]
	private float rotateSpeed = 180f;

	// Token: 0x04000B27 RID: 2855
	[SerializeField]
	private float smoothTime = 2f;

	// Token: 0x04000B28 RID: 2856
	[SerializeField]
	private Vector2 minMaxXRotation = new Vector2(-89f, 89f);

	// Token: 0x04000B29 RID: 2857
	[SerializeField]
	private bool projectMovementOnXZPlane;

	// Token: 0x04000B2A RID: 2858
	[Range(-180f, 180f)]
	private float yaw;

	// Token: 0x04000B2B RID: 2859
	[Range(-89f, 89f)]
	private float pitch;

	// Token: 0x04000B2C RID: 2860
	[SerializeField]
	private CinemachineVirtualCamera vCamera;

	// Token: 0x04000B2D RID: 2861
	private bool followCharacter;

	// Token: 0x04000B2E RID: 2862
	private Vector3 offsetFromCharacter;

	// Token: 0x04000B2F RID: 2863
	private Vector3 worldPosTarget;

	// Token: 0x04000B30 RID: 2864
	private Vector3 velocityWorldSpace;

	// Token: 0x04000B31 RID: 2865
	private Vector3 velocityLocalSpace;

	// Token: 0x04000B32 RID: 2866
	private float yawVelocity;

	// Token: 0x04000B33 RID: 2867
	private float pitchVelocity;

	// Token: 0x04000B34 RID: 2868
	private float yawTarget;

	// Token: 0x04000B35 RID: 2869
	private float pitchTarget;
}
