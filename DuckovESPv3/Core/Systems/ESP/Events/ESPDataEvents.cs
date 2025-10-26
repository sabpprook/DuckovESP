using System;

namespace DuckovESPv3.Core.Systems.ESP.Events
{
    /// <summary>
    /// 发现新箱子事件
    /// 当新的 Inventory 被添加到 LootBoxInventories 时触发
    /// </summary>
    public class LootboxDiscoveredEvent
    {
        /// <summary>
        /// 箱子数据
        /// </summary>
        public Models.LootboxData Data { get; set; }

        /// <summary>
        /// 发现时间
        /// </summary>
        public DateTime DiscoveryTime { get; private set; }

        public LootboxDiscoveredEvent(Models.LootboxData data)
        {
            Data = data;
            DiscoveryTime = DateTime.Now;
        }

        public override string ToString() => $"LootboxDiscovered({Data})";
    }

    /// <summary>
    /// 箱子被移除/销毁事件
    /// 当箱子的 Inventory 从字典中移除时触发
    /// </summary>
    public class LootboxRemovedEvent
    {
        /// <summary>
        /// 箱子数据
        /// </summary>
        public Models.LootboxData Data { get; set; }

        /// <summary>
        /// 移除时间
        /// </summary>
        public DateTime RemovalTime { get; private set; }

        public LootboxRemovedEvent(Models.LootboxData data)
        {
            Data = data;
            RemovalTime = DateTime.Now;
        }

        public override string ToString() => $"LootboxRemoved({Data})";
    }

    /// <summary>
    /// 发现新世界物品事件
    /// 当新的 pickUp 物品被检测到时触发
    /// </summary>
    public class WorldItemDiscoveredEvent
    {
        /// <summary>
        /// 物品数据
        /// </summary>
        public Models.WorldItemData Data { get; set; }

        /// <summary>
        /// 发现时间
        /// </summary>
        public DateTime DiscoveryTime { get; private set; }

        public WorldItemDiscoveredEvent(Models.WorldItemData data)
        {
            Data = data;
            DiscoveryTime = DateTime.Now;
        }

        public override string ToString() => $"WorldItemDiscovered({Data})";
    }

    /// <summary>
    /// 世界物品被移除/销毁事件
    /// 当物品的 Item.onDestroy 被触发时触发
    /// </summary>
    public class WorldItemRemovedEvent
    {
        /// <summary>
        /// 物品数据
        /// </summary>
        public Models.WorldItemData Data { get; set; }

        /// <summary>
        /// 移除时间
        /// </summary>
        public DateTime RemovalTime { get; private set; }

        public WorldItemRemovedEvent(Models.WorldItemData data)
        {
            Data = data;
            RemovalTime = DateTime.Now;
        }

        public override string ToString() => $"WorldItemRemoved({Data})";
    }
}
