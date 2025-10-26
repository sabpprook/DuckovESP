using System;
using HarmonyLib;
using ItemStatsSystem;
using UnityEngine;

namespace DuckovESPv3.Core.Systems.ESP.Detection
{
    /// <summary>
    /// Lootbox 生成 Hook - 拦截 InteractableLootbox.CreateFromItem 方法
    /// 用于实时检测敌人死亡后生成的战利品箱
    /// </summary>
    [HarmonyPatch(typeof(InteractableLootbox), nameof(InteractableLootbox.CreateFromItem))]
    public static class LootboxSpawnHook
    {
        /// <summary>
        /// 战利品箱生成事件（Postfix - 在箱子创建完成后触发）
        /// </summary>
        public static event Action<InteractableLootbox>? OnLootboxSpawned;

        /// <summary>
        /// Postfix Hook - 在 CreateFromItem 执行完成后调用
        /// </summary>
        [HarmonyPostfix]
        public static void Postfix(
            Item item,
            Vector3 position,
            Quaternion rotation,
            bool moveToMainScene,
            InteractableLootbox prefab,
            bool filterDontDropOnDead,
            InteractableLootbox __result)  // __result 是方法的返回值
        {
            try
            {
                // 检查返回的箱子是否有效
                if (__result == null)
                {
                    Debug.LogWarning("[LootboxSpawnHook] __result 为 null");
                    return;
                }

                // 触发事件（让 LootboxDataCollector 订阅）
                OnLootboxSpawned?.Invoke(__result);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[LootboxSpawnHook] Postfix 处理失败: {ex.Message}\n{ex.StackTrace}");
            }
        }
    }
}
