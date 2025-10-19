using System;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion;
using UnityEngine;

namespace Duckov.Quests.Relations
{
	// Token: 0x0200035A RID: 858
	[CreateAssetMenu(menuName = "Quests/Relations")]
	public class QuestRelationGraph : Graph
	{
		// Token: 0x170005CA RID: 1482
		// (get) Token: 0x06001DFF RID: 7679 RVA: 0x00069FD2 File Offset: 0x000681D2
		public override Type baseNodeType
		{
			get
			{
				return typeof(QuestRelationNodeBase);
			}
		}

		// Token: 0x170005CB RID: 1483
		// (get) Token: 0x06001E00 RID: 7680 RVA: 0x00069FDE File Offset: 0x000681DE
		public override bool requiresAgent
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170005CC RID: 1484
		// (get) Token: 0x06001E01 RID: 7681 RVA: 0x00069FE1 File Offset: 0x000681E1
		public override bool requiresPrimeNode
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170005CD RID: 1485
		// (get) Token: 0x06001E02 RID: 7682 RVA: 0x00069FE4 File Offset: 0x000681E4
		public override bool isTree
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170005CE RID: 1486
		// (get) Token: 0x06001E03 RID: 7683 RVA: 0x00069FE7 File Offset: 0x000681E7
		public override PlanarDirection flowDirection
		{
			get
			{
				return PlanarDirection.Vertical;
			}
		}

		// Token: 0x170005CF RID: 1487
		// (get) Token: 0x06001E04 RID: 7684 RVA: 0x00069FEA File Offset: 0x000681EA
		public override bool allowBlackboardOverrides
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170005D0 RID: 1488
		// (get) Token: 0x06001E05 RID: 7685 RVA: 0x00069FED File Offset: 0x000681ED
		public override bool canAcceptVariableDrops
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06001E06 RID: 7686 RVA: 0x00069FF0 File Offset: 0x000681F0
		public QuestRelationNode GetNode(int questID)
		{
			return base.allNodes.Find(delegate(Node node)
			{
				QuestRelationNode questRelationNode = node as QuestRelationNode;
				return questRelationNode != null && questRelationNode.questID == questID;
			}) as QuestRelationNode;
		}

		// Token: 0x06001E07 RID: 7687 RVA: 0x0006A028 File Offset: 0x00068228
		public List<int> GetRequiredIDs(int targetID)
		{
			List<int> list = new List<int>();
			QuestRelationNode node = this.GetNode(targetID);
			if (node == null)
			{
				return list;
			}
			foreach (Connection connection in node.inConnections)
			{
				QuestRelationNode questRelationNode = connection.sourceNode as QuestRelationNode;
				if (questRelationNode != null)
				{
					int questID = questRelationNode.questID;
					list.Add(questID);
				}
				else
				{
					QuestRelationProxyNode questRelationProxyNode = connection.sourceNode as QuestRelationProxyNode;
					if (questRelationProxyNode != null)
					{
						int questID2 = questRelationProxyNode.questID;
						list.Add(questID2);
					}
				}
			}
			return list;
		}

		// Token: 0x06001E08 RID: 7688 RVA: 0x0006A0D0 File Offset: 0x000682D0
		protected override void OnGraphValidate()
		{
			this.CheckDuplicates();
		}

		// Token: 0x06001E09 RID: 7689 RVA: 0x0006A0D8 File Offset: 0x000682D8
		internal void CheckDuplicates()
		{
		}

		// Token: 0x0400148A RID: 5258
		public static int selectedQuestID = -1;
	}
}
