using DuckovESPv3.Core.EventBus;
using DuckovESPv3.Features.QuestMarker.Data;
using DuckovESPv3.Features.QuestMarker.Events;
using DuckovESPv3.Infrastructure.Logging;
using Duckov.MiniMaps;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DuckovESPv3.Features.QuestMarker.Collectors
{
    /// <summary>
    /// 撤离点追踪器（关卡加载时扫描一次，后续仅更新距离）
    /// </summary>
    public class EvacuationPointTracker : IDisposable
    {
        private readonly IEventBus _eventBus;
        private readonly Infrastructure.Logging.ILogger _logger;
        private readonly List<EvacuationPointData> _evacuationPoints = new List<EvacuationPointData>();
        private bool _isInitialized = false;

        // 玩家Transform引用（从LevelManager获取）
        private Transform? _playerTransform;

        public EvacuationPointTracker(IEventBus eventBus, Infrastructure.Logging.ILogger logger)
        {
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// 初始化追踪器（订阅关卡加载事件）
        /// </summary>
        public void Initialize()
        {
            if (_isInitialized)
            {
                _logger.Warning("[EvacuationPointTracker] Already initialized");
                return;
            }

            try
            {
                // 订阅关卡加载完成事件
                LevelManager.OnAfterLevelInitialized += OnLevelLoaded;

                // 立即扫描一次（如果已在关卡中）
                if (LevelManager.LevelInited)
                {
                    ScanEvacuationPoints();
                }

                _isInitialized = true;
                _logger.Info($"[EvacuationPointTracker] Initialized, found {_evacuationPoints.Count} evacuation points");
            }
            catch (Exception ex)
            {
                _logger.Error($"[EvacuationPointTracker] Initialization failed: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 事件处理：关卡加载完成
        /// </summary>
        private void OnLevelLoaded()
        {
            _logger.Debug("[EvacuationPointTracker] Level loaded, scanning evacuation points");
            ScanEvacuationPoints();
        }

        /// <summary>
        /// 扫描所有撤离点（仅在关卡加载时调用）
        /// </summary>
        private void ScanEvacuationPoints()
        {
            try
            {
                _evacuationPoints.Clear();

                // 使用Unity的FindObjectsOfType直接查找（不需要反射）
                var allPoints = UnityEngine.Object.FindObjectsOfType<SimplePointOfInterest>(true);

                if (allPoints == null || allPoints.Length == 0)
                {
                    _logger.Debug("[EvacuationPointTracker] No SimplePointOfInterest objects found");
                    PublishUpdateEvent();
                    return;
                }

                // 过滤撤离点（根据名称判断）
                foreach (var point in allPoints)
                {
                    if (point == null) continue;

                    var name = point.gameObject.name;

                    // 判断是否为撤离点（名称包含特定关键词）
                    if (IsEvacuationPoint(name))
                    {
                        var uniqueId = $"Evac_{point.gameObject.GetInstanceID()}";
                        var evacuationPoint = new EvacuationPointData
                        {
                            Position = point.transform.position,
                            Name = name,
                            Type = GetEvacuationPointType(name),
                            DistanceToPlayer = 0f, // 后续Update时更新
                            IsVisible = false,
                            ScreenPosition = Vector3.zero,
                            IsOnScreen = false,
                            UniqueId = uniqueId
                        };

                        _evacuationPoints.Add(evacuationPoint);
                    }
                }

                _logger.Debug($"[EvacuationPointTracker] Scanned {_evacuationPoints.Count} evacuation points");
                PublishUpdateEvent();
            }
            catch (Exception ex)
            {
                _logger.Error($"[EvacuationPointTracker] Failed to scan evacuation points: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 判断是否为撤离点
        /// </summary>
        private bool IsEvacuationPoint(string name)
        {
            if (string.IsNullOrEmpty(name)) return false;

            var nameLower = name.ToLower();
            return nameLower.Contains("mapelement") ||
                   nameLower.Contains("exit") ||
                   nameLower.Contains("evacuation") ||
                   nameLower.Contains("escape");
        }

        /// <summary>
        /// 根据名称推断撤离点类型
        /// </summary>
        private EvacuationPointType GetEvacuationPointType(string name)
        {
            if (string.IsNullOrEmpty(name)) return EvacuationPointType.Unknown;

            var nameLower = name.ToLower();
            if (nameLower.Contains("mapelement")) return EvacuationPointType.MapElement;
            if (nameLower.Contains("exit")) return EvacuationPointType.Exit;
            if (nameLower.Contains("evacuation")) return EvacuationPointType.Evacuation;
            
            return EvacuationPointType.Unknown;
        }

        /// <summary>
        /// 更新所有撤离点的距离和可见性（每帧调用）
        /// </summary>
        public void Update()
        {
            if (_evacuationPoints.Count == 0) return;

            try
            {
                // 从LevelManager获取玩家Transform（零反射）
                if (_playerTransform == null)
                {
                    var levelManager = LevelManager.Instance;
                    if (levelManager != null && levelManager.MainCharacter != null)
                    {
                        _playerTransform = levelManager.MainCharacter.transform;
                    }
                    else
                    {
                        return; // 玩家未找到
                    }
                }

                var playerPosition = _playerTransform.position;
                var camera = Camera.main;

                // 更新每个撤离点的距离和可见性
                foreach (var point in _evacuationPoints)
                {
                    point.DistanceToPlayer = Vector3.Distance(playerPosition, point.Position);

                    // 简单可见性检查（在摄像机视锥内）
                    if (camera != null)
                    {
                        var viewportPoint = camera.WorldToViewportPoint(point.Position);
                        point.IsVisible = viewportPoint.z > 0 && 
                                         viewportPoint.x >= 0 && viewportPoint.x <= 1 &&
                                         viewportPoint.y >= 0 && viewportPoint.y <= 1;
                        
                        // 更新屏幕坐标和可见性标志
                        point.ScreenPosition = camera.WorldToScreenPoint(point.Position);
                        point.IsOnScreen = point.IsVisible;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"[EvacuationPointTracker] Update failed: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 发布更新事件
        /// </summary>
        private void PublishUpdateEvent()
        {
            var eventData = new EvacuationPointsUpdatedEvent(_evacuationPoints.AsReadOnly());
            _eventBus.Publish(eventData);
        }

        /// <summary>
        /// 获取当前撤离点列表（只读）
        /// </summary>
        public IReadOnlyList<EvacuationPointData> GetEvacuationPoints()
        {
            return _evacuationPoints.AsReadOnly();
        }

        public void Dispose()
        {
            // TODO: 取消订阅事件
            _evacuationPoints.Clear();
            _playerTransform = null;
            _isInitialized = false;
            _logger.Info("[EvacuationPointTracker] Disposed");
        }
    }
}
