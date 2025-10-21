using UnityEngine;

namespace DuckovESP
{
    /// <summary>
    /// 任务区域标记数据
    /// </summary>
    public class QuestZoneMarkerData
    {
        /// <summary>
        /// 任务计数区域组件
        /// </summary>
        public CountDownArea countDownArea;

        /// <summary>
        /// 地图位置显示组件
        /// </summary>
        public ShowLocationInMap locationMarker;

        /// <summary>
        /// 到达检测的碰撞体
        /// </summary>
        public Collider triggerCollider;

        /// <summary>
        /// 小地图标记对象
        /// </summary>
        public GameObject miniMapMarker;

        /// <summary>
        /// 任务名称
        /// </summary>
        public string displayName;

        /// <summary>
        /// 区域中心位置
        /// </summary>
        public Vector3 centerPosition;

        /// <summary>
        /// 到达范围半径（游戏单位）
        /// </summary>
        public float radius;

        /// <summary>
        /// 需要的停留时间（秒）
        /// </summary>
        public float requiredTime;

        /// <summary>
        /// 是否处于激活状态
        /// </summary>
        public bool isActive;

        /// <summary>
        /// 完成进度 (0-1)
        /// </summary>
        public float progress;

        /// <summary>
        /// 剩余时间（秒）
        /// </summary>
        public float remainingTime;

        /// <summary>
        /// 距离玩家的距离
        /// </summary>
        public float distance;
    }
}
