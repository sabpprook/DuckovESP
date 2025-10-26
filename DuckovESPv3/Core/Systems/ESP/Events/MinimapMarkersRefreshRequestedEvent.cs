namespace DuckovESPv3.Core.Systems.ESP.Events
{
    /// <summary>
    /// 小地图标记刷新请求事件
    /// 当配置变更需要重新评估所有标记时触发
    /// </summary>
    public class MinimapMarkersRefreshRequestedEvent
    {
        /// <summary>
        /// 请求时间戳
        /// </summary>
        public float RequestTime { get; }

        public MinimapMarkersRefreshRequestedEvent()
        {
            RequestTime = UnityEngine.Time.time;
        }
    }
}
