using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ItemStatsSystem;
using UnityEngine;
using DuckovESPv3.Core.EventBus;
using DuckovESPv3.Infrastructure.Logging;
using DuckovESPv3.Core.Systems.ESP.Events;
using DuckovESPv3.Core.Systems.ESP.Models;
using DuckovESPv3.Features.QuestMarker.Events;

namespace DuckovESPv3.Core.Systems.ESP.Detection
{
    /// <summary>
    /// 箱子数据采集器（LootboxDataCollector）
    /// 
    /// 核心职责：
    /// 1. 监听LevelManager中的战利品箱字典变化
    /// 2. 维护战利品箱数据缓存
    /// 3. 发布数据变化事件
    /// 
    /// 性能特征：
    /// - 初始化：O(n)，其中 n = 现存箱子数
    /// - 运行时：O(1) 对每个新箱子
    /// - GC：初始化时 ~50-100KB，运行时零额外分配
    /// 
    /// 实现方案：通过FindObjectsOfType初始化，然后被动监听
    /// </summary>
    public class LootboxDataCollector : ILootboxCollectionService
    {
        // ===== 依赖注入 =====
        private readonly IEventBus _eventBus;
        private readonly Infrastructure.Logging.ILogger _logger;

        // ===== 状态管理 =====
        private bool _isInitialized;
        public bool IsInitialized => _isInitialized;

        // ===== 数据缓存 =====
        private Dictionary<InteractableLootbox, LootboxData> _trackedLootboxes;
        private HashSet<InteractableLootbox> _discoveredBoxes;
        
        // ===== Quest物品检测 =====
        private HashSet<int> _questItemIds = new HashSet<int>();

        // ===== 反射缓存 =====
        private static FieldInfo? _lootBoxInventoriesField;
        private static bool _reflectionInitialized = false;

