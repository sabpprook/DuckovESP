using DuckovESPv3.Core.EventBus;
using DuckovESPv3.Features.QuestMarker.Data;
using DuckovESPv3.Features.QuestMarker.Events;
using DuckovESPv3.Infrastructure.Logging;
using Duckov.Quests;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DuckovESPv3.Features.QuestMarker.Collectors
{
    /// <summary>
    /// 任务地点追踪器（完全事件驱动，零周期扫描，零反射）
    /// 关卡加载时扫描一次，通过事件监听任务完成并移除标记
    /// </summary>
    public class QuestZoneTracker : IDisposable
    {
        private readonly IEventBus _eventBus;
        private readonly Infrastructure.Logging.ILogger _logger;
        private readonly Dictionary<string, QuestZoneData> _questZones = new Dictionary<string, QuestZoneData>();
        private bool _isInitialized = false;

        // 玩家Transform引用（从LevelManager获取）
        private Transform? _playerTransform;

        public QuestZoneTracker(IEventBus eventBus, Infrastructure.Logging.ILogger logger)
        {
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// 初始化追踪器（订阅事件，零反射）
        /// </summary>
        public void Initialize()
        {
            if (_isInitialized)
            {
                _logger.Warning("[QuestZoneTracker] Already initialized");
                return;
            }

            try
            {
                // 订阅任务完成事件（零反射，直接使用游戏API）
                QuestManager.OnTaskFinishedEvent += OnTaskFinished;
                Quest.onQuestCompleted += OnQuestCompleted;

                // 订阅关卡加载事件
                LevelManager.OnAfterLevelInitialized += OnLevelLoaded;

                // 立即扫描一次（如果已在关卡中）
                if (LevelManager.LevelInited)
                {
                    ScanAllQuestZones();
                }

                _isInitialized = true;
                _logger.Info($"[QuestZoneTracker] Initialized, found {_questZones.Count} quest zones");
            }
            catch (Exception ex)
            {
                _logger.Error($"[QuestZoneTracker] Initialization failed: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 事件处理：关卡加载完成
        /// </summary>
        private void OnLevelLoaded()
        {
            _logger.Debug("[QuestZoneTracker] Level loaded, scanning quest zones");
            ScanAllQuestZones();
        }

        /// <summary>
        /// 事件处理：任务目标完成（零反射）
        /// </summary>
        private void OnTaskFinished(Quest quest, Task task)
        {
            try
            {
                var key = GetTaskKey(quest.ID, task.ID);
                
                if (_questZones.Remove(key))
                {
                    _logger.Debug($"[QuestZoneTracker] Removed quest zone: {key}");
                    PublishUpdateEvent();
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"[QuestZoneTracker] OnTaskFinished failed: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 事件处理：整个任务完成（零反射）
        /// </summary>
        private void OnQuestCompleted(Quest quest)
        {
            try
            {
                // 移除该任务的所有地点
                var keysToRemove = _questZones.Keys
                    .Where(k => k.StartsWith($"Quest_{quest.ID}_"))
                    .ToList();

                foreach (var key in keysToRemove)
                {
                    _questZones.Remove(key);
                }

                if (keysToRemove.Count > 0)
                {
                    _logger.Debug($"[QuestZoneTracker] Removed {keysToRemove.Count} quest zones for quest {quest.ID}");
                    PublishUpdateEvent();
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"[QuestZoneTracker] OnQuestCompleted failed: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 扫描所有激活任务的地点（零反射，仅在关卡加载时调用）
        /// </summary>
        public void ScanAllQuestZones()
        {
            try
            {
                _questZones.Clear();

                // 获取所有激活的任务（零反射）
                var questManager = QuestManager.Instance;
                if (questManager == null)
                {
                    _logger.Warning("[QuestZoneTracker] QuestManager not available");
                    return;
                }

                var activeQuests = questManager.ActiveQuests;
                if (activeQuests == null || activeQuests.Count == 0)
                {
                    _logger.Debug("[QuestZoneTracker] No active quests");
                    PublishUpdateEvent();
                    return;
                }

                _logger.Info($"[QuestZoneTracker] 🔍 Found {activeQuests.Count} active quests");

                // 遍历每个任务
                foreach (var quest in activeQuests)
                {
                    if (quest == null) continue;
                    _logger.Debug($"[QuestZoneTracker] Processing quest: {quest.ID} - {quest.DisplayName}");
                    int tasksCount = quest.Tasks?.Count ?? 0;
                    _logger.Debug($"[QuestZoneTracker] Quest {quest.ID} has {tasksCount} tasks");
                    ScanQuestTasks(quest);
                }

                _logger.Debug($"[QuestZoneTracker] Scanned {_questZones.Count} quest zones");
                PublishUpdateEvent();
            }
            catch (Exception ex)
            {
                _logger.Error($"[QuestZoneTracker] Failed to scan quest zones: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 扫描单个任务的所有目标（零反射）
        /// </summary>
        private void ScanQuestTasks(Quest quest)
        {
            try
            {
                // 直接访问Quest属性（零反射）
                var questId = quest.ID;
                var tasks = quest.Tasks;

                if (tasks == null || tasks.Count == 0)
                {
                    _logger.Debug($"[QuestZoneTracker] Quest {questId} has no tasks");
                    return;
                }

                _logger.Debug($"[QuestZoneTracker] Quest {questId} has {tasks.Count} tasks to scan");

                // 遍历每个任务目标
                for (int i = 0; i < tasks.Count; i++)
                {
                    var task = tasks[i];
                    if (task == null)
                    {
                        _logger.Debug($"[QuestZoneTracker] Quest {questId} task {i} is null");
                        continue;
                    }

                    var taskTypeName = task.GetType().Name;
                    _logger.Info($"[QuestZoneTracker] Quest {questId} task {i}: Type={taskTypeName}, ID={task.ID}, Description={task.Description}");

                    // 检查是否是地点类任务且未完成
                    var zoneData = ParseLocationTask(quest, task);
                    if (zoneData != null)
                    {
                        var key = GetTaskKey(questId, task.ID);
                        _questZones[key] = zoneData;
                        _logger.Info($"[QuestZoneTracker] ✓ Added quest zone: {key} - {zoneData.Description}");
                    }
                    else
                    {
                        _logger.Info($"[QuestZoneTracker] ✗ Quest {questId} task {i} ({taskTypeName}) not a location task or already completed");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"[QuestZoneTracker] Failed to scan quest tasks: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 解析位置类型的任务目标（最小反射开销）
        /// </summary>
        private QuestZoneData? ParseLocationTask(Quest quest, Task task)
        {
            try
            {
                // 注意：Task是抽象类，需要检查具体子类是否包含位置信息
                // 使用最小反射开销的方式检查
                var taskType = task.GetType();
                var taskTypeName = taskType.Name;

                _logger.Debug($"[QuestZoneTracker] Parsing task type: {taskTypeName}");

                // 只处理位置相关的任务类型（根据类名判断）
                if (!IsLocationTaskByTypeName(taskTypeName))
                {
                    _logger.Debug($"[QuestZoneTracker] Task type {taskTypeName} is not a location task");
                    return null;
                }

                _logger.Debug($"[QuestZoneTracker] Task type {taskTypeName} IS a location task, attempting to get position");

                // 尝试获取目标位置（使用属性反射，但缓存在局部变量中以提高性能）
                Vector3? targetPosition = GetTaskPosition(task, taskType);
                if (!targetPosition.HasValue || targetPosition.Value == Vector3.zero)
                {
                    _logger.Debug($"[QuestZoneTracker] Failed to get position for task {taskTypeName}");
                    return null;
                }

                _logger.Debug($"[QuestZoneTracker] Got position {targetPosition.Value} for task {taskTypeName}");

                // 检查是否已完成
                if (IsTaskCompleted(task, taskType))
                {
                    _logger.Debug($"[QuestZoneTracker] Task {taskTypeName} is already completed");
                    return null;
                }

                var key = GetTaskKey(quest.ID, task.ID);
                return new QuestZoneData
                {
                    Position = targetPosition.Value,
                    QuestId = quest.ID,
                    TaskIndex = task.ID, // 使用Task.ID作为索引
                    Description = task.Description ?? $"Quest {quest.ID} Task {task.ID}",
                    TaskType = taskTypeName,
                    DistanceToPlayer = 0f, // 后续Update时更新
                    IsVisible = false,
                    Key = key,
                    Radius = 10f,
                    Distance = 0f,
                    ScreenPosition = Vector3.zero,
                    IsOnScreen = false,
                    IsActive = true,
                    UniqueId = key
                };
            }
            catch (Exception ex)
            {
                _logger.Error($"[QuestZoneTracker] Failed to parse location task: {ex.Message}", ex);
                return null;
            }
        }

        /// <summary>
        /// 根据任务类型名判断是否为位置任务（零反射）
        /// </summary>
        private bool IsLocationTaskByTypeName(string typeName)
        {
            if (string.IsNullOrEmpty(typeName)) return false;

            var lowerTypeName = typeName.ToLower();
            
            // 【关键改变】不再根据类型名判断，而是直接尝试提取位置
            // 所有类型都有可能包含位置信息：
            // - QuestTask_ReachLocation: 有 target Transform
            // - QuestTask_TaskEvent: 有 mapElement
            // - SubmitItems: 可能有位置
            // 让我们通过直接查找位置字段/属性来判断
            return true;  // 先尝试所有任务类型，在 ParseLocationTask 中过滤
        }

        /// <summary>
        /// 获取任务目标位置（最小反射开销）
        /// </summary>
        private Vector3? GetTaskPosition(Task task, Type taskType)
        {
            try
            {
                var taskTypeName = taskType.Name;
                
                // 【策略1】QuestTask_ReachLocation: 尝试获取 target Transform 或 location
                if (taskTypeName.Contains("ReachLocation"))
                {
                    var pos = TryExtractReachLocationPosition(task, taskType);
                    if (pos.HasValue)
                    {
                        _logger.Info($"[QuestZoneTracker] ✓ QuestTask_ReachLocation 从 target 获取位置: {pos.Value}");
                        return pos.Value;
                    }
                }
                
                // 【策略2】任何任务类型: 尝试从 mapElement 获取位置（QuestTask_TaskEvent, SubmitItems 等）
                var mapElementPos = TryExtractMapElementPosition(task, taskType);
                if (mapElementPos.HasValue)
                {
                    _logger.Info($"[QuestZoneTracker] ✓ 从 mapElement 获取位置 ({taskTypeName}): {mapElementPos.Value}");
                    return mapElementPos.Value;
                }
                
                // 【策略3】从 SpawnPrefabForTask 组件查找位置
                var spawnPos = TryExtractSpawnPrefabForTaskPosition(task, taskType);
                if (spawnPos.HasValue)
                {
                    _logger.Info($"[QuestZoneTracker] ✓ 从 SpawnPrefabForTask 组件获取位置 ({taskTypeName}): {spawnPos.Value}");
                    return spawnPos.Value;
                }
                
                _logger.Info($"[QuestZoneTracker] ✗ 无法从任何方式获取位置 ({taskTypeName})");
                return null;
            }
            catch (Exception ex)
            {
                _logger.Info($"[QuestZoneTracker] GetTaskPosition 异常: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 【策略1】从 QuestTask_ReachLocation 的 target Transform 或 location 获取位置
        /// </summary>
        private Vector3? TryExtractReachLocationPosition(Task task, Type taskType)
        {
            try
            {
                // 方案 A: 直接从 target Transform 获取（最快）
                var targetField = taskType.GetField("target",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                
                if (targetField != null && targetField.FieldType == typeof(Transform))
                {
                    var targetTransform = targetField.GetValue(task) as Transform;
                    if (targetTransform != null)
                    {
                        _logger.Debug($"[QuestZoneTracker] 从 target Transform 成功提取: {targetTransform.position}");
                        return targetTransform.position;
                    }
                }
                
                // 方案 B: 通过 location.GetLocationTransform() 获取
                var locationField = taskType.GetField("location",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                
                if (locationField != null)
                {
                    var locationObj = locationField.GetValue(task);
                    if (locationObj != null)
                    {
                        var getLocationTransformMethod = locationObj.GetType().GetMethod("GetLocationTransform",
                            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                        
                        if (getLocationTransformMethod != null)
                        {
                            var targetTransform = getLocationTransformMethod.Invoke(locationObj, null) as Transform;
                            if (targetTransform != null)
                            {
                                _logger.Debug($"[QuestZoneTracker] 从 location.GetLocationTransform() 成功提取: {targetTransform.position}");
                                return targetTransform.position;
                            }
                        }
                    }
                }
                
                return null;
            }
            catch (Exception ex)
            {
                _logger.Debug($"[QuestZoneTracker] TryExtractReachLocationPosition 异常: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 【策略2】从 mapElement (MapElementForTask) 提取位置信息
        /// 用于 QuestTask_TaskEvent, SubmitItems 等
        /// </summary>
        private Vector3? TryExtractMapElementPosition(Task task, Type taskType)
        {
            try
            {
                // 获取 mapElement 字段
                var mapElementField = taskType.GetField("mapElement",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                
                if (mapElementField == null)
                    return null;
                
                var mapElement = mapElementField.GetValue(task);
                if (mapElement == null)
                    return null;
                
                _logger.Debug($"[QuestZoneTracker] 找到 mapElement，类型: {mapElement.GetType().Name}");
                
                // 获取 locations 属性
                var locationsProperty = mapElement.GetType().GetProperty("locations",
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                
                if (locationsProperty == null)
                    return null;
                
                var locationsList = locationsProperty.GetValue(mapElement);
                if (locationsList == null)
                    return null;
                
                // 获取 List 的 Count
                var countProp = locationsList.GetType().GetProperty("Count");
                if (countProp == null)
                    return null;
                
                int count = (int)countProp.GetValue(locationsList);
                _logger.Debug($"[QuestZoneTracker] mapElement 中有 {count} 个 locations");
                
                if (count == 0)
                    return null;
                
                // 获取第一个位置
                var indexer = locationsList.GetType().GetProperty("Item",
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                
                if (indexer == null)
                    return null;
                
                var firstLocation = indexer.GetValue(locationsList, new object[] { 0 });
                if (firstLocation == null)
                    return null;
                
                // 从第一个位置提取坐标
                var tryGetPositionMethod = firstLocation.GetType().GetMethod("TryGetLocationPosition",
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                
                if (tryGetPositionMethod != null)
                {
                    Vector3 tempPos = Vector3.zero;
                    var parameters = new object[] { tempPos };
                    bool success = (bool)tryGetPositionMethod.Invoke(firstLocation, parameters);
                    if (success)
                    {
                        Vector3 resultPos = (Vector3)parameters[0];
                        _logger.Debug($"[QuestZoneTracker] 从 mapElement.locations[0] 成功提取: {resultPos}");
                        return resultPos;
                    }
                }
                
                return null;
            }
            catch (Exception ex)
            {
                _logger.Debug($"[QuestZoneTracker] TryExtractMapElementPosition 异常: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 【策略3】从 SpawnPrefabForTask 组件查找位置
        /// </summary>
        private Vector3? TryExtractSpawnPrefabForTaskPosition(Task task, Type taskType)
        {
            try
            {
                var taskComponent = task as MonoBehaviour;
                if (taskComponent == null)
                    return null;
                
                var gameObject = taskComponent.gameObject;
                if (gameObject == null)
                    return null;
                
                _logger.Debug($"[QuestZoneTracker] 尝试从 SpawnPrefabForTask 组件查找位置");
                
                // 获取所有组件
                var components = gameObject.GetComponents<MonoBehaviour>();
                MonoBehaviour? spawnPrefabForTask = null;
                
                foreach (var component in components)
                {
                    if (component.GetType().Name == "SpawnPrefabForTask")
                    {
                        spawnPrefabForTask = component;
                        _logger.Debug($"[QuestZoneTracker] 找到 SpawnPrefabForTask 组件");
                        break;
                    }
                }
                
                if (spawnPrefabForTask == null)
                    return null;
                
                // 从 SpawnPrefabForTask 的 locations 字段获取位置
                var locationsField = spawnPrefabForTask.GetType().GetField("locations",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                
                if (locationsField == null)
                    return null;
                
                var locationsList = locationsField.GetValue(spawnPrefabForTask);
                if (locationsList == null)
                    return null;
                
                // 获取 List 的 Count
                var countProp = locationsList.GetType().GetProperty("Count");
                if (countProp == null)
                    return null;
                
                int count = (int)countProp.GetValue(locationsList);
                if (count == 0)
                    return null;
                
                // 获取第一个位置
                var indexer = locationsList.GetType().GetProperty("Item",
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                
                if (indexer == null)
                    return null;
                
                var firstLocation = indexer.GetValue(locationsList, new object[] { 0 });
                if (firstLocation == null)
                    return null;
                
                // 从第一个位置提取坐标
                var tryGetPositionMethod = firstLocation.GetType().GetMethod("TryGetLocationPosition",
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                
                if (tryGetPositionMethod != null)
                {
                    Vector3 tempPos = Vector3.zero;
                    var parameters = new object[] { tempPos };
                    bool success = (bool)tryGetPositionMethod.Invoke(firstLocation, parameters);
                    if (success)
                    {
                        Vector3 resultPos = (Vector3)parameters[0];
                        _logger.Debug($"[QuestZoneTracker] 从 SpawnPrefabForTask 成功提取: {resultPos}");
                        return resultPos;
                    }
                }
                
                return null;
            }
            catch (Exception ex)
            {
                _logger.Debug($"[QuestZoneTracker] TryExtractSpawnPrefabForTaskPosition 异常: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 判断任务目标是否已完成（最小反射开销）
        /// </summary>
        private bool IsTaskCompleted(Task task, Type taskType)
        {
            try
            {
                // 尝试属性
                var property = taskType.GetProperty("IsCompleted", 
                    System.Reflection.BindingFlags.Public | 
                    System.Reflection.BindingFlags.Instance);

                if (property != null && property.PropertyType == typeof(bool))
                {
                    return (bool)property.GetValue(task);
                }

                // 尝试字段
                var field = taskType.GetField("isCompleted",
                    System.Reflection.BindingFlags.Public |
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Instance);

                if (field != null && field.FieldType == typeof(bool))
                {
                    return (bool)field.GetValue(task);
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 生成任务目标的唯一键
        /// </summary>
        private string GetTaskKey(int questId, int taskId)
        {
            return $"Quest_{questId}_Task_{taskId}";
        }

        /// <summary>
        /// 更新所有任务地点的距离和可见性（每帧调用，零反射）
        /// </summary>
        public void Update()
        {
            if (_questZones.Count == 0) return;

            try
            {
                // 获取玩家位置（零反射，遵循ESPSystemManager模式）
                if (_playerTransform == null)
                {
                    var levelManager = LevelManager.Instance;
                    if (levelManager != null && levelManager.MainCharacter != null)
                    {
                        _playerTransform = levelManager.MainCharacter.transform;
                    }
                    else
                    {
                        return; // 玩家未就绪
                    }
                }

                var playerPosition = _playerTransform.position;
                var camera = Camera.main;

                // 更新每个任务地点的距离和可见性
                foreach (var zone in _questZones.Values)
                {
                    zone.DistanceToPlayer = Vector3.Distance(playerPosition, zone.Position);
                    zone.Distance = zone.DistanceToPlayer; // 同步新字段

                    // 简单可见性检查（在摄像机视锥内）
                    if (camera != null)
                    {
                        var viewportPoint = camera.WorldToViewportPoint(zone.Position);
                        zone.IsVisible = viewportPoint.z > 0 && 
                                        viewportPoint.x >= 0 && viewportPoint.x <= 1 &&
                                        viewportPoint.y >= 0 && viewportPoint.y <= 1;
                        
                        // 更新屏幕坐标和可见性标志
                        zone.ScreenPosition = camera.WorldToScreenPoint(zone.Position);
                        zone.IsOnScreen = zone.IsVisible;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"[QuestZoneTracker] Update failed: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 发布更新事件
        /// </summary>
        private void PublishUpdateEvent()
        {
            var eventData = new QuestZonesUpdatedEvent(_questZones.Values.ToList().AsReadOnly());
            _eventBus.Publish(eventData);
        }

        /// <summary>
        /// 获取当前任务地点列表（只读）
        /// </summary>
        public IReadOnlyList<QuestZoneData> GetQuestZones()
        {
            return _questZones.Values.ToList().AsReadOnly();
        }

        /// <summary>
        /// 手动刷新任务地点（用于统计/调试）
        /// </summary>
        public void Refresh()
        {
            ScanAllQuestZones();
        }

        /// <summary>
        /// 获取统计信息
        /// </summary>
        public (int Total, int Visible) GetStatistics()
        {
            return (_questZones.Count, _questZones.Count(z => z.Value.IsVisible));
        }

        public void Dispose()
        {
            // 取消订阅事件（零反射）
            QuestManager.OnTaskFinishedEvent -= OnTaskFinished;
            Quest.onQuestCompleted -= OnQuestCompleted;
            LevelManager.OnAfterLevelInitialized -= OnLevelLoaded;

            _questZones.Clear();
            _playerTransform = null;
            _isInitialized = false;
            _logger.Info("[QuestZoneTracker] Disposed");
        }
    }
}
