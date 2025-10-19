using System;
using Duckov.Quests;
using UnityEngine;

// Token: 0x0200011B RID: 283
public class RequireEnemyKilled : Condition
{
	// Token: 0x06000973 RID: 2419 RVA: 0x00029417 File Offset: 0x00027617
	public override bool Evaluate()
	{
		return !(this.enemyPreset == null) && SavesCounter.GetKillCount(this.enemyPreset.nameKey) >= this.threshold;
	}

	// Token: 0x04000858 RID: 2136
	[SerializeField]
	private CharacterRandomPreset enemyPreset;

	// Token: 0x04000859 RID: 2137
	[SerializeField]
	private int threshold = 1;
}
