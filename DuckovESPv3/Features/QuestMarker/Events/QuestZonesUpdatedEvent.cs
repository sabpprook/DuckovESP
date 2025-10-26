using DuckovESPv3.Features.QuestMarker.Data;
using System.Collections.Generic;

namespace DuckovESPv3.Features.QuestMarker.Events
{
    /// <summary>
    /// 任务地点列表更新事件
    /// </summary>
    public class QuestZonesUpdatedEvent
    {
        /// <summary>
        /// 当前激活的任务地点列表
        /// </summary>
        public IReadOnlyList<QuestZoneData> QuestZones { get; }

        public QuestZonesUpdatedEvent(IReadOnlyList<QuestZoneData> questZones)
        {
            QuestZones = questZones;
        }
    }
}
