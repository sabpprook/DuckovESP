using System;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace SodaCraft
{
	// Token: 0x0200041E RID: 1054
	[VolumeComponentMenu("SodaCraft/TimeOfDayPost")]
	[Serializable]
	public class TimeOfDayPost : VolumeComponent, IPostProcessComponent
	{
		// Token: 0x060025C4 RID: 9668 RVA: 0x0008245D File Offset: 0x0008065D
		public bool IsActive()
		{
			return this.enable.value;
		}

		// Token: 0x060025C5 RID: 9669 RVA: 0x0008246A File Offset: 0x0008066A
		public bool IsTileCompatible()
		{
			return false;
		}

		// Token: 0x060025C6 RID: 9670 RVA: 0x00082470 File Offset: 0x00080670
		public override void Override(VolumeComponent state, float interpFactor)
		{
			TimeOfDayPost timeOfDayPost = state as TimeOfDayPost;
			base.Override(state, interpFactor);
			if (timeOfDayPost == null)
			{
				return;
			}
			TimeOfDayController.NightViewAngleFactor = timeOfDayPost.nightViewAngleFactor.value;
			TimeOfDayController.NightViewDistanceFactor = timeOfDayPost.nightViewDistanceFactor.value;
			TimeOfDayController.NightSenseRangeFactor = timeOfDayPost.nightSenseRangeFactor.value;
		}

		// Token: 0x040019D6 RID: 6614
		public BoolParameter enable = new BoolParameter(false, false);

		// Token: 0x040019D7 RID: 6615
		public ClampedFloatParameter nightViewAngleFactor = new ClampedFloatParameter(0.2f, 0f, 1f, false);

		// Token: 0x040019D8 RID: 6616
		public ClampedFloatParameter nightViewDistanceFactor = new ClampedFloatParameter(0.2f, 0f, 1f, false);

		// Token: 0x040019D9 RID: 6617
		public ClampedFloatParameter nightSenseRangeFactor = new ClampedFloatParameter(0.2f, 0f, 1f, false);
	}
}
