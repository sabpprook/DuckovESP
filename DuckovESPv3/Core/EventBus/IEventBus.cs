using System;

namespace DuckovESPv3.Core.EventBus
{
    /// <summary>
    /// 事件总线接口
    /// </summary>
    /// <remarks>
    /// 架构层级：Core/EventBus
    /// 职责：定义事件发布/订阅系统的契约
    /// 设计意图：解耦系统间的通信，支持事件驱动架构
    /// </remarks>
    public interface IEventBus
    {
        /// <summary>
        /// 订阅事件
        /// </summary>
        /// <typeparam name="T">事件类型</typeparam>
        /// <param name="handler">事件处理器</param>
        /// <returns>订阅令牌，用于后续取消订阅</returns>
        IDisposable Subscribe<T>(Action<T> handler) where T : class;

        /// <summary>
        /// 取消订阅事件
        /// </summary>
        /// <typeparam name="T">事件类型</typeparam>
        /// <param name="handler">事件处理器</param>
        void Unsubscribe<T>(Action<T> handler) where T : class;

        /// <summary>
        /// 发布事件
        /// </summary>
        /// <typeparam name="T">事件类型</typeparam>
        /// <param name="eventData">事件数据</param>
        void Publish<T>(T eventData) where T : class;

        /// <summary>
        /// 清空所有订阅
        /// </summary>
        void Clear();
    }
}
