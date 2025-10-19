using System;
using System.Collections.Generic;
using System.Linq;
using Duckov.Quests.Relations;
using Duckov.Quests.Tasks;
using Duckov.UI;
using Duckov.Utilities;
using Saves;
using Sirenix.Utilities;
using SodaCraft.Localizations;
using SodaCraft.StringUtilities;
using UnityEngine;

namespace Duckov.Quests
{
	// Token: 0x02000334 RID: 820
	public class QuestManager : MonoBehaviour, ISaveDataProvider, INeedInspection
	{
		// Token: 0x17000532 RID: 1330
		// (get) Token: 0x06001BF7 RID: 7159 RVA: 0x0006544E File Offset: 0x0006364E
		public string TaskFinishNotificationFormat
		{
			get
			{
				return this.taskFinishNotificationFormatKey.ToPlainText();
			}
		}

		// Token: 0x17000533 RID: 1331
		// (get) Token: 0x06001BF8 RID: 7160 RVA: 0x0006545B File Offset: 0x0006365B
		public static QuestManager Instance
		{
			get
			{
				return QuestManager.instance;
			}
		}

		// Token: 0x17000534 RID: 1332
		// (get) Token: 0x06001BF9 RID: 7161 RVA: 0x00065462 File Offset: 0x00063662
		public static bool AnyQuestNeedsInspection
		{
			get
			{
				return !(QuestManager.Instance == null) && QuestManager.Instance.NeedInspection;
			}
		}

		// Token: 0x17000535 RID: 1333
		// (get) Token: 0x06001BFA RID: 7162 RVA: 0x0006547D File Offset: 0x0006367D
		public bool NeedInspection
		{
			get
			{
				if (this.activeQuests == null)
				{
					return false;
				}
				return this.activeQuests.Any((Quest e) => e != null && e.NeedInspection);
			}
		}

		// Token: 0x06001BFB RID: 7163 RVA: 0x000654B3 File Offset: 0x000636B3
		public static IEnumerable<int> GetAllRequiredItems()
		{
			if (QuestManager.Instance == null)
			{
				yield break;
			}
			List<Quest> list = QuestManager.Instance.ActiveQuests;
			foreach (Quest quest in list)
			{
				if (quest.tasks != null)
				{
					foreach (Task task in quest.tasks)
					{
						SubmitItems submitItems = task as SubmitItems;
						if (submitItems != null && !submitItems.IsFinished())
						{
							yield return submitItems.ItemTypeID;
						}
					}
					List<Task>.Enumerator enumerator2 = default(List<Task>.Enumerator);
				}
			}
			List<Quest>.Enumerator enumerator = default(List<Quest>.Enumerator);
			yield break;
			yield break;
		}

		// Token: 0x06001BFC RID: 7164 RVA: 0x000654BC File Offset: 0x000636BC
		public static bool AnyActiveQuestNeedsInspection(QuestGiverID giverID)
		{
			return !(QuestManager.Instance == null) && QuestManager.Instance.activeQuests != null && QuestManager.Instance.activeQuests.Any((Quest e) => e != null && e.QuestGiverID == giverID && e.NeedInspection);
		}

		// Token: 0x17000536 RID: 1334
		// (get) Token: 0x06001BFD RID: 7165 RVA: 0x0006550E File Offset: 0x0006370E
		private ICollection<Quest> QuestPrefabCollection
		{
			get
			{
				return GameplayDataSettings.QuestCollection;
			}
		}

		// Token: 0x17000537 RID: 1335
		// (get) Token: 0x06001BFE RID: 7166 RVA: 0x00065515 File Offset: 0x00063715
		private QuestRelationGraph QuestRelation
		{
			get
			{
				return GameplayDataSettings.QuestRelation;
			}
		}

		// Token: 0x17000538 RID: 1336
		// (get) Token: 0x06001BFF RID: 7167 RVA: 0x0006551C File Offset: 0x0006371C
		public List<Quest> ActiveQuests
		{
			get
			{
				this.activeQuests.Sort(delegate(Quest a, Quest b)
				{
					int num = (a.AreTasksFinished() ? 1 : 0);
					return (b.AreTasksFinished() ? 1 : 0) - num;
				});
				return this.activeQuests;
			}
		}

		// Token: 0x17000539 RID: 1337
		// (get) Token: 0x06001C00 RID: 7168 RVA: 0x0006554E File Offset: 0x0006374E
		public List<Quest> HistoryQuests
		{
			get
			{
				return this.historyQuests;
			}
		}

