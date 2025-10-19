using System;
using ItemStatsSystem;
using SodaCraft.Localizations;

namespace Duckov.ItemUsage
{
	// Token: 0x02000368 RID: 872
	[MenuPath("食物/食物")]
	public class FoodDrink : UsageBehavior
	{
		// Token: 0x170005E0 RID: 1504
		// (get) Token: 0x06001E3F RID: 7743 RVA: 0x0006A894 File Offset: 0x00068A94
		public override UsageBehavior.DisplaySettingsData DisplaySettings
		{
			get
			{
				UsageBehavior.DisplaySettingsData displaySettingsData = default(UsageBehavior.DisplaySettingsData);
				displaySettingsData.display = true;
				if (this.energyValue != 0f && this.waterValue != 0f)
				{
					displaySettingsData.description = string.Concat(new string[]
					{
						this.energyKey.ToPlainText(),
						": ",
						this.energyValue.ToString(),
						"  ",
						this.waterKey.ToPlainText(),
						": ",
						this.waterValue.ToString()
					});
				}
				else if (this.energyValue != 0f)
				{
					displaySettingsData.description = this.energyKey.ToPlainText() + ": " + this.energyValue.ToString();
				}
				else
				{
					displaySettingsData.description = this.waterKey.ToPlainText() + ": " + this.waterValue.ToString();
				}
				return displaySettingsData;
			}
		}

		// Token: 0x06001E40 RID: 7744 RVA: 0x0006A98D File Offset: 0x00068B8D
		public override bool CanBeUsed(Item item, object user)
		{
			return user as CharacterMainControl;
		}

		// Token: 0x06001E41 RID: 7745 RVA: 0x0006A9A0 File Offset: 0x00068BA0
		protected override void OnUse(Item item, object user)
		{
			CharacterMainControl characterMainControl = user as CharacterMainControl;
			if (!characterMainControl)
			{
				return;
			}
			this.Eat(characterMainControl);
			if (this.UseDurability > 0f && item.UseDurability)
			{
				item.Durability -= this.UseDurability;
			}
		}

		// Token: 0x06001E42 RID: 7746 RVA: 0x0006A9EC File Offset: 0x00068BEC
		private void Eat(CharacterMainControl character)
		{
			if (this.energyValue != 0f)
			{
				character.AddEnergy(this.energyValue);
			}
			if (this.waterValue != 0f)
			{
				character.AddWater(this.waterValue);
			}
		}

		// Token: 0x040014A7 RID: 5287
		public float energyValue;

		// Token: 0x040014A8 RID: 5288
		public float waterValue;

		// Token: 0x040014A9 RID: 5289
		[LocalizationKey("Default")]
		public string energyKey = "Usage_Energy";

		// Token: 0x040014AA RID: 5290
		[LocalizationKey("Default")]
		public string waterKey = "Usage_Water";

		// Token: 0x040014AB RID: 5291
		public float UseDurability;
	}
}
