using DuckovESPv3.Core.EventBus;
using DuckovESPv3.Features.QuestMarker.Collectors;
using DuckovESPv3.Infrastructure.Logging;
using System;
using UnityEngine;

namespace DuckovESPv3.Features.QuestMarker.Services
{
    /// <summary>
    /// 任务标记采集服务（协调三个追踪器）
    /// 完全事件驱动架构，零周期扫描开销
    /// </summary>
    public class QuestMarkerCollectionService : IDisposable
    {
        private readonly IEventBus _eventBus;
        private readonly Infrastructure.Logging.ILogger _logger;

        // 三个追踪器
        private QuestItemDetector? _questItemDetector;
        private EvacuationPointTracker? _evacuationPointTracker;
        private QuestZoneTracker? _questZoneTracker;

        private bool _isInitialized = false;

        public QuestMarkerCollectionService(IEventBus eventBus, Infrastructure.Logging.ILogger logger)
        {
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// 初始化服务（创建并初始化所有追踪器）
        /// </summary>
        public void Initialize()
        {
            if (_isInitialized)
            {
                _logger.Warning("[QuestMarkerCollectionService] Already initialized");
                return;
            }

            try
            {
                _logger.Info("[QuestMarkerCollectionService] Initializing...");

                // 创建并初始化任务物品检测器（P0 - 最高优先级）
                _questItemDetector = new QuestItemDetector(_eventBus, _logger);
                _questItemDetector.Initialize();

                // 创建并初始化撤离点追踪器（P1）
                _evacuationPointTracker = new EvacuationPointTracker(_eventBus, _logger);
                _evacuationPointTracker.Initialize();

                // 创建并初始化任务地点追踪器（P2）
                _questZoneTracker = new QuestZoneTracker(_eventBus, _logger);
                _questZoneTracker.Initialize();

                _isInitialized = true;
                _logger.Info("[QuestMarkerCollectionService] Initialized successfully");
            }
            catch (Exception ex)
            {
                _logger.Error($"[QuestMarkerCollectionService] Initialization failed: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 每帧更新（仅更新距离和可见性，无扫描开销）
        /// </summary>
        public void Update()
        {
            if (!_isInitialized) return;

            try
            {
                // 更新撤离点的距离和可见性
                _evacuationPointTracker?.Update();

                // 更新任务地点的距离和可见性
                _questZoneTracker?.Update();

                // 注意：QuestItemDetector 是完全事件驱动的，不需要 Update
            }
            catch (Exception ex)
            {
                _logger.Error($"[QuestMarkerCollectionService] Update failed: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 手动刷新所有追踪器（用于调试或强制更新）
        /// </summary>
        public void RefreshAll()
        {
            try
            {
                _logger.Debug("[QuestMarkerCollectionService] Manually refreshing all trackers");
                
                // 撤离点追踪器在关卡加载时自动刷新，这里无需手动调用
                // 任务地点追踪器在关卡加载时自动刷新，但可以手动触发
                _questZoneTracker?.ScanAllQuestZones();
                
                _logger.Debug("[QuestMarkerCollectionService] Manual refresh completed");
            }
            catch (Exception ex)
            {
                _logger.Error($"[QuestMarkerCollectionService] RefreshAll failed: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 获取当前统计信息（用于调试UI）
        /// </summary>
        public string GetStatistics()
        {
            try
            {
                var questItemCount = _questItemDetector?.GetQuestItems().Count ?? 0;
                var evacuationPointCount = _evacuationPointTracker?.GetEvacuationPoints().Count ?? 0;
                var questZoneCount = _questZoneTracker?.GetQuestZones().Count ?? 0;

                return $"任务物品: {questItemCount} | 撤离点: {evacuationPointCount} | 任务地点: {questZoneCount}";
            }
            catch (Exception ex)
            {
                _logger.Error($"[QuestMarkerCollectionService] GetStatistics failed: {ex.Message}", ex);
                return "统计信息获取失败";
            }
        }

        /// <summary>
        /// 获取撤离点追踪器（用于传递给ESPSystemManager）
        /// </summary>
        public EvacuationPointTracker? GetEvacuationPointTracker() => _evacuationPointTracker;

        /// <summary>
        /// 获取任务地点追踪器（用于传递给ESPSystemManager）
        /// </summary>
        public QuestZoneTracker? GetQuestZoneTracker() => _questZoneTracker;

        public void Dispose()
        {
            try
            {
                _logger.Info("[QuestMarkerCollectionService] Disposing...");

                _questItemDetector?.Dispose();
                _questItemDetector = null;

                _evacuationPointTracker?.Dispose();
                _evacuationPointTracker = null;

                _questZoneTracker?.Dispose();
                _questZoneTracker = null;

                _isInitialized = false;
                _logger.Info("[QuestMarkerCollectionService] Disposed successfully");
            }
            catch (Exception ex)
            {
                _logger.Error($"[QuestMarkerCollectionService] Dispose failed: {ex.Message}", ex);
            }
        }
    }
}
