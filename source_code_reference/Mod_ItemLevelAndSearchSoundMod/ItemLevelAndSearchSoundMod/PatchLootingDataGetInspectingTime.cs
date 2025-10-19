using System;
using System.Runtime.CompilerServices;
using Duckov.Utilities;
using HarmonyLib;
using ItemStatsSystem;

namespace ItemLevelAndSearchSoundMod
{
	// Token: 0x02000008 RID: 8
	[HarmonyPatch(typeof(GameplayDataSettings.LootingData), "GetInspectingTime")]
	public class PatchLootingDataGetInspectingTime
	{
		// Token: 0x0600000D RID: 13 RVA: 0x0000249C File Offset: 0x0000069C
		[NullableContext(1)]
		private static void Postfix(GameplayDataSettings.LootingData __instance, Item item, ref float __result)
		{
			ItemValueLevel itemValueLevel = Util.GetItemValueLevel(item);
			__result = Util.GetInspectingTime(itemValueLevel);
		}
	}
}
