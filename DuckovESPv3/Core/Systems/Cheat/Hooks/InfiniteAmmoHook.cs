using HarmonyLib;
using DuckovESPv3.Infrastructure.Logging;
using System.Reflection;

namespace DuckovESPv3.Core.Systems.Cheat.Hooks
{
    /// <summary>
    /// 无限子弹 Harmony Patch
    /// 拦截枪械消耗子弹的逻辑 - 直接阻止 UseABullet() 执行
    /// </summary>
    public class InfiniteAmmoHook
    {
        private static CheatSystem? _cheatSystem;
        private static Infrastructure.Logging.ILogger? _logger;
        
        /// <summary>
        /// 应用 Harmony Patches
        /// </summary>
        public static void ApplyPatches(Harmony harmony, CheatSystem cheatSystem, Infrastructure.Logging.ILogger logger)
        {
            _cheatSystem = cheatSystem;
            _logger = logger;
            
            try
            {
                // Patch ItemSetting_Gun.UseABullet() - 这是真正消耗子弹的地方
                var gunSettingType = AccessTools.TypeByName("ItemSetting_Gun");
                if (gunSettingType != null)
                {
                    var useABulletMethod = AccessTools.Method(gunSettingType, "UseABullet");
                    if (useABulletMethod != null)
                    {
                        harmony.Patch(
                            useABulletMethod,
                            prefix: new HarmonyMethod(typeof(InfiniteAmmoHook), nameof(UseABullet_Prefix))
                        );
                        _logger?.Info("[InfiniteAmmoHook] ✓ 已 Patch ItemSetting_Gun.UseABullet()");
                    }
                    else
                    {
                        _logger?.Warning("[InfiniteAmmoHook] 未找到 UseABullet() 方法");
                    }
                }
                else
                {
                    _logger?.Warning("[InfiniteAmmoHook] 未找到 ItemSetting_Gun 类型");
                }
            }
            catch (System.Exception ex)
            {
                _logger?.Error($"[InfiniteAmmoHook] 应用 Harmony Patch 失败: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Prefix Patch for ItemSetting_Gun.UseABullet()
        /// 当无限子弹启用时，阻止子弹消耗
        /// 返回false会跳过原方法执行
        /// </summary>
        private static bool UseABullet_Prefix()
        {
            // 如果无限子弹启用，返回false阻止原方法执行
            if (_cheatSystem?.InfiniteAmmoEnabled == true)
            {
                return false; // 跳过 UseABullet() 的执行，子弹不会被消耗
            }
            
            return true; // 允许正常执行
        }
    }
}
