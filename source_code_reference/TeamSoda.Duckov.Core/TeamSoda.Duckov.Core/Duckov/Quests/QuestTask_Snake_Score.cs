using System;
using Duckov.MiniGames.SnakeForces;
using SodaCraft.Localizations;
using SodaCraft.StringUtilities;
using UnityEngine;

namespace Duckov.Quests
{
	// Token: 0x02000340 RID: 832
	public class QuestTask_Snake_Score : Task
	{
		// Token: 0x1700055B RID: 1371
		// (get) Token: 0x06001C9E RID: 7326 RVA: 0x00066E55 File Offset: 0x00065055
		// (set) Token: 0x06001C9F RID: 7327 RVA: 0x00066E5C File Offset: 0x0006505C
		[LocalizationKey("Default")]
		private string descriptionKey
		{
			get
			{
				return "Task_Snake_Score";
			}
			set
			{
			}
		}

		// Token: 0x1700055C RID: 1372
		// (get) Token: 0x06001CA0 RID: 7328 RVA: 0x00066E5E File Offset: 0x0006505E
		public override string Description
		{
			get
			{
				return this.descriptionKey.ToPlainText().Format(new
				{
					score = this.targetScore
				});
			}
		}

		// Token: 0x06001CA1 RID: 7329 RVA: 0x00066E7B File Offset: 0x0006507B
		public override object GenerateSaveData()
		{
			return this.finished;
		}

		// Token: 0x06001CA2 RID: 7330 RVA: 0x00066E88 File Offset: 0x00065088
		public override void SetupSaveData(object data)
		{
		}

		// Token: 0x06001CA3 RID: 7331 RVA: 0x00066E8A File Offset: 0x0006508A
		protected override bool CheckFinished()
		{
			return SnakeForce.HighScore >= this.targetScore;
		}

		// Token: 0x040013E5 RID: 5093
		[SerializeField]
		private int targetScore;

		// Token: 0x040013E6 RID: 5094
		private bool finished;
	}
}
