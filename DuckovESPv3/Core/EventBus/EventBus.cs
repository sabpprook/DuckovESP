using System;
using System.Collections.Generic;
using UnityEngine;

namespace DuckovESPv3.Core.EventBus
{
    /// <summary>
    /// 事件总线实现
    /// </summary>
    /// <remarks>
    /// 架构层级：Core/EventBus
    /// 性能特征：
    /// - O(1) 发布性能（直接调用订阅者列表）
    /// - 支持多个订阅者
    /// - 线程不安全，仅在主线程使用
    /// </remarks>
    public class EventBus : IEventBus
    {
        private readonly Dictionary<Type, Delegate> _subscribers = new Dictionary<Type, Delegate>();

        /// <summary>
        /// 订阅事件
        /// </summary>
        public IDisposable Subscribe<T>(Action<T> handler) where T : class
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            Type eventType = typeof(T);
            
            if (_subscribers.TryGetValue(eventType, out Delegate existingDelegate))
            {
                _subscribers[eventType] = Delegate.Combine(existingDelegate, (Delegate)(object)handler);
            }
            else
            {
                _subscribers[eventType] = (Delegate)(object)handler;
            }

            // 返回一个可用于取消订阅的令牌
            return new SubscriptionToken<T>(this, handler);
        }

        /// <summary>
        /// 取消订阅事件
        /// </summary>
        public void Unsubscribe<T>(Action<T> handler) where T : class
        {
            if (handler == null)
                return;

            Type eventType = typeof(T);
            
            if (_subscribers.TryGetValue(eventType, out Delegate existingDelegate))
            {
                Delegate newDelegate = Delegate.Remove(existingDelegate, (Delegate)(object)handler);
                
                if (newDelegate == null)
                {
                    _subscribers.Remove(eventType);
                }
                else
                {
                    _subscribers[eventType] = newDelegate;
                }
            }
        }

        /// <summary>
        /// 发布事件
        /// </summary>
        public void Publish<T>(T eventData) where T : class
        {
            Type eventType = typeof(T);
            
            if (_subscribers.TryGetValue(eventType, out Delegate subscriber))
            {
                try
                {
                    var action = subscriber as Action<T>;
                    action?.Invoke(eventData);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[EventBus] 发布事件 {eventType.Name} 时出错: {ex}");
                }
            }
        }

        /// <summary>
        /// 清空所有订阅
        /// </summary>
        public void Clear()
        {
            _subscribers.Clear();
        }

        /// <summary>
        /// 订阅令牌 - 用于自动取消订阅
        /// </summary>
        private class SubscriptionToken<T> : IDisposable where T : class
        {
            private readonly EventBus _eventBus;
            private readonly Action<T> _handler;
            private bool _disposed;

            public SubscriptionToken(EventBus eventBus, Action<T> handler)
            {
                _eventBus = eventBus;
                _handler = handler;
            }

            public void Dispose()
            {
                if (_disposed)
                    return;

                _eventBus.Unsubscribe(_handler);
                _disposed = true;
            }
        }
    }
}
