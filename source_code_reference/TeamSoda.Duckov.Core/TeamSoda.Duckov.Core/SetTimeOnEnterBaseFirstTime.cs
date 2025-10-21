using System;
using Saves;
using UnityEngine;

// Token: 0x020000AD RID: 173
public class SetTimeOnEnterBaseFirstTime : MonoBehaviour
{
	// Token: 0x060005BD RID: 1469 RVA: 0x00019AE0 File Offset: 0x00017CE0
	private void Start()
	{
		if (SavesSystem.Load<bool>("FirstTimeToBaseTimeSetted"))
		{
			return;
		}
		SavesSystem.Save<bool>("FirstTimeToBaseTimeSetted", true);
		TimeSpan timeSpan = new TimeSpan(this.setTimeTo, 0, 0);
		GameClock.Instance.StepTimeTil(timeSpan);
	}

	// Token: 0x04000541 RID: 1345
	public int setTimeTo;
}
