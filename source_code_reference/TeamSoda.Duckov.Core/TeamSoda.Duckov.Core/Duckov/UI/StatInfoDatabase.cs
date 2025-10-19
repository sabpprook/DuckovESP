using System;
using System.Collections.Generic;
using Duckov.Utilities;
using ItemStatsSystem;
using UnityEngine;

namespace Duckov.UI
{
	// Token: 0x02000393 RID: 915
	[CreateAssetMenu(menuName = "Duckov/Stat Info Database")]
	public class StatInfoDatabase : ScriptableObject
	{
		// Token: 0x17000629 RID: 1577
		// (get) Token: 0x0600202A RID: 8234 RVA: 0x00070508 File Offset: 0x0006E708
		public static StatInfoDatabase Instance
		{
			get
			{
				return GameplayDataSettings.StatInfo;
			}
		}

		// Token: 0x1700062A RID: 1578
		// (get) Token: 0x0600202B RID: 8235 RVA: 0x0007050F File Offset: 0x0006E70F
		private static Dictionary<string, StatInfoDatabase.Entry> Dic
		{
			get
			{
				return StatInfoDatabase.Instance._dic;
			}
		}

		// Token: 0x0600202C RID: 8236 RVA: 0x0007051C File Offset: 0x0006E71C
		public static StatInfoDatabase.Entry Get(string statName)
		{
			if (!(StatInfoDatabase.Instance == null))
			{
				if (StatInfoDatabase.Dic == null)
				{
					StatInfoDatabase.RebuildDic();
				}
				StatInfoDatabase.Entry entry;
				if (StatInfoDatabase.Dic.TryGetValue(statName, out entry))
				{
					return entry;
				}
			}
			return new StatInfoDatabase.Entry
			{
				statName = statName,
				polarity = Polarity.Neutral,
				displayFormat = "0.##"
			};
		}

		// Token: 0x0600202D RID: 8237 RVA: 0x00070578 File Offset: 0x0006E778
		public static Polarity GetPolarity(string statName)
		{
			return StatInfoDatabase.Get(statName).polarity;
		}

		// Token: 0x0600202E RID: 8238 RVA: 0x00070588 File Offset: 0x0006E788
		[ContextMenu("Rebuild Dic")]
		private static void RebuildDic()
		{
			if (StatInfoDatabase.Instance == null)
			{
				return;
			}
			StatInfoDatabase.Instance._dic = new Dictionary<string, StatInfoDatabase.Entry>();
			foreach (StatInfoDatabase.Entry entry in StatInfoDatabase.Instance.entries)
			{
				if (StatInfoDatabase.Instance._dic.ContainsKey(entry.statName))
				{
					Debug.LogError("Stat Info 中有重复的 key: " + entry.statName);
				}
				else
				{
					StatInfoDatabase.Instance._dic[entry.statName] = entry;
				}
			}
		}

		// Token: 0x040015F3 RID: 5619
		[SerializeField]
		private StatInfoDatabase.Entry[] entries = new StatInfoDatabase.Entry[0];

		// Token: 0x040015F4 RID: 5620
		private Dictionary<string, StatInfoDatabase.Entry> _dic;

		// Token: 0x02000615 RID: 1557
		[Serializable]
		public struct Entry
		{
			// Token: 0x1700078C RID: 1932
			// (get) Token: 0x0600299F RID: 10655 RVA: 0x0009B70A File Offset: 0x0009990A
			public string DisplayFormat
			{
				get
				{
					if (string.IsNullOrEmpty(this.displayFormat))
					{
						return "0.##";
					}
					return this.displayFormat;
				}
			}

			// Token: 0x0400218C RID: 8588
			public string statName;

			// Token: 0x0400218D RID: 8589
			public Polarity polarity;

			// Token: 0x0400218E RID: 8590
			public string displayFormat;
		}
	}
}
