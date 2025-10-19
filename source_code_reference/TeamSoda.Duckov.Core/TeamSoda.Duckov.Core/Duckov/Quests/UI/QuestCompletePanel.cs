using System;
using Cysharp.Threading.Tasks;
using Duckov.UI.Animations;
using Duckov.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Duckov.Quests.UI
{
	// Token: 0x02000341 RID: 833
	public class QuestCompletePanel : MonoBehaviour
	{
		// Token: 0x1700055D RID: 1373
		// (get) Token: 0x06001CA5 RID: 7333 RVA: 0x00066EA4 File Offset: 0x000650A4
		private PrefabPool<RewardEntry> RewardEntryPool
		{
			get
			{
				if (this._rewardEntryPool == null)
				{
					this._rewardEntryPool = new PrefabPool<RewardEntry>(this.rewardEntryTemplate, this.rewardEntryTemplate.transform.parent, null, null, null, true, 10, 10000, null);
					this.rewardEntryTemplate.gameObject.SetActive(false);
				}
				return this._rewardEntryPool;
			}
		}

		// Token: 0x1700055E RID: 1374
		// (get) Token: 0x06001CA6 RID: 7334 RVA: 0x00066EFD File Offset: 0x000650FD
		public Quest Target
		{
			get
			{
				return this.target;
			}
		}

		// Token: 0x06001CA7 RID: 7335 RVA: 0x00066F05 File Offset: 0x00065105
		private void Awake()
		{
			this.skipButton.onClick.AddListener(new UnityAction(this.Skip));
			this.takeAllButton.onClick.AddListener(new UnityAction(this.TakeAll));
		}

		// Token: 0x06001CA8 RID: 7336 RVA: 0x00066F40 File Offset: 0x00065140
		private void TakeAll()
		{
			if (this.target == null)
			{
				return;
			}
			foreach (Reward reward in this.target.rewards)
			{
				if (!reward.Claimed)
				{
					reward.Claim();
				}
			}
		}

		// Token: 0x06001CA9 RID: 7337 RVA: 0x00066FB0 File Offset: 0x000651B0
		public void Skip()
		{
			this.skipClicked = true;
		}

		// Token: 0x06001CAA RID: 7338 RVA: 0x00066FBC File Offset: 0x000651BC
		public async UniTask Show(Quest quest)
		{
			this.target = quest;
			this.SetupContent(quest);
			await this.mainFadeGroup.ShowAndReturnTask();
			await this.WaitForEndOfInteraction();
			if (!(this.target == null))
			{
				foreach (Reward reward in this.target.rewards)
				{
					if (!reward.Claimed && reward.AutoClaim)
					{
						reward.Claim();
					}
				}
				await this.mainFadeGroup.HideAndReturnTask();
			}
		}

		// Token: 0x06001CAB RID: 7339 RVA: 0x00067008 File Offset: 0x00065208
		private async UniTask WaitForEndOfInteraction()
		{
			this.skipClicked = false;
			while (!(this.target == null) && !this.target.AreRewardsClaimed() && !this.skipClicked)
			{
				await UniTask.NextFrame();
			}
		}

		// Token: 0x06001CAC RID: 7340 RVA: 0x0006704C File Offset: 0x0006524C
		private void SetupContent(Quest quest)
		{
			this.target = quest;
			if (quest == null)
			{
				return;
			}
			this.questNameText.text = quest.DisplayName;
			this.RewardEntryPool.ReleaseAll();
			foreach (Reward reward in quest.rewards)
			{
				RewardEntry rewardEntry = this.RewardEntryPool.Get(this.rewardEntryTemplate.transform.parent);
				rewardEntry.Setup(reward);
				rewardEntry.transform.SetAsLastSibling();
			}
		}

		// Token: 0x040013E7 RID: 5095
		[SerializeField]
		private FadeGroup mainFadeGroup;

		// Token: 0x040013E8 RID: 5096
		[SerializeField]
		private TextMeshProUGUI questNameText;

		// Token: 0x040013E9 RID: 5097
		[SerializeField]
		private RewardEntry rewardEntryTemplate;

		// Token: 0x040013EA RID: 5098
		[SerializeField]
		private Button skipButton;

		// Token: 0x040013EB RID: 5099
		[SerializeField]
		private Button takeAllButton;

		// Token: 0x040013EC RID: 5100
		private PrefabPool<RewardEntry> _rewardEntryPool;

		// Token: 0x040013ED RID: 5101
		private Quest target;

		// Token: 0x040013EE RID: 5102
		private bool skipClicked;
	}
}
