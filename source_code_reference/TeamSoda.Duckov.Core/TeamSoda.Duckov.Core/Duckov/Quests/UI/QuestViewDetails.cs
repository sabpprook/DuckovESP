using System;
using Cysharp.Threading.Tasks;
using Duckov.UI.Animations;
using Duckov.Utilities;
using TMPro;
using UnityEngine;

namespace Duckov.Quests.UI
{
	// Token: 0x02000348 RID: 840
	public class QuestViewDetails : MonoBehaviour
	{
		// Token: 0x1700056D RID: 1389
		// (get) Token: 0x06001D0B RID: 7435 RVA: 0x000681C7 File Offset: 0x000663C7
		public Quest Target
		{
			get
			{
				return this.target;
			}
		}

		// Token: 0x1700056E RID: 1390
		// (get) Token: 0x06001D0C RID: 7436 RVA: 0x000681CF File Offset: 0x000663CF
		// (set) Token: 0x06001D0D RID: 7437 RVA: 0x000681D7 File Offset: 0x000663D7
		public bool Interactable
		{
			get
			{
				return this.interactable;
			}
			internal set
			{
				this.interactable = value;
			}
		}

		// Token: 0x1700056F RID: 1391
		// (get) Token: 0x06001D0E RID: 7438 RVA: 0x000681E0 File Offset: 0x000663E0
		private PrefabPool<TaskEntry> TaskEntryPool
		{
			get
			{
				if (this._taskEntryPool == null)
				{
					this._taskEntryPool = new PrefabPool<TaskEntry>(this.taskEntryPrefab, this.tasksParent, null, null, null, true, 10, 10000, null);
				}
				return this._taskEntryPool;
			}
		}

		// Token: 0x17000570 RID: 1392
		// (get) Token: 0x06001D0F RID: 7439 RVA: 0x00068220 File Offset: 0x00066420
		private PrefabPool<RewardEntry> RewardEntryPool
		{
			get
			{
				if (this._rewardEntryPool == null)
				{
					this._rewardEntryPool = new PrefabPool<RewardEntry>(this.rewardEntry, this.rewardsParent, null, null, null, true, 10, 10000, null);
				}
				return this._rewardEntryPool;
			}
		}

		// Token: 0x06001D10 RID: 7440 RVA: 0x0006825E File Offset: 0x0006645E
		private void Awake()
		{
			this.rewardEntry.gameObject.SetActive(false);
			this.taskEntryPrefab.gameObject.SetActive(false);
		}

		// Token: 0x06001D11 RID: 7441 RVA: 0x00068282 File Offset: 0x00066482
		internal void Refresh()
		{
			this.RefreshAsync().Forget();
		}

		// Token: 0x06001D12 RID: 7442 RVA: 0x00068290 File Offset: 0x00066490
		private int GetNewToken()
		{
			int num;
			for (num = this.activeTaskToken; num == this.activeTaskToken; num = global::UnityEngine.Random.Range(1, int.MaxValue))
			{
			}
			this.activeTaskToken = num;
			return this.activeTaskToken;
		}

		// Token: 0x06001D13 RID: 7443 RVA: 0x000682C8 File Offset: 0x000664C8
		private async UniTask RefreshAsync()
		{
			int token = this.GetNewToken();
			this.UnregisterEvents();
			if (this.showingQuest != this.target)
			{
				if (this.target == null)
				{
					this.placeHolder.SetActive(true);
				}
				await this.contentFadeGroup.HideAndReturnTask();
				if (token != this.activeTaskToken)
				{
					return;
				}
			}
			this.showingQuest = this.target;
			if (this.target == null)
			{
				this.placeHolder.SetActive(true);
				this.contentFadeGroup.SkipHide();
			}
			else
			{
				this.placeHolder.SetActive(false);
				this.target.NeedInspection = false;
				this.displayName.text = this.target.DisplayName;
				this.questGiverDisplayName.text = GameplayDataSettings.Quests.GetDisplayName(this.target.QuestGiverID);
				this.description.text = this.target.Description;
				this.requiredItem.Set(this.target.RequiredItemID, this.target.RequiredItemCount);
				this.SetupTasks();
				this.SetupRewards();
				this.RegisterEvents();
				await this.contentFadeGroup.ShowAndReturnTask();
			}
		}

