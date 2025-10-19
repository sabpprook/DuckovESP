using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Duckov.Quests.UI
{
	// Token: 0x02000344 RID: 836
	public class QuestGiverTabButton : MonoBehaviour
	{
		// Token: 0x17000565 RID: 1381
		// (get) Token: 0x06001CDD RID: 7389 RVA: 0x00067A99 File Offset: 0x00065C99
		public QuestStatus Status
		{
			get
			{
				return this.status;
			}
		}

		// Token: 0x06001CDE RID: 7390 RVA: 0x00067AA1 File Offset: 0x00065CA1
		internal void Setup(QuestGiverTabs questGiverTabs)
		{
			this.master = questGiverTabs;
			this.Refresh();
		}

		// Token: 0x06001CDF RID: 7391 RVA: 0x00067AB0 File Offset: 0x00065CB0
		private void Awake()
		{
			this.button.onClick.AddListener(new UnityAction(this.OnClick));
		}

		// Token: 0x06001CE0 RID: 7392 RVA: 0x00067ACE File Offset: 0x00065CCE
		private void OnClick()
		{
			if (this.master == null)
			{
				return;
			}
			this.master.SetSelection(this);
		}

		// Token: 0x17000566 RID: 1382
		// (get) Token: 0x06001CE1 RID: 7393 RVA: 0x00067AEC File Offset: 0x00065CEC
		private bool Selected
		{
			get
			{
				return !(this.master == null) && this.master.GetSelection() == this;
			}
		}

		// Token: 0x06001CE2 RID: 7394 RVA: 0x00067B0F File Offset: 0x00065D0F
		internal void Refresh()
		{
			this.selectedIndicator.SetActive(this.Selected);
		}

		// Token: 0x04001412 RID: 5138
		[SerializeField]
		private Button button;

		// Token: 0x04001413 RID: 5139
		[SerializeField]
		private GameObject selectedIndicator;

		// Token: 0x04001414 RID: 5140
		[SerializeField]
		private QuestStatus status;

		// Token: 0x04001415 RID: 5141
		private QuestGiverTabs master;
	}
}
