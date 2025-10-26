using System;
using System.Collections.Generic;
using ItemStatsSystem;
using UnityEngine;

namespace DuckovESPv3.Core.Systems.ESP.Models
{
    /// <summary>
    /// 地面物品数据模型 - 包含物品及其源代理的完整信息
    /// 用于 ESP 渲染、分析、缓存等所有场景
    /// </summary>
    public class WorldItemData
    {
        /// <summary>
        /// 物品对象引用
        /// </summary>
        public Item Item { get; set; }

        /// <summary>
        /// 物品代理引用（DuckovItemAgent）
        /// </summary>
        public DuckovItemAgent Agent { get; set; }

        /// <summary>
        /// 物品在世界中的位置
        /// 从 Agent 的 transform 获取
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// 物品品质等级
        /// 用于 ESP 过滤和显示
        /// </summary>
        public ItemValueLevel Quality { get; set; }

        /// <summary>
        /// 物品数量（堆叠数量）
        /// </summary>
        public int StackCount { get; set; }

        /// <summary>
        /// 物品类型 ID
        /// </summary>
        public int TypeID { get; set; }

        /// <summary>
        /// 物品显示名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 是否是任务物品
        /// </summary>
        public bool IsQuestItem { get; set; }

        /// <summary>
        /// 是否是建筑材料
        /// </summary>
        public bool IsBuildingMaterial { get; set; }
        
        /// <summary>
        /// 是否被手动标记到心愿单
        /// </summary>
        public bool IsManuallyWishlisted { get; set; }
        
        /// <summary>
        /// 是否是任务需求物品
        /// </summary>
        public bool IsQuestRequired { get; set; }
        
        /// <summary>
        /// 是否是建筑需求物品
        /// </summary>
        public bool IsBuildingRequired { get; set; }
        
        /// <summary>
        /// 是否在心愿单中（任意类型）
        /// </summary>
        public bool IsWishlisted => IsManuallyWishlisted || IsQuestRequired || IsBuildingRequired;

        /// <summary>
        /// 物品被发现的时间
        /// </summary>
        public DateTime DiscoveryTime { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public WorldItemData()
        {
            DiscoveryTime = DateTime.Now;
            Quality = ItemValueLevel.White;
            StackCount = 1;
            TypeID = -1;
            DisplayName = string.Empty;
            IsQuestItem = false;
            IsBuildingMaterial = false;
            IsManuallyWishlisted = false;
            IsQuestRequired = false;
            IsBuildingRequired = false;
        }

        /// <summary>
        /// 检查数据完整性
        /// </summary>
        public bool IsValid()
        {
            return Item != null && Agent != null;
        }

        public override string ToString()
        {
            return $"WorldItemData(Name={DisplayName}, Quality={Quality}, Stack={StackCount}, Pos={Position})";
        }
    }
}
