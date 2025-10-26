using ItemStatsSystem;
using DuckovESPv3.Core.Systems.ESP.Models;
using UnityEngine;

namespace DuckovESPv3.Utils
{
    /// <summary>
    /// 物品品质计算工具类
    /// 参考 ItemLevelAndSearchSoundMod 的实现
    /// </summary>
    public static class ItemQualityUtil
    {
        /// <summary>
        /// 品质颜色映射表
        /// </summary>
        private static readonly System.Collections.Generic.Dictionary<ItemValueLevel, Color> _qualityColors = new()
        {
            { ItemValueLevel.White, new Color(0.8f, 0.8f, 0.8f) },
            { ItemValueLevel.Green, new Color(0.2f, 1f, 0.2f) },
            { ItemValueLevel.Blue, new Color(0.3f, 0.5f, 1f) },
            { ItemValueLevel.Purple, new Color(0.8f, 0.3f, 1f) },
            { ItemValueLevel.Orange, new Color(1f, 0.6f, 0f) },
            { ItemValueLevel.LightRed, new Color(1f, 0.5f, 0.5f) },
            { ItemValueLevel.Red, new Color(1f, 0.2f, 0.2f) }
        };

        /// <summary>
        /// 获取品质对应的颜色
        /// </summary>
        public static Color GetColor(ItemValueLevel quality)
        {
            return _qualityColors.TryGetValue(quality, out var color) ? color : Color.white;
        }

        /// <summary>
        /// 获取物品的价值等级
        /// </summary>
        public static ItemValueLevel GetItemValueLevel(Item item)
        {
            if (item == null)
                return ItemValueLevel.White;

            float halfValue = (float)item.Value / 2f;

            // 子弹类物品
            if (item.Tags.Contains("Bullet"))
            {
                return GetBulletQuality(item, halfValue);
            }

            // 装备类物品
            if (item.Tags.Contains("Equipment"))
            {
                return GetEquipmentQuality(item, halfValue);
            }

            // 配饰类物品
            if (item.Tags.Contains("Accessory"))
            {
                return GetAccessoryQuality(item);
            }

            // 特殊物品：充电器
            if (item.TypeID == 862)
            {
                return ItemValueLevel.Orange;
            }

            // 其他物品：按价值计算，并与 DisplayQuality 取最大值
            var valueLevel = CalculateItemValueLevel((int)halfValue);
            var displayLevel = ParseDisplayQuality(item.DisplayQuality);
            
            return valueLevel > displayLevel ? valueLevel : displayLevel;
        }

        /// <summary>
        /// 计算子弹品质
        /// </summary>
        private static ItemValueLevel GetBulletQuality(Item item, float halfValue)
        {
            // 优先使用 DisplayQuality
            if (item.DisplayQuality > DisplayQuality.None)
            {
                // Orange 映射为 LightRed
                if (item.DisplayQuality == DisplayQuality.Orange)
                    return ItemValueLevel.LightRed;
                
                return ParseDisplayQuality(item.DisplayQuality);
            }

            // 使用 Quality 属性
            if (item.Quality == 1)
                return ItemValueLevel.White;
            
            if (item.Quality == 2)
                return ItemValueLevel.Green;

            // 按价值计算（子弹价值 * 30）
            var level = CalculateItemValueLevel((int)(halfValue * 30f));
            return level > ItemValueLevel.Orange ? ItemValueLevel.Orange : level;
        }

        /// <summary>
        /// 计算装备品质
        /// </summary>
        private static ItemValueLevel GetEquipmentQuality(Item item, float halfValue)
        {
            // 特殊装备
            if (item.Tags.Contains("Special"))
            {
                // 风暴防护装备
                if (item.name.Contains("StormProtection"))
                {
                    return (ItemValueLevel)(item.Quality - 1);
                }

                // 其他特殊装备
                int qualityIndex = item.Quality - 2;
                if (qualityIndex > 6)
                    return ItemValueLevel.Red;
                if (qualityIndex < 0)
                    return ItemValueLevel.White;
                
                return (ItemValueLevel)qualityIndex;
            }

            // 普通装备
            if (item.Quality <= 7)
            {
                return (ItemValueLevel)(item.Quality - 1);
            }

            return CalculateItemValueLevel((int)halfValue);
        }

        /// <summary>
        /// 计算配饰品质
        /// </summary>
        private static ItemValueLevel GetAccessoryQuality(Item item)
        {
            if (item.Quality <= 7)
            {
                return (ItemValueLevel)(item.Quality - 1);
            }

            return ParseDisplayQuality(item.DisplayQuality);
        }

        /// <summary>
        /// 根据价值计算品质等级
        /// </summary>
        private static ItemValueLevel CalculateItemValueLevel(int value)
        {
            if (value >= 10000)
                return ItemValueLevel.Red;
            if (value >= 5000)
                return ItemValueLevel.LightRed;
            if (value >= 2500)
                return ItemValueLevel.Orange;
            if (value >= 1200)
                return ItemValueLevel.Purple;
            if (value >= 600)
                return ItemValueLevel.Blue;
            if (value >= 200)
                return ItemValueLevel.Green;
            
            return ItemValueLevel.White;
        }

        /// <summary>
        /// 将 DisplayQuality 转换为 ItemValueLevel
        /// </summary>
        private static ItemValueLevel ParseDisplayQuality(DisplayQuality displayQuality)
        {
            return displayQuality switch
            {
                DisplayQuality.White => ItemValueLevel.White,
                DisplayQuality.Green => ItemValueLevel.Green,
                DisplayQuality.Blue => ItemValueLevel.Blue,
                DisplayQuality.Purple => ItemValueLevel.Purple,
                DisplayQuality.Orange => ItemValueLevel.Orange,
                DisplayQuality.Red => ItemValueLevel.Red,
                DisplayQuality.Q7 => ItemValueLevel.Red,
                DisplayQuality.Q8 => ItemValueLevel.Red,
                _ => ItemValueLevel.White
            };
        }
    }
}
