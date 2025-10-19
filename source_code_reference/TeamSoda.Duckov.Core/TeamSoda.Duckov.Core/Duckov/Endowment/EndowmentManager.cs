using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Duckov.Achievements;
using Saves;
using UnityEngine;

namespace Duckov.Endowment
{
	// Token: 0x020002F1 RID: 753
	public class EndowmentManager : MonoBehaviour
	{
		// Token: 0x1700046E RID: 1134
		// (get) Token: 0x0600186E RID: 6254 RVA: 0x0005917D File Offset: 0x0005737D
		// (set) Token: 0x0600186F RID: 6255 RVA: 0x00059184 File Offset: 0x00057384
		private static EndowmentManager _instance { get; set; }

		// Token: 0x1700046F RID: 1135
		// (get) Token: 0x06001870 RID: 6256 RVA: 0x0005918C File Offset: 0x0005738C
		public static EndowmentManager Instance
		{
			get
			{
				if (EndowmentManager._instance == null)
				{
					GameManager instance = GameManager.Instance;
				}
				return EndowmentManager._instance;
			}
		}

		// Token: 0x17000470 RID: 1136
		// (get) Token: 0x06001871 RID: 6257 RVA: 0x000591A6 File Offset: 0x000573A6
		// (set) Token: 0x06001872 RID: 6258 RVA: 0x000591B2 File Offset: 0x000573B2
		public static EndowmentIndex SelectedIndex
		{
			get
			{
				return SavesSystem.Load<EndowmentIndex>("Endowment_SelectedIndex");
			}
			private set
			{
				SavesSystem.Save<EndowmentIndex>("Endowment_SelectedIndex", value);
			}
		}

		// Token: 0x17000471 RID: 1137
		// (get) Token: 0x06001873 RID: 6259 RVA: 0x000591BF File Offset: 0x000573BF
		public ReadOnlyCollection<EndowmentEntry> Entries
		{
			get
			{
				if (this._entries_ReadOnly == null)
				{
					this._entries_ReadOnly = new ReadOnlyCollection<EndowmentEntry>(this.entries);
				}
				return this._entries_ReadOnly;
			}
		}

		// Token: 0x17000472 RID: 1138
		// (get) Token: 0x06001874 RID: 6260 RVA: 0x000591E0 File Offset: 0x000573E0
		public static EndowmentEntry Current
		{
			get
			{
				if (EndowmentManager._instance == null)
				{
					return null;
				}
				return EndowmentManager._instance.entries.Find((EndowmentEntry e) => e != null && e.Index == EndowmentManager.SelectedIndex);
			}
		}

		// Token: 0x17000473 RID: 1139
		// (get) Token: 0x06001875 RID: 6261 RVA: 0x0005921F File Offset: 0x0005741F
		public static EndowmentIndex CurrentIndex
		{
			get
			{
				if (EndowmentManager.Current == null)
				{
					return EndowmentIndex.None;
				}
				return EndowmentManager.Current.Index;
			}
		}

		// Token: 0x06001876 RID: 6262 RVA: 0x0005923C File Offset: 0x0005743C
		private EndowmentEntry GetEntry(EndowmentIndex index)
		{
			return this.entries.Find((EndowmentEntry e) => e != null && e.Index == index);
		}

		// Token: 0x06001877 RID: 6263 RVA: 0x0005926D File Offset: 0x0005746D
		private static string GetUnlockKey(EndowmentIndex index)
		{
			return string.Format("Endowment_Unlock_R_{0}", index);
		}

		// Token: 0x06001878 RID: 6264 RVA: 0x0005927F File Offset: 0x0005747F
		public static bool GetEndowmentUnlocked(EndowmentIndex index)
		{
			if (EndowmentManager.Instance != null)
			{
				if (EndowmentManager.Instance.GetEntry(index).UnlockedByDefault)
				{
					return true;
				}
			}
			else
			{
				Debug.LogError("Endowment Manager 不存在。");
			}
			return SavesSystem.LoadGlobal<bool>(EndowmentManager.GetUnlockKey(index), false);
		}

