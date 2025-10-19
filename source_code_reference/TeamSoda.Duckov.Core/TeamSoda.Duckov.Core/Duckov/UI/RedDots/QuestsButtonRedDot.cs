using System;
using Duckov.Quests;
using UnityEngine;

namespace Duckov.UI.RedDots
{
	// Token: 0x020003E4 RID: 996
	public class QuestsButtonRedDot : MonoBehaviour
	{
		// Token: 0x060023FF RID: 9215 RVA: 0x0007D58F File Offset: 0x0007B78F
		private void Awake()
		{
			Quest.onQuestNeedInspectionChanged += this.OnQuestNeedInspectionChanged;
		}

		// Token: 0x06002400 RID: 9216 RVA: 0x0007D5A2 File Offset: 0x0007B7A2
		private void OnDestroy()
		{
			Quest.onQuestNeedInspectionChanged -= this.OnQuestNeedInspectionChanged;
		}

		// Token: 0x06002401 RID: 9217 RVA: 0x0007D5B5 File Offset: 0x0007B7B5
		private void OnQuestNeedInspectionChanged(Quest quest)
		{
			this.Refresh();
		}

		// Token: 0x06002402 RID: 9218 RVA: 0x0007D5BD File Offset: 0x0007B7BD
		private void Start()
		{
			this.Refresh();
		}

		// Token: 0x06002403 RID: 9219 RVA: 0x0007D5C5 File Offset: 0x0007B7C5
		private void Refresh()
		{
			this.dot.SetActive(QuestManager.AnyQuestNeedsInspection);
		}

		// Token: 0x04001878 RID: 6264
		public GameObject dot;
	}
}
