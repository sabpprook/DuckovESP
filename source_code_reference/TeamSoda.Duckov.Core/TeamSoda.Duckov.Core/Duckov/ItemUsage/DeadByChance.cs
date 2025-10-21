using System;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.CompilerServices;
using ItemStatsSystem;
using SodaCraft.Localizations;
using UnityEngine;

namespace Duckov.ItemUsage
{
	// Token: 0x02000366 RID: 870
	[MenuPath("概率死亡")]
	public class DeadByChance : UsageBehavior
	{
		// Token: 0x170005DE RID: 1502
		// (get) Token: 0x06001E34 RID: 7732 RVA: 0x0006A534 File Offset: 0x00068734
		public override UsageBehavior.DisplaySettingsData DisplaySettings
		{
			get
			{
				return new UsageBehavior.DisplaySettingsData
				{
					display = true,
					description = string.Format("{0}:  {1:0}%", this.descriptionKey.ToPlainText(), this.chance * 100f)
				};
			}
		}

		// Token: 0x06001E35 RID: 7733 RVA: 0x0006A57F File Offset: 0x0006877F
		public override bool CanBeUsed(Item item, object user)
		{
			return user as CharacterMainControl;
		}

		// Token: 0x06001E36 RID: 7734 RVA: 0x0006A594 File Offset: 0x00068794
		protected override void OnUse(Item item, object user)
		{
			CharacterMainControl characterMainControl = user as CharacterMainControl;
			if (!characterMainControl)
			{
				return;
			}
			if (global::UnityEngine.Random.Range(0f, 1f) > this.chance)
			{
				return;
			}
			this.KillSelf(characterMainControl, item.TypeID).Forget();
		}

		// Token: 0x06001E37 RID: 7735 RVA: 0x0006A5E0 File Offset: 0x000687E0
		private UniTaskVoid KillSelf(CharacterMainControl character, int weaponID)
		{
			DeadByChance.<KillSelf>d__8 <KillSelf>d__;
			<KillSelf>d__.<>t__builder = AsyncUniTaskVoidMethodBuilder.Create();
			<KillSelf>d__.<>4__this = this;
			<KillSelf>d__.character = character;
			<KillSelf>d__.weaponID = weaponID;
			<KillSelf>d__.<>1__state = -1;
			<KillSelf>d__.<>t__builder.Start<DeadByChance.<KillSelf>d__8>(ref <KillSelf>d__);
			return <KillSelf>d__.<>t__builder.Task;
		}

		// Token: 0x0400149D RID: 5277
		public int damageValue = 9999;

		// Token: 0x0400149E RID: 5278
		public float chance;

		// Token: 0x0400149F RID: 5279
		[LocalizationKey("Default")]
		public string descriptionKey = "Usage_DeadByChance";

		// Token: 0x040014A0 RID: 5280
		[LocalizationKey("Default")]
		public string popTextKey = "Usage_DeadByChance_PopText";
	}
}
