using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Duckov.Endowment.UI
{
	// Token: 0x020002F4 RID: 756
	public class EndowmentSelectionEntry : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
	{
		// Token: 0x17000474 RID: 1140
		// (get) Token: 0x06001885 RID: 6277 RVA: 0x000594E3 File Offset: 0x000576E3
		public string DisplayName
		{
			get
			{
				if (this.Target == null)
				{
					return "-";
				}
				return this.Target.DisplayName;
			}
		}

		// Token: 0x17000475 RID: 1141
		// (get) Token: 0x06001886 RID: 6278 RVA: 0x00059504 File Offset: 0x00057704
		public string Description
		{
			get
			{
				if (this.Target == null)
				{
					return "-";
				}
				return this.Target.Description;
			}
		}

		// Token: 0x17000476 RID: 1142
		// (get) Token: 0x06001887 RID: 6279 RVA: 0x00059525 File Offset: 0x00057725
		public string DescriptionAndEffects
		{
			get
			{
				if (this.Target == null)
				{
					return "-";
				}
				return this.Target.DescriptionAndEffects;
			}
		}

		// Token: 0x17000477 RID: 1143
		// (get) Token: 0x06001888 RID: 6280 RVA: 0x00059546 File Offset: 0x00057746
		public EndowmentIndex Index
		{
			get
			{
				if (this.Target == null)
				{
					return EndowmentIndex.None;
				}
				return this.Target.Index;
			}
		}

		// Token: 0x17000478 RID: 1144
		// (get) Token: 0x06001889 RID: 6281 RVA: 0x00059563 File Offset: 0x00057763
		// (set) Token: 0x0600188A RID: 6282 RVA: 0x0005956B File Offset: 0x0005776B
		public EndowmentEntry Target { get; private set; }

		// Token: 0x17000479 RID: 1145
		// (get) Token: 0x0600188B RID: 6283 RVA: 0x00059574 File Offset: 0x00057774
		// (set) Token: 0x0600188C RID: 6284 RVA: 0x0005957C File Offset: 0x0005777C
		public bool Selected { get; private set; }

		// Token: 0x1700047A RID: 1146
		// (get) Token: 0x0600188D RID: 6285 RVA: 0x00059585 File Offset: 0x00057785
		public bool Unlocked
		{
			get
			{
				return EndowmentManager.GetEndowmentUnlocked(this.Index);
			}
		}

		// Token: 0x1700047B RID: 1147
		// (get) Token: 0x0600188E RID: 6286 RVA: 0x00059592 File Offset: 0x00057792
		public bool Locked
		{
			get
			{
				return !this.Unlocked;
			}
		}

		// Token: 0x0600188F RID: 6287 RVA: 0x000595A0 File Offset: 0x000577A0
		public void Setup(EndowmentEntry target)
		{
			this.Target = target;
			if (this.Target == null)
			{
				return;
			}
			this.displayNameText.text = this.Target.DisplayName;
			this.icon.sprite = this.Target.Icon;
			this.requirementText.text = "- " + this.Target.RequirementText + " -";
			this.Refresh();
		}

		// Token: 0x06001890 RID: 6288 RVA: 0x0005961A File Offset: 0x0005781A
		private void Refresh()
		{
			if (this.Target == null)
			{
				return;
			}
			this.selectedIndicator.SetActive(this.Selected);
			this.lockedIndcator.SetActive(this.Locked);
		}

		// Token: 0x06001891 RID: 6289 RVA: 0x0005964D File Offset: 0x0005784D
		public void OnPointerClick(PointerEventData eventData)
		{
			if (this.Locked)
			{
				return;
			}
			Action<EndowmentSelectionEntry, PointerEventData> action = this.onClicked;
			if (action == null)
			{
				return;
			}
			action(this, eventData);
		}

		// Token: 0x06001892 RID: 6290 RVA: 0x0005966A File Offset: 0x0005786A
		internal void SetSelection(bool value)
		{
			this.Selected = value;
			this.Refresh();
		}

		// Token: 0x040011DF RID: 4575
		[SerializeField]
		private TextMeshProUGUI displayNameText;

		// Token: 0x040011E0 RID: 4576
		[SerializeField]
		private Image icon;

		// Token: 0x040011E1 RID: 4577
		[SerializeField]
		private GameObject selectedIndicator;

		// Token: 0x040011E2 RID: 4578
		[SerializeField]
		private GameObject lockedIndcator;

		// Token: 0x040011E3 RID: 4579
		[SerializeField]
		private TextMeshProUGUI requirementText;

		// Token: 0x040011E4 RID: 4580
		public Action<EndowmentSelectionEntry, PointerEventData> onClicked;
	}
}
