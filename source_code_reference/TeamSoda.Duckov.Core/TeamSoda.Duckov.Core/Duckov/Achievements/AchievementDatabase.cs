using System;
using System.Collections.Generic;
using System.IO;
using Duckov.Utilities;
using MiniExcelLibs;
using SodaCraft.Localizations;
using UnityEngine;

namespace Duckov.Achievements
{
	// Token: 0x0200031E RID: 798
	[CreateAssetMenu]
	public class AchievementDatabase : ScriptableObject
	{
		// Token: 0x170004DA RID: 1242
		// (get) Token: 0x06001A80 RID: 6784 RVA: 0x0005FD36 File Offset: 0x0005DF36
		public static AchievementDatabase Instance
		{
			get
			{
				return GameplayDataSettings.AchievementDatabase;
			}
		}

		// Token: 0x170004DB RID: 1243
		// (get) Token: 0x06001A81 RID: 6785 RVA: 0x0005FD3D File Offset: 0x0005DF3D
		private Dictionary<string, AchievementDatabase.Achievement> dic
		{
			get
			{
				if (this._dic == null)
				{
					this.RebuildDictionary();
				}
				return this._dic;
			}
		}

		// Token: 0x06001A82 RID: 6786 RVA: 0x0005FD54 File Offset: 0x0005DF54
		private void RebuildDictionary()
		{
			if (this._dic == null)
			{
				this._dic = new Dictionary<string, AchievementDatabase.Achievement>();
			}
			this._dic.Clear();
			if (this.achievementChart == null)
			{
				Debug.LogError("Achievement Chart is not assinged", this);
				return;
			}
			using (MemoryStream memoryStream = new MemoryStream(this.achievementChart.bytes))
			{
				foreach (AchievementDatabase.Achievement achievement in memoryStream.Query(null, ExcelType.UNKNOWN, "A1", null))
				{
					this._dic[achievement.id.Trim()] = achievement;
				}
			}
		}

		// Token: 0x06001A83 RID: 6787 RVA: 0x0005FE18 File Offset: 0x0005E018
		public static bool TryGetAchievementData(string id, out AchievementDatabase.Achievement achievement)
		{
			achievement = null;
			return !(AchievementDatabase.Instance == null) && AchievementDatabase.Instance.dic.TryGetValue(id, out achievement);
		}

		// Token: 0x06001A84 RID: 6788 RVA: 0x0005FE3D File Offset: 0x0005E03D
		internal bool IsIDValid(string id)
		{
			return this.dic.ContainsKey(id);
		}

		// Token: 0x04001302 RID: 4866
		[SerializeField]
		private XlsxObject achievementChart;

		// Token: 0x04001303 RID: 4867
		private Dictionary<string, AchievementDatabase.Achievement> _dic;

		// Token: 0x020005BB RID: 1467
		[Serializable]
		public class Achievement
		{
			// Token: 0x1700077A RID: 1914
			// (get) Token: 0x060028C7 RID: 10439 RVA: 0x00096F1A File Offset: 0x0009511A
			// (set) Token: 0x060028C8 RID: 10440 RVA: 0x00096F22 File Offset: 0x00095122
			public string id { get; set; }

			// Token: 0x1700077B RID: 1915
			// (get) Token: 0x060028C9 RID: 10441 RVA: 0x00096F2B File Offset: 0x0009512B
			// (set) Token: 0x060028CA RID: 10442 RVA: 0x00096F33 File Offset: 0x00095133
			public string overrideDisplayNameKey { get; set; }

			// Token: 0x1700077C RID: 1916
			// (get) Token: 0x060028CB RID: 10443 RVA: 0x00096F3C File Offset: 0x0009513C
			// (set) Token: 0x060028CC RID: 10444 RVA: 0x00096F44 File Offset: 0x00095144
			public string overrideDescriptionKey { get; set; }

			// Token: 0x1700077D RID: 1917
			// (get) Token: 0x060028CD RID: 10445 RVA: 0x00096F4D File Offset: 0x0009514D
			// (set) Token: 0x060028CE RID: 10446 RVA: 0x00096F73 File Offset: 0x00095173
			[LocalizationKey("Default")]
			private string DisplayNameKey
			{
				get
				{
					if (!string.IsNullOrWhiteSpace(this.overrideDisplayNameKey))
					{
						return this.overrideDisplayNameKey;
					}
					return "Achievement_" + this.id;
				}
				set
				{
				}
			}

			// Token: 0x1700077E RID: 1918
			// (get) Token: 0x060028CF RID: 10447 RVA: 0x00096F75 File Offset: 0x00095175
			// (set) Token: 0x060028D0 RID: 10448 RVA: 0x00096FA0 File Offset: 0x000951A0
			[LocalizationKey("Default")]
			public string DescriptionKey
			{
				get
				{
					if (!string.IsNullOrWhiteSpace(this.overrideDescriptionKey))
					{
						return this.overrideDescriptionKey;
					}
					return "Achievement_" + this.id + "_Desc";
				}
				set
				{
				}
			}

			// Token: 0x1700077F RID: 1919
			// (get) Token: 0x060028D1 RID: 10449 RVA: 0x00096FA2 File Offset: 0x000951A2
			public string DisplayName
			{
				get
				{
					return this.DisplayNameKey.ToPlainText();
				}
			}

			// Token: 0x17000780 RID: 1920
			// (get) Token: 0x060028D2 RID: 10450 RVA: 0x00096FAF File Offset: 0x000951AF
			public string Description
			{
				get
				{
					return this.DescriptionKey.ToPlainText();
				}
			}
		}
	}
}
