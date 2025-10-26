using UnityEngine;

namespace DuckovESPv3.Features.QuestMarker.Data
{
    /// <summary>
    /// 任务地点数据
    /// </summary>
    public class QuestZoneData
    {
        /// <summary>
        /// 任务地点世界坐标
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// 任务ID
        /// </summary>
        public int QuestId { get; set; }

        /// <summary>
        /// 任务目标索引
        /// </summary>
        public int TaskIndex { get; set; }

        /// <summary>
        /// 任务描述
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// 目标类型（例如：GoToLocation, PlaceItem等）
        /// </summary>
        public string TaskType { get; set; } = string.Empty;

        /// <summary>
        /// 到玩家的距离（米）
        /// </summary>
        public float DistanceToPlayer { get; set; }

        /// <summary>
        /// 是否在视野内
        /// </summary>
        public bool IsVisible { get; set; }

        /// <summary>
        /// 唯一标识符（Quest_{questId}_Task_{taskIndex}）
        /// </summary>
        public string Key { get; set; } = string.Empty;

        /// <summary>
        /// 影响范围半径（用于屏幕内圆形绘制）
        /// </summary>
        public float Radius { get; set; } = 10f;

        /// <summary>
        /// 到玩家的距离（实时更新）
        /// </summary>
        public float Distance { get; set; }

        /// <summary>
        /// 屏幕坐标（每帧更新，用于文字标签位置）
        /// </summary>
        public Vector3 ScreenPosition { get; set; }

        /// <summary>
        /// 是否在屏幕内
        /// </summary>
        public bool IsOnScreen { get; set; }

        /// <summary>
        /// 是否激活显示（任务进行中）
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// 唯一ID（用于小地图标点识别）
        /// </summary>
        public string UniqueId { get; set; } = string.Empty;
    }
}
