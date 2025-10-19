using System;
using Duckov.PerkTrees;
using Duckov.Utilities;
using LeTai.TrueShadow;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Duckov.UI
{
	// Token: 0x020003B7 RID: 951
	public class PerkEntry : MonoBehaviour, IPointerClickHandler, IEventSystemHandler, IPoolable
	{
		// Token: 0x0600227D RID: 8829 RVA: 0x00078ADF File Offset: 0x00076CDF
		private void SwitchToActiveLook()
		{
			this.ApplyLook(this.activeLook);
		}

		// Token: 0x0600227E RID: 8830 RVA: 0x00078AED File Offset: 0x00076CED
		private void SwitchToAvaliableLook()
		{
			this.ApplyLook(this.avaliableLook);
		}

		// Token: 0x0600227F RID: 8831 RVA: 0x00078AFB File Offset: 0x00076CFB
		private void SwitchToUnavaliableLook()
		{
			this.ApplyLook(this.unavaliableLook);
		}

		// Token: 0x17000694 RID: 1684
		// (get) Token: 0x06002280 RID: 8832 RVA: 0x00078B09 File Offset: 0x00076D09
		public RectTransform RectTransform
		{
			get
			{
				if (this._rectTransform == null)
				{
					this._rectTransform = base.GetComponent<RectTransform>();
				}
				return this._rectTransform;
			}
		}

		// Token: 0x17000695 RID: 1685
		// (get) Token: 0x06002281 RID: 8833 RVA: 0x00078B2B File Offset: 0x00076D2B
		public Perk Target
		{
			get
			{
				return this.target;
			}
		}

		// Token: 0x06002282 RID: 8834 RVA: 0x00078B34 File Offset: 0x00076D34
		public void Setup(PerkTreeView master, Perk target)
		{
			this.UnregisterEvents();
			this.master = master;
			this.target = target;
			this.icon.sprite = target.Icon;
			ValueTuple<float, Color, bool> shadowOffsetAndColorOfQuality = GameplayDataSettings.UIStyle.GetShadowOffsetAndColorOfQuality(target.DisplayQuality);
			this.iconShadow.IgnoreCasterColor = true;
			this.iconShadow.Color = shadowOffsetAndColorOfQuality.Item2;
			this.iconShadow.OffsetDistance = shadowOffsetAndColorOfQuality.Item1;
			this.iconShadow.Inset = shadowOffsetAndColorOfQuality.Item3;
			this.displayNameText.text = target.DisplayName;
			this.Refresh();
			this.RegisterEvents();
		}

		// Token: 0x06002283 RID: 8835 RVA: 0x00078BD4 File Offset: 0x00076DD4
		private void Refresh()
		{
			if (this.target == null)
			{
				return;
			}
			bool unlocked = this.target.Unlocked;
			bool flag = this.target.AreAllParentsUnlocked();
			if (unlocked)
			{
				this.SwitchToActiveLook();
			}
			else if (flag)
			{
				this.SwitchToAvaliableLook();
			}
			else
			{
				this.SwitchToUnavaliableLook();
			}
			bool unlocking = this.target.Unlocking;
			bool flag2 = this.target.GetRemainingTime() <= TimeSpan.Zero;
			this.avaliableForResearchIndicator.SetActive(!unlocked && !unlocking && this.target.AreAllParentsUnlocked() && this.target.Requirement.AreSatisfied());
			this.inProgressIndicator.SetActive(!unlocked && unlocking && !flag2);
			this.timeUpIndicator.SetActive(!unlocked && unlocking && flag2);
			if (this.master == null)
			{
				return;
			}
			this.selectionIndicator.SetActive(this.master.GetSelection() == this);
		}

		// Token: 0x06002284 RID: 8836 RVA: 0x00078CCF File Offset: 0x00076ECF
		private void OnMasterSelectionChanged(PerkEntry entry)
		{
			this.Refresh();
		}

		// Token: 0x06002285 RID: 8837 RVA: 0x00078CD8 File Offset: 0x00076ED8
		private void RegisterEvents()
		{
			if (this.master)
			{
				this.master.onSelectionChanged += this.OnMasterSelectionChanged;
			}
			if (this.target)
			{
				this.target.onUnlockStateChanged += this.OnTargetStateChanged;
			}
		}

		// Token: 0x06002286 RID: 8838 RVA: 0x00078D2D File Offset: 0x00076F2D
		private void OnTargetStateChanged(Perk perk, bool state)
		{
			PunchReceiver punchReceiver = this.punchReceiver;
			if (punchReceiver != null)
			{
				punchReceiver.Punch();
			}
			this.Refresh();
		}

		// Token: 0x06002287 RID: 8839 RVA: 0x00078D48 File Offset: 0x00076F48
		private void UnregisterEvents()
		{
			if (this.master)
			{
				this.master.onSelectionChanged -= this.OnMasterSelectionChanged;
			}
			if (this.target)
			{
				this.target.onUnlockStateChanged -= this.OnTargetStateChanged;
			}
		}

		// Token: 0x06002288 RID: 8840 RVA: 0x00078D9D File Offset: 0x00076F9D
		private void OnDisable()
		{
			this.UnregisterEvents();
		}

		// Token: 0x06002289 RID: 8841 RVA: 0x00078DA5 File Offset: 0x00076FA5
		public void NotifyPooled()
		{
		}

		// Token: 0x0600228A RID: 8842 RVA: 0x00078DA7 File Offset: 0x00076FA7
		public void NotifyReleased()
		{
		}

		// Token: 0x0600228B RID: 8843 RVA: 0x00078DA9 File Offset: 0x00076FA9
		public void OnPointerClick(PointerEventData eventData)
		{
			if (this.master == null)
			{
				return;
			}
			PunchReceiver punchReceiver = this.punchReceiver;
			if (punchReceiver != null)
			{
				punchReceiver.Punch();
			}
			this.master.SetSelection(this);
		}

		// Token: 0x0600228C RID: 8844 RVA: 0x00078DD8 File Offset: 0x00076FD8
		internal Vector2 GetLayoutPosition()
		{
			if (this.target == null)
			{
				return Vector2.zero;
			}
			return this.target.GetLayoutPosition();
		}

		// Token: 0x0600228D RID: 8845 RVA: 0x00078DFC File Offset: 0x00076FFC
		private void ApplyLook(PerkEntry.Look look)
		{
			this.icon.material = look.material;
			this.icon.color = look.iconColor;
			this.frame.color = look.frameColor;
			this.frameGlow.enabled = look.frameGlowColor.a > 0f;
			this.frameGlow.Color = look.frameGlowColor;
			this.background.color = look.backgroundColor;
		}

		// Token: 0x0600228E RID: 8846 RVA: 0x00078E7B File Offset: 0x0007707B
		private void FixedUpdate()
		{
			if (this.inProgressIndicator.activeSelf && this.target.GetRemainingTime() <= TimeSpan.Zero)
			{
				this.Refresh();
			}
		}

		// Token: 0x0400177E RID: 6014
		[SerializeField]
		private Image icon;

		// Token: 0x0400177F RID: 6015
		[SerializeField]
		private TrueShadow iconShadow;

		// Token: 0x04001780 RID: 6016
		[SerializeField]
		private GameObject selectionIndicator;

		// Token: 0x04001781 RID: 6017
		[SerializeField]
		private Image frame;

		// Token: 0x04001782 RID: 6018
		[SerializeField]
		private TrueShadow frameGlow;

		// Token: 0x04001783 RID: 6019
		[SerializeField]
		private Image background;

		// Token: 0x04001784 RID: 6020
		[SerializeField]
		private TextMeshProUGUI displayNameText;

		// Token: 0x04001785 RID: 6021
		[SerializeField]
		private PunchReceiver punchReceiver;

		// Token: 0x04001786 RID: 6022
		[SerializeField]
		private GameObject inProgressIndicator;

		// Token: 0x04001787 RID: 6023
		[SerializeField]
		private GameObject timeUpIndicator;

		// Token: 0x04001788 RID: 6024
		[SerializeField]
		private GameObject avaliableForResearchIndicator;

		// Token: 0x04001789 RID: 6025
		[SerializeField]
		private PerkEntry.Look activeLook;

		// Token: 0x0400178A RID: 6026
		[SerializeField]
		private PerkEntry.Look avaliableLook;

		// Token: 0x0400178B RID: 6027
		[SerializeField]
		private PerkEntry.Look unavaliableLook;

		// Token: 0x0400178C RID: 6028
		private RectTransform _rectTransform;

		// Token: 0x0400178D RID: 6029
		private PerkTreeView master;

		// Token: 0x0400178E RID: 6030
		private Perk target;

		// Token: 0x02000628 RID: 1576
		[Serializable]
		public struct Look
		{
			// Token: 0x040021D8 RID: 8664
			public Color iconColor;

			// Token: 0x040021D9 RID: 8665
			public Material material;

			// Token: 0x040021DA RID: 8666
			public Color frameColor;

			// Token: 0x040021DB RID: 8667
			public Color frameGlowColor;

			// Token: 0x040021DC RID: 8668
			public Color backgroundColor;
		}
	}
}
