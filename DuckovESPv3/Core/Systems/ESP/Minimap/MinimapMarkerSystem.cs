using DuckovESPv3.Core.Configuration;
using DuckovESPv3.Core.EventBus;
using DuckovESPv3.Core.Systems.ESP.Events;
using UnityEngine;

namespace DuckovESPv3.Core.Systems.ESP.Minimap
{
    /// <summary>
    /// 小地图标记系统管理器 - 统一入口点
    /// </summary>
    public class MinimapMarkerSystem
    {
        private readonly ESPSystemConfig _config;
        private readonly IEventBus _eventBus;
        private MinimapMarkerService? _service;

        private bool _isInitialized = false;

        public MinimapMarkerSystem(ESPSystemConfig config, IEventBus eventBus)
        {
            _config = config;
            _eventBus = eventBus;
        }

        /// <summary>
        /// 初始化小地图标记系统
        /// </summary>
        public void Initialize()
        {
            if (_isInitialized)
            {
                Debug.LogWarning("[MinimapMarkerSystem] Already initialized");
                return;
            }

            Debug.Log("[MinimapMarkerSystem] Initializing...");

            // 创建服务
            _service = new MinimapMarkerService(_config, _eventBus);

            // 开始监听事件
            _service.StartMonitoring();

            _isInitialized = true;
            Debug.Log("[MinimapMarkerSystem] Initialized successfully");
        }

        /// <summary>
        /// 清理小地图标记系统
        /// </summary>
        public void Cleanup()
        {
            if (!_isInitialized)
            {
                return;
            }

            Debug.Log("[MinimapMarkerSystem] Cleaning up...");

            // 停止监听事件
            _service?.StopMonitoring();

            // 清理所有标记
            _service?.Cleanup();

            _service = null;
            _isInitialized = false;

            Debug.Log("[MinimapMarkerSystem] Cleanup completed");
        }

        /// <summary>
        /// 获取当前活跃标记数量
        /// </summary>
        public int GetActiveMarkerCount()
        {
            return _service?.ActiveMarkerCount ?? 0;
        }

        /// <summary>
        /// 系统是否已初始化
        /// </summary>
        public bool IsInitialized => _isInitialized;

        /// <summary>
        /// 刷新所有标记（配置变更后重新评估）
        /// </summary>
        public void RefreshAllMarkers()
        {
            if (!_isInitialized || _service == null)
            {
                Debug.LogWarning("[MinimapMarkerSystem] Cannot refresh markers: system not initialized");
                return;
            }

            Debug.Log($"[MinimapMarkerSystem] 配置变更，刷新所有标记... (最小品质: {_config.MinQualityForMapMarkers})");
            
            // 清理现有标记
            int oldCount = _service.ActiveMarkerCount;
            _service.Cleanup();
            
            Debug.Log($"[MinimapMarkerSystem] 已清理 {oldCount} 个旧标记");
            
            // 重新开始监听（这样新的箱子发现事件会按新配置创建标记）
            _service.StartMonitoring();
            
            // 发布刷新请求事件，让数据采集器重新发送所有已追踪的箱子
            _eventBus.Publish(new MinimapMarkersRefreshRequestedEvent());
            
            Debug.Log("[MinimapMarkerSystem] 已发送刷新请求事件");
        }
    }
}
