using System;
using Duckov.Quests;
using Duckov.Scenes;
using Duckvo.Beacons;
using UnityEngine;

// Token: 0x0200011A RID: 282
public class RequireBeaconUnlocked : Condition
{
	// Token: 0x06000971 RID: 2417 RVA: 0x000293FC File Offset: 0x000275FC
	public override bool Evaluate()
	{
		return BeaconManager.GetBeaconUnlocked(this.beaconID, this.beaconIndex);
	}

	// Token: 0x04000856 RID: 2134
	[SerializeField]
	[SceneID]
	private string beaconID;

	// Token: 0x04000857 RID: 2135
	[SerializeField]
	private int beaconIndex;
}
