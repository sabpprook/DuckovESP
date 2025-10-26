using System;
using System.Linq;
using ItemStatsSystem;

namespace DuckovESPv3.Core.Systems.ESP.Utils
{
    /// <summary>
    /// 钥匙物品辅助工具
    /// 用于判断物品是否为钥匙以及钥匙是否已录入
    /// </summary>
    public static class KeyItemHelper
    {
        /// <summary>
        /// 检查物品是否为未录入的钥匙
        /// </summary>
        /// <param name="item">要检查的物品</param>
        /// <returns>是钥匙且未录入返回true，否则返回false</returns>
        public static bool IsUnregisteredKey(Item item)
        {
            if (item == null)
                return false;

            try
            {
                // 检查物品是否有"Key"或"SpecialKey"标签
                bool isKey = item.Tags.Any(tag => 
                    tag != null && 
                    tag.name != null && 
                    (tag.name.Equals("Key", StringComparison.OrdinalIgnoreCase) ||
                     tag.name.Equals("SpecialKey", StringComparison.OrdinalIgnoreCase)));

                if (!isKey)
                    return false;

                // 检查钥匙是否已经录入到万能钥匙系统
                bool isRegistered = Duckov.MasterKeys.MasterKeysManager.IsActive(item.TypeID);
                
                // 返回：是钥匙 且 未录入
                return !isRegistered;
            }
            catch (Exception)
            {
                // 忽略错误，保守处理
                return false;
            }
        }

        /// <summary>
        /// 检查物品是否为钥匙（无论是否录入）
        /// </summary>
        /// <param name="item">要检查的物品</param>
        /// <returns>是钥匙返回true，否则返回false</returns>
        public static bool IsKey(Item item)
        {
            if (item == null)
                return false;

            try
            {
                return item.Tags.Any(tag => 
                    tag != null && 
                    tag.name != null && 
                    (tag.name.Equals("Key", StringComparison.OrdinalIgnoreCase) ||
                     tag.name.Equals("SpecialKey", StringComparison.OrdinalIgnoreCase)));
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
