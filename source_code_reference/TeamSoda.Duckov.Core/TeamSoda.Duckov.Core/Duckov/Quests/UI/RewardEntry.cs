using System;
using Duckov.Utilities;
using SodaCraft.Localizations;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Duckov.Quests.UI
{
	// Token: 0x0200034A RID: 842
	public class RewardEntry : MonoBehaviour, IPoolable
	{
		// Token: 0x17000571 RID: 1393
		// (get) Token: 0x06001D21 RID: 7457 RVA: 0x0006857D File Offset: 0x0006677D
		// (set) Token: 0x06001D22 RID: 7458 RVA: 0x00068585 File Offset: 0x00066785
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

		// Token: 0x06001D23 RID: 7459 RVA: 0x0006858E File Offset: 0x0006678E
		private void Awake()
		{
			this.claimButton.onClick.AddListener(new UnityAction(this.OnClaimButtonClicked));
		}

		// Token: 0x06001D24 RID: 7460 RVA: 0x000685AC File Offset: 0x000667AC
		private void OnClaimButtonClicked()
		{
			Reward reward = this.target;
			if (reward == null)
			{
				return;
			}
			reward.Claim();
		}

		// Token: 0x06001D25 RID: 7461 RVA: 0x000685BE File Offset: 0x000667BE
		public void NotifyPooled()
		{
		}

		// Token: 0x06001D26 RID: 7462 RVA: 0x000685C0 File Offset: 0x000667C0
		public void NotifyReleased()
		{
			this.UnregisterEvents();
		}

		// Token: 0x06001D27 RID: 7463 RVA: 0x000685C8 File Offset: 0x000667C8
		internal void Setup(Reward target)
		{
			this.UnregisterEvents();
			this.target = target;
			if (target == null)
			{
				return;
			}
			this.Refresh();
			this.RegisterEvents();
		}

		// Token: 0x06001D28 RID: 7464 RVA: 0x000685ED File Offset: 0x000667ED
		private void RegisterEvents()
		{
			if (this.target == null)
			{
				return;
			}
			this.target.onStatusChanged += this.OnTargetStatusChanged;
		}

		// Token: 0x06001D29 RID: 7465 RVA: 0x00068615 File Offset: 0x00066815
		private void UnregisterEvents()
		{
			if (this.target == null)
			{
				return;
			}
			this.target.onStatusChanged -= this.OnTargetStatusChanged;
		}

		// Token: 0x06001D2A RID: 7466 RVA: 0x0006863D File Offset: 0x0006683D
		private void OnTargetStatusChanged()
		{
			this.Refresh();
		}

		// Token: 0x06001D2B RID: 7467 RVA: 0x00068648 File Offset: 0x00066848
		private void Refresh()
		{
			if (this.target == null)
			{
				return;
			}
			this.rewardText.text = this.target.Description;
			Sprite icon = this.target.Icon;
			this.rewardIcon.gameObject.SetActive(icon);
			this.rewardIcon.sprite = icon;
			bool claimed = this.target.Claimed;
			bool claimable = this.target.Claimable;
			bool flag = this.Interactable && claimable;
			bool flag2 = !this.Interactable && claimable && !claimed;
			this.claimButton.gameObject.SetActive(flag);
			if (this.claimableIndicator != null)
			{
				this.claimableIndicator.SetActive(flag2);
			}
			if (flag)
			{
				if (this.buttonText)
				{
					this.buttonText.text = (claimed ? this.claimedTextKey.ToPlainText() : this.claimTextKey.ToPlainText());
				}
				this.statusIcon.sprite = (claimed ? this.claimedIcon : this.claimIcon);
				this.claimButton.interactable = !claimed;
				this.statusIcon.gameObject.SetActive(!this.target.Claiming);
				this.claimingIcon.gameObject.SetActive(this.target.Claiming);
			}
		}

		// Token: 0x0400143F RID: 5183
		[SerializeField]
		private Image rewardIcon;

		// Token: 0x04001440 RID: 5184
		[SerializeField]
		private TextMeshProUGUI rewardText;

		// Token: 0x04001441 RID: 5185
		[SerializeField]
		private Button claimButton;

		// Token: 0x04001442 RID: 5186
		[SerializeField]
		private GameObject claimableIndicator;

		// Token: 0x04001443 RID: 5187
		[SerializeField]
		private Image statusIcon;

		// Token: 0x04001444 RID: 5188
		[SerializeField]
		private TextMeshProUGUI buttonText;

		// Token: 0x04001445 RID: 5189
		[SerializeField]
		private GameObject claimingIcon;

		// Token: 0x04001446 RID: 5190
		[SerializeField]
		private Sprite claimIcon;

		// Token: 0x04001447 RID: 5191
		[LocalizationKey("Default")]
		[SerializeField]
		private string claimTextKey = "UI_Quest_RewardClaim";

		// Token: 0x04001448 RID: 5192
		[SerializeField]
		private Sprite claimedIcon;

		// Token: 0x04001449 RID: 5193
		[LocalizationKey("Default")]
		[SerializeField]
		private string claimedTextKey = "UI_Quest_RewardClaimed";

		// Token: 0x0400144A RID: 5194
		[SerializeField]
		private bool interactable;

		// Token: 0x0400144B RID: 5195
		private Reward target;
	}
}
