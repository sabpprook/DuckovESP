using System;
using Duckov.Scenes;
using Duckvo.Beacons;
using UnityEngine;

// Token: 0x020000B3 RID: 179
public class TeleportBeacon : MonoBehaviour
{
	// Token: 0x060005E1 RID: 1505 RVA: 0x0001A41C File Offset: 0x0001861C
	private void Start()
	{
		bool beaconUnlocked = BeaconManager.GetBeaconUnlocked(this.beaconScene, this.beaconIndex);
		this.activeByUnlocked.SetActive(beaconUnlocked);
		this.interactable.gameObject.SetActive(!beaconUnlocked);
	}

	// Token: 0x060005E2 RID: 1506 RVA: 0x0001A45B File Offset: 0x0001865B
	public void ActivateBeacon()
	{
		BeaconManager.UnlockBeacon(this.beaconScene, this.beaconIndex);
		this.activeByUnlocked.SetActive(true);
		this.interactable.gameObject.SetActive(false);
	}

	// Token: 0x0400056B RID: 1387
	[SceneID]
	public string beaconScene;

	// Token: 0x0400056C RID: 1388
	public int beaconIndex;

	// Token: 0x0400056D RID: 1389
	public GameObject activeByUnlocked;

	// Token: 0x0400056E RID: 1390
	public InteractableBase interactable;
}