		// Token: 0x1700053A RID: 1338
		// (get) Token: 0x06001C01 RID: 7169 RVA: 0x00065556 File Offset: 0x00063756
		public List<int> EverInspectedQuest
		{
			get
			{
				return this.everInspectedQuest;
			}
		}

		// Token: 0x140000C7 RID: 199
		// (add) Token: 0x06001C02 RID: 7170 RVA: 0x00065560 File Offset: 0x00063760
		// (remove) Token: 0x06001C03 RID: 7171 RVA: 0x00065594 File Offset: 0x00063794
		public static event Action<QuestManager> onQuestListsChanged;

		// Token: 0x06001C04 RID: 7172 RVA: 0x000655C8 File Offset: 0x000637C8
		public object GenerateSaveData()
		{
			QuestManager.SaveData saveData = default(QuestManager.SaveData);
			saveData.activeQuestsData = new List<object>();
			saveData.historyQuestsData = new List<object>();
			saveData.everInspectedQuest = new List<int>();
			foreach (Quest quest in this.ActiveQuests)
			{
				saveData.activeQuestsData.Add(quest.GenerateSaveData());
			}
			foreach (Quest quest2 in this.HistoryQuests)
			{
				saveData.historyQuestsData.Add(quest2.GenerateSaveData());
			}
			saveData.everInspectedQuest.AddRange(this.EverInspectedQuest);
			return saveData;
		}

		// Token: 0x06001C05 RID: 7173 RVA: 0x000656B4 File Offset: 0x000638B4
		public void SetupSaveData(object dataObj)
		{
			if (dataObj is QuestManager.SaveData)
			{
				QuestManager.SaveData saveData = (QuestManager.SaveData)dataObj;
				if (saveData.activeQuestsData != null)
				{
					foreach (object obj in saveData.activeQuestsData)
					{
						int id = ((Quest.SaveData)obj).id;
						Quest questPrefab = this.GetQuestPrefab(id);
						if (questPrefab == null)
						{
							Debug.LogError(string.Format("未找到Quest {0}", id));
						}
						else
						{
							Quest quest = global::UnityEngine.Object.Instantiate<Quest>(questPrefab, base.transform);
							quest.SetupSaveData(obj);
							this.activeQuests.Add(quest);
						}
					}
				}
				if (saveData.historyQuestsData != null)
				{
					foreach (object obj2 in saveData.historyQuestsData)
					{
						int id2 = ((Quest.SaveData)obj2).id;
						Quest questPrefab2 = this.GetQuestPrefab(id2);
						if (questPrefab2 == null)
						{
							Debug.LogError(string.Format("未找到Quest {0}", id2));
						}
						else
						{
							Quest quest2 = global::UnityEngine.Object.Instantiate<Quest>(questPrefab2, base.transform);
							quest2.SetupSaveData(obj2);
							this.historyQuests.Add(quest2);
						}
					}
				}
				if (saveData.everInspectedQuest != null)
				{
					this.EverInspectedQuest.Clear();
					this.EverInspectedQuest.AddRange(saveData.everInspectedQuest);
				}
				return;
			}
			Debug.LogError("错误的数据类型");
		}

		// Token: 0x06001C06 RID: 7174 RVA: 0x0006584C File Offset: 0x00063A4C
		private void Save()
		{
			SavesSystem.Save<object>("Quest", "Data", this.GenerateSaveData());
		}

		// Token: 0x06001C07 RID: 7175 RVA: 0x00065864 File Offset: 0x00063A64
		private void Load()
		{
			try
			{
				QuestManager.SaveData saveData = SavesSystem.Load<QuestManager.SaveData>("Quest", "Data");
				this.SetupSaveData(saveData);
			}
			catch
			{
				Debug.LogError("在加载Quest存档时出现了错误");
			}
		}

		// Token: 0x06001C08 RID: 7176 RVA: 0x000658AC File Offset: 0x00063AAC
		public IEnumerable<Quest> GetAllQuestsByQuestGiverID(QuestGiverID questGiverID)
		{
			return this.QuestPrefabCollection.Where((Quest e) => e != null && e.QuestGiverID == questGiverID);
		}

		// Token: 0x06001C09 RID: 7177 RVA: 0x000658E0 File Offset: 0x00063AE0
		private Quest GetQuestPrefab(int id)
		{
			return this.QuestPrefabCollection.FirstOrDefault((Quest q) => q != null && q.ID == id);
		}

