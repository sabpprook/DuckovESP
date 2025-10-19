using System;
using Duckov.Utilities;

namespace Duckov.Quests.Relations
{
	// Token: 0x0200035D RID: 861
	public class QuestRelationProxyNode : QuestRelationNodeBase
	{
		// Token: 0x170005D9 RID: 1497
		// (get) Token: 0x06001E19 RID: 7705 RVA: 0x0006A238 File Offset: 0x00068438
		public override int maxInConnections
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x170005DA RID: 1498
		// (get) Token: 0x06001E1A RID: 7706 RVA: 0x0006A23B File Offset: 0x0006843B
		private static QuestCollection QuestCollection
		{
			get
			{
				if (QuestRelationProxyNode._questCollection == null)
				{
					QuestRelationProxyNode._questCollection = GameplayDataSettings.QuestCollection;
				}
				return QuestRelationProxyNode._questCollection;
			}
		}

		// Token: 0x06001E1B RID: 7707 RVA: 0x0006A259 File Offset: 0x00068459
		private void SelectQuest()
		{
		}

		// Token: 0x0400148E RID: 5262
		private static QuestCollection _questCollection;

		// Token: 0x0400148F RID: 5263
		public int questID;
	}
}
