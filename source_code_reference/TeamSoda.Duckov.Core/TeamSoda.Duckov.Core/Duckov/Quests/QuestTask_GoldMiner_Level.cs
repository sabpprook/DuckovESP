using System;
using Duckov.MiniGames.GoldMiner;
using SodaCraft.Localizations;
using SodaCraft.StringUtilities;
using UnityEngine;

namespace Duckov.Quests
{
	// Token: 0x0200033F RID: 831
	public class QuestTask_GoldMiner_Level : Task
	{
		// Token: 0x17000559 RID: 1369
		// (get) Token: 0x06001C97 RID: 7319 RVA: 0x00066E04 File Offset: 0x00065004
		// (set) Token: 0x06001C98 RID: 7320 RVA: 0x00066E0B File Offset: 0x0006500B
		[LocalizationKey("Default")]
		private string descriptionKey
		{
			get
			{
				return "Task_GoldMiner_Level";
			}
			set
			{
			}
		}

		// Token: 0x1700055A RID: 1370
		// (get) Token: 0x06001C99 RID: 7321 RVA: 0x00066E0D File Offset: 0x0006500D
		public override string Description
		{
			get
			{
				return this.descriptionKey.ToPlainText().Format(new
				{
					level = this.targetLevel
				});
			}
		}

		// Token: 0x06001C9A RID: 7322 RVA: 0x00066E2A File Offset: 0x0006502A
		public override object GenerateSaveData()
		{
			return this.finished;
		}

		// Token: 0x06001C9B RID: 7323 RVA: 0x00066E37 File Offset: 0x00065037
		public override void SetupSaveData(object data)
		{
		}

		// Token: 0x06001C9C RID: 7324 RVA: 0x00066E39 File Offset: 0x00065039
		protected override bool CheckFinished()
		{
			return GoldMiner.HighLevel + 1 >= this.targetLevel;
		}

		// Token: 0x040013E3 RID: 5091
		[SerializeField]
		private int targetLevel;

		// Token: 0x040013E4 RID: 5092
		private bool finished;
	}
}
