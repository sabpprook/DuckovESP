using System;
using Duckov.Buffs;
using ItemStatsSystem;
using SodaCraft.Localizations;
using UnityEngine;

namespace Duckov.ItemUsage
{
	// Token: 0x02000365 RID: 869
	public class AddBuff : UsageBehavior
	{
		// Token: 0x170005DD RID: 1501
		// (get) Token: 0x06001E30 RID: 7728 RVA: 0x0006A408 File Offset: 0x00068608
		public override UsageBehavior.DisplaySettingsData DisplaySettings
		{
			get
			{
				UsageBehavior.DisplaySettingsData displaySettingsData = default(UsageBehavior.DisplaySettingsData);
				displaySettingsData.display = true;
				displaySettingsData.description = "";
				displaySettingsData.description = this.buffPrefab.DisplayName ?? "";
				if (this.buffPrefab.LimitedLifeTime)
				{
					displaySettingsData.description += string.Format(" : {0}s ", this.buffPrefab.TotalLifeTime);
				}
				if (this.chance < 1f)
				{
					displaySettingsData.description += string.Format(" ({0} : {1}%)", this.chanceKey.ToPlainText(), Mathf.RoundToInt(this.chance * 100f));
				}
				return displaySettingsData;
			}
		}

		// Token: 0x06001E31 RID: 7729 RVA: 0x0006A4CA File Offset: 0x000686CA
		public override bool CanBeUsed(Item item, object user)
		{
			return true;
		}

		// Token: 0x06001E32 RID: 7730 RVA: 0x0006A4D0 File Offset: 0x000686D0
		protected override void OnUse(Item item, object user)
		{
			CharacterMainControl characterMainControl = user as CharacterMainControl;
			if (characterMainControl == null)
			{
				return;
			}
			if (global::UnityEngine.Random.Range(0f, 1f) > this.chance)
			{
				return;
			}
			characterMainControl.AddBuff(this.buffPrefab, characterMainControl, 0);
		}

		// Token: 0x0400149A RID: 5274
		public Buff buffPrefab;

		// Token: 0x0400149B RID: 5275
		[Range(0.01f, 1f)]
		public float chance = 1f;

		// Token: 0x0400149C RID: 5276
		[LocalizationKey("Default")]
		private string chanceKey = "UI_AddBuffChance";
	}
}
