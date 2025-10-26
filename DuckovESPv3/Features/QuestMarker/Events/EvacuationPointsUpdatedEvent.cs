using DuckovESPv3.Features.QuestMarker.Data;
using System.Collections.Generic;

namespace DuckovESPv3.Features.QuestMarker.Events
{
    /// <summary>
    /// 撤离点列表更新事件
    /// </summary>
    public class EvacuationPointsUpdatedEvent
    {
        /// <summary>
        /// 当前关卡的所有撤离点
        /// </summary>
        public IReadOnlyList<EvacuationPointData> EvacuationPoints { get; }

        public EvacuationPointsUpdatedEvent(IReadOnlyList<EvacuationPointData> evacuationPoints)
        {
            EvacuationPoints = evacuationPoints;
        }
    }
}
