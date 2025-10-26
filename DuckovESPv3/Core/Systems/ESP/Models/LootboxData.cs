using System;
using System.Collections.Generic;
using ItemStatsSystem;
using UnityEngine;

namespace DuckovESPv3.Core.Systems.ESP.Models
{
    /// <summary>
    /// 箱子数据模型 - 包含箱子及其内容的完整信息
    /// 用于 ESP 渲染、分析、缓存等所有场景
    /// </summary>
    public class LootboxData
    {
        /// <summary>
        /// 箱子本体引用
        /// </summary>
        public InteractableLootbox Lootbox { get; set; }

        /// <summary>
        /// 箱子 Inventory 引用
        /// </summary>
        public Inventory Inventory { get; set; }

        /// <summary>
        /// 箱子在世界中的位置
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// 箱子内所有物品列表
        /// 使用 ReadOnly 列表，防止外部修改
        /// </summary>
        public IReadOnlyList<Item> Items { get; set; }

        /// <summary>
        /// 箱子内最高品质等级
        /// 用于 ESP 过滤和渲染优先级
        /// </summary>
        public ItemValueLevel MaxItemQuality { get; set; }

        /// <summary>
        /// 是否包含任务物品
        /// </summary>
        public bool HasQuestItems { get; set; }

        /// <summary>
        /// 是否包含建筑材料
        /// </summary>
        public bool HasBuildingMaterials { get; set; }
        
        /// <summary>
        /// 是否包含心愿单物品（手动标记）
        /// </summary>
        public bool HasWishlistedItems { get; set; }
        
        /// <summary>
        /// 是否包含任务需求物品
        /// </summary>
        public bool HasQuestRequiredItems { get; set; }
        
        /// <summary>
        /// 是否包含建筑需求物品
        /// </summary>
        public bool HasBuildingRequiredItems { get; set; }
        
        /// <summary>
        /// 心愿单物品数量（任意类型）
        /// </summary>
        public int WishlistedItemCount { get; set; }

        /// <summary>
        /// 箱子是否已被打开过（Looted 状态）
        /// </summary>
        public bool IsLooted { get; set; }

        /// <summary>
        /// 箱子被发现的时间
        /// </summary>
        public DateTime DiscoveryTime { get; set; }

        /// <summary>
        /// 箱子的唯一标识符（基于位置哈希）
        /// </summary>
        public int Key { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public LootboxData()
        {
            DiscoveryTime = DateTime.Now;
            Items = new List<Item>();
            MaxItemQuality = ItemValueLevel.White;
            HasQuestItems = false;
            HasBuildingMaterials = false;
            HasWishlistedItems = false;
            HasQuestRequiredItems = false;
            HasBuildingRequiredItems = false;
            WishlistedItemCount = 0;
            IsLooted = false;
        }

        /// <summary>
        /// 检查数据完整性
        /// </summary>
        public bool IsValid()
        {
            return Lootbox != null && Inventory != null;
        }

        public override string ToString()
        {
            return $"LootboxData(Key={Key}, Position={Position}, Items={Items.Count}, Quality={MaxItemQuality})";
        }
    }
}
