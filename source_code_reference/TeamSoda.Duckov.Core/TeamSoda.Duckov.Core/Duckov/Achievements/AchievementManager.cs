using System;
using System.Collections.Generic;
using System.Linq;
using Duckov.Economy;
using Duckov.Endowment;
using Duckov.Quests;
using Duckov.Rules.UI;
using Duckov.Scenes;
using Saves;
using UnityEngine;

namespace Duckov.Achievements
{
	// Token: 0x0200031F RID: 799
	public class AchievementManager : MonoBehaviour
	{
		// Token: 0x170004DC RID: 1244
		// (get) Token: 0x06001A86 RID: 6790 RVA: 0x0005FE53 File Offset: 0x0005E053
		public static AchievementManager Instance
		{
			get
			{
				return GameManager.AchievementManager;
			}
		}

		// Token: 0x170004DD RID: 1245
		// (get) Token: 0x06001A87 RID: 6791 RVA: 0x0005FE5A File Offset: 0x0005E05A
		public static bool CanUnlockAchievement
		{
			get
			{
				return !DifficultySelection.CustomDifficultyMarker;
			}
		}

		// Token: 0x170004DE RID: 1246
		// (get) Token: 0x06001A88 RID: 6792 RVA: 0x0005FE66 File Offset: 0x0005E066
		public List<string> UnlockedAchievements
		{
			get
			{
				return this._unlockedAchievements;
			}
		}

		// Token: 0x140000AF RID: 175
		// (add) Token: 0x06001A89 RID: 6793 RVA: 0x0005FE70 File Offset: 0x0005E070
		// (remove) Token: 0x06001A8A RID: 6794 RVA: 0x0005FEA4 File Offset: 0x0005E0A4
		public static event Action<AchievementManager> OnAchievementDataLoaded;

		// Token: 0x140000B0 RID: 176
		// (add) Token: 0x06001A8B RID: 6795 RVA: 0x0005FED8 File Offset: 0x0005E0D8
		// (remove) Token: 0x06001A8C RID: 6796 RVA: 0x0005FF0C File Offset: 0x0005E10C
		public static event Action<string> OnAchievementUnlocked;

		// Token: 0x06001A8D RID: 6797 RVA: 0x0005FF3F File Offset: 0x0005E13F
		private void Awake()
		{
			this.Load();
			this.RegisterEvents();
		}

		// Token: 0x06001A8E RID: 6798 RVA: 0x0005FF4D File Offset: 0x0005E14D
		private void OnDestroy()
		{
			this.UnregisterEvents();
		}

		// Token: 0x06001A8F RID: 6799 RVA: 0x0005FF55 File Offset: 0x0005E155
		private void Start()
		{
			this.MakeSureMoneyAchievementsUnlocked();
		}

		// Token: 0x06001A90 RID: 6800 RVA: 0x0005FF60 File Offset: 0x0005E160
		private void RegisterEvents()
		{
			Quest.onQuestCompleted += this.OnQuestCompleted;
			SavesCounter.OnKillCountChanged = (Action<string, int>)Delegate.Combine(SavesCounter.OnKillCountChanged, new Action<string, int>(this.OnKillCountChanged));
			MultiSceneCore.OnSetSceneVisited += this.OnSetSceneVisited;
			LevelManager.OnEvacuated += this.OnEvacuated;
			EconomyManager.OnMoneyChanged += this.OnMoneyChanged;
			EndowmentManager.OnEndowmentUnlock = (Action<EndowmentIndex>)Delegate.Combine(EndowmentManager.OnEndowmentUnlock, new Action<EndowmentIndex>(this.OnEndowmentUnlocked));
			EconomyManager.OnEconomyManagerLoaded += this.OnEconomyManagerLoaded;
		}

		// Token: 0x06001A91 RID: 6801 RVA: 0x00060004 File Offset: 0x0005E204
		private void UnregisterEvents()
		{
			Quest.onQuestCompleted -= this.OnQuestCompleted;
			SavesCounter.OnKillCountChanged = (Action<string, int>)Delegate.Remove(SavesCounter.OnKillCountChanged, new Action<string, int>(this.OnKillCountChanged));
			MultiSceneCore.OnSetSceneVisited -= this.OnSetSceneVisited;
			LevelManager.OnEvacuated -= this.OnEvacuated;
			EconomyManager.OnMoneyChanged -= this.OnMoneyChanged;
			EndowmentManager.OnEndowmentUnlock = (Action<EndowmentIndex>)Delegate.Remove(EndowmentManager.OnEndowmentUnlock, new Action<EndowmentIndex>(this.OnEndowmentUnlocked));
			EconomyManager.OnEconomyManagerLoaded -= this.OnEconomyManagerLoaded;
		}

