using System;
using Cysharp.Threading.Tasks;
using Duckov.Scenes;
using Duckov.UI.Animations;
using Duckov.Utilities;
using Saves;
using SodaCraft.Localizations;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Duckov.Rules.UI
{
	// Token: 0x020003F0 RID: 1008
	public class DifficultySelection : MonoBehaviour
	{
		// Token: 0x170006ED RID: 1773
		// (get) Token: 0x06002457 RID: 9303 RVA: 0x0007E2E4 File Offset: 0x0007C4E4
		private PrefabPool<DifficultySelection_Entry> EntryPool
		{
			get
			{
				if (this._entryPool == null)
				{
					this._entryPool = new PrefabPool<DifficultySelection_Entry>(this.entryTemplate, null, null, null, null, true, 10, 10000, null);
				}
				return this._entryPool;
			}
		}

		// Token: 0x06002458 RID: 9304 RVA: 0x0007E31D File Offset: 0x0007C51D
		private void Awake()
		{
			this.confirmButton.onClick.AddListener(new UnityAction(this.OnConfirmButtonClicked));
		}

		// Token: 0x06002459 RID: 9305 RVA: 0x0007E33B File Offset: 0x0007C53B
		private void OnConfirmButtonClicked()
		{
			this.confirmed = true;
		}

		// Token: 0x0600245A RID: 9306 RVA: 0x0007E344 File Offset: 0x0007C544
		public async UniTask Execute()
		{
			this.EntryPool.ReleaseAll();
			this.fadeGroup.Show();
			foreach (DifficultySelection.SettingEntry settingEntry in this.displaySettings)
			{
				if (this.CheckShouldDisplay(settingEntry))
				{
					DifficultySelection_Entry difficultySelection_Entry = this.EntryPool.Get(null);
					bool flag = !this.CheckUnlocked(settingEntry);
					difficultySelection_Entry.Setup(this, settingEntry, flag);
				}
			}
			foreach (DifficultySelection_Entry difficultySelection_Entry2 in this.EntryPool.ActiveEntries)
			{
				if (difficultySelection_Entry2.Setting.ruleIndex == GameRulesManager.SelectedRuleIndex)
				{
					this.NotifySelected(difficultySelection_Entry2);
					break;
				}
			}
			if ((GameRulesManager.SelectedRuleIndex = await this.WaitForConfirmation()) == RuleIndex.Custom)
			{
				DifficultySelection.CustomDifficultyMarker = true;
			}
			await this.fadeGroup.HideAndReturnTask();
		}

		// Token: 0x0600245B RID: 9307 RVA: 0x0007E388 File Offset: 0x0007C588
		private bool CheckUnlocked(DifficultySelection.SettingEntry setting)
		{
			bool flag = !MultiSceneCore.GetVisited("Base");
			RuleIndex ruleIndex = setting.ruleIndex;
			if (ruleIndex <= RuleIndex.Custom)
			{
				if (ruleIndex != RuleIndex.Standard)
				{
					if (ruleIndex != RuleIndex.Custom)
					{
						return false;
					}
					return flag || GameRulesManager.SelectedRuleIndex == RuleIndex.Custom;
				}
			}
			else if (ruleIndex - RuleIndex.Easy > 2 && ruleIndex - RuleIndex.Hard > 1)
			{
				if (ruleIndex != RuleIndex.Rage)
				{
					return false;
				}
				return this.GetRageUnlocked(flag);
			}
			return flag || (GameRulesManager.SelectedRuleIndex != RuleIndex.Custom && GameRulesManager.SelectedRuleIndex != RuleIndex.Rage);
		}

		// Token: 0x0600245C RID: 9308 RVA: 0x0007E3FF File Offset: 0x0007C5FF
		public static void UnlockRage()
		{
			SavesSystem.SaveGlobal<bool>("Difficulty/RageUnlocked", true);
		}

		// Token: 0x0600245D RID: 9309 RVA: 0x0007E40C File Offset: 0x0007C60C
		public bool GetRageUnlocked(bool isFirstSelect)
		{
			return SavesSystem.LoadGlobal<bool>("Difficulty/RageUnlocked", false) && (isFirstSelect || (GameRulesManager.SelectedRuleIndex != RuleIndex.Custom && GameRulesManager.SelectedRuleIndex == RuleIndex.Rage));
		}

		// Token: 0x0600245E RID: 9310 RVA: 0x0007E438 File Offset: 0x0007C638
		private bool CheckShouldDisplay(DifficultySelection.SettingEntry setting)
		{
			return true;
		}

		// Token: 0x170006EE RID: 1774
		// (get) Token: 0x0600245F RID: 9311 RVA: 0x0007E43B File Offset: 0x0007C63B
		// (set) Token: 0x06002460 RID: 9312 RVA: 0x0007E447 File Offset: 0x0007C647
		public static bool CustomDifficultyMarker
		{
			get
			{
				return SavesSystem.Load<bool>("CustomDifficultyMarker");
			}
			set
			{
				SavesSystem.Save<bool>("CustomDifficultyMarker", value);
			}
		}

		// Token: 0x170006EF RID: 1775
		// (get) Token: 0x06002461 RID: 9313 RVA: 0x0007E454 File Offset: 0x0007C654
		public RuleIndex SelectedRuleIndex
		{
			get
			{
				if (this.SelectedEntry == null)
				{
					return RuleIndex.Standard;
				}
				return this.SelectedEntry.Setting.ruleIndex;
			}
		}

		// Token: 0x170006F0 RID: 1776
		// (get) Token: 0x06002462 RID: 9314 RVA: 0x0007E476 File Offset: 0x0007C676
		// (set) Token: 0x06002463 RID: 9315 RVA: 0x0007E47E File Offset: 0x0007C67E
		public DifficultySelection_Entry SelectedEntry { get; private set; }

		// Token: 0x170006F1 RID: 1777
		// (get) Token: 0x06002464 RID: 9316 RVA: 0x0007E487 File Offset: 0x0007C687
		// (set) Token: 0x06002465 RID: 9317 RVA: 0x0007E48F File Offset: 0x0007C68F
		public DifficultySelection_Entry HoveringEntry { get; private set; }

		// Token: 0x06002466 RID: 9318 RVA: 0x0007E498 File Offset: 0x0007C698
		private async UniTask<RuleIndex> WaitForConfirmation()
		{
			this.confirmed = false;
			while (!this.confirmed)
			{
				await UniTask.Yield();
			}
			return this.SelectedRuleIndex;
		}

		// Token: 0x06002467 RID: 9319 RVA: 0x0007E4DC File Offset: 0x0007C6DC
		internal void NotifySelected(DifficultySelection_Entry entry)
		{
			this.SelectedEntry = entry;
			GameRulesManager.SelectedRuleIndex = this.SelectedRuleIndex;
			foreach (DifficultySelection_Entry difficultySelection_Entry in this.EntryPool.ActiveEntries)
			{
				if (!(difficultySelection_Entry == null))
				{
					difficultySelection_Entry.Refresh();
				}
			}
			this.RefreshDescription();
			if (this.SelectedRuleIndex == RuleIndex.Custom)
			{
				this.ShowCustomRuleSetupPanel();
			}
			bool flag = this.SelectedRuleIndex == RuleIndex.Custom;
			this.achievementDisabledIndicator.SetActive(flag || DifficultySelection.CustomDifficultyMarker);
			this.selectedCustomDifficultyBefore.SetActive(DifficultySelection.CustomDifficultyMarker);
		}

		// Token: 0x06002468 RID: 9320 RVA: 0x0007E590 File Offset: 0x0007C790
		private void ShowCustomRuleSetupPanel()
		{
			FadeGroup fadeGroup = this.customPanel;
			if (fadeGroup == null)
			{
				return;
			}
			fadeGroup.Show();
		}

		// Token: 0x06002469 RID: 9321 RVA: 0x0007E5A2 File Offset: 0x0007C7A2
		internal void NotifyEntryPointerEnter(DifficultySelection_Entry entry)
		{
			this.HoveringEntry = entry;
			this.RefreshDescription();
		}

		// Token: 0x0600246A RID: 9322 RVA: 0x0007E5B1 File Offset: 0x0007C7B1
		internal void NotifyEntryPointerExit(DifficultySelection_Entry entry)
		{
			if (this.HoveringEntry == entry)
			{
				this.HoveringEntry = null;
				this.RefreshDescription();
			}
		}

		// Token: 0x0600246B RID: 9323 RVA: 0x0007E5D0 File Offset: 0x0007C7D0
		private void RefreshDescription()
		{
			string text;
			if (this.SelectedEntry != null)
			{
				text = this.SelectedEntry.Setting.Description;
			}
			else
			{
				text = this.description_PlaceHolderKey.ToPlainText();
			}
			this.textDescription.text = text;
		}

		// Token: 0x0600246C RID: 9324 RVA: 0x0007E619 File Offset: 0x0007C819
		internal void SkipHide()
		{
			if (this.fadeGroup != null)
			{
				this.fadeGroup.SkipHide();
			}
		}

		// Token: 0x040018C4 RID: 6340
		[SerializeField]
		private FadeGroup fadeGroup;

		// Token: 0x040018C5 RID: 6341
		[SerializeField]
		private TextMeshProUGUI textDescription;

		// Token: 0x040018C6 RID: 6342
		[SerializeField]
		[LocalizationKey("Default")]
		private string description_PlaceHolderKey = "DifficultySelection_Desc_PlaceHolder";

		// Token: 0x040018C7 RID: 6343
		[SerializeField]
		private Button confirmButton;

		// Token: 0x040018C8 RID: 6344
		[SerializeField]
		private FadeGroup customPanel;

		// Token: 0x040018C9 RID: 6345
		[SerializeField]
		private DifficultySelection_Entry entryTemplate;

		// Token: 0x040018CA RID: 6346
		[SerializeField]
		private GameObject achievementDisabledIndicator;

		// Token: 0x040018CB RID: 6347
		[SerializeField]
		private GameObject selectedCustomDifficultyBefore;

		// Token: 0x040018CC RID: 6348
		private PrefabPool<DifficultySelection_Entry> _entryPool;

		// Token: 0x040018CD RID: 6349
		[SerializeField]
		private DifficultySelection.SettingEntry[] displaySettings;

		// Token: 0x040018D0 RID: 6352
		private bool confirmed;

		// Token: 0x0200064B RID: 1611
		[Serializable]
		public struct SettingEntry
		{
			// Token: 0x1700078D RID: 1933
			// (get) Token: 0x06002A01 RID: 10753 RVA: 0x0009F52E File Offset: 0x0009D72E
			// (set) Token: 0x06002A02 RID: 10754 RVA: 0x0009F545 File Offset: 0x0009D745
			[LocalizationKey("Default")]
			private string TitleKey
			{
				get
				{
					return string.Format("Rule_{0}", this.ruleIndex);
				}
				set
				{
				}
			}

			// Token: 0x1700078E RID: 1934
			// (get) Token: 0x06002A03 RID: 10755 RVA: 0x0009F547 File Offset: 0x0009D747
			public string Title
			{
				get
				{
					return this.TitleKey.ToPlainText();
				}
			}

			// Token: 0x1700078F RID: 1935
			// (get) Token: 0x06002A04 RID: 10756 RVA: 0x0009F554 File Offset: 0x0009D754
			// (set) Token: 0x06002A05 RID: 10757 RVA: 0x0009F56B File Offset: 0x0009D76B
			[LocalizationKey("Default")]
			private string DescriptionKey
			{
				get
				{
					return string.Format("Rule_{0}_Desc", this.ruleIndex);
				}
				set
				{
				}
			}

			// Token: 0x17000790 RID: 1936
			// (get) Token: 0x06002A06 RID: 10758 RVA: 0x0009F56D File Offset: 0x0009D76D
			public string Description
			{
				get
				{
					return this.DescriptionKey.ToPlainText();
				}
			}

			// Token: 0x04002279 RID: 8825
			public RuleIndex ruleIndex;

			// Token: 0x0400227A RID: 8826
			public Sprite icon;

			// Token: 0x0400227B RID: 8827
			public bool recommended;
		}
	}
}