        // ===== 静态初始化 =====
        static LootboxDataCollector()
        {
            CacheReflectionFields();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public LootboxDataCollector(IEventBus eventBus, Infrastructure.Logging.ILogger logger)
        {
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _trackedLootboxes = new Dictionary<InteractableLootbox, LootboxData>();
            _discoveredBoxes = new HashSet<InteractableLootbox>();
            _isInitialized = false;
        }

        /// <summary>
        /// 缓存反射字段信息（静态方法，仅执行一次）
        /// </summary>
        private static void CacheReflectionFields()
        {
            if (_reflectionInitialized)
                return;

            try
            {
                var levelManagerType = typeof(LevelManager);
                _lootBoxInventoriesField = levelManagerType.GetField(
                    "_lootBoxInventories",
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.IgnoreCase
                );

                if (_lootBoxInventoriesField == null)
                {
                    Debug.LogWarning("[LootboxDataCollector] 警告：无法找到 LevelManager._lootBoxInventories 字段");
                }

                _reflectionInitialized = true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[LootboxDataCollector] 缓存反射字段失败: {ex.Message}");
                _reflectionInitialized = true;
            }
        }

        /// <summary>
        /// 初始化采集器
        /// 在 LevelManager.OnAfterLevelInitialized 时调用
        /// </summary>
        public void Initialize()
        {
            if (_isInitialized)
            {
                _logger.Debug("[LootboxDataCollector] 已初始化过，跳过重复初始化");
                return;
            }

            try
            {
                _logger.Info("[LootboxDataCollector] 开始初始化箱子采集器...");

                // 订阅 Lootbox 生成事件（实时检测敌人死亡后的箱子）
                LootboxSpawnHook.OnLootboxSpawned += HandleLootboxSpawned;
                _logger.Info("[LootboxDataCollector] 已订阅 LootboxSpawnHook 事件");

                // 订阅箱子可见性变化事件（0轮询监听任务/蓝图锁定的箱子）
                Hooks.LootboxVisibilityHook.OnLootboxVisibilityChanged += HandleLootboxVisibilityChanged;
                _logger.Info("[LootboxDataCollector] 已订阅 LootboxVisibilityHook 事件");

                // 订阅小地图标记刷新请求事件
                _eventBus.Subscribe<Events.MinimapMarkersRefreshRequestedEvent>(OnMinimapRefreshRequested);
                _logger.Info("[LootboxDataCollector] 已订阅 MinimapMarkersRefreshRequestedEvent 事件");
                
                // 订阅Quest物品更新事件
                _eventBus.Subscribe<QuestItemsUpdatedEvent>(OnQuestItemsUpdated);
                _logger.Info("[LootboxDataCollector] 已订阅 QuestItemsUpdatedEvent 事件");

                // 扫描现有的箱子
                ScanExistingLootboxes();

                _isInitialized = true;
                _logger.Info($"[LootboxDataCollector] ✓ 初始化完成，已追踪 {_trackedLootboxes.Count} 个战利品箱");
            }
            catch (Exception ex)
            {
                _logger.Error($"[LootboxDataCollector] 初始化失败: {ex.Message}\n{ex.StackTrace}");
                _isInitialized = false;
            }
        }
        
        /// <summary>
        /// 重新扫描（用于子场景加载，不清理现有数据）
        /// </summary>
        public void Rescan()
        {
            try
            {
                _logger.Info("[LootboxDataCollector] 开始重新扫描箱子（追加模式）...");
                
                int beforeCount = _trackedLootboxes.Count;
                ScanExistingLootboxes();
                int afterCount = _trackedLootboxes.Count;
                
                _logger.Info($"[LootboxDataCollector] ✓ 重新扫描完成，新增 {afterCount - beforeCount} 个战利品箱，总计 {afterCount} 个");
            }
            catch (Exception ex)
            {
                _logger.Error($"[LootboxDataCollector] 重新扫描失败: {ex.Message}\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// 扫描并处理所有现存的战利品箱
        /// </summary>
        private void ScanExistingLootboxes()
        {
            try
            {
                var allLootboxes = UnityEngine.Object.FindObjectsOfType<InteractableLootbox>();
                _logger.Info($"[LootboxDataCollector] 扫描找到 {allLootboxes.Length} 个战利品箱");

                int processedCount = 0;
                int skippedCount = 0;

                foreach (var lootbox in allLootboxes)
                {
                    try
                    {
                        if (lootbox == null)
                        {
                            skippedCount++;
                            continue;
                        }

                        // 检查箱子是否在场景中激活
                        if (!lootbox.gameObject.activeInHierarchy)
                        {
                            _logger.Debug($"[LootboxDataCollector] 跳过未激活的箱子: {lootbox.name}");
                            skippedCount++;
                            continue;
                        }

                        // 检查箱子组件是否启用
                        if (!lootbox.enabled)
                        {
                            _logger.Debug($"[LootboxDataCollector] 跳过未启用的箱子组件: {lootbox.name}");
                            skippedCount++;
                            continue;
                        }

                        // 检查箱子是否应该可见（评估 SetActiveByCondition 条件）
                        if (!ShouldLootboxBeVisible(lootbox))
                        {
                            _logger.Debug($"[LootboxDataCollector] 跳过条件不满足的箱子: {lootbox.name}");
                            skippedCount++;
                            continue;
                        }

                        var key = lootbox.GetInstanceID();
                        if (_discoveredBoxes.Contains(lootbox))
                        {
                            skippedCount++;
                            continue;
                        }

                        var inventory = InteractableLootbox.GetOrCreateInventory(lootbox);
                        if (inventory == null)
                        {
                            _logger.Warning($"[LootboxDataCollector] 箱子 {lootbox.name} 的 Inventory 为 null");
                            skippedCount++;
                            continue;
                        }

                        ProcessLootbox(lootbox, inventory);
                        processedCount++;
                    }
                    catch (Exception ex)
                    {
                        _logger.Warning($"[LootboxDataCollector] 处理箱子时出错: {ex.Message}");
                        skippedCount++;
                    }
                }

                _logger.Debug($"[LootboxDataCollector] 扫描完成：处理 {processedCount} 个，跳过 {skippedCount} 个");
            }
            catch (Exception ex)
            {
                _logger.Error($"[LootboxDataCollector] 扫描箱子失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 处理单个箱子：创建数据模型，发布事件
        /// </summary>
        private void ProcessLootbox(InteractableLootbox lootbox, Inventory inventory)
        {
            try
            {
                var data = CreateLootboxData(lootbox, inventory);
                if (data == null)
                {
                    _logger.Warning($"[LootboxDataCollector] 创建箱子数据失败");
                    return;
                }

                _trackedLootboxes[lootbox] = data;
                _discoveredBoxes.Add(lootbox);

                // 发布事件
                var evt = new LootboxDiscoveredEvent(data);
                _eventBus.Publish(evt);

                _logger.Debug($"[LootboxDataCollector] ✓ 添加箱子: {lootbox.name}, 物品数: {data.Items.Count}, 位置: {data.Position}");
            }
            catch (Exception ex)
            {
                _logger.Error($"[LootboxDataCollector] 处理箱子时出错: {ex.Message}\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// 创建箱子数据模型
        /// </summary>
        private LootboxData? CreateLootboxData(InteractableLootbox lootbox, Inventory inventory)
        {
            try
            {
                var items = new List<Item>();

                // Inventory 实现了 IEnumerable<Item>，可以直接迭代
                foreach (var item in inventory)
                {
                    if (item != null)
                    {
                        items.Add(item);
                    }
                }

                var data = new LootboxData
                {
                    Lootbox = lootbox,
                    Inventory = inventory,
                    Position = lootbox.transform.position,
                    Items = items,
                    DiscoveryTime = DateTime.UtcNow,
                    Key = lootbox.GetInstanceID()
                };

                // 分析内容
                AnalyzeLootboxContents(data);

                return data;
            }
            catch (Exception ex)
            {
                _logger.Error($"[LootboxDataCollector] 创建LootboxData失败: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 检查箱子是否应该可见
        /// 手动评估 SetActiveByCondition 条件，避免时序问题
        /// </summary>
        private bool ShouldLootboxBeVisible(InteractableLootbox lootbox)
        {
            try
            {
                // 获取 SetActiveByCondition 组件
                var setActiveByCondition = lootbox.GetComponent<SetActiveByCondition>();
                if (setActiveByCondition != null)
                {
                    // 通过反射获取必要字段
                    var targetObjectField = setActiveByCondition.GetType().GetField("targetObject", 
                        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    var inverseField = setActiveByCondition.GetType().GetField("inverse", 
                        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    var conditionsField = setActiveByCondition.GetType().GetField("conditions", 
                        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                    if (targetObjectField != null && conditionsField != null)
                    {
                        var targetObject = targetObjectField.GetValue(setActiveByCondition) as GameObject;
                        var inverse = inverseField != null && (bool)inverseField.GetValue(setActiveByCondition);
                        var conditions = conditionsField.GetValue(setActiveByCondition) as System.Collections.IList;

                        // 评估条件
                        bool satisfied = conditions != null ? EvaluateConditions(conditions) : true;
                        if (inverse)
                            satisfied = !satisfied;

                        // 如果目标对象是箱子自己或其父对象
                        if (targetObject == lootbox.gameObject || 
                            (targetObject != null && lootbox.transform.IsChildOf(targetObject.transform)))
                        {
                            if (!satisfied)
                            {
                                _logger.Debug($"[LootboxDataCollector] 箱子 {lootbox.name} 的 SetActiveByCondition 条件不满足");
                                return false;
                            }
                        }
                    }
                }

                // 没有 SetActiveByCondition 或条件满足，认为应该可见
                return true;
            }
            catch (Exception ex)
            {
                _logger.Warning($"[LootboxDataCollector] 检查箱子可见性失败: {ex.Message}");
                // 出错时保守处理，认为可见
                return true;
            }
        }

        /// <summary>
        /// 评估条件列表（所有条件都必须满足）
        /// </summary>
        private bool EvaluateConditions(System.Collections.IList conditions)
        {
            if (conditions == null || conditions.Count == 0)
                return true;

            foreach (var condition in conditions)
            {
                if (condition == null)
                    continue;

                // 获取 Condition 类型的 Evaluate 方法
                var evaluateMethod = condition.GetType().GetMethod("Evaluate", 
                    BindingFlags.Public | BindingFlags.Instance);
                
                if (evaluateMethod != null)
                {
                    var result = evaluateMethod.Invoke(condition, null);
                    if (result is bool evaluated && !evaluated)
                    {
                        // 任何一个条件不满足，整体就不满足
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// 分析箱子内容（物品质量、特殊标签等）
        /// </summary>
        private void AnalyzeLootboxContents(LootboxData data)
        {
            try
            {
                var maxQuality = ItemValueLevel.White;
                int wishlistedCount = 0;
                bool hasQuestItems = false;

                foreach (var item in data.Items)
                {
                    if (item == null)
                        continue;

                    // 检测物品品质（使用专业的品质计算工具）
                    var itemQuality = DuckovESPv3.Utils.ItemQualityUtil.GetItemValueLevel(item);
                    
                    if ((int)itemQuality > (int)maxQuality)
                    {
                        maxQuality = itemQuality;
                    }
                    
                    // 检查 WishList 信息
                    var wishlistInfo = ItemWishlist.GetWishlistInfo(item.TypeID);
                    
                    if (wishlistInfo.isManuallyWishlisted)
                    {
                        data.HasWishlistedItems = true;
                        wishlistedCount++;
                    }
                    
                    if (wishlistInfo.isQuestRequired)
                    {
                        data.HasQuestRequiredItems = true;
                        wishlistedCount++;
                    }
                    
                    if (wishlistInfo.isBuildingRequired)
                    {
                        data.HasBuildingRequiredItems = true;
                        wishlistedCount++;
                    }
                    
                    // 检查是否在我们的Quest物品列表中
                    if (_questItemIds.Contains(item.TypeID))
                    {
                        hasQuestItems = true;
                    }
                }

                data.MaxItemQuality = maxQuality;
                data.WishlistedItemCount = wishlistedCount;
                data.HasQuestItems = hasQuestItems;
            }
            catch (Exception ex)
            {
                _logger.Warning($"[LootboxDataCollector] 分析箱子内容失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 获取所有追踪中的战利品箱数据
        /// </summary>
        public IReadOnlyDictionary<InteractableLootbox, Models.LootboxData> GetTrackedLootboxes()
        {
            return _trackedLootboxes;
        }

        /// <summary>
        /// 根据InteractableLootbox获取特定战利品箱数据
        /// </summary>
        public Models.LootboxData GetLootboxData(InteractableLootbox lootbox)
        {
            if (lootbox == null)
                return new LootboxData();

            _trackedLootboxes.TryGetValue(lootbox, out var data);
            return data ?? new LootboxData();
        }

        /// <summary>
        /// 获取追踪中的战利品箱数量
        /// </summary>
        public int GetTrackedLootboxCount()
        {
            return _trackedLootboxes.Count;
        }

        /// <summary>
        /// 获取所有追踪的战利品箱数据
        /// </summary>
        public IEnumerable<LootboxData> GetAllTrackedLootboxes()
        {
            return _trackedLootboxes.Values;
        }

        /// <summary>
        /// 处理新箱子生成事件（Harmony Hook 触发）
        /// </summary>
        private void HandleLootboxSpawned(InteractableLootbox lootbox)
        {
            try
            {
                if (lootbox == null)
                {
                    _logger.Warning("[LootboxDataCollector] 收到空箱子生成事件");
                    return;
                }

                // 检查是否已激活（过滤未激活的箱子）
                if (!lootbox.gameObject.activeInHierarchy)
                {
                    _logger.Debug($"[LootboxDataCollector] 跳过未激活的新生成箱子: {lootbox.name}");
                    return;
                }

                // 检查箱子组件是否启用
                if (!lootbox.enabled)
                {
                    _logger.Debug($"[LootboxDataCollector] 跳过未启用的新生成箱子组件: {lootbox.name}");
                    return;
                }

                // 检查是否已追踪
                if (_discoveredBoxes.Contains(lootbox))
                {
                    _logger.Debug($"[LootboxDataCollector] 箱子已存在，跳过: {lootbox.name}");
                    return;
                }

                // 获取库存（使用本地 Inventory，而不是全局字典）
                var inventory = lootbox.Inventory;
                if (inventory == null)
                {
                    _logger.Warning($"[LootboxDataCollector] 新箱子的 Inventory 为 null: {lootbox.name}");
                    return;
                }

                // 立即处理（CreateFromItem 是同步的，物品应该已经添加完成）
                ProcessLootbox(lootbox, inventory);
                
                _logger.Info($"[LootboxDataCollector] ✨ 实时检测到新箱子: {lootbox.name} (物品数: {inventory.Content.Count})");
            }
            catch (Exception ex)
            {
                _logger.Error($"[LootboxDataCollector] 处理新箱子生成事件失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 处理箱子可见性变化事件（0轮询）
        /// 当 SetActiveByCondition 或 LootBoxLoader 改变箱子激活状态时触发
        /// </summary>
        private void HandleLootboxVisibilityChanged(InteractableLootbox lootbox, bool isVisible)
        {
            try
            {
                if (lootbox == null)
                    return;

                if (isVisible)
                {
                    // 箱子变为可见 - 尝试追踪
                    if (!_discoveredBoxes.Contains(lootbox))
                    {
                        _logger.Info($"[LootboxDataCollector] 箱子动态变为可见，开始追踪: {lootbox.name}");
                        
                        var inventory = lootbox.Inventory;
                        if (inventory != null)
                        {
                            ProcessLootbox(lootbox, inventory);
                        }
                        else
                        {
                            _logger.Warning($"[LootboxDataCollector] 可见箱子的 Inventory 为 null: {lootbox.name}");
                        }
                    }
                }
                else
                {
                    // 箱子变为不可见 - 移除追踪
                    if (_trackedLootboxes.ContainsKey(lootbox))
                    {
                        _logger.Info($"[LootboxDataCollector] 箱子动态变为不可见，移除追踪: {lootbox.name}");
                        
                        var lootboxData = _trackedLootboxes[lootbox];
                        _trackedLootboxes.Remove(lootbox);
                        _discoveredBoxes.Remove(lootbox);
                        
                        // 发布移除事件
                        _eventBus.Publish(new Events.LootboxRemovedEvent(lootboxData));
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"[LootboxDataCollector] 处理箱子可见性变化失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 处理小地图标记刷新请求
        /// 重新发布所有已追踪箱子的发现事件
        /// </summary>
        private void OnMinimapRefreshRequested(Events.MinimapMarkersRefreshRequestedEvent evt)
        {
            try
            {
                _logger.Info($"[LootboxDataCollector] 收到小地图刷新请求，重新发布 {_trackedLootboxes.Count} 个箱子数据");

                int republishedCount = 0;
                foreach (var lootboxData in _trackedLootboxes.Values)
                {
                    if (lootboxData != null && lootboxData.Lootbox != null)
                    {
                        // 重新发布箱子发现事件
                        _eventBus.Publish(new Events.LootboxDiscoveredEvent(lootboxData));
                        republishedCount++;
                    }
                }

                _logger.Info($"[LootboxDataCollector] ✓ 已重新发布 {republishedCount} 个箱子的发现事件");
            }
            catch (Exception ex)
            {
                _logger.Error($"[LootboxDataCollector] 处理小地图刷新请求失败: {ex.Message}");
            }
        }
        
        /// <summary>
        /// 处理Quest物品更新事件
        /// </summary>
        private void OnQuestItemsUpdated(QuestItemsUpdatedEvent evt)
        {
            try
            {
                // 更新Quest物品ID集合
                _questItemIds.Clear();
                foreach (var questItem in evt.QuestItems)
                {
                    _questItemIds.Add(questItem.ItemId);
                }
                
                _logger.Debug($"[LootboxDataCollector] Quest物品列表已更新，当前 {_questItemIds.Count} 个任务物品");
                
                // 重新分析所有箱子（检查是否包含任务物品）
                int updatedCount = 0;
                foreach (var lootboxData in _trackedLootboxes.Values)
                {
                    if (lootboxData != null)
                    {
                        bool hadQuestItems = lootboxData.HasQuestItems;
                        AnalyzeLootboxContents(lootboxData);
                        
                        // 如果Quest状态改变，发布内容变化事件
                        if (hadQuestItems != lootboxData.HasQuestItems)
                        {
                            _eventBus.Publish(new Events.LootboxContentChangedEvent(lootboxData, Events.ChangeReason.ItemAdded));
                            updatedCount++;
                        }
                    }
                }
                
                if (updatedCount > 0)
                {
                    _logger.Info($"[LootboxDataCollector] ✓ 已更新 {updatedCount} 个箱子的Quest物品状态");
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"[LootboxDataCollector] 处理Quest物品更新失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 清理资源
        /// </summary>
        public void Cleanup()
        {
            try
            {
                _logger.Info("[LootboxDataCollector] 正在清理...");

                // 取消订阅 Harmony Hook 事件
                LootboxSpawnHook.OnLootboxSpawned -= HandleLootboxSpawned;
                Hooks.LootboxVisibilityHook.OnLootboxVisibilityChanged -= HandleLootboxVisibilityChanged;

                // 取消订阅小地图刷新事件
                _eventBus.Unsubscribe<Events.MinimapMarkersRefreshRequestedEvent>(OnMinimapRefreshRequested);
                
                // 取消订阅Quest事件
                _eventBus.Unsubscribe<QuestItemsUpdatedEvent>(OnQuestItemsUpdated);

                _trackedLootboxes.Clear();
                _discoveredBoxes.Clear();
                _questItemIds.Clear();
                _isInitialized = false;

                _logger.Info("[LootboxDataCollector] ✓ 清理完成");
            }
            catch (Exception ex)
            {
                _logger.Error($"[LootboxDataCollector] 清理失败: {ex.Message}");
            }
        }
    }
}
