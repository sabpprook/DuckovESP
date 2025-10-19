using System;
using System.Collections.Generic;
using System.Linq;
using Duckov.Buildings;
using Duckov.Quests.UI;
using Duckov.Utilities;
using UnityEngine;

namespace Duckov.Quests
{
	// Token: 0x02000332 RID: 818
	public class QuestGiver : InteractableBase
	{
		// Token: 0x17000530 RID: 1328
		// (get) Token: 0x06001BE6 RID: 7142 RVA: 0x000650F9 File Offset: 0x000632F9
		private IEnumerable<Quest> PossibleQuests
		{
			get
			{
				if (this._possibleQuests == null && QuestManager.Instance != null)
				{
					this._possibleQuests = QuestManager.Instance.GetAllQuestsByQuestGiverID(this.questGiverID);
				}
				return this._possibleQuests;
			}
		}

		// Token: 0x17000531 RID: 1329
		// (get) Token: 0x06001BE7 RID: 7143 RVA: 0x0006512C File Offset: 0x0006332C
		public QuestGiverID ID
		{
			get
			{
				return this.questGiverID;
			}
		}

		// Token: 0x06001BE8 RID: 7144 RVA: 0x00065134 File Offset: 0x00063334
		protected override void Awake()
		{
			base.Awake();
			QuestManager.onQuestListsChanged += this.OnQuestListsChanged;
			BuildingManager.OnBuildingBuilt += this.OnBuildingBuilt;
			QuestManager.OnTaskFinishedEvent = (Action<Quest, Task>)Delegate.Combine(QuestManager.OnTaskFinishedEvent, new Action<Quest, Task>(this.OnTaskFinished));
			this.inspectionIndicator = global::UnityEngine.Object.Instantiate<GameObject>(GameplayDataSettings.Prefabs.QuestMarker);
			this.inspectionIndicator.transform.SetParent(base.transform);
			this.inspectionIndicator.transform.position = base.transform.TransformPoint(this.interactMarkerOffset + Vector3.up * 0.5f);
		}

		// Token: 0x06001BE9 RID: 7145 RVA: 0x000651E9 File Offset: 0x000633E9
		protected override void Start()
		{
			base.Start();
			this.RefreshInspectionIndicator();
		}

		// Token: 0x06001BEA RID: 7146 RVA: 0x000651F8 File Offset: 0x000633F8
		protected override void OnDestroy()
		{
			base.OnDestroy();
			QuestManager.onQuestListsChanged -= this.OnQuestListsChanged;
			BuildingManager.OnBuildingBuilt -= this.OnBuildingBuilt;
			QuestManager.OnTaskFinishedEvent = (Action<Quest, Task>)Delegate.Remove(QuestManager.OnTaskFinishedEvent, new Action<Quest, Task>(this.OnTaskFinished));
		}

		// Token: 0x06001BEB RID: 7147 RVA: 0x0006524D File Offset: 0x0006344D
		private void OnTaskFinished(Quest quest, Task task)
		{
			this.RefreshInspectionIndicator();
		}

		// Token: 0x06001BEC RID: 7148 RVA: 0x00065255 File Offset: 0x00063455
		private void OnBuildingBuilt(int buildingID)
		{
			this.RefreshInspectionIndicator();
		}

		// Token: 0x06001BED RID: 7149 RVA: 0x0006525D File Offset: 0x0006345D
		private bool AnyQuestNeedsInspection()
		{
			return QuestManager.GetActiveQuestsFromGiver(this.questGiverID).Any((Quest e) => e != null && e.NeedInspection);
		}

		// Token: 0x06001BEE RID: 7150 RVA: 0x00065290 File Offset: 0x00063490
		private bool AnyQuestAvaliable()
		{
			using (IEnumerator<Quest> enumerator = this.PossibleQuests.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (QuestManager.IsQuestAvaliable(enumerator.Current.ID))
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06001BEF RID: 7151 RVA: 0x000652E8 File Offset: 0x000634E8
		private void OnQuestListsChanged(QuestManager manager)
		{
			this.RefreshInspectionIndicator();
		}

		// Token: 0x06001BF0 RID: 7152 RVA: 0x000652F0 File Offset: 0x000634F0
		private void RefreshInspectionIndicator()
		{
			if (this.inspectionIndicator)
			{
				bool flag = this.AnyQuestNeedsInspection();
				bool flag2 = this.AnyQuestAvaliable();
				bool flag3 = flag || flag2;
				this.inspectionIndicator.gameObject.SetActive(flag3);
			}
		}

		// Token: 0x06001BF1 RID: 7153 RVA: 0x0006532B File Offset: 0x0006352B
		public void ActivateQuest(Quest quest)
		{
			QuestManager.Instance.ActivateQuest(quest.ID, new QuestGiverID?(this.questGiverID));
		}

		// Token: 0x06001BF2 RID: 7154 RVA: 0x00065348 File Offset: 0x00063548
		internal List<Quest> GetAvaliableQuests()
		{
			List<Quest> list = new List<Quest>();
			foreach (Quest quest in this.PossibleQuests)
			{
				if (QuestManager.IsQuestAvaliable(quest.ID))
				{
					list.Add(quest);
				}
			}
			return list;
		}

		// Token: 0x06001BF3 RID: 7155 RVA: 0x000653AC File Offset: 0x000635AC
		protected override void OnInteractStart(CharacterMainControl interactCharacter)
		{
			base.OnInteractStart(interactCharacter);
			QuestGiverView instance = QuestGiverView.Instance;
			if (instance == null)
			{
				base.StopInteract();
				return;
			}
			instance.Setup(this);
			instance.Open(null);
		}

		// Token: 0x06001BF4 RID: 7156 RVA: 0x000653E4 File Offset: 0x000635E4
		protected override void OnInteractStop()
		{
			base.OnInteractStop();
			if (QuestGiverView.Instance && QuestGiverView.Instance.open)
			{
				QuestGiverView instance = QuestGiverView.Instance;
				if (instance == null)
				{
					return;
				}
				instance.Close();
			}
		}

		// Token: 0x06001BF5 RID: 7157 RVA: 0x00065413 File Offset: 0x00063613
		protected override void OnUpdate(CharacterMainControl _interactCharacter, float deltaTime)
		{
			base.OnUpdate(_interactCharacter, deltaTime);
			if (!QuestGiverView.Instance || !QuestGiverView.Instance.open)
			{
				base.StopInteract();
			}
		}

		// Token: 0x040013A1 RID: 5025
		[SerializeField]
		private QuestGiverID questGiverID;

		// Token: 0x040013A2 RID: 5026
		private GameObject inspectionIndicator;

		// Token: 0x040013A3 RID: 5027
		private IEnumerable<Quest> _possibleQuests;

		// Token: 0x040013A4 RID: 5028
		private List<Quest> avaliableQuests = new List<Quest>();
	}
}
