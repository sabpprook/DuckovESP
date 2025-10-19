using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UnityTemplateProjects
{
	// Token: 0x02000220 RID: 544
	public class SimpleCameraController : MonoBehaviour
	{
		// Token: 0x06001053 RID: 4179 RVA: 0x0003F2F0 File Offset: 0x0003D4F0
		private void Start()
		{
			InputActionMap inputActionMap = new InputActionMap("Simple Camera Controller");
			this.lookAction = inputActionMap.AddAction("look", InputActionType.Value, "<Mouse>/delta", null, null, null, null);
			this.movementAction = inputActionMap.AddAction("move", InputActionType.Value, "<Gamepad>/leftStick", null, null, null, null);
			this.verticalMovementAction = inputActionMap.AddAction("Vertical Movement", InputActionType.Value, null, null, null, null, null);
			this.boostFactorAction = inputActionMap.AddAction("Boost Factor", InputActionType.Value, "<Mouse>/scroll", null, null, null, null);
			this.lookAction.AddBinding("<Gamepad>/rightStick", null, null, null).WithProcessor("scaleVector2(x=15, y=15)");
			this.movementAction.AddCompositeBinding("Dpad", null, null).With("Up", "<Keyboard>/w", null, null).With("Up", "<Keyboard>/upArrow", null, null)
				.With("Down", "<Keyboard>/s", null, null)
				.With("Down", "<Keyboard>/downArrow", null, null)
				.With("Left", "<Keyboard>/a", null, null)
				.With("Left", "<Keyboard>/leftArrow", null, null)
				.With("Right", "<Keyboard>/d", null, null)
				.With("Right", "<Keyboard>/rightArrow", null, null);
			this.verticalMovementAction.AddCompositeBinding("Dpad", null, null).With("Up", "<Keyboard>/pageUp", null, null).With("Down", "<Keyboard>/pageDown", null, null)
				.With("Up", "<Keyboard>/e", null, null)
				.With("Down", "<Keyboard>/q", null, null)
				.With("Up", "<Gamepad>/rightshoulder", null, null)
				.With("Down", "<Gamepad>/leftshoulder", null, null);
			this.boostFactorAction.AddBinding("<Gamepad>/Dpad", null, null, null).WithProcessor("scaleVector2(x=1, y=4)");
			this.movementAction.Enable();
			this.lookAction.Enable();
			this.verticalMovementAction.Enable();
			this.boostFactorAction.Enable();
		}

		// Token: 0x06001054 RID: 4180 RVA: 0x0003F51C File Offset: 0x0003D71C
		private void OnEnable()
		{
			this.m_TargetCameraState.SetFromTransform(base.transform);
			this.m_InterpolatingCameraState.SetFromTransform(base.transform);
		}

		// Token: 0x06001055 RID: 4181 RVA: 0x0003F540 File Offset: 0x0003D740
		private Vector3 GetInputTranslationDirection()
		{
			Vector3 zero = Vector3.zero;
			Vector2 vector = this.movementAction.ReadValue<Vector2>();
			zero.x = vector.x;
			zero.z = vector.y;
			zero.y = this.verticalMovementAction.ReadValue<Vector2>().y;
			return zero;
		}

		// Token: 0x06001056 RID: 4182 RVA: 0x0003F594 File Offset: 0x0003D794
		private void Update()
		{
			if (this.IsEscapePressed())
			{
				Application.Quit();
			}
			if (this.IsRightMouseButtonDown())
			{
				Cursor.lockState = CursorLockMode.Locked;
			}
			if (this.IsRightMouseButtonUp())
			{
				Cursor.visible = true;
				Cursor.lockState = CursorLockMode.None;
			}
			if (this.IsCameraRotationAllowed())
			{
				Vector2 vector = this.GetInputLookRotation() * Time.deltaTime * 5f;
				if (this.invertY)
				{
					vector.y = -vector.y;
				}
				float num = this.mouseSensitivityCurve.Evaluate(vector.magnitude);
				this.m_TargetCameraState.yaw += vector.x * num;
				this.m_TargetCameraState.pitch += vector.y * num;
			}
			Vector3 vector2 = this.GetInputTranslationDirection() * Time.deltaTime;
			if (this.IsBoostPressed())
			{
				vector2 *= 10f;
			}
			this.boost += this.GetBoostFactor();
			vector2 *= Mathf.Pow(2f, this.boost);
			this.m_TargetCameraState.Translate(vector2);
			float num2 = 1f - Mathf.Exp(Mathf.Log(0.00999999f) / this.positionLerpTime * Time.deltaTime);
			float num3 = 1f - Mathf.Exp(Mathf.Log(0.00999999f) / this.rotationLerpTime * Time.deltaTime);
			this.m_InterpolatingCameraState.LerpTowards(this.m_TargetCameraState, num2, num3);
			this.m_InterpolatingCameraState.UpdateTransform(base.transform);
		}

		// Token: 0x06001057 RID: 4183 RVA: 0x0003F718 File Offset: 0x0003D918
		private float GetBoostFactor()
		{
			return this.boostFactorAction.ReadValue<Vector2>().y * 0.01f;
		}

		// Token: 0x06001058 RID: 4184 RVA: 0x0003F730 File Offset: 0x0003D930
		private Vector2 GetInputLookRotation()
		{
			return this.lookAction.ReadValue<Vector2>();
		}

		// Token: 0x06001059 RID: 4185 RVA: 0x0003F73D File Offset: 0x0003D93D
		private bool IsBoostPressed()
		{
			return (Keyboard.current != null && Keyboard.current.leftShiftKey.isPressed) | (Gamepad.current != null && Gamepad.current.xButton.isPressed);
		}

		// Token: 0x0600105A RID: 4186 RVA: 0x0003F772 File Offset: 0x0003D972
		private bool IsEscapePressed()
		{
			return Keyboard.current != null && Keyboard.current.escapeKey.isPressed;
		}

		// Token: 0x0600105B RID: 4187 RVA: 0x0003F78C File Offset: 0x0003D98C
		private bool IsCameraRotationAllowed()
		{
			return (Mouse.current != null && Mouse.current.rightButton.isPressed) | (Gamepad.current != null && Gamepad.current.rightStick.ReadValue().magnitude > 0f);
		}

		// Token: 0x0600105C RID: 4188 RVA: 0x0003F7DB File Offset: 0x0003D9DB
		private bool IsRightMouseButtonDown()
		{
			return Mouse.current != null && Mouse.current.rightButton.isPressed;
		}

		// Token: 0x0600105D RID: 4189 RVA: 0x0003F7F5 File Offset: 0x0003D9F5
		private bool IsRightMouseButtonUp()
		{
			return Mouse.current != null && !Mouse.current.rightButton.isPressed;
		}

		// Token: 0x04000D01 RID: 3329
		private SimpleCameraController.CameraState m_TargetCameraState = new SimpleCameraController.CameraState();

		// Token: 0x04000D02 RID: 3330
		private SimpleCameraController.CameraState m_InterpolatingCameraState = new SimpleCameraController.CameraState();

		// Token: 0x04000D03 RID: 3331
		[Header("Movement Settings")]
		[Tooltip("Exponential boost factor on translation, controllable by mouse wheel.")]
		public float boost = 3.5f;

		// Token: 0x04000D04 RID: 3332
		[Tooltip("Time it takes to interpolate camera position 99% of the way to the target.")]
		[Range(0.001f, 1f)]
		public float positionLerpTime = 0.2f;

		// Token: 0x04000D05 RID: 3333
		[Header("Rotation Settings")]
		[Tooltip("X = Change in mouse position.\nY = Multiplicative factor for camera rotation.")]
		public AnimationCurve mouseSensitivityCurve = new AnimationCurve(new Keyframe[]
		{
			new Keyframe(0f, 0.5f, 0f, 5f),
			new Keyframe(1f, 2.5f, 0f, 0f)
		});

		// Token: 0x04000D06 RID: 3334
		[Tooltip("Time it takes to interpolate camera rotation 99% of the way to the target.")]
		[Range(0.001f, 1f)]
		public float rotationLerpTime = 0.01f;

		// Token: 0x04000D07 RID: 3335
		[Tooltip("Whether or not to invert our Y axis for mouse input to rotation.")]
		public bool invertY;

		// Token: 0x04000D08 RID: 3336
		private InputAction movementAction;

		// Token: 0x04000D09 RID: 3337
		private InputAction verticalMovementAction;

		// Token: 0x04000D0A RID: 3338
		private InputAction lookAction;

		// Token: 0x04000D0B RID: 3339
		private InputAction boostFactorAction;

		// Token: 0x04000D0C RID: 3340
		private bool mouseRightButtonPressed;

		// Token: 0x02000502 RID: 1282
		private class CameraState
		{
			// Token: 0x06002746 RID: 10054 RVA: 0x0008F5EC File Offset: 0x0008D7EC
			public void SetFromTransform(Transform t)
			{
				this.pitch = t.eulerAngles.x;
				this.yaw = t.eulerAngles.y;
				this.roll = t.eulerAngles.z;
				this.x = t.position.x;
				this.y = t.position.y;
				this.z = t.position.z;
			}

			// Token: 0x06002747 RID: 10055 RVA: 0x0008F660 File Offset: 0x0008D860
			public void Translate(Vector3 translation)
			{
				Vector3 vector = Quaternion.Euler(this.pitch, this.yaw, this.roll) * translation;
				this.x += vector.x;
				this.y += vector.y;
				this.z += vector.z;
			}

			// Token: 0x06002748 RID: 10056 RVA: 0x0008F6C4 File Offset: 0x0008D8C4
			public void LerpTowards(SimpleCameraController.CameraState target, float positionLerpPct, float rotationLerpPct)
			{
				this.yaw = Mathf.Lerp(this.yaw, target.yaw, rotationLerpPct);
				this.pitch = Mathf.Lerp(this.pitch, target.pitch, rotationLerpPct);
				this.roll = Mathf.Lerp(this.roll, target.roll, rotationLerpPct);
				this.x = Mathf.Lerp(this.x, target.x, positionLerpPct);
				this.y = Mathf.Lerp(this.y, target.y, positionLerpPct);
				this.z = Mathf.Lerp(this.z, target.z, positionLerpPct);
			}

			// Token: 0x06002749 RID: 10057 RVA: 0x0008F761 File Offset: 0x0008D961
			public void UpdateTransform(Transform t)
			{
				t.eulerAngles = new Vector3(this.pitch, this.yaw, this.roll);
				t.position = new Vector3(this.x, this.y, this.z);
			}

			// Token: 0x04001DA8 RID: 7592
			public float yaw;

			// Token: 0x04001DA9 RID: 7593
			public float pitch;

			// Token: 0x04001DAA RID: 7594
			public float roll;

			// Token: 0x04001DAB RID: 7595
			public float x;

			// Token: 0x04001DAC RID: 7596
			public float y;

			// Token: 0x04001DAD RID: 7597
			public float z;
		}
	}
}
