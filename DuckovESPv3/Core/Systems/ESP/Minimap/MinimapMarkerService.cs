using System;
using System.Collections.Generic;
using DuckovESPv3.Core.Configuration;
using DuckovESPv3.Core.EventBus;
using DuckovESPv3.Core.Systems.ESP.Events;
using DuckovESPv3.Core.Systems.ESP.Minimap.Models;
using DuckovESPv3.Core.Systems.ESP.Models;
using DuckovESPv3.Features.QuestMarker.Events;
using DuckovESPv3.Features.QuestMarker.Data;
using ItemStatsSystem;
using UnityEngine;

namespace DuckovESPv3.Core.Systems.ESP.Minimap
{
    /// <summary>
    /// 小地图标记服务 - 负责标记的生命周期管理
    /// </summary>
    public class MinimapMarkerService
    {
        private readonly ESPSystemConfig _config;
        private readonly IEventBus _eventBus;
        private readonly MinimapMarkerFactory _factory;

        // 标记存储：键为源对象（ItemAgent或Lootbox），值为标记数据
        private readonly Dictionary<object, MinimapMarkerData> _markers;
        
        // Quest区域标记存储：键为Quest Key
        private readonly Dictionary<string, MinimapMarkerData> _questZoneMarkers;

        /// <summary>
        /// 当前活跃的标记数量
        /// </summary>
        public int ActiveMarkerCount => _markers.Count + _questZoneMarkers.Count;

        public MinimapMarkerService(ESPSystemConfig config, IEventBus eventBus)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _factory = new MinimapMarkerFactory();
            _markers = new Dictionary<object, MinimapMarkerData>();
            _questZoneMarkers = new Dictionary<string, MinimapMarkerData>();
        }

        /// <summary>
        /// 开始监听事件
        /// </summary>
        public void StartMonitoring()
        {
            _eventBus.Subscribe<WorldItemDiscoveredEvent>(OnWorldItemDiscovered);
            _eventBus.Subscribe<WorldItemRemovedEvent>(OnWorldItemRemoved);
            _eventBus.Subscribe<LootboxDiscoveredEvent>(OnLootboxDiscovered);
            _eventBus.Subscribe<LootboxRemovedEvent>(OnLootboxRemoved);
            _eventBus.Subscribe<QuestZonesUpdatedEvent>(OnQuestZonesUpdated);

            Debug.Log("[MinimapMarkerService] Started monitoring ESP events");
        }

        /// <summary>
        /// 停止监听事件
        /// </summary>
        public void StopMonitoring()
        {
            _eventBus.Unsubscribe<WorldItemDiscoveredEvent>(OnWorldItemDiscovered);
            _eventBus.Unsubscribe<WorldItemRemovedEvent>(OnWorldItemRemoved);
            _eventBus.Unsubscribe<LootboxDiscoveredEvent>(OnLootboxDiscovered);
            _eventBus.Unsubscribe<LootboxRemovedEvent>(OnLootboxRemoved);
            _eventBus.Unsubscribe<QuestZonesUpdatedEvent>(OnQuestZonesUpdated);

            Debug.Log("[MinimapMarkerService] Stopped monitoring ESP events");
        }

        /// <summary>
        /// 清理所有标记
        /// </summary>
        public void Cleanup()
        {
            Debug.Log($"[MinimapMarkerService] Cleaning up {_markers.Count} item/lootbox markers and {_questZoneMarkers.Count} quest zone markers");

            foreach (var marker in _markers.Values)
            {
                marker.Destroy();
            }

            _markers.Clear();
            
            foreach (var marker in _questZoneMarkers.Values)
            {
                marker.Destroy();
            }
            
            _questZoneMarkers.Clear();
        }

        /// <summary>
        /// 处理世界物品发现事件
        /// </summary>
        private void OnWorldItemDiscovered(WorldItemDiscoveredEvent evt)
        {
            if (!_config.EnableMapMarkers || evt?.Data == null)
            {
                return;
            }

            var itemData = evt.Data;

            // 检查品质过滤
            if (!PassesQualityFilter(itemData.Quality))
            {
                return;
            }

            // 检查是否已存在标记
            if (_markers.ContainsKey(itemData.Agent))
            {
                return;
            }

            // 创建标记
            var marker = _factory.CreateMarkerForItem(itemData);
            if (marker != null)
            {
                _markers[itemData.Agent] = marker;
                Debug.Log($"[MinimapMarkerService] Created marker for item: {itemData.DisplayName} (Type: {marker.Type})");
            }
        }

        /// <summary>
        /// 处理世界物品移除事件
        /// </summary>
        private void OnWorldItemRemoved(WorldItemRemovedEvent evt)
        {
            if (evt?.Data?.Agent == null)
            {
                return;
            }

            RemoveMarker(evt.Data.Agent);
        }

