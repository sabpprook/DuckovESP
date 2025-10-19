using ItemStatsSystem;
using UnityEngine;

namespace DuckovESP
{
    /// <summary>
    /// 物品品质判断工具类
    /// 与Mod_ItemLevelAndSearchSoundMod的Util类保持一致
    /// </summary>
    public static class ItemQualityUtil
    {
        /// <summary>
        /// 获取物品的品质等级
        /// 完全复刻Mod_ItemLevelAndSearchSoundMod的GetItemValueLevel逻辑
        /// </summary>
        public static ItemValueLevel GetItemValueLevel(Item item)
        {
            if (item == null)
            {
                return ItemValueLevel.White;
            }

            // 排除某些特殊物品（TypeID 308, 309）
            if (item.TypeID == 308 || item.TypeID == 309)
            {
                return ItemValueLevel.White;
            }

            // 物品价值除以2（与原mod一致）
            float halfValue = (float)item.Value / 2f;

            // 子弹类物品特殊处理
            if (item.Tags.Contains("Bullet"))
            {
                if (item.DisplayQuality > 0)
                {
                    return ParseDisplayQuality(item.DisplayQuality);
                }
                else
                {
                    if (item.Quality == 1)
                    {
                        return ItemValueLevel.White;
                    }
                    else if (item.Quality == 2)
                    {
                        return ItemValueLevel.Green;
                    }
                    else
                    {
                        ItemValueLevel level = CalculateItemValueLevel((int)(halfValue * 30f));
                        // 子弹最高Orange
                        if (level > ItemValueLevel.Orange)
                        {
                            return ItemValueLevel.Orange;
                        }
                        return level;
                    }
                }
            }
            // 装备类物品特殊处理
            else if (item.Tags.Contains("Equipment"))
            {
                if (item.Tags.Contains("Special"))
                {
                    // 风暴防护装备特殊处理
                    if (item.name.Contains("StormProtection"))
                    {
                        return (ItemValueLevel)(item.Quality - 1);
                    }
                    else
                    {
                        return CalculateItemValueLevel((int)halfValue);
                    }
                }
                else
                {
                    // 普通装备，Quality ≤ 7直接映射
                    if (item.Quality <= 7)
                    {
                        return (ItemValueLevel)(item.Quality - 1);
                    }
                    else
                    {
                        return CalculateItemValueLevel((int)halfValue);
                    }
                }
            }
            // 其他物品
            else
            {
                ItemValueLevel valueLevel = CalculateItemValueLevel((int)halfValue);
                ItemValueLevel displayLevel = ParseDisplayQuality(item.DisplayQuality);
                
                // 取两者中较高的品质
                if (displayLevel > valueLevel)
                {
                    return displayLevel;
                }
                return valueLevel;
            }
        }

        /// <summary>
        /// 根据物品价值计算品质等级
        /// 与原mod的价值阈值完全一致
        /// </summary>
        public static ItemValueLevel CalculateItemValueLevel(int value)
        {
            if (value >= 10000)
            {
                return ItemValueLevel.Red;
            }
            else if (value >= 5000)
            {
                return ItemValueLevel.LightRed;
            }
            else if (value >= 2500)
            {
                return ItemValueLevel.Orange;
            }
            else if (value >= 1200)
            {
                return ItemValueLevel.Purple;
            }
            else if (value >= 600)
            {
                return ItemValueLevel.Blue;
            }
            else if (value >= 200)
            {
                return ItemValueLevel.Green;
            }
            else
            {
                return ItemValueLevel.White;
            }
        }

        /// <summary>
        /// 解析DisplayQuality到品质等级
        /// 与原mod的映射关系完全一致
        /// </summary>
        public static ItemValueLevel ParseDisplayQuality(DisplayQuality displayQuality)
        {
            switch (displayQuality)
            {
                case 0:
                case (DisplayQuality)1:
                    return ItemValueLevel.White;
                case (DisplayQuality)2:
                    return ItemValueLevel.Green;
                case (DisplayQuality)3:
                    return ItemValueLevel.Blue;
                case (DisplayQuality)4:
                    return ItemValueLevel.Purple;
                case (DisplayQuality)5:
                    return ItemValueLevel.Orange;
                case (DisplayQuality)6:
                case (DisplayQuality)7:
                case (DisplayQuality)8:
                    return ItemValueLevel.Red;
                default:
                    return ItemValueLevel.White;
            }
        }

        /// <summary>
        /// 获取品质等级对应的颜色
        /// </summary>
        public static Color GetItemValueLevelColor(ItemValueLevel level)
        {
            switch (level)
            {
                case ItemValueLevel.White:
                    return new Color(0.8f, 0.8f, 0.8f); // 浅灰色
                case ItemValueLevel.Green:
                    return new Color(0.3f, 1f, 0.3f); // 绿色
                case ItemValueLevel.Blue:
                    return new Color(0.3f, 0.5f, 1f); // 蓝色
                case ItemValueLevel.Purple:
                    return new Color(0.7f, 0.3f, 1f); // 紫色
                case ItemValueLevel.Orange:
                    return new Color(1f, 0.6f, 0f); // 橙色
                case ItemValueLevel.LightRed:
                    return new Color(1f, 0.4f, 0.4f); // 浅红色
                case ItemValueLevel.Red:
                    return new Color(1f, 0f, 0f); // 红色
                default:
                    return Color.white;
            }
        }

        /// <summary>
        /// 获取品质等级的中文名称
        /// </summary>
        public static string GetQualityName(ItemValueLevel level)
        {
            switch (level)
            {
                case ItemValueLevel.White:
                    return "普通";
                case ItemValueLevel.Green:
                    return "稀有";
                case ItemValueLevel.Blue:
                    return "精良";
                case ItemValueLevel.Purple:
                    return "史诗";
                case ItemValueLevel.Orange:
                    return "传说";
                case ItemValueLevel.LightRed:
                    return "神话";
                case ItemValueLevel.Red:
                    return "至尊";
                default:
                    return "未知";
            }
        }
    }
}
