using UnityEngine;

namespace DuckovESPv3.Core.Systems.ESP.Minimap.Models
{
    /// <summary>
    /// 小地图标记类型
    /// </summary>
    public enum MinimapMarkerType
    {
        /// <summary>
        /// 普通物品标记（图标索引5）
        /// </summary>
        Normal = 5,

        /// <summary>
        /// 愿望单物品标记（图标索引8）
        /// </summary>
        Wishlisted = 8,

        /// <summary>
        /// 未注册钥匙标记（图标索引10）
        /// </summary>
        UnregisteredKey = 10,

        /// <summary>
        /// 建筑材料标记（图标索引7）
        /// </summary>
        BuildingMaterial = 7
    }

    /// <summary>
    /// 小地图标记数据
    /// </summary>
    public class MinimapMarkerData
    {
        /// <summary>
        /// 标记游戏对象（包含SimplePointOfInterest组件）
        /// </summary>
        public GameObject MarkerObject { get; set; }

        /// <summary>
        /// 源物品对象（ItemAgent或Lootbox）
        /// </summary>
        public object SourceObject { get; set; }

        /// <summary>
        /// 标记类型
        /// </summary>
        public MinimapMarkerType Type { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public float CreatedTime { get; set; }

        /// <summary>
        /// 物品位置
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// 标记是否有效
        /// </summary>
        public bool IsValid => MarkerObject != null;

        public MinimapMarkerData(GameObject markerObject, object sourceObject, MinimapMarkerType type, Vector3 position)
        {
            MarkerObject = markerObject;
            SourceObject = sourceObject;
            Type = type;
            Position = position;
            CreatedTime = Time.time;
        }

        /// <summary>
        /// 销毁标记
        /// </summary>
        public void Destroy()
        {
            if (MarkerObject != null)
            {
                Object.Destroy(MarkerObject);
                MarkerObject = null;
            }
        }
    }
}
