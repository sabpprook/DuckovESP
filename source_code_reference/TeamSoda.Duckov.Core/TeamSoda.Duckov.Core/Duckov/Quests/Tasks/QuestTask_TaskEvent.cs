using System;
using SodaCraft.Localizations;
using UnityEngine;

namespace Duckov.Quests.Tasks
{
	// Token: 0x02000352 RID: 850
	public class QuestTask_TaskEvent : Task
	{
		// Token: 0x1700059E RID: 1438
		// (get) Token: 0x06001D9B RID: 7579 RVA: 0x000693D9 File Offset: 0x000675D9
		public string EventKey
		{
			get
			{
				return this.eventKey;
			}
		}

		// Token: 0x1700059F RID: 1439
		// (get) Token: 0x06001D9C RID: 7580 RVA: 0x000693E1 File Offset: 0x000675E1
		public override string Description
		{
			get
			{
				return this.description.ToPlainText();
			}
		}

		// Token: 0x06001D9D RID: 7581 RVA: 0x000693EE File Offset: 0x000675EE
		private void OnTaskEvent(string _key)
		{
			if (_key == this.eventKey)
			{
				this.finished = true;
				this.SetMapElementVisable(false);
				base.ReportStatusChanged();
			}
		}

		// Token: 0x06001D9E RID: 7582 RVA: 0x00069412 File Offset: 0x00067612
		protected override void OnInit()
		{
			base.OnInit();
			TaskEvent.OnTaskEvent += this.OnTaskEvent;
			this.SetMapElementVisable(!base.IsFinished());
		}

		// Token: 0x06001D9F RID: 7583 RVA: 0x0006943A File Offset: 0x0006763A
		private void OnDisable()
		{
			TaskEvent.OnTaskEvent -= this.OnTaskEvent;
		}

		// Token: 0x06001DA0 RID: 7584 RVA: 0x0006944D File Offset: 0x0006764D
		protected override bool CheckFinished()
		{
			return this.finished;
		}

		// Token: 0x06001DA1 RID: 7585 RVA: 0x00069455 File Offset: 0x00067655
		public override object GenerateSaveData()
		{
			return this.finished;
		}

		// Token: 0x06001DA2 RID: 7586 RVA: 0x00069464 File Offset: 0x00067664
		public override void SetupSaveData(object data)
		{
			if (data is bool)
			{
				bool flag = (bool)data;
				this.finished = flag;
			}
		}

		// Token: 0x06001DA3 RID: 7587 RVA: 0x00069488 File Offset: 0x00067688
		private void SetMapElementVisable(bool visable)
		{
			if (!this.mapElement)
			{
				return;
			}
			if (!this.mapElement.enabled)
			{
				return;
			}
			if (visable)
			{
				this.mapElement.name = base.Master.DisplayName;
			}
			this.mapElement.SetVisibility(visable);
		}

		// Token: 0x04001470 RID: 5232
		[SerializeField]
		private string eventKey;

		// Token: 0x04001471 RID: 5233
		[SerializeField]
		[LocalizationKey("Quests")]
		private string description;

		// Token: 0x04001472 RID: 5234
		private bool finished;

		// Token: 0x04001473 RID: 5235
		[SerializeField]
		private MapElementForTask mapElement;
	}
}
