using System;
using System.Collections.Generic;
using Duckov.UI;
using Duckov.UI.Animations;
using Duckov.Utilities;
using UnityEngine;

namespace Duckov.Quests.UI
{
	// Token: 0x02000347 RID: 839
	public class QuestView : View, ISingleSelectionMenu<QuestEntry>
	{
		// Token: 0x17000567 RID: 1383
		// (get) Token: 0x06001CED RID: 7405 RVA: 0x00067CCF File Offset: 0x00065ECF
		public static QuestView Instance
		{
			get
			{
				return View.GetViewInstance<QuestView>();
			}
		}

		// Token: 0x17000568 RID: 1384
		// (get) Token: 0x06001CEE RID: 7406 RVA: 0x00067CD6 File Offset: 0x00065ED6
		public QuestView.ShowContent ShowingContentType
		{
			get
			{
				return this.showingContentType;
			}
		}

		// Token: 0x17000569 RID: 1385
		// (get) Token: 0x06001CEF RID: 7407 RVA: 0x00067CE0 File Offset: 0x00065EE0
		public IList<Quest> ShowingContent
		{
			get
			{
				if (this.target == null)
				{
					return null;
				}
				QuestView.ShowContent showContent = this.showingContentType;
				if (showContent == QuestView.ShowContent.Active)
				{
					return this.target.ActiveQuests;
				}
				if (showContent != QuestView.ShowContent.History)
				{
					return null;
				}
				return this.target.HistoryQuests;
			}
		}

		// Token: 0x1700056A RID: 1386
		// (get) Token: 0x06001CF0 RID: 7408 RVA: 0x00067D28 File Offset: 0x00065F28
		private PrefabPool<QuestEntry> QuestEntryPool
		{
			get
			{
				if (this._questEntryPool == null)
				{
					this._questEntryPool = new PrefabPool<QuestEntry>(this.questEntry, this.questEntryParent, delegate(QuestEntry e)
					{
						this.activeEntries.Add(e);
						e.SetMenu(this);
					}, delegate(QuestEntry e)
					{
						this.activeEntries.Remove(e);
					}, null, true, 10, 10000, null);
				}
				return this._questEntryPool;
			}
		}

		// Token: 0x1700056B RID: 1387
		// (get) Token: 0x06001CF1 RID: 7409 RVA: 0x00067D7C File Offset: 0x00065F7C
		private QuestEntry SelectedQuestEntry
		{
			get
			{
				return this.selectedQuestEntry;
			}
		}

		// Token: 0x1700056C RID: 1388
		// (get) Token: 0x06001CF2 RID: 7410 RVA: 0x00067D84 File Offset: 0x00065F84
		public Quest SelectedQuest
		{
			get
			{
				QuestEntry questEntry = this.selectedQuestEntry;
				if (questEntry == null)
				{
					return null;
				}
				return questEntry.Target;
			}
		}

		// Token: 0x140000CD RID: 205
		// (add) Token: 0x06001CF3 RID: 7411 RVA: 0x00067D98 File Offset: 0x00065F98
		// (remove) Token: 0x06001CF4 RID: 7412 RVA: 0x00067DD0 File Offset: 0x00065FD0
		internal event Action<QuestView, QuestView.ShowContent> onShowingContentChanged;

		// Token: 0x140000CE RID: 206
		// (add) Token: 0x06001CF5 RID: 7413 RVA: 0x00067E08 File Offset: 0x00066008
		// (remove) Token: 0x06001CF6 RID: 7414 RVA: 0x00067E40 File Offset: 0x00066040
		internal event Action<QuestView, QuestEntry> onSelectedEntryChanged;

		// Token: 0x06001CF7 RID: 7415 RVA: 0x00067E75 File Offset: 0x00066075
		public void Setup()
		{
			this.Setup(QuestManager.Instance);
		}

		// Token: 0x06001CF8 RID: 7416 RVA: 0x00067E84 File Offset: 0x00066084
		private void Setup(QuestManager target)
		{
			this.target = target;
			Quest oldSelection = this.SelectedQuest;
			this.RefreshEntryList();
			QuestEntry questEntry = this.activeEntries.Find((QuestEntry e) => e.Target == oldSelection);
			if (questEntry != null)
			{
				this.SetSelection(questEntry);
			}
			else
			{
				this.SetSelection(null);
			}
			this.RefreshDetails();
		}

		// Token: 0x06001CF9 RID: 7417 RVA: 0x00067EE9 File Offset: 0x000660E9
		public static void Show()
		{
			QuestView instance = QuestView.Instance;
			if (instance == null)
			{
				return;
			}
			instance.Open(null);
		}

		// Token: 0x06001CFA RID: 7418 RVA: 0x00067EFB File Offset: 0x000660FB
		protected override void OnOpen()
		{
			base.OnOpen();
			this.Setup();
			this.fadeGroup.Show();
		}

		// Token: 0x06001CFB RID: 7419 RVA: 0x00067F14 File Offset: 0x00066114
		protected override void OnClose()
		{
			base.OnClose();
			this.fadeGroup.Hide();
		}

