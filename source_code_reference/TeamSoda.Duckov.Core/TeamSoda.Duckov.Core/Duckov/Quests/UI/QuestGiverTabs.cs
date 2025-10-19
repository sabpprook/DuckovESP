using System;
using System.Collections.Generic;
using UnityEngine;

namespace Duckov.Quests.UI
{
	// Token: 0x02000346 RID: 838
	public class QuestGiverTabs : MonoBehaviour, ISingleSelectionMenu<QuestGiverTabButton>
	{
		// Token: 0x140000CC RID: 204
		// (add) Token: 0x06001CE4 RID: 7396 RVA: 0x00067B2C File Offset: 0x00065D2C
		// (remove) Token: 0x06001CE5 RID: 7397 RVA: 0x00067B64 File Offset: 0x00065D64
		public event Action<QuestGiverTabs> onSelectionChanged;

		// Token: 0x06001CE6 RID: 7398 RVA: 0x00067B99 File Offset: 0x00065D99
		public QuestGiverTabButton GetSelection()
		{
			return this.selectedButton;
		}

		// Token: 0x06001CE7 RID: 7399 RVA: 0x00067BA1 File Offset: 0x00065DA1
		public QuestStatus GetStatus()
		{
			if (!this.initialized)
			{
				this.Initialize();
			}
			return this.selectedButton.Status;
		}

		// Token: 0x06001CE8 RID: 7400 RVA: 0x00067BBC File Offset: 0x00065DBC
		public bool SetSelection(QuestGiverTabButton selection)
		{
			this.selectedButton = selection;
			this.RefreshAllButtons();
			Action<QuestGiverTabs> action = this.onSelectionChanged;
			if (action != null)
			{
				action(this);
			}
			return true;
		}

		// Token: 0x06001CE9 RID: 7401 RVA: 0x00067BE0 File Offset: 0x00065DE0
		private void Initialize()
		{
			foreach (QuestGiverTabButton questGiverTabButton in this.buttons)
			{
				questGiverTabButton.Setup(this);
			}
			if (this.buttons.Count > 0)
			{
				this.SetSelection(this.buttons[0]);
			}
			this.initialized = true;
		}

		// Token: 0x06001CEA RID: 7402 RVA: 0x00067C5C File Offset: 0x00065E5C
		private void Awake()
		{
			if (!this.initialized)
			{
				this.Initialize();
			}
		}

		// Token: 0x06001CEB RID: 7403 RVA: 0x00067C6C File Offset: 0x00065E6C
		private void RefreshAllButtons()
		{
			foreach (QuestGiverTabButton questGiverTabButton in this.buttons)
			{
				questGiverTabButton.Refresh();
			}
		}

		// Token: 0x0400141B RID: 5147
		[SerializeField]
		private List<QuestGiverTabButton> buttons = new List<QuestGiverTabButton>();

		// Token: 0x0400141C RID: 5148
		private QuestGiverTabButton selectedButton;

		// Token: 0x0400141E RID: 5150
		private bool initialized;
	}
}
