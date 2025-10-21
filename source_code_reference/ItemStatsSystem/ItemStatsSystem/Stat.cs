using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Duckov.Utilities;
using ItemStatsSystem.Stats;
using SodaCraft.Localizations;
using UnityEngine;

namespace ItemStatsSystem
{
	// Token: 0x02000022 RID: 34
	[Serializable]
	public class Stat
	{
		// Token: 0x1700007A RID: 122
		// (get) Token: 0x060001CB RID: 459 RVA: 0x00007797 File Offset: 0x00005997
		public Item Master
		{
			get
			{
				return this.collection.Master;
			}
		}

		// Token: 0x1700007B RID: 123
		// (get) Token: 0x060001CC RID: 460 RVA: 0x000077A4 File Offset: 0x000059A4
		private StringList referenceKeys
		{
			get
			{
				return StringLists.StatKeys;
			}
		}

		// Token: 0x1700007C RID: 124
		// (get) Token: 0x060001CD RID: 461 RVA: 0x000077AB File Offset: 0x000059AB
		public string Key
		{
			get
			{
				return this.key;
			}
		}

		// Token: 0x1700007D RID: 125
		// (get) Token: 0x060001CE RID: 462 RVA: 0x000077B3 File Offset: 0x000059B3
		public string DisplayNameKey
		{
			get
			{
				return "Stat_" + this.key;
			}
		}

		// Token: 0x1700007E RID: 126
		// (get) Token: 0x060001CF RID: 463 RVA: 0x000077C5 File Offset: 0x000059C5
		public string DisplayName
		{
			get
			{
				return this.DisplayNameKey.ToPlainText();
			}
		}

		// Token: 0x1700007F RID: 127
		// (get) Token: 0x060001D0 RID: 464 RVA: 0x000077D2 File Offset: 0x000059D2
		// (set) Token: 0x060001D1 RID: 465 RVA: 0x000077DA File Offset: 0x000059DA
		public float BaseValue
		{
			get
			{
				return this.baseValue;
			}
			set
			{
				this.baseValue = value;
			}
		}

		// Token: 0x17000080 RID: 128
		// (get) Token: 0x060001D2 RID: 466 RVA: 0x000077E3 File Offset: 0x000059E3
		public List<Modifier> Modifiers
		{
			get
			{
				return this.modifiers;
			}
		}

		// Token: 0x17000081 RID: 129
		// (get) Token: 0x060001D3 RID: 467 RVA: 0x000077EB File Offset: 0x000059EB
		// (set) Token: 0x060001D4 RID: 468 RVA: 0x000077F3 File Offset: 0x000059F3
		private bool Dirty
		{
			get
			{
				return this._dirty;
			}
			set
			{
				this._dirty = value;
				if (value)
				{
					Action<Stat> onSetDirty = this.OnSetDirty;
					if (onSetDirty == null)
					{
						return;
					}
					onSetDirty(this);
				}
			}
		}

		// Token: 0x17000082 RID: 130
		// (get) Token: 0x060001D5 RID: 469 RVA: 0x00007810 File Offset: 0x00005A10
		public float Value
		{
			get
			{
				if (this.Dirty || this.cachedBaseValue != this.BaseValue)
				{
					this.Recalculate();
				}
				return this.cachedValue;
			}
		}

