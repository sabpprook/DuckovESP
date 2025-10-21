using System;
using System.Collections.Generic;
using System.Text;
using Duckov.Utilities;
using ItemStatsSystem;
using SodaCraft.Localizations;
using SodaCraft.StringUtilities;
using UnityEngine;

namespace Duckov.PerkTrees.Behaviours
{
	// Token: 0x02000256 RID: 598
	public class ModifyCharacterStatsBase : PerkBehaviour
	{
		// Token: 0x1700035F RID: 863
		// (get) Token: 0x060012A0 RID: 4768 RVA: 0x00046145 File Offset: 0x00044345
		private string DescriptionFormat
		{
			get
			{
				return "PerkBehaviour_ModifyCharacterStatsBase".ToPlainText();
			}
		}

		// Token: 0x17000360 RID: 864
		// (get) Token: 0x060012A1 RID: 4769 RVA: 0x00046154 File Offset: 0x00044354
		public override string Description
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (ModifyCharacterStatsBase.Entry entry in this.entries)
				{
					if (entry != null && !string.IsNullOrEmpty(entry.key))
					{
						string text = ("Stat_" + entry.key.Trim()).ToPlainText();
						bool flag = entry.value > 0f;
						float value = entry.value;
						string text2 = (entry.percentage ? string.Format("{0}%", value * 100f) : value.ToString());
						string text3 = (flag ? "+" : "") + text2;
						string text4 = this.DescriptionFormat.Format(new
						{
							statDisplayName = text,
							value = text3
						});
						stringBuilder.AppendLine(text4);
					}
				}
				return stringBuilder.ToString().Trim();
			}
		}

		// Token: 0x060012A2 RID: 4770 RVA: 0x0004625C File Offset: 0x0004445C
		protected override void OnUnlocked()
		{
			LevelManager instance = LevelManager.Instance;
			Item item;
			if (instance == null)
			{
				item = null;
			}
			else
			{
				CharacterMainControl mainCharacter = instance.MainCharacter;
				item = ((mainCharacter != null) ? mainCharacter.CharacterItem : null);
			}
			this.targetItem = item;
			if (this.targetItem == null)
			{
				return;
			}
			StatCollection stats = this.targetItem.Stats;
			if (stats == null)
			{
				return;
			}
			foreach (ModifyCharacterStatsBase.Entry entry in this.entries)
			{
				Stat stat = stats.GetStat(entry.key);
				if (stat == null)
				{
					break;
				}
				stat.BaseValue += entry.value;
				this.records.Add(new ModifyCharacterStatsBase.Record
				{
					stat = stat,
					value = entry.value
				});
			}
		}

		// Token: 0x060012A3 RID: 4771 RVA: 0x00046340 File Offset: 0x00044540
		protected override void OnLocked()
		{
			if (this.targetItem == null)
			{
				return;
			}
			if (this.targetItem.Stats == null)
			{
				return;
			}
			foreach (ModifyCharacterStatsBase.Record record in this.records)
			{
				if (record.stat == null)
				{
					break;
				}
				record.stat.BaseValue -= record.value;
			}
		}

		// Token: 0x04000E16 RID: 3606
		[SerializeField]
		private List<ModifyCharacterStatsBase.Entry> entries = new List<ModifyCharacterStatsBase.Entry>();

		// Token: 0x04000E17 RID: 3607
		private Item targetItem;

		// Token: 0x04000E18 RID: 3608
		private List<ModifyCharacterStatsBase.Record> records = new List<ModifyCharacterStatsBase.Record>();

		// Token: 0x02000533 RID: 1331
		[Serializable]
		public class Entry
		{
			// Token: 0x17000753 RID: 1875
			// (get) Token: 0x0600279D RID: 10141 RVA: 0x00090F87 File Offset: 0x0008F187
			private StringList AvaliableKeys
			{
				get
				{
					return StringLists.StatKeys;
				}
			}

			// Token: 0x04001E6A RID: 7786
			public string key;

			// Token: 0x04001E6B RID: 7787
			public float value;

			// Token: 0x04001E6C RID: 7788
			public bool percentage;
		}

		// Token: 0x02000534 RID: 1332
		private struct Record
		{
			// Token: 0x04001E6D RID: 7789
			public Stat stat;

			// Token: 0x04001E6E RID: 7790
			public float value;
		}
	}
}
