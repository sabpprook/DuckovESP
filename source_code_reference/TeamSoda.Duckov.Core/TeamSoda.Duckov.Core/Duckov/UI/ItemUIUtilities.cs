using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Duckov.Utilities;
using ItemStatsSystem;
using UnityEngine;

namespace Duckov.UI
{
	// Token: 0x020003A2 RID: 930
	public static class ItemUIUtilities
	{
		// Token: 0x140000E1 RID: 225
		// (add) Token: 0x0600213B RID: 8507 RVA: 0x000740C8 File Offset: 0x000722C8
		// (remove) Token: 0x0600213C RID: 8508 RVA: 0x000740FC File Offset: 0x000722FC
		public static event Action OnSelectionChanged;

		// Token: 0x140000E2 RID: 226
		// (add) Token: 0x0600213D RID: 8509 RVA: 0x00074130 File Offset: 0x00072330
		// (remove) Token: 0x0600213E RID: 8510 RVA: 0x00074164 File Offset: 0x00072364
		public static event Action<Item> OnOrphanRaised;

		// Token: 0x1700065C RID: 1628
		// (get) Token: 0x0600213F RID: 8511 RVA: 0x00074197 File Offset: 0x00072397
		public static ItemDisplay SelectedItemDisplayRaw
		{
			get
			{
				return ItemUIUtilities.selectedItemDisplay;
			}
		}

		// Token: 0x1700065D RID: 1629
		// (get) Token: 0x06002140 RID: 8512 RVA: 0x0007419E File Offset: 0x0007239E
		// (set) Token: 0x06002141 RID: 8513 RVA: 0x000741C8 File Offset: 0x000723C8
		public static ItemDisplay SelectedItemDisplay
		{
			get
			{
				if (ItemUIUtilities.selectedItemDisplay == null)
				{
					return null;
				}
				if (ItemUIUtilities.selectedItemDisplay.Target == null)
				{
					return null;
				}
				return ItemUIUtilities.selectedItemDisplay;
			}
			private set
			{
				ItemDisplay itemDisplay = ItemUIUtilities.selectedItemDisplay;
				if (itemDisplay != null)
				{
					itemDisplay.NotifyUnselected();
				}
				ItemUIUtilities.selectedItemDisplay = value;
				Item selectedItem = ItemUIUtilities.SelectedItem;
				if (selectedItem == null)
				{
					ItemUIUtilities.selectedItemTypeID = -1;
				}
				else
				{
					ItemUIUtilities.selectedItemTypeID = selectedItem.TypeID;
					ItemUIUtilities.cachedSelectedItemMeta = ItemAssetsCollection.GetMetaData(ItemUIUtilities.selectedItemTypeID);
					ItemUIUtilities.cacheGunSelected = selectedItem.Tags.Contains("Gun");
				}
				ItemDisplay itemDisplay2 = ItemUIUtilities.selectedItemDisplay;
				if (itemDisplay2 != null)
				{
					itemDisplay2.NotifySelected();
				}
				Action onSelectionChanged = ItemUIUtilities.OnSelectionChanged;
				if (onSelectionChanged == null)
				{
					return;
				}
				onSelectionChanged();
			}
		}

		// Token: 0x1700065E RID: 1630
		// (get) Token: 0x06002142 RID: 8514 RVA: 0x00074250 File Offset: 0x00072450
		public static Item SelectedItem
		{
			get
			{
				if (ItemUIUtilities.SelectedItemDisplay == null)
				{
					return null;
				}
				return ItemUIUtilities.SelectedItemDisplay.Target;
			}
		}

		// Token: 0x1700065F RID: 1631
		// (get) Token: 0x06002143 RID: 8515 RVA: 0x0007426B File Offset: 0x0007246B
		public static bool IsGunSelected
		{
			get
			{
				return !(ItemUIUtilities.SelectedItem == null) && ItemUIUtilities.cacheGunSelected;
			}
		}

		// Token: 0x17000660 RID: 1632
		// (get) Token: 0x06002144 RID: 8516 RVA: 0x00074281 File Offset: 0x00072481
		public static string SelectedItemCaliber
		{
			get
			{
				return ItemUIUtilities.cachedSelectedItemMeta.caliber;
			}
		}

		// Token: 0x140000E3 RID: 227
		// (add) Token: 0x06002145 RID: 8517 RVA: 0x00074290 File Offset: 0x00072490
		// (remove) Token: 0x06002146 RID: 8518 RVA: 0x000742C4 File Offset: 0x000724C4
		public static event Action<Item, bool> OnPutItem;

