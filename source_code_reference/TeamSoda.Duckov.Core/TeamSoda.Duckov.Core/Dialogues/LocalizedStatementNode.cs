using System;
using NodeCanvas.DialogueTrees;
using NodeCanvas.Framework;
using UnityEngine;

namespace Dialogues
{
	// Token: 0x0200021B RID: 539
	public class LocalizedStatementNode : DTNode
	{
		// Token: 0x170002E6 RID: 742
		// (get) Token: 0x06001035 RID: 4149 RVA: 0x0003EF75 File Offset: 0x0003D175
		public override bool requireActorSelection
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170002E7 RID: 743
		// (get) Token: 0x06001036 RID: 4150 RVA: 0x0003EF78 File Offset: 0x0003D178
		private string Key
		{
			get
			{
				if (this.useSequence.value)
				{
					return string.Format("{0}_{1}", this.key.value, this.sequenceIndex.value);
				}
				return this.key.value;
			}
		}

		// Token: 0x06001037 RID: 4151 RVA: 0x0003EFB8 File Offset: 0x0003D1B8
		private LocalizedStatement CreateStatement()
		{
			return new LocalizedStatement(this.Key);
		}

		// Token: 0x06001038 RID: 4152 RVA: 0x0003EFC8 File Offset: 0x0003D1C8
		protected override Status OnExecute(Component agent, IBlackboard bb)
		{
			LocalizedStatement localizedStatement = this.CreateStatement();
			DialogueTree.RequestSubtitles(new SubtitlesRequestInfo(base.finalActor, localizedStatement, new Action(this.OnStatementFinish)));
			return Status.Running;
		}

		// Token: 0x06001039 RID: 4153 RVA: 0x0003EFFA File Offset: 0x0003D1FA
		private void OnStatementFinish()
		{
			base.status = Status.Success;
			base.DLGTree.Continue(0);
		}

		// Token: 0x04000CF1 RID: 3313
		public BBParameter<string> key;

		// Token: 0x04000CF2 RID: 3314
		public BBParameter<bool> useSequence;

		// Token: 0x04000CF3 RID: 3315
		public BBParameter<int> sequenceIndex;
	}
}