		// Token: 0x06001879 RID: 6265 RVA: 0x000592B8 File Offset: 0x000574B8
		private static void SetEndowmentUnlocked(EndowmentIndex index, bool value = true)
		{
			SavesSystem.SaveGlobal<bool>(EndowmentManager.GetUnlockKey(index), value);
		}

		// Token: 0x0600187A RID: 6266 RVA: 0x000592C8 File Offset: 0x000574C8
		public static bool UnlockEndowment(EndowmentIndex index)
		{
			try
			{
				Action<EndowmentIndex> onEndowmentUnlock = EndowmentManager.OnEndowmentUnlock;
				if (onEndowmentUnlock != null)
				{
					onEndowmentUnlock(index);
				}
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
			if (EndowmentManager.GetEndowmentUnlocked(index))
			{
				Debug.Log("尝试解锁天赋，但天赋已经解锁");
				return false;
			}
			EndowmentManager.SetEndowmentUnlocked(index, true);
			return true;
		}

		// Token: 0x0600187B RID: 6267 RVA: 0x0005931C File Offset: 0x0005751C
		private void Awake()
		{
			if (EndowmentManager._instance != null)
			{
				Debug.LogError("检测到多个Endowment Manager");
				return;
			}
			EndowmentManager._instance = this;
			if (LevelManager.LevelInited)
			{
				this.ApplyCurrentEndowment();
			}
			LevelManager.OnLevelInitialized += this.OnLevelInitialized;
		}

		// Token: 0x0600187C RID: 6268 RVA: 0x0005935A File Offset: 0x0005755A
		private void OnDestroy()
		{
			LevelManager.OnLevelInitialized -= this.OnLevelInitialized;
		}

		// Token: 0x0600187D RID: 6269 RVA: 0x0005936D File Offset: 0x0005756D
		private void OnLevelInitialized()
		{
			this.ApplyCurrentEndowment();
			this.MakeSureEndowmentAchievementsUnlocked();
		}

		// Token: 0x0600187E RID: 6270 RVA: 0x0005937C File Offset: 0x0005757C
		private void MakeSureEndowmentAchievementsUnlocked()
		{
			for (int i = 0; i < 5; i++)
			{
				EndowmentIndex endowmentIndex = (EndowmentIndex)i;
				EndowmentEntry entry = EndowmentManager.Instance.GetEntry(endowmentIndex);
				if (!(entry == null) && !entry.UnlockedByDefault && EndowmentManager.GetEndowmentUnlocked(endowmentIndex))
				{
					AchievementManager.UnlockEndowmentAchievement(endowmentIndex);
				}
			}
		}

		// Token: 0x0600187F RID: 6271 RVA: 0x000593C4 File Offset: 0x000575C4
		private void ApplyCurrentEndowment()
		{
			if (!LevelManager.LevelInited)
			{
				return;
			}
			foreach (EndowmentEntry endowmentEntry in this.entries)
			{
				if (!(endowmentEntry == null))
				{
					endowmentEntry.Deactivate();
				}
			}
			EndowmentEntry endowmentEntry2 = EndowmentManager.Current;
			if (endowmentEntry2 == null)
			{
				return;
			}
			endowmentEntry2.Activate();
		}

		// Token: 0x06001880 RID: 6272 RVA: 0x00059440 File Offset: 0x00057640
		internal void SelectIndex(EndowmentIndex index)
		{
			EndowmentManager.SelectedIndex = index;
			this.ApplyCurrentEndowment();
			Action<EndowmentIndex> onEndowmentChanged = EndowmentManager.OnEndowmentChanged;
			if (onEndowmentChanged == null)
			{
				return;
			}
			onEndowmentChanged(index);
		}

		// Token: 0x040011D1 RID: 4561
		private const string SaveKey = "Endowment_SelectedIndex";

		// Token: 0x040011D2 RID: 4562
		public static Action<EndowmentIndex> OnEndowmentChanged;

		// Token: 0x040011D3 RID: 4563
		public static Action<EndowmentIndex> OnEndowmentUnlock;

		// Token: 0x040011D4 RID: 4564
		[SerializeField]
		private List<EndowmentEntry> entries = new List<EndowmentEntry>();

		// Token: 0x040011D5 RID: 4565
		private ReadOnlyCollection<EndowmentEntry> _entries_ReadOnly;
	}
}
