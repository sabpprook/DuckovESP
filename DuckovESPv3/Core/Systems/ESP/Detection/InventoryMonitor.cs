using System;
using System.Collections.Generic;
using UnityEngine;
using ItemStatsSystem;
using DuckovESPv3.Infrastructure.Logging;

namespace DuckovESPv3.Core.Systems.ESP.Detection
{
    /// <summary>
    /// Inventory 监听器
    /// 实时监听箱子和地面物品的拾取事件，触发 ESP 更新
    /// </summary>
    public class InventoryMonitor : IDisposable
    {
        // ===== 事件定义 =====
        /// <summary>
        /// 当物品从箱子中被拿走时触发
        /// 参数：(箱子Inventory, 物品, 位置索引)
        /// </summary>
        public event Action<Inventory, Item, int>? OnItemRemovedFromLootbox;

        /// <summary>
        /// 当地面物品被捡起时触发
        /// 参数：(被捡起的物品)
        /// </summary>
        public event Action<Item>? OnWorldItemPickedUp;

        // ===== 依赖 =====
        private readonly DuckovESPv3.Infrastructure.Logging.ILogger _logger;

        // ===== 追踪数据 =====
        private Dictionary<Inventory, HashSet<Item>> _trackedLootboxes = new Dictionary<Inventory, HashSet<Item>>();
        private HashSet<Item> _trackedWorldItems = new HashSet<Item>();

        // ===== 生命周期标记 =====
        private bool _isDisposed = false;

        public InventoryMonitor(DuckovESPv3.Infrastructure.Logging.ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _logger.Info("[InventoryMonitor] Inventory 监听器已创建");
        }

