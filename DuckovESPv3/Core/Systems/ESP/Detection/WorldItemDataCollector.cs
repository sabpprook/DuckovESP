using System;
using System.Collections.Generic;
using UnityEngine;
using DuckovESPv3.Core.EventBus;
using DuckovESPv3.Infrastructure.Logging;
using DuckovESPv3.Core.Systems.ESP.Models;
using DuckovESPv3.Core.Systems.ESP.Events;
using DuckovESPv3.Features.QuestMarker.Events;
using ItemStatsSystem;

namespace DuckovESPv3.Core.Systems.ESP.Detection
{
    /// <summary>
    /// 世界物品数据收集器
    /// 职责：实时跟踪所有地面物品，通过Item.onDestroy事件驱动
    /// 性能：事件驱动，O(1)每次发现或销毁，无轮询开销
    /// </summary>
    public class WorldItemDataCollector : IWorldItemCollectionService
    {
        private readonly IEventBus _eventBus;
        private readonly Infrastructure.Logging.ILogger _logger;

        private bool _isInitialized = false;
        private readonly Dictionary<Item, WorldItemData> _trackedItems = new Dictionary<Item, WorldItemData>();
        private readonly Dictionary<Item, Action<Item>> _destroyCallbacks = new Dictionary<Item, Action<Item>>();
        
        // Quest物品检测
        private HashSet<int> _questItemIds = new HashSet<int>();

        public bool IsInitialized => _isInitialized;

