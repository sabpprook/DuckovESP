using System;
using System.Runtime.CompilerServices;
using ItemStatsSystem;
using UnityEngine;

namespace ItemLevelAndSearchSoundMod
{
	// Token: 0x02000009 RID: 9
	[NullableContext(1)]
	[Nullable(0)]
	public static class Util
	{
		// Token: 0x0600000F RID: 15 RVA: 0x000024C4 File Offset: 0x000006C4
		public static ItemValueLevel GetItemValueLevel(Item item)
		{
			bool flag = item == null;
			ItemValueLevel itemValueLevel;
			if (flag)
			{
				itemValueLevel = ItemValueLevel.White;
			}
			else
			{
				bool flag2 = item.TypeID == 308 || item.TypeID == 309;
				if (flag2)
				{
					itemValueLevel = ItemValueLevel.White;
				}
				else
				{
					float num = (float)item.Value / 2f;
					bool flag3 = item.Tags.Contains("Bullet");
					if (flag3)
					{
						bool flag4 = item.DisplayQuality > 0;
						if (flag4)
						{
							itemValueLevel = Util.ParseDisplayQuality(item.DisplayQuality);
						}
						else
						{
							bool flag5 = item.Quality == 1;
							if (flag5)
							{
								itemValueLevel = ItemValueLevel.White;
							}
							else
							{
								bool flag6 = item.Quality == 2;
								if (flag6)
								{
									itemValueLevel = ItemValueLevel.Green;
								}
								else
								{
									ItemValueLevel itemValueLevel2 = Util.CalculateItemValueLevel((int)(num * 30f));
									bool flag7 = itemValueLevel2 > ItemValueLevel.Orange;
									if (flag7)
									{
										itemValueLevel = ItemValueLevel.Orange;
									}
									else
									{
										itemValueLevel = itemValueLevel2;
									}
								}
							}
						}
					}
					else
					{
						bool flag8 = item.Tags.Contains("Equipment");
						if (flag8)
						{
							bool flag9 = item.Tags.Contains("Special");
							if (flag9)
							{
								bool flag10 = item.name.Contains("StormProtection");
								if (flag10)
								{
									itemValueLevel = (ItemValueLevel)(item.Quality - 1);
								}
								else
								{
									itemValueLevel = Util.CalculateItemValueLevel((int)num);
								}
							}
							else
							{
								bool flag11 = item.Quality <= 7;
								if (flag11)
								{
									itemValueLevel = (ItemValueLevel)(item.Quality - 1);
								}
								else
								{
									itemValueLevel = Util.CalculateItemValueLevel((int)num);
								}
							}
						}
						else
						{
							ItemValueLevel itemValueLevel3 = Util.CalculateItemValueLevel((int)num);
							ItemValueLevel itemValueLevel4 = Util.ParseDisplayQuality(item.DisplayQuality);
							bool flag12 = itemValueLevel4 > itemValueLevel3;
							if (flag12)
							{
								itemValueLevel3 = itemValueLevel4;
							}
							itemValueLevel = itemValueLevel3;
						}
					}
				}
			}
			return itemValueLevel;
		}

		// Token: 0x06000010 RID: 16 RVA: 0x00002668 File Offset: 0x00000868
		public static ItemValueLevel CalculateItemValueLevel(int value)
		{
			bool flag = value >= 10000;
			ItemValueLevel itemValueLevel;
			if (flag)
			{
				itemValueLevel = ItemValueLevel.Red;
			}
			else
			{
				bool flag2 = value >= 5000;
				if (flag2)
				{
					itemValueLevel = ItemValueLevel.LightRed;
				}
				else
				{
					bool flag3 = value >= 2500;
					if (flag3)
					{
						itemValueLevel = ItemValueLevel.Orange;
					}
					else
					{
						bool flag4 = value >= 1200;
						if (flag4)
						{
							itemValueLevel = ItemValueLevel.Purple;
						}
						else
						{
							bool flag5 = value >= 600;
							if (flag5)
							{
								itemValueLevel = ItemValueLevel.Blue;
							}
							else
							{
								bool flag6 = value >= 200;
								if (flag6)
								{
									itemValueLevel = ItemValueLevel.Green;
								}
								else
								{
									itemValueLevel = ItemValueLevel.White;
								}
							}
						}
					}
				}
			}
			return itemValueLevel;
		}

