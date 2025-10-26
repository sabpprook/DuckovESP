using System;
using HarmonyLib;
using ItemStatsSystem;

namespace DuckovESPv3.Core.Systems.ESP.Detection
{
    /// <summary>
    /// ItemAgent生成Hook：拦截物品动态生成事件
    /// </summary>
    [HarmonyPatch(typeof(ItemAgent), nameof(ItemAgent.Initialize))]
    public static class ItemAgentSpawnHook
    {
        /// <summary>
        /// 物品生成事件（ItemAgent.Initialize调用后触发）
        /// </summary>
        public static event Action<ItemAgent>? OnItemAgentInitialized;

        /// <summary>
        /// Postfix：在ItemAgent.Initialize方法执行后调用
        /// </summary>
        [HarmonyPostfix]
        public static void InitializePostfix(ItemAgent __instance, Item item, ItemAgent.AgentTypes _agentType)
        {
            try
            {
                // 只关注pickUp类型（地面物品）
                if (_agentType == ItemAgent.AgentTypes.pickUp && __instance != null && item != null)
                {
                    // 触发事件
                    OnItemAgentInitialized?.Invoke(__instance);
                }
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError($"[ItemAgentSpawnHook] Hook执行失败: {ex.Message}\n{ex.StackTrace}");
            }
        }
    }
}
