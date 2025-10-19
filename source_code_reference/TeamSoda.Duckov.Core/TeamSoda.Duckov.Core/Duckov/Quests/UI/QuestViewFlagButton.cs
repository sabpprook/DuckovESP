using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Duckov.Quests.UI
{
	// Token: 0x02000349 RID: 841
	public class QuestViewFlagButton : MonoBehaviour
	{
		// Token: 0x06001D1C RID: 7452 RVA: 0x000684EF File Offset: 0x000666EF
		private void Awake()
		{
			this.button.onClick.AddListener(new UnityAction(this.OnButtonClicked));
			this.master.onShowingContentChanged += this.OnMasterShowingContentChanged;
			this.Refresh();
		}

		// Token: 0x06001D1D RID: 7453 RVA: 0x0006852A File Offset: 0x0006672A
		private void OnButtonClicked()
		{
			this.master.SetShowingContent(this.content);
		}

		// Token: 0x06001D1E RID: 7454 RVA: 0x0006853D File Offset: 0x0006673D
		private void OnMasterShowingContentChanged(QuestView view, QuestView.ShowContent content)
		{
			this.Refresh();
		}

		// Token: 0x06001D1F RID: 7455 RVA: 0x00068548 File Offset: 0x00066748
		private void Refresh()
		{
			bool flag = this.master.ShowingContentType == this.content;
			this.selectionIndicator.SetActive(flag);
		}

		// Token: 0x0400143B RID: 5179
		[SerializeField]
		private QuestView master;

		// Token: 0x0400143C RID: 5180
		[SerializeField]
		private Button button;

		// Token: 0x0400143D RID: 5181
		[SerializeField]
		private QuestView.ShowContent content;

		// Token: 0x0400143E RID: 5182
		[SerializeField]
		private GameObject selectionIndicator;
	}
}
