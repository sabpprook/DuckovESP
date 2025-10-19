using System;
using System.Collections.Generic;
using Duckov.Utilities;
using NodeCanvas.Framework;

namespace Duckov.Quests.Relations
{
	// Token: 0x0200035C RID: 860
	public class QuestRelationNode : QuestRelationNodeBase
	{
		// Token: 0x170005D8 RID: 1496
		// (get) Token: 0x06001E14 RID: 7700 RVA: 0x0006A112 File Offset: 0x00068312
		private static QuestCollection QuestCollection
		{
			get
			{
				if (QuestRelationNode._questCollection == null)
				{
					QuestRelationNode._questCollection = GameplayDataSettings.QuestCollection;
				}
				return QuestRelationNode._questCollection;
			}
		}

		// Token: 0x06001E15 RID: 7701 RVA: 0x0006A130 File Offset: 0x00068330
		private void SelectQuest()
		{
		}

		// Token: 0x06001E16 RID: 7702 RVA: 0x0006A134 File Offset: 0x00068334
		public List<int> GetParents()
		{
			List<int> list = new List<int>();
			foreach (Connection connection in base.inConnections)
			{
				QuestRelationNode questRelationNode = connection.sourceNode as QuestRelationNode;
				if (questRelationNode != null)
				{
					list.Add(questRelationNode.questID);
				}
				else
				{
					QuestRelationProxyNode questRelationProxyNode = connection.sourceNode as QuestRelationProxyNode;
					if (questRelationProxyNode != null)
					{
						list.Add(questRelationProxyNode.questID);
					}
				}
			}
			return list;
		}

		// Token: 0x06001E17 RID: 7703 RVA: 0x0006A1C4 File Offset: 0x000683C4
		public List<int> GetChildren()
		{
			List<int> list = new List<int>();
			foreach (Connection connection in base.outConnections)
			{
				QuestRelationNode questRelationNode = connection.sourceNode as QuestRelationNode;
				if (questRelationNode != null)
				{
					list.Add(questRelationNode.questID);
				}
			}
			return list;
		}

		// Token: 0x0400148B RID: 5259
		public int questID;

		// Token: 0x0400148C RID: 5260
		private static QuestCollection _questCollection;

		// Token: 0x0400148D RID: 5261
		internal bool isDuplicate;
	}
}
