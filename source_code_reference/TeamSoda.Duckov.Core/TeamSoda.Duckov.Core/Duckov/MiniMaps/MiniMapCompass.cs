using System;
using Cinemachine.Utility;
using UnityEngine;

namespace Duckov.MiniMaps
{
	// Token: 0x02000274 RID: 628
	public class MiniMapCompass : MonoBehaviour
	{
		// Token: 0x060013CF RID: 5071 RVA: 0x00049324 File Offset: 0x00047524
		private void SetupRotation()
		{
			Vector3 vector = LevelManager.Instance.GameCamera.mainVCam.transform.up.ProjectOntoPlane(Vector3.up);
			Vector3 forward = Vector3.forward;
			float num = Vector3.SignedAngle(vector, forward, Vector3.up);
			this.arrow.localRotation = Quaternion.Euler(0f, 0f, -num);
		}

		// Token: 0x060013D0 RID: 5072 RVA: 0x00049382 File Offset: 0x00047582
		private void Update()
		{
			this.SetupRotation();
		}

		// Token: 0x04000E99 RID: 3737
		[SerializeField]
		private Transform arrow;
	}
}
