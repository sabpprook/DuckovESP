using System;
using System.Collections.Generic;

namespace DuckovESPv3.Core.Systems.ESP.Detection
{
    /// <summary>
    /// 数据采集服务接口
    /// 定义所有数据采集系统的通用契约
    /// </summary>
    public interface IDataCollectionService
    {
        /// <summary>
        /// 初始化采集服务
        /// 应在 LevelManager.OnAfterLevelInitialized 时调用
        /// 负责：
        /// - 订阅必要的游戏事件
        /// - 进行初始扫描
        /// - Hook 关键数据结构
        /// </summary>
        void Initialize();

        /// <summary>
        /// 清理采集服务
        /// 应在场景卸载时调用
        /// 负责：
        /// - 取消订阅事件
        /// - 释放资源
        /// - 清空缓存
        /// </summary>
        void Cleanup();
        
        /// <summary>
        /// 重新扫描（用于子场景加载，不清理现有数据）
        /// 应在 MultiSceneCore.OnSubSceneLoaded 时调用
        /// </summary>
        void Rescan();

        /// <summary>
        /// 服务是否已初始化
        /// </summary>
        bool IsInitialized { get; }
    }

    /// <summary>
    /// 箱子数据采集服务接口
    /// 专门用于箱子数据的采集
    /// </summary>
    public interface ILootboxCollectionService : IDataCollectionService
    {
        /// <summary>
        /// 获取所有追踪的箱子
        /// </summary>
        IReadOnlyDictionary<InteractableLootbox, Models.LootboxData> GetTrackedLootboxes();
        
        /// <summary>
        /// 获取所有追踪的箱子数据（用于迭代）
        /// </summary>
        IEnumerable<Models.LootboxData> GetAllTrackedLootboxes();

        /// <summary>
        /// 获取特定箱子的数据
        /// </summary>
        Models.LootboxData GetLootboxData(InteractableLootbox lootbox);

        /// <summary>
        /// 获取当前追踪的箱子总数
        /// </summary>
        int GetTrackedLootboxCount();
    }

    /// <summary>
    /// 地面物品采集服务接口
    /// 专门用于地面物品数据的采集
    /// </summary>
    public interface IWorldItemCollectionService : IDataCollectionService
    {
        /// <summary>
        /// 获取所有追踪的地面物品
        /// </summary>
        IReadOnlyDictionary<ItemStatsSystem.Item, Models.WorldItemData> GetTrackedWorldItems();

        /// <summary>
        /// 获取特定物品的数据
        /// </summary>
        Models.WorldItemData GetWorldItemData(ItemStatsSystem.Item item);

        /// <summary>
        /// 获取当前追踪的物品总数
        /// </summary>
        int GetTrackedItemCount();
    }
}
