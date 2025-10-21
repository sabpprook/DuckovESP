using System;
using Duckov.MiniGames.BubblePoppers;
using SodaCraft.Localizations;
using SodaCraft.StringUtilities;
using UnityEngine;

namespace Duckov.Quests
{
	// Token: 0x0200033E RID: 830
	public class QuestTask_BubblePopper_Level : Task
	{
		// Token: 0x17000557 RID: 1367
		// (get) Token: 0x06001C90 RID: 7312 RVA: 0x00066DB5 File Offset: 0x00064FB5
		// (set) Token: 0x06001C91 RID: 7313 RVA: 0x00066DBC File Offset: 0x00064FBC
		[LocalizationKey("Default")]
		private string descriptionKey
		{
			get
			{
				return "Task_BubblePopper_Level";
			}
			set
			{
			}
		}

		// Token: 0x17000558 RID: 1368
		// (get) Token: 0x06001C92 RID: 7314 RVA: 0x00066DBE File Offset: 0x00064FBE
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

		// Token: 0x06001C93 RID: 7315 RVA: 0x00066DDB File Offset: 0x00064FDB
		public override object GenerateSaveData()
		{
			return this.finished;
		}

		// Token: 0x06001C94 RID: 7316 RVA: 0x00066DE8 File Offset: 0x00064FE8
		public override void SetupSaveData(object data)
		{
		}

		// Token: 0x06001C95 RID: 7317 RVA: 0x00066DEA File Offset: 0x00064FEA
		protected override bool CheckFinished()
		{
			return BubblePopper.HighLevel >= this.targetLevel;
		}

		// Token: 0x040013E1 RID: 5089
		[SerializeField]
		private int targetLevel;

		// Token: 0x040013E2 RID: 5090
		private bool finished;
	}
}