        /// <summary>
        /// 处理箱子发现事件
        /// </summary>
        private void OnLootboxDiscovered(LootboxDiscoveredEvent evt)
        {
            if (!_config.EnableMapMarkers || evt?.Data == null)
            {
                return;
            }

            var lootboxData = evt.Data;

            // 检查箱子是否为空
            if (lootboxData.Items == null || lootboxData.Items.Count == 0)
            {
                return; // 空箱子不创建标记
            }

            // 分析箱子内容
            bool hasUnregisteredKey = false;
            bool hasBuildingMaterial = false;
            bool hasWishlistedItem = false;
            var qualifiedItems = new System.Collections.Generic.List<string>();
            ItemValueLevel highestQuality = ItemValueLevel.White; // 追踪最高品质

            foreach (var item in lootboxData.Items)
            {
                if (item == null) continue;

                // 检查未注册钥匙
                bool isKey = DuckovESPv3.Core.Systems.ESP.Utils.KeyItemHelper.IsUnregisteredKey(item);
                if (isKey)
                {
                    hasUnregisteredKey = true;
                    // 【Bug Fix #1】未录入钥匙无论品质如何都应显示名称
                    string keyName = !string.IsNullOrEmpty(item.DisplayName) ? item.DisplayName : item.name;
                    qualifiedItems.Add(keyName);
                    Debug.Log($"[MinimapMarkerService] 箱子包含未注册钥匙: {keyName}");
                    continue; // 钥匙已处理，跳过后续品质检查
                }

                // 获取物品品质
                var quality = DuckovESPv3.Utils.ItemQualityUtil.GetItemValueLevel(item);

                // 更新最高品质
                if ((int)quality > (int)highestQuality)
                {
                    highestQuality = quality;
                }

                // 检查是否通过品质过滤
                if (PassesQualityFilter(quality))
                {
                    // 获取物品显示名称
                    string itemName = !string.IsNullOrEmpty(item.DisplayName) ? item.DisplayName : item.name;
                    qualifiedItems.Add(itemName);
                    Debug.Log($"[MinimapMarkerService] 箱子物品通过过滤: {itemName} (品质: {quality})");
                }
                else
                {
                    Debug.Log($"[MinimapMarkerService] 箱子物品未通过过滤: {item.DisplayName ?? item.name} (品质: {quality}, 最低要求: {_config.MinQualityForMapMarkers})");
                }
            }

            // 如果箱子里没有符合品质的物品且不是特殊箱子，跳过
            if (qualifiedItems.Count == 0 && !hasUnregisteredKey)
            {
                Debug.Log($"[MinimapMarkerService] 箱子无符合条件的物品，跳过标记创建 (最高品质: {highestQuality})");
                return;
            }

            // 检查是否已存在标记
            if (_markers.ContainsKey(lootboxData.Inventory))
            {
                return;
            }

            // 创建标记
            var marker = _factory.CreateMarkerForLootbox(
                lootboxData.Inventory,
                lootboxData.Position,
                hasUnregisteredKey,
                hasBuildingMaterial,
                hasWishlistedItem,
                qualifiedItems,
                highestQuality
            );

            if (marker != null)
            {
                _markers[lootboxData.Inventory] = marker;
                Debug.Log($"[MinimapMarkerService] ✓ 创建箱子标记 (位置: {lootboxData.Position}, 类型: {marker.Type}, 物品数: {qualifiedItems.Count}, 最高品质: {highestQuality})");
            }
        }

        /// <summary>
        /// 处理箱子移除事件
        /// </summary>
        private void OnLootboxRemoved(LootboxRemovedEvent evt)
        {
            if (evt?.Data?.Inventory == null)
            {
                return;
            }

            RemoveMarker(evt.Data.Inventory);
        }

