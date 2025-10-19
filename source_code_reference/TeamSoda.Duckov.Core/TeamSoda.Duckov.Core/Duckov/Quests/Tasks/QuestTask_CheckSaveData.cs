using System;
using Saves;
using SodaCraft.Localizations;
using UnityEngine;

namespace Duckov.Quests.Tasks
{
	// Token: 0x0200034C RID: 844
	public class QuestTask_CheckSaveData : Task
	{
		// Token: 0x17000573 RID: 1395
		// (get) Token: 0x06001D3A RID: 7482 RVA: 0x00068A69 File Offset: 0x00066C69
		public string SaveDataKey
		{
			get
			{
				return this.saveDataKey;
			}
		}

		// Token: 0x17000574 RID: 1396
		// (get) Token: 0x06001D3B RID: 7483 RVA: 0x00068A71 File Offset: 0x00066C71
		// (set) Token: 0x06001D3C RID: 7484 RVA: 0x00068A7E File Offset: 0x00066C7E
		private bool SaveDataTrue
		{
			get
			{
				return SavesSystem.Load<bool>(this.saveDataKey);
			}
			set
			{
				SavesSystem.Save<bool>(this.saveDataKey, value);
			}
		}

		// Token: 0x17000575 RID: 1397
		// (get) Token: 0x06001D3D RID: 7485 RVA: 0x00068A8C File Offset: 0x00066C8C
		public override string Description
		{
			get
			{
				return this.description.ToPlainText();
			}
		}

		// Token: 0x06001D3E RID: 7486 RVA: 0x00068A99 File Offset: 0x00066C99
		protected override void OnInit()
		{
			base.OnInit();
		}

		// Token: 0x06001D3F RID: 7487 RVA: 0x00068AA1 File Offset: 0x00066CA1
		private void OnDisable()
		{
		}

		// Token: 0x06001D40 RID: 7488 RVA: 0x00068AA3 File Offset: 0x00066CA3
		protected override bool CheckFinished()
		{
			return this.SaveDataTrue;
		}

		// Token: 0x06001D41 RID: 7489 RVA: 0x00068AAB File Offset: 0x00066CAB
		public override object GenerateSaveData()
		{
			return this.SaveDataTrue;
		}

		// Token: 0x06001D42 RID: 7490 RVA: 0x00068AB8 File Offset: 0x00066CB8
		public override void SetupSaveData(object data)
		{
		}

		// Token: 0x04001457 RID: 5207
		[SerializeField]
		private string saveDataKey;

		// Token: 0x04001458 RID: 5208
		[SerializeField]
		[LocalizationKey("Quests")]
		private string description;
	}
}