		// Token: 0x06001CFC RID: 7420 RVA: 0x00067F27 File Offset: 0x00066127
		protected override void Awake()
		{
			base.Awake();
		}

		// Token: 0x06001CFD RID: 7421 RVA: 0x00067F2F File Offset: 0x0006612F
		private void OnEnable()
		{
			this.RegisterStaticEvents();
			this.Setup(QuestManager.Instance);
		}

		// Token: 0x06001CFE RID: 7422 RVA: 0x00067F42 File Offset: 0x00066142
		private void OnDisable()
		{
			if (this.details != null)
			{
				this.details.Setup(null);
			}
			this.UnregisterStaticEvents();
		}

		// Token: 0x06001CFF RID: 7423 RVA: 0x00067F64 File Offset: 0x00066164
		private void RegisterStaticEvents()
		{
			QuestManager.onQuestListsChanged += this.Setup;
		}

		// Token: 0x06001D00 RID: 7424 RVA: 0x00067F77 File Offset: 0x00066177
		private void UnregisterStaticEvents()
		{
			QuestManager.onQuestListsChanged -= this.Setup;
		}

		// Token: 0x06001D01 RID: 7425 RVA: 0x00067F8C File Offset: 0x0006618C
		private void RefreshEntryList()
		{
			this.QuestEntryPool.ReleaseAll();
			bool flag = this.target != null && this.ShowingContent != null && this.ShowingContent.Count > 0;
			this.entryListPlaceHolder.SetActive(!flag);
			if (!flag)
			{
				return;
			}
			foreach (Quest quest in this.ShowingContent)
			{
				QuestEntry questEntry = this.QuestEntryPool.Get(this.questEntryParent);
				questEntry.Setup(quest);
				questEntry.transform.SetAsLastSibling();
			}
		}

		// Token: 0x06001D02 RID: 7426 RVA: 0x0006803C File Offset: 0x0006623C
		private void RefreshDetails()
		{
			this.details.Setup(this.SelectedQuest);
		}

		// Token: 0x06001D03 RID: 7427 RVA: 0x00068050 File Offset: 0x00066250
		public void SetShowingContent(QuestView.ShowContent flags)
		{
			this.showingContentType = flags;
			this.RefreshEntryList();
			List<QuestEntry> list = this.activeEntries;
			if (list != null && list.Count > 0)
			{
				this.SetSelection(this.activeEntries[0]);
			}
			else
			{
				this.SetSelection(null);
			}
			this.RefreshDetails();
			foreach (QuestEntry questEntry in this.activeEntries)
			{
				questEntry.NotifyRefresh();
			}
			Action<QuestView, QuestView.ShowContent> action = this.onShowingContentChanged;
			if (action == null)
			{
				return;
			}
			action(this, flags);
		}

		// Token: 0x06001D04 RID: 7428 RVA: 0x000680FC File Offset: 0x000662FC
		public void ShowActiveQuests()
		{
			this.SetShowingContent(QuestView.ShowContent.Active);
		}

		// Token: 0x06001D05 RID: 7429 RVA: 0x00068105 File Offset: 0x00066305
		public void ShowHistoryQuests()
		{
			this.SetShowingContent(QuestView.ShowContent.History);
		}

		// Token: 0x06001D06 RID: 7430 RVA: 0x0006810E File Offset: 0x0006630E
		public QuestEntry GetSelection()
		{
			return this.selectedQuestEntry;
		}

		// Token: 0x06001D07 RID: 7431 RVA: 0x00068118 File Offset: 0x00066318
		public bool SetSelection(QuestEntry selection)
		{
			this.selectedQuestEntry = selection;
			Action<QuestView, QuestEntry> action = this.onSelectedEntryChanged;
			if (action != null)
			{
				action(this, this.selectedQuestEntry);
			}
			foreach (QuestEntry questEntry in this.activeEntries)
			{
				questEntry.NotifyRefresh();
			}
			this.RefreshDetails();
			return true;
		}

		// Token: 0x0400141F RID: 5151
		[SerializeField]
		private QuestEntry questEntry;

		// Token: 0x04001420 RID: 5152
		[SerializeField]
		private Transform questEntryParent;

		// Token: 0x04001421 RID: 5153
		[SerializeField]
		private GameObject entryListPlaceHolder;

		// Token: 0x04001422 RID: 5154
		[SerializeField]
		private QuestViewDetails details;

		// Token: 0x04001423 RID: 5155
		[SerializeField]
		private FadeGroup fadeGroup;

		// Token: 0x04001424 RID: 5156
		private QuestManager target;

		// Token: 0x04001425 RID: 5157
		[SerializeField]
		private QuestView.ShowContent showingContentType;

		// Token: 0x04001426 RID: 5158
		private PrefabPool<QuestEntry> _questEntryPool;

		// Token: 0x04001427 RID: 5159
		private List<QuestEntry> activeEntries = new List<QuestEntry>();

		// Token: 0x04001428 RID: 5160
		private QuestEntry selectedQuestEntry;

		// Token: 0x020005FD RID: 1533
		[Flags]
		public enum ShowContent
		{
			// Token: 0x04002128 RID: 8488
			Active = 1,
			// Token: 0x04002129 RID: 8489
			History = 2
		}
	}
}
