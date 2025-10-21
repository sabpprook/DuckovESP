using System;
using System.Collections.Generic;
using Dialogues;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.DialogueTrees
{
	// Token: 0x020003FB RID: 1019
	[ParadoxNotion.Design.Icon("List", false, "")]
	[Name("Multiple Choice Localized", 0)]
	[Category("Branch")]
	[Description("Prompt a Dialogue Multiple Choice. A choice will be available if the choice condition(s) are true or there is no choice conditions. The Actor selected is used for the condition checks and will also Say the selection if the option is checked.")]
	[Color("b3ff7f")]
	public class LocalizedMultipleChoiceNode : DTNode
	{
		// Token: 0x17000727 RID: 1831
		// (get) Token: 0x06002506 RID: 9478 RVA: 0x0007FD8E File Offset: 0x0007DF8E
		public override int maxOutConnections
		{
			get
			{
				return this.availableChoices.Count;
			}
		}

		// Token: 0x17000728 RID: 1832
		// (get) Token: 0x06002507 RID: 9479 RVA: 0x0007FD9B File Offset: 0x0007DF9B
		public override bool requireActorSelection
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06002508 RID: 9480 RVA: 0x0007FDA0 File Offset: 0x0007DFA0
		protected override Status OnExecute(Component agent, IBlackboard bb)
		{
			if (base.outConnections.Count == 0)
			{
				return base.Error("There are no connections to the Multiple Choice Node!");
			}
			Dictionary<IStatement, int> dictionary = new Dictionary<IStatement, int>();
			for (int i = 0; i < this.availableChoices.Count; i++)
			{
				ConditionTask condition = this.availableChoices[i].condition;
				if (condition == null || condition.CheckOnce(base.finalActor.transform, bb))
				{
					LocalizedStatement statement = this.availableChoices[i].statement;
					dictionary[statement] = i;
				}
			}
			if (dictionary.Count == 0)
			{
				base.DLGTree.Stop(false);
				return Status.Failure;
			}
			DialogueTree.RequestMultipleChoices(new MultipleChoiceRequestInfo(base.finalActor, dictionary, this.availableTime, new Action<int>(this.OnOptionSelected))
			{
				showLastStatement = true
			});
			return Status.Running;
		}

		// Token: 0x06002509 RID: 9481 RVA: 0x0007FE68 File Offset: 0x0007E068
		private void OnOptionSelected(int index)
		{
			base.status = Status.Success;
			Action action = delegate
			{
				this.DLGTree.Continue(index);
			};
			if (this.saySelection)
			{
				LocalizedStatement statement = this.availableChoices[index].statement;
				DialogueTree.RequestSubtitles(new SubtitlesRequestInfo(base.finalActor, statement, action));
				return;
			}
			action();
		}

		// Token: 0x0400193F RID: 6463
		[SliderField(0f, 10f)]
		public float availableTime;

		// Token: 0x04001940 RID: 6464
		public bool saySelection;

		// Token: 0x04001941 RID: 6465
		[SerializeField]
		[Node.AutoSortWithChildrenConnections]
		private List<LocalizedMultipleChoiceNode.Choice> availableChoices = new List<LocalizedMultipleChoiceNode.Choice>();

		// Token: 0x02000669 RID: 1641
		[Serializable]
		public class Choice
		{
			// Token: 0x06002A7E RID: 10878 RVA: 0x000A0B34 File Offset: 0x0009ED34
			public Choice()
			{
			}

			// Token: 0x06002A7F RID: 10879 RVA: 0x000A0B43 File Offset: 0x0009ED43
			public Choice(LocalizedStatement statement)
			{
				this.statement = statement;
			}

			// Token: 0x0400230F RID: 8975
			public bool isUnfolded = true;

			// Token: 0x04002310 RID: 8976
			public LocalizedStatement statement;

			// Token: 0x04002311 RID: 8977
			public ConditionTask condition;
		}
	}
}