		// Token: 0x06001D14 RID: 7444 RVA: 0x0006830C File Offset: 0x0006650C
		private void SetupTasks()
		{
			this.TaskEntryPool.ReleaseAll();
			if (this.target == null)
			{
				return;
			}
			foreach (Task task in this.target.tasks)
			{
				TaskEntry taskEntry = this.TaskEntryPool.Get(this.tasksParent);
				taskEntry.Interactable = this.Interactable;
				taskEntry.Setup(task);
				taskEntry.transform.SetAsLastSibling();
			}
		}

		// Token: 0x06001D15 RID: 7445 RVA: 0x000683A8 File Offset: 0x000665A8
		private void SetupRewards()
		{
			this.RewardEntryPool.ReleaseAll();
			if (this.target == null)
			{
				return;
			}
			foreach (Reward reward in this.target.rewards)
			{
				if (reward == null)
				{
					Debug.LogError(string.Format("任务 {0} - {1} 中包含值为 null 的奖励。", this.target.ID, this.target.DisplayName));
				}
				else
				{
					RewardEntry rewardEntry = this.RewardEntryPool.Get(this.rewardsParent);
					rewardEntry.Interactable = this.Interactable;
					rewardEntry.Setup(reward);
					rewardEntry.transform.SetAsLastSibling();
				}
			}
		}

		// Token: 0x06001D16 RID: 7446 RVA: 0x00068478 File Offset: 0x00066678
		private void RegisterEvents()
		{
			if (this.target == null)
			{
				return;
			}
			this.target.onStatusChanged += this.OnTargetStatusChanged;
		}

		// Token: 0x06001D17 RID: 7447 RVA: 0x000684A0 File Offset: 0x000666A0
		private void UnregisterEvents()
		{
			if (this.target == null)
			{
				return;
			}
			this.target.onStatusChanged -= this.OnTargetStatusChanged;
		}

		// Token: 0x06001D18 RID: 7448 RVA: 0x000684C8 File Offset: 0x000666C8
		private void OnTargetStatusChanged(Quest quest)
		{
			this.Refresh();
		}

		// Token: 0x06001D19 RID: 7449 RVA: 0x000684D0 File Offset: 0x000666D0
		internal void Setup(Quest quest)
		{
			this.target = quest;
			this.Refresh();
		}

		// Token: 0x06001D1A RID: 7450 RVA: 0x000684DF File Offset: 0x000666DF
		private void OnDestroy()
		{
			this.UnregisterEvents();
		}

		// Token: 0x0400142B RID: 5163
		private Quest target;

		// Token: 0x0400142C RID: 5164
		[SerializeField]
		private TaskEntry taskEntryPrefab;

		// Token: 0x0400142D RID: 5165
		[SerializeField]
		private RewardEntry rewardEntry;

		// Token: 0x0400142E RID: 5166
		[SerializeField]
		private GameObject placeHolder;

		// Token: 0x0400142F RID: 5167
		[SerializeField]
		private FadeGroup contentFadeGroup;

		// Token: 0x04001430 RID: 5168
		[SerializeField]
		private TextMeshProUGUI displayName;

		// Token: 0x04001431 RID: 5169
		[SerializeField]
		private TextMeshProUGUI description;

		// Token: 0x04001432 RID: 5170
		[SerializeField]
		private TextMeshProUGUI questGiverDisplayName;

		// Token: 0x04001433 RID: 5171
		[SerializeField]
		private Transform tasksParent;

		// Token: 0x04001434 RID: 5172
		[SerializeField]
		private Transform rewardsParent;

		// Token: 0x04001435 RID: 5173
		[SerializeField]
		private QuestRequiredItem requiredItem;

		// Token: 0x04001436 RID: 5174
		[SerializeField]
		private bool interactable;

		// Token: 0x04001437 RID: 5175
		private PrefabPool<TaskEntry> _taskEntryPool;

		// Token: 0x04001438 RID: 5176
		private PrefabPool<RewardEntry> _rewardEntryPool;

		// Token: 0x04001439 RID: 5177
		private Quest showingQuest;

		// Token: 0x0400143A RID: 5178
		private int activeTaskToken;
	}
}
