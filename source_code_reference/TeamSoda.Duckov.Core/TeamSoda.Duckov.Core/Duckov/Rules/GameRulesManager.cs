using System;
using Saves;
using SodaCraft.Localizations;
using UnityEngine;

namespace Duckov.Rules
{
	// Token: 0x020003EC RID: 1004
	public class GameRulesManager : MonoBehaviour
	{
		// Token: 0x170006DA RID: 1754
		// (get) Token: 0x06002435 RID: 9269 RVA: 0x0007DFB4 File Offset: 0x0007C1B4
		public static GameRulesManager Instance
		{
			get
			{
				return GameManager.DifficultyManager;
			}
		}

		// Token: 0x170006DB RID: 1755
		// (get) Token: 0x06002436 RID: 9270 RVA: 0x0007DFBB File Offset: 0x0007C1BB
		public static Ruleset Current
		{
			get
			{
				return GameRulesManager.Instance.mCurrent;
			}
		}

		// Token: 0x140000F2 RID: 242
		// (add) Token: 0x06002437 RID: 9271 RVA: 0x0007DFC8 File Offset: 0x0007C1C8
		// (remove) Token: 0x06002438 RID: 9272 RVA: 0x0007DFFC File Offset: 0x0007C1FC
		public static event Action OnRuleChanged;

		// Token: 0x06002439 RID: 9273 RVA: 0x0007E02F File Offset: 0x0007C22F
		public static void NotifyRuleChanged()
		{
			Action onRuleChanged = GameRulesManager.OnRuleChanged;
			if (onRuleChanged == null)
			{
				return;
			}
			onRuleChanged();
		}

		// Token: 0x170006DC RID: 1756
		// (get) Token: 0x0600243A RID: 9274 RVA: 0x0007E040 File Offset: 0x0007C240
		private Ruleset mCurrent
		{
			get
			{
				if (GameRulesManager.SelectedRuleIndex == RuleIndex.Custom)
				{
					return this.CustomRuleSet;
				}
				foreach (GameRulesManager.RuleIndexFileEntry ruleIndexFileEntry in this.entries)
				{
					if (ruleIndexFileEntry.index == GameRulesManager.SelectedRuleIndex)
					{
						return ruleIndexFileEntry.file.Data;
					}
				}
				return this.entries[0].file.Data;
			}
		}

		// Token: 0x170006DD RID: 1757
		// (get) Token: 0x0600243B RID: 9275 RVA: 0x0007E0A8 File Offset: 0x0007C2A8
		// (set) Token: 0x0600243C RID: 9276 RVA: 0x0007E0C2 File Offset: 0x0007C2C2
		public static RuleIndex SelectedRuleIndex
		{
			get
			{
				if (SavesSystem.KeyExisits("GameRulesManager_RuleIndex"))
				{
					return SavesSystem.Load<RuleIndex>("GameRulesManager_RuleIndex");
				}
				return RuleIndex.Standard;
			}
			internal set
			{
				SavesSystem.Save<RuleIndex>("GameRulesManager_RuleIndex", value);
				GameRulesManager.NotifyRuleChanged();
			}
		}

		// Token: 0x0600243D RID: 9277 RVA: 0x0007E0D4 File Offset: 0x0007C2D4
		public static RuleIndex GetRuleIndexOfSaveSlot(int slot)
		{
			return SavesSystem.Load<RuleIndex>("GameRulesManager_RuleIndex", slot);
		}

		// Token: 0x170006DE RID: 1758
		// (get) Token: 0x0600243E RID: 9278 RVA: 0x0007E0E1 File Offset: 0x0007C2E1
		private Ruleset CustomRuleSet
		{
			get
			{
				if (this.customRuleSet == null)
				{
					this.ReloadCustomRuleSet();
				}
				return this.customRuleSet;
			}
		}

		// Token: 0x0600243F RID: 9279 RVA: 0x0007E0F7 File Offset: 0x0007C2F7
		private void Awake()
		{
			SavesSystem.OnCollectSaveData += this.OnCollectSaveData;
			SavesSystem.OnSetFile += this.OnSetFile;
		}

		// Token: 0x06002440 RID: 9280 RVA: 0x0007E11B File Offset: 0x0007C31B
		private void OnDestroy()
		{
			SavesSystem.OnCollectSaveData -= this.OnCollectSaveData;
			SavesSystem.OnSetFile -= this.OnSetFile;
		}

		// Token: 0x06002441 RID: 9281 RVA: 0x0007E13F File Offset: 0x0007C33F
		private void OnSetFile()
		{
			this.ReloadCustomRuleSet();
		}

		// Token: 0x06002442 RID: 9282 RVA: 0x0007E148 File Offset: 0x0007C348
		private void ReloadCustomRuleSet()
		{
			if (SavesSystem.KeyExisits("Rule_Custom"))
			{
				this.customRuleSet = SavesSystem.Load<Ruleset>("Rule_Custom");
			}
			if (this.customRuleSet == null)
			{
				this.customRuleSet = new Ruleset();
				this.customRuleSet.displayNameKey = "Rule_Custom";
			}
		}

		// Token: 0x06002443 RID: 9283 RVA: 0x0007E194 File Offset: 0x0007C394
		private void OnCollectSaveData()
		{
			if (GameRulesManager.SelectedRuleIndex == RuleIndex.Custom && this.customRuleSet != null)
			{
				SavesSystem.Save<Ruleset>("Rule_Custom", this.customRuleSet);
			}
		}

		// Token: 0x06002444 RID: 9284 RVA: 0x0007E1B8 File Offset: 0x0007C3B8
		internal static string GetRuleIndexDisplayNameOfSlot(int slotIndex)
		{
			RuleIndex ruleIndexOfSaveSlot = GameRulesManager.GetRuleIndexOfSaveSlot(slotIndex);
			return string.Format("Rule_{0}", ruleIndexOfSaveSlot).ToPlainText();
		}

		// Token: 0x040018AB RID: 6315
		private const string SelectedRuleIndexSaveKey = "GameRulesManager_RuleIndex";

		// Token: 0x040018AC RID: 6316
		private Ruleset customRuleSet;

		// Token: 0x040018AD RID: 6317
		private const string CustomRuleSetKey = "Rule_Custom";

		// Token: 0x040018AE RID: 6318
		[SerializeField]
		private GameRulesManager.RuleIndexFileEntry[] entries;

		// Token: 0x0200064A RID: 1610
		[Serializable]
		private struct RuleIndexFileEntry
		{
			// Token: 0x04002277 RID: 8823
			public RuleIndex index;

			// Token: 0x04002278 RID: 8824
			public RulesetFile file;
		}
	}
}
