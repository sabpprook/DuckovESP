using System;
using NodeCanvas.DialogueTrees;
using NodeCanvas.Framework;
using SodaCraft.StringUtilities;
using UnityEngine;

namespace Dialogues
{
	// Token: 0x0200021C RID: 540
	public class LocalizedStatementSequence : DTNode
	{
		// Token: 0x170002E8 RID: 744
		// (get) Token: 0x0600103B RID: 4155 RVA: 0x0003F017 File Offset: 0x0003D217
		public override bool requireActorSelection
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600103C RID: 4156 RVA: 0x0003F01A File Offset: 0x0003D21A
		protected override Status OnExecute(Component agent, IBlackboard bb)
		{
			this.Begin();
			return Status.Running;
		}

		// Token: 0x0600103D RID: 4157 RVA: 0x0003F023 File Offset: 0x0003D223
		private void Begin()
		{
			this.index = this.beginIndex.value - 1;
			this.Next();
		}

		// Token: 0x0600103E RID: 4158 RVA: 0x0003F040 File Offset: 0x0003D240
		private void Next()
		{
			this.index++;
			if (this.index > this.endIndex.value)
			{
				base.status = Status.Success;
				base.DLGTree.Continue(0);
				return;
			}
			LocalizedStatement localizedStatement = new LocalizedStatement(this.format.value.Format(new
			{
				keyPrefix = this.keyPrefix.value,
				index = this.index
			}));
			DialogueTree.RequestSubtitles(new SubtitlesRequestInfo(base.finalActor, localizedStatement, new Action(this.OnStatementFinish)));
		}

		// Token: 0x0600103F RID: 4159 RVA: 0x0003F0CB File Offset: 0x0003D2CB
		private void OnStatementFinish()
		{
			this.Next();
		}

		// Token: 0x04000CF4 RID: 3316
		public BBParameter<string> keyPrefix;

		// Token: 0x04000CF5 RID: 3317
		public BBParameter<int> beginIndex;

		// Token: 0x04000CF6 RID: 3318
		public BBParameter<int> endIndex;

		// Token: 0x04000CF7 RID: 3319
		public BBParameter<string> format = new BBParameter<string>("{keyPrefix}_{index}");

		// Token: 0x04000CF8 RID: 3320
		private int index;
	}
}
