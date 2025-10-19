using System;
using ItemStatsSystem;
using SodaCraft.Localizations;
using UnityEngine;

namespace Duckov.ItemUsage
{
	// Token: 0x0200036A RID: 874
	public class SpawnEgg : UsageBehavior
	{
		// Token: 0x170005E1 RID: 1505
		// (get) Token: 0x06001E47 RID: 7751 RVA: 0x0006AB28 File Offset: 0x00068D28
		public override UsageBehavior.DisplaySettingsData DisplaySettings
		{
			get
			{
				return new UsageBehavior.DisplaySettingsData
				{
					display = true,
					description = (this.descriptionKey.ToPlainText() ?? "")
				};
			}
		}

		// Token: 0x06001E48 RID: 7752 RVA: 0x0006AB61 File Offset: 0x00068D61
		public override bool CanBeUsed(Item item, object user)
		{
			return true;
		}

		// Token: 0x06001E49 RID: 7753 RVA: 0x0006AB64 File Offset: 0x00068D64
		protected override void OnUse(Item item, object user)
		{
			CharacterMainControl characterMainControl = user as CharacterMainControl;
			if (characterMainControl == null)
			{
				return;
			}
			Egg egg = global::UnityEngine.Object.Instantiate<Egg>(this.eggPrefab, characterMainControl.transform.position, Quaternion.identity);
			Collider component = egg.GetComponent<Collider>();
			Collider component2 = characterMainControl.GetComponent<Collider>();
			if (component && component2)
			{
				Debug.Log("关掉角色和蛋的碰撞");
				Physics.IgnoreCollision(component, component2, true);
			}
			egg.Init(characterMainControl.transform.position, characterMainControl.CurrentAimDirection * 1f, characterMainControl, this.spawnCharacter, this.eggSpawnDelay);
		}

		// Token: 0x040014B1 RID: 5297
		public Egg eggPrefab;

		// Token: 0x040014B2 RID: 5298
		public CharacterRandomPreset spawnCharacter;

		// Token: 0x040014B3 RID: 5299
		public float eggSpawnDelay = 2f;

		// Token: 0x040014B4 RID: 5300
		[LocalizationKey("Default")]
		public string descriptionKey = "Usage_SpawnEgg";
	}
}
