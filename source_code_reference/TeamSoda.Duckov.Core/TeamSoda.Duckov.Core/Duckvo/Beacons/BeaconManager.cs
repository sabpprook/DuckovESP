using System;
using System.Collections.Generic;
using System.Linq;
using Saves;
using UnityEngine;

namespace Duckvo.Beacons
{
	// Token: 0x0200021F RID: 543
	public class BeaconManager : MonoBehaviour
	{
		// Token: 0x170002E9 RID: 745
		// (get) Token: 0x0600104A RID: 4170 RVA: 0x0003F1A5 File Offset: 0x0003D3A5
		// (set) Token: 0x0600104B RID: 4171 RVA: 0x0003F1AC File Offset: 0x0003D3AC
		public static BeaconManager Instance { get; private set; }

		// Token: 0x0600104C RID: 4172 RVA: 0x0003F1B4 File Offset: 0x0003D3B4
		private void Awake()
		{
			BeaconManager.Instance = this;
			this.Load();
			SavesSystem.OnCollectSaveData += this.Save;
		}

		// Token: 0x0600104D RID: 4173 RVA: 0x0003F1D3 File Offset: 0x0003D3D3
		private void OnDestroy()
		{
			SavesSystem.OnCollectSaveData -= this.Save;
		}

		// Token: 0x0600104E RID: 4174 RVA: 0x0003F1E6 File Offset: 0x0003D3E6
		public void Load()
		{
			if (SavesSystem.KeyExisits("BeaconManager"))
			{
				this.data = SavesSystem.Load<BeaconManager.Data>("BeaconManager");
			}
			if (this.data.entries == null)
			{
				this.data.entries = new List<BeaconManager.BeaconStatus>();
			}
		}

		// Token: 0x0600104F RID: 4175 RVA: 0x0003F221 File Offset: 0x0003D421
		public void Save()
		{
			SavesSystem.Save<BeaconManager.Data>("BeaconManager", this.data);
		}

		// Token: 0x06001050 RID: 4176 RVA: 0x0003F234 File Offset: 0x0003D434
		public static void UnlockBeacon(string id, int index)
		{
			if (BeaconManager.Instance == null)
			{
				return;
			}
			if (BeaconManager.GetBeaconUnlocked(id, index))
			{
				return;
			}
			BeaconManager.Instance.data.entries.Add(new BeaconManager.BeaconStatus
			{
				beaconID = id,
				beaconIndex = index
			});
			Action<string, int> onBeaconUnlocked = BeaconManager.OnBeaconUnlocked;
			if (onBeaconUnlocked == null)
			{
				return;
			}
			onBeaconUnlocked(id, index);
		}

		// Token: 0x06001051 RID: 4177 RVA: 0x0003F298 File Offset: 0x0003D498
		public static bool GetBeaconUnlocked(string id, int index)
		{
			return !(BeaconManager.Instance == null) && BeaconManager.Instance.data.entries.Any((BeaconManager.BeaconStatus e) => e.beaconID == id && e.beaconIndex == index);
		}

		// Token: 0x04000CFE RID: 3326
		private BeaconManager.Data data;

		// Token: 0x04000CFF RID: 3327
		public static Action<string, int> OnBeaconUnlocked;

		// Token: 0x04000D00 RID: 3328
		private const string SaveKey = "BeaconManager";

		// Token: 0x020004FF RID: 1279
		[Serializable]
		public struct BeaconStatus
		{
			// Token: 0x04001DA3 RID: 7587
			public string beaconID;

			// Token: 0x04001DA4 RID: 7588
			public int beaconIndex;
		}

		// Token: 0x02000500 RID: 1280
		[Serializable]
		public struct Data
		{
			// Token: 0x04001DA5 RID: 7589
			public List<BeaconManager.BeaconStatus> entries;
		}
	}
}