		// Token: 0x06001A92 RID: 6802 RVA: 0x000600A6 File Offset: 0x0005E2A6
		private void OnEconomyManagerLoaded()
		{
			this.MakeSureMoneyAchievementsUnlocked();
		}

		// Token: 0x06001A93 RID: 6803 RVA: 0x000600AE File Offset: 0x0005E2AE
		private void OnEndowmentUnlocked(EndowmentIndex index)
		{
			this.Unlock(string.Format("Endowmment_{0}", index));
		}

		// Token: 0x06001A94 RID: 6804 RVA: 0x000600C6 File Offset: 0x0005E2C6
		public static void UnlockEndowmentAchievement(EndowmentIndex index)
		{
			if (AchievementManager.Instance == null)
			{
				return;
			}
			AchievementManager.Instance.Unlock(string.Format("Endowmment_{0}", index));
		}

		// Token: 0x06001A95 RID: 6805 RVA: 0x000600F0 File Offset: 0x0005E2F0
		private void OnMoneyChanged(long oldValue, long newValue)
		{
			if (oldValue < 10000L && newValue >= 10000L)
			{
				this.Unlock("Money_10K");
			}
			if (oldValue < 100000L && newValue >= 100000L)
			{
				this.Unlock("Money_100K");
			}
			if (oldValue < 1000000L && newValue >= 1000000L)
			{
				this.Unlock("Money_1M");
			}
		}

		// Token: 0x06001A96 RID: 6806 RVA: 0x00060154 File Offset: 0x0005E354
		private void MakeSureMoneyAchievementsUnlocked()
		{
			long money = EconomyManager.Money;
			if (money >= 10000L)
			{
				this.Unlock("Money_10K");
			}
			if (money >= 100000L)
			{
				this.Unlock("Money_100K");
			}
			if (money >= 1000000L)
			{
				this.Unlock("Money_1M");
			}
		}

		// Token: 0x06001A97 RID: 6807 RVA: 0x000601A4 File Offset: 0x0005E3A4
		private void OnEvacuated(EvacuationInfo info)
		{
			string mainSceneID = MultiSceneCore.MainSceneID;
			if (!this.evacuateSceneIDs.Contains(mainSceneID))
			{
				return;
			}
			this.Unlock("Evacuate_" + mainSceneID);
		}

		// Token: 0x06001A98 RID: 6808 RVA: 0x000601D7 File Offset: 0x0005E3D7
		private void OnSetSceneVisited(string id)
		{
			if (!this.achievementSceneIDs.Contains(id))
			{
				return;
			}
			this.Unlock("Arrive_" + id);
		}

		// Token: 0x06001A99 RID: 6809 RVA: 0x000601FC File Offset: 0x0005E3FC
		private void OnKillCountChanged(string key, int value)
		{
			this.Unlock("FirstBlood");
			if (AchievementDatabase.Instance == null)
			{
				return;
			}
			Debug.Log("COUNTING " + key);
			foreach (AchievementManager.KillCountAchievement killCountAchievement in this.KillCountAchivements)
			{
				if (killCountAchievement.key == key && value >= killCountAchievement.value)
				{
					this.Unlock(string.Format("Kill_{0}_{1}", key, killCountAchievement.value));
				}
			}
		}

		// Token: 0x06001A9A RID: 6810 RVA: 0x00060284 File Offset: 0x0005E484
		private void OnQuestCompleted(Quest quest)
		{
			if (AchievementDatabase.Instance == null)
			{
				return;
			}
			string text = string.Format("Quest_{0}", quest.ID);
			AchievementDatabase.Achievement achievement;
			if (!AchievementDatabase.TryGetAchievementData(text, out achievement))
			{
				return;
			}
			this.Unlock(text);
		}

		// Token: 0x06001A9B RID: 6811 RVA: 0x000602C7 File Offset: 0x0005E4C7
		private void Save()
		{
			SavesSystem.SaveGlobal<List<string>>("Achievements", this.UnlockedAchievements);
		}

