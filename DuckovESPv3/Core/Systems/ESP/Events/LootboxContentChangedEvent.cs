namespace DuckovESPv3.Core.Systems.ESP.Events
{
    /// <summary>
    /// 箱子内容变更事件
    /// 当箱子中的物品被拾取、添加或内容发生变化时触发
    /// </summary>
    public class LootboxContentChangedEvent
    {
        /// <summary>
        /// 箱子数据
        /// </summary>
        public Models.LootboxData Data { get; }

        /// <summary>
        /// 变更原因
        /// </summary>
        public ChangeReason Reason { get; }

        public LootboxContentChangedEvent(Models.LootboxData data, ChangeReason reason)
        {
            Data = data;
            Reason = reason;
        }
    }

    /// <summary>
    /// 箱子内容变更原因
    /// </summary>
    public enum ChangeReason
    {
        /// <summary>
        /// 物品被移除（拾取）
        /// </summary>
        ItemRemoved,

        /// <summary>
        /// 物品被添加
        /// </summary>
        ItemAdded,

        /// <summary>
        /// 箱子变为空
        /// </summary>
        BecameEmpty
    }
}
