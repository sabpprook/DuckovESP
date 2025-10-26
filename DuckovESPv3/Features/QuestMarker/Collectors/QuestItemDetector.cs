using DuckovESPv3.Core.EventBus;
using DuckovESPv3.Features.QuestMarker.Data;
using DuckovESPv3.Features.QuestMarker.Events;
using DuckovESPv3.Infrastructure.Logging;
using Duckov.Quests;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckovESPv3.Features.QuestMarker.Collectors
{
    /// <summary>
    /// 任务物品检测器（完全事件驱动，无周期扫描，零反射）
    /// 监听任务状态变化事件，维护当前需要收集的任务物品列表
    /// </summary>
    public class QuestItemDetector : IDisposable
    {
        private readonly IEventBus _eventBus;
        private readonly Infrastructure.Logging.ILogger _logger;
        private readonly List<QuestItemData> _questItems = new List<QuestItemData>();
        private bool _isInitialized = false;

        public QuestItemDetector(IEventBus eventBus, Infrastructure.Logging.ILogger logger)
        {
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// 初始化检测器（订阅事件）
        /// </summary>
        public void Initialize()
        {
            if (_isInitialized)
            {
                _logger.Warning("[QuestItemDetector] Already initialized");
                return;
            }

            try
            {
                // 订阅任务状态变化事件（零反射，直接使用游戏API）
                Quest.onQuestStatusChanged += OnQuestStatusChanged;
                Quest.onQuestActivated += OnQuestActivated;
                Quest.onQuestCompleted += OnQuestCompleted;
                
                // 订阅心愿单变化事件（修复Issue #2: 心愿单实时更新）
                ItemWishlist.OnWishlistChanged += OnWishlistChanged;

                // 初始扫描当前激活的任务
                RefreshQuestItems();

                _isInitialized = true;
                _logger.Info($"[QuestItemDetector] Initialized, found {_questItems.Count} quest items");
            }
            catch (Exception ex)
            {
                _logger.Error($"[QuestItemDetector] Initialization failed: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 事件处理：任务状态变化
        /// </summary>
        private void OnQuestStatusChanged(Quest quest)
        {
            _logger.Debug("[QuestItemDetector] Quest status changed, refreshing items");
            RefreshQuestItems();
        }

        /// <summary>
        /// 事件处理：任务激活
        /// </summary>
        private void OnQuestActivated(Quest quest)
        {
            _logger.Debug("[QuestItemDetector] Quest activated, refreshing items");
            RefreshQuestItems();
        }

        /// <summary>
        /// 事件处理：任务完成
        /// </summary>
        private void OnQuestCompleted(Quest quest)
        {
            _logger.Debug("[QuestItemDetector] Quest completed, refreshing items");
            RefreshQuestItems();
        }

        /// <summary>
        /// 事件处理：心愿单变化（修复Issue #2: 实时更新）
        /// </summary>
        private void OnWishlistChanged(int itemTypeID)
        {
            _logger.Debug($"[QuestItemDetector] Wishlist changed for item {itemTypeID}, refreshing items");
            RefreshQuestItems();
        }

        /// <summary>
        /// 刷新任务物品列表（调用游戏API，零反射）
        /// </summary>
        private void RefreshQuestItems()
        {
            try
            {
                _questItems.Clear();

                // 调用游戏API获取所有需要的物品（零反射）
                var requiredItemIds = QuestManager.GetAllRequiredItems();
                
                if (requiredItemIds == null)
                {
                    _logger.Debug("[QuestItemDetector] No required items returned");
                    PublishUpdateEvent();
                    return;
                }

                // 将ItemTypeID转换为QuestItemData
                foreach (var itemId in requiredItemIds)
                {
                    var questItem = new QuestItemData
                    {
                        ItemId = itemId, // 直接使用int类型
                        DisplayName = $"Item_{itemId}", // 可以后续通过ItemMetaData查询真实名称
                        RequiredCount = 1, // API未提供数量信息，默认1
                        ItemType = "Unknown",
                        IsBuildingMaterial = false, // 需要进一步判断
                        QuestId = 0, // API未提供Quest信息
                        QuestDescription = string.Empty
                    };

                    _questItems.Add(questItem);
                }

                _logger.Debug($"[QuestItemDetector] Refreshed {_questItems.Count} quest items");
                PublishUpdateEvent();
            }
            catch (Exception ex)
            {
                _logger.Error($"[QuestItemDetector] Failed to refresh quest items: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 发布更新事件
        /// </summary>
        private void PublishUpdateEvent()
        {
            var eventData = new QuestItemsUpdatedEvent(_questItems.AsReadOnly());
            _eventBus.Publish(eventData);
        }

        /// <summary>
        /// 获取当前任务物品列表（只读）
        /// </summary>
        public IReadOnlyList<QuestItemData> GetQuestItems()
        {
            return _questItems.AsReadOnly();
        }

        public void Dispose()
        {
            // 取消订阅事件
            Quest.onQuestStatusChanged -= OnQuestStatusChanged;
            Quest.onQuestActivated -= OnQuestActivated;
            Quest.onQuestCompleted -= OnQuestCompleted;
            ItemWishlist.OnWishlistChanged -= OnWishlistChanged;

            _questItems.Clear();
            _isInitialized = false;
            _logger.Info("[QuestItemDetector] Disposed");
        }
    }
}
