using System;
using ItemStatsSystem;
using SodaCraft.Localizations;
using UnityEngine;

namespace Duckov.ItemUsage
{
	// Token: 0x02000367 RID: 871
	[MenuPath("医疗/药")]
	public class Drug : UsageBehavior
	{
		// Token: 0x170005DF RID: 1503
		// (get) Token: 0x06001E39 RID: 7737 RVA: 0x0006A65C File Offset: 0x0006885C
		public override UsageBehavior.DisplaySettingsData DisplaySettings
		{
			get
			{
				UsageBehavior.DisplaySettingsData displaySettingsData = default(UsageBehavior.DisplaySettingsData);
				displaySettingsData.display = true;
				displaySettingsData.description = string.Format("{0} : {1}", this.healValueDescriptionKey.ToPlainText(), this.healValue);
				if (this.useDurability)
				{
					displaySettingsData.description += string.Format(" ({0} : {1})", this.durabilityUsageDescriptionKey.ToPlainText(), this.durabilityUsage);
				}
				return displaySettingsData;
			}
		}

		// Token: 0x06001E3A RID: 7738 RVA: 0x0006A6D8 File Offset: 0x000688D8
		public override bool CanBeUsed(Item item, object user)
		{
			CharacterMainControl characterMainControl = user as CharacterMainControl;
			return characterMainControl && this.CheckCanHeal(characterMainControl);
		}

		// Token: 0x06001E3B RID: 7739 RVA: 0x0006A704 File Offset: 0x00068904
		protected override void OnUse(Item item, object user)
		{
			CharacterMainControl characterMainControl = user as CharacterMainControl;
			if (!characterMainControl)
			{
				return;
			}
			float num = (float)this.healValue;
			if (this.useDurability && item.UseDurability)
			{
				float num2 = this.durabilityUsage;
				if (this.canUsePart)
				{
					num = characterMainControl.Health.MaxHealth - characterMainControl.Health.CurrentHealth;
					if (num > (float)this.healValue)
					{
						num = (float)this.healValue;
					}
					num2 = num / (float)this.healValue * this.durabilityUsage;
					if (num2 > item.Durability)
					{
						num2 = item.Durability;
						num = (float)this.healValue * item.Durability / this.durabilityUsage;
					}
					Debug.Log(string.Format("治疗：{0}耐久消耗：{1}", num, num2));
					item.Durability -= num2;
				}
			}
			this.Heal(characterMainControl, item, num);
		}

		// Token: 0x06001E3C RID: 7740 RVA: 0x0006A7E4 File Offset: 0x000689E4
		private bool CheckCanHeal(CharacterMainControl character)
		{
			return this.healValue <= 0 || character.Health.CurrentHealth < character.Health.MaxHealth;
		}

		// Token: 0x06001E3D RID: 7741 RVA: 0x0006A80C File Offset: 0x00068A0C
		private void Heal(CharacterMainControl character, Item selfItem, float _healValue)
		{
			if (_healValue > 0f)
			{
				character.AddHealth((float)Mathf.CeilToInt(_healValue));
				return;
			}
			if (_healValue < 0f)
			{
				DamageInfo damageInfo = new DamageInfo(null);
				damageInfo.damageValue = -_healValue;
				damageInfo.damagePoint = character.transform.position;
				damageInfo.damageNormal = Vector3.up;
				character.Health.Hurt(damageInfo);
			}
		}

		// Token: 0x040014A1 RID: 5281
		public int healValue;

		// Token: 0x040014A2 RID: 5282
		[LocalizationKey("Default")]
		public string healValueDescriptionKey = "Usage_HealValue";

		// Token: 0x040014A3 RID: 5283
		[LocalizationKey("Default")]
		public string durabilityUsageDescriptionKey = "Usage_Durability";

		// Token: 0x040014A4 RID: 5284
		public bool useDurability;

		// Token: 0x040014A5 RID: 5285
		public float durabilityUsage;

		// Token: 0x040014A6 RID: 5286
		public bool canUsePart;
	}
}