        /// <summary>
        /// 处理箱子内容变更事件
        /// 当箱子中的物品被拾取或添加时，重新评估是否需要标记
        /// </summary>
        private void OnLootboxContentChanged(LootboxContentChangedEvent evt)
        {
            if (evt?.Data == null)
            {
                return;
            }

            var lootboxData = evt.Data;

            // 如果箱子变空或只剩下不符合品质的物品，移除标记
            if (evt.Reason == ChangeReason.BecameEmpty)
            {
                Debug.Log($"[MinimapMarkerService] 箱子变空，移除标记: {lootboxData.Position}");
                RemoveMarker(lootboxData.Inventory);
                return;
            }

            // 从 Inventory 实时读取最新的物品列表
            var inventory = lootboxData.Inventory;
            if (inventory == null)
            {
                Debug.LogWarning($"[MinimapMarkerService] 箱子 Inventory 为 null，无法更新标记");
                RemoveMarker(lootboxData.Inventory);
                return;
            }

            // 重新评估箱子内容（使用实时数据）
            bool hasUnregisteredKey = false;
            bool hasBuildingMaterial = false;
            bool hasWishlistedItem = false;
            var qualifiedItems = new System.Collections.Generic.List<string>();
            ItemValueLevel highestQuality = ItemValueLevel.White;

            // 直接从 Inventory.Content 读取当前物品
            int totalItemCount = 0;
            if (inventory.Content != null)
            {
                foreach (var item in inventory.Content)
                {
                    if (item == null) continue;
                    totalItemCount++;

                    // 检查未注册钥匙
                    if (DuckovESPv3.Core.Systems.ESP.Utils.KeyItemHelper.IsUnregisteredKey(item))
                    {
                        hasUnregisteredKey = true;
                    }

                    // 获取物品品质
                    var quality = DuckovESPv3.Utils.ItemQualityUtil.GetItemValueLevel(item);

                    // 更新最高品质
                    if ((int)quality > (int)highestQuality)
                    {
                        highestQuality = quality;
                    }

                    // 检查是否通过品质过滤
                    if (PassesQualityFilter(quality))
                    {
                        string itemName = !string.IsNullOrEmpty(item.DisplayName) ? item.DisplayName : item.name;
                        qualifiedItems.Add(itemName);
                    }
                }
            }

            Debug.Log($"[MinimapMarkerService] 箱子内容变更 - 实时物品总数: {totalItemCount}, 符合条件: {qualifiedItems.Count}, 最高品质: {highestQuality}");

            // 如果没有符合条件的物品，移除标记
            if (qualifiedItems.Count == 0 && !hasUnregisteredKey)
            {
                Debug.Log($"[MinimapMarkerService] 箱子内容变更后无符合条件物品，移除标记");
                RemoveMarker(lootboxData.Inventory);
                return;
            }

            // 移除旧标记
            RemoveMarker(lootboxData.Inventory);

            // 创建新标记（使用更新后的内容）
            var marker = _factory.CreateMarkerForLootbox(
                lootboxData.Inventory,
                lootboxData.Position,
                hasUnregisteredKey,
                hasBuildingMaterial,
                hasWishlistedItem,
                qualifiedItems,
                highestQuality
            );

            if (marker != null)
            {
                _markers[lootboxData.Lootbox] = marker;
                Debug.Log($"[MinimapMarkerService] ✓ 箱子内容变更，已更新标记 (实时物品数: {qualifiedItems.Count}, 最高品质: {highestQuality})");
            }
        }

        /// <summary>
        /// 移除标记
        /// </summary>
        private void RemoveMarker(object sourceObject)
        {
            if (sourceObject == null)
            {
                return;
            }

            if (_markers.TryGetValue(sourceObject, out var marker))
            {
                marker.Destroy();
                _markers.Remove(sourceObject);
                Debug.Log($"[MinimapMarkerService] Removed marker for object");
            }
        }

        /// <summary>
        /// 检查品质是否通过过滤
        /// </summary>
        private bool PassesQualityFilter(ItemValueLevel quality)
        {
            return (int)quality >= _config.MinQualityForMapMarkers;
        }

        /// <summary>
        /// 获取所有标记（调试用）
        /// </summary>
        public IReadOnlyDictionary<object, MinimapMarkerData> GetAllMarkers()
        {
            return _markers;
        }

        /// <summary>
        /// 处理Quest区域更新事件
        /// </summary>
        private void OnQuestZonesUpdated(QuestZonesUpdatedEvent evt)
        {
            if (!_config.EnableMapMarkers || !_config.ShowQuestZones)
            {
                // 清除所有Quest区域标记
                foreach (var marker in _questZoneMarkers.Values)
                {
                    marker.Destroy();
                }
                _questZoneMarkers.Clear();
                return;
            }

            // 清除旧标记
            foreach (var marker in _questZoneMarkers.Values)
            {
                marker.Destroy();
            }
            _questZoneMarkers.Clear();

            // 创建新标记
            int createdCount = 0;
            foreach (var zone in evt.QuestZones)
            {
                var markerData = _factory.CreateMarkerForQuestZone(
                    zone.Description,
                    zone.Position,
                    _config.QuestZoneColor
                );

                if (markerData != null)
                {
                    _questZoneMarkers[zone.Key] = markerData;
                    createdCount++;
                }
            }

            Debug.Log($"[MinimapMarkerService] Quest zones updated: {createdCount} markers created");
        }
    }
}
