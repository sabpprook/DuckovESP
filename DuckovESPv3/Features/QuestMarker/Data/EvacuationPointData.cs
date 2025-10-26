using UnityEngine;

namespace DuckovESPv3.Features.QuestMarker.Data
{
    /// <summary>
    /// 撤离点数据
    /// </summary>
    public class EvacuationPointData
    {
        /// <summary>
        /// 撤离点世界坐标
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// 撤离点名称（例如：MapElement, Exit, Evacuation）
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 到玩家的距离（米）
        /// </summary>
        public float DistanceToPlayer { get; set; }

        /// <summary>
        /// 是否在视野内
        /// </summary>
        public bool IsVisible { get; set; }

        /// <summary>
        /// 撤离点类型（根据名称推断）
        /// </summary>
        public EvacuationPointType Type { get; set; }

        /// <summary>
        /// 屏幕坐标（每帧更新，用于文字标签位置）
        /// </summary>
        public Vector3 ScreenPosition { get; set; }

        /// <summary>
        /// 是否在屏幕内
        /// </summary>
        public bool IsOnScreen { get; set; }

        /// <summary>
        /// 唯一ID（用于小地图标点识别）
        /// </summary>
        public string UniqueId { get; set; } = string.Empty;
    }

    public enum EvacuationPointType
    {
        Unknown,
        MapElement,     // 地图元素
        Exit,           // 出口
        Evacuation      // 撤离点
    }
}
