using System;
using HarmonyLib;
using UnityEngine;
using DuckovESPv3.Infrastructure.Logging;

namespace DuckovESPv3.Core.Systems.ESP.Hooks
{
    /// <summary>
    /// 箱子可见性变化 Hook
    /// 监听 SetActiveByCondition 和 LootBoxLoader 对箱子激活状态的修改
    /// 实现 0 轮询的动态箱子过滤
    /// </summary>
    public static class LootboxVisibilityHook
    {
        private static Infrastructure.Logging.ILogger? _logger;
        private static Action<InteractableLootbox, bool>? _onLootboxVisibilityChanged;

        /// <summary>
        /// 箱子可见性变化事件
        /// 参数: (InteractableLootbox, isVisible)
        /// </summary>
        public static event Action<InteractableLootbox, bool> OnLootboxVisibilityChanged
        {
            add => _onLootboxVisibilityChanged += value;
            remove => _onLootboxVisibilityChanged -= value;
        }

        /// <summary>
        /// 应用 Harmony Patches
        /// </summary>
        public static void ApplyPatches(Harmony harmony, Infrastructure.Logging.ILogger logger)
        {
            try
            {
                _logger = logger;

                // Hook SetActiveByCondition.Set() 方法
                var setActiveByConditionType = AccessTools.TypeByName("SetActiveByCondition");
                if (setActiveByConditionType != null)
                {
                    var setMethod = AccessTools.Method(setActiveByConditionType, "Set");
                    if (setMethod != null)
                    {
                        harmony.Patch(
                            setMethod,
                            postfix: new HarmonyMethod(typeof(LootboxVisibilityHook), nameof(SetActiveByCondition_Set_Postfix))
                        );
                        _logger.Info("[LootboxVisibilityHook] ✓ 已 Hook SetActiveByCondition.Set()");
                    }
                    else
                    {
                        _logger.Warning("[LootboxVisibilityHook] 未找到 SetActiveByCondition.Set() 方法");
                    }
                }
                else
                {
                    _logger.Warning("[LootboxVisibilityHook] 未找到 SetActiveByCondition 类型");
                }

                // Hook LootBoxLoader.RandomActive() 方法
                var lootBoxLoaderType = AccessTools.TypeByName("Duckov.Utilities.LootBoxLoader");
                if (lootBoxLoaderType != null)
                {
                    var randomActiveMethod = AccessTools.Method(lootBoxLoaderType, "RandomActive");
                    if (randomActiveMethod != null)
                    {
                        harmony.Patch(
                            randomActiveMethod,
                            postfix: new HarmonyMethod(typeof(LootboxVisibilityHook), nameof(LootBoxLoader_RandomActive_Postfix))
                        );
                        _logger.Info("[LootboxVisibilityHook] ✓ 已 Hook LootBoxLoader.RandomActive()");
                    }
                    else
                    {
                        _logger.Warning("[LootboxVisibilityHook] 未找到 LootBoxLoader.RandomActive() 方法");
                    }
                }
                else
                {
                    _logger.Warning("[LootboxVisibilityHook] 未找到 LootBoxLoader 类型");
                }
            }
            catch (Exception ex)
            {
                _logger?.Error($"[LootboxVisibilityHook] 应用 Patch 失败: {ex.Message}\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// SetActiveByCondition.Set() 后置钩子
        /// 监听条件评估后的箱子激活状态变化
        /// </summary>
        private static void SetActiveByCondition_Set_Postfix(MonoBehaviour __instance)
        {
            try
            {
                // 获取 targetObject 字段
                var targetObjectField = AccessTools.Field(__instance.GetType(), "targetObject");
                if (targetObjectField == null)
                    return;

                var targetObject = targetObjectField.GetValue(__instance) as GameObject;
                if (targetObject == null)
                    return;

                // 检查是否是箱子
                var lootbox = targetObject.GetComponent<InteractableLootbox>();
                if (lootbox == null)
                {
                    // 可能是箱子的父对象，尝试向上查找
                    lootbox = targetObject.GetComponentInParent<InteractableLootbox>();
                    if (lootbox == null)
                        return;
                }

                // 获取当前激活状态
                bool isActive = targetObject.activeInHierarchy;

                // 触发事件
                _onLootboxVisibilityChanged?.Invoke(lootbox, isActive);

                _logger?.Debug($"[LootboxVisibilityHook] SetActiveByCondition 设置箱子可见性: {lootbox.name} → {(isActive ? "可见" : "不可见")}");
            }
            catch (Exception ex)
            {
                _logger?.Warning($"[LootboxVisibilityHook] SetActiveByCondition_Set_Postfix 执行失败: {ex.Message}");
            }
        }

        /// <summary>
        /// LootBoxLoader.RandomActive() 后置钩子
        /// 监听概率生成的箱子激活状态
        /// </summary>
        private static void LootBoxLoader_RandomActive_Postfix(MonoBehaviour __instance)
        {
            try
            {
                // LootBoxLoader 组件所在的 GameObject 就是箱子
                var lootbox = __instance.GetComponent<InteractableLootbox>();
                if (lootbox == null)
                    return;

                // 获取当前激活状态
                bool isActive = __instance.gameObject.activeInHierarchy;

                // 触发事件
                _onLootboxVisibilityChanged?.Invoke(lootbox, isActive);

                _logger?.Debug($"[LootboxVisibilityHook] LootBoxLoader 随机激活箱子: {lootbox.name} → {(isActive ? "生成" : "未生成")}");
            }
            catch (Exception ex)
            {
                _logger?.Warning($"[LootboxVisibilityHook] LootBoxLoader_RandomActive_Postfix 执行失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 清理
        /// </summary>
        public static void Cleanup()
        {
            _onLootboxVisibilityChanged = null;
            _logger = null;
        }
    }
}
