using DuckovESPv3.Core.Systems.ESP.Minimap.Models;
using DuckovESPv3.Core.Systems.ESP.Models;
using DuckovESPv3.Utils;
using Duckov.MiniMaps;
using Duckov.Scenes;
using ItemStatsSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DuckovESPv3.Core.Systems.ESP.Minimap
{
    /// <summary>
    /// 小地图标记工厂 - 负责创建和配置标记对象
    /// </summary>
    public class MinimapMarkerFactory
    {
        /// <summary>
        /// 为物品创建小地图标记
        /// </summary>
        /// <param name="itemData">物品数据</param>
        /// <returns>标记数据，如果创建失败则返回 null</returns>
        public MinimapMarkerData CreateMarkerForItem(WorldItemData itemData)
        {
            if (itemData?.Item == null || itemData.Agent == null)
            {
                return null;
            }

            // 确定标记类型
            var markerType = DetermineMarkerType(itemData);
            
            // 获取图标
            var icon = GetIcon((int)markerType);
            if (icon == null)
            {
                Debug.LogWarning($"[MinimapMarkerFactory] Failed to get icon for marker type {markerType}");
                return null;
            }

            // 获取颜色
            var color = GetColorForItem(itemData);

            // 创建标记游戏对象
            var markerObject = CreateMarkerObject(itemData.DisplayName, icon, itemData.Position, color);
            if (markerObject == null)
            {
                return null;
            }

            // 创建并返回标记数据
            return new MinimapMarkerData(markerObject, itemData.Agent, markerType, itemData.Position);
        }

        /// <summary>
        /// 为箱子创建小地图标记
        /// </summary>
        /// <param name="lootbox">箱子对象</param>
        /// <param name="position">位置</param>
        /// <param name="hasUnregisteredKey">是否包含未注册钥匙</param>
        /// <param name="hasBuildingMaterial">是否包含建筑材料</param>
        /// <param name="hasWishlistedItem">是否包含愿望单物品</param>
        /// <param name="qualifiedItems">符合品质条件的物品名称列表</param>
        /// <param name="highestQuality">箱子中最高品质物品的等级</param>
        /// <returns>标记数据，如果创建失败则返回 null</returns>
        public MinimapMarkerData CreateMarkerForLootbox(
            InteractableLootbox lootbox, 
            Vector3 position,
            bool hasUnregisteredKey,
            bool hasBuildingMaterial,
            bool hasWishlistedItem,
            System.Collections.Generic.List<string> qualifiedItems,
            ItemValueLevel highestQuality)
        {
            if (lootbox == null)
            {
                return null;
            }

            // 确定标记类型（优先级：未注册钥匙 > 建筑材料 > 愿望单 > 普通）
            MinimapMarkerType markerType = MinimapMarkerType.Normal;
            if (hasUnregisteredKey)
            {
                markerType = MinimapMarkerType.UnregisteredKey;
            }
            else if (hasBuildingMaterial)
            {
                markerType = MinimapMarkerType.BuildingMaterial;
            }
            else if (hasWishlistedItem)
            {
                markerType = MinimapMarkerType.Wishlisted;
            }

            // 获取图标
            var icon = GetIcon((int)markerType);
            if (icon == null)
            {
                Debug.LogWarning($"[MinimapMarkerFactory] Failed to get icon for lootbox marker type {markerType}");
                return null;
            }

            // 根据最高品质决定颜色（而非标记类型）
            var color = ItemQualityUtil.GetColor(highestQuality);

            // 构建显示名称：显示物品列表（最多3个，剩余用+N表示）
            string displayName = BuildLootboxDisplayName(qualifiedItems);

            // 创建标记游戏对象
            var markerObject = CreateMarkerObject(displayName, icon, position, color);
            if (markerObject == null)
            {
                return null;
            }

            // 创建并返回标记数据
            return new MinimapMarkerData(markerObject, lootbox, markerType, position);
        }

        /// <summary>
        /// 为Quest区域创建小地图标记（简化版本）
        /// </summary>
        public MinimapMarkerData CreateMarkerForQuestZone(string description, Vector3 position, Color color)
        {
            // 【修改】使用图标索引 0 作为任务区域标记
            var icon = GetIcon(0);
            if (icon == null)
            {
                Debug.LogWarning("[MinimapMarkerFactory] Failed to get icon for quest zone marker");
                return null;
            }

            // 创建标记游戏对象
            var markerObject = CreateMarkerObject(description, icon, position, color);
            if (markerObject == null)
            {
                return null;
            }

            // 创建并返回标记数据（sourceObject为null，表示没有物理对象）
            return new MinimapMarkerData(markerObject, null, MinimapMarkerType.Normal, position);
        }

        /// <summary>
        /// 构建箱子的显示名称
        /// </summary>
        private string BuildLootboxDisplayName(System.Collections.Generic.List<string> qualifiedItems)
        {
            if (qualifiedItems == null || qualifiedItems.Count == 0)
            {
                return "空箱子";
            }

            const int maxDisplay = 3; // 最多显示3个物品
            if (qualifiedItems.Count <= maxDisplay)
            {
                return string.Join(", ", qualifiedItems);
            }
            else
            {
                var displayItems = qualifiedItems.GetRange(0, maxDisplay);
                int remaining = qualifiedItems.Count - maxDisplay;
                return $"{string.Join(", ", displayItems)} +{remaining}";
            }
        }

        /// <summary>
        /// 确定物品的标记类型
        /// 优先级：未注册钥匙 > 建筑材料 > 愿望单 > 普通
        /// </summary>
        private MinimapMarkerType DetermineMarkerType(WorldItemData itemData)
        {
            // 检查是否是未注册钥匙（最高优先级）
            if (itemData.Item != null && DuckovESPv3.Core.Systems.ESP.Utils.KeyItemHelper.IsUnregisteredKey(itemData.Item))
            {
                return MinimapMarkerType.UnregisteredKey;
            }

            // 检查是否是建筑需求物品
            if (itemData.IsBuildingRequired)
            {
                return MinimapMarkerType.BuildingMaterial;
            }

            // 检查是否在愿望单中
            if (itemData.IsWishlisted)
            {
                return MinimapMarkerType.Wishlisted;
            }

            // 默认为普通物品
            return MinimapMarkerType.Normal;
        }

        /// <summary>
        /// 获取物品的颜色（基于品质）
        /// </summary>
        private Color GetColorForItem(WorldItemData itemData)
        {
            return ItemQualityUtil.GetColor(itemData.Quality);
        }

        /// <summary>
        /// 获取箱子的颜色（基于标记类型）
        /// </summary>
        private Color GetColorForLootbox(MinimapMarkerType markerType)
        {
            return markerType switch
            {
                MinimapMarkerType.UnregisteredKey => Color.magenta,      // 紫色 - 钥匙
                MinimapMarkerType.BuildingMaterial => Color.cyan,        // 青色 - 建筑材料
                MinimapMarkerType.Wishlisted => Color.yellow,            // 黄色 - 愿望单
                _ => Color.white                                         // 白色 - 普通
            };
        }

        /// <summary>
        /// 从 MapMarkerManager 获取图标
        /// </summary>
        /// <param name="index">图标索引</param>
        /// <returns>图标精灵，如果获取失败则尝试备用索引</returns>
        private Sprite GetIcon(int index)
        {
            // 尝试获取指定索引的图标
            var icon = TryGetIconAtIndex(index);
            if (icon != null)
            {
                return icon;
            }

            // 备用方案：尝试常用索引 [5, 7, 1, 0]
            int[] fallbackIndices = { 5, 7, 1, 0 };
            foreach (var fallbackIndex in fallbackIndices)
            {
                if (fallbackIndex == index) continue; // 跳过已经尝试过的索引
                
                icon = TryGetIconAtIndex(fallbackIndex);
                if (icon != null)
                {
                    Debug.LogWarning($"[MinimapMarkerFactory] Icon at index {index} not found, using fallback index {fallbackIndex}");
                    return icon;
                }
            }

            Debug.LogError("[MinimapMarkerFactory] Failed to get any valid icon from MapMarkerManager");
            return null;
        }

        /// <summary>
        /// 尝试从指定索引获取图标
        /// </summary>
        private Sprite TryGetIconAtIndex(int index)
        {
            try
            {
                if (MapMarkerManager.Icons != null && 
                    index >= 0 && 
                    index < MapMarkerManager.Icons.Count)
                {
                    return MapMarkerManager.Icons[index];
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[MinimapMarkerFactory] Error accessing MapMarkerManager.Icons[{index}]: {ex.Message}");
            }

            return null;
        }

        /// <summary>
        /// 创建标记游戏对象
        /// </summary>
        private GameObject CreateMarkerObject(string displayName, Sprite icon, Vector3 position, Color color)
        {
            try
            {
                // 创建游戏对象
                var markerObject = new GameObject($"MinimapMarker_{displayName}");
                markerObject.transform.position = position;

                // 添加 SimplePointOfInterest 组件
                var poi = markerObject.AddComponent<SimplePointOfInterest>();
                
                // 配置 POI
                poi.Setup(icon, displayName, followActiveScene: true, overrideSceneID: null);
                poi.Color = color;
                poi.ShadowColor = new Color(0, 0, 0, 0.5f);
                poi.ShadowDistance = 1f;

                // 将标记移动到主场景（minimap 在主场景中渲染）
                if (MultiSceneCore.MainScene.HasValue)
                {
                    SceneManager.MoveGameObjectToScene(markerObject, MultiSceneCore.MainScene.Value);
                }

                return markerObject;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[MinimapMarkerFactory] Error creating marker object: {ex.Message}");
                return null;
            }
        }
    }
}