        public WorldItemDataCollector(IEventBus eventBus, Infrastructure.Logging.ILogger logger)
        {
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// 初始化数据收集器，扫描现有物品
        /// </summary>
        public void Initialize()
        {
            if (_isInitialized)
                return;

            try
            {
                _logger.Info("[WorldItemDataCollector] 正在初始化...");

                // 订阅动态生成事件（0轮询）
                ItemAgentSpawnHook.OnItemAgentInitialized += OnItemAgentSpawned;
                _logger.Info("[WorldItemDataCollector] 已订阅ItemAgentSpawnHook.OnItemAgentInitialized事件");
                
                // 订阅Quest物品更新事件
                _eventBus.Subscribe<QuestItemsUpdatedEvent>(OnQuestItemsUpdated);
                _logger.Info("[WorldItemDataCollector] 已订阅QuestItemsUpdatedEvent事件");

                // 扫描现有物品
                DiscoverExistingItems();

                _isInitialized = true;
                _logger.Info($"[WorldItemDataCollector] 初始化完成，发现 {_trackedItems.Count} 个物品");
            }
            catch (Exception ex)
            {
                _logger.Error($"[WorldItemDataCollector] 初始化失败: {ex.Message}\n{ex.StackTrace}");
            }
        }
        
        /// <summary>
        /// 重新扫描（用于子场景加载，不清理现有数据）
        /// </summary>
        public void Rescan()
        {
            try
            {
                _logger.Info("[WorldItemDataCollector] 开始重新扫描物品（追加模式）...");
                
                int beforeCount = _trackedItems.Count;
                DiscoverExistingItems();
                int afterCount = _trackedItems.Count;
                
                _logger.Info($"[WorldItemDataCollector] ✓ 重新扫描完成，新增 {afterCount - beforeCount} 个物品，总计 {afterCount} 个");
            }
            catch (Exception ex)
            {
                _logger.Error($"[WorldItemDataCollector] 重新扫描失败: {ex.Message}\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// 扫描并追踪现有的世界物品
        /// </summary>
        private void DiscoverExistingItems()
        {
            try
            {
                var agents = UnityEngine.Object.FindObjectsOfType<DuckovItemAgent>();
                int discovered = 0;
                int skipped = 0;

                foreach (var agent in agents)
                {
                    try
                    {
                        // 检查GameObject是否激活
                        if (agent == null || !agent.gameObject.activeInHierarchy)
                        {
                            skipped++;
                            continue;
                        }

                        // 检查组件是否启用
                        if (!agent.enabled)
                        {
                            skipped++;
                            continue;
                        }

                        // 只追踪地面物品（pickUp类型）
                        if (agent.AgentType == ItemAgent.AgentTypes.pickUp)
                        {
                            if (agent.Item != null)
                            {
                                TrackItem(agent.Item, agent);
                                discovered++;
                            }
                        }
                        else
                        {
                            skipped++;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Warning($"[WorldItemDataCollector] 处理Agent失败: {ex.Message}");
                    }
                }

                _logger.Debug($"[WorldItemDataCollector] 扫描完成: 发现 {discovered} 个物品, 跳过 {skipped} 个非地面物品");
            }
            catch (Exception ex)
            {
                _logger.Error($"[WorldItemDataCollector] 扫描物品失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 处理动态生成的ItemAgent（通过Hook事件触发）
        /// </summary>
        private void OnItemAgentSpawned(ItemAgent agent)
        {
            try
            {
                if (agent == null || agent.Item == null)
                    return;

                // 转换为DuckovItemAgent（如果可以）
                var duckovAgent = agent as DuckovItemAgent ?? agent.GetComponent<DuckovItemAgent>();
                if (duckovAgent == null)
                {
                    _logger.Warning($"[WorldItemDataCollector] ItemAgent不是DuckovItemAgent类型: {agent.name}");
                    return;
                }

                _logger.Info($"[WorldItemDataCollector] 🎯 检测到动态生成的物品: {agent.Item.DisplayName} at {agent.transform.position}");
                TrackItem(agent.Item, duckovAgent);
            }
            catch (Exception ex)
            {
                _logger.Error($"[WorldItemDataCollector] 处理动态生成物品失败: {ex.Message}\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// 追踪一个物品，并订阅其销毁事件
        /// </summary>
        private void TrackItem(Item item, DuckovItemAgent agent)
        {
            try
            {
                if (item == null || agent == null)
                    return;

                // 检查GameObject是否激活
                if (!agent.gameObject.activeInHierarchy)
                {
                    _logger.Debug($"[WorldItemDataCollector] 跳过未激活的物品: {item.DisplayName}");
                    return;
                }

                // 检查组件是否启用
                if (!agent.enabled)
                {
                    _logger.Debug($"[WorldItemDataCollector] 跳过未启用的物品组件: {item.DisplayName}");
                    return;
                }

                // 如果已追踪，跳过
                if (_trackedItems.ContainsKey(item))
                    return;

                // 创建数据模型
                var itemData = CreateWorldItemData(item, agent);
                if (itemData == null)
                    return;

                // 添加到追踪集合
                _trackedItems.Add(item, itemData);

                // 订阅销毁事件
                SubscribeItemDestroy(item);

                // 发布发现事件
                var evt = new WorldItemDiscoveredEvent(itemData);
                _eventBus.Publish(evt);

                _logger.Debug($"[WorldItemDataCollector] 追踪物品: {itemData.DisplayName} at {itemData.Position}");
            }
            catch (Exception ex)
            {
                _logger.Error($"[WorldItemDataCollector] 追踪物品失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 创建WorldItemData数据模型
        /// </summary>
        private WorldItemData CreateWorldItemData(Item item, DuckovItemAgent agent)
        {
            try
            {
                // 获取 WishList 信息
                var wishlistInfo = ItemWishlist.GetWishlistInfo(item.TypeID);
                
                // 检查是否在我们的Quest物品列表中（优先级更高）
                bool isQuestItem = _questItemIds.Contains(item.TypeID) || wishlistInfo.isQuestRequired;
                
                var itemData = new WorldItemData
                {
                    Item = item,
                    Agent = agent,
                    Position = agent.transform.position,
                    Quality = DuckovESPv3.Utils.ItemQualityUtil.GetItemValueLevel(item),
                    StackCount = item.StackCount,
                    TypeID = item.TypeID,
                    DisplayName = item.DisplayName,
                    DiscoveryTime = DateTime.UtcNow,
                    
                    // WishList 信息（合并我们的Quest检测）
                    IsManuallyWishlisted = wishlistInfo.isManuallyWishlisted,
                    IsQuestRequired = isQuestItem, // 使用合并后的结果
                    IsBuildingRequired = wishlistInfo.isBuildingRequired
                };

                return itemData;
            }
            catch (Exception ex)
            {
                _logger.Error($"[WorldItemDataCollector] 创建WorldItemData失败: {ex.Message}");
                return new WorldItemData();
            }
        }

        /// <summary>
        /// 订阅物品销毁事件
        /// </summary>
        private void SubscribeItemDestroy(Item item)
        {
            try
            {
                if (item == null)
                    return;

                // 创建回调
                Action<Item> callback = OnItemDestroyed;
                _destroyCallbacks[item] = callback;
                
                // 订阅事件
                item.onDestroy += callback;
            }
            catch (Exception ex)
            {
                _logger.Warning($"[WorldItemDataCollector] 订阅物品销毁事件失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 物品销毁时的回调
        /// </summary>
        private void OnItemDestroyed(Item item)
        {
            try
            {
                if (!_trackedItems.ContainsKey(item))
                    return;

                var itemData = _trackedItems[item];
                _trackedItems.Remove(item);

                // 移除回调记录
                if (_destroyCallbacks.ContainsKey(item))
                {
                    _destroyCallbacks.Remove(item);
                }

                // 发布移除事件
                var evt = new WorldItemRemovedEvent(itemData);
                _eventBus.Publish(evt);

                _logger.Debug($"[WorldItemDataCollector] 物品已销毁: {itemData.DisplayName}");
            }
            catch (Exception ex)
            {
                _logger.Error($"[WorldItemDataCollector] 处理物品销毁失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 获取所有追踪中的世界物品
        /// </summary>
        public IReadOnlyDictionary<Item, WorldItemData> GetTrackedWorldItems()
        {
            return _trackedItems;
        }

        /// <summary>
        /// 根据Item获取数据
        /// </summary>
        public WorldItemData GetWorldItemData(Item item)
        {
            if (item == null)
                return new WorldItemData();

            _trackedItems.TryGetValue(item, out var data);
            return data ?? new WorldItemData();
        }

        /// <summary>
        /// 获取追踪中的物品数量
        /// </summary>
        public int GetTrackedItemCount()
        {
            return _trackedItems.Count;
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
                
                _logger.Debug($"[WorldItemDataCollector] Quest物品列表已更新，当前 {_questItemIds.Count} 个任务物品");
                
                // 重新检查所有物品的Quest状态
                int updatedCount = 0;
                foreach (var kvp in _trackedItems)
                {
                    var itemData = kvp.Value;
                    if (itemData != null && itemData.Item != null)
                    {
                        bool wasQuestItem = itemData.IsQuestRequired;
                        bool isQuestItem = _questItemIds.Contains(itemData.TypeID);
                        
                        // 如果Quest状态改变，更新并发布事件
                        if (wasQuestItem != isQuestItem)
                        {
                            itemData.IsQuestRequired = isQuestItem;
                            _eventBus.Publish(new WorldItemDiscoveredEvent(itemData)); // 重新发布
                            updatedCount++;
                        }
                    }
                }
                
                if (updatedCount > 0)
                {
                    _logger.Info($"[WorldItemDataCollector] ✓ 已更新 {updatedCount} 个物品的Quest状态");
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"[WorldItemDataCollector] 处理Quest物品更新失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 清理资源
        /// </summary>
        public void Cleanup()
        {
            try
            {
                _logger.Info("[WorldItemDataCollector] 正在清理...");

                // 取消订阅动态生成事件
                ItemAgentSpawnHook.OnItemAgentInitialized -= OnItemAgentSpawned;
                
                // 取消订阅Quest事件
                _eventBus.Unsubscribe<QuestItemsUpdatedEvent>(OnQuestItemsUpdated);

                // 取消所有物品的销毁事件订阅
                foreach (var item in _trackedItems.Keys)
                {
                    try
                    {
                        if (item != null && _destroyCallbacks.TryGetValue(item, out var callback))
                        {
                            item.onDestroy -= callback;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Warning($"[WorldItemDataCollector] 取消事件订阅失败: {ex.Message}");
                    }
                }

                _trackedItems.Clear();
                _destroyCallbacks.Clear();
                _questItemIds.Clear();
                _isInitialized = false;

                _logger.Info("[WorldItemDataCollector] 清理完成");
            }
            catch (Exception ex)
            {
                _logger.Error($"[WorldItemDataCollector] 清理失败: {ex.Message}");
            }
        }
    }
}
