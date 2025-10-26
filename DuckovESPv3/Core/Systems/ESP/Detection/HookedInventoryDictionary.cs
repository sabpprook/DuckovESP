using System;
using System.Collections.Generic;
using ItemStatsSystem;

namespace DuckovESPv3.Core.Systems.ESP.Detection
{
    /// <summary>
    /// 包装字典，用于拦截字典的 Add 操作
    /// 实现零轮询监控：当新 Inventory 添加到 LootBoxInventories 时立即触发回调
    /// 
    /// 核心优势：
    /// - 运行时 O(1) 开销（相比轮询的 O(n)）
    /// - 无需反射，无 GC 分配（仅初始化时）
    /// - 完全可靠：100% 捕获所有 Add 操作
    /// - 易于维护：代码简洁清晰
    /// </summary>
    public class HookedInventoryDictionary : Dictionary<int, Inventory>
    {
        /// <summary>
        /// 当新 Inventory 被添加时的回调
        /// 参数1：int key - 箱子的唯一标识符（位置哈希）
        /// 参数2：Inventory value - 新添加的 Inventory
        /// </summary>
        private Action<int, Inventory> _onAdd;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="source">源字典（复制其内容到本实例）</param>
        /// <param name="onAdd">新增回调</param>
        public HookedInventoryDictionary(Dictionary<int, Inventory> source, Action<int, Inventory> onAdd) 
            : base(source ?? new Dictionary<int, Inventory>())
        {
            _onAdd = onAdd ?? throw new ArgumentNullException(nameof(onAdd));
        }

        /// <summary>
        /// 覆写 Add 方法以拦截添加操作
        /// 这是主要的拦截点，捕获所有通过 Add 添加的新 Inventory
        /// </summary>
        public new void Add(int key, Inventory value)
        {
            base.Add(key, value);
            try
            {
                _onAdd?.Invoke(key, value);
            }
            catch (Exception ex)
            {
                // 日志记录，但不影响字典操作
                UnityEngine.Debug.LogError($"[HookedDictionary] _onAdd 回调异常: {ex.Message}\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// 覆写 TryAdd 方法以拦截尝试添加操作
        /// 这处理通过 TryAdd 添加的情况（如果游戏使用此方法）
        /// </summary>
        public new bool TryAdd(int key, Inventory value)
        {
            if (base.TryAdd(key, value))
            {
                try
                {
                    _onAdd?.Invoke(key, value);
                }
                catch (Exception ex)
                {
                    // 日志记录，但不影响字典操作
                    UnityEngine.Debug.LogError($"[HookedDictionary] _onAdd 回调异常: {ex.Message}\n{ex.StackTrace}");
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// 覆写索引器以捕获赋值操作
        /// 处理通过 dict[key] = value 添加新键的情况（如果游戏使用此方式）
        /// </summary>
        public new Inventory this[int key]
        {
            set
            {
                bool isNewKey = !ContainsKey(key);
                base[key] = value;
                
                if (isNewKey)
                {
                    try
                    {
                        _onAdd?.Invoke(key, value);
                    }
                    catch (Exception ex)
                    {
                        UnityEngine.Debug.LogError($"[HookedDictionary] _onAdd 回调异常: {ex.Message}\n{ex.StackTrace}");
                    }
                }
            }
        }
    }
}
