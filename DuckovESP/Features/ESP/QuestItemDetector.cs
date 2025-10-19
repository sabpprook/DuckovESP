using System;
using System.Collections.Generic;
using System.Linq;
using Duckov.Quests;
using Duckov.Quests.Tasks;
using Duckov.Buildings;
using ItemStatsSystem;
using UnityEngine;

namespace DuckovESP
{
    /// <summary>
    /// 任务物品和建筑材料检测器
    /// </summary>
    public class QuestItemDetector
    {
        private HashSet<int> _questRequiredItems = new HashSet<int>();
        private HashSet<int> _buildingRequiredItems = new HashSet<int>();
        private float _lastUpdateTime = 0f;
        private const float UPDATE_INTERVAL = 2f; // 每2秒更新一次
        
        /// <summary>
        /// 更新任务和建筑所需物品列表
        /// </summary>
        public void Update()
        {
            if (Time.time - _lastUpdateTime < UPDATE_INTERVAL)
                return;
            
            _lastUpdateTime = Time.time;
            
            UpdateQuestRequiredItems();
            UpdateBuildingRequiredItems();
        }
        
        /// <summary>
        /// 更新任务所需物品列表（只包含活跃任务且未完成的任务）
        /// </summary>
        private void UpdateQuestRequiredItems()
        {
            _questRequiredItems.Clear();
            
            try
            {
                // 使用 QuestManager.GetAllRequiredItems() 已经只返回活跃且未完成的任务物品
                // 这个方法内部已经检查了 !submitItems.IsFinished()
                IEnumerable<int> requiredItems = QuestManager.GetAllRequiredItems();
                if (requiredItems != null)
                {
                    foreach (int itemTypeID in requiredItems)
                    {
                        _questRequiredItems.Add(itemTypeID);
                    }
                    
                    // 调试日志
                    if (_questRequiredItems.Count > 0)
                    {
                        Debug.Log($"DuckovESP: 检测到 {_questRequiredItems.Count} 个任务所需物品");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"DuckovESP QuestItemDetector: 更新任务物品失败 - {ex.Message}");
            }
        }
        
        /// <summary>
        /// 更新建筑所需材料列表
        /// 只包含玩家当前缺少的建筑材料
        /// </summary>
        private void UpdateBuildingRequiredItems()
        {
            _buildingRequiredItems.Clear();
            
            try
            {
                // 获取所有已解锁但未建造的建筑
                var unlockedBuildings = GetUnlockedUnbuiltBuildings();
                
                // 获取玩家仓库中的物品数量
                Dictionary<int, int> playerItemCounts = GetPlayerItemCounts();
                
                foreach (string buildingID in unlockedBuildings)
                {
                    // 获取建筑信息
                    BuildingInfo buildingInfo = BuildingManager.GetBuildingInfo(buildingID);
                    if (!buildingInfo.Valid)
                        continue;
                    
                    // 检查建筑所需的材料
                    if (buildingInfo.cost.items != null)
                    {
                        foreach (var itemEntry in buildingInfo.cost.items)
                        {
                            if (itemEntry.id <= 0)
                                continue;
                            
                            // 检查玩家是否已经有足够的材料
                            int playerHas = playerItemCounts.ContainsKey(itemEntry.id) ? playerItemCounts[itemEntry.id] : 0;
                            int required = (int)itemEntry.amount;  // 显式转换 long 为 int
                            
                            // 只有当玩家缺少这个材料时才添加到列表
                            if (playerHas < required)
                            {
                                _buildingRequiredItems.Add(itemEntry.id);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"DuckovESP QuestItemDetector: 更新建筑材料失败 - {ex.Message}");
            }
        }
        
        /// <summary>
        /// 获取玩家仓库中所有物品的数量
        /// </summary>
        private Dictionary<int, int> GetPlayerItemCounts()
        {
            Dictionary<int, int> itemCounts = new Dictionary<int, int>();
            
            try
            {
                CharacterMainControl player = CharacterMainControl.Main;
                if (player == null || player.CharacterItem == null)
                    return itemCounts;
                
                Inventory playerInventory = player.CharacterItem.Inventory;
                if (playerInventory == null)
                    return itemCounts;
                
                // 遍历玩家背包中的所有物品
                foreach (Item item in playerInventory)
                {
                    if (item == null)
                        continue;
                    
                    int typeID = item.TypeID;
                    int stackCount = Mathf.Max(1, item.StackCount);
                    
                    if (itemCounts.ContainsKey(typeID))
                    {
                        itemCounts[typeID] += stackCount;
                    }
                    else
                    {
                        itemCounts[typeID] = stackCount;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"DuckovESP QuestItemDetector: 获取玩家物品数量失败 - {ex.Message}");
            }
            
            return itemCounts;
        }
        
        /// <summary>
        /// 获取所有已解锁但未建造的建筑ID
        /// </summary>
        private HashSet<string> GetUnlockedUnbuiltBuildings()
        {
            HashSet<string> result = new HashSet<string>();
            
            try
            {
                // 获取建筑数据集合
                var buildingCollection = Duckov.Utilities.GameplayDataSettings.BuildingDataCollection;
                if (buildingCollection == null || buildingCollection.Infos == null)
                    return result;
                
                // 遍历所有建筑
                foreach (BuildingInfo info in buildingCollection.Infos)
                {
                    if (!info.Valid || string.IsNullOrEmpty(info.id))
                        continue;
                    
                    // 检查是否已建造
                    if (BuildingManager.Any(info.id, false))
                        continue;
                    
                    result.Add(info.id);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"DuckovESP QuestItemDetector: 获取未建造建筑失败 - {ex.Message}");
            }
            
            return result;
        }
        
        /// <summary>
        /// 检查物品是否是任务需要的
        /// </summary>
        public bool IsQuestRequiredItem(Item item)
        {
            if (item == null)
                return false;
            
            return _questRequiredItems.Contains(item.TypeID);
        }
        
        /// <summary>
        /// 检查物品是否是建筑材料
        /// </summary>
        public bool IsBuildingRequiredItem(Item item)
        {
            if (item == null)
                return false;
            
            return _buildingRequiredItems.Contains(item.TypeID);
        }
        
        /// <summary>
        /// 检查物品是否是任务或建筑需要的
        /// </summary>
        public bool IsRequiredItem(Item item)
        {
            return IsQuestRequiredItem(item) || IsBuildingRequiredItem(item);
        }
        
        /// <summary>
        /// 获取物品的需求类型描述
        /// </summary>
        public string GetItemRequirementType(Item item)
        {
            if (item == null)
                return "";
            
            bool isQuest = IsQuestRequiredItem(item);
            bool isBuilding = IsBuildingRequiredItem(item);
            
            if (isQuest && isBuilding)
                return "[任务+建筑]";
            else if (isQuest)
                return "[任务物品]";
            else if (isBuilding)
                return "[建筑材料]";
            
            return "";
        }
        
        /// <summary>
        /// 获取当前任务所需物品数量
        /// </summary>
        public int GetQuestRequiredItemCount()
        {
            return _questRequiredItems.Count;
        }
        
        /// <summary>
        /// 获取当前建筑所需材料数量
        /// </summary>
        public int GetBuildingRequiredItemCount()
        {
            return _buildingRequiredItems.Count;
        }
    }
}