		// Token: 0x06001A9C RID: 6812 RVA: 0x000602DC File Offset: 0x0005E4DC
		private void Load()
		{
			this.UnlockedAchievements.Clear();
			List<string> list = SavesSystem.LoadGlobal<List<string>>("Achievements", null);
			if (list != null)
			{
				this.UnlockedAchievements.AddRange(list);
			}
			Action<AchievementManager> onAchievementDataLoaded = AchievementManager.OnAchievementDataLoaded;
			if (onAchievementDataLoaded == null)
			{
				return;
			}
			onAchievementDataLoaded(this);
		}

		// Token: 0x06001A9D RID: 6813 RVA: 0x00060320 File Offset: 0x0005E520
		public void Unlock(string id)
		{
			if (string.IsNullOrWhiteSpace(id))
			{
				Debug.LogError("Trying to unlock a empty acheivement.", this);
				return;
			}
			id = id.Trim();
			AchievementDatabase.Achievement achievement;
			if (!AchievementDatabase.TryGetAchievementData(id, out achievement))
			{
				Debug.LogError("Invalid acheivement id: " + id);
			}
			if (this.UnlockedAchievements.Contains(id))
			{
				return;
			}
			if (!AchievementManager.CanUnlockAchievement)
			{
				return;
			}
			this.UnlockedAchievements.Add(id);
			this.Save();
			Action<string> onAchievementUnlocked = AchievementManager.OnAchievementUnlocked;
			if (onAchievementUnlocked == null)
			{
				return;
			}
			onAchievementUnlocked(id);
		}

		// Token: 0x06001A9E RID: 6814 RVA: 0x0006039C File Offset: 0x0005E59C
		public static bool IsIDValid(string id)
		{
			return !(AchievementDatabase.Instance == null) && AchievementDatabase.Instance.IsIDValid(id);
		}

		// Token: 0x04001304 RID: 4868
		private List<string> _unlockedAchievements = new List<string>();

		// Token: 0x04001307 RID: 4871
		private readonly string[] evacuateSceneIDs = new string[] { "Level_GroundZero_Main" };

		// Token: 0x04001308 RID: 4872
		private readonly string[] achievementSceneIDs = new string[] { "Base", "Level_GroundZero_Main", "Level_HiddenWarehouse_Main", "Level_Farm_Main", "Level_JLab_Main", "Level_StormZone_Main" };

		// Token: 0x04001309 RID: 4873
		private readonly AchievementManager.KillCountAchievement[] KillCountAchivements = new AchievementManager.KillCountAchievement[]
		{
			new AchievementManager.KillCountAchievement("Cname_ShortEagle", 10),
			new AchievementManager.KillCountAchievement("Cname_ShortEagle", 1),
			new AchievementManager.KillCountAchievement("Cname_Speedy", 1),
			new AchievementManager.KillCountAchievement("Cname_StormBoss1", 1),
			new AchievementManager.KillCountAchievement("Cname_StormBoss2", 1),
			new AchievementManager.KillCountAchievement("Cname_StormBoss3", 1),
			new AchievementManager.KillCountAchievement("Cname_StormBoss4", 1),
			new AchievementManager.KillCountAchievement("Cname_StormBoss5", 1),
			new AchievementManager.KillCountAchievement("Cname_Boss_Sniper", 1),
			new AchievementManager.KillCountAchievement("Cname_Vida", 1),
			new AchievementManager.KillCountAchievement("Cname_Roadblock", 1),
			new AchievementManager.KillCountAchievement("Cname_SchoolBully", 1),
			new AchievementManager.KillCountAchievement("Cname_Boss_Fly", 1),
			new AchievementManager.KillCountAchievement("Cname_Boss_Arcade", 1),
			new AchievementManager.KillCountAchievement("Cname_UltraMan", 1),
			new AchievementManager.KillCountAchievement("Cname_LabTestObjective", 1)
		};

		// Token: 0x020005BC RID: 1468
		private struct KillCountAchievement
		{
			// Token: 0x060028D4 RID: 10452 RVA: 0x00096FC4 File Offset: 0x000951C4
			public KillCountAchievement(string key, int value)
			{
				this.key = key;
				this.value = value;
			}

			// Token: 0x04002064 RID: 8292
			public string key;

			// Token: 0x04002065 RID: 8293
			public int value;
		}
	}
}