		// Token: 0x06001C0A RID: 7178 RVA: 0x00065911 File Offset: 0x00063B11
		private void Awake()
		{
			if (QuestManager.instance == null)
			{
				QuestManager.instance = this;
			}
			if (QuestManager.instance != this)
			{
				Debug.LogError("侦测到多个QuestManager！");
				return;
			}
			this.RegisterEvents();
			this.Load();
		}

		// Token: 0x06001C0B RID: 7179 RVA: 0x0006594A File Offset: 0x00063B4A
		private void OnDestroy()
		{
			this.UnregisterEvents();
		}

		// Token: 0x06001C0C RID: 7180 RVA: 0x00065954 File Offset: 0x00063B54
		private void RegisterEvents()
		{
			Quest.onQuestStatusChanged += this.OnQuestStatusChanged;
			Quest.onQuestCompleted += this.OnQuestCompleted;
			SavesSystem.OnCollectSaveData += this.Save;
			SavesSystem.OnSetFile += this.Load;
		}

		// Token: 0x06001C0D RID: 7181 RVA: 0x000659A8 File Offset: 0x00063BA8
		private void UnregisterEvents()
		{
			Quest.onQuestStatusChanged -= this.OnQuestStatusChanged;
			Quest.onQuestCompleted -= this.OnQuestCompleted;
			SavesSystem.OnCollectSaveData -= this.Save;
			SavesSystem.OnSetFile -= this.Load;
		}

		// Token: 0x06001C0E RID: 7182 RVA: 0x000659FC File Offset: 0x00063BFC
		private void OnQuestCompleted(Quest quest)
		{
			if (!this.activeQuests.Remove(quest))
			{
				Debug.LogWarning(quest.DisplayName + " 并不存在于活跃任务表中。已终止操作。");
				return;
			}
			this.historyQuests.Add(quest);
			Action<QuestManager> action = QuestManager.onQuestListsChanged;
			if (action == null)
			{
				return;
			}
			action(this);
		}

		// Token: 0x06001C0F RID: 7183 RVA: 0x00065A49 File Offset: 0x00063C49
		private void OnQuestStatusChanged(Quest quest)
		{
		}

		// Token: 0x06001C10 RID: 7184 RVA: 0x00065A4C File Offset: 0x00063C4C
		public void ActivateQuest(int id, QuestGiverID? overrideQuestGiverID = null)
		{
			Quest quest = global::UnityEngine.Object.Instantiate<Quest>(this.GetQuestPrefab(id), base.transform);
			if (overrideQuestGiverID != null)
			{
				quest.QuestGiverID = overrideQuestGiverID.Value;
			}
			this.activeQuests.Add(quest);
			quest.NotifyActivated();
			Action<QuestManager> action = QuestManager.onQuestListsChanged;
			if (action == null)
			{
				return;
			}
			action(this);
		}

		// Token: 0x06001C11 RID: 7185 RVA: 0x00065AA4 File Offset: 0x00063CA4
		internal static bool IsQuestAvaliable(int id)
		{
			return !(QuestManager.Instance == null) && !QuestManager.IsQuestFinished(id) && !QuestManager.instance.activeQuests.Any((Quest e) => e.ID == id) && QuestManager.Instance.GetQuestPrefab(id).MeetsPrerequisit();
		}

		// Token: 0x06001C12 RID: 7186 RVA: 0x00065B10 File Offset: 0x00063D10
		internal static bool IsQuestFinished(int id)
		{
			return !(QuestManager.instance == null) && QuestManager.instance.historyQuests.Any((Quest e) => e.ID == id);
		}

		// Token: 0x06001C13 RID: 7187 RVA: 0x00065B54 File Offset: 0x00063D54
		internal static bool AreQuestFinished(IEnumerable<int> requiredQuestIDs)
		{
			if (QuestManager.instance == null)
			{
				return false;
			}
			HashSet<int> hashSet = new HashSet<int>();
			hashSet.AddRange(requiredQuestIDs);
			foreach (Quest quest in QuestManager.instance.historyQuests)
			{
				hashSet.Remove(quest.ID);
			}
			return hashSet.Count <= 0;
		}

		// Token: 0x06001C14 RID: 7188 RVA: 0x00065BDC File Offset: 0x00063DDC
		internal static List<Quest> GetActiveQuestsFromGiver(QuestGiverID giverID)
		{
			List<Quest> list = new List<Quest>();
			if (QuestManager.instance == null)
			{
				return list;
			}
			return QuestManager.instance.ActiveQuests.Where((Quest e) => e.QuestGiverID == giverID).ToList<Quest>();
		}