		// Token: 0x06002147 RID: 8519 RVA: 0x000742F7 File Offset: 0x000724F7
		public static void Select(ItemDisplay itemDisplay)
		{
			ItemUIUtilities.SelectedItemDisplay = itemDisplay;
		}

		// Token: 0x06002148 RID: 8520 RVA: 0x000742FF File Offset: 0x000724FF
		public static void RaiseOrphan(Item orphan)
		{
			if (orphan == null)
			{
				return;
			}
			Action<Item> onOrphanRaised = ItemUIUtilities.OnOrphanRaised;
			if (onOrphanRaised != null)
			{
				onOrphanRaised(orphan);
			}
			Debug.LogWarning(string.Format("游戏中出现了孤儿Item {0}。", orphan));
		}

		// Token: 0x06002149 RID: 8521 RVA: 0x0007432C File Offset: 0x0007252C
		public static void NotifyPutItem(Item item, bool pickup = false)
		{
			Action<Item, bool> onPutItem = ItemUIUtilities.OnPutItem;
			if (onPutItem == null)
			{
				return;
			}
			onPutItem(item, pickup);
		}

		// Token: 0x0600214A RID: 8522 RVA: 0x00074340 File Offset: 0x00072540
		public static string GetPropertiesDisplayText(this Item item)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (item.Variables != null)
			{
				foreach (CustomData customData in item.Variables)
				{
					if (customData.Display)
					{
						stringBuilder.AppendLine(customData.DisplayName + "\t" + customData.GetValueDisplayString(""));
					}
				}
			}
			if (item.Constants != null)
			{
				foreach (CustomData customData2 in item.Constants)
				{
					if (customData2.Display)
					{
						stringBuilder.AppendLine(customData2.DisplayName + "\t" + customData2.GetValueDisplayString(""));
					}
				}
			}
			if (item.Stats != null)
			{
				foreach (Stat stat in item.Stats)
				{
					if (stat.Display)
					{
						stringBuilder.AppendLine(string.Format("{0}\t{1}", stat.DisplayName, stat.Value));
					}
				}
			}
			if (item.Modifiers != null)
			{
				foreach (ModifierDescription modifierDescription in item.Modifiers)
				{
					if (modifierDescription.Display)
					{
						stringBuilder.AppendLine(modifierDescription.DisplayName + "\t" + modifierDescription.GetDisplayValueString("0.##"));
					}
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x0600214B RID: 8523 RVA: 0x00074518 File Offset: 0x00072718
		[return: TupleElementNames(new string[] { "name", "value", "polarity" })]
		public static List<ValueTuple<string, string, Polarity>> GetPropertyValueTextPair(this Item item)
		{
			List<ValueTuple<string, string, Polarity>> list = new List<ValueTuple<string, string, Polarity>>();
			if (item.Variables != null)
			{
				foreach (CustomData customData in item.Variables)
				{
					if (customData.Display)
					{
						list.Add(new ValueTuple<string, string, Polarity>(customData.DisplayName, customData.GetValueDisplayString(""), Polarity.Neutral));
					}
				}
			}
			if (item.Constants != null)
			{
				foreach (CustomData customData2 in item.Constants)
				{
					if (customData2.Display)
					{
						list.Add(new ValueTuple<string, string, Polarity>(customData2.DisplayName, customData2.GetValueDisplayString(""), Polarity.Neutral));
					}
				}
			}
			if (item.Stats != null)
			{
				foreach (Stat stat in item.Stats)
				{
					if (stat.Display)
					{
						list.Add(new ValueTuple<string, string, Polarity>(stat.DisplayName, stat.Value.ToString(), Polarity.Neutral));
					}
				}
			}
			if (item.Modifiers != null)
			{
				foreach (ModifierDescription modifierDescription in item.Modifiers)
				{
					if (modifierDescription.Display)
					{
						Polarity polarity = StatInfoDatabase.GetPolarity(modifierDescription.Key);
						if (modifierDescription.Value < 0f)
						{
							polarity = -polarity;
						}
						list.Add(new ValueTuple<string, string, Polarity>(modifierDescription.DisplayName, modifierDescription.GetDisplayValueString("0.##"), polarity));
					}
				}
			}
			return list;
		}

		// Token: 0x04001696 RID: 5782
		private static ItemDisplay selectedItemDisplay;

		// Token: 0x04001697 RID: 5783
		private static bool cacheGunSelected;

		// Token: 0x04001698 RID: 5784
		private static int selectedItemTypeID;

		// Token: 0x04001699 RID: 5785
		private static ItemMetaData cachedSelectedItemMeta;
	}
}
