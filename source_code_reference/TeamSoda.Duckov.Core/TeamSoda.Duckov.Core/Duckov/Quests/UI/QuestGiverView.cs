using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Duckov.UI;
using Duckov.UI.Animations;
using Duckov.Utilities;
using SodaCraft.Localizations;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Duckov.Quests.UI
{
	// Token: 0x02000343 RID: 835
	public class QuestGiverView : View, ISingleSelectionMenu<QuestEntry>
	{
		// Token: 0x17000561 RID: 1377
		// (get) Token: 0x06001CC0 RID: 7360 RVA: 0x00067357 File Offset: 0x00065557
		public static QuestGiverView Instance
		{
			get
			{
				return View.GetViewInstance<QuestGiverView>();
			}
		}

		// Token: 0x17000562 RID: 1378
		// (get) Token: 0x06001CC1 RID: 7361 RVA: 0x0006735E File Offset: 0x0006555E
		public string BtnText_CompleteQuest
		{
			get
			{
				return this.btnText_CompleteQuest.ToPlainText();
			}
		}

		// Token: 0x17000563 RID: 1379
		// (get) Token: 0x06001CC2 RID: 7362 RVA: 0x0006736B File Offset: 0x0006556B
		public string BtnText_AcceptQuest
		{
			get
			{
				return this.btnText_AcceptQuest.ToPlainText();
			}
		}

		// Token: 0x17000564 RID: 1380
		// (get) Token: 0x06001CC3 RID: 7363 RVA: 0x00067378 File Offset: 0x00065578
		private PrefabPool<QuestEntry> EntryPool
		{
			get
			{
				if (this._entryPool == null)
				{
					this._entryPool = new PrefabPool<QuestEntry>(this.entryPrefab, this.questEntriesParent, delegate(QuestEntry e)
					{
						this.activeEntries.Add(e);
					}, delegate(QuestEntry e)
					{
						this.activeEntries.Remove(e);
					}, null, true, 10, 10000, null);
				}
				return this._entryPool;
			}
		}

		// Token: 0x06001CC4 RID: 7364 RVA: 0x000673CC File Offset: 0x000655CC
		protected override void OnOpen()
		{
			base.OnOpen();
			this.fadeGroup.Show();
			this.RefreshList();
			this.RefreshDetails();
			QuestManager.onQuestListsChanged += this.OnQuestListChanged;
			Quest.onQuestStatusChanged += this.OnQuestStatusChanged;
		}

		// Token: 0x06001CC5 RID: 7365 RVA: 0x00067418 File Offset: 0x00065618
		protected override void OnClose()
		{
			base.OnClose();
			this.fadeGroup.Hide();
			QuestManager.onQuestListsChanged -= this.OnQuestListChanged;
			Quest.onQuestStatusChanged -= this.OnQuestStatusChanged;
		}

		// Token: 0x06001CC6 RID: 7366 RVA: 0x0006744D File Offset: 0x0006564D
		private void OnDisable()
		{
			if (this.details != null)
			{
				this.details.Setup(null);
			}
		}

		// Token: 0x06001CC7 RID: 7367 RVA: 0x00067469 File Offset: 0x00065669
		private void OnQuestStatusChanged(Quest quest)
		{
			QuestEntry questEntry = this.selectedQuestEntry;
			if (quest == ((questEntry != null) ? questEntry.Target : null))
			{
				this.RefreshDetails();
			}
		}

		// Token: 0x06001CC8 RID: 7368 RVA: 0x0006748B File Offset: 0x0006568B
		protected override void Awake()
		{
			base.Awake();
			this.tabs.onSelectionChanged += this.OnTabChanged;
			this.btn_Interact.onClick.AddListener(new UnityAction(this.OnInteractButtonClicked));
		}

		// Token: 0x06001CC9 RID: 7369 RVA: 0x000674C8 File Offset: 0x000656C8
		private void OnInteractButtonClicked()
		{
			if (this.btnAcceptQuest)
			{
				Quest quest = this.details.Target;
				if (quest != null && QuestManager.IsQuestAvaliable(quest.ID))
				{
					QuestManager.Instance.ActivateQuest(quest.ID, new QuestGiverID?(this.target.ID));
					AudioManager.Post(this.sfx_AcceptQuest);
					return;
				}
			}
			else if (this.btnCompleteQuest)
			{
				Quest quest2 = this.details.Target;
				if (quest2 == null)
				{
					return;
				}
				if (quest2.TryComplete())
				{
					this.ShowCompleteUI(quest2);
					AudioManager.Post(this.sfx_CompleteQuest);
				}
			}
		}

		// Token: 0x06001CCA RID: 7370 RVA: 0x00067565 File Offset: 0x00065765
		private void ShowCompleteUI(Quest quest)
		{
			this.completeUITask = this.questCompletePanel.Show(quest);
		}

		// Token: 0x06001CCB RID: 7371 RVA: 0x00067579 File Offset: 0x00065779
		private void OnTabChanged(QuestGiverTabs tabs)
		{
			this.RefreshList();
			this.RefreshDetails();
		}

		// Token: 0x06001CCC RID: 7372 RVA: 0x00067587 File Offset: 0x00065787
		protected override void OnDestroy()
		{
			base.OnDestroy();
			QuestManager.onQuestListsChanged -= this.OnQuestListChanged;
		}

		// Token: 0x06001CCD RID: 7373 RVA: 0x000675A0 File Offset: 0x000657A0
		private void OnQuestListChanged(QuestManager manager)
		{
			this.RefreshList();
			this.SetSelection(null);
			this.RefreshDetails();
		}

		// Token: 0x06001CCE RID: 7374 RVA: 0x000675B6 File Offset: 0x000657B6
		public void Setup(QuestGiver target)
		{
			this.target = target;
			this.RefreshList();
		}

		// Token: 0x06001CCF RID: 7375 RVA: 0x000675C8 File Offset: 0x000657C8
		private void RefreshList()
		{
			QuestGiverView.<>c__DisplayClass42_0 CS$<>8__locals1 = new QuestGiverView.<>c__DisplayClass42_0();
			QuestGiverView.<>c__DisplayClass42_0 CS$<>8__locals2 = CS$<>8__locals1;
			QuestEntry questEntry = this.selectedQuestEntry;
			CS$<>8__locals2.keepQuest = ((questEntry != null) ? questEntry.Target : null);
			this.selectedQuestEntry = null;
			this.EntryPool.ReleaseAll();
			List<Quest> questsToShow = this.GetQuestsToShow();
			bool flag = questsToShow.Count > 0;
			this.entryPlaceHolder.SetActive(!flag);
			this.RefreshRedDots();
			if (!flag)
			{
				return;
			}
			foreach (Quest quest in questsToShow)
			{
				QuestEntry questEntry2 = this.EntryPool.Get(this.questEntriesParent);
				questEntry2.transform.SetAsLastSibling();
				questEntry2.SetMenu(this);
				questEntry2.Setup(quest);
			}
			QuestEntry questEntry3 = this.activeEntries.Find((QuestEntry e) => e.Target == CS$<>8__locals1.keepQuest);
			if (questEntry3 != null)
			{
				this.SetSelection(questEntry3);
				return;
			}
			this.SetSelection(null);
		}

		// Token: 0x06001CD0 RID: 7376 RVA: 0x000676C8 File Offset: 0x000658C8
		private void RefreshRedDots()
		{
			this.uninspectedAvaliableRedDot.SetActive(this.AnyUninspectedAvaliableQuest());
			this.activeRedDot.SetActive(this.AnyUninspectedActiveQuest());
		}

		// Token: 0x06001CD1 RID: 7377 RVA: 0x000676EC File Offset: 0x000658EC
		private bool AnyUninspectedActiveQuest()
		{
			return !(this.target == null) && QuestManager.AnyActiveQuestNeedsInspection(this.target.ID);
		}

		// Token: 0x06001CD2 RID: 7378 RVA: 0x00067710 File Offset: 0x00065910
		private bool AnyUninspectedAvaliableQuest()
		{
			if (this.target == null)
			{
				return false;
			}
			return this.target.GetAvaliableQuests().Any((Quest e) => e != null && e.NeedInspection);
		}

		// Token: 0x06001CD3 RID: 7379 RVA: 0x0006775C File Offset: 0x0006595C
		private List<Quest> GetQuestsToShow()
		{
			List<Quest> list = new List<Quest>();
			if (this.target == null)
			{
				return list;
			}
			QuestStatus status = this.tabs.GetStatus();
			switch (status)
			{
			case QuestStatus.None:
				return list;
			case (QuestStatus)1:
			case (QuestStatus)3:
				break;
			case QuestStatus.Avaliable:
				list.AddRange(this.target.GetAvaliableQuests());
				break;
			case QuestStatus.Active:
				list.AddRange(QuestManager.GetActiveQuestsFromGiver(this.target.ID));
				break;
			default:
				if (status == QuestStatus.Finished)
				{
					list.AddRange(QuestManager.GetHistoryQuestsFromGiver(this.target.ID));
				}
				break;
			}
			return list;
		}

		// Token: 0x06001CD4 RID: 7380 RVA: 0x000677F0 File Offset: 0x000659F0
		private void RefreshDetails()
		{
			QuestEntry questEntry = this.selectedQuestEntry;
			Quest quest = ((questEntry != null) ? questEntry.Target : null);
			this.details.Setup(quest);
			this.RefreshInteractButton();
			bool flag = quest && (QuestManager.IsQuestActive(quest) || quest.Complete);
			this.details.Interactable = flag;
			this.details.Refresh();
		}

		// Token: 0x06001CD5 RID: 7381 RVA: 0x00067858 File Offset: 0x00065A58
		private void RefreshInteractButton()
		{
			this.btnAcceptQuest = false;
			this.btnCompleteQuest = false;
			QuestEntry questEntry = this.selectedQuestEntry;
			Quest quest = ((questEntry != null) ? questEntry.Target : null);
			if (quest == null)
			{
				this.btn_Interact.gameObject.SetActive(false);
				return;
			}
			QuestStatus status = this.tabs.GetStatus();
			bool flag = false;
			switch (status)
			{
			case QuestStatus.None:
			case (QuestStatus)1:
			case (QuestStatus)3:
				break;
			case QuestStatus.Avaliable:
				flag = true;
				this.btn_Interact.interactable = true;
				this.btnImage.color = this.interactableBtnImageColor;
				this.btnText.text = this.BtnText_AcceptQuest;
				this.btnAcceptQuest = true;
				break;
			case QuestStatus.Active:
			{
				flag = true;
				bool flag2 = quest.AreTasksFinished();
				this.btn_Interact.interactable = flag2;
				this.btnImage.color = (flag2 ? this.interactableBtnImageColor : this.uninteractableBtnImageColor);
				this.btnText.text = this.BtnText_CompleteQuest;
				this.btnCompleteQuest = true;
				break;
			}
			default:
				if (status != QuestStatus.Finished)
				{
				}
				break;
			}
			this.btn_Interact.gameObject.SetActive(flag);
		}

		// Token: 0x06001CD6 RID: 7382 RVA: 0x00067968 File Offset: 0x00065B68
		public QuestEntry GetSelection()
		{
			return this.selectedQuestEntry;
		}

		// Token: 0x06001CD7 RID: 7383 RVA: 0x00067970 File Offset: 0x00065B70
		public bool SetSelection(QuestEntry selection)
		{
			this.selectedQuestEntry = selection;
			if (selection != null)
			{
				QuestManager.SetEverInspected(selection.Target.ID);
			}
			this.RefreshDetails();
			this.RefreshEntries();
			this.RefreshRedDots();
			return true;
		}

		// Token: 0x06001CD8 RID: 7384 RVA: 0x000679A8 File Offset: 0x00065BA8
		private void RefreshEntries()
		{
			foreach (QuestEntry questEntry in this.activeEntries)
			{
				questEntry.NotifyRefresh();
			}
		}

		// Token: 0x06001CD9 RID: 7385 RVA: 0x000679F8 File Offset: 0x00065BF8
		internal override void TryQuit()
		{
			if (this.questCompletePanel.isActiveAndEnabled)
			{
				this.questCompletePanel.Skip();
				return;
			}
			base.Close();
		}

		// Token: 0x040013F9 RID: 5113
		[SerializeField]
		private FadeGroup fadeGroup;

		// Token: 0x040013FA RID: 5114
		[SerializeField]
		private RectTransform questEntriesParent;

		// Token: 0x040013FB RID: 5115
		[SerializeField]
		private QuestCompletePanel questCompletePanel;

		// Token: 0x040013FC RID: 5116
		[SerializeField]
		private QuestGiverTabs tabs;

		// Token: 0x040013FD RID: 5117
		[SerializeField]
		private QuestEntry entryPrefab;

		// Token: 0x040013FE RID: 5118
		[SerializeField]
		private GameObject entryPlaceHolder;

		// Token: 0x040013FF RID: 5119
		[SerializeField]
		private QuestViewDetails details;

		// Token: 0x04001400 RID: 5120
		[SerializeField]
		private Button btn_Interact;

		// Token: 0x04001401 RID: 5121
		[SerializeField]
		private TextMeshProUGUI btnText;

		// Token: 0x04001402 RID: 5122
		[SerializeField]
		private Image btnImage;

		// Token: 0x04001403 RID: 5123
		[SerializeField]
		private string btnText_AcceptQuest = "接受任务";

		// Token: 0x04001404 RID: 5124
		[SerializeField]
		private string btnText_CompleteQuest = "完成任务";

		// Token: 0x04001405 RID: 5125
		[SerializeField]
		private Color interactableBtnImageColor = Color.green;

		// Token: 0x04001406 RID: 5126
		[SerializeField]
		private Color uninteractableBtnImageColor = Color.gray;

		// Token: 0x04001407 RID: 5127
		[SerializeField]
		private GameObject uninspectedAvaliableRedDot;

		// Token: 0x04001408 RID: 5128
		[SerializeField]
		private GameObject activeRedDot;

		// Token: 0x04001409 RID: 5129
		private string sfx_AcceptQuest = "UI/mission_accept";

		// Token: 0x0400140A RID: 5130
		private string sfx_CompleteQuest = "UI/mission_large";

		// Token: 0x0400140B RID: 5131
		private PrefabPool<QuestEntry> _entryPool;

		// Token: 0x0400140C RID: 5132
		private List<QuestEntry> activeEntries = new List<QuestEntry>();

		// Token: 0x0400140D RID: 5133
		private QuestGiver target;

		// Token: 0x0400140E RID: 5134
		private QuestEntry selectedQuestEntry;

		// Token: 0x0400140F RID: 5135
		private UniTask completeUITask;

		// Token: 0x04001410 RID: 5136
		private bool btnAcceptQuest;

		// Token: 0x04001411 RID: 5137
		private bool btnCompleteQuest;
	}
}
