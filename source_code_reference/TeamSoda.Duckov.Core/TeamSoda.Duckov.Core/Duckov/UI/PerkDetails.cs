using System;
using Duckov.PerkTrees;
using Duckov.UI.Animations;
using Duckov.Utilities;
using LeTai.TrueShadow;
using SodaCraft.Localizations;
using SodaCraft.StringUtilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Duckov.UI
{
	// Token: 0x020003B6 RID: 950
	public class PerkDetails : MonoBehaviour
	{
		// Token: 0x17000692 RID: 1682
		// (get) Token: 0x0600226E RID: 8814 RVA: 0x0007861F File Offset: 0x0007681F
		[SerializeField]
		private string RequireLevelFormatKey
		{
			get
			{
				return "UI_Perk_RequireLevel";
			}
		}

		// Token: 0x17000693 RID: 1683
		// (get) Token: 0x0600226F RID: 8815 RVA: 0x00078626 File Offset: 0x00076826
		[SerializeField]
		private string RequireLevelFormat
		{
			get
			{
				return this.RequireLevelFormatKey.ToPlainText();
			}
		}

		// Token: 0x06002270 RID: 8816 RVA: 0x00078633 File Offset: 0x00076833
		private void Awake()
		{
			this.beginButton.onClick.AddListener(new UnityAction(this.OnBeginButtonClicked));
			this.activateButton.onClick.AddListener(new UnityAction(this.OnActivateButtonClicked));
		}

		// Token: 0x06002271 RID: 8817 RVA: 0x0007866D File Offset: 0x0007686D
		private void OnActivateButtonClicked()
		{
			this.showingPerk.ConfirmUnlock();
		}

		// Token: 0x06002272 RID: 8818 RVA: 0x0007867B File Offset: 0x0007687B
		private void OnBeginButtonClicked()
		{
			this.showingPerk.SubmitItemsAndBeginUnlocking();
		}

		// Token: 0x06002273 RID: 8819 RVA: 0x00078689 File Offset: 0x00076889
		private void OnEnable()
		{
			this.Refresh();
		}

		// Token: 0x06002274 RID: 8820 RVA: 0x00078691 File Offset: 0x00076891
		public void Setup(Perk perk, bool editable = false)
		{
			this.UnregisterEvents();
			this.showingPerk = perk;
			this.editable = editable;
			this.Refresh();
			this.RegisterEvents();
		}

		// Token: 0x06002275 RID: 8821 RVA: 0x000786B3 File Offset: 0x000768B3
		private void RegisterEvents()
		{
			if (this.showingPerk)
			{
				this.showingPerk.onUnlockStateChanged += this.OnTargetStateChanged;
			}
		}

		// Token: 0x06002276 RID: 8822 RVA: 0x000786D9 File Offset: 0x000768D9
		private void OnTargetStateChanged(Perk perk, bool arg2)
		{
			this.Refresh();
		}

		// Token: 0x06002277 RID: 8823 RVA: 0x000786E1 File Offset: 0x000768E1
		private void UnregisterEvents()
		{
			if (this.showingPerk)
			{
				this.showingPerk.onUnlockStateChanged -= this.OnTargetStateChanged;
			}
		}

		// Token: 0x06002278 RID: 8824 RVA: 0x00078708 File Offset: 0x00076908
		private void Refresh()
		{
			if (this.showingPerk == null)
			{
				this.content.Hide();
				this.placeHolder.Show();
				return;
			}
			this.text_Name.text = this.showingPerk.DisplayName;
			this.text_Description.text = this.showingPerk.Description;
			this.icon.sprite = this.showingPerk.Icon;
			ValueTuple<float, Color, bool> shadowOffsetAndColorOfQuality = GameplayDataSettings.UIStyle.GetShadowOffsetAndColorOfQuality(this.showingPerk.DisplayQuality);
			this.iconShadow.IgnoreCasterColor = true;
			this.iconShadow.Color = shadowOffsetAndColorOfQuality.Item2;
			this.iconShadow.Inset = shadowOffsetAndColorOfQuality.Item3;
			this.iconShadow.OffsetDistance = shadowOffsetAndColorOfQuality.Item1;
			bool flag = !this.showingPerk.Unlocked && this.editable;
			bool flag2 = this.showingPerk.AreAllParentsUnlocked();
			bool flag3 = false;
			if (flag2)
			{
				flag3 = this.showingPerk.Requirement.AreSatisfied();
			}
			this.activateButton.gameObject.SetActive(false);
			this.beginButton.gameObject.SetActive(false);
			this.buttonUnavaliablePlaceHolder.SetActive(false);
			this.buttonUnsatisfiedPlaceHolder.SetActive(false);
			this.inProgressPlaceHolder.SetActive(false);
			this.unlockedIndicator.SetActive(this.showingPerk.Unlocked);
			if (!this.showingPerk.Unlocked)
			{
				if (this.showingPerk.Unlocking)
				{
					if (this.showingPerk.GetRemainingTime() <= TimeSpan.Zero)
					{
						this.activateButton.gameObject.SetActive(true);
					}
					else
					{
						this.inProgressPlaceHolder.SetActive(true);
					}
				}
				else if (flag2)
				{
					if (flag3)
					{
						this.beginButton.gameObject.SetActive(true);
					}
					else
					{
						this.buttonUnsatisfiedPlaceHolder.SetActive(true);
					}
				}
				else
				{
					this.buttonUnavaliablePlaceHolder.SetActive(true);
				}
			}
			if (flag)
			{
				this.SetupActivationInfo();
			}
			this.activationInfoParent.SetActive(flag);
			this.content.Show();
			this.placeHolder.Hide();
		}

		// Token: 0x06002279 RID: 8825 RVA: 0x00078918 File Offset: 0x00076B18
		private void SetupActivationInfo()
		{
			if (!this.showingPerk)
			{
				return;
			}
			int level = this.showingPerk.Requirement.level;
			if (level > 0)
			{
				bool flag = EXPManager.Level >= level;
				string text = "#" + (flag ? this.normalTextColor.ToHexString() : this.unsatisfiedTextColor.ToHexString());
				this.text_RequireLevel.gameObject.SetActive(true);
				int level2 = this.showingPerk.Requirement.level;
				string text2 = text;
				this.text_RequireLevel.text = this.RequireLevelFormat.Format(new
				{
					level = level2,
					color = text2
				});
			}
			else
			{
				this.text_RequireLevel.gameObject.SetActive(false);
			}
			this.costDisplay.Setup(this.showingPerk.Requirement.cost, 1);
		}

		// Token: 0x0600227A RID: 8826 RVA: 0x000789E8 File Offset: 0x00076BE8
		private void Update()
		{
			if (this.showingPerk && this.showingPerk.Unlocking && this.inProgressPlaceHolder.activeSelf)
			{
				this.UpdateCountDown();
			}
		}

		// Token: 0x0600227B RID: 8827 RVA: 0x00078A18 File Offset: 0x00076C18
		private void UpdateCountDown()
		{
			TimeSpan remainingTime = this.showingPerk.GetRemainingTime();
			if (remainingTime <= TimeSpan.Zero)
			{
				this.Refresh();
				return;
			}
			this.progressFillImage.fillAmount = this.showingPerk.GetProgress01();
			this.countDownText.text = string.Format("{0} {1:00}:{2:00}:{3:00}.{4:000}", new object[] { remainingTime.Days, remainingTime.Hours, remainingTime.Minutes, remainingTime.Seconds, remainingTime.Milliseconds });
		}

		// Token: 0x04001769 RID: 5993
		[SerializeField]
		private FadeGroup content;

		// Token: 0x0400176A RID: 5994
		[SerializeField]
		private FadeGroup placeHolder;

		// Token: 0x0400176B RID: 5995
		[SerializeField]
		private TextMeshProUGUI text_Name;

		// Token: 0x0400176C RID: 5996
		[SerializeField]
		private TextMeshProUGUI text_Description;

		// Token: 0x0400176D RID: 5997
		[SerializeField]
		private Image icon;

		// Token: 0x0400176E RID: 5998
		[SerializeField]
		private TrueShadow iconShadow;

		// Token: 0x0400176F RID: 5999
		[SerializeField]
		private GameObject unlockedIndicator;

		// Token: 0x04001770 RID: 6000
		[SerializeField]
		private GameObject activationInfoParent;

		// Token: 0x04001771 RID: 6001
		[SerializeField]
		private TextMeshProUGUI text_RequireLevel;

		// Token: 0x04001772 RID: 6002
		[SerializeField]
		private CostDisplay costDisplay;

		// Token: 0x04001773 RID: 6003
		[SerializeField]
		private Color normalTextColor = Color.white;

		// Token: 0x04001774 RID: 6004
		[SerializeField]
		private Color unsatisfiedTextColor = Color.red;

		// Token: 0x04001775 RID: 6005
		[SerializeField]
		private Button activateButton;

		// Token: 0x04001776 RID: 6006
		[SerializeField]
		private Button beginButton;

		// Token: 0x04001777 RID: 6007
		[SerializeField]
		private GameObject buttonUnsatisfiedPlaceHolder;

		// Token: 0x04001778 RID: 6008
		[SerializeField]
		private GameObject buttonUnavaliablePlaceHolder;

		// Token: 0x04001779 RID: 6009
		[SerializeField]
		private GameObject inProgressPlaceHolder;

		// Token: 0x0400177A RID: 6010
		[SerializeField]
		private Image progressFillImage;

		// Token: 0x0400177B RID: 6011
		[SerializeField]
		private TextMeshProUGUI countDownText;

		// Token: 0x0400177C RID: 6012
		private Perk showingPerk;

		// Token: 0x0400177D RID: 6013
		private bool editable;
	}
}
