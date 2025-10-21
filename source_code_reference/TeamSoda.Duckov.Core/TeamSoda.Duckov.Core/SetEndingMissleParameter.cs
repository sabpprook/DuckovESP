using System;
using Duckov;
using Duckov.Quests;
using UnityEngine;

// Token: 0x0200007C RID: 124
public class SetEndingMissleParameter : MonoBehaviour
{
	// Token: 0x060004AE RID: 1198 RVA: 0x00015790 File Offset: 0x00013990
	private void Start()
	{
		bool flag = this.launcherClosedCondition.Evaluate();
		AudioManager.SetRTPC("Ending_Missile", (float)(flag ? 1 : 0), null);
	}

	// Token: 0x040003F7 RID: 1015
	[SerializeField]
	private Condition launcherClosedCondition;
}
