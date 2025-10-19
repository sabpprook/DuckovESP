using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200018F RID: 399
public class TimeOfDayEntry : MonoBehaviour
{
	// Token: 0x06000BD4 RID: 3028 RVA: 0x00032498 File Offset: 0x00030698
	private void Start()
	{
		if (this.phases.Count > 0)
		{
			TimeOfDayPhase timeOfDayPhase = this.phases[0];
			this.phases[0] = timeOfDayPhase;
		}
	}

	// Token: 0x06000BD5 RID: 3029 RVA: 0x000324D0 File Offset: 0x000306D0
	public TimeOfDayPhase GetPhase(TimePhaseTags timePhaseTags)
	{
		for (int i = 0; i < this.phases.Count; i++)
		{
			TimeOfDayPhase timeOfDayPhase = this.phases[i];
			if (timeOfDayPhase.timePhaseTag == timePhaseTags)
			{
				return timeOfDayPhase;
			}
		}
		if (timePhaseTags == TimePhaseTags.dawn)
		{
			return this.GetPhase(TimePhaseTags.day);
		}
		return this.phases[0];
	}

	// Token: 0x04000A43 RID: 2627
	[SerializeField]
	private List<TimeOfDayPhase> phases;
}