		// Token: 0x06001C15 RID: 7189 RVA: 0x00065C30 File Offset: 0x00063E30
		internal static List<Quest> GetHistoryQuestsFromGiver(QuestGiverID giverID)
		{
			List<Quest> list = new List<Quest>();
			if (QuestManager.instance == null)
			{
				return list;
			}
			return QuestManager.instance.historyQuests.Where((Quest e) => e != null && e.QuestGiverID == giverID).ToList<Quest>();
		}

		// Token: 0x06001C16 RID: 7190 RVA: 0x00065C7F File Offset: 0x00063E7F
		internal static bool IsQuestActive(Quest quest)
		{
			return !(QuestManager.instance == null) && QuestManager.instance.activeQuests.Contains(quest);
		}

		// Token: 0x06001C17 RID: 7191 RVA: 0x00065CA0 File Offset: 0x00063EA0
		internal static bool IsQuestActive(int questID)
		{
			return !(QuestManager.instance == null) && QuestManager.instance.activeQuests.Any((Quest e) => e.ID == questID);
		}

		// Token: 0x06001C18 RID: 7192 RVA: 0x00065CEC File Offset: 0x00063EEC
		internal static bool AreQuestsActive(IEnumerable<int> requiredQuestIDs)
		{
			if (QuestManager.instance == null)
			{
				return false;
			}
			using (IEnumerator<int> enumerator = requiredQuestIDs.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					int id = enumerator.Current;
					if (!QuestManager.instance.activeQuests.Any((Quest e) => e.ID == id))
					{
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x06001C19 RID: 7193 RVA: 0x00065D6C File Offset: 0x00063F6C
		private void OnTaskFinished(Quest quest, Task task)
		{
			NotificationText.Push(this.TaskFinishNotificationFormat.Format(new
			{
				questDisplayName = quest.DisplayName,
				finishedTasks = quest.FinishedTaskCount,
				totalTasks = quest.tasks.Count
			}));
			Action<Quest, Task> onTaskFinishedEvent = QuestManager.OnTaskFinishedEvent;
			if (onTaskFinishedEvent != null)
			{
				onTaskFinishedEvent(quest, task);
			}
			AudioManager.Post("UI/mission_small");
		}

		// Token: 0x06001C1A RID: 7194 RVA: 0x00065DC2 File Offset: 0x00063FC2
		internal static void NotifyTaskFinished(Quest quest, Task task)
		{
			QuestManager questManager = QuestManager.instance;
			if (questManager == null)
			{
				return;
			}
			questManager.OnTaskFinished(quest, task);
		}

		// Token: 0x06001C1B RID: 7195 RVA: 0x00065DD5 File Offset: 0x00063FD5
		internal static bool EverInspected(int id)
		{
			return !(QuestManager.Instance == null) && QuestManager.Instance.EverInspectedQuest.Contains(id);
		}

		// Token: 0x06001C1C RID: 7196 RVA: 0x00065DF6 File Offset: 0x00063FF6
		internal static void SetEverInspected(int id)
		{
			if (QuestManager.EverInspected(id))
			{
				return;
			}
			if (QuestManager.Instance == null)
			{
				return;
			}
			QuestManager.Instance.EverInspectedQuest.Add(id);
		}

		// Token: 0x040013AF RID: 5039
		[SerializeField]
		private string taskFinishNotificationFormatKey = "UI_Quest_TaskFinishedNotification";

		// Token: 0x040013B0 RID: 5040
		private static QuestManager instance;

		// Token: 0x040013B1 RID: 5041
		public static Action<Quest, Task> OnTaskFinishedEvent;

		// Token: 0x040013B2 RID: 5042
		private List<Quest> activeQuests = new List<Quest>();

		// Token: 0x040013B3 RID: 5043
		private List<Quest> historyQuests = new List<Quest>();

		// Token: 0x040013B4 RID: 5044
		private List<int> everInspectedQuest = new List<int>();

		// Token: 0x040013B6 RID: 5046
		private const string savePrefix = "Quest";

		// Token: 0x040013B7 RID: 5047
		private const string saveKey = "Data";

		// Token: 0x020005E7 RID: 1511
		[Serializable]
		public struct SaveData
		{
			// Token: 0x040020ED RID: 8429
			public List<object> activeQuestsData;

			// Token: 0x040020EE RID: 8430
			public List<object> historyQuestsData;

			// Token: 0x040020EF RID: 8431
			public List<int> everInspectedQuest;
		}
	}
}
