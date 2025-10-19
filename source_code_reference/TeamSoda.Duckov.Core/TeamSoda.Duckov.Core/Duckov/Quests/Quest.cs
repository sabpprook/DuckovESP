using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Duckov.Quests.Relations;
using Duckov.Scenes;
using Duckov.Utilities;
using Eflatun.SceneReference;
using ItemStatsSystem;
using Saves;
using SodaCraft.Localizations;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Duckov.Quests
{
	// Token: 0x02000330 RID: 816
	public class Quest : MonoBehaviour, ISaveDataProvider, INeedInspection
	{
		// Token: 0x17000518 RID: 1304
		// (get) Token: 0x06001B99 RID: 7065 RVA: 0x0006416C File Offset: 0x0006236C
		public SceneInfoEntry RequireSceneInfo
		{
			get
			{
				return SceneInfoCollection.GetSceneInfo(this.requireSceneID);
			}
		}

		// Token: 0x17000519 RID: 1305
		// (get) Token: 0x06001B9A RID: 7066 RVA: 0x0006417C File Offset: 0x0006237C
		public SceneReference RequireScene
		{
			get
			{
				SceneInfoEntry requireSceneInfo = this.RequireSceneInfo;
				if (requireSceneInfo == null)
				{
					return null;
				}
				return requireSceneInfo.SceneReference;
			}
		}

		// Token: 0x1700051A RID: 1306
		// (get) Token: 0x06001B9B RID: 7067 RVA: 0x0006419B File Offset: 0x0006239B
		public List<Task> Tasks
		{
			get
			{
				return this.tasks;
			}
		}

		// Token: 0x1700051B RID: 1307
		// (get) Token: 0x06001B9C RID: 7068 RVA: 0x000641A3 File Offset: 0x000623A3
		public ReadOnlyCollection<Reward> Rewards
		{
			get
			{
				if (this._readonly_rewards == null)
				{
					this._readonly_rewards = new ReadOnlyCollection<Reward>(this.rewards);
				}
				return this._readonly_rewards;
			}
		}

		// Token: 0x1700051C RID: 1308
		// (get) Token: 0x06001B9D RID: 7069 RVA: 0x000641C4 File Offset: 0x000623C4
		public ReadOnlyCollection<Condition> Prerequisits
		{
			get
			{
				if (this.prerequisits_ReadOnly == null)
				{
					this.prerequisits_ReadOnly = new ReadOnlyCollection<Condition>(this.prerequisit);
				}
				return this.prerequisits_ReadOnly;
			}
		}

		// Token: 0x1700051D RID: 1309
		// (get) Token: 0x06001B9E RID: 7070 RVA: 0x000641E8 File Offset: 0x000623E8
		public bool SceneRequirementSatisfied
		{
			get
			{
				SceneReference requireScene = this.RequireScene;
				return requireScene == null || requireScene.UnsafeReason == SceneReferenceUnsafeReason.Empty || requireScene.UnsafeReason != SceneReferenceUnsafeReason.None || requireScene.LoadedScene.isLoaded;
			}
		}

		// Token: 0x1700051E RID: 1310
		// (get) Token: 0x06001B9F RID: 7071 RVA: 0x00064224 File Offset: 0x00062424
		public int RequireLevel
		{
			get
			{
				return this.requireLevel;
			}
		}

		// Token: 0x1700051F RID: 1311
		// (get) Token: 0x06001BA0 RID: 7072 RVA: 0x0006422C File Offset: 0x0006242C
		public bool LockInDemo
		{
			get
			{
				return this.lockInDemo;
			}
		}

		// Token: 0x17000520 RID: 1312
		// (get) Token: 0x06001BA1 RID: 7073 RVA: 0x00064234 File Offset: 0x00062434
		// (set) Token: 0x06001BA2 RID: 7074 RVA: 0x0006423C File Offset: 0x0006243C
		public bool Complete
		{
			get
			{
				return this.complete;
			}
			internal set
			{
				this.complete = value;
				Action<Quest> action = this.onStatusChanged;
				if (action != null)
				{
					action(this);
				}
				Action<Quest> action2 = Quest.onQuestStatusChanged;
				if (action2 != null)
				{
					action2(this);
				}
				if (this.complete)
				{
					Action<Quest> action3 = this.onCompleted;
					if (action3 != null)
					{
						action3(this);
					}
					UnityEvent onCompletedUnityEvent = this.OnCompletedUnityEvent;
					if (onCompletedUnityEvent != null)
					{
						onCompletedUnityEvent.Invoke();
					}
					Action<Quest> action4 = Quest.onQuestCompleted;
					if (action4 == null)
					{
						return;
					}
					action4(this);
				}
			}
		}

		// Token: 0x17000521 RID: 1313
		// (get) Token: 0x06001BA3 RID: 7075 RVA: 0x000642B0 File Offset: 0x000624B0
		// (set) Token: 0x06001BA4 RID: 7076 RVA: 0x00064300 File Offset: 0x00062500
		public bool NeedInspection
		{
			get
			{
				return (!this.Active && !QuestManager.EverInspected(this.ID)) || (this.Active && ((this.Active && this.AreTasksFinished()) || this.AnyTaskNeedInspection() || this.needInspection));
			}
			set
			{
				this.needInspection = value;
				Action<Quest> action = this.onNeedInspectionChanged;
				if (action != null)
				{
					action(this);
				}
				Action<Quest> action2 = Quest.onQuestNeedInspectionChanged;
				if (action2 == null)
				{
					return;
				}
				action2(this);
			}
		}

		// Token: 0x06001BA5 RID: 7077 RVA: 0x0006432C File Offset: 0x0006252C
		private bool AnyTaskNeedInspection()
		{
			if (this.tasks != null)
			{
				foreach (Task task in this.tasks)
				{
					if (!(task == null) && task.NeedInspection)
					{
						return true;
					}
				}
				return false;
			}
			return false;
		}

		// Token: 0x140000BF RID: 191
		// (add) Token: 0x06001BA6 RID: 7078 RVA: 0x00064398 File Offset: 0x00062598
		// (remove) Token: 0x06001BA7 RID: 7079 RVA: 0x000643D0 File Offset: 0x000625D0
		public event Action<Quest> onNeedInspectionChanged;

		// Token: 0x140000C0 RID: 192
		// (add) Token: 0x06001BA8 RID: 7080 RVA: 0x00064408 File Offset: 0x00062608
		// (remove) Token: 0x06001BA9 RID: 7081 RVA: 0x00064440 File Offset: 0x00062640
		internal event Action<Quest> onStatusChanged;

		// Token: 0x140000C1 RID: 193
		// (add) Token: 0x06001BAA RID: 7082 RVA: 0x00064478 File Offset: 0x00062678
		// (remove) Token: 0x06001BAB RID: 7083 RVA: 0x000644B0 File Offset: 0x000626B0
		internal event Action<Quest> onActivated;

		// Token: 0x140000C2 RID: 194
		// (add) Token: 0x06001BAC RID: 7084 RVA: 0x000644E8 File Offset: 0x000626E8
		// (remove) Token: 0x06001BAD RID: 7085 RVA: 0x00064520 File Offset: 0x00062720
		internal event Action<Quest> onCompleted;

		// Token: 0x140000C3 RID: 195
		// (add) Token: 0x06001BAE RID: 7086 RVA: 0x00064558 File Offset: 0x00062758
		// (remove) Token: 0x06001BAF RID: 7087 RVA: 0x0006458C File Offset: 0x0006278C
		public static event Action<Quest> onQuestStatusChanged;

		// Token: 0x140000C4 RID: 196
		// (add) Token: 0x06001BB0 RID: 7088 RVA: 0x000645C0 File Offset: 0x000627C0
		// (remove) Token: 0x06001BB1 RID: 7089 RVA: 0x000645F4 File Offset: 0x000627F4
		public static event Action<Quest> onQuestNeedInspectionChanged;

		// Token: 0x140000C5 RID: 197
		// (add) Token: 0x06001BB2 RID: 7090 RVA: 0x00064628 File Offset: 0x00062828
		// (remove) Token: 0x06001BB3 RID: 7091 RVA: 0x0006465C File Offset: 0x0006285C
		public static event Action<Quest> onQuestActivated;

		// Token: 0x140000C6 RID: 198
		// (add) Token: 0x06001BB4 RID: 7092 RVA: 0x00064690 File Offset: 0x00062890
		// (remove) Token: 0x06001BB5 RID: 7093 RVA: 0x000646C4 File Offset: 0x000628C4
		public static event Action<Quest> onQuestCompleted;

		// Token: 0x17000522 RID: 1314
		// (get) Token: 0x06001BB6 RID: 7094 RVA: 0x000646F7 File Offset: 0x000628F7
		// (set) Token: 0x06001BB7 RID: 7095 RVA: 0x000646FF File Offset: 0x000628FF
		public int ID
		{
			get
			{
				return this.id;
			}
			internal set
			{
				this.id = value;
			}
		}

		// Token: 0x17000523 RID: 1315
		// (get) Token: 0x06001BB8 RID: 7096 RVA: 0x00064708 File Offset: 0x00062908
		public bool Active
		{
			get
			{
				return QuestManager.IsQuestActive(this);
			}
		}

		// Token: 0x17000524 RID: 1316
		// (get) Token: 0x06001BB9 RID: 7097 RVA: 0x00064710 File Offset: 0x00062910
		public int RequiredItemID
		{
			get
			{
				return this.requiredItemID;
			}
		}

		// Token: 0x17000525 RID: 1317
		// (get) Token: 0x06001BBA RID: 7098 RVA: 0x00064718 File Offset: 0x00062918
		public int RequiredItemCount
		{
			get
			{
				return this.requiredItemCount;
			}
		}

		// Token: 0x17000526 RID: 1318
		// (get) Token: 0x06001BBB RID: 7099 RVA: 0x00064720 File Offset: 0x00062920
		public string DisplayName
		{
			get
			{
				return this.displayName.ToPlainText();
			}
		}

		// Token: 0x17000527 RID: 1319
		// (get) Token: 0x06001BBC RID: 7100 RVA: 0x0006472D File Offset: 0x0006292D
		public string Description
		{
			get
			{
				return this.description.ToPlainText();
			}
		}

		// Token: 0x17000528 RID: 1320
		// (get) Token: 0x06001BBD RID: 7101 RVA: 0x0006473A File Offset: 0x0006293A
		// (set) Token: 0x06001BBE RID: 7102 RVA: 0x00064742 File Offset: 0x00062942
		public string DisplayNameRaw
		{
			get
			{
				return this.displayName;
			}
			set
			{
				this.displayName = value;
			}
		}

		// Token: 0x17000529 RID: 1321
		// (get) Token: 0x06001BBF RID: 7103 RVA: 0x0006474B File Offset: 0x0006294B
		// (set) Token: 0x06001BC0 RID: 7104 RVA: 0x00064753 File Offset: 0x00062953
		public string DescriptionRaw
		{
			get
			{
				return this.description;
			}
			set
			{
				this.description = value;
			}
		}

		// Token: 0x1700052A RID: 1322
		// (get) Token: 0x06001BC1 RID: 7105 RVA: 0x0006475C File Offset: 0x0006295C
		// (set) Token: 0x06001BC2 RID: 7106 RVA: 0x00064764 File Offset: 0x00062964
		public QuestGiverID QuestGiverID
		{
			get
			{
				return this.questGiverID;
			}
			internal set
			{
				this.questGiverID = value;
			}
		}

		// Token: 0x1700052B RID: 1323
		// (get) Token: 0x06001BC3 RID: 7107 RVA: 0x0006476D File Offset: 0x0006296D
		public object FinishedTaskCount
		{
			get
			{
				return this.tasks.Count((Task e) => e.IsFinished());
			}
		}

		// Token: 0x06001BC4 RID: 7108 RVA: 0x000647A0 File Offset: 0x000629A0
		public bool MeetsPrerequisit()
		{
			if (this.RequireLevel > EXPManager.Level)
			{
				return false;
			}
			if (this.LockInDemo && GameMetaData.Instance.IsDemo)
			{
				return false;
			}
			QuestRelationGraph questRelation = GameplayDataSettings.QuestRelation;
			if (questRelation.GetNode(this.id) == null)
			{
				return false;
			}
			if (!QuestManager.AreQuestFinished(questRelation.GetRequiredIDs(this.id)))
			{
				return false;
			}
			using (List<Condition>.Enumerator enumerator = this.prerequisit.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (!enumerator.Current.Evaluate())
					{
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x06001BC5 RID: 7109 RVA: 0x0006484C File Offset: 0x00062A4C
		public bool AreTasksFinished()
		{
			foreach (Task task in this.tasks)
			{
				if (task == null)
				{
					Debug.LogError(string.Format("存在空的Task，QuestID：{0}", this.id));
				}
				else if (!task.IsFinished())
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06001BC6 RID: 7110 RVA: 0x000648CC File Offset: 0x00062ACC
		public void Initialize()
		{
		}

		// Token: 0x06001BC7 RID: 7111 RVA: 0x000648CE File Offset: 0x00062ACE
		public void OnValidate()
		{
			this.displayName = string.Format("Quest_{0}", this.id);
			this.description = string.Format("Quest_{0}_Desc", this.id);
		}

		// Token: 0x06001BC8 RID: 7112 RVA: 0x00064908 File Offset: 0x00062B08
		public object GenerateSaveData()
		{
			Quest.SaveData saveData = default(Quest.SaveData);
			saveData.id = this.id;
			saveData.complete = this.complete;
			saveData.needInspection = this.needInspection;
			saveData.taskStatus = new List<ValueTuple<int, object>>();
			saveData.rewardStatus = new List<ValueTuple<int, object>>();
			foreach (Task task in this.tasks)
			{
				int num = task.ID;
				object obj = task.GenerateSaveData();
				if (!(task == null))
				{
					saveData.taskStatus.Add(new ValueTuple<int, object>(num, obj));
				}
			}
			foreach (Reward reward in this.rewards)
			{
				if (reward == null)
				{
					Debug.LogError(string.Format("Null Reward detected in quest {0}", this.id));
				}
				else
				{
					int num2 = reward.ID;
					object obj2 = reward.GenerateSaveData();
					saveData.rewardStatus.Add(new ValueTuple<int, object>(num2, obj2));
				}
			}
			return saveData;
		}

		// Token: 0x06001BC9 RID: 7113 RVA: 0x00064A54 File Offset: 0x00062C54
		public void SetupSaveData(object obj)
		{
			Quest.SaveData saveData = (Quest.SaveData)obj;
			if (saveData.id != this.id)
			{
				Debug.LogError("任务ID不匹配，加载失败");
				return;
			}
			this.complete = saveData.complete;
			this.needInspection = saveData.needInspection;
			using (List<ValueTuple<int, object>>.Enumerator enumerator = saveData.taskStatus.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ValueTuple<int, object> cur2 = enumerator.Current;
					Task task = this.tasks.Find((Task e) => e.ID == cur2.Item1);
					if (task == null)
					{
						Debug.LogWarning(string.Format("未找到Task {0}", cur2.Item1));
					}
					else
					{
						task.SetupSaveData(cur2.Item2);
					}
				}
			}
			using (List<ValueTuple<int, object>>.Enumerator enumerator = saveData.rewardStatus.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ValueTuple<int, object> cur = enumerator.Current;
					Reward reward = this.rewards.Find((Reward e) => e.ID == cur.Item1);
					if (reward == null)
					{
						Debug.LogWarning(string.Format("未找到Reward {0}", cur.Item1));
					}
					else
					{
						reward.SetupSaveData(cur.Item2);
						reward.NotifyReload(this);
					}
				}
			}
			this.InitTasks();
			if (this.complete)
			{
				foreach (Reward reward2 in this.rewards)
				{
					if (!(reward2 == null) && !reward2.Claimed && reward2.AutoClaim)
					{
						reward2.Claim();
					}
				}
			}
		}

		// Token: 0x06001BCA RID: 7114 RVA: 0x00064C50 File Offset: 0x00062E50
		internal void NotifyTaskFinished(Task task)
		{
			if (task.Master != this)
			{
				Debug.LogError("Task.Master 与 Quest不匹配");
				return;
			}
			Action<Quest> action = Quest.onQuestStatusChanged;
			if (action != null)
			{
				action(this);
			}
			Action<Quest> action2 = this.onStatusChanged;
			if (action2 != null)
			{
				action2(this);
			}
			QuestManager.NotifyTaskFinished(this, task);
		}

		// Token: 0x06001BCB RID: 7115 RVA: 0x00064CA0 File Offset: 0x00062EA0
		internal void NotifyRewardClaimed(Reward reward)
		{
			if (reward.Master != this)
			{
				Debug.LogError("Reward.Master 与Quest 不匹配");
			}
			if (this.AreRewardsClaimed())
			{
				this.needInspection = false;
			}
			Action<Quest> action = Quest.onQuestStatusChanged;
			if (action != null)
			{
				action(this);
			}
			Action<Quest> action2 = this.onStatusChanged;
			if (action2 != null)
			{
				action2(this);
			}
			Action<Quest> action3 = Quest.onQuestNeedInspectionChanged;
			if (action3 == null)
			{
				return;
			}
			action3(this);
		}

		// Token: 0x06001BCC RID: 7116 RVA: 0x00064D08 File Offset: 0x00062F08
		internal bool AreRewardsClaimed()
		{
			using (List<Reward>.Enumerator enumerator = this.rewards.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (!enumerator.Current.Claimed)
					{
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x06001BCD RID: 7117 RVA: 0x00064D64 File Offset: 0x00062F64
		internal void NotifyActivated()
		{
			this.InitTasks();
			Action<Quest> action = this.onStatusChanged;
			if (action != null)
			{
				action(this);
			}
			Action<Quest> action2 = this.onActivated;
			if (action2 != null)
			{
				action2(this);
			}
			Action<Quest> action3 = Quest.onQuestActivated;
			if (action3 == null)
			{
				return;
			}
			action3(this);
		}

		// Token: 0x06001BCE RID: 7118 RVA: 0x00064DA0 File Offset: 0x00062FA0
		private void InitTasks()
		{
			foreach (Task task in this.tasks)
			{
				task.Init();
			}
		}

		// Token: 0x06001BCF RID: 7119 RVA: 0x00064DF0 File Offset: 0x00062FF0
		public bool TryComplete()
		{
			if (this.Complete)
			{
				return false;
			}
			if (!this.AreTasksFinished())
			{
				return false;
			}
			this.Complete = true;
			return true;
		}

		// Token: 0x06001BD0 RID: 7120 RVA: 0x00064E0E File Offset: 0x0006300E
		internal Quest.QuestInfo GetInfo()
		{
			return new Quest.QuestInfo(this);
		}

		// Token: 0x04001385 RID: 4997
		[SerializeField]
		private int id;

		// Token: 0x04001386 RID: 4998
		[LocalizationKey("Quests")]
		[SerializeField]
		private string displayName;

		// Token: 0x04001387 RID: 4999
		[LocalizationKey("Quests")]
		[SerializeField]
		private string description;

		// Token: 0x04001388 RID: 5000
		[SerializeField]
		private int requireLevel;

		// Token: 0x04001389 RID: 5001
		[SerializeField]
		private bool lockInDemo;

		// Token: 0x0400138A RID: 5002
		[FormerlySerializedAs("requiredItem")]
		[SerializeField]
		[ItemTypeID]
		private int requiredItemID;

		// Token: 0x0400138B RID: 5003
		[SerializeField]
		private int requiredItemCount = 1;

		// Token: 0x0400138C RID: 5004
		[SceneID]
		[SerializeField]
		private string requireSceneID;

		// Token: 0x0400138D RID: 5005
		[SerializeField]
		private QuestGiverID questGiverID;

		// Token: 0x0400138E RID: 5006
		[SerializeField]
		internal List<Condition> prerequisit = new List<Condition>();

		// Token: 0x0400138F RID: 5007
		[SerializeField]
		internal List<Task> tasks = new List<Task>();

		// Token: 0x04001390 RID: 5008
		[SerializeField]
		internal List<Reward> rewards = new List<Reward>();

		// Token: 0x04001391 RID: 5009
		private ReadOnlyCollection<Reward> _readonly_rewards;

		// Token: 0x04001392 RID: 5010
		[SerializeField]
		[HideInInspector]
		private int nextTaskID;

		// Token: 0x04001393 RID: 5011
		[SerializeField]
		[HideInInspector]
		private int nextRewardID;

		// Token: 0x04001394 RID: 5012
		private ReadOnlyCollection<Condition> prerequisits_ReadOnly;

		// Token: 0x04001395 RID: 5013
		[SerializeField]
		private bool complete;

		// Token: 0x04001396 RID: 5014
		[SerializeField]
		private bool needInspection;

		// Token: 0x0400139B RID: 5019
		public UnityEvent OnCompletedUnityEvent;

		// Token: 0x020005DF RID: 1503
		[Serializable]
		public struct SaveData
		{
			// Token: 0x040020D8 RID: 8408
			public int id;

			// Token: 0x040020D9 RID: 8409
			public bool complete;

			// Token: 0x040020DA RID: 8410
			public bool needInspection;

			// Token: 0x040020DB RID: 8411
			public QuestGiverID questGiverID;

			// Token: 0x040020DC RID: 8412
			[TupleElementNames(new string[] { "id", "data" })]
			public List<ValueTuple<int, object>> taskStatus;

			// Token: 0x040020DD RID: 8413
			[TupleElementNames(new string[] { "id", "data" })]
			public List<ValueTuple<int, object>> rewardStatus;
		}

		// Token: 0x020005E0 RID: 1504
		public struct QuestInfo
		{
			// Token: 0x06002923 RID: 10531 RVA: 0x00098853 File Offset: 0x00096A53
			public QuestInfo(Quest quest)
			{
				this.questId = quest.id;
			}

			// Token: 0x040020DE RID: 8414
			public int questId;
		}
	}
}
