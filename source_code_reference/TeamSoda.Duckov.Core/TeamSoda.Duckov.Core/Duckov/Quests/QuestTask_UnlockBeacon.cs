using System;
using Duckvo.Beacons;
using SodaCraft.Localizations;
using UnityEngine;

namespace Duckov.Quests
{
	// Token: 0x02000338 RID: 824
	public class QuestTask_UnlockBeacon : Task
	{
		// Token: 0x17000551 RID: 1361
		// (get) Token: 0x06001C5C RID: 7260 RVA: 0x0006638F File Offset: 0x0006458F
		// (set) Token: 0x06001C5D RID: 7261 RVA: 0x000663A1 File Offset: 0x000645A1
		[LocalizationKey("Default")]
		private string DescriptionKey
		{
			get
			{
				return "Task_Beacon_" + this.beaconID;
			}
			set
			{
			}
		}

		// Token: 0x17000552 RID: 1362
		// (get) Token: 0x06001C5E RID: 7262 RVA: 0x000663A3 File Offset: 0x000645A3
		public override string Description
		{
			get
			{
				return this.DescriptionKey.ToPlainText();
			}
		}

		// Token: 0x06001C5F RID: 7263 RVA: 0x000663B0 File Offset: 0x000645B0
		public override object GenerateSaveData()
		{
			return 0;
		}

		// Token: 0x06001C60 RID: 7264 RVA: 0x000663B8 File Offset: 0x000645B8
		public override void SetupSaveData(object data)
		{
		}

		// Token: 0x06001C61 RID: 7265 RVA: 0x000663BA File Offset: 0x000645BA
		protected override bool CheckFinished()
		{
			return BeaconManager.GetBeaconUnlocked(this.beaconID, this.beaconIndex);
		}

		// Token: 0x040013C6 RID: 5062
		[SerializeField]
		private string beaconID;

		// Token: 0x040013C7 RID: 5063
		[SerializeField]
		private int beaconIndex;
	}
}
