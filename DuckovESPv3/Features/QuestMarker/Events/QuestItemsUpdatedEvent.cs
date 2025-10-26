using DuckovESPv3.Features.QuestMarker.Data;
using System.Collections.Generic;

namespace DuckovESPv3.Features.QuestMarker.Events
{
    /// <summary>
    /// 任务物品列表更新事件
    /// </summary>
    public class QuestItemsUpdatedEvent
    {
        /// <summary>
        /// 当前需要收集的任务物品列表
        /// </summary>
        public IReadOnlyList<QuestItemData> QuestItems { get; }

        public QuestItemsUpdatedEvent(IReadOnlyList<QuestItemData> questItems)
        {
            QuestItems = questItems;
        }
    }
}
