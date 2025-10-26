using System;
using HarmonyLib;
using UnityEngine;

namespace DuckovESPv3.Core.Systems.ESP.Detection
{
    /// <summary>
    /// Harmony Hook：监听角色生成事件
    /// Hook CharacterSpawnerRoot.AddCreatedCharacter 方法，实现 0 轮询的敌人检测
    /// </summary>
    [HarmonyPatch(typeof(CharacterSpawnerRoot), nameof(CharacterSpawnerRoot.AddCreatedCharacter))]
    public class CharacterSpawnHook
    {
        /// <summary>
        /// 角色生成事件（当新角色被 Spawner 创建时触发）
        /// </summary>
        public static event Action<CharacterMainControl>? OnCharacterSpawned;
        
        /// <summary>
        /// Postfix Hook：在 AddCreatedCharacter 方法执行后调用
        /// </summary>
        [HarmonyPostfix]
        public static void Postfix(CharacterMainControl c)
        {
            try
            {
                if (c == null) return;
                
                // 触发角色生成事件
                OnCharacterSpawned?.Invoke(c);
                
                Debug.Log($"[CharacterSpawnHook] 检测到新角色生成: {c.name}, Team: {c.Team}, Position: {c.transform.position}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[CharacterSpawnHook] Hook 执行失败: {ex.Message}\n{ex.StackTrace}");
            }
        }
    }
}
