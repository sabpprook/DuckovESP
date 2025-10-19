using System;
using UnityEngine.Rendering;

namespace SodaCraft
{
	// Token: 0x0200041B RID: 1051
	[VolumeComponentMenu("SodaCraft/CameraArmControl")]
	[Serializable]
	public class CameraArmControl : VolumeComponent
	{
		// Token: 0x060025B9 RID: 9657 RVA: 0x00081F95 File Offset: 0x00080195
		public bool IsActive()
		{
			return this.enable.value;
		}

		// Token: 0x060025BA RID: 9658 RVA: 0x00081FA2 File Offset: 0x000801A2
		public override void Override(VolumeComponent state, float interpFactor)
		{
			CameraArmControl cameraArmControl = state as CameraArmControl;
			base.Override(state, interpFactor);
			CameraArm.globalPitch = cameraArmControl.pitch.value;
			CameraArm.globalYaw = cameraArmControl.yaw.value;
			CameraArm.globalDistance = cameraArmControl.distance.value;
		}

		// Token: 0x040019B7 RID: 6583
		public BoolParameter enable = new BoolParameter(false, false);

		// Token: 0x040019B8 RID: 6584
		public MinFloatParameter pitch = new MinFloatParameter(55f, 0f, false);

		// Token: 0x040019B9 RID: 6585
		public FloatParameter yaw = new FloatParameter(-30f, false);

		// Token: 0x040019BA RID: 6586
		public MinFloatParameter distance = new MinFloatParameter(45f, 2f, false);
	}
}
