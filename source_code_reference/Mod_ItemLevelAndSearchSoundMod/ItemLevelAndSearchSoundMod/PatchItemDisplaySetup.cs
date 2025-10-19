using System;
using System.Runtime.CompilerServices;
using Duckov.UI;
using HarmonyLib;
using ItemStatsSystem;
using UnityEngine;
using UnityEngine.UI;

namespace ItemLevelAndSearchSoundMod
{
	// Token: 0x02000007 RID: 7
	[NullableContext(1)]
	[Nullable(0)]
	[HarmonyPatch(typeof(ItemDisplay), "Setup")]
	public class PatchItemDisplaySetup
	{
		// Token: 0x0600000A RID: 10 RVA: 0x00002374 File Offset: 0x00000574
		private static void Postfix(ItemDisplay __instance, Item target)
		{
			PatchItemDisplaySetup.<>c__DisplayClass0_0 CS$<>8__locals1 = new PatchItemDisplaySetup.<>c__DisplayClass0_0();
			CS$<>8__locals1.__instance = __instance;
			bool flag = CS$<>8__locals1.__instance == null;
			if (!flag)
			{
				CS$<>8__locals1.level = Util.GetItemValueLevel(target);
				Color color = Util.GetItemValueLevelColor(CS$<>8__locals1.level);
				bool flag2 = target != null && target.InInventory != null && target.InInventory.NeedInspection && !target.Inspected;
				if (flag2)
				{
					PatchItemDisplaySetup.<>c__DisplayClass0_1 CS$<>8__locals2 = new PatchItemDisplaySetup.<>c__DisplayClass0_1();
					CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
					CS$<>8__locals2.originalColor = color;
					color = ModBehaviour.White;
					target.onInspectionStateChanged += CS$<>8__locals2.<Postfix>g__OnInspectionStateChanged|0;
				}
				PatchItemDisplaySetup.SetColor(CS$<>8__locals1.__instance, color);
			}
		}

		// Token: 0x0600000B RID: 11 RVA: 0x00002434 File Offset: 0x00000634
		private static void SetColor(ItemDisplay __instance, Color color)
		{
			try
			{
				__instance.transform.Find("BG").GetComponent<Image>().color = color;
			}
			catch (Exception ex)
			{
				Debug.LogError("ItemLevelAndSearchSoundMod Patch SetColor Error: " + ex.Message);
			}
		}
	}
}