		// Token: 0x06000011 RID: 17 RVA: 0x000026FC File Offset: 0x000008FC
		public static ItemValueLevel ParseDisplayQuality(DisplayQuality displayQuality)
		{
			ItemValueLevel itemValueLevel;
			switch (displayQuality)
			{
			case 0:
			case 1:
				itemValueLevel = ItemValueLevel.White;
				break;
			case 2:
				itemValueLevel = ItemValueLevel.Green;
				break;
			case 3:
				itemValueLevel = ItemValueLevel.Blue;
				break;
			case 4:
				itemValueLevel = ItemValueLevel.Purple;
				break;
			case 5:
				itemValueLevel = ItemValueLevel.Orange;
				break;
			case 6:
			case 7:
			case 8:
				itemValueLevel = ItemValueLevel.Red;
				break;
			default:
				itemValueLevel = ItemValueLevel.White;
				break;
			}
			return itemValueLevel;
		}

		// Token: 0x06000012 RID: 18 RVA: 0x00002758 File Offset: 0x00000958
		public static Color GetItemValueLevelColor(ItemValueLevel level)
		{
			Color color;
			switch (level)
			{
			case ItemValueLevel.White:
				color = ModBehaviour.White;
				break;
			case ItemValueLevel.Green:
				color = ModBehaviour.Green;
				break;
			case ItemValueLevel.Blue:
				color = ModBehaviour.Blue;
				break;
			case ItemValueLevel.Purple:
				color = ModBehaviour.Purple;
				break;
			case ItemValueLevel.Orange:
				color = ModBehaviour.Orange;
				break;
			case ItemValueLevel.LightRed:
				color = ModBehaviour.LightRed;
				break;
			case ItemValueLevel.Red:
				color = ModBehaviour.Red;
				break;
			default:
				color = ModBehaviour.White;
				break;
			}
			return color;
		}

		// Token: 0x06000013 RID: 19 RVA: 0x000027D0 File Offset: 0x000009D0
		public static float GetInspectingTime(ItemValueLevel level)
		{
			float num;
			switch (level)
			{
			case ItemValueLevel.White:
				num = 0.75f;
				break;
			case ItemValueLevel.Green:
				num = 1f;
				break;
			case ItemValueLevel.Blue:
				num = 1.25f;
				break;
			case ItemValueLevel.Purple:
				num = 1.75f;
				break;
			case ItemValueLevel.Orange:
				num = 2.25f;
				break;
			case ItemValueLevel.LightRed:
				num = 3.25f;
				break;
			case ItemValueLevel.Red:
				num = 4.5f;
				break;
			default:
				num = 0.75f;
				break;
			}
			return num;
		}

		// Token: 0x06000014 RID: 20 RVA: 0x00002848 File Offset: 0x00000A48
		public static string GetInspectedSound(ItemValueLevel level)
		{
			string text;
			switch (level)
			{
			case ItemValueLevel.White:
				text = "UI/click";
				break;
			case ItemValueLevel.Green:
				text = "UI/click";
				break;
			case ItemValueLevel.Blue:
				text = "UI/sceneloader_click";
				break;
			case ItemValueLevel.Purple:
				text = "UI/sceneloader_click";
				break;
			case ItemValueLevel.Orange:
				text = "UI/game_start";
				break;
			case ItemValueLevel.LightRed:
				text = "UI/game_start";
				break;
			case ItemValueLevel.Red:
				text = "UI/game_start";
				break;
			default:
				text = "UI/click";
				break;
			}
			return text;
		}
	}
}
