using System;
using Duckov.Buildings;
using SodaCraft.Localizations;
using SodaCraft.StringUtilities;
using UnityEngine;

namespace Duckov.Quests.Tasks
{
	// Token: 0x0200034D RID: 845
	public class QuestTask_ConstructBuilding : Task
	{
		// Token: 0x17000576 RID: 1398
		// (get) Token: 0x06001D44 RID: 7492 RVA: 0x00068AC2 File Offset: 0x00066CC2
		[LocalizationKey("Default")]
		private string descriptionFormatKey
		{
			get
			{
				return "Task_ConstructBuilding";
			}
		}

		// Token: 0x17000577 RID: 1399
		// (get) Token: 0x06001D45 RID: 7493 RVA: 0x00068AC9 File Offset: 0x00066CC9
		private string DescriptionFormat
		{
			get
			{
				return this.descriptionFormatKey.ToPlainText();
			}
		}

		// Token: 0x17000578 RID: 1400
		// (get) Token: 0x06001D46 RID: 7494 RVA: 0x00068AD6 File Offset: 0x00066CD6
		public override string Description
		{
			get
			{
				return this.DescriptionFormat.Format(new
				{
					BuildingName = Building.GetDisplayName(this.buildingID)
				});
			}
		}

		// Token: 0x06001D47 RID: 7495 RVA: 0x00068AF3 File Offset: 0x00066CF3
		public override object GenerateSaveData()
		{
			return null;
		}

		// Token: 0x06001D48 RID: 7496 RVA: 0x00068AF6 File Offset: 0x00066CF6
		protected override bool CheckFinished()
		{
			return BuildingManager.Any(this.buildingID, false);
		}

		// Token: 0x06001D49 RID: 7497 RVA: 0x00068B04 File Offset: 0x00066D04
		public override void SetupSaveData(object data)
		{
		}

		// Token: 0x04001459 RID: 5209
		[SerializeField]
		private string buildingID;
	}
}