		// Token: 0x17000083 RID: 131
		// (get) Token: 0x060001D6 RID: 470 RVA: 0x00007834 File Offset: 0x00005A34
		private string ValueToolTip
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine(string.Format("基础:{0}", this.BaseValue));
				foreach (Modifier modifier in this.modifiers)
				{
					stringBuilder.AppendLine(modifier.ToString());
				}
				return stringBuilder.ToString();
			}
		}

		// Token: 0x17000084 RID: 132
		// (get) Token: 0x060001D7 RID: 471 RVA: 0x000078B8 File Offset: 0x00005AB8
		public bool Display
		{
			get
			{
				return this.display;
			}
		}

		// Token: 0x14000015 RID: 21
		// (add) Token: 0x060001D8 RID: 472 RVA: 0x000078C0 File Offset: 0x00005AC0
		// (remove) Token: 0x060001D9 RID: 473 RVA: 0x000078F8 File Offset: 0x00005AF8
		public event Action<Stat> OnSetDirty;

		// Token: 0x060001DA RID: 474 RVA: 0x00007930 File Offset: 0x00005B30
		private void Recalculate()
		{
			this.cachedBaseValue = this.baseValue;
			this.modifiers.RemoveAll((Modifier e) => e == null);
			this.modifiers.Sort(Modifier.OrderComparison);
			Stat.<>c__DisplayClass35_0 CS$<>8__locals1;
			CS$<>8__locals1.result = this.baseValue;
			CS$<>8__locals1.percentageAddValue = 0f;
			CS$<>8__locals1.percentageAdding = false;
			int num = int.MinValue;
			for (int i = 0; i < this.modifiers.Count; i++)
			{
				Modifier modifier = this.modifiers[i];
				int order = modifier.Order;
				if (CS$<>8__locals1.percentageAdding && (order != num || modifier.Type != ModifierType.PercentageAdd))
				{
					Stat.<Recalculate>g__ApplyPercentageAdd|35_1(ref CS$<>8__locals1);
				}
				num = modifier.Order;
				ModifierType type = modifier.Type;
				if (type != ModifierType.Add)
				{
					if (type != ModifierType.PercentageAdd)
					{
						if (type == ModifierType.PercentageMultiply)
						{
							CS$<>8__locals1.result *= Mathf.Max(0f, 1f + modifier.Value);
						}
					}
					else
					{
						CS$<>8__locals1.percentageAdding = true;
						CS$<>8__locals1.percentageAddValue += modifier.Value;
					}
				}
				else
				{
					CS$<>8__locals1.result += modifier.Value;
				}
			}
			if (CS$<>8__locals1.percentageAdding)
			{
				Stat.<Recalculate>g__ApplyPercentageAdd|35_1(ref CS$<>8__locals1);
			}
			this.cachedValue = CS$<>8__locals1.result;
			this._dirty = false;
		}

		// Token: 0x060001DB RID: 475 RVA: 0x00007A98 File Offset: 0x00005C98
		public void AddModifier(Modifier modifier)
		{
			this.modifiers.Add(modifier);
			modifier.NotifyAddedToStat(this);
			this.Dirty = true;
		}

		// Token: 0x060001DC RID: 476 RVA: 0x00007AB4 File Offset: 0x00005CB4
		public bool RemoveModifier(Modifier modifier)
		{
			bool flag = this.modifiers.Remove(modifier);
			this.Dirty = true;
			return flag;
		}

		// Token: 0x060001DD RID: 477 RVA: 0x00007ACC File Offset: 0x00005CCC
		public int RemoveAllModifiersFromSource(object source)
		{
			int num = this.modifiers.RemoveAll((Modifier e) => e.Source == source);
			this.Dirty = true;
			return num;
		}

		// Token: 0x060001DE RID: 478 RVA: 0x00007B04 File Offset: 0x00005D04
		internal void Initialize(StatCollection collection)
		{
			this.collection = collection;
			this.Recalculate();
		}

		// Token: 0x060001DF RID: 479 RVA: 0x00007B13 File Offset: 0x00005D13
		internal void SetDirty()
		{
			this.Dirty = true;
		}

		// Token: 0x060001E0 RID: 480 RVA: 0x00007B1C File Offset: 0x00005D1C
		public Stat()
		{
		}

		// Token: 0x060001E1 RID: 481 RVA: 0x00007B3A File Offset: 0x00005D3A
		public Stat(string key, float value, bool display = false)
		{
			this.key = key;
			this.baseValue = value;
			this.display = display;
		}

		// Token: 0x060001E2 RID: 482 RVA: 0x00007B70 File Offset: 0x00005D70
		[CompilerGenerated]
		internal static void <Recalculate>g__ApplyPercentageAdd|35_1(ref Stat.<>c__DisplayClass35_0 A_0)
		{
			Mathf.Max(0f, 1f + A_0.percentageAddValue);
			A_0.result *= Mathf.Max(0f, 1f + A_0.percentageAddValue);
			A_0.percentageAddValue = 0f;
			A_0.percentageAdding = false;
		}

		// Token: 0x040000A1 RID: 161
		[NonSerialized]
		private StatCollection collection;

		// Token: 0x040000A2 RID: 162
		[Tooltip("Stat Key")]
		[SerializeField]
		private string key;

		// Token: 0x040000A3 RID: 163
		[SerializeField]
		private bool display;

		// Token: 0x040000A4 RID: 164
		[Tooltip("Base Value")]
		[SerializeField]
		private float baseValue;

		// Token: 0x040000A5 RID: 165
		private List<Modifier> modifiers = new List<Modifier>();

		// Token: 0x040000A6 RID: 166
		private bool _dirty;

		// Token: 0x040000A7 RID: 167
		private float cachedBaseValue = float.NaN;

		// Token: 0x040000A8 RID: 168
		private float cachedValue;
	}
}
