using System;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

// Token: 0x02000191 RID: 401
[Serializable]
public struct TimeOfDayPhase
{
	// Token: 0x04000A48 RID: 2632
	[FormerlySerializedAs("phaseTag")]
	public TimePhaseTags timePhaseTag;

	// Token: 0x04000A49 RID: 2633
	public VolumeProfile volumeProfile;
}
