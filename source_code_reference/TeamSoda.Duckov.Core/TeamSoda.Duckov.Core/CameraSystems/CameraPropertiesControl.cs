using System;
using Cinemachine;
using Cinemachine.PostFX;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

namespace CameraSystems
{
	// Token: 0x0200020F RID: 527
	public class CameraPropertiesControl : MonoBehaviour
	{
		// Token: 0x06000FC2 RID: 4034 RVA: 0x0003DD64 File Offset: 0x0003BF64
		private void Awake()
		{
			this.vCam = base.GetComponent<CinemachineVirtualCamera>();
			this.volumeSettings = base.GetComponent<CinemachineVolumeSettings>();
		}

		// Token: 0x06000FC3 RID: 4035 RVA: 0x0003DD80 File Offset: 0x0003BF80
		private unsafe void Update()
		{
			float num = *Gamepad.current.dpad.x.value;
			if (*Gamepad.current.dpad.y.value != 0f)
			{
				float num2 = -(*Gamepad.current.dpad.y.value);
				if (*Gamepad.current.rightShoulder.value > 0f)
				{
					num2 *= 10f;
				}
				this.vCam.m_Lens.FieldOfView = Mathf.Clamp(this.vCam.m_Lens.FieldOfView + num2 * 5f * Time.deltaTime, 8f, 100f);
			}
		}

		// Token: 0x04000CAB RID: 3243
		private CinemachineVirtualCamera vCam;

		// Token: 0x04000CAC RID: 3244
		private CinemachineVolumeSettings volumeSettings;

		// Token: 0x04000CAD RID: 3245
		[SerializeField]
		private VolumeProfile volumeProfile;
	}
}