        /// <summary>
        /// 开始监听箱子的 Inventory
        /// </summary>
        public void StartMonitoringLootbox(InteractableLootbox lootbox, Inventory inventory)
        {
            if (lootbox == null || inventory == null) return;
            
            try
            {
                // 如果已经在监听，先取消订阅
                if (_trackedLootboxes.ContainsKey(inventory))
                {
                    StopMonitoringLootbox(inventory);
                }

                // 记录初始物品快照
                var itemSnapshot = new HashSet<Item>();
                foreach (var item in inventory.Content)
                {
                    if (item != null)
                    {
                        itemSnapshot.Add(item);
                    }
                }
                _trackedLootboxes[inventory] = itemSnapshot;

                // 订阅 Inventory 的内容变化事件
                inventory.onContentChanged += OnLootboxContentChanged;

                _logger.Debug($"[InventoryMonitor] 开始监听箱子 Inventory，初始物品数: {itemSnapshot.Count}");
            }
            catch (Exception ex)
            {
                _logger.Error($"[InventoryMonitor] 开始监听箱子失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 停止监听箱子的 Inventory
        /// </summary>
        public void StopMonitoringLootbox(Inventory inventory)
        {
            if (inventory == null) return;

            try
            {
                if (_trackedLootboxes.ContainsKey(inventory))
                {
                    inventory.onContentChanged -= OnLootboxContentChanged;
                    _trackedLootboxes.Remove(inventory);
                    _logger.Debug("[InventoryMonitor] 停止监听箱子 Inventory");
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"[InventoryMonitor] 停止监听箱子失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 开始监听地面物品（通过 Item.InInventory 变化检测）
        /// </summary>
        public void StartMonitoringWorldItem(Item worldItem)
        {
            if (worldItem == null) return;

            try
            {
                if (!_trackedWorldItems.Contains(worldItem))
                {
                    _trackedWorldItems.Add(worldItem);
                    
                    // 监听物品的父对象变化（当物品被捡起，会被添加到玩家Inventory）
                    worldItem.onParentChanged += OnWorldItemParentChanged;
                    
                    _logger.Debug($"[InventoryMonitor] 开始监听地面物品: {worldItem.DisplayName}");
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"[InventoryMonitor] 开始监听地面物品失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 停止监听地面物品
        /// </summary>
        public void StopMonitoringWorldItem(Item worldItem)
        {
            if (worldItem == null) return;

            try
            {
                if (_trackedWorldItems.Contains(worldItem))
                {
                    worldItem.onParentChanged -= OnWorldItemParentChanged;
                    _trackedWorldItems.Remove(worldItem);
                    _logger.Debug($"[InventoryMonitor] 停止监听地面物品: {worldItem.DisplayName}");
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"[InventoryMonitor] 停止监听地面物品失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 箱子内容变化处理
        /// </summary>
        private void OnLootboxContentChanged(Inventory inventory, int position)
        {
            if (!_trackedLootboxes.ContainsKey(inventory)) return;

            try
            {
                var previousItems = _trackedLootboxes[inventory];
                var currentItems = new HashSet<Item>();

                // 收集当前所有物品
                foreach (var item in inventory.Content)
                {
                    if (item != null)
                    {
                        currentItems.Add(item);
                    }
                }

                // 找出被移除的物品
                var removedItems = new HashSet<Item>(previousItems);
                removedItems.ExceptWith(currentItems);

                // 触发移除事件
                foreach (var removedItem in removedItems)
                {
                    _logger.Debug($"[InventoryMonitor] 物品从箱子移除: {removedItem.DisplayName} (位置: {position})");
                    OnItemRemovedFromLootbox?.Invoke(inventory, removedItem, position);
                }

                // 更新快照
                _trackedLootboxes[inventory] = currentItems;
            }
            catch (Exception ex)
            {
                _logger.Error($"[InventoryMonitor] 处理箱子内容变化失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 地面物品父对象变化处理（检测是否被捡起）
        /// </summary>
        private void OnWorldItemParentChanged(Item item)
        {
            if (!_trackedWorldItems.Contains(item)) return;

            try
            {
                // 检查物品是否被添加到 Inventory（被捡起）
                if (item.InInventory != null)
                {
                    // 进一步检查是否在玩家 Inventory 中（排除箱子间转移）
                    if (IsPlayerInventory(item.InInventory))
                    {
                        _logger.Info($"[InventoryMonitor] 地面物品被玩家捡起: {item.DisplayName}");
                        OnWorldItemPickedUp?.Invoke(item);
                        
                        // 停止监听已捡起的物品
                        StopMonitoringWorldItem(item);
                    }
                }
                else if (item.gameObject == null || !item.gameObject.activeInHierarchy)
                {
                    // 物品被销毁或禁用
                    _logger.Debug($"[InventoryMonitor] 地面物品被销毁: {item.DisplayName}");
                    OnWorldItemPickedUp?.Invoke(item);
                    StopMonitoringWorldItem(item);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"[InventoryMonitor] 处理地面物品变化失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 检查 Inventory 是否属于玩家
        /// </summary>
        private bool IsPlayerInventory(Inventory inventory)
        {
            if (inventory == null) return false;

            try
            {
                // 方法1：检查是否在 LootBoxInventoriesParent 下（箱子都在这里）
                var lootBoxParent = LevelManager.LootBoxInventoriesParent;
                if (lootBoxParent != null && inventory.transform.IsChildOf(lootBoxParent))
                {
                    return false; // 是箱子，不是玩家
                }

                // 方法2：检查是否属于玩家角色
                var character = inventory.GetComponentInParent<CharacterMainControl>();
                if (character != null && character == CharacterMainControl.Main)
                {
                    return true; // 是玩家
                }

                // 方法3：检查是否是 PlayerStorage
                var playerStorage = inventory.GetComponentInParent<PlayerStorage>();
                if (playerStorage != null && playerStorage == PlayerStorage.Instance)
                {
                    return true; // 是玩家仓库
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 清理所有监听
        /// </summary>
        public void ClearAll()
        {
            try
            {
                // 取消所有箱子监听
                foreach (var kvp in _trackedLootboxes)
                {
                    if (kvp.Key != null)
                    {
                        kvp.Key.onContentChanged -= OnLootboxContentChanged;
                    }
                }
                _trackedLootboxes.Clear();

                // 取消所有地面物品监听
                foreach (var item in _trackedWorldItems)
                {
                    if (item != null)
                    {
                        item.onParentChanged -= OnWorldItemParentChanged;
                    }
                }
                _trackedWorldItems.Clear();

                _logger.Info("[InventoryMonitor] 所有监听已清理");
            }
            catch (Exception ex)
            {
                _logger.Error($"[InventoryMonitor] 清理监听失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (_isDisposed) return;

            ClearAll();
            _isDisposed = true;
            _logger.Info("[InventoryMonitor] Inventory 监听器已释放");
        }
    }
}
