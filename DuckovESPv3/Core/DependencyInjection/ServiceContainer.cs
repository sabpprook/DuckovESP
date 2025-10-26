using System;
using System.Collections.Generic;
using UnityEngine;

namespace DuckovESPv3.Core.DependencyInjection
{
    /// <summary>
    /// 服务容器 - 简单的依赖注入实现
    /// </summary>
    /// <remarks>
    /// 架构层级：Core/DependencyInjection
    /// 职责：
    /// - 注册和解析服务
    /// - 支持单例和瞬态两种生命周期
    /// - 支持工厂模式创建服务
    /// 性能特征：O(1) 查询（基于字典）
    /// </remarks>
    public class ServiceContainer
    {
        private readonly Dictionary<Type, ServiceDescriptor> _services = new Dictionary<Type, ServiceDescriptor>();
        private readonly Dictionary<Type, object> _singletons = new Dictionary<Type, object>();

        /// <summary>
        /// 注册单例服务（只创建一次）
        /// </summary>
        /// <typeparam name="TInterface">服务接口类型</typeparam>
        /// <typeparam name="TImplementation">实现类型</typeparam>
        public void RegisterSingleton<TInterface, TImplementation>()
            where TImplementation : class, TInterface, new()
        {
            Type interfaceType = typeof(TInterface);
            _services[interfaceType] = new ServiceDescriptor
            {
                ServiceType = interfaceType,
                ImplementationType = typeof(TImplementation),
                Lifetime = ServiceLifetime.Singleton
            };
        }

        /// <summary>
        /// 注册单例服务实例
        /// </summary>
        /// <typeparam name="TInterface">服务接口类型</typeparam>
        /// <param name="instance">服务实例</param>
        public void RegisterSingleton<TInterface>(TInterface instance) where TInterface : class
        {
            Type interfaceType = typeof(TInterface);
            _services[interfaceType] = new ServiceDescriptor
            {
                ServiceType = interfaceType,
                ImplementationType = typeof(TInterface),
                Lifetime = ServiceLifetime.Singleton
            };
            _singletons[interfaceType] = instance;
        }

        /// <summary>
        /// 注册单例服务工厂
        /// </summary>
        /// <typeparam name="TInterface">服务接口类型</typeparam>
        /// <param name="factory">工厂方法</param>
        public void RegisterSingleton<TInterface>(Func<ServiceContainer, TInterface> factory) where TInterface : class
        {
            Type interfaceType = typeof(TInterface);
            _services[interfaceType] = new ServiceDescriptor
            {
                ServiceType = interfaceType,
                Factory = (Func<ServiceContainer, object>)(object)factory,
                Lifetime = ServiceLifetime.Singleton
            };
        }

        /// <summary>
        /// 注册瞬态服务（每次都创建新实例）
        /// </summary>
        /// <typeparam name="TInterface">服务接口类型</typeparam>
        /// <typeparam name="TImplementation">实现类型</typeparam>
        public void RegisterTransient<TInterface, TImplementation>()
            where TImplementation : class, TInterface, new()
        {
            Type interfaceType = typeof(TInterface);
            _services[interfaceType] = new ServiceDescriptor
            {
                ServiceType = interfaceType,
                ImplementationType = typeof(TImplementation),
                Lifetime = ServiceLifetime.Transient
            };
        }

        /// <summary>
        /// 解析服务
        /// </summary>
        /// <typeparam name="T">服务接口类型</typeparam>
        /// <returns>服务实例</returns>
        public T Resolve<T>() where T : class
        {
            Type serviceType = typeof(T);

            if (!_services.TryGetValue(serviceType, out ServiceDescriptor descriptor))
            {
                throw new InvalidOperationException($"服务 {serviceType.Name} 未注册");
            }

            // 单例：返回缓存实例
            if (descriptor.Lifetime == ServiceLifetime.Singleton)
            {
                if (_singletons.TryGetValue(serviceType, out object singletonInstance))
                {
                    return (T)singletonInstance;
                }

                // 首次创建单例
                object instance = CreateInstance(descriptor);
                _singletons[serviceType] = instance;
                return (T)instance;
            }

            // 瞬态：每次创建新实例
            return (T)CreateInstance(descriptor);
        }

        /// <summary>
        /// 检查服务是否已注册
        /// </summary>
        /// <typeparam name="T">服务接口类型</typeparam>
        /// <returns>是否已注册</returns>
        public bool IsRegistered<T>() where T : class
        {
            return _services.ContainsKey(typeof(T));
        }

        /// <summary>
        /// 清空所有注册的服务
        /// </summary>
        public void Clear()
        {
            _services.Clear();
            _singletons.Clear();
        }

        /// <summary>
        /// 创建服务实例
        /// </summary>
        private object CreateInstance(ServiceDescriptor descriptor)
        {
            // 使用工厂方法
            if (descriptor.Factory != null)
            {
                return descriptor.Factory(this);
            }

            // 使用反射创建实例
            if (descriptor.ImplementationType != null)
            {
                try
                {
                    return Activator.CreateInstance(descriptor.ImplementationType)!;
                }
                catch (Exception ex)
                {
                    Debug.LogError($"创建服务 {descriptor.ImplementationType.Name} 失败: {ex}");
                    throw;
                }
            }

            throw new InvalidOperationException($"无法创建服务 {descriptor.ServiceType?.Name}");
        }

        /// <summary>
        /// 服务描述符
        /// </summary>
        private class ServiceDescriptor
        {
            public Type? ServiceType { get; set; }
            public Type? ImplementationType { get; set; }
            public Func<ServiceContainer, object>? Factory { get; set; }
            public ServiceLifetime Lifetime { get; set; }
        }

        /// <summary>
        /// 服务生命周期
        /// </summary>
        private enum ServiceLifetime
        {
            Singleton,  // 单例
            Transient   // 瞬态
        }
    }
}
